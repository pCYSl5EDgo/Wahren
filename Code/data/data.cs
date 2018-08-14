using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using MessagePack;

namespace Wahren.Analysis
{
    public enum StructureDataType
    {
        Attribute, Context, Detail, Sound, Workspace,
        Class, Power, Spot, Story, Unit,
        Dungeon, Event, Field, Movetype, World,
        Fight, Function, Deploy,
        Object, Race, Scenario, Skill, Skillset, Voice
    }
    [MessagePackObject]
    public class BaseData : IName, IDebugInfo
    {
        [IgnoreMember]
        [System.Runtime.Serialization.IgnoreDataMember]
        private string _file;
        [Key(0)]
        public string File { get => _file; set => _file = String.Intern(value); }
        [Key(1)]
        public int Line { get; set; }
        [Key(2)]
        public string Name { get; set; }
        //継承のためのもの
        //どの項目がnull(@)に設定されたかを示す
        [Key(3)]
        public List<string> FilledWithNull { get; } = new List<string>();

        protected BaseData(string name, string file, int line)
        {
            Name = name ?? "";
            File = file;
            Line = line;
        }
        [IgnoreMember]
        [System.Runtime.Serialization.IgnoreDataMember]
        public string DebugInfo => File + '/' + (Line + 1);
    }
    [MessagePackObject]
    public abstract class InheritData : BaseData, IInherit
    {
        [Key(4)]
        public string Inherit { get; set; }
        protected InheritData(string name, string inherit, string file, int line) : base(name, file, line) { Inherit = inherit ?? ""; }
        [SerializationConstructor]
        protected InheritData() : base("", "", 0) { }
    }
    [MessagePackObject]
    public class ScenarioVariantData : InheritData
    {
        //scenario, 項目, データ
        [Key(5)]
        public Dictionary<string, Dictionary<string, LexicalTree_Assign>> VariantData { get; } = new Dictionary<string, Dictionary<string, LexicalTree_Assign>>();
        protected ScenarioVariantData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
    }
    public sealed class AttributeData : ConcurrentDictionary<string, ValueTuple<string, int>>
    {
        public AttributeData()
        {
            this["poi"] = new ValueTuple<string, int>("毒", 10);
            this["para"] = new ValueTuple<string, int>("麻痺", 10);
            this["ill"] = new ValueTuple<string, int>("幻覚", 10);
            this["sil"] = new ValueTuple<string, int>("沈黙", 10);
            this["conf"] = new ValueTuple<string, int>("混乱", 10);
            this["stone"] = new ValueTuple<string, int>("石化", 10);
            this["fear"] = new ValueTuple<string, int>("恐慌", 10);
            this["suck"] = new ValueTuple<string, int>("吸血", 10);
            this["magsuck"] = new ValueTuple<string, int>("魔吸", 10);
            this["drain"] = new ValueTuple<string, int>("ﾄﾞﾚｲﾝ", 10);
            this["death"] = new ValueTuple<string, int>("即死", 10);
            this["wall"] = new ValueTuple<string, int>("城壁", 10);
        }
        public new bool Clear()
        {
            base.Clear();
            this["poi"] = new ValueTuple<string, int>("毒", 10);
            this["para"] = new ValueTuple<string, int>("麻痺", 10);
            this["ill"] = new ValueTuple<string, int>("幻覚", 10);
            this["sil"] = new ValueTuple<string, int>("沈黙", 10);
            this["conf"] = new ValueTuple<string, int>("混乱", 10);
            this["stone"] = new ValueTuple<string, int>("石化", 10);
            this["fear"] = new ValueTuple<string, int>("恐慌", 10);
            this["suck"] = new ValueTuple<string, int>("吸血", 10);
            this["magsuck"] = new ValueTuple<string, int>("魔吸", 10);
            this["drain"] = new ValueTuple<string, int>("ﾄﾞﾚｲﾝ", 10);
            this["death"] = new ValueTuple<string, int>("即死", 10);
            this["wall"] = new ValueTuple<string, int>("城壁", 10);
            return true;
        }
    }
    public sealed class WorkspaceData : ConcurrentDictionary<string, string> { }

