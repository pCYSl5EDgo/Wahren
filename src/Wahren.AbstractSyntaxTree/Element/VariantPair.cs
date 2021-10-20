namespace Wahren.AbstractSyntaxTree.Element;

public struct VariantPair<T> : IDisposable, ITokenIdModifiable
    where T : class, IElement
{
    public T? Value = null;
    public T[] VariantArray = Array.Empty<T>();
    public ulong[] HashArray = Array.Empty<ulong>();
    public int Count = 0;

    public Span<T> Variants => VariantArray.AsSpan(0, Count);
    public Span<ulong> HashSpan => HashArray.AsSpan(0, Count);

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        Value?.IncrementToken(indexEqualToOrGreaterThan, count);
        foreach (ref var item in Variants)
        {
            item.IncrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        Value?.DecrementToken(indexEqualToOrGreaterThan, count);
        foreach (ref var item in Variants)
        {
            item.DecrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void EnsureCapacity()
    {
        if (Count == VariantArray.Length)
        {
            var tmp = ArrayPool<T>.Shared.Rent(Count + 1);
            if (VariantArray.Length != 0)
            {
                VariantArray.CopyTo(tmp, 0);
                Array.Clear(VariantArray);
            }
            VariantArray = tmp;
        }

        if (Count == HashArray.Length)
        {
            var tmp = ArrayPool<ulong>.Shared.Rent(Count + 1);
            if (HashArray.Length != 0)
            {
                HashArray.CopyTo(tmp, 0);
            }
            HashArray = tmp;
        }
    }

    public void Dispose()
    {
        if (Value is IDisposable disposable)
        {
            disposable.Dispose();
        }

        Value = default;

        if (VariantArray is { Length: > 0 })
        {
            foreach (ref var variant in Variants)
            {
                if (variant is IDisposable variantDispoable)
                {
                    variantDispoable.Dispose();
                }
            }

            Variants.Clear();
            ArrayPool<T>.Shared.Return(VariantArray);
            VariantArray = Array.Empty<T>();
        }

        if (HashArray is { Length: > 0 })
        {
            ArrayPool<ulong>.Shared.Return(HashArray);
            HashArray = Array.Empty<ulong>();
        }

        Count = 0;
    }
}
