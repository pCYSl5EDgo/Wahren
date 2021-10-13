namespace Wahren.AbstractSyntaxTree.Parser;

public static class VariantPairUtility
{
    public static bool EqualsKey<T>(ref this VariantPair<T> pair, ReadOnlySpan<char> span, ref Result result)
        where T : class, IElement
    {
        uint element;
        int length;
        if (pair.Value is null)
        {
            if (pair.VariantArray is null)
            {
                return false;
            }

            for (int i = 0; i < pair.VariantArray.Length; i++)
            {
                ref T? item = ref pair.VariantArray[i];
                if (item is not null)
                {
                    length = (int)item.ElementKeyRange.Length;
                    element = item.ElementTokenId;
                    goto EQUALITY_COMPARE;
                }
            }

            return false;
        }
        else
        {
            length = (int)pair.Value.ElementKeyRange.Length;
            element = pair.Value.ElementTokenId;
        }

    EQUALITY_COMPARE:
        return span.SequenceEqual(result.GetSpan(element).Slice(0, length));
    }

    public static ref T? EnsureGet<T>(ref this VariantPair<T> pair, uint scenario)
        where T : class, IElement
    {
        if (scenario == uint.MaxValue)
        {
            return ref pair.Value;
        }

        if (pair.VariantArray is null)
        {
            pair.VariantArray = ArrayPool<T?>.Shared.Rent((int)scenario + 1);
            Array.Clear(pair.VariantArray);
            return ref pair.VariantArray[scenario];
        }

        if (scenario >= pair.VariantArray.LongLength)
        {
            var newArray = ArrayPool<T?>.Shared.Rent((int)scenario + 1);
            pair.VariantArray.CopyTo(newArray.AsSpan(0, pair.VariantArray.Length));
            newArray.AsSpan(pair.VariantArray.Length).Clear();
            Array.Clear(pair.VariantArray);
            pair.VariantArray = newArray;
        }

        return ref pair.VariantArray[scenario];
    }
}
