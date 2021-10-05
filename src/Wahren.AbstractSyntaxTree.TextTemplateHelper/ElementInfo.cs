﻿namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct ElementInfo
{
    public string name;
    public string variantType;
    public ReferenceKind referenceKind;
    public string[]? specialStringArray;

    public string Name => name switch
    {
        "picture@cutin" => "picture_atmark_cutin",
        _ => name,
    };

    public ElementInfo(string name, ReferenceKind referenceKind = ReferenceKind.Unknown, string variantType = nameof(Scenario))
    {
        this.name = name;
        this.variantType = variantType;
        this.referenceKind = referenceKind;
        this.specialStringArray = null;
    }

    internal static readonly ElementInfo[] Voice = new ElementInfo[]
    {
        new("voice_type", ReferenceKind.VoiceTypeReader),
        new("delskill", ReferenceKind.VoiceTypeReader),
        new("spot", ReferenceKind.Text),
        new("roam", ReferenceKind.Text),
        new("power", ReferenceKind.Text),
    };

    internal static readonly ElementInfo[] Workspace = System.Array.Empty<ElementInfo>();

    internal static readonly ElementInfo[] Story = new ElementInfo[]
    {
        new("friend", ReferenceKind.Scenario),
        new("fight", ReferenceKind.Boolean),
    };

    internal static readonly ElementInfo[] Event = new ElementInfo[]
    {
        new("disperse", ReferenceKind.Boolean),
        new("castle", ReferenceKind.Number),
        new("castle_battle", ReferenceKind.Boolean),
        new("blind", ReferenceKind.Boolean | ReferenceKind.Number),
        new("w", ReferenceKind.Number),
        new("h", ReferenceKind.Number),
        new("bg"),
        new("bcg"),
        new("bgm", ReferenceKind.bgm),
        new("map", ReferenceKind.map),
        new("name", ReferenceKind.Text),
        new("size"),
        new("color", ReferenceKind.Number),
        new("block"),
        new("limit", ReferenceKind.Number),
        new("title", ReferenceKind.Text),
        new("center"),
        new("italic"),
        new("handle", ReferenceKind.RedBlue),
        new("member"),
        new("second"),
        new("volume", ReferenceKind.Number),
        new("bg_fade"),
        new("dark_fade"),
        new("dark_alpha"),
        new("bg_interval"),
        new("dark_fade_e"),
        new("last_second"),
    };

    internal static readonly ElementInfo[] Dungeon = new ElementInfo[]
    {
        new("name", ReferenceKind.Text),
        new("max", ReferenceKind.Number),
        new("move_speed", ReferenceKind.Number),
        new("prefix", ReferenceKind.Text),
        new("suffix", ReferenceKind.Text),
        new("lv_adjust", ReferenceKind.Number),
        new("open", ReferenceKind.Boolean),
        new("limit", ReferenceKind.Number),
        new("bgm", ReferenceKind.bgm),
        new("volume", ReferenceKind.Number),
        new("blind", ReferenceKind.Number),
        new("base_level", ReferenceKind.Number),
        new("color", ReferenceKind.Number),
        new("map", ReferenceKind.map),
        new("floor", ReferenceKind.Field),
        new("wall", ReferenceKind.Object),
        new("start", ReferenceKind.Object),
        new("goal", ReferenceKind.Object),
        new("monster", ReferenceKind.Unit | ReferenceKind.Class),
        new("monster_num", ReferenceKind.Number),
        new("box", ReferenceKind.Object),
        new("item"),
        new("item_num", ReferenceKind.Number),
        new("item_text", ReferenceKind.Boolean),
        new("home", ReferenceKind.Number),
        new("ray", ReferenceKind.Number),
    };

    internal static readonly ElementInfo[] Field = new ElementInfo[]
    {
        new("type"),
        new("attr", ReferenceKind.FieldAttributeTypeWriter),
        new("color", ReferenceKind.Number),
        new("id", ReferenceKind.FieldIdWriter),
        new("edge", ReferenceKind.Boolean),
        new("joint"),
        new("image"),
        new("add2"),
        new("member"),
        new("alt", ReferenceKind.Number),
        new("alt_max", ReferenceKind.Number),
        new("smooth"),
    };

    internal static readonly ElementInfo[] Movetype = new ElementInfo[]
    {
        new("name", ReferenceKind.Text),
        new("help", ReferenceKind.Text),
        new("consti", ReferenceKind.FieldAttributeTypeReader),
    };

    internal static readonly ElementInfo[] Object = new ElementInfo[]
    {
        new("skill"),
        new("front"),
        new("width", ReferenceKind.Number),
        new("height", ReferenceKind.Number),
        new("alpha", ReferenceKind.Number),
        new("type"),
        new("breakfire"),
        new("color", ReferenceKind.Number),
        new("land_base", ReferenceKind.Boolean | ReferenceKind.Number),
        new("no_stop", ReferenceKind.Boolean),
        new("no_wall2", ReferenceKind.Number),
        new("no_arc_hit", ReferenceKind.Boolean),
        new("radius", ReferenceKind.Number),
        new("blk", ReferenceKind.Boolean),
        new("w", ReferenceKind.Number),
        new("h", ReferenceKind.Number),
        new("a", ReferenceKind.Number),
        new("image", ReferenceKind.imagedata),
        new("image2", ReferenceKind.imagedata),
        new("image2_w", ReferenceKind.Number),
        new("image2_h", ReferenceKind.Number),
        new("image2_a", ReferenceKind.Number),
        new("member"),
        new("ground", ReferenceKind.Boolean),
    };

    internal static readonly ElementInfo[] Race = new ElementInfo[]
    {
        new("name", ReferenceKind.Text),
        new("align", ReferenceKind.Number),
        new("brave", ReferenceKind.Number),
        new("consti", ReferenceKind.AttributeType),
        new("movetype", ReferenceKind.Movetype),
    };

    internal static readonly ElementInfo[] Skill = new ElementInfo[]
    {
        new("force_ray", ReferenceKind.Boolean),
        new("bright", ReferenceKind.Boolean),
        new("func", ReferenceKind.Special) { specialStringArray = new[] { "missile", "sword", "heal", "summon", "charge", "status", } },
        new("name", ReferenceKind.Text),
        new("icon", ReferenceKind.icon),
        new("fkey", ReferenceKind.SkillGroupReader),
        new("sortkey", ReferenceKind.Number),
        new("special", ReferenceKind.Boolean | ReferenceKind.Number),
        new("delay", ReferenceKind.Number),
        new("gun_delay"),
        new("quickreload", ReferenceKind.Boolean),
        new("help", ReferenceKind.Text),
        new("hide_help", ReferenceKind.Boolean),
        new("sound", ReferenceKind.sound),
        new("msg", ReferenceKind.Text, variantType: "Unit"),
        new("picture", ReferenceKind.picture, variantType: "Unit"),
        new("cutin"),
        new("value", ReferenceKind.Number),
        new("talent", ReferenceKind.Boolean | ReferenceKind.Skill),
        new("exp_per", ReferenceKind.Number | ReferenceKind.Boolean),
        new("movetype"),
        new("type"),
        new("color", ReferenceKind.Number),
        new("w", ReferenceKind.Number),
        new("h", ReferenceKind.Number),
        new("a", ReferenceKind.Number),
        new("mp", ReferenceKind.Number),
        new("image", ReferenceKind.imagedata2),
        new("alpha_tip"),
        new("alpha_butt"),
        new("anime", ReferenceKind.Number),
        new("anime_interval", ReferenceKind.Number),
        new("center"),
        new("ground"),
        new("d360"),
        new("d360_adj"),
        new("rotate"),
        new("direct"),
        new("resize_interval", ReferenceKind.Number),
        new("resize_start", ReferenceKind.Number),
        new("resize_reverse", ReferenceKind.Number),
        new("resize_w", ReferenceKind.Number),
        new("resize_w_start", ReferenceKind.Number),
        new("resize_w_max", ReferenceKind.Number),
        new("resize_w_min", ReferenceKind.Number),
        new("resize_h_min", ReferenceKind.Number),
        new("resize_h_max", ReferenceKind.Number),
        new("resize_h_start", ReferenceKind.Number),
        new("resize_h", ReferenceKind.Number),
        new("resize_x", ReferenceKind.Number),
        new("resize_x_start", ReferenceKind.Number),
        new("resize_x_max", ReferenceKind.Number),
        new("resize_x_min", ReferenceKind.Number),
        new("resize_y_min", ReferenceKind.Number),
        new("resize_y_max", ReferenceKind.Number),
        new("resize_y_start", ReferenceKind.Number),
        new("resize_y", ReferenceKind.Number),
        new("resize_a", ReferenceKind.Number),
        new("resize_s", ReferenceKind.Number),
        new("resize_a_start", ReferenceKind.Number),
        new("resize_s_start", ReferenceKind.Number),
        new("resize_a_max", ReferenceKind.Number),
        new("resize_s_max", ReferenceKind.Number),
        new("resize_a_min", ReferenceKind.Number),
        new("resize_s_min", ReferenceKind.Number),
        new("force_fire"),
        new("slow_per"),
        new("slow_time"),
        new("slide"),
        new("slide_speed"),
        new("slide_delay"),
        new("slide_stamp"),
        new("wait_time"),
        new("wait_time2"),
        new("shake"),
        new("ray"),
        new("flash"),
        new("flash_anime"),
        new("flash_image"),
        new("collision"),
        new("afterdeath"),
        new("afterhit"),
        new("yorozu"),
        new("str"),
        new("str_ratio"),
        new("attr"),
        new("add"),
        new("add2"),
        new("add_all"),
        new("add_per"),
        new("damage"),
        new("damage_range_adjust"),
        new("attack_us"),
        new("allfunc"),
        new("bom"),
        new("homing"),
        new("homing2"),
        new("forward"),
        new("far"),
        new("hard"),
        new("hard2"),
        new("onehit"),
        new("offset"),
        new("offset_attr"),
        new("knock"),
        new("knock_speed"),
        new("knock_power"),
        new("range"),
        new("range_min"),
        new("check"),
        new("speed"),
        new("wave"),
        new("origin"),
        new("random_space"),
        new("random_space_min"),
        new("time"),
        new("height"),
        new("rush"),
        new("rush_interval"),
        new("rush_degree"),
        new("rush_random_degree"),
        new("follow"),
        new("start_degree", ReferenceKind.Number),
        new("start_degree_fix"),
        new("start_degree_turnunit"),
        new("start_degree_type"),
        new("start_random_degree", ReferenceKind.Number),
        new("drop_degree"),
        new("drop_degree2"),
        new("joint_skill"),
        new("send_target"),
        new("send_image_degree"),
        new("next"),
        new("next2"),
        new("next3"),
        new("next4"),
        new("next_order"),
        new("next_last"),
        new("next_first"),
        new("next_interval"),
        new("just_next"),
        new("pair_next"),
        new("item_type"),
        new("item_sort"),
        new("item_nosell"),
        new("price"),
        new("friend"),
        new("summon_level"),
    };

    internal static readonly ElementInfo[] Skillset = new ElementInfo[]
    {
        new("name", ReferenceKind.Text),
        new("back", ReferenceKind.icon),
        new("member", ReferenceKind.Skill),
    };

    internal static readonly ElementInfo[] Spot = new ElementInfo[]
    {
        new("value", ReferenceKind.Number),
        new("politics", ReferenceKind.Special) { specialStringArray = new[] { "on", } },
        new("merce", ReferenceKind.Unit | ReferenceKind.Class),
        new("name", ReferenceKind.Text),
        new("image", ReferenceKind.imagedata),
        new("x", ReferenceKind.Number),
        new("y", ReferenceKind.Number),
        new("w", ReferenceKind.Number),
        new("h", ReferenceKind.Number),
        new("big"),
        new("map", ReferenceKind.map),
        new("castle_battle", ReferenceKind.Boolean),
        new("yorozu", ReferenceKind.Class),
        new("limit", ReferenceKind.Number),
        new("bgm", ReferenceKind.bgm),
        new("volume", ReferenceKind.Number),
        new("gain", ReferenceKind.Number),
        new("castle", ReferenceKind.Number),
        new("capacity", ReferenceKind.Number),
        new("monster", ReferenceKind.Unit | ReferenceKind.Class),
        new("member", ReferenceKind.Unit | ReferenceKind.Class),
        new("dungeon", ReferenceKind.Dungeon),
        new("no_home", ReferenceKind.Boolean),
        new("no_raise", ReferenceKind.Boolean),
        new("castle_lot", ReferenceKind.Number),
        new("text", ReferenceKind.Text),
    };

    internal static readonly ElementInfo[] UnitFallthroughClass = new ElementInfo[]
    {
        new("troop_sort", ReferenceKind.Number),
        new("stealth", ReferenceKind.Number),
        new("free_move", ReferenceKind.Number),
        new("name", ReferenceKind.Text),
        new("help", ReferenceKind.Text),
        new("sex", ReferenceKind.Special) { specialStringArray = new[] { "neuter", "male", "female", } },
        new("a", ReferenceKind.Number),
        new("h", ReferenceKind.Number),
        new("w", ReferenceKind.Number),
        new("text", ReferenceKind.Text),
        new("sub_image_even", ReferenceKind.Boolean),
        new("yorozu", ReferenceKind.Special) { specialStringArray = new[] { "keep_direct", "no_circle", "base", "keep_color", } },
        new("radius", ReferenceKind.Number),
        new("radius_press", ReferenceKind.Number),
        new("no_escape", ReferenceKind.Boolean),
        new("no_regular", ReferenceKind.Boolean),
        new("no_knock", ReferenceKind.Boolean | ReferenceKind.Number),
        new("no_cover", ReferenceKind.Boolean),
        new("dead_event", ReferenceKind.Event),
        new("beast_unit", ReferenceKind.Boolean),
        new("summon_max", ReferenceKind.Number),
        new("summon_level", ReferenceKind.Number),
        new("attack_range", ReferenceKind.Number),
        new("escape_range", ReferenceKind.Number),
        new("escape_run", ReferenceKind.Number),
        new("hand_range", ReferenceKind.Number),
        new("wake_range", ReferenceKind.Number),
        new("view_range", ReferenceKind.Number),
        new("cavalry_range", ReferenceKind.Number),
        new("view_unit", ReferenceKind.Boolean),
        new("force_view_unit", ReferenceKind.Boolean),
        new("force_blind_unit", ReferenceKind.Boolean),
        new("satellite", ReferenceKind.Boolean | ReferenceKind.Number),
        new("hasexp", ReferenceKind.Number),
        new("brave", ReferenceKind.Boolean | ReferenceKind.Number),
        new("level", ReferenceKind.Number),
        new("hp", ReferenceKind.Number),
        new("mp", ReferenceKind.Number),
        new("attack", ReferenceKind.Number),
        new("defense", ReferenceKind.Number),
        new("magic", ReferenceKind.Number),
        new("magdef", ReferenceKind.Number),
        new("speed", ReferenceKind.Number),
        new("dext", ReferenceKind.Number),
        new("move", ReferenceKind.Number),
        new("hprec", ReferenceKind.Number),
        new("mprec", ReferenceKind.Number),
        new("heal_max", ReferenceKind.Number),
        new("attack_max", ReferenceKind.Number),
        new("defense_max", ReferenceKind.Number),
        new("magic_max", ReferenceKind.Number),
        new("magdef_max", ReferenceKind.Number),
        new("speed_max", ReferenceKind.Number),
        new("dext_max", ReferenceKind.Number),
        new("move_max", ReferenceKind.Number),
        new("hprec_max", ReferenceKind.Number),
        new("mprec_max", ReferenceKind.Number),
        new("movetype", ReferenceKind.Movetype),
        new("hpUp", ReferenceKind.Number),
        new("mpUp", ReferenceKind.Number),
        new("attackUp", ReferenceKind.Number),
        new("defenseUp", ReferenceKind.Number),
        new("magicUp", ReferenceKind.Number),
        new("magdefUp", ReferenceKind.Number),
        new("speedUp", ReferenceKind.Number),
        new("dextUp", ReferenceKind.Number),
        new("moveUp", ReferenceKind.Number),
        new("hprecUp", ReferenceKind.Number),
        new("mprecUp", ReferenceKind.Number),
        new("hpMax", ReferenceKind.Number),
        new("mpMax", ReferenceKind.Number),
        new("attackMax", ReferenceKind.Number),
        new("defenseMax", ReferenceKind.Number),
        new("magicMax", ReferenceKind.Number),
        new("magdefMax", ReferenceKind.Number),
        new("speedMax", ReferenceKind.Number),
        new("dextMax", ReferenceKind.Number),
        new("moveMax", ReferenceKind.Number),
        new("hprecMax", ReferenceKind.Number),
        new("mprecMax", ReferenceKind.Number),
        new("race", ReferenceKind.Race),
        new("sortkey",  referenceKind: ReferenceKind.Number),
        new("picture", ReferenceKind.picture),
        new("picture@cutin"),
        new("picture_detail", ReferenceKind.Special) { specialStringArray = new[] { "off", "on", "on1", "on2", "on3", } },
        new("picture_menu",  referenceKind: ReferenceKind.Number),
        new("picture_floor", ReferenceKind.Special) { specialStringArray = new[] { "top", "msg", "base", "bottom", } },
        new("picture_shift",  referenceKind: ReferenceKind.Number),
        new("picture_shift_up",  referenceKind: ReferenceKind.Number),
        new("picture_center",  referenceKind: ReferenceKind.Number),
        new("picture_back"),
        new("price", ReferenceKind.Number),
        new("cost", ReferenceKind.Number),
        new("finance", ReferenceKind.Number),
        new("tkool", ReferenceKind.Boolean),
        new("keep_form", ReferenceKind.Boolean | ReferenceKind.Number),
        new("breast_width", ReferenceKind.Number),
        new("medical", ReferenceKind.Number),
        new("active", ReferenceKind.Special) { specialStringArray = new[] { "never", "rect", "range", "time", "troop", "never2", "rect2", "range2", "time2", "troop2", } },
        new("activenum"),
        new("handle", ReferenceKind.Boolean),
        new("red", ReferenceKind.Boolean),
        new("rank_text", ReferenceKind.Text),
        new("no_training", ReferenceKind.Boolean),
        new("force_voice", ReferenceKind.Boolean),
        new("face", ReferenceKind.face),
        new("same_friend", ReferenceKind.Boolean),
        new("same_call", ReferenceKind.Boolean),
        new("member", ReferenceKind.Unit | ReferenceKind.Class),
        new("level_max", ReferenceKind.Number),
        new("exp", ReferenceKind.Number),
        new("exp_mul", ReferenceKind.Number),
        new("exp_max", ReferenceKind.Number),
        new("line", ReferenceKind.Special) { specialStringArray = new[] { "back", "front", } },
        new("image", ReferenceKind.imagedata),
        new("sub_image", ReferenceKind.imagedata),
        new("politics", ReferenceKind.Special) { specialStringArray = new[] { "on", "fix", "erase", "unique", } },
        new("element_lost", ReferenceKind.Boolean),
        new("fkey", ReferenceKind.ClassTypeWriter),
        new("friend", ReferenceKind.SpecialLate),
        new("merce", ReferenceKind.Unit | ReferenceKind.Class),
        new("consti", ReferenceKind.AttributeType),
        new("multi", ReferenceKind.Status | ReferenceKind.Number),
        new("lost_corpse", ReferenceKind.Number),
        new("add_vassal", ReferenceKind.Special) { specialStringArray = new[] { "off", "on", "roam", } },
        new("value", ReferenceKind.Number),
        new("break", ReferenceKind.Skill),
        new("scream", ReferenceKind.sound),
        new("skill", ReferenceKind.Skill | ReferenceKind.Skillset),
        new("skill2", ReferenceKind.Skill | ReferenceKind.Skillset),
        new("learn", ReferenceKind.Skill | ReferenceKind.Skillset),
        new("delskill", ReferenceKind.Skill | ReferenceKind.Skillset),
        new("delskill2", ReferenceKind.Skill | ReferenceKind.Skillset),
        new("item", ReferenceKind.Skill),
    };

    internal static readonly ElementInfo[] Unit = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(UnitFallthroughClass, new ElementInfo[]
    {
        new("talent", ReferenceKind.Boolean),
        new("class", ReferenceKind.Class),
        new("bgm", ReferenceKind.bgm),
        new("volume", ReferenceKind.Number),
        new("alive_per", ReferenceKind.Number),
        new("leader_skill", ReferenceKind.Skill | ReferenceKind.Skillset),
        new("assist_skill", ReferenceKind.Skill | ReferenceKind.Skillset),
        new("yabo", ReferenceKind.Number),
        new("kosen", ReferenceKind.Number),
        new("align", ReferenceKind.Number),
        new("loyal", ReferenceKind.Unit),
        new("power_name", ReferenceKind.Text),
        new("enemy", ReferenceKind.Unit),
        new("flag", ReferenceKind.flag),
        new("staff", ReferenceKind.Race | ReferenceKind.Class),
        new("diplomacy", ReferenceKind.Boolean),
        new("castle_guard", ReferenceKind.Unit | ReferenceKind.Class),
        new("actor", ReferenceKind.Boolean),
        new("enable", ReferenceKind.Number),
        new("enable_select", ReferenceKind.Boolean),
        new("enable_max", ReferenceKind.Number),
        new("fix", ReferenceKind.Special) { specialStringArray = new[] { "off", "on", "home", } },
        new("home", ReferenceKind.Spot),
        new("noremove_unit", ReferenceKind.Boolean),
        new("noemploy_unit", ReferenceKind.Boolean),
        new("noitem_unit", ReferenceKind.Boolean),
        new("arbeit", ReferenceKind.Special) { specialStringArray = new[] { "off", "on", "power", "fix", } },
        new("arbeit_capacity", ReferenceKind.Number),
        new("join", ReferenceKind.Text),
        new("dead", ReferenceKind.Text),
        new("retreat", ReferenceKind.Text),
        new("voice_type", ReferenceKind.VoiceTypeWriter),
    }));

    internal static readonly ElementInfo[] Class = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(UnitFallthroughClass, new ElementInfo[]
    {
        new("image2", ReferenceKind.imagedata),
        new("sub_image2", ReferenceKind.imagedata),
        new("unique", ReferenceKind.Boolean),
        new("same_sex", ReferenceKind.Boolean),
        new("change", ReferenceKind.Class),
        new("friend_ex", ReferenceKind.Race | ReferenceKind.Class | ReferenceKind.Unit),
        // new("samecall_baseup"),
    }));

    internal static readonly ElementInfo[] Power = new ElementInfo[]
    {
        new("castle_battle", ReferenceKind.Boolean),
        new("event", ReferenceKind.Boolean),
        new("name", ReferenceKind.Text),
        new("help", ReferenceKind.Text),
        new("master", ReferenceKind.Unit),
        new("flag", ReferenceKind.flag),
        new("bgm", ReferenceKind.bgm),
        new("volume", ReferenceKind.Number),
        new("diplomacy", ReferenceKind.Boolean),
        new("enable_select", ReferenceKind.Boolean),
        new("enable_talent", ReferenceKind.Boolean),
        new("free_raise", ReferenceKind.Boolean),
        new("money", ReferenceKind.Number),
        new("home", ReferenceKind.Spot),
        new("fix", ReferenceKind.Special) { specialStringArray = new[] { "off", "on", "home", "hold", "warlike", "freeze", } },
        new("diplo", ReferenceKind.Power),
        new("league", ReferenceKind.Power),
        new("enemy_power", ReferenceKind.Power),
        new("staff", ReferenceKind.Unit | ReferenceKind.Class | ReferenceKind.Race),
        new("merce", ReferenceKind.Unit | ReferenceKind.Class | ReferenceKind.Race),
        new("training_average", ReferenceKind.Number),
        new("base_merits", ReferenceKind.Number),
        new("merits", ReferenceKind.Unit),
        new("base_loyal", ReferenceKind.Number),
        new("loyals", ReferenceKind.Unit),
        new("head", ReferenceKind.Text),
        new("head2", ReferenceKind.Text),
        new("head3", ReferenceKind.Text),
        new("head4", ReferenceKind.Text),
        new("head5", ReferenceKind.Text),
        new("head6", ReferenceKind.Text),
        new("diff", ReferenceKind.Text),
        new("yabo", ReferenceKind.Number),
        new("kosen", ReferenceKind.Number),
        new("text", ReferenceKind.Text),
        new("member", ReferenceKind.Spot),
        new("friend", ReferenceKind.Scenario),
        new("master2", ReferenceKind.Text),
        new("master3", ReferenceKind.Text),
        new("master4", ReferenceKind.Text),
        new("master5", ReferenceKind.Text),
        new("master6", ReferenceKind.Text),
        new("enable"),
        new("training_up", ReferenceKind.Number),
    };

    internal static readonly ElementInfo[] Scenario = new ElementInfo[]
    {
        new("ws_red", ReferenceKind.Number),
        new("ws_blue", ReferenceKind.Number),
        new("ws_green", ReferenceKind.Number),
        new("ws_alpha", ReferenceKind.Number),
        new("ws_light", ReferenceKind.Number),
        new("ws_light_range", ReferenceKind.Number),
        new("discus"),
        new("save_name", ReferenceKind.Text),
        new("enable_select", ReferenceKind.Boolean),
        new("max_unit", ReferenceKind.Number),
        new("blind", ReferenceKind.Number),
        new("name", ReferenceKind.Text),
        new("map", ReferenceKind.image_file),
        new("help", ReferenceKind.Text),
        new("locate_x", ReferenceKind.Number),
        new("locate_y", ReferenceKind.Number),
        new("begin_text", ReferenceKind.Text),
        new("world", ReferenceKind.Event),
        new("fight", ReferenceKind.Event),
        new("politics", ReferenceKind.Event),
        new("war_capacity", ReferenceKind.Number),
        new("spot_capacity", ReferenceKind.Number),
        new("gain_per", ReferenceKind.Number),
        new("support_range", ReferenceKind.Number),
        new("my_range", ReferenceKind.Number),
        new("myhelp_range", ReferenceKind.Number),
        new("base_level", ReferenceKind.Number),
        new("monster_level", ReferenceKind.Number),
        new("training_up", ReferenceKind.Number),
        new("actor_per", ReferenceKind.Number),
        new("sortkey", ReferenceKind.Number),
        new("default_ending", ReferenceKind.Boolean),
        new("power_order", ReferenceKind.Special) { specialStringArray = new[] { "normal", "test", "dash", } },
        new("enable", ReferenceKind.Boolean),
        new("enable_talent", ReferenceKind.Boolean),
        new("party", ReferenceKind.Boolean),
        new("no_autosave", ReferenceKind.Boolean),
        new("zone"),
        new("nozone", ReferenceKind.Boolean),
        new("item0", ReferenceKind.Boolean | ReferenceKind.Number),
        new("item1", ReferenceKind.Boolean | ReferenceKind.Number),
        new("item2", ReferenceKind.Boolean | ReferenceKind.Number),
        new("item3", ReferenceKind.Boolean | ReferenceKind.Number),
        new("item4", ReferenceKind.Boolean | ReferenceKind.Number),
        new("item5", ReferenceKind.Boolean | ReferenceKind.Number),
        new("item6", ReferenceKind.Boolean | ReferenceKind.Number),
        new("item_limit", ReferenceKind.Boolean),
        new("poli"),
        new("camp"),
        new("multi"),
        new("item"),
        new("item_sale", ReferenceKind.Skill),
        new("item_hold", ReferenceKind.Skill),
        new("text", ReferenceKind.Text),
        new("roam", ReferenceKind.Unit),
        new("spot", ReferenceKind.Spot),
        new("power", ReferenceKind.Power),
        new("offset"),
    };

    public static readonly ElementInfo[] Discards = new ElementInfo[] {
        new("str"),
        new("fkey"),
        new("loyal"),
        new("brave"),
        new("arbeit"),
        new("change"),
        new("ground"),
        new("gun_delay"),
        new("text"),
        new("icon"),
        new("wave"),
        new("cutin"),
        new("diplo"),
        new("consti"),
        new("league"),
        new("loyals"),
        new("merits"),
        new("yorozu"),
        new("enemy_power"),
        new("leader_skill"),
        new("assist_skill"),
        new("roam"),
        new("spot"),
        new("power"),
        new("add2"),
        new("item"),
        new("merce"),
        new("next2"),
        new("next3"),
        new("sound"),
        new("member"),
        new("monster"),
        new("item_hold"),
        new("item_sale"),
        new("just_next"),
        new("castle_guard"),
        new("enemy"),
        new("staff"),
        new("friend"),
        new("offset"),
        new("delskill"),
        new("delskill2"),
        new("voice_type"),
        new("ray"),
        new("poli"),
        new("camp"),
        new("home"),
        new("multi"),
        new("learn"),
        new("color"),
        new("joint"),
        new("skill"),
        new("skill2"),
        new("weapon"),
        new("weapon2"),
        new("activenum"),
        new("friend_ex"),
    };
}
