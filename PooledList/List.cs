﻿using System;
using System.Buffers;

namespace Wahren.PooledList;

public struct List<T> : IDisposable, System.Collections.Generic.IList<T>, IEquatable<List<T>>
{
    private T[] array;
    private int count;

    public List()
    {
        array = Array.Empty<T>();
        count = default;
    }

    public uint LastIndex => (uint)(count - 1);

    public ref T Last => ref array[count - 1];

    public void RemoveLast()
    {
        if (count != 0)
        {
            --count;
        }
    }

    public Span<T> AsSpan() => new(array, 0, count);

    public Span<T> AsSpan(uint start) => new(array, (int)start, count - (int)start);

    public Span<T> AsSpan(uint start, uint length) => new(array, (int)start, (int)length);

    public ref T this[int index]
    {
        get
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException($"index should be greater than -1. index : {index}");
            }

            if (index >= count)
            {
                throw new IndexOutOfRangeException($"size is limited to {count}. index : {index}");
            }

            return ref array[index];
        }
    }

    public ref T this[uint index]
    {
        get
        {
            if (index >= count)
            {
                throw new IndexOutOfRangeException($"size is limited to {count}. index : {index}");
            }

            return ref array[index];
        }
    }

    public int Count => count;

    public bool IsReadOnly => false;

    T System.Collections.Generic.IList<T>.this[int index]
    {
        get => this[index];
        set => this[index] = value;
    }

    public void PrepareAddRange(int sizeAdditive, bool clear = false)
    {
        int length = array.Length;
        if (length == 0)
        {
            array = ArrayPool<T>.Shared.Rent(sizeAdditive);
            if (clear)
            {
                Array.Clear(array);
            }

            return;
        }

        int requiredCapacity = count + sizeAdditive;
        if (requiredCapacity <= length)
        {
            return;
        }

        var newArray = ArrayPool<T>.Shared.Rent(requiredCapacity);
        array.AsSpan(0, count).CopyTo(newArray);
        ArrayPool<T>.Shared.Return(array);
        if (clear)
        {
            newArray.AsSpan(length).Clear();
        }

        array = newArray;
    }

    public delegate int AddConverter<TSource>(ReadOnlySpan<TSource> sourceSpan, Span<T> destinationSpan);

    public void AddRangeConversion<TSource>(AddConverter<TSource> converter, ReadOnlySpan<TSource> sourceSpan)
    {
        count += converter(sourceSpan, new(array, 0, count));
    }

    public void AddRange(ReadOnlySpan<T> span)
    {
        if (array.Length == 0)
        {
            count = span.Length;
            array = ArrayPool<T>.Shared.Rent(count);
            span.CopyTo(array.AsSpan());
            return;
        }

        var newCount = count + span.Length;
        if (newCount > array.Length)
        {
            var newArray = ArrayPool<T>.Shared.Rent(newCount << 1);
            if (count != 0)
            {
                array.AsSpan(0, count).CopyTo(newArray);
            }

            ArrayPool<T>.Shared.Return(array);
            array = newArray;
        }

        span.CopyTo(array.AsSpan(count));
        count = newCount;
    }

    public void Add(ref T item)
    {
        if (array.Length == 0)
        {
            array = ArrayPool<T>.Shared.Rent(16);
        }
        else if (count == array.Length)
        {
            var newArray = ArrayPool<T>.Shared.Rent(count << 1);
            array.AsSpan().CopyTo(new(newArray, 0, count));
            ArrayPool<T>.Shared.Return(array);
            array = newArray;
        }

        array[count++] = item;
    }

    public void Add(ref T item, bool clear)
    {
        int length = array.Length;
        if (length == 0)
        {
            array = ArrayPool<T>.Shared.Rent(16);
            if (clear)
            {
                Array.Clear(array, 0, length);
            }
        }
        else if (count == length)
        {
            var newArray = ArrayPool<T>.Shared.Rent(count << 1);
            array.AsSpan().CopyTo(new(newArray, 0, count));
            ArrayPool<T>.Shared.Return(array);
            if (clear)
            {
                newArray.AsSpan(length).Clear();
            }

            array = newArray;
        }

        array[count++] = item;
    }

    public void Add(T item) => Add(ref item);

    public void Add(T item, bool clear) => Add(ref item, clear);

    public bool IsEmpty => count == 0;

    public void Dispose()
    {
        if (array.Length == 0)
        {
            return;
        }

        ArrayPool<T>.Shared.Return(array);
        array = Array.Empty<T>();
        count = 0;
    }

    public int IndexOf(T item)
    {
        var span = new ReadOnlySpan<T>(array, 0, count);
        for (int index = 0; index < span.Length; ++index)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(item, span[index]))
            {
                return index;
            }
        }

        return -1;
    }

    public void Insert(int index, T item)
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
            var newArray = ArrayPool<T>.Shared.Rent(count << 1);
            array.AsSpan(0, index).CopyTo(newArray.AsSpan());
            newArray[index] = item;
            array.AsSpan(index, count - index).CopyTo(newArray.AsSpan(index + 1));
            ArrayPool<T>.Shared.Return(array);
            array = newArray;
            ++count;
            return;
        }

        Array.Copy(array, index, array, index + 1, count++ - index);
        array[index] = item;
    }

    public void RemoveAt(int index)
    {
        if (index >= count)
        {
            throw new IndexOutOfRangeException($"index : {index} should not be larger than or equal to count : {count}.");
        }

        if (index == count - 1)
        {
            --count;
            return;
        }

        Array.Copy(array, index + 1, array, index, --count - index);
    }

    public void Clear()
    {
        count = 0;
    }

    public bool Contains(T item)
    {
        var span = new ReadOnlySpan<T>(array, 0, count);
        foreach (ref readonly var i in span)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(item, i))
            {
                return true;
            }
        }

        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(this.array, 0, array, arrayIndex, count);
    }

    public T[] ToArray()
    {
        if (count == 0)
        {
            return Array.Empty<T>();
        }

        var ret = new T[count];
        CopyTo(ret, 0);
        return ret;
    }

    public bool Remove(T item)
    {
        var span = new ReadOnlySpan<T>(array, 0, count);
        for (int index = 0; index < span.Length; ++index)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(item, span[index]))
            {
                Array.Copy(array, index + 1, array, index, --count - index);
                return true;
            }
        }

        return false;
    }

    System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public Enumerator GetEnumerator() => new(ref this);

    public bool Equals(ref List<T> other)
    {
        return count == other.count && array == other.array;
    }

    bool IEquatable<List<T>>.Equals(List<T> other)
    {
        return count == other.count && array == other.array;
    }

    public struct Enumerator : System.Collections.Generic.IEnumerator<T>
    {
        private readonly List<T> parent;
        private int index;

        public Enumerator(ref List<T> parent)
        {
            this.parent = parent;
            index = -1;
        }

        public ref T Current => ref parent.array[index];

        T System.Collections.Generic.IEnumerator<T>.Current => Current;

        object? System.Collections.IEnumerator.Current => Current;

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