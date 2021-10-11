namespace Wahren.AbstractSyntaxTree.Parser;
using Statement;
using Statement.Expression;

public static partial class PerResultValidator
{
    private static void AddReferenceAndValidateStatement(ref Context context, ref Result result, AnalysisResult analysisResult, IStatement statement)
    {
        switch (statement)
        {
            case CallActionStatement call:
                if (ArgumentCountValidation(ref context, ref result, call.Kind, call.Arguments.Count, call.TokenId))
                {
                    AddReferenceAndValidateCallAction(ref context, ref result, analysisResult, call);
                }
                break;
            case WhileStatement @while:
                AddReferenceAndValidateCondition(ref context, ref result, analysisResult, @while.Condition);
                foreach (var item in @while.Statements.AsSpan())
                {
                    AddReferenceAndValidateStatement(ref context, ref result, analysisResult, item);
                }
                break;
            case IfStatement @if:
                AddReferenceAndValidateCondition(ref context, ref result, analysisResult, @if.Condition);
                foreach (var item in @if.Statements.AsSpan())
                {
                    AddReferenceAndValidateStatement(ref context, ref result, analysisResult, item);
                }
                if (@if.HasElseStatement)
                {
                    foreach (var item in @if.ElseStatements.AsSpan())
                    {
                        AddReferenceAndValidateStatement(ref context, ref result, analysisResult, item);
                    }
                }
                break;
            case BattleStatement battle:
                foreach (var item in battle.Statements.AsSpan())
                {
                    AddReferenceAndValidateStatement(ref context, ref result, analysisResult, item);
                }
                break;
        }
    }

    private static void AddReferenceAndValidateCondition(ref Context context, ref Result result, AnalysisResult analysisResult, IReturnBooleanExpression? expression)
    {
        switch (expression)
        {
            case IdentifierExpression identifier:
                identifier.ReferenceId = analysisResult.NumberVariableReaderSet.GetOrAdd(result.GetSpan(identifier.TokenId), identifier.TokenId);
                break;
            case CallFunctionExpression call:
                if (ArgumentCountValidation(ref context, ref result, call.Kind, call.Arguments.Count, call.TokenId))
                {
                    AddReferenceAndValidateCallFunction(ref context, ref result, analysisResult, call);
                }
                break;
            case LogicOperatorExpression logic:
                AddReferenceAndValidateCondition(ref context, ref result, analysisResult, logic.Left);
                AddReferenceAndValidateCondition(ref context, ref result, analysisResult, logic.Right);
                break;
            case StringEqualityComparerExpression stringCompare:
                static void AddString(ref Result result, AnalysisResult analysisResult, uint id, ref uint referenceId)
                {
                    var span = result.GetSpan(id);
                    if (!span.IsEmpty && span[0] == '@')
                    {
                        span = span.Slice(1);
                        if (!span.IsEmpty)
                        {
                            referenceId = analysisResult.StringVariableReaderSet.GetOrAdd(span, id);
                        }
                    }
                    else
                    {
                        result.ErrorAdd($"Invalid Program Exception or Invalid Input. '{span}'", id);
                    }
                }
                if (stringCompare.Left is StringVariableExpression variableLeft)
                {
                    AddString(ref result, analysisResult, variableLeft.TokenId, ref variableLeft.ReferenceId);
                }
                if (stringCompare.Left is StringVariableExpression variableRight)
                {
                    AddString(ref result, analysisResult, variableRight.TokenId, ref variableRight.ReferenceId);
                }
                break;
            case NumberComparerExpression numberCompare:
                AddReferenceNumber(ref context, ref result, analysisResult, numberCompare.Left);
                AddReferenceNumber(ref context, ref result, analysisResult, numberCompare.Right);
                break;
        }
    }

    private static void AddReferenceNumber(ref Context context, ref Result result, AnalysisResult analysisResult, IReturnNumberExpression? expression)
    {
        switch (expression)
        {
            case IdentifierExpression identifier:
                identifier.ReferenceId = analysisResult.NumberVariableReaderSet.GetOrAdd(result.GetSpan(identifier.TokenId), identifier.TokenId);
                break;
            case CallFunctionExpression callFunction:
                AddReferenceAndValidateCallFunction(ref context, ref result, analysisResult, callFunction);
                break;
            case NumberCalculatorOperatorExpression numberCalculator:
                AddReferenceNumber(ref context, ref result, analysisResult, numberCalculator.Left);
                AddReferenceNumber(ref context, ref result, analysisResult, numberCalculator.Right);
                break;
        }
    }
}
