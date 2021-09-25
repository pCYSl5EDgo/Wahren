namespace Wahren.AbstractSyntaxTree;

[System.Flags]
public enum ReferenceKind : ulong
{
    Unknown         = 0x00000000UL,
    Write           = 0x00000001UL,
    Scenario        = 0x00000002UL,
    Event           = 0x00000004UL,
    Story           = 0x00000008UL,
    Movetype        = 0x00000010UL,
    Skill           = 0x00000020UL,
    Skillset        = 0x00000040UL,
    Race            = 0x00000080UL,
    Unit            = 0x00000100UL,
    Class           = 0x00000200UL,
    Power           = 0x00000400UL,
    Spot            = 0x00000800UL,
    Field           = 0x00001000UL,
    Object          = 0x00002000UL,
    Dungeon         = 0x00004000UL,
    Voice           = 0x00008000UL,
    VoiceType       = 0x00010000UL,
    AttributeType   = 0x00020000UL,
    NumberVariable  = 0x00040000UL,
    StringVariabl   = 0x00080000UL,
    Number          = 0x00100000UL,
    FloatingNumber  = 0x00200000UL,
    Text            = 0x00400000UL,
    Boolean         = 0x00800000UL,
    Special         = 0x01000000UL,
}
