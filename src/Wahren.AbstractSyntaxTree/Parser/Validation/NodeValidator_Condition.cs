namespace Wahren.AbstractSyntaxTree.Parser;
using Element.Statement;
using Element.Statement.Expression;

public static partial class NodeValidator
{
    private static void AddReference(ref Result result, IReturnBooleanExpression? expression)
    {
        switch (expression)
        {
            case IdentifierExpression identifier:
                identifier.ReferenceId = result.NumberVariableReaderSet.GetOrAdd(result.GetSpan(identifier.TokenId), identifier.TokenId);
                break;
            case CallFunctionExpression callFunction:
                AddReference(ref result, callFunction.FunctionId, ref callFunction.Arguments);
                break;
            case LogicOperatorExpression logic:
                AddReference(ref result, logic.Left);
                AddReference(ref result, logic.Right);
                break;
            case StringEqualityComparerExpression stringCompare:
                if (stringCompare.Left is StringVariableExpression variableLeft)
                {
                    variableLeft.ReferenceId = AddStringVariableReaderReference(ref result, variableLeft.TokenId);
                }
                if (stringCompare.Left is StringVariableExpression variableRight)
                {
                    variableRight.ReferenceId = AddStringVariableReaderReference(ref result, variableRight.TokenId);
                }
                break;
            case NumberComparerExpression numberCompare:
                AddReference(ref result, numberCompare.Left);
                AddReference(ref result, numberCompare.Right);
                break;
        }
    }

    private static void AddReference(ref Result result, IReturnNumberExpression? expression)
    {
        switch (expression)
        {
            case IdentifierExpression identifier:
                identifier.ReferenceId = result.NumberVariableReaderSet.GetOrAdd(result.GetSpan(identifier.TokenId), identifier.TokenId);
                break;
            case CallFunctionExpression callFunction:
                AddReference(ref result, callFunction.FunctionId, ref callFunction.Arguments);
                break;
            case NumberCalculatorOperatorExpression numberCalculator:
                AddReference(ref result, numberCalculator.Left);
                AddReference(ref result, numberCalculator.Right);
                break;
        }
    }

    private static void AddReference(ref Result result, FunctionKind kind, ref List<Argument> expression)
    {
        switch (kind)
        {
        }
    }

    private static uint AddStringVariableReaderReference(ref Result result, uint tokenId)
    {
        return 0;
    }

    private static uint AddStringVariableWriterReference(ref Result result, uint tokenId)
    {
        return 0;
    }
}
