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

            var elementIndex = tokenList.LastIndex;
            if (!result.SplitElementPlain(elementIndex, out var span, out var variantSpan))
            {
                return false;
            }

            if (!variantSpan.IsEmpty)
            {
                result.ErrorAdd_NoVariation("sound", elementIndex);
                return false;
            }

            if (!ReadAssign(ref context, ref result, elementIndex))
            {
                return false;
            }

            var element = new Pair_NullableString_NullableIntElement(elementIndex)
            {
                ElementKeyLength = span.Length,
            };
            if (!Parse_Element_DEFAULT(ref context, ref result, element))
            {
                return false;
            }

            if (!node.Others.TryAdd(span, element))
            {
                result.ErrorAdd_MultipleAssignment(element.ElementTokenId);
            }
        } while (true);
    }
}
