using System.Runtime.InteropServices;

namespace Wahren.AbstractSyntaxTree.Element;

[StructLayout(LayoutKind.Explicit, Pack = 32, Size = 32)]
public struct Pair_NullableString_NullableInt : ITokenIdModifiable
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
    public uint NumberTokenId;
    [FieldOffset(28)]
    public bool HasText;
    [FieldOffset(29)]
    public bool HasNumber;
    [FieldOffset(30)]
    public bool HasReference;
    [FieldOffset(31)]
    public bool HasNumberTokenId;

    public Pair_NullableString_NullableInt(uint tokenId) : this()
    {
        Text = tokenId;
        HasText = true;
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (Text >= indexEqualToOrGreaterThan)
        {
            Text += count;
        }

        if (HasNumberTokenId && NumberTokenId >= indexEqualToOrGreaterThan)
        {
            NumberTokenId += count;
        }
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (Text >= indexEqualToOrGreaterThan)
        {
            Text -= count;
        }

        if (HasNumberTokenId && NumberTokenId >= indexEqualToOrGreaterThan)
        {
            NumberTokenId -= count;
        }
    }
}
