namespace Wahren.Compiler;

public partial class Program
{
    [Command(new string[] {
        "interact",
    })]
    public async ValueTask<int> Interact(
        [Option(0, "input folder")] string rootFolder = ".",
        [Option(1, "enable color theme")] bool enableColor = false,
        [Option("color-success")] ConsoleColor colorSuccess = ConsoleColor.Blue,
        [Option("color-error")] ConsoleColor colorError = ConsoleColor.Red,
        [Option("color-normal0")] ConsoleColor colorNormal0 = ConsoleColor.Black,
        [Option("color-normal1")] ConsoleColor colorNormal1 = ConsoleColor.DarkBlue
    )
    {
        using var cancellationTokenSource = PrepareCancellationTokenSource(TimeSpan.FromDays(1.0));
        var token = cancellationTokenSource.Token;
        try
        {
            var (pathSet, isUnicode, isEnglish) = await GetInitialSettingsAsync(rootFolder, token).ConfigureAwait(false);
            if (pathSet is null)
            {
                return 1;
            }

            Project project = new()
            {
                RequiredSeverity = DiagnosticSeverity.Error,
                IsUnicode = isUnicode,
                IsEnglish = isEnglish,
                IsSwitch = false,
            };

            static async ValueTask<int> Reload(Project project, PathSet pathSet, CancellationToken token)
            {
                project.Dispose();
                var files = pathSet.GetScriptDatArray();
                for (int i = 0; i < files.Length; i++)
                {
                    project.Files.Add(new((uint)i));
                    project.Files.Last.FilePath = files[i];
                    project.FileAnalysisList.Add(new());
                }

                var loadAndParseTask = ParallelLoadAndParseAsync(project, token);
                await loadAndParseTask.ConfigureAwait(false);
                project.AddReferenceAndValidate();
                return 0;
            }

            string input = "";
            bool ReadLineOrBreak()
            {
                var _input = Console.ReadLine();
                if (_input is null || _input == "q")
                {
                    return false;
                }

                input = _input;
                return true;
            }

            int stage = 0;
            ConsoleColor prev = Console.ForegroundColor;
            ColorTheme? theme = enableColor ? new(colorSuccess, colorError, colorNormal0, colorNormal1) : null;
            do
            {
                switch (stage)
                {
                    case 0:
                        var reloadResult = await Reload(project, pathSet, token).ConfigureAwait(false);
                        if (reloadResult == 0)
                        {
                            if (theme is not null)
                            {
                                prev = Console.ForegroundColor;
                                Console.ForegroundColor = theme.Success;
                            }
                            Console.WriteLine(Stage0SuggestBreak);
                            Console.Write(Stage0Success);
                            if (theme is not null)
                            {
                                Console.ForegroundColor = prev;
                            }
                            stage++;
                        }
                        else
                        {
                            Console.WriteLine(Stage0Fail);
                            Console.WriteLine(Stage0SuggestBreak);
                        }
                        break;
                    case 1:
                        stage = ProcessStage1(project, input.AsSpan().Trim(), theme);
                        if (stage == 0)
                        {
                            goto case 0;
                        }
                        break;
                }
            } while (ReadLineOrBreak());
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Cancel Requested...");
        }
        return 0;
    }

    private static int ProcessStage1(Project project, ReadOnlySpan<char> readOnlySpan, ColorTheme? theme)
    {
        if (readOnlySpan.SequenceEqual("reload"))
        {
            return 0;
        }
        else if (readOnlySpan.SequenceEqual("help"))
        {
            Console.WriteLine(Stage1Help);
        }
        else if (readOnlySpan.StartsWith(CommandList))
        {
            ProcessCommandListStructure(project, readOnlySpan.Slice(CommandList.Length).TrimStart(), theme);
        }
        else if (readOnlySpan.StartsWith(CommandReferences))
        {
            ProcessCommandReferences(project, readOnlySpan.Slice(CommandReferences.Length).TrimStart(), theme);
        }

        if (theme is null)
        {
            Console.Write(Stage1Success);
        }
        else
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = theme.Success;
            Console.Write(Stage1Success);
            Console.ForegroundColor = prev;
        }

