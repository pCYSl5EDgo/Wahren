namespace Wahren.AbstractSyntaxTree.Element.Statement;

public sealed record class CallActionStatement(uint TokenId, ActionKind Kind) : IStatement
{
    public uint ParenRightTokenId;

    public List<Argument> ArgumentTokenIds = new();

    public string DisplayName => "action call";

    public void Dispose()
    {
        ArgumentTokenIds.Dispose();
    }
}