    [MessagePackObject]
    public sealed class VoiceData : InheritData
    {
        public VoiceData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public VoiceData() : base("", "", "", 0) { }
        [Key(5)]
        public List<string> VoiceType { get; } = new List<string>();
        [Key(6)]
        public List<string> DeleteVoiceType { get; } = new List<string>();
        [Key(7)]
        public List<string> SpotVoice { get; } = new List<string>();
        [Key(8)]
        public List<string> PowerVoice { get; } = new List<string>();
        [Key(9)]
        public bool NoPower { get; set; }
    }
    [MessagePackObject]
    public class SpotData : ScenarioVariantData
    {
        public SpotData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public SpotData() : base("", "", "", 0) { }
        [Key(6)]
        public string DisplayName { get; set; }
        [Key(7)]
        public string Image { get; set; }
        [Key(8)]
        public int? X { get; set; }
        [Key(9)]
        public int? Y { get; set; }
        [Key(10)]
        public int? Width { get; set; }
        [Key(11)]
        public int? Height { get; set; }
        [Key(12)]
        public string Map { get; set; }

        [Key(13)]
        public bool? IsCastleBattle { get; set; } = false;

        [Key(14)]
        public List<string> Yorozu { get; } = new List<string>();

        [Key(15)]
        public int? Limit { get; set; }

        [Key(16)]
        public string BGM { get; set; }
        [Key(17)]
        public byte? Volume { get; set; }

        [Key(18)]
        public int? Gain { get; set; }
        [Key(19)]
        public int? Castle { get; set; }
        [Key(20)]
        public byte? Capacity { get; set; }

        [Key(21)]
        public List<string> Members { get; } = new List<string>();

        [Key(22)]
        public List<string> Monsters { get; } = new List<string>();

        [Key(23)]
        public List<string> Merce { get; } = new List<string>();

        [Key(24)]
        public string Dungeon { get; set; }

        [Key(25)]
        public bool? IsNotHome { get; set; }
        [Key(26)]
        public bool? IsNotRaisableSpot { get; set; }
        [Key(27)]
        public int? CastleLot { get; set; }
        [Key(28)]
        public bool? Politics { get; set; }
    }
    public sealed class SoundData : ConcurrentDictionary<string, int>
    {
        public int Default { get; set; } = -2500;
        public new bool Clear()
        {
            base.Clear();
            Default = -2500;
            return true;
        }
    }
    [MessagePackObject]
    public sealed class SkillSetData : ScenarioVariantData
    {
        public SkillSetData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public SkillSetData() : base("", "", "", 0) { }
        [Key(6)]
        public string DisplayName { get; set; }
        [Key(7)]
        public List<string> MemberSkill { get; } = new List<string>();
        [Key(8)]
        public string BackIconPath { get; set; }
    }
    [MessagePackObject]
    public sealed class RaceData : ScenarioVariantData
    {
        public RaceData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public RaceData() : base("", "", "", 0) { }
        [Key(6)]
        public string DisplayName { get; set; }
        [Key(7)]
        public byte? Align { get; set; }
        [Key(8)]
        public byte? Brave { get; set; }
        [Key(9)]
        public Dictionary<string, byte> Consti { get; } = new Dictionary<string, byte>();
        [Key(10)]
        public string MoveType { get; set; }
    }
    [MessagePackObject]
    public sealed class PowerData : ScenarioVariantData
    {
        public PowerData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public PowerData() : base("", "", "", 0) { }
        [Key(6)]
        public string DisplayName { get; set; }
        [Key(7)]
        public string FlagPath { get; set; }
        [Key(8)]
        public string Master { get; set; }

        [Key(9)]
        public string BGM { get; set; }
        [Key(10)]
        public byte? Volume { get; set; }
        [Key(11)]
        public bool? DoDiplomacy { get; set; }
        [Key(12)]
        public bool? IsSelectable { get; set; }
        [Key(13)]
        public bool? IsPlayableAsTalent { get; set; }
        [Key(14)]
        public bool? IsRaisable { get; set; }

        [Key(15)]
        public int? Money { get; set; }
        [Key(16)]
        public List<string> HomeSpot { get; } = new List<string>();
        public enum FixType : byte { off, on, home, hold, warlike, freeze }
        [Key(17)]
        public FixType? Fix { get; set; }

        [Key(18)]
        public Dictionary<string, int> Diplo { get; } = new Dictionary<string, int>();
        [Key(19)]
        public Dictionary<string, int> League { get; } = new Dictionary<string, int>();
        [Key(20)]
        public Dictionary<string, int> EnemyPower { get; } = new Dictionary<string, int>();

        [Key(21)]
        public List<string> Staff { get; } = new List<string>();
        [Key(22)]
        public List<string> Merce { get; } = new List<string>();

