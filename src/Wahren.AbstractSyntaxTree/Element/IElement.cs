namespace Wahren.AbstractSyntaxTree.Element;

public interface IElement
{
    uint ElementTokenId { get; }
    
    uint ElementKeyLength { get; set; }

    bool HasElementVariant { get; set; }
    
    ref uint ElementScenarioId { get; }

    bool HasValue { get; set; }
}

public interface IElement<T> : IElement
{
    ref T Value { get; }
}
