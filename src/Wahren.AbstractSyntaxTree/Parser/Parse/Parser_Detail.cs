﻿namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseDetail(ref Context context, ref Result result, AnalysisResult analysisResult)
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

            Pair_NullableString_NullableIntElement element = new(tokenList.LastIndex);
            if (!SplitElementPlain(ref result, element.ElementTokenId, out var keySpan, out var variantSpan))
            {
                return false;
            }

            element.ElementKeyRange.Length = (uint)keySpan.Length;
            element.ElementKeyRange.Line = tokenList.GetLine(element.ElementTokenId);
            element.ElementKeyRange.Offset = tokenList.GetOffset(element.ElementTokenId);
            element.ElementScenarioId = analysisResult.ScenarioSet.GetOrAdd(variantSpan, element.ElementTokenId);

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
