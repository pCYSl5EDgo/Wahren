namespace Wahren.AbstractSyntaxTree.Element;

public interface IElement
{
    uint ElementTokenId { get; }

    ref SingleLineRange ElementKeyRange { get; }

    ref uint ElementScenarioId { get; }

    bool HasValue { get; set; }
}

public interface IElement<T> : IElement
{
    ref T Value { get; }
}
