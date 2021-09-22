namespace Wahren.AbstractSyntaxTree.Element.Statement;

public sealed record class BattleStatement(uint TokenId, NextStatement? Next) : IBlockStatement
{
    private List<IStatement> statements = new();

    public ref List<IStatement> Statements => ref statements;

    public ref List<IStatement> LastStatements => ref statements;

    public string DisplayName => "battle";

    public void Dispose()
    {
        Statements.Dispose();
    }
}
