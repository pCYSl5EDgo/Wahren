namespace Wahren.AbstractSyntaxTree.Statement;

public interface IBlockStatement : IStatement
{
    ref ArrayPoolList<IStatement> Statements { get; }

    ref ArrayPoolList<IStatement> LastStatements { get; }
}
