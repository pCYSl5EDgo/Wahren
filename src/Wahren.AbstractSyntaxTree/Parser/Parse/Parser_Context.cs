namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseContext(ref Context context, ref Result result, AnalysisResult analysisResult)
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        var kindIndex = tokenList.LastIndex;
        if (!ParseNamelessToBracketLeft(ref context, ref result, kindIndex))
        {
            return false;
        }

        var node = result.ContextNode = new ContextNode();
        node.Kind = tokenList.LastIndex;
        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                result.ErrorAdd_BracketRightNotFound(node.Kind);
                return false;
            }

            if (result.IsBracketRight(tokenList.LastIndex))
            {
                node.BracketRight = tokenList.LastIndex;
                return true;
            }

            var element = new Pair_NullableString_NullableInt_ArrayElement(tokenList.LastIndex);
            if (!result.SplitElement(analysisResult, element))
            {
                return false;
            }

            if (element.ElementScenarioId != uint.MaxValue)
            {
                result.ErrorAdd("'@scenario' is not allowed in structure context.", element.ElementTokenId);
                return false;
            }

            if (!ReadAssign(ref context, ref result, element.ElementTokenId))
            {
                return false;
            }

            if (!Parse_Element_MEMBER(ref context, ref result, element))
            {
                return false;
            }

            var elementSpan = source[element.ElementKeyRange.Line].AsSpan(element.ElementKeyRange.Offset, element.ElementKeyRange.Length);
            ref var elementReference = ref node.TryGet(elementSpan, out var validElement);
            if (validElement)
            {
                if (elementReference is null)
                {
                    elementReference = element;
                }
                else
                {
                    if (context.CreateError(DiagnosticSeverity.Warning))
                    {
                        result.WarningAdd_MultipleAssignment(element.ElementTokenId);
                    }
                }
            }
            else
            {
                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd_UnexpectedElementName(kindIndex, element.ElementTokenId);
                }

                node.Others.TryAdd(elementSpan, element);
            }
        } while (true);
    }
}
