using System.Collections.Generic;
using MessagePack;

namespace Wahren.Analysis
{
    public enum StrType { None, Attack, Magic, AttackMagic, AttackDext, MagicDext, Fix }
    [MessagePackObject]
    public class SkillData : ScenarioVariantData
    {
        public SkillData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [SerializationConstructor]
        public SkillData() : base("", "", "", 0) { }
        [Key(6)]
        public string DisplayName { get; set; }
        [Key(7)]
        public List<string> Icon { get; } = new List<string>();
        [Key(8)]
        public List<byte> IconAlpha { get; } = new List<byte>();
        [Key(9)]
        public int? FkeyNumber { get; set; }
        [Key(10)]
        public string Fkey { get; set; }
        [Key(11)]
        public int? SortKey { get; set; }
        [Key(12)]
        public int? Special { get; set; }
        [Key(13)]
        public int? Delay { get; set; }
        [Key(14)]
        public string GunDelayName { get; set; }
        [Key(15)]
        public int? GunDelay { get; set; }
        [Key(16)]
        public bool? QuickReload { get; set; }
        [Key(17)]
        public string Help { get; set; }
        [Key(18)]
        public bool? HideHelp { get; set; }
        [Key(19)]
        public List<string> Sound { get; } = new List<string>();
        [Key(20)]
        public string Message { get; set; }
        [Key(21)]
        public bool? CutinOn { get; set; }
        [Key(22)]
        public byte? CutinType { get; set; }
        [Key(23)]
        public int? CutinTop { get; set; }
        [Key(24)]
        public byte? CutinY { get; set; }
        [Key(25)]
        public byte? CutinY2 { get; set; }
        [Key(26)]
        public bool? CutinStop { get; set; }
        [Key(27)]
        public int? CutinWakeTime { get; set; }
        [Key(28)]
        public byte? CutinFlashR { get; set; }
        [Key(29)]
        public byte? CutinFlashG { get; set; }
        [Key(30)]
        public byte? CutinFlashB { get; set; }
        [Key(31)]
        public byte? CutinPhantom { get; set; }
        [Key(32)]
        public byte? CutinAlpha { get; set; }
        [Key(33)]
        public int? CutinZoom { get; set; }
        [Key(34)]
        public int? CutinInflate { get; set; }
        [Key(35)]
        public int? CutinSlide { get; set; }
        [Key(36)]
        public int? CutinTrans { get; set; }

        [Key(37)]
        public int? Value { get; set; }
        [Key(38)]
        public bool? Talent { get; set; }
        [Key(39)]
        public string TalentSkill { get; set; }
        [Key(40)]
        public int? ExpPercentage { get; set; }
        public enum FuncType { None, missile, sword, heal, summon, charge, status }
        [Key(41)]
        public FuncType Type { get; set; } = FuncType.None;
        [Key(42)]
        public byte? ItemType { get; set; }
        [Key(43)]
        public int? ItemSort { get; set; }
        [Key(44)]
        public bool? ItemNoSell { get; set; }
        [Key(45)]
        public int? Price { get; set; }
        [Key(46)]
        public int? MP { get; set; }
        [Key(47)]
        public List<string> Friend { get; } = new List<string>();
        [Key(48)]
        public StrType Strength { get; set; }
        [Key(49)]
        public int? StrPercent { get; set; }
        [Key(50)]
        public byte? StrRatio { get; set; }
        [Key(51)]
        public string Image { get; set; }
        [Key(52)]
        public int? Width { get; set; }
        [Key(53)]
        public int? Height { get; set; }
        [Key(54)]
        public byte? Alpha { get; set; }
        [Key(55)]
        public int? Anime { get; set; }
        [Key(56)]
        public int? AnimeInterval { get; set; }

        /*----------------------------------------------------------- */
        //0 なし
        //1 arc
        //2 throw
        //3 drop
        //4 circle
        //5 swing
        //6 missile
        //7 normal
        [Key(57)]
        public byte? MoveType { get; set; }
        [Key(58)]
        public int? Speed { get; set; }
        [Key(59)]
        public byte? AlphaTip { get; set; }
        [Key(60)]
        public byte? AlphaButt { get; set; }
        //0 off
        //1 on
        //2 end
        [Key(61)]
        public byte? Center { get; set; }
        [Key(62)]
        public int? Ground { get; set; }
        [Key(63)]
        public bool? D360 { get; set; }
        [Key(64)]
        public int? Rotate { get; set; }
        [Key(65)]
        public int? D360Adjust { get; set; }
        //0 off
        //1 on
        //2 roll
        [Key(66)]
        public byte? Direct { get; set; }
        [Key(67)]
        public int? ResizeW { get; set; }
        [Key(68)]
        public int? ResizeH { get; set; }
        [Key(69)]
        public byte? ResizeA { get; set; }
        [Key(70)]
        public int? ResizeS { get; set; }
        [Key(71)]
        public int? ResizeInterval { get; set; }
        [Key(72)]
        public int? ResizeStart { get; set; }
        [Key(73)]
        public int? ResizeReverse { get; set; }

