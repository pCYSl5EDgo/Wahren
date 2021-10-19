namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class Pair_NullableString_NullableInt_ArrayElement(uint ElementTokenId) : IElement<ArrayPoolList<Pair_NullableString_NullableInt>>
{
    private ArrayPoolList<Pair_NullableString_NullableInt> value = new();

    public ref ArrayPoolList<Pair_NullableString_NullableInt> Value => ref value;
    public int ElementKeyLength { get; set; }
    public bool HasElementVariant { get; set; }

    public bool HasValue { get; set; }

    public void Dispose()
    {
        value.Dispose();
    }
}
