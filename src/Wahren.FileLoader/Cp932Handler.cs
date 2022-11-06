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
        if (content.Length >= 32 && Unsafe.As<byte, uint>(ref MemoryMarshall.GetReference(content)) == 0x04030201U)
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
        var remainder = default(byte?);
        var tempBuffer = ArrayPool<byte>.Shared.Rent(1024 * 64);
        var buffer = ArrayPool<byte>.Shared.Rent(32 + 7 * 8 * 1024);
        byte[]? cryptBuffer = null
        try
        {
            const int firstReadSize = 4 + 28 + 32 * 7;
            if (legnth < firstReadSize)
            {
                _ = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, (int)length), 0, token).ConfigureAwait(false);
                if (length >= 32 && Unsafe.As<byte, uint>(ref buffer) == 0x04030201U)
                {
                    CryptUtility.Decrypt(buffer.AsSpan(4, 28), buffer.AsSpan(32, (int)length - 32));
                    InnerLoad(ref source, buffer.AsSpan(32, (int)length - 32), MemoryMarshal.Cast<byte, char>(tempBuffer.AsSpan()), ref remainder, token);
                }
                else
                {
                    InnerLoad(ref source, buffer.AsSpan(0, (int)length), MemoryMarshal.Cast<byte, char>(tempBuffer.AsSpan()), ref remainder, token);
                }
                return source;
            }

            _ = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, firstReadSize), 0, token).ConfigureAwait();
            if (Unsafe.As<byte, uint>(ref buffer[0]) == 0x04030201)
            {
                CryptUtility.Decrypt(buffer.AsSpan(4, 28), buffer.AsSpan(32, firstReadSize - 32));
                InnerLoad(ref source, buffer.AsSpan(32, firstReadSize - 32), MemoryMarshal.Cast<byte, char>(tempBuffer.AsSpan()), ref remainder, token);
                cryptBuffer = ArrayPool<byte>.Shared.Rent(56);
                buffer.AsSpan(4, 28).CopyTo(cryptBuffer);
                buffer.AsSpan(4, 28).CopyTo(cryptBuffer.AsSpan(28));
                var loopCount = Math.DivRem(length - firstReadSize, stride, out var remainder);
                for (long i = 0, offset = firstReadSize; i < loopCount; i++, offset += stride)
                {
                    _ = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, stride), offset, token).ConfigureAwait(false);
                    CryptUtility.Decrypt(cryptBuffer.AsSpan(0, 56), buffer.AsSpan(0, stride));
                    InnerLoad(ref source, buffer.AsSpan(0, stride), MemoryMarshal.Cast<byte, char>(tempBuffer.AsSpan()), ref remainder, token);
                }
                if (remainder != 0)
                {
                    var actual = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, remainder), length - remainder, token);
                    CryptUtility.Decrypt(cryptBuffer.AsSpan(0, 56), buffer.AsSpan(0, actual));
                    InnerLoad(ref source, buffer.AsSpan(0, actual), MemoryMarshal.Cast<byte, char>(tempBuffer.AsSpan()), ref remainder, token);
                }
            }
            else
            {
                InnerLoad(ref source, buffer.AsSpan(0, firstReadSize), MemoryMarshal.Cast<byte, char>(tempBuffer.AsSpan()), ref remainder, token);
                long offset = firstReadSize;
                do
                {
                    var actual = await RandomAccess.ReadAsync(handle, buffer.AsMemory(), offset, token).ConfigureAwait(false);
                    if (actual == 0)
                    {
                        break;
                    }
                    actual &= -2;
                    InnerLoad(ref source, buffer.AsSpan(0, actual), MemoryMarshal.Cast<byte, char>(tempBuffer.AsSpan()), ref remainder, token);
                    offset += actual;
                } while (true);
            }
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

    private static void InnerLoad(ref DualList<char> source, ReadOnlySpan<byte> input, Span<char> tempSpan, ref byte? remainder, CancellationToken token)
    {
        ushort bytes4 = 0;
        var remainderSpan = MemoryMarshal.Cast<ushort, byte>(MemoryMarshal.CreateSpan(ref bytes4, 1));
        while (!input.IsEmpty)
        {
            token.ThrowIfCancellationRequested();
            if (remainder.HasValue)
            {
                remainderSpan[0] = remainder.Value;
                remainderSpan[1] = input[1];
                input = input[1..];
                _ = Encoding.GetChars(remainderSpan, tempSpan);
                source.Last.AddRange(tempSpan[..((tempSpan[0].IsSurrogatePair ? 1 : 0) + 1)]);
                remainder = default;
            }
            else if (input.Length == 1)
            {
                if (Encoding.GetChars(input, tempSpan) == 0)
                {
                    remainder = input[0];
                    return;
                }
                switch (tempSpan[0])
                {
                    case '\n':
                        source.AddEmpty();
                        return;
                    case '\r':
                        return;
                    default:
                        source.Last.Add(ref tempSpan[0]);
                        return;
                }
            }

            var readCount = Encoding.GetChars(input, tempSpan);
            input = input[readCount..];
            var content = tempSpan;
            while (!content.IsEmtpy)
            {
                var linefeedIndex = content.IndexOf('\n');
                ref var lastLine = ref source.Last;
                if (linefeedIndex < 0)
                {
                    lastLine.AddRange(content);
                    break;
                }
            
                lastLine.AddRange(content.Slice(0, linefeedIndex - ((linefeedIndex > 0 && content[linefeedIndex - 1] == '\r') ? 1 : 0)));
                source.AddEmpty();
                content = content.Slice(linefeedIndex + 1);
            }
        }
    }
}
