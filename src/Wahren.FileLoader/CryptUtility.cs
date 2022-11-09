namespace Wahren.FileLoader;

public static class CryptUtility
{
    public static void Decrypt(scoped ReadOnlySpan<byte> cryptSpan, Span<byte> content)
    {
        const int stride256 = 32 * 7;
        const int stride128 = 16 * 7;
        Span<byte> temp = stackalloc byte[56];
        if (cryptSpan.Length == 28)
        {
            cryptSpan.CopyTo(temp);
            cryptSpan.CopyTo(temp.Slice(28));
            cryptSpan = temp;
        }
        ref var itr = ref MemoryMarshal.GetReference(content);
        ref var itrEnd = ref Unsafe.AddByteOffset(ref itr, content.Length);
        ref var cryptStart = ref MemoryMarshal.GetReference(cryptSpan);
        if (content.Length >= stride256 && Vector256.IsHardwareAccelerated)
        {
            var v0 = Vector256.LoadUnsafe(ref cryptStart);
            var v1 = Vector256.LoadUnsafe(ref cryptStart, 4);
            var v2 = Vector256.LoadUnsafe(ref cryptStart, 8);
            var v3 = Vector256.LoadUnsafe(ref cryptStart, 12);
            var v4 = Vector256.LoadUnsafe(ref cryptStart, 16);
            var v5 = Vector256.LoadUnsafe(ref cryptStart, 20);
            var v6 = Vector256.LoadUnsafe(ref cryptStart, 24);
            itrEnd = ref Unsafe.Subtract(ref itrEnd, stride256);
            do
            {
                (Vector256.LoadUnsafe(ref itr) - v1).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 32);
                (Vector256.LoadUnsafe(ref itr) - v2).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 32);
                (Vector256.LoadUnsafe(ref itr) - v3).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 32);
                (Vector256.LoadUnsafe(ref itr) - v4).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 32);
                (Vector256.LoadUnsafe(ref itr) - v5).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 32);
                (Vector256.LoadUnsafe(ref itr) - v6).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 32);
                (Vector256.LoadUnsafe(ref itr) - v0).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 32);
            } while (!Unsafe.IsAddressGreaterThan(ref itr, ref itrEnd));
            itrEnd = ref Unsafe.AddByteOffset(ref itrEnd, stride256);
        }
        else if (content.Length >= stride128 && Vector128.IsHardwareAccelerated)
        {
            var v0 = Vector128.LoadUnsafe(ref cryptStart);
            var v1 = Vector128.LoadUnsafe(ref cryptStart, 4);
            var v2 = Vector128.LoadUnsafe(ref cryptStart, 8);
            var v3 = Vector128.LoadUnsafe(ref cryptStart, 12);
            var v4 = Vector128.LoadUnsafe(ref cryptStart, 16);
            var v5 = Vector128.LoadUnsafe(ref cryptStart, 20);
            var v6 = Vector128.LoadUnsafe(ref cryptStart, 24);
            itrEnd = ref Unsafe.Subtract(ref itrEnd, stride128);
            do
            {
                (Vector128.LoadUnsafe(ref itr) - v1).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 16);
                (Vector128.LoadUnsafe(ref itr) - v5).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 16);
                (Vector128.LoadUnsafe(ref itr) - v2).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 16);
                (Vector128.LoadUnsafe(ref itr) - v6).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 16);
                (Vector128.LoadUnsafe(ref itr) - v3).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 16);
                (Vector128.LoadUnsafe(ref itr) - v0).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 16);
                (Vector128.LoadUnsafe(ref itr) - v4).StoreUnsafe(ref itr);
                itr = ref Unsafe.AddByteOffset(ref itr, 16);
            } while (!Unsafe.IsAddressGreaterThan(ref itr, ref itrEnd));
            itrEnd = ref Unsafe.AddByteOffset(ref itrEnd, stride128);
        }

        ref var cryptItr = ref Unsafe.AddByteOffset(ref cryptStart, 4);
        ref var cryptEnd = ref Unsafe.AddByteOffset(ref cryptItr, 52);
        while (Unsafe.IsAddressLessThan(ref itr, ref itrEnd))
        {
            itr -= cryptItr;
            itr = ref Unsafe.AddByteOffset(ref itr, 1);
            cryptItr = ref Unsafe.AddByteOffset(ref cryptItr, 1);
            if (Unsafe.AreSame(ref cryptItr, ref cryptEnd))
            {
                cryptItr = ref cryptStart;
            }
        }
    }
}
