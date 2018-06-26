using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Wahren.Specific;

namespace Wahren
{
    public static partial class ScriptLoader
    {
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
            scenarios = new ScenarioData2[_scenarios.Length];
            var wait = new Task[_scenarios.Length];
            for (int i = 0; i < _scenarios.Length; i++)
            {
                scenarios[i] = new ScenarioData2(_scenarios[i].Value);
                wait[i] = scenarios[i].LoadingDone;
            }
            Task.WaitAll(wait);
        }

        public static Task[] LoadAllAsync()
        {
            if (Folder == null) throw new ApplicationException("Folder must not be null!");
            var ans = new Task[Folder.Script_Dat.Count];
            for (int i = 0; i < ans.Length; i++)
                ans[i] = Intern(Folder.Script_Dat[i]).LoadAsync(Folder.Encoding, Folder.IsEnglishMode, Folder.IsDebug);
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
                        var buf_detail = new StringBuilder();
                        foreach (var assign in SelectAssign(tree))
                        {
                            buf_detail.Clear();
                            var content = assign.Content;
                            for (int j = 0; j < content.Count; j++)
                                buf_detail.Append(content[j].ToString());
                            Detail.TryAdd(string.Intern(assign.Name), buf_detail.ToString());
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
                            switch (assign.Name.ToLower())
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
                                default: throw new ApplicationException(assign.DebugInfo);
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
                                if (eventData.Inherit == inheritEvent.Inherit)
                                    throw new CircularReferenceException(inheritEvent.DebugInfo + ':' + inheritEvent.Inherit);
                                eventData.Inherit = inheritEvent.Inherit;
                                Resolve(eventData, inheritEvent);
                            }
                            else break;
                        }
                        EventDictionary.TryAdd(eventData.Name, eventData);
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
                        StoryDictionary.TryAdd(story.Name, story);
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
                        DungeonDictionary.TryAdd(dungeon.Name, dungeon);
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
                        ScenarioDictionary.TryAdd(scenario.Name, scenario);
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
                        FieldDictionary.TryAdd(field.Name, field);
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
                        ObjectDictionary.TryAdd(object1.Name, object1);
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
                        PowerDictionary.TryAdd(power.Name, power);
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
                        RaceDictionary.TryAdd(race.Name, race);
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
                        SkillDictionary.TryAdd(skill.Name, skill);
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
                        SkillSetDictionary.TryAdd(skillset.Name, skillset);
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
                        SpotDictionary.TryAdd(spot.Name, spot);
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
                        VoiceDictionary.TryAdd(voice.Name, voice);
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
                        GenericUnitDictionary.TryAdd(genericunit.Name, genericunit);
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
                        UnitDictionary.TryAdd(unit.Name, unit);
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
                    foreach (var (key, value) in sc.Value)
                        if (!tmpDic.ContainsKey(key))
                            tmpDic[key] = value;
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
                    foreach (var (key, value) in skillData2.YorozuAttribute)
                        skillData1.YorozuAttribute.Add(key, value);
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
                foreach (var (key, value) in dungeonData2.Monsters)
                    dungeonData1.Monsters[key] = value;
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
                foreach (var (key, value) in raceData2.Consti)
                    raceData1.Consti[key] = value;
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
                foreach (var (key, value) in powerData2.Diplo)
                    powerData1.Diplo[key] = value;
            if (powerData1.EnemyPower.Count == 0 && powerData2.EnemyPower.Count != 0 && !powerData1.FilledWithNull.Contains("enemy"))
                foreach (var (key, value) in powerData2.EnemyPower)
                    powerData1.EnemyPower[key] = value;
            if (powerData1.League.Count == 0 && powerData2.League.Count != 0 && !powerData1.FilledWithNull.Contains("league"))
                foreach (var (key, value) in powerData2.League)
                    powerData1.League[key] = value;
            if (powerData1.Loyals.Count == 0 && powerData2.Loyals.Count != 0 && !powerData1.FilledWithNull.Contains("loyal"))
                foreach (var (key, value) in powerData2.Loyals)
                    powerData1.Loyals[key] = value;
            if (powerData1.Merits.Count == 0 && powerData2.Merits.Count != 0 && !powerData1.FilledWithNull.Contains("merit"))
                foreach (var (key, value) in powerData2.Merits)
                    powerData1.Merits[key] = value;
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
            if (unitData1.Class == null && unitData2.Class != null && !unitData1.FilledWithNull.Contains("class"))
                unitData1.Class = unitData2.Class;
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
                foreach (var (key, value) in unitData2.LeaderSkill)
                    unitData1.LeaderSkill[key] = value;
            if (unitData1.AssistSkill.Count == 0 && unitData2.AssistSkill.Count != 0 && !unitData1.FilledWithNull.Contains("assist_skill"))
                foreach (var (key, value) in unitData2.AssistSkill)
                    unitData1.AssistSkill[key] = value;
            if (unitData1.CastleGuard.Count == 0 && unitData2.CastleGuard.Count != 0 && !unitData1.FilledWithNull.Contains("castle_guard"))
                foreach (var (key, value) in unitData2.CastleGuard)
                    unitData1.CastleGuard[key] = value;
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
                foreach (var (key, value) in unitData2.Skill2)
                    unitData1.Skill2[key] = value;
            if (unitData1.Consti.Count == 0 && unitData2.Consti.Count != 0 && !unitData1.FilledWithNull.Contains("consti"))
                foreach (var (key, value) in unitData2.Consti)
                    unitData1.Consti[key] = value;
            if (unitData1.Learn.Count == 0 && unitData2.Learn.Count != 0 && !unitData1.FilledWithNull.Contains("learn"))
                foreach (var (key, value) in unitData2.Learn)
                    unitData1.Learn[key] = value;
            if (unitData1.Multi.Count == 0 && unitData2.Multi.Count != 0 && !unitData1.FilledWithNull.Contains("multi"))
                foreach (var (key, value) in unitData2.Multi)
                    unitData1.Multi[key] = value;
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
            if (scenarioData1.IsDefaultEnding == null && scenarioData2.IsDefaultEnding != null && !scenarioData1.FilledWithNull.Contains("default_ending"))
                scenarioData1.IsDefaultEnding = scenarioData2.IsDefaultEnding;
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
                foreach (var (key, value) in scenarioData2.CampingData)
                    scenarioData1.CampingData[key] = value;
            if (scenarioData1.ItemSale.Count == 0 && scenarioData2.ItemSale.Count != 0 && !scenarioData1.FilledWithNull.Contains("item_sale"))
                scenarioData1.ItemSale.AddRange(scenarioData2.ItemSale);
            if (scenarioData1.ItemWindowTab[0].Item1 == null && scenarioData2.ItemWindowTab[0].Item1 != null)
                for (int i = 0; i < scenarioData1.ItemWindowTab.Length; i++)
                    scenarioData1.ItemWindowTab[i] = scenarioData2.ItemWindowTab[i];
            if (scenarioData1.PoliticsData.Count == 0 && scenarioData2.PoliticsData.Count != 0 && !scenarioData1.FilledWithNull.Contains("poli"))
                foreach (var (key, value) in scenarioData2.PoliticsData)
                    scenarioData1.PoliticsData[key] = value;
        }
    }
}