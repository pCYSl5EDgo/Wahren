#if NETCOREAPP2_1
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wahren.Analysis.NetCoreApp2_1
{
    public static class TokenParser
    {
        public static Task<Token[]> Parse(this string file, Encoding encoding, bool englishMode)
        {
            if (englishMode) throw new NotImplementedException("英文モードは現在サポートしてません。");
            if (string.IsNullOrWhiteSpace(file) || !new FileInfo(file).Exists) throw new ArgumentException($"{file}は存在しません。");
            using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true), encoding, true))
                return sr.ReadToEndAsync().ContinueWith<Token[]>(Parse, file);
        }
        private static Token[] Parse(Task<string> result, object obj)
        {
            var file = obj as string;
            var resultMemory = result.Result.AsMemory();
            uint line = 0, column = 0;
            var answer = new List<Token>(0x10000);
            bool isMemo = false;
            void inOneLineProcess(ReadOnlySpan<char> span, ReadOnlyMemory<char> memory)
            {
                int start = isMemo ? span.IndexOf("*/") : 0;
                if (start == -1) return;
                //00 whitespace
                //01 identifier
                //02 text
                //03 digit
                //04 /
                //05 =
                //06 !
                //07 >
                //08 <
                //09 &
                //0a |
                //0b +
                byte Mode = 0;
                start += 2;
                isMemo = false;
                int length = 0;
                // /+と同じ列では\nが出るまでdebugLine = true;
                var debugLine = false;
                void TryParse(int i, int len = 1, byte mode = 0)
                {
                    if (length != 0)
                        answer.Add(new Token(file, line, (uint)(column - length), debugLine, memory.Slice(start, length)));
                    start = i;
                    length = len;
                    Mode = mode;
                }
                for (int i = start; i < span.Length; i++)
                {
                    ++column;
                    switch (span[i])
                    {
                        case ' ':
                        case '\t':
                            if (Mode == 0)
                                ++length;
                            else
                                TryParse(i);
                            break;
                        case '+':
                            switch (Mode)
                            {
                                case 2:
                                    ++length;
                                    break;
                                case 4:
                                    if (debugLine)
                                        throw new Exception($"{file}/{line}/{column - length}");
                                    debugLine = true;
                                    start = i + 1;
                                    Mode = 0;
                                    length = 0;
                                    break;
                                case 0xb:
                                    TryParse(i, mode: 3);
                                    break;
                                default:
                                    TryParse(i, mode: 0xb);
                                    break;
                            }
                            break;
                        case '|':
                            switch (Mode)
                            {
                                case 2:
                                    ++length;
                                    break;
                                case 0xa:
                                    length = 2;
                                    TryParse(i + 1, 0);
                                    break;
                                default:
                                    TryParse(i, mode: 0xa);
                                    break;
                            }
                            break;
                        case '&':
                            switch (Mode)
                            {
                                case 9:
                                    length = 2;
                                    TryParse(i + 1, 0);
                                    break;
                                default:
                                    TryParse(i, mode: 9);
                                    break;
                            }
                            break;
                        case '<':
                            switch (Mode)
                            {
                                case 2:
                                    ++length;
                                    break;
                                case 8:
                                    length = 2;
                                    TryParse(i + 1, 0);
                                    break;
                                default:
                                    TryParse(i, mode: 8);
                                    break;
                            }
                            break;
                        case '>':
                            switch (Mode)
                            {
                                case 2:
                                    ++length;
                                    break;
                                case 7:
                                    length = 2;
                                    TryParse(i + 1, 0);
                                    break;
                                default:
                                    TryParse(i, mode: 7);
                                    break;
                            }
                            break;
                        case '!':
                            switch (Mode)
                            {
                                case 2:
                                    ++length;
                                    break;
                                default:
                                    TryParse(i, mode: 6);
                                    break;
                            }
                            break;
                        case '=':
                            switch (Mode)
                            {
                                case 2:
                                    ++length;
                                    break;
                                case 5:
                                case 6:
                                case 7:
                                case 8:
                                    length = 2;
                                    TryParse(i + 1, 0);
                                    break;
                                default:
                                    TryParse(i, mode: 5);
                                    break;
                            }
                            break;
                        case '*':
                            switch (Mode)
                            {
                                case 2:
                                    ++length;
                                    break;
                                case 4:
                                    var memoEndIndex = span.IndexOf("*/");
                                    if (memoEndIndex == -1)
                                    {
                                        isMemo = true;
                                        return;
                                    }
                                    else
                                    {
                                        inOneLineProcess(span.Slice(memoEndIndex + 2), memory.Slice(memoEndIndex + 2));
                                        return;
                                    }
                                default:
                                    TryParse(i);
                                    TryParse(i + 1, 0);
                                    break;
                            }
                            break;
                        case '/':
                            switch (Mode)
                            {
                                case 2:
                                    ++length;
                                    break;
                                case 4:
                                    return;
                                default:
                                    TryParse(i, mode: 4);
                                    break;
                            }
                            break;
                        #region Digit
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            #endregion
                            switch (Mode)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    ++length;
                                    break;
                                default:
                                    TryParse(i, mode: 3);
                                    break;
                            }
                            break;
                        #region Alphabet
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                        case 'g':
                        case 'h':
                        case 'i':
                        case 'j':
                        case 'k':
                        case 'l':
                        case 'm':
                        case 'n':
                        case 'o':
                        case 'p':
                        case 'q':
                        case 'r':
                        case 's':
                        case 't':
                        case 'u':
                        case 'v':
                        case 'w':
                        case 'x':
                        case 'y':
                        case 'z':
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                        case 'G':
                        case 'H':
                        case 'I':
                        case 'J':
                        case 'K':
                        case 'L':
                        case 'M':
                        case 'N':
                        case 'O':
                        case 'P':
                        case 'Q':
                        case 'R':
                        case 'S':
                        case 'T':
                        case 'U':
                        case 'V':
                        case 'W':
                        case 'X':
                        case 'Y':
                        case 'Z':
                            #endregion
                            switch (Mode)
                            {
                                case 1:
                                case 2:
                                    ++length;
                                    break;
                                case 3:
                                    throw new Exception($"{file}/{line}/{column - length}");
                                default:
                                    TryParse(i, mode: 1);
                                    break;
                            }
                            break;
                        case '_':
                        case '@':
                            switch (Mode)
                            {
                                case 1:
                                case 2:
                                    ++length;
                                    break;
                                case 3:
                                    throw new Exception($"{file}/{line}/{column - length}");
                                default:
                                    TryParse(i, mode: 1);
                                    break;
                            }
                            break;
                        case '(':
                        case ')':
                        case ';':
                        case ',':
                            TryParse(i);
                            TryParse(i + 1, 0);
                            break;
                        case '-':
                        case '{':
                        case '}':
                        case ':':
                            if (Mode == 2)
                                ++length;
                            else
                            {
                                TryParse(i);
                                TryParse(i + 1, 0);
                            }
                            break;
                        default:
                            switch (Mode)
                            {
                                case 1:
                                case 3:
                                    ++length;
                                    Mode = 2;
                                    break;
                                case 2:
                                    ++length;
                                    break;
                                default:
                                    TryParse(i, mode: 2);
                                    break;
                            }
                            break;
                    }
                    ++column;
                }
                if (length != 0)
                    TryParse(0, 0, 0);
            }

            return answer.ToArray();
        }
    }
}
#endif