namespace Wahren.AbstractSyntaxTree.Statement;

public interface IIfStatement : IConditionalStatement
{
    uint ElseTokenId { get; set; }

    bool HasElseStatement { get; set; }

    ref ArrayPoolList<IStatement> ElseStatements { get; }
}