        [Key(23)]
        public int? TrainingAveragePercent { get; set; }
        [Key(24)]
        public int? TrainingUp { get; set; }

        [Key(25)]
        public int? BaseMerit { get; set; }

        [Key(26)]
        public Dictionary<string, int> Merits { get; } = new Dictionary<string, int>();

        [Key(27)]
        public byte? BaseLoyal { get; set; }
        [Key(28)]
        public Dictionary<string, int> Loyals { get; } = new Dictionary<string, int>();

        [Key(29)]
        public string Head { get; set; }
        [Key(30)]
        public string Difficulty { get; set; }
        [Key(31)]
        public byte? Kosen { get; set; }
        [Key(32)]
        public byte? Yabo { get; set; }
        [Key(33)]
        public string Description { get; set; }
        [Key(34)]
        public List<string> MemberSpot { get; } = new List<string>();
        [Key(35)]
        public bool? IsEvent { get; set; }
        [Key(36)]
        public List<string> Friend { get; } = new List<string>();
        [Key(37)]
        public string Master2 { get; set; }
        [Key(38)]
        public string Master3 { get; set; }
        [Key(39)]
        public string Master4 { get; set; }
        [Key(40)]
        public string Master5 { get; set; }
        [Key(41)]
        public string Master6 { get; set; }
        [Key(42)]
        public string Head2 { get; set; }
        [Key(43)]
        public string Head3 { get; set; }
        [Key(44)]
        public string Head4 { get; set; }
        [Key(45)]
        public string Head5 { get; set; }
        [Key(46)]
        public string Head6 { get; set; }
    }
    [MessagePackObject]
    public sealed class ObjectData : InheritData
    {
        public ObjectData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public ObjectData() : base("", "", "", 0) { }
        public enum ChipType
        {
            None, coll, wall, wall2, breakable, gate, floor, start, goal, box, cover
        }
        [Key(5)]
        public ChipType Type { get; set; } = ChipType.None;
        [Key(6)]
        public bool? IsLandBase { get; set; }
        [Key(7)]
        public int? LandBase { get; set; }
        [Key(8)]
        public bool? NoStop { get; set; }
        [Key(9)]
        public byte? NoWall2 { get; set; }
        [Key(10)]
        public bool? NoArcHit { get; set; }
        [Key(11)]
        public int? Radius { get; set; }
        [Key(12)]
        public bool? Blk { get; set; }
        [Key(13)]
        public int? Width { get; set; }
        [Key(14)]
        public int? Height { get; set; }
        [Key(15)]
        public byte? Alpha { get; set; }
        [Key(16)]
        public List<string> ImageNameRandomList { get; } = new List<string>();

        [Key(17)]
        public string Image2Name { get; set; }
        [Key(18)]
        public int? Image2Width { get; set; }
        [Key(19)]
        public int? Image2Height { get; set; }
        [Key(20)]
        public byte? Image2Alpha { get; set; }
        [Key(21)]
        public bool? IsGound { get; set; }
    }
    //name, (displayName, help, (field, move))
    [MessagePackObject]
    public sealed class MoveTypeData : BaseData
    {
        public MoveTypeData(string name, string file, int line) : base(name, file, line) { }
        [SerializationConstructor]
        public MoveTypeData() : base("", "", 0) { }
        [Key(4)]
        public string DisplayName { get; set; }
        [Key(5)]
        public string Help { get; set; }
        [Key(6)]
        public Dictionary<string, byte> FieldMoveDictionary { get; } = new Dictionary<string, byte>();
        [Key(7)]
        public bool? HideHelp { get; internal set; }
    }
    [MessagePackObject]
    public sealed class FieldData : InheritData
    {
        [IgnoreMember]
        private string _attribute;

