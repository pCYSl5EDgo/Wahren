namespace Wahren.AbstractSyntaxTree.Parser;

using Statement;

public static partial class Parser
{
    /// <summary>
    /// Already read '('.
    /// </summary>
    private static bool Parse_RootBlock(ref Context context, ref Result result, uint currentIndex, ref List<IStatement> statements, ref List<IBlockStatement> blockStack)
    {
        ref var tokenList = ref result.TokenList;
        var actionKind = ActionKindHelper.Convert(result.GetSpan(currentIndex));
        result.UnionLast2Tokens();
        switch (actionKind)
        {
            case ActionKind.@while:
                return Parse_While(ref context, ref result, currentIndex, ref statements, ref blockStack);
            case ActionKind.next:
                tokenList.GetKind(tokenList.LastIndex) = TokenKind.next;
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (result.IsParenRight(tokenList.LastIndex))
                {
                    result.UnionLast2Tokens();
                }
                else
                {
                    CancelTokenReadback(ref context, ref result);
                    result.ErrorAdd_ParenRightIsExpected(currentIndex);
                }
                statements.Add(new NextStatement(currentIndex));
                return true;
            case ActionKind.@return:
                tokenList.GetKind(tokenList.LastIndex) = TokenKind.@return;
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (result.IsParenRight(tokenList.LastIndex))
                {
                    result.UnionLast2Tokens();
                }
                else
                {
                    CancelTokenReadback(ref context, ref result);
                    result.ErrorAdd_ParenRightIsExpected(currentIndex);
                }

                statements.Add(new ReturnStatement(currentIndex));
                return true;
            case ActionKind.@break:
                tokenList.GetKind(tokenList.LastIndex) = TokenKind.@break;
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (result.IsParenRight(tokenList.LastIndex))
                {
                    result.UnionLast2Tokens();
                }
                else
                {
                    CancelTokenReadback(ref context, ref result);
                    result.ErrorAdd_ParenRightIsExpected(currentIndex);
                }

                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd_MustBeInWhileBlock("break", currentIndex);
                }

                statements.Add(new BreakStatement(currentIndex, null));
                return true;
            case ActionKind.@continue:
                tokenList.GetKind(tokenList.LastIndex) = TokenKind.@continue;
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (result.IsParenRight(tokenList.LastIndex))
                {
                    result.UnionLast2Tokens();
                }
                else
                {
                    CancelTokenReadback(ref context, ref result);
                    result.ErrorAdd_ParenRightIsExpected(currentIndex);
                }

                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd_MustBeInWhileBlock("continue", currentIndex);
                }

