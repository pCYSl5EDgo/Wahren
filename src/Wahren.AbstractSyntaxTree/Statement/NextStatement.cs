namespace Wahren.AbstractSyntaxTree.Statement;

public sealed class NextStatement : IStatement
{
    public string DisplayName => "next";

    public uint TokenId { get; set; }

    public NextStatement(uint tokenId)
    {
        TokenId = tokenId;
    }

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId += count;
        }
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        if (TokenId >= indexEqualToOrGreaterThan)
        {
            TokenId -= count;
        }
    }

    public void Dispose()
    {
    }
}
