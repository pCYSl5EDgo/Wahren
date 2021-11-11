global using Wahren.AbstractSyntaxTree;
global using Wahren.AbstractSyntaxTree.Node;
global using Wahren.AbstractSyntaxTree.Parser;
global using Wahren.AbstractSyntaxTree.Element;
global using Wahren.AbstractSyntaxTree.Statement;
global using Wahren.AbstractSyntaxTree.Statement.Expression;
global using Wahren.ArrayPoolCollections;
global using System;

namespace Wahren.AbstractSyntaxTree.Modifier;

public static partial class PerFileModifier
{
    public static void RemoveRange(ref Result result, uint tokenIndex, uint removeCount)
    {
        if (removeCount == 0)
        {
            return;
        }

        result.DecrementToken(tokenIndex, removeCount);
        ref var tokenList = ref result.TokenList;
        var firstLine = tokenList.GetLine(tokenIndex);
        var endIndex = tokenIndex + removeCount;
        var endLine = tokenList.GetLine(endIndex);
        var firstOffset = tokenList.GetOffset(tokenIndex);
        var firstPrecedingWhitespaceCount = tokenList.GetPrecedingWhitespaceCount(tokenIndex);
        var endOffset = tokenList.GetOffset(endIndex);
        var endLength = tokenList.GetLength(endIndex);
        result.Source.RemoveRange(firstLine, firstOffset - firstPrecedingWhitespaceCount, endLine, endOffset + endLength);
        result.TokenList.RemoveRange((int)tokenIndex, (int)removeCount);
    }

    public static void InsertLine(ref Result result, uint lineIndex)
    {
        result.Source.InsertEmpty(lineIndex);
        ref var tokenList = ref result.TokenList;
        var lineSpan = tokenList.Line;
        int i = 0;
        for (; i < lineSpan.Length; ++i)
        {
            if (lineSpan[i] >= lineIndex)
            {
                break;
            }
        }

        for (; i < lineSpan.Length; ++i)
        {
            lineSpan[i]++;
        }
    }

    public static void IncreaseOffset(ref TokenList tokenList, uint lineIndex, uint startTokenIndex, uint diff)
    {
        for (; startTokenIndex < tokenList.Count; ++startTokenIndex)
        {
            if (tokenList.GetKind(startTokenIndex) == TokenKind.Deleted)
            {
                continue;
            }

            if (tokenList.GetLine(startTokenIndex) != lineIndex)
            {
                break;
            }

            tokenList.GetOffset(startTokenIndex) += diff;
        }
    }

    public static void DecreaseOffset(ref TokenList tokenList, uint lineIndex, uint startTokenIndex, uint diff)
    {
        for (; startTokenIndex < tokenList.Count; ++startTokenIndex)
        {
            if (tokenList.GetKind(startTokenIndex) == TokenKind.Deleted)
            {
                continue;
            }

            if (tokenList.GetLine(startTokenIndex) != lineIndex)
            {
                break;
            }

            tokenList.GetOffset(startTokenIndex) -= diff;
        }
    }

    private static void AdjustCopy(ref Result result, uint tokenIndex, ReadOnlySpan<char> span)
    {
        var offset = result.TokenList.GetOffset(tokenIndex);
        ref var length = ref result.TokenList.GetLength(tokenIndex);
        var lineIndex = result.TokenList.GetLine(tokenIndex);
        ref var line = ref result.Source[lineIndex];
        if (length > span.Length)
        {
            var diff = length - (uint)span.Length;
            line.RemoveRange(offset + (uint)span.Length, diff);
            length = (uint)span.Length;
            DecreaseOffset(ref result.TokenList, lineIndex, tokenIndex + 1U, diff);
        }
        else if (length < span.Length)
        {
            var diff = (uint)span.Length - length;
            line.InsertUndefinedRange(offset + length, diff);
            length = (uint)span.Length;
            IncreaseOffset(ref result.TokenList, lineIndex, tokenIndex + 1U, diff);
        }

        span.CopyTo(line.AsSpan(offset, length));
    }

    public static bool TryRewriteNumber(ref Result result, uint tokenIndex, int value)
    {
        Span<char> numberSpan = stackalloc char[16];
        if (!value.TryFormat(numberSpan, out var charsWritten))
        {
            return false;
        }

        AdjustCopy(ref result, tokenIndex, numberSpan.Slice(0, charsWritten));
        return true;
    }

