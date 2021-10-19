global using System;
global using System.Buffers;
global using System.Diagnostics;
global using System.IO;
global using System.Runtime.InteropServices;
global using System.Text;
global using System.Threading;
global using Wahren.AbstractSyntaxTree.Parser;
global using Wahren.AbstractSyntaxTree.Project;
global using Wahren.FileLoader;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Wahren.Map;

namespace Wahren.Compiler;

public partial class Program
{
    [Command(new string[] {
        "analyze",
    })]
#pragma warning disable CA1822
    public async ValueTask<int> Analyze(
#pragma warning restore CA1822
        [Option(0, "input folder")] string rootFolder = ".",
        [Option("switch")] bool @switch = false,
        [Option("s")] PseudoDiagnosticSeverity severity = PseudoDiagnosticSeverity.Error,
        [Option("t")] bool time = false
    )
    {
        var stopwatch = time ? Stopwatch.StartNew() : null;
        using var cancellationTokenSource = PrepareCancellationTokenSource(TimeSpan.FromMinutes(1));
        var token = cancellationTokenSource.Token;
        try
        {
            var (success, contentsFolder, _, files, isUnicode, isEnglish) = await GetInitialSettingsAsync(rootFolder, token).ConfigureAwait(false);
            if (!success)
            {
                return 1;
            }

            using var project = new Project()
            {
                RequiredSeverity = (DiagnosticSeverity)(uint)severity
            };

            project.Files.PrepareAddRange(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                project.Files.Add(new((uint)i));
                project.Files.Last.FilePath = Path.GetRelativePath(Environment.CurrentDirectory, files[i]);
                project.FileAnalysisList.Add(new());
            }

            await ParallelLoadAndParseAsync(project, @switch, isUnicode, isEnglish, token).ConfigureAwait(false);

            token.ThrowIfCancellationRequested();
            if (!CompileResultSync(project))
            {
                return 1;
            }

            token.ThrowIfCancellationRequested();
            if (!CheckSync(project))
            {
                return 1;
            }

            token.ThrowIfCancellationRequested();
            await ParallelCheckFileExistanceAsync(project, contentsFolder, token).ConfigureAwait(false);
            foreach (var error in project.ErrorBag)
            {
                Console.WriteLine(error.Text);
            }
            project.ErrorBag.Clear();
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Cancel Requested...");
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

    private static async ValueTask<(bool success, string contentsFolder, string scriptFolder, string[] scriptFiles, bool isUnicode, bool isEnglish)> GetInitialSettingsAsync(string rootFolder, CancellationToken token)
    {
        var getDebugPaperTask = GetDebugPaper(rootFolder, token);
        string? contentsFolder = DetectContentsFolder(rootFolder); ;
        var debugPaper = await getDebugPaperTask.ConfigureAwait(false);
        if (debugPaper.Folder is not null)
        {
            var debugFolder = Path.Combine(rootFolder, debugPaper.Folder);
            contentsFolder ??= debugFolder;
        }

        if (contentsFolder is null || !Directory.Exists(contentsFolder))
        {
#if JAPANESE
            Console.Error.WriteLine("コンテンツが含まれるフォルダが見つかりませんでした。'script'/'image'/'stage'などが含まれるフォルダをa_deaultという名前で設置してみてください。");
#else
            Console.Error.WriteLine("Contents folder is not found.\n\nContents folder contains 'script'/'image'/'stage' folders.");
#endif
            return (false, "", "", Array.Empty<string>(), false, false);
        }

        var scriptFolderPath = Path.Combine(contentsFolder, "script");
        var isEnglishTask = IsEnglish(scriptFolderPath);
        var files = Directory.GetFiles(scriptFolderPath, "*.dat", SearchOption.AllDirectories);
        var (isUnicode, isEnglish) = await isEnglishTask.ConfigureAwait(false);
        return (true, contentsFolder, scriptFolderPath, files, isUnicode, isEnglish);
    }

    private static CancellationTokenSource PrepareCancellationTokenSource(TimeSpan timeSpan)
    {
        CancellationTokenSource cancellationTokenSource = new(timeSpan);
        Console.CancelKeyPress += new((object? _, ConsoleCancelEventArgs args) =>
        {
            cancellationTokenSource?.Cancel();
            args.Cancel = true;
        });
        return cancellationTokenSource;
    }

    private static async ValueTask<MapInfo[]> ParallelMapLoadAsync(string stageFolderPath, CancellationToken token)
    {
        var mapPaths = Directory.GetFiles(stageFolderPath, "*.map", SearchOption.TopDirectoryOnly);
        if (mapPaths.Length == 0)
        {
            return Array.Empty<MapInfo>();
        }

        var answer = new MapInfo[mapPaths.Length];
        for (nint i = 0; i < answer.Length; i++)
        {
            answer[i] = new(mapPaths[i]);
        }

        await Parallel.ForEachAsync(System.Linq.Enumerable.Range(0, answer.Length), token, async (int index, CancellationToken token) =>
        {
            var path = answer[index].Path;
            Debug.Assert(path is not null);
            var (rental, actual) = await ReadFileUtility.ReadAsync(path, token).ConfigureAwait(false);
            try
            {
                if (actual == 0)
                {
                    return;
                }
                var success = MapFileInfoLoader.TryParse(rental.AsSpan(0, actual), ref answer[index]);
                if (success)
                {
                    ;
                }
                else
                {
                    ;
                }
            }
            finally
            {
                if (rental.Length != 0)
                {
                    ArrayPool<byte>.Shared.Return(rental);
                }
            }
        }).ConfigureAwait(false);
        return answer;
    }

    private static Task ParallelLoadAndParseAsync(Project project, bool @switch, bool isUnicode, bool isEnglish, CancellationToken token)
    {
        return Parallel.ForEachAsync(System.Linq.Enumerable.Range(0, project.Files.Count), token, async (int index, CancellationToken token) =>
        {
            var path = project.Files[index].FilePath;
            Debug.Assert(path is not null);
            var (rental, actual) = await ReadFileUtility.ReadAsync(path, token).ConfigureAwait(false);
            try
            {
                if (actual == 0)
                {
                    return;
                }
                token.ThrowIfCancellationRequested();
                LoadAndParse(project.RequiredSeverity, @switch, isUnicode, isEnglish, ref project.Files[index], project.FileAnalysisList[index], rental.AsSpan(0, actual));
            }
            finally
            {
                if (rental.Length != 0)
                {
                    ArrayPool<byte>.Shared.Return(rental);
                }
            }
        });
    }

    private static Task ParallelCheckFileExistanceAsync(Project project, string contentsFolder, CancellationToken token)
    {
        return Parallel.ForEachAsync(System.Linq.Enumerable.Range(0, 8), token, (int index, CancellationToken token) =>
        {
            HashSet<string> hashSet = new();
            switch (index)
            {
                /*
                NOTIMPLEMENTED YET
                case 0:
                    for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                    {
                        for (uint i = 0, count = file.imagedataSet.Count; i != count; ++i)
                        {
                            hashSet.Add(new(file.imagedataSet[i]));
                        }
                    }
                    return EnsureExistanceImageDataAsync(project, hashSet, contentsFolder, "imagedata.dat", "chip", token);
                case 1:
                    for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                    {
                        for (uint i = 0, count = file.imagedata2Set.Count; i != count; ++i)
                        {
                            hashSet.Add(new(file.imagedata2Set[i]));
                        }
                    }
                    hashSet.Remove("@@");
                    return EnsureExistanceImageDataAsync(project, hashSet, contentsFolder, "imagedata2.dat", "chip2", token);
                */
                case 2:
                    foreach (var analysisResult in project.FileAnalysisList.AsSpan())
                    {
                        for (uint i = 0, count = analysisResult.faceSet.Count; i != count; ++i)
                        {
                            hashSet.Add(new(analysisResult.faceSet[i]));
                        }
                    }
                    token.ThrowIfCancellationRequested();
                    foreach (var name in hashSet)
                    {
                        var original = $"{contentsFolder}/face/{name}";
                        if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                        {
                            for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                            {
                                if (!project.FileAnalysisList[i].faceSet.TryGet(name.AsSpan(), out var setId))
                                {
                                    continue;
                                }
                                project.ErrorAdd_FileNotFound("face", name, ref project.Files[i], project.FileAnalysisList[i].faceSet.References[setId].AsSpan());
                            }
                        }
                    }
                    break;
                case 3:
                    foreach (var analysisResult in project.FileAnalysisList.AsSpan())
                    {
                        for (uint i = 0, count = analysisResult.iconSet.Count; i != count; ++i)
                        {
                            hashSet.Add(new(analysisResult.iconSet[i]));
                        }
                    }
                    token.ThrowIfCancellationRequested();
                    foreach (var name in hashSet)
                    {
                        var original = $"{contentsFolder}/icon/{name}";
                        if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                        {
                            for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                            {
                                if (!project.FileAnalysisList[i].iconSet.TryGet(name.AsSpan(), out var setId))
                                {
                                    continue;
                                }
                                project.ErrorAdd_FileNotFound("icon", name, ref project.Files[i], project.FileAnalysisList[i].iconSet.References[setId].AsSpan());
                            }
                        }
                    }
                    break;
                case 4:
                    foreach (var analysisResult in project.FileAnalysisList.AsSpan())
                    {
                        for (uint i = 0, count = analysisResult.soundSet.Count; i != count; ++i)
                        {
                            hashSet.Add(new(analysisResult.soundSet[i]));
                        }
                    }
                    token.ThrowIfCancellationRequested();
                    foreach (var name in hashSet)
                    {
                        var original = $"{contentsFolder}/sound/{name}.wav";
                        if (!File.Exists(original))
                        {
                            for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                            {
                                if (!project.FileAnalysisList[i].soundSet.TryGet(name.AsSpan(), out var setId))
                                {
                                    continue;
                                }
                                project.ErrorAdd_FileNotFound("sound", name, ref project.Files[i], project.FileAnalysisList[i].soundSet.References[setId].AsSpan());
                            }
                        }
                    }
                    break;
                case 5:
                    foreach (var analysisResult in project.FileAnalysisList.AsSpan())
                    {
                        for (uint i = 0, count = analysisResult.pictureSet.Count; i != count; ++i)
                        {
                            hashSet.Add(new(analysisResult.pictureSet[i]));
                        }
                    }
                    token.ThrowIfCancellationRequested();
                    foreach (var name in hashSet)
                    {
                        var original = $"{contentsFolder}/picture/{name}";
                        if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                        {
                            for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                            {
                                if (!project.FileAnalysisList[i].pictureSet.TryGet(name.AsSpan(), out var setId))
                                {
                                    continue;
                                }
                                project.ErrorAdd_FileNotFound("picture", name, ref project.Files[i], project.FileAnalysisList[i].pictureSet.References[setId].AsSpan());
                            }
                        }
                    }
                    break;
                case 6:
                    foreach (var analysisResult in project.FileAnalysisList.AsSpan())
                    {
                        for (uint i = 0, count = analysisResult.imageSet.Count; i != count; ++i)
                        {
                            hashSet.Add(new(analysisResult.imageSet[i]));
                        }
                    }
                    token.ThrowIfCancellationRequested();
                    foreach (var name in hashSet)
                    {
                        var original = $"{contentsFolder}/image/{name}";
                        if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                        {
                            for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                            {
                                if (!project.FileAnalysisList[i].imageSet.TryGet(name.AsSpan(), out var setId))
                                {
                                    continue;
                                }
                                project.ErrorAdd_FileNotFound("image", name, ref project.Files[i], project.FileAnalysisList[i].imageSet.References[setId].AsSpan());
                            }
                        }
                    }
                    break;
                case 7:
                    foreach (var analysisResult in project.FileAnalysisList.AsSpan())
                    {
                        for (uint i = 0, count = analysisResult.flagSet.Count; i != count; ++i)
                        {
                            hashSet.Add(new(analysisResult.flagSet[i]));
                        }
                    }
                    token.ThrowIfCancellationRequested();
                    foreach (var name in hashSet)
                    {
                        var original = $"{contentsFolder}/flag/{name}";
                        if (!File.Exists(original) && !File.Exists(original + ".png") && !File.Exists(original + ".bmp") && !File.Exists(original + ".jpg"))
                        {
                            for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                            {
                                if (!project.FileAnalysisList[i].flagSet.TryGet(name.AsSpan(), out var setId))
                                {
                                    continue;
                                }
                                project.ErrorAdd_FileNotFound("flag", name, ref project.Files[i], project.FileAnalysisList[i].flagSet.References[setId].AsSpan());
                            }
                        }
                    }
                    break;
                case 8:
                    foreach (var analysisResult in project.FileAnalysisList.AsSpan())
                    {
                        for (uint i = 0, count = analysisResult.mapSet.Count; i != count; ++i)
                        {
                            hashSet.Add(new(analysisResult.mapSet[i]));
                        }
                    }
                    token.ThrowIfCancellationRequested();
                    foreach (var name in hashSet)
                    {
                        var original = $"{contentsFolder}/stage/{name}.map";
                        if (!File.Exists(original))
                        {
                            for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                            {
                                if (!project.FileAnalysisList[i].mapSet.TryGet(name.AsSpan(), out var setId))
                                {
                                    continue;
                                }
                                project.ErrorAdd_FileNotFound("stage", name, ref project.Files[i], project.FileAnalysisList[i].mapSet.References[setId].AsSpan());
                            }
                        }
                    }
                    break;
                case 9:
                    foreach (var analysisResult in project.FileAnalysisList.AsSpan())
                    {
                        for (uint i = 0, count = analysisResult.bgmSet.Count; i != count; ++i)
                        {
                            hashSet.Add(new(analysisResult.bgmSet[i]));
                        }
                    }
                    token.ThrowIfCancellationRequested();
                    foreach (var name in hashSet)
                    {
                        var original = $"{contentsFolder}/bgm/{name}";
                        if (!File.Exists(original) && !File.Exists(original + ".mp3") && !File.Exists(original + ".ogg") && !File.Exists(original + ".mid"))
                        {
                            for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                            {
                                if (!project.FileAnalysisList[i].bgmSet.TryGet(name.AsSpan(), out var setId))
                                {
                                    continue;
                                }
                                project.ErrorAdd_FileNotFound("bgm", name, ref project.Files[i], project.FileAnalysisList[i].bgmSet.References[setId].AsSpan());
                            }
                        }
                    }
                    break;
            }
            return ValueTask.CompletedTask;
        });
    }

    private static async ValueTask EnsureExistanceImageDataAsync(Project project, HashSet<string> hashSet, string contentsFolder, string datFileName, string chipFolderName, CancellationToken token)
    {
        var imagedatPath = Path.Combine(contentsFolder, "image", datFileName);
        using var handle = File.OpenHandle(imagedatPath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous);
        var length = RandomAccess.GetLength(handle);
        if (length == 0 && hashSet.Count != 0)
        {
#if JAPANESE
            project.ErrorBag.Add(new($"{datFileName}が空です。"));
#else
            project.ErrorBag.Add(new($"{datFileName} is empty."));
#endif
            return;
        }
        if (length > int.MaxValue)
        {
            project.ErrorAdd_InsufficientMemory(imagedatPath, length);
            return;
        }
        var buffer = ArrayPool<byte>.Shared.Rent((int)length);
        try
        {
            ValueTask<int> readingTask = RandomAccess.ReadAsync(handle, buffer.AsMemory(0, (int)length), 0, token);
            var chipFolderPath = Path.Combine(contentsFolder, chipFolderName);
            foreach (var item in Directory.EnumerateFiles(chipFolderPath, "*.png", SearchOption.TopDirectoryOnly))
            {
                hashSet.Remove(Path.GetFileNameWithoutExtension(item));
            }
            foreach (var item in Directory.EnumerateFiles(chipFolderPath, "*.bmp", SearchOption.TopDirectoryOnly))
            {
                hashSet.Remove(Path.GetFileNameWithoutExtension(item));
            }
            var actual = await readingTask.ConfigureAwait(false);
            EnsureExistanceImageDataSync(project, hashSet, buffer, actual, datFileName);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
        foreach (var name in hashSet)
        {
            static void Process(Project project, string chipFolderName, string name)
            {
                for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                {
                    ref var file = ref project.Files[i];
                    var analysisResult = project.FileAnalysisList[i];
                    ref var set = ref (chipFolderName == "chip" ? ref analysisResult.imagedataSet : ref analysisResult.imagedata2Set);
                    if (!set.TryGet(name.AsSpan(), out var setId))
                    {
                        continue;
                    }
                    project.ErrorAdd_FileNotFound(chipFolderName, name, ref file, set.References[setId].AsSpan());
                }
            }

            Process(project, chipFolderName, name);
        }
    }

    private static void EnsureExistanceImageDataSync(Project project, HashSet<string> hashSet, byte[] buffer, int length, string datFileName)
    {
        int offset = length < 12 ? length : 12;
        do
        {
            var index = buffer.AsSpan(offset).IndexOf<byte>(0x00);
            if (index <= 0)
            {
#if JAPANESE
                project.ErrorBag.Add(new($"不正なデータフォーマットをしています。\n  {datFileName}"));
#else
                project.ErrorBag.Add(new($"Invalid data format error.\n  {datFileName}"));
#endif
                return;
            }

            if (index == 8 && Unsafe.As<byte, ulong>(ref buffer[offset]) == 0x5f5f5f5f5f5f5f5fUL)
            {
                break;
            }

            var imageName = string.Create(index, (buffer, offset), static (Span<char> destination, (byte[] buffer, int offset) pair) =>
            {
                var span = pair.buffer.AsSpan(pair.offset, destination.Length);
                for (int i = 0; i < destination.Length; i++)
                {
                    destination[i] = (char)span[i];
                }
            });
            hashSet.Remove(imageName);
            offset += index + 17;
        } while (true);
    }

    private static bool CheckSync(Project project)
    {
        var success = project.CheckExistance();
        if (!success)
        {
            foreach (var error in project.ErrorBag)
            {
                Console.WriteLine(error.Text);
            }
            project.ErrorBag.Clear();
        }

        return success;
    }

    private static bool CompileResultSync(Project project)
    {
        bool isSuccess = true;
        var files = project.Files.AsSpan();
        foreach (ref var file in files)
        {
            isSuccess &= file.NoError();
        }
        if (isSuccess)
        {
            project.AddReferenceAndValidate();
            foreach (ref var file in files)
            {
                isSuccess &= file.NoError();
            }
            if (isSuccess)
            {
                isSuccess &= project.DetectInfiniteLoop();
            }
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
        stringBuilder.Append("  ");
        stringBuilder.Append(filePath);
        stringBuilder.Append('(');
        stringBuilder.Append(error.Line + 1);
        stringBuilder.Append(", ");
        stringBuilder.Append(error.Offset + 1);
        stringBuilder.Append(')');
        if (showNotError)
        {
            stringBuilder.Append(", Severity: ");
            stringBuilder.Append(error.Severity);
        }

        stringBuilder.AppendLine();
    }

    private static void LoadAndParse(DiagnosticSeverity severity, bool treatSlashPlusAsSingleLineComment, bool isUnicode, bool isEnglish, ref Result result, AnalysisResult analysisResult, Span<byte> input)
    {
        if (isUnicode)
        {
            UnicodeHandler.Load(input, out result.Source);
        }
        else
        {
            Cp932Handler.Load(input, out result.Source);
        }

        Context context = new(treatSlashPlusAsSingleLineComment, isEnglish, false, severity);
        result.Success = Parser.Parse(ref context, ref result);
        PerResultValidator.AddReferenceAndValidate(ref context, ref result, analysisResult);
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
