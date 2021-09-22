namespace Wahren.AbstractSyntaxTree.Element.Statement;

public interface IBlockStatement : IStatement
{
    ref List<IStatement> Statements { get; }

    ref List<IStatement> LastStatements { get; }
}