                statements.Add(new ContinueStatement(currentIndex, null));
                return true;
        }

        var isRepeatIf = actionKind == ActionKind.rif;
        if (!isRepeatIf && actionKind != ActionKind.@if)
        {
            return Parse_CallAction(ref context, ref result, currentIndex, ref statements, actionKind);
        }

        return Parse_If(ref context, ref result, currentIndex, ref statements, ref blockStack, isRepeatIf);
    }

    /// <summary>
    /// Already read '{'.
    /// </summary>
    private static bool Parse_Block(ref Context context, ref Result result, ref List<IStatement> statements, ref List<IBlockStatement> blockStack)
    {
        static WhileStatement? GetWhile(ref List<IBlockStatement> blockStack)
        {
            for (int i = blockStack.Count - 1; i >= 0; --i)
            {
                if (blockStack[i] is WhileStatement @while)
                {
                    return @while;
                }
            }

            return null;
        }

        ref var tokenList = ref result.TokenList;
        WhileStatement? @while;
        var createWarning = context.CreateError(DiagnosticSeverity.Warning);
        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(blockStack.Last.TokenId);
                return false;
            }

            if (result.IsBracketRight(tokenList.LastIndex))
            {
                return true;
            }

            var currentIndex = tokenList.LastIndex;
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(tokenList.LastIndex);
                return false;
            }

            if (result.IsAssign(tokenList.LastIndex))
            {
                if (Parse_Discard(ref context, ref result, currentIndex))
                {
                    result.ErrorAdd_AssignmentInConditionalBlock(currentIndex);
                    continue;
                }
                else
                {
                    return false;
                }
            }

            if (!result.IsParenLeft(tokenList.LastIndex))
            {
                result.ErrorAdd_UnexpectedOperatorToken(currentIndex);
                return false;
            }

            var actionKind = ActionKindHelper.Convert(result.GetSpan(currentIndex));
            result.UnionLast2Tokens();
            switch (actionKind)
            {
                case ActionKind.@return:
                    tokenList.GetKind(tokenList.LastIndex) = TokenKind.@return;
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                        return false;
                    }

                    if (result.IsParenRight(tokenList.LastIndex))
                    {
                        result.UnionLast2Tokens();
                    }
                    else
                    {
                        CancelTokenReadback(ref context, ref result);
                        result.ErrorAdd_ParenRightIsExpected(currentIndex);
                    }
                    statements.Add(new ReturnStatement(currentIndex));
                    continue;
                case ActionKind.next:
                    tokenList.GetKind(tokenList.LastIndex) = TokenKind.next;
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                        return false;
                    }

                    if (result.IsParenRight(tokenList.LastIndex))
                    {
                        result.UnionLast2Tokens();
                    }
                    else
                    {
                        CancelTokenReadback(ref context, ref result);
                        result.ErrorAdd_ParenRightIsExpected(currentIndex);
                    }
                    statements.Add(new NextStatement(currentIndex));
                    continue;
                case ActionKind.@while:
                    if (Parse_While(ref context, ref result, currentIndex, ref statements, ref blockStack))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                case ActionKind.@break:
                    tokenList.GetKind(tokenList.LastIndex) = TokenKind.@break;
                    @while = GetWhile(ref blockStack);
                    if (@while is null && createWarning)
                    {
                        result.WarningAdd_MustBeInWhileBlock("break", currentIndex);
                    }
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                        return false;
                    }

                    if (result.IsParenRight(tokenList.LastIndex))
                    {
                        result.UnionLast2Tokens();
                    }
                    else
                    {
                        result.ErrorAdd_ParenRightIsExpected(currentIndex);
                        CancelTokenReadback(ref context, ref result);
                    }
                    statements.Add(new BreakStatement(currentIndex, @while));
                    continue;
                case ActionKind.@continue:
                    tokenList.GetKind(tokenList.LastIndex) = TokenKind.@continue;
                    @while = GetWhile(ref blockStack);
                    if (@while is null && createWarning)
                    {
                        result.WarningAdd_MustBeInWhileBlock("continue", currentIndex);
                    }
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                        return false;
                    }

                    if (result.IsParenRight(tokenList.LastIndex))
                    {
                        result.UnionLast2Tokens();
                    }
                    else
                    {
                        result.ErrorAdd_ParenRightIsExpected(currentIndex);
                        CancelTokenReadback(ref context, ref result);
                    }
                    statements.Add(new ContinueStatement(currentIndex, @while));
                    continue;
            }

            var isRepeatIf = actionKind == ActionKind.rif;
            if (!isRepeatIf && actionKind != ActionKind.@if)
            {
                if (Parse_CallAction(ref context, ref result, currentIndex, ref statements, actionKind))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            if (Parse_If(ref context, ref result, currentIndex, ref statements, ref blockStack, isRepeatIf))
            {
                continue;
            }
            else
            {
                return false;
            }
        } while (true);
    }

    private static bool Parse_While(ref Context context, ref Result result, uint currentIndex, ref List<IStatement> statements, ref List<IBlockStatement> blockStack)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(tokenList.LastIndex) = TokenKind.@while;
        var condition = ParseCondition(ref context, ref result, currentIndex, ConditionStatementKind.While);
        if (condition is null)
        {
            return false;
        }

        var statement = new WhileStatement(currentIndex, condition);
        statements.Add(statement);
        blockStack.Add(statement);
        var answer = Parse_Block(ref context, ref result, ref statement.LastStatements, ref blockStack);
        blockStack.RemoveLast();
        return answer;
    }

    private static bool Parse_If(ref Context context, ref Result result, uint currentIndex, ref List<IStatement> statements, ref List<IBlockStatement> blockStack, bool isRepeatIf)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(tokenList.LastIndex) = isRepeatIf ? TokenKind.rif : TokenKind.@if;
        var condition = ParseCondition(ref context, ref result, currentIndex, isRepeatIf ? ConditionStatementKind.Rif : ConditionStatementKind.If);
        if (condition is null)
        {
            return false;
        }

        var statement = new IfStatement(currentIndex, condition, isRepeatIf);
        statements.Add(statement);
        blockStack.Add(statement);
        var answer = Parse_Block(ref context, ref result, ref statement.LastStatements, ref blockStack);
        blockStack.RemoveLast();
        if (!answer)
        {
            return false;
        }

        if (!ReadToken(ref context, ref result))
        {
            return true;
        }

        if (!result.Is_else(tokenList.LastIndex))
        {
            CancelTokenReadback(ref context, ref result);
            return true;
        }

        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Else;
        statement.ElseTokenId = result.TokenList.LastIndex;
        statement.HasElseStatement = true;
        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(statement.ElseTokenId);
            return false;
        }

        blockStack.Add(statement);
        if (result.IsBracketLeft(tokenList.LastIndex))
        {
            answer = Parse_Block(ref context, ref result, ref statement.ElseStatements, ref blockStack);
        }
        else
        {
            var elseifIndex = result.TokenList.LastIndex;
            if (!ReadToken(ref context, ref result) || !result.IsParenLeft(tokenList.LastIndex))
            {
                result.ErrorAdd("'(' of else (r)if statement is not found.", elseifIndex);
                return false;
            }

            result.UnionLast2Tokens();
            answer = Parse_If(ref context, ref result, elseifIndex, ref statement.ElseStatements, ref blockStack, result.GetSpan(elseifIndex).SequenceEqual("rif"));
        }
        blockStack.RemoveLast();
        return answer;
    }

    private static bool Parse_CallAction_English(ref Context context, ref Result result, CallActionStatement statement, int beforeLastCompoundTextArgumentCount)
    {
        ref var tokenList = ref result.TokenList;
        Argument argument = new();
        ref var arguments = ref statement.Arguments;

        for (int i = 0; i != beforeLastCompoundTextArgumentCount; ++i)
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(statement.TokenId);
                return false;
            }

            if (result.IsParenRight(tokenList.LastIndex))
            {
                result.ErrorAdd_TooLessArguments(statement.Kind, i + 1, beforeLastCompoundTextArgumentCount + 1, tokenList.LastIndex);
                return true;
            }

            if (result.IsComma(tokenList.LastIndex))
            {
                result.ErrorAdd_ArgumentDoesNotExist(',', ',', tokenList.LastIndex);
                return true;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            argument.TokenId = tokenList.LastIndex;
            argument.IsNumber = result.TryParse(argument.TokenId, out argument.Number);
            arguments.Add(ref argument);
            ref var lastArgument = ref arguments.Last;
            do
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(statement.TokenId);
                    return false;
                }

                if (result.IsComma(tokenList.LastIndex))
                {
                    break;
                }

                if (result.IsParenRight(tokenList.LastIndex))
                {
                    result.ErrorAdd_TooLessArguments(statement.Kind, i + 1, beforeLastCompoundTextArgumentCount + 1, tokenList.LastIndex);
                    return true;
                }

                lastArgument.IsNumber = false;
                lastArgument.TrailingTokenCount++;
                tokenList.GetKind(tokenList.LastIndex) = TokenKind.ContentTrailing;
            } while (true);
        }

        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(statement.TokenId);
            return false;
        }

        if (result.IsParenRight(tokenList.LastIndex))
        {
            result.ErrorAdd_ArgumentDoesNotExist(',', ')', tokenList.LastIndex - 1);
            return true;
        }

        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
        argument.TokenId = tokenList.LastIndex;
        arguments.Add(ref argument);

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(statement.TokenId);
                return false;
            }

            if (result.IsParenRight(tokenList.LastIndex) && result.IsEndOfLine(tokenList.LastIndex))
            {
                return true;
            }

            arguments.Last.IsNumber = false;
            arguments.Last.TrailingTokenCount++;
            tokenList.GetKind(tokenList.LastIndex) = TokenKind.ContentTrailing;
        } while (true);
    }

    /// <summary>
    /// Already read '('.
    /// </summary>
    private static bool Parse_CallAction(ref Context context, ref Result result, uint currentIndex, ref List<IStatement> statements, ActionKind actionKind)
    {
        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(currentIndex) = TokenKind.CallAction;
        tokenList.GetOther(currentIndex) = (uint)actionKind;
        var statement = new CallActionStatement(currentIndex, actionKind);
        statements.Add(statement);
        if (context.IsEnglishMode)
        {
            switch (actionKind)
            {
                case ActionKind.msg:
                case ActionKind.msg2:
                case ActionKind.dialog:
                    return Parse_CallAction_English(ref context, ref result, statement, 0);
                case ActionKind.talk:
                case ActionKind.talk2:
                case ActionKind.dialogF:
                case ActionKind.select:
                    return Parse_CallAction_English(ref context, ref result, statement, 1);
                case ActionKind.chat:
                case ActionKind.chat2:
                    return Parse_CallAction_English(ref context, ref result, statement, 2);
            }
        }

        Argument argument = new();
        ref var arguments = ref statement.Arguments;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                return false;
            }

            if (result.IsParenRight(tokenList.LastIndex))
            {
                if (!arguments.IsEmpty)
                {
                    result.ErrorAdd_ArgumentDoesNotExist(',', ')', tokenList.LastIndex - 1);
                }

                goto TRUE;
            }

            if (result.IsComma(tokenList.LastIndex))
            {
                result.ErrorAdd_ArgumentDoesNotExist(',', ',', currentIndex);
                continue;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            argument.TokenId = tokenList.LastIndex;
            argument.IsNumber = result.TryParse(argument.TokenId, out argument.Number);

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                return false;
            }

            arguments.Add(ref argument);
            if (result.IsComma(tokenList.LastIndex))
            {
                continue;
            }
            else if (result.IsParenRight(tokenList.LastIndex))
            {
                goto TRUE;
            }

            arguments.Last.IsNumber = false;
            arguments.Last.TrailingTokenCount++;
            tokenList.GetKind(tokenList.LastIndex) = TokenKind.ContentTrailing;

            do
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (result.IsComma(tokenList.LastIndex))
                {
                    break;
                }
                else if (result.IsParenRight(tokenList.LastIndex))
                {
                    goto TRUE;
                }

                arguments.Last.TrailingTokenCount++;
                tokenList.GetKind(tokenList.LastIndex) = TokenKind.ContentTrailing;
            } while (true);
        } while (true);

    TRUE:
        if (actionKind != ActionKind.None)
        {
            return true;
        }

        var endingRoll = default(CallEndingRollActionStatement);
        if (arguments.IsEmpty)
        {
            endingRoll = TryParseCallEndingRollActionStatement(result.GetSpan(currentIndex), currentIndex, uint.MaxValue, uint.MaxValue);
        }
        else if (arguments.Count == 1)
        {
            endingRoll = TryParseCallEndingRollActionStatement(result.GetSpan(currentIndex), currentIndex, arguments[0].TokenId, uint.MaxValue);
        }
        else if (arguments.Count == 2)
        {
            endingRoll = TryParseCallEndingRollActionStatement(result.GetSpan(currentIndex), currentIndex, arguments[0].TokenId, arguments[1].TokenId);
        }

        if (endingRoll is null)
        {
            result.ErrorAdd_UnexpectedCall(currentIndex);
        }
        else
        {
            statements.Last.Dispose();
            statements.RemoveLast();
            statements.Add(endingRoll);
        }

        return true;
    }

    private static CallEndingRollActionStatement? TryParseCallEndingRollActionStatement(Span<char> span, uint tokenId, uint text0, uint text1)
    {
        if (span.IsEmpty)
        {
            return null;
        }
        var any = span.IndexOfAny(' ', '\t', '(');
        if (any <= 0)
        {
            return null;
        }

        span = span.Slice(0, any);
        byte font0;
        byte font1 = byte.MaxValue;
        char margin = '\0';
        byte marginCount = 0;
        EndingRollDisplayMethod displayMethod = EndingRollDisplayMethod.None;
        int index = 0;

    Text0_or_Display:
        switch (span[index++])
        {
            case '@':
                displayMethod |= EndingRollDisplayMethod.Fade;
                if (index == span.Length)
                {
                    return null;
                }

                goto Text0_or_Display;
            case '#':
                displayMethod |= EndingRollDisplayMethod.StopCenter;
                if (index == span.Length)
                {
                    return null;
                }

                goto Text0_or_Display;
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
            case 'h':
            case 'i':
            case 'j':
                font0 = (byte)(span[index - 1] - 'a');
                break;
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'G':
            case 'H':
            case 'I':
            case 'J':
                font0 = (byte)(span[index - 1] - 'A');
                break;
            default:
                return null;
        }

        if (index == span.Length)
        {
            goto Return;
        }

        switch (span[index++])
        {
            case '_':
                break;
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
            case 'h':
            case 'i':
            case 'j':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'G':
            case 'H':
            case 'I':
            case 'J':
                margin = span[index - 1];
                marginCount = 1;
                goto Margin;
            default:
                return null;
        }

        if (index == span.Length)
        {
            return null;
        }

        switch (span[index++])
        {
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
            case 'h':
            case 'i':
            case 'j':
                font1 = (byte)(span[index - 1] - 'a');
                break;
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'G':
            case 'H':
            case 'I':
            case 'J':
                font1 = (byte)(span[index - 1] - 'A');
                break;
            default:
                return null;
        }

        if (index == span.Length)
        {
            goto Return;
        }

        switch (span[index++])
        {
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
            case 'h':
            case 'i':
            case 'j':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'G':
            case 'H':
            case 'I':
            case 'J':
                margin = span[index - 1];
                marginCount = 1;
                goto Margin;
            default:
                return null;
        }

    Margin:
        for (; index < span.Length; index++)
        {
            var c = span[index];
            if (c != margin)
            {
                return null;
            }

            marginCount++;
        }

    Return:
        return new CallEndingRollActionStatement(tokenId, text0, text1, font0, font1, displayMethod, margin == '\0' ? byte.MaxValue : (byte)(margin >= 'a' ? margin - 'a' : margin - 'A'), marginCount);
    }
}