        public FieldData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public FieldData() : base("", "", "", 0) { }
        public enum Smooth : byte { on, off, step }
        public enum ChipType : byte { None, coll, wall, wall2 }
        [Key(5)]
        public ChipType Type { get; set; }
        [Key(6)]
        public string Attribute { get => _attribute; set => _attribute = string.Intern(value); }
        [Key(7)]
        public byte? ColorR { get; set; }
        [Key(8)]
        public byte? ColorG { get; set; }
        [Key(9)]
        public byte? ColorB { get; set; }
        [Key(10)]
        public string BaseId { get; set; }
        [Key(11)]
        public bool? IsEdge { get; set; }
        [Key(12)]
        public List<string> JointChip { get; } = new List<string>();
        [Key(13)]
        public List<string> ImageNameRandomList { get; } = new List<string>();
        [Key(14)]
        public int? Alt_min { get; set; }
        [Key(15)]
        public int? Alt_max { get; set; }
        [Key(16)]
        public Smooth SmoothType { get; set; }
        [Key(17)]
        public List<string> MemberRandomList { get; } = new List<string>();
        [Key(18)]
        public List<string> JointImage { get; } = new List<string>();
    }
    [MessagePackObject]
    public sealed class DungeonData : InheritData
    {
        public DungeonData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public DungeonData() : base("", "", "", 0) { }
        [Key(5)]
        public string DisplayName { get; set; }
        [Key(6)]
        public int? Max { get; set; }
        [Key(7)]
        public int? MoveSpeedRatio { get; set; }
        [Key(8)]
        public string Prefix { get; set; }
        [Key(9)]
        public string Suffix { get; set; }
        [Key(10)]
        public bool? IsOpened { get; set; }
        [Key(11)]
        public int? LevelAdjust { get; set; }
        [Key(12)]
        public int? Limit { get; set; }
        [Key(13)]
        public string BGM { get; set; }
        [Key(14)]
        public byte? Volume { get; set; }
        [Key(15)]
        public byte? Blind { get; set; }
        [Key(16)]
        public int? BaseLevel { get; set; }
        [Key(17)]
        public byte? ColorDirection { get; set; }
        [Key(18)]
        public byte? ForeColorA { get; set; }
        [Key(19)]
        public byte? ForeColorR { get; set; }
        [Key(20)]
        public byte? ForeColorG { get; set; }
        [Key(21)]
        public byte? ForeColorB { get; set; }
        [Key(22)]
        public byte? BackColorA { get; set; }
        [Key(23)]
        public byte? BackColorR { get; set; }
        [Key(24)]
        public byte? BackColorG { get; set; }
        [Key(25)]
        public byte? BackColorB { get; set; }
        [Key(26)]
        public byte? Dense { get; set; }
        [Key(27)]
        public string MapFileName { get; set; }

