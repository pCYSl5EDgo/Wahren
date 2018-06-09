using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;

namespace Wahren.Specific
{
    [MessagePackObject]
    public sealed class ScenarioData2
    {
        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        internal Task LoadingDone;

        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        public string Name => Scenario.Name;
        [Key(0)]
        public ScenarioData Scenario { get; set; }
        //数値変数（読み取り）の集合
        [Key(1)]
        public SortedSet<string> Identifier_Get { get; } = new SortedSet<string>();
        //数値変数（書き出し）の集合
        [Key(2)]
        public SortedSet<string> Identifier_Set { get; } = new SortedSet<string>();
        //スクリプト中で0あるいは1とのみsetされたことがあり、add,sub,mul,divなどの数式操作もされなかった変数の集合
        [Key(3)]
        public SortedSet<string> BoolIdentifier { get; } = new SortedSet<string>();
        [Key(4)]
        public SortedSet<string> NotBoolIdentifier { get; } = new SortedSet<string>();
        //文字変数（読み取り）の集合
        [Key(5)]
        public SortedSet<string> Variable_Get { get; } = new SortedSet<string>();
        [Key(6)]
        public SortedSet<string> Spot_Variable_Get { get; } = new SortedSet<string>();
        [Key(7)]
        public SortedSet<string> Power_Variable_Get { get; } = new SortedSet<string>();
        [Key(8)]
        public SortedSet<string> Dungeon_Variable_Get { get; } = new SortedSet<string>();
        [Key(9)]
        public SortedSet<string> Unit_Variable_Get { get; } = new SortedSet<string>();
        [Key(10)]
        public SortedSet<string> GenericUnit_Variable_Get { get; } = new SortedSet<string>();
        [Key(11)]
        public SortedSet<string> Fkey_Variable_Get { get; } = new SortedSet<string>();
        [Key(12)]
        public SortedSet<string> Race_Variable_Get { get; } = new SortedSet<string>();
        [Key(13)]
        public SortedSet<string> Skill_Variable_Get { get; } = new SortedSet<string>();
        //文字変数（書き出し）の集合
        [Key(14)]
        public SortedSet<string> Variable_Set { get; } = new SortedSet<string>();
        [Key(15)]
        public SortedSet<string> Spot_Variable_Set { get; } = new SortedSet<string>();
        [Key(16)]
        public SortedSet<string> Power_Variable_Set { get; } = new SortedSet<string>();
        [Key(17)]
        public SortedSet<string> Unit_Variable_Set { get; } = new SortedSet<string>();
        [Key(18)]
        public SortedSet<string> GenericUnit_Variable_Set { get; } = new SortedSet<string>();
        [Key(19)]
        public SortedSet<string> Fkey_Variable_Set { get; } = new SortedSet<string>();
        [Key(20)]
        public SortedSet<string> Race_Variable_Set { get; } = new SortedSet<string>();
        [Key(21)]
        public SortedSet<string> Skill_Variable_Set { get; } = new SortedSet<string>();

        [Key(22)]
        public SortedSet<string> Routine { get; } = new SortedSet<string>();
        [Key(23)]
        public SortedSet<string> Event { get; } = new SortedSet<string>();
        [Key(24)]
        public SortedSet<string> BattleEvent { get; } = new SortedSet<string>();
        [Key(25)]
        public SortedSet<string> Yet { get; } = new SortedSet<string>();
        [Key(26)]
        public Dictionary<string, UnitData> Unit { get; } = new Dictionary<string, UnitData>();
        [Key(27)]
        public Dictionary<string, GenericUnitData> GenericUnit { get; } = new Dictionary<string, GenericUnitData>();
        [Key(28)]
        public Dictionary<string, PowerData> Power { get; } = new Dictionary<string, PowerData>();
        [Key(29)]
        public Dictionary<string, SpotData> Spot { get; } = new Dictionary<string, SpotData>();
        [Key(30)]
        public Dictionary<string, string> Detail { get; } = new Dictionary<string, string>();
        [Key(31)]
        public List<LexicalTree> InitialRoutine { get; } = new List<LexicalTree>();
        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        public List<LexicalTree> World { get; } = new List<LexicalTree>();
        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        public List<LexicalTree> Fight { get; } = new List<LexicalTree>();
        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        public List<LexicalTree> Politics { get; } = new List<LexicalTree>();
        [SerializationConstructor]
        public ScenarioData2(){}
        public ScenarioData2(ScenarioData data)
        {
            Scenario = data;
            LoadingDone = Task.Factory.StartNew(() =>
            {
                DetailCollect();
                CollectData(ScriptLoader.SpotDictionary);
                CollectData(ScriptLoader.UnitDictionary);
                CollectData(ScriptLoader.GenericUnitDictionary);
                CollectData(ScriptLoader.PowerDictionary);
                CollectDataInScript(Scenario.Script);
                WPFCollect();
                FunctionCollect();
            });
        }
        private void CollectData(GenericUnitData data)
        {
        }
        private void CollectData(UnitData data)
        {
        }
        private void CollectData(SpotData data)
        {
        }
        private void CollectData(PowerData data)
        {
        }

        private void CollectData<T>(ConcurrentDictionary<string, T> dictionary) where T : ScenarioVariantData, new()
        {
            foreach (var item in dictionary.Values)
            {
                T data = new T();
                data.File = item.File;
                data.Line = item.Line;
                data.Name = item.Name;
                data.Inherit = item.Inherit;
                data.FilledWithNull.AddRange(item.FilledWithNull);
                ScriptLoader.Resolve(data, item);
                if (!string.IsNullOrEmpty(data.Inherit))
                    throw new Exception(data.GetType().Name + "->" + data.Name + ':' + data.Inherit + '\n' + data.DebugInfo);
                data.VariantData.Clear();
                if (item.VariantData.TryGetValue("", out var tmpDic))
                    ScriptLoader.Parse(tmpDic.Values, data);
                if (item.VariantData.TryGetValue(Name, out tmpDic))
                    ScriptLoader.Parse(tmpDic.Values, data);
                switch (data)
                {
                    case GenericUnitData _1:
                        GenericUnit[data.Name] = _1;
                        break;
                    case SpotData _2:
                        Spot[data.Name] = _2;
                        break;
                    case UnitData _3:
                        Unit[data.Name] = _3;
                        break;
                    case PowerData _4:
                        Power[data.Name] = _4;
                        break;
                }
            }
        }

        private void DetailCollect()
        {
            foreach (var item in ScriptLoader.Detail)
                Detail[item.Key] = item.Value;
            if (!ScriptLoader.Detail.VariantData.TryGetValue(Name, out var variant)) return;
            foreach (var item in variant)
                Detail[item.Key] = item.Value;
        }

