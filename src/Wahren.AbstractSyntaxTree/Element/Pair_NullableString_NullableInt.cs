namespace Wahren.AbstractSyntaxTree.Element;

public struct Pair_NullableString_NullableInt
{
    /// <summary>
    /// TokenId
    /// </summary>
    public uint Text;
    public int Number;
    public ReferenceKind ReferenceKind;
    public uint ReferenceId;
    public bool HasText;
    public bool HasNumber;
    public bool HasReference;
}
