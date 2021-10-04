namespace Wahren.PooledList;

public struct StringSpanKeySlowSet : IDisposable
{
    private List<char> keys= new();
    private uint[][] offset_value_pairBuckets = Array.Empty<uint[]>();
    private List<uint> eachBucketCount = new();
    private List<ulong> offset_length_Pairs = new();
    public DualList<uint> References = new();
    private uint count = default;

    public bool TryGet(ReadOnlySpan<char> key, out uint id)
    {
        id = uint.MaxValue;
        if (key.IsEmpty || count == 0)
        {
            return false;
        }

        if (key.Length > eachBucketCount.Count)
        {
            return false;
        }

        uint lengthIndex = (uint)key.Length - 1U;
        var countInBucket = eachBucketCount[lengthIndex];
        var bucket = offset_value_pairBuckets[lengthIndex];
        for (uint i = 0; i < countInBucket; i += 2)
        {
            if (key.SequenceEqual(keys.AsSpan(bucket[i], (uint)key.Length)))
            {
                id = bucket[i + 1];
                return true;
            }
        }

        return false;
    }

    public void InitialAdd(ReadOnlySpan<char> key)
    {
        if (key.IsEmpty)
        {
            return;
        }

        uint lengthIndex = (uint)key.Length - 1U;
        if (key.Length > offset_value_pairBuckets.Length)
        {
            var newArray = ArrayPool<uint[]>.Shared.Rent(key.Length);
            if (offset_value_pairBuckets.Length != 0)
            {
                offset_value_pairBuckets.CopyTo(newArray.AsSpan());
                Array.Clear(offset_value_pairBuckets);
                ArrayPool<uint[]>.Shared.Return(offset_value_pairBuckets);
            }

            newArray.AsSpan(offset_value_pairBuckets.Length).Fill(Array.Empty<uint>());
            (newArray[lengthIndex] = ArrayPool<uint>.Shared.Rent(16)).AsSpan().Clear();
            offset_value_pairBuckets = newArray;
        }

        if (key.Length > eachBucketCount.Count)
        {
            var tmp = ArrayPool<uint>.Shared.Rent(key.Length - eachBucketCount.Count);
            Array.Clear(tmp);
            eachBucketCount.AddRange(tmp.AsSpan());
            ArrayPool<uint>.Shared.Return(tmp);
        }

        ref var countInBucket = ref eachBucketCount[lengthIndex];
        ref uint[] bucket = ref offset_value_pairBuckets[lengthIndex];

        for (uint i = 0; i < countInBucket; i += 2)
        {
            if (key.SequenceEqual(keys.AsSpan(bucket[i], (uint)key.Length)))
            {
                return;
            }
        }

        var currentBucketLength = bucket.Length;
        countInBucket += 2;
        if (countInBucket > currentBucketLength)
        {
            var newArray = ArrayPool<uint>.Shared.Rent((int)countInBucket);
            if (currentBucketLength != 0)
            {
                bucket.CopyTo(newArray.AsSpan());
                ArrayPool<uint>.Shared.Return(bucket);
            }

            newArray.AsSpan(currentBucketLength).Clear();
            bucket = newArray;
        }

        bucket[countInBucket - 2] = (uint)keys.Count;
        bucket[countInBucket - 1] = count;
        ulong pair = (uint)key.Length;
        pair <<= 32;
        pair |= (uint)keys.Count;
        offset_length_Pairs.Add(pair);
        keys.AddRange(key);
        References.AddEmpty();
        count++;
    }

    public uint GetOrAdd(ReadOnlySpan<char> key, uint registerId)
    {
        if (key.IsEmpty)
        {
            return uint.MaxValue;
        }

        uint lengthIndex = (uint)key.Length - 1U;
        if (key.Length > offset_value_pairBuckets.Length)
        {
            var newArray = ArrayPool<uint[]>.Shared.Rent(key.Length);
            if (offset_value_pairBuckets.Length != 0)
            {
                offset_value_pairBuckets.CopyTo(newArray.AsSpan());
                Array.Clear(offset_value_pairBuckets);
                ArrayPool<uint[]>.Shared.Return(offset_value_pairBuckets);
            }

            newArray.AsSpan(offset_value_pairBuckets.Length).Fill(Array.Empty<uint>());
            (newArray[lengthIndex] = ArrayPool<uint>.Shared.Rent(16)).AsSpan().Clear();
            offset_value_pairBuckets = newArray;
        }

        if (key.Length > eachBucketCount.Count)
        {
            var tmp = ArrayPool<uint>.Shared.Rent(key.Length - eachBucketCount.Count);
            Array.Clear(tmp);
            eachBucketCount.AddRange(tmp.AsSpan());
            ArrayPool<uint>.Shared.Return(tmp);
        }

        ref var countInBucket = ref eachBucketCount[lengthIndex];
        ref uint[] bucket = ref offset_value_pairBuckets[lengthIndex];

        for (uint i = 0; i < countInBucket; i += 2)
        {
            if (key.SequenceEqual(keys.AsSpan(bucket[i], (uint)key.Length)))
            {
                var id = bucket[i + 1];
                References[id].Add(registerId);
                return id;
            }
        }

        var currentBucketLength = bucket.Length;
        countInBucket += 2;
        if (countInBucket > currentBucketLength)
        {
            var newArray = ArrayPool<uint>.Shared.Rent((int)countInBucket);
            if (currentBucketLength != 0)
            {
                bucket.CopyTo(newArray.AsSpan());
                ArrayPool<uint>.Shared.Return(bucket);
            }

            newArray.AsSpan(currentBucketLength).Clear();
            bucket = newArray;
        }

        bucket[countInBucket - 2] = (uint)keys.Count;
        bucket[countInBucket - 1] = count;
        ulong pair = (uint)key.Length;
        pair <<= 32;
        pair |= (uint)keys.Count;
        offset_length_Pairs.Add(pair);
        keys.AddRange(key);
        References.AddEmpty();
        References[count].Add(registerId);
        return count++;
    }

    public uint Count => count;

    public ReadOnlySpan<char> this[uint id]
    {
        get
        {
            ulong pair = offset_length_Pairs[id];
            var offset = (uint)pair;
            var length = (uint)(pair >> 32);
            return keys.AsSpan(offset, length);
        }
    }

    public void Dispose()
    {
        keys.Dispose();

        if (offset_value_pairBuckets.Length != 0)
        {
            foreach (uint[] inner in offset_value_pairBuckets)
            {
                if (inner.Length == 0)
                {
                    continue;
                }
                
                ArrayPool<uint>.Shared.Return(inner);
            }

            Array.Clear(offset_value_pairBuckets);
            ArrayPool<uint[]>.Shared.Return(offset_value_pairBuckets);
            offset_value_pairBuckets = Array.Empty<uint[]>();
        }

        eachBucketCount.Dispose();
        offset_length_Pairs.Dispose();
        References.Dispose();
        count = 0;
    }
}
