namespace Wahren.AbstractSyntaxTree.Element.Statement;
using Expression;

public sealed record class IfStatement(uint TokenId, IReturnBooleanExpression Condition, bool IsRepeatIf) : IIfStatement
{
    private List<IStatement> statements = new();

    public ref List<IStatement> Statements => ref statements;

    public uint ElseTokenId { get; set; }
    public bool HasElseStatement { get; set; }

    private List<IStatement> elseStatements = new();

    public ref List<IStatement> ElseStatements => ref elseStatements;

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

    public ref List<IStatement> LastStatements
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
