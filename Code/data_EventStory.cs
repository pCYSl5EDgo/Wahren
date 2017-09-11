using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
namespace Wahren
{
    public interface IScript
    {
        List<LexicalTree> Script { get; }
    }
    public class StoryData : InheritData, IScript
    {
        public List<string> Friend { get; } = new List<string>();
        public List<LexicalTree> Script { get; } = new List<LexicalTree>();
        public bool? Fight { get; set; }
        public bool? Politics { get; set; }
        public bool? Pre { get; set; }
        public StoryData(string name, string inherit) : base(name, inherit) { }
    }
    public class EventData : InheritData, IScript
    {
        public List<LexicalTree> Script { get; } = new List<LexicalTree>();

        public EventData(string name, string inherit) : base(name, inherit) { }
        public string BackGround { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string BGM { get; set; }
        public byte? Volume { get; set; }

        public string Map { get; set; }
        //blue 0
        //red 1
        public byte? Handle { get; set; }
        public bool? Disperse { get; set; }
        public bool? CastleBattle { get; set; }
        public int? Castle { get; set; }
        public string Title { get; set; }
        public int? Limit { get; set; }
        public bool? IsBlind { get; set; }
        public byte? Blind { get; set; }
    }
    public partial class ScenarioData : InheritData, IScript
    {
        public List<LexicalTree> Script { get; } = new List<LexicalTree>();
        public ScenarioData(string name, string inherit) : base(name, inherit) { }
        public override string ToString() => DisplayName;
        public string DisplayName { get; set; }
        public string WorldMapPath { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public string DescriptionText { get; set; }
        public string BeginText { get; set; }

        public List<string> Power { get; } = new List<string>();
        public List<string> Spot { get; } = new List<string>();
        public List<string> Roamer { get; } = new List<string>();

        public string WorldEvent { get; set; }
        public string FightEvent { get; set; }
        public string PoliticsEvent { get; set; }

        public byte? WarCapacity { get; set; }
        public byte? SpotCapacity { get; set; }

        public int? GainPercent { get; set; }
        public byte? SupportRange { get; set; }
        public byte? MyRange { get; set; }
        public byte? MyHelpRange { get; set; }

        public int? BaseLevelUp { get; set; }
        public int? MonsterLevel { get; set; }
        public int? TrainingUp { get; set; }
        public byte? ActorPercent { get; set; }

        public int? SortKey { get; set; }
        public bool? IsDefaulEnding { get; set; }

        public ContextData.Order? Order { get; set; }

        public bool? IsEnable { get; set; }
        public bool? IsPlayableAsTalent { get; set; }
        public bool? IsAbleToMerce { get; set; }

        public bool? IsNoAutoSave { get; set; }

        public string[] MenuButtonPower { get; set; } = new string[] { "ﾀｰﾝ終了", "ﾀｰﾝ委任", "人材雇用", "外交", "内政", "探索", "静観", "機能" };
        public string[] MenuButtonTalent { get; set; } = new string[] { "行動終了", "状態", "放浪or旗上げ", "陪臣雇用", "キャンプ", "探索", "静観", "機能" };

        public bool? IsNoZone { get; set; }
        public string ZoneName { get; set; }

        public Dictionary<string, Tuple<string, string>> PoliticsData { get; set; } = new Dictionary<string, Tuple<string, string>>();
        public Dictionary<string, Tuple<string, string>> CampingData { get; } = new Dictionary<string, Tuple<string, string>>();

        public Dictionary<string, int> ItemWindowTab { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemSale { get; } = new Dictionary<string, int>();
        public bool? IsItemLimit { get; set; }

        public List<string> PlayerInitialItem { get; } = new List<string>();
    }
}