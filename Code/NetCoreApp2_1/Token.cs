#if NETCOREAPP2_1
using System;
using System.Numerics;
using System.Globalization;

namespace Wahren.Analysis.NetCoreApp2_1
{
    public struct Token
    {
        public Token(string file, uint line, uint column, bool isDebug, ReadOnlyMemory<char> content)
        {
            File = file;
            Line = line;
            Column = column;
            IsDebug = isDebug;
            Content = content;
        }
        public string File;
        public uint Line;
        public uint Column;
        public bool IsDebug;
        public ReadOnlyMemory<char> Content;
        public override string ToString() => string.Intern(new string(Content.Span));
        public int ToLower(Span<char> destination) => Content.Span.ToLower(destination, CultureInfo.CurrentCulture);
        public bool TryParse(out BigInteger result) => BigInteger.TryParse(Content.Span, out result);
        public long Number => long.Parse(Content.Span);
    }
}
#endif