namespace Wahren.AbstractSyntaxTree.Statement;

public sealed record class CallActionStatement(uint TokenId, ActionKind Kind) : IStatement
{
    public uint ParenRightTokenId;

    public ArrayPoolList<Argument> Arguments = new();

    public string DisplayName => "action call";

    public void Dispose()
    {
        Arguments.Dispose();
    }
}
