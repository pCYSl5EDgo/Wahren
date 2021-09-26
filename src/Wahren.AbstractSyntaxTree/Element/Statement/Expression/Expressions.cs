namespace Wahren.AbstractSyntaxTree.Element.Statement.Expression;

public sealed record class NumberExpression(uint TokenId, int Number) : IReturnNumberExpression
{
    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
}

public sealed record class StringExpression(uint TokenId) : IReturnStringExpression
{
    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
}

public sealed record class StringVariableExpression(uint TokenId) : IReturnStringExpression
{
    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
    public uint ReferenceId = uint.MaxValue;
}

public sealed record class IdentifierExpression(uint TokenId) : ISingleTermExpression
{
    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
    public uint ReferenceId = uint.MaxValue;
}

public enum LogicOperator
{
    Or,
    And,
}

public sealed record class LogicOperatorExpression(uint TokenId, LogicOperator Operator, IReturnBooleanExpression Left) : IReturnBooleanLogicalBooleanBinaryOperatorExpression
{
    private IReturnBooleanExpression? right = null;

    public ref IReturnBooleanExpression? Right => ref right;

    IExpression? IBinaryOperatorExpression.Right
    {
        get => right;
        set => right = value as IReturnBooleanExpression;
    }

    IExpression IBinaryOperatorExpression.Left => Left;

    public bool IsLowPriority => Operator == LogicOperator.Or;

    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
}

public enum EqualityComparerOperator
{
    Equal,
    NotEqual
}

public sealed record class StringEqualityComparerExpression(uint TokenId, EqualityComparerOperator Operator, IReturnStringExpression Left)
    : IReturnBooleanExpression, IBinaryOperatorExpression<IReturnStringExpression>
{
    private IReturnStringExpression? right;

    public ref IReturnStringExpression? Right => ref right;

    IExpression IBinaryOperatorExpression.Left => Left;

    IExpression? IBinaryOperatorExpression.Right
    {
        get => right;
        set => right = value as IReturnStringExpression;
    }

    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
}

public enum NumberComparerOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqualTo,
    LessThan,
    LessThanOrEqualTo,
}

public sealed record class NumberComparerExpression(uint TokenId, NumberComparerOperator Operator, IReturnNumberExpression Left)
    : IReturnBooleanCompareNumberBinaryOperatorExpression
{
    private IReturnNumberExpression? right;

    public ref IReturnNumberExpression? Right => ref right;

    IExpression IBinaryOperatorExpression.Left => Left;

    IExpression? IBinaryOperatorExpression.Right
    {
        get => right;
        set => right = value as IReturnNumberExpression;
    }

    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
}

public enum NumberCalculatorOperator
{
    Add,
    Sub,
    Mul,
    Div,
    Percent
}

public sealed record class NumberCalculatorOperatorExpression(uint TokenId, NumberCalculatorOperator Operator, IReturnNumberExpression Left)
    : IReturnNumberBinaryOperatorExpression
{
    private IReturnNumberExpression? right;

    public ref IReturnNumberExpression? Right => ref right;

    public bool IsLowPriority => Operator <= NumberCalculatorOperator.Sub;

    IExpression IBinaryOperatorExpression.Left => Left;

    IExpression? IBinaryOperatorExpression.Right
    {
        get => right;
        set => right = value as IReturnNumberExpression;
    }

    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
}

public sealed record class CallFunctionExpression(uint TokenId, FunctionKind FunctionId)
    : ISingleTermExpression
{
    public List<Argument> Arguments = new();

    public void Dispose()
    {
        Arguments.Dispose();
    }

    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
}
