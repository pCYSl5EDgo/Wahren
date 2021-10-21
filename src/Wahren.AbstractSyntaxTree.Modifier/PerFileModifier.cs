global using Wahren.AbstractSyntaxTree.Parser;
global using Wahren.AbstractSyntaxTree.Element;
global using Wahren.AbstractSyntaxTree.Statement;
global using Wahren.AbstractSyntaxTree.Statement.Expression;
global using Wahren.PooledList;
global using System;

namespace Wahren.AbstractSyntaxTree.Modifier;

public static class PerFileModifier
{
    public static void InsertLine(ref this Result result, int lineIndex)
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

        if (lineSpan[i]++ == lineIndex && tokenList.GetOffset((uint)i) == tokenList.GetPrecedingWhitespaceCount((uint)i))
        {
            tokenList.GetPrecedingNewLineCount((uint)i)++;
        }

        for (++i; i < lineSpan.Length; ++i)
        {
            lineSpan[i]++;
        }
    }

    public static void IncreaseOffset(ref TokenList tokenList, uint lineIndex, uint startTokenIndex, uint diff)
    {
        for (; startTokenIndex < tokenList.Count ; ++startTokenIndex)
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
}
