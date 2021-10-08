namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseSound(ref Context context, ref Result result)
    {
        ref var tokenList = ref result.TokenList;
        var kindIndex = tokenList.LastIndex;
        if (!ParseNamelessToBracketLeft(ref context, ref result, kindIndex))
        {
            return false;
        }

        ref var source = ref result.Source;
        var node = result.SoundNode = new SoundNode();
        node.Kind = tokenList.LastIndex;
        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                result.ErrorAdd_BracketRightNotFound(node.Kind);
                return false;
            }

            ref var last = ref tokenList.Last;
            if (last.IsBracketRight(ref source))
            {
                node.BracketRight = tokenList.LastIndex;
                return true;
            }

            var element = new Pair_NullableString_NullableIntElement(tokenList.LastIndex);
            if (!result.SplitElement(element))
            {
                return false;
            }

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

            var elementSpan = source[element.ElementKeyRange.Line].AsSpan(element.ElementKeyRange.Offset, element.ElementKeyRange.Length);
            if (!node.Others.TryAdd(elementSpan, element))
            {
                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd_MultipleAssignment(element.ElementTokenId);
                }
            }
        } while (true);
    }
}
