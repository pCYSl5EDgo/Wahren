namespace Wahren.AbstractSyntaxTree.Node;

public partial struct DungeonNode : IInheritableNode
{
    public uint Kind { get; set; }
    public uint BracketLeft { get; set; }
    public uint BracketRight { get; set; }
    public uint? Super { get; set; }
    public uint Name { get; set; }

	public VariantPair<Pair_NullableString_NullableIntElement> name = new();
	public VariantPair<Pair_NullableString_NullableIntElement> max = new();
	public VariantPair<Pair_NullableString_NullableIntElement> move_speed = new();
	public VariantPair<Pair_NullableString_NullableIntElement> prefix = new();
	public VariantPair<Pair_NullableString_NullableIntElement> suffix = new();
	public VariantPair<Pair_NullableString_NullableIntElement> lv_adjust = new();
	public VariantPair<Pair_NullableString_NullableIntElement> open = new();
	public VariantPair<Pair_NullableString_NullableIntElement> limit = new();
	public VariantPair<Pair_NullableString_NullableIntElement> bgm = new();
	public VariantPair<Pair_NullableString_NullableIntElement> volume = new();
	public VariantPair<Pair_NullableString_NullableIntElement> blind = new();
	public VariantPair<Pair_NullableString_NullableIntElement> base_level = new();
	public VariantPair<Pair_NullableString_NullableIntElement> map = new();
	public VariantPair<Pair_NullableString_NullableIntElement> floor = new();
	public VariantPair<Pair_NullableString_NullableIntElement> wall = new();
	public VariantPair<Pair_NullableString_NullableIntElement> start = new();
	public VariantPair<Pair_NullableString_NullableIntElement> goal = new();
	public VariantPair<Pair_NullableString_NullableIntElement> monster_num = new();
	public VariantPair<Pair_NullableString_NullableIntElement> box = new();
	public VariantPair<Pair_NullableString_NullableIntElement> item_num = new();
	public VariantPair<Pair_NullableString_NullableIntElement> item_text = new();
	public VariantPair<Pair_NullableString_NullableInt_ArrayElement> color = new();
	public VariantPair<Pair_NullableString_NullableInt_ArrayElement> home = new();
	public VariantPair<Pair_NullableString_NullableInt_ArrayElement> ray = new();
	public VariantPair<Pair_NullableString_NullableInt_ArrayElement> monster = new();
	public VariantPair<Pair_NullableString_NullableInt_ArrayElement> item = new();

	public void Dispose()
	{
		name.Dispose();
		max.Dispose();
		move_speed.Dispose();
		prefix.Dispose();
		suffix.Dispose();
		lv_adjust.Dispose();
		open.Dispose();
		limit.Dispose();
		bgm.Dispose();
		volume.Dispose();
		blind.Dispose();
		base_level.Dispose();
		map.Dispose();
		floor.Dispose();
		wall.Dispose();
		start.Dispose();
		goal.Dispose();
		monster_num.Dispose();
		box.Dispose();
		item_num.Dispose();
		item_text.Dispose();
		color.Dispose();
		home.Dispose();
		ray.Dispose();
		monster.Dispose();
		item.Dispose();
	}
}