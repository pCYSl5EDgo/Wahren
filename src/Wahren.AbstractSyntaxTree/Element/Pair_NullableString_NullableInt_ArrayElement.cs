namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class Pair_NullableString_NullableInt_ArrayElement(uint ElementTokenId) : IElement<List<Pair_NullableString_NullableInt>>
{
    private uint elementScenario = uint.MaxValue;
    private List<Pair_NullableString_NullableInt> value = new();

    public ref uint ElementScenarioId => ref elementScenario;

    public ref List<Pair_NullableString_NullableInt> Value => ref value;
    public uint ElementKeyLength { get; set; }
    public uint ElementVariantOffset { get; set; }
    public bool HasElementVariant { get; set; }

    public bool HasValue { get; set; }

    public void Dispose()
    {
        value.Dispose();
    }
}
