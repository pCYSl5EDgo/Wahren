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
        tokenList[element.ElementTokenId].Kind = TokenKind.LOYAL;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd("Element must have value. There is no value text after '='.", element.ElementTokenId);
            return false;
        }

        ref var source = ref result.Source;
        tokenList.Last.Kind = TokenKind.Content;
        if (element.Value.HasNumber = tokenList.Last.TryParse(ref source, out element.Value.Number))
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

            if (!tokenList.Last.IsMul(ref source))
            {
                if (tokenList.Last.PrecedingNewLineCount == 0)
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

            tokenList.Last.Kind = TokenKind.Content;
            element.Value.HasNumber = tokenList.Last.TryParse(ref source, out element.Value.Number);
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

        if (tokenList[textIndex].IsAtmark(ref source))
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
        tokenList[element.ElementTokenId].Kind = TokenKind.RAY;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd("Element must have value. There is no value text after '='.", element.ElementTokenId);
            return false;
        }

        ref var source = ref result.Source;
        element.HasValue = true;
        ref var thisLine = ref source[tokenList.Last.Position.Line];
        do
        {
            element.Value.Add(new());
            ref var elementLastValue = ref element.Value.Last;
            tokenList.Last.Kind = TokenKind.Content;
            elementLastValue.HasText = !(elementLastValue.HasNumber = int.TryParse(result.GetSpan(elementLastValue.Text = tokenList.LastIndex), out elementLastValue.Number));

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.Last.Position.Line != tokenList[tokenList.LastIndex - 1].Position.Line)
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            if (!tokenList.Last.IsComma(ref source))
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
        if (element.HasValue && element.Value.Count == 1 && element.Value[0].HasText && tokenList[element.Value[0].Text].IsAtmark(ref source))
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
        tokenList[element.ElementTokenId].Kind = TokenKind.DEFAULT;
        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
            return false;
        }

        ref var source = ref result.Source;
        element.HasValue = true;
        element.Value.HasText = true;
        element.Value.Text = tokenList.LastIndex;
        tokenList.Last.Kind = TokenKind.Content;
        var processingLine = tokenList.Last.Position.Line;
        if (element.Value.HasNumber = tokenList.Last.TryParse(ref source, out element.Value.Number))
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

            if (processingLine != tokenList.Last.Position.Line)
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            result.UnionLast2Tokens();
        } while (true);

    TRUE:
        if (tokenList[element.Value.Text].IsAtmark(ref source))
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
        tokenList[element.ElementTokenId].Kind = TokenKind.MEMBER;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd("Element must have value. There is no value text after '='.", element.ElementTokenId);
            return false;
        }

        static bool AddValue_Pair_NullableString_NullableInt(IElement<List<Pair_NullableString_NullableInt>> element, ref DualList<char> source, ref List<Token> tokenList)
        {
            if (tokenList.Last.IsOperator(ref source))
            {
                return false;
            }

            tokenList.Last.Kind = TokenKind.Content;
            var item = new Pair_NullableString_NullableInt()
            {
                HasText = true,
                Text = tokenList.LastIndex,
            };
            item.HasNumber = tokenList.Last.TryParse(ref source, out item.Number);
            element.Value.Add(ref item);
            return true;
        }

        ref var source = ref result.Source;
        element.HasValue = true;
        if (!AddValue_Pair_NullableString_NullableInt(element, ref source, ref tokenList))
        {
            result.ErrorAdd($"Unexpected operator char{source[tokenList.Last.Position.Line][tokenList.Last.Position.Offset]} appears. Text is expected.", tokenList.LastIndex);
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

            if (tokenList[element.Value.Last.Text].Position.Line != tokenList.Last.Position.Line)
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            if (tokenList.Last.IsComma(ref source))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                    return false;
                }

                if (AddValue_Pair_NullableString_NullableInt(element, ref source, ref tokenList))
                {
                    continue;
                }

                result.ErrorAdd($"Unexpected operator char{source[tokenList.Last.Position.Line][tokenList.Last.Position.Offset]} appears. Text is expected.", tokenList.LastIndex);
                return false;
            }

            if (!tokenList.Last.IsMul(ref source))
            {
                result.UnionLast2Tokens();
                continue;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            tokenList.Last.Kind = TokenKind.Content;
            if (tokenList.Last.TryParse(ref source, out int repeatCount))
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

            var processingLineIndex = tokenList.Last.Position.Line;
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (processingLineIndex != tokenList.Last.Position.Line)
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            if (!tokenList.Last.IsComma(ref source))
            {
                result.ErrorAdd_CommaIsExpected(element.ElementTokenId);
                return false;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (!AddValue_Pair_NullableString_NullableInt(element, ref source, ref tokenList))
            {
                result.ErrorAdd($"Unexpected operator char{source[tokenList.Last.Position.Line][tokenList.Last.Position.Offset]} appears. Text is expected.", tokenList.LastIndex);
                return false;
            }
        } while (true);

    TRUE:
        if (element.HasValue && element.Value.Count == 1 && element.Value[0].HasText && tokenList[element.Value[0].Text].IsAtmark(ref source))
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
        tokenList[element.ElementTokenId].Kind = TokenKind.CONSTI;
        element.HasValue = true;

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            tokenList.Last.Kind = TokenKind.Content;
            element.Value.Add(new()
            {
                HasText = true,
                Text = tokenList.LastIndex,
            });

            do
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                    return false;
                }

                if (tokenList.Last.IsMul(ref source))
                {
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                        return false;
                    }

                    tokenList.Last.Kind = TokenKind.Content;
                    if (!(element.Value.Last.HasNumber = tokenList.Last.TryParse(ref source, out element.Value.Last.Number)))
                    {
                        result.ErrorAdd_NumberIsExpected(element.ElementTokenId);
                        return false;
                    }

                    var processingLine = tokenList.Last.Position.Line;
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                        return false;
                    }

                    if (tokenList.Last.Position.Line != processingLine)
                    {
                        CancelTokenReadback(ref context, ref result);
                        goto TRUE;
                    }

                    if (tokenList.Last.IsComma(ref source))
                    {
                        break;
                    }

                    result.ErrorAdd_CommaIsExpected(element.ElementTokenId);
                    return false;
                }

                if (tokenList.Last.IsComma(ref source))
                {
                    break;
                }

                if (tokenList.Last.Position.Line != tokenList[tokenList.LastIndex - 1].Position.Line)
                {
                    CancelTokenReadback(ref context, ref result);
                    goto TRUE;
                }

                result.UnionLast2Tokens();
            } while (true);
        } while (true);

    TRUE:
        if (element.HasValue && element.Value.Count == 1 && element.Value[0].HasText && tokenList[element.Value[0].Text].IsAtmark(ref source))
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
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[element.ElementTokenId].Kind = TokenKind.OFFSET;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId, "Element must have value. There is no value text after '='.");
            return false;
        }

        tokenList.Last.Kind = TokenKind.Content;
        element.HasValue = true;
        element.Value.Add(new(tokenList.LastIndex));

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.Last.Position.Line != tokenList[tokenList.LastIndex - 1].Position.Line)
            {
                CancelTokenReadback(ref context, ref result);
                goto TRUE;
            }

            if (!tokenList.Last.IsComma(ref source))
            {
                result.UnionLast2Tokens();
                continue;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            tokenList.Last.Kind = TokenKind.Content;
            element.Value.Add(new(tokenList.LastIndex));
        } while (true);

    TRUE:
        if (element.Value.Count == 1 && tokenList[element.Value[0].Text].IsAtmark(ref source))
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
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[element.ElementTokenId].Kind = TokenKind.ROAM;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd("Element must have value. There is no value text after '='.", element.ElementTokenId);
            return false;
        }

        tokenList.Last.Kind = TokenKind.Content;
        element.HasValue = true;
        element.Value.Add(new(tokenList.LastIndex));

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.Last.IsSemicolon(ref source))
            {
                goto TRUE;
            }

            if (tokenList.Last.IsComma(ref source))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                    return false;
                }

                if (tokenList.Last.IsSemicolon(ref source))
                {
                    goto TRUE;
                }

                tokenList.Last.Kind = TokenKind.Content;
                element.Value.Add(new(tokenList.LastIndex));
            }
            else
            {
                result.UnionLast2Tokens();
            }
        } while (true);

    TRUE:
        if (element.Value.Count == 1 && tokenList[element.Value[0].Text].IsAtmark(ref source))
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
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[element.ElementTokenId].Kind = TokenKind.TEXT;

        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
            return false;
        }

        if (tokenList.Last.IsSemicolon(ref source))
        {
            element.HasValue = false;
            return true;
        }

        element.HasValue = true;
        element.Value = new(tokenList.LastIndex);
        tokenList.Last.Kind = TokenKind.Content;

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.Last.IsSemicolon(ref source))
            {
                return true;
            }

            tokenList.Last.Kind = TokenKind.ContentTrailing;
            element.Value.HasNumber = false;
            element.Value.TrailingTokenCount++;
        } while (true);
    }
}
