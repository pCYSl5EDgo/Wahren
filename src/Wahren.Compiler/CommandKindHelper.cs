namespace Wahren.Compiler;

internal static class CommandKindHelper
{
    public static (CommandKind, int) DecideCommandKind(ReadOnlySpan<string> commandLineArgs) => commandLineArgs.Length >= 1 ? (DecideCommandKind(commandLineArgs[0]), 1) : (CommandKind.Analyze, 0);

    private static CommandKind DecideCommandKind(ReadOnlySpan<char> commandLineArg) => commandLineArg.Trim() switch
    {
        "a" or "analyze" => CommandKind.Analyze,
        "v" or "version" => CommandKind.Version,
        "f" or "format" => CommandKind.Format,
        "l" or "license" => CommandKind.License,
        "h" or "help" => CommandKind.Help,
        "u" or "update" => CommandKind.Help,
        _ => throw new ArgumentOutOfRangeException(nameof(commandLineArg)),
    };
}
