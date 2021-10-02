global using System;
global using System.Buffers;
global using System.Diagnostics.CodeAnalysis;
global using System.Runtime.CompilerServices;
global using Wahren.PooledList;
global using Wahren.AbstractSyntaxTree.Element;
global using Wahren.AbstractSyntaxTree.Node;

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
            ref var lastRange = ref last.Range;
            if (last.IsSingleLineComment(ref source))
            {
                if (last.Range.StartAndEndLineAreSame)
                {
                    last.Length = (uint)source[lastRange.StartInclusive.Line].Count - lastRange.StartInclusive.Offset;
                    position.Offset = (uint)source[lastRange.StartInclusive.Line].Count;
                    lastRange.EndExclusive = position;
                }
            }
            else if (last.IsSingleLineCommentSlashPlus(ref source))
            {
                if (context.TreatSlashPlusAsSingleLineComment && lastRange.StartAndEndLineAreSame)
                {
                    last.Length = (uint)source[lastRange.StartInclusive.Line].Count - lastRange.StartInclusive.Offset;
                    position.Offset = (uint)source[lastRange.StartInclusive.Line].Count;
                    lastRange.EndExclusive = position;
                }
            }
            else if (last.IsMultiLineCommentStart(ref source))
            {
                var mulslashIndex = source[last.Range.EndExclusive.Line].AsSpan(last.Range.EndExclusive.Offset).IndexOf("*/");
                if (mulslashIndex == -1)
                {
                    last.Length = (uint)source[lastRange.StartInclusive.Line].Count - lastRange.StartInclusive.Offset;
                    position.Offset = (uint)source[lastRange.StartInclusive.Line].Count;
                    lastRange.EndExclusive = position;
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
                    lastRange.EndExclusive = position;
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
            last.Range.StartInclusive = position;
            last.Range.EndExclusive = position;
            ref var line = ref source[position.Line];
            var span = line.AsSpan(position.Offset);
            var index = span.IndexOf("*/");
            if (index == -1)
            {
                last.Length = (uint)span.Length;
                last.Range.EndExclusive.Offset += (uint)span.Length;
                position.Offset = 0;
                position.Line++;
                continue;
            }

            position.Offset += (uint)index + 2U;
            last.Length = (uint)index + 2U;
            last.Range.EndExclusive = position;
            return true;
        }

        result.ErrorAdd("Multiline comment \"/*\" needs corresponding \"*/\". But it is not found in this file.", tokenList.LastIndex);
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
            result.ErrorAdd("Element key consists of plain key and scenario name. The length of plain key must not be zero.", tokenId);
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
            result.ErrorAdd("Element key consists of plain key and scenario name. The length of plain key must not be zero.", tokenId);
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
        ref var range = ref token.Range;
        elementKey.Line = range.StartInclusive.Line;
        elementKey.Offset = range.StartInclusive.Offset;
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
            result.ErrorAdd("Element key consists of plain key and scenario name. The length of plain key must not be zero.", element.ElementTokenId);
            return false;
        }

        return true;
    }

    private static bool IsValidIdentifier(ref Context context, ref Result result, uint tokenIndex)
    {
        ref var token = ref result.TokenList[tokenIndex];
        var length = token.Length;
        ref var range = ref token.Range;
        if (!range.OneLine)
        {
            result.ErrorAdd("Identifier must be one liner.", tokenIndex);
            return false;
        }

        ref var line = ref result.Source[range.StartInclusive.Line];
        var span = line.AsSpan(range.StartInclusive.Offset, length);
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
                result.WarningAdd("Identifier should start with alphabet or underscore.", tokenIndex);
            }

            for (int i = 1; i < span.Length; ++i)
            {
                c = span[i];
                if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '_' && (c < '0' || c > '9'))
                {
                    result.WarningAdd("Identifier should consist of alphabet, underscore or digit.", tokenIndex);
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
            result.ErrorAdd($"{result.GetSpan(node.Kind)} {result.GetSpan(node.Name)} should start with '{{' but not found.", node.Kind);
            return false;
        }

        if (tokenList.Last.IsColon(ref source))
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                result.ErrorAdd($"After ':', name of the super is needed.", node.Name);
                return false;
            }

            tokenList.Last.Kind = TokenKind.Super;
            var superIndex = superSet.GetOrAdd(result.GetSpan(tokenList.LastIndex), tokenList.LastIndex);
            node.Super = superIndex;
            if (context.CreateError(DiagnosticSeverity.Warning) && !IsValidIdentifier(ref context, ref result, tokenList.LastIndex))
            {
                result.ErrorAdd($"Not appropriate super name '{superSet[superIndex]}' of struct {result.GetSpan(node.Kind)} '{result.GetSpan(node.Name)}'.", tokenList.LastIndex);
            }

            if (!ReadUsefulToken(ref context, ref result))
            {
                result.ErrorAdd($"{result.GetSpan(node.Kind)} {result.GetSpan(node.Name)} : {superSet[superIndex]} should start with '{{' but not found.", node.Kind);
                return false;
            }
        }

        if (!tokenList.Last.IsBracketLeft(ref source))
        {
            result.ErrorAdd($"{result.GetSpan(node.Kind)} {result.GetSpan(node.Name)} should start with '{{'.", tokenList.LastIndex);
            return false;
        }

        node.BracketLeft = tokenList.LastIndex;
        return true;
    }

    private static bool ParseNamelessUniqueToBracketLeft(ref Context context, ref Result result, uint kindIndex)
    {
        ref var source = ref result.Source;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd($"{result.GetSpan(kindIndex)} needs '{{' but no token exists.", kindIndex);
            return false;
        }

        ref var last = ref result.TokenList.Last;
        if (last.IsBracketLeft(ref source))
        {
            return true;
        }
        
        var text = $"{result.GetSpan(kindIndex)} needs '{{' but {result.GetSpan(result.TokenList.LastIndex)} (Line : {last.Range.StartInclusive.Line + 1}, Offset : {last.Range.StartInclusive.Offset + 1}) appears.";
        result.ErrorAdd(text, kindIndex);
        return false;
    }

    private static bool ParseNamelessUbiquitousToBracketLeft(ref Context context, ref Result result, uint kindIndex)
    {
        ref var source = ref result.Source;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd($"{result.GetSpan(kindIndex)} needs '{{' but no token exists.", kindIndex);
            return false;
        }

        ref var last = ref result.TokenList.Last;
        if (last.IsBracketLeft(ref source))
        {
            return true;
        }

        var text = $"{result.GetSpan(kindIndex)} needs '{{' but {result.GetSpan(result.TokenList.LastIndex)} (Line : {last.Range.StartInclusive.Line + 1}, Offset : {last.Range.StartInclusive.Offset + 1}) appears.";
        result.ErrorAdd(text, kindIndex);
        return false;
    }

    private static void CancelTokenReadback(ref Context context, ref Result result)
    {
        result.TokenList.RemoveLast();
        context.Position = result.TokenList.Last.Range.EndExclusive;
    }

    private static bool ReadAssign(ref Context context, ref Result result, uint elementTokenId)
    {
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd("Element must have '=' but not found", elementTokenId);
            return false;
        }

        ref var last = ref result.TokenList.Last;
        if (!last.IsAssign(ref result.Source))
        {
            result.ErrorAdd("Element must have '=' but first found token is not '='.", result.TokenList.LastIndex);
            return false;
        }

        return true;
    }
}
