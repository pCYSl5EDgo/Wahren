[assembly: InternalsVisibleTo("Wahren.Tests")]
namespace Wahren.AbstractSyntaxTree.Parser;

using Statement;
using Statement.Expression;

public static partial class Parser
{
    delegate T? StartReduce<T>(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, bool isLowPriority) where T : class, IExpression;

    /// <summary>
    /// Already read 'if/rif/while' and '('.
    /// </summary>
    /// <returns></returns>
    internal static IReturnBooleanExpression? ParseCondition(ref Context context, ref Result result, uint statementTokenId, ConditionStatementKind statementKind)
    {
        ref var tokenList = ref result.TokenList;
        ref var source = ref result.Source;
        List<IExpression> expressionList = new();
        try
        {
            var condition = ExpressionParseInParen(ref context, ref result, ref expressionList, 0, statementTokenId, statementKind, GetCompleteBoolean);
            if (expressionList.IsEmpty && condition is not null)
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(statementTokenId, "'{' of while statement is not found.");
                    return null;
                }

                if (!result.IsBracketLeft(tokenList.LastIndex))
                {
                    CancelTokenReadback(ref context, ref result);
                    result.ErrorAdd("'{' of while/if/rif statement is not found.", statementTokenId);
                }

                return condition;
            }

            if (result.ErrorList.IsEmpty || result.ErrorList.Last.Severity != DiagnosticSeverity.Error)
            {
#if JAPANESE
                result.ErrorAdd("C条件式の解釈に失敗しました。", statementTokenId);
#else
                result.ErrorAdd("Condition Parse Failed.", statementTokenId);
#endif
            }

