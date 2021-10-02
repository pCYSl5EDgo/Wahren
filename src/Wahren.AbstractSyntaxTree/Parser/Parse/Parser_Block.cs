﻿namespace Wahren.AbstractSyntaxTree.Parser;

using Statement;

public static partial class Parser
{
    /// <summary>
    /// Already read '('.
    /// </summary>
    private static bool Parse_RootBlock(ref Context context, ref Result result, uint currentIndex, ref List<IStatement> statements, ref List<IBlockStatement> blockStack)
    {
        ref var tokenList = ref result.TokenList;
        ref var source = ref result.Source;
        var actionKind = ActionKindHelper.Convert(result.GetSpan(currentIndex));
        result.UnionLast2Tokens();
        switch (actionKind)
        {
            case ActionKind.@while:
                return Parse_While(ref context, ref result, currentIndex, ref statements, ref blockStack);
            case ActionKind.next:
                tokenList.Last.Kind = TokenKind.next;
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (tokenList.Last.IsParenRight(ref source))
                {
                    result.UnionLast2Tokens();
                }
                else
                {
                    CancelTokenReadback(ref context, ref result);
                    result.ErrorAdd("')' of 'next()' is not found.", currentIndex);
                }
                statements.Add(new NextStatement(currentIndex));
                return true;
            case ActionKind.@return:
                tokenList.Last.Kind = TokenKind.@return;
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (tokenList.Last.IsParenRight(ref source))
                {
                    result.UnionLast2Tokens();
                }
                else
                {
                    CancelTokenReadback(ref context, ref result);
                    result.ErrorAdd("return()'s ')' is not found.", currentIndex);
                }

                statements.Add(new ReturnStatement(currentIndex));
                return true;
            case ActionKind.@break:
                tokenList.Last.Kind = TokenKind.@break;
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (tokenList.Last.IsParenRight(ref source))
                {
                    result.UnionLast2Tokens();
                }
                else
                {
                    CancelTokenReadback(ref context, ref result);
                    result.ErrorAdd("break()'s ')' is not found.", currentIndex);
                }

                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd("break() statement must be in while loop.", currentIndex);
                }

                statements.Add(new BreakStatement(currentIndex, null));
                return true;
            case ActionKind.@continue:
                tokenList.Last.Kind = TokenKind.@continue;
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (tokenList.Last.IsParenRight(ref source))
                {
                    result.UnionLast2Tokens();
                }
                else
                {
                    CancelTokenReadback(ref context, ref result);
                    result.ErrorAdd("continue()'s ')' is not found.", currentIndex);
                }

                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd("continue() statement must be in while loop.", currentIndex);
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
        ref var source = ref result.Source;
        WhileStatement? @while;
        var createWarning = context.CreateError(DiagnosticSeverity.Warning);
        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(blockStack.Last.TokenId);
                return false;
            }

            if (tokenList.Last.IsBracketRight(ref source))
            {
                return true;
            }

