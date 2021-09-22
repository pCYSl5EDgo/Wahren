namespace Wahren.AbstractSyntaxTree.Node;

public sealed class SoundNode
    : INode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }

    public StringSpanKeyDictionary<Pair_NullableString_NullableIntElement> Others = new();

    public void Dispose()
    {
        Others.Dispose();
    }
}
