namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class TokenUtility
{
    public static bool Is_attribute(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("attribute");

    public static bool Is_attribute_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("tribute");

    public static bool Is_battle(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("battle");

    public static bool Is_battle_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ttle");

    public static bool Is_class(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("class");

    public static bool Is_class_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ass");

    public static bool Is_context(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("context");

    public static bool Is_context_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ntext");

    public static bool Is_delskill(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("delskill");

    public static bool Is_delskill_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("lskill");

    public static bool Is_detail(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("detail");

    public static bool Is_detail_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("tail");

    public static bool Is_dungeon(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("dungeon");

    public static bool Is_dungeon_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ngeon");

    public static bool Is_else(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("else");

    public static bool Is_else_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("se");

    public static bool Is_event(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("event");

    public static bool Is_event_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ent");

    public static bool Is_field(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("field");

    public static bool Is_field_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("eld");

    public static bool Is_fight(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("fight");

    public static bool Is_fight_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ght");

    public static bool Is_friend(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("friend");

    public static bool Is_friend_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("iend");

    public static bool Is_if(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("if");

    public static bool Is_movetype(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("movetype");

    public static bool Is_movetype_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("vetype");

    public static bool Is_multi(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("multi");

    public static bool Is_multi_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("lti");

    public static bool Is_object(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("object");

    public static bool Is_object_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ject");

    public static bool Is_on(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("on");

    public static bool Is_power(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("power");

    public static bool Is_power_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("wer");

    public static bool Is_race(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("race");

    public static bool Is_race_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ce");

    public static bool Is_rif(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("rif");

    public static bool Is_rif_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("f");

    public static bool Is_roam(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("roam");

    public static bool Is_roam_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("am");

    public static bool Is_scenario(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("scenario");

    public static bool Is_scenario_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("enario");

    public static bool Is_skill(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("skill");

    public static bool Is_skill_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ill");

    public static bool Is_skillset(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("skillset");

    public static bool Is_skillset_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("illset");

    public static bool Is_sound(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("sound");

    public static bool Is_sound_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("und");

    public static bool Is_spot(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("spot");

    public static bool Is_spot_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ot");

    public static bool Is_story(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("story");

    public static bool Is_story_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ory");

    public static bool Is_unit(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("unit");

    public static bool Is_unit_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("it");

    public static bool Is_voice(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("voice");

    public static bool Is_voice_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ice");

    public static bool Is_voice_type(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("voice_type");

    public static bool Is_voice_type_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ice_type");

    public static bool Is_while(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("while");

    public static bool Is_while_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("ile");

    public static bool Is_world(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("world");

    public static bool Is_world_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("rld");

    public static bool Is_workspace(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).SequenceEqual("workspace");

    public static bool Is_workspace_Skip2(ref this Result result, uint tokenIndex) => result.GetSpan(tokenIndex).Slice(2).SequenceEqual("rkspace");

}
