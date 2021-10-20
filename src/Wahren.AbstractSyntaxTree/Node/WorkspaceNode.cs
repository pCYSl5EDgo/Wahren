namespace Wahren.AbstractSyntaxTree.Node;

public struct WorkspaceNode
    : INode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }

    public StringSpanKeyDictionary<Pair_NullableString_NullableInt_ArrayElement> Dictionary = new();

    public void Dispose()
    {
        Dictionary.Dispose();
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

        foreach (var item in Dictionary)
        {
            item?.IncrementToken(indexEqualToOrGreaterThan, count);
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

        foreach (var item in Dictionary)
        {
            item?.DecrementToken(indexEqualToOrGreaterThan, count);
        }
    }
}
