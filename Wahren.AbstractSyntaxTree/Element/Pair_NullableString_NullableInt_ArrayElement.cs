namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class Pair_NullableString_NullableInt_ArrayElement(uint ElementTokenId) : IElement<List<Pair_NullableString_NullableInt>>
{
    private SingleLineRange elementKey = default;
    private uint elementScenario = uint.MaxValue;
    private List<Pair_NullableString_NullableInt> value = new();

    public ref SingleLineRange ElementKeyRange => ref elementKey;

    public ref uint ElementScenarioId => ref elementScenario;

    public ref List<Pair_NullableString_NullableInt> Value => ref value;

    public bool HasValue { get; set; }

    public void Dispose()
    {
        value.Dispose();
    }
}
