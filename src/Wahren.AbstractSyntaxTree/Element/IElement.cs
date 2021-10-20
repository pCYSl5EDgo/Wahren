namespace Wahren.AbstractSyntaxTree.Element;

public interface IElement : ITokenIdModifiable
{
    uint ElementTokenId { get; }
    
    int ElementKeyLength { get; set; }

    bool HasElementVariant { get; set; }
    
    bool HasValue { get; set; }
}

public interface IElement<T> : IElement
{
    ref T Value { get; }
}
