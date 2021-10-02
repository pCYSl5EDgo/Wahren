namespace Wahren.AbstractSyntaxTree.Statement;

public sealed record class NextStatement(uint TokenId) : IStatement
{
    public string DisplayName => "next";

    public void Dispose()
    {
    }
}
