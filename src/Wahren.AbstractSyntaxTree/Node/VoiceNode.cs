namespace Wahren.AbstractSyntaxTree.Node;

public struct VoiceNode
    : IInheritableNode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }
    public uint? Super { get; set; }
    public uint Name { get; set; }

    public ScenarioVariantPair<VoiceTypeElement> VoiceType = new();
    public ScenarioVariantPair<VoiceTypeElement> DelSkill = new();
    public ScenarioVariantPair<StringArrayElement> Spot = new();
    public ScenarioVariantPair<StringArrayElement> Roam = new();
    public ScenarioVariantPair<StringArrayElement> Power = new();

    public void Dispose()
    {
        VoiceType.Dispose();
        DelSkill.Dispose();
        Spot.Dispose();
        Roam.Dispose();
        Power.Dispose();
    }
}
