using System;
using System.Collections.Generic;

namespace Wahren.Analysis.Map
{
    public static class DefaultChipReader
    {
        public static (int transparent, Dictionary<string, (int left, int top, int width, int height)> imageMap) Read(this ReadOnlySpan<byte> input)
        {
            if (input.IsEmpty) throw new ArgumentException();
            var answer = new Dictionary<string, ValueTuple<int, int, int, int>>();
            var transparent = (input[11] << 24) | (input[10] << 16) | (input[9] << 8) | input[8];
            input = input.Slice(sizeof(int)*3);
            Span<char> name = stackalloc char[256];
            Span<byte> endOfFile = stackalloc byte[8];
            for (int i = 0; i < endOfFile.Length; i++)
                endOfFile[i] = (byte)'_';
            while (true)
            {
                var nameLength = input.IndexOf<byte>(0);
                if (nameLength == -1) throw new ApplicationException();
                var nameInput = input.Slice(0, nameLength);
                if (nameInput.SequenceEqual(endOfFile))
                    break;
                for (int i = 0; i < nameInput.Length; i++)
                    name[i] = (char)nameInput[i];
                input = input.Slice(nameLength + 1);
                int left = (input[3] << 24) + (input[2] << 16) + (input[1] << 8) + input[0];
                int top = (input[7] << 24) + (input[6] << 16) + (input[5] << 8) + input[4];
                int right = (input[11] << 24) + (input[10] << 16) + (input[9] << 8) + input[8];
                int bottom = (input[15] << 24) + (input[14] << 16) + (input[13] << 8) + input[12];
                answer.Add(String.Intern(new string(name.Slice(0, nameInput.Length))), (left, top, right - left, bottom - top));
                input = input.Slice(sizeof(int) << 2);
            }
            return (transparent, answer);
        }
    }
}