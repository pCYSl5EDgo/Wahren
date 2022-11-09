namespace Wahren.AbstractSyntaxTree.Statement;
using Expression;

public enum ConditionStatementKind
{
    If,
    Rif,
    While
}

public sealed class WhileStatement : IConditionalStatement
{
    private ArrayPoolList<IStatement> statements = new();

    public ref ArrayPoolList<IStatement> Statements => ref statements;

    public ref ArrayPoolList<IStatement> LastStatements => ref statements;

    public string DisplayName => "while";

    public void Dispose()
    {
        Condition.Dispose();

        foreach (ref var statement in Statements)
        {
            statement.Dispose();
        }

        Statements.Dispose();
    }

    public WhileStatement(uint tokenId, IReturnBooleanExpression condition)
    {
        TokenId = tokenId;
        Condition = condition;
    }

    public uint TokenId { get; set; }
    public IReturnBooleanExpression Condition { get; set; }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }

        foreach (ref var statement in statements.AsSpan())
        {
            statement.DecrementToken(indexEqualToOrGreaterThan, count);
        }

        Condition.DecrementToken(indexEqualToOrGreaterThan, count);
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }

        foreach (ref var statement in statements.AsSpan())
        {
            statement.IncrementToken(indexEqualToOrGreaterThan, count);
        }

        Condition.IncrementToken(indexEqualToOrGreaterThan, count);
    }
}

public sealed class BreakStatement : IStatement
{
    public string DisplayName => "break";

    public void Dispose()
    {
    }

    public uint TokenId { get; set; }

    public WeakReference<WhileStatement>? While { get; set; }

    public BreakStatement(uint tokenId, WhileStatement? @while)
    {
        TokenId = tokenId;
        While = @while is null ? null : new(@while);
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }
    }
}

public sealed class ContinueStatement : IStatement
{
    public string DisplayName => "continue";

    public void Dispose()
    {
    }

    public uint TokenId { get; set; }
    public WeakReference<WhileStatement>? While { get; set; }

    public ContinueStatement(uint tokenId, WhileStatement? @while)
    {
        TokenId = tokenId;
        While = @while is null ? null : new(@while);
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }
    }
}
