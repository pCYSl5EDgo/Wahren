namespace Wahren.AbstractSyntaxTree.Node;

public struct DetailNode : INode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }

    public DisposableList<VariantPair<Pair_NullableString_NullableIntElement>> StringElementList;

    public DetailNode()
    {
        StringElementList = new();
    }

    public void Dispose()
    {
        StringElementList.Dispose();
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (Kind >= indexEqualToOrGreaterThan)
        {
            Kind += count;
        }

        if (BracketLeft >= indexEqualToOrGreaterThan)
        {
            BracketLeft += count;
        }

        if (BracketRight >= indexEqualToOrGreaterThan)
        {
            BracketRight += count;
        }

        foreach (ref var pair in StringElementList.AsSpan())
        {
            pair.IncrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (Kind >= indexEqualToOrGreaterThan)
        {
            Kind -= count;
        }

        if (BracketLeft >= indexEqualToOrGreaterThan)
        {
            BracketLeft -= count;
        }

        if (BracketRight >= indexEqualToOrGreaterThan)
        {
            BracketRight -= count;
        }

        foreach (ref var pair in StringElementList.AsSpan())
        {
            pair.DecrementToken(indexEqualToOrGreaterThan, count);
        }
    }
}
