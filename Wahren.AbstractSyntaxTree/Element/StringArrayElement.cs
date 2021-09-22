namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class StringArrayElement(uint ElementTokenId) : IElement<List<uint>>
{
    private SingleLineRange elementKey = default;
    private uint elementScenario = uint.MaxValue;
    private List<uint> value = new();

    public ref SingleLineRange ElementKeyRange => ref elementKey;

    public ref uint ElementScenarioId => ref elementScenario;

    public ref List<uint> Value => ref value;

    public bool HasValue { get; set; }

    public void Dispose()
    {
        value.Dispose();
    }
}
