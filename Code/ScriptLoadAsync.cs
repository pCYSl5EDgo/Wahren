using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Wahren.Specific;

namespace Wahren
{
    public static class ScriptLoader
    {
        public static ScenarioFolder Folder { get; set; }
        public static ScenarioData2[] Scenarios => scenarios == null ? throw new NullReferenceException() : scenarios;

        internal static readonly ConcurrentDictionary<string, ScenarioData> ScenarioDictionary = new ConcurrentDictionary<string, ScenarioData>();
        private static Specific.ScenarioData2[] scenarios = null;
        public static readonly ConcurrentDictionary<string, GenericUnitData> GenericUnitDictionary = new ConcurrentDictionary<string, GenericUnitData>();
        public static readonly ConcurrentDictionary<string, UnitData> UnitDictionary = new ConcurrentDictionary<string, UnitData>();
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
        //Vahren.exeと同階層にあるシナリオフォルダを指定してネ
        //シナリオフォルダのエンコード（Shift-JIS）や英文モードとかを解析するヨ
        public static void InitializeComponent(string folderPath, bool isDebug = false)
        {
            Folder = new ScenarioFolder(folderPath, isDebug);
            //Scriptフォルダ以下の.dat全てをかるーく非同期に解析するよ
            //ここである程度構造体の継承も処理するが別ファイルからのは継承できなかったりする
            Task.WaitAll(LoadAllAsync());
            //全ファイルが出揃ったら解決できていない継承を解決する
            //名前は良い名前が思い浮かんだら変更する予定
            ResolveDependencyWithAllFiles();
            var _scenarios = ScenarioDictionary.ToArray();
            scenarios = new Specific.ScenarioData2[_scenarios.Length];
            var wait = new Task[_scenarios.Length];
            for (int i = 0; i < _scenarios.Length; i++)
            {
                scenarios[i] = new Specific.ScenarioData2(_scenarios[i].Value);
                wait[i] = scenarios[i].LoadingDone;
            }
            Task.WaitAll(wait);
        }

