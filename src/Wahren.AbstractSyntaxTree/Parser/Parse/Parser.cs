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
        Token token = new();
        if (!Lexer.ReadToken(ref result.Source, ref context.Position, ref token))
        {
            return false;
        }

        result.TokenList.Add(ref token);
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

            ref var lastLength = ref tokenList.GetLength(tokenList.LastIndex);
            ref var lastOffset = ref tokenList.GetOffset(tokenList.LastIndex);
            ref var lastLine = ref tokenList.GetLine(tokenList.LastIndex);
            if (result.IsSingleLineComment(tokenList.LastIndex))
            {
                lastLength = (uint)source[lastLine].Count - lastOffset;
                position.Offset = (uint)source[lastLine].Count;
            }
            else if (result.IsSingleLineCommentSlashPlus(tokenList.LastIndex))
            {
                if (context.TreatSlashPlusAsSingleLineComment)
                {
                    lastLength = (uint)source[lastLine].Count - lastOffset;
                    position.Offset = (uint)source[lastLine].Count;
                }
            }
            else if (result.IsMultiLineCommentStart(tokenList.LastIndex))
            {
                var mulslashIndex = source[lastLine].AsSpan(lastOffset + 2).IndexOf("*/");
                if (mulslashIndex == -1)
                {
                    lastLength = (uint)source[lastLine].Count - lastOffset;
                    position.Offset = (uint)source[lastLine].Count;
                    position.Line++;
                    position.Offset = 0;
                    if (!ParseMultiLineComment(ref position, ref result))
                    {
                        return false;
                    }
                }
                else
                {
                    lastLength += (uint)mulslashIndex + 2U;
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
            var last = new Token
            {
                Kind = TokenKind.Comment,
                PrecedingNewLineCount = 1,
                Position = position
            };
            ref var line = ref source[position.Line];
            var span = line.AsSpan(position.Offset);
            var index = span.IndexOf("*/");
            if (index == -1)
            {
                last.Length = (uint)span.Length;
                position.Offset = 0;
                position.Line++;
                tokenList.Add(ref last);
                continue;
            }

            position.Offset += (uint)index + 2U;
            last.Length = (uint)index + 2U;
            tokenList.Add(ref last);
            return true;
        }

        result.ErrorAdd_MultiLineCommentEndIsExpected();
        return false;
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

    private static bool IsValidIdentifier(ref Context context, ref Result result, uint tokenIndex)
    {
        ref var tokenList = ref result.TokenList;
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

    private static bool ParseNameAndSuperAndBracketLeft<T>(ref Context context, ref Result result, ref T node)
        where T : struct, IInheritableNode
    {
        ref var tokenList = ref result.TokenList;
        ref var errorList = ref result.ErrorList;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd($"{result.GetSpan(node.Kind)} should have name.", node.Kind);
            return false;
        }

        node.Name = tokenList.LastIndex;
        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Name;
        if (!IsValidIdentifier(ref context, ref result, tokenList.LastIndex))
        {
            return false;
        }

        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_BracketLeftNotFound(result.GetSpan(node.Kind));
            return false;
        }

        if (result.IsColon(tokenList.LastIndex) && !ProcessSuper(ref context, ref result, ref node))
        {
            return false;
        }

        if (!result.IsBracketLeft(tokenList.LastIndex))
        {
            result.ErrorAdd_BracketLeftNotFound(result.GetSpan(node.Kind));
            return false;
        }

        node.BracketLeft = tokenList.LastIndex;
        return true;
    }

    private static bool ProcessSuper<T>(ref Context context, ref Result result, ref T node)
        where T : struct, IInheritableNode
    {
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_SucceedingSuperIsExpected(node.Name);
            return true;
        }

        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Super;
        node.HasSuper = true;
        node.Super = tokenList.LastIndex;
        var superSpan = result.GetSpan(tokenList.LastIndex);

        var nodeNameSpan = result.GetSpan(node.Name);
        if (superSpan.SequenceEqual(nodeNameSpan))
        {
            result.ErrorAdd_InfiniteLoop(result.GetSpan(node.Kind), nodeNameSpan, superSpan, node.Name);
        }

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
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(kindIndex);
            return false;
        }

        if (result.IsBracketLeft(result.TokenList.LastIndex))
        {
            return true;
        }

        result.ErrorAdd_BracketLeftNotFound(result.GetSpan(kindIndex));
        return false;
    }

    private static void CancelTokenReadback(ref Context context, ref Result result)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.RemoveLast();
        context.Position.Line = tokenList.GetLine(tokenList.LastIndex);
        context.Position.Offset = tokenList.GetOffset(tokenList.LastIndex) + tokenList.GetLength(tokenList.LastIndex);
    }

    private static bool ReadAssign(ref Context context, ref Result result, uint elementTokenId)
    {
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_AssignmentDoesNotExist(elementTokenId);
            return false;
        }

        if (!result.IsAssign(result.TokenList.LastIndex))
        {
            result.ErrorAdd_AssignmentDoesNotExist(elementTokenId);
            return false;
        }

        return true;
    }
}
