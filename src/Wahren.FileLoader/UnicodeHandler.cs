namespace Wahren.FileLoader;

public static class UnicodeHandler
{
    public static void LoadFromString(ReadOnlySpan<char> content, out DualList<char> source)
    {
        source = new DualList<char>();
        source.AddEmpty();

        if (content.IsEmpty)
        {
            return;
        }

        do
        {
            var linefeedIndex = content.IndexOf('\n');
            ref var lastLine = ref source.Last;
            if (linefeedIndex < 0)
            {
                lastLine.AddRange(content);
                return;
            }

            lastLine.AddRange(content.Slice(0, linefeedIndex - ((linefeedIndex > 0 && content[linefeedIndex - 1] == '\r') ? 1 : 0)));
            source.AddEmpty();
            content = content.Slice(linefeedIndex + 1);
        } while (!content.IsEmpty);
    }

    public static void Load(Span<byte> content, out DualList<char> source)
    {
        source = new DualList<char>();
        source.AddEmpty();

        if (content.Length >= 32 && Unsafe.As<byte, uint>(ref MemoryMarshal.GetReference(content)) == 0x04030201U)
        {
            CryptUtility.Decrypt(content.Slice(4, 28), content.Slice(32));
            content = content.Slice(32);
        }

        if (content.IsEmpty)
        {
            return;
        }

        var charSpan = MemoryMarshal.Cast<byte, char>(content);
        do
        {
            var linefeedIndex = charSpan.IndexOf('\n');
            ref var lastLine = ref source.Last;
            if (linefeedIndex == -1)
            {
                lastLine.AddRange(charSpan);
                return;
            }

            lastLine.AddRange(charSpan.Slice(0, linefeedIndex - ((linefeedIndex > 0 && charSpan[linefeedIndex - 1] == '\r') ? 1 : 0)));
            source.AddEmpty();
            charSpan = charSpan.Slice(linefeedIndex + 1);
        } while (!charSpan.IsEmpty);
    }

    public static async ValueTask<DualList<char>> LoadAsync(SafeFileHandle handle, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var source = new DualList<char>();
        source.AddEmpty();
        var length = RandomAccess.GetLength(handle);
        if (length == 0L) { return source; }
        const int stride = 1024 * 7 * 8;
        var buffer = ArrayPool<byte>.Shared.Rent(4 + 28 + stride);
        byte[]? cryptBuffer = null;
        try
        {
            const int firstReadSize = 4 + 28 + 32 * 7;
            if (length < firstReadSize)
            {
                _ = await RandomAccess.ReadAsync(handle, new Memory<byte>(buffer, 0, (int)length), 0, token).ConfigureAwait(false);
                if (length >= 32 && Unsafe.As<byte, uint>(ref buffer[0]) == 0x04030201U)
                {
                    CryptUtility.Decrypt(buffer.AsSpan(4, 28), buffer.AsSpan(32, (int)length - 32));
                    InnerLoad(ref source, buffer.AsSpan(32, (int)length - 32), token);
                }
                else
                {
                    InnerLoad(ref source, buffer.AsSpan(0, (int)length), token);
                }
                return source;
            }

            _ = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, firstReadSize), 0, token).ConfigureAwait(false);
            if (Unsafe.As<byte, uint>(ref buffer[0]) == 0x04030201)
            {
                CryptUtility.Decrypt(buffer.AsSpan(4, 28), buffer.AsSpan(32, firstReadSize - 32));
                InnerLoad(ref source, buffer.AsSpan(32, firstReadSize - 32), token);
                cryptBuffer = ArrayPool<byte>.Shared.Rent(56);
                buffer.AsSpan(4, 28).CopyTo(cryptBuffer);
                buffer.AsSpan(4, 28).CopyTo(cryptBuffer.AsSpan(28));
                var loopCount = Math.DivRem(length - firstReadSize, stride, out var remainder);
                for (long i = 0, offset = firstReadSize; i < loopCount; i++, offset += stride)
                {
                    _ = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, stride), offset, token).ConfigureAwait(false);
                    CryptUtility.Decrypt(cryptBuffer.AsSpan(0, 56), buffer.AsSpan(0, stride));
                    InnerLoad(ref source, buffer.AsSpan(0, stride), token);
                }
                if (remainder != 0)
                {
                    var actual = await RandomAccess.ReadAsync(handle, new Memory<byte>(buffer, 0, (int)remainder), length - remainder, token);
                    CryptUtility.Decrypt(cryptBuffer.AsSpan(0, 56), buffer.AsSpan(0, actual));
                    InnerLoad(ref source, buffer.AsSpan(0, actual), token);
                }
            }
            else
            {
                InnerLoad(ref source, buffer.AsSpan(0, firstReadSize), token);
                long offset = firstReadSize;
                do
                {
                    var actual = await RandomAccess.ReadAsync(handle, new Memory<byte>(buffer), offset, token).ConfigureAwait(false);
                    if (actual == 0)
                    {
                        break;
                    }
                    actual &= -2;
                    InnerLoad(ref source, buffer.AsSpan(0, actual), token);
                    offset += actual;
                } while (true);
            }
            return source;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            if (cryptBuffer is not null)
            {
                ArrayPool<byte>.Shared.Return(cryptBuffer);
            }
        }
    }

    private static void InnerLoad(ref DualList<char> source, ReadOnlySpan<byte> input, CancellationToken token)
    {
        var charSpan = MemoryMarshal.Cast<byte, char>(input);
        do
        {
            token.ThrowIfCancellationRequested();
            var linefeedIndex = charSpan.IndexOf('\n');
            ref var lastLine = ref source.Last;
            if (linefeedIndex < 0)
            {
                lastLine.AddRange(charSpan);
                return;
            }

            lastLine.AddRange(charSpan.Slice(0, linefeedIndex - ((linefeedIndex > 0 && charSpan[linefeedIndex - 1] == '\r') ? 1 : 0)));
            source.AddEmpty();
            charSpan = charSpan.Slice(linefeedIndex + 1);
        } while (!charSpan.IsEmpty);
    }
}
