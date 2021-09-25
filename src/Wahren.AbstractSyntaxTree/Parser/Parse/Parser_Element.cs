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
            result.ErrorList.Add(new("Element must have value. There is no value text after '='.", result.TokenList[element.ElementTokenId].Range));
            return false;
        }

        ref var source = ref result.Source;
        tokenList.Last.Kind = TokenKind.Content;
        if (element.Value.HasNumber = tokenList.Last.TryParse(ref source, out element.Value.Number))
        {
            if (tokenList.Last.Range.EndExclusive.Offset != 0)
            {
                result.ErrorList.Add(new("Line feed must be next to number value in this kind of element. Neither text nor comment are allowed.", tokenList.Last.Range));
                return false;
            }
        }

        var textIndex = tokenList.LastIndex;
        while (tokenList.Last.Range.EndExclusive.Offset != 0)
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(textIndex);
                return false;
            }

            if (!tokenList.Last.IsMul(ref source))
            {
                result.UnionLast2Tokens();
                continue;
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
                result.ErrorList.Add(new("Number text must follows '*'.", tokenList[textIndex].Range));
            }

            if (tokenList.Last.Range.EndExclusive.Offset == 0)
            {
                break;
            }

            result.ErrorList.Add(new($"Line feed must be next to number value in this kind of element '{result.GetSpan(element.ElementTokenId)}'. Neither text nor comment are allowed.", tokenList[textIndex].Range));
            return false;
        }

        if (tokenList[textIndex].IsAtmark(ref source))
        {
            element.HasValue = false;
        }
        else
        {
            element.HasValue = true;
            element.Value.HasText = true;
            element.Value.Text = textIndex;
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
            result.ErrorList.Add(new("Element must have value. There is no value text after '='.", result.TokenList[element.ElementTokenId].Range));
            return false;
        }

        ref var source = ref result.Source;
        element.HasValue = true;
        ref var thisLine = ref source[tokenList.Last.Range.StartInclusive.Line];
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

            if (tokenList.Last.Range.StartInclusive.Line != tokenList[tokenList.LastIndex - 1].Range.StartInclusive.Line)
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
        var processingLine = tokenList.Last.Range.StartInclusive.Line;
        if (element.Value.HasNumber = tokenList.Last.TryParse(ref source, out element.Value.Number))
        {
            ref var end = ref tokenList.Last.Range.EndExclusive;
            if (processingLine == end.Line)
            {
                var span = source[end.Line].AsSpan(end.Offset);
                if (!span.IsEmpty)
                {
                    foreach (var item in span)
                    {
                        if (item != ' ' && item != '\t')
                        {
                            result.ErrorList.Add(new($"Line feed is needed immediately after number text({element.Value.Number}).", tokenList[element.ElementTokenId].Range));
                            return false;
                        }
                    }

                    if (context.CreateError(DiagnosticSeverity.Hint))
                    {
                        result.ErrorList.Add(new($"Line feed is needed immediately after number text({element.Value.Number}).  Trailing whitespaces are allowed, but not recommended.", tokenList[element.ElementTokenId].Range, DiagnosticSeverity.Hint));
                    }
                }
            }

            goto TRUE;
        }

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (processingLine != tokenList.Last.Range.StartInclusive.Line)
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
            result.ErrorList.Add(new("Element must have value. There is no value text after '='.", result.TokenList[element.ElementTokenId].Range));
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
            result.ErrorList.Add(new($"Unexpected operator char{source[tokenList.Last.Range.StartInclusive.Line][tokenList.Last.Range.StartInclusive.Offset]} appears. Text is expected.", tokenList.Last.Range));
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

            if (tokenList[element.Value.Last.Text].Range.StartInclusive.Line != tokenList.Last.Range.StartInclusive.Line)
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

                result.ErrorList.Add(new($"Unexpected operator char{source[tokenList.Last.Range.StartInclusive.Line][tokenList.Last.Range.StartInclusive.Offset]} appears. Text is expected.", tokenList.Last.Range));
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
                        result.ErrorList.Add(new($"Repeat count({repeatCount}) should be greater than -1.", tokenList.Last.Range, DiagnosticSeverity.Warning, ErrorCode.Semantics));
                    }
                    else if (repeatCount == 0)
                    {
                        result.ErrorList.Add(new("Repeat count is 0. I recommend you not to write \"*0\".", tokenList.Last.Range, DiagnosticSeverity.Warning, ErrorCode.Semantics));
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

            var processingLineIndex = tokenList.Last.Range.StartInclusive.Line;
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (processingLineIndex != tokenList.Last.Range.StartInclusive.Line)
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
                result.ErrorList.Add(new($"Unexpected operator char{source[tokenList.Last.Range.StartInclusive.Line][tokenList.Last.Range.StartInclusive.Offset]} appears. Text is expected.", tokenList.Last.Range));
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

                    var processingLine = tokenList.Last.Range.StartInclusive.Line;
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                        return false;
                    }

                    if (tokenList.Last.Range.StartInclusive.Line != processingLine)
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

                if (tokenList.Last.Range.StartInclusive.Line != tokenList[tokenList.LastIndex - 1].Range.StartInclusive.Line)
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
    private static bool Parse_Element_OFFSET(ref Context context, ref Result result, IElement<List<uint>> element)
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
        element.Value.Add(tokenList.LastIndex);

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.Last.Range.StartInclusive.Line != tokenList[tokenList.LastIndex - 1].Range.StartInclusive.Line)
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
            element.Value.Add(tokenList.LastIndex);
        } while (true);

    TRUE:
        if (element.Value.Count == 1 && tokenList[element.Value[0]].IsAtmark(ref source))
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
    private static bool Parse_Element_ROAM(ref Context context, ref Result result, IElement<List<uint>> element)
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[element.ElementTokenId].Kind = TokenKind.ROAM;
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
                element.Value.Add(tokenList.LastIndex);
            }
            else
            {
                result.UnionLast2Tokens();
            }
        } while (true);

    TRUE:
        if (element.Value.Count == 1 && tokenList[element.Value[0]].IsAtmark(ref source))
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
    private static bool Parse_Element_TEXT(ref Context context, ref Result result, StringElement element)
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[element.ElementTokenId].Kind = TokenKind.TEXT;
        tokenList.Add(new());
        if (!Lexer.ReadTokenSemicolon(ref source, ref context.Position, ref tokenList.Last))
        {
            tokenList.RemoveLast();
            result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId, "';' is expected.");
            return false;
        }

        tokenList.Last.Kind = TokenKind.Content;
        element.HasValue = true;
        element.Value = tokenList.LastIndex;

        if (!ReadUsefulToken(ref context, ref result) || !tokenList.Last.IsSemicolon(ref source))
        {
            throw new InvalidOperationException();
        }

        return true;
    }
}