        return 1;
    }

    private const string CommandList = "list ";
    private const string CommandReferences = "ref ";

    private const string Stage1InvalidKind =
#if JAPANESE
        "種類名が正しくありません。\n適切な変数の種類 : " + StructureKinds;
#else
        "This is a invalid kind.\nAppropriate kinds: " + StructureKinds;
#endif

    private const string Stage1Help =
#if JAPANESE
        "コマンド\n" +
#else
        "Command\n" +
#endif
        $@"  {CommandList}[kind]
    kind : {StructureKinds}

  {CommandReferences}[kind] [name]
    kind : {StructureKinds}

  reload

  q";

    private const string Stage1Success =
#if JAPANESE
        "何を知りたいのですか？コマンドの使用法を知りたい場合はhelpと記述してください。\n> ";
#else
        "What do you want to know?\n> ";
#endif
    private const string Stage0Success =
#if JAPANESE
        "スクリプトのロードに成功しました。\n何を知りたいのですか？コマンドの使用法を知りたい場合はhelpと記述してください。\n> ";
#else
        "Script load success!\nWhat do you want to know?\n> ";
#endif
    private const string Stage0Fail =
#if JAPANESE
        "\nスクリプトの再解析を行う場合はEnterキーを押してください。";
#else
        "\nPress Enter when you want to reanalyze the scripts.";
#endif
    private const string Stage0SuggestBreak =
#if JAPANESE
        "プログラムを終了したい場合は'q'と入力してEnterキーを押してください。";
#else
        "Press 'q' when you want to exit.";
#endif
}

internal class ColorTheme
{
    public ConsoleColor Success;
    public ConsoleColor Error;
    public ConsoleColor Normal0;
    public ConsoleColor Normal1;

    public ColorTheme(ConsoleColor success, ConsoleColor error, ConsoleColor normal0, ConsoleColor normal1)
    {
        Success = success;
        Error = error;
        Normal0 = normal0;
        Normal1 = normal1;
    }
}

internal class PathSet
{
    public readonly string Contents;
    public readonly string Script;
    public readonly string Chip;
    public readonly string Chip2;
    public readonly string Picture;
    public readonly string Image;
    public readonly string Face;
    public readonly string Image_dat;
    public readonly string Image2_dat;
    public readonly string Imagedata_dat;
    public readonly string Imagedata2_dat;

    public PathSet(string contents)
    {
        Contents = contents;
        Script = Path.Combine(Contents, "script");
        Chip = Path.Combine(Contents, "chip");
        Chip2 = Path.Combine(Contents, "chip2");
        Image = Path.Combine(Contents, "image");
        Image_dat = Path.Combine(Image, "image.dat");
        Image2_dat = Path.Combine(Image, "image2.dat");
        Imagedata_dat = Path.Combine(Image, "imagedata.dat");
        Imagedata2_dat = Path.Combine(Image, "imagedata2.dat");
        Picture = Path.Combine(Contents, "picture");
        Face = Path.Combine(Contents, "face");
    }

    public string[] GetScriptDatArray()
    {
        return Directory.GetFiles(Script, "*.dat", SearchOption.AllDirectories);
    }

    public TripleImageSet GetImages()
    {
        return new(Directory.GetFiles(Image, "*.png"), Directory.GetFiles(Image, "*.bmp"), Directory.GetFiles(Image, "*.jpg"));
    }

    public record struct TripleImageSet(string[] Png, string[] Bmp, string[] Jpg)
    {
        public Enumerator GetEnumerator() => new(ref this);

        public struct Enumerator
        {
            private readonly TripleImageSet set;

            private string[] array;
            private int index;

            public Enumerator(ref TripleImageSet set)
            {
                this.set = set;
                array = set.Png;
                index = -1;
            }

            public bool MoveNext()
            {
                if (++index >= array.Length)
                {
                    if (array == set.Jpg)
                    {
                        return false;
                    }
                    else if (array == set.Bmp)
                    {
                        array = set.Jpg;
                    }
                    else
                    {
                        array = set.Bmp;
                    }

                    index = 0;
                }

                return true;
            }

            public ReadOnlySpan<char> Current => Path.GetFileNameWithoutExtension(array[index].AsSpan());
        }
    }
}
