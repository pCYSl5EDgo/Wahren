using System;
using System.IO;
using System.Text;
using MessagePack;

namespace Wahren
{
    [MessagePackObject]
    public struct Token : IDebugInfo
    {
        [Key(0)]
        public readonly string File;
        [Key(1)]
        public readonly int Line;
        [Key(2)]
        public readonly int Column;
        [Key(3)]
        /// <summary>
        /// 0: 通常文字列("a_system"とか)
        /// 1: 記号
        /// 2: 数値
        /// </summary>
        public readonly byte Type;
        [Key(4)]
        public readonly bool IsDebug;
        [Key(5)]
        public readonly bool IsMemo;
        [Key(6)]
        public readonly string Content;
        [Key(7)]
        public readonly char Symbol1;
        [Key(8)]
        public readonly char Symbol2;
        [Key(9)]
        public readonly long Number;

        public override string ToString() => Type == 0 ? Content : (Type == 2 ? Number.ToString() : (Symbol2 == default(char) ? new string(new char[1] { Symbol1 }) : new string(new char[2] { Symbol1, Symbol2 })));
        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        public string DebugInfo => File + '/' + (Line + 1) + '/' + Column;

        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        private string toLowerString;

        public string ToLowerString()
        {
            if (toLowerString != null)
                return toLowerString;
            switch (Type)
            {
                case 0:
                    return toLowerString = String.Intern(Content.ToLower());
                case 1:
                    return toLowerString = Number.ToString();
                case 2:
                    if (Symbol2 == default(char))
                        return toLowerString = Symbol1.ToString();
                    else
                        return toLowerString = new string(new char[2] { Symbol1, Symbol2 });
            }
            throw new InvalidDataException();
        }
        [SerializationConstructor]
        public Token(string file, int line, int column, byte type, bool isDebug, bool isMemo, string content, char symbol1, char symbol2, long number)
        {
            File = String.Intern(file);
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = content;
            Type = type;
            Symbol1 = symbol1;
            Symbol2 = symbol2;
            Number = number;
            toLowerString = null;
        }

        public Token(string file, int line, int column, bool isDebug, bool isMemo, string content)
        {
            File = String.Intern(file);
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = content;
            Type = 0;
            Symbol1 = Symbol2 = default(char);
            Number = default(long);
            toLowerString = null;
        }
        public Token(string file, int line, int column, bool isDebug, bool isMemo, char symbol1)
        {
            File = String.Intern(file);
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = null;
            Type = 1;
            Symbol1 = symbol1;
            Symbol2 = default(char);
            Number = default(long);
            toLowerString = null;
        }
        public Token(string file, int line, int column, bool isDebug, bool isMemo, char symbol1, char symbol2)
        {
            File = String.Intern(file);
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = null;
            Type = 1;
            Symbol1 = symbol1;
            Symbol2 = symbol2;
            Number = default(long);
            toLowerString = null;
        }
        public Token(string file, int line, int column, bool isDebug, bool isMemo, long number)
        {
            File = String.Intern(file);
            Line = line;
            Column = column;
            IsDebug = isDebug;
            IsMemo = isMemo;
            Content = null;
            Type = 2;
            Symbol1 = Symbol2 = default(char);
            Number = number;
            toLowerString = null;
        }
        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        public bool IsSingleSymbol => Type == 1 && Symbol2 == default(char);
        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        public bool IsDoubleSymbol => Type == 1 && Symbol2 != default(char);

        public bool IsNext(in Token next)
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
        public bool IsOneWhiteSpaced(in Token next)
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

        public Token ForceMerge(in Token next)
        {
            if (this.Type == 1 && this.IsNext(next) && next.Type == 2)
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
        public Token Merge(in Token next)
        {
            if (IsOneWhiteSpaced(next))
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
            return ForceMerge(next);
        }
    }
}