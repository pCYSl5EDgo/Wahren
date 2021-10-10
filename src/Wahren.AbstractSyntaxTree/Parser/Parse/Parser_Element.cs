namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    /// <summary>
    /// Already read '='. Already Split.
    /// structure: attribute
    /// element: [ loyal, change, fkey, str, arbeit, brave, ground, gun_delay ]
    /// </summary>
    public static bool Parse_Element_LOYAL(ref Context context, ref Result result, Pair_NullableString_NullableIntElement element)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(element.ElementTokenId) = TokenKind.LOYAL;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_ValueDoesNotExistAfterAssignment(element.ElementTokenId);
            return false;
        }

        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
        if (element.Value.HasNumber = result.TryParse(tokenList.LastIndex, out element.Value.Number))
        {
            if (!result.IsEndOfLine(tokenList.LastIndex))
            {
                result.ErrorAdd("Line feed must be next to number value in this kind of element. Neither text nor comment are allowed.", tokenList.LastIndex);
                return false;
            }
        }

        var textIndex = tokenList.LastIndex;
        element.HasValue = true;
        element.Value.HasText = true;
        element.Value.Text = textIndex;
        while (!result.IsEndOfLine(tokenList.LastIndex))
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(textIndex);
                return false;
            }

            if (!result.IsMul(tokenList.LastIndex))
            {
                if (tokenList.GetPrecedingNewLineCount(tokenList.LastIndex) == 0)
                {
                    result.UnionLast2Tokens();
                    continue;
                }
                else
                {
                    CancelTokenReadback(ref context, ref result);
                    break;
                }
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(textIndex, "Number text is expected after '*'.");
                return false;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            element.Value.HasNumber = result.TryParse(tokenList.LastIndex, out element.Value.Number);
            if (!element.Value.HasNumber)
            {
                result.ErrorAdd("Number text must follows '*'.", textIndex);
            }

            if (result.IsEndOfLine(tokenList.LastIndex))
            {
                break;
            }

            result.ErrorAdd($"Line feed must be next to number value in this kind of element '{result.GetSpan(element.ElementTokenId)}'. Neither text nor comment are allowed.", textIndex);
            return false;
        }

        if (result.IsAtmark(textIndex))
        {
            element.HasValue = false;
        }

        return true;
    }

    /// <summary>
    /// Already read '='. Already split element.
    /// element: [ ray, poli, camp, home, multi, learn, skill, color, joint, weapon, skill2, weapon2, activenum, friend_ex ]
    /// </summary>
    private static bool Parse_Element_RAY(ref Context context, ref Result result, IElement<List<Pair_NullableString_NullableInt>> element)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(element.ElementTokenId) = TokenKind.RAY;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_ValueDoesNotExistAfterAssignment(element.ElementTokenId);
            return false;
        }

        element.HasValue = true;
        do
        {
            element.Value.Add(new());
            ref var elementLastValue = ref element.Value.Last;
            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            elementLastValue.HasText = !(elementLastValue.HasNumber = int.TryParse(result.GetSpan(elementLastValue.Text = tokenList.LastIndex), out elementLastValue.Number));

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.GetPrecedingNewLineCount(tokenList.LastIndex) != 0)
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            if (!result.IsComma(tokenList.LastIndex))
            {
                result.ErrorAdd_UnexpectedOperatorToken(element.ElementTokenId);
                return false;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }
        } while (true);

    TRUE:
        if (element.HasValue && element.Value.Count == 1 && element.Value[0].HasText && result.IsAtmark(element.Value[0].Text))
        {
            element.Value.Clear();
        }

        return true;
    }

    /// <summary>
    /// Default parsing function.
    /// '=' is already read. Already split.
    /// </summary>
    private static bool Parse_Element_DEFAULT(ref Context context, ref Result result, IElement<Pair_NullableString_NullableInt> element)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(element.ElementTokenId) = TokenKind.DEFAULT;
        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
            return false;
        }

        ref var source = ref result.Source;
        element.HasValue = true;
        element.Value.HasText = true;
        element.Value.Text = tokenList.LastIndex;
        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
        var processingLine = tokenList.GetLine(tokenList.LastIndex);
        if (element.Value.HasNumber = result.TryParse(tokenList.LastIndex, out element.Value.Number))
        {
            goto TRUE;
        }

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (processingLine != tokenList.GetLine(tokenList.LastIndex))
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            result.UnionLast2Tokens();
        } while (true);

    TRUE:
        if (result.IsAtmark(element.Value.Text))
        {
            element.HasValue = false;
        }

        return true;
    }

    /// <summary>
    /// Already read '='. Already split.
    /// struct: [ workspace, context ]
    /// element: [ member, merce, add2, next2, next3, just_next, monster, sound, item, castle_guard, item_sale, item_hold ]
    /// </summary>
    private static bool Parse_Element_MEMBER(ref Context context, ref Result result, IElement<List<Pair_NullableString_NullableInt>> element)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(element.ElementTokenId) = TokenKind.MEMBER;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_ValueDoesNotExistAfterAssignment(element.ElementTokenId);
            return false;
        }

        static bool AddValue_Pair_NullableString_NullableInt(IElement<List<Pair_NullableString_NullableInt>> element, ref Result result, ref TokenList tokenList)
        {
            if (result.IsOperator(tokenList.LastIndex))
            {
                return false;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            var item = new Pair_NullableString_NullableInt()
            {
                HasText = true,
                Text = tokenList.LastIndex,
            };
            item.HasNumber = result.TryParse(tokenList.LastIndex, out item.Number);
            element.Value.Add(ref item);
            return true;
        }

        ref var source = ref result.Source;
        element.HasValue = true;
        if (!AddValue_Pair_NullableString_NullableInt(element, ref result, ref tokenList))
        {
            result.ErrorAdd($"Unexpected operator char {result.GetSpan(tokenList.LastIndex)} appears. Text is expected.", tokenList.LastIndex);
            return false;
        }

        var createWarning = context.CreateError(DiagnosticSeverity.Warning);
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.GetLine(element.Value.Last.Text) != tokenList.GetLine(tokenList.LastIndex))
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            if (result.IsComma(tokenList.LastIndex))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                    return false;
                }

                if (AddValue_Pair_NullableString_NullableInt(element, ref result, ref tokenList))
                {
                    continue;
                }

                result.ErrorAdd($"Unexpected operator char {result.GetSpan(tokenList.LastIndex)} appears. Text is expected.", tokenList.LastIndex);
                return false;
            }

            if (!result.IsMul(tokenList.LastIndex))
            {
                result.UnionLast2Tokens();
                continue;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            if (result.TryParse(tokenList.LastIndex, out int repeatCount))
            {
                if (createWarning)
                {
                    if (repeatCount < 0)
                    {
                        result.WarningAdd($"Repeat count({repeatCount}) should be greater than -1.", tokenList.LastIndex);
                    }
                    else if (repeatCount == 0)
                    {
                        result.WarningAdd("Repeat count is 0. I recommend you not to write \"*0\".", tokenList.LastIndex);
                        element.Value.RemoveLast();
                    }
                }

                for (int i = 2; i < repeatCount; i++)
                {
                    element.Value.Add(element.Value.Last);
                }
            }
            else
            {
                result.ErrorAdd_NumberIsExpected(element.ElementTokenId);
            }

            var processingLineIndex = tokenList.GetLine(tokenList.LastIndex);
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (processingLineIndex != tokenList.GetLine(tokenList.LastIndex))
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            if (!result.IsComma(tokenList.LastIndex))
            {
                result.ErrorAdd_CommaIsExpected(element.ElementTokenId);
                return false;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (!AddValue_Pair_NullableString_NullableInt(element, ref result, ref tokenList))
            {
                result.ErrorAdd($"Unexpected operator char {result.GetSpan(tokenList.LastIndex)} appears. Text is expected.", tokenList.LastIndex);
                return false;
            }
        } while (true);

    TRUE:
        if (element.HasValue && element.Value.Count == 1 && element.Value[0].HasText && result.IsAtmark(element.Value[0].Text))
        {
            element.Value.Clear();
        }

        return true;
    }

    /// <summary>
    /// Already read '='. Already Split.
    /// element [ consti, icon, leader_skill, assist_skill, diplo, league, enemy_power, merits, loyals, wave, cutin, yorozu ]
    /// </summary>
    private static bool Parse_Element_CONSTI(ref Context context, ref Result result, IElement<List<Pair_NullableString_NullableInt>> element)
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(element.ElementTokenId) = TokenKind.CONSTI;
        element.HasValue = true;

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            element.Value.Add(new()
            {
                HasText = true,
                Text = tokenList.LastIndex,
            });
            element.Value.Last.HasNumber = int.TryParse(result.GetSpan(tokenList.LastIndex), out element.Value.Last.Number);
            do
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                    return false;
                }

                if (result.IsMul(tokenList.LastIndex))
                {
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                        return false;
                    }

                    tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
                    if (!(element.Value.Last.HasNumber = result.TryParse(tokenList.LastIndex, out element.Value.Last.Number)))
                    {
                        result.ErrorAdd_NumberIsExpected(element.ElementTokenId);
                        return false;
                    }

                    var processingLine = tokenList.GetLine(tokenList.LastIndex);
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                        return false;
                    }

                    if (tokenList.GetLine(tokenList.LastIndex) != processingLine)
                    {
                        CancelTokenReadback(ref context, ref result);
                        goto TRUE;
                    }

                    if (result.IsComma(tokenList.LastIndex))
                    {
                        break;
                    }

                    result.ErrorAdd_CommaIsExpected(element.ElementTokenId);
                    return false;
                }

                if (result.IsComma(tokenList.LastIndex))
                {
                    break;
                }

                if (tokenList.GetPrecedingNewLineCount(tokenList.LastIndex) != 0)
                {
                    CancelTokenReadback(ref context, ref result);
                    goto TRUE;
                }

                result.UnionLast2Tokens();
            } while (true);
        } while (true);

    TRUE:
        if (element.HasValue && element.Value.Count == 1 && element.Value[0].HasText && result.IsAtmark(element.Value[0].Text))
        {
            element.Value.Clear();
        }

        return true;
    }

    /// <summary>
    /// Already read '='. Already split.
    /// element: [ voice_type, delskill, delskill2, friend, enemy, staff, offset ]
    /// </summary>
    private static bool Parse_Element_OFFSET(ref Context context, ref Result result, Pair_NullableString_NullableInt_ArrayElement element)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(element.ElementTokenId) = TokenKind.OFFSET;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId, "Element must have value. There is no value text after '='.");
            return false;
        }

        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
        element.HasValue = true;
        element.Value.Add(new(tokenList.LastIndex));

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.GetPrecedingNewLineCount(tokenList.LastIndex) != 0)
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            if (!result.IsComma(tokenList.LastIndex))
            {
                result.UnionLast2Tokens();
                continue;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            element.Value.Add(new(tokenList.LastIndex));
        } while (true);

    TRUE:
        if (element.Value.Count == 1 && result.IsAtmark(element.Value[0].Text))
        {
            element.Value.Clear();
            element.HasValue = false;
        }

        return true;
    }

    /// <summary>
    /// Already read '='. Already split.
    /// element: [ roam, power, spot ]
    /// </summary>
    private static bool Parse_Element_ROAM(ref Context context, ref Result result, Pair_NullableString_NullableInt_ArrayElement element)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(element.ElementTokenId) = TokenKind.ROAM;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_ValueDoesNotExistAfterAssignment(element.ElementTokenId);
            return false;
        }

        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
        element.HasValue = true;
        element.Value.Add(new(tokenList.LastIndex));

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (result.IsSemicolon(tokenList.LastIndex))
            {
                goto TRUE;
            }

            if (result.IsComma(tokenList.LastIndex))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                    return false;
                }

                if (result.IsSemicolon(tokenList.LastIndex))
                {
                    goto TRUE;
                }

                tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
                element.Value.Add(new(tokenList.LastIndex));
            }
            else
            {
                result.UnionLast2Tokens();
            }
        } while (true);

    TRUE:
        if (element.Value.Count == 1 && result.IsAtmark(element.Value[0].Text))
        {
            element.Value.Clear();
            element.HasValue = false;
        }

        return true;
    }

    /// <summary>
    /// Already read '='. Already Split.
    /// detail
    /// element: [ text ]
    /// </summary>
    private static bool Parse_Element_TEXT(ref Context context, ref Result result, Pair_NullableString_NullableIntElement element)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(element.ElementTokenId) = TokenKind.TEXT;

        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
            return false;
        }

        if (result.IsSemicolon(tokenList.LastIndex))
        {
            element.HasValue = false;
            return true;
        }

        element.HasValue = true;
        element.Value = new(tokenList.LastIndex);
        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (result.IsSemicolon(tokenList.LastIndex))
            {
                return true;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.ContentTrailing;
            element.Value.HasNumber = false;
            element.Value.TrailingTokenCount++;
        } while (true);
    }
}
