﻿namespace Wahren.AbstractSyntaxTree.Statement;
using Expression;

public enum ConditionStatementKind
{
    If,
    Rif,
    While
}

public sealed record class WhileStatement(uint TokenId, IReturnBooleanExpression Condition) : IConditionalStatement
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
}

public sealed record class BreakStatement(uint TokenId, WhileStatement? While) : IStatement
{
    public string DisplayName => "break";

    public void Dispose()
    {
    }
}

public sealed record class ContinueStatement(uint TokenId, WhileStatement? While) : IStatement
{
    public string DisplayName => "continue";

    public void Dispose()
    {
    }
}
