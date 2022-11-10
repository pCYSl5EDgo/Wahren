using System.Text;

namespace Wahren.FileLoader;

public static class Cp932Handler
{
    static Cp932Handler()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding = Encoding.GetEncoding(932);
    }

    private static readonly Encoding Encoding;

    public static void Load(Span<byte> content, out DualList<char> source)
    {
        if (content.Length >= 32 && Unsafe.As<byte, uint>(ref MemoryMarshal.GetReference(content)) == 0x04030201U)
        {
            CryptUtility.Decrypt(content.Slice(4, 28), content.Slice(32));
            content = content.Slice(32);
        }

        ReadOnlySpan<byte> span = content;
        var rental = ArrayPool<char>.Shared.Rent(Encoding.GetCharCount(span));
        var count = Encoding.GetChars(span, rental.AsSpan());
        UnicodeHandler.LoadFromString(rental.AsSpan(0, count), out source);
        ArrayPool<char>.Shared.Return(rental);
    }

    public static async ValueTask<DualList<char>> LoadAsync(SafeFileHandle handle, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var source = new DualList<char>();
        source.AddEmpty();
        var length = RandomAccess.GetLength(handle);
        if (length == 0L) { return source; }
        const int stride = 1024 * 7 * 8;
        var tempCount = 0;
        var tempBuffer = ArrayPool<byte>.Shared.Rent(1024 * 64);
        var buffer = ArrayPool<byte>.Shared.Rent(32 + 7 * 8 * 1024);
        byte[]? cryptBuffer = null;
        try
        {
            const int firstReadSize = 4 + 28 + 32 * 7;
            if (length < firstReadSize)
            {
                _ = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, (int)length), 0, token).ConfigureAwait(false);
                if (length >= 32 && Unsafe.As<byte, uint>(ref buffer[0]) == 0x04030201U)
                {
                    CryptUtility.Decrypt(buffer.AsSpan(4, 28), buffer.AsSpan(32, (int)length - 32));
                    InnerLoad(ref source, buffer.AsSpan(32, (int)length - 32), tempBuffer, ref tempCount, token);
                }
                else
                {
                    InnerLoad(ref source, buffer.AsSpan(0, (int)length), tempBuffer, ref tempCount, token);
                }
                InnerLastLoad(ref source, tempBuffer.AsSpan(0, tempCount));
                return source;
            }

            _ = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, firstReadSize), 0, token).ConfigureAwait(false);
            if (Unsafe.As<byte, uint>(ref buffer[0]) == 0x04030201)
            {
                CryptUtility.Decrypt(buffer.AsSpan(4, 28), buffer.AsSpan(32, firstReadSize - 32));
                InnerLoad(ref source, buffer.AsSpan(32, firstReadSize - 32), tempBuffer, ref tempCount, token);
                cryptBuffer = ArrayPool<byte>.Shared.Rent(56);
                buffer.AsSpan(4, 28).CopyTo(cryptBuffer);
                buffer.AsSpan(4, 28).CopyTo(cryptBuffer.AsSpan(28));
                var loopCount = Math.DivRem(length - firstReadSize, stride, out var remainder);
                for (long i = 0, offset = firstReadSize; i < loopCount; i++, offset += stride)
                {
                    _ = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, stride), offset, token).ConfigureAwait(false);
                    CryptUtility.Decrypt(cryptBuffer.AsSpan(0, 56), buffer.AsSpan(0, stride));
                    InnerLoad(ref source, buffer.AsSpan(0, stride), tempBuffer, ref tempCount, token);
                }
                if (remainder != 0)
                {
                    var actual = await RandomAccess.ReadAsync(handle, new Memory<byte>(buffer, 0, (int)remainder), length - remainder, token);
                    CryptUtility.Decrypt(cryptBuffer.AsSpan(0, 56), buffer.AsSpan(0, actual));
                    InnerLoad(ref source, buffer.AsSpan(0, actual), tempBuffer, ref tempCount, token);
                }
            }
            else
            {
                InnerLoad(ref source, buffer.AsSpan(0, firstReadSize), tempBuffer, ref tempCount, token);
                long offset = firstReadSize;
                do
                {
                    var actual = await RandomAccess.ReadAsync(handle, buffer.AsMemory(), offset, token).ConfigureAwait(false);
                    if (actual == 0)
                    {
                        break;
                    }
                    InnerLoad(ref source, buffer.AsSpan(0, actual), tempBuffer, ref tempCount, token);
                    offset += actual;
                } while (true);
            }
            InnerLastLoad(ref source, tempBuffer.AsSpan(0, tempCount));
            return source;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(tempBuffer);
            ArrayPool<byte>.Shared.Return(buffer);
            if (cryptBuffer is not null)
            {
                ArrayPool<byte>.Shared.Return(cryptBuffer);
            }
        }
    }

    private static void InnerLoad(ref DualList<char> source, ReadOnlySpan<byte> input, Span<byte> tempBuffer, ref int tempCount, CancellationToken token)
    {
        for (var newLineIndex = input.IndexOf((byte)'\n'); newLineIndex >= 0; input = input[(newLineIndex + 1)..], newLineIndex = input.IndexOf((byte)'\n'))
        {
            token.ThrowIfCancellationRequested();
            var slice = input[..(newLineIndex != 0 ? newLineIndex - 1 : 0)];
            if (tempCount != 0)
            {
                slice.CopyTo(tempBuffer[tempCount..]);
                slice = tempBuffer[..(tempCount + slice.Length)];
                tempCount = 0;
            }
            var charCount = Encoding.GetCharCount(slice);
            if (charCount != 0)
            {
                ref var last = ref source.Last;
                var dest = last.InsertUndefinedRange((uint)last.Count, (uint)charCount);
                Encoding.GetChars(slice, dest);
            }
            source.AddEmpty();
        }
        if (!input.IsEmpty)
        {
            tempCount = input.Length;
            input.CopyTo(tempBuffer);
        }
    }

    private static void InnerLastLoad(ref DualList<char> source, Span<byte> input)
    {
        var count = Encoding.GetCharCount(input);
        if (count == 0)
        {
            return;
        }
        ref var last = ref source.Last;
        var dest = last.InsertUndefinedRange((uint)last.Count, (uint)count);
        Encoding.GetChars(input, dest);
    }
}
