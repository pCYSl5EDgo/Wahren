namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseAttribute(ref Context context, ref Result result, AnalysisResult analysisResult)
    {
        ref var source = ref result.Source;
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

            var element = new Pair_NullableString_NullableIntElement(tokenList.LastIndex);
            if (!SplitElement(ref result, analysisResult, element))
            {
                return false;
            }

            if (!ReadAssign(ref context, ref result, element.ElementTokenId))
            {
                return false;
            }

            if (!Parse_Element_LOYAL(ref context, ref result, element))
            {
                return false;
            }

            ref var destination = ref Specific_Attribute_GetOrAddPair(ref result, ref element.ElementKeyRange, node).EnsureGet(element.ElementScenarioId);
            if (destination is not null)
            {
                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd_MultipleAssignment(element.ElementTokenId);
                }
            }
            else
            {
                destination = element;
            }
        } while (true);
    }

    private static ref VariantPair<Pair_NullableString_NullableIntElement> Specific_Attribute_GetOrAddPair(ref Result result, ref SingleLineRange key, AttributeNode node)
    {
        if (key.IsEmpty)
        {
            return ref Unsafe.NullRef<VariantPair<Pair_NullableString_NullableIntElement>>();
        }

        var span = result.Source[key.Line].AsSpan(key.Offset, key.Length);
        var sliced = span.Slice(1);
        switch (key.Length)
        {
            case 3:
                switch (span[0])
                {
                    case 'p' when sliced.SequenceEqual("oi"):
                        return ref node.poi;
                    case 'i' when sliced.SequenceEqual("ll"):
                        return ref node.ill;
                    case 's' when sliced.SequenceEqual("il"):
                        return ref node.sil;
                }

                break;
            case 4:
                switch (span[0])
                {
                    case 'p' when sliced.SequenceEqual("ara"):
                        return ref node.para;
                    case 'c' when sliced.SequenceEqual("onf"):
                        return ref node.conf;
                    case 'f' when sliced.SequenceEqual("ear"):
                        return ref node.fear;
                    case 's' when sliced.SequenceEqual("uck"):
                        return ref node.suck;
                    case 'w' when sliced.SequenceEqual("all"):
                        return ref node.wall;
                }

                break;
            case 5:
                switch (span[0])
                {
                    case 'd':
                        if (sliced.SequenceEqual("rain"))
                        {
                            return ref node.drain;
                        }
                        else if (sliced.SequenceEqual("eath"))
                        {
                            return ref node.death;
                        }
                        break;
                    case 's' when sliced.SequenceEqual("tone"):
                        return ref node.stone;
                }

                break;
            case 7 when span.SequenceEqual("magsuck"):
                return ref node.magsuck;
        }

        foreach (ref var other in node.Others)
        {
            if (other.EqualsKey(span, ref result))
            {
                return ref other;
            }
        }

        node.Others.Add(new());
        return ref node.Others.Last;
    }
}
