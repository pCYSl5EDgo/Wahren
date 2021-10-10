namespace Wahren.AbstractSyntaxTree.Parser;

public struct Context
{
    public Position Position;

    public bool TreatSlashPlusAsSingleLineComment;
    public bool IsEnglishMode;
    public bool DeleteDiscardedToken;

    public DiagnosticSeverity RequiredSeverity;

    public Context(bool treatSlashPlusAsSingleLineComment = false, bool isEnglishMode = false, bool deleteDiscardedToken = false, DiagnosticSeverity requiredSeverity = DiagnosticSeverity.Warning)
    {
        Position = default;

        TreatSlashPlusAsSingleLineComment = treatSlashPlusAsSingleLineComment;
        IsEnglishMode = isEnglishMode;
        RequiredSeverity = requiredSeverity;
        DeleteDiscardedToken = deleteDiscardedToken;
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
