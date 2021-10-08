global using System;
global using System.Buffers;
global using System.Diagnostics.CodeAnalysis;
global using System.Runtime.CompilerServices;
global using Wahren.AbstractSyntaxTree.Element;
global using Wahren.AbstractSyntaxTree.Node;
global using Wahren.PooledList;

[module: SkipLocalsInit]

namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ReadToken(ref Context context, ref Result result)
    {
        result.TokenList.Add(new());
        if (!Lexer.ReadToken(ref result.Source, ref context.Position, ref result.TokenList.Last))
        {
            result.TokenList.RemoveLast();
            return false;
        }

        return true;
    }

    /// <summary>
    /// </summary>
    /// <return>True : Not comment token is read.</return>
    private static bool ReadUsefulToken(ref Context context, ref Result result)
    {
        ref var source = ref result.Source;
        ref var position = ref context.Position;
        ref var tokenList = ref result.TokenList;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                return false;
            }

            ref var last = ref tokenList.Last;
            if (last.IsSingleLineComment(ref source))
            {
                last.Length = (uint)source[last.Position.Line].Count - last.Position.Offset;
                position.Offset = (uint)source[last.Position.Line].Count;
            }
            else if (last.IsSingleLineCommentSlashPlus(ref source))
            {
                if (context.TreatSlashPlusAsSingleLineComment)
                {
                    last.Length = (uint)source[last.Position.Line].Count - last.Position.Offset;
                    position.Offset = (uint)source[last.Position.Line].Count;
                }
            }
            else if (last.IsMultiLineCommentStart(ref source))
            {
                var mulslashIndex = source[last.Position.Line].AsSpan(last.Position.Offset + 2).IndexOf("*/");
                if (mulslashIndex == -1)
                {
                    last.Length = (uint)source[last.Position.Line].Count - last.Position.Offset;
                    position.Offset = (uint)source[last.Position.Line].Count;
                    position.Line++;
                    position.Offset = 0;
                    if (!ParseMultiLineComment(ref position, ref result))
                    {
                        return false;
                    }
                }
                else
                {
                    last.Length += (uint)mulslashIndex + 2U;
                    position.Offset = (uint)mulslashIndex + 2U;
                }
            }
            else
            {
                return true;
            }
        } while (true);
    }

    private static bool ParseMultiLineComment(ref Position position, ref Result result)
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        while (position.Line < source.Count)
        {
            tokenList.Add(new());
            ref var last = ref tokenList.Last;
            last.Kind = TokenKind.Comment;
            last.PrecedingNewLineCount = 1;
            last.Position = position;
            ref var line = ref source[position.Line];
            var span = line.AsSpan(position.Offset);
            var index = span.IndexOf("*/");
            if (index == -1)
            {
                last.Length = (uint)span.Length;
                position.Offset = 0;
                position.Line++;
                continue;
            }

            position.Offset += (uint)index + 2U;
            last.Length = (uint)index + 2U;
            return true;
        }

        result.ErrorAdd_MultiLineCommentEndIsExpected();
        return false;
    }

    private static bool SplitElement(ref this Result result, uint tokenId, out Span<char> span, out uint scenarioId)
    {
        span = result.GetSpan(tokenId);
        var index = span.LastIndexOf('@');
        switch (index)
        {
            case -1:
                scenarioId = uint.MaxValue;
                break;
            default:
                scenarioId = result.ScenarioSet.GetOrAdd(span.Slice(index + 1), tokenId);
                span = span.Slice(0, index);
                break;
        }

        if (span.IsEmpty)
        {
            result.ErrorAdd_EmptyElementKey(tokenId);
            return false;
        }

        return true;
    }

    private static bool SplitElementPlain(ref this Result result, uint tokenId, out Span<char> span, out Span<char> afterAtmark)
    {
        span = result.GetSpan(tokenId);
        var index = span.LastIndexOf('@');
        switch (index)
        {
            case -1:
                afterAtmark = Span<char>.Empty;
                break;
            default:
                afterAtmark = span.Slice(index + 1);
                span = span.Slice(0, index);
                break;
        }

        if (span.IsEmpty)
        {
            result.ErrorAdd_EmptyElementKey(tokenId);
            return false;
        }

        return true;
    }

    private static bool SplitElement(ref this Result result, IElement element)
    {
        var span = result.GetSpan(element.ElementTokenId);
        var index = span.LastIndexOf('@');
        ref var token = ref result.TokenList[element.ElementTokenId];
        ref var elementKey = ref element.ElementKeyRange;
        elementKey.Line = token.Position.Line;
        elementKey.Offset = token.Position.Offset;
        switch (index)
        {
            case -1:
                elementKey.Length = token.Length;
                element.ElementScenarioId = uint.MaxValue;
                break;
            default:
                elementKey.Length = (uint)index;
                element.ElementScenarioId = result.ScenarioSet.GetOrAdd(span.Slice(index + 1), element.ElementTokenId);
                break;
        }

        if (elementKey.Length == 0)
        {
            result.ErrorAdd_EmptyElementKey(element.ElementTokenId);
            return false;
        }

        return true;
    }

    private static bool IsValidIdentifier(ref Context context, ref Result result, uint tokenIndex)
    {
        ref var token = ref result.TokenList[tokenIndex];
        var span = result.GetSpan(tokenIndex);
        var invalidIndex = span.IndexOfAny(" \t@:;^|+-*/.,");
        if (invalidIndex != -1)
        {
            result.ErrorAdd($"Identifier must not contain {span[invalidIndex]}.", tokenIndex);
            return false;
        }

        if (context.CreateError(DiagnosticSeverity.Warning))
        {
            ushort c = span[0];
            if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '_')
            {
                result.WarningAdd_InvalidIdentifier(tokenIndex);
                return true;
            }

            for (int i = 1; i < span.Length; ++i)
            {
                c = span[i];
                if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '_' && (c < '0' || c > '9'))
                {
                    result.WarningAdd_InvalidIdentifier(tokenIndex);
                    break;
                }
            }
        }

        return true;
    }

    private static bool ParseNameAndSuperAndBracketLeft<T>(ref Context context, ref Result result, ref T node, ref StringSpanKeySlowSet superSet)
        where T : struct, IInheritableNode
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        ref var errorList = ref result.ErrorList;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd($"{result.GetSpan(node.Kind)} should have name.", node.Kind);
            return false;
        }

        node.Name = tokenList.LastIndex;
        tokenList.Last.Kind = TokenKind.Name;
        if (!IsValidIdentifier(ref context, ref result, tokenList.LastIndex))
        {
            return false;
        }

        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_BracketLeftNotFound(result.GetSpan(node.Kind));
            return false;
        }

        if (tokenList.Last.IsColon(ref source) && !ProcessSuper(ref context, ref result, ref node, ref superSet))
        {
            return false;
        }

        if (!tokenList.Last.IsBracketLeft(ref source))
        {
            result.ErrorAdd_BracketLeftNotFound(result.GetSpan(node.Kind));
            return false;
        }

        node.BracketLeft = tokenList.LastIndex;
        return true;
    }

    private static bool ProcessSuper<T>(ref Context context, ref Result result, ref T node, ref StringSpanKeySlowSet superSet)
        where T : struct, IInheritableNode
    {
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_SucceedingSuperIsExpected(node.Name);
            return true;
        }

        ref var tokenList = ref result.TokenList;
        tokenList.Last.Kind = TokenKind.Super;
        var superSpan = result.GetSpan(tokenList.LastIndex);
        var nodeNameSpan = result.GetSpan(node.Name);
        if (superSpan.SequenceEqual(nodeNameSpan))
        {
            result.ErrorAdd_InfiniteLoop(result.GetSpan(node.Kind), nodeNameSpan, superSpan, node.Name);
        }
        var superIndex = superSet.GetOrAdd(superSpan, tokenList.LastIndex);
        node.Super = superIndex;
        if (context.CreateError(DiagnosticSeverity.Warning) && !IsValidIdentifier(ref context, ref result, tokenList.LastIndex))
        {
            result.WarningAdd_InvalidIdentifier(tokenList.LastIndex);
        }

        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(node.Kind);
            return false;
        }

        return true;
    }

    private static bool ParseNamelessToBracketLeft(ref Context context, ref Result result, uint kindIndex)
    {
        ref var source = ref result.Source;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(kindIndex);
            return false;
        }

        ref var last = ref result.TokenList.Last;
        if (last.IsBracketLeft(ref source))
        {
            return true;
        }

        result.ErrorAdd_BracketLeftNotFound(result.GetSpan(kindIndex));
        return false;
    }

    private static void CancelTokenReadback(ref Context context, ref Result result)
    {
        result.TokenList.RemoveLast();
        ref var last = ref result.TokenList.Last;
        context.Position = last.Position;
        context.Position.Offset += last.Length;
    }

    private static bool ReadAssign(ref Context context, ref Result result, uint elementTokenId)
    {
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_AssignmentDoesNotExist(elementTokenId);
            return false;
        }

        ref var last = ref result.TokenList.Last;
        if (!last.IsAssign(ref result.Source))
        {
            result.ErrorAdd_AssignmentDoesNotExist(elementTokenId);
            return false;
        }

        return true;
    }
}
