namespace Wahren.AbstractSyntaxTree.Element.Statement;
using Expression;

public interface IConditionalStatement : IBlockStatement
{
    IReturnBooleanExpression Condition { get; }
}
