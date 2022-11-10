using System.Collections.Generic;
using Wahren.ArrayPoolCollections;
using Wahren.Map;

namespace Wahren.Compiler;

public partial class Program
{
    public static async ValueTask<int> AnalyzeAsync(
        string rootFolder = ".",
        bool @switch = false,
        PseudoDiagnosticSeverity severity = PseudoDiagnosticSeverity.Error,
        bool time = false
    )
    {
        var stopwatch = time ? Stopwatch.StartNew() : null;
        using var cancellationTokenSource = PrepareCancellationTokenSource(Timeout.InfiniteTimeSpan);
        var token = cancellationTokenSource.Token;
        try
        {
            var (pathSet, isUnicode, isEnglish) = await GetInitialSettingsAsync(rootFolder, token).ConfigureAwait(false);
            if (pathSet is null)
            {
                return 1;
            }

            using var project = new Project()
            {
                RequiredSeverity = (DiagnosticSeverity)(uint)severity,
                IsUnicode = isUnicode,
                IsEnglish = isEnglish,
                IsSwitch = @switch,
            };

            var files = pathSet.GetScriptDatArray();
            project.Files.PrepareAddRange(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                project.Files.Add(new((uint)i));
                project.Files.Last.FilePath = Path.GetRelativePath(Environment.CurrentDirectory, files[i]);
                project.FileAnalysisList.Add(new());
            }

            await ParallelLoadAndParseAsync(project, token).ConfigureAwait(false);

            if (severity == PseudoDiagnosticSeverity.Info)
            {
                Console.WriteLine("parallel parse complete");
            }
            
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
            await ParallelCheckFileExistanceAsync(project, pathSet.Contents, token).ConfigureAwait(false);
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

    private static async ValueTask<(PathSet? pathSet, bool isUnicode, bool isEnglish)> GetInitialSettingsAsync(string rootFolder, CancellationToken token)
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
            return (null, false, false);
        }

        PathSet pathSet = new(contentsFolder);
        var (isUnicode, isEnglish) = await IsEnglish(pathSet.Script).ConfigureAwait(false);
        return (pathSet, isUnicode, isEnglish);
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

    private static Task ParallelLoadAndParseAsync(Project project, CancellationToken token)
    {
        return Parallel.ForEachAsync(System.Linq.Enumerable.Range(0, project.Files.Count), token, async (int index, CancellationToken token) =>
        {
            var path = project.Files[index].FilePath;
            Debug.Assert(path is not null);
            token.ThrowIfCancellationRequested();
            if (project.RequiredSeverity == DiagnosticSeverity.Hint)
            {
                Console.WriteLine($"Load Start. Index: {index}. Path: {path}");
            }
            DualList<char> source;
            using (var input = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous))
            {
                Debug.Assert(input is not null);
                source = await (project.IsUnicode
                 ? UnicodeHandler.LoadAsync(input, token)
                 : Cp932Handler.LoadAsync(input, token)).ConfigureAwait(false);
            }

            if (project.RequiredSeverity == DiagnosticSeverity.Hint)
            {
                Console.WriteLine($"Load Done Parse Start. Index: {index}. Path: {path}");
            }
            project.Files[index].Source = source;
            Parse(project.RequiredSeverity, project.IsSwitch, project.IsEnglish, ref project.Files[index], project.FileAnalysisList[index]);
            if (project.RequiredSeverity == DiagnosticSeverity.Hint)
            {
                Console.WriteLine($"Parse Done. Index: {index}. Path: {path}");
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
                case 0:
                    for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                    {
                        for (uint j = 0, count = project.FileAnalysisList[i].imagedataSet.Count; j != count; ++j)
                        {
                            hashSet.Add(new(project.FileAnalysisList[i].imagedataSet[j]));
                        }
                    }
                    return EnsureExistanceImageDataAsync(project, hashSet, contentsFolder, "imagedata.dat", "chip", token);
                case 1:
                    for (uint i = 0, end = (uint)project.Files.Count; i < end; ++i)
                    {
                        for (uint j = 0, count = project.FileAnalysisList[i].imagedata2Set.Count; j != count; ++j)
                        {
                            hashSet.Add(new(project.FileAnalysisList[i].imagedata2Set[j]));
                        }
                    }
                    hashSet.Remove("@@");
                    return EnsureExistanceImageDataAsync(project, hashSet, contentsFolder, "imagedata2.dat", "chip2", token);
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
        using Imagedata.Imagedata data = new();
        var imagedatPath = Path.Combine(contentsFolder, "image", datFileName);
        var readTask = ReadFileUtility.ReadAsync(imagedatPath, token);

        var chipFolderPath = Path.Combine(contentsFolder, chipFolderName);
        foreach (var item in Directory.EnumerateFiles(chipFolderPath, "*.png", SearchOption.TopDirectoryOnly))
        {
            hashSet.Remove(Path.GetFileNameWithoutExtension(item));
        }
        foreach (var item in Directory.EnumerateFiles(chipFolderPath, "*.bmp", SearchOption.TopDirectoryOnly))
        {
            hashSet.Remove(Path.GetFileNameWithoutExtension(item));
        }

        var (buffer, bufferSize) = await readTask.ConfigureAwait(false);
        if (!data.TryLoadDataFileBinary(buffer, bufferSize))
        {
            return;
        }

        for (uint i = 0, end = data.Set.Count; i != end; ++i)
        {
            var str = new string(data.Set[i]);
            hashSet.Remove(str);
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

    private static void Parse(DiagnosticSeverity severity, bool treatSlashPlusAsSingleLineComment, bool isEnglish, ref Result result, AnalysisResult analysisResult)
    {
        Context context = new(treatSlashPlusAsSingleLineComment, isEnglish, false, severity);
        result.Success = Parser.Parse(ref context, ref result);
        PerResultValidator.AddReferenceAndValidate(ref context, ref result, analysisResult);
        if (!result.Success)
        {
            return;
        }

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
