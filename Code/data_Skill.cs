using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
namespace Wahren
{
    public enum StrType { None, Attack, Magic, AttackMagic, AttackDext, MagicDext, Fix }
    public class SkillData : ScenarioVariantData
    {
        public string DisplayName { get; set; }
        public List<string> Icon { get; } = new List<string>();
        public List<byte> IconAlpha { get; } = new List<byte>();
        public int? FkeyNumber { get; set; }
        public string Fkey { get; set; }
        public int? SortKey { get; set; }
        public int? Special { get; set; }
        public int? Delay { get; set; }
        public string GunDelayName { get; set; }
        public int? GunDelay { get; set; }
        public bool? QuickReload { get; set; }
        public string Help { get; set; }
        public bool? HideHelp { get; set; }
        public List<string> Sound { get; } = new List<string>();
        public string Message { get; set; }
        public bool? CutinOn { get; set; }
        public byte? CutinType { get; set; }
        public int? CutinTop { get; set; }
        public byte? CutinY { get; set; }
        public byte? CutinY2 { get; set; }
        public bool? CutinStop { get; set; }
        public int? CutinWakeTime { get; set; }
        public byte? CutinFlashR { get; set; }
        public byte? CutinFlashG { get; set; }
        public byte? CutinFlashB { get; set; }
        public byte? CutinPhantom { get; set; }
        public byte? CutinAlpha { get; set; }
        public int? CutinZoom { get; set; }
        public int? CutinInflate { get; set; }
        public int? CutinSlide { get; set; }
        public int? CutinTrans { get; set; }

