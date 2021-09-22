﻿namespace Wahren.AbstractSyntaxTree;

public static partial class TokenUtility
{
    public static string ToString(ref this Token token, ref DualList<char> source)
    {
        ref var range = ref token.Range;
        if (range.IsEmpty)
        {
            return "";
        }

        var builder = PooledStringBuilder.Rent();
        if (range.StartAndEndLineAreSame)
        {
            builder.Append(source[range.StartInclusive.Line].AsSpan(range.StartInclusive.Offset, range.EndExclusive.Offset - range.StartInclusive.Offset));
            goto RETURN;
        }

        builder.Append(source[range.StartInclusive.Line].AsSpan(range.StartInclusive.Offset));

        for (uint i = range.StartInclusive.Line + 1; i < range.EndExclusive.Line; i++)
        {
            builder.AppendLine().Append(source[i].AsSpan());
        }

        if (!range.IsEndAtLineEnd)
        {
            builder.AppendLine().Append(source[range.EndExclusive.Line].AsSpan(0, range.EndExclusive.Offset));
        }

    RETURN:
        return builder.ToString();
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, char other)
    {
        if (!token.Range.OneLine || token.LengthInFirstLine != 1)
        {
            return false;
        }

        return source[token.Range.StartInclusive.Line][token.Range.StartInclusive.Offset] == other;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, ReadOnlySpan<char> other)
    {
        ref var start = ref token.Range.StartInclusive;
        ref var end = ref token.Range.EndExclusive;
        return (start.Line == end.Line && start.Offset + other.Length == end.Offset) || (start.Line + 1 == end.Line && end.Offset == 0) && source[start.Line].AsSpan(start.Offset).SequenceEqual(other);
    }

    public static bool Equals(ref this SingleLineRange range, ref DualList<char> source, ReadOnlySpan<char> other)
    {
        return range.Length == other.Length && source[range.Line].AsSpan(range.Offset, range.Length).SequenceEqual(other);
    }

    public static bool EqualsIgnoreCase(ref this Token token, ref DualList<char> source, char other00, char other01)
    {
        ref var start = ref token.Range.StartInclusive;
        ref var end = ref token.Range.EndExclusive;
        if ((start.Line == end.Line && start.Offset + 1 == end.Offset) || (start.Line + 1 == end.Line && end.Offset == 0))
        {
            var c0 = source[start.Line][start.Offset];
            return c0 == other00 || c0 == other01;
        }

        return false;
    }

    public static bool IsOperator(ref this Token token, ref DualList<char> source)
    {
        var span = source[token.Range.StartInclusive.Line].AsSpan(token.Range.StartInclusive.Offset, token.LengthInFirstLine);
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

    public static bool IsSingleLineComment(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, '/', '/');

    public static bool IsSingleLineCommentSlashPlus(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, '/', '+');

    public static bool IsMultiLineCommentStart(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, '/', '*');

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