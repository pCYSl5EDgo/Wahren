namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseAttribute(ref Context context, ref Result result)
    {
        ref var tokenList = ref result.TokenList;
        var kindIndex = tokenList.LastIndex;
        if (!ParseNamelessToBracketLeft(ref context, ref result, kindIndex))
        {
            return false;
        }

        var node = result.AttributeNode = new AttributeNode();
        node.Kind = kindIndex;

        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                result.ErrorAdd_BracketRightNotFound(kindIndex);
                return false;
            }

            if (result.IsBracketRight(tokenList.LastIndex))
            {
                node.BracketRight = tokenList.LastIndex;
                return true;
            }

            var elementIndex = tokenList.LastIndex;
            var span = result.GetSpan(elementIndex);
            if (span.IsEmpty)
            {
                return false;
            }

            if (span[0] == '@')
            {
                if (span.Length == 1 || span[span.Length - 1] != '@')
                {
                    result.ErrorAdd_NoVariation("attribute", elementIndex);
                    return false;
                }

                span = span.Slice(1, span.Length - 2);
                if (!ReadAssign(ref context, ref result, elementIndex))
                {
                    return false;
                }

                ref var elementReference = ref node.Hides.TryGetRef(span);
                if (Unsafe.IsNullRef(ref elementReference))
                {
                    var element = new Pair_NullableString_NullableIntElement(elementIndex);
                    element.ElementKeyLength = span.Length;
                    if (!Parse_Element_DEFAULT(ref context, ref result, element))
                    {
                        return false;
                    }
                    node.Hides.TryAdd(span, element);
                }
                else if (elementReference is null)
                {
                    elementReference = new(elementIndex);
                    if (!Parse_Element_DEFAULT(ref context, ref result, elementReference))
                    {
                        return false;
                    }
                }
                else
                {
                    result.ErrorAdd_MultipleAssignment(elementIndex);
                    if (!Parse_Discard_DEFAULT(ref context, ref result, elementIndex))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!ReadAssign(ref context, ref result, elementIndex))
                {
                    return false;
                }

                ref var elementReference = ref node.TryGet(span);
                if (Unsafe.IsNullRef(ref elementReference))
                {
                    var element = new Pair_NullableString_NullableIntElement(elementIndex);
                    element.ElementKeyLength = span.Length;

                    if (!Parse_Element_LOYAL(ref context, ref result, element))
                    {
                        return false;
                    }

                    node.Others.TryAdd(span, element);
                }
                else if (elementReference is null)
                {
                    elementReference = new Pair_NullableString_NullableIntElement(elementIndex);
                    elementReference.ElementKeyLength = span.Length;

                    if (!Parse_Element_LOYAL(ref context, ref result, elementReference))
                    {
                        return false;
                    }
                }
                else
                {
                    result.ErrorAdd_MultipleAssignment(elementIndex);

                    if (!Parse_Discard_LOYAL(ref context, ref result, elementIndex))
                    {
                        return false;
                    }
                }
            }
        } while (true);
    }
}
