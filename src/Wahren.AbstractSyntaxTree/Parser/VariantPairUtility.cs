namespace Wahren.AbstractSyntaxTree.Parser;

public static class VariantPairUtility
{
    public static bool EqualsKey<T>(ref this VariantPair<T> pair, ReadOnlySpan<char> span, ref Result result)
        where T : class, IElement
    {
        if (pair.Value is not null)
        {
            return span.SequenceEqual(result.GetSpan(pair.Value.ElementTokenId).Slice(0, pair.Value.ElementKeyLength));
        }

        if (pair.Count == 0)
        {
            return false;
        }

        ref var item = ref pair.VariantArray[0];
        return span.SequenceEqual(result.GetSpan(item.ElementTokenId).Slice(0, item.ElementKeyLength));
    }

    public static ref T? EnsureGet<T>(ref this VariantPair<T> pair, ReadOnlySpan<char> variantSpan, ref Result result)
        where T : class, IElement
    {
        ref T? answer = ref pair.Value;
        if (variantSpan.IsEmpty)
        {
            return ref answer;
        }

        var hash = StringHashUtility.Calc(variantSpan);
        pair.EnsureCapacity();
        if (pair.Count == 0)
        {
            pair.HashArray[0] = hash;
            ++pair.Count;
            answer = ref pair.VariantArray[0]!;
            answer = null;
            return ref answer;
        }

        var index = pair.HashSpan.BinarySearch(hash);
        var variants = pair.Variants;
        int i;
        if (index >= 0)
        {
            answer = ref variants[index]!;
            if (variantSpan.Length < StringHashUtility.HashLengthMax || variantSpan.SequenceEqual(result.GetSpan(answer.ElementTokenId).Slice(answer.ElementKeyLength + 1)))
            {
                return ref answer;
            }

            for (i = index - 1; i >= 0 && pair.HashArray[i] == hash; --i)
            {
                answer = ref variants[i]!;
                if (variantSpan.SequenceEqual(result.GetSpan(answer.ElementTokenId).Slice(answer.ElementKeyLength + 1)))
                {
                    return ref answer;
                }
            }

            for (i = index + 1; i < variants.Length && pair.HashArray[i] == hash; ++i)
            {
                answer = ref variants[i]!;
                if (variantSpan.SequenceEqual(result.GetSpan(answer.ElementTokenId).Slice(answer.ElementKeyLength + 1)))
                {
                    return ref answer;
                }
            }
        }
        else
        {
            i = ~index;
        }

        variants.Slice(i).CopyTo(pair.VariantArray.AsSpan(i + 1));
        pair.HashArray.AsSpan(i, pair.Count - i).CopyTo(pair.HashArray.AsSpan(i + 1));
        pair.HashArray[i] = hash;
        ++pair.Count;
        answer = ref pair.VariantArray[i]!;
        answer = null;
        return ref answer;
    }
}
