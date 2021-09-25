namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseDetail(ref Context context, ref Result result)
    {
        ref var tokenList = ref result.TokenList;
        var kindIndex = tokenList.LastIndex;
        if (!ParseNamelessUbiquitousToBracketLeft(ref context, ref result, kindIndex))
        {
            return false;
        }

        ref var source = ref result.Source;
        var node = new DetailNode() { Kind = kindIndex, BracketLeft = tokenList.LastIndex };
        result.DetailNodeList.Add(node);
        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                var text = $"struct deail needs '}}' but not found.";
                result.ErrorList.Add(new(text, tokenList[node.Kind].Range));
                return false;
            }

            if (tokenList.Last.IsBracketRight(ref source))
            {
                node.BracketRight = tokenList.LastIndex;
                return true;
            }

            StringElement element = new(tokenList.LastIndex);
            if (!SplitElement(ref result, element))
            {
                return false;
            }

            if (!ReadAssign(ref context, ref result, element.ElementTokenId))
            {
                return false;
            }

            if (!Parse_Element_TEXT(ref context, ref result, element))
            {
                return false;
            }

            ReadOnlySpan<char> keySpan = source[element.ElementKeyRange.Line].AsSpan(element.ElementKeyRange.Offset, element.ElementKeyRange.Length);
            foreach (ref var itr in node.StringElementList)
            {
                if (!itr.EqualsKey(keySpan, ref result))
                {
                    continue;
                }

                ref var destination = ref itr.EnsureGet(element.ElementScenarioId);
                if (destination is null)
                {
                    destination = element;
                }
                else
                {
                    if (context.CreateError(DiagnosticSeverity.Warning))
                    {
                        result.WarningAdd_MultipleAssignment(element.ElementTokenId);
                    }
                }
                continue;
            }

            node.StringElementList.Add(new());
            ref var destinationLast = ref node.StringElementList.Last.EnsureGet(element.ElementScenarioId);
            if (destinationLast is null)
            {
                destinationLast = element;
            }
            else
            {
                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd_MultipleAssignment(element.ElementTokenId);
                }
            }
        } while (true);
    }
}