        public int? Value { get; set; }
        public bool? Talent { get; set; }
        public string TalentSkill { get; set; }
        public int? ExpPercentage { get; set; }
        public enum FuncType { None, missile, sword, heal, summon, charge, status }
        public FuncType Type { get; set; } = FuncType.None;
        public byte? ItemType { get; set; }
        public int? ItemSort { get; set; }
        public bool? ItemNoSell { get; set; }
        public int? Price { get; set; }
        public int? MP { get; set; }
        public List<string> Friend { get; } = new List<string>();
        public StrType Strength { get; set; }
        public int? StrPercent { get; set; }
        public byte? StrRatio { get; set; }
        public string Image { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public byte? Alpha { get; set; }
        public int? Anime { get; set; }
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
        public byte? MoveType { get; set; }
        public int? Speed { get; set; }
        public byte? AlphaTip { get; set; }
        public byte? AlphaButt { get; set; }
        //0 off
        //1 on
        //2 end
        public byte? Center { get; set; }
        public int? Ground { get; set; }
        public bool? D360 { get; set; }
        public int? Rotate { get; set; }
        public int? D360Adjust { get; set; }
        //0 off
        //1 on
        //2 roll
        public byte? Direct { get; set; }
        public int? ResizeW { get; set; }
        public int? ResizeH { get; set; }
        public byte? ResizeA { get; set; }
        public int? ResizeS { get; set; }
        public int? ResizeInterval { get; set; }
        public int? ResizeStart { get; set; }
        public int? ResizeReverse { get; set; }

        public bool? ForceFire { get; set; }
        public int? SlowPercentage { get; set; }
        public int? SlowTime { get; set; }
        public int? Slide { get; set; }
        public int? SlideSpeed { get; set; }
        public int? SlideDelay { get; set; }
        public int? SlideStamp { get; set; }
        public bool? SlideStampOn { get; set; }
        public int? WaitTimeMin { get; set; }
        public int? WaitTimeMax { get; set; }
        public int? Shake { get; set; }
        public byte? RayR { get; set; }
        public byte? RayG { get; set; }
        public byte? RayB { get; set; }
        public byte? RayA { get; set; }
        public bool? ForceRay { get; set; }
        public byte? Flash { get; set; }
        public string FlashImage { get; set; }
        public int? FlashAnime { get; set; }
        public string Collision { get; set; }
        public string AfterDeath { get; set; }
        //0 normal
        //1 summon
        //2 object
        //3 both
        public byte? AfterDeathType { get; set; }
        public string AfterHit { get; set; }
        public byte? AfterHitType { get; set; }
        //1 left  false
        //2 right true
        public bool? YorozuTurn { get; set; }
        public int? YorozuRadius { get; set; }
        public int? YorozuThrowMax { get; set; }
        public string Attribute { get; set; }
        public List<string> Add { get; } = new List<string>();
        public bool? AddAll { get; set; }
        public byte? AddPercentage { get; set; }
        public int? DamageType { get; set; }
        public int? DamageRangeAdjust { get; set; }
        //0 敵
        //1 味方
        //2 敵と味方
        //3 敵と味方
        //4 敵と自分
        //5 味方と自分
        //6 全員
        //7 自分
        public byte? AttackUs { get; set; }
        public bool? AllFunc { get; set; }
        public bool? Bom { get; set; }
        //0 Number
        //1 on
        //2 origin
        //3 fix
        //4 hold
        public byte? HomingType { get; set; }
        public int? HomingNumber { get; set; }
        public bool? Forward { get; set; }
        public bool? Far { get; set; }
        public byte? Hard { get; set; }
        public bool? OneHit { get; set; }
        public byte? Hard2 { get; set; }
        public bool? OffsetOn { get; set; }
        public List<string> Offset { get; } = new List<string>();
        public List<string> OffsetAttribute { get; } = new List<string>();
        public int? Knock { get; set; }
        public int? KnockPower { get; set; }
        public int? KnockSpeed { get; set; }
        public int? Range { get; set; }
        public int? RangeMin { get; set; }
        public int? Check { get; set; }
        public bool? Origin { get; set; }
        public int? RandomSpace { get; set; }
        public int? RandomSpaceMin { get; set; }
        public int? Time { get; set; }
        public int? Rush { get; set; }
        public int? RushInterval { get; set; }
        public int? RushDegree { get; set; }
        public int? RushRandomDegree { get; set; }
        public bool? FollowOn { get; set; }
        public int? Follow { get; set; }
        public int? StartDegree { get; set; }
        public int? StartRandomDegree { get; set; }
        //0 start_degreeの±方向を五分五分のランダムにします
        //1 +
        //2 -
        //3 rushスキルで発射ごとに±を交互にずらします
        //4 start_degreeの数値角度方向に発射します
        //5 ＋start_degree～－start_degree範囲のランダムで角度をずらします
        //6 ユニットが右向きだと符号が反転します。これはswingで用います
        //7 スキル発射方向をユニットの向きにします
        public byte? StartDegreeType { get; set; }
        public bool? StartDegreeTurnUnit { get; set; }
        public int? StartDegreeFix { get; set; }
        public int? Homing2 { get; set; }
        public int? DropDegree { get; set; }
        public int? DropDegreeMax { get; set; }
        public bool? SendTarget { get; set; }
        public bool? SendImageDegree { get; set; }
        public string Next { get; set; }
        public string Next4 { get; set; }
        public List<string> Next2 { get; } = new List<string>();
        public List<string> Next3 { get; } = new List<string>();
        public bool? NextOrder { get; set; }
        public bool? NextLast { get; set; }
        public bool? NextFirst { get; set; }
        public bool? JointSkill { get; set; }
        public int? NextInterval { get; set; }
        public List<string> JustNext { get; } = new List<string>();
        public List<string> PairNext { get; } = new List<string>();
        /*------------------------------------------------------------*/
        /*Summon */
        public byte? TroopType { get; set; }
        public int? EffectWidth { get; set; }
        public int? EffectHeight { get; set; }
        /*Summon */
        /*Charge */
        public int? Height_Charge_Arc { get; set; }
        public bool? HomingOn { get; set; }
        /*Charge */
        /*Status */
        //0 メンバー
        //1 個人
        //2 リーダーとメンバー
        //3 アシスト
        //4 メンバー時に自身以外の全てのメンバー
        public byte? StatusType { get; set; }
        public int? YorozuHp { get; internal set; }
        public int? YorozuMp { get; internal set; }
        public int? YorozuAttack { get; internal set; }
        public int? YorozuDefense { get; internal set; }
        public int? YorozuMagic { get; internal set; }
        public int? YorozuMagdef { get; internal set; }
        public int? YorozuSpeed { get; internal set; }
        public int? YorozuDext { get; internal set; }
        public int? YorozuMove { get; internal set; }
        public int? YorozuHprec { get; internal set; }
        public int? YorozuMprec { get; internal set; }
        public int? YorozuSummonMax { get; internal set; }
        public int? YorozuDrain { get; internal set; }
        public int? YorozuDeath { get; internal set; }
        public int? YorozuMagsuck { get; internal set; }
        public int? YorozuSuck { get; internal set; }
        public int? YorozuFear { get; internal set; }
        public int? YorozuPoi { get; internal set; }
        public int? YorozuPara { get; internal set; }
        public int? YorozuIll { get; internal set; }
        public int? YorozuConf { get; internal set; }
        public int? YorozuSil { get; internal set; }
        public int? YorozuStone { get; internal set; }
        public Dictionary<string, int> YorozuAttribute { get; } = new Dictionary<string, int>();

        /*Status */

        public SkillData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
    }
}