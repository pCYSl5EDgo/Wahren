namespace Wahren.AbstractSyntaxTree.Parser;

public record class Error(string Text, Position Position, uint Length, DiagnosticSeverity Severity = DiagnosticSeverity.Error)
{
}

public record class ProjectError(string Text)
{
}
