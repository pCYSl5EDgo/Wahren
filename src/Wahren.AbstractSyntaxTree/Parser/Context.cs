namespace Wahren.AbstractSyntaxTree.Parser;

public struct Context
{
    public Position Position;

    public bool TreatSlashPlusAsSingleLineComment;
    public bool IsEnglishMode;

    public DiagnosticSeverity RequiredSeverity;

    public Context(bool treatSlashPlusAsSingleLineComment = false, bool isEnglishMode = false, DiagnosticSeverity requiredSeverity = DiagnosticSeverity.Warning)
    {
        Position = default;

        TreatSlashPlusAsSingleLineComment = treatSlashPlusAsSingleLineComment;
        IsEnglishMode = isEnglishMode;
        RequiredSeverity = requiredSeverity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CreateError(DiagnosticSeverity severity)
    {
        return (uint)severity <= (uint)RequiredSeverity;
    }

    public void ResetToProcessAnotherFile()
    {
        Position = default;
    }
}
