namespace Wahren.AbstractSyntaxTree.Node;

public struct WorkspaceNode
    : INode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }

    public StringSpanKeyDictionary<Pair_NullableString_NullableInt_ArrayElement> Dictionary = new();

    public void Dispose()
    {
        Dictionary.Dispose();
    }
}
