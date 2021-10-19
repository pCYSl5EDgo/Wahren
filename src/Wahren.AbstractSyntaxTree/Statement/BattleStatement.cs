namespace Wahren.AbstractSyntaxTree.Statement;

public sealed record class BattleStatement(uint TokenId, NextStatement? Next) : IBlockStatement
{
    private ArrayPoolList<IStatement> statements = new();

    public ref ArrayPoolList<IStatement> Statements => ref statements;

    public ref ArrayPoolList<IStatement> LastStatements => ref statements;

    public string DisplayName => "battle";

    public void Dispose()
    {
        Statements.Dispose();
    }
}
