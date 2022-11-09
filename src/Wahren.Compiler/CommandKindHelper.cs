namespace Wahren.Compiler;

internal static class CommandKindHelper
{
    public static (CommandKind, int) DecideCommandKind(ReadOnlySpan<string> commandLineArgs) => commandLineArgs.IsEmpty ? (CommandKind.Analyze, 0) : DecideCommandKind(commandLineArgs[0]);

    private static (CommandKind, int) DecideCommandKind(ReadOnlySpan<char> commandLineArg) => commandLineArg.Trim() switch
    {
        "a" or "analyze" => (CommandKind.Analyze, 1),
        "v" or "version" or "-v" or "--version" => (CommandKind.Version, 1),
        "f" or "format" => (CommandKind.Format, 1),
        "l" or "license" or "-l" or "--license" => (CommandKind.License, 1),
        "h" or "help" or "-h" or "--help" => (CommandKind.Help, 1),
        "u" or "update" or "-u" or "--update" => (CommandKind.Update, 1),
        _ => (CommandKind.Analyze, 0),
    };
}
