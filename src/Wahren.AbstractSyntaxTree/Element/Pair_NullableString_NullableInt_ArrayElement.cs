namespace Wahren.AbstractSyntaxTree.Element;

public sealed class Pair_NullableString_NullableInt_ArrayElement : IElement<ArrayPoolList<Pair_NullableString_NullableInt>>
{
    public uint ElementTokenId { get; set; }
    private ArrayPoolList<Pair_NullableString_NullableInt> value = new();

    public ref ArrayPoolList<Pair_NullableString_NullableInt> Value => ref value;
    public int ElementKeyLength { get; set; }
    public bool HasElementVariant { get; set; }

    public bool HasValue { get; set; }

    public Pair_NullableString_NullableInt_ArrayElement(uint elementTokenId)
    {
        ElementTokenId = elementTokenId;
    }

    public void Dispose()
    {
        value.Dispose();
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (ElementTokenId >= indexEqualToOrGreaterThan)
        {
            ElementTokenId += count;
        }

        var span = value.AsSpan();
        if (span.IsEmpty)
        {
            return;
        }

        span[0].IncrementToken(indexEqualToOrGreaterThan, count);
        for (int i = 1; i < span.Length; i++)
        {
            ref var prev = ref span[i - 1];
            ref var item = ref span[i];
            if (prev.Text == item.Text)
            {
                continue;
            }

            item.IncrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (ElementTokenId >= indexEqualToOrGreaterThan)
        {
            ElementTokenId -= count;
        }

        var span = value.AsSpan();
        if (span.IsEmpty)
        {
            return;
        }

        span[0].DecrementToken(indexEqualToOrGreaterThan, count);
        for (int i = 1; i < span.Length; i++)
        {
            ref var prev = ref span[i - 1];
            ref var item = ref span[i];
            if (prev.Text == item.Text)
            {
                continue;
            }

            item.DecrementToken(indexEqualToOrGreaterThan, count);
        }
    }
}
