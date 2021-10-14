namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class Pair_NullableString_NullableIntElement(uint ElementTokenId) : IElement<Pair_NullableString_NullableInt>
{
    private Pair_NullableString_NullableInt value = default;

    public ref Pair_NullableString_NullableInt Value => ref value;

    public int ElementKeyLength { get; set; }
    public bool HasElementVariant { get; set; }

    public bool HasValue { get; set; }
}
