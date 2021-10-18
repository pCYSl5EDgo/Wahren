namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseWorkspace(ref Context context, ref Result result)
    {
        ref var tokenList = ref result.TokenList;
        var kindIndex = tokenList.LastIndex;
        if (!ParseNamelessToBracketLeft(ref context, ref result, kindIndex))
        {
            return false;
        }

        ref var source = ref result.Source;
        result.WorkspaceNodeList.Add(new());
        ref var node = ref result.WorkspaceNodeList.Last;
        node.Kind = kindIndex;
        node.BracketLeft = tokenList.LastIndex;
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
            if (!result.SplitElementPlain(element.ElementTokenId, out var span, out var variantSpan))
            {
                return false;
            }

            if (!variantSpan.IsEmpty)
            {
                result.ErrorAdd_NoVariation("workspace", element.ElementTokenId);
                return false;
            }

            element.ElementKeyLength = span.Length;
            if (!ReadAssign(ref context, ref result, element.ElementTokenId))
            {
                return false;
            }

            if (!Parse_Element_MEMBER(ref context, ref result, element))
            {
                return false;
            }
            
            if (!node.Dictionary.TryAdd(span, element))
            {
                result.ErrorAdd_MultipleAssignment(element.ElementTokenId);
            }
        } while (true);
    }
}
