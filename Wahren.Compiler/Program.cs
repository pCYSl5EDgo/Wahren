using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Wahren.AbstractSyntaxTree.Parser;
using Wahren.FileLoader;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text;

namespace Wahren.Compiler;

public class Program : ConsoleAppBase
{
    public async static Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
    }

    public async ValueTask<int> Run(
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

    private static async ValueTask<(bool isUnicode, bool isEnglish)> IsEnglish(string scriptFolderPath)
    {
        var unicode = Path.Combine(scriptFolderPath, "unicode.txt");
        if (!File.Exists(unicode))
        {
            return (false, File.Exists(Path.Combine(scriptFolderPath, "english.txt")));
        }

        using var handle = File.OpenHandle(unicode, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous, 4096);
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
            using var handle = File.OpenHandle(path_debug_paper_txt, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous, 4096);
            var count = RandomAccess.GetLength(handle);
            if (count > int.MaxValue)
            {
                return DebugPaper.DebugPaper.DefaultDebug;
            }

            var rental = ArrayPool<byte>.Shared.Rent((int)count);
            try
            {
                var actual = await RandomAccess.ReadAsync(handle, rental.AsMemory(0, (int)count), 0, token).ConfigureAwait(false);
                var answer = DebugPaper.DebugPaper.CreateFromSpan(rental.AsSpan(0, actual), true);
                return  answer ?? DebugPaper.DebugPaper.DefaultDebug;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rental);
            }
        }

        var path_a_system_debug_paper_txt = Path.Combine(rootFolder, "a_system", "debug_paper.txt");
        if (File.Exists(path_a_system_debug_paper_txt))
        {
            using var handle = File.OpenHandle(path_a_system_debug_paper_txt, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous, 4096);
            var count = RandomAccess.GetLength(handle);
            if (count > int.MaxValue)
            {
                return DebugPaper.DebugPaper.DefaultDebug;
            }

            var rental = ArrayPool<byte>.Shared.Rent((int)count);
            try
            {
                var actual = await RandomAccess.ReadAsync(handle, rental.AsMemory(0, (int)count), 0, token).ConfigureAwait(false);
                var answer = DebugPaper.DebugPaper.CreateFromSpan(rental.AsSpan(0, actual), false);
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

public enum PseudoDiagnosticSeverity : uint
{
    Error = 0,
    Warning = 1,
    Info = 2,
    Hint = 3,
    error = 0,
    warning = 1,
    info = 2,
    hint = 3,
    e = 0,
    w = 1,
    i = 2,
    h = 3,
}
