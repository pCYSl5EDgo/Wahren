namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class Pair_NullableString_NullableIntElement(uint ElementTokenId) : IElement<Pair_NullableString_NullableInt>
{
    private SingleLineRange elementKey = default;
    private uint elementScenario = uint.MaxValue;
    private Pair_NullableString_NullableInt value = default;

    public ref SingleLineRange ElementKeyRange => ref elementKey;

    public ref uint ElementScenarioId => ref elementScenario;

    public ref Pair_NullableString_NullableInt Value => ref value;

    public bool HasValue { get; set; }
}
