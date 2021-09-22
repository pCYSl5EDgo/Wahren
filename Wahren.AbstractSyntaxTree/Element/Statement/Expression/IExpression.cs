namespace Wahren.AbstractSyntaxTree.Element.Statement.Expression;

public interface IExpression : IDisposable
{
    uint TokenId { get; }

    bool IsImcomplete() => false;

    void IDisposable.Dispose() { }

    uint ParenCount { get; }

    void IncrementParenCount();
}

public interface IReturnStringExpression : IExpression
{
}

public interface IReturnNumberExpression : IExpression
{
}

public interface IReturnBooleanExpression : IExpression
{
}

public interface ISingleTermExpression : IReturnNumberExpression, IReturnBooleanExpression
{
}

public interface IBinaryOperatorExpression : IExpression
{
    IExpression Left { get; }
    IExpression? Right { get; set; }

    bool IExpression.IsImcomplete()
    {
        var right = Right;
        if (right is null)
        {
            return true;
        }

        if (right is not IBinaryOperatorExpression binary)
        {
            return false;
        }

        do
        {
            var binaryRight = binary.Right;
            if (binaryRight is null)
            {
                return true;
            }

            if (binaryRight is not IBinaryOperatorExpression binaryRightCast)
            {
                return false;
            }
            binary = binaryRightCast;
        } while (true);
    }

    void IDisposable.Dispose()
    {
        Left.Dispose();
        Right?.Dispose();
    }
}

public interface IBinaryOperatorExpression<T> : IBinaryOperatorExpression
{
    new T Left { get; }
    new ref T? Right { get; }

    void AssignToNull(T rightValue)
    {
        ref var right = ref Right;
        if (right is null)
        {
            right = rightValue;
            return;
        }

        var binary = (IBinaryOperatorExpression<T>)right;

        do
        {
            ref var binaryRight = ref binary.Right;
            if (binaryRight is null)
            {
                binaryRight = rightValue;
                return;
            }

            binary = (IBinaryOperatorExpression<T>)binaryRight;
        } while (true);
    }
}

public interface IReturnNumberBinaryOperatorExpression : IBinaryOperatorExpression<IReturnNumberExpression>, IReturnNumberExpression
{
    bool IsLowPriority { get; }
}

public interface IReturnBooleanCompareNumberBinaryOperatorExpression : IBinaryOperatorExpression<IReturnNumberExpression>, IReturnBooleanExpression
{
}

public interface IReturnBooleanLogicalBooleanBinaryOperatorExpression : IBinaryOperatorExpression<IReturnBooleanExpression>, IReturnBooleanExpression
{
    bool IsLowPriority { get; }
}
