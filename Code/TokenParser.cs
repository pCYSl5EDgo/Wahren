using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;

namespace Wahren.Analysis
{
    public static class TokenParser
    {
        static readonly char[] array = new char[] { default(char), default(char), default(char), default(char), '/', '*', '=', '!', '>', '<', '&', '|', '+', '-' };

        public static async Task<List<Token>> Parse(this string file, Encoding encoding, bool englishMode)
        {
            if (englishMode) throw new NotImplementedException("英文モードは現在サポートしてません。");
            if (string.IsNullOrWhiteSpace(file) || !new FileInfo(file).Exists) throw new ArgumentException($"{file}は存在しません。");
            #region Locals
            void TryParse(List<Token> ans, StringBuilder _, string f, int l, int col, bool dbg)
            {
                var b = _.ToString();
                if (long.TryParse(b, out var tmp))
                    ans.Add(new Token(f, l, col - _.Length, dbg, false, tmp));
                else
                    ans.Add(new Token(f, l, col - _.Length, dbg, false, b));
                _.Clear();
            }
            void CommentAdd(List<Token> ans, StringBuilder _, string f, int l, int col, bool dbg)
            {
                ans.Add(new Token(f, l, col - _.Length, dbg, true, _.ToString()));
                _.Clear();
            }
            int line = 0;
            int column = 0;
            //00 whitespace
            //01 identifier
            //02 //
            //03 /*
            //04 /
            //05 */ (/*のあとの場合のみ)
            //06 =
            //07 !
            //08 >
            //09 <
            //0a &
            //0b |
            //0c +
            //0d -
            byte mode = 0;
            // /+と同じ列では\nが出るまでdebugLine = true;
            bool debugLine = false;
            var buf = new StringBuilder();
            var answer = new List<Token>(4096);
            string content;
            void inOneLineProcess(ReadOnlySpan<char> span)
            {
                for (int i = 0; i < span.Length; i++)
                    switch (span[i])
                    {
                        case '　':
                            switch (mode)
                            {
                                case 0:
                                case 2:
                                case 3:
                                    buf.Append('　');
                                    break;
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 0;
                                    buf.Append('　');
                                    break;
                                case 5:
                                    buf.Append('*').Append('　');
                                    mode = 3;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 0;
                                    buf.Append('　');
                                    break;
                            }
                            break;
                        case ' ':
                        case '\t':
                            switch (mode)
                            {
                                case 0:
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 0;
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 0;
                                    buf.Append(span[i]);
                                    break;
                            }
                            ++column;
                            break;
                        case '(':
                        case ')':
                        case '{':
                        case '}':
                        case '%':
                        case '$':
                        case '[':
                        case ']':
                        case ';':
                        case ':':
                        case ',':
                        case '.':
                        case '#':
                        case '@':
                        case '\'':
                        case '\\':
                        case '~':
                        case '^':
                        case '"':
                        case '`':
                        case '?':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    answer.Add(new Token(file, line, column, debugLine, false, span[i]));
                                    mode = 0;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    answer.Add(new Token(file, line, column, debugLine, false, span[i]));
                                    mode = 0;
                                    break;
                            }
                            ++column;
                            break;
                        case '-':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 13;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 13;
                                    break;
                            }
                            ++column;
                            break;
                        case '+':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 12;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 4:
                                    answer.Add(new Token(file, line, column - 1, debugLine, true, '/', '+'));
                                    mode = 0;
                                    debugLine = true;
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 12;
                                    break;
                            }
                            ++column;
                            break;
                        case '|':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 11;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                case 11:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, '|', '|'));
                                    mode = 0;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 11;
                                    break;
                            }
                            ++column;
                            break;
                        case '&':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 10;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                case 10:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, '&', '&'));
                                    mode = 0;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 10;
                                    break;
                            }
                            ++column;
                            break;
                        case '<':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 9;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                case 9:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, '<', '<'));
                                    mode = 0;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 9;
                                    break;
                            }
                            ++column;
                            break;
                        case '>':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 8;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                case 8:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, '>', '>'));
                                    mode = 0;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 8;
                                    break;
                            }
                            ++column;
                            break;
                        case '!':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 7;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 7;
                                    break;
                            }
                            ++column;
                            break;
                        case '=':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 6;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                case 6:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, '=', '='));
                                    mode = 0;
                                    break;
                                case 7:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, '!', '='));
                                    mode = 0;
                                    break;
                                case 8:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, '>', '='));
                                    mode = 0;
                                    break;
                                case 9:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, '<', '='));
                                    mode = 0;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 6;
                                    break;
                            }
                            ++column;
                            break;
                        case '*':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    answer.Add(new Token(file, line, column, debugLine, false, span[i]));
                                    mode = 0;
                                    break;
                                case 2:
                                    buf.Append(span[i]);
                                    break;
                                case 3:
                                    mode = 5;
                                    break;
                                case 4:
                                    answer.Add(new Token(file, line, column - 1, debugLine, true, '/', '*'));
                                    mode = 3;
                                    break;
                                case 5:
                                    buf.Append(span[i]);
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, span[i]));
                                    mode = 0;
                                    break;
                            }
                            ++column;
                            break;
                        case '/':
                            switch (mode)
                            {
                                case 0:
                                case 1:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 4;
                                    break;
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 4:
                                    answer.Add(new Token(file, line, column - 1, debugLine, true, '/', '/'));
                                    mode = 2;
                                    break;
                                case 5:
                                    if (buf.Length != 0)
                                    {
                                        answer.Add(new Token(file, line, column - buf.Length, debugLine, true, buf.ToString()));
                                        buf.Clear();
                                    }
                                    answer.Add(new Token(file, line, column - 1, debugLine, true, '*', '/'));
                                    mode = 0;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 4;
                                    break;
                            }
                            ++column;
                            break;
                        default:
                            switch (mode)
                            {
                                case 0:
                                    if (buf.Length != 0)
                                        TryParse(answer, buf, file, line, column, debugLine);
                                    mode = 1;
                                    buf.Append(span[i]);
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                    buf.Append(span[i]);
                                    break;
                                case 5:
                                    buf.Append('*').Append(span[i]);
                                    mode = 3;
                                    break;
                                default:
                                    answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                    mode = 1;
                                    buf.Append(span[i]);
                                    break;
                            }
                            ++column;
                            break;
                    }
                switch (mode)
                {
                    case 0:
                    case 1:
                        if (buf.Length != 0)
                            TryParse(answer, buf, file, line, column, debugLine);
                        mode = 0;
                        break;
                    case 2:
                        if (buf.Length != 0)
                            CommentAdd(answer, buf, file, line, column, debugLine);
                        mode = 0;
                        break;
                    case 3:
                        if (buf.Length != 0)
                            CommentAdd(answer, buf, file, line, column, debugLine);
                        break;
                    case 5:
                        buf.Append('*');
                        CommentAdd(answer, buf, file, line, column, debugLine);
                        mode = 3;
                        break;
                    default:
                        answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                        mode = 0;
                        break;
                }
                debugLine = false;
                ++line;
                column = 0;
            }
            #endregion
            using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true), encoding))
                while ((content = await sr.ReadLineAsync()) != null)
                    inOneLineProcess(content.AsSpan());
            switch (mode)
            {
                case 0:
                case 1:
                    if (buf.Length != 0)
                        TryParse(answer, buf, file, line, column, debugLine);
                    break;
                case 2:
                case 3:
                    if (buf.Length != 0)
                        CommentAdd(answer, buf, file, line, column, debugLine);
                    break;
                case 5:
                    buf.Append('*');
                    CommentAdd(answer, buf, file, line, column, debugLine);
                    break;
                default:
                    answer.Add(new Token(file, line, column, debugLine, false, array[mode]));
                    break;
            }
            return answer;
        }


        public static IEnumerable<Token> RemoveDebugCommentWhiteSpace(this List<Token> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].IsMemo || input[i].IsDebug) continue;
                if (input[i].Type == 0 && string.IsNullOrWhiteSpace(input[i].Content)) continue;
                yield return input[i];
            }
        }
        public static IEnumerable<Token> RemoveCommentWhiteSpace(this List<Token> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].IsMemo) continue;
                if (input[i].Type == 0 && string.IsNullOrWhiteSpace(input[i].Content)) continue;
                yield return input[i];
            }
        }



        static Task A(){
            return Task.CompletedTask;
        }
    }
}