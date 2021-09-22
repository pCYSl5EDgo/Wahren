using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace Wahren.FileLoader;

public static unsafe class CryptUtility
{
    public static void Decrypt(ReadOnlySpan<byte> cryptSpan, Span<byte> content)
    {
        byte* temp = stackalloc byte[56];
        fixed (byte* cryptSource = cryptSpan)
        {
            Buffer.MemoryCopy(cryptSource, temp, 28, 28);
            Buffer.MemoryCopy(cryptSource, temp + 28, 28, 28);
        }

        fixed (byte* contentPtr = content)
        {
            byte* end = contentPtr + content.Length;
            byte* itrEnd = contentPtr;

            if (Avx2.IsSupported)
            {
                const int LoopSize = 32;
                itrEnd += (content.Length / (LoopSize * 7)) * (LoopSize * 7);
                Vector256<byte> v0 = Avx.LoadVector256(temp);
                Vector256<byte> v1 = Avx.LoadVector256(temp + 4);
                Vector256<byte> v2 = Avx.LoadVector256(temp + 8);
                Vector256<byte> v3 = Avx.LoadVector256(temp + 12);
                Vector256<byte> v4 = Avx.LoadVector256(temp + 16);
                Vector256<byte> v5 = Avx.LoadVector256(temp + 20);
                Vector256<byte> v6 = Avx.LoadVector256(temp + 24);
                for (var itr = contentPtr; itr != itrEnd;)
                {
                    Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v1));
                    itr += LoopSize;
                    Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v2));
                    itr += LoopSize;
                    Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v3));
                    itr += LoopSize;
                    Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v4));
                    itr += LoopSize;
                    Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v5));
                    itr += LoopSize;
                    Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v6));
                    itr += LoopSize;
                    Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v0));
                    itr += LoopSize;
                }
            }
            else if (Sse2.IsSupported)
            {
                const int LoopSize = 16;
                itrEnd += (content.Length / (LoopSize * 7)) * (LoopSize * 7);
                Vector128<byte> v0 = Sse2.LoadVector128(temp);
                Vector128<byte> v1 = Sse2.LoadVector128(temp + 4);
                Vector128<byte> v2 = Sse2.LoadVector128(temp + 8);
                Vector128<byte> v3 = Sse2.LoadVector128(temp + 12);
                Vector128<byte> v4 = Sse2.LoadVector128(temp + 16);
                Vector128<byte> v5 = Sse2.LoadVector128(temp + 20);
                Vector128<byte> v6 = Sse2.LoadVector128(temp + 24);
                for (var itr = contentPtr; itr != itrEnd;)
                {
                    Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v1));
                    itr += LoopSize;
                    Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v5));
                    itr += LoopSize;
                    Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v2));
                    itr += LoopSize;
                    Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v6));
                    itr += LoopSize;
                    Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v3));
                    itr += LoopSize;
                    Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v0));
                    itr += LoopSize;
                    Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v4));
                    itr += LoopSize;
                }
            }
            else if (AdvSimd.IsSupported)
            {
                const int LoopSize = 16;
                itrEnd += (content.Length / (LoopSize * 7)) * (LoopSize * 7);
                Vector128<byte> v0 = AdvSimd.LoadVector128(temp);
                Vector128<byte> v1 = AdvSimd.LoadVector128(temp + 4);
                Vector128<byte> v2 = AdvSimd.LoadVector128(temp + 8);
                Vector128<byte> v3 = AdvSimd.LoadVector128(temp + 12);
                Vector128<byte> v4 = AdvSimd.LoadVector128(temp + 16);
                Vector128<byte> v5 = AdvSimd.LoadVector128(temp + 20);
                Vector128<byte> v6 = AdvSimd.LoadVector128(temp + 24);
                for (var itr = contentPtr; itr != itrEnd;)
                {
                    AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v1));
                    itr += LoopSize;
                    AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v5));
                    itr += LoopSize;
                    AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v2));
                    itr += LoopSize;
                    AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v6));
                    itr += LoopSize;
                    AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v3));
                    itr += LoopSize;
                    AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v0));
                    itr += LoopSize;
                    AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v4));
                    itr += LoopSize;
                }
            }

            for (int index = 4; itrEnd != end; itrEnd++)
            {
                *itrEnd -= temp[index];
                if (++index == 56)
                {
                    index = 0;
                }
            }
        }
    }

    [UnmanagedCallersOnly]
    public static void DecryptUnamanagedOnly(byte* cryptSource, byte* contentPtr, nuint contentSize)
    {
        byte* temp = stackalloc byte[56];
        Buffer.MemoryCopy(cryptSource, temp, 28, 28);
        Buffer.MemoryCopy(cryptSource, temp + 28, 28, 28);

        byte* end = contentPtr + contentSize;
        byte* itrEnd = contentPtr;

        if (Avx2.IsSupported)
        {
            const int LoopSize = 32;
            itrEnd += (contentSize / (LoopSize * 7)) * (LoopSize * 7);
            Vector256<byte> v0 = Avx.LoadVector256(temp);
            Vector256<byte> v1 = Avx.LoadVector256(temp + 4);
            Vector256<byte> v2 = Avx.LoadVector256(temp + 8);
            Vector256<byte> v3 = Avx.LoadVector256(temp + 12);
            Vector256<byte> v4 = Avx.LoadVector256(temp + 16);
            Vector256<byte> v5 = Avx.LoadVector256(temp + 20);
            Vector256<byte> v6 = Avx.LoadVector256(temp + 24);
            for (var itr = contentPtr; itr != itrEnd;)
            {
                Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v1));
                itr += LoopSize;
                Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v2));
                itr += LoopSize;
                Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v3));
                itr += LoopSize;
                Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v4));
                itr += LoopSize;
                Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v5));
                itr += LoopSize;
                Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v6));
                itr += LoopSize;
                Avx.Store(itr, Avx2.Subtract(Avx.LoadVector256(itr), v0));
                itr += LoopSize;
            }
        }
        else if (Sse2.IsSupported)
        {
            const int LoopSize = 16;
            itrEnd += (contentSize / (LoopSize * 7)) * (LoopSize * 7);
            Vector128<byte> v0 = Sse2.LoadVector128(temp);
            Vector128<byte> v1 = Sse2.LoadVector128(temp + 4);
            Vector128<byte> v2 = Sse2.LoadVector128(temp + 8);
            Vector128<byte> v3 = Sse2.LoadVector128(temp + 12);
            Vector128<byte> v4 = Sse2.LoadVector128(temp + 16);
            Vector128<byte> v5 = Sse2.LoadVector128(temp + 20);
            Vector128<byte> v6 = Sse2.LoadVector128(temp + 24);
            for (var itr = contentPtr; itr != itrEnd;)
            {
                Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v1));
                itr += LoopSize;
                Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v5));
                itr += LoopSize;
                Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v2));
                itr += LoopSize;
                Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v6));
                itr += LoopSize;
                Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v3));
                itr += LoopSize;
                Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v0));
                itr += LoopSize;
                Sse2.Store(itr, Sse2.Subtract(Sse2.LoadVector128(itr), v4));
                itr += LoopSize;
            }
        }
        else if (AdvSimd.IsSupported)
        {
            const int LoopSize = 16;
            itrEnd += (contentSize / (LoopSize * 7)) * (LoopSize * 7);
            Vector128<byte> v0 = AdvSimd.LoadVector128(temp);
            Vector128<byte> v1 = AdvSimd.LoadVector128(temp + 4);
            Vector128<byte> v2 = AdvSimd.LoadVector128(temp + 8);
            Vector128<byte> v3 = AdvSimd.LoadVector128(temp + 12);
            Vector128<byte> v4 = AdvSimd.LoadVector128(temp + 16);
            Vector128<byte> v5 = AdvSimd.LoadVector128(temp + 20);
            Vector128<byte> v6 = AdvSimd.LoadVector128(temp + 24);
            for (var itr = contentPtr; itr != itrEnd;)
            {
                AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v1));
                itr += LoopSize;
                AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v5));
                itr += LoopSize;
                AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v2));
                itr += LoopSize;
                AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v6));
                itr += LoopSize;
                AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v3));
                itr += LoopSize;
                AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v0));
                itr += LoopSize;
                AdvSimd.Store(itr, AdvSimd.Subtract(AdvSimd.LoadVector128(itr), v4));
                itr += LoopSize;
            }
        }

        for (int index = 4; itrEnd != end; itrEnd++)
        {
            *itrEnd -= temp[index];
            if (++index == 56)
            {
                index = 0;
            }
        }
    }

    [UnmanagedCallersOnly]
    public static void EncryptUnamanagedOnly(byte* cryptSource, byte* contentPtr, nuint contentSize)
    {
        byte* temp = stackalloc byte[56];
        Buffer.MemoryCopy(cryptSource, temp, 28, 28);
        Buffer.MemoryCopy(cryptSource, temp + 28, 28, 28);

        byte* end = contentPtr + contentSize;
        byte* itrEnd = contentPtr;
        byte* destItr = cryptSource + 28;

        if (Avx2.IsSupported)
        {
            const int LoopSize = 32;
            itrEnd += (contentSize / (LoopSize * 7)) * (LoopSize * 7);
            Vector256<byte> v0 = Avx.LoadVector256(temp);
            Vector256<byte> v1 = Avx.LoadVector256(temp + 4);
            Vector256<byte> v2 = Avx.LoadVector256(temp + 8);
            Vector256<byte> v3 = Avx.LoadVector256(temp + 12);
            Vector256<byte> v4 = Avx.LoadVector256(temp + 16);
            Vector256<byte> v5 = Avx.LoadVector256(temp + 20);
            Vector256<byte> v6 = Avx.LoadVector256(temp + 24);
            for (var itr = contentPtr; itr != itrEnd;)
            {
                Avx.Store(destItr, Avx2.Add(Avx.LoadVector256(itr), v1));
                itr += LoopSize;
                destItr += LoopSize;
                Avx.Store(destItr, Avx2.Add(Avx.LoadVector256(itr), v2));
                itr += LoopSize;
                destItr += LoopSize;
                Avx.Store(destItr, Avx2.Add(Avx.LoadVector256(itr), v3));
                itr += LoopSize;
                destItr += LoopSize;
                Avx.Store(destItr, Avx2.Add(Avx.LoadVector256(itr), v4));
                itr += LoopSize;
                destItr += LoopSize;
                Avx.Store(destItr, Avx2.Add(Avx.LoadVector256(itr), v5));
                itr += LoopSize;
                destItr += LoopSize;
                Avx.Store(destItr, Avx2.Add(Avx.LoadVector256(itr), v6));
                itr += LoopSize;
                destItr += LoopSize;
                Avx.Store(destItr, Avx2.Add(Avx.LoadVector256(itr), v0));
                itr += LoopSize;
                destItr += LoopSize;
            }
        }
        else if (Sse2.IsSupported)
        {
            const int LoopSize = 16;
            itrEnd += (contentSize / (LoopSize * 7)) * (LoopSize * 7);
            Vector128<byte> v0 = Sse2.LoadVector128(temp);
            Vector128<byte> v1 = Sse2.LoadVector128(temp + 4);
            Vector128<byte> v2 = Sse2.LoadVector128(temp + 8);
            Vector128<byte> v3 = Sse2.LoadVector128(temp + 12);
            Vector128<byte> v4 = Sse2.LoadVector128(temp + 16);
            Vector128<byte> v5 = Sse2.LoadVector128(temp + 20);
            Vector128<byte> v6 = Sse2.LoadVector128(temp + 24);
            for (var itr = contentPtr; itr != itrEnd;)
            {
                Sse2.Store(destItr, Sse2.Add(Sse2.LoadVector128(itr), v1));
                itr += LoopSize;
                destItr += LoopSize;
                Sse2.Store(destItr, Sse2.Add(Sse2.LoadVector128(itr), v5));
                itr += LoopSize;
                destItr += LoopSize;
                Sse2.Store(destItr, Sse2.Add(Sse2.LoadVector128(itr), v2));
                itr += LoopSize;
                destItr += LoopSize;
                Sse2.Store(destItr, Sse2.Add(Sse2.LoadVector128(itr), v6));
                itr += LoopSize;
                destItr += LoopSize;
                Sse2.Store(destItr, Sse2.Add(Sse2.LoadVector128(itr), v3));
                itr += LoopSize;
                destItr += LoopSize;
                Sse2.Store(destItr, Sse2.Add(Sse2.LoadVector128(itr), v0));
                itr += LoopSize;
                destItr += LoopSize;
                Sse2.Store(destItr, Sse2.Add(Sse2.LoadVector128(itr), v4));
                itr += LoopSize;
                destItr += LoopSize;
            }
        }
        else if (AdvSimd.IsSupported)
        {
            const int LoopSize = 16;
            itrEnd += (contentSize / (LoopSize * 7)) * (LoopSize * 7);
            Vector128<byte> v0 = AdvSimd.LoadVector128(temp);
            Vector128<byte> v1 = AdvSimd.LoadVector128(temp + 4);
            Vector128<byte> v2 = AdvSimd.LoadVector128(temp + 8);
            Vector128<byte> v3 = AdvSimd.LoadVector128(temp + 12);
            Vector128<byte> v4 = AdvSimd.LoadVector128(temp + 16);
            Vector128<byte> v5 = AdvSimd.LoadVector128(temp + 20);
            Vector128<byte> v6 = AdvSimd.LoadVector128(temp + 24);
            for (var itr = contentPtr; itr != itrEnd;)
            {
                AdvSimd.Store(destItr, AdvSimd.Add(AdvSimd.LoadVector128(itr), v1));
                itr += LoopSize;
                destItr += LoopSize;
                AdvSimd.Store(destItr, AdvSimd.Add(AdvSimd.LoadVector128(itr), v5));
                itr += LoopSize;
                destItr += LoopSize;
                AdvSimd.Store(destItr, AdvSimd.Add(AdvSimd.LoadVector128(itr), v2));
                itr += LoopSize;
                destItr += LoopSize;
                AdvSimd.Store(destItr, AdvSimd.Add(AdvSimd.LoadVector128(itr), v6));
                itr += LoopSize;
                destItr += LoopSize;
                AdvSimd.Store(destItr, AdvSimd.Add(AdvSimd.LoadVector128(itr), v3));
                itr += LoopSize;
                destItr += LoopSize;
                AdvSimd.Store(destItr, AdvSimd.Add(AdvSimd.LoadVector128(itr), v0));
                itr += LoopSize;
                destItr += LoopSize;
                AdvSimd.Store(destItr, AdvSimd.Add(AdvSimd.LoadVector128(itr), v4));
                itr += LoopSize;
                destItr += LoopSize;
            }
        }

        for (int index = 4; itrEnd != end; itrEnd++, destItr++)
        {
            *destItr = (byte)(*itrEnd + temp[index]);
            if (++index == 56)
            {
                index = 0;
            }
        }
    }
}
