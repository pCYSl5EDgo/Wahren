namespace Wahren.AbstractSyntaxTree.Node;

public sealed class WorkspaceNode
    : INode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }

    public StringSpanUIntKeyDictionary<Pair_NullableString_NullableInt_ArrayElement> Dictionary = new();

    public void Dispose()
    {
        Dictionary.Dispose();
    }

    public bool TryAdd(ref DualList<char> source, Pair_NullableString_NullableInt_ArrayElement element)
    {
        ref SingleLineRange elementKey = ref element.ElementKeyRange;
        return Dictionary.TryAdd(source[elementKey.Line].AsSpan(elementKey.Offset, elementKey.Length), element.ElementScenarioId, element);
    }

    public bool TryGet(ReadOnlySpan<char> key, uint variation, [NotNullWhen(true)] out Pair_NullableString_NullableInt_ArrayElement? element)
    {
        return Dictionary.TryGet(key, variation, out element);
    }
}
