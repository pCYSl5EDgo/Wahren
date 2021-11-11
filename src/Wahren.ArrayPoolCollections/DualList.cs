namespace Wahren.ArrayPoolCollections;

public struct DualList<T> : IDisposable
{
    private ArrayPoolList<T>[] array;
    private int count;

    public DualList()
    {
        array = Array.Empty<ArrayPoolList<T>>();
        count = default;
    }

    public ref ArrayPoolList<T> this[int index]
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

    public ref ArrayPoolList<T> this[uint index]
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

    public Span<ArrayPoolList<T>> AsSpan() => array.AsSpan(0, count);

    public ref ArrayPoolList<T> Last => ref count == 0 ? ref Unsafe.NullRef<ArrayPoolList<T>>() : ref array[count - 1];

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

        ArrayPool<ArrayPoolList<T>>.Shared.Return(array);
        array = Array.Empty<ArrayPoolList<T>>();
        count = 0;
    }

    public void AddEmpty()
    {
        if (array.Length == 0)
        {
            array = ArrayPool<ArrayPoolList<T>>.Shared.Rent(16);
        }
        else if (count == array.Length)
        {
            var newArray = ArrayPool<ArrayPoolList<T>>.Shared.Rent(count << 1);
            array.CopyTo(newArray.AsSpan(0, count));
            ArrayPool<ArrayPoolList<T>>.Shared.Return(array);
            array = newArray;
        }

        array[count++] = new();
    }

    public int Count => count;

    public void InsertEmpty(uint index)
    {
        AddEmpty();
        if (index == count - 1)
        {
            return;
        }

        var last = array[count - 1];
        Array.Copy(array, index, array, index + 1, count - index - 1);
        array[index] = last;
    }

    public void RemoveAt(uint index)
    {
        if (index >= count)
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

    public void RemoveRange(uint startLine, uint startOffsetInclusive, uint endLine, uint endOffsetExclusive)
    {
        var lineMoveCount = 1;
        for (uint iterator = startLine, iteratorOffset = startOffsetInclusive; iterator != endLine; ++iterator, iteratorOffset = 0)
        {
            ref var line = ref array[iterator];
            if (iteratorOffset == 0)
            {
                line.Dispose();
                ++lineMoveCount;
            }
            else
            {
                line.RemoveRange(iteratorOffset, (uint)line.Count - startOffsetInclusive);
            }
        }

        if (startLine == endLine)
        {
            ref var line = ref array[endLine];
            if (startOffsetInclusive == 0 && line.Count == endOffsetExclusive)
            {
                line.Dispose();
            }
            else
            {
                line.RemoveRange(startOffsetInclusive, endOffsetExclusive - startOffsetInclusive);
                --lineMoveCount;
            }
        }
        else
        {
            ref var line = ref array[endLine];
            if (line.Count == endOffsetExclusive)
            {
                line.Dispose();
            }
            else
            {
                array[startLine].AddRange(line.AsSpan(endOffsetExclusive));
                line.Dispose();
            }
        }

        if (lineMoveCount == 0)
        {
            return;
        }

        count -= lineMoveCount;
        var targetLine = startLine + (startOffsetInclusive != 0 ? 1 : 0);
        if (targetLine == count)
        {
            return;
        }

        Array.Copy(array, targetLine + lineMoveCount, array, targetLine, count - startLine);
    }
}
