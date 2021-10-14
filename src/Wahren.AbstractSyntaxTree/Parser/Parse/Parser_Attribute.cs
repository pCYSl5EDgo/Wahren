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
            if (!SplitElementPlain(ref result, elementIndex, out var span, out var variantSpan))
            {
                return false;
            }

            if (!variantSpan.IsEmpty)
            {
                result.ErrorAdd_NoVariation("attribute", elementIndex);
                return false;
            }

            if (!ReadAssign(ref context, ref result, elementIndex))
            {
                return false;
            }

            ref var elementReference = ref node.TryGet(span);
            if (Unsafe.IsNullRef(ref elementReference))
            {
                var element = new Pair_NullableString_NullableIntElement(elementIndex);
                element.ElementKeyRange.Length = (uint)span.Length;
                element.ElementKeyRange.Line = tokenList.GetLine(elementIndex);
                element.ElementKeyRange.Offset = tokenList.GetOffset(elementIndex);

                if (!Parse_Element_LOYAL(ref context, ref result, element))
                {
                    return false;
                }

                node.Others.TryAdd(span, element);
            }
            else if (elementReference is null)
            {
                elementReference = new Pair_NullableString_NullableIntElement(elementIndex);
                elementReference.ElementKeyRange.Length = (uint)span.Length;
                elementReference.ElementKeyRange.Line = tokenList.GetLine(elementIndex);
                elementReference.ElementKeyRange.Offset = tokenList.GetOffset(elementIndex);

                if (!Parse_Element_LOYAL(ref context, ref result, elementReference))
                {
                    return false;
                }
            }
            else
            {
                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd_MultipleAssignment(elementIndex);
                }

                if (!Parse_Discard_LOYAL(ref context, ref result, elementIndex))
                {
                    return false;
                }
            }
        } while (true);
    }
}
