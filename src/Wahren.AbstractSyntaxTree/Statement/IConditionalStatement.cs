namespace Wahren.AbstractSyntaxTree.Statement;
using Expression;

public interface IConditionalStatement : IBlockStatement
{
    IReturnBooleanExpression Condition { get; }
}
