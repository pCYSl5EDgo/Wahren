namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class StringElement(uint ElementTokenId) : IElement<uint>
{
    private SingleLineRange elementKey = default;
    private uint elementScenario = uint.MaxValue;
    private uint value = default;

    public ref SingleLineRange ElementKeyRange => ref elementKey;

    public ref uint ElementScenarioId => ref elementScenario;

    public ref uint Value => ref value;

    public bool HasValue { get; set; }
}
