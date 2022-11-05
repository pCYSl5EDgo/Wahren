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
        do
        {
            if (position.Offset < line.Count)
            {
                goto NO_INCREMENT_LINE;
            }

        INCREMENT_LINE:
            token.PrecedingNewLineCount++;
            position.Offset = 0;
            if (++position.Line >= source.Count)
            {
                return false;
            }

            line = ref source[position.Line];

        NO_INCREMENT_LINE:
            var currentLineRestSpan = line.AsSpan(position.Offset);
            var notWhitespaceIndex = currentLineRestSpan.IndexOfAnyExcept(' ', '\t');
            if (notWhitespaceIndex < 0)
            {
                token.PrecedingWhitespaceCount = currentLineRestSpan.Length;
                goto INCREMENT_LINE;
            }

            token.PrecedingWhitespaceCount = (uint)notWhitespaceIndex;
            position.Offset += (uint)notWhitespaceIndex;
            token.Position = position;
            currentLineRestSpan = currentLineRestSpan.Slice(notWhitespaceIndex);
            var whitespaceIndex = currentLineRestSpan.IndexOfAny(' ', '\t');
            var inspectionSpan = whitespaceIndex < 0 ? currentLineRestSpan : currentLineRestSpan.Slice(0, whitespaceIndex);
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
        token.Length = position.Offset - token.Position.Offset;
        return true;
    }
}
