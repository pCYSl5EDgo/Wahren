global using System;
global using System.Buffers;
global using Wahren.ArrayPoolCollections;

using System.Runtime.InteropServices;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace Wahren.FileLoader;

public static unsafe class UnicodeHandler
{
    public static void LoadFromString(ReadOnlySpan<char> content, out DualList<char> source)
    {
        source = new DualList<char>();
        source.AddEmpty();

        if (content.IsEmpty)
        {
            return;
        }

        do
        {
            var linefeedIndex = content.IndexOf('\n');
            ref var lastLine = ref source.Last;
            if (linefeedIndex == -1)
            {
                lastLine.AddRange(content);
                return;
            }

            lastLine.AddRange(content.Slice(0, linefeedIndex - ((linefeedIndex > 0 && content[linefeedIndex - 1] == '\r') ? 1 : 0)));
            source.AddEmpty();
            content = content.Slice(linefeedIndex + 1);
        } while (!content.IsEmpty);
    }

    public static void Load(Span<byte> content, out DualList<char> source)
    {
        source = new DualList<char>();
        source.AddEmpty();

        if (content.Length >= 32 && content[0] == 1 && content[1] == 2 && content[2] == 3 && content[3] == 4)
        {
            CryptUtility.Decrypt(content.Slice(4, 28), content.Slice(32));
            content = content.Slice(32);
        }

        if (content.IsEmpty)
        {
            return;
        }

        var charSpan = MemoryMarshal.Cast<byte, char>(content);
        do
        {
            var linefeedIndex = charSpan.IndexOf('\n');
            ref var lastLine = ref source.Last;
            if (linefeedIndex == -1)
            {
                lastLine.AddRange(charSpan);
                return;
            }

            lastLine.AddRange(charSpan.Slice(0, linefeedIndex - ((linefeedIndex > 0 && charSpan[linefeedIndex - 1] == '\r') ? 1 : 0)));
            source.AddEmpty();
            charSpan = charSpan.Slice(linefeedIndex + 1);
        } while (!charSpan.IsEmpty);
    }
}
