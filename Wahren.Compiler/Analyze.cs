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
        var contentsFolder = debugPaper.Folder ?? DetectContentsFolder(rootFolder);
        if (contentsFolder is null)
        {
            Console.Error.WriteLine("Contents folder is not found.\n\nContents folder contains 'script'/'image'/'stage' folders.");
            return 1;
        }

        var scriptFolderPath = Path.GetRelativePath(Environment.CurrentDirectory, Path.GetFullPath(Path.Combine(contentsFolder, "script")));
        var (isUnicode, isEnglish) = await IsEnglish(scriptFolderPath);
        PooledList.List<ValueTask<Result>> list = new();
        PooledList.DisposableList<Result> results = new();
        var stringBuilder = new StringBuilder(1 << 14);
        try
        {
            nuint index = default;
            foreach (var path in Directory.EnumerateFiles(scriptFolderPath, "*.dat", SearchOption.AllDirectories))
            {
                list.Add(ProcessEachFiles(path, ((uint)severity << 3) | (isUnicode ? 1U : 0U) | (isEnglish ? 2U : 0U) | (@switch ? 4U : 0U), index++, cancellationTokenSource.Token));
            }

            for (uint i = 0; i < list.Count; i++)
            {
                var result = await list[i].ConfigureAwait(false);
                results.Add(ref result);
                if (result.ErrorList.IsEmpty)
                {
                    continue;
                }

                stringBuilder.AppendLine("\n==== " + (result.Data as string) + " ====");

                for (uint j = 0; j < result.ErrorList.Count; j++)
                {
                    var error = result.ErrorList[j];
                    stringBuilder.Append(error.Text);
                    stringBuilder.Append("    Line: ");
                    stringBuilder.Append(error.Range.StartInclusive.Line + 1);
                    stringBuilder.Append(" Index: ");
                    stringBuilder.Append(error.Range.StartInclusive.Offset + 1);
                    if (severity != PseudoDiagnosticSeverity.Error)
                    {
                        stringBuilder.Append(" Severity: ");
                        stringBuilder.Append(error.Severity);
                    }
                    stringBuilder.AppendLine();
                    //if (severity == DiagnosticSeverity.Error)
                    //{
                    //    cancellationTokenSource.Cancel();
                    //}
                }
            }
        }
        finally
        {
            results.Dispose();
            list.Dispose();
            stopwatch?.Stop();
            Console.WriteLine(stringBuilder.ToString());
            if (stopwatch is not null)
            {
                var milliseconds = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"{milliseconds} milli seconds passed.");
            }
        }

        return 0;
    }

    private static async ValueTask<Result> ProcessEachFiles(string path, uint flag, nuint index, CancellationToken token)
    {
        using var handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous, 4096);
        var length = RandomAccess.GetLength(handle);
        if (length == 0)
        {
            return default;
        }

        var rental = ArrayPool<byte>.Shared.Rent((int)length);
        try
        {
            var actual = await RandomAccess.ReadAsync(handle, rental.AsMemory(0, (int)length), 0, token).ConfigureAwait(false);
            Result result = new(index);
            token.ThrowIfCancellationRequested();
            if ((flag & 1U) == 1U)
            {
                UnicodeHandler.Load(rental.AsSpan(0, actual), out result.Source);
            }
            else
            {
                Cp932Handler.Load(rental.AsSpan(0, actual), out result.Source);
            }

            token.ThrowIfCancellationRequested();
            Context context = new(index, (flag & 4U) != 0U, (flag & 2U) != 0U, (DiagnosticSeverity)(flag >> 3));
            result.Success = Parser.Parse(ref context, ref result);
            if (result.Success)
            {
                for (uint i = 0, end = (uint)result.ErrorList.Count; i != end; i++)
                {
                    if (result.ErrorList[i].Severity == DiagnosticSeverity.Error)
                    {
                        result.Success = false;
                        break;
                    }
                }
            }
            result.Data = path;
            return result;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rental);
        }
    }
}
