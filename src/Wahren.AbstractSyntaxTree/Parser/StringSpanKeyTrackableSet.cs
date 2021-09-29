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

    public Enumerator GetEnumerator() => new(ref this);

    public ref struct Enumerator
    {
        private Item[] itemArray;
        private int count;
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
}
