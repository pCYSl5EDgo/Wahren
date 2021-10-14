global using Wahren.AbstractSyntaxTree.Parser;
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

    public static void InsertDeletedToken(ref this Result result, int tokenIndex, int tokenCount)
    {
        result.TokenList.InsertUndefinedRange(tokenIndex, tokenCount);
        throw new NotImplementedException();
    }
}