        private void FunctionCollect()
        {
            int exCount = Event.Count + Routine.Count + BattleEvent.Count;
            var calledRoutines = new SortedSet<string>(Routine);
            calledRoutines.UnionWith(Event);
            calledRoutines.UnionWith(BattleEvent);
            foreach (var item in calledRoutines)
            {
                if (ScriptLoader.EventDictionary.TryGetValue(item, out var tmp))
                    CollectDataInScript(tmp.Script);
            }
            while (exCount != Event.Count + Routine.Count + BattleEvent.Count)
            {
                var neoRoutines = new SortedSet<string>(Routine);
                neoRoutines.UnionWith(Event);
                neoRoutines.UnionWith(BattleEvent);
                neoRoutines.ExceptWith(calledRoutines);
                calledRoutines.UnionWith(neoRoutines);
                exCount = Routine.Count + Event.Count + BattleEvent.Count;
                foreach (var item in neoRoutines)
                    if (ScriptLoader.EventDictionary.TryGetValue(item, out var tmp))
                        CollectDataInScript(tmp.Script);
            }
        }
        //World, Politics, Fight構造体の情報収集
        private void WPFCollect()
        {
            LinkedList<LexicalTree> wList = null, pList = null, fList = null;
            InitialRoutine.AddRange(Scenario.Script);
            if (!string.IsNullOrEmpty(Scenario.PoliticsEvent) && ScriptLoader.EventDictionary.TryGetValue(Scenario.PoliticsEvent, out var poli))
            {
                pList = new LinkedList<LexicalTree>(poli.Script);
                CollectDataInScript(poli.Script);
            }
            if (!string.IsNullOrEmpty(Scenario.FightEvent) && ScriptLoader.EventDictionary.TryGetValue(Scenario.FightEvent, out var fight))
            {
                fList = new LinkedList<LexicalTree>(fight.Script);
                CollectDataInScript(fight.Script);
            }
            if (!string.IsNullOrEmpty(Scenario.WorldEvent) && ScriptLoader.EventDictionary.TryGetValue(Scenario.WorldEvent, out var world))
            {
                wList = new LinkedList<LexicalTree>(world.Script);
                CollectDataInScript(world.Script);
            }
            var c = ScriptLoader.StoryDictionary.Where(_ => _.Value.Friend.Contains(Scenario.Name) || _.Value.Friend.Count == 0).Select(_ => _.Value).GetEnumerator();
            while (c.MoveNext())
            {
                var tmpStory = c.Current;
                if (tmpStory.Script.Count == 0) continue;
                CollectDataInScript(tmpStory.Script);
                if (tmpStory.Pre.HasValue && tmpStory.Pre.Value)
                {
                    if (tmpStory.Fight.HasValue && tmpStory.Fight.Value)
                    {
                        if (fList == null)
                            fList = new LinkedList<LexicalTree>(tmpStory.Script);
                        else for (int i = tmpStory.Script.Count - 1; i >= 0; --i)
                                fList.AddFirst(tmpStory.Script[i]);
                    }
                    else if (tmpStory.Politics.HasValue && tmpStory.Politics.Value)
                    {
                        if (pList == null)
                            pList = new LinkedList<LexicalTree>(tmpStory.Script);
                        else for (int i = tmpStory.Script.Count - 1; i >= 0; --i)
                                pList.AddFirst(tmpStory.Script[i]);
                    }
                    else if (wList == null)
                        wList = new LinkedList<LexicalTree>(tmpStory.Script);
                    else for (int i = tmpStory.Script.Count - 1; i >= 0; --i)
                            wList.AddFirst(tmpStory.Script[i]);
                }
                else if (tmpStory.Fight.HasValue && tmpStory.Fight.Value)
                {
                    if (fList == null)
                        fList = new LinkedList<LexicalTree>(tmpStory.Script);
                    else for (int i = 0; i < tmpStory.Script.Count; i++)
                            fList.AddLast(tmpStory.Script[i]);
                }
                else if (tmpStory.Politics.HasValue && tmpStory.Politics.Value)
                {
                    if (pList == null)
                        pList = new LinkedList<LexicalTree>(tmpStory.Script);
                    else for (int i = 0; i < tmpStory.Script.Count; i++)
                            pList.AddLast(tmpStory.Script[i]);
                }
                else if (wList == null)
                    wList = new LinkedList<LexicalTree>(tmpStory.Script);
                else for (int i = 0; i < tmpStory.Script.Count; i++)
                        wList.AddLast(tmpStory.Script[i]);
            }
            if (wList != null) World.AddRange(wList);
            if (pList != null) Politics.AddRange(pList);
            if (fList != null) Fight.AddRange(fList);
        }
        private void CollectDataInScript(List<LexicalTree> trees)
        {
            if (trees == null) return;
            for (int i = 0; i < trees.Count; i++)
            {
                switch (trees[i].Type)
                {
                    case LexicalTree.TreeType.Assign:
                    case LexicalTree.TreeType.VariableParen:
                    case LexicalTree.TreeType.NoNameStructure:
                        continue;
                    case LexicalTree.TreeType.Block:
                    case LexicalTree.TreeType.NameStructure:
                        CollectDataInScript((trees[i] as LexicalTree_Block)?.Children);
                        break;
                    case LexicalTree.TreeType.BoolParen:
                        CollectDataInBoolParen((trees[i] as LexicalTree_BoolParen));
                        break;
                    case LexicalTree.TreeType.Statement:
                        var s = trees[i] as LexicalTree_Statement;
                        if (s == null) continue;
                        CollectDataInScript(s.Children);
                        CollectDataInBoolParen((s.Paren as LexicalTree_BoolParen));
                        break;
                    case LexicalTree.TreeType.Function:
                        var f = trees[i] as LexicalTree_Function;
                        if (f == null) continue;
                        try
                        {
                            CollectDataInFunction(f.Name, f.Variable.Content);
                        }
                        catch (ApplicationException)
                        {
                            throw new Exception(f.DebugInfo);
                        }
                        break;
                }
            }
        }
        private void CollectDataInBoolParen(LexicalTree_BoolParen boolParen)
        {
            if (boolParen == null) return;
            CollectDataInBoolParen(new InterpretTreeMachine(boolParen).Tree);
        }
        private void CollectDataInBoolParenFunction(string name, List<Token> content)
        {
            #region LocalRoutines
            void LocalPowerRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Power_Variable_Get);
                else if (!ScriptLoader.PowerDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new PowerNotFoundException(content[index].DebugInfo);
            }
            void LocalSpotRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Spot_Variable_Get);
                else if (!ScriptLoader.SpotDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new SpotNotFoundException(content[index].DebugInfo);
            }
            void LocalPowerSpotRoutine(int index)
            {
                var key = content[index].ToLowerString();
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Power_Variable_Get, Spot_Variable_Get);
                else if (!ScriptLoader.PowerDictionary.ContainsKey(key) && !ScriptLoader.SpotDictionary.ContainsKey(key))
                    throw new UnitSpotNotFoundException(content[index].DebugInfo);
            }
            void LocalUnitSpotRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get, Spot_Variable_Get);
                else if (!ScriptLoader.UnitDictionary.ContainsKey(content[index].ToLowerString()) && !ScriptLoader.SpotDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new UnitSpotNotFoundException(content[index].DebugInfo);
            }
            void LocalEventRoutine(int index)
            {
                if (content.IsVariable(index))
                    throw new Exception(content[index].DebugInfo);
                else if (!ScriptLoader.EventDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new EventNotFoundException(content[index].DebugInfo);
            }
            void LocalUnitRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get);
                else if (!ScriptLoader.UnitDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new UnitNotFoundException(content[index].DebugInfo);
            }
            void LocalUnitClassRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get, GenericUnit_Variable_Get);
                else
                {
                    var key = content[index].ToLowerString();
                    if (!ScriptLoader.UnitDictionary.ContainsKey(key)
                    && !ScriptLoader.GenericUnitDictionary.ContainsKey(key))
                        throw new UnitClassNotFoundException(content[index].DebugInfo);
                }
            }
            void LocalUnitPowerRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get, Power_Variable_Get);
                else
                {
                    var key = content[index].ToLowerString();
                    if (!ScriptLoader.UnitDictionary.ContainsKey(key)
                    && !ScriptLoader.PowerDictionary.ContainsKey(key))
                        throw new UnitPowerNotFoundException(content[index].DebugInfo);
                }
            }
            void LocalDungeonRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Dungeon_Variable_Get);
                else if (!ScriptLoader.DungeonDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new DungeonNotFoundException();
            }
            #endregion
            switch (name)
            {
                case "yet":
                    content.ThrowException(1);
                    LocalEventRoutine(0);
                    Yet.Add(content[0].ToLowerString());
                    break;
                case "amount":
                case "eqvar":
                    content.ThrowException(2);
                    content.AddVariable(0, Variable_Get);
                    content.AddVariable(1, Variable_Get);
                    break;
                case "has":
                    content.ThrowException(2, int.MaxValue);
                    content.AddVariableOnly(0, Variable_Get);
                    for (int i = 1; i < content.Count; i++)
                        content.AddVariable_NotAddIdentifier(i, Variable_Get);
                    break;
                case "inpower":
                    content.ThrowException(2, int.MaxValue);
                    LocalPowerRoutine(0);
                    for (int i = 1; i < content.Count; i++)
                        LocalUnitSpotRoutine(i);
                    break;
                case "inspot":
                case "inroamspot":
                    content.ThrowException(2, int.MaxValue);
                    LocalSpotRoutine(0);
                    for (int i = 1; i < content.Count; i++)
                        LocalUnitRoutine(i);
                    break;
                case "issamearmy":
                    content.ThrowException(2, int.MaxValue);
                    for (int i = 0; i < content.Count; i++)
                        LocalUnitClassRoutine(i);
                    break;
                case "isalive":
                case "isnpc":
                case "inbattle":
                    content.ThrowException(1, int.MaxValue);
                    for (int i = 0; i < content.Count; i++)
                        LocalUnitPowerRoutine(i);
                    break;
                case "isanydead":
                case "isdead":
                    content.ThrowException(1, int.MaxValue);
                    for (int i = 0; i < content.Count; i++)
                        LocalUnitRoutine(i);
                    break;
                case "isplayer":
                    content.ThrowException(1);
                    LocalUnitPowerRoutine(0);
                    break;
                case "isscenario":
                    content.ThrowException(1);
                    if (content.IsVariable(0))
                        throw new NotFoundException(content[0].DebugInfo);
                    else if (!ScriptLoader.ScenarioDictionary.ContainsKey(content[0].ToLowerString()))
                        throw new NotFoundException(content[0].DebugInfo);
                    break;
                case "countmoney":
                case "countgain":
                case "countforce":
                case "countspot":
                case "isinvade":
                    content.ThrowException(1);
                    LocalPowerRoutine(0);
                    break;
                case "isnowspot":
                    content.ThrowException(1);
                    LocalSpotRoutine(0);
                    break;
                case "istalent":
                case "countunit":
                    content.ThrowException(1);
                    LocalUnitClassRoutine(0);
                    break;
                case "isdone":
                case "isarbeit":
                case "isroamleader":
                case "isroamer":
                case "isvassal":
                case "isleader":
                case "ismaster":
                case "isenable":
                case "isactive":
                case "isalldead":
                case "getlife":
                    content.ThrowException(1);
                    LocalUnitRoutine(0);
                    break;
                case "count":
                case "countv":
                case "countvar":
                    content.ThrowException(1);
                    content.AddVariableOnly(0, Variable_Get);
                    break;
                case "ptest":
                    content.ThrowException(2);
                    if (content.IsVariable(0))
                        content.AddVariable(0, Variable_Get);
                    else if (!(ScriptLoader.SpotDictionary.TryGetValue(content[0].ToLowerString(), out var value) && value.Politics.HasValue && value.Politics.Value))
                        throw new SpotNotFoundException(content[0].DebugInfo);
                    LocalUnitClassRoutine(1);
                    break;
                case "isenemy":
                case "isfriend":
                    content.ThrowException(2);
                    LocalUnitSpotRoutine(0);
                    LocalUnitSpotRoutine(1);
                    break;
                case "iswar":
                case "isleague":
                    content.ThrowException(2);
                    LocalPowerRoutine(0);
                    LocalPowerRoutine(1);
                    break;
                case "iscomturn":
                    content.ThrowException(0, 1);
                    if (content.Count == 1)
                        LocalPowerRoutine(0);
                    break;
                case "isjoin":
                    content.ThrowException(2, 3);
                    if (content.Count == 3)
                    {
                        if (content[2].Content.ToLower() != "on")
                            throw new OnOffException(content[2].DebugInfo);
                        LocalSpotRoutine(0);
                        LocalSpotRoutine(1);
                        break;
                    }
                    if (content.IsVariable(0))
                    {
                        content.AddVariable(0, Variable_Get);
                        LocalPowerSpotRoutine(1);
                    }
                    else if (content.IsVariable(1))
                    {
                        LocalPowerSpotRoutine(0);
                        content.AddVariable(1, Variable_Get);
                    }
                    else if (ScriptLoader.PowerDictionary.ContainsKey(content[0].ToLowerString()))
                        LocalPowerRoutine(1);
                    else if (ScriptLoader.SpotDictionary.ContainsKey(content[0].ToLowerString()))
                        LocalSpotRoutine(1);
                    else throw new SpotPowerNotFoundException(content[0].DebugInfo);
                    break;
                case "isnext":
                    content.ThrowException(2, 3);
                    LocalSpotRoutine(0);
                    LocalSpotRoutine(1);
                    if (content.Count == 3)
                        content.AddIdentifierOrNumber(2, Identifier_Get);
                    break;
                case "reckon":
                case "equal":
                    content.ThrowException(2);
                    content.AddVariable(0, Variable_Get);
                    content.AddVariable_NotAddIdentifier(1, Variable_Get);
                    break;
                case "isdungeon":
                    content.ThrowException(0, 2);
                    if (content.Count == 0) break;
                    LocalDungeonRoutine(0);
                    if (content.Count == 2)
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                    break;
                case "getclearfloor":
                    content.ThrowException(1);
                    LocalDungeonRoutine(0);
                    break;
                case "getmode":
                case "isgameclear":
                case "iswatching":
                case "isworld":
                case "isevent":
                case "isworldmusicstop":
                case "ispeace":
                case "isnewturn":
                case "isplayerturn":
                case "isplayerend":
                case "istoworld":
                case "getturn":
                case "gettime":
                case "getlimit":
                case "isredalive":
                case "isbluealive":
                case "getredcount":
                case "getbluecount":
                case "rand":
                case "countpower":
                case "isnpm":
                case "ismap":
                    content.ThrowException(0);
                    break;
                case "isinterval":
                    content.ThrowException(1);
                    content.AddIdentifierOrNumber(0, Identifier_Get);
                    break;
                default:
                    if (content.Count != 0)
                        throw new Exception(content[0].DebugInfo);
                    else throw new ApplicationException();
            }
        }
        //In Bool Paren
        private void CollectDataInBoolParen(Tree tree)
        {
            switch (tree.Type)
            {
                case 0:
                    if (tree.Children[0] == null) throw new ArgumentNullException();
                    CollectDataInBoolParen(tree.Children[0]);
                    CollectDataInBoolParen(tree.Children[1]);
                    break;
                case 1:
                    CollectDataInBoolParenFunction(tree.Function.Name, tree.Function.Variable.Content);
                    break;
                case 3:
                    Identifier_Get.Add(tree.Token.ToLowerString());
                    break;
                case 4:
                    Variable_Get.Add(tree.Token.ToLowerString());
                    break;
            }
        }
        private void CollectDataInString(Token content)
        {
            const char ampersand = '&';
            var rspan = content.Content.AsSpan();
            Span<char> wspan = stackalloc char[256];
            do
            {
                var firstAmpersand = rspan.IndexOf(ampersand);
                if (firstAmpersand == -1)
                    return;
                rspan = rspan.Slice(firstAmpersand + 1);
                var nextAmpersand = rspan.IndexOf(ampersand);
                if (nextAmpersand == -1)
                    throw new Exception(content.DebugInfo);
                if (nextAmpersand > wspan.Length)
                    throw new IndexOutOfRangeException(content.DebugInfo);
                var variable_Spot_Unit_GenericUnit_Power_Identifier = rspan.Slice(0, nextAmpersand);
                rspan = rspan.Slice(nextAmpersand + 1);
                var middle = wspan.Slice(0, variable_Spot_Unit_GenericUnit_Power_Identifier.ToLower(wspan, System.Globalization.CultureInfo.CurrentCulture)).ToString();
                if (variable_Spot_Unit_GenericUnit_Power_Identifier[0] == '@')
                {
                    Variable_Get.Add(middle);
                }
                else if (!ScriptLoader.SpotDictionary.ContainsKey(middle)
                && !ScriptLoader.SkillDictionary.ContainsKey(middle)
                && !ScriptLoader.SkillSetDictionary.ContainsKey(middle)
                && !ScriptLoader.UnitDictionary.ContainsKey(middle)
                && !ScriptLoader.GenericUnitDictionary.ContainsKey(middle)
                && !ScriptLoader.PowerDictionary.ContainsKey(middle))
                {
                    Identifier_Get.Add(middle);
                }
            } while (true);
        }
        private void CollectDataInFunction(string name, List<Token> content)
        {
            #region LocalRoutines
            void LocalAddVariableOrString(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get);
                else CollectDataInString(content[index]);
            }
            void LocalEventRoutine(int index)
            {
                if (content.IsVariable(index))
                    throw new Exception(content[index].DebugInfo);
                else if (!ScriptLoader.EventDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new EventNotFoundException(content[index].DebugInfo);
            }
            void LocalPowerRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Power_Variable_Get);
                else if (!ScriptLoader.PowerDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new PowerNotFoundException(content[index].DebugInfo);
            }
            void LocalUnitPowerRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get, Power_Variable_Get);
                else
                {
                    var key = content[index].ToLowerString();
                    if (!ScriptLoader.UnitDictionary.ContainsKey(key) && !ScriptLoader.PowerDictionary.ContainsKey(key))
                        throw new PowerNotFoundException(content[index].DebugInfo);
                }
            }
            void LocalSkillRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Skill_Variable_Get);
                else if (!ScriptLoader.SkillDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new SkillNotFoundException(content[index].DebugInfo);
            }
            void LocalSkillSkillSetRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Skill_Variable_Get);
                else
                {
                    string key = content[index].ToLowerString();
                    if (!ScriptLoader.SkillDictionary.ContainsKey(key) && !ScriptLoader.SkillSetDictionary.ContainsKey(key))
                        throw new SkillNotFoundException(content[index].DebugInfo);
                }
            }
            void LocalSpotRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Spot_Variable_Get);
                else if (!ScriptLoader.SpotDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new SpotNotFoundException(content[index].DebugInfo);
            }
            void LocalSpotPowerRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Spot_Variable_Get, Power_Variable_Get);
                else
                {
                    string key = content[index].ToLowerString();
                    if (!ScriptLoader.SpotDictionary.ContainsKey(key) && !ScriptLoader.PowerDictionary.ContainsKey(key))
                        throw new SpotPowerNotFoundException(content[index].DebugInfo);
                }
            }
            void LocalUnitSpotRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get, Spot_Variable_Get);
                else
                {
                    string key = content[index].ToLowerString();
                    if (!ScriptLoader.UnitDictionary.ContainsKey(key) && !ScriptLoader.SpotDictionary.ContainsKey(key))
                        throw new Exception(content[index].DebugInfo);
                }
            }
            void LocalUnitSpotPowerRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get, Spot_Variable_Get, Power_Variable_Get);
                else
                {
                    string key = content[index].ToLowerString();
                    if (!ScriptLoader.UnitDictionary.ContainsKey(key) && !ScriptLoader.SpotDictionary.ContainsKey(key) && !ScriptLoader.PowerDictionary.ContainsKey(key))
                        throw new Exception(content[index].DebugInfo);
                }
            }
            void LocalUnitClassRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get, GenericUnit_Variable_Get);
                else
                {
                    var key = content[index].ToLowerString();
                    if (!ScriptLoader.UnitDictionary.ContainsKey(key) && !ScriptLoader.GenericUnitDictionary.ContainsKey(key))
                        throw new UnitClassNotFoundException(content[index].DebugInfo);
                }
            }
            void LocalUnitClassRaceRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get, GenericUnit_Variable_Get, Race_Variable_Get);
                else
                {
                    var key = content[index].ToLowerString();
                    if (!ScriptLoader.UnitDictionary.ContainsKey(key) && !ScriptLoader.GenericUnitDictionary.ContainsKey(key) && !ScriptLoader.RaceDictionary.ContainsKey(key))
                        throw new UnitClassRaceNotFoundException(content[index].DebugInfo);
                }
            }
            void LocalRaceRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Race_Variable_Get);
                else if (!ScriptLoader.RaceDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new UnitNotFoundException(content[index].DebugInfo);
            }
            void LocalUnitRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get);
                else if (!ScriptLoader.UnitDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new UnitNotFoundException(content[index].DebugInfo);
            }
            void LocalDungeonRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Dungeon_Variable_Get);
                else if (!ScriptLoader.DungeonDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new ClassNotFoundException(content[index].DebugInfo);
            }
            void LocalClassRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, GenericUnit_Variable_Get);
                else if (!ScriptLoader.GenericUnitDictionary.ContainsKey(content[index].ToLowerString()))
                    throw new ClassNotFoundException(content[index].DebugInfo);
            }
            void LocalClassRaceRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, GenericUnit_Variable_Get, Race_Variable_Get);
                else
                {
                    var key = content[index].ToLowerString();
                    if (!ScriptLoader.GenericUnitDictionary.ContainsKey(key) && !ScriptLoader.RaceDictionary.ContainsKey(key))
                        throw new ClassNotFoundException(content[index].DebugInfo);
                }
            }
            void LocalPoliticsUnit(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get, Unit_Variable_Get, GenericUnit_Variable_Get);
                else
                {
                    var key = content[index].ToLowerString();
                    if (!((ScriptLoader.UnitDictionary.TryGetValue(key, out var unit) && unit.Politics.HasValue) || (ScriptLoader.GenericUnitDictionary.TryGetValue(key, out var genericUnit) && genericUnit.Politics.HasValue)))
                        throw new Exception(content[index].DebugInfo);
                }
            }
            void LocalOnOffRoutine(int index)
            {
                if (content.IsVariable(index))
                    content.AddVariable(index, Variable_Get);
                else switch (content[index].ToLowerString())
                    {
                        case "on":
                        case "off":
                            break;
                        default: throw new OnOffException(content[index].DebugInfo);
                    }
            }
            #endregion
            switch (name)
            {
                case "storerectunit":
                    content.ThrowException(6);
                    if (content.IsVariable(0))
                        content.AddVariable(0, Variable_Get);
                    else
                    {
                        var coni = content[0].ToLowerString();
                        if (coni != "red" && coni != "blue")
                            throw new OnOffException(content[0].DebugInfo);
                    }
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    content.AddIdentifierOrNumber(3, Identifier_Get);
                    content.AddIdentifierOrNumber(4, Identifier_Get);
                    content.AddVariableOnly(5, Variable_Set, Unit_Variable_Set);
                    break;
                case "storepowerofforce":
                    content.ThrowException(2);
                    content.AddIdentifierOrNumber(0, Identifier_Get);
                    content.AddVariableOnly(1, Variable_Set, Power_Variable_Set);
                    break;
                case "storenextspot":
                    content.ThrowException(2);
                    LocalSpotRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Spot_Variable_Set);
                    break;
                case "storepowerofspot":
                    content.ThrowException(2);
                    LocalSpotRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Power_Variable_Set);
                    break;
                case "storeunitofspot":
                case "storeroamunitofspot":
                case "storeleaderofspot":
                    content.ThrowException(2);
                    LocalSpotRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Unit_Variable_Set);
                    break;
                case "storespotofunit":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Spot_Variable_Set);
                    break;
                case "storepowerofunit":
                case "storetalentpower":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Power_Variable_Set);
                    break;
                case "storememberofunit":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Unit_Variable_Set);
                    break;
                case "storeskillofunit":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Skill_Variable_Set);
                    break;
                case "storeclassofunit":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, GenericUnit_Variable_Set);
                    break;
                case "storebaseclassofunit":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Fkey_Variable_Set);
                    break;
                case "storeraceofunit":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Race_Variable_Set);
                    break;
                case "storespotofpower":
                    content.ThrowException(2);
                    LocalPowerRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Spot_Variable_Set);
                    break;
                case "storeleaderofpower":
                case "storeunitofpower":
                case "storemasterofpower":
                    content.ThrowException(2);
                    LocalPowerRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Unit_Variable_Set);
                    break;
                case "storealiveunit":
                case "storetodounit":
                    content.ThrowException(2);
                    LocalClassRoutine(0);
                    content.AddVariableOnly(1, Variable_Set, Unit_Variable_Set);
                    break;
                case "storeskillset":
                    content.ThrowException(2);
                    if (content.IsVariable(0))
                        content.AddVariable(0, Variable_Get);
                    else if (!ScriptLoader.SkillSetDictionary.ContainsKey(content[0].ToLowerString()))
                        throw new Exception(content[0].DebugInfo);
                    content.AddVariableOnly(1, Variable_Set, Skill_Variable_Set);
                    break;
                case "storeud":
                    content.ThrowException(2);
                    content.AddVariable_NotAddIdentifier(0, Variable_Get);
                    content.AddVariableOnly(1, Variable_Set);
                    break;
                case "index":
                case "storeindexvar":
                    content.ThrowException(3);
                    content.AddVariableOnly(0, Variable_Get);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    content.AddVariableOnly(2, Variable_Set);
                    break;
                case "pushbattlehome":
                case "pushbattlerect":
                    content.ThrowException(2);
                    content.AddIdentifier(0, Identifier_Set);
                    content.AddIdentifier(1, Identifier_Set);
                    break;
                case "pushdiplo":
                    content.ThrowException(3);
                    LocalPowerRoutine(0);
                    LocalPowerRoutine(1);
                    content.AddIdentifier(2, Identifier_Set);
                    break;
                case "pushcon":
                    content.ThrowException(3);
                    LocalSpotRoutine(0);
                    LocalUnitClassRoutine(1);
                    content.AddIdentifier(2, Identifier_Set);
                    break;
                case "pushstatus":
                    content.ThrowException(3);
                    LocalUnitRoutine(0);
                    if (content.IsVariable(1))
                        content.AddVariable(1, Variable_Get);
                    else
                    {
                        var coni = content[1].ToLowerString();
                        if (coni != "hp" && coni != "mp" && coni != "attack" && coni != "defense" && coni != "magic" && coni != "magdef" && coni != "dext" && coni != "speed" && coni != "move" && coni != "hprec" && coni != "mprec")
                            throw new Exception(content[1].DebugInfo);
                    }
                    content.AddIdentifier(2, Identifier_Set);
                    break;
                case "pushspotpos":
                    content.ThrowException(3);
                    LocalSpotRoutine(0);
                    content.AddIdentifier(1, Identifier_Set);
                    content.AddIdentifier(2, Identifier_Set);
                    break;
                case "pushlevel":
                case "pushloyal":
                case "pushmerits":
                case "pushsex":
                case "pushrank":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    content.AddIdentifier(1, Identifier_Set);
                    break;
                case "pushtrust":
                case "pushmoney":
                case "pushforce":
                    content.ThrowException(2);
                    LocalUnitPowerRoutine(0);
                    content.AddIdentifier(1, Identifier_Set);
                    break;
                case "pushspot":
                case "pushbaselevel":
                case "pushtrain":
                case "pushtrainup":
                    content.ThrowException(2);
                    LocalPowerRoutine(0);
                    content.AddIdentifier(1, Identifier_Set);
                    break;
                case "pushitem":
                    content.ThrowException(2);
                    if (content.IsVariable(0))
                        content.AddVariable(0, Variable_Get, Skill_Variable_Get);
                    else if (!(ScriptLoader.SkillDictionary.TryGetValue(content[0].ToLowerString(), out var item) && item.ItemType.HasValue))
                        throw new Exception(content[0].DebugInfo);
                    content.AddIdentifier(1, Identifier_Set);
                    break;
                case "pushgain":
                    content.ThrowException(2);
                    LocalSpotPowerRoutine(0);
                    content.AddIdentifier(1, Identifier_Set);
                    break;
                case "pushcastle":
                case "pushcapa":
                    content.ThrowException(2);
                    LocalSpotRoutine(0);
                    content.AddIdentifier(1, Identifier_Set);
                    break;
                case "pushv":
                case "pushvar":
                    content.ThrowException(2, 3);
                    content.AddVariableOnly(0, Variable_Get);
                    if (content.Count == 3)
                    {
                        content.AddVariable_NotAddIdentifier(1, Variable_Get);
                        content.AddIdentifier(2, Identifier_Set);
                    }
                    else content.AddIdentifier(1, Identifier_Set);
                    break;
                case "hidelink":
                    content.ThrowException(2);
                    LocalSpotRoutine(0);
                    LocalSpotRoutine(1);
                    break;
                case "setpm":
                    content.ThrowException(2);
                    LocalPoliticsUnit(0);
                    content.AddVariable(1, Variable_Get);
                    break;
                case "storepm":
                    content.ThrowException(2);
                    LocalPoliticsUnit(0);
                    content.AddVariableOnly(1, Variable_Set, GenericUnit_Variable_Set);
                    break;
                case "setud":
                    content.ThrowException(2);
                    content.AddVariable_NotAddIdentifier(0, Variable_Get);
                    content.AddVariable_NotAddIdentifier(1, Variable_Get);
                    break;
                case "focus":
                    content.ThrowException(0, 1);
                    if (content.Count == 1)
                        LocalUnitRoutine(0);
                    break;
                case "title":
                case "title2":
                    content.ThrowException(1, 2);
                    if (content.Count == 2)
                    {
                        content.AddVariableOrString(0, Variable_Get);
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                    }
                    else content.AddIdentifierOrNumber(0, Identifier_Get);
                    break;
                case "face":
                case "picture":
                case "image":
                case "showimage":
                case "showpict":
                case "showface":
                    content.ThrowException(1, 5);
                    content.AddVariableOrString(0, Variable_Get);
                    if (content.Count > 1)
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                    if (content.Count > 2)
                        content.AddIdentifierOrNumber(2, Identifier_Get);
                    if (content.Count > 3)
                        content.AddIdentifierOrNumber(3, Identifier_Get);
                    if (content.Count > 4)
                        content.AddIdentifierOrNumber(4, Identifier_Get);
                    break;
                case "setgameclear":
                    content.ThrowException(0, 1);
                    if (content.Count == 0) break;
                    if (!content.IsVariable(0) && content[0].ToLowerString() != "on")
                        throw new OnOffException(content[0].DebugInfo);
                    break;
                case "exit":
                    content.ThrowException(0, 2);
                    break;
                case "wait":
                    content.ThrowException(0, 1);
                    if (content.Count == 1)
                        content.AddIdentifierOrNumber(0, Identifier_Get);
                    break;
                case "linkspot":
                case "linkescape":
                    content.ThrowException(2, 4);
                    LocalSpotRoutine(0);
                    LocalSpotRoutine(1);
                    if (content.Count == 4)
                        content.AddIdentifierOrNumber(3, Identifier_Get);
                    else if (content.Count == 3)
                        if (content[2].Type == 2)
                            content.AddIdentifierOrNumber(2, Identifier_Get);
                        else
                            content.AddVariable_NotAddIdentifier(2, Variable_Get);
                    break;
                case "showchara":
                    content.ThrowException(3, 5);
                    content.AddVariable_NotAddIdentifier(0, Variable_Get);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    if (content.Count == 5)
                    {
                        content.AddIdentifierOrNumber(3, Identifier_Get);
                        content.AddIdentifierOrNumber(4, Identifier_Get);
                    }
                    break;
                case "scroll":
                case "scroll2":
                case "locate":
                    content.ThrowException(1, 2);
                    if (content.Count == 1)
                        LocalUnitSpotRoutine(0);
                    else
                    {
                        content.AddIdentifierOrNumber(0, Identifier_Get);
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                    }
                    break;
                case "doskill":
                    content.ThrowException(5);
                    LocalSkillRoutine(0);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    content.AddIdentifierOrNumber(3, Identifier_Get);
                    LocalOnOffRoutine(4);
                    break;
                case "showspot":
                case "hidespot":
                    content.ThrowException(1);
                    LocalSpotRoutine(0);
                    break;
                case "showspotmark":
                case "spotmark":
                    content.ThrowException(0, 2);
                    if (content.Count > 0)
                        LocalSpotPowerRoutine(0);
                    if (content.Count == 2)
                    {
                        if (content.IsIdentifier(1))
                            content.AddIdentifier(1, Identifier_Get);
                        else if (content[1].Number < -2 || content[1].Number > 6)
                            throw new IndexOutOfRangeException(content[1].DebugInfo);
                    }
                    break;
                case "gread":
                    content.ThrowException(2, 3);
                    if (!content.IsVariableOrIdentifier(0))
                        throw new Exception(content[0].DebugInfo);
                    content.AddIdentifier(1, Identifier_Set);
                    if (content.Count == 3)
                        content.AddVariableOnly(2, Variable_Set);
                    break;
                case "gwrite":
                    content.ThrowException(2, 3);
                    if (!content.IsVariableOrIdentifier(0))
                        throw new Exception(content[0].DebugInfo);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    if (content.Count == 3)
                        content.AddVariableOrString(2, Variable_Get);
                    break;
                case "setpowerhome":
                    content.ThrowException(1, 2);
                    LocalPowerRoutine(1);
                    if (content.Count == 2)
                        content.AddVariable(1, Variable_Get);
                    break;
                case "setv":
                case "setvar":
                case "addv":
                case "addvar":
                case "subv":
                case "subvar":
                case "addstr":
                case "addint":
                    content.ThrowException(2);
                    content.AddVariableOnly(0, Variable_Set);
                    content.AddVariableOrString(1, Variable_Get);
                    break;
                case "storeplayerunit":
                case "storealltalent":
                    content.ThrowException(1);
                    content.AddVariableOnly(0, Variable_Set, Unit_Variable_Set);
                    break;
                case "storeplayerpower":
                case "storeallpower":
                case "storecompower":
                case "storenowpower":
                case "storeattackpower":
                case "storedefensepower":
                    content.ThrowException(1);
                    content.AddVariableOnly(0, Variable_Set, Power_Variable_Set);
                    break;
                case "storebattlespot":
                case "storeallspot":
                case "storeneutralspot":
                    content.ThrowException(1);
                    content.AddVariableOnly(0, Variable_Set, Spot_Variable_Set);
                    break;
                case "clear":
                case "clearvar":
                case "shuffle":
                case "shufflevar":
                    content.ThrowException(1);
                    content.AddVariableOnly(0, Variable_Set);
                    break;
                case "add":
                case "sub":
                case "mul":
                case "div":
                case "log":
                case "pow":
                case "mod":
                case "per":
                    content.ThrowException(2);
                    content.AddIdentifier(0, Identifier_Set);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    content.NotBoolIdentifier(NotBoolIdentifier, BoolIdentifier);
                    break;
                case "set":
                    content.ThrowException(2);
                    content.AddIdentifier(0, Identifier_Set);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    var identifierName = content[0].Content.ToLower();
                    if (content[1].Type == 2 && (content[1].Number == 0 || content[1].Number == 1))
                    {
                        if (!NotBoolIdentifier.Contains(identifierName))
                            BoolIdentifier.Add(identifierName);
                    }
                    else if (!NotBoolIdentifier.Contains(identifierName))
                    {
                        BoolIdentifier.Remove(identifierName);
                        NotBoolIdentifier.Add(identifierName);
                    }
                    break;
                case "pushrand":
                case "pushrand2":
                case "pushturn":
                case "pushlimit":
                case "pushcountpower":
                    content.ThrowException(1);
                    content.AddIdentifier(0, Identifier_Set);
                    break;
                case "hidespotmark":
                case "break":
                case "continue":
                case "clearbattlerecord":
                case "return":
                case "erase":
                case "next":
                case "hideblind":
                case "showblind":
                case "save":
                case "terminate":
                case "backscroll":
                case "stopbgm":
                case "playworld":
                case "playbattle":
                case "setworldmusic":
                case "resetworldmusic":
                case "reloadmenu":
                case "resetzone":
                case "opengoal":
                case "closegoal":
                case "resettime":
                case "win":
                    content.ThrowException(0);
                    break;
                case "movetroop":
                case "movetroopfix":
                case "smovetroop":
                case "smovetroopfix":
                    content.ThrowException(2, 4);
                    LocalUnitClassRoutine(0);
                    if (content.Count == 4)
                    {
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                        content.AddIdentifierOrNumber(2, Identifier_Get);
                        content.AddIdentifierOrNumber(3, Identifier_Get);
                    }
                    else if (content.Count == 3) throw new ArgumentOutOfRangeException();
                    else
                    {
                        LocalUnitClassRoutine(1);
                    }
                    break;
                case "addtroop":
                    content.ThrowException(5);
                    LocalUnitClassRoutine(0);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    content.AddIdentifierOrNumber(3, Identifier_Get);
                    switch (content[4].ToLowerString())
                    {
                        case "red":
                        case "blue":
                            break;
                        default: throw new Exception(content[4].DebugInfo);
                    }
                    break;
                case "ispostin":
                    content.ThrowException(3, 5);
                    content.AddVariable_NotAddIdentifier(0, Variable_Get);
                    if (content.Count == 3)
                    {
                        LocalUnitRoutine(1);
                        content.AddIdentifierOrNumber(2, Identifier_Get);
                        break;
                    }
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    content.AddIdentifierOrNumber(3, Identifier_Get);
                    if (content.Count == 5)
                        content.AddIdentifierOrNumber(4, Identifier_Get);
                    break;
                case "getdistance":
                    content.ThrowException(2, 3);
                    LocalUnitRoutine(0);
                    if (content.Count == 2)
                        LocalUnitRoutine(1);
                    else
                    {
                        content.AddIdentifierOrNumber(1, Identifier_Set);
                        content.AddIdentifierOrNumber(2, Identifier_Set);
                    }
                    break;
                case "shadow":
                    if (content.Count == 0) break;
                    if (content.Count != 10) throw new ArgumentOutOfRangeException();
                    content.AddIdentifierOrNumber(0, Identifier_Get);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    content.AddIdentifierOrNumber(3, Identifier_Get);
                    content.AddIdentifierOrNumber(4, Identifier_Get);
                    content.AddIdentifierOrNumber(5, Identifier_Get);
                    content.AddIdentifierOrNumber(6, Identifier_Get);
                    content.AddIdentifierOrNumber(7, Identifier_Get);
                    content.AddIdentifierOrNumber(8, Identifier_Get);
                    content.AddIdentifierOrNumber(9, Identifier_Get);
                    break;
                case "darkness":
                    content.ThrowException(0, 4);
                    if (content.Count > 0)
                        content.AddIdentifierOrNumber(0, Identifier_Get);
                    if (content.Count > 1)
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                    if (content.Count > 2)
                        content.AddIdentifierOrNumber(2, Identifier_Get);
                    if (content.Count > 3)
                        content.AddIdentifierOrNumber(3, Identifier_Get);
                    break;
                case "loopbgm":
                    content.ThrowException(1, int.MaxValue);
                    for (int i = 0; i < content.Count; i++)
                        content.AddVariableOrString(i, Variable_Get);
                    break;
                case "resettruce":
                    content.ThrowException(1, 2);
                    if (content.Count == 1)
                        content.AddVariable(0, Variable_Get);
                    else
                    {
                        LocalPowerRoutine(0);
                        LocalPowerRoutine(1);
                    }
                    break;
                case "resetleague":
                    content.ThrowException(1, 3);
                    if (content.Count == 1)
                    {
                        content.AddVariable(0, Variable_Get);
                        break;
                    }
                    LocalPowerRoutine(0);
                    LocalPowerRoutine(1);
                    if (content.Count == 2)
                        break;
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    break;
                case "resetenemypower":
                    content.ThrowException(1, 2);
                    LocalPowerRoutine(0);
                    if (content.Count == 2)
                        LocalPowerRoutine(1);
                    break;
                case "setenemypower":
                    content.ThrowException(3);
                    LocalPowerRoutine(0);
                    LocalPowerRoutine(1);
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    break;
                case "setdiplo":
                case "adddiplo":
                case "settruce":
                case "setleague":
                    content.ThrowException(2, 3);
                    if (content.Count == 2)
                    {
                        content.AddVariable(0, Variable_Get);
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                    }
                    else
                    {
                        LocalPowerRoutine(0);
                        LocalPowerRoutine(1);
                        content.AddIdentifierOrNumber(2, Identifier_Get);
                    }
                    break;
                case "addunit":
                    content.ThrowException(2, 3);
                    LocalUnitClassRoutine(0);
                    if (content.Count == 2)
                        LocalUnitSpotPowerRoutine(1);
                    else
                    {
                        LocalSpotRoutine(1);
                        if (!content.IsVariable(2) && content[2].ToLowerString() != "roam")
                            throw new Exception(content[2].DebugInfo);
                    }
                    break;
                case "eraseunit2":
                    content.ThrowException(2, int.MaxValue);
                    LocalSpotPowerRoutine(0);
                    for (int i = 1; i < content.Count; i++)
                        LocalClassRoutine(i);
                    break;
                case "roamunit":
                    content.ThrowException(1);
                    LocalUnitRoutine(0);
                    break;
                case "unionpower":
                    content.ThrowException(2);
                    LocalPowerRoutine(0);
                    LocalPowerRoutine(1);
                    break;
                case "settraining":
                case "addtraining":
                case "settrainingup":
                case "addtrainingup":
                case "setbaselevel":
                case "addbaselevel":
                    content.ThrowException(2);
                    LocalPowerRoutine(0);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    break;
                case "addpowermerce2":
                case "addpowermerce":
                    content.ThrowException(2, int.MaxValue);
                    LocalPowerRoutine(0);
                    for (int i = 1; i < content.Count; i++)
                        LocalClassRoutine(i);
                    break;
                case "addpowerstaff2":
                case "addpowerstaff":
                    content.ThrowException(2, int.MaxValue);
                    LocalPowerRoutine(0);
                    for (int i = 1; i < content.Count; i++)
                        LocalClassRaceRoutine(i);
                    break;
                case "changepowerfix":
                    content.ThrowException(2);
                    LocalPowerRoutine(0);
                    if (content.IsVariable(1))
                        content.AddVariable_NotAddIdentifier(1, Variable_Get);
                    else switch (content[1].ToLowerString())
                        {
                            case "off":
                            case "on":
                            case "home":
                            case "hold":
                            case "warlike":
                            case "freeze":
                                break;
                            default:
                                throw new Exception(content[1].DebugInfo);
                        }
                    break;
                case "changepowername":
                    content.ThrowException(2);
                    LocalPowerRoutine(0);
                    if (content.IsVariable(1))
                        content.AddVariable_NotAddIdentifier(1, Variable_Get);
                    break;
                case "changepowerflag":
                    content.ThrowException(2);
                    LocalPowerRoutine(0);
                    if (!content.IsVariable(1) && !ScriptLoader.Folder.Flag_Bmp.Any(_ => Path.GetFileNameWithoutExtension(_) == content[1].Content) && !ScriptLoader.Folder.Flag_Png.Any(_ => Path.GetFileNameWithoutExtension(_) == content[1].Content) && !ScriptLoader.Folder.Flag_Jpg.Any(_ => Path.GetFileNameWithoutExtension(_) == content[1].Content))
                        throw new Exception(content[1].DebugInfo);
                    else
                        content.AddVariable_NotAddIdentifier(1, Variable_Get);
                    break;
                case "changemaster":
                    content.ThrowException(1, 3);
                    switch (content.Count)
                    {
                        case 1:
                            LocalUnitRoutine(0);
                            break;
                        case 2:
                            if (!content.IsVariable(0) && !ScriptLoader.UnitDictionary.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.PowerDictionary.ContainsKey(content[0].ToLowerString()))
                                throw new Exception(content[0].DebugInfo);
                            LocalUnitRoutine(1);
                            content.AddVariable_NotAddIdentifier(0, Variable_Get);
                            break;
                        case 3:
                            LocalUnitRoutine(0);
                            LocalUnitRoutine(1);
                            if (!content.IsVariable(2) && content[2].ToLowerString() != "flag")
                                throw new Exception(content[2].DebugInfo);
                            content.AddVariable_NotAddIdentifier(2, Variable_Get);
                            break;
                    }
                    break;
                case "changeplayer":
                    content.ThrowException(0, 1);
                    if (content.Count == 0) break;
                    LocalUnitRoutine(0);
                    break;
                case "eraseunit":
                case "eraseunittroop":
                case "roamtroop":
                    content.ThrowException(1);
                    LocalUnitRoutine(0);
                    break;
                case "shifttroop":
                case "shifttroop2":
                    content.ThrowException(3, 4);
                    if (content.Count == 4)
                    {
                        if (content.IsVariable(3)) Variable_Get.Add(content[3].ToLowerString());
                        else if (content[3].ToLowerString() != "on")
                            throw new Exception(content[3].DebugInfo);
                    }
                    LocalUnitClassRoutine(0);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    break;
                case "skilltroop":
                    content.ThrowException(2, 3);
                    LocalUnitClassRoutine(0);
                    if (content.Count == 2)
                    {
                        if (content.IsVariable(1))
                        {
                            Variable_Get.Add(content[1].ToLowerString());
                            break;
                        }
                        if (!ScriptLoader.SkillDictionary.ContainsKey(content[1].ToLowerString())
                        && !ScriptLoader.UnitDictionary.ContainsKey(content[1].ToLowerString())
                        && !ScriptLoader.GenericUnitDictionary.ContainsKey(content[1].ToLowerString()))
                            throw new Exception(content[1].DebugInfo);
                    }
                    else if (content.IsIdentifierOrNumber(1))
                    {
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                        content.AddIdentifierOrNumber(2, Identifier_Get);
                    }
                    else if (content.IsVariable(2))
                        content.AddVariable(2, Variable_Get);
                    else switch (content[2].ToLowerString())
                        {
                            case "on":
                            case "dead_ok":
                                break;
                            default: throw new Exception(content[2].DebugInfo);
                        }
                    break;
                case "formtroop":
                case "speedtroop":
                    content.ThrowException(2);
                    LocalUnitClassRoutine(0);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    break;
                case "unctrltroop":
                case "ctrltroop":
                case "activetroop":
                case "sleeptroop":
                case "retreattroop":
                case "removetroop":
                case "freetroop":
                case "halttroop":
                    content.ThrowException(1);
                    LocalUnitClassRoutine(0);
                    break;
                case "setgain":
                case "setcastle":
                case "setcapa":
                case "addgain":
                case "addcastle":
                case "addcapa":
                case "changecastle":
                    content.ThrowException(2);
                    LocalSpotRoutine(0);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    break;
                case "changemap":
                    content.ThrowException(2);
                    LocalSpotRoutine(0);
                    if (content.IsVariable(1))
                        content.AddVariable(1, Variable_Get);
                    else if (!ScriptLoader.Folder.Stage_Map.Any((_) => Path.GetFileNameWithoutExtension(_).ToLower() == content[1].Content.ToLower()))
                        throw new Exception(content[1].DebugInfo);
                    break;
                case "addpower":
                case "erasepower":
                case "erasepowermerce":
                case "erasepowerstaff":
                    content.ThrowException(1);
                    LocalPowerRoutine(0);
                    break;
                case "addspot":
                    content.ThrowException(1, 2);
                    LocalSpotRoutine(0);
                    if (content.Count == 2)
                        LocalPowerRoutine(1);
                    break;
                case "removespot":
                    content.ThrowException(1);
                    LocalSpotRoutine(0);
                    break;
                case "setarbeit":
                    content.ThrowException(3);
                    LocalPowerRoutine(0);
                    LocalUnitRoutine(1);
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    break;
                case "scrollspeed":
                case "zoom":
                case "volume":
                case "setlimit":
                case "addlimit":
                    content.ThrowException(1);
                    content.AddIdentifierOrNumber(0, Identifier_Get);
                    break;
                case "fadeout":
                case "fadein":
                case "shake":
                    content.ThrowException(0, 1);
                    if (content.Count == 1)
                        content.AddIdentifierOrNumber(0, Identifier_Get);
                    break;
                case "playbgm":
                    content.ThrowException(0, 1);
                    if (content.Count == 0) break;
                    content.AddVariableOrString(0, Variable_Get);
                    break;
                case "playse":
                    content.ThrowException(1);
                    content.AddVariableOrString(0, Variable_Get);
                    break;
                case "showpolitics":
                case "showparty":
                    content.ThrowException(1);
                    LocalOnOffRoutine(0);
                    break;
                case "changespotimage":
                    content.ThrowException(2);
                    LocalSpotRoutine(0);
                    content.AddVariable_NotAddIdentifier(1, Variable_Get);
                    break;
                case "changedungeon":
                    content.ThrowException(2);
                    LocalSpotRoutine(0);
                    LocalDungeonRoutine(1);
                    break;
                case "setdungeonfloor":
                    content.ThrowException(2);
                    LocalDungeonRoutine(0);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    break;
                case "setmoney":
                case "addmoney":
                    content.ThrowException(2);
                    LocalUnitPowerRoutine(0);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    break;
                case "changeclass":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    LocalClassRoutine(1);
                    break;
                case "eraseskill":
                    content.ThrowException(1, int.MaxValue);
                    LocalUnitRoutine(0);
                    for (int i = 1; i < content.Count; i++)
                        LocalSkillSkillSetRoutine(i);
                    break;
                case "addskill":
                case "addskill2":
                case "removeskill":
                    content.ThrowException(2, int.MaxValue);
                    LocalUnitRoutine(0);
                    for (int i = 1; i < content.Count; i++)
                        LocalSkillSkillSetRoutine(i);
                    break;
                case "addstatus":
                case "setstatus":
                    content.ThrowException(3);
                    LocalUnitRoutine(0);
                    if (!content.IsVariable(1))
                        switch (content[1].ToLowerString())
                        {
                            case "hp":
                            case "mp":
                            case "attack":
                            case "defense":
                            case "magic":
                            case "magdef":
                            case "dext":
                            case "speed":
                            case "move":
                            case "hprec":
                            case "mprec":
                                break;
                            default: throw new Exception(content[1].DebugInfo);
                        }
                    content.AddIdentifierOrNumber(2, Identifier_Get);
                    break;
                case "hidechara":
                case "reversechara":
                case "showdungeon":
                    content.ThrowException(1);
                    content.AddVariable_NotAddIdentifier(0, Variable_Get);
                    break;
                case "equipitem":
                    content.ThrowException(2, 3);
                    if (content.Count == 3)
                    {
                        if (content.IsVariable(2))
                        {
                            if (content[2].ToLowerString() != "on")
                                throw new Exception(content[2].DebugInfo);
                        }
                        else Variable_Get.Add(content[2].ToLowerString());
                    }
                    LocalUnitRoutine(0);
                    if (content.IsVariable(1)) Variable_Get.Add(content[1].ToLowerString());
                    else if (ScriptLoader.SkillDictionary.TryGetValue(content[1].ToLowerString(), out var tmpSkill) && tmpSkill.ItemType != null)
                        break;
                    else throw new Exception(content[1].DebugInfo);
                    break;
                case "entryitem":
                case "exititem":
                case "additem":
                case "eraseitem":
                    content.ThrowException(1);
                    if (content.IsVariable(0))
                        Variable_Get.Add(content[0].ToLowerString());
                    else if (ScriptLoader.SkillDictionary.TryGetValue(content[0].ToLowerString(), out var tmpSkill) && tmpSkill.ItemType != null)
                        break;
                    else throw new SkillNotFoundException(content[0].DebugInfo);
                    break;
                case "erasefriend":
                    content.ThrowException(1, int.MaxValue);
                    LocalUnitRoutine(0);
                    for (int i = 1; i < content.Count; i++)
                        LocalUnitClassRaceRoutine(i);
                    break;
                case "addfriend":
                    content.ThrowException(2, int.MaxValue);
                    LocalUnitRoutine(0);
                    for (int i = 1; i < content.Count; i++)
                        LocalUnitClassRaceRoutine(i);
                    break;
                case "changerace":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    LocalRaceRoutine(1);
                    break;
                case "setdone":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    LocalOnOffRoutine(1);
                    break;
                case "addloyal":
                case "addmerits":
                case "addtrust":
                case "addlevel":
                case "levelup":
                case "setlevel":
                    content.ThrowException(2);
                    LocalUnitRoutine(0);
                    content.AddIdentifierOrNumber(1, Identifier_Get);
                    break;
                case "bg":
                case "bcg":
                    content.ThrowException(1, 2);
                    if (content.Count == 2)
                        if (content[1].ToLowerString() != "on")
                            throw new OnOffException(content[1].DebugInfo);
                    content.AddVariableOrString(0, Variable_Get);
                    break;
                case "choice":
                    content.ThrowException(1, int.MaxValue);
                    content.AddIdentifier(0, Identifier_Set);
                    for (int i = 1; i < content.Count; i++)
                        LocalAddVariableOrString(i);
                    break;
                case "choicetitle":
                    content.ThrowException(0, 1);
                    if (content.Count == 1)
                        content.AddVariableOrString(0, Variable_Get);
                    break;
                case "select":
                    content.ThrowException(2);
                    content.AddIdentifier(0, Identifier_Set);
                    break;
                case "dialog":
                    content.ThrowException(1, 2);
                    if (content.Count == 2)
                    {
                        content.AddVariable_NotAddIdentifier(0, Variable_Get);
                        LocalAddVariableOrString(1);
                    }
                    else LocalAddVariableOrString(0);
                    break;
                case "fontc":
                    content.ThrowException(0, 3);
                    if (content.Count == 3)
                    {
                        content.AddIdentifierOrNumber(0, Identifier_Get);
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                        content.AddIdentifierOrNumber(2, Identifier_Get);
                    }
                    break;
                case "font":
                    content.ThrowException(0, 3);
                    if (content.Count == 3)
                    {
                        content.AddVariableOrString(0, Variable_Get);
                        content.AddIdentifierOrNumber(1, Identifier_Get);
                        content.AddIdentifierOrNumber(2, Identifier_Get);
                    }
                    break;
                case "talk":
                case "msg":
                case "msg2":
                    content.ThrowException(1, 3);
                    switch (content.Count)
                    {
                        case 1:
                            LocalAddVariableOrString(0);
                            break;
                        case 2:
                            if (content.IsVariable(0))
                                content.AddVariable(0, Variable_Get);
                            LocalAddVariableOrString(1);
                            break;
                        case 3:
                            content.AddVariable_NotAddIdentifier(0, Variable_Get);
                            LocalAddVariableOrString(1);
                            LocalAddVariableOrString(2);
                            break;
                    }
                    break;
                case "battleevent":
                    content.ThrowException(1);
                    LocalEventRoutine(0);
                    BattleEvent.Add(content[0].ToLowerString());
                    break;
                case "event":
                    content.ThrowException(1);
                    if (content.IsVariable(0))
                        content.AddVariable(0, Variable_Get);
                    else
                    {
                        var key = content[0].ToLowerString();
                        if (!ScriptLoader.EventDictionary.ContainsKey(key) && key != "world_bgm" && key != "count")
                            throw new EventNotFoundException(content[0].DebugInfo);
                    }
                    Event.Add(content[0].ToLowerString());
                    break;
                case "routine":
                    content.ThrowException(1);
                    if (content.IsVariable(0))
                        content.AddVariable(0, Variable_Get);
                    else
                    {
                        var key = content[0].ToLowerString();
                        if (!ScriptLoader.EventDictionary.ContainsKey(key) && key != "world_bgm" && key != "count")
                            throw new EventNotFoundException(content[0].DebugInfo);
                    }
                    Routine.Add(content[0].ToLowerString());
                    break;
                //未だ追加しきれていない関数を調べる際にはコメントインしてください。
                default:
                    if (content.Count != 0)
                        throw new Exception(content[0].DebugInfo);
                    throw new ApplicationException();
            }
        }
    }
}