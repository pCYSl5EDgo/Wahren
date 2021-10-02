namespace Wahren.AbstractSyntaxTree.Element;

public struct VariantPair<T> : IDisposable
    where T : class, IElement
{
    public T? Value;
    public T?[]? VariantArray;

    public VariantPair()
    {
        Value = default;
        VariantArray = null;
    }

    public void Dispose()
    {
        Value = null;
        if (VariantArray is not null)
        {
            Array.Clear(VariantArray);
            ArrayPool<T?>.Shared.Return(VariantArray);
            VariantArray = null;
        }
    }
}
