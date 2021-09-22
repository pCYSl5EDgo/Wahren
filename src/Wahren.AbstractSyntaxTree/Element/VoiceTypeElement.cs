namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class VoiceTypeElement(uint ElementTokenId) : IElement<List<uint>>
{
    private SingleLineRange elementKey = default;
    private uint elementScenario = uint.MaxValue;
    private List<uint> value = new();

    public ref SingleLineRange ElementKeyRange => ref elementKey;

    public ref uint ElementScenarioId => ref elementScenario;

    public ref List<uint> Value => ref value;

    public bool HasValue { get; set; }

    public List<uint> VoiceTypes = new();

    public void Dispose()
    {
        VoiceTypes.Dispose();
        value.Dispose();
    }
}
