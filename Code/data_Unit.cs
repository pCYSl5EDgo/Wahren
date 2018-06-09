using System;
using System.Collections.Generic;
using MessagePack;

namespace Wahren
{
    [MessagePackObject]
    public class GenericUnitData : CommonUnitData
    {
        public GenericUnitData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public GenericUnitData() : base("", "", "", 0) { }
        [Key(119)]
        public string BaseClassKey { get; set; }
        [Key(120)]
        public bool? IsUnique { get; set; }
        [Key(121)]
        public Tuple<string, int> Change { get; set; }
    }
    [MessagePackObject]
    public sealed class UnitData : CommonUnitData
    {
        public UnitData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public UnitData() : base("", "", "", 0) { }
        [Key(119)]
        public bool? IsTalent { get; set; }
        [Key(120)]
        public string BGM { get; set; }
        [Key(121)]
        public byte? Volume { get; set; }
        [Key(122)]
        public string Picture { get; set; }
        [Key(123)]
        public byte? PictureDetail { get; set; }
        [Key(124)]
        public byte? PictureMenu { get; set; }
        [Key(125)]
        public int? PictureFloor { get; set; }
        [Key(126)]
        public int? PictureShift { get; set; }
        [Key(127)]
        public int? PictureShiftUp { get; set; }
        [Key(128)]
        public byte? PictureCenter { get; set; }
        [Key(129)]
        public string PictureBack { get; set; }
        [Key(130)]
        public byte? AlivePercentage { get; set; }
        [Key(131)]
        public int? Medical { get; set; }
        [Key(132)]
        public Dictionary<int, List<string>> LeaderSkill { get; set; } = new Dictionary<int, List<string>>();
        [Key(133)]
        public Dictionary<int, List<string>> AssistSkill { get; set; } = new Dictionary<int, List<string>>();

        [Key(134)]
        public byte? Yabo { get; set; }
        [Key(135)]
        public byte? Kosen { get; set; }
        [Key(136)]
        public byte? Align { get; set; }
        [Key(137)]
        public List<string> Enemy { get; set; } = new List<string>();
        [Key(138)]
        public Tuple<string, byte> Loyal { get; set; }
        [Key(139)]
        public string PowerDisplayName { get; set; }
        [Key(140)]
        public string Flag { get; set; }
        [Key(141)]
        public List<string> Staff { get; set; } = new List<string>();
        [Key(142)]
        public bool? Diplomacy { get; set; }
        [Key(143)]
        public Dictionary<string, int> CastleGuard { get; set; } = new Dictionary<string, int>();
        [Key(144)]
        public bool? IsActor { get; set; }
        [Key(145)]
        public bool? IsEnableSelect { get; set; }
        [Key(146)]
        public int? EnableTurn { get; set; }
        [Key(147)]
        public int? EnableTurnMax { get; set; }
        //off 0
        //on 1
        //home 2
        [Key(148)]
        public byte? Fix { get; set; }
        [Key(149)]
        public List<string> Home { get; set; } = new List<string>();
        [Key(150)]
        public bool? IsNoEscape { get; set; }
        [Key(151)]
        public bool? IsNoRemoveUnit { get; set; }
        [Key(152)]
        public bool? IsNoEmployUnit { get; set; }
        [Key(153)]
        public bool? IsNoItemUnit { get; set; }
        //on 0
        //power 1
        //fix 2
        [Key(154)]
        public byte? ArbeitType { get; set; }
        [Key(155)]
        public byte? ArbeitPercentage { get; set; }
        [Key(156)]
        public int? ArbeitCapacity { get; set; }
        [Key(157)]
        public string Help { get; set; }
        [Key(158)]
        public string Join { get; set; }
        [Key(159)]
        public string Dead { get; set; }
        [Key(160)]
        public string Retreat { get; set; }
        [Key(161)]
        public string Break { get; set; }
        [Key(162)]
        public string VoiceType { get; set; }
        //blue 0/off
        //red 1/on
        [Key(163)]
        public byte? Team { get; set; }
        [Key(164)]
        public bool? KeepForm { get; set; }
        [Key(165)]
        public int? BreastWidth { get; set; }

