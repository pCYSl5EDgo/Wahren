namespace Wahren.AbstractSyntaxTree;

[System.Flags]
public enum ReferenceKind : ulong
{
    Read            = 0x00000001UL,
    Write           = 0x00000002UL,
    Unknown         = 0x00000000UL,
    Scenario        = 0x00000004UL,
    Event           = 0x00000008UL,
    Story           = 0x00000010UL,
    Movetype        = 0x00000020UL,
    Skill           = 0x00000040UL,
    Skillset        = 0x00000080UL,
    Race            = 0x00000100UL,
    Unit            = 0x00000200UL,
    Class           = 0x00000400UL,
    Power           = 0x00000800UL,
    Spot            = 0x00001000UL,
    Field           = 0x00002000UL,
    Object          = 0x00004000UL,
    Dungeon         = 0x00008000UL,
    Voice           = 0x00010000UL,
    VoiceType       = 0x00020000UL,
    AttributeType   = 0x00040000UL,
    NumberVariable  = 0x00080000UL,
    StringVariabl   = 0x00100000UL,
    Number          = 0x00200000UL,
    FloatingNumber  = 0x00400000UL,
    Text            = 0x00800000UL,
    Boolean         = 0x01000000UL,
    Special         = 0x02000000UL,
}
