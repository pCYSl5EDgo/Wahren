namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseDetail(ref Context context, ref Result result)
    {
        ref var tokenList = ref result.TokenList;
        var kindIndex = tokenList.LastIndex;
        if (!ParseNamelessToBracketLeft(ref context, ref result, kindIndex))
        {
            return false;
        }

        var node = new DetailNode() { Kind = kindIndex, BracketLeft = tokenList.LastIndex };
        result.DetailNodeList.Add(node);
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

            if (!SplitElementPlain(ref result, tokenList.LastIndex, out var keySpan, out var variantSpan))
            {
                return false;
            }

            Pair_NullableString_NullableIntElement element = new(tokenList.LastIndex, keySpan.Length, !variantSpan.IsEmpty);
            element.ElementKeyLength = keySpan.Length;
            element.HasElementVariant = !variantSpan.IsEmpty;

            if (!ReadAssign(ref context, ref result, element.ElementTokenId))
            {
                return false;
            }

            if (!Parse_Element_TEXT(ref context, ref result, element))
            {
                return false;
            }

            foreach (ref var itr in node.StringElementList)
            {
                if (!itr.EqualsKey(keySpan, ref result))
                {
                    continue;
                }

                ref var destination = ref itr.EnsureGet(variantSpan, ref result);
                if (destination is null)
                {
                    destination = element;
                }
                else
                {
                    if (context.CreateError(DiagnosticSeverity.Warning))
                    {
                        result.ErrorAdd_MultipleAssignment(element.ElementTokenId);
                    }
                }
                continue;
            }

            node.StringElementList.Add(new());
            ref var destinationLast = ref node.StringElementList.Last.EnsureGet(variantSpan, ref result);
            if (destinationLast is null)
            {
                destinationLast = element;
            }
            else
            {
                result.ErrorAdd_MultipleAssignment(element.ElementTokenId);
            }
        } while (true);
    }
}
