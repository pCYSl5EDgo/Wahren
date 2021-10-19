namespace Wahren.PooledList;

public struct StringSpanUIntKeyDictionary<T> : IDisposable
    where T : class
{
    private ArrayPoolList<char> keys;
    private DualList<T?> originalValueDualList;
    private DualList<uint> originalKeyDualList;
    private DualList<T?>[] variantValueDualListArray;
    private DualList<uint>[] variantKeyDualListArray;

    public StringSpanUIntKeyDictionary()
    {
        keys = new();
        originalValueDualList = new();
        originalKeyDualList = new();
        variantValueDualListArray = Array.Empty<DualList<T?>>();
        variantKeyDualListArray = Array.Empty<DualList<uint>>();
    }

    public bool TryGet(ReadOnlySpan<char> key, uint variation, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value)
    {
        if (key.IsEmpty)
        {
            goto FALSE;
        }

        var keyIndex = (uint)key.Length - 1;
        ref var keyList = ref Unsafe.NullRef<ArrayPoolList<uint>>();
        ref var valueList = ref Unsafe.NullRef<ArrayPoolList<T?>>();
        if (variation == uint.MaxValue)
        {
            if (keyIndex >= originalKeyDualList.Count)
            {
                goto FALSE;
            }

            keyList = ref originalKeyDualList[keyIndex];
            valueList = ref originalValueDualList[keyIndex];
        }
        else
        {
            if (variation >= variantKeyDualListArray.Length)
            {
                goto FALSE;
            }

            ref var keyDualList = ref variantKeyDualListArray[variation];
            if (keyIndex >= keyDualList.Count)
            {
                goto FALSE;
            }

            keyList = ref keyDualList[keyIndex];
            valueList = ref variantValueDualListArray[variation][keyIndex];
        }

        for (uint i = 0; i < keyList.Count; i++)
        {
            if (key.SequenceEqual(keys.AsSpan(keyList[i], keyIndex + 1)))
            {
                value = valueList[i];
                return value is not null;
            }
        }

    FALSE:
        value = null;
        return false;
    }

    public bool TryAdd(ReadOnlySpan<char> key, uint variation, T? value)
    {
        if (key.IsEmpty || value is null)
        {
            return false;
        }

        ref ArrayPoolList<T?> valueList = ref Unsafe.NullRef<ArrayPoolList<T?>>();
        ref ArrayPoolList<uint> keyList = ref Unsafe.NullRef<ArrayPoolList<uint>>();
        uint keyLengthIndex = (uint)(key.Length - 1);
        if (variation == uint.MaxValue)
        {
            while (keyLengthIndex >= originalKeyDualList.Count)
            {
                originalKeyDualList.AddEmpty();
            }

            while (keyLengthIndex >= originalValueDualList.Count)
            {
                originalValueDualList.AddEmpty();
            }

            keyList = ref originalKeyDualList[keyLengthIndex];
            valueList = ref originalValueDualList[keyLengthIndex];
        }
        else
        {
            if (variation >= variantKeyDualListArray.Length)
            {
                var rentalValue = ArrayPool<DualList<T?>>.Shared.Rent((int)(variation + 1));
                if (variantValueDualListArray.Length != 0)
                {
                    variantValueDualListArray.CopyTo(rentalValue.AsSpan(0, variantValueDualListArray.Length));
                }
                rentalValue.AsSpan(variantValueDualListArray.Length).Fill(new());
                if (variantValueDualListArray.Length != 0)
                {
                    ArrayPool<DualList<T?>>.Shared.Return(variantValueDualListArray);
                }
                variantValueDualListArray = rentalValue;

                var rentalKey = ArrayPool<DualList<uint>>.Shared.Rent((int)(variation + 1));
                if (variantKeyDualListArray.Length != 0)
                {
                    variantKeyDualListArray.CopyTo(rentalKey.AsSpan(0, variantKeyDualListArray.Length));
                }
                rentalKey.AsSpan(variantKeyDualListArray.Length).Fill(new());
                if (variantKeyDualListArray.Length != 0)
                {
                    ArrayPool<DualList<uint>>.Shared.Return(variantKeyDualListArray);
                }
                variantKeyDualListArray = rentalKey;
            }

            ref var keyDualList = ref variantKeyDualListArray[variation];
            ref var valueDualList = ref variantValueDualListArray[variation];

            while (keyLengthIndex >= keyDualList.Count)
            {
                keyDualList.AddEmpty();
            }

            while (keyLengthIndex >= valueDualList.Count)
            {
                valueDualList.AddEmpty();
            }

            keyList = ref keyDualList[keyLengthIndex];
            valueList = ref valueDualList[keyLengthIndex];
        }

        for (uint i = 0; i < keyList.Count; i++)
        {
            if (key.SequenceEqual(keys.AsSpan(keyList[i], keyLengthIndex + 1)))
            {
                ref var target = ref valueList[i];
                if (target is null)
                {
                    target = value;
                    return true;
                }

                return false;
            }
        }

        var index = keys.AsSpan().IndexOf(key);
        if (index == -1)
        {
            index = keys.Count;
            keys.AddRange(key);
        }

        keyList.Add((uint)index);
        valueList.Add(value);
        return true;
    }

    public void Dispose()
    {
        keys.Dispose();
        originalValueDualList.Dispose();
        originalKeyDualList.Dispose();
        if (variantValueDualListArray is not null && variantValueDualListArray != Array.Empty<DualList<T?>>())
        {
            for (int i = 0; i < variantValueDualListArray.Length; i++)
            {
                variantValueDualListArray[i].Dispose();
            }

            ArrayPool<DualList<T?>>.Shared.Return(variantValueDualListArray);
        }

        if (variantKeyDualListArray is not null && variantKeyDualListArray != Array.Empty<DualList<uint>>())
        {
            for (int i = 0; i < variantKeyDualListArray.Length; i++)
            {
                variantKeyDualListArray[i].Dispose();
            }

            ArrayPool<DualList<uint>>.Shared.Return(variantKeyDualListArray);
        }
    }
}

