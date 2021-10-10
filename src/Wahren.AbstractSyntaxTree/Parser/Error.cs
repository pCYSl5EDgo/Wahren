namespace Wahren.AbstractSyntaxTree.Parser;

public record class Error(string Text, uint Line, uint Offset, uint Length, DiagnosticSeverity Severity = DiagnosticSeverity.Error)
{
}
