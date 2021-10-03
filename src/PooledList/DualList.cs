using System.Collections;
using System.Collections.Generic;

namespace Wahren.PooledList;

public struct DualList<T> : IDisposable, IList<List<T>>, IEquatable<DualList<T>>
{
    private List<T>[] array;
    private int count;

#if NET6_0
        public DualList()
        {
            array = Array.Empty<List<T>>();
            count = default;
        }
#endif

    public DualList(int initialCapapcity)
    {
        array = initialCapapcity == 0 ? Array.Empty<List<T>>() : ArrayPool<List<T>>.Shared.Rent(initialCapapcity);
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

    public ref List<T> Last() => ref count == 0 ? ref Unsafe.NullRef<List<T>>() : ref array[count - 1];

    public uint AddAllCount()
    {
        uint answer = 0;
        for (int i = 0; i < count; i++)
        {
            answer += (uint)array[i].Count;
        }

        return answer;
    }

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

    int IList<List<T>>.IndexOf(List<T> item) => IndexOf(ref item);

    public void Insert(int index, ref List<T> item)
    {
        if (index == count)
        {
            Add(ref item);
            return;
        }

        if (index > count)
        {
            throw new IndexOutOfRangeException($"index : {index} should not be larger than count : {count}.");
        }

        if (array.Length == count)
        {
            var newArray = ArrayPool<List<T>>.Shared.Rent(count << 1);
            array.AsSpan(0, index).CopyTo(newArray.AsSpan());
            newArray[index] = item;
            array.AsSpan(index, count - index).CopyTo(newArray.AsSpan(index + 1));
            ArrayPool<List<T>>.Shared.Return(array);
            array = newArray;
            ++count;
            return;
        }

        Array.Copy(array, index, array, index + 1, count++ - index);
        array[index] = item;
    }

    void IList<List<T>>.Insert(int index, List<T> item) => Insert(index, ref item);

    public void RemoveAt(int index)
    {
        if (index >= count)
        {
            throw new IndexOutOfRangeException($"index : {index} should not be larger than or equal to count : {count}.");
        }

        if (index == count - 1)
        {
            array[--count].Dispose();
            return;
        }

        array[index].Dispose();
        Array.Copy(array, index + 1, array, index, --count - index);
    }

    public void Add(ref List<T> item)
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

        array[count++] = item;
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

    void ICollection<List<T>>.Add(List<T> item) => Add(ref item);

    public void Clear()
    {
        foreach (ref var list in this)
        {
            list.Dispose();
        }

        count = 0;
    }

    bool ICollection<List<T>>.Contains(List<T> item) => Contains(ref item);

    public bool Contains(ref List<T> item)
    {
        foreach (ref var list in AsSpan())
        {
            if (list.Equals(ref item))
            {
                return true;
            }
        }

        return false;
    }

    public void CopyTo(List<T>[] array, int arrayIndex)
    {
        Array.Copy(this.array, 0, array, arrayIndex, count);
    }

    bool ICollection<List<T>>.Remove(List<T> item) => Remove(ref item);

    public bool Remove(ref List<T> item)
    {
        var span = AsSpan();
        for (int index = 0; index < span.Length; ++index)
        {
            if (item.Equals(span[index]))
            {
                item.Dispose();
                Array.Copy(array, index + 1, array, index, --count - index);
                return true;
            }
        }

        return false;
    }

    bool Equals(ref DualList<T> other) => count == other.count && array == other.array;

    bool IEquatable<DualList<T>>.Equals(DualList<T> other) => Equals(ref other);

    public int Count => count;

    public bool IsReadOnly => false;

    List<T> IList<List<T>>.this[int index] { get => this[index]; set => this[index] = value; }

    public Enumerator GetEnumerator() => new(ref this);

    IEnumerator<List<T>> IEnumerable<List<T>>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
