global using System;
global using System.Buffers;
global using System.IO;
global using System.Runtime.InteropServices;
global using System.Threading;
global using Wahren.AbstractSyntaxTree.Parser;
global using Wahren.FileLoader;
global using System.Diagnostics;
global using System.Text;

namespace Wahren.Compiler;

public partial class Program
{
    public async ValueTask<int> Run(
        [Option(0, "input folder")] string rootFolder = ".",
        [Option("s")] PseudoDiagnosticSeverity severity = PseudoDiagnosticSeverity.Error,
        [Option("t")] bool time = true
    )
    {
        var result = await Analyze(rootFolder, false, severity, time);
        Console.WriteLine("Press Enter Key...");
        Console.ReadLine();
        return result;
    }

    [Command(new string[] {
        "analyze",
    })]
    public async ValueTask<int> Analyze(
        [Option(0, "input folder")] string rootFolder = ".",
        [Option("switch")] bool @switch = false,
        [Option("s")] PseudoDiagnosticSeverity severity = PseudoDiagnosticSeverity.Error,
        [Option("t")] bool time = false
    )
    {
        var stopwatch = time ? Stopwatch.StartNew() : null;
        CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromMinutes(1));

        var debugPaper = await GetDebugPaper(rootFolder, cancellationTokenSource.Token).ConfigureAwait(false);
        string? contentsFolder;
        if (debugPaper.Folder is null)
        {
            contentsFolder = DetectContentsFolder(rootFolder);
        }
        else
        {
            contentsFolder = Path.Combine(rootFolder, debugPaper.Folder);
        }

        if (contentsFolder is null || !Directory.Exists(contentsFolder))
        {
            Console.Error.WriteLine("Contents folder is not found.\n\nContents folder contains 'script'/'image'/'stage' folders.");
            return 1;
        }

        var scriptFolderPath = Path.Combine(contentsFolder, "script");
        var (isUnicode, isEnglish) = await IsEnglish(scriptFolderPath).ConfigureAwait(false);
        var files = Directory.GetFiles(scriptFolderPath, "*.dat", SearchOption.AllDirectories);
        var solution = new Solution()
        {
            RequiredSeverity = (DiagnosticSeverity)(uint)severity
        };
        try
        {
            solution.Files.PrepareAddRange(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                solution.Files.Add(new(solution, (uint)i));
            }

            await Parallel.ForEachAsync(System.Linq.Enumerable.Range(0, files.Length), cancellationTokenSource.Token, async (int index, CancellationToken token) =>
            {
                byte[] rental;
                int actual;
                solution.Files[index].FilePath = Path.GetRelativePath(Environment.CurrentDirectory, files[index]);
                var handle = File.OpenHandle(files[index], FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous, 4096);
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
                        actual = await RandomAccess.ReadAsync(handle, rental.AsMemory(0, (int)length), 0, token).ConfigureAwait(false);
                    }
                    catch
                    {
                        ArrayPool<byte>.Shared.Return(rental);
                        throw;
                    }
                }
                finally
                {
                    handle.Dispose();
                }

                try
                {
                    LoadAndParse(solution.RequiredSeverity, @switch, isUnicode, isEnglish, ref solution.Files[index], rental.AsSpan(0, actual));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(rental);
                }
            }).ConfigureAwait(false);
            
            if (!CompileResultSync(solution))
            {
                return 0;
            }

            if (!CheckSync(solution))
            {
                return 0;
            }
        }
        finally
        {
            solution.Dispose();
            stopwatch?.Stop();
            if (stopwatch is not null)
            {
                var milliseconds = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"{milliseconds} milli seconds passed.");
            }
        }

        return 0;
    }

    private bool CheckSync(Solution solution)
    {
        var success = solution.CheckExistance();
        if (!success)
        {
            foreach (var error in solution.SolutionErrorList)
            {
                Console.WriteLine(error.Text);
            }
        }

        return success;
    }

    private static bool CompileResultSync(Solution solution)
    {
        bool isSuccess = true;
        foreach (ref var file in solution.Files.AsSpan())
        {
            isSuccess &= file.NoError();
        }
        if (isSuccess)
        {
            solution.AddReferenceAndValidate();
        }

        StringBuilder? stringBuilder = null;
        bool showNotError = solution.RequiredSeverity != 0U;
        foreach (ref var result in solution.Files)
        {
            if (result.ErrorList.IsEmpty)
            {
                continue;
            }

            for (uint j = 0; j < result.ErrorList.Count; j++)
            {
                var error = result.ErrorList[j];
                if (error.Severity == DiagnosticSeverity.Error)
                {
                    isSuccess = false;
                }

                stringBuilder ??= new(1 << 14);
                AppendResultError(stringBuilder, showNotError, result.FilePath, error);
            }
        }

        if (stringBuilder is not null)
        {
            Console.WriteLine(stringBuilder.ToString());
        }
        return isSuccess;
    }

    private static void AppendResultError(StringBuilder stringBuilder, bool showNotError, string? filePath, Error error)
    {
        stringBuilder.AppendLine(error.Text);
        stringBuilder.Append(filePath);
        stringBuilder.Append('(');
        stringBuilder.Append(error.Range.StartInclusive.Line + 1);
        stringBuilder.Append(", ");
        stringBuilder.Append(error.Range.StartInclusive.Offset + 1);
        stringBuilder.Append(')');
        if (showNotError)
        {
            stringBuilder.Append(", Severity: ");
            stringBuilder.Append(error.Severity);
        }

        stringBuilder.AppendLine();
    }

    private static void LoadAndParse(DiagnosticSeverity severity, bool treatSlashPlusAsSingleLineComment, bool isUnicode, bool isEnglish, ref Result result, Span<byte> input)
    {
        if (isUnicode)
        {
            UnicodeHandler.Load(input, out result.Source);
        }
        else
        {
            Cp932Handler.Load(input, out result.Source);
        }

        Context context = new(treatSlashPlusAsSingleLineComment, isEnglish, severity);
        result.Success = Parser.Parse(ref context, ref result);
        NodeValidator.AddReferenceAndValidate(ref context, ref result);
        if (result.Success)
        {
            ref var errorList = ref result.ErrorList;
            for (uint i = 0, end = (uint)errorList.Count; i != end; i++)
            {
                if (errorList[i].Severity == DiagnosticSeverity.Error)
                {
                    result.Success = false;
                    break;
                }
            }
        }
    }
}
