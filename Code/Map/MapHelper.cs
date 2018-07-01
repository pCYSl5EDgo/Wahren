using System;
using System.IO;
using System.Collections.Generic;

namespace Wahren.Map
{
    public static partial class MapHelper
    {
        private const string COLLIDER = "________";
        private static readonly (string, byte)[] empty = new(string, byte)[0];
        public static bool TryLoad(string file, out byte width, out byte height, out (string name, byte type)[][] chips)
        {
            if (!File.Exists(file))
            {
                width = height = default;
                chips = default;
                return false;
            }
            Span<byte> input;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false))
            {
                input = new byte[fs.Length];
                fs.Read(input);
            }
            return TryLoad(input, out width, out height, out chips);
        }
        public static bool TryLoad(ReadOnlySpan<byte> input, out byte width, out byte height, out (string name, byte type)[][] chips)
        {
            if (input.Length < 2)
            {
                width = height = 0;
                chips = null;
                return false;
            }
            width = input[0];
            height = input[1];
            chips = new(string, byte)[width * height][];
            var tmpList = new List<(string, byte)>(4);
            input = input.Slice(2);
            Span<char> nameSpan = stackalloc char[256];
            byte x = 0, y = 0;
            while (y < height)
            {
                if (input.IsEmpty)
                    return false;
                var type = input[0];
                input = input.Slice(1);
                if (type == 0xff)
                {
                    chips[x + width * y] = tmpList.Count == 0 ? empty : tmpList.ToArray();
                    tmpList.Clear();
                    if (++x == width)
                    {
                        x = 0;
                        ++y;
                    }
                    continue;
                }
                var endIndex = input.IndexOfAny<byte>(0xff, 0xfe);
                if (endIndex == -1 || endIndex > nameSpan.Length) return false;
                for (int i = 0; i < endIndex; i++)
                    nameSpan[i] = (char)input[i];
                tmpList.Add((string.Intern(new string(nameSpan.Slice(0, endIndex))), type));
                input = input.Slice(endIndex + 1);
            }
            return true;
        }
        public static HashSet<string> FieldAttributeSet(this IEnumerable<FieldData> collection)
        {
            var answer = new HashSet<string>();
            foreach (var item in collection)
                answer.Add(item.Attribute);
            return answer;
        }
        public static ushort[] MoveList(this List<int> validIndexes, SortedDictionary<uint, double> distances)
        {
            int count = validIndexes.Count;
            var answer = new ushort[count * count];
            Array.Fill(answer, ushort.MaxValue);
            for (int i = 0; i < count; i++)
                answer[i * count + i] = 0;
            foreach (var (pair, distance) in distances)
            {
                var x = (ushort)(pair >> 16);
                var y = (ushort)(pair & 0xffff);
            }
            return answer;
        }
        public static SortedDictionary<uint, double> DistanceList(this (string, int)[] array, int width, MoveTypeData moveType, out List<int> validIndexes)
        {
            var root2 = Math.Sqrt(2);
            var height = array.Length / width;
            // 2x2 => 6
            // 4x4 => 3(width-1)*4(height) + 3(height)*4(width) + 2 * 3(width-1)*3(height-1) => (w-1)h+w(h-1)+2(w-1)(h-1)=>4wh-3w-3h+2
            // .-.-.-.
            // |x|x|x|
            // .-.-.-.
            // |x|x|x|
            // .-.-.-.
            // |x|x|x|
            // .-.-.-.
            var answer = new SortedDictionary<uint, double>();
            validIndexes = new List<int>(array.Length);
            var difficulties = new ulong[array.Length];
            for (int i = 0; i < difficulties.Length; i++)
            {
                ref var cell1 = ref array[i];
                if (!moveType.FieldMoveDictionary.TryGetValue(cell1.Item1, out var difficulty))
                    difficulty = 5;
                difficulties[i] = LeastCommonMultipleRange(10) / difficulty;
            }
            double GetDistance(int a, int h_a, int b, int h_b, bool isOblique)
            {
                var _ = difficulties[a] + difficulties[b];
                long __ = h_b - h_a;
                return Math.Sqrt((isOblique ? 2ul : 1ul) * _ * _ + (ulong)(__ * __));
            }
            void GetTuple(int index1, int index2, bool isOblique)
            {
                ref var cell2 = ref array[index2];
                if (cell2.Item1 == MapHelper.COLLIDER)
                    return;
                answer.Add(((ushort)((index1 << 16) | index2)), GetDistance(index2, array[index1].Item2, index2, cell2.Item2, isOblique));
            }
            ushort j = 0;
            ushort k = 0;
            for (ushort i = 0; i < array.Length - 1; j++, i++)
            {
                ref var cell1 = ref array[i];
                if (cell1.Item1 == MapHelper.COLLIDER)
                    continue;
                validIndexes.Add(i);
                if (j == width)
                {
                    j = 0;
                    ++k;
                }
                if (k == height - 1)
                {
                    GetTuple(i, i + 1, false);
                    continue;
                }
                if (j == 0)
                {
                    GetTuple(i, i + 1, false);
                    GetTuple(i, i + width, false);
                    try
                    {
                        GetTuple(i, i + 1 + width, true);
                    }
                    catch
                    {
                        System.Console.Error.WriteLine(width);
                        System.Console.Error.WriteLine(height);
                        System.Console.Error.WriteLine(k);
                        System.Console.Error.WriteLine(i);
                        System.Console.Error.WriteLine(i + 1 + width);
                        System.Console.Error.WriteLine(array.Length);
                    }
                }
                else if (j == width - 1)
                {
                    GetTuple(i, i - 1 + width, true);
                    GetTuple(i, i + width, false);
                }
                else
                {
                    GetTuple(i, i + 1, false);
                    GetTuple(i, i - 1 + width, true);
                    GetTuple(i, i + width, false);
                    GetTuple(i, i + 1 + width, true);
                }
            }
            if (array[array.Length - 1].Item1 != COLLIDER)
                validIndexes.Add(array.Length - 1);
            return answer;
        }
        public static (string, int)[] ConvertToFieldAttributeAndHeightArray(this (string name, byte type)[][] array)
        {
            if (array == null) throw new ArgumentNullException();
            var answer = new(string, int)[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                ref var ans = ref answer[i];
                ref var cell = ref array[i];
                for (int j = 0; j < cell.Length; j++)
                {
                    ref var chip = ref cell[j];
                    switch (chip.type)
                    {
                        case 0:
                            if (ScriptLoader.FieldDictionary.TryGetValue(chip.name, out var fieldData))
                                ans = (fieldData.Type == FieldData.ChipType.None ? fieldData.Attribute : COLLIDER, fieldData.Alt_min ?? 0);
                            else throw new Exception("index:" + i + ',' + j + "\nname:" + chip.name);
                            break;
                        case 1:
                            if (ScriptLoader.ObjectDictionary.TryGetValue(chip.name, out var objectData))
                            {
                                switch (objectData.Type)
                                {
                                    case ObjectData.ChipType.coll:
                                    case ObjectData.ChipType.wall:
                                    case ObjectData.ChipType.wall2:
                                        ans = (COLLIDER, ans.Item2);
                                        break;
                                }
                            }
                            else throw new Exception("index:" + i + ',' + j + "\nname:" + chip.name);
                            break;
                        default:
                            continue;
                    }
                }
            }
            return answer;
        }
        public static ulong GreatestCommonDivisor(ulong num0, ulong num1)
        {
            if (num0 == num1) return num0;
            else if (num0 == 0) return num1;
            else if (num1 == 0) return num0;
            ulong min, max, tmp;
            if (num0 < num1)
            {
                min = num0;
                max = num1;
            }
            else
            {
                min = num1;
                max = num0;
            }
            while (min != 0)
            {
                tmp = max / min;
                max = min;
                min = tmp;
            }
            return max;
        }
        public static ulong LeastCommonMultiple(ulong num0, ulong num1) => num0 * num1 / GreatestCommonDivisor(num0, num1);
        private static readonly List<ulong> leastCommonMultipleRangeList = new List<ulong>(){
            1         ,2         ,2*3          ,4*3          ,4*3*5        ,4*3*5         ,4*3*5*7          ,8*3*5*7          ,8*9*5*7             ,8*5*9*7,
            //8*5*9*7*11,8*5*9*7*11,8*5*9*7*11*13,8*5*9*7*11*13,8*5*9*7*11*13,16*5*9*7*11*13,16*5*9*7*11*13*17,16*5*9*7*11*13*17,16*5*9*7*11*13*17*19,16*5*9*7*11*13*17*19,
        };
        public static ulong LeastCommonMultipleRange(byte number)
        {
            if (number <= leastCommonMultipleRangeList.Count)
                return leastCommonMultipleRangeList[number - 1];
            ulong answer = leastCommonMultipleRangeList[leastCommonMultipleRangeList.Count - 1];
            for (byte i = (byte)leastCommonMultipleRangeList.Count; i < number; i++)
                leastCommonMultipleRangeList.Add(answer = LeastCommonMultiple(answer, i));
            return answer;
        }
    }
}