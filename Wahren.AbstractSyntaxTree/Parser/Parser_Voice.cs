namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseVoice(ref Context context, ref Result result)
    {
        ref var source = ref result.Source;
        var node = new VoiceNode();
        result.NodeList.Add(node);
        ref var tokenList = ref result.TokenList;
        node.Kind = tokenList.LastIndex;
        if (!ParseNameAndSuperAndBracketLeft(ref context, ref result, node))
        {
            return false;
        }

        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                result.ErrorAdd_BracketRightNotFound(node.Kind, node.Name);
                return false;
            }

            var currentIndex = tokenList.LastIndex;
            if (tokenList.Last.IsBracketRight(ref source))
            {
                node.BracketRight = currentIndex;
                return true;
            }

            if (!SplitElement(ref result, currentIndex, out var span, out var scenarioId))
            {
                return false;
            }

            if (!ReadAssign(ref context, ref result, currentIndex))
            {
                return false;
            }

            switch (span.Length)
            {
                case 4 when span.SequenceEqual("spot"):
                    {
                        var element = new StringArrayElement(currentIndex)
                        {
                            ElementScenarioId = scenarioId
                        };
                        if (context.IsEnglishMode)
                        {
                            if (!Parse_StringArrayElement_Voice_EnglishMode(ref context, ref result, ref element))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!Parse_Element_ROAM(ref context, ref result, element))
                            {
                                return false;
                            }
                        }

                        ref var destination = ref node.Spot.EnsureGet(scenarioId);
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
                case 5 when span.SequenceEqual("power"):
                    {
                        var element = new StringArrayElement(currentIndex)
                        {
                            ElementScenarioId = scenarioId
                        };
                        if (context.IsEnglishMode)
                        {
                            if (!Parse_StringArrayElement_Voice_EnglishMode(ref context, ref result, ref element))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!Parse_Element_ROAM(ref context, ref result, element))
                            {
                                return false;
                            }
                        }

                        ref var destination = ref node.Power.EnsureGet(scenarioId);
                        element.ElementScenarioId = scenarioId;
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
                case 8 when span.SequenceEqual("delskill"):
                    {
                        var element = new VoiceTypeElement(currentIndex)
                        {
                            ElementScenarioId = scenarioId
                        };
                        if (!Parse_Element_OFFSET(ref context, ref result, element))
                        {
                            return false;
                        }

                        ref var destination = ref node.DelSkill.EnsureGet(scenarioId);
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
                case 10 when span.SequenceEqual("voice_type"):
                    {
                        var element = new VoiceTypeElement(currentIndex)
                        {
                            ElementScenarioId = scenarioId
                        };
                        if (!Parse_Element_OFFSET(ref context, ref result, element))
                        {
                            return false;
                        }

                        ref var destination = ref node.VoiceType.EnsureGet(scenarioId);
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
            }

            result.ErrorAdd_UnexpectedElementName(node.Kind, tokenList.LastIndex);
            return false;
        } while (true);
    }

    /// <summary>
    /// Already read '='.
    /// </summary>
    private static bool Parse_StringArrayElement_Voice_EnglishMode<T>(ref Context context, ref Result result, ref T element)
        where T : IElement<List<uint>>
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[element.ElementTokenId].Kind = TokenKind.ROAM;
        if (!result.SplitElement(element))
        {
            return false;
        }

        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorList.Add(new("Element must have value. There is no value text after '='.", result.TokenList[element.ElementTokenId].Range));
            return false;
        }

        tokenList.Last.Kind = TokenKind.Content;
        element.HasValue = true;
        element.Value.Add(tokenList.LastIndex);

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.Last.IsSemicolon(ref source))
            {
                if (element.Value.Count == 1 && tokenList[element.Value[0]].IsAtmark(ref source))
                {
                    element.Value.Clear();
                    element.HasValue = false;
                }

                return true;
            }

            if (tokenList.Last.IsComma(ref source) && tokenList.Last.Range.EndExclusive.Offset == 0)
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                    return false;
                }

                tokenList.Last.Kind = TokenKind.Content;
                element.Value.Add(tokenList.LastIndex);
            }
            else
            {
                result.UnionLast2Tokens();
            }
        } while (true);
    }
}
