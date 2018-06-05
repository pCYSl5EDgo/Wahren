using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Wahren
{
    public struct Token : Specific.IDebugInfo
    {
        public readonly string File;
        public readonly int Line;
        public readonly int Column;
        //string 0
        //symbol 1
        //digit 2
        public readonly byte Type;
        public readonly bool IsDebug;
        public readonly bool IsMemo;
        public readonly string Content;
        public readonly char Symbol1;
        public readonly char Symbol2;
        public readonly long Number;

        public override string ToString() => Type == 0 ? Content : (Type == 2 ? Number.ToString() : (Symbol2 == default(char) ? new string(new char[1] { Symbol1 }) : new string(new char[2] { Symbol1, Symbol2 })));
        public string DebugInfo => File + '/' + (Line + 1) + '/' + Column;
        public string ToLowerString()
        {
            switch (Type)
            {
                case 0:
                    return Content.ToLower();
                case 1:
                    return Number.ToString();
                case 2:
                    if (Symbol2 == default(char))
                        return Symbol1.ToString();
                    else
                        return new string(new char[2] { Symbol1, Symbol2 });
            }
            throw new InvalidDataException();
        }

        public Token(string file, int line, int column, bool isDebug, bool isMemo, byte type, string content, char symbol1, char symbol2, long number)
        {
            File = file;
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = content;
            Type = type;
            Symbol1 = symbol1;
            Symbol2 = symbol2;
            Number = number;
        }

        public Token(string file, int line, int column, bool isDebug, bool isMemo, string content)
        {
            File = file;
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = content;
            Type = 0;
            Symbol1 = Symbol2 = default(char);
            Number = default(long);
        }
        public Token(string file, int line, int column, bool isDebug, bool isMemo, char symbol1)
        {
            File = file;
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = null;
            Type = 1;
            Symbol1 = symbol1;
            Symbol2 = default(char);
            Number = default(long);
        }
        public Token(string file, int line, int column, bool isDebug, bool isMemo, char symbol1, char symbol2)
        {
            File = file;
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = null;
            Type = 1;
            Symbol1 = symbol1;
            Symbol2 = symbol2;
            Number = default(long);
        }
        public Token(string file, int line, int column, bool isDebug, bool isMemo, long number)
        {
            File = file;
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = null;
            Type = 2;
            Symbol1 = Symbol2 = default(char);
            Number = number;
        }
        public bool IsSingleSymbol => Type == 1 && Symbol2 == default(char);
        public bool IsDoubleSymbol => Type == 1 && Symbol2 != default(char);

        public bool IsNext(ref Token next)
        {
            if (Line == next.Line)
                switch (this.Type)
                {
                    case 0:
                        if (Column + this.Content.Length == next.Column) return true;
                        return false;
                    case 1:
                        if (IsSingleSymbol && Column + 1 == next.Column) return true;
                        if (IsDoubleSymbol && Column + 2 == next.Column) return true;
                        return false;
                    case 2:
                        if (this.Number == 0 && Column + 1 == next.Column) return true;
                        if (this.Number == 0) return false;
                        if (Column + (int)(Math.Log10(this.Number)) + 1 == next.Column) return true;
                        return false;
                    default: return false;
                }
            return false;
        }
        public bool IsOneWhiteSpaced(ref Token next)
        {
            if (Line == next.Line)
                switch (this.Type)
                {
                    case 0:
                        if (Column + this.Content.Length + 1 == next.Column) return true;
                        return false;
                    case 1:
                        if (IsSingleSymbol && Column + 2 == next.Column) return true;
                        if (IsDoubleSymbol && Column + 3 == next.Column) return true;
                        return false;
                    case 2:
                        if (this.Number == 0 && Column + 2 == next.Column) return true;
                        if (this.Number == 0) return false;
                        if (Column + (int)(Math.Log10(this.Number)) + 2 == next.Column) return true;
                        return false;
                    default: return false;
                }
            return false;
        }

        public Token ForceMerge(ref Token next)
        {
            if (this.Type == 1 && this.IsNext(ref next) && next.Type == 2)
            {
                if (this.Symbol1 == '+')
                    return next;
                else if (this.IsSingleSymbol && this.Symbol1 == '-')
                    return new Token(this.File, this.Line, this.Column, this.IsDebug, this.IsMemo, -next.Number);
            }
            var buf = new StringBuilder();
            switch (this.Type)
            {
                case 0:
                    buf.Append(Content);
                    break;
                case 1:
                    buf.Append(Symbol1);
                    if (IsDoubleSymbol)
                        buf.Append(Symbol2);
                    break;
                case 2:
                    buf.Append(Number);
                    break;
                default: throw new ApplicationException();
            }
            switch (next.Type)
            {
                case 0:
                    buf.Append(next.Content);
                    break;
                case 1:
                    buf.Append(next.Symbol1);
                    if (next.IsDoubleSymbol)
                        buf.Append(next.Symbol2);
                    break;
                case 2:
                    buf.Append(next.Number);
                    break;
                default: throw new ApplicationException();
            }
            return new Token(File, Line, Column, IsDebug, IsMemo, buf.ToString());
        }
        public Token ForceMerge(string text)
        {
            switch (this.Type)
            {
                case 0:
                    return new Token(File, Line, Column, IsDebug, IsMemo, Content + text);
                case 1:
                    if (IsDoubleSymbol)
                        return new Token(File, Line, Column, IsDebug, IsMemo, Symbol1 + text);
                    return new Token(File, Line, Column, IsDebug, IsMemo, Symbol1 + Symbol2 + text);
                case 2:
                    return new Token(File, Line, Column, IsDebug, IsMemo, Number + text);
                default: throw new ApplicationException();
            }
        }
        public Token Merge(ref Token next)
        {
            if (IsOneWhiteSpaced(ref next))
            {
                var buf = new StringBuilder();
                switch (this.Type)
                {
                    case 0:
                        buf.Append(Content);
                        break;
                    case 1:
                        buf.Append(Symbol1);
                        if (IsDoubleSymbol)
                            buf.Append(Symbol2);
                        break;
                    case 2:
                        buf.Append(Number);
                        break;
                    default: throw new ApplicationException();
                }
                buf.Append(' ');
                switch (next.Type)
                {
                    case 0:
                        buf.Append(next.Content);
                        break;
                    case 1:
                        buf.Append(next.Symbol1);
                        if (next.IsDoubleSymbol)
                            buf.Append(next.Symbol2);
                        break;
                    case 2:
                        buf.Append(next.Number);
                        break;
                    default: throw new ApplicationException();
                }
                return new Token(File, Line, Column, IsDebug, IsMemo, buf.ToString());
            }
            return ForceMerge(ref next);
        }
    }

    public static class TokenParser
    {
        public static async Task<List<Token>> Parse(this string file, Encoding encoding, bool englishMode)
        {
            if (englishMode) throw new NotImplementedException("英文モードは現在サポートしてません。");
            if (string.IsNullOrWhiteSpace(file) || !new FileInfo(file).Exists) throw new ArgumentException($"{file}は存在しません。");
            void TryParse(List<Token> ans, StringBuilder _, string f, int l, int col, bool dbg)
            {
                long tmp;
                var b = _.ToString();
                if (long.TryParse(b, out tmp))
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
            char[] array = new char[0x0d + 1] { default(char), default(char), default(char), default(char), '/', '*', '=', '!', '>', '<', '&', '|', '+', '-' };
            var answer = new List<Token>();
            string content;
            using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true), encoding))
                while ((content = await sr.ReadLineAsync()) != null)
                {
                    for (int i = 0; i < content.Length; i++)
                        switch (content[i])
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
                                        buf.Append(content[i]);
                                        break;
                                    case 1:
                                        if (buf.Length != 0)
                                            TryParse(answer, buf, file, line, column, debugLine);
                                        mode = 0;
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
                                        mode = 3;
                                        break;
                                    default:
                                        answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                        mode = 0;
                                        buf.Append(content[i]);
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
                                        answer.Add(new Token(file, line, column, debugLine, false, content[i]));
                                        mode = 0;
                                        break;
                                    case 2:
                                    case 3:
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
                                        mode = 3;
                                        break;
                                    default:
                                        answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                        answer.Add(new Token(file, line, column, debugLine, false, content[i]));
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
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
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
                                        buf.Append(content[i]);
                                        break;
                                    case 4:
                                        answer.Add(new Token(file, line, column - 1, debugLine, true, '/', '+'));
                                        mode = 0;
                                        debugLine = true;
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
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
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
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
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
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
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
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
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
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
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
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
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
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
                                        answer.Add(new Token(file, line, column, debugLine, false, content[i]));
                                        mode = 0;
                                        break;
                                    case 2:
                                        buf.Append(content[i]);
                                        break;
                                    case 3:
                                        mode = 5;
                                        break;
                                    case 4:
                                        answer.Add(new Token(file, line, column - 1, debugLine, true, '/', '*'));
                                        mode = 3;
                                        break;
                                    case 5:
                                        buf.Append(content[i]);
                                        break;
                                    default:
                                        answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                        answer.Add(new Token(file, line, column - 1, debugLine, false, content[i]));
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
                                        buf.Append(content[i]);
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
                                        buf.Append(content[i]);
                                        break;
                                    case 1:
                                    case 2:
                                    case 3:
                                        buf.Append(content[i]);
                                        break;
                                    case 5:
                                        buf.Append('*').Append(content[i]);
                                        mode = 3;
                                        break;
                                    default:
                                        answer.Add(new Token(file, line, column - 1, debugLine, false, array[mode]));
                                        mode = 1;
                                        buf.Append(content[i]);
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
        public static List<Token> RemoveDebugCommentWhiteSpace(this List<Token> input)
        {
            var answer = new List<Token>(input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].IsMemo || input[i].IsDebug) continue;
                if (input[i].Type == 0 && string.IsNullOrWhiteSpace(input[i].Content)) continue;
                answer.Add(input[i]);
            }
            return answer;
        }
        public static List<Token> RemoveCommentWhiteSpace(this List<Token> input)
        {
            var answer = new List<Token>(input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].IsMemo) continue;
                if (input[i].Type == 0 && string.IsNullOrWhiteSpace(input[i].Content)) continue;
                answer.Add(input[i]);
            }
            return answer;
        }

        public static async Task<List<Token>> RemoveDebugComment(this Task<List<Token>> input)
        {
            if (input == null) throw new ArgumentNullException();
            var list = await input;
            var ans = new List<Token>(list.Count);
            for (int i = 0; i < list.Count; i++)
                if (list[i].IsDebug || list[i].IsMemo) continue;
                else ans.Add(list[i]);
            return ans;
        }
        public static async Task<List<Token>> RemoveComment(this Task<IEnumerable<Token>> input)
        {
            if (input == null) throw new ArgumentNullException();
            var ans = new List<Token>();
            ans.AddRange((await input).Where(_ => !_.IsMemo));
            return ans;
        }
        public static async Task<List<Token>> RemoveDebug(this Task<IEnumerable<Token>> input)
        {
            if (input == null) throw new ArgumentNullException();
            var ans = new List<Token>();
            ans.AddRange((await input).Where(_ => !_.IsDebug));
            return ans;
        }
        public static async Task<List<Token>> RemoveComment(this Task<List<Token>> input)
        {
            if (input == null) throw new ArgumentNullException();
            var list = await input;
            var ans = new List<Token>(list.Count);
            for (int i = 0; i < list.Count; i++)
                if (list[i].IsMemo) continue;
                else ans.Add(list[i]);
            return ans;
        }
        public static async Task<List<Token>> RemoveDebug(this Task<List<Token>> input)
        {
            if (input == null) throw new ArgumentNullException();
            var list = await input;
            var ans = new List<Token>(list.Count);
            for (int i = 0; i < list.Count; i++)
                if (list[i].IsDebug) continue;
                else ans.Add(list[i]);
            return ans;
        }
        public static List<Token> RemoveDebugComment(this List<Token> list)
        {
            var ans = new List<Token>(list.Count);
            for (int i = 0; i < list.Count; i++)
                if (list[i].IsDebug || list[i].IsMemo) continue;
                else ans.Add(list[i]);
            return ans;
        }
        public static List<Token> RemoveComment(this IEnumerable<Token> input)
        {
            if (input == null) throw new ArgumentNullException();
            var ans = new List<Token>();
            ans.AddRange(input.Where(_ => !_.IsMemo));
            return ans;
        }
        public static List<Token> RemoveDebug(this IEnumerable<Token> input)
        {
            var ans = new List<Token>();
            ans.AddRange(input.Where(_ => !_.IsDebug));
            return ans;
        }
        public static List<Token> RemoveComment(this List<Token> list)
        {
            var ans = new List<Token>(list.Count);
            for (int i = 0; i < list.Count; i++)
                if (list[i].IsMemo) continue;
                else ans.Add(list[i]);
            return ans;
        }
        public static List<Token> RemoveDebug(this List<Token> list)
        {
            var ans = new List<Token>(list.Count);
            for (int i = 0; i < list.Count; i++)
                if (list[i].IsDebug) continue;
                else ans.Add(list[i]);
            return ans;
        }
    }
}