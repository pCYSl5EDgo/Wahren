namespace Wahren.AbstractSyntaxTree.Statement;
using Expression;

public sealed class IfStatement : IIfStatement
{
    private ArrayPoolList<IStatement> statements = new();

    public ref ArrayPoolList<IStatement> Statements => ref statements;

    public uint ElseTokenId { get; set; }
    public bool HasElseStatement { get; set; }

    private ArrayPoolList<IStatement> elseStatements = new();

    public ref ArrayPoolList<IStatement> ElseStatements => ref elseStatements;

    public string DisplayName => IsRepeatIf ? "rif" : "if";

    public IfStatement(uint tokenId, IReturnBooleanExpression condition, bool isRepeatIf)
    {
        TokenId = tokenId;
        Condition = condition;
        IsRepeatIf = isRepeatIf;
    }

    public uint TokenId { get; set; }
    public IReturnBooleanExpression Condition { get; set; }
    public bool IsRepeatIf { get; set; }

    public void Dispose()
    {
        Condition.Dispose();

        foreach (ref var statement in statements)
        {
            statement.Dispose();
        }

        statements.Dispose();

        foreach (ref var statement in elseStatements)
        {
            statement.Dispose();
        }

        elseStatements.Dispose();
        HasElseStatement = false;
    }

    public ref ArrayPoolList<IStatement> LastStatements
    {
        get
        {
            if (HasElseStatement)
            {
                return ref ElseStatements;
            }
            else
            {
                return ref Statements;
            }
        }
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

        Condition.IncrementToken(indexEqualToOrGreaterThan, count);

        if (HasElseStatement)
        {
            if (ElseTokenId >= indexEqualToOrGreaterThan)
            {
                ElseTokenId += count;
            }

            foreach (ref var statement in elseStatements.AsSpan())
            {
                statement.IncrementToken(indexEqualToOrGreaterThan, count);
            }
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

        Condition.DecrementToken(indexEqualToOrGreaterThan, count);

        if (HasElseStatement)
        {
            if (ElseTokenId >= indexEqualToOrGreaterThan)
            {
                ElseTokenId -= count;
            }

            foreach (ref var statement in elseStatements.AsSpan())
            {
                statement.DecrementToken(indexEqualToOrGreaterThan, count);
            }
        }
    }
}
