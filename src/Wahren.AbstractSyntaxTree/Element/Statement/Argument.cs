namespace Wahren.AbstractSyntaxTree.Element.Statement;

public struct Argument
{
    public uint TokenId;
    public int Number;
    public ReferenceKind ReferenceKind;
    public uint ReferenceId;
    public bool IsNumber;
    public bool HasReference;
}
