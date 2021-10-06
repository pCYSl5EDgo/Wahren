namespace Wahren.AbstractSyntaxTree;

public static partial class TokenUtility
{
    public static bool Equals(ref this Token token, ref DualList<char> source, char other)
    {
        if (token.Length != 1)
        {
            return false;
        }

        return source[token.Position.Line][token.Position.Offset] == other;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, ReadOnlySpan<char> other)
    {
        ref var start = ref token.Position;
        return source[start.Line].AsSpan(start.Offset, token.Length).SequenceEqual(other);
    }

    public static bool IsOperator(ref this Token token, ref DualList<char> source)
    {
        var span = source[token.Position.Line].AsSpan(token.Position.Offset, token.Length);
        switch (span.Length)
        {
            case 1:
                switch (span[0])
                {
                    case '=':
                        token.Kind = TokenKind.Assign;
                        return true;
                    case '>':
                        token.Kind = TokenKind.CompareGreaterThan;
                        return true;
                    case '<':
                        token.Kind = TokenKind.CompareLessThan;
                        return true;
                    case ',':
                        token.Kind = TokenKind.Comma;
                        return true;
                    case ':':
                        token.Kind = TokenKind.Colon;
                        return true;
                    case ';':
                        token.Kind = TokenKind.Semicolon;
                        return true;
                    case '+':
                        token.Kind = TokenKind.Add;
                        return true;
                    case '-':
                        token.Kind = TokenKind.Sub;
                        return true;
                    case '*':
                        token.Kind = TokenKind.Mul;
                        return true;
                    case '/':
                        token.Kind = TokenKind.Div;
                        return true;
                    case '%':
                        token.Kind = TokenKind.Percent;
                        return true;
                    case '{':
                        token.Kind = TokenKind.BracketLeft;
                        return true;
                    case '}':
                        token.Kind = TokenKind.BracketRight;
                        return true;
                    case '(':
                        token.Kind = TokenKind.ParenLeft;
                        return true;
                    case ')':
                        token.Kind = TokenKind.ParenRight;
                        return true;
                }
                break;
            case 2:
                switch (span[0])
                {
                    case '=' when span[1] == '=':
                        token.Kind = TokenKind.CompareEqual;
                        return true;
                    case '!' when span[1] == '=':
                        token.Kind = TokenKind.CompareNotEqual;
                        return true;
                    case '>' when span[1] == '=':
                        token.Kind = TokenKind.CompareGreaterThanOrEqualTo;
                        return true;
                    case '<' when span[1] == '=':
                        token.Kind = TokenKind.CompareLessThanOrEqualTo;
                        return true;
                    case '&' when span[1] == '&':
                        token.Kind = TokenKind.And;
                        return true;
                    case '|' when span[1] == '|':
                        token.Kind = TokenKind.Or;
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

    public static bool IsAtmark(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, '@');

    public static bool IsAssign(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '='))
        {
            token.Kind = TokenKind.Assign;
            return true;
        }

        return false;
    }

    public static bool IsComparerEqual(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '=', '='))
        {
            token.Kind = TokenKind.CompareEqual;
            return true;
        }

        return false;
    }

    public static bool IsComparerNotEqual(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '!', '='))
        {
            token.Kind = TokenKind.CompareNotEqual;
            return true;
        }

        return false;
    }

    public static bool IsComma(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, ','))
        {
            token.Kind = TokenKind.Comma;
            return true;
        }

        return false;
    }

    public static bool IsMul(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '*'))
        {
            token.Kind = TokenKind.Mul;
            return true;
        }

        return false;
    }

    public static bool IsSingleLineComment(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '/', '/'))
        {
            token.Kind = TokenKind.Comment;
            return true;
        }

        return false;
    }

    public static bool IsSingleLineCommentSlashPlus(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '/', '+'))
        {
            token.Kind = TokenKind.Comment;
            return true;
        }

        return false;
    }

    public static bool IsMultiLineCommentStart(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '/', '*'))
        {
            token.Kind = TokenKind.Comment;
            return true;
        }

        return false;
    }

    public static bool IsBracketLeft(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '{'))
        {
            token.Kind = TokenKind.BracketLeft;
            return true;
        }

        return false;
    }

    public static bool IsBracketRight(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '}'))
        {
            token.Kind = TokenKind.BracketRight;
            return true;
        }

        return false;
    }

    public static bool IsParenLeft(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, '('))
        {
            token.Kind = TokenKind.ParenLeft;
            return true;
        }

        return false;
    }

    public static bool IsParenRight(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, ')'))
        {
            token.Kind = TokenKind.ParenRight;
            return true;
        }

        return false;
    }

    public static bool IsColon(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, ':'))
        {
            token.Kind = TokenKind.Colon;
            return true;
        }

        return false;
    }

    public static bool IsSemicolon(ref this Token token, ref DualList<char> source)
    {
        if (Equals(ref token, ref source, ';'))
        {
            token.Kind = TokenKind.Semicolon;
            return true;
        }

        return false;
    }
}