    public static bool TryRewriteIdentifier(ref Result result, uint tokenIndex, ReadOnlySpan<char> singleLineSpan)
    {
        foreach (var item in singleLineSpan)
        {
            if (char.IsWhiteSpace(item) || char.IsControl(item))
            {
                return false;
            }
        }

        AdjustCopy(ref result, tokenIndex, singleLineSpan);
        return true;
    }

    public static void CreateEmptyElement<TNode, TElement>(ref Result result, ref TNode node, out TElement element, ReadOnlySpan<char> elementSpan, ReadOnlySpan<char> variationSpan, TokenKind elementTokenKind)
        where TNode : struct, INode
        where TElement : class, IElement
    {
        var insertRangeStartIndex = node.BracketRight - 1U;
        result.TokenList.InsertUndefinedRange((int)insertRangeStartIndex, 3, out var undefinedRange);
        result.IncrementToken(insertRangeStartIndex, 3U);
        var bracketRightLine = result.TokenList.GetLine(node.BracketRight);
        ref var newLineCount = ref result.TokenList.GetPrecedingNewLineCount(node.BracketRight);
        var oldNewLineCount = newLineCount;
        newLineCount = 1U;
        InsertLine(ref result, bracketRightLine);
        ref var line = ref result.Source[bracketRightLine];
        var additionalVariationSize = variationSpan.IsEmpty ? 0U : 1U + (uint)variationSpan.Length;
        var preparedSpan = line.InsertUndefinedRange(0, 8U + (uint)elementSpan.Length + additionalVariationSize);
        "    ".CopyTo(preparedSpan);
        elementSpan.CopyTo(preparedSpan.Slice(4));
        preparedSpan = preparedSpan.Slice(elementSpan.Length + 4);
        if (!variationSpan.IsEmpty)
        {
            preparedSpan[0] = '@';
            variationSpan.CopyTo(preparedSpan.Slice(1));
            preparedSpan = preparedSpan.Slice(variationSpan.Length + 1);
        }

        " = @".CopyTo(preparedSpan);
        undefinedRange.kindSpan[0] = elementTokenKind;
        undefinedRange.kindSpan[1] = TokenKind.Assign;
        undefinedRange.kindSpan[2] = TokenKind.Content;
        undefinedRange.lengthSpan[0] = (uint)elementSpan.Length + additionalVariationSize;
        undefinedRange.lengthSpan[1] = 1U;
        undefinedRange.lengthSpan[2] = 1U;
        undefinedRange.lineSpan.Fill(bracketRightLine);
        undefinedRange.offsetSpan[0] = 4U;
        undefinedRange.offsetSpan[1] = 5U + (uint)elementSpan.Length + additionalVariationSize;
        undefinedRange.offsetSpan[2] = 7U + (uint)elementSpan.Length + additionalVariationSize;
        undefinedRange.otherSpan.Clear();
        undefinedRange.precedingNewLineCountSpan[0] = oldNewLineCount;
        undefinedRange.precedingNewLineCountSpan[1] = 0U;
        undefinedRange.precedingNewLineCountSpan[2] = 0U;
        undefinedRange.precedingWhitespaceCountSpan[0] = 4U;
        undefinedRange.precedingWhitespaceCountSpan[1] = 1U;
        undefinedRange.precedingWhitespaceCountSpan[2] = 1U;
        element = (TElement)TElement.Create(insertRangeStartIndex, elementSpan.Length, !variationSpan.IsEmpty);
    }

    public static void ForceElementEmpty(ref Result result, Pair_NullableString_NullableIntElement element)
    {
        if (!element.HasValue)
        {
            return;
        }

        ref var value = ref element.Value;
        var valueText = value.Text;
        uint diff;
        if (value.HasNumberTokenId)
        {
            diff = value.NumberTokenId - valueText;
        }
        else
        {
            diff = value.TrailingTokenCount;
        }

        RemoveRange(ref result, valueText + 1U, diff);
        AdjustCopy(ref result, valueText, "@");
        value = default;
        value.Text = valueText;
        element.HasValue = false;
    }
}
