namespace Wahren.AbstractSyntaxTree.Statement;

public interface IStatement : IDisposable, ITokenIdModifiable
{
    uint TokenId { get; }

    string DisplayName { get; }
}
