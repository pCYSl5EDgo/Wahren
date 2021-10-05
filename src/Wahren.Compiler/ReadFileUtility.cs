namespace Wahren.Compiler;

public static class ReadFileUtility
{
    public static async ValueTask<(byte[] array, int size)> ReadAsync(string path, CancellationToken token)
    {
        using var handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous);
        var length = RandomAccess.GetLength(handle);
        if (length == 0)
        {
            return (Array.Empty<byte>(), 0);
        }

        if (length > int.MaxValue)
        {
            throw new InsufficientMemoryException(path);
        }

        var buffer = ArrayPool<byte>.Shared.Rent((int)length);
        try
        {
            var actual = await RandomAccess.ReadAsync(handle, buffer.AsMemory(0, (int)length), 0, token).ConfigureAwait(false);
            return (buffer, actual);
        }
        catch
        {
            ArrayPool<byte>.Shared.Return(buffer);
            throw;
        }
    }
}