        [Key(28)]
        public string Floor { get; set; }
        [Key(29)]
        public string Wall { get; set; }
        [Key(30)]
        public string Start { get; set; }
        [Key(31)]
        public string Goal { get; set; }
        [Key(32)]
        public Dictionary<string, int> Monsters { get; } = new Dictionary<string, int>();
        [Key(33)]
        public int? MonsterNumber { get; set; }
        [Key(34)]
        public string Box { get; set; }
        [Key(35)]
        public List<string> Item { get; } = new List<string>();
        [Key(36)]
        public int? ItemNumber { get; set; }
        [Key(37)]
        public bool? ItemText { get; set; }
        [Key(38)]
        public int? Width { get; set; }
        [Key(39)]
        public int? Height { get; set; }
        [Key(40)]
        public int? HallwayWidth { get; set; }
        [Key(41)]
        public long? Seed { get; set; }
        [Key(42)]
        public byte? Ray { get; set; }
        [Key(43)]
        public byte? Way1 { get; set; }
        [Key(44)]
        public byte? Way2 { get; set; }
        [Key(45)]
        public byte? Way3 { get; set; }
        //階, 項目, データ
        [Key(46)]
        public Dictionary<int, Dictionary<string, List<Token>>> VariantData { get; set; } = new Dictionary<int, Dictionary<string, List<Token>>>();
    }
    public sealed class DetailData : ConcurrentDictionary<string, string>
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
    [MessagePackObject]
    public sealed class ContextData
    {
        [SerializationConstructor]
        public ContextData() { }
        [Key(0)]
        public Dictionary<string, List<Token>> VariantData { get; } = new Dictionary<string, List<Token>>();
        [Key(1)]
        public string TitleName { get; set; } = "";
        [Key(2)]
        public int TitleMenuRight { get; set; } = 200;
        [Key(3)]
        public int TitleMenuTop { get; set; } = 0;
        [Key(4)]
        public int TitleMenuSpace { get; set; } = 50;
        [Key(5)]
        public byte Member { get; set; } = 8;
        [Key(6)]
        public byte MovementNumber { get; set; } = 1;
        [Key(7)]
        public byte EmployRange { get; set; } = 2;
        [Key(8)]
        public bool BattleFast { get; set; } = false;
        [Key(9)]
        public byte SupportRange { get; set; } = 3;
        [Key(10)]
        public byte MyRange { get; set; } = 1;
        [Key(11)]
        public byte MyHelpRange { get; set; } = 1;
        [Key(12)]
        public bool SameCall_easy { get; set; } = false;
        [Key(13)]
        public bool IsBlind_easy { get; set; } = false;
        [Key(14)]
        public bool Dead_easy { get; set; } = false;
        [Key(15)]
        public bool Dead2_easy { get; set; } = false;
        [Key(16)]
        public bool Escape_easy { get; set; } = false;
        [Key(17)]
        public bool Auto_easy { get; set; } = false;
        [Key(18)]
        public bool SDown_easy { get; set; } = false;
        [Key(19)]
        public bool Steal_easy { get; set; } = false;
        [Key(20)]
        public bool Hostil_easy { get; set; } = false;
        [Key(21)]
        public bool CoverFire_easy { get; set; } = false;
        [Key(22)]
        public bool ForceFire_easy { get; set; } = false;
        [Key(23)]
        public bool NoZone_easy { get; set; } = false;
        [Key(24)]
        public bool SameCall_normal { get; set; } = false;
        [Key(25)]
        public bool IsBlind_normal { get; set; } = false;
        [Key(26)]
        public bool Dead_normal { get; set; } = false;
        [Key(27)]
        public bool Dead2_normal { get; set; } = false;
        [Key(28)]
        public bool Escape_normal { get; set; } = false;
        [Key(29)]
        public bool Auto_normal { get; set; } = false;
        [Key(30)]
        public bool SDown_normal { get; set; } = false;
        [Key(31)]
        public bool Steal_normal { get; set; } = false;
        [Key(32)]
        public bool Hostil_normal { get; set; } = false;
        [Key(33)]
        public bool CoverFire_normal { get; set; } = false;
        [Key(34)]
        public bool ForceFire_normal { get; set; } = false;
        [Key(35)]
        public bool NoZone_normal { get; set; } = false;
        [Key(36)]
        public bool SameCall_hard { get; set; } = false;
        [Key(37)]
        public bool IsBlind_hard { get; set; } = false;
        [Key(38)]
        public bool Dead_hard { get; set; } = false;
        [Key(39)]
        public bool Dead2_hard { get; set; } = false;
        [Key(40)]
        public bool Escape_hard { get; set; } = false;
        [Key(41)]
        public bool Auto_hard { get; set; } = false;
        [Key(42)]
        public bool SDown_hard { get; set; } = false;
        [Key(43)]
        public bool Steal_hard { get; set; } = false;
        [Key(44)]
        public bool Hostil_hard { get; set; } = false;
        [Key(45)]
        public bool CoverFire_hard { get; set; } = false;
        [Key(46)]
        public bool ForceFire_hard { get; set; } = false;
        [Key(47)]
        public bool NoZone_hard { get; set; } = false;
        [Key(48)]
        public bool SameCall_luna { get; set; } = false;
        [Key(49)]
        public bool IsBlind_luna { get; set; } = false;
        [Key(50)]
        public bool Dead_luna { get; set; } = false;
        [Key(51)]
        public bool Dead2_luna { get; set; } = false;
        [Key(52)]
        public bool Escape_luna { get; set; } = false;
        [Key(53)]
        public bool Auto_luna { get; set; } = false;
        [Key(54)]
        public bool SDown_luna { get; set; } = false;
        [Key(55)]
        public bool Steal_luna { get; set; } = false;
        [Key(56)]
        public bool Hostil_luna { get; set; } = false;
        [Key(57)]
        public bool CoverFire_luna { get; set; } = false;
        [Key(58)]
        public bool ForceFire_luna { get; set; } = false;
        [Key(59)]
        public bool NoZone_luna { get; set; } = false;

        [Key(60)]
        public string[] ModeText { get; } = new string[4] { "COMと対等の条件で戦います", "戦場での視界が制限されます", "COM勢力は自陣営の人材ユニットに応じた上位クラス兵を雇用します", "人材ユニットが戦死します" };
        [Key(61)]
        public int fv_hp_per { get; set; } = 10;
        [Key(62)]
        public int fv_mp_per { get; set; } = 100;
        [Key(63)]
        public int fv_attack_per { get; set; } = 100;
        [Key(64)]
        public int fv_speed_per { get; set; } = 200;
        [Key(65)]
        public int fv_move_per { get; set; } = 200;
        [Key(66)]
        public int fv_hprec_per { get; set; } = 400;
        [Key(67)]
        public int fv_consti_mul { get; set; } = 10;
        [Key(68)]
        public int fv_summon_mul { get; set; } = 25;
        [Key(69)]
        public int fv_level_per { get; set; } = 10;
        public enum Order { Normal, Dash, NormalTest, DashTest }
        [Key(70)]
        public Order PowerOrder { get; set; } = Order.Normal;
        [Key(71)]
        public bool TalentMode { get; set; } = true;
        [Key(72)]
        public bool NonPlayerMode { get; set; } = false;
        [Key(73)]
        public bool DefaultEnding { get; set; } = true;
        [Key(74)]
        public bool PictureFloorMsg { get; set; } = true;
        [Key(75)]
        public string RaceSuffix { get; set; } = "族";
        [Key(76)]
        public string RaceLabel { get; set; } = "種族";
        [Key(77)]
        public int SoundVolume { get; set; } = -2500;
        [Key(78)]
        public byte SoundCount { get; set; } = 5;
        [Key(79)]
        public byte BGMVolume { get; set; } = 30;

