global using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Wahren.FileLoader;

public static class CryptUtility
{
    public static void Decrypt(ReadOnlySpan<byte> cryptSpan, Span<byte> content)
    {
        const int stride256 = 32 * 7;
        const int stride128 = 16 * 7;
        Span<byte> temp = stackalloc byte[56];
        cryptSpan.CopyTo(temp);
        cryptSpan.CopyTo(temp.Slice(28));
        ref var itr = ref MemoryMarshal.GetReference(content);
        ref byte itrEnd = ref itr;
        Unsafe.AddByteOffset(ref itrEnd, content.Length);
        if (content.Length >= stride256 && Vector256.IsHardwareAccelerated)
        {
            var v0 = Vector256.LoadUnsafe(ref temp);
            var v1 = Vector256.LoadUnsafe(ref temp, 4);
            var v2 = Vector256.LoadUnsafe(ref temp, 8);
            var v3 = Vector256.LoadUnsafe(ref temp, 12);
            var v4 = Vector256.LoadUnsafe(ref temp, 16);
            var v5 = Vector256.LoadUnsafe(ref temp, 20);
            var v6 = Vector256.LoadUnsafe(ref temp, 24);
            do
            {
                Vector256<byte>.Subtract(Vector256.LoadUnsafe(ref itr), v1).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 32);
                Vector256<byte>.Subtract(Vector256.LoadUnsafe(ref itr), v2).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 32);
                Vector256<byte>.Subtract(Vector256.LoadUnsafe(ref itr), v3).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 32);
                Vector256<byte>.Subtract(Vector256.LoadUnsafe(ref itr), v4).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 32);
                Vector256<byte>.Subtract(Vector256.LoadUnsafe(ref itr), v5).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 32);
                Vector256<byte>.Subtract(Vector256.LoadUnsafe(ref itr), v6).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 32);
                Vector256<byte>.Subtract(Vector256.LoadUnsafe(ref itr), v0).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 32);
            } while (Unsafe.IsAddressLessThan(ref itr, ref itrEnd));
        }
        else if (content.Length >= stride128 && Vector128.IsHardwareAccelerated)
        {
            var v0 = Vector128.LoadUnsafe(ref temp);
            var v1 = Vector128.LoadUnsafe(ref temp, 4);
            var v2 = Vector128.LoadUnsafe(ref temp, 8);
            var v3 = Vector128.LoadUnsafe(ref temp, 12);
            var v4 = Vector128.LoadUnsafe(ref temp, 16);
            var v5 = Vector128.LoadUnsafe(ref temp, 20);
            var v6 = Vector128.LoadUnsafe(ref temp, 24);
            do
            {
                Vector128<byte>.Subtract(Vector128.LoadUnsafe(ref itr), v1).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 16);
                Vector128<byte>.Subtract(Vector128.LoadUnsafe(ref itr), v5).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 16);
                Vector128<byte>.Subtract(Vector128.LoadUnsafe(ref itr), v2).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 16);
                Vector128<byte>.Subtract(Vector128.LoadUnsafe(ref itr), v6).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 16);
                Vector128<byte>.Subtract(Vector128.LoadUnsafe(ref itr), v3).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 16);
                Vector128<byte>.Subtract(Vector128.LoadUnsafe(ref itr), v0).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 16);
                Vector128<byte>.Subtract(Vector128.LoadUnsafe(ref itr), v4).StoreUnsafe(ref itr);
                Unsafe.Add(ref itr, 16);
                Unsafe.Add(ref itr, stride128);
            } while (Unsafe.IsAddressLessThan(ref itr, ref itrEnd));
        }
        
        int index = 0;
        while (Unsafe.IsAddressLessThan(ref itr, ref itrEnd))
        {
            itr = itr - temp[index];
            if (++index == 56)
            {
                index = 0;
            }
            Unsafe.Add(ref itr, 1);
        }
    }
}
