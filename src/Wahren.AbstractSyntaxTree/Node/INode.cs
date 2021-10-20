namespace Wahren.AbstractSyntaxTree.Node;

public interface INode : IDisposable, ITokenIdModifiable
{
    uint Kind { get; set; }
    uint BracketLeft { get; set; }
    uint BracketRight { get; set; }
    void IDisposable.Dispose() { }
}

public interface IInheritableNode : INode
{
    /// <summary>
    /// SuperTokenId
    /// </summary>
    uint Super { get; set; }
    bool HasSuper { get; set; }
    /// <summary>
    /// TokenId
    /// </summary>
    uint Name { get; set; }
}
