﻿[assembly: InternalsVisibleTo("Wahren.Tests")]
namespace Wahren.AbstractSyntaxTree.Parser;

using Element.Statement;
using Element.Statement.Expression;

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
                return condition;
            }

            if (result.ErrorList.IsEmpty || result.ErrorList.Last.Severity != DiagnosticSeverity.Error)
            {
                result.ErrorList.Add(new("Condition Parse Failed.", tokenList[statementTokenId].Range));
            }

            return null;
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
            var span = source[tokenList.Last.Range.StartInclusive.Line].AsSpan(tokenList.Last.Range.StartInclusive.Offset, tokenList.Last.LengthInFirstLine);
            switch (span.Length)
            {
                case 1:
                    switch (span[0])
                    {
                        case ')':
                            tokenList.Last.Kind = TokenKind.ParenRight;
                            return reduce(ref result, ref expressionList, expressionListStartIndex, true);
                        case '@':
                            if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, new StringVariableExpression(currentIndex)))
                            {
                                result.ErrorList.Add(new($"{nameof(AddValueReduce)} failed.", tokenList.Last.Range));
                                return null;
                            }
                            continue;
                        case '(':
                            tokenList.Last.Kind = TokenKind.ParenLeft;
                            if (expressionList.Count < expressionListStartIndex)
                            {
                                result.ErrorList.Add(new($"{nameof(expressionList)}.Count: {expressionList.Count}, ${nameof(expressionListStartIndex)}: ${expressionListStartIndex}", tokenList.Last.Range));
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
                                    result.ErrorList.Add(new($"{nameof(AddValueReduce)} failed.", tokenList.Last.Range));
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
                                                    result.ErrorList.Add(new($"{nameof(AddValueReduce)} failed.", tokenList.Last.Range));
                                                    return null;
                                                }
                                            }
                                            else
                                            {
                                                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, paren))
                                                {
                                                    result.ErrorList.Add(new($"{nameof(AddValueReduce)} failed.", tokenList.Last.Range));
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
                                                result.ErrorList.Add(new($"{nameof(AddValueReduce)} failed.", tokenList.Last.Range));
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
                                result.ErrorList.Add(new($"{span[0]} {nameof(AddValueReduce)} failed.", tokenList.Last.Range));
                                return null;
                            }
                            continue;
                        case '>': tokenList.Last.Kind = TokenKind.CompareGreaterThan; comparerOperator = NumberComparerOperator.GreaterThan; goto COMPARE_OPERATOR;
                        case '<': tokenList.Last.Kind = TokenKind.CompareLessThan; comparerOperator = NumberComparerOperator.LessThan; goto COMPARE_OPERATOR;
                        case '+': tokenList.Last.Kind = TokenKind.Add; calculatorOperator = NumberCalculatorOperator.Add; goto CALC_OPERATOR;
                        case '-': tokenList.Last.Kind = TokenKind.Sub; calculatorOperator = NumberCalculatorOperator.Sub; goto CALC_OPERATOR;
                        case '*': tokenList.Last.Kind = TokenKind.Mul; calculatorOperator = NumberCalculatorOperator.Mul; goto CALC_OPERATOR;
                        case '/': tokenList.Last.Kind = TokenKind.Div; calculatorOperator = NumberCalculatorOperator.Div; goto CALC_OPERATOR;
                        case '%': tokenList.Last.Kind = TokenKind.Percent; calculatorOperator = NumberCalculatorOperator.Percent; goto CALC_OPERATOR;
                        case '{': tokenList.Last.Kind = TokenKind.BracketLeft; result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex); return null;
                        case '}': tokenList.Last.Kind = TokenKind.BracketRight; result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex); return null;
                        case ':': tokenList.Last.Kind = TokenKind.Colon; result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex); return null;
                        case ';': tokenList.Last.Kind = TokenKind.Semicolon; result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex); return null;
                        case ',': tokenList.Last.Kind = TokenKind.Comma; result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex); return null;
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
                                case '=': tokenList.Last.Kind = TokenKind.CompareEqual; comparerOperator = NumberComparerOperator.Equal; goto COMPARE_OPERATOR;
                                case '!': tokenList.Last.Kind = TokenKind.CompareNotEqual; comparerOperator = NumberComparerOperator.NotEqual; goto COMPARE_OPERATOR;
                                case '>': tokenList.Last.Kind = TokenKind.CompareGreaterThanOrEqualTo; comparerOperator = NumberComparerOperator.GreaterThanOrEqualTo; goto COMPARE_OPERATOR;
                                case '<': tokenList.Last.Kind = TokenKind.CompareLessThanOrEqualTo; comparerOperator = NumberComparerOperator.LessThanOrEqualTo; goto COMPARE_OPERATOR;
                                default:
                                    result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex);
                                    return null;
                            }
                        case '&':
                            if (span[0] == '&')
                            {
                                tokenList.Last.Kind = TokenKind.And;
                                if (AddOperatorReduce(ref result, ref expressionList, expressionListStartIndex, currentIndex, isOrExpression: false))
                                {
                                    continue;
                                }

                                result.ErrorList.Add(new($"&& {nameof(AddOperatorReduce)} failed.", tokenList.Last.Range));
                            }
                            else
                            {
                                result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex);
                            }

                            return null;
                        case '|':
                            if (span[0] == '|')
                            {
                                tokenList.Last.Kind = TokenKind.Or;
                                if (AddOperatorReduce(ref result, ref expressionList, expressionListStartIndex, currentIndex, isOrExpression: true))
                                {
                                    continue;
                                }

                                result.ErrorList.Add(new($"|| {nameof(AddOperatorReduce)} failed.", tokenList.Last.Range));
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

            tokenList.Last.Kind = TokenKind.Content;
            if (tokenList.Last.TryParse(ref source, out int number))
            {
                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, new NumberExpression(currentIndex, number)))
                {
                    result.ErrorList.Add(new($"{span} {nameof(AddValueReduce)} failed.", tokenList.Last.Range));
                    return null;
                }

                continue;
            }

            if (context.CreateError(DiagnosticSeverity.Warning) && span[0] == '+' || span[0] == '-')
            {
                result.ErrorList.Add(new("This can cause error.", tokenList[currentIndex].Range, DiagnosticSeverity.Warning));
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(statementTokenId);
                return null;
            }

            if (result.TokenList.Last.IsParenLeft(ref source))
            {
                var functionId = FunctionKindHelper.Convert(span);
                result.UnionLast2Tokens();
                result.TokenList.Last.Kind = TokenKind.CallFunction;
                result.TokenList.Last.Other = (uint)functionId;
                if (functionId == FunctionKind.None)
                {
                    result.ErrorList.Add(new($"{span} is not registered function.", tokenList[currentIndex].Range));
                    return null;
                }

                var callFunctionExpression = new CallFunctionExpression(currentIndex, functionId);
                if (!ExpressionParseCallFunction(ref context, ref result, callFunctionExpression, statementKind))
                {
                    return null;
                }

                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, callFunctionExpression as IReturnNumberExpression))
                {
                    result.ErrorList.Add(new($"{span} {nameof(AddValueReduce)} as number failed.", tokenList.Last.Range));
                    return null;
                }
                continue;
            }

            if (result.TokenList.Last.IsOperator(ref source))
            {
                if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, new IdentifierExpression(currentIndex) as IReturnNumberExpression))
                {
                    result.ErrorList.Add(new($"{span} {nameof(AddValueReduce)} as number identifier failed.", tokenList.Last.Range));
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

                ref var last = ref result.TokenList.Last;
                if (last.LengthInFirstLine == 2)
                {
                    var stringSpan = source[last.Range.StartInclusive.Line].AsSpan(last.Range.StartInclusive.Offset, 2);
                    if (stringSpan[1] == '=')
                    {
                        var operatorIndex = result.TokenList.LastIndex;
                        EqualityComparerOperator @operator;
                        if (stringSpan[0] == '!')
                        {
                            @operator = EqualityComparerOperator.NotEqual;
                            last.Kind = TokenKind.CompareNotEqual;
                        }
                        else
                        {
                            @operator = EqualityComparerOperator.Equal;
                            last.Kind = TokenKind.CompareEqual;
                        }

                        if (!ExpressionParseString(ref context, ref result, statementTokenId, out var right))
                        {
                            result.ErrorList.Add(new("Expression Parsing Error. String is expected as the right argument of string equlity comparer.", tokenList.Last.Range));
                            return null;
                        }

                        expressionList.Add(new StringEqualityComparerExpression(operatorIndex, @operator, new StringExpression(currentIndex))
                        {
                            Right = right,
                        });
                        break;
                    }
                }

                if (last.IsOperator(ref source))
                {
                    if (!AddValueReduce(ref result, ref expressionList, expressionListStartIndex, new StringExpression(currentIndex)))
                    {
                        result.ErrorList.Add(new($"{span} {nameof(AddValueReduce)} failed.", tokenList.Last.Range));
                        return null;
                    }

                    goto PREVIOUS_TOKEN_EXISTS;
                }
            } while (true);

            continue;
        COMPARE_OPERATOR:
            if (AddOperatorReduce(ref result, ref expressionList, expressionListStartIndex, currentIndex, comparerOperator))
            {
                continue;
            }
            else
            {
                result.ErrorList.Add(new($"{comparerOperator} {nameof(AddOperatorReduce)} failed.", tokenList.Last.Range));
                return null;
            }

        CALC_OPERATOR:
            if (AddOperatorReduce(ref result, ref expressionList, expressionListStartIndex, currentIndex, calculatorOperator))
            {
                continue;
            }
            else
            {
                result.ErrorList.Add(new($"{calculatorOperator} {nameof(AddOperatorReduce)} failed.", tokenList.Last.Range));
                return null;
            }
        } while (true);
    }

    private static bool AddOperatorReduce(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, uint currentIndex, bool isOrExpression)
    {
        var left = GetCompleteBoolean(ref result, ref expressionList, expressionListStartIndex, isOrExpression);
        if (left is null)
        {
            return false;
        }

        expressionList.Add(new LogicOperatorExpression(currentIndex, isOrExpression ? LogicOperator.Or : LogicOperator.And, left));
        return true;
    }

    private static bool AddOperatorReduce(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, uint currentIndex, NumberCalculatorOperator op)
    {
        var left = GetCompleteNumber(ref result, ref expressionList, expressionListStartIndex, op <= NumberCalculatorOperator.Sub);
        if (left is null)
        {
            return false;
        }

        expressionList.Add(new NumberCalculatorOperatorExpression(currentIndex, op, left));
        return true;
    }

    private static bool AddOperatorReduce(ref Result result, ref List<IExpression> expressionList, int expressionListStartIndex, uint currentIndex, NumberComparerOperator op)
    {
        var left = GetCompleteNumber(ref result, ref expressionList, expressionListStartIndex, true);
        if (left is null)
        {
            return false;
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

        if (expressionList.Last is NumberComparerExpression comparer && comparer.Right is null && comparer.Left is IdentifierExpression identifier)
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

            ref var last = ref tokenList.Last;
            if (isNotFirstArgument)
            {
                if (last.IsOperator(ref source))
                {
                    result.ErrorList.Add(new("Function argument is not written.", tokenList[callFunctionExpression.TokenId].Range));
                    return false;
                }
            }
            else if (last.IsParenRight(ref source))
            {
                break;
            }

            tokenList.Last.Kind = TokenKind.Content;
            argument.TokenId = tokenList.LastIndex;
            argument.IsNumber = last.TryParse(ref source, out argument.Number);
            callFunctionExpression.Arguments.Add(ref argument);

            if (!ReadToken(ref context, ref result))
            {
                return false;
            }

            if (tokenList.Last.IsComma(ref source))
            {
                continue;
            }
            else if (tokenList.Last.IsParenRight(ref source))
            {
                break;
            }

            result.UnionLast2Tokens();
            result.ErrorList.Add(new("Function argument must be number, identifier, string variable, 1-word-length text.", tokenList.Last.Range));
            return false;
        } while (true);

        var validated = callFunctionExpression.FunctionId.IsValidArgumentCount(callFunctionExpression.Arguments.Count);
        if (validated > 0)
        {
            result.ErrorList.Add(new($"Too many function arguments count of '{callFunctionExpression.FunctionId}'. Exceeding arguments are just ignored.", tokenList[callFunctionExpression.TokenId].Range, DiagnosticSeverity.Warning));
        }
        else if (validated < 0)
        {
            result.ErrorList.Add(new($"Insufficient function arguments count of '{callFunctionExpression.FunctionId}'.", tokenList[callFunctionExpression.TokenId].Range));
        }

        if (callFunctionExpression.FunctionId == FunctionKind.isInterval && statementKind != ConditionStatementKind.Rif)
        {
            result.ErrorList.Add(new("'isInterval' can only be called inside of 'rif' condition expression.", tokenList[callFunctionExpression.TokenId].Range));
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
        tokenList.Last.Kind = TokenKind.Content;
        ref var source = ref result.Source;
        var span = source[tokenList.Last.Range.StartInclusive.Line].AsSpan(tokenList.Last.Range.StartInclusive.Offset, tokenList.Last.LengthInFirstLine);
        switch (span[0])
        {
            case '@':
                value = new StringVariableExpression(tokenList.LastIndex);
                return true;
            case '+':
            case '-':
                if (context.CreateError(DiagnosticSeverity.Warning))
                {
                    result.ErrorList.Add(new("This can cause error.", tokenList.Last.Range, DiagnosticSeverity.Warning));
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

            if (tokenList.Last.IsOperator(ref source))
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            result.UnionLast2Tokens();
        } while (true);
    }
}