namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct NodeInfo
{
    public string Name;
    public bool HasBlock;
    public bool IsAllScenarioVariant;
    public ElementInfo[] Elements;
    public AdditionalField[] Fields;
    public bool IsAllowedUndefinedContent => Name == "Power";

    public NodeInfo(string name, ElementInfo[] elements, bool hasBlock = false, bool isAllScenarioVariant = true)
    {
        Name = name;
        HasBlock = hasBlock;
        IsAllScenarioVariant = isAllScenarioVariant;
        Elements = elements;
        Fields = System.Array.Empty<AdditionalField>();
    }

    public struct AdditionalField
    {
        public string Type;
        public string Name;
        public bool IsDisposable;

        public AdditionalField(string type, string name, bool isDisposable)
        {
            Type = type;
            Name = name;
            IsDisposable = isDisposable;
        }
    }

    public static readonly NodeInfo Power = new("Power", ElementInfo.Power) { Fields = new AdditionalField[] { new("DisposableList<VariantPair<Pair_NullableString_NullableIntElement>>", "Others", true) } };
    public static readonly NodeInfo Class = new("Class", ElementInfo.Class);
    public static readonly NodeInfo Dungeon = new("Dungeon", ElementInfo.Dungeon, isAllScenarioVariant: false);
    public static readonly NodeInfo Field = new("Field", ElementInfo.Field);
    public static readonly NodeInfo Movetype = new("Movetype", ElementInfo.Movetype);
    public static readonly NodeInfo Object = new("Object", ElementInfo.Object);
    public static readonly NodeInfo Race = new("Race", ElementInfo.Race);
    public static readonly NodeInfo Skill = new("Skill", ElementInfo.Skill, isAllScenarioVariant: false) { Fields = new AdditionalField[] { new(nameof(SkillKind), nameof(SkillKind), false) } };
    public static readonly NodeInfo Skillset = new("Skillset", ElementInfo.Skillset);
    public static readonly NodeInfo Spot = new("Spot", ElementInfo.Spot);
    public static readonly NodeInfo Unit = new("Unit", ElementInfo.Unit);
    public static readonly NodeInfo Voice = new("Voice", ElementInfo.Voice);
    public static readonly NodeInfo Scenario = new("Scenario", ElementInfo.Scenario, hasBlock: true);
    public static readonly NodeInfo Event = new("Event", ElementInfo.Event, hasBlock: true) { Fields = new AdditionalField[] { new(nameof(EventKind), nameof(EventKind), false) } };
    public static readonly NodeInfo Story = new("Story", ElementInfo.Story, hasBlock: true);

    public static readonly NodeInfo[] Nodes = new NodeInfo[]
    {
        Power,
        Class,
        Dungeon,
        Field,
        Movetype,
        Object,
        Race,
        Skill,
        Skillset,
        Spot,
        Unit,
        Voice,
        Scenario,
        Event,
        Story,
    };
}
