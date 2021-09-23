using Wahren.PooledList;
using Wahren.AbstractSyntaxTree.Formatter;
namespace Wahren.Compiler;

public partial class Program
{
    [Command(new string[] {
        "format",
    })]
    public async ValueTask<int> Format(
        [Option(0, "input folder")] string rootFolder = ".",
        [Option("switch")] bool @switch = false,
        [Option("t")] bool time = false
    )
    {
        var stopwatch = time ? Stopwatch.StartNew() : null;
        CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromMinutes(1));

        var debugPaper = await GetDebugPaper(rootFolder, cancellationTokenSource.Token).ConfigureAwait(false);
        var contentsFolder = debugPaper.Folder ?? DetectContentsFolder(rootFolder);
        if (contentsFolder is null)
        {
            Console.Error.WriteLine("Contents folder is not found.\n\nContents folder contains 'script'/'image'/'stage' folders.");
            return 1;
        }

        var scriptFolderPath = Path.GetRelativePath(Environment.CurrentDirectory, Path.GetFullPath(Path.Combine(contentsFolder, "script")));
        var (isUnicode, isEnglish) = await IsEnglish(scriptFolderPath);
        try
        {
            var enumerates = Directory.EnumerateFiles(scriptFolderPath, "*.dat", SearchOption.AllDirectories);
            await Parallel.ForEachAsync(enumerates, cancellationTokenSource.Token, 
                (path, token) =>
                {
                    return ProcessEachFilesOverwrite(path, @switch, isUnicode, isEnglish, token);
                }).ConfigureAwait(false);
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

    private static async ValueTask ProcessEachFilesOverwrite(string path, bool @switch, bool isUnicode, bool isEnglish, CancellationToken token)
    {
        Result result;
        byte[]? rental;
        var handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.SequentialScan | FileOptions.Asynchronous, 4096);
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

                result = new();
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

        Context context = new(0, treatSlashPlusAsSingleLineComment: @switch, isEnglishMode: isEnglish, DiagnosticSeverity.Error);
        Parser.Parse(ref context, ref result);
        var byteList = new List<byte>();
        try
        {
            if (isUnicode)
            {
                byteList.Add(0xff);
                byteList.Add(0xfe);
                var formatter = BinaryFormatter.GetDefault_Utf16Le(true);
                if (!formatter.TryFormat(ref result, ref byteList))
                {
                    return;
                }
            }
            else
            {
                var formatter = BinaryFormatter.GetDefault_Cp932(true);
                if (!formatter.TryFormat(ref result, ref byteList))
                {
                    return;
                }
            }

            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, false);
            stream.Write(byteList.AsSpan());
        }
        finally
        {
            byteList.Dispose();
        }
    }
}