            var currentIndex = tokenList.LastIndex;
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(tokenList.LastIndex);
                return false;
            }

            if (tokenList.Last.IsAssign(ref source))
            {
                if (Parse_Discard(ref context, ref result, currentIndex))
                {
                    if (createWarning)
                    {
                        result.WarningAdd($"Assignment to '{result.GetSpan(currentIndex)}' in the conditional block does not behave as you expected.", currentIndex);
                    }
                    continue;
                }
                else
                {
                    return false;
                }
            }

            if (!tokenList.Last.IsParenLeft(ref source))
            {
                result.ErrorAdd_UnexpectedOperatorToken(currentIndex);
                return false;
            }

            var actionKind = ActionKindHelper.Convert(result.GetSpan(currentIndex));
            result.UnionLast2Tokens();
            switch (actionKind)
            {
                case ActionKind.@return:
                    tokenList.Last.Kind = TokenKind.@return;
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                        return false;
                    }

                    if (tokenList.Last.IsParenRight(ref source))
                    {
                        result.UnionLast2Tokens();
                    }
                    else
                    {
                        CancelTokenReadback(ref context, ref result);
                        result.ErrorAdd("return()'s ')' is not found.", currentIndex);
                    }
                    statements.Add(new ReturnStatement(currentIndex));
                    continue;
                case ActionKind.next:
                    tokenList.Last.Kind = TokenKind.next;
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                        return false;
                    }

                    if (tokenList.Last.IsParenRight(ref source))
                    {
                        result.UnionLast2Tokens();
                    }
                    else
                    {
                        CancelTokenReadback(ref context, ref result);
                        result.ErrorAdd("')' of 'next()' is not found.", currentIndex);
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
                    tokenList.Last.Kind = TokenKind.@break;
                    @while = GetWhile(ref blockStack);
                    if (@while is null && createWarning)
                    {
                        result.WarningAdd("break() statement must be in while loop.", currentIndex);
                    }
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                        return false;
                    }

                    if (tokenList.Last.IsParenRight(ref source))
                    {
                        result.UnionLast2Tokens();
                    }
                    else
                    {
                        result.ErrorAdd("break()'s ')' is not found.", currentIndex);
                        CancelTokenReadback(ref context, ref result);
                    }
                    statements.Add(new BreakStatement(currentIndex, @while));
                    continue;
                case ActionKind.@continue:
                    tokenList.Last.Kind = TokenKind.@continue;
                    @while = GetWhile(ref blockStack);
                    if (@while is null && createWarning)
                    {
                        result.WarningAdd("continue() statement must be in while loop.", currentIndex);
                    }
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                        return false;
                    }

                    if (tokenList.Last.IsParenRight(ref source))
                    {
                        result.UnionLast2Tokens();
                    }
                    else
                    {
                        result.ErrorAdd("continue()'s ')' is not found.", currentIndex);
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
        result.TokenList.Last.Kind = TokenKind.@while;
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
        result.TokenList.Last.Kind = isRepeatIf ? TokenKind.rif : TokenKind.@if;
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

        if (!result.TokenList.Last.Is_else(ref result.Source))
        {
            CancelTokenReadback(ref context, ref result);
            return true;
        }

        result.TokenList.Last.Kind = TokenKind.Else;
        statement.ElseTokenId = result.TokenList.LastIndex;
        statement.HasElseStatement = true;
        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(statement.ElseTokenId);
            return false;
        }

        blockStack.Add(statement);
        if (result.TokenList.Last.IsBracketLeft(ref result.Source))
        {
            answer = Parse_Block(ref context, ref result, ref statement.ElseStatements, ref blockStack);
        }
        else
        {
            var elseifIndex = result.TokenList.LastIndex;
            if (!ReadToken(ref context, ref result) || !result.TokenList.Last.IsParenLeft(ref result.Source))
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
        ref var source = ref result.Source;
        Argument argument = new();
        ref var arguments = ref statement.Arguments;

        for (int i = 0; i != beforeLastCompoundTextArgumentCount; ++i)
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(statement.TokenId);
                return false;
            }

            if (tokenList.Last.IsParenRight(ref source))
            {
                result.ErrorAdd($"{statement.Kind} must have {beforeLastCompoundTextArgumentCount + 1} argument(s) but actually {i} argument(s).", tokenList.LastIndex);
                return true;
            }

            if (tokenList.Last.IsComma(ref source))
            {
                result.ErrorAdd($"{statement.Kind} {i}-th argument does not exist.", tokenList.LastIndex);
                return true;
            }

            tokenList.Last.Kind = TokenKind.Content;
            argument.TokenId = tokenList.LastIndex;
            argument.IsNumber = tokenList.Last.TryParse(ref source, out argument.Number);
            arguments.Add(ref argument);
            ref var lastArgument = ref arguments.Last;
            do
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(statement.TokenId);
                    return false;
                }

                if (tokenList.Last.IsComma(ref source))
                {
                    break;
                }

                if (tokenList.Last.IsParenRight(ref source))
                {
                    result.ErrorAdd($"{statement.Kind} must have {beforeLastCompoundTextArgumentCount + 1} argument(s) but actually {i} argument(s).", tokenList.LastIndex);
                    return true;
                }

                lastArgument.IsNumber = false;
                lastArgument.TrailingTokenCount++;
                tokenList.Last.Kind = TokenKind.ContentTrailing;
            } while (true);
        }

        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(statement.TokenId);
            return false;
        }

        if (tokenList.Last.IsParenRight(ref source))
        {
            result.ErrorAdd("Argument does not exists between ',' and ')'.", tokenList.LastIndex - 1);
            return true;
        }

        tokenList.Last.Kind = TokenKind.Content;
        argument.TokenId = tokenList.LastIndex;
        arguments.Add(ref argument);

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(statement.TokenId);
                return false;
            }

            if (tokenList.Last.IsParenRight(ref source) && tokenList.Last.Range.EndExclusive.Offset == 0)
            {
                return true;
            }

            arguments.Last.IsNumber = false;
            arguments.Last.TrailingTokenCount++;
            tokenList.Last.Kind = TokenKind.ContentTrailing;
        } while (true);
    }

    /// <summary>
    /// Already read '('.
    /// </summary>
    private static bool Parse_CallAction(ref Context context, ref Result result, uint currentIndex, ref List<IStatement> statements, ActionKind actionKind)
    {
        ref var tokenList = ref result.TokenList;
        tokenList[currentIndex].Kind = TokenKind.CallAction;
        tokenList[currentIndex].Other = (uint)actionKind;
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

        ref var source = ref result.Source;
        Argument argument = new();
        ref var arguments = ref statement.Arguments;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                return false;
            }

            if (tokenList.Last.IsParenRight(ref source))
            {
                if (!arguments.IsEmpty)
                {
                    result.ErrorAdd("Argument does not exists between ',' and ')'.", tokenList.LastIndex - 1);
                }

                goto TRUE;
            }

            if (tokenList.Last.IsComma(ref source))
            {
                result.ErrorAdd("Between ',' and ',' nothing is written.", currentIndex);
                continue;
            }

            tokenList.Last.Kind = TokenKind.Content;
            argument.TokenId = tokenList.LastIndex;
            argument.IsNumber = tokenList.Last.TryParse(ref source, out argument.Number);

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                return false;
            }

            arguments.Add(ref argument);
            if (tokenList.Last.IsComma(ref source))
            {
                continue;
            }
            else if (tokenList.Last.IsParenRight(ref source))
            {
                goto TRUE;
            }

            arguments.Last.IsNumber = false;
            arguments.Last.TrailingTokenCount++;
            tokenList.Last.Kind = TokenKind.ContentTrailing;

            do
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(currentIndex);
                    return false;
                }

                if (tokenList.Last.IsComma(ref source))
                {
                    break;
                }
                else if (tokenList.Last.IsParenRight(ref source))
                {
                    goto TRUE;
                }

                arguments.Last.TrailingTokenCount++;
                tokenList.Last.Kind = TokenKind.ContentTrailing;
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
            if (endingRoll is null)
            {
                result.ErrorAdd($"Invalid action {result.GetSpan(currentIndex)}).", currentIndex);
            }
        }
        else if (arguments.Count == 1)
        {
            endingRoll = TryParseCallEndingRollActionStatement(result.GetSpan(currentIndex), currentIndex, arguments[0].TokenId, uint.MaxValue);
            if (endingRoll is null)
            {
                result.ErrorAdd($"Invalid action {result.GetSpan(currentIndex)}{result.GetSpan(arguments[0].TokenId)}).", currentIndex);
            }
        }
        else if (arguments.Count == 2)
        {
            endingRoll = TryParseCallEndingRollActionStatement(result.GetSpan(currentIndex), currentIndex, arguments[0].TokenId, arguments[1].TokenId);
            if (endingRoll is null)
            {
                result.ErrorAdd($"Invalid action {result.GetSpan(currentIndex)}{result.GetSpan(arguments[0].TokenId)}, {result.GetSpan(arguments[1].TokenId)}).", currentIndex);
            }
        }
        else
        {
            result.ErrorAdd($"Invalid action {result.GetSpan(currentIndex)}) or too many ending roll action arguments count of '{result.GetSpan(currentIndex)}'. Exceeding arguments are just ignored.", currentIndex);
        }

        if (endingRoll is not null)
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
