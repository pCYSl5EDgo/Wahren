﻿namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct ElementInfo
{
    public string name;
    public string variantType;
    public ReferenceKind referenceKind;

    public ElementInfo(string name, string variantType = nameof(Scenario), ReferenceKind referenceKind = ReferenceKind.Unknown)
    {
        this.name = name;
        this.variantType = variantType;
        this.referenceKind = referenceKind;
    }

    public static bool IsAllScenarioVariant(string name) => name switch
    {
        nameof(Dungeon) or nameof(Skill) => false,
        _ => true,
    };

    public static ElementInfo[] Get(string name)
    {
        switch (name)
        {
            case nameof(Power): return Power;
            case nameof(Class): return Class;
            case nameof(Dungeon): return Dungeon;
            case nameof(Field): return Field;
            case nameof(Movetype): return Movetype;
            case nameof(Object): return Object;
            case nameof(Race): return Race;
            case nameof(Skill): return Skill;
            case nameof(Skillset): return Skillset;
            case nameof(Spot): return Spot;
            case nameof(Unit): return Unit;
            case nameof(Scenario): return Scenario;
            case nameof(Event): return Event;
            case nameof(Story): return Story;
            case nameof(Voice): return Voice;
            default: return System.Array.Empty<ElementInfo>();
        }
    }

    internal static readonly ElementInfo[] Voice = new ElementInfo[]
    {
        new("voice_type", referenceKind: ReferenceKind.VoiceTypeReader),
        new("delskill", referenceKind: ReferenceKind.VoiceTypeReader),
        new("spot", referenceKind: ReferenceKind.Text),
        new("roam", referenceKind: ReferenceKind.Text),
        new("power", referenceKind: ReferenceKind.Text),
    };

    internal static readonly ElementInfo[] Workspace = System.Array.Empty<ElementInfo>();

    internal static readonly ElementInfo[] Story = new ElementInfo[]
    {
        new("friend", referenceKind: ReferenceKind.Scenario),
        new("fight", referenceKind: ReferenceKind.Boolean),
    };

    internal static readonly ElementInfo[] Event = new ElementInfo[]
    {
        new("disperse", referenceKind: ReferenceKind.Boolean),
        new("castle"),
        new("castle_battle"),
        new("blind"),
        new("w", referenceKind: ReferenceKind.Number),
        new("h", referenceKind: ReferenceKind.Number),
        new("bg"),
        new("bcg"),
        new("bgm"),
        new("map"),
        new("name", referenceKind: ReferenceKind.Text),
        new("size"),
        new("color"),
        new("block"),
        new("limit"),
        new("title", referenceKind: ReferenceKind.Text),
        new("center"),
        new("italic"),
        new("handle", referenceKind: ReferenceKind.Special),
        new("member"),
        new("second"),
        new("volume"),
        new("bg_fade"),
        new("dark_fade"),
        new("dark_alpha"),
        new("bg_interval"),
        new("dark_fade_e"),
        new("last_second"),
    };

    internal static readonly ElementInfo[] Dungeon = new ElementInfo[]
    {
        new("name", referenceKind: ReferenceKind.Text),
        new("max"),
        new("move_speed", referenceKind: ReferenceKind.Number),
        new("prefix", referenceKind: ReferenceKind.Text),
        new("suffix", referenceKind: ReferenceKind.Text),
        new("lv_adjust"),
        new("open"),
        new("limit", referenceKind: ReferenceKind.Number),
        new("bgm"),
        new("volume"),
        new("blind"),
        new("base_level"),
        new("color"),
        new("map"),
        new("floor"),
        new("wall"),
        new("start"),
        new("goal"),
        new("monster"),
        new("monster_num"),
        new("box"),
        new("item"),
        new("item_num"),
        new("item_text"),
        new("home"),
        new("ray"),
    };

    internal static readonly ElementInfo[] Field = new ElementInfo[]
    {
        new("type"),
        new("attr", referenceKind: ReferenceKind.FieldAttributeTypeWriter),
        new("color", referenceKind: ReferenceKind.Number),
        new("id", referenceKind: ReferenceKind.FieldIdWriter),
        new("edge", referenceKind: ReferenceKind.Boolean),
        new("joint"),
        new("image"),
        new("add2"),
        new("member"),
        new("alt", referenceKind: ReferenceKind.Number),
        new("alt_max", referenceKind: ReferenceKind.Number),
        new("smooth"),
    };

    internal static readonly ElementInfo[] Movetype = new ElementInfo[]
    {
        new("name", referenceKind: ReferenceKind.Text),
        new("help", referenceKind: ReferenceKind.Text),
        new("consti", referenceKind: ReferenceKind.FieldAttributeTypeReader),
    };

    internal static readonly ElementInfo[] Object = new ElementInfo[]
    {
        new("skill"),
        new("front"),
        new("width"),
        new("height"),
        new("alpha"),
        new("type"),
        new("breakfire"),
        new("land_base", referenceKind: ReferenceKind.Boolean | ReferenceKind.Number),
        new("no_stop", referenceKind: ReferenceKind.Boolean),
        new("no_wall2", referenceKind: ReferenceKind.Number),
        new("no_arc_hit", referenceKind: ReferenceKind.Boolean),
        new("radius", referenceKind: ReferenceKind.Number),
        new("blk", referenceKind: ReferenceKind.Boolean),
        new("w", referenceKind: ReferenceKind.Number),
        new("h", referenceKind: ReferenceKind.Number),
        new("a", referenceKind: ReferenceKind.Number),
        new("image"),
        new("image2"),
        new("image2_w", referenceKind: ReferenceKind.Number),
        new("image2_h", referenceKind: ReferenceKind.Number),
        new("image2_a", referenceKind: ReferenceKind.Number),
        new("member"),
        new("ground", referenceKind: ReferenceKind.Boolean),
    };

    internal static readonly ElementInfo[] Race = new ElementInfo[]
    {
        new("name", referenceKind: ReferenceKind.Text),
        new("align", referenceKind: ReferenceKind.Number),
        new("brave", referenceKind: ReferenceKind.Number),
        new("consti", referenceKind: ReferenceKind.AttributeType),
        new("movetype", referenceKind: ReferenceKind.Movetype),
    };

    internal static readonly ElementInfo[] Skill = new ElementInfo[]
    {
        new("force_ray", referenceKind: ReferenceKind.Boolean),
        new("bright", referenceKind: ReferenceKind.Boolean),
        new("func"),
        new("name", referenceKind: ReferenceKind.Text),
        new("icon"),
        new("fkey"),
        new("sortkey", referenceKind: ReferenceKind.Number),
        new("special", referenceKind: ReferenceKind.Boolean | ReferenceKind.Number),
        new("delay"),
        new("gun_delay"),
        new("quickreload"),
        new("help", referenceKind: ReferenceKind.Text),
        new("hide_help"),
        new("sound"),
        new("msg", variantType: "Unit", referenceKind: ReferenceKind.Text),
        new("picture", variantType: "Unit"),
        new("cutin"),
        new("value", referenceKind: ReferenceKind.Number),
        new("talent"),
        new("exp_per", referenceKind: ReferenceKind.Number),
        new("movetype"),
        new("type"),
        new("color", referenceKind: ReferenceKind.Number),
        new("w", referenceKind: ReferenceKind.Number),
        new("h", referenceKind: ReferenceKind.Number),
        new("a", referenceKind: ReferenceKind.Number),
        new("mp", referenceKind: ReferenceKind.Number),
        new("image"),
        new("alpha_tip"),
        new("alpha_butt"),
        new("anime", referenceKind: ReferenceKind.Number),
        new("anime_interval", referenceKind: ReferenceKind.Number),
        new("center"),
        new("ground"),
        new("d360"),
        new("d360_adj"),
        new("rotate"),
        new("direct"),
        new("resize_interval", referenceKind: ReferenceKind.Number),
        new("resize_start", referenceKind: ReferenceKind.Number),
        new("resize_reverse", referenceKind: ReferenceKind.Number),
        new("resize_w", referenceKind: ReferenceKind.Number),
        new("resize_w_start", referenceKind: ReferenceKind.Number),
        new("resize_w_max", referenceKind: ReferenceKind.Number),
        new("resize_w_min", referenceKind: ReferenceKind.Number),
        new("resize_h_min", referenceKind: ReferenceKind.Number),
        new("resize_h_max", referenceKind: ReferenceKind.Number),
        new("resize_h_start", referenceKind: ReferenceKind.Number),
        new("resize_h", referenceKind: ReferenceKind.Number),
        new("resize_x", referenceKind: ReferenceKind.Number),
        new("resize_x_start", referenceKind: ReferenceKind.Number),
        new("resize_x_max", referenceKind: ReferenceKind.Number),
        new("resize_x_min", referenceKind: ReferenceKind.Number),
        new("resize_y_min", referenceKind: ReferenceKind.Number),
        new("resize_y_max", referenceKind: ReferenceKind.Number),
        new("resize_y_start", referenceKind: ReferenceKind.Number),
        new("resize_y", referenceKind: ReferenceKind.Number),
        new("resize_a", referenceKind: ReferenceKind.Number),
        new("resize_s", referenceKind: ReferenceKind.Number),
        new("resize_a_start", referenceKind: ReferenceKind.Number),
        new("resize_s_start", referenceKind: ReferenceKind.Number),
        new("resize_a_max", referenceKind: ReferenceKind.Number),
        new("resize_s_max", referenceKind: ReferenceKind.Number),
        new("resize_a_min", referenceKind: ReferenceKind.Number),
        new("resize_s_min", referenceKind: ReferenceKind.Number),
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
        new("start_degree", referenceKind: ReferenceKind.Number),
        new("start_degree_fix"),
        new("start_degree_turnunit"),
        new("start_degree_type"),
        new("start_random_degree", referenceKind: ReferenceKind.Number),
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
        new("name", referenceKind: ReferenceKind.Text),
        new("back"),
        new("member", referenceKind: ReferenceKind.Skill),
    };

    internal static readonly ElementInfo[] Spot = new ElementInfo[]
    {
        new("value", referenceKind: ReferenceKind.Number),
        new("politics", referenceKind: ReferenceKind.Special),
        new("merce", referenceKind: ReferenceKind.Unit | ReferenceKind.Class),
        new("name", referenceKind: ReferenceKind.Text),
        new("image"),
        new("x", referenceKind: ReferenceKind.Number),
        new("y", referenceKind: ReferenceKind.Number),
        new("w", referenceKind: ReferenceKind.Number),
        new("h", referenceKind: ReferenceKind.Number),
        new("big"),
        new("map"),
        new("castle_battle", referenceKind: ReferenceKind.Boolean),
        new("yorozu", referenceKind: ReferenceKind.Class),
        new("limit", referenceKind: ReferenceKind.Number),
        new("bgm"),
        new("volume", referenceKind: ReferenceKind.Number),
        new("gain", referenceKind: ReferenceKind.Number),
        new("castle", referenceKind: ReferenceKind.Number),
        new("capacity", referenceKind: ReferenceKind.Number),
        new("monster", referenceKind: ReferenceKind.Unit | ReferenceKind.Class),
        new("member", referenceKind: ReferenceKind.Unit | ReferenceKind.Class),
        new("dungeon", referenceKind: ReferenceKind.Dungeon),
        new("no_home", referenceKind: ReferenceKind.Boolean),
        new("no_raise", referenceKind: ReferenceKind.Boolean),
        new("castle_lot", referenceKind: ReferenceKind.Number),
        new("text", referenceKind: ReferenceKind.Text),
    };

    internal static readonly ElementInfo[] UnitFallthroughClass = new ElementInfo[]
    {
        new("troop_sort", referenceKind: ReferenceKind.Number),
        new("stealth", referenceKind: ReferenceKind.Number),
        new("free_move", referenceKind: ReferenceKind.Number),
        new("name", referenceKind: ReferenceKind.Text),
        new("help", referenceKind: ReferenceKind.Text),
        new("sex", referenceKind: ReferenceKind.Special),
        new("a", referenceKind: ReferenceKind.Number),
        new("h", referenceKind: ReferenceKind.Number),
        new("w", referenceKind: ReferenceKind.Number),
        new("text", referenceKind: ReferenceKind.Text),
        new("sub_image_even", referenceKind: ReferenceKind.Boolean),
        new("yorozu", referenceKind: ReferenceKind.Special),
        new("radius", referenceKind: ReferenceKind.Number),
        new("radius_press", referenceKind: ReferenceKind.Number),
        new("no_escape", referenceKind: ReferenceKind.Boolean),
        new("no_regular", referenceKind: ReferenceKind.Boolean),
        new("no_knock", referenceKind: ReferenceKind.Boolean | ReferenceKind.Number),
        new("no_cover", referenceKind: ReferenceKind.Boolean),
        new("dead_event", referenceKind: ReferenceKind.Event),
        new("beast_unit", referenceKind: ReferenceKind.Boolean),
        new("summon_max", referenceKind: ReferenceKind.Number),
        new("summon_level", referenceKind: ReferenceKind.Number),
        new("attack_range", referenceKind: ReferenceKind.Number),
        new("escape_range", referenceKind: ReferenceKind.Number),
        new("escape_run", referenceKind: ReferenceKind.Number),
        new("hand_range", referenceKind: ReferenceKind.Number),
        new("wake_range", referenceKind: ReferenceKind.Number),
        new("view_range", referenceKind: ReferenceKind.Number),
        new("cavalry_range", referenceKind: ReferenceKind.Number),
        new("view_unit", referenceKind: ReferenceKind.Boolean),
        new("force_view_unit", referenceKind: ReferenceKind.Boolean),
        new("force_blind_unit", referenceKind: ReferenceKind.Boolean),
        new("satellite", referenceKind: ReferenceKind.Boolean | ReferenceKind.Number),
        new("hasexp", referenceKind: ReferenceKind.Number),
        new("brave", referenceKind: ReferenceKind.Boolean | ReferenceKind.Number),
        new("hp", referenceKind: ReferenceKind.Number),
        new("mp", referenceKind: ReferenceKind.Number),
        new("attack", referenceKind: ReferenceKind.Number),
        new("defense", referenceKind: ReferenceKind.Number),
        new("magic", referenceKind: ReferenceKind.Number),
        new("magdef", referenceKind: ReferenceKind.Number),
        new("speed", referenceKind: ReferenceKind.Number),
        new("dext", referenceKind: ReferenceKind.Number),
        new("move", referenceKind: ReferenceKind.Number),
        new("hprec", referenceKind: ReferenceKind.Number),
        new("mprec", referenceKind: ReferenceKind.Number),
        new("heal_max", referenceKind: ReferenceKind.Number),
        new("attack_max", referenceKind: ReferenceKind.Number),
        new("defense_max", referenceKind: ReferenceKind.Number),
        new("magic_max", referenceKind: ReferenceKind.Number),
        new("magdef_max", referenceKind: ReferenceKind.Number),
        new("speed_max", referenceKind: ReferenceKind.Number),
        new("dext_max", referenceKind: ReferenceKind.Number),
        new("move_max", referenceKind: ReferenceKind.Number),
        new("hprec_max", referenceKind: ReferenceKind.Number),
        new("mprec_max", referenceKind: ReferenceKind.Number),
        new("movetype", referenceKind: ReferenceKind.Movetype),
        new("hpUp", referenceKind: ReferenceKind.Number),
        new("mpUp", referenceKind: ReferenceKind.Number),
        new("attackUp", referenceKind: ReferenceKind.Number),
        new("defenseUp", referenceKind: ReferenceKind.Number),
        new("magicUp", referenceKind: ReferenceKind.Number),
        new("magdefUp", referenceKind: ReferenceKind.Number),
        new("speedUp", referenceKind: ReferenceKind.Number),
        new("dextUp", referenceKind: ReferenceKind.Number),
        new("moveUp", referenceKind: ReferenceKind.Number),
        new("hprecUp", referenceKind: ReferenceKind.Number),
        new("mprecUp", referenceKind: ReferenceKind.Number),
        new("hpMax", referenceKind: ReferenceKind.Number),
        new("mpMax", referenceKind: ReferenceKind.Number),
        new("attackMax", referenceKind: ReferenceKind.Number),
        new("defenseMax", referenceKind: ReferenceKind.Number),
        new("magicMax", referenceKind: ReferenceKind.Number),
        new("magdefMax", referenceKind: ReferenceKind.Number),
        new("speedMax", referenceKind: ReferenceKind.Number),
        new("dextMax", referenceKind: ReferenceKind.Number),
        new("moveMax", referenceKind: ReferenceKind.Number),
        new("hprecMax", referenceKind: ReferenceKind.Number),
        new("mprecMax", referenceKind: ReferenceKind.Number),
        new("race", referenceKind: ReferenceKind.Race),
        new("sortkey",  referenceKind: ReferenceKind.Number),
        new("picture"),
        new("picture@cutin"),
        new("picture_detail", referenceKind: ReferenceKind.Boolean),
        new("picture_menu",  referenceKind: ReferenceKind.Number),
        new("picture_floor", referenceKind: ReferenceKind.Special),
        new("picture_shift",  referenceKind: ReferenceKind.Number),
        new("picture_shift_up",  referenceKind: ReferenceKind.Number),
        new("picture_center",  referenceKind: ReferenceKind.Number),
        new("picture_back"),
        new("price", referenceKind: ReferenceKind.Number),
        new("cost", referenceKind: ReferenceKind.Number),
        new("finance", referenceKind: ReferenceKind.Number),
        new("tkool", referenceKind: ReferenceKind.Boolean),
        new("keep_form", referenceKind: ReferenceKind.Boolean | ReferenceKind.Number),
        new("breast_width", referenceKind: ReferenceKind.Number),
        new("medical", referenceKind: ReferenceKind.Number),
        new("active"),
        new("activenum"),
        new("handle", referenceKind: ReferenceKind.Boolean),
        new("red", referenceKind: ReferenceKind.Boolean),
        new("sub_image"),
        new("rank_text", referenceKind: ReferenceKind.Text),
        new("no_training", referenceKind: ReferenceKind.Boolean),
        new("force_voice", referenceKind: ReferenceKind.Boolean),
        new("face"),
        new("same_friend", referenceKind: ReferenceKind.Boolean),
        new("same_call", referenceKind: ReferenceKind.Boolean),
        new("member", referenceKind: ReferenceKind.Unit | ReferenceKind.Class),
        new("level_max", referenceKind: ReferenceKind.Number),
        new("exp", referenceKind: ReferenceKind.Number),
        new("exp_mul", referenceKind: ReferenceKind.Number),
        new("exp_max", referenceKind: ReferenceKind.Number),
        new("line", referenceKind: ReferenceKind.Special),
        new("image"),
        new("politics", referenceKind: ReferenceKind.Special),
        new("element_lost", referenceKind: ReferenceKind.Boolean),
        new("fkey"),
        new("friend"),
        new("merce", referenceKind: ReferenceKind.Unit | ReferenceKind.Class),
        new("consti", referenceKind: ReferenceKind.AttributeType),
        new("multi", referenceKind: ReferenceKind.Status),
        new("lost_corpse", referenceKind: ReferenceKind.Number),
        new("add_vassal", referenceKind: ReferenceKind.Special),
        new("value", referenceKind: ReferenceKind.Number),
        new("break", referenceKind: ReferenceKind.Skill),
        new("scream"),
        new("skill", referenceKind: ReferenceKind.Skill | ReferenceKind.Skillset),
        new("skill2", referenceKind: ReferenceKind.Skill | ReferenceKind.Skillset),
        new("learn", referenceKind: ReferenceKind.Skill | ReferenceKind.Skillset),
        new("delskill", referenceKind: ReferenceKind.Skill | ReferenceKind.Skillset),
        new("delskill2", referenceKind: ReferenceKind.Skill | ReferenceKind.Skillset),
        new("item", referenceKind: ReferenceKind.Skill),
    };

    internal static readonly ElementInfo[] Unit = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(UnitFallthroughClass, new ElementInfo[]
    {
        new("talent", referenceKind: ReferenceKind.Boolean),
        new("class", referenceKind: ReferenceKind.Class),
        new("bgm"),
        new("volume"),
        new("alive_per", referenceKind: ReferenceKind.Number),
        new("level", referenceKind: ReferenceKind.Number),
        new("leader_skill", referenceKind: ReferenceKind.Skill | ReferenceKind.Skillset),
        new("assist_skill", referenceKind: ReferenceKind.Skill | ReferenceKind.Skillset),
        new("yabo", referenceKind: ReferenceKind.Number),
        new("kosen", referenceKind: ReferenceKind.Number),
        new("align", referenceKind: ReferenceKind.Number),
        new("loyal", referenceKind: ReferenceKind.Unit),
        new("power_name", referenceKind: ReferenceKind.Text),
        new("enemy", referenceKind: ReferenceKind.Unit),
        new("flag"),
        new("staff"),
        new("diplomacy", referenceKind: ReferenceKind.Boolean),
        new("castle_guard"),
        new("actor", referenceKind: ReferenceKind.Boolean),
        new("enable", referenceKind: ReferenceKind.Number),
        new("enable_select", referenceKind: ReferenceKind.Boolean),
        new("enable_max", referenceKind: ReferenceKind.Number),
        new("fix", referenceKind: ReferenceKind.Special),
        new("home", referenceKind: ReferenceKind.Spot),
        new("noremove_unit", referenceKind: ReferenceKind.Boolean),
        new("noemploy_unit", referenceKind: ReferenceKind.Boolean),
        new("noitem_unit", referenceKind: ReferenceKind.Boolean),
        new("arbeit", referenceKind: ReferenceKind.Special),
        new("arbeit_capacity", referenceKind: ReferenceKind.Number),
        new("join", referenceKind: ReferenceKind.Text),
        new("dead", referenceKind: ReferenceKind.Text),
        new("retreat", referenceKind: ReferenceKind.Text),
        new("voice_type", referenceKind: ReferenceKind.VoiceTypeWriter),
    }));

    internal static readonly ElementInfo[] Class = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(UnitFallthroughClass, new ElementInfo[]
    {
        new("image2"),
        new("unique"),
        new("same_sex"),
        new("change"),
        new("friend_ex"),
        new("sub_image2"),
        // new("samecall_baseup"),
    }));

    internal static readonly ElementInfo[] Power = new ElementInfo[]
    {
        new("castle_battle"),
        new("event"),
        new("name", referenceKind: ReferenceKind.Text),
        new("help", referenceKind: ReferenceKind.Text),
        new("master", referenceKind: ReferenceKind.Unit),
        new("flag"),
        new("bgm"),
        new("volume"),
        new("diplomacy"),
        new("enable_select"),
        new("enable_talent"),
        new("free_raise"),
        new("money", referenceKind: ReferenceKind.Number),
        new("home", referenceKind: ReferenceKind.Spot),
        new("fix"),
        new("diplo"),
        new("league"),
        new("enemy_power"),
        new("staff"),
        new("merce"),
        new("training_average", referenceKind: ReferenceKind.Number),
        new("base_merits"),
        new("merits"),
        new("base_loyal", referenceKind: ReferenceKind.Number),
        new("loyals"),
        new("head"),
        new("head2"),
        new("head3"),
        new("head4"),
        new("head5"),
        new("head6"),
        new("diff"),
        new("yabo", referenceKind: ReferenceKind.Number),
        new("kosen", referenceKind: ReferenceKind.Number),
        new("text", referenceKind: ReferenceKind.Text),
        new("member", referenceKind: ReferenceKind.Spot),
        new("friend"),
        new("master2"),
        new("master3"),
        new("master4"),
        new("master5"),
        new("master6"),
        new("enable"),
        new("training_up", referenceKind: ReferenceKind.Number),
    };

    internal static readonly ElementInfo[] Scenario = new ElementInfo[]
    {
        new("ws_red", referenceKind: ReferenceKind.Number),
        new("ws_blue", referenceKind: ReferenceKind.Number),
        new("ws_green", referenceKind: ReferenceKind.Number),
        new("ws_alpha", referenceKind: ReferenceKind.Number),
        new("ws_light", referenceKind: ReferenceKind.Number),
        new("ws_light_range", referenceKind: ReferenceKind.Number),
        new("discus"),
        new("save_name"),
        new("enable_select"),
        new("max_unit", referenceKind: ReferenceKind.Number),
        new("blind"),
        new("name", referenceKind: ReferenceKind.Text),
        new("map"),
        new("help", referenceKind: ReferenceKind.Text),
        new("locate_x", referenceKind: ReferenceKind.Number),
        new("locate_y", referenceKind: ReferenceKind.Number),
        new("begin_text", referenceKind: ReferenceKind.Text),
        new("world", referenceKind: ReferenceKind.Event),
        new("fight", referenceKind: ReferenceKind.Event),
        new("politics", referenceKind: ReferenceKind.Event),
        new("war_capacity", referenceKind: ReferenceKind.Number),
        new("spot_capacity", referenceKind: ReferenceKind.Number),
        new("gain_per", referenceKind: ReferenceKind.Number),
        new("support_range", referenceKind: ReferenceKind.Number),
        new("my_range", referenceKind: ReferenceKind.Number),
        new("myhelp_range", referenceKind: ReferenceKind.Number),
        new("base_level", referenceKind: ReferenceKind.Number),
        new("monster_level", referenceKind: ReferenceKind.Number),
        new("training_up", referenceKind: ReferenceKind.Number),
        new("actor_per", referenceKind: ReferenceKind.Number),
        new("sortkey", referenceKind: ReferenceKind.Number),
        new("default_ending"),
        new("power_order"),
        new("enable"),
        new("enable_talent"),
        new("party"),
        new("no_autosave"),
        new("zone"),
        new("nozone"),
        new("item0"),
        new("item1"),
        new("item2"),
        new("item3"),
        new("item4"),
        new("item5"),
        new("item6"),
        new("item_limit"),
        new("poli"),
        new("camp"),
        new("multi"),
        new("item"),
        new("item_sale"),
        new("item_hold"),
        new("text", referenceKind: ReferenceKind.Text),
        new("roam", referenceKind: ReferenceKind.Unit),
        new("spot", referenceKind: ReferenceKind.Spot),
        new("power", referenceKind: ReferenceKind.Power),
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
