using System.Linq;

namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public static partial class Helper
{
    public static readonly string[] Inheritables = new string[]
    {
        nameof(ElementInfo.Spot),
        nameof(ElementInfo.Unit),
        nameof(ElementInfo.Race),
        nameof(ElementInfo.Class),
        nameof(ElementInfo.Field),
        nameof(ElementInfo.Skill),
        nameof(ElementInfo.Power),
        nameof(ElementInfo.Object),
        nameof(ElementInfo.Dungeon),
        nameof(ElementInfo.Movetype),
        nameof(ElementInfo.Skillset),
    };

    public static readonly string[] BlockContainers = new string[]
    {
        nameof(ElementInfo.Event),
        nameof(ElementInfo.Scenario),
        nameof(ElementInfo.Story),
    };

    public static bool IsAllowedUndefinedContent(string name)
    {
        switch (name)
        {
            case nameof(ElementInfo.Power):
            case nameof(ElementInfo.Workspace):
                return true;
            default: return false;
        }
    }

    public static void Deconstruct(this IGrouping<UsagePair, ElementInfo> group, out UsagePair pair, out IEnumerable<ElementInfo> names)
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

    public static SwitchGroupOuter[] MakeGroup(this ElementInfo[] elements)
    {
        return elements.GroupBy(
            x => x.name.Length,
            item => new SwitchGroupInner(item, item.name.GetKey().ToString("X16"), item.name.Length > 4 ? item.name.Substring(4) : ""),
            (len, items) => new SwitchGroupOuter(len, items)).OrderBy(x => x.len).ToArray();
    }

    public static UsageGroup[] MakeUsageGroup(this ElementInfo[] elements)
    {
        return elements.GroupBy(
           x => x.name.GetCorrespondingType(),
           x => x,
           (type, xs) => new UsageGroup(type, xs.GroupBy(x => new UsagePair(x.name.GetCorrespondingTrailer(), x.name.GetCorrespondingParserFunction())).ToArray())).ToArray();
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
                return nameof(Trailer.LOYAL);
            case "enemy":
            case "staff":
            case "friend":
            case "offset":
            case "delskill":
            case "delskill2":
            case "voice_type":
                return nameof(Trailer.OFFSET);
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
                return nameof(Trailer.RAY);
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
                return nameof(Trailer.MEMBER);
            case "roam":
            case "spot":
            case "power":
                return nameof(Trailer.ROAM);
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
                return nameof(Trailer.CONSTI);
            case "text":
                return nameof(Trailer.TEXT);
            default:
                return nameof(Trailer.DEFAULT);
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
