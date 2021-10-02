namespace Wahren.AbstractSyntaxTree.Statement;

public interface IStatement : IDisposable
{
    uint TokenId { get; }

    string DisplayName { get; }
}
