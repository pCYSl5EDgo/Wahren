namespace Wahren.AbstractSyntaxTree.Node;

public sealed class AttributeNode : INode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }

    public VariantPair<Pair_NullableString_NullableIntElement> poi = new();
    public VariantPair<Pair_NullableString_NullableIntElement> para = new();
    public VariantPair<Pair_NullableString_NullableIntElement> ill = new();
    public VariantPair<Pair_NullableString_NullableIntElement> sil = new();
    public VariantPair<Pair_NullableString_NullableIntElement> conf = new();
    public VariantPair<Pair_NullableString_NullableIntElement> stone = new();
    public VariantPair<Pair_NullableString_NullableIntElement> fear = new();
    public VariantPair<Pair_NullableString_NullableIntElement> suck = new();
    public VariantPair<Pair_NullableString_NullableIntElement> magsuck = new();
    public VariantPair<Pair_NullableString_NullableIntElement> drain = new();
    public VariantPair<Pair_NullableString_NullableIntElement> death = new();
    public VariantPair<Pair_NullableString_NullableIntElement> wall = new();
    public DisposableList<VariantPair<Pair_NullableString_NullableIntElement>> Others = new();

    void IDisposable.Dispose()
    {
        poi.Dispose();
        para.Dispose();
        ill.Dispose();
        sil.Dispose();
        conf.Dispose();
        stone.Dispose();
        fear.Dispose();
        suck.Dispose();
        magsuck.Dispose();
        drain.Dispose();
        death.Dispose();
        wall.Dispose();
        Others.Dispose();
    }
}
