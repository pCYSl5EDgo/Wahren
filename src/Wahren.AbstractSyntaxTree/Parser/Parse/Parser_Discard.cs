namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    /// <summary>
    /// Already read '='.
    /// </summary>
    private static bool Parse_Discard(ref Context context, ref Result result, uint elementTokenId)
    {
        if (!SplitElement(ref result, elementTokenId, out var span, out _))
        {
            return false;
        }

        return Parse_Discard(ref context, ref result, elementTokenId, StringHashUtility.Calc(span));
    }

    /// <summary>
    /// Already read '='.
    /// structure: attribute
    /// element: [ loyal, change, fkey, str, arbeit, brave, ground, gun_delay ]
    /// </summary>
    private static bool Parse_Discard_LOYAL(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(elementTokenId) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.LOYAL;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_ValueDoesNotExistAfterAssignment(elementTokenId);
            return false;
        }

        tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
        if (result.TryParse(tokenList.LastIndex, out _))
        {
            if (!result.IsEndOfLine(tokenList.LastIndex))
            {
                result.ErrorAdd("Line feed must be next to number value in this kind of element. Neither text nor comment are allowed.", tokenList.LastIndex);
                return false;
            }
        }

        var textIndex = tokenList.LastIndex;
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

            tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
            if (!result.TryParse(tokenList.LastIndex, out _))
            {
                result.ErrorAdd("Number text must follows '*'.", textIndex);
            }

            if (result.IsEndOfLine(tokenList.LastIndex))
            {
                break;
            }

            result.ErrorAdd("Line feed must be next to number value in this kind of element. Neither text nor comment are allowed.", textIndex);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Already read '='. Already split element.
    /// element: [ ray, poli, camp, home, multi, learn, skill, color, joint, weapon, skill2, weapon2, activenum, friend_ex ]
    /// </summary>
    private static bool Parse_Discard_RAY(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(elementTokenId) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.RAY;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_ValueDoesNotExistAfterAssignment(elementTokenId);
            return false;
        }

        do
        {
            tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (tokenList.GetPrecedingNewLineCount(tokenList.LastIndex) != 0)
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            if (!result.IsComma(tokenList.LastIndex))
            {
                result.ErrorAdd_UnexpectedOperatorToken(elementTokenId);
                return false;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }
        } while (true);
    }

    /// <summary>
    /// Already read '='. Already Split.
    /// detail
    /// element: [ text ]
    /// </summary>
    private static bool Parse_Discard_TEXT(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(elementTokenId) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.TEXT;
        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
            return false;
        }

        if (result.IsSemicolon(tokenList.LastIndex))
        {
            return true;
        }

        tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }
            tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.ContentTrailing;
        } while (!result.IsSemicolon(tokenList.LastIndex));
        return true;
    }

    /// <summary>
    /// Already read '='. Already Split.
    /// element [ consti, icon, leader_skill, assist_skill, diplo, league, enemy_power, merits, loyals, wave, cutin, yorozu ]
    /// </summary>
    private static bool Parse_Discard_CONSTI(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(elementTokenId) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.CONSTI;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
            do
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                    return false;
                }

                if (result.IsMul(tokenList.LastIndex))
                {
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                        return false;
                    }

                    tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
                    if (!result.TryParse(tokenList.LastIndex, out _))
                    {
                        result.ErrorAdd_NumberIsExpected(elementTokenId);
                    }

                    var processingLine = tokenList.GetLine(tokenList.LastIndex);
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                        return false;
                    }

                    if (tokenList.GetLine(tokenList.LastIndex) != processingLine)
                    {
                        CancelTokenReadback(ref context, ref result);
                        return true;
                    }

                    if (result.IsComma(tokenList.LastIndex))
                    {
                        break;
                    }

                    result.ErrorAdd_CommaIsExpected(elementTokenId);
                    return false;
                }

                if (result.IsComma(tokenList.LastIndex))
                {
                    break;
                }

                if (tokenList.GetPrecedingNewLineCount(tokenList.LastIndex) != 0)
                {
                    CancelTokenReadback(ref context, ref result);
                    return true;
                }

                result.UnionLast2Tokens();
            } while (true);
        } while (true);
    }

    /// <summary>
    /// Already read '='. Already split.
    /// element: [ roam, power, spot ]
    /// </summary>
    private static bool Parse_Discard_ROAM(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(elementTokenId) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.ROAM;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_ValueDoesNotExistAfterAssignment(elementTokenId);
            return false;
        }

        tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
        ref var source = ref result.Source;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (result.IsSemicolon(tokenList.LastIndex))
            {
                return true;
            }

            if (result.IsComma(tokenList.LastIndex))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                    return false;
                }

                if (result.IsSemicolon(tokenList.LastIndex))
                {
                    return true;
                }

                tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
            }
            else
            {
                result.UnionLast2Tokens();
            }
        } while (true);
    }

    /// <summary>
    /// Already read '='. Already split.
    /// struct: [ workspace, context ]
    /// element: [ member, merce, add2, next2, next3, just_next, monster, sound, item, castle_guard, item_sale, item_hold ]
    /// </summary>
    private static bool Parse_Discard_MEMBER(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(elementTokenId) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.MEMBER;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_ValueDoesNotExistAfterAssignment(elementTokenId);
            return false;
        }

        uint lastAddIndex = tokenList.LastIndex;
        tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
        if (result.IsOperator(tokenList.LastIndex))
        {
            result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex);
            return false;
        }

        var createWarning = context.CreateError(DiagnosticSeverity.Warning);
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (tokenList.GetLine(lastAddIndex) != tokenList.GetLine(tokenList.LastIndex))
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            if (result.IsComma(tokenList.LastIndex))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                    return false;
                }

                lastAddIndex = tokenList.LastIndex;
                tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
                if (result.IsOperator(tokenList.LastIndex))
                {
                    result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex);
                    return false;
                }

                continue;
            }

            if (!result.IsMul(tokenList.LastIndex))
            {
                result.UnionLast2Tokens();
                continue;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
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
                    }
                    else if (repeatCount == 1)
                    {
                        result.WarningAdd("Repeat count is 1. I recommend you not to write \"*1\".", tokenList.LastIndex);
                    }
                }
            }
            else
            {
                result.ErrorAdd_NumberIsExpected(elementTokenId);
            }

            var processingLineIndex = tokenList.GetLine(tokenList.LastIndex);
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (processingLineIndex != tokenList.GetLine(tokenList.LastIndex))
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            if (!result.IsComma(tokenList.LastIndex))
            {
                result.ErrorAdd_CommaIsExpected(elementTokenId);
                return false;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
            lastAddIndex = tokenList.LastIndex;
            if (result.IsOperator(tokenList.LastIndex))
            {
                result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex);
                return false;
            }
        } while (true);
    }

    /// <summary>
    /// Already read '='. Already split.
    /// element: [ voice_type, delskill, delskill2, friend, enemy, staff, offset ]
    /// </summary>
    private static bool Parse_Discard_OFFSET(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(elementTokenId) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.OFFSET;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(elementTokenId, "Element must have value. There is no value text after '='.");
            return false;
        }

        tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
        ref var source = ref result.Source;
        var processingLineIndex = tokenList.GetLine(tokenList.LastIndex);
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (tokenList.GetLine(tokenList.LastIndex) != processingLineIndex)
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            if (!result.IsComma(tokenList.LastIndex))
            {
                result.UnionLast2Tokens();
                continue;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
            if (tokenList.GetLine(tokenList.LastIndex) != processingLineIndex)
            {
                result.ErrorAdd("Unexpected Line Feed. ',' needs succeding token on the same line.", tokenList.LastIndex - 1);
                return false;
            }
        } while (true);
    }

    /// <summary>
    /// Default parsing function.
    /// '=' is already read. Already split.
    /// </summary>
    private static bool Parse_Discard_DEFAULT(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(elementTokenId) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.DEFAULT;
        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
            return false;
        }

        ref var source = ref result.Source;
        tokenList.GetKind(tokenList.LastIndex) = context.DeleteDiscardedToken ? TokenKind.Deleted : TokenKind.Content;
        var processingLine = tokenList.GetLine(tokenList.LastIndex);
        var hasNumber = result.TryParse(tokenList.LastIndex, out _);
        if (hasNumber)
        {
            return true;
        }

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (processingLine != tokenList.GetLine(tokenList.LastIndex))
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            result.UnionLast2Tokens();
        } while (true);
    }
}
