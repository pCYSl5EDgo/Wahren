﻿namespace Wahren.PooledList;

public struct DualList<T> : IDisposable
{
    private List<T>[] array;
    private int count;

    public DualList()
    {
        array = Array.Empty<List<T>>();
        count = default;
    }

    public ref List<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException($"index should be greater than -1. index : {index}");
            }

            return ref this[(uint)index];
        }
    }

    public ref List<T> this[uint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index >= count)
            {
                throw new IndexOutOfRangeException($"size is limited to {count}. index : {index}");
            }

            return ref array[index];
        }
    }

    public Span<List<T>> AsSpan() => array.AsSpan(0, count);

    public ref List<T> Last => ref count == 0 ? ref Unsafe.NullRef<List<T>>() : ref array[count - 1];

    public uint LastIndex => (uint)count - 1U;

    public void Dispose()
    {
        if (array is null || array.Length == 0)
        {
            return;
        }

        foreach (ref var item in AsSpan())
        {
            item.Dispose();
        }

        ArrayPool<List<T>>.Shared.Return(array);
        array = Array.Empty<List<T>>();
        count = 0;
    }

    public void AddEmpty()
    {
        if (array.Length == 0)
        {
            array = ArrayPool<List<T>>.Shared.Rent(16);
        }
        else if (count == array.Length)
        {
            var newArray = ArrayPool<List<T>>.Shared.Rent(count << 1);
            array.CopyTo(newArray.AsSpan(0, count));
            ArrayPool<List<T>>.Shared.Return(array);
            array = newArray;
        }

        array[count++] = new();
    }

    public int Count => count;

    public void InsertEmpty(int index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        AddEmpty();
        if (index == count - 1)
        {
            return;
        }

        var last = array[count - 1];
        Array.Copy(array, index, array, index + 1, count - index - 1);
        array[index] = last;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count)
        {
            throw new ArgumentOutOfRangeException();
        }

        array[index].Dispose();
        --count;
        if (index == count)
        {
            return;
        }

        Array.Copy(array, index + 1, array, index, count - index);
    }
}
