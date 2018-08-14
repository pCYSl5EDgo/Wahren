using System;
using System.Linq;
using System.Collections.Generic;

namespace Wahren.Analysis.Map
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
    }
}