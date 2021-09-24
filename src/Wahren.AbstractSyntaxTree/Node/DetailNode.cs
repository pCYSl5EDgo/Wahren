namespace Wahren.AbstractSyntaxTree.Node;

public struct DetailNode : INode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }

    public DisposableList<ScenarioVariantPair<StringElement>> StringElementList = new();

    public void Dispose()
    {
        StringElementList.Dispose();
    }
}
