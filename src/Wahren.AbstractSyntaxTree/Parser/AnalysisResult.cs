namespace Wahren.AbstractSyntaxTree.Parser;

public sealed class AnalysisResult : IDisposable
{
    public StringSpanKeySlowSet ScenarioSet = new();
    public StringSpanKeySlowSet EventSet = new();
    public StringSpanKeySlowSet StorySet = new();
    public StringSpanKeySlowSet MovetypeSet = new();
    public StringSpanKeySlowSet SkillSet = new();
    public StringSpanKeySlowSet SkillsetSet = new();
    public StringSpanKeySlowSet RaceSet = new();
    public StringSpanKeySlowSet UnitSet = new();
    public StringSpanKeySlowSet ClassSet = new();
    public StringSpanKeySlowSet PowerSet = new();
    public StringSpanKeySlowSet SpotSet = new();
    public StringSpanKeySlowSet FieldSet = new();
    public StringSpanKeySlowSet ObjectSet = new();
    public StringSpanKeySlowSet DungeonSet = new();
    public StringSpanKeySlowSet VoiceSet = new();

    public StringSpanKeySlowSet AttributeTypeSet = new();
    public StringSpanKeySlowSet FieldAttributeTypeWriterSet = new();
    public StringSpanKeySlowSet FieldAttributeTypeReaderSet = new();
    public StringSpanKeySlowSet FieldIdWriterSet = new();
    public StringSpanKeySlowSet FieldIdReaderSet = new();
    public StringSpanKeySlowSet VoiceTypeWriterSet = new();
    public StringSpanKeySlowSet VoiceTypeReaderSet = new();
    public StringSpanKeySlowSet ClassTypeWriterSet = new();
    public StringSpanKeySlowSet ClassTypeReaderSet = new();
    public StringSpanKeySlowSet SkillGroupReaderSet = new();

    public StringSpanKeySlowSet imagedataSet = new();
    public StringSpanKeySlowSet imagedata2Set = new();
    public StringSpanKeySlowSet mapSet = new();
    public StringSpanKeySlowSet bgmSet = new();
    public StringSpanKeySlowSet iconSet = new();
    public StringSpanKeySlowSet faceSet = new();
    public StringSpanKeySlowSet soundSet = new();
    public StringSpanKeySlowSet pictureSet = new();
    public StringSpanKeySlowSet image_fileSet = new();
    public StringSpanKeySlowSet flagSet = new();
    public StringSpanKeySlowSet fontSet = new();

    public StringSpanKeySlowSet NumberVariableWriterSet = new();
    public StringSpanKeySlowSet NumberVariableReaderSet = new();
    public StringSpanKeySlowSet StringVariableWriterSet = new();
    public StringSpanKeySlowSet StringVariableReaderSet = new();
    public StringSpanKeySlowSet GlobalVariableWriterSet = new();
    public StringSpanKeySlowSet GlobalVariableReaderSet = new();
    public StringSpanKeySlowSet GlobalStringVariableWriterSet = new();
    public StringSpanKeySlowSet GlobalStringVariableReaderSet = new();

    public AnalysisResult()
    {
        Initialize();
    }

    public void Dispose()
    {
        ScenarioSet.Dispose();
        StorySet.Dispose();
        MovetypeSet.Dispose();
        EventSet.Dispose();
        PowerSet.Dispose();
        RaceSet.Dispose();
        SpotSet.Dispose();
        UnitSet.Dispose();
        ClassSet.Dispose();
        SkillSet.Dispose();
        SkillsetSet.Dispose();
        VoiceSet.Dispose();
        FieldSet.Dispose();
        ObjectSet.Dispose();
        DungeonSet.Dispose();

        AttributeTypeSet.Dispose();
        FieldAttributeTypeWriterSet.Dispose();
        FieldAttributeTypeReaderSet.Dispose();
        FieldIdWriterSet.Dispose();
        FieldIdReaderSet.Dispose();
        VoiceTypeWriterSet.Dispose();
        VoiceTypeReaderSet.Dispose();
        ClassTypeWriterSet.Dispose();
        ClassTypeReaderSet.Dispose();

        NumberVariableWriterSet.Dispose();
        StringVariableWriterSet.Dispose();
        NumberVariableReaderSet.Dispose();
        StringVariableReaderSet.Dispose();
        GlobalVariableWriterSet.Dispose();
        GlobalVariableReaderSet.Dispose();
        GlobalStringVariableWriterSet.Dispose();
        GlobalStringVariableReaderSet.Dispose();
        SkillGroupReaderSet.Dispose();

        imagedataSet.Dispose();
        imagedata2Set.Dispose();
        mapSet.Dispose();
        bgmSet.Dispose();
        iconSet.Dispose();
        faceSet.Dispose();
        soundSet.Dispose();
        pictureSet.Dispose();
        image_fileSet.Dispose();
        flagSet.Dispose();
        fontSet.Dispose();
    }

    private void Initialize()
    {
        UnitSet.InitialAdd("dead_event_a");
        UnitSet.InitialAdd("dead_event_d");
        ScenarioSet.InitialAdd("a");
        ScenarioSet.InitialAdd("b");
        ScenarioSet.InitialAdd("c");
        ScenarioSet.InitialAdd("d");
        ScenarioSet.InitialAdd("e");
        ScenarioSet.InitialAdd("f");
        ScenarioSet.InitialAdd("g");
        ScenarioSet.InitialAdd("h");
        ScenarioSet.InitialAdd("i");
        ScenarioSet.InitialAdd("j");
        VoiceTypeWriterSet.InitialAdd("hold");
        VoiceTypeWriterSet.InitialAdd("advance");
        VoiceTypeWriterSet.InitialAdd("even");
        VoiceTypeWriterSet.InitialAdd("push");
        VoiceTypeWriterSet.InitialAdd("back");
        VoiceTypeReaderSet.InitialAdd("hold");
        VoiceTypeReaderSet.InitialAdd("advance");
        VoiceTypeReaderSet.InitialAdd("even");
        VoiceTypeReaderSet.InitialAdd("push");
        VoiceTypeReaderSet.InitialAdd("back");

        foreach (var attribute in Enum.GetNames<AttributeTypeKind>())
        {
            AttributeTypeSet.InitialAdd(attribute);
        }

        imagedata2Set.InitialAdd("@@");
    }
}
