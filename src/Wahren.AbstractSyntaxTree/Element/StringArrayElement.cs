namespace Wahren.AbstractSyntaxTree.Element;

public sealed record class StringArrayElement(uint ElementTokenId) : IElement<List<StringArrayElement.StringValue>>
{
    public struct StringValue
    {
        public uint Text;
        public uint ReferenceId;
        public ReferenceKind ReferenceKind;
        public bool HasReference;

        public StringValue(uint textTokenId)
        {
            Text = textTokenId;
            ReferenceId = default;
            ReferenceKind = default;
            HasReference = false;
        }
    }

    private SingleLineRange elementKey = default;
    private uint elementScenario = uint.MaxValue;
    private List<StringValue> value = new();

    public ref SingleLineRange ElementKeyRange => ref elementKey;

    public ref uint ElementScenarioId => ref elementScenario;

    public ref List<StringValue> Value => ref value;

    public bool HasValue { get; set; }

    public void Dispose()
    {
        value.Dispose();
    }
}
