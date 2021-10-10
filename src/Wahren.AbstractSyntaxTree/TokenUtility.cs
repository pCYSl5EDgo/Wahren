namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class TokenUtility
{
    public static bool Equals(ref this Result result, uint tokenIndex, char other)
    {
        if (result.TokenList.GetLength(tokenIndex) != 1)
        {
            return false;
        }

        return result.GetSpan(tokenIndex)[0] == other;
    }

    public static bool TryParse(ref this Result result, uint tokenIndex, out int value)
    {
        return int.TryParse(result.GetSpan(tokenIndex), out value);
    }

    public static bool IsOperator(ref this Result result, uint tokenIndex)
    {
        var span = result.GetSpan(tokenIndex);
        switch (span.Length)
        {
            case 1:
                switch (span[0])
                {
                    case '=':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Assign;
                        return true;
                    case '>':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.CompareGreaterThan;
                        return true;
                    case '<':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.CompareLessThan;
                        return true;
                    case ',':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Comma;
                        return true;
                    case ':':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Colon;
                        return true;
                    case ';':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Semicolon;
                        return true;
                    case '+':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Add;
                        return true;
                    case '-':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Sub;
                        return true;
                    case '*':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Mul;
                        return true;
                    case '/':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Div;
                        return true;
                    case '%':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Percent;
                        return true;
                    case '{':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.BracketLeft;
                        return true;
                    case '}':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.BracketRight;
                        return true;
                    case '(':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.ParenLeft;
                        return true;
                    case ')':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.ParenRight;
                        return true;
                }
                break;
            case 2:
                switch (span[0])
                {
                    case '=' when span[1] == '=':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.CompareEqual;
                        return true;
                    case '!' when span[1] == '=':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.CompareNotEqual;
                        return true;
                    case '>' when span[1] == '=':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.CompareGreaterThanOrEqualTo;
                        return true;
                    case '<' when span[1] == '=':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.CompareLessThanOrEqualTo;
                        return true;
                    case '&' when span[1] == '&':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.And;
                        return true;
                    case '|' when span[1] == '|':
                        result.TokenList.GetKind(tokenIndex) = TokenKind.Or;
                        return true;
                    case '/' when span[1] == '/':
                    case '/' when span[1] == '*':
                    case '/' when span[1] == '+':
                    case '*' when span[1] == '/':
                        return true;
                }
                break;
        }

        return false;
    }

    public static bool IsAtmark(ref this Result result, uint tokenIndex) => Equals(ref result, tokenIndex, '@');

    public static bool IsAssign(ref this Result result, uint tokenIndex)
    {
        if (Equals(ref result, tokenIndex, '='))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.Assign;
            return true;
        }

        return false;
    }

    public static bool IsComparerEqual(ref this Result result, uint tokenIndex)
    {
        if (result.GetSpan(tokenIndex).SequenceEqual("=="))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.CompareEqual;
            return true;
        }

        return false;
    }

    public static bool IsComparerNotEqual(ref this Result result, uint tokenIndex)
    {
        if (result.GetSpan(tokenIndex).SequenceEqual("!="))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.CompareNotEqual;
            return true;
        }

        return false;
    }

    public static bool IsComma(ref this Result result, uint tokenIndex)
    {
        if (Equals(ref result, tokenIndex, ','))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.Comma;
            return true;
        }

        return false;
    }

    public static bool IsMul(ref this Result result, uint tokenIndex)
    {
        if (Equals(ref result, tokenIndex, '*'))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.Mul;
            return true;
        }

        return false;
    }

    public static bool IsSingleLineComment(ref this Result result, uint tokenIndex)
    {
        if (result.GetSpan(tokenIndex).SequenceEqual("//"))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.Comment;
            return true;
        }

        return false;
    }

    public static bool IsSingleLineCommentSlashPlus(ref this Result result, uint tokenIndex)
    {
        if (result.GetSpan(tokenIndex).SequenceEqual("/+"))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.Comment;
            return true;
        }

        return false;
    }

    public static bool IsMultiLineCommentStart(ref this Result result, uint tokenIndex)
    {
        if (result.GetSpan(tokenIndex).SequenceEqual("/*"))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.Comment;
            return true;
        }

        return false;
    }

    public static bool IsBracketLeft(ref this Result result, uint tokenIndex)
    {
        if (Equals(ref result, tokenIndex, '{'))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.BracketLeft;
            return true;
        }

        return false;
    }

    public static bool IsBracketRight(ref this Result result, uint tokenIndex)
    {
        if (Equals(ref result, tokenIndex, '}'))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.BracketRight;
            return true;
        }

        return false;
    }

    public static bool IsParenLeft(ref this Result result, uint tokenIndex)
    {
        if (Equals(ref result, tokenIndex, '('))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.ParenLeft;
            return true;
        }

        return false;
    }

    public static bool IsParenRight(ref this Result result, uint tokenIndex)
    {
        if (Equals(ref result, tokenIndex, ')'))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.ParenRight;
            return true;
        }

        return false;
    }

    public static bool IsColon(ref this Result result, uint tokenIndex)
    {
        if (Equals(ref result, tokenIndex, ':'))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.Colon;
            return true;
        }

        return false;
    }

    public static bool IsSemicolon(ref this Result result, uint tokenIndex)
    {
        if (Equals(ref result, tokenIndex, ';'))
        {
            result.TokenList.GetKind(tokenIndex) = TokenKind.Semicolon;
            return true;
        }

        return false;
    }
}
