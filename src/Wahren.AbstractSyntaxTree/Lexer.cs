using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Wahren.AbstractSyntaxTree;

public static class Lexer
{
    public static bool ReadToken(ref DualList<char> source, ref Position position, ref Token token)
    {
        if (position.Line >= source.Count)
        {
            return false;
        }

        ref var line = ref source[position.Line];
        ref var range = ref token.Range;

        do
        {
            if (position.Offset < line.Count)
            {
                goto NO_INCREMENT_LINE;
            }

        INCREMENT_LINE:
            position.Offset = 0;
            if (++position.Line >= source.Count)
            {
                return false;
            }

            line = ref source[position.Line];

        NO_INCREMENT_LINE:
            var currentLineRestSpan = line.AsSpan(position.Offset);
            token.PrecedingWhitespaceCount = CountLeadingWhitespace(currentLineRestSpan);
            if (token.PrecedingWhitespaceCount == currentLineRestSpan.Length)
            {
                goto INCREMENT_LINE;
            }

            position.Offset += token.PrecedingWhitespaceCount;
            range.StartInclusive = position;
            currentLineRestSpan = currentLineRestSpan.Slice((int)token.PrecedingWhitespaceCount);
            var whitespaceIndex = currentLineRestSpan.IndexOfAny(' ', '\t');
            var inspectionSpan = whitespaceIndex == -1 ? currentLineRestSpan : currentLineRestSpan.Slice(0, whitespaceIndex);
            var symbolIndex = inspectionSpan.IndexOfAny("{}(),;:=!<>&|*/+-%");
            switch (symbolIndex)
            {
                default:
                    position.Offset += (uint)symbolIndex;
                    goto RETURN_VALIDATION;
                case -1:
                    position.Offset += (uint)inspectionSpan.Length;
                    goto RETURN_VALIDATION;
                case 0:
                    position.Offset++;
                    if (inspectionSpan.Length == 1)
                    {
                        goto RETURN_VALIDATION;
                    }

                    /*
                     * ==
                     * !=
                     * >=
                     * <=
                     * =
                     * <
                     * >
                     * &&
                     * ||
                     * //
                     * /*
                     * /+
                     * /
                     * *
                     * * /
                     * +
                     * -
                     * %
                     * {
                     * }
                     * (
                     * )
                     * ,
                     * ;
                     * :
                     */

                    switch (inspectionSpan[0])
                    {
                        case '%':
                        case '{':
                        case '}':
                        case '(':
                        case ')':
                        case ',':
                        case ';':
                        case ':':
                            goto RETURN_VALIDATION;
                        case '=':
                        case '!':
                        case '>':
                        case '<':
                            if (inspectionSpan[1] == '=')
                            {
                                position.Offset++;
                            }

                            goto RETURN_VALIDATION;
                        case '&':
                        case '|':
                            if (inspectionSpan[0] == inspectionSpan[1])
                            {
                                position.Offset++;
                                goto RETURN_VALIDATION;
                            }

                            break;
                        case '*':
                            if (inspectionSpan[1] == '/')
                            {
                                position.Offset++;
                            }

                            goto RETURN_VALIDATION;
                        case '/':
                            switch (inspectionSpan[1])
                            {
                                case '/':
                                case '*':
                                case '+':
                                    position.Offset++;
                                    break;
                            }

                            goto RETURN_VALIDATION;
                    }
                    var nextSymbolIndex = inspectionSpan.Slice(1).IndexOfAny("{}(),;:=!<>&|*/+-%");
                    if (nextSymbolIndex == -1)
                    {
                        position.Offset += (uint)(inspectionSpan.Length - 1);
                    }
                    else
                    {
                        position.Offset += (uint)nextSymbolIndex;
                    }
                    goto RETURN_VALIDATION;
            }
        } while (true);

    RETURN_VALIDATION:
        if (position.Offset >= line.Count)
        {
            token.Length = (uint)line.Count - range.StartInclusive.Offset;
            position.Line++;
            position.Offset = 0;
        }
        else
        {
            token.Length = position.Offset - range.StartInclusive.Offset;
        }

        range.EndExclusive = position;
        return true;
    }

