namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

[System.Flags]
public enum ElementContentType : ulong
{
    Unknown = 0UL,
    Scenario = 0x1UL,
    Event = 0x2UL,
    Story = 0x4UL,
    Movetype = 0x8UL,
    Skill = 0x10UL,
    Skillset = 0x20UL,
    Race = 0x40UL,
    Unit = 0x80UL,
    Class = 0x100UL,
    Power = 0x200UL,
    Spot = 0x400UL,
    Field = 0x800UL,
    Object = 0x1000UL,
    Dungeon = 0x2000UL,
    Voice = 0x4000UL,
    VoiceType = 0x8000UL,
    AttributeType = 0x10000UL,
    NumberVariable = 0x20000UL,
    StringVariabl = 0x40000UL,
    Number = 0x80000UL,
    FloatingNumber = 0x100000UL,
    Text = 0x200000UL,
    Boolean = 0x400000UL,
    Special = 0x800000UL,
}
