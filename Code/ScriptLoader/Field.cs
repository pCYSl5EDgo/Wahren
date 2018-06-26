using System;
using System.Collections.Concurrent;
using Wahren.Specific;

namespace Wahren
{
    public static partial class ScriptLoader
    {
        public static ScenarioFolder Folder { get; set; }
        public static ScenarioData2[] Scenarios => scenarios;

        internal static readonly ConcurrentDictionary<string, ScenarioData> ScenarioDictionary = new ConcurrentDictionary<string, ScenarioData>();
        private static ScenarioData2[] scenarios = null;
        public static readonly ConcurrentDictionary<string, GenericUnitData> GenericUnitDictionary = new ConcurrentDictionary<string, GenericUnitData>();
        public static readonly ConcurrentDictionary<string, UnitData> UnitDictionary = new ConcurrentDictionary<string, UnitData>();
        public static readonly ConcurrentDictionary<string, byte> BaseClassKeyDictionary = new ConcurrentDictionary<string, byte>();
        public static readonly ConcurrentDictionary<string, PowerData> PowerDictionary = new ConcurrentDictionary<string, PowerData>();
        public static readonly ConcurrentDictionary<string, SpotData> SpotDictionary = new ConcurrentDictionary<string, SpotData>();
        public static readonly ConcurrentDictionary<string, RaceData> RaceDictionary = new ConcurrentDictionary<string, RaceData>();
        public static readonly ConcurrentDictionary<string, StoryData> StoryDictionary = new ConcurrentDictionary<string, StoryData>();
        public static readonly ConcurrentDictionary<string, DungeonData> DungeonDictionary = new ConcurrentDictionary<string, DungeonData>();
        public static readonly ConcurrentDictionary<string, FieldData> FieldDictionary = new ConcurrentDictionary<string, FieldData>();
        public static readonly ConcurrentDictionary<string, EventData> EventDictionary = new ConcurrentDictionary<string, EventData>();
        public static readonly ConcurrentDictionary<string, ObjectData> ObjectDictionary = new ConcurrentDictionary<string, ObjectData>();
        public static readonly ConcurrentDictionary<string, SkillData> SkillDictionary = new ConcurrentDictionary<string, SkillData>();
        public static readonly ConcurrentDictionary<string, SkillSetData> SkillSetDictionary = new ConcurrentDictionary<string, SkillSetData>();
        public static readonly ConcurrentDictionary<string, VoiceData> VoiceDictionary = new ConcurrentDictionary<string, VoiceData>();
        public static readonly ConcurrentDictionary<string, MoveTypeData> MoveTypeDictionary = new ConcurrentDictionary<string, MoveTypeData>();
        public static readonly SoundData Sound = new SoundData();
        public static readonly ContextData Context = new ContextData();
        public static readonly AttributeData Attribute = new AttributeData();
        public static readonly WorkspaceData Workspace = new WorkspaceData();
        public static readonly DetailData Detail = new DetailData();
    }
}