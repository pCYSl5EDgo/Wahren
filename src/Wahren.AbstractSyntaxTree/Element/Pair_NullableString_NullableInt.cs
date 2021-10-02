using System.Runtime.InteropServices;

namespace Wahren.AbstractSyntaxTree.Element;

[StructLayout(LayoutKind.Explicit, Pack = 32, Size = 32)]
public struct Pair_NullableString_NullableInt
{
    /// <summary>
    /// TokenId
    /// </summary>
    [FieldOffset(0)]
    public uint Text;
    [FieldOffset(4)]
    public uint TrailingTokenCount;
    [FieldOffset(8)]
    public int Number;
    [FieldOffset(12)]
    public uint ReferenceId;
    [FieldOffset(16)]
    public ReferenceKind ReferenceKind;
    [FieldOffset(24)]
    public bool HasText;
    [FieldOffset(25)]
    public bool HasNumber;
    [FieldOffset(26)]
    public bool HasReference;

    public Pair_NullableString_NullableInt(uint tokenId) : this()
    {
        Text = tokenId;
        HasText = true;
    }
}