            do
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(statementTokenId, "'{' of while/if/rif statement is not found.");
                    return null;
                }
            } while (!result.IsBracketLeft(tokenList.LastIndex));
            return condition;
        }
        finally
        {
            expressionList.Dispose();
        }
    }

    private static T? ExpressionParseInParen<T>(ref Context context, ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, uint statementTokenId, ConditionStatementKind statementKind, StartReduce<T> reduce)
        where T : class, IExpression
    {
        ref var tokenList = ref result.TokenList;
        ref var source = ref result.Source;
        NumberComparerOperator comparerOperator = default;
        NumberCalculatorOperator calculatorOperator = default;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(statementTokenId);
                return null;
            }

        PREVIOUS_TOKEN_EXISTS:
            var currentIndex = tokenList.LastIndex;
            var span = result.GetSpan(tokenList.LastIndex);
            switch (span.Length)
            {
                case 1:
                    switch (span[0])
                    {
                        case ')':
                            tokenList.GetKind(tokenList.LastIndex) = TokenKind.ParenRight;
                            return reduce(ref result, ref expressionList, expressionListStartIndex, true);
                        case '@':
                            if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, new StringVariableExpression(currentIndex)))
                            {
                                result.ErrorAdd($"{nameof(AddValueReduce)} failed.", tokenList.LastIndex);
                                return null;
                            }
                            continue;
                        case '(':
                            tokenList.GetKind(tokenList.LastIndex) = TokenKind.ParenLeft;
                            if (expressionList.Count < expressionListStartIndex)
                            {
                                result.ErrorAdd($"{nameof(expressionList)}.Count: {expressionList.Count}, ${nameof(expressionListStartIndex)}: ${expressionListStartIndex}", tokenList.LastIndex);
                                return null;
                            }
                            else if (expressionList.Count == expressionListStartIndex)
                            {
                                var paren = ExpressionParseInParen(ref context, ref result, ref expressionList, expressionList.Count, statementTokenId, statementKind, GetUnknown);
                                if (paren is null)
                                {
                                    return null;
                                }

                                paren.IncrementParenCount();
                                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, paren))
                                {
                                    result.ErrorAdd($"{nameof(AddValueReduce)} failed.", tokenList.LastIndex);
                                    return null;
                                }
                                continue;
                            }
                            else
                            {
                                switch (expressionList.Last)
                                {
                                    case IBinaryOperatorExpression<IReturnBooleanExpression>:
                                        {
                                            var paren = ExpressionParseInParen(ref context, ref result, ref expressionList, expressionList.Count, statementTokenId, statementKind, GetCompleteBoolean);
                                            if (paren is null)
                                            {
                                                return null;
                                            }

                                            paren.IncrementParenCount();
                                            if (paren is IdentifierExpression identifierExpression)
                                            {
                                                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, identifierExpression as IReturnNumberExpression))
                                                {
                                                    result.ErrorAdd($"{nameof(AddValueReduce)} failed.", tokenList.LastIndex);
                                                    return null;
                                                }
                                            }
                                            else
                                            {
                                                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, paren))
                                                {
                                                    result.ErrorAdd($"{nameof(AddValueReduce)} failed.", tokenList.LastIndex);
                                                    return null;
                                                }
                                            }

                                            continue;
                                        }
                                    case IBinaryOperatorExpression<IReturnNumberExpression>:
                                        {
                                            var paren = ExpressionParseInParen(ref context, ref result, ref expressionList, expressionList.Count, statementTokenId, statementKind, GetCompleteNumber);
                                            if (paren is null)
                                            {
                                                return null;
                                            }

                                            paren.IncrementParenCount();
                                            if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, paren))
                                            {
                                                result.ErrorAdd($"{nameof(AddValueReduce)} failed.", tokenList.LastIndex);
                                                return null;
                                            }

                                            continue;
                                        }
                                }
                                continue;
                            }
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, new NumberExpression(currentIndex, span[0] - '0')))
                            {
                                result.ErrorAdd($"{span[0]} {nameof(AddValueReduce)} failed.", tokenList.LastIndex);
                                return null;
                            }
                            continue;
                        case '>': tokenList.GetKind(tokenList.LastIndex) = TokenKind.CompareGreaterThan; comparerOperator = NumberComparerOperator.GreaterThan; goto COMPARE_OPERATOR;
                        case '<': tokenList.GetKind(tokenList.LastIndex) = TokenKind.CompareLessThan; comparerOperator = NumberComparerOperator.LessThan; goto COMPARE_OPERATOR;
                        case '+': tokenList.GetKind(tokenList.LastIndex) = TokenKind.Add; calculatorOperator = NumberCalculatorOperator.Add; goto CALC_OPERATOR;
                        case '-': tokenList.GetKind(tokenList.LastIndex) = TokenKind.Sub; calculatorOperator = NumberCalculatorOperator.Sub; goto CALC_OPERATOR;
                        case '*': tokenList.GetKind(tokenList.LastIndex) = TokenKind.Mul; calculatorOperator = NumberCalculatorOperator.Mul; goto CALC_OPERATOR;
                        case '/': tokenList.GetKind(tokenList.LastIndex) = TokenKind.Div; calculatorOperator = NumberCalculatorOperator.Div; goto CALC_OPERATOR;
                        case '%': tokenList.GetKind(tokenList.LastIndex) = TokenKind.Percent; calculatorOperator = NumberCalculatorOperator.Percent; goto CALC_OPERATOR;
                        case '}': tokenList.GetKind(tokenList.LastIndex) = TokenKind.BracketRight; result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex); return null;
                        case ':': tokenList.GetKind(tokenList.LastIndex) = TokenKind.Colon; result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex); return null;
                        case ';': tokenList.GetKind(tokenList.LastIndex) = TokenKind.Semicolon; result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex); return null;
                        case ',': tokenList.GetKind(tokenList.LastIndex) = TokenKind.Comma; result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex); return null;
                        case '{':
                            CancelTokenReadback(ref context, ref result);
                            return null;
                    }
                    break;
                case 2:
                    switch (span[1])
                    {
                        case '=':
                            if (expressionListStartIndex >= expressionList.Count)
                            {
                                return null;
                            }

                            switch (span[0])
                            {
                                case '=': tokenList.GetKind(tokenList.LastIndex) = TokenKind.CompareEqual; comparerOperator = NumberComparerOperator.Equal; goto COMPARE_OPERATOR;
                                case '!': tokenList.GetKind(tokenList.LastIndex) = TokenKind.CompareNotEqual; comparerOperator = NumberComparerOperator.NotEqual; goto COMPARE_OPERATOR;
                                case '>': tokenList.GetKind(tokenList.LastIndex) = TokenKind.CompareGreaterThanOrEqualTo; comparerOperator = NumberComparerOperator.GreaterThanOrEqualTo; goto COMPARE_OPERATOR;
                                case '<': tokenList.GetKind(tokenList.LastIndex) = TokenKind.CompareLessThanOrEqualTo; comparerOperator = NumberComparerOperator.LessThanOrEqualTo; goto COMPARE_OPERATOR;
                                default:
                                    result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex);
                                    return null;
                            }
                        case '&':
                            if (span[0] == '&')
                            {
                                tokenList.GetKind(tokenList.LastIndex) = TokenKind.And;
                                if (AddOperatorReduce(ref context, ref result, ref expressionList, expressionListStartIndex, currentIndex, isOrExpression: false))
                                {
                                    continue;
                                }

                                result.ErrorAdd($"&& {nameof(AddOperatorReduce)} failed.", tokenList.LastIndex);
                            }
                            else
                            {
                                result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex);
                            }

                            return null;
                        case '|':
                            if (span[0] == '|')
                            {
                                tokenList.GetKind(tokenList.LastIndex) = TokenKind.Or;
                                if (AddOperatorReduce(ref context, ref result, ref expressionList, expressionListStartIndex, currentIndex, isOrExpression: true))
                                {
                                    continue;
                                }

                                result.ErrorAdd($"|| {nameof(AddOperatorReduce)} failed.", tokenList.LastIndex);
                            }
                            else
                            {
                                result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex);
                            }
                            return null;
                        case '*':
                        case '/':
                        case '+':
                            result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex);
                            return null;
                    }
                    break;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            if (result.TryParse(tokenList.LastIndex, out int number))
            {
                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, new NumberExpression(currentIndex, number)))
                {
                    result.ErrorAdd($"{span} {nameof(AddValueReduce)} failed.", tokenList.LastIndex);
                    return null;
                }

                continue;
            }

            if (context.CreateError(DiagnosticSeverity.Warning) && span[0] == '+' || span[0] == '-')
            {
                result.WarningAdd("This can cause error.", currentIndex);
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(statementTokenId);
                return null;
            }

            if (result.IsParenLeft(tokenList.LastIndex))
            {
                var functionId = FunctionKindHelper.Convert(span);
                result.UnionLast2Tokens();
                tokenList.GetKind(tokenList.LastIndex) = TokenKind.CallFunction;
                tokenList.GetOther(tokenList.LastIndex) = (uint)functionId;
                if (functionId == FunctionKind.None)
                {
                    result.ErrorAdd_UnexpectedCall(currentIndex);
                    return null;
                }

                var callFunctionExpression = new CallFunctionExpression(currentIndex, functionId);
                if (!ExpressionParseCallFunction(ref context, ref result, callFunctionExpression, statementKind))
                {
                    return null;
                }

                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, callFunctionExpression as IReturnNumberExpression))
                {
                    result.ErrorAdd($"{span} {nameof(AddValueReduce)} as number failed.", tokenList.LastIndex);
                    return null;
                }
                continue;
            }

            if (result.IsOperator(tokenList.LastIndex))
            {
                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, new IdentifierExpression(currentIndex) as IReturnNumberExpression))
                {
                    result.ErrorAdd($"{span} {nameof(AddValueReduce)} as number identifier failed.", tokenList.LastIndex);
                    return null;
                }

                goto PREVIOUS_TOKEN_EXISTS;
            }

            do
            {
                result.UnionLast2Tokens();
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(statementTokenId);
                    return null;
                }

                var lastIndex = tokenList.LastIndex;
                if (tokenList.GetLength(lastIndex) == 2)
                {
                    var stringSpan = result.GetSpan(lastIndex);
                    if (stringSpan[1] == '=')
                    {
                        var operatorIndex = tokenList.LastIndex;
                        EqualityComparerOperator @operator;
                        if (stringSpan[0] == '!')
                        {
                            @operator = EqualityComparerOperator.NotEqual;
                            tokenList.GetKind(lastIndex) = TokenKind.CompareNotEqual;
                        }
                        else
                        {
                            @operator = EqualityComparerOperator.Equal;
                            tokenList.GetKind(lastIndex) = TokenKind.CompareEqual;
                        }

                        if (!ExpressionParseString(ref context, ref result, statementTokenId, out var right))
                        {
                            result.ErrorAdd("Expression Parsing Error. String is expected as the right argument of string equlity comparer.", tokenList.LastIndex);
                            return null;
                        }

                        expressionList.Add(new StringEqualityComparerExpression(operatorIndex, @operator, new StringExpression(currentIndex))
                        {
                            Right = right,
                        });
                        break;
                    }
                }

                if (result.IsOperator(lastIndex))
                {
                    if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, new StringExpression(currentIndex)))
                    {
                        result.ErrorAdd($"{span} {nameof(AddValueReduce)} failed.", tokenList.LastIndex);
                        return null;
                    }

                    goto PREVIOUS_TOKEN_EXISTS;
                }
            } while (true);

            continue;
        COMPARE_OPERATOR:
            if (AddOperatorReduce(ref context, ref result, ref expressionList, expressionListStartIndex, currentIndex, comparerOperator))
            {
                continue;
            }
            else
            {
                result.ErrorAdd($"{comparerOperator} {nameof(AddOperatorReduce)} failed.", tokenList.LastIndex);
                return null;
            }

        CALC_OPERATOR:
            if (AddOperatorReduce(ref context, ref result, ref expressionList, expressionListStartIndex, currentIndex, calculatorOperator))
            {
                continue;
            }
            else
            {
                result.ErrorAdd($"{calculatorOperator} {nameof(AddOperatorReduce)} failed.", tokenList.LastIndex);
                return null;
            }
        } while (true);
    }

    private static bool AddOperatorReduce(ref Context context, ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, uint currentIndex, bool isOrExpression)
    {
        var left = GetCompleteBoolean(ref result, ref expressionList, expressionListStartIndex, isOrExpression);
        if (left is null)
        {
            return false;
        }

        expressionList.Add(new LogicOperatorExpression(currentIndex, isOrExpression ? LogicOperator.Or : LogicOperator.And, left));
        return true;
    }

    private static bool AddOperatorReduce(ref Context context, ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, uint currentIndex, NumberCalculatorOperator op)
    {
        var left = GetCompleteNumber(ref result, ref expressionList, expressionListStartIndex, op <= NumberCalculatorOperator.Sub);
        if (left is null)
        {
            return false;
        }

        expressionList.Add(new NumberCalculatorOperatorExpression(currentIndex, op, left));
        return true;
    }

    private static bool AddOperatorReduce(ref Context context, ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, uint currentIndex, NumberComparerOperator op)
    {
        var left = GetCompleteNumber(ref result, ref expressionList, expressionListStartIndex, true);
        if (left is null)
        {
            return false;
        }

        if ((op == NumberComparerOperator.Equal || op == NumberComparerOperator.NotEqual) && left is IdentifierExpression identifier)
        {
            var span = result.GetSpan(identifier.TokenId);
            if (span.Length != 0 && span[0] == '@')
            {
                if (!ExpressionParseString(ref context, ref result, currentIndex, out var right))
                {
                    result.ErrorAdd("Expression Parsing Error. String is expected as the right argument of string equlity comparer.", result.TokenList.LastIndex);
                    return false;
                }

                expressionList.Add(new StringEqualityComparerExpression(currentIndex, (EqualityComparerOperator)(int)op, new StringExpression(identifier.TokenId))
                {
                    Right = right,
                });
                return true;
            }
        }

        expressionList.Add(new NumberComparerExpression(currentIndex, op, left));
        return true;
    }

    private static bool AddValueReduce(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, IExpression expression)
    {
        if (expressionListStartIndex >= expressionList.Count)
        {
            goto TRUE;
        }

        switch (expression)
        {
            case IReturnBooleanExpression boolean:
                return AddValueReduce(ref result, ref expressionList, expressionListStartIndex, boolean);
            case IReturnNumberExpression number:
                return AddValueReduce(ref result, ref expressionList, expressionListStartIndex, number);
        }

    TRUE:
        expressionList.Add(expression);
        return true;
    }

    private static bool AddValueReduce(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, IReturnBooleanExpression expression)
    {
        if (expressionListStartIndex >= expressionList.Count)
        {
            goto TRUE;
        }

        if (expressionList.Last is IReturnBooleanLogicalBooleanBinaryOperatorExpression binary)
        {
            if (!binary.IsLowPriority && binary.IsImcomplete())
            {
                binary.AssignToNull(expression);
                return true;
            }

            if (expressionListStartIndex + 1 < expressionList.Count && expressionList[expressionList.LastIndex - 1] is IReturnBooleanLogicalBooleanBinaryOperatorExpression preBinary)
            {
                if (preBinary.IsImcomplete())
                {
                    expressionList.RemoveLast();
                    preBinary.AssignToNull(binary);
                }
            }
        }

    TRUE:
        expressionList.Add(expression);
        return true;
    }

    private static bool AddValueReduce(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, IReturnStringExpression expression)
    {
        if (expressionListStartIndex >= expressionList.Count)
        {
            goto TRUE;
        }

        if (expressionList.Last is StringEqualityComparerExpression stringComparer && stringComparer.Right is null)
        {
            stringComparer.Right = expression;
            return true;
        }
        else if (expressionList.Last is NumberComparerExpression comparer && comparer.Right is null && comparer.Left is IdentifierExpression identifier)
        {
            EqualityComparerOperator op;
            switch (comparer.Operator)
            {
                case NumberComparerOperator.Equal:
                    op = EqualityComparerOperator.Equal;
                    break;
                case NumberComparerOperator.NotEqual:
                    op = EqualityComparerOperator.NotEqual;
                    break;
                default:
                    expressionList.Add(expression);
                    return false;
            }

            expressionList.Last = new StringEqualityComparerExpression(comparer.TokenId, op, new StringExpression(identifier.TokenId))
            {
                Right = expression
            };
            return true;
        }

    TRUE:
        expressionList.Add(expression);
        return true;
    }

    private static bool AddValueReduce(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, IReturnNumberExpression expression)
    {
        if (expressionListStartIndex >= expressionList.Count)
        {
            goto TRUE;
        }

        if (expressionList.Last is IReturnNumberBinaryOperatorExpression binary)
        {
            if (!binary.IsLowPriority && binary.IsImcomplete())
            {
                binary.AssignToNull(expression);
                return true;
            }

            if (expressionListStartIndex + 1 < expressionList.Count && expressionList[expressionList.LastIndex - 1] is IReturnNumberBinaryOperatorExpression preBinary)
            {
                if (preBinary.IsImcomplete())
                {
                    expressionList.RemoveLast();
                    preBinary.AssignToNull(binary);
                }
            }
        }

    TRUE:
        expressionList.Add(expression);
        return true;
    }

    /// <summary>
    /// '(' is already read.
    /// </summary>
    private static bool ExpressionParseCallFunction(ref Context context, ref Result result, CallFunctionExpression callFunctionExpression, ConditionStatementKind statementKind)
    {
        Argument argument = default;
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        bool isNotFirstArgument = false;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                return false;
            }

            if (isNotFirstArgument)
            {
                if (result.IsOperator(tokenList.LastIndex))
                {
                    result.ErrorAdd("Function argument is not written.", callFunctionExpression.TokenId);
                    return false;
                }
            }
            else if (result.IsParenRight(tokenList.LastIndex))
            {
                break;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
            argument = new();
            argument.TokenId = tokenList.LastIndex;
            argument.IsNumber = result.TryParse(tokenList.LastIndex, out argument.Number);
            callFunctionExpression.Arguments.Add(ref argument);

            if (!ReadToken(ref context, ref result))
            {
                return false;
            }

            if (result.IsComma(tokenList.LastIndex))
            {
                continue;
            }
            else if (result.IsParenRight(tokenList.LastIndex))
            {
                break;
            }

            tokenList.GetKind(tokenList.LastIndex) = TokenKind.ContentTrailing;
            argument.IsNumber = false;
            argument.TrailingTokenCount++;
            result.ErrorAdd("Function argument must be number, identifier, string variable, 1-word-length text.", tokenList.LastIndex);
            return false;
        } while (true);

        if (callFunctionExpression.Kind == FunctionKind.isInterval && statementKind != ConditionStatementKind.Rif)
        {
            result.ErrorAdd("'isInterval' can only be called inside of 'rif' condition expression.", callFunctionExpression.TokenId);
        }

        return true;
    }

    private static IReturnBooleanExpression? GetCompleteBoolean(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, bool isLowPriority)
    {
        if (expressionListStartIndex >= expressionList.Count)
        {
            return null;
        }

        var last = expressionList.Last;
        if (last.IsImcomplete())
        {
            return null;
        }

        switch (last)
        {
            case IReturnNumberExpression:
                {
                    var number = GetCompleteNumber(ref result, ref expressionList, expressionListStartIndex, true)!;
                    if (expressionListStartIndex < expressionList.Count && expressionList.Last is IReturnBooleanCompareNumberBinaryOperatorExpression pre && pre.IsImcomplete())
                    {
                        pre.AssignToNull(number);
                        return GetCompleteBoolean(ref result, ref expressionList, expressionListStartIndex, isLowPriority);
                    }
                    else if (number is ISingleTermExpression singleTerm)
                    {
                        if (expressionListStartIndex >= expressionList.Count || expressionList.Last is not IReturnBooleanLogicalBooleanBinaryOperatorExpression preLogical || !preLogical.IsImcomplete())
                        {
                            return singleTerm;
                        }

                        if (!isLowPriority && preLogical.IsLowPriority)
                        {
                            return singleTerm;
                        }

                        expressionList.RemoveLast();
                        preLogical.AssignToNull(singleTerm);
                        if (AddValueReduce(ref result, ref expressionList, expressionListStartIndex, preLogical))
                        {
                            return GetCompleteBoolean(ref result, ref expressionList, expressionListStartIndex, isLowPriority);
                        }

                        return null;
                    }

                    expressionList.Add(number);
                    return null;
                }
            case IReturnBooleanExpression boolean:
                {
                    var itr = boolean;
                    do
                    {
                        expressionList.RemoveLast();
                        if (expressionListStartIndex >= expressionList.Count || expressionList.Last is not IReturnBooleanLogicalBooleanBinaryOperatorExpression pre || !pre.IsImcomplete())
                        {
                            return itr;
                        }

                        if (!isLowPriority && pre.IsLowPriority)
                        {
                            return itr;
                        }

                        pre.AssignToNull(itr);
                        itr = pre;
                    } while (true);
                }
        }

        return null;
    }

    private static IReturnNumberExpression? GetCompleteNumber(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, bool isLowPriority)
    {
        if (expressionListStartIndex >= expressionList.Count)
        {
            return null;
        }

        var last = expressionList.Last;
        if (last.IsImcomplete())
        {
            return null;
        }

        expressionList.RemoveLast();
        if (last is not IReturnNumberExpression number)
        {
            expressionList.Add(last);
            return null;
        }

        if (expressionListStartIndex >= expressionList.Count || expressionList.Last is not IReturnNumberBinaryOperatorExpression pre || !pre.IsImcomplete())
        {
            return number;
        }

        if (!isLowPriority && pre.IsLowPriority)
        {
            return number;
        }

        expressionList.RemoveLast();
        pre.AssignToNull(number);
        return pre;
    }

    private static IExpression? GetUnknown(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, bool isLowPrioty)
    {
        if (expressionListStartIndex >= expressionList.Count)
        {
            return null;
        }

        var last = expressionList.Last;
        switch (last)
        {
            case IReturnNumberExpression:
                return GetCompleteNumber(ref result, ref expressionList, expressionListStartIndex, isLowPrioty);
            case IReturnBooleanExpression:
                return GetCompleteBoolean(ref result, ref expressionList, expressionListStartIndex, isLowPrioty);
            default:
                expressionList.RemoveLast();
                return last;
        }
    }

    private static bool ExpressionParseString(ref Context context, ref Result result, uint statementTokenId, [NotNullWhen(true)] out IReturnStringExpression? value)
    {
        value = default;
        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(statementTokenId);
            return false;
        }

        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
        ref var source = ref result.Source;
        var span = result.GetSpan(tokenList.LastIndex);
        switch (span[0])
        {
            case '@':
                value = new StringVariableExpression(tokenList.LastIndex);
                return true;
            case '+':
            case '-':
                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.WarningAdd("This can cause error.", tokenList.LastIndex);
                }
                break;
        }

        value = new StringExpression(tokenList.LastIndex);
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(statementTokenId);
                return false;
            }

            if (result.IsOperator(tokenList.LastIndex))
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            result.UnionLast2Tokens();
        } while (true);
    }
}
