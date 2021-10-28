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
            var (success, contentsFolder, scriptFolder, _, isUnicode, isEnglish) = await GetInitialSettingsAsync(rootFolder, token).ConfigureAwait(false);
            if (!success)
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

            static async ValueTask<int> Reload(Project project, string[] files, bool isUnicode, bool isEnglish, CancellationToken token)
            {
                project.Dispose();
                for (int i = 0; i < files.Length; i++)
                {
                    project.Files.Add(new((uint)i));
                    project.Files.Last.FilePath = files[i];
                    project.FileAnalysisList.Add(new());
                }

                await ParallelLoadAndParseAsync(project, token).ConfigureAwait(false);
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
                        var files = Directory.GetFiles(scriptFolder, "*.dat", SearchOption.AllDirectories);
                        var reloadResult = await Reload(project, files, isUnicode, isEnglish, token).ConfigureAwait(false);
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
