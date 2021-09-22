namespace Wahren.Compiler;

public partial class Program
{
    [Command(new string[] {
        "format",
    })]
    public async Task<int> Format(
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
            UnicodeEncoding encoding = new UnicodeEncoding(false, byteOrderMark: true);
            foreach (var path in Directory.EnumerateFiles(scriptFolderPath, "*.dat", SearchOption.AllDirectories))
            {
                ProcessEachFilesOverwrite(path, @switch, isUnicode, isEnglish, encoding);
            }
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

    private static void ProcessEachFilesOverwrite(string path, bool @switch, bool isUnicode, bool isEnglish, UnicodeEncoding encoding)
    {
        Result result;
        byte[]? rental;
        var handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.SequentialScan, 4096);
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
                var actual = RandomAccess.Read(handle, rental.AsSpan(0, (int)length), 0);
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
        var success = Parser.Parse(ref context, ref result);
        var formatCode = result.ToString("b");
        File.WriteAllText(path, formatCode, encoding);
    }
}
