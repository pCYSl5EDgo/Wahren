using System;
using System.Linq;
using System.Collections.Generic;

namespace Wahren.Map
{
    public sealed class Map
    {
        private readonly static ChipData[] emptyChipArray = new ChipData[0];
        private readonly ChipData[][] chips;
        private readonly byte width, height;

        public byte Width => width;

        public byte Height => height;

        private Map(byte width, byte height, IEnumerable<IEnumerable<ChipData>> data)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException();
            this.width = width;
            this.height = height;
            if (data == null)
            {
                chips = new ChipData[width * height][];
                for (int i = 0; i < chips.Length; i++)
                    chips[i] = emptyChipArray;
                return;
            }
            switch (data)
            {
                case IEnumerable<ChipData>[] array:
                    if (array.Length != width * height)
                        throw new ArgumentException();
                    chips = new ChipData[width * height][];
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (Object.ReferenceEquals(null, array[i]))
                        {
                            chips[i] = emptyChipArray;
                            continue;
                        }
                        switch (array[i])
                        {
                            case ChipData[] arrayInner:
                                chips[i] = arrayInner;
                                break;
                            default:
                                chips[i] = array[i].ToArray();
                                break;
                        }
                    }
                    break;
                case List<IEnumerable<ChipData>> list:
                    if (list.Count != width * height)
                        throw new ArgumentException();
                    chips = new ChipData[width * height][];
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (Object.ReferenceEquals(null, list[i]))
                        {
                            chips[i] = emptyChipArray;
                            continue;
                        }
                        switch (list[i])
                        {
                            case ChipData[] arrayInner:
                                chips[i] = arrayInner;
                                break;
                            default:
                                chips[i] = list[i].ToArray();
                                break;
                        }
                    }
                    break;
                default:
                    chips = new ChipData[width * height][];
                    int index = -1;
                    foreach (var item in data)
                    {
                        ++index;
                        if (Object.ReferenceEquals(null, item))
                        {
                            chips[index] = emptyChipArray;
                            continue;
                        }
                        switch (item)
                        {
                            case ChipData[] arrayInner:
                                chips[index] = arrayInner;
                                break;
                            default:
                                chips[index] = item.ToArray();
                                break;
                        }
                    }
                    if (index + 1 != chips.Length)
                        throw new ArgumentOutOfRangeException(index.ToString());
                    break;
            }
        }
        public ref ChipData[] this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                    throw new IndexOutOfRangeException(nameof(x) + " or " + nameof(y));
                return ref chips[x + y * width];
            }
        }
        public Span<ChipData[]> this[int y]
        {
            get
            {
                if (y < 0 || y >= height)
                    throw new IndexOutOfRangeException(nameof(y));
                return chips.AsSpan(y * width, width);
            }
        }

        public static bool TryLoad(ReadOnlySpan<byte> input, out byte width, out byte height, out List<(string name, byte type)>[] chips)
        {
            if (input.Length < 2)
            {
                width = height = 0;
                chips = null;
                return false;
            }
            width = input[0];
            height = input[1];
            chips = new List<(string name, byte type)>[width * height];
            input = input.Slice(2);
            Span<char> nameSpan = stackalloc char[256];
            byte x = 0, y = 0;
            while (y < height)
            {
                if (input.IsEmpty)
                    return false;
                var type = input[0];
                input = input.Slice(1);
                ref var chipList = ref chips[x + width * y];
                if (chipList == null)
                    chipList = new List<(string name, byte type)>(type == 0xff ? 0 : 4);
                if (type == 0xff)
                {
                    if (++x == width)
                    {
                        x = 0;
                        ++y;
                    }
                    continue;
                }
                var endIndex = input.IndexOfAny<byte>(0xff, 0xfe);
                if (endIndex == -1) return false;
                for (int i = 0; i < endIndex; i++)
                    nameSpan[i] = (char)input[i];
                chipList.Add((string.Intern(new string(nameSpan.Slice(0, endIndex))), type));
                input = input.Slice(endIndex + 1);
            }
            return true;
        }
    }
}