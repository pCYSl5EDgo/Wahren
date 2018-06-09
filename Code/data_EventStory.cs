using System;
using System.Collections.Generic;
using MessagePack;

namespace Wahren
{
    [Union(0, typeof(StoryData))]
    [Union(1, typeof(EventData))]
    [Union(2, typeof(ScenarioData))]
    public interface IScript
    {
        List<LexicalTree> Script { get; }
    }
    [MessagePackObject]
    public sealed class StoryData : InheritData, IScript
    {
        [Key(5)]
        public List<LexicalTree> Script { get; } = new List<LexicalTree>();
        [Key(6)]
        public List<string> Friend { get; } = new List<string>();
        [Key(7)]
        public bool? Fight { get; set; }
        [Key(8)]
        public bool? Politics { get; set; }
        [Key(9)]
        public bool? Pre { get; set; }
        public StoryData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
    }
    [MessagePackObject]
    public sealed class EventData : InheritData, IScript
    {
        [Key(5)]
        public List<LexicalTree> Script { get; } = new List<LexicalTree>();

        public EventData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        [Key(6)]
        public string BackGround { get; set; }
        [Key(7)]
        public int? Width { get; set; }
        [Key(8)]
        public int? Height { get; set; }
        [Key(9)]
        public string BGM { get; set; }
        [Key(10)]
        public byte? Volume { get; set; }

        [Key(11)]
        public string Map { get; set; }
        //blue 0
        //red 1
        [Key(12)]
        public byte? Handle { get; set; }
        [Key(13)]
        public bool? Disperse { get; set; }
        [Key(14)]
        public bool? CastleBattle { get; set; }
        [Key(15)]
        public int? Castle { get; set; }
        [Key(16)]
        public string Title { get; set; }
        [Key(17)]
        public int? Limit { get; set; }
        [Key(18)]
        public bool? IsBlind { get; set; }
        [Key(19)]
        public byte? Blind { get; set; }
    }
    [MessagePackObject]
    public partial class ScenarioData : InheritData, IScript
    {
        [Key(5)]
        public List<LexicalTree> Script { get; } = new List<LexicalTree>();
        public ScenarioData(string name, string inherit, string file, int line) : base(name, inherit, file, line) { }
        public override string ToString() => DisplayName;
        [Key(6)]
        public string DisplayName { get; set; }
        [Key(7)]
        public string WorldMapPath { get; set; }
        [Key(8)]
        public int? X { get; set; }
        [Key(9)]
        public int? Y { get; set; }
        [Key(10)]
        public string DescriptionText { get; set; }
        [Key(11)]
        public string BeginText { get; set; }

        [Key(12)]
        public List<string> Power { get; } = new List<string>();
        [Key(13)]
        public List<string> Spot { get; } = new List<string>();
        [Key(14)]
        public List<string> Roamer { get; } = new List<string>();

        [Key(15)]
        public string WorldEvent { get; set; }
        [Key(16)]
        public string FightEvent { get; set; }
        [Key(17)]
        public string PoliticsEvent { get; set; }

        [Key(18)]
        public byte? WarCapacity { get; set; }
        [Key(19)]
        public byte? SpotCapacity { get; set; }

        [Key(20)]
        public int? GainPercent { get; set; }
        [Key(21)]
        public byte? SupportRange { get; set; }
        [Key(22)]
        public byte? MyRange { get; set; }
        [Key(23)]
        public byte? MyHelpRange { get; set; }

        [Key(24)]
        public int? BaseLevelUp { get; set; }
        [Key(25)]
        public int? MonsterLevel { get; set; }
        [Key(26)]
        public int? TrainingUp { get; set; }
        [Key(27)]
        public byte? ActorPercent { get; set; }

        [Key(28)]
        public int? SortKey { get; set; }
        [Key(29)]
        public bool? IsDefaulEnding { get; set; }

        [Key(30)]
        public ContextData.Order? Order { get; set; }

        [Key(31)]
        public bool? IsEnable { get; set; }
        [Key(32)]
        public bool? IsPlayableAsTalent { get; set; }
        [Key(33)]
        public bool? IsAbleToMerce { get; set; }

        [Key(34)]
        public bool? IsNoAutoSave { get; set; }

        [Key(35)]
        public string[] MenuButtonPower { get; set; } = new string[] { "ﾀｰﾝ終了", "ﾀｰﾝ委任", "人材雇用", "外交", "内政", "探索", "静観", "機能" };
        [Key(36)]
        public string[] MenuButtonTalent { get; set; } = new string[] { "行動終了", "状態", "放浪or旗上げ", "陪臣雇用", "キャンプ", "探索", "静観", "機能" };

        [Key(37)]
        public bool? IsNoZone { get; set; }
        [Key(38)]
        public string ZoneName { get; set; }

        [Key(39)]
        public Dictionary<string, ValueTuple<string, string>> PoliticsData { get; set; } = new Dictionary<string, ValueTuple<string, string>>();
        [Key(40)]
        public Dictionary<string, ValueTuple<string, string>> CampingData { get; } = new Dictionary<string, ValueTuple<string, string>>();

        [Key(41)]
        public ValueTuple<string, int>[] ItemWindowTab { get; } = new ValueTuple<string, int>[7];
        [Key(42)]
        public List<string> ItemSale { get; } = new List<string>();
        [Key(43)]
        public bool? IsItemLimit { get; set; }

        [Key(44)]
        public List<string> PlayerInitialItem { get; } = new List<string>();
    }
}