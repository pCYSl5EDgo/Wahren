namespace Wahren.AbstractSyntaxTree.Parser;

public struct StringSpanKeyTrackableSet<TTrackId> : IDisposable
    where TTrackId : unmanaged
{
    private struct Item : IDisposable
    {
        public char[]? keyArray;
        public int keyArrayUsed;
        public TTrackId[]?[]? trackIdArrayArray;
        public int[]? trackIdArrayUsedArray;

        public void Dispose()
        {
            if (keyArray is not null)
            {
                ArrayPool<char>.Shared.Return(keyArray);
                keyArray = null;
            }

            keyArrayUsed = 0;

            if (trackIdArrayUsedArray is not null)
            {
                ArrayPool<int>.Shared.Return(trackIdArrayUsedArray);
                trackIdArrayUsedArray = null;
            }

            if (trackIdArrayArray is not null)
            {
                foreach (var array in trackIdArrayArray)
                {
                    if (array is null)
                    {
                        continue;
                    }

                    ArrayPool<TTrackId>.Shared.Return(array);
                }

                Array.Clear(trackIdArrayArray);
                ArrayPool<TTrackId[]?>.Shared.Return(trackIdArrayArray);
                trackIdArrayArray = null;
            }
        }
    }

    private Item[]? itemArray;
    private int count;
    public int Count => count;

    public StringSpanKeyTrackableSet()
    {
        count = 0;
        itemArray = ArrayPool<Item>.Shared.Rent(32);
        Array.Clear(itemArray);
    }

    public bool TryGet(ReadOnlySpan<char> key, out ReadOnlySpan<TTrackId> references)
    {
        if (key.IsEmpty || itemArray is null)
        {
            goto NOT_FOUND;
        }

        var keyId = key.Length - 1;
        if (keyId >= itemArray.Length)
        {
            goto NOT_FOUND;
        }
        ref var item = ref itemArray[keyId];
        if (item.keyArrayUsed == 0 || item.keyArray is null || item.trackIdArrayArray is null || item.trackIdArrayUsedArray is null)
        {
            goto NOT_FOUND;
        }

        int index = 0;
        var tmpKeyArray = item.keyArray.AsSpan();
        var small = item.keyArrayUsed;
        if (item.trackIdArrayArray.Length < small)
        {
            small = item.trackIdArrayArray.Length;
        }
        if (item.trackIdArrayUsedArray.Length < small)
        {
            small = item.trackIdArrayUsedArray.Length;
        }
        for (; index < small; ++index, tmpKeyArray = tmpKeyArray.Slice(key.Length))
        {
            if (key.SequenceEqual(tmpKeyArray.Slice(0, key.Length)))
            {
                references = item.trackIdArrayArray[index].AsSpan(0, item.trackIdArrayUsedArray[index]);
                return true;
            }
        }

    NOT_FOUND:
        references = ReadOnlySpan<TTrackId>.Empty;
        return false;
    }

    public bool TryGetTrack(ReadOnlySpan<char> key, TTrackId id)
    {
        if (key.IsEmpty || itemArray is null)
        {
            goto NOT_FOUND;
        }
        var keyId = key.Length - 1;
        if (keyId >= itemArray.Length)
        {
            goto NOT_FOUND;
        }
        ref var item = ref itemArray[keyId];
        ref var keyArray = ref item.keyArray;
        if (keyArray is null)
        {
            goto NOT_FOUND;
        }
        if (item.keyArrayUsed == 0)
        {
            goto NOT_FOUND;
        }

        int index = 0;
        var tmpKeyArray = keyArray.AsSpan();
        for (var small = item.keyArrayUsed; index < small; ++index, tmpKeyArray = tmpKeyArray.Slice(key.Length))
        {
            if (key.SequenceEqual(tmpKeyArray.Slice(0, key.Length)))
            {
                StringSpanKeyTrackableSet<TTrackId>.RegisterTrackId(index, id, ref item);
                return true;
            }
        }

    NOT_FOUND:
        return false;
    }

    private static void RegisterTrackId(int index, TTrackId id, ref Item item)
    {
        ref var trackIdArrayUsedArray = ref item.trackIdArrayUsedArray;
        if (trackIdArrayUsedArray is null)
        {
            trackIdArrayUsedArray = ArrayPool<int>.Shared.Rent(index + 1);
            Array.Clear(trackIdArrayUsedArray);
        }
        else if (index >= trackIdArrayUsedArray.Length)
        {
            var tmp = ArrayPool<int>.Shared.Rent(index + 1);
            tmp.AsSpan(trackIdArrayUsedArray.Length).Clear();
            trackIdArrayUsedArray.AsSpan().CopyTo(tmp);
            trackIdArrayUsedArray = tmp;
        }
        ref var trackIdArrayUsed = ref trackIdArrayUsedArray[index];

        ref var trackIdArrayArray = ref item.trackIdArrayArray;
        if (trackIdArrayArray is null)
        {
            trackIdArrayArray = ArrayPool<TTrackId[]?>.Shared.Rent(index + 1);
            Array.Clear(trackIdArrayArray);
        }
        else if (index >= trackIdArrayArray.Length)
        {
            var tmp = ArrayPool<TTrackId[]?>.Shared.Rent(index + 1);
            tmp.AsSpan(trackIdArrayArray.Length).Clear();
            trackIdArrayArray.AsSpan().CopyTo(tmp);
            Array.Clear(trackIdArrayArray);
            trackIdArrayArray = tmp;
        }
        ref var trackIdArray = ref trackIdArrayArray[index];
        if (trackIdArray is null)
        {
            trackIdArray = ArrayPool<TTrackId>.Shared.Rent(trackIdArrayUsed + 1);
            Array.Clear(trackIdArray);
        }
        else if (trackIdArrayUsed >= trackIdArray.Length)
        {
            var tmp = ArrayPool<TTrackId>.Shared.Rent(index + 1);
            tmp.AsSpan(trackIdArray.Length).Clear();
            trackIdArray.AsSpan().CopyTo(tmp);
            trackIdArray = tmp;
        }
        trackIdArray[trackIdArrayUsed] = id;
        trackIdArrayUsed++;
    }

    public bool TryRegisterTrack(ReadOnlySpan<char> key, TTrackId id)
    {
        if (key.IsEmpty)
        {
            return false;
        }

        var keyId = key.Length - 1;
        if (itemArray is null)
        {
            itemArray = ArrayPool<Item>.Shared.Rent(key.Length);
            Array.Clear(itemArray);
        }
        else if (keyId >= itemArray.Length)
        {
            var tmp = ArrayPool<Item>.Shared.Rent(key.Length);
            tmp.AsSpan(itemArray.Length).Clear();
            itemArray.AsSpan().CopyTo(tmp);
            Array.Clear(itemArray);
            itemArray = tmp;
        }
        ref var item = ref itemArray[keyId];
        ref var keyArray = ref item.keyArray;
        ref var keyArrayUsed = ref item.keyArrayUsed;

        int index = 0;
        var tmpKeyArray = keyArray.AsSpan();
        bool newAdd = true;
        for (; index < keyArrayUsed; ++index, tmpKeyArray = tmpKeyArray.Slice(key.Length))
        {
            if (key.SequenceEqual(tmpKeyArray.Slice(0, key.Length)))
            {
                newAdd = false;
                break;
            }
        }

        if (index == keyArrayUsed)
        {
            ++keyArrayUsed;
            if (keyArray is null)
            {
                keyArray = ArrayPool<char>.Shared.Rent(keyArrayUsed * key.Length);
                keyArray.AsSpan(key.Length).Clear();
            }
            else if (keyArrayUsed * key.Length > keyArray.Length)
            {
                var tmp = ArrayPool<char>.Shared.Rent(keyArrayUsed * key.Length);
                tmp.AsSpan(keyArray.Length).Clear();
                keyArray.AsSpan().CopyTo(tmp);
                keyArray = tmp;
            }

            key.CopyTo(keyArray.AsSpan(index * key.Length));
            ++count;
        }

        StringSpanKeyTrackableSet<TTrackId>.RegisterTrackId(index, id, ref item);
        return newAdd;
    }

    public void Dispose()
    {
        if (itemArray is not null)
        {
            foreach (ref var item in itemArray.AsSpan())
            {
                item.Dispose();
            }

            Array.Clear(itemArray);
            ArrayPool<Item>.Shared.Return(itemArray);
            itemArray = null;
        }

        count = 0;
    }

    public Single ToSingle() => new(ref this);

    public Enumerator GetEnumerator() => new(ref this);

    public ref struct Enumerator
    {
        private readonly Item[] itemArray;
        private readonly int count;
        private int index;
        private int itemIndex;
        private int arrayIndex;

        public Enumerator(ref StringSpanKeyTrackableSet<TTrackId> parent)
        {
            index = -1;
            itemIndex = 0;
            arrayIndex = -1;
            count = parent.count;
            itemArray = parent.itemArray ?? Array.Empty<Item>();
        }

        public bool MoveNext(out ReadOnlySpan<char> key, out ReadOnlySpan<TTrackId> references)
        {
            if (++index >= count || itemIndex >= itemArray.Length)
            {
                goto NOT_FOUND;
            }

            while (++arrayIndex >= itemArray[itemIndex].keyArrayUsed)
            {
                arrayIndex = -1;
                if (++itemIndex >= itemArray.Length)
                {
                    goto NOT_FOUND;
                }
            }

            ref var item = ref itemArray[itemIndex];
            if (item.trackIdArrayArray is null || item.trackIdArrayUsedArray is null)
            {
                goto NOT_FOUND;
            }

            key = item.keyArray.AsSpan(arrayIndex * (itemIndex + 1), itemIndex + 1);
            references = item.trackIdArrayArray[arrayIndex].AsSpan(0, item.trackIdArrayUsedArray[arrayIndex]);
            return true;

        NOT_FOUND:
            key = default;
            references = default;
            return false;
        }
    }

    public struct Single : IDisposable
    {
        private int count;
        private char[] keyArray;
        private System.Range[] infoArray;
        private ulong[] hashArray;
        private TTrackId[] trackIdArray;
        private int maxKeyLength;

        public void Dispose()
        {
            if (keyArray != Array.Empty<char>() && keyArray is not null)
            {
                ArrayPool<char>.Shared.Return(keyArray);
                keyArray = Array.Empty<char>();
            }

            if (infoArray != Array.Empty<System.Range>() && infoArray is not null)
            {
                ArrayPool<System.Range>.Shared.Return(infoArray);
                infoArray = Array.Empty<System.Range>();
            }

            if (hashArray != Array.Empty<ulong>() && hashArray is not null)
            {
                ArrayPool<ulong>.Shared.Return(hashArray);
                hashArray = Array.Empty<ulong>();
            }

            if (trackIdArray != Array.Empty<TTrackId>() && trackIdArray is not null)
            {
                ArrayPool<TTrackId>.Shared.Return(trackIdArray);
                trackIdArray = Array.Empty<TTrackId>();
            }

            count = 0;
            maxKeyLength = 0;
        }

        public Single(ref StringSpanKeyTrackableSet<TTrackId> parent)
        {
            count = parent.count;
            infoArray = ArrayPool<System.Range>.Shared.Rent(count);
            hashArray = ArrayPool<ulong>.Shared.Rent(count);
            trackIdArray = ArrayPool<TTrackId>.Shared.Rent(count);
            maxKeyLength = 0;
            var tmp = ArrayPool<(int itemIndex, int innerIndex)>.Shared.Rent(count);
            var tmpSpan = tmp.AsSpan(0, count);
            try
            {
                var fillIndex = 0;
                var itemSpan = parent.itemArray.AsSpan();
                var needCharCount = 0;
                for (int itemIndex = 0; itemIndex < itemSpan.Length; itemIndex++)
                {
                    ref var item = ref itemSpan[itemIndex];
                    if (item.keyArrayUsed == 0 || item.keyArray is null || item.trackIdArrayArray is null || item.trackIdArrayUsedArray is null)
                    {
                        continue;
                    }

                    maxKeyLength = itemIndex + 1;
                    for (int innerIndex = 0; innerIndex < item.keyArrayUsed; innerIndex++)
                    {
                        if (item.trackIdArrayUsedArray[innerIndex] == 0)
                        {
                            continue;
                        }

                        var trackArray = item.trackIdArrayArray[innerIndex];
                        if (trackArray is null || trackArray.Length == 0)
                        {
                            continue;
                        }

                        needCharCount += maxKeyLength;
                        hashArray[fillIndex] = CalcHash(item.keyArray.AsSpan(innerIndex * maxKeyLength, maxKeyLength));
                        tmpSpan[fillIndex] = (itemIndex, innerIndex);
                        fillIndex++;
                    }
                }

                MemoryExtensions.Sort(hashArray.AsSpan(0, count), tmpSpan);
                keyArray = ArrayPool<char>.Shared.Rent(needCharCount);
                var keySpan = keyArray.AsSpan(0, needCharCount);
                var sliceOffset = 0;
                for (int i = 0; i < tmpSpan.Length; i++)
                {
                    var (itemIndex, innerIndex) = tmpSpan[i];
                    ref var item = ref itemSpan[itemIndex];
                    trackIdArray[i] = item.trackIdArrayArray![innerIndex]![0];
                    infoArray[i] = new(new(sliceOffset), new(sliceOffset + itemIndex + 1));
                    item.keyArray.AsSpan(innerIndex * (itemIndex + 1), itemIndex + 1).CopyTo(keySpan.Slice(sliceOffset));
                    sliceOffset += itemIndex + 1;
                }
            }
            finally
            {
                ArrayPool<(int itemIndex, int innerIndex)>.Shared.Return(tmp);
            }
        }

        internal static ulong CalcHash(ReadOnlySpan<char> key)
        {
            var end = key.Length;
            const int endMax = 12; // 64 * log2 / log(10 + 26 + 1)
            if (endMax < end)
            {
                end = endMax;
            }

            ulong answer = 0;
            for (int i = 0; i < end; i++)
            {
                ulong c = key[i];
                
                answer *= 37;
                if (c >= '0' && c <= '9')
                {
                    answer += c - '0';
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    answer += c - ('A' - 10);
                }
                else if (c >= 'a' && c <= 'z')
                {
                    answer += c - ('a' - 10);
                }
                else if (c == '_')
                {
                    answer += 36;
                }
                else
                {
                    return ulong.MaxValue;
                }
            }

            return answer;
        }

        public ref TTrackId TryGet(ReadOnlySpan<char> key)
        {
            if (key.IsEmpty || key.Length > maxKeyLength)
            {
                goto NOT_FOUND;
            }

            var hash = CalcHash(key);
            if (hash == ulong.MaxValue)
            {
                goto NOT_FOUND;
            }

            var index = hashArray.AsSpan(0, count).BinarySearch(hash);
            if (index < 0)
            {
                goto NOT_FOUND;
            }
            if (keyArray.AsSpan(infoArray[index]).SequenceEqual(key))
            {
                return ref trackIdArray[index];
            }
            for (var currentIndex = index + 1; currentIndex < count && hash == hashArray[currentIndex]; ++currentIndex)
            {
                if (keyArray.AsSpan(infoArray[currentIndex]).SequenceEqual(key))
                {
                    return ref trackIdArray[currentIndex];
                }
            }
            for (var currentIndex = index - 1; currentIndex >= 0 && hash == hashArray[currentIndex]; --currentIndex)
            {
                if (keyArray.AsSpan(infoArray[currentIndex]).SequenceEqual(key))
                {
                    return ref trackIdArray[currentIndex];
                }
            }
        NOT_FOUND:
            return ref Unsafe.NullRef<TTrackId>();
        }
    }
}
