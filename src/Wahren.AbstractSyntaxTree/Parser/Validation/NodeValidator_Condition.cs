namespace Wahren.AbstractSyntaxTree.Parser;
using Element.Statement;
using Element.Statement.Expression;

public static partial class NodeValidator
{
    private static void AddReferenceAndValidate(ref Context context, ref Result result, IStatement statement)
    {
        switch (statement)
        {
            case CallActionStatement call:
                if (ArgumentCountValidation(ref context, ref result, call.Kind, call.Arguments.Count, call.TokenId))
                {
                    AddReferenceAndValidate(ref context, ref result, call);
                }
                break;
            case WhileStatement @while:
                AddReferenceAndValidate(ref context, ref result, @while.Condition);
                foreach (var item in @while.Statements.AsSpan())
                {
                    AddReferenceAndValidate(ref context, ref result, item);
                }
                break;
            case IfStatement @if:
                AddReferenceAndValidate(ref context, ref result, @if.Condition);
                foreach (var item in @if.Statements.AsSpan())
                {
                    AddReferenceAndValidate(ref context, ref result, item);
                }
                if (@if.HasElseStatement)
                {
                    foreach (var item in @if.ElseStatements.AsSpan())
                    {
                        AddReferenceAndValidate(ref context, ref result, item);
                    }
                }
                break;
            case BattleStatement battle:
                foreach (var item in battle.Statements.AsSpan())
                {
                    AddReferenceAndValidate(ref context, ref result, item);
                }
                break;
        }
    }

    private static void AddReferenceAndValidate(ref Context context, ref Result result, IReturnBooleanExpression? expression)
    {
        switch (expression)
        {
            case IdentifierExpression identifier:
                identifier.ReferenceId = result.NumberVariableReaderSet.GetOrAdd(result.GetSpan(identifier.TokenId), identifier.TokenId);
                break;
            case CallFunctionExpression call:
                if (ArgumentCountValidation(ref context, ref result, call.Kind, call.Arguments.Count, call.TokenId))
                {
                    AddReferenceAndValidate(ref context, ref result, call);
                }
                break;
            case LogicOperatorExpression logic:
                AddReferenceAndValidate(ref context, ref result, logic.Left);
                AddReferenceAndValidate(ref context, ref result, logic.Right);
                break;
            case StringEqualityComparerExpression stringCompare:
                static void AddString(ref Result result, uint id, ref uint referenceId)
                {
                    var span = result.GetSpan(id);
                    if (!span.IsEmpty && span[0] == '@')
                    {
                        span = span.Slice(1);
                        if (!span.IsEmpty)
                        {
                            referenceId = result.StringVariableReaderSet.GetOrAdd(span, id);
                        }
                    }
                    else
                    {
                        result.ErrorList.Add(new($"Invalid Program Exception or Invalid Input. '{span}'", result.TokenList[id].Range));
                    }
                }
                if (stringCompare.Left is StringVariableExpression variableLeft)
                {
                    AddString(ref result, variableLeft.TokenId, ref variableLeft.ReferenceId);
                }
                if (stringCompare.Left is StringVariableExpression variableRight)
                {
                    AddString(ref result, variableRight.TokenId, ref variableRight.ReferenceId);
                }
                break;
            case NumberComparerExpression numberCompare:
                AddReference(ref context, ref result, numberCompare.Left);
                AddReference(ref context, ref result, numberCompare.Right);
                break;
        }
    }

    private static void AddReference(ref Context context, ref Result result, IReturnNumberExpression? expression)
    {
        switch (expression)
        {
            case IdentifierExpression identifier:
                identifier.ReferenceId = result.NumberVariableReaderSet.GetOrAdd(result.GetSpan(identifier.TokenId), identifier.TokenId);
                break;
            case CallFunctionExpression callFunction:
                AddReferenceAndValidate(ref context, ref result, callFunction);
                break;
            case NumberCalculatorOperatorExpression numberCalculator:
                AddReference(ref context, ref result, numberCalculator.Left);
                AddReference(ref context, ref result, numberCalculator.Right);
                break;
        }
    }
}
