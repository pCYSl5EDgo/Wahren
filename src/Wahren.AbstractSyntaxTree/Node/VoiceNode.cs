namespace Wahren.AbstractSyntaxTree.Node;

public struct VoiceNode
    : IInheritableNode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }
    public uint? Super { get; set; }
    public uint Name { get; set; }

    public VariantPair<VoiceTypeElement> VoiceType = new();
    public VariantPair<VoiceTypeElement> DelSkill = new();
    public VariantPair<StringArrayElement> Spot = new();
    public VariantPair<StringArrayElement> Roam = new();
    public VariantPair<StringArrayElement> Power = new();

    public void Dispose()
    {
        VoiceType.Dispose();
        DelSkill.Dispose();
        Spot.Dispose();
        Roam.Dispose();
        Power.Dispose();
    }
}
