namespace Wahren.AbstractSyntaxTree.Element.Statement;

public interface IStatement : IDisposable
{
    uint TokenId { get; }

    string DisplayName { get; }
}
