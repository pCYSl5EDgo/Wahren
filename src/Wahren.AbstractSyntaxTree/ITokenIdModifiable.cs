namespace Wahren.AbstractSyntaxTree;

public interface ITokenIdModifiable
{
    void IncrementToken(uint indexEqualToOrGreaterThan, uint count);
    void DecrementToken(uint indexEqualToOrGreaterThan, uint count);
}
