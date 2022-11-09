namespace Wahren.AbstractSyntaxTree.Element;

public interface IElement : ITokenIdModifiable
{
    uint ElementTokenId { get; }

    int ElementKeyLength { get; set; }

    bool HasElementVariant { get; set; }

    bool HasValue { get; set; }

    abstract static IElement Create(uint elementTokenId, int elementKeyLength, bool hasElementVariant);
}

public interface IElement<T> : IElement
{
    ref T Value { get; }
}
