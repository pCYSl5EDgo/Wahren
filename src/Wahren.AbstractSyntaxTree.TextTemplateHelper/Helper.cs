using System.Linq;

namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public static partial class Helper
{
    public static void Deconstruct(this IGrouping<UsagePair, ElementInfo> group, out UsagePair pair, out IEnumerable<ElementInfo> names)
    {
        pair = group.Key;
        names = group;
    }

    public static string Escape(this string element) => element switch
    {
        "event" => "@event",
        "break" => "@break",
        "continue" => "@continue",
        "while" => "@while",
        "if" => "@if",
        "class" => "@class",
        "struct" => "@struct",
        "record" => "@record",
        "for" => "@for",
        "return" => "@return",
        "foreach" => "@foreach",
        "picture@cutin" => "picture_atmark_cutin",
        "object" => "@object",
        _ => element,
    };

    public static ulong GetKey(this string element) => StringHashUtility.Calc(element);

    public static SwitchGroupOuter[] MakeGroup(this ElementInfo[] elements)
    {
        return elements.GroupBy(
            x => x.name.Length,
            item => new SwitchGroupInner(item, item.name.GetKey().ToString("X16"), item.name.Length > StringHashUtility.HashLengthMax ? item.name.Substring(StringHashUtility.HashLengthMax) : ""),
            (len, items) => new SwitchGroupOuter(len, items)).OrderBy(x => x.len).ToArray();
    }

    public static UsageGroup[] MakeUsageGroup(this ElementInfo[] elements)
    {
        return elements.GroupBy(
           x => x.name.GetCorrespondingType(),
           x => x,
           (type, xs) => new UsageGroup(type, xs.GroupBy(x => new UsagePair(x.name.GetCorrespondingTrailer(), x.name.GetCorrespondingParserFunction())).ToArray())).ToArray();
    }

    public static string GetCorrespondingTrailer(this string element) => element switch
    {
        "str" or "fkey" or "loyal" or "brave" or "arbeit" or "change" or "ground" or "gun_delay" => nameof(Trailer.LOYAL),
        "enemy" or "staff" or "friend" or "offset" or "delskill" or "delskill2" or "voice_type" => nameof(Trailer.OFFSET),
        "ray" or "poli" or "camp" or "home" or "multi" or "learn" or "color" or "joint" or "skill" or "skill2" or "weapon" or "weapon2" or "activenum" or "friend_ex" => nameof(Trailer.RAY),
        "add2" or "item" or "merce" or "next2" or "next3" or "sound" or "member" or "monster" or "item_hold" or "item_sale" or "just_next" or "castle_guard" => nameof(Trailer.MEMBER),
        "roam" or "spot" or "power" => nameof(Trailer.ROAM),
        "icon" or "wave" or "cutin" or "diplo" or "consti" or "league" or "loyals" or "merits" or "yorozu" or "enemy_power" or "leader_skill" or "assist_skill" => nameof(Trailer.CONSTI),
        "text" => nameof(Trailer.TEXT),
        _ => nameof(Trailer.DEFAULT),
    };

    public static string GetCorrespondingParserFunction(this string element) => "Parse_Element_" + element.GetCorrespondingTrailer();

    public static string GetCorrespondingType(this string element) => element switch
    {
        "enemy" or "staff" or "friend" or "offset" or "delskill" or "delskill2" or "voice_type" or "roam" or "spot" or "power" or "multi" or "ray" or "poli" or "camp" or "home" or "learn" or "color" or "joint" or "skill" or "skill2" or "weapon" or "weapon2" or "activenum" or "friend_ex" or "add2" or "item" or "merce" or "next2" or "next3" or "sound" or "member" or "monster" or "item_hold" or "item_sale" or "just_next" or "castle_guard" or "icon" or "wave" or "cutin" or "diplo" or "consti" or "league" or "loyals" or "merits" or "yorozu" or "enemy_power" or "leader_skill" or "assist_skill" => "Pair_NullableString_NullableInt_ArrayElement",
        _ => "Pair_NullableString_NullableIntElement",
    };
}
