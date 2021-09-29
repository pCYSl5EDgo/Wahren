namespace Wahren.AbstractSyntaxTree.Parser;

public record struct AmbiguousNameReference(int ResultId, int NodeIndex, ReferenceKind Kind, uint TokenId)
{
}
