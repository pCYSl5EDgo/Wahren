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

        foreach (ref var item in value.AsSpan())
        {
            item.IncrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (ElementTokenId >= indexEqualToOrGreaterThan)
        {
            ElementTokenId -= count;
        }

        foreach (ref var item in value.AsSpan())
        {
            item.DecrementToken(indexEqualToOrGreaterThan, count);
        }
    }
}
