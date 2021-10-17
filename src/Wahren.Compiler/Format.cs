using Wahren.AbstractSyntaxTree.Formatter;
using Wahren.PooledList;
namespace Wahren.Compiler;

public partial class Program
{
    [Command(new string[] {
        "format",
    })]
    public async ValueTask<int> Format(
        [Option(0, "input folder")] string rootFolder = ".",
        [Option("switch")] bool @switch = false,
        [Option("t")] bool time = false,
        [Option("unicode")] bool forceUnicode = false,
        [Option("delete")] bool deleteDiscardedToken = false
    )
    {
        var stopwatch = time ? Stopwatch.StartNew() : null;
        using var cancellationTokenSource = PrepareCancellationTokenSource(TimeSpan.FromMinutes(1));
        try
        {
            var (success, contentsFolder, scriptFolderPath, scriptFiles, isUnicode, isEnglish) = await GetInitialSettingsAsync(rootFolder, cancellationTokenSource.Token).ConfigureAwait(false);
            if (!success)
            {
                return 1;
            }

            var toUnicodeFolderStructureTask = ToUnicodeFolderStructure(isUnicode, forceUnicode, scriptFolderPath, cancellationTokenSource);
            await Parallel.ForEachAsync(scriptFiles, cancellationTokenSource.Token,
                (path, token) =>
                {
                    return ProcessEachFilesOverwrite(path, @switch, isUnicode, isEnglish, forceUnicode, deleteDiscardedToken, token);
                }).ConfigureAwait(false);
            await toUnicodeFolderStructureTask.ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.ToString());
            return 1;
        }
        finally
        {
            stopwatch?.Stop();
            if (stopwatch is not null)
            {
                var milliseconds = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"{milliseconds} milli seconds passed.");
            }
        }

        return 0;
    }

    private static ValueTask ToUnicodeFolderStructure(bool isUnicode, bool forceUnicode, string scriptFolderPath, CancellationTokenSource cancellationTokenSource)
    {
        if (isUnicode || !forceUnicode)
        {
            return ValueTask.CompletedTask;
        }

        File.Create(Path.Combine(scriptFolderPath, "unicode.txt")).Dispose();
        string path_dot_gitattributes = Path.Combine(scriptFolderPath, ".gitattributes");
        if (File.Exists(path_dot_gitattributes))
        {
            return ValueTask.CompletedTask;
        }

        static async ValueTask ToUnicodeFolderStructure(CancellationTokenSource cancellationTokenSource, string scriptFolderPath, string path_dot_gitattributes)
        {
            const string content = "*.dat text working-tree-encoding=UTF-16LE-BOM eol=CRLF";
            var buffer = ArrayPool<byte>.Shared.Rent(content.Length);
            try
            {
                using var stream = File.Create(path_dot_gitattributes, 4096);
                stream.Write(buffer.AsSpan(0, Encoding.UTF8.GetBytes(content.AsSpan(), buffer.AsSpan())));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            var enumerates = Directory.EnumerateFiles(scriptFolderPath, "language*.txt", SearchOption.TopDirectoryOnly);
            await Parallel.ForEachAsync(enumerates, cancellationTokenSource.Token,
                async (path, token) =>
                {
                    Microsoft.Win32.SafeHandles.SafeFileHandle? handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous);
                    try
                    {
                        var bytesCount = RandomAccess.GetLength(handle);
                        if (bytesCount > int.MaxValue)
                        {
                            throw new InsufficientMemoryException();
                        }
                        var originalBuffer = ArrayPool<byte>.Shared.Rent((int)bytesCount);
                        try
                        {
                            var actualBytesCount = await RandomAccess.ReadAsync(handle, originalBuffer.AsMemory(0, (int)bytesCount), 0, token).ConfigureAwait(false);
                            handle.Dispose();
                            handle = default;

                            var sjis = Encoding.GetEncoding(932);
                            var maximum = sjis.GetMaxCharCount(actualBytesCount);
                            var charBuffer = ArrayPool<char>.Shared.Rent(maximum);
                            try
                            {
                                var charCount = sjis.GetChars(originalBuffer.AsSpan(0, actualBytesCount), charBuffer.AsSpan());
                                var writingBuffer = ArrayPool<byte>.Shared.Rent((charCount + 1) << 1);
                                try
                                {
                                    writingBuffer[0] = 0xff;
                                    writingBuffer[1] = 0xfe;
                                    MemoryMarshal.Cast<char, byte>(charBuffer.AsSpan(0, charCount)).CopyTo(writingBuffer.AsSpan(2));
                                    using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 0, false);
                                    stream.Write(writingBuffer.AsSpan(0, (charCount + 1) << 1));
                                }
                                finally
                                {
                                    ArrayPool<byte>.Shared.Return(writingBuffer);
                                }
                            }
                            finally
                            {
                                ArrayPool<char>.Shared.Return(charBuffer);
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(originalBuffer);
                        }
                    }
                    finally
                    {
                        handle?.Dispose();
                    }
                }).ConfigureAwait(false);
        }

        return ToUnicodeFolderStructure(cancellationTokenSource, scriptFolderPath, path_dot_gitattributes);
    }

    private static async ValueTask ProcessEachFilesOverwrite(string path, bool @switch, bool isUnicode, bool isEnglish, bool forceUnicode, bool deleteDiscardedToken, CancellationToken token)
    {
        Result result;
        byte[]? rental;
        var handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.SequentialScan | FileOptions.Asynchronous);
        try
        {
            var length = RandomAccess.GetLength(handle);
            if (length == 0)
            {
                return;
            }

            rental = ArrayPool<byte>.Shared.Rent((int)length);
            try
            {
                var actual = await RandomAccess.ReadAsync(handle, rental.AsMemory(0, (int)length), 0, token).ConfigureAwait(false);
                if (actual == 0)
                {
                    return;
                }

                result = new(0);
                if (isUnicode)
                {
                    UnicodeHandler.Load(rental.AsSpan(0, actual), out result.Source);
                }
                else
                {
                    Cp932Handler.Load(rental.AsSpan(0, actual), out result.Source);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rental);
            }
        }
        finally
        {
            handle.Dispose();
        }

        Context context = new(treatSlashPlusAsSingleLineComment: @switch, isEnglishMode: isEnglish, deleteDiscardedToken, DiagnosticSeverity.Error);
        Parser.Parse(ref context, ref result);
        var byteList = new List<byte>();
        try
        {
            if (isUnicode || forceUnicode)
            {
                byteList.Add(0xff);
                byteList.Add(0xfe);
                var formatter = BinaryFormatter.GetDefault_Utf16Le(true);
                if (!formatter.TryFormat(ref result, ref byteList))
                {
                    StringBuilder stringBuilder = new();
                    foreach (var error in result.ErrorList)
                    {
                        AppendResultError(stringBuilder, false, result.FilePath, error);
                    }
                    Console.WriteLine(stringBuilder.ToString());

                    if (isUnicode || !forceUnicode)
                    {
                        return;
                    }

                    for (int i = 0; i < result.Source.Count; i++)
                    {
                        var line = result.Source[i];
                        if (i != 0)
                        {
                            byteList.Add((byte)'\r');
                            byteList.Add(0);
                            byteList.Add((byte)'\n');
                            byteList.Add(0);
                        }

                        byteList.AddRange(MemoryMarshal.Cast<char, byte>(line.AsSpan()));
                    }
                }
            }
            else
            {
                var formatter = BinaryFormatter.GetDefault_Cp932(true);
                if (!formatter.TryFormat(ref result, ref byteList))
                {
                    StringBuilder stringBuilder = new();
                    foreach (var error in result.ErrorList)
                    {
                        AppendResultError(stringBuilder, false, result.FilePath, error);
                    }
                    Console.WriteLine(stringBuilder.ToString());
                    return;
                }
            }

            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 0, false);
            stream.Write(byteList.AsSpan());
        }
        finally
        {
            byteList.Dispose();
        }
    }
}
