namespace Wahren.AbstractSyntaxTree.Node;

public interface INode : IDisposable
{
    uint Kind { get; set; }
    uint BracketLeft { get; set; }
    uint BracketRight { get; set; }
    void IDisposable.Dispose() { }
}

public interface IInheritableNode : INode
{
    uint? Super { get; set; }
    uint Name { get; set; }
}
