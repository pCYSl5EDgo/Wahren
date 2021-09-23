using System.Linq;
using System;

namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public static partial class Helper
{
    public static readonly string[] Inheritables = new string[]
    {
        "Spot",
        "Unit",
        "Race",
        "Class",
        "Field",
        "Skill",
        "Power",
        "Object",
        "Dungeon",
        "Movetype",
        "Skillset",
    };

    public static readonly string[] BlockContainers = new string[]
    {
        "Event",
        "Scenario",
        "Story",
    };

    public static bool IsAllowedUndefinedContent(string name)
    {
        switch (name)
        {
            case "Power":
            case "Workspace":
                return true;
            default: return false;
        }
    }

    public static string[] Get(string name)
    {
        switch (name)
        {
            case "Power": return Power;
            case "Class": return Class;
            case "Dungeon": return Dungeon;
            case "Field": return Field;
            case "Movetype": return Movetype;
            case "Object": return Object;
            case "Race": return Race;
            case "Skill": return Skill;
            case "Skillset": return Skillset;
            case "Spot": return Spot;
            case "Unit": return Unit;
            case "Scenario": return Scenario;
            case "Event": return Event;
            case "Story": return Story;
            default: return Array.Empty<string>();
        }
    }

    public static void Deconstruct(this IGrouping<UsagePair, string> group, out UsagePair pair, out IEnumerable<string> names)
    {
        pair = group.Key;
        names = group;
    }

    public static string Escape(this string element)
    {
        switch (element)
        {
            case "event": return "@event";
            case "break": return "@break";
            case "continue": return "@continue";
            case "while": return "@while";
            case "if": return "@if";
            case "class": return "@class";
            case "struct": return "@struct";
            case "record": return "@record";
            case "for": return "@for";
            case "return": return "@return";
            case "foreach": return "@foreach";
            case "picture@cutin": return "picture_atmark_cutin";
            case "object": return "@object";
            default: return element;
        }
    }

    public static ulong GetKey(this string element)
    {
        switch (element.Length)
        {
            case 0: return 0;
            case 1: return element[0];
            case 2: return (element[0]) | (((ulong)element[1]) << 16);
            case 3: return (element[0]) | (((ulong)element[1]) << 16) | (((ulong)element[2]) << 32);
            default: return (element[0]) | (((ulong)element[1]) << 16) | (((ulong)element[2]) << 32) | (((ulong)element[3]) << 48);
        }
    }

    public static SwitchGroupOuter[] MakeGroup(this string[] elements)
    {
        return elements.GroupBy(
            x => x.Length,
            item => new SwitchGroupInner(item, item.GetKey().ToString("X16"), item.Length > 4 ? item.Substring(4) : ""),
            (len, items) => new SwitchGroupOuter(len, items)).OrderBy(x => x.len).ToArray();
    }

    public static UsageGroup[] MakeUsageGroup(this string[] elements)
    {
        return elements.GroupBy(
           x => x.GetCorrespondingType(),
           x => x,
           (type, xs) => new UsageGroup(type, xs.GroupBy(x => new UsagePair(x.GetCorrespondingTrailer(), x.GetCorrespondingParserFunction())).ToArray())).ToArray();
    }

    public static string GetCorrespondingTrailer(this string element)
    {
        switch (element)
        {
            case "str":
            case "fkey":
            case "loyal":
            case "brave":
            case "arbeit":
            case "change":
            case "ground":
            case "gun_delay":
                return "LOYAL";
            case "enemy":
            case "staff":
            case "friend":
            case "offset":
            case "delskill":
            case "delskill2":
            case "voice_type":
                return "OFFSET";
            case "ray":
            case "poli":
            case "camp":
            case "home":
            case "multi":
            case "learn":
            case "color":
            case "joint":
            case "skill":
            case "skill2":
            case "weapon":
            case "weapon2":
            case "activenum":
            case "friend_ex":
                return "RAY";
            case "add2":
            case "item":
            case "merce":
            case "next2":
            case "next3":
            case "sound":
            case "member":
            case "monster":
            case "item_hold":
            case "item_sale":
            case "just_next":
            case "castle_guard":
                return "MEMBER";
            case "roam":
            case "spot":
            case "power":
                return "ROAM";
            case "icon":
            case "wave":
            case "cutin":
            case "diplo":
            case "consti":
            case "league":
            case "loyals":
            case "merits":
            case "yorozu":
            case "enemy_power":
            case "leader_skill":
            case "assist_skill":
                return "CONSTI";
            case "text":
                return "TEXT";
            default:
                return "DEFAULT";
        }
    }

    public static string GetCorrespondingParserFunction(this string element) => "Parse_Element_" + element.GetCorrespondingTrailer();

    public static string GetCorrespondingType(this string element)
    {
        switch (element)
        {
            case "enemy":
            case "staff":
            case "friend":
            case "offset":
            case "delskill":
            case "delskill2":
            case "voice_type":
            case "roam":
            case "spot":
            case "power":
                return "StringArrayElement";
            case "multi":
            case "ray":
            case "poli":
            case "camp":
            case "home":
            case "learn":
            case "color":
            case "joint":
            case "skill":
            case "skill2":
            case "weapon":
            case "weapon2":
            case "activenum":
            case "friend_ex":
            case "add2":
            case "item":
            case "merce":
            case "next2":
            case "next3":
            case "sound":
            case "member":
            case "monster":
            case "item_hold":
            case "item_sale":
            case "just_next":
            case "castle_guard":
            case "icon":
            case "wave":
            case "cutin":
            case "diplo":
            case "consti":
            case "league":
            case "loyals":
            case "merits":
            case "yorozu":
            case "enemy_power":
            case "leader_skill":
            case "assist_skill":
                return "Pair_NullableString_NullableInt_ArrayElement";
            case "text":
                return "StringElement";
            default:
                return "Pair_NullableString_NullableIntElement";
        }
    }
}