        [Key(74)]
        public bool? ForceFire { get; set; }
        [Key(75)]
        public int? SlowPercentage { get; set; }
        [Key(76)]
        public int? SlowTime { get; set; }
        [Key(77)]
        public int? Slide { get; set; }
        [Key(78)]
        public int? SlideSpeed { get; set; }
        [Key(79)]
        public int? SlideDelay { get; set; }
        [Key(80)]
        public int? SlideStamp { get; set; }
        [Key(81)]
        public bool? SlideStampOn { get; set; }
        [Key(82)]
        public int? WaitTimeMin { get; set; }
        [Key(83)]
        public int? WaitTimeMax { get; set; }
        [Key(84)]
        public int? Shake { get; set; }
        [Key(85)]
        public byte? RayR { get; set; }
        [Key(86)]
        public byte? RayG { get; set; }
        [Key(87)]
        public byte? RayB { get; set; }
        [Key(88)]
        public byte? RayA { get; set; }
        [Key(89)]
        public bool? ForceRay { get; set; }
        [Key(90)]
        public byte? Flash { get; set; }
        [Key(91)]
        public string FlashImage { get; set; }
        [Key(92)]
        public int? FlashAnime { get; set; }
        [Key(93)]
        public string Collision { get; set; }
        [Key(94)]
        public string AfterDeath { get; set; }
        //0 normal
        //1 summon
        //2 object
        //3 both
        [Key(95)]
        public byte? AfterDeathType { get; set; }
        [Key(96)]
        public string AfterHit { get; set; }
        [Key(97)]
        public byte? AfterHitType { get; set; }
        //1 left  false
        //2 right true
        [Key(98)]
        public bool? YorozuTurn { get; set; }
        [Key(99)]
        public int? YorozuRadius { get; set; }
        [Key(100)]
        public int? YorozuThrowMax { get; set; }
        [Key(101)]
        public string Attribute { get; set; }
        [Key(102)]
        public List<string> Add { get; } = new List<string>();
        [Key(103)]
        public bool? AddAll { get; set; }
        [Key(104)]
        public byte? AddPercentage { get; set; }
        [Key(105)]
        public int? DamageType { get; set; }
        [Key(106)]
        public int? DamageRangeAdjust { get; set; }
        //0 敵
        //1 味方
        //2 敵と味方
        //3 敵と味方
        //4 敵と自分
        //5 味方と自分
        //6 全員
        //7 自分
        [Key(107)]
        public byte? AttackUs { get; set; }
        [Key(108)]
        public bool? AllFunc { get; set; }
        [Key(109)]
        public bool? Bom { get; set; }
        //0 Number
        //1 on
        //2 origin
        //3 fix
        //4 hold
        [Key(110)]
        public byte? HomingType { get; set; }
        [Key(111)]
        public int? HomingNumber { get; set; }
        [Key(112)]
        public bool? Forward { get; set; }
        [Key(113)]
        public bool? Far { get; set; }
        [Key(114)]
        public byte? Hard { get; set; }
        [Key(115)]
        public bool? OneHit { get; set; }
        [Key(116)]
        public byte? Hard2 { get; set; }
        [Key(117)]
        public bool? OffsetOn { get; set; }
        [Key(118)]
        public List<string> Offset { get; } = new List<string>();
        [Key(119)]
        public List<string> OffsetAttribute { get; } = new List<string>();
        [Key(120)]
        public int? Knock { get; set; }
        [Key(121)]
        public int? KnockPower { get; set; }
        [Key(122)]
        public int? KnockSpeed { get; set; }
        [Key(123)]
        public int? Range { get; set; }
        [Key(124)]
        public int? RangeMin { get; set; }
        [Key(125)]
        public int? Check { get; set; }
        [Key(126)]
        public bool? Origin { get; set; }
        [Key(127)]
        public int? RandomSpace { get; set; }
        [Key(128)]
        public int? RandomSpaceMin { get; set; }
        [Key(129)]
        public int? Time { get; set; }
        [Key(130)]
        public int? Rush { get; set; }
        [Key(131)]
        public int? RushInterval { get; set; }
        [Key(132)]
        public int? RushDegree { get; set; }
        [Key(133)]
        public int? RushRandomDegree { get; set; }
        [Key(134)]
        public bool? FollowOn { get; set; }
        [Key(135)]
        public int? Follow { get; set; }
        [Key(136)]
        public int? StartDegree { get; set; }
        [Key(137)]
        public int? StartRandomDegree { get; set; }
        //0 start_degreeの±方向を五分五分のランダムにします
        //1 +
        //2 -
        //3 rushスキルで発射ごとに±を交互にずらします
        //4 start_degreeの数値角度方向に発射します
        //5 ＋start_degree～－start_degree範囲のランダムで角度をずらします
        //6 ユニットが右向きだと符号が反転します。これはswingで用います
        //7 スキル発射方向をユニットの向きにします
        [Key(138)]
        public byte? StartDegreeType { get; set; }
        [Key(139)]
        public bool? StartDegreeTurnUnit { get; set; }
        [Key(140)]
        public int? StartDegreeFix { get; set; }
        [Key(141)]
        public int? Homing2 { get; set; }
        [Key(142)]
        public int? DropDegree { get; set; }
        [Key(143)]
        public int? DropDegreeMax { get; set; }
        [Key(144)]
        public bool? SendTarget { get; set; }
        [Key(145)]
        public bool? SendImageDegree { get; set; }
        [Key(146)]
        public string Next { get; set; }
        [Key(147)]
        public string Next4 { get; set; }
        [Key(148)]
        public List<string> Next2 { get; } = new List<string>();
        [Key(149)]
        public List<string> Next3 { get; } = new List<string>();
        [Key(150)]
        public bool? NextOrder { get; set; }
        [Key(151)]
        public bool? NextLast { get; set; }
        [Key(152)]
        public bool? NextFirst { get; set; }
        [Key(153)]
        public bool? JointSkill { get; set; }
        [Key(154)]
        public int? NextInterval { get; set; }
        [Key(155)]
        public List<string> JustNext { get; } = new List<string>();
        [Key(156)]
        public List<string> PairNext { get; } = new List<string>();
        /*------------------------------------------------------------*/
        /*Summon */
        [Key(157)]
        public byte? TroopType { get; set; }
        [Key(158)]
        public int? EffectWidth { get; set; }
        [Key(159)]
        public int? EffectHeight { get; set; }
        /*Summon */
        /*Charge */
        [Key(160)]
        public int? Height_Charge_Arc { get; set; }
        [Key(161)]
        public bool? HomingOn { get; set; }
        /*Charge */
        /*Status */
        //0 メンバー
        //1 個人
        //2 リーダーとメンバー
        //3 アシスト
        //4 メンバー時に自身以外の全てのメンバー
        [Key(162)]
        public byte? StatusType { get; set; }
        [Key(163)]
        public int? YorozuHp { get; internal set; }
        [Key(164)]
        public int? YorozuMp { get; internal set; }
        [Key(165)]
        public int? YorozuAttack { get; internal set; }
        [Key(166)]
        public int? YorozuDefense { get; internal set; }
        [Key(167)]
        public int? YorozuMagic { get; internal set; }
        [Key(168)]
        public int? YorozuMagdef { get; internal set; }
        [Key(169)]
        public int? YorozuSpeed { get; internal set; }
        [Key(170)]
        public int? YorozuDext { get; internal set; }
        [Key(171)]
        public int? YorozuMove { get; internal set; }
        [Key(172)]
        public int? YorozuHprec { get; internal set; }
        [Key(173)]
        public int? YorozuMprec { get; internal set; }
        [Key(174)]
        public int? YorozuSummonMax { get; internal set; }
        [Key(175)]
        public int? YorozuDrain { get; internal set; }
        [Key(176)]
        public int? YorozuDeath { get; internal set; }
        [Key(177)]
        public int? YorozuMagsuck { get; internal set; }
        [Key(178)]
        public int? YorozuSuck { get; internal set; }
        [Key(179)]
        public int? YorozuFear { get; internal set; }
        [Key(180)]
        public int? YorozuPoi { get; internal set; }
        [Key(181)]
        public int? YorozuPara { get; internal set; }
        [Key(182)]
        public int? YorozuIll { get; internal set; }
        [Key(183)]
        public int? YorozuConf { get; internal set; }
        [Key(184)]
        public int? YorozuSil { get; internal set; }
        [Key(185)]
        public int? YorozuStone { get; internal set; }
        [Key(186)]
        public Dictionary<string, int> YorozuAttribute { get; } = new Dictionary<string, int>();
    }
}