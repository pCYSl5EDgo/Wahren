using System;
using System.Text;

namespace Wahren.Unity
{
    public static class StringModifier
    {
        private static readonly StringBuilder buf = new StringBuilder();
        public static string Escape(this string input)
        {
            buf.Clear();
            var span = input.AsSpan();
            while (true)
            {
                var i = span.IndexOfAny('"', '\\');
                if (i == -1)
                {
                    buf.Append(span);
                    break;
                }
                buf.Append(span.Slice(0, i));
                switch (span[i])
                {
                    case '"':
                        buf.Append("\\\"");
                        break;
                    case '\\':
                        buf.Append("\\\\");
                        break;
                }
                span = span.Slice(i + 1);
            }
            return buf.ToString();
        }
        public static string StringEscape(this string input)
        {
            buf.Clear();
            var span = input.AsSpan();
            while (true)
            {
                var i = span.IndexOfAny('"', '$', '\\');
                var j = span.IndexOf('\n');
                if (i == -1 && j == -1)
                {
                    buf.Append(span);
                    break;
                }
                var min = i == -1 ? j : (j == -1 ? i : (i < j ? i : j));
                buf.Append(span.Slice(0, min));
                switch (span[min])
                {
                    case '\n':
                        break;
                    case '"':
                        buf.Append("\\\"");
                        break;
                    case '\\':
                        buf.Append("\\\\");
                        break;
                    default:
                        buf.Append("\\n");
                        break;
                }
                span = span.Slice(min + 1);
            }
            return buf.ToString();
        }
    }
}