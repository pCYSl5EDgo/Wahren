namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct NodeInfo
{
    public string Name;
    public bool HasBlock;
    public ElementInfo[] Elements;
    public AdditionalField[] Fields;

    public NodeInfo(string name, ElementInfo[] elements, bool hasBlock = false)
    {
        Name = name;
        HasBlock = hasBlock;
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

    public bool IsDetail5 => Name == nameof(Power) || Name == nameof(Spot) || Name == nameof(Race) || Name == nameof(Unit) || Name == nameof(Class);
    public bool IsSkillOrSkillset => Name == nameof(Skill) || Name == nameof(Skillset);

    public static readonly NodeInfo Power = new("Power", ElementInfo.Power);
    public static readonly NodeInfo Class = new("Class", ElementInfo.Class);
    public static readonly NodeInfo Dungeon = new("Dungeon", ElementInfo.Dungeon);
    public static readonly NodeInfo Field = new("Field", ElementInfo.Field);
    public static readonly NodeInfo Movetype = new("Movetype", ElementInfo.Movetype);
    public static readonly NodeInfo Object = new("Object", ElementInfo.Object);
    public static readonly NodeInfo Race = new("Race", ElementInfo.Race);
    public static readonly NodeInfo Skill = new("Skill", ElementInfo.Skill) { Fields = new AdditionalField[] { new(nameof(SkillKind), nameof(SkillKind), false), new(nameof(SkillMovetype), nameof(SkillMovetype), false) } };
    public static readonly NodeInfo Skillset = new("Skillset", ElementInfo.Skillset);
    public static readonly NodeInfo Spot = new("Spot", ElementInfo.Spot);
    public static readonly NodeInfo Unit = new("Unit", ElementInfo.Unit);
    public static readonly NodeInfo Voice = new("Voice", ElementInfo.Voice);
    public static readonly NodeInfo Scenario = new("Scenario", ElementInfo.Scenario, hasBlock: true);
    public static readonly NodeInfo Event = new("Event", ElementInfo.Event, hasBlock: true) { Fields = new AdditionalField[] { new(nameof(EventKind), nameof(EventKind), false) } };
    public static readonly NodeInfo Story = new("Story", ElementInfo.Story, hasBlock: true);
    public static readonly NodeInfo Context = new("Context", ElementInfo.Context);

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
