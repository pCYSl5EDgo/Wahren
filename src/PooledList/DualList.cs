using System.Collections;
using System.Collections.Generic;

namespace Wahren.PooledList;

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

    public int IndexOf(ref List<T> item)
    {
        var span = new ReadOnlySpan<List<T>>(array, 0, count);
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i].Equals(ref item))
            {
                return i;
            }
        }

        return -1;
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

    public Enumerator GetEnumerator() => new(ref this);

    public struct Enumerator : IEnumerator<List<T>>
    {
        private readonly DualList<T> parent;
        private int index;

        public Enumerator(ref DualList<T> parent)
        {
            this.parent = parent;
            index = -1;
        }

        public ref List<T> Current => ref parent.array[index];

        List<T> IEnumerator<List<T>>.Current => Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return ++index < parent.count;
        }

        public void Reset()
        {
            index = -1;
        }
    }
}
