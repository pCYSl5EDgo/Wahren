using System;

namespace Wahren.Map;

public static class MapFileInfoLoader
{
    public static bool TryParse(ReadOnlySpan<byte> content, ref MapInfo mapInfo)
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
                mapInfo.Board.AddEmpty();
                ref var list = ref mapInfo.Board.Last;
                do
                {
                    var zOrder = content[0];
                    content = content.Slice(1);

                    if (zOrder == 0xff)
                    {
                        break;
                    }

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
                    var nameId = mapInfo.NameSet.GetOrAdd(nameSpan.Slice(0, nameBytes.Length), ((uint)x << 16) | ((uint)y << 8) | ((uint)list.Count));
                    list.Add(new(zOrder, nameId));
                } while (true);
            }
        }

        mapInfo.Width = (byte)width;
        mapInfo.Height = (byte)height;
        return true;
    }
}
