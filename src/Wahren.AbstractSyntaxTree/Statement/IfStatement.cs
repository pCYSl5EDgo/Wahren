namespace Wahren.AbstractSyntaxTree.Statement;
using Expression;

public sealed record class IfStatement(uint TokenId, IReturnBooleanExpression Condition, bool IsRepeatIf) : IIfStatement
{
    private ArrayPoolList<IStatement> statements = new();

    public ref ArrayPoolList<IStatement> Statements => ref statements;

    public uint ElseTokenId { get; set; }
    public bool HasElseStatement { get; set; }

    private ArrayPoolList<IStatement> elseStatements = new();

    public ref ArrayPoolList<IStatement> ElseStatements => ref elseStatements;

    public string DisplayName => IsRepeatIf ? "rif" : "if";

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
}
