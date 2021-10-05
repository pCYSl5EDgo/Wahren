﻿namespace Wahren.AbstractSyntaxTree;

[System.Flags]
public enum ReferenceKind : ulong
{
    Unknown                     = 0x_0000_0000_0000_0000UL,
    Special                     = 0x_0000_0000_0000_0001UL,
    SpecialLate                 = 0x_0000_0000_0000_0002UL,
    Scenario                    = 0x_0000_0000_0000_0004UL,
    Event                       = 0x_0000_0000_0000_0008UL,
    Story                       = 0x_0000_0000_0000_0010UL,
    Movetype                    = 0x_0000_0000_0000_0020UL,
    Skill                       = 0x_0000_0000_0000_0040UL,
    Skillset                    = 0x_0000_0000_0000_0080UL,
    Race                        = 0x_0000_0000_0000_0100UL,
    Unit                        = 0x_0000_0000_0000_0200UL,
    Class                       = 0x_0000_0000_0000_0400UL,
    Power                       = 0x_0000_0000_0000_0800UL,
    Spot                        = 0x_0000_0000_0000_1000UL,
    Field                       = 0x_0000_0000_0000_2000UL,
    Object                      = 0x_0000_0000_0000_4000UL,
    Dungeon                     = 0x_0000_0000_0000_8000UL,
    Voice                       = 0x_0000_0000_0001_0000UL,
    AttributeType               = 0x_0000_0000_0002_0000UL,
    Number                      = 0x_0000_0000_0004_0000UL,
    Text                        = 0x_0000_0000_0008_0000UL,
    CompoundText                = 0x_0000_0000_0010_0000UL,
    Boolean                     = 0x_0000_0000_0020_0000UL,
    Status                      = 0x_0000_0000_0040_0000UL,
    RedBlue                     = 0x_0000_0000_0080_0000UL,
    VoiceTypeReader             = 0x_0000_0000_0100_0000UL,
    VoiceTypeWriter             = 0x_0000_0000_0200_0000UL,
    NumberVariableReader        = 0x_0000_0000_0400_0000UL,
    NumberVariableWriter        = 0x_0000_0000_0800_0000UL,
    StringVariableReader        = 0x_0000_0000_1000_0000UL,
    StringVariableWriter        = 0x_0000_0000_2000_0000UL,
    GlobalVariableReader        = 0x_0000_0000_4000_0000UL,
    GlobalVariableWriter        = 0x_0000_0000_8000_0000UL,
    FieldAttributeTypeReader    = 0x_0000_0001_0000_0000UL,
    FieldAttributeTypeWriter    = 0x_0000_0002_0000_0000UL,
    FieldIdReader               = 0x_0000_0004_0000_0000UL,
    FieldIdWriter               = 0x_0000_0008_0000_0000UL,
    ClassTypeReader             = 0x_0000_0010_0000_0000UL,
    ClassTypeWriter             = 0x_0000_0020_0000_0000UL,
    SkillGroupReader            = 0x_0000_0040_0000_0000UL,
    GlobalStringVariableReader  = 0x_0000_0080_0000_0000UL,
    GlobalStringVariableWriter  = 0x_0000_0100_0000_0000UL,
    imagedata                   = 0x_0000_0200_0000_0000UL,
    imagedata2                  = 0x_0000_0400_0000_0000UL,
    map                         = 0x_0000_0800_0000_0000UL,
    bgm                         = 0x_0000_1000_0000_0000UL,
    face                        = 0x_0000_2000_0000_0000UL,
    sound                       = 0x_0000_4000_0000_0000UL,
    picture                     = 0x_0000_8000_0000_0000UL,
    image_file                  = 0x_0001_0000_0000_0000UL,
    flag                        = 0x_0002_0000_0000_0000UL,
    icon                        = 0x_0004_0000_0000_0000UL,
    font                        = 0x_0008_0000_0000_0000UL,
}
