using System.Runtime.InteropServices;

namespace Wahren.AbstractSyntaxTree.Statement;

[StructLayout(LayoutKind.Explicit, Pack = 32, Size = 32)]
public struct Argument : ITokenIdModifiable
{
    [FieldOffset(0)]
    public uint TokenId;
    [FieldOffset(4)]
    public uint TrailingTokenCount;
    [FieldOffset(8)]
    public int Number;
    [FieldOffset(12)]
    public uint ReferenceId;
    [FieldOffset(16)]
    public ReferenceKind ReferenceKind;
    [FieldOffset(24)]
    public bool IsNumber;
    [FieldOffset(25)]
    public bool HasReference;
    [FieldOffset(26)]
    private bool _0;
    [FieldOffset(27)]
    private bool _1;
    [FieldOffset(28)]
    private bool _2;
    [FieldOffset(29)]
    private bool _3;
    [FieldOffset(30)]
    private bool _4;
    [FieldOffset(31)]
    private bool _5;

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }
    }
}
