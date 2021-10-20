namespace Wahren.AbstractSyntaxTree.Statement;

public sealed class CallEndingRollActionStatement : IStatement
{
    public CallEndingRollActionStatement(uint tokenId, uint text0, uint text1, byte font0, byte font1, EndingRollDisplayMethod displyMethod, byte marginFontType, byte marginFontCount)
    {
        TokenId = tokenId;
        Text0 = text0;
        Text1 = text1;
        Font0 = font0;
        Font1 = font1;
        DisplyMethod = displyMethod;
        MarginFontType = marginFontType;
        MarginFontCount = marginFontCount;
    }

    public uint TokenId { get; set; }
    public uint Text0 { get; set; }
    public uint Text1 { get; set; }
    public byte Font0 { get; set; }
    public byte Font1 { get; set; }
    public EndingRollDisplayMethod DisplyMethod { get; set; }
    public byte MarginFontType { get; set; }
    public byte MarginFontCount { get; set; }

    public string DisplayName => "ending";

    public uint TextCount => 2U - (Text1 == uint.MaxValue ? 1U : 0U);

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }

        if (Text0 >= indexEqualToOrGreaterThan)
        {
            Text0 += count;
        }

        if (Text1 >= indexEqualToOrGreaterThan)
        {
            Text1 += count;
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }

        if (Text0 >= indexEqualToOrGreaterThan)
        {
            Text0 -= count;
        }

        if (Text1 >= indexEqualToOrGreaterThan)
        {
            Text1 -= count;
        }
    }

    public uint this[uint index]
    {
        get
        {
            if (index == 0)
            {
                return Text0;
            }

            if (index > 1)
            {
                throw new IndexOutOfRangeException();
            }

            return Text1;
        }
    }

    public void Dispose()
    {
    }
}

[Flags]
public enum EndingRollDisplayMethod : byte
{
    None = 0,
    Fade = 1,
    StopCenter = 2,
}
