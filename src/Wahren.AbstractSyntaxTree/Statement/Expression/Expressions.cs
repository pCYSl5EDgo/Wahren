namespace Wahren.AbstractSyntaxTree.Statement.Expression;

public sealed class NumberExpression : IReturnNumberExpression
{
    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;

    public uint TokenId { get; set; }
    public int Number { get; set; }

    public NumberExpression(uint tokenId, int number)
    {
        TokenId = tokenId;
        Number = number;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }
    }
}

public sealed class StringExpression : IReturnStringExpression
{
    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
    public uint TokenId { get; set; }

    public StringExpression(uint tokenId)
    {
        TokenId = tokenId;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }
    }
}

public sealed class StringVariableExpression : IReturnStringExpression
{
    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
    public uint ReferenceId = uint.MaxValue;

    public uint TokenId { get; set; }

    public StringVariableExpression(uint tokenId)
    {
        TokenId = tokenId;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }
    }
}

public sealed class IdentifierExpression : ISingleTermExpression
{
    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
    public uint ReferenceId = uint.MaxValue;

    public uint TokenId { get; set; }

    public IdentifierExpression(uint tokenId)
    {
        TokenId = tokenId;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }
    }
}

public enum LogicOperator
{
    Or,
    And,
}

public sealed class LogicOperatorExpression : IReturnBooleanLogicalBooleanBinaryOperatorExpression
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

    public uint TokenId { get; set; }
    public LogicOperator Operator { get; set; }
    public IReturnBooleanExpression Left { get; set; }

    public LogicOperatorExpression(uint tokenId, LogicOperator @operator, IReturnBooleanExpression left)
    {
        TokenId = tokenId;
        Operator = @operator;
        Left = left;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }

        Left.IncrementToken(indexEqualToOrGreaterThan, count);
        right?.IncrementToken(indexEqualToOrGreaterThan, count);
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }

        Left.DecrementToken(indexEqualToOrGreaterThan, count);
        right?.DecrementToken(indexEqualToOrGreaterThan, count);
    }
}

public enum EqualityComparerOperator
{
    Equal,
    NotEqual
}

public sealed record class StringEqualityComparerExpression
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

    public uint TokenId { get; set; }
    public EqualityComparerOperator Operator { get; set; }
    public IReturnStringExpression Left { get; set; }

    public StringEqualityComparerExpression(uint tokenId, EqualityComparerOperator @operator, IReturnStringExpression left)
    {
        TokenId = tokenId;
        Operator = @operator;
        Left = left;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }

        Left.IncrementToken(indexEqualToOrGreaterThan, count);
        right?.IncrementToken(indexEqualToOrGreaterThan, count);
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }

        Left.DecrementToken(indexEqualToOrGreaterThan, count);
        right?.DecrementToken(indexEqualToOrGreaterThan, count);
    }
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

public sealed class NumberComparerExpression
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

    public uint TokenId { get; set; }
    public NumberComparerOperator Operator { get; set; }
    public IReturnNumberExpression Left { get; set; }

    public void IncrementParenCount() => parenCount++;

    public NumberComparerExpression(uint tokenId, NumberComparerOperator @operator, IReturnNumberExpression left)
    {
        TokenId = tokenId;
        Operator = @operator;
        Left = left;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }

        Left.IncrementToken(indexEqualToOrGreaterThan, count);
        right?.IncrementToken(indexEqualToOrGreaterThan, count);
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }

        Left.DecrementToken(indexEqualToOrGreaterThan, count);
        right?.DecrementToken(indexEqualToOrGreaterThan, count);
    }
}

public enum NumberCalculatorOperator
{
    Add,
    Sub,
    Mul,
    Div,
    Percent
}

public sealed class NumberCalculatorOperatorExpression
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

    public uint TokenId { get; set; }
    public NumberCalculatorOperator Operator { get; set; }
    public IReturnNumberExpression Left { get; set; }

    public void IncrementParenCount() => parenCount++;

    public NumberCalculatorOperatorExpression(uint tokenId, NumberCalculatorOperator @operator, IReturnNumberExpression left)
    {
        TokenId = tokenId;
        Operator = @operator;
        Left = left;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }

        Left.IncrementToken(indexEqualToOrGreaterThan, count);
        right?.IncrementToken(indexEqualToOrGreaterThan, count);
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }

        Left.DecrementToken(indexEqualToOrGreaterThan, count);
        right?.DecrementToken(indexEqualToOrGreaterThan, count);
    }
}

public sealed class CallFunctionExpression
    : ISingleTermExpression
{
    public ArrayPoolList<Argument> Arguments = new();

    public void Dispose()
    {
        Arguments.Dispose();
    }

    private uint parenCount;
    public uint ParenCount => parenCount;
    public void IncrementParenCount() => parenCount++;
    
    public uint TokenId { get; set; }
    public FunctionKind Kind { get; set; }

    public CallFunctionExpression(uint tokenId, FunctionKind kind)
    {
        TokenId = tokenId;
        Kind = kind;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }

        foreach (ref var argument in Arguments.AsSpan())
        {
            argument.IncrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }

        foreach (ref var argument in Arguments.AsSpan())
        {
            argument.DecrementToken(indexEqualToOrGreaterThan, count);
        }
    }
}
