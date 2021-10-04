namespace Wahren.FileLoader;

public static unsafe class AsciiCompatibleHandler
{
    public static void Load(Span<byte> content, List<char>.AddConverter<byte> GetChars, out DualList<char> source)
    {
        source = new DualList<char>();
        source.AddEmpty();

        if (content.Length >= 32 && content[0] == 1 && content[1] == 2 && content[2] == 3 && content[3] == 4)
        {
            CryptUtility.Decrypt(content.Slice(4, 28), content.Slice(32));
            content = content.Slice(32);
        }

        ReadOnlySpan<byte> span = content;
        while (!span.IsEmpty)
        {
            ref var line = ref source.Last();
            var linefeedIndex = span.IndexOf((byte)'\n');
            if (linefeedIndex == 0)
            {
                span = span.Slice(1);
                source.AddEmpty();
                continue;
            }

            if (linefeedIndex == -1)
            {
                line.PrepareAddRange(span.Length << 2);
                line.AddRangeConversion(GetChars, span);
                break;
            }

            var oldSpan = span.Slice(0, linefeedIndex - (span[linefeedIndex - 1] == (byte)'\r' ? 1 : 0));
            if (!oldSpan.IsEmpty)
            {
                line.PrepareAddRange(oldSpan.Length << 2);
                line.AddRangeConversion(GetChars, oldSpan);
            }

            span = span.Slice(linefeedIndex + 1);
            source.AddEmpty();
        }
    }
}
