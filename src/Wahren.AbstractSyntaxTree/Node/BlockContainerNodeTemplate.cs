﻿#nullable enable
// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE .tt FILE INSTEAD.
// </auto-generated>
namespace Wahren.AbstractSyntaxTree.Node;
using Element.Statement;
public partial struct EventNode : IInheritableNode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }
    public uint? Super { get; set; }
    public uint Name { get; set; }

    public List<IStatement> Statements = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> disperse = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> castle = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> castle_battle = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> blind = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> w = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> h = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> bg = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> bcg = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> bgm = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> map = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> name = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> size = new();
	public ScenarioVariantPair<Pair_NullableString_NullableInt_ArrayElement> color = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> block = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> limit = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> title = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> center = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> italic = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> handle = new();
	public ScenarioVariantPair<Pair_NullableString_NullableInt_ArrayElement> member = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> second = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> volume = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> bg_fade = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> dark_fade = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> dark_alpha = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> bg_interval = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> dark_fade_e = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> last_second = new();

	public void Dispose()
	{
		Statements.Dispose();
		disperse.Dispose();
		castle.Dispose();
		castle_battle.Dispose();
		blind.Dispose();
		w.Dispose();
		h.Dispose();
		bg.Dispose();
		bcg.Dispose();
		bgm.Dispose();
		map.Dispose();
		name.Dispose();
		size.Dispose();
		color.Dispose();
		block.Dispose();
		limit.Dispose();
		title.Dispose();
		center.Dispose();
		italic.Dispose();
		handle.Dispose();
		member.Dispose();
		second.Dispose();
		volume.Dispose();
		bg_fade.Dispose();
		dark_fade.Dispose();
		dark_alpha.Dispose();
		bg_interval.Dispose();
		dark_fade_e.Dispose();
		last_second.Dispose();
	}
}
public partial struct ScenarioNode : IInheritableNode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }
    public uint? Super { get; set; }
    public uint Name { get; set; }

    public List<IStatement> Statements = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> ws_red = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> ws_blue = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> ws_green = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> ws_alpha = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> ws_light = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> ws_light_range = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> discus = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> save_name = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> enable_select = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> max_unit = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> blind = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> name = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> map = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> help = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> locate_x = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> locate_y = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> begin_text = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> world = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> fight = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> politics = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> war_capacity = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> spot_capacity = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> gain_per = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> support_range = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> my_range = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> myhelp_range = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> base_level = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> monster_level = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> training_up = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> actor_per = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> sortkey = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> default_ending = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> power_order = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> enable = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> enable_talent = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> party = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> no_autosave = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> zone = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> nozone = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> item0 = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> item1 = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> item2 = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> item3 = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> item4 = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> item5 = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> item6 = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> item_limit = new();
	public ScenarioVariantPair<Pair_NullableString_NullableInt_ArrayElement> poli = new();
	public ScenarioVariantPair<Pair_NullableString_NullableInt_ArrayElement> camp = new();
	public ScenarioVariantPair<Pair_NullableString_NullableInt_ArrayElement> multi = new();
	public ScenarioVariantPair<Pair_NullableString_NullableInt_ArrayElement> item = new();
	public ScenarioVariantPair<Pair_NullableString_NullableInt_ArrayElement> item_sale = new();
	public ScenarioVariantPair<Pair_NullableString_NullableInt_ArrayElement> item_hold = new();
	public ScenarioVariantPair<StringElement> text = new();
	public ScenarioVariantPair<StringArrayElement> roam = new();
	public ScenarioVariantPair<StringArrayElement> spot = new();
	public ScenarioVariantPair<StringArrayElement> power = new();
	public ScenarioVariantPair<StringArrayElement> offset = new();

	public void Dispose()
	{
		Statements.Dispose();
		ws_red.Dispose();
		ws_blue.Dispose();
		ws_green.Dispose();
		ws_alpha.Dispose();
		ws_light.Dispose();
		ws_light_range.Dispose();
		discus.Dispose();
		save_name.Dispose();
		enable_select.Dispose();
		max_unit.Dispose();
		blind.Dispose();
		name.Dispose();
		map.Dispose();
		help.Dispose();
		locate_x.Dispose();
		locate_y.Dispose();
		begin_text.Dispose();
		world.Dispose();
		fight.Dispose();
		politics.Dispose();
		war_capacity.Dispose();
		spot_capacity.Dispose();
		gain_per.Dispose();
		support_range.Dispose();
		my_range.Dispose();
		myhelp_range.Dispose();
		base_level.Dispose();
		monster_level.Dispose();
		training_up.Dispose();
		actor_per.Dispose();
		sortkey.Dispose();
		default_ending.Dispose();
		power_order.Dispose();
		enable.Dispose();
		enable_talent.Dispose();
		party.Dispose();
		no_autosave.Dispose();
		zone.Dispose();
		nozone.Dispose();
		item0.Dispose();
		item1.Dispose();
		item2.Dispose();
		item3.Dispose();
		item4.Dispose();
		item5.Dispose();
		item6.Dispose();
		item_limit.Dispose();
		poli.Dispose();
		camp.Dispose();
		multi.Dispose();
		item.Dispose();
		item_sale.Dispose();
		item_hold.Dispose();
		text.Dispose();
		roam.Dispose();
		spot.Dispose();
		power.Dispose();
		offset.Dispose();
	}
}
public partial struct StoryNode : IInheritableNode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }
    public uint? Super { get; set; }
    public uint Name { get; set; }

    public List<IStatement> Statements = new();
	public ScenarioVariantPair<StringArrayElement> friend = new();
	public ScenarioVariantPair<Pair_NullableString_NullableIntElement> fight = new();

	public void Dispose()
	{
		Statements.Dispose();
		friend.Dispose();
		fight.Dispose();
	}
}