        [Key(80)]
        public int GainPer { get; set; } = 200;

        [Key(81)]
        private byte _SpotCapacity = 8;

        [Key(82)]
        public byte SpotCapacity
        {
            get => _SpotCapacity;
            set
            {
                _SpotCapacity = value > 24 ? (byte)24 : value;
            }
        }

        [Key(83)]
        private byte _WarCapacity = 12;

        [Key(84)]
        public byte WarCapacity
        {
            get => _WarCapacity;
            set
            {
                _WarCapacity = value > 24 ? (byte)24 : value;
            }
        }
        [Key(85)]
        public int MaxUnit { get; set; } = 1000;
        [Key(86)]
        public byte DeadPenalty { get; set; } = 25;
        [Key(87)]
        public int WinGain { get; set; } = 200;
        [Key(88)]
        public int TrainingAverage { get; set; } = 100;
        [Key(89)]
        public byte LeavePeriod { get; set; } = 5;
        [Key(90)]
        public int MeritsBonus { get; set; } = 1000;
        [Key(91)]
        public byte CautionAdjust { get; set; } = 30;
        [Key(92)]
        public int NoTalentAbility { get; set; } = 100;

        [Key(93)]
        public int unit_level_max { get; set; } = 100;
        [Key(94)]
        public int unit_attack_range { get; set; } = 300;
        [Key(95)]
        public int unit_escape_range { get; set; } = 300;
        [Key(96)]
        public int unit_escape_run { get; set; } = 100;
        [Key(97)]
        public int unit_hand_range { get; set; } = 16;
        [Key(98)]
        public int unit_slow_per { get; set; } = 75;
        [Key(99)]
        public int unit_summon_level { get; set; } = 20;
        [Key(100)]
        public int unit_summon_min { get; set; } = 1;
        [Key(101)]
        public int unit_poison_per { get; set; } = 10;
        [Key(102)]
        public int unit_drain_mul { get; set; } = 5;
        [Key(103)]
        public int unit_status_hp { get; set; } = 100;
        [Key(104)]
        public int unit_status_death { get; set; } = 100;
        [Key(105)]
        public int unit_retreat_damage { get; set; } = 20;
        [Key(106)]
        public int unit_retreat_speed { get; set; } = 200;
        [Key(107)]
        public int unit_view_range { get; set; } = 10;
        [Key(108)]
        public bool unit_castle_cram { get; set; } = true;
        [Key(109)]
        public bool unit_battle_cram { get; set; } = false;
        [Key(110)]
        public bool unit_castle_forcefire { get; set; } = true;
        [Key(111)]
        public bool unit_keep_form { get; set; } = false;
        [Key(112)]
        public bool unit_element_heal { get; set; } = false;

        [Key(113)]
        public int btl_limit { get; set; } = 500;
        [Key(114)]
        public int btl_limit_c { get; set; } = 500;
        [Key(115)]
        public int btl_auto { get; set; } = 100;
        [Key(116)]
        public int btl_unitmax { get; set; } = 300;
        [Key(117)]
        public int btl_nocastle_bdr { get; set; } = 50;
        [Key(118)]
        public int btl_lineshift { get; set; } = 160;
        [Key(119)]
        public int btl_prepare_min { get; set; } = 50;
        [Key(120)]
        public int btl_prepare_max { get; set; } = 100;
        [Key(121)]
        public int btl_breast_width { get; set; } = 15;
        [Key(122)]
        public byte btl_blind_alpha { get; set; } = 160;
        [Key(123)]
        public byte btl_replace_line { get; set; } = 70;
        [Key(124)]
        public int btl_wingmax { get; set; } = 300;
        [Key(125)]
        public int btl_msgtime { get; set; } = 25;
        [Key(126)]
        public int btl_min_damage { get; set; } = 10;

