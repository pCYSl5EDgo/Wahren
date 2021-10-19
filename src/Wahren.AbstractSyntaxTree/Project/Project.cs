using Wahren.AbstractSyntaxTree.Statement;
using Wahren.AbstractSyntaxTree.Statement.Expression;

namespace Wahren.AbstractSyntaxTree.Project;

using Parser;

public sealed partial class Project : IDisposable
{
    public DiagnosticSeverity RequiredSeverity;
    public DisposableList<Result> Files = new();
    public DisposableList<AnalysisResult> FileAnalysisList = new();
    public System.Collections.Concurrent.ConcurrentBag<ProjectError> ErrorBag = new();

    public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_SkillSkillset = default;
    public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_UnitClassPowerSpotRace = default;

    public bool ContainsAttributeType(ReadOnlySpan<char> name)
    {
        if (name.IsEmpty)
        {
            return false;
        }

        foreach (ref var file in Files)
        {
            var node = file.AttributeNode;
            if (node is null)
            {
                continue;
            }

            ref var value = ref node.Others.TryGetRef(name);
            if (!Unsafe.IsNullRef(ref value) && value is not null)
            {
                return true;
            }
        }

        return Enum.TryParse<AttributeTypeKind>(name, out _);
    }

    private void AddReferenceAndValidate_CompoundText(ref Result result, AnalysisResult analysisResult, ref Argument argument)
    {
        if (argument.IsNumber)
        {
            return;
        }

        for (uint i = argument.TokenId + 2U, end = argument.TokenId + argument.TrailingTokenCount + 1U; i < end; ++i)
        {
            var span = result.GetSpan(i);
            if (span.Length != 1 || span[0] != '&')
            {
                continue;
            }

            span = result.GetSpan(i - 2U);
            if (span.Length != 1 || span[0] != '&')
            {
                continue;
            }

            ref var tokenList = ref result.TokenList;
            if (tokenList.GetPrecedingWhitespaceCount(i) != 0 || tokenList.GetPrecedingNewLineCount(i) != 0)
            {
                i++;
                continue;
            }
            i++;
            if (tokenList.GetPrecedingWhitespaceCount(i -2U) != 0 || tokenList.GetPrecedingNewLineCount(i - 2U) != 0)
            {
                continue;
            }
            span = result.GetSpan(i - 2U);
            if (span.IsEmpty)
            {
                continue;
            }

            static bool IsIdentifier(ReadOnlySpan<char> span)
            {
                foreach (var item in span)
                {
                    if ((item < '0' || item > '9') && (item < 'A' || item > 'Z') && (item < 'a' || item > 'z') && item != '_')
                    {
                        return false;
                    }
                }

                return true;
            }
            if (span[0] == '@')
            {
                if (span.Length == 1)
                {
                    continue;
                }

                span = span.Slice(1);
                if (span.IsEmpty || !IsIdentifier(span))
                {
                    continue;
                }

                argument.HasReference = true;
                analysisResult.StringVariableReaderSet.GetOrAdd(span, i - 2U);
                i++;
                continue;
            }
            else if (!IsIdentifier(span))
            {
                continue;
            }

            argument.HasReference = true;
            ref var reference = ref AmbiguousDictionary_UnitClassPowerSpotRace.TryGet(span);
            if (!Unsafe.IsNullRef(ref reference))
            {
                switch (reference.Kind)
                {
                    case ReferenceKind.Unit:
                        analysisResult.UnitSet.GetOrAdd(span, i - 2U);
                        continue;
                    case ReferenceKind.Class:
                        analysisResult.ClassSet.GetOrAdd(span, i - 2U);
                        continue;
                    case ReferenceKind.Power:
                        analysisResult.PowerSet.GetOrAdd(span, i - 2U);
                        continue;
                    case ReferenceKind.Spot:
                        analysisResult.SpotSet.GetOrAdd(span, i - 2U);
                        continue;
                }
            }

            analysisResult.NumberVariableReaderSet.GetOrAdd(span, i - 2U);
        }
    }

    private void AddReferenceAndValidate_Statement(ref Result result, AnalysisResult analysisResult, IStatement statement)
    {
        switch (statement)
        {
            case CallActionStatement call:
                AddReferenceAndValidate_Call(ref result, analysisResult, call);
                break;
            case WhileStatement @while:
                AddReferenceAndValidate_Condition(ref result, analysisResult, @while.Condition);
                foreach (var item in @while.Statements.AsSpan())
                {
                    AddReferenceAndValidate_Statement(ref result, analysisResult, item);
                }
                break;
            case IfStatement @if:
                AddReferenceAndValidate_Condition(ref result, analysisResult, @if.Condition);
                foreach (var item in @if.Statements.AsSpan())
                {
                    AddReferenceAndValidate_Statement(ref result, analysisResult, item);
                }
                if (@if.HasElseStatement)
                {
                    foreach (var item in @if.ElseStatements.AsSpan())
                    {
                        AddReferenceAndValidate_Statement(ref result, analysisResult, item);
                    }
                }
                break;
            case BattleStatement battle:
                foreach (var item in battle.Statements.AsSpan())
                {
                    AddReferenceAndValidate_Statement(ref result, analysisResult, item);
                }
                break;
        }
    }

    private void AddReferenceAndValidate_Condition(ref Result result, AnalysisResult analysisResult, IReturnBooleanExpression? expression)
    {
        switch (expression)
        {
            case CallFunctionExpression call:
                AddReferenceAndValidate_Call(ref result, analysisResult, call);
                break;
            case LogicOperatorExpression logic:
                AddReferenceAndValidate_Condition(ref result, analysisResult, logic.Left);
                AddReferenceAndValidate_Condition(ref result, analysisResult, logic.Right);
                break;
            case NumberComparerExpression numberCompare:
                AddReferenceAndValidate_Number(ref result, analysisResult, numberCompare.Left);
                AddReferenceAndValidate_Number(ref result, analysisResult, numberCompare.Right);
                break;
        }
    }

    private void AddReferenceAndValidate_Number(ref Result result, AnalysisResult analysisResult, IReturnNumberExpression? expression)
    {
        switch (expression)
        {
            case CallFunctionExpression call:
                AddReferenceAndValidate_Call(ref result, analysisResult, call);
                break;
            case NumberCalculatorOperatorExpression numberCalculator:
                AddReferenceAndValidate_Number(ref result, analysisResult, numberCalculator.Left);
                AddReferenceAndValidate_Number(ref result, analysisResult, numberCalculator.Right);
                break;
        }
    }
}
