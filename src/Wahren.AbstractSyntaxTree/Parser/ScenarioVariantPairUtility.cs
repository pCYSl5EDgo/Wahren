namespace Wahren.AbstractSyntaxTree.Parser;

public static class VariantPairUtility
{
    public static bool EqualsKey<T>(ref this VariantPair<T> pair, ReadOnlySpan<char> span, ref Result result)
        where T : class, IElement
    {
        uint element;
        if (pair.Value is null)
        {
            if (pair.ScenarioVariant is null)
            {
                return false;
            }

            for (int i = 0; i < pair.ScenarioVariant.Length; i++)
            {
                ref T? item = ref pair.ScenarioVariant[i];
                if (item is not null)
                {
                    element = item.ElementTokenId;
                    goto EQUALITY_COMPARE;
                }
            }

            return false;
        }
        else
        {
            element = pair.Value.ElementTokenId;
        }

    EQUALITY_COMPARE:
        return span.SequenceEqual(result.GetSpan(element));
    }

    public static ref T? EnsureGet<T>(ref this VariantPair<T> pair, uint scenario)
        where T : class, IElement
    {
        if (scenario == uint.MaxValue)
        {
            return ref pair.Value;
        }

        if (pair.ScenarioVariant is null)
        {
            pair.ScenarioVariant = ArrayPool<T?>.Shared.Rent((int)scenario + 1);
            Array.Clear(pair.ScenarioVariant);
            return ref pair.ScenarioVariant[scenario];
        }

        if (scenario >= pair.ScenarioVariant.LongLength)
        {
            var newArray = ArrayPool<T?>.Shared.Rent((int)scenario + 1);
            pair.ScenarioVariant.CopyTo(newArray.AsSpan(0, pair.ScenarioVariant.Length));
            newArray.AsSpan(pair.ScenarioVariant.Length).Clear();
            Array.Clear(pair.ScenarioVariant);
            pair.ScenarioVariant = newArray;
        }

        return ref pair.ScenarioVariant[scenario];
    }
}
