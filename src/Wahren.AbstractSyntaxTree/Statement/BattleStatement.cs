namespace Wahren.AbstractSyntaxTree.Statement;

public sealed class BattleStatement : IBlockStatement
{
    private ArrayPoolList<IStatement> statements = new();

    public ref ArrayPoolList<IStatement> Statements => ref statements;

    public ref ArrayPoolList<IStatement> LastStatements => ref statements;

    public string DisplayName => "battle";
    public uint TokenId { get; set; }
    public WeakReference<NextStatement>? Next { get; set; }

    public BattleStatement(uint tokenId, NextStatement? next)
    {
        TokenId = tokenId;
        Next = next is null ? null : new(next);
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }

        foreach (ref var statement in statements.AsSpan())
        {
            statement.IncrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }

        foreach (ref var statement in statements.AsSpan())
        {
            statement.DecrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void Dispose()
    {
        Statements.Dispose();
    }
}
