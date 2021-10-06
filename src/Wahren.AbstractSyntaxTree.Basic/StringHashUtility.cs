using System;

namespace Wahren.AbstractSyntaxTree;

public static class StringHashUtility
{
    public const int HashLengthMax = 12;

    public static ulong Calc(string value) => Calc(value.AsSpan());
    public static ulong Calc(ReadOnlySpan<char> value)
    {
        ulong answer = 0UL;
        if (HashLengthMax < value.Length)
        {
            value = value.Slice(0, HashLengthMax);
        }
        for (int i = 0; i < value.Length; i++)
        {
            answer *= 37;
            ulong c = value[i];
            if (c == '_')
            {
                answer += 36;
                continue;
            }
            if (c < '0')
            {
                return ulong.MaxValue;
            }
            if (c <= '9')
            {
                answer += c - '0';
                continue;
            }
            if (c < 'A')
            {
                return ulong.MaxValue;
            }
            if (c <= 'Z')
            {
                answer += c - ('A' - 10);
                continue;
            }
            if (c >= 'a' && c <= 'z')
            {
                answer += c - ('a' - 10);
                continue;
            }

            return ulong.MaxValue;
        }

        return answer;
    }
}
