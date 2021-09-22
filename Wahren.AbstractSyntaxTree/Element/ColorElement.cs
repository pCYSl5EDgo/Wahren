namespace Wahren.AbstractSyntaxTree.Element;

public sealed class ColorElement : IElement<List<Pair_NullableString_NullableInt>>
{
    private SingleLineRange elementKey = default;
    private uint elementScenario = uint.MaxValue;
    private List<Pair_NullableString_NullableInt> value_Pair_NullableString_NullableInt;

    public uint ElementTokenId { get; set; }

    public ref SingleLineRange ElementKeyRange => ref elementKey;

    public ref uint ElementScenarioId => ref elementScenario;

    public ref List<Pair_NullableString_NullableInt> Value => ref value_Pair_NullableString_NullableInt;

    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public bool HasValue { get; set; }

    public ColorElement()
    {
        value_Pair_NullableString_NullableInt = new();
    }

    public void Dispose()
    {
        value_Pair_NullableString_NullableInt.Dispose();
    }
}
