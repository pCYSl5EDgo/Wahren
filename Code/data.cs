using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Wahren
{
    public enum StructureDataType
    {
        Attribute, Context, Detail, Sound, Workspace,
        Class, Power, Spot, Story, Unit,
        Dungeon, Event, Field, Movetype, World,
        Fight, Function, Deploy,
        Object, Race, Scenario, Skill, Skillset, Voice
    }
    public abstract class BaseData : IName, IDebugInfo
    {
        private string _file;

        public string File { get => _file; set => _file = String.Intern(value); }
        public int Line { get; set; }
        public string Name { get; set; }
        //継承のためのもの
        //どの項目がnull(@)に設定されたかを示す
        public List<string> FilledWithNull { get; } = new List<string>();

        protected BaseData(string name, string file, int line)
        {
            Name = name?.ToLower() ?? "";
            File = file;
            Line = line;
        }
        public string DebugInfo => File + '/' + (Line + 1);
    }

    public abstract class InheritData : BaseData, IInherit
    {
        public string Inherit { get; set; }
        protected InheritData(string name, string inherit, string file, int line) : base(name, file, line) { Inherit = inherit?.ToLower() ?? ""; }
    }

    public class ScenarioVariantData : InheritData
    {
        //scenario, 項目, データ
        public Dictionary<string, Dictionary<string, LexicalTree_Assign>> VariantData { get; } = new Dictionary<string, Dictionary<string, LexicalTree_Assign>>();
        protected ScenarioVariantData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
    }

    public class AttributeData : ConcurrentDictionary<string, Tuple<string, int>>
    {
        public AttributeData()
        {
            this["poi"] = new Tuple<string, int>("毒", 10);
            this["para"] = new Tuple<string, int>("麻痺", 10);
            this["ill"] = new Tuple<string, int>("幻覚", 10);
            this["sil"] = new Tuple<string, int>("沈黙", 10);
            this["conf"] = new Tuple<string, int>("混乱", 10);
            this["stone"] = new Tuple<string, int>("石化", 10);
            this["fear"] = new Tuple<string, int>("恐慌", 10);
            this["suck"] = new Tuple<string, int>("吸血", 10);
            this["magsuck"] = new Tuple<string, int>("魔吸", 10);
            this["drain"] = new Tuple<string, int>("ﾄﾞﾚｲﾝ", 10);
            this["death"] = new Tuple<string, int>("即死", 10);
            this["wall"] = new Tuple<string, int>("城壁", 10);
        }
        public new bool Clear()
        {
            base.Clear();
            this["poi"] = new Tuple<string, int>("毒", 10);
            this["para"] = new Tuple<string, int>("麻痺", 10);
            this["ill"] = new Tuple<string, int>("幻覚", 10);
            this["sil"] = new Tuple<string, int>("沈黙", 10);
            this["conf"] = new Tuple<string, int>("混乱", 10);
            this["stone"] = new Tuple<string, int>("石化", 10);
            this["fear"] = new Tuple<string, int>("恐慌", 10);
            this["suck"] = new Tuple<string, int>("吸血", 10);
            this["magsuck"] = new Tuple<string, int>("魔吸", 10);
            this["drain"] = new Tuple<string, int>("ﾄﾞﾚｲﾝ", 10);
            this["death"] = new Tuple<string, int>("即死", 10);
            this["wall"] = new Tuple<string, int>("城壁", 10);
            return true;
        }
    }
    public class WorkspaceData : ConcurrentDictionary<string, string>
    { }
    public sealed class VoiceData : InheritData
    {
        public VoiceData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public List<string> VoiceType { get; } = new List<string>();
        public List<string> DeleteVoiceType { get; } = new List<string>();
        public List<string> SpotVoice { get; } = new List<string>();
        public List<string> PowerVoice { get; } = new List<string>();
        public bool NoPower { get; set; }
    }
    public class SpotData : ScenarioVariantData
    {
        public SpotData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public SpotData() : base("", "", "", 0) { }
        public string DisplayName { get; set; }
        public string Image { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string Map { get; set; }

        public bool? IsCastleBattle { get; set; } = false;

        public List<string> Yorozu { get; } = new List<string>();

        public int? Limit { get; set; }

        public string BGM { get; set; }
        public byte? Volume { get; set; }

        public int? Gain { get; set; }
        public int? Castle { get; set; }
        public byte? Capacity { get; set; }

        public List<string> Members { get; } = new List<string>();

        public List<string> Monsters { get; } = new List<string>();

        public List<string> Merce { get; } = new List<string>();

        public string Dungeon { get; set; }

        public bool? IsNotHome { get; set; }
        public bool? IsNotRaisableSpot { get; set; }
        public int? CastleLot { get; set; }
        public bool? Politics { get; set; }
    }
    public class SoundData : ConcurrentDictionary<string, int>
    {
        public int Default { get; set; } = -2500;
        public new bool Clear()
        {
            base.Clear();
            Default = -2500;
            return true;
        }
    }
    public class SkillSetData : ScenarioVariantData
    {
        public SkillSetData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public string DisplayName { get; set; }
        public List<string> MemberSkill { get; } = new List<string>();
        public string BackIconPath { get; set; }
    }
    public class RaceData : ScenarioVariantData
    {
        public RaceData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public string DisplayName { get; set; }
        public byte? Align { get; set; }
        public byte? Brave { get; set; }
        public Dictionary<string, byte> Consti { get; } = new Dictionary<string, byte>();
        public string MoveType { get; set; }
    }
    public class PowerData : ScenarioVariantData
    {
        public PowerData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public PowerData() : base("", "", "", 0) { }
        public string DisplayName { get; set; }
        public string FlagPath { get; set; }
        public string Master { get; set; }

        public string BGM { get; set; }
        public byte? Volume { get; set; }
        public bool? DoDiplomacy { get; set; }
        public bool? IsSelectable { get; set; }
        public bool? IsPlayableAsTalent { get; set; }
        public bool? IsRaisable { get; set; }

        public int? Money { get; set; }
        public List<string> HomeSpot { get; } = new List<string>();
        public enum FixType : byte { off, on, home, hold, warlike, freeze }
        public FixType? Fix { get; set; }

        public Dictionary<string, int> Diplo { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> League { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> EnemyPower { get; } = new Dictionary<string, int>();

        public List<string> Staff { get; } = new List<string>();
        public List<string> Merce { get; } = new List<string>();

        public int? TrainingAveragePercent { get; set; }
        public int? TrainingUp { get; set; }

        public int? BaseMerit { get; set; }

        public Dictionary<string, int> Merits { get; } = new Dictionary<string, int>();

        public byte? BaseLoyal { get; set; }
        public Dictionary<string, int> Loyals { get; } = new Dictionary<string, int>();

        public string Head { get; set; }
        public string Difficulty { get; set; }
        public byte? Kosen { get; set; }
        public byte? Yabo { get; set; }
        public string Description { get; set; }
        public List<string> MemberSpot { get; } = new List<string>();
        public bool? IsEvent { get; set; }
        public List<string> Friend { get; } = new List<string>();
        public string Master2 { get; set; }
        public string Master3 { get; set; }
        public string Master4 { get; set; }
        public string Master5 { get; set; }
        public string Master6 { get; set; }
        public string Head2 { get; set; }
        public string Head3 { get; set; }
        public string Head4 { get; set; }
        public string Head5 { get; set; }
        public string Head6 { get; set; }
    }
    public class ObjectData : InheritData
    {
        public ObjectData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public enum ChipType
        {
            None, coll, wall, wall2, breakable, gate, floor, start, goal, box, cover
        }
        public ChipType Type { get; set; } = ChipType.None;
        public bool? IsLandBase { get; set; }
        public int? LandBase { get; set; }
        public bool? NoStop { get; set; }
        public byte? NoWall2 { get; set; }
        public bool? NoArcHit { get; set; }
        public int? Radius { get; set; }
        public bool? Blk { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public byte? Alpha { get; set; }
        public List<string> ImageNameRandomList { get; } = new List<string>();

        public string Image2Name { get; set; }
        public int? Image2Width { get; set; }
        public int? Image2Height { get; set; }
        public byte? Image2Alpha { get; set; }
        public bool? IsGound { get; set; }
    }
    //name, (displayName, help, (field, move))
    public class MoveTypeData : BaseData
    {
        public MoveTypeData(string name, string file, int line) : base(name, file, line) { }
        public string DisplayName { get; set; }
        public string Help { get; set; }
        public Dictionary<string, byte> FieldMoveDictionary { get; } = new Dictionary<string, byte>();
        public bool? HideHelp { get; internal set; }
    }
    public class FieldData : InheritData
    {
        public FieldData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public enum Smooth : byte { on, off, step }
        public enum ChipType : byte { None, coll, wall, wall2 }
        public ChipType Type { get; set; }
        public string Attribute { get; set; }
        public byte? ColorR { get; set; }
        public byte? ColorG { get; set; }
        public byte? ColorB { get; set; }
        public string BaseId { get; set; }
        public bool? IsEdge { get; set; }
        public List<string> JointChip { get; } = new List<string>();
        public List<string> ImageNameRandomList { get; } = new List<string>();
        public int? Alt_min { get; set; }
        public int? Alt_max { get; set; }
        public Smooth SmoothType { get; set; }
        public List<string> MemberRandomList { get; } = new List<string>();
        public List<string> JointImage { get; } = new List<string>();
    }
    public class DungeonData : InheritData
    {
        public DungeonData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public string DisplayName { get; set; }
        public int? Max { get; set; }
        public int? MoveSpeedRatio { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public bool? IsOpened { get; set; }
        public int? LevelAdjust { get; set; }
        public int? Limit { get; set; }
        public string BGM { get; set; }
        public byte? Volume { get; set; }
        public byte? Blind { get; set; }
        public int? BaseLevel { get; set; }
        public byte? ColorDirection { get; set; }
        public byte? ForeColorA { get; set; }
        public byte? ForeColorR { get; set; }
        public byte? ForeColorG { get; set; }
        public byte? ForeColorB { get; set; }
        public byte? BackColorA { get; set; }
        public byte? BackColorR { get; set; }
        public byte? BackColorG { get; set; }
        public byte? BackColorB { get; set; }
        public byte? Dense { get; set; }
        public string MapFileName { get; set; }

        public string Floor { get; set; }
        public string Wall { get; set; }
        public string Start { get; set; }
        public string Goal { get; set; }
        public Dictionary<string, int> Monsters { get; } = new Dictionary<string, int>();
        public int? MonsterNumber { get; set; }
        public string Box { get; set; }
        public List<string> Item { get; } = new List<string>();
        public int? ItemNumber { get; set; }
        public bool? ItemText { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? HallwayWidth { get; set; }
        public long? Seed { get; set; }
        public byte? Ray { get; set; }
        public byte? Way1 { get; set; }
        public byte? Way2 { get; set; }
        public byte? Way3 { get; set; }
        //階, 項目, データ
        public Dictionary<int, Dictionary<string, List<Token>>> VariantData { get; set; } = new Dictionary<int, Dictionary<string, List<Token>>>();
    }
    public class DetailData : ConcurrentDictionary<string, string>
    {
        public ICollection<string> Names => Keys;
        public ICollection<string> Descriptions => Values;
        //scenario, name, 文章
        public Dictionary<string, Dictionary<string, string>> VariantData { get; } = new Dictionary<string, Dictionary<string, string>>();
        public new bool Clear()
        {
            base.Clear();
            VariantData.Clear();
            return true;
        }
    }
    public class ContextData
    {
        public Dictionary<string, List<Token>> VariantData { get; } = new Dictionary<string, List<Token>>();
        public string TitleName { get; set; } = "";
        public int TitleMenuRight { get; set; } = -50;
        public int TitleMenuTop { get; set; } = -50;
        public int TitleMenuSpace { get; set; } = 50;
        public byte Member { get; set; } = 8;
        public byte MovementNumber { get; set; } = 1;
        public byte EmployRange { get; set; } = 2;
        public bool BattleFast { get; set; } = false;
        public byte SupportRange { get; set; } = 3;
        public byte Myrange { get; set; } = 1;
        public byte MyHelpRange { get; set; } = 1;
        public bool SameCall_easy { get; set; } = false;
        public bool IsBlind_easy { get; set; } = false;
        public bool Dead_easy { get; set; } = false;
        public bool Dead2_easy { get; set; } = false;
        public bool Escape_easy { get; set; } = false;
        public bool Auto_easy { get; set; } = false;
        public bool SDown_easy { get; set; } = false;
        public bool Steal_easy { get; set; } = false;
        public bool Hostil_easy { get; set; } = false;
        public bool CoverFire_easy { get; set; } = false;
        public bool ForceFire_easy { get; set; } = false;
        public bool NoZone_easy { get; set; } = false;
        public bool SameCall_normal { get; set; } = false;
        public bool IsBlind_normal { get; set; } = false;
        public bool Dead_normal { get; set; } = false;
        public bool Dead2_normal { get; set; } = false;
        public bool Escape_normal { get; set; } = false;
        public bool Auto_normal { get; set; } = false;
        public bool SDown_normal { get; set; } = false;
        public bool Steal_normal { get; set; } = false;
        public bool Hostil_normal { get; set; } = false;
        public bool CoverFire_normal { get; set; } = false;
        public bool ForceFire_normal { get; set; } = false;
        public bool NoZone_normal { get; set; } = false;
        public bool SameCall_hard { get; set; } = false;
        public bool IsBlind_hard { get; set; } = false;
        public bool Dead_hard { get; set; } = false;
        public bool Dead2_hard { get; set; } = false;
        public bool Escape_hard { get; set; } = false;
        public bool Auto_hard { get; set; } = false;
        public bool SDown_hard { get; set; } = false;
        public bool Steal_hard { get; set; } = false;
        public bool Hostil_hard { get; set; } = false;
        public bool CoverFire_hard { get; set; } = false;
        public bool ForceFire_hard { get; set; } = false;
        public bool NoZone_hard { get; set; } = false;
        public bool SameCall_luna { get; set; } = false;
        public bool IsBlind_luna { get; set; } = false;
        public bool Dead_luna { get; set; } = false;
        public bool Dead2_luna { get; set; } = false;
        public bool Escape_luna { get; set; } = false;
        public bool Auto_luna { get; set; } = false;
        public bool SDown_luna { get; set; } = false;
        public bool Steal_luna { get; set; } = false;
        public bool Hostil_luna { get; set; } = false;
        public bool CoverFire_luna { get; set; } = false;
        public bool ForceFire_luna { get; set; } = false;
        public bool NoZone_luna { get; set; } = false;

        public string[] ModeText { get; } = new string[4] { "COMと対等の条件で戦います", "戦場での視界が制限されます", "COM勢力は自陣営の人材ユニットに応じた上位クラス兵を雇用します", "人材ユニットが戦死します" };
        public int fv_hp_per { get; set; } = 10;
        public int fv_mp_per { get; set; } = 100;
        public int fv_attack_per { get; set; } = 100;
        public int fv_speed_per { get; set; } = 200;
        public int fv_move_per { get; set; } = 200;
        public int fv_hprec_per { get; set; } = 400;
        public int fv_consti_mul { get; set; } = 10;
        public int fv_summon_mul { get; set; } = 25;
        public int fv_level_per { get; set; } = 10;
        public enum Order { Normal, Dash, NormalTest, DashTest }
        public Order PowerOrder { get; set; } = Order.Normal;
        public bool TalentMode { get; set; } = true;
        public bool NonPlayerMode { get; set; } = false;
        public bool DefaultEnding { get; set; } = true;
        public bool PictureFloorMsg { get; set; } = true;
        public string RaceSuffix { get; set; } = "族";
        public string RaceLabel { get; set; } = "種族";
        public int SoundVolume { get; set; } = -2500;
        public byte SoundCount { get; set; } = 5;
        public byte BGMVolume { get; set; } = 30;

        public int GainPer { get; set; } = 200;
        private byte _SpotCapacity = 8;
        public byte SpotCapacity
        {
            get
            {
                return _SpotCapacity;
            }
            set
            {
                _SpotCapacity = value > 24 ? (byte)24 : value;
            }
        }
        private byte _WarCapacity = 12;
        public byte WarCapacity
        {
            get
            {
                return _WarCapacity;
            }
            set
            {
                _WarCapacity = value > 24 ? (byte)24 : value;
            }
        }
        public int MaxUnit { get; set; } = 1000;
        public byte DeadPenalty { get; set; } = 25;
        public int WinGain { get; set; } = 200;
        public int TrainingAverage { get; set; } = 100;
        public byte LeavePeriod { get; set; } = 5;
        public int MeritsBonus { get; set; } = 1000;
        public byte CautionAdjust { get; set; } = 30;
        public int NoTalentAbility { get; set; } = 100;

        public int unit_level_max { get; set; } = 100;
        public int unit_attack_range { get; set; } = 300;
        public int unit_escape_range { get; set; } = 300;
        public int unit_escape_run { get; set; } = 100;
        public int unit_hand_range { get; set; } = 16;
        public int unit_slow_per { get; set; } = 75;
        public int unit_summon_level { get; set; } = 20;
        public int unit_summon_min { get; set; } = 1;
        public int unit_poison_per { get; set; } = 10;
        public int unit_drain_mul { get; set; } = 5;
        public int unit_status_hp { get; set; } = 100;
        public int unit_status_death { get; set; } = 100;
        public int unit_retreat_damage { get; set; } = 20;
        public int unit_retreat_speed { get; set; } = 200;
        public int unit_view_range { get; set; } = 10;
        public bool unit_castle_cram { get; set; } = true;
        public bool unit_battle_cram { get; set; } = false;
        public bool unit_castle_forcefire { get; set; } = true;
        public bool unit_keep_form { get; set; } = false;
        public bool unit_element_heal { get; set; } = false;

        public int btl_limit { get; set; } = 500;
        public int btl_limit_c { get; set; } = 500;
        public int btl_auto { get; set; } = 100;
        public int btl_unitmax { get; set; } = 300;
        public int btl_nocastle_bdr { get; set; } = 50;
        public int btl_lineshift { get; set; } = 160;
        public int btl_prepare_min { get; set; } = 50;
        public int btl_prepare_max { get; set; } = 100;
        public int btl_breast_width { get; set; } = 15;
        public byte btl_blind_alpha { get; set; } = 160;
        public byte btl_replace_line { get; set; } = 70;
        public int btl_wingmax { get; set; } = 300;
        public int btl_msgtime { get; set; } = 25;
        public int btl_min_damage { get; set; } = 10;

        public byte neutral_max { get; set; } = 24;
        public byte neutral_min { get; set; } = 4;
        public byte neutral_member_max { get; set; } = 8;
        public byte neutral_member_min { get; set; } = 2;

        public string[] SkillIcon { get; } = new string[4] { "", "", "", "" };
        public int ItemSellPercentage { get; set; } = 50;
        public byte ExecutiveBorder { get; set; } = 10;
        public byte SeniorBorder { get; set; } = 50;
        public int EmployPrice { get; set; } = 5000;
        public int VassalPrice { get; set; } = 1000;

        public int support_gain { get; set; } = 10;
        public byte compati_vassal_bdr { get; set; } = 65;
        public byte compati_bad { get; set; } = 5;
        public byte loyal_up { get; set; } = 3;
        public byte loyal_down { get; set; } = 5;
        public int loyal_border { get; set; } = 50;
        public int loyal_escape_border { get; set; } = 10;
        public int arbeit_gain { get; set; } = 10;
        public byte arbeit_turn { get; set; } = 3;
        public int arbeit_price_coe { get; set; } = 33;
        public int arbeit_vassal_coe { get; set; } = 100;
        public bool arbeit_player { get; set; } = false;

        public int RaidBorder { get; set; } = 100;
        public int RaidMax { get; set; } = 50;
        public int RaidMin { get; set; } = -50;
        public int RaidCoe { get; set; } = 15;
        public int BattleCastleLot { get; set; } = 200;

        public bool ScenarioSelect2On { get; set; } = false;
        public string ScenarioSelect2 { get; set; } = "";
        public byte ScenarioSelect2Percentage { get; set; } = 50;

        public bool UnitImageRight { get; set; } = false;

        public List<string> FontFiles { get; } = new List<string>();
        public byte[] ColorWhite { get; } = new byte[3] { 255, 255, 255 };
        public byte[] ColorBlue { get; } = new byte[3] { 0, 0, 255 };
        public byte[] ColorRed { get; } = new byte[3] { 255, 0, 0 };
        public byte[] ColorMagenta { get; } = new byte[3] { 255, 0, 255 };
        public byte[] ColorGreen { get; } = new byte[3] { 0, 255, 0 };
        public byte[] ColorCyan { get; } = new byte[3] { 0, 255, 255 };
        public byte[] ColorYellow { get; } = new byte[3] { 255, 255, 0 };
        public byte[] ColorOrange { get; } = new byte[3] { 255, 99, 71 };
        public byte[] ColorText { get; } = new byte[3] { 0, 0, 0 };
        public byte[] ColorName { get; } = new byte[3] { 0, 0, 0 };
        public byte[] ColorNameHelp { get; } = new byte[3] { 0, 0, 0 };
        private sbyte _WindowBottom = -1;
        public SByte WindowBottom
        {
            get { return _WindowBottom; }
            set { _WindowBottom = (value < 0 || value > 19) ? (sbyte)0 : value; }
        }
        public sbyte[] Window_menu { get; } = new sbyte[2];
        public sbyte[] Window_heromenu { get; } = new sbyte[2];
        public sbyte[] Window_commenu { get; } = new sbyte[2];
        public sbyte[] Window_spot { get; } = new sbyte[2];
        public sbyte[] Window_spot2 { get; } = new sbyte[2];
        public sbyte[] Window_status { get; } = new sbyte[2];
        public sbyte[] Window_merce { get; } = new sbyte[2];
        public sbyte[] Window_war { get; } = new sbyte[2];
        public sbyte[] Window_diplo { get; } = new sbyte[2];
        public sbyte[] Window_powerinfo { get; } = new sbyte[2];
        public sbyte[] Window_personinfo { get; } = new sbyte[2];
        public sbyte[] Window_save { get; } = new sbyte[2];
        public sbyte[] Window_load { get; } = new sbyte[2];
        public sbyte[] Window_tool { get; } = new sbyte[2];
        public sbyte[] Window_battle { get; } = new sbyte[2];
        public sbyte[] Window_message { get; } = new sbyte[2];
        public sbyte[] Window_name { get; } = new sbyte[2];
        public sbyte[] Window_dialog { get; } = new sbyte[2];
        public sbyte[] Window_help { get; } = new sbyte[2];
        public sbyte[] Window_detail { get; } = new sbyte[2];
        public sbyte[] Window_corps { get; } = new sbyte[2];
        public sbyte[] Window_bstatus { get; } = new sbyte[2];
        public sbyte[] Window_powerselect { get; } = new sbyte[2];
        public sbyte[] Window_personselect { get; } = new sbyte[2];
        public sbyte[] Window_scenario { get; } = new sbyte[2];
        public sbyte[] Window_scenariotext { get; } = new sbyte[2];
        public byte Alpha_Window_menu { get; set; } = 255;
        public byte Alpha_Window_heromenu { get; set; } = 255;
        public byte Alpha_Window_commenu { get; set; } = 255;
        public byte Alpha_Window_spot { get; set; } = 255;
        public byte Alpha_Window_spot2 { get; set; } = 255;
        public byte Alpha_Window_status { get; set; } = 255;
        public byte Alpha_Window_merce { get; set; } = 255;
        public byte Alpha_Window_war { get; set; } = 255;
        public byte Alpha_Window_diplo { get; set; } = 255;
        public byte Alpha_Window_powerinfo { get; set; } = 255;
        public byte Alpha_Window_personinfo { get; set; } = 255;
        public byte Alpha_Window_save { get; set; } = 255;
        public byte Alpha_Window_load { get; set; } = 255;
        public byte Alpha_Window_tool { get; set; } = 255;
        public byte Alpha_Window_battle { get; set; } = 255;
        public byte Alpha_Window_message { get; set; } = 255;
        public byte Alpha_Window_name { get; set; } = 255;
        public byte Alpha_Window_dialog { get; set; } = 255;
        public byte Alpha_Window_help { get; set; } = 255;
        public byte Alpha_Window_detail { get; set; } = 255;
        public byte Alpha_Window_corps { get; set; } = 255;
        public byte Alpha_Window_bstatus { get; set; } = 255;
        public byte Alpha_Window_powerselect { get; set; } = 255;
        public byte Alpha_Window_personselect { get; set; } = 255;
        public byte Alpha_Window_scenario { get; set; } = 255;
        public byte Alpha_Window_scenariotext { get; set; } = 255;
        public bool FullBodyDetail { get; set; } = false;
        public int event_bg_size { get; set; } = 768;
    }
}