public struct StringSpanKeyDictionary<T> : IDisposable
    where T : class
{
    private ArrayPoolList<char> keys;
    private DualList<T?> originalValueDualList;
    private DualList<uint> originalKeyDualList;

    public StringSpanKeyDictionary()
    {
        keys = new();
        originalValueDualList = new();
        originalKeyDualList = new();
    }

    public ref T? TryGetRef(ReadOnlySpan<char> key)
    {
        if (key.IsEmpty)
        {
            goto FALSE;
        }

        var keyIndex = (uint)key.Length - 1;

        if (keyIndex >= originalKeyDualList.Count)
        {
            goto FALSE;
        }

        ref var keyList = ref originalKeyDualList[keyIndex];
        ref var valueList = ref originalValueDualList[keyIndex];

        for (uint i = 0; i < keyList.Count; i++)
        {
            if (key.SequenceEqual(keys.AsSpan(keyList[i], keyIndex + 1)))
            {
                return ref valueList[i];
            }
        }

    FALSE:
        return ref Unsafe.NullRef<T?>();
    }

    public bool TryAdd(ReadOnlySpan<char> key, T value)
    {
        if (key.IsEmpty)
        {
            return false;
        }

        uint keyLengthIndex = (uint)(key.Length - 1);
        while (keyLengthIndex >= originalKeyDualList.Count)
        {
            originalKeyDualList.AddEmpty();
        }

        while (keyLengthIndex >= originalValueDualList.Count)
        {
            originalValueDualList.AddEmpty();
        }

        ref var keyList = ref originalKeyDualList[keyLengthIndex];
        ref var valueList = ref originalValueDualList[keyLengthIndex];

        for (uint i = 0; i < keyList.Count; i++)
        {
            if (key.SequenceEqual(keys.AsSpan(keyList[i], keyLengthIndex + 1)))
            {
                ref var target = ref valueList[i];
                if (target is null)
                {
                    target = value;
                    return true;
                }

                return false;
            }
        }

        var index = keys.AsSpan().IndexOf(key);
        if (index == -1)
        {
            index = keys.Count;
            keys.AddRange(key);
        }

        keyList.Add((uint)index);
        valueList.Add(value);
        return true;
    }

    public void Dispose()
    {
        keys.Dispose();
        originalValueDualList.Dispose();
        originalKeyDualList.Dispose();
    }
}