    public static bool ReadTokenSemicolon(ref DualList<char> source, ref Position position, ref Token token)
    {
        if (position.Line >= source.Count)
        {
            return false;
        }

        ref var line = ref source[position.Line];
        ref var range = ref token.Range;

        Span<char> currentLineRestSpan;
        do
        {
            currentLineRestSpan = line.AsSpan(position.Offset);
            token.PrecedingWhitespaceCount = CountLeadingWhitespace(currentLineRestSpan);
            if (token.PrecedingWhitespaceCount != currentLineRestSpan.Length)
            {
                break;
            }

            position.Offset = 0;
            if (++position.Line >= source.Count)
            {
                return false;
            }

            line = ref source[position.Line];
        } while (true);

        position.Offset += token.PrecedingWhitespaceCount;
        range.StartInclusive = position;
        currentLineRestSpan = currentLineRestSpan.Slice((int)token.PrecedingWhitespaceCount);
        var symbolIndex = currentLineRestSpan.IndexOf(';');
        while (symbolIndex == -1)
        {
            position.Offset = 0;
            if (++position.Line >= source.Count)
            {
                return false;
            }

            symbolIndex = source[position.Line].AsSpan().IndexOf(';');
        }

        position.Offset += (uint)symbolIndex;
        ReadBackUntilNotWhitespaceAppears(ref source, ref position, ref range.StartInclusive);

        range.EndExclusive = position;
        if (range.EndExclusive.Line == range.StartInclusive.Line)
        {
            token.Length = range.EndExclusive.Offset - range.StartInclusive.Offset;
        }
        else
        {
            token.Length = (uint)line.Count - range.StartInclusive.Offset;
        }

        return true;
    }

    private static void ReadBackUntilNotWhitespaceAppears(ref DualList<char> source, ref Position position, ref Position startInclusive)
    {
        ref List<char> currentLine = ref source[position.Line];
        do
        {
            if (position.Line < startInclusive.Line || (position.Line == startInclusive.Line && position.Offset <= startInclusive.Offset))
            {
                position = startInclusive;
                return;
            }

            if (position.Offset == 0)
            {
                do
                {
                    if (--position.Line < 0)
                    {
                        position = startInclusive;
                        return;
                    }
                    currentLine = ref source[position.Line];
                    position.Offset = currentLine.LastIndex;
                } while (currentLine.IsEmpty);
            }
            else
            {
                --position.Offset;
            }

            switch (currentLine[position.Offset])
            {
                case ' ':
                case '\t':
                    continue;
                default:
                    if (++position.Offset >= currentLine.Count)
                    {
                        position.Line++;
                        position.Offset = 0;
                    }
                    return;
            }
        } while (true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="position"></param>
    /// <param name="token"></param>
    /// <returns>If already at the end of the source, return false else return true.</returns>
    public static bool ReadTokenToEndOfLine(ref DualList<char> source, ref Position position, ref Token token)
    {
        if (position.Line >= source.Count)
        {
            return false;
        }

        token.Range.StartInclusive = position;
        token.Length = (uint)source[position.Line].Count - position.Offset;
        position.Offset = 0;
        position.Line++;
        token.Range.EndExclusive = position;
        return true;
    }

    public static unsafe uint CountLeadingWhitespace(ReadOnlySpan<char> span)
    {
        var index = 0U;
        if (Avx2.IsSupported && span.Length >= 32)
        {
            const uint dropMask = 0b1111_1111_1111_1111_1111_1111_1111_0000U;
            index = (uint)span.Length & dropMask;
            fixed (char* ptr = span)
            {
                var vector0x20 = Vector256.Create((short)0x20);
                var vectorZero = Vector256<short>.Zero;
                var itr = (short*)ptr;
                var itrEnd = itr + index;
                for (; itr != itrEnd; itr += 16)
                {
                    var itrVector = Avx.LoadVector256(itr);
                    var trailingZero = Bmi1.TrailingZeroCount((uint)Avx2.MoveMask(Avx2.Or(Avx2.CompareGreaterThan(itrVector, vector0x20), Avx2.CompareGreaterThan(vectorZero, itrVector)).AsByte()));
                    if (trailingZero != 32)
                    {
                        return (uint)(((nuint)itr - (nuint)ptr + trailingZero) >> 1);
                    }
                }
            }
        }

        for (; index < span.Length; ++index)
        {
            var c = (ushort)span[(int)index];
            if (c > 0x20U)
            {
                return index;
            }
        }

        return (uint)span.Length;
    }
}
