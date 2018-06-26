using System;
using System.IO;
using System.Collections.Generic;

namespace Wahren.Map
{
    public static class MapHelper
    {
        private const string COLLIDER = "____coll____";
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
        public static string[] ConvertToMoveTypeArray(this (string name, byte type)[][] array)
        {
            if (array == null) throw new ArgumentNullException();
            var answer = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                ref var cell = ref array[i];
                for (int j = 0; j < cell.Length; j++)
                {
                    if (!Object.ReferenceEquals(answer[i], null))
                        break;
                    ref var chip = ref cell[j];
                    switch (chip.type)
                    {
                        case 0:
                            if (ScriptLoader.FieldDictionary.TryGetValue(chip.name, out var fieldData))
                            {
                                if (fieldData.Type == FieldData.ChipType.None)
                                    answer[i] = fieldData.Attribute;
                                else answer[i] = COLLIDER;
                            }
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
                                        answer[i] = COLLIDER;
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