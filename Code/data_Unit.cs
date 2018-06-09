using System;
using System.Collections.Generic;

namespace Wahren
{
    public class GenericUnitData : CommonUnitData
    {
        public GenericUnitData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public GenericUnitData() : base("", "", "", 0) { }
        public string BaseClassKey { get; set; }
        public bool? IsUnique { get; set; }
        public ValueTuple<string, int>? Change { get; set; }
    }
    public sealed class UnitData : CommonUnitData
    {
        public UnitData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public UnitData() : base("", "", "", 0) { }
        public bool? IsTalent { get; set; }
        public string BGM { get; set; }
        public byte? Volume { get; set; }
        public string Picture { get; set; }
        public byte? PictureDetail { get; set; }
        public byte? PictureMenu { get; set; }
        public int? PictureFloor { get; set; }
        public int? PictureShift { get; set; }
        public int? PictureShiftUp { get; set; }
        public byte? PictureCenter { get; set; }
        public string PictureBack { get; set; }
        public byte? AlivePercentage { get; set; }
        public int? Medical { get; set; }
        public Dictionary<int, List<string>> LeaderSkill { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> AssistSkill { get; set; } = new Dictionary<int, List<string>>();

        public byte? Yabo { get; set; }
        public byte? Kosen { get; set; }
        public byte? Align { get; set; }
        public List<string> Enemy { get; set; } = new List<string>();
        public ValueTuple<string, byte>? Loyal { get; set; }
        public string PowerDisplayName { get; set; }
        public string Flag { get; set; }
        public List<string> Staff { get; set; } = new List<string>();
        public bool? Diplomacy { get; set; }
        public Dictionary<string, int> CastleGuard { get; set; } = new Dictionary<string, int>();
        public bool? IsActor { get; set; }
        public bool? IsEnableSelect { get; set; }
        public int? EnableTurn { get; set; }
        public int? EnableTurnMax { get; set; }
        //off 0
        //on 1
        //home 2
        public byte? Fix { get; set; }
        public List<string> Home { get; set; } = new List<string>();
        public bool? IsNoEscape { get; set; }
        public bool? IsNoRemoveUnit { get; set; }
        public bool? IsNoEmployUnit { get; set; }
        public bool? IsNoItemUnit { get; set; }
        //on 0
        //power 1
        //fix 2
        public byte? ArbeitType { get; set; }
        public byte? ArbeitPercentage { get; set; }
        public int? ArbeitCapacity { get; set; }
        public string Help { get; set; }
        public string Join { get; set; }
        public string Dead { get; set; }
        public string Retreat { get; set; }
        public string Break { get; set; }
        public string VoiceType { get; set; }
        //blue 0/off
        //red 1/on
        public byte? Team { get; set; }
        public bool? KeepForm { get; set; }
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
        public byte? ActiveType { get; set; }
        public int? ActiveRange { get; set; }
        public int? ActiveTime { get; set; }
        public string ActiveTroop { get; set; }
        public ValueTuple<int, int, int, int>? ActiveRect { get; set; }
    }
    public class CommonUnitData : ScenarioVariantData
    {
        public CommonUnitData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public string DisplayName { get; set; }
        // 0 neuter
        // 1 male
        // 2 female
        public byte? Sex { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public int? Radius { get; set; }
        public byte? RadiusPress { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public byte? Alpha { get; set; }
        public string Image { get; set; }
        public string Image2 { get; set; }
        public bool? IsTkool { get; set; }
        public int? Price { get; set; }
        public int? Cost { get; set; }
        public int? HasEXP { get; set; }
        public bool? IsFriendAllClass { get; set; }
        public bool? IsFriendAllRace { get; set; }
        public List<string> Friends { get; set; } = new List<string>();
        public int? FriendExCount { get; set; }
        public List<string> FriendEx1 { get; set; } = new List<string>();
        public List<string> FriendEx2 { get; set; } = new List<string>();
        public List<string> Merce { get; set; } = new List<string>();
        public bool? IsSameFriend { get; set; }
        public bool? IsSameCall { get; set; }
        public bool? IsSameCallBaseUp { get; set; }
        public bool? IsSameSex { get; set; }
        public List<string> Member { get; set; } = new List<string>();
        public int? Level { get; set; }
        public int? Hp { get; set; }
        public int? Mp { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }
        public int? Magic { get; set; }
        public int? Magdef { get; set; }
        public int? Speed { get; set; }
        public int? Dext { get; set; }
        public int? Move { get; set; }
        public int? HpRec { get; set; }
        public int? MpRec { get; set; }
        public int? summon_max { get; set; }
        public int? summon_level { get; set; }
        public int? heal_max { get; set; }
        public int? attack_max { get; set; }
        public int? defense_max { get; set; }
        public int? magic_max { get; set; }
        public int? magdef_max { get; set; }
        public int? speed_max { get; set; }
        public int? dext_max { get; set; }
        public int? move_max { get; set; }
        public int? hprec_max { get; set; }
        public int? mprec_max { get; set; }
        public List<string> Skill { get; set; } = new List<string>();
        public List<string> Skill2 { get; set; } = new List<string>();
        public List<string> DeleteSkill { get; set; } = new List<string>();
        public Dictionary<int, List<string>> Learn { get; set; } = new Dictionary<int, List<string>>();
        public List<string> DeleteSkill2 { get; set; } = new List<string>();
        public Dictionary<string, int> Consti { get; set; } = new Dictionary<string, int>();
        public string MoveType { get; set; }
        //front 10
        //back 0
        public byte? DefenseLine { get; set; }
        public bool? IsSatellite { get; set; }
        public int? Satellite { get; set; }
        public bool? IsBeast { get; set; }
        public int? NoKnock { get; set; }
        public bool? IsNoCover { get; set; }
        public bool? IsViewUnit { get; set; }
        public bool? IsElementLost { get; set; }
        public int? AttackRange { get; set; }
        public int? EscapeRange { get; set; }
        public int? EscapeRun { get; set; }
        public int? HandRange { get; set; }
        public int? WakeRange { get; set; }
        public int? ViewRange { get; set; }
        public int? CavalryRange { get; set; }
        public int? level_max { get; set; }
        public Dictionary<string, int> Multi { get; set; } = new Dictionary<string, int>();
        public int? exp { get; set; }
        public int? exp_mul { get; set; }
        public int? exp_max { get; set; }
        public int? hpUp { get; set; }
        public int? mpUp { get; set; }
        public int? attackUp { get; set; }
        public int? defenseUp { get; set; }
        public int? magicUp { get; set; }
        public int? magdefUp { get; set; }
        public int? speedUp { get; set; }
        public int? dextUp { get; set; }
        public int? moveUp { get; set; }
        public int? hprecUp { get; set; }
        public int? mprecUp { get; set; }
        public int? hpMax { get; set; }
        public int? mpMax { get; set; }
        public int? attackMax { get; set; }
        public int? defenseMax { get; set; }
        public int? magicMax { get; set; }
        public int? magdefMax { get; set; }
        public int? speedMax { get; set; }
        public int? dextMax { get; set; }
        public int? moveMax { get; set; }
        public int? hprecMax { get; set; }
        public int? mprecMax { get; set; }
        public string Scream { get; set; }
        public int? LostCorpse { get; set; }
        public int? FreeMove { get; set; }
        //off 0
        //on 1
        //roam 2
        public byte? AddVassal { get; set; }
        public bool? IsBrave { get; set; }
        public bool? IsHandle { get; set; }
        public int? SortKey { get; set; }
        public byte? Brave { get; set; }
        public string Face { get; set; }
        public List<string> Item { get; set; } = new List<string>();
        public bool? IsNoTraining { get; set; }
        public string DeadEvent { get; set; }
        //1: on
        //2: fix
        //3: erase
        //4: unique
        public byte? Politics { get; set; }
    }
}