namespace Wahren.AbstractSyntaxTree.Element;

public struct ScenarioVariantPair<T> : IDisposable
    where T : class, IElement
{
    public T? Value;
    public T?[]? ScenarioVariant;

    public ScenarioVariantPair()
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
}
