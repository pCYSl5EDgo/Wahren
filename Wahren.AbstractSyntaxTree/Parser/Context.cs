namespace Wahren.AbstractSyntaxTree.Parser;

public struct Context
{
    public Position Position;

    public bool TreatSlashPlusAsSingleLineComment;
    public bool IsEnglishMode;

    public nuint Id;
    public DiagnosticSeverity RequiredSeverity;

    public Context(nuint id, bool treatSlashPlusAsSingleLineComment = false, bool isEnglishMode = false, DiagnosticSeverity requiredSeverity = DiagnosticSeverity.Warning)
    {
        Id = id;
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
