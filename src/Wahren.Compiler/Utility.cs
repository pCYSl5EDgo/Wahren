namespace Wahren.Compiler;

public partial class Program
{
    private static async ValueTask<(bool isUnicode, bool isEnglish)> IsEnglish(string scriptFolderPath)
    {
        var unicode = Path.Combine(scriptFolderPath, "unicode.txt");
        if (!File.Exists(unicode))
        {
            return (false, File.Exists(Path.Combine(scriptFolderPath, "english.txt")));
        }

        using var handle = File.OpenHandle(unicode, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous);
        var length = RandomAccess.GetLength(handle);
        if (length == 0)
        {
            return (true, false);
        }

        var rental = ArrayPool<byte>.Shared.Rent((int)length);
        try
        {
            var actual = await RandomAccess.ReadAsync(handle, rental.AsMemory(0, (int)length), 0, default).ConfigureAwait(false);
            if ((actual & 1) != 1)
            {
                return (true, false);
            }

            static bool IsEnglishInner(byte[] array, int actual)
            {
                Span<char> span = MemoryMarshal.Cast<byte, char>(array.AsSpan(0, actual));
                if (span.IsEmpty)
                {
                    return false;
                }

                if (span[0] == '\ufeff')
                {
                    span = span.Slice(1);
                }

                return span.TrimStart().StartsWith("foreign");
            }

            return (true, IsEnglishInner(rental, actual));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rental);
        }
    }

    private static async ValueTask<DebugPaper.DebugPaper> GetDebugPaper(string rootFolder, CancellationToken token)
    {
        var path_debug_paper_txt = Path.Combine(rootFolder, "debug_paper.txt");
        if (File.Exists(path_debug_paper_txt))
        {
            using var handle = File.OpenHandle(path_debug_paper_txt, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous);
            var count = RandomAccess.GetLength(handle);
            if (count > int.MaxValue)
            {
                return DebugPaper.DebugPaper.DefaultDebug;
            }

            var rental = ArrayPool<byte>.Shared.Rent((int)count);
            try
            {
                var actual = await RandomAccess.ReadAsync(handle, rental.AsMemory(0, (int)count), 0, token).ConfigureAwait(false);
                var answer = DebugPaper.DebugPaper.CreateFromSpan(rental.AsSpan(0, actual), true, out var error);
                if (error is not null)
                {
                    Console.WriteLine(error);
                }
                return answer ?? DebugPaper.DebugPaper.DefaultDebug;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rental);
            }
        }

        var path_a_system_debug_paper_txt = Path.Combine(rootFolder, "a_system", "debug_paper.txt");
        if (File.Exists(path_a_system_debug_paper_txt))
        {
            using var handle = File.OpenHandle(path_a_system_debug_paper_txt, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous);
            var count = RandomAccess.GetLength(handle);
            if (count > int.MaxValue)
            {
                return DebugPaper.DebugPaper.DefaultDebug;
            }

            var rental = ArrayPool<byte>.Shared.Rent((int)count);
            try
            {
                var actual = await RandomAccess.ReadAsync(handle, rental.AsMemory(0, (int)count), 0, token).ConfigureAwait(false);
                var answer = DebugPaper.DebugPaper.CreateFromSpan(rental.AsSpan(0, actual), false, out var error);
                if (error is not null)
                {
                    Console.WriteLine(error);
                }
                return answer ?? DebugPaper.DebugPaper.DefaultRelease;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rental);
            }
        }

        return DebugPaper.DebugPaper.DefaultRelease;
    }

    private static string? DetectContentsFolder(string rootFolder)
    {
        Span<char> a_system = stackalloc char["a_system".Length + 1];
        "a_system".CopyTo(a_system.Slice(1));
        a_system[0] = Path.DirectorySeparatorChar;
        foreach (var topDirectory in Directory.EnumerateDirectories(rootFolder, "*", SearchOption.TopDirectoryOnly))
        {
            if (topDirectory.AsSpan().EndsWith(a_system, StringComparison.Ordinal))
            {
                continue;
            }

            if (Directory.Exists(Path.Combine(topDirectory, "script")) && Directory.Exists(Path.Combine(topDirectory, "image")) && Directory.Exists(Path.Combine(topDirectory, "stage")))
            {
                return topDirectory;
            }
        }

        return null;
    }
}