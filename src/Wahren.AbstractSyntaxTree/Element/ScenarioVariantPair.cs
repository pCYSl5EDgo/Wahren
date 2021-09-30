namespace Wahren.AbstractSyntaxTree.Element;

public struct VariantPair<T> : IDisposable
    where T : class, IElement
{
    public T? Value;
    public T?[]? ScenarioVariant;

    public VariantPair()
    {
        Value = default;
        ScenarioVariant = null;
    }

    public void Dispose()
    {
        Value = null;
        if (ScenarioVariant is not null)
        {
            Array.Clear(ScenarioVariant);
            ArrayPool<T?>.Shared.Return(ScenarioVariant);
            ScenarioVariant = null;
        }
    }

    public T? TryGetMainOrVariant()
    {
        if (Value is not null)
        {
            return Value;
        }

        if (ScenarioVariant is null)
        {
            return null;
        }

        foreach (var item in ScenarioVariant)
        {
            if (item is not null)
            {
                return item;
            }
        }

        return null;
    }
}
