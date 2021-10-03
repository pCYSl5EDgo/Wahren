global using System;
global using System.Buffers;
global using System.IO;
global using System.Runtime.InteropServices;
global using System.Threading;
global using Wahren.AbstractSyntaxTree.Parser;
global using Wahren.AbstractSyntaxTree.Project;
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
        Console.CancelKeyPress += new((object? _, ConsoleCancelEventArgs args) =>
        {
            cancellationTokenSource?.Cancel();
            args.Cancel = true;
        });
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
        var project = new Project()
        {
            RequiredSeverity = (DiagnosticSeverity)(uint)severity
        };
        try
        {
            project.Files.PrepareAddRange(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                project.Files.Add(new((uint)i));
            }

            await Parallel.ForEachAsync(System.Linq.Enumerable.Range(0, files.Length), cancellationTokenSource.Token, async (int index, CancellationToken token) =>
            {
                byte[] rental;
                int actual;
                project.Files[index].FilePath = Path.GetRelativePath(Environment.CurrentDirectory, files[index]);
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
                    token.ThrowIfCancellationRequested();
                    LoadAndParse(project.RequiredSeverity, @switch, isUnicode, isEnglish, ref project.Files[index], rental.AsSpan(0, actual));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(rental);
                }
            }).ConfigureAwait(false);

            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!CompileResultSync(project))
            {
                return 0;
            }

            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!CheckSync(project))
            {
                return 0;
            }

            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await Parallel.ForEachAsync(System.Linq.Enumerable.Range(0, 8), cancellationTokenSource.Token, (int index, CancellationToken token) =>
            {
                System.Collections.Generic.HashSet<string> hashSet = new();
                switch (index)
                {
                    case 0:
                        foreach (ref var file in project.Files.AsSpan())
                        {
                            for (uint i = 0, count = file.imagedataSet.Count; i != count; ++i)
                            {
                                hashSet.Add(new(file.imagedataSet[i]));
                            }
                        }
                        return EnsureExistanceImageDataAsync(project, token, hashSet);
                    case 1:
                        return EnsureExistanceImageData2Async(project, token, hashSet);
                    case 8:
                        foreach (ref var file in project.Files.AsSpan())
                        {
                            for (uint i = 0, count = file.mapSet.Count; i != count; ++i)
                            {
                                hashSet.Add(new(file.mapSet[i]));
                            }
                        }
                        token.ThrowIfCancellationRequested();
                        foreach (var name in hashSet)
                        {
                            var original = $"{contentsFolder}/stage/{name}.map";
                            if (!File.Exists(original))
                            {
                                Console.WriteLine($"file map '{name}' is not found.");
                            }
                        }
                        break;
                    case 9:
                        foreach (ref var file in project.Files.AsSpan())
                        {
                            for (uint i = 0, count = file.bgmSet.Count; i != count; ++i)
                            {
                                hashSet.Add(new(file.bgmSet[i]));
                            }
                        }
                        token.ThrowIfCancellationRequested();
                        foreach (var name in hashSet)
                        {
                            var original = $"{contentsFolder}/bgm/{name}";
                            if (!File.Exists(original) && !File.Exists(original + ".mp3") && !File.Exists(original + ".ogg") && !File.Exists(original + ".mid"))
                            {
                                Console.WriteLine($"file bgm '{name}' is not found.");
                            }
                        }
                        break;
                    case 2:
                        foreach (ref var file in project.Files.AsSpan())
                        {
                            for (uint i = 0, count = file.faceSet.Count; i != count; ++i)
                            {
                                hashSet.Add(new(file.faceSet[i]));
                            }
                        }
                        token.ThrowIfCancellationRequested();
                        foreach (var name in hashSet)
                        {
                            var original = $"{contentsFolder}/face/{name}";
                            if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                            {
                                Console.WriteLine($"file face '{name}' is not found.");
                            }
                        }
                        break;
                    case 3:
                        foreach (ref var file in project.Files.AsSpan())
                        {
                            for (uint i = 0, count = file.iconSet.Count; i != count; ++i)
                            {
                                hashSet.Add(new(file.iconSet[i]));
                            }
                        }
                        token.ThrowIfCancellationRequested();
                        foreach (var name in hashSet)
                        {
                            var original = $"{contentsFolder}/icon/{name}";
                            if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                            {
                                Console.WriteLine($"file icon '{name}' is not found.");
                            }
                        }
                        break;
                    case 4:
                        foreach (ref var file in project.Files.AsSpan())
                        {
                            for (uint i = 0, count = file.soundSet.Count; i != count; ++i)
                            {
                                hashSet.Add(new(file.soundSet[i]));
                            }
                        }
                        token.ThrowIfCancellationRequested();
                        foreach (var name in hashSet)
                        {
                            var original = $"{contentsFolder}/sound/{name}.wav";
                            if (!File.Exists(original))
                            {
                                Console.WriteLine($"file sound '{name}' is not found.");
                            }
                        }
                        break;
                    case 5:
                        foreach (ref var file in project.Files.AsSpan())
                        {
                            for (uint i = 0, count = file.pictureSet.Count; i != count; ++i)
                            {
                                hashSet.Add(new(file.pictureSet[i]));
                            }
                        }
                        token.ThrowIfCancellationRequested();
                        foreach (var name in hashSet)
                        {
                            var original = $"{contentsFolder}/picture/{name}";
                            if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                            {
                                Console.WriteLine($"file pciture '{name}' is not found.");
                            }
                        }
                        break;
                    case 6:
                        foreach (ref var file in project.Files.AsSpan())
                        {
                            for (uint i = 0, count = file.image_fileSet.Count; i != count; ++i)
                            {
                                hashSet.Add(new(file.image_fileSet[i]));
                            }
                        }
                        token.ThrowIfCancellationRequested();
                        foreach (var name in hashSet)
                        {
                            var original = $"{contentsFolder}/image/{name}";
                            if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                            {
                                Console.WriteLine($"file image '{name}' is not found.");
                            }
                        }
                        break;
                    case 7:
                        foreach (ref var file in project.Files.AsSpan())
                        {
                            for (uint i = 0, count = file.flagSet.Count; i != count; ++i)
                            {
                                hashSet.Add(new(file.flagSet[i]));
                            }
                        }
                        token.ThrowIfCancellationRequested();
                        foreach (var name in hashSet)
                        {
                            var original = $"{contentsFolder}/flag/{name}";
                            if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                            {
                                Console.WriteLine($"file flag '{name}' is not found.");
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
                return ValueTask.CompletedTask;
            }).ConfigureAwait(false);
        }
        catch(TaskCanceledException)
        {
            Console.WriteLine("Cancel Requested...");
        }
        finally
        {
            project.Dispose();
            stopwatch?.Stop();
            if (stopwatch is not null)
            {
                var milliseconds = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"{milliseconds} milli seconds passed.");
            }
        }

        return 0;
    }

    private bool CheckSync(Project project)
    {
        var success = project.CheckExistance();
        if (!success)
        {
            foreach (var error in project.ErrorList)
            {
                Console.WriteLine(error.Text);
            }
        }

        return success;
    }

    private static bool CompileResultSync(Project project)
    {
        bool isSuccess = true;
        foreach (ref var file in project.Files.AsSpan())
        {
            isSuccess &= file.NoError();
        }
        if (isSuccess)
        {
            project.AddReferenceAndValidate();
        }

        StringBuilder? stringBuilder = null;
        bool showNotError = project.RequiredSeverity != 0U;
        foreach (ref var result in project.Files)
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
        stringBuilder.Append(error.Position.Line + 1);
        stringBuilder.Append(", ");
        stringBuilder.Append(error.Position.Offset + 1);
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
        PerResultValidator.AddReferenceAndValidate(ref context, ref result);
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
