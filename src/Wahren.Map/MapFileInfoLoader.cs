using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Wahren.PooledList;

namespace Wahren.Map;

public static class MapFileInfoLoader
{
    public static bool TryParse(ReadOnlySpan<byte> content, ref DualList<ChipData> board, ref StringSpanKeySlowSet set)
    {
        if (content.Length < 2)
        {
            return false;
        }

        int width = content[0];
        int height = content[1];
        content = content.Slice(2);
        Span<char> nameSpan = stackalloc char[256];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (content.Length < 3)
                {
                    return false;
                }
                board.AddEmpty();
                ref var list = ref board.Last;
                do
                {
                    var zOrder = content[0];

                    if (zOrder == 0xff)
                    {
                        break;
                    }

                    content = content.Slice(1);
                    var nameEnd = content.IndexOf<byte>(0xfe);
                    if (nameEnd > 255)
                    {
                        return false;
                    }
                    var nameBytes = content.Slice(0, nameEnd);
                    for (int i = 0; i < nameBytes.Length; i++)
                    {
                        nameSpan[i] = (char)nameBytes[i];
                    }
                    content = content.Slice(nameEnd + 1);
                    var nameId = set.GetOrAdd(nameSpan, ((uint)x << 16) | ((uint)y << 8) | ((uint)list.Count));
                    list.Add(new(zOrder, nameId));
                } while (true);
            }
        }

        return true;
    }
}

public record struct ChipData(byte ZOrder, uint NameId)
{
}

public record struct ChipPosition(byte X, byte Y, byte Index)
{
}