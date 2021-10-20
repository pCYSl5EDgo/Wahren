namespace Wahren.AbstractSyntaxTree.Element;

public sealed class Pair_NullableString_NullableIntElement : IElement<Pair_NullableString_NullableInt>
{
    public uint ElementTokenId { get; set; }

    private Pair_NullableString_NullableInt value = default;

    public ref Pair_NullableString_NullableInt Value => ref value;

    public int ElementKeyLength { get; set; }
    public bool HasElementVariant { get; set; }

    public bool HasValue { get; set; }

    public Pair_NullableString_NullableIntElement(uint elementTokenId)
    {
        ElementTokenId = elementTokenId;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (ElementTokenId >= indexEqualToOrGreaterThan)
        {
            ElementTokenId += count;
        }

        value.IncrementToken(indexEqualToOrGreaterThan, count);
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (ElementTokenId >= indexEqualToOrGreaterThan)
        {
            ElementTokenId -= count;
        }
        
        value.DecrementToken(indexEqualToOrGreaterThan, count);
    }
}