        [Key(127)]
        public byte neutral_max { get; set; } = 24;
        [Key(128)]
        public byte neutral_min { get; set; } = 4;
        [Key(129)]
        public byte neutral_member_max { get; set; } = 8;
        [Key(130)]
        public byte neutral_member_min { get; set; } = 2;

        [Key(131)]
        public string[] SkillIcon { get; } = new string[4] { "", "", "", "" };
        [Key(132)]
        public int ItemSellPercentage { get; set; } = 50;
        [Key(133)]
        public byte ExecutiveBorder { get; set; } = 10;
        [Key(134)]
        public byte SeniorBorder { get; set; } = 50;
        [Key(135)]
        public int EmployPrice { get; set; } = 5000;
        [Key(136)]
        public int VassalPrice { get; set; } = 1000;

        [Key(137)]
        public int support_gain { get; set; } = 10;
        [Key(138)]
        public byte compati_vassal_bdr { get; set; } = 65;
        [Key(139)]
        public byte compati_bad { get; set; } = 5;
        [Key(140)]
        public byte loyal_up { get; set; } = 3;
        [Key(141)]
        public byte loyal_down { get; set; } = 5;
        [Key(142)]
        public int loyal_border { get; set; } = 50;
        [Key(143)]
        public int loyal_escape_border { get; set; } = 10;
        [Key(144)]
        public int arbeit_gain { get; set; } = 10;
        [Key(145)]
        public byte arbeit_turn { get; set; } = 3;
        [Key(146)]
        public int arbeit_price_coe { get; set; } = 33;
        [Key(147)]
        public int arbeit_vassal_coe { get; set; } = 100;
        [Key(148)]
        public bool arbeit_player { get; set; } = false;

        [Key(149)]
        public int RaidBorder { get; set; } = 100;
        [Key(150)]
        public int RaidMax { get; set; } = 50;
        [Key(151)]
        public int RaidMin { get; set; } = -50;
        [Key(152)]
        public int RaidCoe { get; set; } = 15;
        [Key(153)]
        public int BattleCastleLot { get; set; } = 200;

        [Key(154)]
        public bool ScenarioSelect2On { get; set; } = false;
        [Key(155)]
        public string ScenarioSelect2 { get; set; } = "";
        [Key(156)]
        public byte ScenarioSelect2Percentage { get; set; } = 50;

        [Key(157)]
        public bool UnitImageRight { get; set; } = false;

        [Key(158)]
        public List<string> FontFiles { get; } = new List<string>();
        [Key(159)]
        public byte[] ColorWhite { get; } = new byte[3] { 255, 255, 255 };
        [Key(160)]
        public byte[] ColorBlue { get; } = new byte[3] { 0, 0, 255 };
        [Key(161)]
        public byte[] ColorRed { get; } = new byte[3] { 255, 0, 0 };
        [Key(162)]
        public byte[] ColorMagenta { get; } = new byte[3] { 255, 0, 255 };
        [Key(163)]
        public byte[] ColorGreen { get; } = new byte[3] { 0, 255, 0 };
        [Key(164)]
        public byte[] ColorCyan { get; } = new byte[3] { 0, 255, 255 };
        [Key(165)]
        public byte[] ColorYellow { get; } = new byte[3] { 255, 255, 0 };
        [Key(166)]
        public byte[] ColorOrange { get; } = new byte[3] { 255, 99, 71 };
        [Key(167)]
        public byte[] ColorText { get; } = new byte[3] { 0, 0, 0 };
        [Key(168)]
        public byte[] ColorName { get; } = new byte[3] { 0, 0, 0 };
        [Key(169)]
        public byte[] ColorNameHelp { get; } = new byte[3] { 0, 0, 0 };

        [Key(170)]
        private sbyte _WindowBottom = -1;