        public static Task[] LoadAllAsync()
        {
            if (Folder == null) throw new ApplicationException("Folder must not be null!");
            var ans = new Task[Folder.Script_Dat.Count];
            for (int i = 0; i < ans.Length; i++)
                ans[i] = String.Intern(Folder.Script_Dat[i]).LoadAsync(Folder.Encoding, Folder.IsEnglishMode, Folder.IsDebug);
            return ans;
        }
        public static async Task LoadAsync(this string scriptFile, Encoding encoding, bool englishMode, bool isDebug)
        {
            foreach (var tree in (await scriptFile.Parse(encoding, englishMode)).Parse(isDebug))
            {
                var inh = (tree as IInherit)?.Inherit;
                if (tree.Name == inh) throw new CircularReferenceException(tree.DebugInfo);
                switch ((tree as IStructureDataType).Structure)
                {
                    case StructureDataType.Attribute:
                        foreach (var attribute in SelectAssign(tree))
                        {
                            ValueTuple<string, int> tpl;
                            if (attribute.Content.Count >= 2)
                                tpl = new ValueTuple<string, int>(attribute.Content[0].Content, (int)attribute.Content[1].Number);
                            else
                                tpl = new ValueTuple<string, int>(attribute.Content[0].Content, 0);
                            Attribute.TryAdd(attribute.Name, tpl);
                        }
                        break;
                    case StructureDataType.Context:
                        ContextParse(SelectAssign(tree));
                        break;
                    case StructureDataType.Detail:
                        foreach (var assign in SelectAssign(tree))
                        {
                            Detail.TryAdd(assign.Name, assign.Content.Aggregate(new StringBuilder(), (buf, _) => _.Symbol1 == '$' ? buf.AppendLine() : buf.Append(_.ToString()), buf => buf.ToString()));
                        }
                        break;
                    case StructureDataType.Sound:
                        foreach (var assign in SelectAssign(tree))
                        {
                            if (assign.Name == "default") { Sound.Default = (int)assign.Content[0].Number; }
                            else { Sound.TryAdd(assign.Name, (int)assign.Content[0].Number); }
                        }
                        break;
                    case StructureDataType.Workspace:
                        foreach (var assign in SelectAssign(tree))
                            Workspace.TryAdd(assign.Name, assign.ToString());
                        break;
                    case StructureDataType.Movetype:
                        var movetype = new MoveTypeData(tree.Name, tree.File, tree.Line);
                        foreach (var assign in SelectAssign(tree))
                        {
                            switch (assign.Name)
                            {
                                case "name":
                                    movetype.DisplayName = InsertString(assign, movetype.FilledWithNull);
                                    break;
                                case "help":
                                    movetype.Help = InsertString(assign, movetype.FilledWithNull);
                                    break;
                                case "hide_help":
                                    movetype.HideHelp = InsertBool(assign, movetype.FilledWithNull);
                                    break;
                                case "consti":
                                    movetype.FieldMoveDictionary.Clear();
                                    if (assign.Content[0].Symbol1 == '@')
                                    {
                                        movetype.FilledWithNull.Add("consti");
                                        break;
                                    }
                                    for (int i = 0; (i << 1) < assign.Content.Count; i++)
                                    {
                                        movetype.FieldMoveDictionary[assign.Content[i << 1].ToLowerString()] = (byte)assign.Content[(i << 1) + 1].Number;
                                    }
                                    break;
                                default: throw new ApplicationException($"{assign.File}:{assign.Line}");
                            }
                        }
                        MoveTypeDictionary.TryAdd(tree.Name, movetype);
                        break;
                    case StructureDataType.Event:
                    case StructureDataType.Deploy:
                    case StructureDataType.Fight:
                    case StructureDataType.Function:
                    case StructureDataType.World:
                        var eventData = new EventData(tree.Name, inh, tree.File, tree.Line);
                        Parse(tree.Children, eventData);
                        while (!string.IsNullOrEmpty(eventData.Inherit))
                        {
                            if (EventDictionary.TryGetValue(eventData.Inherit, out var inheritEvent))
                            {
                                if (eventData.Inherit == inheritEvent.Inherit) throw new CircularReferenceException(inheritEvent.DebugInfo + ':' + inheritEvent.Inherit);
                                eventData.Inherit = inheritEvent.Inherit;
                                Resolve(eventData, inheritEvent);
                            }
                            else break;
                        }
                        EventDictionary.GetOrAdd(eventData.Name, eventData);
                        break;
                    case StructureDataType.Story:
                        var story = new StoryData(tree.Name, inh, tree.File, tree.Line);
                        Parse(tree.Children, story);
                        while (!string.IsNullOrEmpty(story.Inherit))
                        {
                            if (StoryDictionary.TryGetValue(story.Inherit, out var inheritStory))
                            {
                                if (story.Inherit == inheritStory.Inherit) throw new CircularReferenceException(inheritStory.DebugInfo + ':' + inheritStory.Inherit);
                                story.Inherit = inheritStory.Inherit;
                                Resolve(story, inheritStory);
                            }
                            else break;
                        }
                        StoryDictionary.GetOrAdd(story.Name, story);
                        break;
                    case StructureDataType.Dungeon:
                        var dungeon = new DungeonData(tree.Name, inh, tree.File, tree.Line);
                        Parse(SelectAssign(tree), dungeon);
                        while (!string.IsNullOrEmpty(dungeon.Inherit))
                        {
                            if (DungeonDictionary.TryGetValue(dungeon.Inherit, out var inheritdungeon))
                            {
                                if (dungeon.Inherit == inheritdungeon.Inherit) throw new CircularReferenceException(inheritdungeon.DebugInfo + ':' + inheritdungeon.Inherit);
                                dungeon.Inherit = inheritdungeon.Inherit;
                                Resolve(dungeon, inheritdungeon);
                            }
                            else break;
                        }
                        DungeonDictionary.GetOrAdd(dungeon.Name, dungeon);
                        break;
                    case StructureDataType.Scenario:
                        var scenario = new ScenarioData(tree.Name, inh, tree.File, tree.Line);
                        Parse(tree.Children, scenario);
                        while (!string.IsNullOrEmpty(scenario.Inherit))
                        {
                            if (ScenarioDictionary.TryGetValue(scenario.Inherit, out var inheritscenario))
                            {
                                if (scenario.Inherit == inheritscenario.Inherit) throw new CircularReferenceException(inheritscenario.DebugInfo + ':' + inheritscenario.Inherit);
                                scenario.Inherit = inheritscenario.Inherit;
                                Resolve(scenario, inheritscenario);
                            }
                            else break;
                        }
                        ScenarioDictionary.GetOrAdd(scenario.Name, scenario);
                        break;
                    case StructureDataType.Field:
                        if (ObjectDictionary.ContainsKey(tree.Name)) throw new Exception(tree.DebugInfo);
                        var field = new FieldData(tree.Name, inh, tree.File, tree.Line);
                        Parse(SelectAssign(tree), field);
                        while (!string.IsNullOrEmpty(field.Inherit))
                        {
                            if (FieldDictionary.TryGetValue(field.Inherit, out var inheritfield))
                            {
                                if (field.Inherit == inheritfield.Inherit) throw new CircularReferenceException(inheritfield.DebugInfo + ':' + inheritfield.Inherit);
                                field.Inherit = inheritfield.Inherit;
                                Resolve(field, inheritfield);
                            }
                            else break;
                        }
                        FieldDictionary.GetOrAdd(field.Name, field);
                        break;
                    case StructureDataType.Object:
                        if (FieldDictionary.ContainsKey(tree.Name)) throw new Exception(tree.DebugInfo);
                        var object1 = new ObjectData(tree.Name, inh, tree.File, tree.Line);
                        Parse(SelectAssign(tree), object1);
                        while (!string.IsNullOrEmpty(object1.Inherit))
                        {
                            if (ObjectDictionary.TryGetValue(object1.Inherit, out var inheritobject1))
                            {
                                if (object1.Inherit == inheritobject1.Inherit) throw new CircularReferenceException(inheritobject1.DebugInfo + ':' + inheritobject1.Inherit);
                                object1.Inherit = inheritobject1.Inherit;
                                Resolve(object1, inheritobject1);
                            }
                            else break;
                        }
                        ObjectDictionary.GetOrAdd(object1.Name, object1);
                        break;
                    case StructureDataType.Power:
                        var power = new PowerData(tree.Name, inh, tree.File, tree.Line);
                        Parse(SelectAssign(tree), power);
                        while (!string.IsNullOrEmpty(power.Inherit))
                        {
                            if (PowerDictionary.TryGetValue(power.Inherit, out var inheritpower))
                            {
                                power.Inherit = inheritpower.Inherit;
                                Resolve(power, inheritpower);
                            }
                            else break;
                        }
                        PowerDictionary.GetOrAdd(power.Name, power);
                        break;
                    case StructureDataType.Race:
                        var race = new RaceData(tree.Name, inh, tree.File, tree.Line);
                        Parse(SelectAssign(tree), race);
                        while (!string.IsNullOrEmpty(race.Inherit))
                        {
                            if (RaceDictionary.TryGetValue(race.Inherit, out var inheritrace))
                            {
                                race.Inherit = inheritrace.Inherit;
                                Resolve(race, inheritrace);
                            }
                            else break;
                        }
                        RaceDictionary.GetOrAdd(race.Name, race);
                        break;
                    case StructureDataType.Skill:
                        var skill = new SkillData(tree.Name, inh, tree.File, tree.Line);
                        Parse(SelectAssign(tree), skill);
                        while (!string.IsNullOrEmpty(skill.Inherit))
                        {
                            if (SkillDictionary.TryGetValue(skill.Inherit, out var inheritskill))
                            {
                                skill.Inherit = inheritskill.Inherit;
                                Resolve(skill, inheritskill);
                            }
                            else break;
                        }
                        SkillDictionary.GetOrAdd(skill.Name, skill);
                        break;
                    case StructureDataType.Skillset:
                        var skillset = new SkillSetData(tree.Name, inh, tree.File, tree.Line);
                        Parse(SelectAssign(tree), skillset);

                        while (!string.IsNullOrEmpty(skillset.Inherit))
                        {
                            if (SkillSetDictionary.TryGetValue(skillset.Inherit, out var inheritskillset))
                            {
                                skillset.Inherit = inheritskillset.Inherit;
                                Resolve(skillset, inheritskillset);
                            }
                            else break;
                        }
                        SkillSetDictionary.GetOrAdd(skillset.Name, skillset);
                        break;
                    case StructureDataType.Spot:
                        var spot = new SpotData(tree.Name, inh, tree.File, tree.Line);
                        Parse(SelectAssign(tree), spot);
                        while (!string.IsNullOrEmpty(spot.Inherit))
                        {
                            if (SpotDictionary.TryGetValue(spot.Inherit, out var inheritspot))
                            {
                                spot.Inherit = inheritspot.Inherit;
                                Resolve(spot, inheritspot);
                            }
                            else break;
                        }
                        SpotDictionary.GetOrAdd(spot.Name, spot);
                        break;
                    case StructureDataType.Voice:
                        var voice = new VoiceData(tree.Name, inh, tree.File, tree.Line) { NoPower = true };
                        Parse(SelectAssign(tree), voice);
                        while (!string.IsNullOrEmpty(voice.Inherit))
                        {
                            if (VoiceDictionary.TryGetValue(voice.Inherit, out var inheritvoice))
                            {
                                voice.Inherit = inheritvoice.Inherit;
                                Resolve(voice, inheritvoice);
                            }
                            else break;
                        }
                        VoiceDictionary.GetOrAdd(voice.Name, voice);
                        break;
                    case StructureDataType.Class:
                        var genericunit = new GenericUnitData(tree.Name, inh, tree.File, tree.Line);
                        CommonParse<GenericUnitData>(SelectAssign(tree), genericunit);
                        Parse(genericunit);
                        while (!string.IsNullOrEmpty(genericunit.Inherit))
                        {
                            if (GenericUnitDictionary.TryGetValue(genericunit.Inherit, out var inheritgenericunit))
                            {
                                genericunit.Inherit = inheritgenericunit.Inherit;
                                Resolve(genericunit, inheritgenericunit);
                            }
                            else break;
                        }
                        GenericUnitDictionary.GetOrAdd(genericunit.Name, genericunit);
                        break;
                    case StructureDataType.Unit:
                        var unit = new UnitData(tree.Name, inh, tree.File, tree.Line);
                        CommonParse<UnitData>(SelectAssign(tree), unit);
                        Parse(unit);
                        while (!string.IsNullOrEmpty(unit.Inherit))
                        {
                            if (UnitDictionary.TryGetValue(unit.Inherit, out var inheritunit))
                            {
                                if (unit.Inherit == inheritunit.Inherit) throw new ApplicationException();
                                unit.Inherit = inheritunit.Inherit;
                                Resolve(unit, inheritunit);
                            }
                            else break;
                        }
                        UnitDictionary.GetOrAdd(unit.Name, unit);
                        break;
                }
            }
        }
        static bool resolve2nd_done = false;
        internal static void ResolveDependencyWithAllFiles()
        {
            if (resolve2nd_done) return;
            resolve2nd_done = true;
            IEnumerable<T> WHERESELECT<T>(ConcurrentDictionary<string, T> dic) where T : IInherit
            {
                return dic.Where(_ => !string.IsNullOrEmpty(_.Value.Inherit)).Select(_ => _.Value);
            }
            foreach (var item in WHERESELECT(VoiceDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (VoiceDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(SkillSetDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (SkillSetDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(SkillDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (SkillDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(ObjectDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (ObjectDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(EventDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (EventDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(FieldDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (FieldDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(DungeonDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (DungeonDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(StoryDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (StoryDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(RaceDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (RaceDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(SpotDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (SpotDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(PowerDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (PowerDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(UnitDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (UnitDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(GenericUnitDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (GenericUnitDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
            foreach (var item in WHERESELECT(ScenarioDictionary))
            {
                while (!string.IsNullOrEmpty(item.Inherit))
                {
                    if (ScenarioDictionary.TryGetValue(item.Inherit, out var inh))
                    {
                        if (item.Inherit == inh.Inherit) throw new ApplicationException();
                        item.Inherit = inh.Inherit;
                        Resolve(item, inh);
                    }
                    else break;
                }
            }
        }
        internal static void ResolveVariant<T1, T2, T3>(this Dictionary<T1, Dictionary<T2, T3>> dic1, Dictionary<T1, Dictionary<T2, T3>> dic2)
        {
            foreach (var sc in dic2)
            {
                if (dic1.TryGetValue(sc.Key, out var tmpDic))
                {
                    foreach (var item in sc.Value)
                        if (!tmpDic.ContainsKey(item.Key)) tmpDic[item.Key] = item.Value;
                }
                else dic1.Add(sc.Key, sc.Value);
            }
        }
        internal static void Resolve<T>(T data1, T data2) where T : ScenarioVariantData
        {
            if (data1 == null || data2 == null) throw new ArgumentNullException();
            switch (data1)
            {
                case GenericUnitData _1:
                    Resolve(_1, data2 as GenericUnitData);
                    break;
                case PowerData _2:
                    Resolve(_2, data2 as PowerData);
                    break;
                case SpotData _3:
                    Resolve(_3, data2 as SpotData);
                    break;
                case UnitData _4:
                    Resolve(_4, data2 as UnitData);
                    break;
            }
        }
        internal static void Resolve(VoiceData voiceData1, VoiceData voiceData2)
        {
            if (voiceData1.VoiceType.Count == 0 && voiceData2.VoiceType.Count > 0 && !voiceData1.FilledWithNull.Contains("voice_type"))
                voiceData1.VoiceType.AddRange(voiceData2.VoiceType);
            if (voiceData1.DeleteVoiceType.Count == 0 && voiceData2.DeleteVoiceType.Count > 0 && !voiceData1.FilledWithNull.Contains("delskill"))
                voiceData1.DeleteVoiceType.AddRange(voiceData2.DeleteVoiceType);
            if (voiceData1.PowerVoice.Count == 0 && voiceData2.PowerVoice.Count > 0 && !voiceData1.FilledWithNull.Contains("power"))
                voiceData1.PowerVoice.AddRange(voiceData2.PowerVoice);
            if (voiceData1.SpotVoice.Count == 0 && voiceData2.SpotVoice.Count > 0 && !voiceData1.FilledWithNull.Contains("spot"))
                voiceData1.SpotVoice.AddRange(voiceData2.SpotVoice);
        }
        internal static void Resolve(SkillSetData skillSetData1, SkillSetData skillSetData2)
        {
            if (skillSetData1.BackIconPath == null && skillSetData2.BackIconPath != null && !skillSetData1.FilledWithNull.Contains("back"))
                skillSetData1.BackIconPath = skillSetData2.BackIconPath;
            if (skillSetData1.DisplayName == null && skillSetData2.DisplayName != null && !skillSetData1.FilledWithNull.Contains("name"))
                skillSetData1.DisplayName = skillSetData2.DisplayName;
            if (skillSetData1.MemberSkill.Count == 0 && skillSetData2.MemberSkill.Count > 0 && !skillSetData1.FilledWithNull.Contains("member"))
                skillSetData1.MemberSkill.AddRange(skillSetData2.MemberSkill);
            skillSetData1.VariantData.ResolveVariant(skillSetData2.VariantData);
        }
        internal static void Resolve(SkillData skillData1, SkillData skillData2)
        {
            skillData1.VariantData.ResolveVariant(skillData2.VariantData);
            if (skillData1.DisplayName == null && skillData2.DisplayName != null && !skillData1.FilledWithNull.Contains("name"))
                skillData1.DisplayName = skillData2.DisplayName;
            if (skillData1.Icon.Count == 0 && skillData2.Icon.Count > 0 && !skillData1.FilledWithNull.Contains("icon"))
            {
                skillData1.Icon.AddRange(skillData2.Icon);
                skillData1.IconAlpha.AddRange(skillData2.IconAlpha);
            }
            if (skillData1.Fkey == null && skillData2.Fkey != null && !skillData1.FilledWithNull.Contains("fkey"))
            {
                skillData1.FkeyNumber = skillData2.FkeyNumber;
                skillData1.Fkey = skillData2.Fkey;
            }
            if (skillData1.SortKey == null && skillData2.SortKey != null && !skillData1.FilledWithNull.Contains("sortkey"))
                skillData1.SortKey = skillData2.SortKey;
            if (skillData1.Special == null && skillData2.Special != null && !skillData1.FilledWithNull.Contains("special"))
                skillData1.Special = skillData2.Special;
            if (skillData1.Delay == null && skillData2.Delay != null && !skillData1.FilledWithNull.Contains("delay"))
                skillData1.Delay = skillData2.Delay;
            if (skillData1.GunDelayName == null && skillData2.GunDelayName != null && !skillData1.FilledWithNull.Contains("gun_delay"))
            {
                skillData1.GunDelayName = skillData2.GunDelayName;
                skillData1.GunDelay = skillData2.GunDelay;
            }
            if (skillData1.QuickReload == null && skillData2.QuickReload != null && !skillData1.FilledWithNull.Contains("quickreload"))
                skillData1.QuickReload = skillData2.QuickReload;
            if (skillData1.Help == null && skillData2.Help != null && !skillData1.FilledWithNull.Contains("help"))
                skillData1.Help = skillData2.Help;
            if (skillData1.HideHelp == null && skillData2.HideHelp != null && !skillData1.FilledWithNull.Contains("hide_help"))
                skillData1.HideHelp = skillData2.HideHelp;
            if (skillData1.Sound.Count == 0 && skillData2.Sound.Count > 0 && !skillData1.FilledWithNull.Contains("sound"))
                skillData1.Sound.AddRange(skillData2.Sound);
            if (skillData1.Message == null && skillData2.Message != null && !skillData1.FilledWithNull.Contains("msg"))
                skillData1.Message = skillData2.Message;
            if (!skillData1.FilledWithNull.Contains("cutin"))
            {
                if (skillData1.CutinAlpha == null && skillData2.CutinAlpha != null) skillData1.CutinAlpha = skillData2.CutinAlpha;
                if (skillData1.CutinFlashB == null && skillData2.CutinFlashB != null) skillData1.CutinFlashB = skillData2.CutinFlashB;
                if (skillData1.CutinFlashG == null && skillData2.CutinFlashG != null) skillData1.CutinFlashG = skillData2.CutinFlashG;
                if (skillData1.CutinFlashR == null && skillData2.CutinFlashR != null) skillData1.CutinFlashR = skillData2.CutinFlashR;
                if (skillData1.CutinPhantom == null && skillData2.CutinPhantom != null) skillData1.CutinPhantom = skillData2.CutinPhantom;
                if (skillData1.CutinSlide == null && skillData2.CutinSlide != null) skillData1.CutinSlide = skillData2.CutinSlide;
                if (skillData1.CutinStop == null && skillData2.CutinStop != null) skillData1.CutinStop = skillData2.CutinStop;
                if (skillData1.CutinTop == null && skillData2.CutinTop != null) skillData1.CutinTop = skillData2.CutinTop;
                if (skillData1.CutinTrans == null && skillData2.CutinTrans != null) skillData1.CutinTrans = skillData2.CutinTrans;
                if (skillData1.CutinType == null && skillData2.CutinType != null) skillData1.CutinType = skillData2.CutinType;
                if (skillData1.CutinWakeTime == null && skillData2.CutinWakeTime != null) skillData1.CutinWakeTime = skillData2.CutinWakeTime;
                if (skillData1.CutinY == null && skillData2.CutinY != null) skillData1.CutinY = skillData2.CutinY;
                if (skillData1.CutinY2 == null && skillData2.CutinY2 != null) skillData1.CutinY2 = skillData2.CutinY2;
                if (skillData1.CutinZoom == null && skillData2.CutinZoom != null) skillData1.CutinZoom = skillData2.CutinZoom;
            }
            if (skillData1.Value == null && skillData2.Value != null && !skillData1.FilledWithNull.Contains("value"))
                skillData1.Value = skillData2.Value;
            if (skillData1.Talent == null && skillData2.Talent != null && !skillData1.FilledWithNull.Contains("talent"))
            {
                skillData1.Talent = skillData2.Talent;
                skillData1.TalentSkill = skillData2.TalentSkill;
            }
            if (skillData1.ExpPercentage == null && skillData2.ExpPercentage != null && !skillData1.FilledWithNull.Contains("exp_per"))
                skillData1.ExpPercentage = skillData2.ExpPercentage;
            if (skillData1.Type == SkillData.FuncType.None && skillData2.Type != SkillData.FuncType.None && !skillData1.FilledWithNull.Contains("func"))
                skillData1.Type = skillData2.Type;
            if (skillData1.ItemType == null && skillData2.ItemType != null && !skillData1.FilledWithNull.Contains("item_type"))
                skillData1.ItemType = skillData2.ItemType;
            if (skillData1.ItemSort == null && skillData2.ItemSort != null && !skillData1.FilledWithNull.Contains("item_sort"))
                skillData1.ItemSort = skillData2.ItemSort;
            if (skillData1.ItemNoSell == null && skillData2.ItemNoSell != null && !skillData1.FilledWithNull.Contains("item_nosell"))
                skillData1.ItemNoSell = skillData2.ItemNoSell;
            if (skillData1.Price == null && skillData2.Price != null && !skillData1.FilledWithNull.Contains("price"))
                skillData1.Price = skillData2.Price;
            if (skillData1.Friend.Count == 0 && skillData2.Friend.Count > 0 && !skillData1.FilledWithNull.Contains("friend"))
                skillData1.Friend.AddRange(skillData2.Friend);
            if (skillData1.MP == null && skillData2.MP != null && !skillData1.FilledWithNull.Contains("mp"))
                skillData1.MP = skillData2.MP;
            if (skillData1.StrPercent == null && skillData2.StrPercent != null && !skillData1.FilledWithNull.Contains("str"))
            {
                skillData1.Strength = skillData2.Strength;
                skillData1.StrPercent = skillData2.StrPercent;
            }
            if (skillData1.StrRatio == null && skillData2.StrRatio != null && !skillData1.FilledWithNull.Contains("str_ratio"))
                skillData1.StrRatio = skillData2.StrRatio;
            if (skillData1.Image == null && skillData2.Image != null && !skillData1.FilledWithNull.Contains("image"))
                skillData1.Image = skillData2.Image;
            if (skillData1.Width == null && skillData2.Width != null && !skillData1.FilledWithNull.Contains("w"))
                skillData1.Width = skillData2.Width;
            if (skillData1.Height == null && skillData2.Height != null && !skillData1.FilledWithNull.Contains("h"))
                skillData1.Height = skillData2.Height;
            if (skillData1.Alpha == null && skillData2.Alpha != null && !skillData1.FilledWithNull.Contains("a"))
                skillData1.Alpha = skillData2.Alpha;
            if (skillData1.Anime == null && skillData2.Anime != null && !skillData1.FilledWithNull.Contains("anime"))
                skillData1.Anime = skillData2.Anime;
            if (skillData1.AnimeInterval == null && skillData2.AnimeInterval != null && !skillData1.FilledWithNull.Contains("anime_interval"))
                skillData1.AnimeInterval = skillData2.AnimeInterval;
            if (skillData1.AlphaTip == null && skillData2.AlphaTip != null && !skillData1.FilledWithNull.Contains("alpha_tip"))
                skillData1.AlphaTip = skillData2.AlphaTip;
            if (skillData1.AlphaButt == null && skillData2.AlphaButt != null && !skillData1.FilledWithNull.Contains("alpha_butt"))
                skillData1.AlphaButt = skillData2.AlphaButt;
            if (skillData1.Center == null && skillData2.Center != null && !skillData1.FilledWithNull.Contains("center"))
                skillData1.Center = skillData2.Center;
            if (skillData1.Ground == null && skillData2.Ground != null && !skillData1.FilledWithNull.Contains("ground"))
                skillData1.Ground = skillData2.Ground;
            if (skillData1.D360 == null && skillData2.D360 != null && !skillData1.FilledWithNull.Contains("d360"))
                skillData1.D360 = skillData2.D360;
            if (skillData1.Rotate == null && skillData2.Rotate != null && !skillData1.FilledWithNull.Contains("rotate"))
                skillData1.Rotate = skillData2.Rotate;
            if (skillData1.D360Adjust == null && skillData2.D360Adjust != null && !skillData1.FilledWithNull.Contains("d360_adj"))
                skillData1.D360Adjust = skillData2.D360Adjust;
            if (skillData1.ResizeW == null && skillData2.ResizeW != null && !skillData1.FilledWithNull.Contains("resize_w"))
                skillData1.ResizeW = skillData2.ResizeW;
            if (skillData1.ResizeH == null && skillData2.ResizeH != null && !skillData1.FilledWithNull.Contains("resize_h"))
                skillData1.ResizeH = skillData2.ResizeH;
            if (skillData1.ResizeA == null && skillData2.ResizeA != null && !skillData1.FilledWithNull.Contains("resize_a"))
                skillData1.ResizeA = skillData2.ResizeA;
            if (skillData1.ResizeS == null && skillData2.ResizeS != null && !skillData1.FilledWithNull.Contains("resize_s"))
                skillData1.ResizeS = skillData2.ResizeS;
            if (skillData1.ResizeInterval == null && skillData2.ResizeInterval != null && !skillData1.FilledWithNull.Contains("resize_interval"))
                skillData1.ResizeInterval = skillData2.ResizeInterval;
            if (skillData1.ResizeStart == null && skillData2.ResizeStart != null && !skillData1.FilledWithNull.Contains("resize_start"))
                skillData1.ResizeStart = skillData2.ResizeStart;
            if (skillData1.ResizeReverse == null && skillData2.ResizeReverse != null && !skillData1.FilledWithNull.Contains("resize_reverse"))
                skillData1.ResizeReverse = skillData2.ResizeReverse;
            if (skillData1.Direct == null && skillData2.Direct != null && !skillData1.FilledWithNull.Contains("direct"))
                skillData1.Direct = skillData2.Direct;
            if (skillData1.ForceFire == null && skillData2.ForceFire != null && !skillData1.FilledWithNull.Contains("force_fire"))
                skillData1.ForceFire = skillData2.ForceFire;
            if (skillData1.SlowPercentage == null && skillData2.SlowPercentage != null && !skillData1.FilledWithNull.Contains("slow_per"))
                skillData1.SlowPercentage = skillData2.SlowPercentage;
            if (skillData1.SlowTime == null && skillData2.SlowTime != null && !skillData1.FilledWithNull.Contains("slow_time"))
                skillData1.SlowTime = skillData2.SlowTime;
            if (skillData1.Slide == null && skillData2.Slide != null && !skillData1.FilledWithNull.Contains("slide"))
                skillData1.Slide = skillData2.Slide;
            if (skillData1.SlideSpeed == null && skillData2.SlideSpeed != null && !skillData1.FilledWithNull.Contains("slide_speed"))
                skillData1.SlideSpeed = skillData2.SlideSpeed;
            if (skillData1.SlideDelay == null && skillData2.SlideDelay != null && !skillData1.FilledWithNull.Contains("slide_delay"))
                skillData1.SlideDelay = skillData2.SlideDelay;
            if (skillData1.SlideStamp == null && skillData2.SlideStamp != null && !skillData1.FilledWithNull.Contains("slide_stamp"))
            {
                skillData1.SlideStamp = skillData2.SlideStamp;
                skillData1.SlideStampOn = skillData2.SlideStampOn;
            }
            if (skillData1.WaitTimeMin == null && skillData2.WaitTimeMin != null && !skillData1.FilledWithNull.Contains("wait_time"))
                skillData1.WaitTimeMin = skillData2.WaitTimeMin;
            if (skillData1.WaitTimeMax == null && skillData2.WaitTimeMax != null && !skillData1.FilledWithNull.Contains("wait_time2"))
                skillData1.WaitTimeMax = skillData2.WaitTimeMax;
            if (skillData1.Shake == null && skillData2.Shake != null && !skillData1.FilledWithNull.Contains("shake"))
                skillData1.Shake = skillData2.Shake;
            if (skillData1.RayR == null && skillData2.RayR != null && !skillData1.FilledWithNull.Contains("ray"))
            {
                skillData1.RayR = skillData2.RayR;
                skillData1.RayG = skillData2.RayG;
                skillData1.RayB = skillData2.RayB;
                skillData1.RayA = skillData2.RayA;
            }
            if (skillData1.ForceRay == null && skillData2.ForceRay != null && !skillData1.FilledWithNull.Contains("force_ray"))
                skillData1.ForceRay = skillData2.ForceRay;
            if (skillData1.Flash == null && skillData2.Flash != null && !skillData1.FilledWithNull.Contains("flash"))
                skillData1.Flash = skillData2.Flash;
            if (skillData1.FlashImage == null && skillData2.FlashImage != null && !skillData1.FilledWithNull.Contains("flash_image"))
                skillData1.FlashImage = skillData2.FlashImage;
            if (skillData1.FlashAnime == null && skillData2.FlashAnime != null && !skillData1.FilledWithNull.Contains("flash_anime"))
                skillData1.FlashAnime = skillData2.FlashAnime;
            if (skillData1.Collision == null && skillData2.Collision != null && !skillData1.FilledWithNull.Contains("collision"))
                skillData1.Collision = skillData2.Collision;
            if (skillData1.AfterDeathType == null && skillData2.AfterDeathType != null && !skillData1.FilledWithNull.Contains("afterdeath"))
            {
                skillData1.AfterDeath = skillData2.AfterDeath;
                skillData1.AfterDeathType = skillData2.AfterDeathType;
            }
            if (skillData1.AfterHitType == null && skillData2.AfterHitType != null && !skillData1.FilledWithNull.Contains("afterhit"))
            {
                skillData1.AfterHit = skillData2.AfterHit;
                skillData1.AfterHitType = skillData2.AfterHitType;
            }
            if (skillData1.EffectHeight == null && skillData1.EffectWidth == null && skillData1.TroopType == null && skillData1.YorozuAttribute.Count == 0 && !skillData1.FilledWithNull.Contains("yorozu"))
            {
                if (skillData2.YorozuTurn != null) skillData1.YorozuTurn = skillData2.YorozuTurn;
                if (skillData2.YorozuRadius != null) skillData1.YorozuRadius = skillData2.YorozuRadius;
                if (skillData2.YorozuThrowMax != null) skillData1.YorozuThrowMax = skillData2.YorozuThrowMax;
                if (skillData2.YorozuHp != null) skillData1.YorozuHp = skillData2.YorozuHp;
                if (skillData2.YorozuMp != null) skillData1.YorozuMp = skillData2.YorozuMp;
                if (skillData2.YorozuAttack != null) skillData1.YorozuAttack = skillData2.YorozuAttack;
                if (skillData2.YorozuDefense != null) skillData1.YorozuDefense = skillData2.YorozuDefense;
                if (skillData2.YorozuMagic != null) skillData1.YorozuMagic = skillData2.YorozuMagic;
                if (skillData2.YorozuMagdef != null) skillData1.YorozuMagdef = skillData2.YorozuMagdef;
                if (skillData2.YorozuSpeed != null) skillData1.YorozuSpeed = skillData2.YorozuSpeed;
                if (skillData2.YorozuDext != null) skillData1.YorozuDext = skillData2.YorozuDext;
                if (skillData2.YorozuMove != null) skillData1.YorozuMove = skillData2.YorozuMove;
                if (skillData2.YorozuHprec != null) skillData1.YorozuHprec = skillData2.YorozuHprec;
                if (skillData2.YorozuMprec != null) skillData1.YorozuMprec = skillData2.YorozuMprec;
                if (skillData2.YorozuSummonMax != null) skillData1.YorozuSummonMax = skillData2.YorozuSummonMax;
                if (skillData2.YorozuDrain != null) skillData1.YorozuDrain = skillData2.YorozuDrain;
                if (skillData2.YorozuDeath != null) skillData1.YorozuDeath = skillData2.YorozuDeath;
                if (skillData2.YorozuMagsuck != null) skillData1.YorozuMagsuck = skillData2.YorozuMagsuck;
                if (skillData2.YorozuSuck != null) skillData1.YorozuSuck = skillData2.YorozuSuck;
                if (skillData2.YorozuFear != null) skillData1.YorozuFear = skillData2.YorozuFear;
                if (skillData2.YorozuPoi != null) skillData1.YorozuPoi = skillData2.YorozuPoi;
                if (skillData2.YorozuPara != null) skillData1.YorozuPara = skillData2.YorozuPara;
                if (skillData2.YorozuIll != null) skillData1.YorozuIll = skillData2.YorozuIll;
                if (skillData2.YorozuConf != null) skillData1.YorozuConf = skillData2.YorozuConf;
                if (skillData2.YorozuSil != null) skillData1.YorozuSil = skillData2.YorozuSil;
                if (skillData2.YorozuStone != null) skillData1.YorozuStone = skillData2.YorozuStone;
                if (skillData2.YorozuAttribute.Count != 0)
                    foreach (var item in skillData2.YorozuAttribute)
                        skillData1.YorozuAttribute.Add(item.Key, item.Value);
                if (skillData2.TroopType != null) skillData1.TroopType = skillData2.TroopType;
                if (skillData2.EffectWidth != null) skillData1.EffectWidth = skillData2.EffectWidth;
                if (skillData2.EffectHeight != null) skillData1.EffectHeight = skillData2.EffectHeight;
            }
            if (skillData1.Attribute == null && skillData2.Attribute != null && !skillData1.FilledWithNull.Contains("attr"))
                skillData1.Attribute = skillData2.Attribute;
            if (skillData1.Add.Count == 0 && skillData2.Add.Count > 0 && !skillData1.FilledWithNull.Contains("add"))
                skillData1.Add.AddRange(skillData2.Add);
            if (skillData1.AddAll == null && skillData2.AddAll != null && !skillData1.FilledWithNull.Contains("add_all"))
                skillData1.AddAll = skillData2.AddAll;
            if (skillData1.AddPercentage == null && skillData2.AddPercentage != null && !skillData1.FilledWithNull.Contains("add_per"))
                skillData1.AddPercentage = skillData2.AddPercentage;
            if (skillData1.DamageType == null && skillData2.DamageType != null && !skillData1.FilledWithNull.Contains("damage"))
                skillData1.DamageType = skillData2.DamageType;
            if (skillData1.DamageRangeAdjust == null && skillData2.DamageRangeAdjust != null && !skillData1.FilledWithNull.Contains("damage_range_adjust"))
                skillData1.DamageRangeAdjust = skillData2.DamageRangeAdjust;
            if (skillData1.AttackUs == null && skillData2.AttackUs != null && !skillData1.FilledWithNull.Contains("attack_us"))
                skillData1.AttackUs = skillData2.AttackUs;
            if (skillData1.AllFunc == null && skillData2.AllFunc != null && !skillData1.FilledWithNull.Contains("allfunc"))
                skillData1.AllFunc = skillData2.AllFunc;
            if (skillData1.Bom == null && skillData2.Bom != null && !skillData1.FilledWithNull.Contains("bom"))
                skillData1.Bom = skillData2.Bom;
            if (skillData1.HomingType == null && skillData2.HomingType != null && !skillData1.FilledWithNull.Contains("homing"))
            {
                skillData1.HomingType = skillData2.HomingType;
                skillData1.HomingNumber = skillData2.HomingNumber;
                skillData1.HomingOn = skillData2.HomingOn;
            }
            if (skillData1.Forward == null && skillData2.Forward != null && !skillData1.FilledWithNull.Contains("forward"))
                skillData1.Forward = skillData2.Forward;
            if (skillData1.Far == null && skillData2.Far != null && !skillData1.FilledWithNull.Contains("far"))
                skillData1.Far = skillData2.Far;
            if (skillData1.Hard == null && skillData2.Hard != null && !skillData1.FilledWithNull.Contains("hard"))
                skillData1.Hard = skillData2.Hard;
            if (skillData1.OneHit == null && skillData2.OneHit != null && !skillData1.FilledWithNull.Contains("onehit"))
                skillData1.OneHit = skillData2.OneHit;
            if (skillData1.Hard2 == null && skillData2.Hard2 != null && !skillData1.FilledWithNull.Contains("hard2"))
                skillData1.Hard2 = skillData2.Hard2;
            if (skillData1.OffsetOn == null && skillData2.OffsetOn != null && !skillData1.FilledWithNull.Contains("offset"))
            {
                skillData1.OffsetOn = skillData2.OffsetOn;
                skillData1.Offset.AddRange(skillData2.Offset);
            }
            if (skillData1.OffsetAttribute.Count == 0 && skillData2.OffsetAttribute.Count > 0 && !skillData1.FilledWithNull.Contains("offset_attr"))
                skillData1.OffsetAttribute.AddRange(skillData2.OffsetAttribute);
            if (skillData1.Knock == null && skillData2.Knock != null && !skillData1.FilledWithNull.Contains("knock"))
                skillData1.Knock = skillData2.Knock;
            if (skillData1.KnockPower == null && skillData2.KnockPower != null && !skillData1.FilledWithNull.Contains("knock_power"))
                skillData1.KnockPower = skillData2.KnockPower;
            if (skillData1.KnockSpeed == null && skillData2.KnockSpeed != null && !skillData1.FilledWithNull.Contains("knock_speed"))
                skillData1.KnockSpeed = skillData2.KnockSpeed;
            if (skillData1.Range == null && skillData2.Range != null && !skillData1.FilledWithNull.Contains("range"))
                skillData1.Range = skillData2.Range;
            if (skillData1.RangeMin == null && skillData2.RangeMin != null && !skillData1.FilledWithNull.Contains("range_min"))
                skillData1.RangeMin = skillData2.RangeMin;
            if (skillData1.Check == null && skillData2.Check != null && !skillData1.FilledWithNull.Contains("check"))
                skillData1.Check = skillData2.Check;
            if (skillData1.Origin == null && skillData2.Origin != null && !skillData1.FilledWithNull.Contains("origin"))
                skillData1.Origin = skillData2.Origin;
            if (skillData1.RandomSpace == null && skillData2.RandomSpace != null && !skillData1.FilledWithNull.Contains("random_space"))
                skillData1.RandomSpace = skillData2.RandomSpace;
            if (skillData1.RandomSpaceMin == null && skillData2.RandomSpaceMin != null && !skillData1.FilledWithNull.Contains("random_space_min"))
                skillData1.RandomSpaceMin = skillData2.RandomSpaceMin;
            if (skillData1.Time == null && skillData2.Time != null && !skillData1.FilledWithNull.Contains("time"))
                skillData1.Time = skillData2.Time;
            if (skillData1.Rush == null && skillData2.Rush != null && !skillData1.FilledWithNull.Contains("rush"))
                skillData1.Rush = skillData2.Rush;
            if (skillData1.RushInterval == null && skillData2.RushInterval != null && !skillData1.FilledWithNull.Contains("rush_interval"))
                skillData1.RushInterval = skillData2.RushInterval;
            if (skillData1.RushDegree == null && skillData2.RushDegree != null && !skillData1.FilledWithNull.Contains("rush_degree"))
                skillData1.RushDegree = skillData2.RushDegree;
            if (skillData1.RushRandomDegree == null && skillData2.RushRandomDegree != null && !skillData1.FilledWithNull.Contains("rush_random_degree"))
                skillData1.RushRandomDegree = skillData2.RushRandomDegree;
            if (skillData1.FollowOn == null && skillData2.FollowOn != null && !skillData1.FilledWithNull.Contains("follow"))
            {
                skillData1.Follow = skillData2.Follow;
                skillData1.FollowOn = skillData2.FollowOn;
            }
            if (skillData1.StartDegree == null && skillData2.StartDegree != null && !skillData1.FilledWithNull.Contains("start_degree"))
                skillData1.StartDegree = skillData2.StartDegree;
            if (skillData1.StartRandomDegree == null && skillData2.StartRandomDegree != null && !skillData1.FilledWithNull.Contains("start_random_degree"))
                skillData1.StartRandomDegree = skillData2.StartRandomDegree;
            if (skillData1.StartDegreeType == null && skillData2.StartDegreeType != null && !skillData1.FilledWithNull.Contains("start_degree_type"))
                skillData1.StartDegreeType = skillData2.StartDegreeType;
            if (skillData1.StartDegreeTurnUnit == null && skillData2.StartDegreeTurnUnit != null && !skillData1.FilledWithNull.Contains("start_degree_turnunit"))
                skillData1.StartDegreeTurnUnit = skillData2.StartDegreeTurnUnit;
            if (skillData1.StartDegreeFix == null && skillData2.StartDegreeFix != null && !skillData1.FilledWithNull.Contains("start_degree_fix"))
                skillData1.StartDegreeFix = skillData2.StartDegreeFix;
            if (skillData1.Homing2 == null && skillData2.Homing2 != null && !skillData1.FilledWithNull.Contains("homing2"))
                skillData1.Homing2 = skillData2.Homing2;
            if (skillData1.DropDegree == null && skillData2.DropDegree != null && !skillData1.FilledWithNull.Contains("drop_degree"))
                skillData1.DropDegree = skillData2.DropDegree;
            if (skillData1.DropDegreeMax == null && skillData2.DropDegreeMax != null && !skillData1.FilledWithNull.Contains("drop_degree2"))
                skillData1.DropDegreeMax = skillData2.DropDegreeMax;
            if (skillData1.SendTarget == null && skillData2.SendTarget != null && !skillData1.FilledWithNull.Contains("send_target"))
                skillData1.SendTarget = skillData2.SendTarget;
            if (skillData1.SendImageDegree == null && skillData2.SendImageDegree != null && !skillData1.FilledWithNull.Contains("send_image_degree"))
                skillData1.SendImageDegree = skillData2.SendImageDegree;
            if (skillData1.Next == null && skillData2.Next != null && !skillData1.FilledWithNull.Contains("next"))
                skillData1.Next = skillData2.Next;
            if (skillData1.Next4 == null && skillData2.Next4 != null && !skillData1.FilledWithNull.Contains("next4"))
                skillData1.Next4 = skillData2.Next4;
            if (skillData1.Next2.Count == 0 && skillData2.Next2.Count > 0 && !skillData1.FilledWithNull.Contains("next2"))
                skillData1.Next2.AddRange(skillData2.Next2);
            if (skillData1.Next3.Count == 0 && skillData2.Next3.Count > 0 && !skillData1.FilledWithNull.Contains("next3"))
                skillData1.Next3.AddRange(skillData2.Next3);
            if (skillData1.NextOrder == null && skillData2.NextOrder != null && !skillData1.FilledWithNull.Contains("next_order"))
                skillData1.NextOrder = skillData2.NextOrder;
            if (skillData1.NextFirst == null && skillData2.NextFirst != null && !skillData1.FilledWithNull.Contains("next_first"))
                skillData1.NextFirst = skillData2.NextFirst;
            if (skillData1.NextLast == null && skillData2.NextLast != null && !skillData1.FilledWithNull.Contains("next_last"))
                skillData1.NextLast = skillData2.NextLast;
            if (skillData1.NextInterval == null && skillData2.NextInterval != null && !skillData1.FilledWithNull.Contains("next_interval"))
                skillData1.NextInterval = skillData2.NextInterval;
            if (skillData1.JointSkill == null && skillData2.JointSkill != null && !skillData1.FilledWithNull.Contains("joint_skill"))
                skillData1.JointSkill = skillData2.JointSkill;
            if (skillData1.JustNext.Count == 0 && skillData2.JustNext.Count > 0 && !skillData1.FilledWithNull.Contains("just_next"))
                skillData1.JustNext.AddRange(skillData2.JustNext);
            if (skillData1.PairNext.Count == 0 && skillData2.PairNext.Count > 0 && !skillData1.FilledWithNull.Contains("pair_next"))
                skillData1.PairNext.AddRange(skillData2.PairNext);
            if (skillData1.Height_Charge_Arc == null && skillData2.Height_Charge_Arc != null && !skillData1.FilledWithNull.Contains("height"))
                skillData1.Height_Charge_Arc = skillData2.Height_Charge_Arc;
            if (skillData1.StatusType == null && skillData2.StatusType != null && !skillData1.FilledWithNull.Contains("type"))
                skillData1.StatusType = skillData2.StatusType;
        }
        internal static void Resolve(ObjectData objectChipData1, ObjectData objectChipData2)
        {
            if (objectChipData1.Alpha == null && objectChipData2.Alpha != null && !objectChipData1.FilledWithNull.Contains("a"))
                objectChipData1.Alpha = objectChipData2.Alpha;
            if (objectChipData1.Blk == null && objectChipData2.Blk != null && !objectChipData1.FilledWithNull.Contains("blk"))
                objectChipData1.Blk = objectChipData2.Blk;
            if (objectChipData1.Height == null && objectChipData2.Height != null && !objectChipData1.FilledWithNull.Contains("h"))
                objectChipData1.Height = objectChipData2.Height;
            if (objectChipData1.Image2Alpha == null && objectChipData2.Image2Alpha != null && !objectChipData1.FilledWithNull.Contains("image2_a"))
                objectChipData1.Image2Alpha = objectChipData2.Image2Alpha;
            if (objectChipData1.Image2Height == null && objectChipData2.Image2Height != null && !objectChipData1.FilledWithNull.Contains("image2_h"))
                objectChipData1.Image2Height = objectChipData2.Image2Height;
            if (objectChipData1.Image2Name == null && objectChipData2.Image2Name != null && !objectChipData1.FilledWithNull.Contains("image2"))
                objectChipData1.Image2Name = objectChipData2.Image2Name;
            if (objectChipData1.Image2Width == null && objectChipData2.Image2Width != null && !objectChipData1.FilledWithNull.Contains("image2_w"))
                objectChipData1.Image2Width = objectChipData2.Image2Width;
            if (objectChipData1.ImageNameRandomList.Count == 0 && objectChipData2.ImageNameRandomList.Count != 0 && !objectChipData1.FilledWithNull.Contains("image") && !objectChipData1.FilledWithNull.Contains("member"))
                objectChipData1.ImageNameRandomList.AddRange(objectChipData2.ImageNameRandomList);
            if (objectChipData1.IsGound == null && objectChipData2.IsGound != null && !objectChipData1.FilledWithNull.Contains("ground"))
                objectChipData1.IsGound = objectChipData2.IsGound;
            if (objectChipData1.IsLandBase == null && objectChipData2.IsLandBase != null && !objectChipData1.FilledWithNull.Contains("land_base"))
            {
                objectChipData1.IsLandBase = objectChipData2.IsLandBase;
                objectChipData1.LandBase = objectChipData2.LandBase;
            }
            if (objectChipData1.NoArcHit == null && objectChipData2.NoArcHit != null && !objectChipData1.FilledWithNull.Contains("no_arc_hit"))
                objectChipData1.NoArcHit = objectChipData2.NoArcHit;
            if (objectChipData1.NoStop == null && objectChipData2.NoStop != null && !objectChipData1.FilledWithNull.Contains("no_stop"))
                objectChipData1.NoStop = objectChipData2.NoStop;
            if (objectChipData1.NoWall2 == null && objectChipData2.NoWall2 != null && !objectChipData1.FilledWithNull.Contains("no_wall2"))
                objectChipData1.NoWall2 = objectChipData2.NoWall2;
            if (objectChipData1.Radius == null && objectChipData2.Radius != null && !objectChipData1.FilledWithNull.Contains("radius"))
                objectChipData1.Radius = objectChipData2.Radius;
            if (objectChipData1.Type == ObjectData.ChipType.None && objectChipData2.Type != ObjectData.ChipType.None && !objectChipData1.FilledWithNull.Contains("type"))
                objectChipData1.Type = objectChipData2.Type;
            if (objectChipData1.Width == null && objectChipData2.Width != null && !objectChipData1.FilledWithNull.Contains("w"))
                objectChipData1.Width = objectChipData2.Width;
        }
        internal static void Resolve(EventData eventData1, EventData eventData2)
        {
            if (eventData1.BackGround == null && eventData2.BackGround != null && !eventData1.FilledWithNull.Contains("bg"))
                eventData1.BackGround = eventData2.BackGround;
            if (eventData1.BGM == null && eventData2.BGM != null && !eventData1.FilledWithNull.Contains("bgm"))
                eventData1.BGM = eventData2.BGM;
            if (eventData1.IsBlind == null && eventData2.IsBlind != null && !eventData1.FilledWithNull.Contains("blind"))
            {
                eventData1.Blind = eventData2.Blind;
                eventData1.IsBlind = eventData2.IsBlind;
            }
            if (eventData1.Castle == null && eventData2.Castle != null && !eventData1.FilledWithNull.Contains("castle"))
                eventData1.Castle = eventData2.Castle;
            if (eventData1.CastleBattle == null && eventData2.CastleBattle != null && !eventData1.FilledWithNull.Contains("castle_battle"))
                eventData1.CastleBattle = eventData2.CastleBattle;
            if (eventData1.Disperse == null && eventData2.Disperse != null && !eventData1.FilledWithNull.Contains("disperse"))
                eventData1.Disperse = eventData2.Disperse;
            if (eventData1.Handle == null && eventData2.Handle != null && !eventData1.FilledWithNull.Contains("handle"))
                eventData1.Handle = eventData2.Handle;
            if (eventData1.Height == null && eventData2.Height != null && !eventData1.FilledWithNull.Contains("h"))
                eventData1.Height = eventData2.Height;
            if (eventData1.Width == null && eventData2.Width != null && !eventData1.FilledWithNull.Contains("w"))
                eventData1.Width = eventData2.Width;
            if (eventData1.Limit == null && eventData2.Limit != null && !eventData1.FilledWithNull.Contains("limit"))
                eventData1.Limit = eventData2.Limit;
            if (eventData1.Map == null && eventData2.Map != null && !eventData1.FilledWithNull.Contains("map"))
                eventData1.Map = eventData2.Map;
            if (eventData1.Title == null && eventData2.Title != null && !eventData1.FilledWithNull.Contains("title"))
                eventData1.Title = eventData2.Title;
            if (eventData1.Volume == null && eventData2.Volume != null && !eventData1.FilledWithNull.Contains("volume"))
                eventData1.Volume = eventData2.Volume;
        }
        internal static void Resolve(FieldData fieldChipData1, FieldData fieldChipData2)
        {
            if (fieldChipData1.Alt_max == null && fieldChipData2.Alt_max != null && !fieldChipData1.FilledWithNull.Contains("alt_max"))
                fieldChipData1.Alt_max = fieldChipData2.Alt_max;
            if (fieldChipData1.Alt_min == null && fieldChipData2.Alt_min != null && !fieldChipData1.FilledWithNull.Contains("alt"))
                fieldChipData1.Alt_min = fieldChipData2.Alt_min;
            if (fieldChipData1.Attribute == null && fieldChipData2.Attribute != null && !fieldChipData1.FilledWithNull.Contains("attr"))
                fieldChipData1.Attribute = fieldChipData2.Attribute;
            if (fieldChipData1.BaseId == null && fieldChipData2.BaseId != null && !fieldChipData1.FilledWithNull.Contains("id"))
                fieldChipData1.BaseId = fieldChipData2.BaseId;
            if (fieldChipData1.ColorB == null && fieldChipData2.ColorB != null && !fieldChipData1.FilledWithNull.Contains("color"))
            {
                fieldChipData1.ColorB = fieldChipData2.ColorB;
                fieldChipData1.ColorG = fieldChipData2.ColorG;
                fieldChipData1.ColorR = fieldChipData2.ColorR;
            }
            if (fieldChipData1.ImageNameRandomList.Count == 0 && fieldChipData2.ImageNameRandomList.Count != 0 && !fieldChipData1.FilledWithNull.Contains("image") && !fieldChipData1.FilledWithNull.Contains("add2"))
                fieldChipData1.ImageNameRandomList.AddRange(fieldChipData2.ImageNameRandomList);
            if (fieldChipData1.IsEdge == null && fieldChipData2.IsEdge != null && !fieldChipData1.FilledWithNull.Contains("edge"))
                fieldChipData1.IsEdge = fieldChipData2.IsEdge;
            if (fieldChipData1.JointChip.Count == 0 && fieldChipData2.JointChip.Count != 0 && !fieldChipData1.FilledWithNull.Contains("joint"))
            {
                fieldChipData1.JointChip.AddRange(fieldChipData2.JointChip);
                fieldChipData1.JointImage.AddRange(fieldChipData2.JointImage);
            }
            if (fieldChipData1.MemberRandomList.Count == 0 && fieldChipData2.MemberRandomList.Count != 0 && !fieldChipData1.FilledWithNull.Contains("member"))
                fieldChipData1.MemberRandomList.AddRange(fieldChipData2.MemberRandomList);
            if (fieldChipData1.SmoothType == FieldData.Smooth.off && fieldChipData2.SmoothType != FieldData.Smooth.off && !fieldChipData1.FilledWithNull.Contains("smooth"))
                fieldChipData1.SmoothType = fieldChipData2.SmoothType;
            if (fieldChipData1.Type == FieldData.ChipType.None && fieldChipData2.Type != FieldData.ChipType.None && !fieldChipData1.FilledWithNull.Contains("type"))
                fieldChipData1.Type = fieldChipData2.Type;
        }
        internal static void Resolve(DungeonData dungeonData1, DungeonData dungeonData2)
        {
            dungeonData1.VariantData.ResolveVariant(dungeonData2.VariantData);
            if (dungeonData1.BackColorA == null && dungeonData2.BackColorA != null && !dungeonData1.FilledWithNull.Contains("color"))
            {
                dungeonData1.BackColorA = dungeonData2.BackColorA;
                dungeonData1.BackColorR = dungeonData2.BackColorR;
                dungeonData1.BackColorG = dungeonData2.BackColorG;
                dungeonData1.BackColorB = dungeonData2.BackColorB;
                dungeonData1.Dense = dungeonData2.Dense;
                dungeonData1.ForeColorA = dungeonData2.ForeColorA;
                dungeonData1.ForeColorR = dungeonData2.ForeColorR;
                dungeonData1.ForeColorG = dungeonData2.ForeColorG;
                dungeonData1.ForeColorB = dungeonData2.ForeColorB;
                dungeonData1.ColorDirection = dungeonData2.ColorDirection;
            }
            if (dungeonData1.BaseLevel == null && dungeonData2.BaseLevel != null && !dungeonData1.FilledWithNull.Contains("base_level"))
                dungeonData1.BaseLevel = dungeonData2.BaseLevel;
            if (dungeonData1.BGM == null && dungeonData2.BGM != null && !dungeonData1.FilledWithNull.Contains("bgm"))
                dungeonData1.BGM = dungeonData2.BGM;
            if (dungeonData1.Blind == null && dungeonData2.Blind != null && !dungeonData1.FilledWithNull.Contains("blind"))
                dungeonData1.Blind = dungeonData2.Blind;
            if (dungeonData1.Box == null && dungeonData2.Box != null && !dungeonData1.FilledWithNull.Contains("box"))
                dungeonData1.Box = dungeonData2.Box;
            if (dungeonData1.Floor == null && dungeonData2.Floor != null && !dungeonData1.FilledWithNull.Contains("floor"))
                dungeonData1.Floor = dungeonData2.Floor;
            if (dungeonData1.Goal == null && dungeonData2.Goal != null && !dungeonData1.FilledWithNull.Contains("goal"))
                dungeonData1.Goal = dungeonData2.Goal;
            if (dungeonData1.HallwayWidth == null && dungeonData2.HallwayWidth != null && !dungeonData1.FilledWithNull.Contains("home"))
            {
                dungeonData1.HallwayWidth = dungeonData2.HallwayWidth;
                dungeonData1.Width = dungeonData2.Width;
                dungeonData1.Height = dungeonData2.Height;
                dungeonData1.Seed = dungeonData2.Seed;
            }
            if (dungeonData1.IsOpened == null && dungeonData2.IsOpened != null && !dungeonData1.FilledWithNull.Contains("open"))
                dungeonData1.IsOpened = dungeonData2.IsOpened;
            if (dungeonData1.Item.Count == 0 && dungeonData2.Item.Count != 0 && !dungeonData1.FilledWithNull.Contains("item"))
                dungeonData1.Item.AddRange(dungeonData2.Item);
            if (dungeonData1.ItemNumber == null && dungeonData2.ItemNumber != null && !dungeonData1.FilledWithNull.Contains("item_num"))
                dungeonData1.ItemNumber = dungeonData2.ItemNumber;
            if (dungeonData1.ItemText == null && dungeonData2.ItemText != null && !dungeonData1.FilledWithNull.Contains("item_text"))
                dungeonData1.ItemText = dungeonData2.ItemText;
            if (dungeonData1.LevelAdjust == null && dungeonData2.LevelAdjust != null && !dungeonData1.FilledWithNull.Contains("lv_adjust"))
                dungeonData1.LevelAdjust = dungeonData2.LevelAdjust;
            if (dungeonData1.Limit == null && dungeonData2.Limit != null && !dungeonData1.FilledWithNull.Contains("limit"))
                dungeonData1.Limit = dungeonData2.Limit;
            if (dungeonData1.MapFileName == null && dungeonData2.MapFileName != null && !dungeonData1.FilledWithNull.Contains("map"))
                dungeonData1.MapFileName = dungeonData2.MapFileName;
            if (dungeonData1.Max == null && dungeonData2.Max != null && !dungeonData1.FilledWithNull.Contains("max"))
                dungeonData1.Max = dungeonData2.Max;
            if (dungeonData1.MonsterNumber == null && dungeonData2.MonsterNumber != null && !dungeonData1.FilledWithNull.Contains("monster_num"))
                dungeonData1.MonsterNumber = dungeonData2.MonsterNumber;
            if (dungeonData1.Monsters.Count == 0 && dungeonData2.Monsters.Count != 0 && !dungeonData1.FilledWithNull.Contains("monster"))
                foreach (var item in dungeonData2.Monsters)
                    dungeonData1.Monsters[item.Key] = item.Value;
            if (dungeonData1.MoveSpeedRatio == null && dungeonData2.MoveSpeedRatio != null && !dungeonData1.FilledWithNull.Contains("move_speed"))
                dungeonData1.MoveSpeedRatio = dungeonData2.MoveSpeedRatio;
            if (dungeonData1.DisplayName == null && dungeonData2.DisplayName != null && !dungeonData1.FilledWithNull.Contains("name"))
                dungeonData1.DisplayName = dungeonData2.DisplayName;
            if (dungeonData1.Prefix == null && dungeonData2.Prefix != null && !dungeonData1.FilledWithNull.Contains("prefix"))
                dungeonData1.Prefix = dungeonData2.Prefix;
            if (dungeonData1.Ray == null && dungeonData2.Ray != null && !dungeonData1.FilledWithNull.Contains("ray"))
            {
                dungeonData1.Ray = dungeonData2.Ray;
                dungeonData1.Way1 = dungeonData2.Way1;
                dungeonData1.Way2 = dungeonData2.Way2;
                dungeonData1.Way3 = dungeonData2.Way3;
            }
            if (dungeonData1.Start == null && dungeonData2.Start != null && !dungeonData1.FilledWithNull.Contains("start"))
                dungeonData1.Start = dungeonData2.Start;
            if (dungeonData1.Suffix == null && dungeonData2.Suffix != null && !dungeonData1.FilledWithNull.Contains("suffix"))
                dungeonData1.Suffix = dungeonData2.Suffix;
            if (dungeonData1.Volume == null && dungeonData2.Volume != null && !dungeonData1.FilledWithNull.Contains("volume"))
                dungeonData1.Volume = dungeonData2.Volume;
            if (dungeonData1.Wall == null && dungeonData2.Wall != null && !dungeonData1.FilledWithNull.Contains("wall"))
                dungeonData1.Wall = dungeonData2.Wall;
        }
        internal static void Resolve(StoryData storyData1, StoryData storyData2)
        {
            if (storyData1.Fight == null && storyData2.Fight != null && storyData1.FilledWithNull.Contains("fight"))
                storyData1.Fight = storyData2.Fight;
            if (storyData1.Politics == null && storyData2.Politics != null && storyData1.FilledWithNull.Contains("politics"))
                storyData1.Politics = storyData2.Politics;
            if (storyData1.Pre == null && storyData2.Pre != null && storyData1.FilledWithNull.Contains("pre"))
                storyData1.Pre = storyData2.Pre;
            if (storyData1.Friend.Count == 0 && storyData2.Friend.Count != 0 && !storyData1.FilledWithNull.Contains("friend"))
                storyData1.Friend.AddRange(storyData2.Friend);
        }
        internal static void Resolve(RaceData raceData1, RaceData raceData2)
        {
            if (raceData1.Align == null && raceData2.Align != null && !raceData1.FilledWithNull.Contains("align"))
                raceData1.Align = raceData2.Align;
            if (raceData1.Brave == null && raceData2.Brave != null && !raceData1.FilledWithNull.Contains("brave"))
                raceData1.Brave = raceData2.Brave;
            if (raceData1.DisplayName == null && raceData2.DisplayName != null && !raceData1.FilledWithNull.Contains("name"))
                raceData1.DisplayName = raceData2.DisplayName;
            if (raceData1.MoveType == null && raceData2.MoveType != null && !raceData1.FilledWithNull.Contains("movetype"))
                raceData1.MoveType = raceData2.MoveType;
            if (raceData1.Consti.Count == 0 && raceData2.Consti.Count != 0 && !raceData1.FilledWithNull.Contains("consti"))
                foreach (var item in raceData2.Consti)
                    raceData1.Consti[item.Key] = item.Value;
        }
        internal static void Resolve(SpotData spotData1, SpotData spotData2)
        {
            spotData1.VariantData.ResolveVariant(spotData2.VariantData);
            if (spotData1.Politics == null && spotData2.Politics != null && !spotData1.FilledWithNull.Contains("politics"))
                spotData1.Politics = spotData2.Politics;
            if (spotData1.BGM == null && spotData2.BGM != null && !spotData1.FilledWithNull.Contains("bgm"))
                spotData1.BGM = spotData2.BGM;
            if (spotData1.Capacity == null && spotData2.Capacity != null && !spotData1.FilledWithNull.Contains("capacity"))
                spotData1.Capacity = spotData2.Capacity;
            if (spotData1.Castle == null && spotData2.Castle != null && !spotData1.FilledWithNull.Contains("castle"))
                spotData1.Castle = spotData2.Castle;
            if (spotData1.CastleLot == null && spotData2.CastleLot != null && !spotData1.FilledWithNull.Contains("castle_lot"))
                spotData1.CastleLot = spotData2.CastleLot;
            if (spotData1.DisplayName == null && spotData2.DisplayName != null && !spotData1.FilledWithNull.Contains("name"))
                spotData1.DisplayName = spotData2.DisplayName;
            if (spotData1.Dungeon == null && spotData2.Dungeon != null && !spotData1.FilledWithNull.Contains("dungeon"))
                spotData1.Dungeon = spotData2.Dungeon;
            if (spotData1.Gain == null && spotData2.Gain != null && !spotData1.FilledWithNull.Contains("gain"))
                spotData1.Gain = spotData2.Gain;
            if (spotData1.Height == null && spotData2.Height != null && !spotData1.FilledWithNull.Contains("h"))
                spotData1.Height = spotData2.Height;
            if (spotData1.Image == null && spotData2.Image != null && !spotData1.FilledWithNull.Contains("image"))
                spotData1.Image = spotData2.Image;
            if (spotData1.IsCastleBattle == null && spotData2.IsCastleBattle != null && !spotData1.FilledWithNull.Contains("castle_battle"))
                spotData1.IsCastleBattle = spotData2.IsCastleBattle;
            if (spotData1.IsNotHome == null && spotData2.IsNotHome != null && !spotData1.FilledWithNull.Contains("no_home"))
                spotData1.IsNotHome = spotData2.IsNotHome;
            if (spotData1.IsNotRaisableSpot == null && spotData2.IsNotRaisableSpot != null && !spotData1.FilledWithNull.Contains("no_raise"))
                spotData1.IsNotRaisableSpot = spotData2.IsNotRaisableSpot;
            if (spotData1.Limit == null && spotData2.Limit != null && !spotData1.FilledWithNull.Contains("limit"))
                spotData1.Limit = spotData2.Limit;
            if (spotData1.Map == null && spotData2.Map != null && !spotData1.FilledWithNull.Contains("map"))
                spotData1.Map = spotData2.Map;
            if (spotData1.Volume == null && spotData2.Volume != null && !spotData1.FilledWithNull.Contains("volume"))
                spotData1.Volume = spotData2.Volume;
            if (spotData1.Width == null && spotData2.Width != null && !spotData1.FilledWithNull.Contains("w"))
                spotData1.Width = spotData2.Width;
            if (spotData1.X == null && spotData2.X != null && !spotData1.FilledWithNull.Contains("x"))
                spotData1.X = spotData2.X;
            if (spotData1.Y == null && spotData2.Y != null && !spotData1.FilledWithNull.Contains("y"))
                spotData1.Y = spotData2.Y;
            if (spotData1.Members.Count == 0 && spotData2.Members.Count != 0 && !spotData1.FilledWithNull.Contains("member"))
                spotData1.Members.AddRange(spotData2.Members);
            if (spotData1.Merce.Count == 0 && spotData2.Merce.Count != 0 && !spotData1.FilledWithNull.Contains("merce"))
                spotData1.Merce.AddRange(spotData2.Merce);
            if (spotData1.Monsters.Count == 0 && spotData2.Monsters.Count != 0 && !spotData1.FilledWithNull.Contains("monster"))
                spotData1.Monsters.AddRange(spotData2.Monsters);
            if (spotData1.Yorozu.Count == 0 && spotData2.Yorozu.Count != 0 && !spotData1.FilledWithNull.Contains("yorozu"))
                spotData1.Yorozu.AddRange(spotData2.Yorozu);
        }
        internal static void Resolve(PowerData powerData1, PowerData powerData2)
        {
            powerData1.VariantData.ResolveVariant(powerData2.VariantData);
            if (powerData1.BaseLoyal == null && powerData2.BaseLoyal != null && !powerData1.FilledWithNull.Contains("base_loyal"))
                powerData1.BaseLoyal = powerData2.BaseLoyal;
            if (powerData1.BaseMerit == null && powerData2.BaseMerit != null && !powerData1.FilledWithNull.Contains("base_merit"))
                powerData1.BaseMerit = powerData2.BaseMerit;
            if (powerData1.BGM == null && powerData2.BGM != null && !powerData1.FilledWithNull.Contains("bgm"))
                powerData1.BGM = powerData2.BGM;
            if (powerData1.Description == null && powerData2.Description != null && !powerData1.FilledWithNull.Contains("text"))
                powerData1.Description = powerData2.Description;
            if (powerData1.Difficulty == null && powerData2.Difficulty != null && !powerData1.FilledWithNull.Contains("diff"))
                powerData1.Difficulty = powerData2.Difficulty;
            if (powerData1.DisplayName == null && powerData2.DisplayName != null && !powerData1.FilledWithNull.Contains("name"))
                powerData1.DisplayName = powerData2.DisplayName;
            if (powerData1.DoDiplomacy == null && powerData2.DoDiplomacy != null && !powerData1.FilledWithNull.Contains("diplomacy"))
                powerData1.DoDiplomacy = powerData2.DoDiplomacy;
            if (powerData1.Fix == null && powerData2.Fix != null && !powerData1.FilledWithNull.Contains("fix"))
                powerData1.Fix = powerData2.Fix;
            if (powerData1.FlagPath == null && powerData2.FlagPath != null && !powerData1.FilledWithNull.Contains("flag"))
                powerData1.FlagPath = powerData2.FlagPath;
            if (powerData1.Head == null && powerData2.Head != null && !powerData1.FilledWithNull.Contains("head"))
                powerData1.Head = powerData2.Head;
            if (powerData1.Head2 == null && powerData2.Head2 != null && !powerData1.FilledWithNull.Contains("head2"))
                powerData1.Head2 = powerData2.Head2;
            if (powerData1.Head3 == null && powerData2.Head3 != null && !powerData1.FilledWithNull.Contains("head3"))
                powerData1.Head3 = powerData2.Head3;
            if (powerData1.Head4 == null && powerData2.Head4 != null && !powerData1.FilledWithNull.Contains("head4"))
                powerData1.Head4 = powerData2.Head4;
            if (powerData1.Head5 == null && powerData2.Head5 != null && !powerData1.FilledWithNull.Contains("head5"))
                powerData1.Head5 = powerData2.Head5;
            if (powerData1.Head6 == null && powerData2.Head6 != null && !powerData1.FilledWithNull.Contains("head6"))
                powerData1.Head6 = powerData2.Head6;
            if (powerData1.IsEvent == null && powerData2.IsEvent != null && !powerData1.FilledWithNull.Contains("event"))
                powerData1.IsEvent = powerData2.IsEvent;
            if (powerData1.IsPlayableAsTalent == null && powerData2.IsPlayableAsTalent != null && !powerData1.FilledWithNull.Contains("enable_talent"))
                powerData1.IsPlayableAsTalent = powerData2.IsPlayableAsTalent;
            if (powerData1.IsRaisable == null && powerData2.IsRaisable != null && !powerData1.FilledWithNull.Contains("free_raise"))
                powerData1.IsRaisable = powerData2.IsRaisable;
            if (powerData1.IsSelectable == null && powerData2.IsSelectable != null && !powerData1.FilledWithNull.Contains("enable_select"))
                powerData1.IsSelectable = powerData2.IsSelectable;
            if (powerData1.Kosen == null && powerData2.Kosen != null && !powerData1.FilledWithNull.Contains("kosen"))
                powerData1.Kosen = powerData2.Kosen;
            if (powerData1.Master == null && powerData2.Master != null && !powerData1.FilledWithNull.Contains("master"))
                powerData1.Master = powerData2.Master;
            if (powerData1.Master2 == null && powerData2.Master2 != null && !powerData1.FilledWithNull.Contains("master2"))
                powerData1.Master2 = powerData2.Master2;
            if (powerData1.Master3 == null && powerData2.Master3 != null && !powerData1.FilledWithNull.Contains("master3"))
                powerData1.Master3 = powerData2.Master3;
            if (powerData1.Master4 == null && powerData2.Master4 != null && !powerData1.FilledWithNull.Contains("master4"))
                powerData1.Master4 = powerData2.Master4;
            if (powerData1.Master5 == null && powerData2.Master5 != null && !powerData1.FilledWithNull.Contains("master5"))
                powerData1.Master5 = powerData2.Master5;
            if (powerData1.Master6 == null && powerData2.Master6 != null && !powerData1.FilledWithNull.Contains("master6"))
                powerData1.Master6 = powerData2.Master6;
            if (powerData1.Money == null && powerData2.Money != null && !powerData1.FilledWithNull.Contains("money"))
                powerData1.Money = powerData2.Money;
            if (powerData1.TrainingAveragePercent == null && powerData2.TrainingAveragePercent != null && !powerData1.FilledWithNull.Contains("training_average"))
                powerData1.TrainingAveragePercent = powerData2.TrainingAveragePercent;
            if (powerData1.TrainingUp == null && powerData2.TrainingUp != null && !powerData1.FilledWithNull.Contains("training_up"))
                powerData1.TrainingUp = powerData2.TrainingUp;
            if (powerData1.Volume == null && powerData2.Volume != null && !powerData1.FilledWithNull.Contains("volume"))
                powerData1.Volume = powerData2.Volume;
            if (powerData1.Yabo == null && powerData2.Yabo != null && !powerData1.FilledWithNull.Contains("yabo"))
                powerData1.Yabo = powerData2.Yabo;
            if (powerData1.Diplo.Count == 0 && powerData2.Diplo.Count != 0 && !powerData1.FilledWithNull.Contains("diplo"))
                foreach (var item in powerData2.Diplo)
                    powerData1.Diplo[item.Key] = item.Value;
            if (powerData1.EnemyPower.Count == 0 && powerData2.EnemyPower.Count != 0 && !powerData1.FilledWithNull.Contains("enemy"))
                foreach (var item in powerData2.EnemyPower)
                    powerData1.EnemyPower[item.Key] = item.Value;
            if (powerData1.League.Count == 0 && powerData2.League.Count != 0 && !powerData1.FilledWithNull.Contains("league"))
                foreach (var item in powerData2.League)
                    powerData1.League[item.Key] = item.Value;
            if (powerData1.Loyals.Count == 0 && powerData2.Loyals.Count != 0 && !powerData1.FilledWithNull.Contains("loyal"))
                foreach (var item in powerData2.Loyals)
                    powerData1.Loyals[item.Key] = item.Value;
            if (powerData1.Merits.Count == 0 && powerData2.Merits.Count != 0 && !powerData1.FilledWithNull.Contains("merit"))
                foreach (var item in powerData2.Merits)
                    powerData1.Merits[item.Key] = item.Value;
            if (powerData1.Friend.Count == 0 && powerData2.Friend.Count != 0 && !powerData1.FilledWithNull.Contains("friend"))
                powerData1.Friend.AddRange(powerData2.Friend);
            if (powerData1.MemberSpot.Count == 0 && powerData2.MemberSpot.Count != 0 && !powerData1.FilledWithNull.Contains("member"))
                powerData1.MemberSpot.AddRange(powerData2.MemberSpot);
            if (powerData1.Merce.Count == 0 && powerData2.Merce.Count != 0 && !powerData1.FilledWithNull.Contains("merce"))
                powerData1.Merce.AddRange(powerData2.Merce);
            if (powerData1.Staff.Count == 0 && powerData2.Staff.Count != 0 && !powerData1.FilledWithNull.Contains("staff"))
                powerData1.Staff.AddRange(powerData2.Staff);
            if (powerData1.HomeSpot.Count == 0 && powerData2.HomeSpot.Count != 0 && !powerData1.FilledWithNull.Contains("home"))
                powerData1.HomeSpot.AddRange(powerData2.HomeSpot);
        }
        internal static void Resolve(UnitData unitData1, UnitData unitData2)
        {
            Resolve(unitData1, (CommonUnitData)unitData2);
            if (unitData1.ActiveTroop == null && unitData1.ActiveTime == null && unitData1.ActiveRect == null && unitData1.ActiveRange == null && !unitData1.FilledWithNull.Contains("active_num"))
            {
                unitData1.ActiveRange = unitData2.ActiveRange;
                unitData1.ActiveRect = unitData2.ActiveRect;
                unitData1.ActiveTroop = unitData2.ActiveTroop;
                unitData1.ActiveTime = unitData2.ActiveTime;
            }
            if (unitData1.ActiveType == null && unitData2.ActiveType != null && !unitData1.FilledWithNull.Contains("active"))
                unitData1.ActiveType = unitData2.ActiveType;
            if (unitData1.Align == null && unitData2.Align != null && !unitData1.FilledWithNull.Contains("align"))
                unitData1.Align = unitData2.Align;
            if (unitData1.AlivePercentage == null && unitData2.AlivePercentage != null && !unitData1.FilledWithNull.Contains("alive_per"))
                unitData1.AlivePercentage = unitData2.AlivePercentage;
            if (unitData1.ArbeitCapacity == null && unitData2.ArbeitCapacity != null && !unitData1.FilledWithNull.Contains("arbeit_capacity"))
                unitData1.ArbeitCapacity = unitData2.ArbeitCapacity;
            if (unitData1.ArbeitType == null && unitData1.ArbeitPercentage == null && !unitData1.FilledWithNull.Contains("arbeit"))
            {
                unitData1.ArbeitPercentage = unitData2.ArbeitPercentage;
                unitData1.ArbeitType = unitData2.ArbeitType;
            }
            if (unitData1.BGM == null && unitData2.BGM != null && !unitData1.FilledWithNull.Contains("bgm"))
                unitData1.BGM = unitData2.BGM;
            if (unitData1.Break == null && unitData2.Break != null && !unitData1.FilledWithNull.Contains("break"))
                unitData1.Break = unitData2.Break;
            if (unitData1.BreastWidth == null && unitData2.BreastWidth != null && !unitData1.FilledWithNull.Contains("breast_width"))
                unitData1.BreastWidth = unitData2.BreastWidth;
            if (unitData1.Dead == null && unitData2.Dead != null && !unitData1.FilledWithNull.Contains("dead"))
                unitData1.Dead = unitData2.Dead;
            if (unitData1.Diplomacy == null && unitData2.Diplomacy != null && !unitData1.FilledWithNull.Contains("diplomacy"))
                unitData1.Diplomacy = unitData2.Diplomacy;
            if (unitData1.EnableTurn == null && unitData2.EnableTurn != null && !unitData1.FilledWithNull.Contains("enable"))
                unitData1.EnableTurn = unitData2.EnableTurn;
            if (unitData1.EnableTurnMax == null && unitData2.EnableTurnMax != null && !unitData1.FilledWithNull.Contains("enable_max"))
                unitData1.EnableTurnMax = unitData2.EnableTurnMax;
            if (unitData1.Fix == null && unitData2.Fix != null && !unitData1.FilledWithNull.Contains("fix"))
                unitData1.Fix = unitData2.Fix;
            if (unitData1.Flag == null && unitData2.Flag != null && !unitData1.FilledWithNull.Contains("flag"))
                unitData1.Flag = unitData2.Flag;
            if (unitData1.Help == null && unitData2.Help != null && !unitData1.FilledWithNull.Contains("help"))
                unitData1.Help = unitData2.Help;
            if (unitData1.IsActor == null && unitData2.IsActor != null && !unitData1.FilledWithNull.Contains("actor"))
                unitData1.IsActor = unitData2.IsActor;
            if (unitData1.IsEnableSelect == null && unitData2.IsEnableSelect != null && !unitData1.FilledWithNull.Contains("enable_select"))
                unitData1.IsEnableSelect = unitData2.IsEnableSelect;
            if (unitData1.IsNoEmployUnit == null && unitData2.IsNoEmployUnit != null && !unitData1.FilledWithNull.Contains("noemploy_unit"))
                unitData1.IsNoEmployUnit = unitData2.IsNoEmployUnit;
            if (unitData1.IsNoEscape == null && unitData2.IsNoEscape != null && !unitData1.FilledWithNull.Contains("no_escape"))
                unitData1.IsNoEscape = unitData2.IsNoEscape;
            if (unitData1.IsNoItemUnit == null && unitData2.IsNoItemUnit != null && !unitData1.FilledWithNull.Contains("noitem_unit"))
                unitData1.IsNoItemUnit = unitData2.IsNoItemUnit;
            if (unitData1.IsNoRemoveUnit == null && unitData2.IsNoRemoveUnit != null && !unitData1.FilledWithNull.Contains("noremove_unit"))
                unitData1.IsNoRemoveUnit = unitData2.IsNoRemoveUnit;
            if (unitData1.IsTalent == null && unitData2.IsTalent != null && !unitData1.FilledWithNull.Contains("talent"))
                unitData1.IsTalent = unitData2.IsTalent;
            if (unitData1.Volume == null && unitData2.Volume != null && !unitData1.FilledWithNull.Contains("volume"))
                unitData1.Volume = unitData2.Volume;
            if (unitData1.Picture == null && unitData2.Picture != null && !unitData1.FilledWithNull.Contains("picture"))
                unitData1.Picture = unitData2.Picture;
            if (unitData1.PictureBack == null && unitData2.PictureBack != null && !unitData1.FilledWithNull.Contains("picture_back"))
                unitData1.PictureBack = unitData2.PictureBack;
            if (unitData1.PictureCenter == null && unitData2.PictureCenter != null && !unitData1.FilledWithNull.Contains("picture_center"))
                unitData1.PictureCenter = unitData2.PictureCenter;
            if (unitData1.PictureDetail == null && unitData2.PictureDetail != null && !unitData1.FilledWithNull.Contains("picture_detail"))
                unitData1.PictureDetail = unitData2.PictureDetail;
            if (unitData1.PictureFloor == null && unitData2.PictureFloor != null && !unitData1.FilledWithNull.Contains("picture_floor"))
                unitData1.PictureFloor = unitData2.PictureFloor;
            if (unitData1.PictureMenu == null && unitData2.PictureMenu != null && !unitData1.FilledWithNull.Contains("picture_menu"))
                unitData1.PictureMenu = unitData2.PictureMenu;
            if (unitData1.PictureShift == null && unitData2.PictureShift != null && !unitData1.FilledWithNull.Contains("picture_shift"))
                unitData1.PictureShift = unitData2.PictureShift;
            if (unitData1.PictureShiftUp == null && unitData2.PictureShiftUp != null && !unitData1.FilledWithNull.Contains("picture_shift_up"))
                unitData1.PictureShiftUp = unitData2.PictureShiftUp;
            if (unitData1.Medical == null && unitData2.Medical != null && !unitData1.FilledWithNull.Contains("medical"))
                unitData1.Medical = unitData2.Medical;
            if (unitData1.Yabo == null && unitData2.Yabo != null && !unitData1.FilledWithNull.Contains("yabo"))
                unitData1.Yabo = unitData2.Yabo;
            if (unitData1.Kosen == null && unitData2.Kosen != null && !unitData1.FilledWithNull.Contains("kosen"))
                unitData1.Kosen = unitData2.Kosen;
            if (unitData1.Loyal == null && unitData2.Loyal != null && !unitData1.FilledWithNull.Contains("loyal"))
                unitData1.Loyal = unitData2.Loyal;
            if (unitData1.PowerDisplayName == null && unitData2.PowerDisplayName != null && !unitData1.FilledWithNull.Contains("power_name"))
                unitData1.PowerDisplayName = unitData2.PowerDisplayName;
            if (unitData1.Join == null && unitData2.Join != null && !unitData1.FilledWithNull.Contains("join"))
                unitData1.Join = unitData2.Join;
            if (unitData1.Retreat == null && unitData2.Retreat != null && !unitData1.FilledWithNull.Contains("retreat"))
                unitData1.Retreat = unitData2.Retreat;
            if (unitData1.VoiceType == null && unitData2.VoiceType != null && !unitData1.FilledWithNull.Contains("voice_type"))
                unitData1.VoiceType = unitData2.VoiceType;
            if (unitData1.Team == null && unitData2.Team != null && !unitData1.FilledWithNull.Contains("team"))
                unitData1.Team = unitData2.Team;
            if (unitData1.KeepForm == null && unitData2.KeepForm != null && !unitData1.FilledWithNull.Contains("keep_form"))
                unitData1.KeepForm = unitData2.KeepForm;
            if (unitData1.Staff.Count == 0 && unitData2.Staff.Count != 0 && !unitData1.FilledWithNull.Contains("staff"))
                unitData1.Staff.AddRange(unitData2.Staff);
            if (unitData1.Enemy.Count == 0 && unitData2.Enemy.Count != 0 && !unitData1.FilledWithNull.Contains("enemy"))
                unitData1.Enemy.AddRange(unitData2.Enemy);
            if (unitData1.Home.Count == 0 && unitData2.Home.Count != 0 && !unitData1.FilledWithNull.Contains("home"))
                unitData1.Home.AddRange(unitData2.Home);
            if (unitData1.LeaderSkill.Count == 0 && unitData2.LeaderSkill.Count != 0 && !unitData1.FilledWithNull.Contains("leader_skill"))
                foreach (var item in unitData2.LeaderSkill)
                    unitData1.LeaderSkill[item.Key] = item.Value;
            if (unitData1.AssistSkill.Count == 0 && unitData2.AssistSkill.Count != 0 && !unitData1.FilledWithNull.Contains("assist_skill"))
                foreach (var item in unitData2.AssistSkill)
                    unitData1.AssistSkill[item.Key] = item.Value;
            if (unitData1.CastleGuard.Count == 0 && unitData2.CastleGuard.Count != 0 && !unitData1.FilledWithNull.Contains("castle_guard"))
                foreach (var item in unitData2.CastleGuard)
                    unitData1.CastleGuard[item.Key] = item.Value;
        }
        internal static void Resolve(GenericUnitData genericUnitData1, GenericUnitData genericUnitData2)
        {
            Resolve(genericUnitData1, (CommonUnitData)genericUnitData2);
            if (genericUnitData1.BaseClassKey == null && genericUnitData2.BaseClassKey != null && !genericUnitData1.FilledWithNull.Contains("fkey"))
                genericUnitData1.BaseClassKey = genericUnitData2.BaseClassKey;
            if (genericUnitData1.Change == null && genericUnitData2.Change != null && !genericUnitData1.FilledWithNull.Contains("change"))
                genericUnitData1.Change = genericUnitData2.Change;
            if (genericUnitData1.IsUnique == null && genericUnitData2.IsUnique != null && !genericUnitData1.FilledWithNull.Contains("unique"))
                genericUnitData1.IsUnique = genericUnitData2.IsUnique;
        }
        internal static void Resolve(CommonUnitData unitData1, CommonUnitData unitData2)
        {
            unitData1.VariantData.ResolveVariant(unitData2.VariantData);
            if (unitData1.Alpha == null && unitData2.Alpha != null && !unitData1.FilledWithNull.Contains("a"))
                unitData1.Alpha = unitData2.Alpha;
            if (unitData1.AddVassal == null && unitData2.AddVassal != null && !unitData1.FilledWithNull.Contains("vassal"))
                unitData1.AddVassal = unitData2.AddVassal;
            if (unitData1.Attack == null && unitData2.Attack != null && !unitData1.FilledWithNull.Contains("attack"))
                unitData1.Attack = unitData2.Attack;
            if (unitData1.attack_max == null && unitData2.attack_max != null && !unitData1.FilledWithNull.Contains("attack_max"))
                unitData1.attack_max = unitData2.attack_max;
            if (unitData1.attackMax == null && unitData2.attackMax != null && !unitData1.FilledWithNull.Contains("attackmax"))
                unitData1.attackMax = unitData2.attackMax;
            if (unitData1.attackUp == null && unitData2.attackUp != null && !unitData1.FilledWithNull.Contains("attackup"))
                unitData1.attackUp = unitData2.attackUp;
            if (unitData1.Defense == null && unitData2.Defense != null && !unitData1.FilledWithNull.Contains("defense"))
                unitData1.Defense = unitData2.Defense;
            if (unitData1.defense_max == null && unitData2.defense_max != null && !unitData1.FilledWithNull.Contains("defense_max"))
                unitData1.defense_max = unitData2.defense_max;
            if (unitData1.defenseMax == null && unitData2.defenseMax != null && !unitData1.FilledWithNull.Contains("defensemax"))
                unitData1.defenseMax = unitData2.defenseMax;
            if (unitData1.defenseUp == null && unitData2.defenseUp != null && !unitData1.FilledWithNull.Contains("defenseup"))
                unitData1.defenseUp = unitData2.defenseUp;
            if (unitData1.Magic == null && unitData2.Magic != null && !unitData1.FilledWithNull.Contains("magic"))
                unitData1.Magic = unitData2.Magic;
            if (unitData1.magic_max == null && unitData2.magic_max != null && !unitData1.FilledWithNull.Contains("magic_max"))
                unitData1.magic_max = unitData2.magic_max;
            if (unitData1.magicMax == null && unitData2.magicMax != null && !unitData1.FilledWithNull.Contains("magicmax"))
                unitData1.magicMax = unitData2.magicMax;
            if (unitData1.magicUp == null && unitData2.magicUp != null && !unitData1.FilledWithNull.Contains("magicup"))
                unitData1.magicUp = unitData2.magicUp;
            if (unitData1.Magdef == null && unitData2.Magdef != null && !unitData1.FilledWithNull.Contains("magdef"))
                unitData1.Magdef = unitData2.Magdef;
            if (unitData1.magdef_max == null && unitData2.magdef_max != null && !unitData1.FilledWithNull.Contains("magdef_max"))
                unitData1.magdef_max = unitData2.magdef_max;
            if (unitData1.magdefMax == null && unitData2.magdefMax != null && !unitData1.FilledWithNull.Contains("magdefmax"))
                unitData1.magdefMax = unitData2.magdefMax;
            if (unitData1.magdefUp == null && unitData2.magdefUp != null && !unitData1.FilledWithNull.Contains("magdefup"))
                unitData1.magdefUp = unitData2.magdefUp;
            if (unitData1.Dext == null && unitData2.Dext != null && !unitData1.FilledWithNull.Contains("dext"))
                unitData1.Dext = unitData2.Dext;
            if (unitData1.dext_max == null && unitData2.dext_max != null && !unitData1.FilledWithNull.Contains("dext_max"))
                unitData1.dext_max = unitData2.dext_max;
            if (unitData1.dextMax == null && unitData2.dextMax != null && !unitData1.FilledWithNull.Contains("dextmax"))
                unitData1.dextMax = unitData2.dextMax;
            if (unitData1.dextUp == null && unitData2.dextUp != null && !unitData1.FilledWithNull.Contains("dextup"))
                unitData1.dextUp = unitData2.dextUp;
            if (unitData1.Speed == null && unitData2.Speed != null && !unitData1.FilledWithNull.Contains("speed"))
                unitData1.Speed = unitData2.Speed;
            if (unitData1.speed_max == null && unitData2.speed_max != null && !unitData1.FilledWithNull.Contains("speed_max"))
                unitData1.speed_max = unitData2.speed_max;
            if (unitData1.speedMax == null && unitData2.speedMax != null && !unitData1.FilledWithNull.Contains("speedmax"))
                unitData1.speedMax = unitData2.speedMax;
            if (unitData1.speedUp == null && unitData2.speedUp != null && !unitData1.FilledWithNull.Contains("speedup"))
                unitData1.speedUp = unitData2.speedUp;
            if (unitData1.Move == null && unitData2.Move != null && !unitData1.FilledWithNull.Contains("move"))
                unitData1.Move = unitData2.Move;
            if (unitData1.move_max == null && unitData2.move_max != null && !unitData1.FilledWithNull.Contains("move_max"))
                unitData1.move_max = unitData2.move_max;
            if (unitData1.moveMax == null && unitData2.moveMax != null && !unitData1.FilledWithNull.Contains("movemax"))
                unitData1.moveMax = unitData2.moveMax;
            if (unitData1.moveUp == null && unitData2.moveUp != null && !unitData1.FilledWithNull.Contains("moveup"))
                unitData1.moveUp = unitData2.moveUp;
            if (unitData1.Hp == null && unitData2.Hp != null && !unitData1.FilledWithNull.Contains("hp"))
                unitData1.Hp = unitData2.Hp;
            if (unitData1.HpRec == null && unitData2.HpRec != null && !unitData1.FilledWithNull.Contains("hprec"))
                unitData1.HpRec = unitData2.HpRec;
            if (unitData1.hprec_max == null && unitData2.hprec_max != null && !unitData1.FilledWithNull.Contains("hprec_max"))
                unitData1.hprec_max = unitData2.hprec_max;
            if (unitData1.hpMax == null && unitData2.hpMax != null && !unitData1.FilledWithNull.Contains("hpmax"))
                unitData1.hpMax = unitData2.hpMax;
            if (unitData1.hpUp == null && unitData2.hpUp != null && !unitData1.FilledWithNull.Contains("hpup"))
                unitData1.hpUp = unitData2.hpUp;
            if (unitData1.hprecMax == null && unitData2.hprecMax != null && !unitData1.FilledWithNull.Contains("hprecmax"))
                unitData1.hprecMax = unitData2.hprecMax;
            if (unitData1.hprecUp == null && unitData2.hprecUp != null && !unitData1.FilledWithNull.Contains("hprecup"))
                unitData1.hprecUp = unitData2.hprecUp;
            if (unitData1.Mp == null && unitData2.Mp != null && !unitData1.FilledWithNull.Contains("mp"))
                unitData1.Mp = unitData2.Mp;
            if (unitData1.MpRec == null && unitData2.MpRec != null && !unitData1.FilledWithNull.Contains("mprec"))
                unitData1.MpRec = unitData2.MpRec;
            if (unitData1.mprec_max == null && unitData2.mprec_max != null && !unitData1.FilledWithNull.Contains("mprec_max"))
                unitData1.mprec_max = unitData2.mprec_max;
            if (unitData1.mprecMax == null && unitData2.mprecMax != null && !unitData1.FilledWithNull.Contains("mprecmax"))
                unitData1.mprecMax = unitData2.mprecMax;
            if (unitData1.mprecUp == null && unitData2.mprecUp != null && !unitData1.FilledWithNull.Contains("mprecup"))
                unitData1.mprecUp = unitData2.mprecUp;
            if (unitData1.mpUp == null && unitData2.mpUp != null && !unitData1.FilledWithNull.Contains("mpup"))
                unitData1.mpUp = unitData2.mpUp;
            if (unitData1.Brave == null && unitData2.Brave != null && !unitData1.FilledWithNull.Contains("brave"))
            {
                unitData1.Brave = unitData2.Brave;
                unitData1.IsBrave = unitData2.IsBrave;
            }
            if (unitData1.CavalryRange == null && unitData2.CavalryRange != null && !unitData1.FilledWithNull.Contains("cavalary_range"))
                unitData1.CavalryRange = unitData2.CavalryRange;
            if (unitData1.Class == null && unitData2.Class != null && !unitData1.FilledWithNull.Contains("class"))
                unitData1.Class = unitData2.Class;
            if (unitData1.Cost == null && unitData2.Cost != null && !unitData1.FilledWithNull.Contains("cost"))
                unitData1.Cost = unitData2.Cost;
            if (unitData1.DeadEvent == null && unitData2.DeadEvent != null && !unitData1.FilledWithNull.Contains("dead_event"))
                unitData1.DeadEvent = unitData2.DeadEvent;
            if (unitData1.DisplayName == null && unitData2.DisplayName != null && !unitData1.FilledWithNull.Contains("name"))
                unitData1.DisplayName = unitData2.DisplayName;
            if (unitData1.exp == null && unitData2.exp != null && !unitData1.FilledWithNull.Contains("exp"))
                unitData1.exp = unitData2.exp;
            if (unitData1.exp_max == null && unitData2.exp_max != null && !unitData1.FilledWithNull.Contains("exp_max"))
                unitData1.exp_max = unitData2.exp_max;
            if (unitData1.exp_mul == null && unitData2.exp_mul != null && !unitData1.FilledWithNull.Contains("exp_mul"))
                unitData1.exp_mul = unitData2.exp_mul;
            if (unitData1.EscapeRange == null && unitData2.EscapeRange != null && !unitData1.FilledWithNull.Contains("escape_range"))
                unitData1.EscapeRange = unitData2.EscapeRange;
            if (unitData1.EscapeRun == null && unitData2.EscapeRun != null && !unitData1.FilledWithNull.Contains("escape_run"))
                unitData1.EscapeRun = unitData2.EscapeRun;
            if (unitData1.Face == null && unitData2.Face != null && !unitData1.FilledWithNull.Contains("face"))
                unitData1.Face = unitData2.Face;
            if (unitData1.FreeMove == null && unitData2.FreeMove != null && !unitData1.FilledWithNull.Contains("free_move"))
                unitData1.FreeMove = unitData2.FreeMove;
            if (unitData1.HandRange == null && unitData2.HandRange != null && !unitData1.FilledWithNull.Contains("hand_range"))
                unitData1.HandRange = unitData2.HandRange;
            if (unitData1.heal_max == null && unitData2.heal_max != null && !unitData1.FilledWithNull.Contains("heal_max"))
                unitData1.heal_max = unitData2.heal_max;
            if (unitData1.HasEXP == null && unitData2.HasEXP != null && !unitData1.FilledWithNull.Contains("hasexp"))
                unitData1.HasEXP = unitData2.HasEXP;
            if (unitData1.Height == null && unitData2.Height != null && !unitData1.FilledWithNull.Contains("h"))
                unitData1.Height = unitData2.Height;
            if (unitData1.Image == null && unitData2.Image != null && !unitData1.FilledWithNull.Contains("image"))
                unitData1.Image = unitData2.Image;
            if (unitData1.Image2 == null && unitData2.Image2 != null && !unitData1.FilledWithNull.Contains("image2"))
                unitData1.Image2 = unitData2.Image2;
            if (unitData1.IsBeast == null && unitData2.IsBeast != null && !unitData1.FilledWithNull.Contains("beast"))
                unitData1.IsBeast = unitData2.IsBeast;
            if (unitData1.IsElementLost == null && unitData2.IsElementLost != null && !unitData1.FilledWithNull.Contains("element_lost"))
                unitData1.IsElementLost = unitData2.IsElementLost;
            if (unitData1.IsHandle == null && unitData2.IsHandle != null && !unitData1.FilledWithNull.Contains("handle"))
                unitData1.IsHandle = unitData2.IsHandle;
            if (unitData1.IsNoCover == null && unitData2.IsNoCover != null && !unitData1.FilledWithNull.Contains("no_cover"))
                unitData1.IsNoCover = unitData2.IsNoCover;
            if (unitData1.IsNoTraining == null && unitData2.IsNoTraining != null && !unitData1.FilledWithNull.Contains("no_training"))
                unitData1.IsNoTraining = unitData2.IsNoTraining;
            if (unitData1.IsSameCall == null && unitData2.IsSameCall != null && !unitData1.FilledWithNull.Contains("same_call"))
                unitData1.IsSameCall = unitData2.IsSameCall;
            if (unitData1.IsSameCallBaseUp == null && unitData2.IsSameCallBaseUp != null && !unitData1.FilledWithNull.Contains("samecall_baseup"))
                unitData1.IsSameCallBaseUp = unitData2.IsSameCallBaseUp;
            if (unitData1.IsSameFriend == null && unitData2.IsSameFriend != null && !unitData1.FilledWithNull.Contains("same_friend"))
                unitData1.IsSameFriend = unitData2.IsSameFriend;
            if (unitData1.IsSameSex == null && unitData2.IsSameSex != null && !unitData1.FilledWithNull.Contains("same_sex"))
                unitData1.IsSameSex = unitData2.IsSameSex;
            if (unitData1.IsSatellite == null && unitData2.IsSatellite != null && !unitData1.FilledWithNull.Contains("satellite"))
            {
                unitData1.IsSatellite = unitData2.IsSatellite;
                unitData1.Satellite = unitData2.Satellite;
            }
            if (unitData1.IsTkool == null && unitData2.IsTkool != null && !unitData1.FilledWithNull.Contains("tkool"))
                unitData1.IsTkool = unitData2.IsTkool;
            if (unitData1.IsViewUnit == null && unitData2.IsViewUnit != null && !unitData1.FilledWithNull.Contains("view_unit"))
                unitData1.IsViewUnit = unitData2.IsViewUnit;
            if (unitData1.Level == null && unitData2.Level != null && !unitData1.FilledWithNull.Contains("level"))
                unitData1.Level = unitData2.Level;
            if (unitData1.DefenseLine == null && unitData2.DefenseLine != null && !unitData1.FilledWithNull.Contains("line"))
                unitData1.DefenseLine = unitData2.DefenseLine;
            if (unitData1.LostCorpse == null && unitData2.LostCorpse != null && !unitData1.FilledWithNull.Contains("lost_corpse"))
                unitData1.LostCorpse = unitData2.LostCorpse;
            if (unitData1.MoveType == null && unitData2.MoveType != null && !unitData1.FilledWithNull.Contains("movetype"))
                unitData1.MoveType = unitData2.MoveType;
            if (unitData1.NoKnock == null && unitData2.NoKnock != null && !unitData1.FilledWithNull.Contains("no_knock"))
                unitData1.NoKnock = unitData2.NoKnock;
            if (unitData1.Price == null && unitData2.Price != null && !unitData1.FilledWithNull.Contains("price"))
                unitData1.Price = unitData2.Price;
            if (unitData1.Race == null && unitData2.Race != null && !unitData1.FilledWithNull.Contains("race"))
                unitData1.Race = unitData2.Race;
            if (unitData1.Radius == null && unitData2.Radius != null && !unitData1.FilledWithNull.Contains("radius"))
                unitData1.Radius = unitData2.Radius;
            if (unitData1.RadiusPress == null && unitData2.RadiusPress != null && !unitData1.FilledWithNull.Contains("radius_press"))
                unitData1.RadiusPress = unitData2.RadiusPress;
            if (unitData1.Scream == null && unitData2.Scream != null && !unitData1.FilledWithNull.Contains("scream"))
                unitData1.Scream = unitData2.Scream;
            if (unitData1.Sex == null && unitData2.Sex != null && !unitData1.FilledWithNull.Contains("sex"))
                unitData1.Sex = unitData2.Sex;
            if (unitData1.SortKey == null && unitData2.SortKey != null && !unitData1.FilledWithNull.Contains("sortkey"))
                unitData1.SortKey = unitData2.SortKey;
            if (unitData1.summon_level == null && unitData2.summon_level != null && !unitData1.FilledWithNull.Contains("summon_level"))
                unitData1.summon_level = unitData2.summon_level;
            if (unitData1.summon_max == null && unitData2.summon_max != null && !unitData1.FilledWithNull.Contains("summon_max"))
                unitData1.summon_max = unitData2.summon_max;
            if (unitData1.ViewRange == null && unitData2.ViewRange != null && !unitData1.FilledWithNull.Contains("view_range"))
                unitData1.ViewRange = unitData2.ViewRange;
            if (unitData1.WakeRange == null && unitData2.WakeRange != null && !unitData1.FilledWithNull.Contains("wake_range"))
                unitData1.WakeRange = unitData2.WakeRange;
            if (unitData1.Width == null && unitData2.Width != null && !unitData1.FilledWithNull.Contains("w"))
                unitData1.Width = unitData2.Width;
            if (unitData1.DeleteSkill.Count == 0 && unitData2.DeleteSkill.Count != 0 && !unitData1.FilledWithNull.Contains("delskill"))
                unitData1.DeleteSkill.AddRange(unitData2.DeleteSkill);
            if (unitData1.DeleteSkill2.Count == 0 && unitData2.DeleteSkill2.Count != 0 && !unitData1.FilledWithNull.Contains("delskill2"))
                unitData1.DeleteSkill2.AddRange(unitData2.DeleteSkill2);
            if (unitData1.FriendEx1.Count == 0 && unitData2.FriendEx1.Count != 0 && !unitData1.FilledWithNull.Contains("friendex"))
            {
                unitData1.FriendEx1.AddRange(unitData2.FriendEx1);
                unitData1.FriendEx2.AddRange(unitData2.FriendEx2);
                unitData1.FriendExCount = unitData2.FriendExCount;
            }
            if (unitData1.Friends.Count == 0 && unitData2.Friends.Count != 0 && !unitData1.FilledWithNull.Contains("friend"))
            {
                unitData1.Friends.AddRange(unitData2.Friends);
                unitData1.IsFriendAllClass = unitData2.IsFriendAllClass;
                unitData1.IsFriendAllRace = unitData2.IsFriendAllRace;
            }
            if (unitData1.Item.Count == 0 && unitData2.Item.Count != 0 && !unitData1.FilledWithNull.Contains("item"))
                unitData1.Item.AddRange(unitData2.Item);
            if (unitData1.Member.Count == 0 && unitData2.Member.Count != 0 && !unitData1.FilledWithNull.Contains("member"))
                unitData1.Member.AddRange(unitData2.Member);
            if (unitData1.Merce.Count == 0 && unitData2.Merce.Count != 0 && !unitData1.FilledWithNull.Contains("merce"))
                unitData1.Merce.AddRange(unitData2.Merce);
            if (unitData1.Skill.Count == 0 && unitData2.Skill.Count != 0 && !unitData1.FilledWithNull.Contains("skill"))
                unitData1.Skill.AddRange(unitData2.Skill);
            if (unitData1.Skill2.Count == 0 && unitData2.Skill2.Count != 0 && !unitData1.FilledWithNull.Contains("skill2"))
                unitData1.Skill2.AddRange(unitData2.Skill2);
            if (unitData1.Consti.Count == 0 && unitData2.Consti.Count != 0 && !unitData1.FilledWithNull.Contains("consti"))
                foreach (var item in unitData2.Consti)
                    unitData1.Consti[item.Key] = item.Value;
            if (unitData1.Learn.Count == 0 && unitData2.Learn.Count != 0 && !unitData1.FilledWithNull.Contains("learn"))
                foreach (var item in unitData2.Learn)
                    unitData1.Learn[item.Key] = item.Value;
            if (unitData1.Multi.Count == 0 && unitData2.Multi.Count != 0 && !unitData1.FilledWithNull.Contains("multi"))
                foreach (var item in unitData2.Multi)
                    unitData1.Multi[item.Key] = item.Value;
        }
        internal static void Resolve(ScenarioData scenarioData1, ScenarioData scenarioData2)
        {
            if (scenarioData1.ActorPercent == null && scenarioData2.ActorPercent != null && !scenarioData1.FilledWithNull.Contains("actor_per"))
                scenarioData1.ActorPercent = scenarioData2.ActorPercent;
            if (scenarioData1.BaseLevelUp == null && scenarioData2.BaseLevelUp != null && !scenarioData1.FilledWithNull.Contains("base_level"))
                scenarioData1.BaseLevelUp = scenarioData2.BaseLevelUp;
            if (scenarioData1.BeginText == null && scenarioData2.BeginText != null && !scenarioData1.FilledWithNull.Contains("begin_text"))
                scenarioData1.BeginText = scenarioData2.BeginText;
            if (scenarioData1.DescriptionText == null && scenarioData2.DescriptionText != null && !scenarioData1.FilledWithNull.Contains("text"))
                scenarioData1.DescriptionText = scenarioData2.DescriptionText;
            if (scenarioData1.DisplayName == null && scenarioData2.DisplayName != null && !scenarioData1.FilledWithNull.Contains("name"))
                scenarioData1.DisplayName = scenarioData2.DisplayName;
            if (scenarioData1.FightEvent == null && scenarioData2.FightEvent != null && !scenarioData1.FilledWithNull.Contains("fight"))
                scenarioData1.FightEvent = scenarioData2.FightEvent;
            if (scenarioData1.GainPercent == null && scenarioData2.GainPercent != null && !scenarioData1.FilledWithNull.Contains("gain_per"))
                scenarioData1.GainPercent = scenarioData2.GainPercent;
            if (scenarioData1.X == null && scenarioData2.X != null && !scenarioData1.FilledWithNull.Contains("x"))
                scenarioData1.X = scenarioData2.X;
            if (scenarioData1.Y == null && scenarioData2.Y != null && !scenarioData1.FilledWithNull.Contains("y"))
                scenarioData1.Y = scenarioData2.Y;
            if (scenarioData1.IsAbleToMerce == null && scenarioData2.IsAbleToMerce != null && !scenarioData1.FilledWithNull.Contains("party"))
                scenarioData1.IsAbleToMerce = scenarioData2.IsAbleToMerce;
            if (scenarioData1.IsNoAutoSave == null && scenarioData2.IsNoAutoSave != null && !scenarioData1.FilledWithNull.Contains("no_autosave"))
                scenarioData1.IsNoAutoSave = scenarioData2.IsNoAutoSave;
            if (scenarioData1.IsDefaulEnding == null && scenarioData2.IsDefaulEnding != null && !scenarioData1.FilledWithNull.Contains("default_ending"))
                scenarioData1.IsDefaulEnding = scenarioData2.IsDefaulEnding;
            if (scenarioData1.IsEnable == null && scenarioData2.IsEnable != null && !scenarioData1.FilledWithNull.Contains("enable"))
                scenarioData1.IsEnable = scenarioData2.IsEnable;
            if (scenarioData1.IsItemLimit == null && scenarioData2.IsItemLimit != null && !scenarioData1.FilledWithNull.Contains("item_limit"))
                scenarioData1.IsItemLimit = scenarioData2.IsItemLimit;
            if (scenarioData1.IsNoZone == null && scenarioData2.IsNoZone != null && !scenarioData1.FilledWithNull.Contains("nozone"))
                scenarioData1.IsNoZone = scenarioData2.IsNoZone;
            if (scenarioData1.IsPlayableAsTalent == null && scenarioData2.IsPlayableAsTalent != null && !scenarioData1.FilledWithNull.Contains("enable_talent"))
                scenarioData1.IsPlayableAsTalent = scenarioData2.IsPlayableAsTalent;
            if (scenarioData1.MonsterLevel == null && scenarioData2.MonsterLevel != null && !scenarioData1.FilledWithNull.Contains("monster_level"))
                scenarioData1.MonsterLevel = scenarioData2.MonsterLevel;
            if (scenarioData1.MyHelpRange == null && scenarioData2.MyHelpRange != null && !scenarioData1.FilledWithNull.Contains("myhelp_range"))
                scenarioData1.MyHelpRange = scenarioData2.MyHelpRange;
            if (scenarioData1.MyRange == null && scenarioData2.MyRange != null && !scenarioData1.FilledWithNull.Contains("my_range"))
                scenarioData1.MyRange = scenarioData2.MyRange;
            if (scenarioData1.Order == null && scenarioData2.Order != null && !scenarioData1.FilledWithNull.Contains("order"))
                scenarioData1.Order = scenarioData2.Order;
            if (scenarioData1.PoliticsEvent == null && scenarioData2.PoliticsEvent != null && !scenarioData1.FilledWithNull.Contains("politics"))
                scenarioData1.PoliticsEvent = scenarioData2.PoliticsEvent;
            if (scenarioData1.SortKey == null && scenarioData2.SortKey != null && !scenarioData1.FilledWithNull.Contains("sotkey"))
                scenarioData1.SortKey = scenarioData2.SortKey;
            if (scenarioData1.SpotCapacity == null && scenarioData2.SpotCapacity != null && !scenarioData1.FilledWithNull.Contains("spot_capacity"))
                scenarioData1.SpotCapacity = scenarioData2.SpotCapacity;
            if (scenarioData1.SupportRange == null && scenarioData2.SupportRange != null && !scenarioData1.FilledWithNull.Contains("support_range"))
                scenarioData1.SupportRange = scenarioData2.SupportRange;
            if (scenarioData1.TrainingUp == null && scenarioData2.TrainingUp != null && !scenarioData1.FilledWithNull.Contains("training_up"))
                scenarioData1.TrainingUp = scenarioData2.TrainingUp;
            if (scenarioData1.WorldEvent == null && scenarioData2.WorldEvent != null && !scenarioData1.FilledWithNull.Contains("world"))
                scenarioData1.WorldEvent = scenarioData2.WorldEvent;
            if (scenarioData1.WorldMapPath == null && scenarioData2.WorldMapPath != null && !scenarioData1.FilledWithNull.Contains("map"))
                scenarioData1.WorldMapPath = scenarioData2.WorldMapPath;
            if (scenarioData1.WarCapacity == null && scenarioData2.WarCapacity != null && !scenarioData1.FilledWithNull.Contains("war_capacity"))
                scenarioData1.WarCapacity = scenarioData2.WarCapacity;
            if (scenarioData1.ZoneName == null && scenarioData2.ZoneName != null && !scenarioData1.FilledWithNull.Contains("zone"))
                scenarioData1.ZoneName = scenarioData2.ZoneName;
            if (scenarioData1.PlayerInitialItem.Count == 0 && scenarioData2.PlayerInitialItem.Count != 0 && !scenarioData1.FilledWithNull.Contains("item"))
                scenarioData1.PlayerInitialItem.AddRange(scenarioData2.PlayerInitialItem);
            if (scenarioData1.Power.Count == 0 && scenarioData2.Power.Count != 0 && !scenarioData1.FilledWithNull.Contains("power"))
                scenarioData1.Power.AddRange(scenarioData2.Power);
            if (scenarioData1.Roamer.Count == 0 && scenarioData2.Roamer.Count != 0 && !scenarioData1.FilledWithNull.Contains("roam"))
                scenarioData1.Roamer.AddRange(scenarioData2.Roamer);
            if (scenarioData1.Spot.Count == 0 && scenarioData2.Spot.Count != 0 && !scenarioData1.FilledWithNull.Contains("spot"))
                scenarioData1.Spot.AddRange(scenarioData2.Spot);
            if (scenarioData1.CampingData.Count == 0 && scenarioData2.CampingData.Count != 0 && !scenarioData1.FilledWithNull.Contains("camp"))
                foreach (var item in scenarioData2.CampingData)
                    scenarioData1.CampingData[item.Key] = item.Value;
            if (scenarioData1.ItemSale.Count == 0 && scenarioData2.ItemSale.Count != 0 && !scenarioData1.FilledWithNull.Contains("item_sale"))
                scenarioData1.ItemSale.AddRange(scenarioData2.ItemSale);
            if (scenarioData1.ItemWindowTab[0].Item1 == null && scenarioData2.ItemWindowTab[0].Item1 != null)
                for (int i = 0; i < scenarioData1.ItemWindowTab.Length; i++)
                    scenarioData1.ItemWindowTab[i] = scenarioData2.ItemWindowTab[i];
            if (scenarioData1.PoliticsData.Count == 0 && scenarioData2.PoliticsData.Count != 0 && !scenarioData1.FilledWithNull.Contains("poli"))
                foreach (var item in scenarioData2.PoliticsData)
                    scenarioData1.PoliticsData[item.Key] = item.Value;
        }
        internal static void Parse<T>(IEnumerable<LexicalTree_Assign> enumerable, T data) where T : ScenarioVariantData
        {
            switch (data)
            {
                case SpotData _1:
                    Parse(enumerable, _1);
                    break;
                case PowerData _2:
                    Parse(enumerable, _2);
                    break;
                case GenericUnitData _3:
                    Parse(_3);
                    break;
                case UnitData _4:
                    Parse(_4);
                    break;
            }
        }
        internal static void Parse(IEnumerable<LexicalTree_Assign> enumerable, SkillData skill)
        {
            foreach (var assign in enumerable)
            {
                string content = assign.Content[0].Content;
                int i32 = (int)assign.Content[0].Number;
                byte b8 = (byte)assign.Content[0].Number;
                switch (assign.Name)
                {
                    case "name":
                        skill.DisplayName = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "icon":
                        skill.Icon.Clear();
                        skill.IconAlpha.Clear();
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add("icon");
                            break;
                        }
                        skill.FilledWithNull.Remove("icon");
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            if (i + 1 < assign.Content.Count && assign.Content[i + 1].Type == 2)
                            {
                                skill.Icon.Add(assign.Content[i].Content);
                                skill.IconAlpha.Add((byte)assign.Content[i + 1].Number);
                                ++i;
                            }
                            else
                            {
                                skill.Icon.Add(assign.Content[i].Content);
                                skill.IconAlpha.Add(255);
                            }
                        }
                        break;
                    case "fkey":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Fkey = null;
                            skill.FkeyNumber = null;
                            skill.FilledWithNull.Add("fkey");
                            continue;
                        }
                        skill.FilledWithNull.Remove("fkey");
                        skill.Fkey = assign.Content[0].ToLowerString();
                        if (assign.Content.Count == 2)
                        { skill.FkeyNumber = (int)assign.Content[1].Number; }
                        else { skill.FkeyNumber = 0; }
                        break;
                    case "sortkey":
                        skill.SortKey = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "special":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Special = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                            skill.Special = i32;
                        else if (assign.Content[0].ToLowerString() == "on")
                            skill.Special = 1;
                        else skill.Special = 0;
                        break;
                    case "gun_delay":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.GunDelay = null;
                            skill.GunDelayName = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.GunDelay = i32;
                            skill.GunDelayName = skill.Name;
                        }
                        else
                        {
                            skill.GunDelayName = content.ToLower();
                            skill.GunDelay = (int)assign.Content[1].Number;
                        }
                        break;
                    case "quickreload":
                        skill.QuickReload = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "help":
                        skill.Help = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "hide_help":
                        skill.HideHelp = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "sound":
                        InsertStringList(assign, skill.FilledWithNull, skill.Sound);
                        break;
                    case "msg":
                        skill.Message = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "cutin":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.CutinAlpha = null;
                            skill.CutinFlashR = null;
                            skill.CutinFlashG = null;
                            skill.CutinFlashB = null;
                            skill.CutinInflate = null;
                            skill.CutinOn = null;
                            skill.CutinPhantom = null;
                            skill.CutinSlide = null;
                            skill.CutinStop = null;
                            skill.CutinTop = null;
                            skill.CutinTrans = null;
                            skill.CutinType = null;
                            skill.CutinWakeTime = null;
                            skill.CutinY = null;
                            skill.CutinY2 = null;
                            skill.CutinZoom = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i < assign.Content.Count; i += 2)
                        {
                            switch (assign.Content[i].ToLowerString())
                            {
                                case "type":
                                    skill.CutinType = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "top":
                                    skill.CutinTop = (int)assign.Content[i + 1].Number;
                                    break;
                                case "y":
                                    skill.CutinY = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "y2":
                                    skill.CutinY2 = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "stop":
                                    skill.CutinStop = true;
                                    --i;
                                    break;
                                case "wake_time":
                                    skill.CutinWakeTime = (int)assign.Content[i + 1].Number;
                                    break;
                                case "flash":
                                    var n = (int)assign.Content[i + 1].Number;
                                    var r = n / 100;
                                    var g = (n - 100 * r) / 10;
                                    var b = n - 100 * r - 10 * g;
                                    switch (r)
                                    {
                                        case 0:
                                            skill.CutinFlashR = 0;
                                            break;
                                        case 1:
                                            skill.CutinFlashR = 28;
                                            break;
                                        case 2:
                                            skill.CutinFlashR = 56;
                                            break;
                                        case 3:
                                            skill.CutinFlashR = 85;
                                            break;
                                        case 4:
                                            skill.CutinFlashR = 113;
                                            break;
                                        case 5:
                                            skill.CutinFlashR = 141;
                                            break;
                                        case 6:
                                            skill.CutinFlashR = 170;
                                            break;
                                        case 7:
                                            skill.CutinFlashR = 198;
                                            break;
                                        case 8:
                                            skill.CutinFlashR = 227;
                                            break;
                                        case 9:
                                            skill.CutinFlashR = 255;
                                            break;
                                    }
                                    switch (g)
                                    {
                                        case 0:
                                            skill.CutinFlashG = 0;
                                            break;
                                        case 1:
                                            skill.CutinFlashG = 28;
                                            break;
                                        case 2:
                                            skill.CutinFlashG = 56;
                                            break;
                                        case 3:
                                            skill.CutinFlashG = 85;
                                            break;
                                        case 4:
                                            skill.CutinFlashG = 113;
                                            break;
                                        case 5:
                                            skill.CutinFlashG = 141;
                                            break;
                                        case 6:
                                            skill.CutinFlashG = 170;
                                            break;
                                        case 7:
                                            skill.CutinFlashG = 198;
                                            break;
                                        case 8:
                                            skill.CutinFlashG = 227;
                                            break;
                                        case 9:
                                            skill.CutinFlashG = 255;
                                            break;
                                    }
                                    switch (b)
                                    {
                                        case 0:
                                            skill.CutinFlashB = 0;
                                            break;
                                        case 1:
                                            skill.CutinFlashB = 28;
                                            break;
                                        case 2:
                                            skill.CutinFlashB = 56;
                                            break;
                                        case 3:
                                            skill.CutinFlashB = 85;
                                            break;
                                        case 4:
                                            skill.CutinFlashB = 113;
                                            break;
                                        case 5:
                                            skill.CutinFlashB = 141;
                                            break;
                                        case 6:
                                            skill.CutinFlashB = 170;
                                            break;
                                        case 7:
                                            skill.CutinFlashB = 198;
                                            break;
                                        case 8:
                                            skill.CutinFlashB = 227;
                                            break;
                                        case 9:
                                            skill.CutinFlashR = 255;
                                            break;
                                    }
                                    break;
                                case "phantom":
                                    skill.CutinPhantom = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "alpha":
                                    skill.CutinAlpha = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "zoom":
                                    skill.CutinZoom = (int)assign.Content[i + 1].Number;
                                    break;
                                case "inflate":
                                    skill.CutinInflate = (int)assign.Content[i + 1].Number;
                                    break;
                                case "slide":
                                    skill.CutinSlide = (int)assign.Content[i + 1].Number;
                                    break;
                                case "trans":
                                    skill.CutinTrans = (int)assign.Content[i + 1].Number;
                                    break;
                            }
                        }
                        break;
                    case "value":
                        skill.Value = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "talent":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Talent = null;
                            skill.TalentSkill = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (content.ToLower())
                        {
                            case "off":
                                skill.Talent = false;
                                skill.TalentSkill = null;
                                break;
                            case "on":
                                skill.Talent = true;
                                skill.TalentSkill = null;
                                break;
                            default:
                                skill.Talent = true;
                                skill.TalentSkill = content.ToLower();
                                break;
                        }
                        break;
                    case "exp_per":
                        skill.ExpPercentage = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "mp":
                        skill.MP = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "image":
                        skill.Image = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "w":
                        skill.Width = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "h":
                        skill.Height = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "a":
                        skill.Alpha = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "alpha_tip":
                        skill.AlphaTip = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "alpha_butt":
                        skill.AlphaButt = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "anime":
                        skill.Anime = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "anime_interval":
                        skill.AnimeInterval = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "center":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Center = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (content.ToLower())
                        {
                            case "on":
                                skill.Center = 1;
                                break;
                            case "end":
                                skill.Center = 2;
                                break;
                            default:
                                skill.Center = 0;
                                break;
                        }
                        break;
                    case "ground":
                        skill.Ground = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "d360":
                        skill.D360 = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "rotate":
                        skill.Rotate = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "d360_adj":
                        skill.D360Adjust = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "direct":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Direct = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (content.ToLower())
                        {
                            case "on":
                                skill.Direct = 1;
                                break;
                            case "roll":
                                skill.Direct = 2;
                                break;
                            default:
                                skill.Direct = 0;
                                break;
                        }
                        break;
                    case "resize_a":
                        skill.ResizeA = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "resize_h":
                        skill.ResizeH = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_interval":
                        skill.ResizeInterval = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_reverse":
                        skill.ResizeReverse = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_s":
                        skill.ResizeS = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_start":
                        skill.ResizeStart = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_w":
                        skill.ResizeW = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "force_fire":
                        skill.ForceFire = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "slow_per":
                        skill.SlowPercentage = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slow_time":
                        skill.SlowTime = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slide":
                        skill.Slide = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slide_delay":
                        skill.SlideDelay = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slide_speed":
                        skill.SlideSpeed = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slide_stamp":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.SlideStamp = null;
                            skill.SlideStampOn = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.SlideStampOn = true;
                            skill.Slide = i32;
                        }
                        else if (content.ToLower() == "on")
                        {
                            skill.SlideStampOn = true;
                        }
                        break;
                    case "wait_time":
                        skill.WaitTimeMin = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "wait_time2":
                        skill.WaitTimeMax = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "shake":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Shake = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                            skill.Shake = i32;
                        else if (content.ToLower() == "on")
                            skill.Shake = 30;
                        else skill.Shake = 0;
                        break;
                    case "ray":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.RayA = null;
                            skill.RayR = null;
                            skill.RayG = null;
                            skill.RayB = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content.Count == 4)
                        {
                            skill.RayA = b8;
                            skill.RayR = (byte)assign.Content[1].Number;
                            skill.RayG = (byte)assign.Content[2].Number;
                            skill.RayB = (byte)assign.Content[3].Number;
                        }
                        else
                        {
                            skill.RayA = 255;
                            skill.RayR = b8;
                            skill.RayG = (byte)assign.Content[1].Number;
                            skill.RayB = (byte)assign.Content[2].Number;
                        }
                        break;
                    case "force_ray":
                        skill.ForceRay = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "flash":
                        skill.Flash = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "flash_anime":
                        skill.FlashAnime = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "flash_image":
                        skill.FlashImage = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "collision":
                        skill.Collision = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "afterdeath":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.AfterDeath = null;
                            skill.AfterDeathType = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        skill.AfterDeath = content.ToLower();
                        if (assign.Content.Count == 2) skill.AfterDeathType = (byte)assign.Content[1].Number;
                        break;
                    case "afterhit":
                        skill.AfterHit = null;
                        skill.AfterHitType = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        skill.AfterHit = content.ToLower();
                        if (assign.Content.Count == 2) skill.AfterHitType = (byte)assign.Content[1].Number;
                        break;
                    case "yorozu":
                        skill.YorozuAttribute.Clear();
                        skill.YorozuRadius = null;
                        skill.YorozuThrowMax = null;
                        skill.YorozuTurn = null;
                        skill.TroopType = null;
                        skill.EffectHeight = null;
                        skill.EffectWidth = null;
                        skill.YorozuAttack = null;
                        skill.YorozuConf = null;
                        skill.YorozuDeath = null;
                        skill.YorozuDefense = null;
                        skill.YorozuDext = null;
                        skill.YorozuDrain = null;
                        skill.YorozuFear = null;
                        skill.YorozuHp = null;
                        skill.YorozuHprec = null;
                        skill.YorozuIll = null;
                        skill.YorozuMagdef = null;
                        skill.YorozuMagic = null;
                        skill.YorozuMagsuck = null;
                        skill.YorozuMove = null;
                        skill.YorozuMp = null;
                        skill.YorozuMprec = null;
                        skill.YorozuPara = null;
                        skill.YorozuPoi = null;
                        skill.YorozuRadius = null;
                        skill.YorozuSil = null;
                        skill.YorozuSpeed = null;
                        skill.YorozuStone = null;
                        skill.YorozuSuck = null;
                        skill.YorozuSummonMax = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            if (assign.Content[i].Type != 0) continue;
                            try
                            {
                                switch (assign.Content[i].Content.ToLower())
                                {
                                    case "type":
                                        skill.YorozuTurn = assign.Content[i + 1].Number == 2;
                                        break;
                                    case "radius":
                                        skill.YorozuRadius = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "thm":
                                        skill.YorozuThrowMax = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "w":
                                        skill.EffectWidth = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "h":
                                        skill.EffectHeight = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "troop":
                                        skill.TroopType = 0;
                                        break;
                                    case "troop2":
                                        skill.TroopType = 1;
                                        break;
                                    case "troop3":
                                        skill.TroopType = 2;
                                        break;
                                    case "hp":
                                        skill.YorozuHp = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "mp":
                                        skill.YorozuMp = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "attack":
                                        skill.YorozuAttack = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "defense":
                                        skill.YorozuDefense = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "magic":
                                        skill.YorozuMagic = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "magdef":
                                        skill.YorozuMagdef = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "speed":
                                        skill.YorozuSpeed = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "dext":
                                        skill.YorozuDext = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "move":
                                        skill.YorozuMove = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "hprec":
                                        skill.YorozuHprec = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "mprec":
                                        skill.YorozuMprec = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "summon_max":
                                        skill.YorozuSummonMax = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "poi":
                                        skill.YorozuPoi = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "para":
                                        skill.YorozuPara = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "ill":
                                        skill.YorozuIll = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "conf":
                                        skill.YorozuConf = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "sil":
                                        skill.YorozuSil = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "stone":
                                        skill.YorozuStone = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "fear":
                                        skill.YorozuFear = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "suck":
                                        skill.YorozuSuck = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "magsuck":
                                        skill.YorozuMagsuck = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "drain":
                                        skill.YorozuDrain = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "death":
                                        skill.YorozuDeath = (int)assign.Content[i + 1].Number;
                                        break;
                                    default:
                                        skill.YorozuAttribute[assign.Content[i].ToLowerString()] = (int)assign.Content[i + 1].Number;
                                        break;
                                }
                            }
                            catch
                            {
                                Console.Error.WriteLine(i + 1);
                                Console.Error.WriteLine(assign.File + '/' + (assign.Line + 1));
                            }
                        }
                        break;
                    case "str":
                        skill.Strength = StrType.None;
                        skill.StrPercent = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (content.ToLower())
                        {
                            case "attack":
                                skill.Strength = StrType.Attack;
                                break;
                            case "attack_dext":
                                skill.Strength = StrType.AttackDext;
                                break;
                            case "attack_magic":
                                skill.Strength = StrType.AttackMagic;
                                break;
                            case "fix":
                                skill.Strength = StrType.Fix;
                                break;
                            case "magic_dext":
                                skill.Strength = StrType.MagicDext;
                                break;
                            case "magic":
                                skill.Strength = StrType.Magic;
                                break;
                            case "none":
                                skill.Strength = StrType.None;
                                skill.StrPercent = null;
                                continue;
                            default: throw new ApplicationException();
                        }
                        if (assign.Content.Count >= 2)
                            skill.StrPercent = (int)assign.Content[1].Number;
                        break;
                    case "str_ratio":
                        skill.StrRatio = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "attr":
                        skill.Attribute = InsertString(assign, skill.FilledWithNull)?.ToLower();
                        break;
                    case "add":
                    case "add2":
                        InsertStringList(assign, skill.FilledWithNull, skill.Add);
                        break;
                    case "add_all":
                        skill.AddAll = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "add_per":
                        skill.AddPercentage = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "damage":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.DamageType = null;
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.DamageType = i32;
                            continue;
                        }
                        switch (content.ToLower())
                        {
                            case "off":
                                skill.DamageType = -2;
                                break;
                            case "pass":
                                skill.DamageType = -3;
                                break;
                            default: throw new ScriptLoadingException(assign);
                        }
                        break;
                    case "damage_range_adjust":
                        skill.DamageRangeAdjust = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "attack_us":
                        skill.AttackUs = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "allfunc":
                        skill.AllFunc = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "bom":
                        skill.Bom = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "homing":
                        skill.HomingNumber = null;
                        skill.HomingOn = null;
                        skill.HomingType = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.HomingOn = true;
                            skill.HomingNumber = i32;
                            skill.HomingType = 0;
                            continue;
                        }
                        switch (content.ToLower())
                        {
                            case "on":
                                skill.HomingType = 1;
                                skill.HomingOn = true;
                                break;
                            case "origin":
                                skill.HomingType = 2;
                                skill.HomingOn = true;
                                break;
                            case "fix":
                                skill.HomingType = 3;
                                skill.HomingOn = true;
                                break;
                            case "hold":
                                skill.HomingType = 4;
                                skill.HomingOn = true;
                                break;
                            case "off":
                                skill.HomingOn = false;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "forward":
                        skill.Forward = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "far":
                        skill.Far = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "hard":
                        skill.Hard = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "onehit":
                        skill.OneHit = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "hard2":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Hard2 = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.Hard2 = b8;
                            continue;
                        }
                        switch (content.ToLower())
                        {
                            case "on":
                                skill.Hard2 = 1;
                                break;
                            default:
                                skill.Hard2 = 0;
                                break;
                        }
                        break;
                    case "offset":
                        skill.Offset.Clear();
                        skill.OffsetOn = false;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (content.ToLower())
                        {
                            case "on":
                                skill.OffsetOn = true;
                                continue;
                            case "off":
                                continue;
                        }
                        skill.OffsetOn = true;
                        foreach (var item in assign.Content)
                            skill.Offset.Add(item.ToLowerString());
                        break;
                    case "offset_attr":
                        InsertStringList(assign, skill.FilledWithNull, skill.OffsetAttribute);
                        break;
                    case "knock":
                        skill.Knock = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "knock_speed":
                        skill.KnockSpeed = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "knock_power":
                        skill.KnockPower = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "range":
                        skill.Range = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "range_min":
                        skill.RangeMin = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "check":
                        skill.Check = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "speed":
                        skill.Speed = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "origin":
                        skill.Origin = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "random_space":
                        skill.RandomSpace = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "random_space_min":
                        skill.RandomSpaceMin = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "time":
                        skill.Time = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "height":
                        skill.Height_Charge_Arc = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "rush":
                        skill.Rush = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "rush_degree":
                        skill.RushDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "rush_interval":
                        skill.RushInterval = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "rush_random_degree":
                        skill.RushRandomDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "follow":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FollowOn = null;
                            skill.Follow = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.FollowOn = true;
                            skill.Follow = i32;
                        }
                        else if (content.ToLower() == "on")
                        {
                            skill.FollowOn = true;
                            skill.Follow = null;
                        }
                        else
                        {
                            skill.FollowOn = false;
                            skill.Follow = null;
                        }
                        break;
                    case "start_degree":
                        skill.StartDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "start_random_degree":
                        skill.StartRandomDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "start_degree_type":
                        skill.StartDegreeType = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "start_degree_turnunit":
                        skill.StartDegreeTurnUnit = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "start_degree_fix":
                        skill.StartDegreeFix = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "homing2":
                        skill.Homing2 = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "drop_degree":
                        skill.DropDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "drop_degree2":
                        skill.DropDegreeMax = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "joint_skill":
                        skill.JointSkill = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "send_target":
                        skill.SendTarget = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "send_image_degree":
                        skill.SendImageDegree = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "next":
                        skill.Next = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "next2":
                        InsertStringList(assign, skill.FilledWithNull, skill.Next2);
                        break;
                    case "next3":
                        InsertStringList(assign, skill.FilledWithNull, skill.Next3);
                        break;
                    case "next4":
                        skill.Next4 = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "next_order":
                        skill.NextOrder = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "next_last":
                        skill.NextLast = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "next_first":
                        skill.NextFirst = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "just_next":
                        InsertStringList(assign, skill.FilledWithNull, skill.JustNext);
                        break;
                    case "pair_next":
                        InsertStringList(assign, skill.FilledWithNull, skill.PairNext);
                        break;
                    case "item_type":
                        skill.ItemType = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "item_sort":
                        skill.ItemSort = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "item_nosell":
                        skill.ItemNoSell = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "price":
                        skill.Price = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "friend":
                        InsertStringList(assign, skill.FilledWithNull, skill.Friend);
                        break;
                    case "movetype":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.MoveType = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (content.ToLower())
                        {
                            case "arc":
                                skill.MoveType = 1;
                                break;
                            case "drop":
                                skill.MoveType = 3;
                                break;
                            case "throw":
                                skill.MoveType = 2;
                                break;
                            case "circle":
                                skill.MoveType = 4;
                                break;
                            case "swing":
                                skill.MoveType = 5;
                                break;
                            case "missile":
                                skill.MoveType = 6;
                                break;
                            case "normal":
                                skill.MoveType = 7;
                                break;
                            case "none":
                                skill.MoveType = 0;
                                break;
                            default: throw new ScriptLoadingException(assign);
                        }
                        break;
                    case "type":
                        skill.StatusType = InsertByte(assign, skill.FilledWithNull);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, skill.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(List<LexicalTree> children, StoryData story)
        {
            foreach (var tree in children)
            {
                var assign = tree as LexicalTree_Assign;
                if (assign == null)
                {
                    story.Script.Add(tree);
                    continue;
                }
                switch (assign.Name)
                {
                    case "friend":
                        InsertStringList(assign, story.FilledWithNull, story.Friend);
                        break;
                    case "fight":
                        story.Fight = InsertBool(assign, story.FilledWithNull);
                        break;
                    case "politics":
                        story.Politics = InsertBool(assign, story.FilledWithNull);
                        break;
                    case "pre":
                        story.Pre = InsertBool(assign, story.FilledWithNull);
                        break;
                    default: throw new ApplicationException();
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree_Assign> enumerable, FieldData field)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "type":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            field.Type = FieldData.ChipType.None;
                            continue;
                        }
                        switch (assign.Content[0].Content.ToLower())
                        {
                            case "coll":
                                field.Type = FieldData.ChipType.coll;
                                break;
                            case "wall":
                                field.Type = FieldData.ChipType.wall;
                                break;
                            case "wall2":
                                field.Type = FieldData.ChipType.wall2;
                                break;
                        }
                        break;
                    case "attr":
                        field.Attribute = InsertString(assign, field.FilledWithNull);
                        break;
                    case "color":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            field.ColorB = null;
                            field.ColorG = null;
                            field.ColorR = null;
                            field.FilledWithNull.Add("color");
                            continue;
                        }
                        field.FilledWithNull.Remove("color");
                        field.ColorR = (byte)assign.Content[0].Number;
                        field.ColorG = (byte)assign.Content[1].Number;
                        field.ColorB = (byte)assign.Content[2].Number;
                        break;
                    case "id":
                        field.BaseId = InsertString(assign, field.FilledWithNull);
                        break;
                    case "edge":
                        field.IsEdge = InsertBool(assign, field.FilledWithNull);
                        break;
                    case "joint":
                        field.JointChip.Clear();
                        field.JointImage.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            field.FilledWithNull.Add("joint");
                            continue;
                        }
                        field.FilledWithNull.Remove("joint");
                        if (assign.Content.Count == 1)
                        {
                            field.JointChip.Add(assign.Content[0].ToLowerString());
                        }
                        else
                        {
                            for (int i = 0; i < assign.Content.Count; i += 2)
                            {
                                field.JointChip.Add(assign.Content[i].ToLowerString());
                                field.JointImage.Add(assign.Content[i + 1].ToLowerString());
                            }
                        }
                        break;
                    case "image":
                        field.ImageNameRandomList.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            field.FilledWithNull.Add("image");
                            continue;
                        }
                        field.FilledWithNull.Remove("image");
                        field.ImageNameRandomList.Add(assign.Content[0].Content.ToLower());
                        break;
                    case "add2":
                        AddStringIntPair(assign, field.FilledWithNull, field.ImageNameRandomList);
                        break;
                    case "member":
                        AddStringIntPair(assign, field.FilledWithNull, field.MemberRandomList);
                        break;
                    case "alt":
                        field.Alt_min = InsertInt(assign, field.FilledWithNull);
                        break;
                    case "alt_max":
                        field.Alt_max = InsertInt(assign, field.FilledWithNull);
                        break;
                    case "smooth":
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "@":
                            case "on":
                                field.SmoothType = FieldData.Smooth.on;
                                break;
                            case "off":
                                field.SmoothType = FieldData.Smooth.off;
                                break;
                            case "step":
                                field.SmoothType = FieldData.Smooth.step;
                                break;
                        }
                        break;
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree_Assign> enumerable, ObjectData object1)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "land_base":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            object1.FilledWithNull.Add("land_base");
                            object1.IsLandBase = null;
                            object1.LandBase = null;
                            continue;
                        }
                        object1.FilledWithNull.Add("land_base");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                object1.IsLandBase = true;
                                object1.LandBase = 0;
                                break;
                            case "off":
                                object1.IsLandBase = false;
                                break;
                            default:
                                object1.IsLandBase = false;
                                object1.LandBase = (int)assign.Content[0].Number;
                                break;
                        }
                        break;
                    case "type":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            object1.Type = ObjectData.ChipType.None;
                            continue;
                        }
                        switch (assign.Content[0].Content.ToLower())
                        {
                            case "coll":
                                object1.Type = ObjectData.ChipType.coll;
                                break;
                            case "wall":
                                object1.Type = ObjectData.ChipType.wall;
                                break;
                            case "wall2":
                                object1.Type = ObjectData.ChipType.wall2;
                                break;
                            case "break":
                                object1.Type = ObjectData.ChipType.breakable;
                                break;
                            case "gate":
                            case "break2":
                                object1.Type = ObjectData.ChipType.gate;
                                break;
                            case "floor":
                                object1.Type = ObjectData.ChipType.floor;
                                break;
                            case "start":
                                object1.Type = ObjectData.ChipType.start;
                                break;
                            case "goal":
                                object1.Type = ObjectData.ChipType.goal;
                                break;
                            case "box":
                                object1.Type = ObjectData.ChipType.box;
                                break;
                            case "cover":
                                object1.Type = ObjectData.ChipType.cover;
                                break;
                            default: throw new ScriptLoadingException(assign);
                        }
                        break;
                    case "no_stop":
                        object1.NoStop = InsertBool(assign, object1.FilledWithNull);
                        break;
                    case "no_wall2":
                        object1.NoWall2 = InsertByte(assign, object1.FilledWithNull);
                        break;
                    case "no_arc_hit":
                        object1.NoArcHit = InsertBool(assign, object1.FilledWithNull);
                        break;
                    case "radius":
                        object1.Radius = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "blk":
                        object1.Blk = InsertBool(assign, object1.FilledWithNull);
                        break;
                    case "w":
                        object1.Width = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "h":
                        object1.Height = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "a":
                        object1.Alpha = InsertByte(assign, object1.FilledWithNull);
                        break;
                    case "image":
                        object1.ImageNameRandomList.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            object1.FilledWithNull.Add("image");
                            continue;
                        }
                        object1.FilledWithNull.Remove("image");
                        object1.ImageNameRandomList.Add(assign.Content[0].ToLowerString());
                        break;
                    case "member":
                        AddStringIntPair(assign, object1.FilledWithNull, object1.ImageNameRandomList);
                        break;
                    case "image2":
                        object1.Image2Name = InsertString(assign, object1.FilledWithNull);
                        break;
                    case "image2_w":
                        object1.Image2Width = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "image2_h":
                        object1.Image2Height = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "image2_a":
                        object1.Image2Alpha = InsertByte(assign, object1.FilledWithNull);
                        break;
                    case "ground":
                        object1.IsGound = InsertBool(assign, object1.FilledWithNull);
                        break;
                    default:
                        break;
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree_Assign> enumerable, PowerData power)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {

                    case "name":
                        power.DisplayName = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master":
                        power.Master = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master2":
                        power.Master2 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master3":
                        power.Master3 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master4":
                        power.Master4 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master5":
                        power.Master5 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master6":
                        power.Master6 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head":
                        power.Head = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head2":
                        power.Head2 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head3":
                        power.Head3 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head4":
                        power.Head4 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head5":
                        power.Head5 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head6":
                        power.Head6 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "member":
                        InsertStringList(assign, power.FilledWithNull, power.MemberSpot);
                        break;
                    case "friend":
                        InsertStringList(assign, power.FilledWithNull, power.Friend);
                        break;
                    case "flag":
                        power.FlagPath = InsertString(assign, power.FilledWithNull);
                        break;
                    case "event":
                        power.IsEvent = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "bgm":
                        power.BGM = InsertString(assign, power.FilledWithNull);
                        break;
                    case "volume":
                        power.Volume = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "diplomacy":
                        power.DoDiplomacy = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "enable_select":
                        power.IsSelectable = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "enable_talent":
                        power.IsPlayableAsTalent = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "free_raise":
                        power.IsRaisable = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "money":
                        power.Money = InsertInt(assign, power.FilledWithNull);
                        break;
                    case "home":
                        InsertStringList(assign, power.FilledWithNull, power.HomeSpot);
                        break;
                    case "fix":
                        power.FilledWithNull.Remove("fix");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "@":
                                power.FilledWithNull.Add("fix");
                                power.Fix = null;
                                break;
                            case "on":
                                power.Fix = PowerData.FixType.on;
                                break;
                            case "off":
                                power.Fix = PowerData.FixType.off;
                                break;
                            case "home":
                                power.Fix = PowerData.FixType.home;
                                break;
                            case "warlike":
                                power.Fix = PowerData.FixType.warlike;
                                break;
                            case "freeze":
                                power.Fix = PowerData.FixType.freeze;
                                break;
                            case "hold":
                                power.Fix = PowerData.FixType.hold;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "diplo":
                        InsertStringIntPair(assign, power.FilledWithNull, power.Diplo);
                        break;
                    case "league":
                        InsertStringIntPair(assign, power.FilledWithNull, power.League);
                        break;
                    case "enemy":
                        InsertStringIntPair(assign, power.FilledWithNull, power.EnemyPower);
                        break;
                    case "staff":
                        InsertStringList(assign, power.FilledWithNull, power.Staff);
                        break;
                    case "merce":
                        AddStringIntPair(assign, power.FilledWithNull, power.Merce);
                        break;
                    case "training_average":
                        power.TrainingAveragePercent = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "training_up":
                        power.TrainingUp = InsertInt(assign, power.FilledWithNull);
                        break;
                    case "base_merits":
                        power.BaseMerit = InsertInt(assign, power.FilledWithNull);
                        break;
                    case "merits":
                        InsertStringIntPair(assign, power.FilledWithNull, power.Merits);
                        break;
                    case "loyals":
                        InsertStringIntPair(assign, power.FilledWithNull, power.Loyals);
                        break;
                    case "base_loyal":
                        power.BaseLoyal = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "diff":
                        power.Difficulty = InsertString(assign, power.FilledWithNull);
                        break;
                    case "yabo":
                        power.Yabo = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "kosen":
                        power.Kosen = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "text":
                        if (assign.Content.Count == 0)
                        {
                            power.Description = "";
                            power.FilledWithNull.Remove("text");
                            break;
                        }
                        power.Description = InsertString(assign, power.FilledWithNull);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, power.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree_Assign> enumerable, RaceData race)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        race.DisplayName = InsertString(assign, race.FilledWithNull);
                        break;
                    case "align":
                        race.Align = InsertByte(assign, race.FilledWithNull);
                        break;
                    case "brave":
                        race.Brave = InsertByte(assign, race.FilledWithNull);
                        break;
                    case "movetype":
                        race.MoveType = InsertString(assign, race.FilledWithNull);
                        break;
                    case "consti":
                        InsertStringBytePair(assign, race.FilledWithNull, race.Consti);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, race.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree_Assign> enumerable, SkillSetData skillset)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        skillset.DisplayName = InsertString(assign, skillset.FilledWithNull);
                        break;
                    case "member":
                        InsertStringList(assign, skillset.FilledWithNull, skillset.MemberSkill);
                        break;
                    case "back":
                        skillset.BackIconPath = InsertString(assign, skillset.FilledWithNull);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, skillset.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(GenericUnitData genericunit)
        {
            if (!genericunit.VariantData.ContainsKey("")) return;
            var removeList = new List<string>();
            foreach (var keyVal in genericunit.VariantData[""])
            {
                switch (keyVal.Key)
                {
                    case "fkey":
                        genericunit.BaseClassKey = InsertString(keyVal.Value, genericunit.FilledWithNull);
                        break;
                    case "unique":
                        genericunit.IsUnique = InsertBool(keyVal.Value, genericunit.FilledWithNull);
                        break;
                    case "change":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            genericunit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        genericunit.FilledWithNull.Remove("change");
                        if (keyVal.Value.Content.Count != 2 || keyVal.Value.Content[1].Type != 2)
                            throw new ApplicationException();
                        genericunit.Change = new ValueTuple<string, int>(keyVal.Value.Content[0].ToLowerString(), (int)keyVal.Value.Content[1].Number);
                        break;
                }
            }
            foreach (var item in removeList)
                genericunit.VariantData[""].Remove(item);
        }
        internal static void Parse(UnitData unit)
        {
            if (!unit.VariantData.ContainsKey("")) return;
            var removeList = new List<string>();
            foreach (var keyVal in unit.VariantData[""])
            {
                switch (keyVal.Key)
                {
                    case "talent":
                        unit.IsTalent = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "bgm":
                        unit.BGM = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "volume":
                        unit.Volume = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture":
                        unit.Picture = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_detail":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("picture_detail");
                            unit.PictureDetail = null;
                            break;
                        }
                        unit.FilledWithNull.Remove("picture_detail");
                        switch (keyVal.Value.Content[0].ToLowerString())
                        {
                            case "off":
                                unit.PictureDetail = 0;
                                break;
                            case "on":
                                unit.PictureDetail = 1;
                                break;
                            case "on1":
                                unit.PictureDetail = 2;
                                break;
                            case "on2":
                                unit.PictureDetail = 3;
                                break;
                            case "on3":
                                unit.PictureDetail = 4;
                                break;
                            default: throw new Exception();
                        }
                        break;
                    case "picture_menu":
                        unit.PictureMenu = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_floor":
                        unit.PictureFloor = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_shift":
                        unit.PictureShift = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_shift_up":
                        unit.PictureShiftUp = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_center":
                        unit.PictureCenter = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_back":
                        unit.PictureBack = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "alive_per":
                        unit.AlivePercentage = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "medical":
                        unit.Medical = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "leader_skill":
                        removeList.Add(keyVal.Key);
                        unit.LeaderSkill.Clear();
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("leader_skill");
                            break;
                        }
                        unit.FilledWithNull.Remove("leader_skill");
                        int tmplevel = 0;
                        List<string> list = null;
                        for (int i = 0; i < keyVal.Value.Content.Count; i++)
                        {
                            if (keyVal.Value.Content[i].Type == 2) { tmplevel = (int)keyVal.Value.Content[i].Number; }
                            else if (unit.LeaderSkill.TryGetValue(tmplevel, out list)) { list.Add(keyVal.Value.Content[i].ToLowerString()); }
                            else { unit.LeaderSkill[tmplevel] = new List<string>() { keyVal.Value.Content[i].ToLowerString() }; }
                        }
                        break;
                    case "assist_skill":
                        removeList.Add(keyVal.Key);
                        unit.AssistSkill.Clear();
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("assist_skill");
                            break;
                        }
                        unit.FilledWithNull.Remove("assist_skill");
                        tmplevel = 0;
                        list = null;
                        for (int i = 0; i < keyVal.Value.Content.Count; i++)
                        {
                            if (keyVal.Value.Content[i].Type == 2) { tmplevel = (int)keyVal.Value.Content[i].Number; }
                            else if (unit.AssistSkill.TryGetValue(tmplevel, out list)) { list.Add(keyVal.Value.Content[i].ToLowerString()); }
                            else { unit.AssistSkill[tmplevel] = new List<string>() { keyVal.Value.Content[i].ToLowerString() }; }
                        }
                        break;
                    case "yabo":
                        unit.Yabo = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "kosen":
                        unit.Kosen = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "align":
                        unit.Align = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "enemy":
                        InsertStringList(keyVal.Value, unit.FilledWithNull, unit.Enemy); removeList.Add(keyVal.Key);
                        break;
                    case "loyal":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.Loyal = null;
                            unit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        unit.FilledWithNull.Remove("loyal");
                        if (keyVal.Value.Content.Count == 2)
                            unit.Loyal = new ValueTuple<string, byte>(keyVal.Value.Content[0].ToLowerString(), (byte)keyVal.Value.Content[0].Number);
                        else
                            unit.Loyal = new ValueTuple<string, byte>(keyVal.Value.Content[0].ToLowerString(), 0);
                        break;
                    case "power_name":
                        unit.PowerDisplayName = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "flag":
                        unit.Flag = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "staff":
                        InsertStringList(keyVal.Value, unit.FilledWithNull, unit.Staff); removeList.Add(keyVal.Key);
                        break;
                    case "diplomacy":
                        unit.Diplomacy = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "castle_guard":
                        removeList.Add(keyVal.Key);
                        unit.CastleGuard.Clear();
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("castle_guard");
                            break;
                        }
                        unit.FilledWithNull.Remove("castle_guard");
                        for (int i = 0; (i << 1) < keyVal.Value.Content.Count; i++)
                            unit.CastleGuard[keyVal.Value.Content[i << 1].ToLowerString()] = (int)keyVal.Value.Content[(i << 1) + 1].Number;
                        break;
                    case "actor":
                        unit.IsActor = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "enable_select":
                        unit.IsEnableSelect = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "enable":
                        unit.EnableTurn = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "enable_max":
                        unit.EnableTurnMax = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "fix":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.Fix = null;
                            unit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        unit.FilledWithNull.Remove("fix");
                        switch (keyVal.Value.Content[0].ToLowerString())
                        {
                            case "on":
                                unit.Fix = 1;
                                break;
                            case "off":
                                unit.Fix = 0;
                                break;
                            case "home":
                                unit.Fix = 2;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "home":
                        InsertStringList(keyVal.Value, unit.FilledWithNull, unit.Home); removeList.Add(keyVal.Key);
                        break;
                    case "no_escape":
                        unit.IsNoEscape = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "noremove_unit":
                        unit.IsNoRemoveUnit = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "noemploy_unit":
                        unit.IsNoEmployUnit = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "noitem_unit":
                        unit.IsNoItemUnit = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "arbeit":
                        removeList.Add(keyVal.Key);
                        unit.FilledWithNull.Remove("arbeit");
                        switch (keyVal.Value.Content.Count)
                        {
                            case 1:
                                if (byte.TryParse(keyVal.Value.Content[0].Content, out var arbeitPercentage))
                                {
                                    unit.ArbeitPercentage = arbeitPercentage;
                                    unit.ArbeitType = 0;
                                }
                                else if (keyVal.Value.Content[0].Symbol1 == '@')
                                {
                                    unit.ArbeitPercentage = null;
                                    unit.ArbeitType = null;
                                }
                                else
                                {
                                    unit.ArbeitPercentage = 100;
                                    switch (keyVal.Value.Content[0].ToLowerString())
                                    {
                                        case "on":
                                            unit.ArbeitType = 0;
                                            break;
                                        case "power":
                                            unit.ArbeitType = 1;
                                            break;
                                        case "fix":
                                            unit.ArbeitType = 2;
                                            break;
                                        default: throw new ApplicationException();
                                    }
                                }
                                break;
                            case 2:
                                switch (keyVal.Value.Content[0].ToLowerString())
                                {
                                    case "on":
                                        unit.ArbeitType = 0;
                                        break;
                                    case "power":
                                        unit.ArbeitType = 1;
                                        break;
                                    case "fix":
                                        unit.ArbeitType = 2;
                                        break;
                                    default: throw new ApplicationException();
                                }
                                unit.ArbeitPercentage = (byte)(keyVal.Value.Content[1].Number);
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "arbeit_capacity":
                        unit.ArbeitCapacity = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "help":
                        unit.Help = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "join":
                        unit.Join = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "dead":
                        unit.Dead = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "retreat":
                        unit.Retreat = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "break":
                        unit.Break = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "voice_type":
                        unit.VoiceType = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "red":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        switch (keyVal.Value.Content[0].ToLowerString())
                        {
                            case "off":
                                unit.Team = 0;
                                break;
                            case "on":
                                unit.Team = 1;
                                break;
                            default:
                                if (keyVal.Value.Content[0].Type != 2)
                                    throw new ApplicationException();
                                unit.Team = (byte)keyVal.Value.Content[0].Number;
                                break;
                        }
                        break;
                    case "keep_form":
                        unit.KeepForm = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "breast_width":
                        unit.BreastWidth = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "active":
                        if (keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.ActiveType = null;
                            unit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        unit.FilledWithNull.Remove("active");
                        switch (keyVal.Value.Content[0].ToLowerString())
                        {
                            case "never":
                                unit.ActiveType = 0;
                                break;
                            case "rect":
                                unit.ActiveType = 2;
                                break;
                            case "range":
                                unit.ActiveType = 4;
                                break;
                            case "time":
                                unit.ActiveType = 6;
                                break;
                            case "troop":
                                unit.ActiveType = 8;
                                break;
                            case "never2":
                                unit.ActiveType = 1;
                                break;
                            case "rect2":
                                unit.ActiveType = 3;
                                break;
                            case "range2":
                                unit.ActiveType = 5;
                                break;
                            case "time2":
                                unit.ActiveType = 7;
                                break;
                            case "troop2":
                                unit.ActiveType = 9;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "activenum":
                        unit.FilledWithNull.Remove("activenum");
                        unit.ActiveRange = null;
                        unit.ActiveRect = null;
                        unit.ActiveTime = null;
                        unit.ActiveTroop = null;
                        switch (keyVal.Value.Content.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                int parse = (int)keyVal.Value.Content[0].Number;
                                if (keyVal.Value.Content[0].Symbol1 == '@')
                                {
                                    unit.FilledWithNull.Add(keyVal.Key);
                                    break;
                                }
                                else if (keyVal.Value.Content[0].Type == 2)
                                    if (!unit.ActiveType.HasValue)
                                    {
                                        unit.ActiveTime = parse;
                                        unit.ActiveRange = parse;
                                    }
                                    else if (unit.ActiveType <= 5)
                                        unit.ActiveRange = parse;
                                    else unit.ActiveTime = parse;
                                else
                                    unit.ActiveTroop = keyVal.Value.Content[0].ToLowerString();
                                break;
                            case 4:
                                unit.ActiveRect = new ValueTuple<int, int, int, int>((int)keyVal.Value.Content[0].Number, (int)keyVal.Value.Content[1].Number, (int)keyVal.Value.Content[2].Number, (int)keyVal.Value.Content[3].Number);
                                break;
                            default: throw new ApplicationException();
                        }
                        break;

                }
            }
            foreach (var item in removeList)
                unit.VariantData[""].Remove(item);
        }
        static void CommonParse<T>(IEnumerable<LexicalTree_Assign> enumerable, T unit) where T : CommonUnitData
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {

                    case "name":
                        unit.DisplayName = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "sex":
                        unit.FilledWithNull.Remove("sex");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "male":
                                unit.Sex = 1;
                                break;
                            case "female":
                                unit.Sex = 2;
                                break;
                            case "@":
                                unit.Sex = null;
                                unit.FilledWithNull.Add(assign.Name);
                                break;
                            default:
                                unit.Sex = 0;
                                break;
                        }
                        break;
                    case "race":
                        unit.Race = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "class":
                        unit.Class = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "radius":
                        unit.Radius = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "radius_press":
                        unit.RadiusPress = InsertByte(assign, unit.FilledWithNull);
                        break;
                    case "w":
                        unit.Width = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "h":
                        unit.Height = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "a":
                        unit.Alpha = InsertByte(assign, unit.FilledWithNull);
                        break;
                    case "image":
                        unit.Image = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "image2":
                        unit.Image2 = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "tkool":
                        unit.IsTkool = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "face":
                        unit.Face = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "price":
                        unit.Price = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "cost":
                        unit.Cost = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hasexp":
                        unit.HasEXP = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "friend":
                        unit.IsFriendAllRace = null;
                        unit.IsFriendAllClass = null;
                        InsertStringList(assign, unit.FilledWithNull, unit.Friends);
                        if (unit.Friends.Contains("allrace"))
                            unit.IsFriendAllRace = true;
                        if (unit.Friends.Contains("allclass"))
                            unit.IsFriendAllClass = true;
                        break;
                    case "friend_ex":
                        unit.FriendEx1.Clear();
                        unit.FriendEx2.Clear();
                        unit.FriendExCount = null;
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("friend_ex");
                        for (int splitIndex = 0; splitIndex < assign.Content.Count; ++splitIndex)
                        {
                            if (unit.FriendExCount == null)
                                if (assign.Content[splitIndex].Type == 2)
                                {
                                    unit.FriendExCount = (int)assign.Content[splitIndex].Number;
                                }
                                else { unit.FriendEx1.Add(assign.Content[splitIndex].ToLowerString()); }
                            else { unit.FriendEx2.Add(assign.Content[splitIndex].ToLowerString()); }
                        }
                        break;
                    case "merce":
                        InsertStringList(assign, unit.FilledWithNull, unit.Merce);
                        break;
                    case "same_friend":
                        unit.IsSameFriend = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "same_call":
                        unit.IsSameCall = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "samecall_baseup":
                        unit.IsSameCallBaseUp = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "same_sex":
                        unit.IsSameSex = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "member":
                        AddStringIntPair(assign, unit.FilledWithNull, unit.Member);
                        break;
                    case "level":
                        unit.Level = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hp":
                        unit.Hp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mp":
                        unit.Mp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "attack":
                        unit.Attack = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "defense":
                        unit.Defense = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magic":
                        unit.Magic = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magdef":
                        unit.Magdef = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dext":
                        unit.Dext = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "speed":
                        unit.Speed = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "move":
                        unit.Move = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hprec":
                        unit.HpRec = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mprec":
                        unit.MpRec = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "summon_max":
                        unit.summon_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "summon_level":
                        unit.summon_level = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "heal_max":
                        unit.heal_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "attack_max":
                        unit.attack_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "defense_max":
                        unit.defense_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magic_max":
                        unit.magic_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magdef_max":
                        unit.magdef_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dext_max":
                        unit.dext_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "speed_max":
                        unit.speed_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "move_max":
                        unit.move_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hprec_max":
                        unit.hprec_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mprec_max":
                        unit.mprec_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "skill":
                        InsertStringList(assign, unit.FilledWithNull, unit.Skill);
                        break;
                    case "skill2":
                        InsertStringList(assign, unit.FilledWithNull, unit.Skill2);
                        break;
                    case "delskill":
                        InsertStringList(assign, unit.FilledWithNull, unit.DeleteSkill);
                        break;
                    case "learn":
                        unit.Learn.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("learn");
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            var tmplevel = (int)assign.Content[i].Number;
                            if (assign.Content[i].Type == 2)
                                unit.Learn[(int)assign.Content[i].Number] = new List<string>();
                            else if (!unit.Learn.ContainsKey(tmplevel))
                                unit.Learn[tmplevel] = new List<string>() { assign.Content[i].ToLowerString() };
                            else
                                unit.Learn[tmplevel].Add(assign.Content[i].ToLowerString());
                        }
                        break;
                    case "delskill2":
                        InsertStringList(assign, unit.FilledWithNull, unit.DeleteSkill2);
                        break;
                    case "consti":
                        InsertStringIntPair(assign, unit.FilledWithNull, unit.Consti);
                        break;
                    case "movetype":
                        unit.MoveType = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "line":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.DefenseLine = null;
                            unit.FilledWithNull.Add("line");
                            break;
                        }
                        unit.FilledWithNull.Remove("line");
                        byte line = (byte)assign.Content[0].Number;
                        if (assign.Content[0].Type == 1)
                            unit.DefenseLine = line;
                        else if (assign.Content[0].ToLowerString() == "front")
                            unit.DefenseLine = 10;
                        else unit.DefenseLine = 0;
                        break;
                    case "satellite":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.IsSatellite = null;
                            unit.Satellite = null;
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("satellite");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                unit.IsSatellite = true;
                                break;
                            case "off":
                                unit.IsSatellite = false;
                                break;
                            default:
                                if (assign.Content[0].Type == 2)
                                {
                                    unit.IsSatellite = true;
                                    unit.Satellite = (int)assign.Content[0].Number;
                                }
                                break;
                        }
                        break;
                    case "beast_unit":
                        unit.IsBeast = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "no_knock":
                        if (assign.Content[0].ToLowerString() == "on")
                            unit.NoKnock = 1;
                        else unit.NoKnock = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "no_cover":
                        unit.IsNoCover = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "view_unit":
                        unit.IsViewUnit = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "element_lost":
                        unit.IsElementLost = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "attack_range":
                        unit.AttackRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "escape_range":
                        unit.EscapeRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hand_range":
                        unit.HandRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "wake_range":
                        unit.WakeRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "view_range":
                        unit.ViewRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "cavalary_range":
                        unit.CavalryRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "escape_run":
                        unit.EscapeRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "multi":
                        unit.Multi.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("multi");
                            break;
                        }
                        unit.FilledWithNull.Remove("multi");
                        int multiLevel = 0;
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            if (assign.Content[i].Type != 2)
                                multiLevel = (int)assign.Content[i].Number;
                            else unit.Multi[assign.Content[i].ToLowerString()] = multiLevel;
                        }
                        break;
                    case "exp":
                        unit.exp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "exp_mul":
                        unit.exp_mul = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "level_max":
                        unit.level_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "exp_max":
                        unit.exp_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hpup":
                        unit.hpUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mpup":
                        unit.mpUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "attackup":
                        unit.attackUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "defenseup":
                        unit.defenseUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magicup":
                        unit.magicUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magdefup":
                        unit.magdefUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dextup":
                        unit.dextUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "speedup":
                        unit.speedUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "moveup":
                        unit.moveUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hprecup":
                        unit.hprecUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mprecup":
                        unit.mprecUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hpmax":
                        unit.hpMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mpmax":
                        unit.mpMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "attackmax":
                        unit.attackMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "defensemax":
                        unit.defenseMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magicmax":
                        unit.magicMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magdefmax":
                        unit.magdefMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dextmax":
                        unit.dextMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "speedmax":
                        unit.speedMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "movemax":
                        unit.moveMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hprecmax":
                        unit.hprecMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mprecmax":
                        unit.mprecMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "brave":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.IsBrave = null;
                            unit.Brave = null;
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("brave");
                        if (assign.Content.Count == 2)
                        {
                            if (assign.Content[0].ToLowerString() != "on") throw new ApplicationException();
                            unit.IsBrave = true;
                            unit.Brave = (byte)assign.Content[1].Number;
                            break;
                        }
                        unit.Brave = (byte)assign.Content[0].Number;
                        break;
                    case "handle":
                        unit.IsHandle = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "sortkey":
                        unit.SortKey = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "item":
                        InsertStringList(assign, unit.FilledWithNull, unit.Item);
                        break;
                    case "no_training":
                        unit.IsNoTraining = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "scream":
                        unit.Scream = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "lost_corpse":
                        unit.LostCorpse = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dead_event":
                        unit.DeadEvent = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "free_move":
                        unit.FreeMove = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "add_vassal":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.AddVassal = null;
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("add_vassal");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "off":
                                unit.AddVassal = 0;
                                break;
                            case "on":
                                unit.AddVassal = 1;
                                break;
                            case "roam":
                                unit.AddVassal = 2;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "politics":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.Politics = null;
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("politics");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                unit.Politics = 1;
                                break;
                            case "fix":
                                unit.Politics = 2;
                                break;
                            case "erase":
                                unit.Politics = 3;
                                break;
                            case "unique":
                                unit.Politics = 4;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    default:
                        ScenarioVariantRoutine(assign, unit.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree> enumerable, EventData eventData)
        {
            foreach (var item in enumerable)
            {
                if (item.Type != LexicalTree.TreeType.Assign)
                {
                    eventData.Script.Add(item);
                    continue;
                }
                var assign = item as LexicalTree_Assign;
                switch (assign.Name)
                {
                    case "bg":
                    case "bcg":
                        assign.Name = "bg";
                        eventData.BackGround = InsertString(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "bgm":
                        eventData.BGM = InsertString(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "w":
                        eventData.Width = InsertInt(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "h":
                        eventData.Height = InsertInt(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "map":
                        eventData.Map = InsertString(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "handle":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            eventData.FilledWithNull.Add("handle");
                            eventData.Handle = null;
                            break;
                        }
                        eventData.FilledWithNull.Remove("handle");
                        if (assign.Content[0].Type == 2)
                            eventData.Handle = (byte)assign.Content[0].Number;
                        else if (assign.Content[0].ToLowerString() == "red")
                            eventData.Handle = 1;
                        else eventData.Handle = 0;
                        break;
                    case "disperse":
                        eventData.Disperse = InsertBool(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "castle_battle":
                        eventData.CastleBattle = InsertBool(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "castle":
                        eventData.Castle = InsertInt(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "title":
                        eventData.Title = InsertString(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "limit":
                        eventData.Limit = InsertInt(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "volume":
                        eventData.Volume = InsertByte(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "blind":
                        eventData.Blind = null;
                        eventData.IsBlind = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            eventData.FilledWithNull.Add("blind");
                        }
                        eventData.FilledWithNull.Remove("blind");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                eventData.IsBlind = true;
                                break;
                            case "off":
                                eventData.IsBlind = false;
                                break;
                            default:
                                eventData.IsBlind = true;
                                eventData.Blind = (byte)assign.Content[0].Number;
                                break;
                        }
                        break;
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree_Assign> enumerable, SpotData spot)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        spot.DisplayName = InsertString(assign, spot.FilledWithNull);
                        break;
                    case "image":
                        spot.Image = InsertString(assign, spot.FilledWithNull);
                        break;
                    case "x":
                        spot.X = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "y":
                        spot.Y = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "w":
                        spot.Width = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "h":
                        spot.Height = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "map":
                        spot.Map = InsertString(assign, spot.FilledWithNull);
                        break;
                    case "castle_battle":
                        spot.IsCastleBattle = InsertBool(assign, spot.FilledWithNull);
                        break;
                    case "yorozu":
                        InsertStringList(assign, spot.FilledWithNull, spot.Yorozu);
                        break;
                    case "limit":
                        spot.Limit = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "bgm":
                        spot.BGM = InsertString(assign, spot.FilledWithNull);
                        break;
                    case "volume":
                        spot.Volume = InsertByte(assign, spot.FilledWithNull);
                        break;
                    case "gain":
                        spot.Gain = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "castle":
                        spot.Castle = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "capacity":
                        spot.Capacity = InsertByte(assign, spot.FilledWithNull);
                        break;
                    case "monster":
                        AddStringIntPair(assign, spot.FilledWithNull, spot.Monsters);
                        break;
                    case "merce":
                        InsertStringList(assign, spot.FilledWithNull, spot.Merce);
                        break;
                    case "member":
                        AddStringIntPair(assign, spot.FilledWithNull, spot.Members);
                        break;
                    case "dungeon":
                        spot.Dungeon = InsertString(assign, spot.FilledWithNull);
                        break;
                    case "no_home":
                        spot.IsNotHome = InsertBool(assign, spot.FilledWithNull);
                        break;
                    case "no_raise":
                        spot.IsNotRaisableSpot = InsertBool(assign, spot.FilledWithNull);
                        break;
                    case "castle_lot":
                        spot.CastleLot = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "politics":
                        spot.Politics = InsertBool(assign, spot.FilledWithNull);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, spot.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree> enumerable, ScenarioData scenario)
        {
            int? locate_x = null;
            int? locate_y = null;
            foreach (var item in enumerable)
            {
                var assign = item as LexicalTree_Assign;
                if (assign == null)
                {
                    scenario.Script.Add(item);
                    continue;
                }
                switch (assign.Name)
                {
                    case "name":
                        scenario.DisplayName = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "map":
                        scenario.WorldMapPath = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "locate_x":
                        locate_x = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "locate_y":
                        locate_y = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "text":
                        scenario.DescriptionText = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "begin_text":
                        scenario.BeginText = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "power":
                        InsertStringList(assign, scenario.FilledWithNull, scenario.Power);
                        break;
                    case "spot":
                        InsertStringList(assign, scenario.FilledWithNull, scenario.Spot);
                        break;
                    case "roam":
                        InsertStringList(assign, scenario.FilledWithNull, scenario.Roamer);
                        break;
                    case "world":
                        scenario.WorldEvent = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "fight":
                        scenario.FightEvent = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "politics":
                        scenario.PoliticsEvent = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "war_capcity":
                        scenario.WarCapacity = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "spot_capacity":
                        scenario.SpotCapacity = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "gain_per":
                        scenario.GainPercent = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "support_range":
                        scenario.SupportRange = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "my_range":
                        scenario.MyRange = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "myhelp_range":
                        scenario.MyHelpRange = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "base_level":
                        scenario.BaseLevelUp = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "monster_level":
                        scenario.MonsterLevel = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "training_up":
                        scenario.TrainingUp = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "actor_per":
                        scenario.ActorPercent = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "sortkey":
                        if (assign.Content[0].Symbol1 == '@') scenario.SortKey = 0;
                        else scenario.SortKey = (int)assign.Content[0].Number;
                        break;
                    case "default_ending":
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                scenario.IsDefaulEnding = true;
                                break;
                            case "@":
                                scenario.FilledWithNull.Add("default_ending");
                                scenario.IsDefaulEnding = null;
                                break;
                            case "off":
                                scenario.IsDefaulEnding = false;
                                break;
                        }
                        break;
                    case "power_order":
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "normal":
                                scenario.Order = ContextData.Order.Normal;
                                break;
                            case "dash":
                                scenario.Order = ContextData.Order.Dash;
                                break;
                            case "test":
                            case "dashtest":
                                scenario.Order = ContextData.Order.DashTest;
                                break;
                            default:
                                throw new ApplicationException();
                        }
                        break;
                    case "enable":
                        scenario.IsEnable = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "enable_talent":
                        scenario.IsPlayableAsTalent = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "party":
                        scenario.IsAbleToMerce = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "no_autosave":
                        scenario.IsNoAutoSave = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "offset":
                        foreach (var remove in assign.Content)
                            for (int i = 0; i < scenario.MenuButtonPower.Length; ++i)
                            {
                                if (scenario.MenuButtonPower[i] == remove.Content)
                                    scenario.MenuButtonPower[i] = "";
                                if (scenario.MenuButtonTalent[i] == remove.Content)
                                    scenario.MenuButtonTalent[i] = "";
                            }
                        break;
                    case "zone":
                        scenario.ZoneName = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "nozone":
                        scenario.IsNoZone = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "poli":
                        scenario.PoliticsData.Clear();
                        if (assign.Content[0].Symbol1 == '@' || assign.Content[0].ToLowerString() == "none")
                        {
                            scenario.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        scenario.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i * 3 < assign.Content.Count; i++)
                        {
                            var con1 = assign.Content[i * 3].ToLowerString();
                            var con2 = assign.Content[i * 3 + 1].ToLowerString();
                            var con3 = assign.Content[i * 3 + 2].ToLowerString();
                            scenario.PoliticsData[con1] = new ValueTuple<string, string>(con2 == "@" ? "" : con2, con3 == "@" ? "" : con3);
                        }
                        break;
                    case "camp":
                        scenario.CampingData.Clear();
                        if (assign.Content[0].Symbol1 == '@' || assign.Content[0].ToLowerString() == "none")
                        {
                            scenario.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        scenario.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i * 3 < assign.Content.Count; i++)
                        {
                            var con1 = assign.Content[i * 3].Content;
                            var con2 = assign.Content[i * 3 + 1].Content;
                            var con3 = assign.Content[i * 3 + 2].Content;
                            scenario.CampingData[con1] = new ValueTuple<string, string>(con2 == "@" ? "" : con2, con3 == "@" ? "" : con3);
                        }
                        break;
                    case "item":
                        for (int i = 0; i < scenario.ItemWindowTab.Length; i++)
                        {
                            scenario.ItemWindowTab[i] = (null, 0);
                        }
                        if (assign.Content[0].Symbol1 == '@' || assign.Content[0].ToLowerString() == "none")
                        {
                            scenario.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        scenario.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i < assign.Content.Count; i++)
                            scenario.ItemWindowTab[i] = (assign.Content[i].ToLowerString(), 1);
                        break;
                    case "item0":
                    case "item1":
                    case "item2":
                    case "item3":
                    case "item4":
                    case "item5":
                    case "item6":
                        var itemIndex = int.Parse(assign.Name.Last().ToString());
                        var itemTabName = scenario.ItemWindowTab[itemIndex].Item1;
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                scenario.ItemWindowTab[itemIndex] = (itemTabName, int.MaxValue);
                                break;
                            case "@":
                            case "off":
                                scenario.ItemWindowTab[itemIndex] = (itemTabName, 0);
                                break;
                            default:
                                scenario.ItemWindowTab[itemIndex] = (itemTabName, (int)assign.Content[0].Number);
                                break;
                        }
                        break;
                    case "item_sale":
                        InsertStringList(assign, scenario.FilledWithNull, scenario.ItemSale);
                        break;
                    case "item_limit":
                        scenario.IsItemLimit = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "item_hold":
                        InsertStringList(assign, scenario.FilledWithNull, scenario.PlayerInitialItem);
                        break;
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree_Assign> enumerable, VoiceData voice)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "voice_type":
                        InsertStringList(assign, voice.FilledWithNull, voice.VoiceType);
                        break;
                    case "delskill":
                        InsertStringList(assign, voice.FilledWithNull, voice.DeleteVoiceType);
                        break;
                    case "power":
                        InsertStringList(assign, voice.FilledWithNull, voice.PowerVoice);
                        if (voice.PowerVoice.Count != 0)
                        {
                            voice.NoPower = false;
                        }
                        break;
                    case "spot":
                        InsertStringList(assign, voice.FilledWithNull, voice.SpotVoice);
                        break;
                }
            }
        }
        internal static void Parse(IEnumerable<LexicalTree_Assign> enumerable, DungeonData answer)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        answer.DisplayName = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "prefix":
                        answer.Prefix = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "suffix":
                        answer.Suffix = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "bgm":
                        answer.BGM = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "map":
                        answer.MapFileName = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "floor":
                        answer.Floor = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "wall":
                        answer.Wall = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "start":
                        answer.Start = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "goal":
                        answer.Goal = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "box":
                        answer.Box = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "open":
                        answer.IsOpened = InsertBool(assign, answer.FilledWithNull);
                        break;
                    case "item_text":
                        answer.ItemText = InsertBool(assign, answer.FilledWithNull);
                        break;
                    case "volume":
                        answer.Volume = InsertByte(assign, answer.FilledWithNull);
                        break;
                    case "blind":
                        answer.Blind = InsertByte(assign, answer.FilledWithNull);
                        break;
                    case "max":
                        answer.Max = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "move_speed":
                        answer.MoveSpeedRatio = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "lv_adjust":
                        answer.LevelAdjust = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "limit":
                        answer.Limit = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "base_level":
                        answer.BaseLevel = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "monster_num":
                        answer.MonsterNumber = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "item_num":
                        answer.ItemNumber = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "color":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.ColorDirection = null;
                            answer.ForeColorA = null;
                            answer.ForeColorB = null;
                            answer.ForeColorG = null;
                            answer.ForeColorR = null;
                            answer.BackColorA = null;
                            answer.BackColorB = null;
                            answer.BackColorG = null;
                            answer.BackColorR = null;
                            answer.Dense = null;
                            answer.FilledWithNull.Add("color");
                            break;
                        }
                        answer.FilledWithNull.Remove("color");
                        answer.ColorDirection = (byte)assign.Content[0].Number;
                        answer.ForeColorB = (byte)assign.Content[3].Number;
                        answer.ForeColorG = (byte)assign.Content[2].Number;
                        answer.ForeColorR = (byte)assign.Content[1].Number;
                        answer.BackColorB = (byte)assign.Content[6].Number;
                        answer.BackColorG = (byte)assign.Content[5].Number;
                        answer.BackColorR = (byte)assign.Content[4].Number;
                        answer.ForeColorA = (byte)assign.Content[7].Number;
                        answer.BackColorA = (byte)assign.Content[8].Number;
                        answer.Dense = (byte)assign.Content[9].Number;
                        break;
                    case "ray":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.Ray = null;
                            answer.Way1 = null;
                            answer.Way2 = null;
                            answer.Way3 = null;
                            answer.FilledWithNull.Add("ray");
                            break;
                        }
                        answer.FilledWithNull.Remove("ray");
                        answer.Ray = (byte)assign.Content[0].Number;
                        answer.Way1 = (byte)assign.Content[1].Number;
                        answer.Way2 = (byte)assign.Content[2].Number;
                        answer.Way3 = (byte)assign.Content[3].Number;
                        break;
                    case "home":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.Width = null;
                            answer.Height = null;
                            answer.HallwayWidth = null;
                            answer.Seed = null;
                            answer.FilledWithNull.Add("home");
                            break;
                        }
                        answer.FilledWithNull.Remove("home");
                        if (assign.Content.Count < 3 || assign.Content.Count > 4) throw new ApplicationException();
                        answer.Width = (int)assign.Content[0].Number;
                        answer.Height = (int)assign.Content[1].Number;
                        answer.HallwayWidth = (int)assign.Content[2].Number;
                        if (assign.Content.Count == 4)
                            answer.Seed = (long)assign.Content[3].Number;
                        break;
                    case "monster":
                        answer.Monsters.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.FilledWithNull.Add("monster");
                            break;
                        }
                        answer.FilledWithNull.Remove("monster");
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            if (i + 1 < assign.Content.Count && int.TryParse(assign.Content[i + 1].Content, out var count))
                                answer.Monsters[assign.Content[i].ToLowerString()] = count;
                            else if (answer.Monsters.TryGetValue(assign.Content[i].ToLowerString(), out count))
                                answer.Monsters[assign.Content[i].ToLowerString()] = count + 1;
                            else answer.Monsters[assign.Content[i].ToLowerString()] = 1;
                        }
                        break;
                    case "item":
                        answer.Item.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.FilledWithNull.Add("item");
                            break;
                        }
                        answer.FilledWithNull.Remove("item");
                        answer.Item.AddRange(assign.Content.Select(_ => _.ToLowerString()));
                        break;
                    default:
                        var split = assign.Name.Split(new char[1] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        if (split.Length == 1)
                            answer.VariantData[int.Parse(split[1])][split[0]] = assign.Content;
                        else
                            answer.VariantData[0][split[0]] = assign.Content;
                        break;
                }
            }
        }
        static void ContextParse(IEnumerable<LexicalTree_Assign> enumerable)
        {
            foreach (var assign in enumerable)
            {
                byte firstByte = (byte)assign.Content[0].Number;
                int firstInt = (int)assign.Content[0].Number;
                string firstString = assign.Content[0].ToString();
                string firstIdentifier = assign.Content[0].ToLowerString();
                bool onoff = firstString?.ToLower() == "on";
                if (assign.Content[0].Symbol1 == '@') continue;
                switch (assign.Name)
                {
                    case "title_name":
                        Context.TitleName = firstString;
                        break;
                    case "title_menu_right":
                        Context.TitleMenuRight = firstInt;
                        break;
                    case "title_menu_top":
                        Context.TitleMenuTop = firstInt;
                        break;
                    case "title_menu_space":
                        Context.TitleMenuSpace = firstInt;
                        break;
                    case "event_bg_size":
                        Context.event_bg_size = firstInt;
                        break;
                    case "member":
                        Context.Member = firstByte;
                        break;
                    case "movement_num":
                        if (assign.Content[0].Symbol1 == '@') continue;
                        Context.MovementNumber = firstByte;
                        break;
                    case "employ_range":
                        Context.EmployRange = firstByte;
                        break;
                    case "battle_fast":
                        Context.BattleFast = onoff;
                        break;
                    case "support_range":
                        Context.SupportRange = firstByte;
                        break;
                    case "my_range":
                        Context.Myrange = firstByte;
                        break;
                    case "myhelp_range":
                        Context.MyHelpRange = firstByte;
                        break;
                    case "mode_easy":
                        foreach (var token in assign.Content)
                        {
                            switch (token.ToLowerString())
                            {
                                case "samecall":
                                    Context.SameCall_easy = true;
                                    break;
                                case "isblind":
                                    Context.IsBlind_easy = true;
                                    break;
                                case "dead":
                                    Context.Dead_easy = true;
                                    break;
                                case "dead2":
                                    Context.Dead2_easy = true;
                                    break;
                                case "escape":
                                    Context.Escape_easy = true;
                                    break;
                                case "auto":
                                    Context.Auto_easy = true;
                                    break;
                                case "sdown":
                                    Context.SDown_easy = true;
                                    break;
                                case "steal":
                                    Context.Steal_easy = true;
                                    break;
                                case "hostil":
                                    Context.Hostil_easy = true;
                                    break;
                                case "cover_fire":
                                    Context.CoverFire_easy = true;
                                    break;
                                case "force_fire":
                                    Context.ForceFire_easy = true;
                                    break;
                                case "nozone":
                                    Context.NoZone_easy = true;
                                    break;
                            }
                        }
                        break;
                    case "mode_normal":
                        foreach (var token in assign.Content)
                        {
                            switch (token.ToLowerString())
                            {
                                case "samecall":
                                    Context.SameCall_normal = true;
                                    break;
                                case "isblind":
                                    Context.IsBlind_normal = true;
                                    break;
                                case "dead":
                                    Context.Dead_normal = true;
                                    break;
                                case "dead2":
                                    Context.Dead2_normal = true;
                                    break;
                                case "escape":
                                    Context.Escape_normal = true;
                                    break;
                                case "auto":
                                    Context.Auto_normal = true;
                                    break;
                                case "sdown":
                                    Context.SDown_normal = true;
                                    break;
                                case "steal":
                                    Context.Steal_normal = true;
                                    break;
                                case "hostil":
                                    Context.Hostil_normal = true;
                                    break;
                                case "cover_fire":
                                    Context.CoverFire_normal = true;
                                    break;
                                case "force_fire":
                                    Context.ForceFire_normal = true;
                                    break;
                                case "nozone":
                                    Context.NoZone_normal = true;
                                    break;
                            }
                        }
                        break;
                    case "mode_hard":
                        foreach (var token in assign.Content)
                        {
                            switch (token.ToLowerString())
                            {
                                case "samecall":
                                    Context.SameCall_hard = true;
                                    break;
                                case "isblind":
                                    Context.IsBlind_hard = true;
                                    break;
                                case "dead":
                                    Context.Dead_hard = true;
                                    break;
                                case "dead2":
                                    Context.Dead2_hard = true;
                                    break;
                                case "escape":
                                    Context.Escape_hard = true;
                                    break;
                                case "auto":
                                    Context.Auto_hard = true;
                                    break;
                                case "sdown":
                                    Context.SDown_hard = true;
                                    break;
                                case "steal":
                                    Context.Steal_hard = true;
                                    break;
                                case "hostil":
                                    Context.Hostil_hard = true;
                                    break;
                                case "cover_fire":
                                    Context.CoverFire_hard = true;
                                    break;
                                case "force_fire":
                                    Context.ForceFire_hard = true;
                                    break;
                                case "nozone":
                                    Context.NoZone_hard = true;
                                    break;
                            }
                        }
                        break;
                    case "mode_luna":
                        foreach (var token in assign.Content)
                        {
                            switch (token.ToLowerString())
                            {
                                case "samecall":
                                    Context.SameCall_luna = true;
                                    break;
                                case "isblind":
                                    Context.IsBlind_luna = true;
                                    break;
                                case "dead":
                                    Context.Dead_luna = true;
                                    break;
                                case "dead2":
                                    Context.Dead2_luna = true;
                                    break;
                                case "escape":
                                    Context.Escape_luna = true;
                                    break;
                                case "auto":
                                    Context.Auto_luna = true;
                                    break;
                                case "sdown":
                                    Context.SDown_luna = true;
                                    break;
                                case "steal":
                                    Context.Steal_luna = true;
                                    break;
                                case "hostil":
                                    Context.Hostil_luna = true;
                                    break;
                                case "cover_fire":
                                    Context.CoverFire_luna = true;
                                    break;
                                case "force_fire":
                                    Context.ForceFire_luna = true;
                                    break;
                                case "nozone":
                                    Context.NoZone_luna = true;
                                    break;
                            }
                        }
                        break;
                    case "mode_easy_text":
                        Context.ModeText[0] = assign.Content[0].Content;
                        break;
                    case "mode_normal_text":
                        Context.ModeText[1] = assign.Content[0].Content;
                        break;
                    case "mode_hard_text":
                        Context.ModeText[2] = assign.Content[0].Content;
                        break;
                    case "mode_luna_text":
                        Context.ModeText[3] = assign.Content[0].Content;
                        break;
                    case "fv_hp_per":
                        Context.fv_hp_per = firstInt;
                        break;
                    case "fv_mp_per":
                        Context.fv_mp_per = firstInt;
                        break;
                    case "fv_attack_per":
                        Context.fv_attack_per = firstInt;
                        break;
                    case "fv_speed_per":
                        Context.fv_speed_per = firstInt;
                        break;
                    case "fv_move_per":
                        Context.fv_move_per = firstInt;
                        break;
                    case "fv_hprec_per":
                        Context.fv_hprec_per = firstInt;
                        break;
                    case "fv_consti_mul":
                        Context.fv_consti_mul = firstInt;
                        break;
                    case "fv_summon_mul":
                        Context.fv_summon_mul = firstInt;
                        break;
                    case "fv_level_per":
                        Context.fv_level_per = firstInt;
                        break;
                    case "power_order":
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "normal":
                                Context.PowerOrder = ContextData.Order.Normal;
                                break;
                            case "dash":
                                Context.PowerOrder = ContextData.Order.Dash;
                                break;
                            case "test":
                            case "dashtest":
                                Context.PowerOrder = ContextData.Order.DashTest;
                                break;
                            case "normaltest":
                                Context.PowerOrder = ContextData.Order.NormalTest;
                                break;
                        }
                        break;
                    case "talent_mode":
                        Context.TalentMode = onoff;
                        break;
                    case "npm_play":
                        Context.NonPlayerMode = onoff;
                        break;
                    case "default_ending":
                        Context.DefaultEnding = onoff;
                        break;
                    case "picture_floor":
                        if (assign.Content[0].ToLowerString() == "bottom")
                            Context.PictureFloorMsg = false;
                        break;
                    case "race_suffix":
                        Context.RaceSuffix = assign.Content[0].Content;
                        break;
                    case "race_label":
                        Context.RaceLabel = firstString;
                        break;
                    case "sound_volume":
                        Context.SoundVolume = firstInt;
                        break;
                    case "bgm_volume":
                        Context.BGMVolume = firstByte;
                        break;
                    case "gain_per":
                        Context.GainPer = firstInt;
                        break;
                    case "spot_capacity":
                        Context.SpotCapacity = firstByte;
                        break;
                    case "war_capacity":
                        Context.WarCapacity = firstByte;
                        break;
                    case "max_unit":
                        Context.MaxUnit = firstInt;
                        break;
                    case "dead_penalty":
                        Context.DeadPenalty = firstByte;
                        break;
                    case "win_gain":
                        Context.WinGain = firstInt;
                        break;
                    case "training_average":
                        Context.TrainingAverage = firstInt;
                        break;
                    case "leave_period":
                        Context.LeavePeriod = firstByte;
                        break;
                    case "merits_bonus":
                        Context.MeritsBonus = firstInt;
                        break;
                    case "caution_adjust":
                        Context.CautionAdjust = firstByte;
                        break;
                    case "notalent_down":
                        Context.NoTalentAbility = firstInt;
                        break;
                    case "unit_castle_forcefire":
                        Context.unit_castle_forcefire = onoff;
                        break;
                    case "unit_attack_range":
                        Context.unit_attack_range = firstInt;
                        break;
                    case "unit_battle_cram":
                        Context.unit_battle_cram = onoff;
                        break;
                    case "unit_catle_cram":
                        Context.unit_castle_cram = onoff;
                        break;
                    case "unit_drain_mul":
                        Context.unit_drain_mul = firstInt;
                        break;
                    case "unit_element_heal":
                        Context.unit_element_heal = onoff;
                        break;
                    case "unit_escape_range":
                        Context.unit_escape_range = firstInt;
                        break;
                    case "unit_escape_run":
                        Context.unit_escape_run = firstInt;
                        break;
                    case "unit_hand_range":
                        Context.unit_hand_range = firstInt;
                        break;
                    case "unit_keep_form":
                        Context.unit_keep_form = onoff;
                        break;
                    case "unit_level_max":
                        Context.unit_level_max = firstInt;
                        break;
                    case "unit_poison_per":
                        Context.unit_poison_per = firstInt;
                        break;
                    case "unit_retreat_damage":
                        Context.unit_retreat_damage = firstInt;
                        break;
                    case "unit_retreat_speed":
                        Context.unit_retreat_speed = firstInt;
                        break;
                    case "unit_slow_per":
                        Context.unit_slow_per = firstInt;
                        break;
                    case "unit_status_death":
                        Context.unit_status_death = firstInt;
                        break;
                    case "unit_summon_level":
                        Context.unit_summon_level = firstInt;
                        break;
                    case "unit_summon_min":
                        Context.unit_summon_min = firstInt;
                        break;
                    case "unit_view_range":
                        Context.unit_view_range = firstInt;
                        break;
                    case "btl_auto":
                        Context.btl_auto = firstInt;
                        break;
                    case "btl_blind_alpha":
                        Context.btl_blind_alpha = firstByte;
                        break;
                    case "btl_breast_width":
                        Context.btl_breast_width = firstInt;
                        break;
                    case "btl_limit":
                        Context.btl_limit = firstInt;
                        break;
                    case "btl_limit_c":
                        Context.btl_limit_c = firstInt;
                        break;
                    case "btl_lineshift":
                        Context.btl_lineshift = firstInt;
                        break;
                    case "btl_min_damage":
                        Context.btl_min_damage = firstInt;
                        break;
                    case "btl_msgtime":
                        Context.btl_msgtime = firstInt;
                        break;
                    case "btl_nocastle_bdr":
                        Context.btl_nocastle_bdr = firstInt;
                        break;
                    case "btl_prepare_max":
                        Context.btl_prepare_max = firstInt;
                        break;
                    case "btl_prepare_min":
                        Context.btl_prepare_min = firstInt;
                        break;
                    case "btl_replace_line":
                        Context.btl_replace_line = firstByte;
                        break;
                    case "btl_unitmax":
                        Context.btl_unitmax = firstInt;
                        break;
                    case "btl_wingmax":
                        Context.btl_wingmax = firstInt;
                        break;
                    case "neutral_max":
                        Context.neutral_max = firstByte;
                        break;
                    case "neutral_member_max":
                        Context.neutral_member_max = firstByte;
                        break;
                    case "neutral_member_min":
                        Context.neutral_member_min = firstByte;
                        break;
                    case "neutral_min":
                        Context.neutral_min = firstByte;
                        break;
                    case "skillicon_leader":
                        Context.SkillIcon[0] = assign.Content[0].Content;
                        break;
                    case "skillicon_all":
                        Context.SkillIcon[1] = assign.Content[0].Content;
                        break;
                    case "skillicon_add":
                        Context.SkillIcon[2] = assign.Content[0].Content;
                        break;
                    case "skillicon_special":
                        Context.SkillIcon[3] = assign.Content[0].Content;
                        break;
                    case "item_sell_per":
                        Context.ItemSellPercentage = firstInt;
                        break;
                    case "executive_bdr":
                        Context.ExecutiveBorder = firstByte;
                        break;
                    case "senior_bdr":
                        Context.SeniorBorder = firstByte;
                        break;
                    case "employ_price_coe":
                        Context.EmployPrice = firstInt;
                        break;
                    case "vassal_price_coe":
                        Context.VassalPrice = firstInt;
                        break;
                    case "support_gain":
                        Context.support_gain = firstInt;
                        break;
                    case "compati_vassal_bdr":
                        Context.compati_vassal_bdr = firstByte;
                        break;
                    case "compati_bad":
                        Context.compati_bad = firstByte;
                        break;
                    case "loyal_down":
                        Context.loyal_down = firstByte;
                        break;
                    case "loyal_up":
                        Context.loyal_up = firstByte;
                        break;
                    case "loyal_border":
                        Context.loyal_border = firstInt;
                        break;
                    case "loyal_escape_border":
                        Context.loyal_escape_border = firstInt;
                        break;
                    case "arbeit_gain":
                        Context.arbeit_gain = firstInt;
                        break;
                    case "arbeit_player":
                        Context.arbeit_player = onoff;
                        break;
                    case "arbeit_price_coe":
                        Context.arbeit_price_coe = firstInt;
                        break;
                    case "arbeit_turn":
                        Context.arbeit_turn = firstByte;
                        break;
                    case "arbeit_vassal_coe":
                        Context.arbeit_vassal_coe = firstInt;
                        break;
                    case "raid_border":
                        Context.RaidBorder = firstInt;
                        break;
                    case "raid_coe":
                        Context.RaidCoe = firstInt;
                        break;
                    case "raid_max":
                        Context.RaidMax = firstInt;
                        break;
                    case "raid_min":
                        Context.RaidMin = firstInt;
                        break;
                    case "btl_castle_lot":
                        Context.BattleCastleLot = firstInt;
                        break;
                    case "scenario_select2":
                        Context.ScenarioSelect2On = true;
                        Context.ScenarioSelect2Percentage = firstByte;
                        Context.ScenarioSelect2 = assign.Content[1].Content;
                        break;
                    case "unit_image_right":
                        Context.UnitImageRight = onoff;
                        break;
                    case "font_file":
                        foreach (var item in assign.Content)
                            Context.FontFiles.Add(item.Content);
                        break;
                    case "color_blue":
                        Context.ColorBlue[0] = (byte)assign.Content[0].Number;
                        Context.ColorBlue[1] = (byte)assign.Content[1].Number;
                        Context.ColorBlue[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_red":
                        Context.ColorRed[0] = (byte)assign.Content[0].Number;
                        Context.ColorRed[1] = (byte)assign.Content[1].Number;
                        Context.ColorRed[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_text":
                        Context.ColorText[0] = (byte)assign.Content[0].Number;
                        Context.ColorText[1] = (byte)assign.Content[1].Number;
                        Context.ColorText[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_name":
                        Context.ColorName[0] = (byte)assign.Content[0].Number;
                        Context.ColorName[1] = (byte)assign.Content[1].Number;
                        Context.ColorName[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_nameHelp":
                        Context.ColorNameHelp[0] = (byte)assign.Content[0].Number;
                        Context.ColorNameHelp[1] = (byte)assign.Content[1].Number;
                        Context.ColorNameHelp[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_white":
                        Context.ColorWhite[0] = (byte)assign.Content[0].Number;
                        Context.ColorWhite[1] = (byte)assign.Content[1].Number;
                        Context.ColorWhite[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_magenta":
                        Context.ColorMagenta[0] = (byte)assign.Content[0].Number;
                        Context.ColorMagenta[1] = (byte)assign.Content[1].Number;
                        Context.ColorMagenta[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_green":
                        Context.ColorGreen[0] = (byte)assign.Content[0].Number;
                        Context.ColorGreen[1] = (byte)assign.Content[1].Number;
                        Context.ColorGreen[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_cyan":
                        Context.ColorCyan[0] = (byte)assign.Content[0].Number;
                        Context.ColorCyan[1] = (byte)assign.Content[1].Number;
                        Context.ColorCyan[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_yellow":
                        Context.ColorYellow[0] = (byte)assign.Content[0].Number;
                        Context.ColorYellow[1] = (byte)assign.Content[1].Number;
                        Context.ColorYellow[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_orange":
                        Context.ColorOrange[0] = (byte)assign.Content[0].Number;
                        Context.ColorOrange[1] = (byte)assign.Content[1].Number;
                        Context.ColorOrange[2] = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_button":
                        Context.WindowBottom = (sbyte)assign.Content[0].Number;
                        break;
                    case "wnd_menu":
                        Context.Window_menu[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_menu[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_menu = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_heromenu":
                        Context.Window_heromenu[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_heromenu[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_heromenu = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_commenu":
                        Context.Window_commenu[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_commenu[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_commenu = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_spot":
                        Context.Window_spot[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_spot[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_spot = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_spot2":
                        Context.Window_spot2[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_spot2[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_spot2 = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_status":
                        Context.Window_status[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_status[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_status = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_merce":
                        Context.Window_merce[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_merce[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_merce = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_war":
                        Context.Window_war[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_war[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_war = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_diplo":
                        Context.Window_diplo[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_diplo[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_diplo = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_powerinfo":
                        Context.Window_powerinfo[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_powerinfo[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_powerinfo = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_personinfo":
                        Context.Window_personinfo[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_personinfo[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_personinfo = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_save":
                        Context.Window_save[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_save[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_save = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_load":
                        Context.Window_load[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_load[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_load = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_tool":
                        Context.Window_tool[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_tool[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_tool = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_battle":
                        Context.Window_battle[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_battle[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_battle = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_message":
                        Context.Window_message[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_message[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_message = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_name":
                        Context.Window_name[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_name[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_name = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_dialog":
                        Context.Window_dialog[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_dialog[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_dialog = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_help":
                        Context.Window_help[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_help[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_help = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_detail":
                        Context.Window_detail[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_detail[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_detail = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_corps":
                        Context.Window_corps[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_corps[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_corps = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_bstatus":
                        Context.Window_bstatus[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_bstatus[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_bstatus = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_powerselect":
                        Context.Window_powerselect[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_powerselect[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_powerselect = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_personselect":
                        Context.Window_personselect[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_personselect[1] = (sbyte)assign.Content[1].Number;
                        Context.Alpha_Window_personselect = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_scenario":
                        Context.Window_scenario[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_scenario[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_scenario = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_scenariotext":
                        Context.Window_scenariotext[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_scenariotext[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_scenariotext = (byte)assign.Content[2].Number;
                        break;
                    case "fullbody_detail":
                        Context.FullBodyDetail = onoff;
                        break;
                    default:
                        Context.VariantData[assign.Name] = assign.Content;
                        break;
                }
            }
        }
        static string InsertString(LexicalTree_Assign assign, List<string> fillWithNull)
        {
            if (assign.Content.Count == 0 || assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return null;
            }
            fillWithNull.Remove(assign.Name);
            return assign.Content[0].ToString();
        }
        static byte? InsertByte(LexicalTree_Assign assign, List<string> fillWithNull)
        {
            if (assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return null;
            }
            fillWithNull.Remove(assign.Name);
            return (byte)assign.Content[0].Number;
        }
        static int? InsertInt(LexicalTree_Assign assign, List<string> fillWithNull)
        {
            if (assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return null;
            }
            fillWithNull.Remove(assign.Name);
            return (int)assign.Content[0].Number;
        }
        static bool? InsertBool(LexicalTree_Assign assign, List<string> fillWithNull)
        {
            if (assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return null;
            }
            fillWithNull.Remove(assign.Name);
            switch (assign.Content[0].ToLowerString())
            {
                case "on":
                    return true;
                case "off":
                    return false;
                default:
                    throw new ScriptLoadingException(assign.Content[0]);
            }
        }
        static void InsertStringList(LexicalTree_Assign assign, List<string> fillWithNull, List<string> list)
        {
            list.Clear();
            if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return;
            }
            fillWithNull.Remove(assign.Name);
            foreach (var item in assign.Content)
            {
                list.Add(item.ToLowerString());
            }
            return;
        }
        static void InsertStringIntPair(LexicalTree_Assign assign, List<string> fillWithNull, Dictionary<string, int> dic)
        {
            dic.Clear();
            if (assign.Content.Count == 0 || assign.Content[0].Symbol1 == '@' || assign.Content[0].ToLowerString() == "none")
            {
                fillWithNull.Add(assign.Name);
                return;
            }
            fillWithNull.Remove(assign.Name);
            for (int i = 0; (i << 1) < assign.Content.Count; i++)
            {
                dic[assign.Content[i << 1].ToLowerString()] = (int)assign.Content[(i << 1) + 1].Number;
            }
            return;
        }
        static void InsertStringBytePair(LexicalTree_Assign assign, List<string> fillWithNull, Dictionary<string, byte> dic)
        {
            dic.Clear();
            if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return;
            }
            fillWithNull.Remove(assign.Name);
            for (int i = 0; (i << 1) < assign.Content.Count; i++)
            {
                dic[assign.Content[i << 1].ToLowerString()] = (byte)assign.Content[(i << 1) + 1].Number;
            }
            return;
        }
        static void AddStringIntPair(LexicalTree_Assign assign, List<string> filledWithNull, List<string> monsters)
        {
            monsters.Clear();
            if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
            {
                filledWithNull.Add(assign.Name);
                return;
            }
            filledWithNull.Remove(assign.Name);
            foreach (var item in assign.Content)
            {
                if (item.Type == 0)
                    monsters.Add(item.Content);
                else if (item.Type == 2)
                {
                    var lst = monsters.Last();
                    for (long i = 1; i < item.Number; ++i)
                        monsters.Add(lst);
                }
            }
        }
        static void ScenarioVariantRoutine(LexicalTree_Assign assign, Dictionary<string, Dictionary<string, LexicalTree_Assign>> VariantData)
        {
            if (assign.Name[assign.Name.Length - 1] == '@')
            {
                if (VariantData.ContainsKey(""))
                    VariantData[""][assign.Name.Substring(0, assign.Name.Length - 1)] = assign;
                else
                    VariantData[""] = new Dictionary<string, LexicalTree_Assign>() { { assign.Name.Substring(0, assign.Name.Length - 1), assign } };
                return;
            }
            var array = assign.Name.Split(new char[1] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            switch (array.Length)
            {
                case 1:
                    assign.Name = array[0].ToLower();
                    if (VariantData.ContainsKey(""))
                        VariantData[""][assign.Name] = assign;
                    else
                        VariantData[""] = new Dictionary<string, LexicalTree_Assign>() { { assign.Name, assign } };
                    break;
                case 2:
                    assign.Name = array[0].ToLower();
                    if (VariantData.ContainsKey(array[1].ToLower()))
                        VariantData[array[1].ToLower()][assign.Name] = assign;
                    else
                        VariantData[array[1]] = new Dictionary<string, LexicalTree_Assign>() { { assign.Name, assign } };
                    break;
                default: throw new ScriptLoadingException(assign);
            }
        }
        static IEnumerable<LexicalTree_Assign> SelectAssign(LexicalTree_Block input)
        {
            for (int i = 0; i < input.Children.Count; i++)
            {
                var assign = input.Children[i] as LexicalTree_Assign;
                if (assign == null) continue;
                yield return assign;
            }
        }
    }

    public sealed class ScriptLoadingException : ApplicationException
    {
        public ScriptLoadingException(LexicalTree_Assign assign) : base($"{assign.Name}@{assign.File}/{assign.Line + 1}:{assign.Column + 1}") { }
        public ScriptLoadingException(LexicalTree_Statement stmt) : base($"{stmt.Type}@{stmt.File}/{stmt.Line + 1}:{stmt.Column + 1}") { }
        public ScriptLoadingException(Token token) : base($"{token.ToString()}@{token.File}/{token.Line + 1}:{token.Column}") { }
    }
}