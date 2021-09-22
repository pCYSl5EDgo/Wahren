namespace Wahren.AbstractSyntaxTree.Element;

public record class StoryFriendElement(uint ElementTokenId) : IElement<List<uint>>
{
    private List<uint> value = new();

    /// <summary>
    /// Not TokenId but ScenarioId.
    /// </summary>
    public ref List<uint> Value => ref value;

    private SingleLineRange elementKeyRange;

    public ref SingleLineRange ElementKeyRange => ref elementKeyRange;

    private uint elementScenarioId;

    public ref uint ElementScenarioId => ref elementScenarioId;

    public bool HasValue { get; set; }

    public void Dispose()
    {
        value.Dispose();
    }
}
