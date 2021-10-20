namespace Wahren.AbstractSyntaxTree.Statement;

public sealed class ReturnStatement : IStatement
{
    public string DisplayName => "return";

    public void Dispose()
    {
    }

    public uint TokenId { get; set; }

    public ReturnStatement(uint tokenId)
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
}
