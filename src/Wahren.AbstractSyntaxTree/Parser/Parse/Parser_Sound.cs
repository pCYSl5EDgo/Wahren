namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseSound(ref Context context, ref Result result, AnalysisResult analysisResult)
    {
        ref var tokenList = ref result.TokenList;
        var kindIndex = tokenList.LastIndex;
        if (!ParseNamelessToBracketLeft(ref context, ref result, kindIndex))
        {
            return false;
        }

        var node = result.SoundNode = new SoundNode();
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

            var element = new Pair_NullableString_NullableIntElement(tokenList.LastIndex);
            if (!result.SplitElementPlain(element.ElementTokenId, out var span, out var variantSpan))
            {
                return false;
            }

            element.ElementScenarioId = analysisResult.ScenarioSet.GetOrAdd(variantSpan, element.ElementTokenId);
            if (element.ElementScenarioId != uint.MaxValue)
            {
                result.ErrorAdd("'@scenario' is not allowed in structure sound.", element.ElementTokenId);
                return false;
            }

            if (!ReadAssign(ref context, ref result, element.ElementTokenId))
            {
                return false;
            }

            if (!Parse_Element_DEFAULT(ref context, ref result, element))
            {
                return false;
            }

            if (!node.Others.TryAdd(span, element))
            {
                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd_MultipleAssignment(element.ElementTokenId);
                }
            }
        } while (true);
    }
}
