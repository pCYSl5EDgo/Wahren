namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;
using Wahren;
using Wahren.AbstractSyntaxTree;

public struct ArgumentInfo
{
    public ReferenceKind Kind;

    public ArgumentInfo(ReferenceKind kind)
    {
        Kind = kind;
    }
}
