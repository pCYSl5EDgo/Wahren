namespace Wahren.AbstractSyntaxTree.Parser;

public record class Error(string Text, Range Range, DiagnosticSeverity Severity = DiagnosticSeverity.Error, ErrorCode Code = ErrorCode.Syntax, [CallerFilePath] string InternalCSharpFilePath = "", [CallerLineNumber] int InternalCSharpLineNumber = 0)
{
}
