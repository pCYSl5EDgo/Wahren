namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseContext(ref Context context, ref Result result)
    {
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

            if (!result.SplitElementPlain(tokenList.LastIndex, out var elementSpan, out var variantSpan))
            {
                return false;
            }

            if (!variantSpan.IsEmpty)
            {
                result.ErrorAdd_NoVariation("context", tokenList.LastIndex);
                return false;
            }

            var elementIndex = tokenList.LastIndex;

            if (!ReadAssign(ref context, ref result, elementIndex))
            {
                return false;
            }

            ref var elementReference = ref node.TryGet(elementSpan);
            if (Unsafe.IsNullRef(ref elementReference))
            {
                result.ErrorAdd_UnexpectedElementName(kindIndex, elementIndex);
                if (Parse_Discard_MEMBER(ref context, ref result, elementIndex))
                {
                    continue;
                }

                return false;
            }

            if (elementReference is null)
            {
                elementReference = new Pair_NullableString_NullableInt_ArrayElement(elementIndex);
                elementReference.ElementKeyLength = elementSpan.Length;

                if (Parse_Element_MEMBER(ref context, ref result, elementReference))
                {
                    continue;
                }
                
                return false;
            }
            else
            {
                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd_MultipleAssignment(elementIndex);
                }

                if (Parse_Discard_MEMBER(ref context, ref result, elementIndex))
                {
                    continue;
                }

                return false;
            }
        } while (true);
    }
}
