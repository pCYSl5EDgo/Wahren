namespace Wahren.AbstractSyntaxTree.Statement;

public sealed class CallActionStatement : IStatement
{
    public uint ParenRightTokenId;

    public ArrayPoolList<Argument> Arguments = new();

    public string DisplayName => "action call";

    public uint TokenId { get; set; }
    public ActionKind Kind { get; set; }

    public CallActionStatement(uint tokenId, ActionKind actionKind)
    {
        TokenId = tokenId;
        Kind = actionKind;
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += indexEqualToOrGreaterThan;
        }

        foreach (ref var argument in Arguments.AsSpan())
        {
            argument.IncrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= indexEqualToOrGreaterThan;
        }

        foreach (ref var argument in Arguments.AsSpan())
        {
            argument.DecrementToken(indexEqualToOrGreaterThan, count);
        }
    }

    public void Dispose()
    {
        Arguments.Dispose();
    }
}
