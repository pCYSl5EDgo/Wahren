namespace Wahren.AbstractSyntaxTree.Element.Statement;

public sealed record class CallEndingRollActionStatement(uint TokenId, uint Text0, uint Text1, byte Font0, byte Font1, EndingRollDisplayMethod DisplyMethod, byte MargnFontType, byte MarginFontCount) : IStatement
{
    public string DisplayName => "ending";

    public uint TextCount => 2U - (Text1 == uint.MaxValue ? 1U : 0U);

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
