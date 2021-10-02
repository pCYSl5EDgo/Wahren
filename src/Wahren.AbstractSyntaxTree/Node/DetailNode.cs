namespace Wahren.AbstractSyntaxTree.Node;

public struct DetailNode : INode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }

    public DisposableList<VariantPair<Pair_NullableString_NullableIntElement>> StringElementList = new();

    public void Dispose()
    {
        StringElementList.Dispose();
    }
}