        [Key(171)]
        public SByte WindowBottom
        {
            get { return _WindowBottom; }
            set { _WindowBottom = (value < 0 || value > 19) ? (sbyte)0 : value; }
        }
        [Key(172)]
        public sbyte[] Window_menu { get; } = new sbyte[2];
        [Key(173)]
        public sbyte[] Window_heromenu { get; } = new sbyte[2];
        [Key(174)]
        public sbyte[] Window_commenu { get; } = new sbyte[2];
        [Key(175)]
        public sbyte[] Window_spot { get; } = new sbyte[2];
        [Key(176)]
        public sbyte[] Window_spot2 { get; } = new sbyte[2];
        [Key(177)]
        public sbyte[] Window_status { get; } = new sbyte[2];
        [Key(178)]
        public sbyte[] Window_merce { get; } = new sbyte[2];
        [Key(179)]
        public sbyte[] Window_war { get; } = new sbyte[2];
        [Key(180)]
        public sbyte[] Window_diplo { get; } = new sbyte[2];
        [Key(181)]
        public sbyte[] Window_powerinfo { get; } = new sbyte[2];
        [Key(182)]
        public sbyte[] Window_personinfo { get; } = new sbyte[2];
        [Key(183)]
        public sbyte[] Window_save { get; } = new sbyte[2];
        [Key(184)]
        public sbyte[] Window_load { get; } = new sbyte[2];
        [Key(185)]
        public sbyte[] Window_tool { get; } = new sbyte[2];
        [Key(186)]
        public sbyte[] Window_battle { get; } = new sbyte[2];
        [Key(187)]
        public sbyte[] Window_message { get; } = new sbyte[2];
        [Key(188)]
        public sbyte[] Window_name { get; } = new sbyte[2];
        [Key(189)]
        public sbyte[] Window_dialog { get; } = new sbyte[2];
        [Key(190)]
        public sbyte[] Window_help { get; } = new sbyte[2];
        [Key(191)]
        public sbyte[] Window_detail { get; } = new sbyte[2];
        [Key(192)]
        public sbyte[] Window_corps { get; } = new sbyte[2];
        [Key(193)]
        public sbyte[] Window_bstatus { get; } = new sbyte[2];
        [Key(194)]
        public sbyte[] Window_powerselect { get; } = new sbyte[2];
        [Key(195)]
        public sbyte[] Window_personselect { get; } = new sbyte[2];
        [Key(196)]
        public sbyte[] Window_scenario { get; } = new sbyte[2];
        [Key(197)]
        public sbyte[] Window_scenariotext { get; } = new sbyte[2];
        [Key(198)]
        public byte Alpha_Window_menu { get; set; } = 255;
        [Key(199)]
        public byte Alpha_Window_heromenu { get; set; } = 255;
        [Key(200)]
        public byte Alpha_Window_commenu { get; set; } = 255;
        [Key(201)]
        public byte Alpha_Window_spot { get; set; } = 255;
        [Key(202)]
        public byte Alpha_Window_spot2 { get; set; } = 255;
        [Key(203)]
        public byte Alpha_Window_status { get; set; } = 255;
        [Key(204)]
        public byte Alpha_Window_merce { get; set; } = 255;
        [Key(205)]
        public byte Alpha_Window_war { get; set; } = 255;
        [Key(206)]
        public byte Alpha_Window_diplo { get; set; } = 255;
        [Key(207)]
        public byte Alpha_Window_powerinfo { get; set; } = 255;
        [Key(208)]
        public byte Alpha_Window_personinfo { get; set; } = 255;
        [Key(209)]
        public byte Alpha_Window_save { get; set; } = 255;
        [Key(210)]
        public byte Alpha_Window_load { get; set; } = 255;
        [Key(211)]
        public byte Alpha_Window_tool { get; set; } = 255;
        [Key(212)]
        public byte Alpha_Window_battle { get; set; } = 255;
        [Key(213)]
        public byte Alpha_Window_message { get; set; } = 255;
        [Key(214)]
        public byte Alpha_Window_name { get; set; } = 255;
        [Key(215)]
        public byte Alpha_Window_dialog { get; set; } = 255;
        [Key(216)]
        public byte Alpha_Window_help { get; set; } = 255;
        [Key(217)]
        public byte Alpha_Window_detail { get; set; } = 255;
        [Key(218)]
        public byte Alpha_Window_corps { get; set; } = 255;
        [Key(219)]
        public byte Alpha_Window_bstatus { get; set; } = 255;
        [Key(220)]
        public byte Alpha_Window_powerselect { get; set; } = 255;
        [Key(221)]
        public byte Alpha_Window_personselect { get; set; } = 255;
        [Key(222)]
        public byte Alpha_Window_scenario { get; set; } = 255;
        [Key(223)]
        public byte Alpha_Window_scenariotext { get; set; } = 255;
        [Key(224)]
        public bool FullBodyDetail { get; set; } = false;
        [Key(225)]
        public int event_bg_size { get; set; } = 768;
    }
}