        //never  0
        //never2 1
        //rect   2
        //rect2  3
        //range  4
        //range2 5
        //time   6
        //time2  7
        //troop  8
        //troop2 9
        [Key(166)]
        public byte? ActiveType { get; set; }
        [Key(167)]
        public int? ActiveRange { get; set; }
        [Key(168)]
        public int? ActiveTime { get; set; }
        [Key(169)]
        public string ActiveTroop { get; set; }
        [Key(170)]
        public Tuple<int, int, int, int> ActiveRect { get; set; }
    }
    [MessagePackObject]
    public class CommonUnitData : ScenarioVariantData
    {
        public CommonUnitData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public CommonUnitData() : base("", "", "", 0) { }
        [Key(6)]
        public string DisplayName { get; set; }
        // 0 neuter
        // 1 male
        // 2 female
        [Key(7)]
        public byte? Sex { get; set; }
        [Key(8)]
        public string Race { get; set; }
        [Key(9)]
        public string Class { get; set; }
        [Key(10)]
        public int? Radius { get; set; }
        [Key(11)]
        public byte? RadiusPress { get; set; }
        [Key(12)]
        public int? Width { get; set; }
        [Key(13)]
        public int? Height { get; set; }
        [Key(14)]
        public byte? Alpha { get; set; }
        [Key(15)]
        public string Image { get; set; }
        [Key(16)]
        public string Image2 { get; set; }
        [Key(17)]
        public bool? IsTkool { get; set; }
        [Key(18)]
        public int? Price { get; set; }
        [Key(19)]
        public int? Cost { get; set; }
        [Key(20)]
        public int? HasEXP { get; set; }
        [Key(21)]
        public bool? IsFriendAllClass { get; set; }
        [Key(22)]
        public bool? IsFriendAllRace { get; set; }
        [Key(23)]
        public List<string> Friends { get; set; } = new List<string>();
        [Key(24)]
        public int? FriendExCount { get; set; }
        [Key(25)]
        public List<string> FriendEx1 { get; set; } = new List<string>();
        [Key(26)]
        public List<string> FriendEx2 { get; set; } = new List<string>();
        [Key(27)]
        public List<string> Merce { get; set; } = new List<string>();
        [Key(28)]
        public bool? IsSameFriend { get; set; }
        [Key(29)]
        public bool? IsSameCall { get; set; }
        [Key(30)]
        public bool? IsSameCallBaseUp { get; set; }
        [Key(31)]
        public bool? IsSameSex { get; set; }
        [Key(32)]
        public List<string> Member { get; set; } = new List<string>();
        [Key(33)]
        public int? Level { get; set; }
        [Key(34)]
        public int? Hp { get; set; }
        [Key(35)]
        public int? Mp { get; set; }
        [Key(36)]
        public int? Attack { get; set; }
        [Key(37)]
        public int? Defense { get; set; }
        [Key(38)]
        public int? Magic { get; set; }
        [Key(39)]
        public int? Magdef { get; set; }
        [Key(40)]
        public int? Speed { get; set; }
        [Key(41)]
        public int? Dext { get; set; }
        [Key(42)]
        public int? Move { get; set; }
        [Key(43)]
        public int? HpRec { get; set; }
        [Key(44)]
        public int? MpRec { get; set; }
        [Key(45)]
        public int? summon_max { get; set; }
        [Key(46)]
        public int? summon_level { get; set; }
        [Key(47)]
        public int? heal_max { get; set; }
        [Key(48)]
        public int? attack_max { get; set; }
        [Key(49)]
        public int? defense_max { get; set; }
        [Key(50)]
        public int? magic_max { get; set; }
        [Key(51)]
        public int? magdef_max { get; set; }
        [Key(52)]
        public int? speed_max { get; set; }
        [Key(53)]
        public int? dext_max { get; set; }
        [Key(54)]
        public int? move_max { get; set; }
        [Key(55)]
        public int? hprec_max { get; set; }
        [Key(56)]
        public int? mprec_max { get; set; }
        [Key(57)]
        public List<string> Skill { get; set; } = new List<string>();
        [Key(58)]
        public Dictionary<int, List<string>> Skill2 { get; set; } = new Dictionary<int, List<string>>();
        [Key(59)]
        public List<string> DeleteSkill { get; set; } = new List<string>();
        [Key(60)]
        public Dictionary<int, List<string>> Learn { get; set; } = new Dictionary<int, List<string>>();
        [Key(61)]
        public List<string> DeleteSkill2 { get; set; } = new List<string>();
        [Key(62)]
        public Dictionary<string, int> Consti { get; set; } = new Dictionary<string, int>();
        [Key(63)]
        public string MoveType { get; set; }
        //front 10
        //back 0
        [Key(64)]
        public byte? DefenseLine { get; set; }
        [Key(65)]
        public bool? IsSatellite { get; set; }
        [Key(66)]
        public int? Satellite { get; set; }
        [Key(67)]
        public bool? IsBeast { get; set; }
        [Key(68)]
        public int? NoKnock { get; set; }
        [Key(69)]
        public bool? IsNoCover { get; set; }
        [Key(70)]
        public bool? IsViewUnit { get; set; }
        [Key(71)]
        public bool? IsElementLost { get; set; }
        [Key(72)]
        public int? AttackRange { get; set; }
        [Key(73)]
        public int? EscapeRange { get; set; }
        [Key(74)]
        public int? EscapeRun { get; set; }
        [Key(75)]
        public int? HandRange { get; set; }
        [Key(76)]
        public int? WakeRange { get; set; }
        [Key(77)]
        public int? ViewRange { get; set; }
        [Key(78)]
        public int? CavalryRange { get; set; }
        [Key(79)]
        public int? level_max { get; set; }
        [Key(80)]
        public Dictionary<string, int> Multi { get; set; } = new Dictionary<string, int>();
        [Key(81)]
        public int? exp { get; set; }
        [Key(82)]
        public int? exp_mul { get; set; }
        [Key(83)]
        public int? exp_max { get; set; }
        [Key(84)]
        public int? hpUp { get; set; }
        [Key(85)]
        public int? mpUp { get; set; }
        [Key(86)]
        public int? attackUp { get; set; }
        [Key(87)]
        public int? defenseUp { get; set; }
        [Key(88)]
        public int? magicUp { get; set; }
        [Key(89)]
        public int? magdefUp { get; set; }
        [Key(90)]
        public int? speedUp { get; set; }
        [Key(91)]
        public int? dextUp { get; set; }
        [Key(92)]
        public int? moveUp { get; set; }
        [Key(93)]
        public int? hprecUp { get; set; }
        [Key(94)]
        public int? mprecUp { get; set; }
        [Key(95)]
        public int? hpMax { get; set; }
        [Key(96)]
        public int? mpMax { get; set; }
        [Key(97)]
        public int? attackMax { get; set; }
        [Key(98)]
        public int? defenseMax { get; set; }
        [Key(99)]
        public int? magicMax { get; set; }
        [Key(100)]
        public int? magdefMax { get; set; }
        [Key(101)]
        public int? speedMax { get; set; }
        [Key(102)]
        public int? dextMax { get; set; }
        [Key(103)]
        public int? moveMax { get; set; }
        [Key(104)]
        public int? hprecMax { get; set; }
        [Key(105)]
        public int? mprecMax { get; set; }
        [Key(106)]
        public string Scream { get; set; }
        [Key(107)]
        public int? LostCorpse { get; set; }
        [Key(108)]
        public int? FreeMove { get; set; }
        //off 0
        //on 1
        //roam 2
        [Key(109)]
        public byte? AddVassal { get; set; }
        [Key(110)]
        public bool? IsBrave { get; set; }
        [Key(111)]
        public bool? IsHandle { get; set; }
        [Key(112)]
        public int? SortKey { get; set; }
        [Key(113)]
        public byte? Brave { get; set; }
        [Key(114)]
        public string Face { get; set; }
        [Key(115)]
        public List<string> Item { get; set; } = new List<string>();
        [Key(116)]
        public bool? IsNoTraining { get; set; }
        [Key(117)]
        public string DeadEvent { get; set; }
        //1: on
        //2: fix
        //3: erase
        //4: unique
        [Key(118)]
        public byte? Politics { get; set; }
    }
}