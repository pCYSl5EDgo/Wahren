namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class Pair_NullableString_NullableIntElement(uint ElementTokenId) : IElement<Pair_NullableString_NullableInt>
{
    private uint elementScenario = uint.MaxValue;
    private Pair_NullableString_NullableInt value = default;

    public ref uint ElementScenarioId => ref elementScenario;

    public ref Pair_NullableString_NullableInt Value => ref value;

    public uint ElementKeyLength { get; set; }
    public uint ElementVariantOffset { get; set; }
    public bool HasElementVariant { get; set; }

    public bool HasValue { get; set; }
}
