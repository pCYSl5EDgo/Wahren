using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
namespace Wahren.Specific
{
    public class ScenarioData2
    {
        internal Task LoadingDone;
        public string Name => Scenario.Name;
        public ScenarioData Scenario { get; set; }
        public SortedSet<string> Identifier { get; } = new SortedSet<string>();
        //スクリプト中で0あるいは1とのみsetされたことがあり、add,sub,mul,divなどの数式操作もされなかった変数の集合
        public SortedSet<string> BoolIdentifier { get; } = new SortedSet<string>();
        public SortedSet<string> NotBoolIdentifier { get; } = new SortedSet<string>();
        public SortedSet<string> Variable { get; } = new SortedSet<string>();
        public SortedSet<string> Routine { get; } = new SortedSet<string>();
        public SortedSet<string> Event { get; } = new SortedSet<string>();
        public SortedSet<string> BattleEvent { get; } = new SortedSet<string>();
        public SortedSet<string> Yet { get; } = new SortedSet<string>();
        public SortedSet<string> Func_isenable { get; } = new SortedSet<string>();
        public Dictionary<string, UnitData> Unit { get; } = new Dictionary<string, UnitData>();
        public Dictionary<string, GenericUnitData> GenericUnit { get; } = new Dictionary<string, GenericUnitData>();
        public Dictionary<string, PowerData> Power { get; } = new Dictionary<string, PowerData>();
        public Dictionary<string, SpotData> Spot { get; } = new Dictionary<string, SpotData>();
        public Dictionary<string, string> Detail { get; } = new Dictionary<string, string>();
        public List<LexicalTree> InitialRoutine { get; } = new List<LexicalTree>();
        public List<LexicalTree> World { get; } = new List<LexicalTree>();
        public List<LexicalTree> Fight { get; } = new List<LexicalTree>();
        public List<LexicalTree> Politics { get; } = new List<LexicalTree>();
        public ScenarioData2(ScenarioData data)
        {
            Scenario = data;
            LoadingDone = Task.Factory.StartNew(() =>
              {
                  DetailCollect();
                  CollectData(ScriptLoader.Spot);
                  CollectData(ScriptLoader.Unit);
                  CollectData(ScriptLoader.GenericUnit);
                  CollectData(ScriptLoader.Power);
              });
            WPFCollect();
            FunctionCollect();
        }

        private void CollectData<T>(ConcurrentDictionary<string, T> dictionary) where T : ScenarioVariantData, new()
        {
            var type = typeof(T);
            foreach (var item in dictionary.Values)
            {
                T data = new T();
                data.Name = item.Name;
                data.Inherit = item.Inherit;
                data.FilledWithNull.AddRange(item.FilledWithNull);
                ScriptLoader.Resolve(data, item);
                data.VariantData.Clear();
                Dictionary<string, LexicalTree_Assign> tmpDic;
                if (item.VariantData.TryGetValue("", out tmpDic))
                    ScriptLoader.Parse(tmpDic.Values, data);
                if (item.VariantData.TryGetValue(Name, out tmpDic))
                    ScriptLoader.Parse(tmpDic.Values, data);
                if (type == ScriptLoader.GenericUnitDataType)
                    GenericUnit[data.Name] = data as GenericUnitData;
                else if (type == ScriptLoader.SpotDataType)
                    Spot[data.Name] = data as SpotData;
                else if (type == ScriptLoader.UnitDataType)
                    Unit[data.Name] = data as UnitData;
                else if (type == ScriptLoader.PowerDataType)
                    Power[data.Name] = data as PowerData;
            }
        }

        private void DetailCollect()
        {
            foreach (var item in ScriptLoader.Detail)
                Detail[item.Key] = item.Value;
            Dictionary<string, string> variant;
            if (!ScriptLoader.Detail.VariantData.TryGetValue(Name, out variant)) return;
            foreach (var item in variant)
                Detail[item.Key] = item.Value;
        }

        private void FunctionCollect()
        {
            int exCount = Event.Count + Routine.Count + BattleEvent.Count;
            SortedSet<string> calledRoutines = new SortedSet<string>(Routine);
            calledRoutines.UnionWith(Event);
            calledRoutines.UnionWith(BattleEvent);
            foreach (var item in calledRoutines)
            {
                EventData tmp;
                if (ScriptLoader.Event.TryGetValue(item, out tmp))
                    CollectData(tmp.Script);
            }
            while (exCount != Event.Count + Routine.Count + BattleEvent.Count)
            {
                SortedSet<string> neoRoutines = new SortedSet<string>(Routine);
                neoRoutines.UnionWith(Event);
                neoRoutines.UnionWith(BattleEvent);
                neoRoutines.ExceptWith(calledRoutines);
                calledRoutines.UnionWith(neoRoutines);
                exCount = Routine.Count + Event.Count + BattleEvent.Count;
                foreach (var item in neoRoutines)
                {
                    EventData tmp;
                    if (ScriptLoader.Event.TryGetValue(item, out tmp))
                        CollectData(tmp.Script);
                }
            }
        }
        private void WPFCollect()
        {
            EventData poli, fight, world;
            LinkedList<LexicalTree> wList = null, pList = null, fList = null;
            InitialRoutine.AddRange(Scenario.Script);
            if (!string.IsNullOrEmpty(Scenario.PoliticsEvent) && ScriptLoader.Event.TryGetValue(Scenario.PoliticsEvent, out poli))
            {
                pList = new LinkedList<LexicalTree>(poli.Script);
                CollectData(poli.Script);
            }
            if (!string.IsNullOrEmpty(Scenario.FightEvent) && ScriptLoader.Event.TryGetValue(Scenario.FightEvent, out fight))
            {
                fList = new LinkedList<LexicalTree>(fight.Script);
                CollectData(fight.Script);
            }
            if (!string.IsNullOrEmpty(Scenario.WorldEvent) && ScriptLoader.Event.TryGetValue(Scenario.WorldEvent, out world))
            {
                wList = new LinkedList<LexicalTree>(world.Script);
                CollectData(world.Script);
            }
            var c = ScriptLoader.Story.Where(_ => _.Value.Friend.Contains(Scenario.Name) || _.Value.Friend.Count == 0).Select(_ => _.Value).GetEnumerator();
            while (c.MoveNext())
            {
                var tmp = c.Current;
                if (tmp.Script.Count == 0) continue;
                CollectData(tmp.Script);
                if (tmp.Pre.HasValue && tmp.Pre.Value)
                {
                    if (tmp.Fight.HasValue && tmp.Fight.Value)
                        if (fList == null)
                            fList = new LinkedList<LexicalTree>(tmp.Script);
                        else for (int i = tmp.Script.Count - 1; i >= 0; --i)
                                fList.AddFirst(tmp.Script[i]);
                    else if (tmp.Politics.HasValue && tmp.Politics.Value)
                    {
                        if (pList == null)
                            pList = new LinkedList<LexicalTree>(tmp.Script);
                        else
                            for (int i = tmp.Script.Count - 1; i >= 0; --i)
                                pList.AddFirst(tmp.Script[i]);
                    }
                    else if (wList == null)
                        wList = new LinkedList<LexicalTree>(tmp.Script);
                    else for (int i = tmp.Script.Count - 1; i >= 0; --i)
                            wList.AddFirst(tmp.Script[i]);
                }
                else if (tmp.Fight.HasValue && tmp.Fight.Value)
                {
                    if (fList == null)
                        fList = new LinkedList<LexicalTree>(tmp.Script);
                    else
                        for (int i = 0; i < tmp.Script.Count; i++)
                            fList.AddLast(tmp.Script[i]);
                }
                else if (tmp.Politics.HasValue && tmp.Politics.Value)
                {
                    if (pList == null)
                        pList = new LinkedList<LexicalTree>(tmp.Script);
                    else
                        for (int i = 0; i < tmp.Script.Count; i++)
                            pList.AddLast(tmp.Script[i]);
                }
                else if (wList == null)
                    wList = new LinkedList<LexicalTree>(tmp.Script);
                else for (int i = 0; i < tmp.Script.Count; i++)
                        wList.AddLast(tmp.Script[i]);
            }
            if (wList != null) World.AddRange(wList);
            if (pList != null) Politics.AddRange(pList);
            if (fList != null) Fight.AddRange(fList);
        }
        private void CollectData(List<LexicalTree> trees)
        {
            if (trees == null) return;
            for (int i = 0; i < trees.Count; i++)
                switch (trees[i].Type)
                {
                    case LexicalTree.TreeType.Assign:
                    case LexicalTree.TreeType.VariableParen:
                    case LexicalTree.TreeType.NoNameStructure:
                        continue;
                    case LexicalTree.TreeType.Block:
                    case LexicalTree.TreeType.NameStructure:
                        CollectData((trees[i] as LexicalTree_Block)?.Children);
                        break;
                    case LexicalTree.TreeType.BoolParen:
                        CollectData((trees[i] as LexicalTree_BoolParen));
                        break;
                    case LexicalTree.TreeType.Statement:
                        var s = trees[i] as LexicalTree_Statement;
                        if (s == null) continue;
                        CollectData(s.Children);
                        CollectData(s.Paren as LexicalTree_BoolParen);
                        break;
                    case LexicalTree.TreeType.Function:
                        var f = trees[i] as LexicalTree_Function;
                        if (f == null) continue;
                        CollectData(f.Name, f.Variable.Content);
                        break;
                }
        }
        private void CollectData(LexicalTree_BoolParen boolParen)
        {
            if (boolParen == null) return;
            CollectData(new InterpretTreeMachine(boolParen).Tree);
        }
        private void CollectData(Tree tree)
        {
            switch (tree.Type)
            {
                case 0:
                    if (tree.Children[0] == null) throw new Exception();
                    CollectData(tree.Children[0]);
                    CollectData(tree.Children[1]);
                    break;
                case 1:
                    var content = tree.Function.Variable.Content;
                    switch (tree.Function.Name)
                    {
                        case "yet":
                            content.ThrowException(1);
                            if (content.IsVariable(0)) Variable.Add(content[0].ToLowerString());
                            else if (!ScriptLoader.Event.ContainsKey(content[0].ToLowerString())) throw new Exception();
                            Yet.Add(content[0].ToLowerString());
                            break;
                        case "amount":
                        case "eqvar":
                            content.ThrowException(2);
                            content.AddVariable(0, Variable);
                            content.AddVariable(1, Variable);
                            break;
                        case "has":
                        case "inpower":
                        case "inspot":
                        case "inroamspot":
                            content.ThrowException(2, int.MaxValue);
                            for (int i = 0; i < content.Count; i++)
                                content.AddVariable_NotAddIdentifier(i, Variable);
                            break;
                        case "issamearmy":
                            content.ThrowException(2, int.MaxValue);
                            for (int i = 0; i < content.Count; i++)
                            {
                                var coni = content[i].ToLowerString();
                                if (content.IsVariable(i)) Variable.Add(coni);
                                else if (!ScriptLoader.Unit.ContainsKey(coni) && !ScriptLoader.GenericUnit.ContainsKey(coni)) throw new Exception();
                            }
                            break;
                        case "isalive":
                        case "isnpc":
                        case "inbattle":
                            content.ThrowException(1, int.MaxValue);
                            for (int i = 0; i < content.Count; i++)
                            {
                                var coni = content[i].ToLowerString();
                                if (content.IsVariable(i))
                                    Variable.Add(coni);
                                else if (!ScriptLoader.Power.ContainsKey(coni) && !ScriptLoader.Unit.ContainsKey(coni) && !ScriptLoader.GenericUnit.ContainsKey(coni))
                                    throw new Exception();
                            }
                            break;
                        case "isanydead":
                        case "isdead":
                            content.ThrowException(1, int.MaxValue);
                            for (int i = 0; i < content.Count; i++)
                            {
                                if (content.IsVariable(i)) Variable.Add(content[i].ToLowerString());
                                else if (!ScriptLoader.Unit.ContainsKey(content[i].ToLowerString())) throw new Exception();
                            }
                            break;
                        case "isplayer":
                        case "isscenario":
                        case "countmoney":
                        case "countgain":
                        case "countforce":
                        case "countspot":
                        case "isinvade":
                        case "isdone":
                        case "isarbeit":
                        case "isroamleader":
                        case "isroamer":
                        case "isvassal":
                        case "isleader":
                        case "ismaster":
                        case "istalent":
                        case "isenable":
                        case "getlife":
                        case "countunit":
                        case "isactive":
                        case "isalldead":
                        case "isnowspot":
                        case "count":
                        case "countv":
                        case "countvar":
                            content.ThrowException(1);
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            break;
                        case "ptest":
                        case "isenemy":
                        case "isfriend":
                        case "iswar":
                        case "isleague":
                            content.ThrowException(2);
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            content.AddVariable_NotAddIdentifier(1, Variable);
                            break;
                        case "iscomturn":
                            content.ThrowException(0, 1);
                            if (content.Count == 1)
                                content.AddVariable_NotAddIdentifier(0, Variable);
                            break;
                        case "isjoin":
                            content.ThrowException(2, 3);
                            if (content.Count == 3)
                                if (content[2].Content.ToLower() != "on") throw new Exception();
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            content.AddVariable_NotAddIdentifier(1, Variable);
                            break;
                        case "isnext":
                            content.ThrowException(2, 3);
                            if (content.Count == 3)
                                content.AddIdentifierOrNumber(2, Identifier);
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            content.AddVariable_NotAddIdentifier(1, Variable);
                            break;
                        case "reckon":
                        case "equal":
                            content.ThrowException(2);
                            content.AddVariable(0, Variable);
                            content.AddVariableOrIdentifier(1, Variable, Identifier);
                            break;
                        case "isdungeon":
                            content.ThrowException(1, 2);
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            if (content.Count == 2)
                                content.AddIdentifierOrNumber(1, Identifier);
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
                            content.ThrowException(0);
                            break;
                        case "isinterval":
                            content.ThrowException(1);
                            content.AddIdentifierOrNumber(0, Identifier);
                            break;
                        default: throw new Exception();
                    }
                    break;
                case 3:
                    Identifier.Add(tree.Token.Content.ToLower());
                    break;
                case 4:
                    Variable.Add(tree.Token.Content.ToLower());
                    break;
            }
        }
        private void CollectData(string name, List<Token> content)
        {
            switch (name)
            {
                case "storerectunit":
                    content.ThrowException(6);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    content.AddIdentifierOrNumber(2, Identifier);
                    content.AddIdentifierOrNumber(3, Identifier);
                    content.AddIdentifierOrNumber(4, Identifier);
                    content.AddVariable(5, Variable);
                    break;
                case "storepowerofforce":
                    content.ThrowException(2);
                    content.AddIdentifierOrNumber(0, Identifier);
                    content.AddVariable(1, Variable);
                    break;
                case "storenextspot":
                case "storetalentpower":
                case "storepowerofspot":
                case "storepowerofunit":
                case "storespotofpower":
                case "storespotofunit":
                case "storeleaderofpower":
                case "storeleaderofspot":
                case "storeunitofpower":
                case "storeunitofspot":
                case "storeroamunitofspot":
                case "storememberofunit":
                case "storemasterofpower":
                case "storealiveunit":
                case "storetodounit":
                case "storeskillofunit":
                case "storeclassofunit":
                case "storebaseclassofunit":
                case "storeraceofunit":
                case "storeskillset":
                case "storeud":
                    content.ThrowException(2);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable(1, Variable);
                    break;
                case "index":
                    content.ThrowException(3);
                    content.AddVariable(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    content.AddVariable(2, Variable);
                    break;
                case "pushbattlehome":
                case "pushbattlerect":
                    content.ThrowException(2);
                    content.AddIdentifier(0, Variable);
                    content.AddIdentifier(1, Variable);
                    break;
                case "pushdiplo":
                case "pushcon":
                case "pushstatus":
                    content.ThrowException(3);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    content.AddIdentifier(2, Variable);
                    break;
                case "pushspotpos":
                    content.ThrowException(3);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifier(1, Variable);
                    content.AddIdentifier(2, Variable);
                    break;
                case "pushlevel":
                case "pushmoney":
                case "pushloyal":
                case "pushmerits":
                case "pushsex":
                case "pushrank":
                case "pushitem":
                case "pushtrust":
                case "pushforce":
                case "pushspot":
                case "pushbaselevel":
                case "pushtrain":
                case "pushtrainup":
                case "pushgain":
                case "pushcastle":
                case "pushcapa":
                    content.ThrowException(2);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifier(1, Identifier);
                    break;
                case "pushv":
                case "pushvar":
                    content.ThrowException(2, 3);
                    if (content.Count == 3)
                    {
                        content.AddVariable_NotAddIdentifier(1, Variable);
                        content.AddIdentifier(2, Identifier);
                    }
                    else content.AddIdentifier(1, Identifier);
                    content.AddVariable(0, Variable);
                    break;
                case "hidelink":
                case "setpm":
                case "storepm":
                case "setud":
                    content.ThrowException(2);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    break;
                case "focus":
                    content.ThrowException(0, 1);
                    if (content.Count == 1)
                        content.AddVariable_NotAddIdentifier(0, Variable);
                    break;
                case "title":
                case "title2":
                    content.ThrowException(1, 2);
                    if (content.Count == 2)
                    {
                        content.AddVariableOrString(0, Variable);
                        content.AddIdentifierOrNumber(1, Identifier);
                    }
                    else content.AddIdentifierOrNumber(0, Identifier);
                    break;
                case "face":
                case "picture":
                case "image":
                case "showimage":
                case "showpict":
                case "showface":
                    content.ThrowException(1, 5);
                    content.AddVariableOrString(0, Variable);
                    if (content.Count > 1)
                        content.AddIdentifierOrNumber(1, Identifier);
                    if (content.Count > 2)
                        content.AddIdentifierOrNumber(2, Identifier);
                    if (content.Count > 3)
                        content.AddIdentifierOrNumber(3, Identifier);
                    if (content.Count > 4)
                        content.AddIdentifierOrNumber(4, Identifier);
                    break;
                case "setgameclear":
                    content.ThrowException(0, 1);
                    if (content.Count == 0) break;
                    if (!content.IsVariable(0) && content[0].ToLowerString() != "on") throw new Exception();
                    break;
                case "exit":
                    content.ThrowException(0, 2);
                    break;
                case "wait":
                    content.ThrowException(0, 1);
                    if (content.Count == 1)
                        content.AddIdentifierOrNumber(0, Identifier);
                    break;
                case "linkspot":
                case "linkescape":
                    content.ThrowException(2, 4);
                    if (content.Count == 4)
                        content.AddIdentifierOrNumber(3, Identifier);
                    else if (content.Count == 3)
                        if (content[2].Type == 2)
                            content.AddIdentifierOrNumber(2, Identifier);
                        else
                            content.AddVariable_NotAddIdentifier(2, Variable);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    content[0].IsSpot();
                    content[1].IsSpot();
                    break;
                case "showchara":
                    content.ThrowException(3, 5);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    content.AddIdentifierOrNumber(2, Identifier);
                    if (content.Count == 5)
                    {
                        content.AddIdentifierOrNumber(3, Identifier);
                        content.AddIdentifierOrNumber(4, Identifier);
                    }
                    break;
                case "scroll":
                case "scroll2":
                case "locate":
                    content.ThrowException(1, 2);
                    if (content.Count == 1)
                        content.AddVariable_NotAddIdentifier(0, Variable);
                    else
                    {
                        content.AddIdentifierOrNumber(0, Identifier);
                        content.AddIdentifierOrNumber(1, Identifier);
                    }
                    break;
                case "doskill":
                    content.ThrowException(5);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (!content.IsVariable(0) && !ScriptLoader.Skill.ContainsKey(content[0].ToLowerString()))
                        throw new Exception();
                    content.AddIdentifierOrNumber(1, Identifier);
                    content.AddIdentifierOrNumber(2, Identifier);
                    content.AddIdentifierOrNumber(3, Identifier);
                    switch (content[4].ToLowerString())
                    {
                        case "on":
                        case "off":
                            break;
                        default: throw new Exception();
                    }
                    break;
                case "showspot":
                case "hidespot":
                    content.ThrowException(1);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content[0].IsSpot();
                    break;
                case "spotmark":
                    content.ThrowException(0, 2);
                    if (content.Count > 0)
                        content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.Count == 2)
                        content.AddIdentifierOrNumber(1, Identifier);
                    break;
                case "gread":
                    content.ThrowException(2, 3);
                    if (!content.IsVariableOrIdentifier(0))
                        throw new Exception();
                    content.AddIdentifier(1, Identifier);
                    if (content.Count == 3)
                        content.AddVariable(2, Variable);
                    break;
                case "gwrite":
                    content.ThrowException(2, 3);
                    if (!content.IsVariableOrIdentifier(0))
                        throw new Exception();
                    content.AddIdentifierOrNumber(1, Identifier);
                    if (content.Count == 3)
                        content.AddVariableOrString(2, Variable);
                    break;
                case "setv":
                case "addv":
                case "subv":
                case "addstr":
                case "addint":
                    content.ThrowException(2);
                    content.AddVariable(0, Variable);
                    content.AddVariableOrString(1, Variable);
                    break;
                case "storeplayerunit":
                case "storeplayerpower":
                case "storeallpower":
                case "storecompower":
                case "storeallspot":
                case "storeneutralspot":
                case "storealltalent":
                case "storenowpower":
                case "clear":
                case "shuffle":
                case "storebattlespot":
                case "storeattackpower":
                case "storedefensepower":
                    content.ThrowException(1);
                    content.AddVariable(0, Variable);
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
                    content.AddIdentifier(0, Identifier);
                    content.AddIdentifierOrNumber(1, Identifier);
                    content.NotBoolIdentifier(NotBoolIdentifier, BoolIdentifier);
                    break;
                case "set":
                    content.ThrowException(2);
                    content.AddIdentifier(0, Identifier);
                    content.AddIdentifierOrNumber(1, Identifier);
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
                    content.AddIdentifier(0, Identifier);
                    break;
                case "break":
                case "continue":
                case "clearbattlerecord":
                case "return":
                case "erase":
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
                    if (content.Count == 4)
                    {
                        if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(0, Variable);
                        content.AddIdentifierOrNumber(1, Identifier);
                        content.AddIdentifierOrNumber(2, Identifier);
                        content.AddIdentifierOrNumber(3, Identifier);
                    }
                    else if (content.Count == 3) throw new Exception();
                    else
                    {
                        if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                        if (!content.IsVariable(1) && !ScriptLoader.Unit.ContainsKey(content[1].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[1].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(0, Variable);
                        content.AddVariable_NotAddIdentifier(1, Variable);
                    }
                    break;
                case "addtroop":
                    content.ThrowException(5);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    content.AddIdentifierOrNumber(2, Identifier);
                    content.AddIdentifierOrNumber(3, Identifier);
                    switch (content[4].ToLowerString())
                    {
                        case "red":
                        case "blue":
                            break;
                        default: throw new Exception();
                    }
                    break;
                case "ispostin":
                    content.ThrowException(3, 5);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.Count == 3)
                    {
                        content.AddVariable_NotAddIdentifier(1, Variable);
                        content.AddIdentifierOrNumber(2, Identifier);
                        break;
                    }
                    content.AddIdentifierOrNumber(1, Identifier);
                    content.AddIdentifierOrNumber(2, Identifier);
                    content.AddIdentifierOrNumber(3, Identifier);
                    if (content.Count == 5)
                        content.AddIdentifierOrNumber(4, Identifier);
                    break;
                case "getdistance":
                    content.ThrowException(2, 3);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.Count == 2)
                        content.AddVariable_NotAddIdentifier(1, Variable);
                    else
                    {
                        content.AddIdentifierOrNumber(1, Identifier);
                        content.AddIdentifierOrNumber(2, Identifier);
                    }
                    break;
                case "shadow":
                    if (content.Count == 0) break;
                    if (content.Count != 10) throw new Exception();
                    content.AddIdentifierOrNumber(0, Identifier);
                    content.AddIdentifierOrNumber(1, Identifier);
                    content.AddIdentifierOrNumber(2, Identifier);
                    content.AddIdentifierOrNumber(3, Identifier);
                    content.AddIdentifierOrNumber(4, Identifier);
                    content.AddIdentifierOrNumber(5, Identifier);
                    content.AddIdentifierOrNumber(6, Identifier);
                    content.AddIdentifierOrNumber(7, Identifier);
                    content.AddIdentifierOrNumber(8, Identifier);
                    content.AddIdentifierOrNumber(9, Identifier);
                    break;
                case "darkness":
                    content.ThrowException(0, 4);
                    if (content.Count > 0)
                        content.AddIdentifierOrNumber(0, Identifier);
                    if (content.Count > 1)
                        content.AddIdentifierOrNumber(1, Identifier);
                    if (content.Count > 2)
                        content.AddIdentifierOrNumber(2, Identifier);
                    if (content.Count > 3)
                        content.AddIdentifierOrNumber(3, Identifier);
                    break;
                case "loopbgm":
                    content.ThrowException(1, int.MaxValue);
                    for (int i = 0; i < content.Count; i++)
                        content.AddVariableOrString(i, Variable);
                    break;
                case "resetleague":
                    content.ThrowException(1, 3);
                    if (content.Count == 1) content.AddVariable(0, Variable);
                    else if (content.Count == 2)
                    {
                        if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                        if (!content.IsVariable(1) && !ScriptLoader.Power.ContainsKey(content[1].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(0, Variable);
                        content.AddVariable_NotAddIdentifier(1, Variable);
                    }
                    else
                    {
                        if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                        if (!content.IsVariable(1) && !ScriptLoader.Power.ContainsKey(content[1].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(0, Variable);
                        content.AddVariable_NotAddIdentifier(1, Variable);
                        content.AddIdentifierOrNumber(2, Identifier);
                    }
                    break;
                case "resetenemypower":
                    content.ThrowException(1, 2);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.Count == 2)
                    {
                        if (!content.IsVariable(1) && !ScriptLoader.Power.ContainsKey(content[1].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(1, Variable);
                    }
                    break;
                case "setenemypower":
                    content.ThrowException(3);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    if (!content.IsVariable(1) && !ScriptLoader.Power.ContainsKey(content[1].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    content.AddIdentifierOrNumber(2, Identifier);
                    break;
                case "setdiplo":
                case "adddiplo":
                case "settruce":
                case "setleague":
                    content.ThrowException(2, 3);
                    if (content.Count == 2)
                    {
                        content.AddVariable(0, Variable);
                        content.AddIdentifierOrNumber(1, Identifier);
                    }
                    else
                    {
                        if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                        if (!content.IsVariable(1) && !ScriptLoader.Power.ContainsKey(content[1].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(0, Variable);
                        content.AddVariable_NotAddIdentifier(1, Variable);
                        content.AddIdentifierOrNumber(2, Identifier);
                    }
                    break;
                case "addunit":
                    content.ThrowException(2, 3);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.Count == 2)
                    {
                        if (!content.IsVariable(1))
                        {
                            var cont1 = content[1].ToLowerString();
                            if (!ScriptLoader.Unit.ContainsKey(cont1) && !ScriptLoader.Spot.ContainsKey(cont1) && !ScriptLoader.Power.ContainsKey(cont1)) throw new Exception();
                        }
                        content.AddVariable_NotAddIdentifier(1, Variable);
                    }
                    else
                    {
                        if (!content.IsVariable(1) && !ScriptLoader.Spot.ContainsKey(content[1].ToLowerString()))
                            throw new Exception();
                        content.AddVariable_NotAddIdentifier(1, Variable);
                        if (!content.IsVariable(2) && content[2].ToLowerString() != "roam") throw new Exception();
                    }
                    break;
                case "eraseunit2":
                    content.ThrowException(2, int.MaxValue);
                    if (!content.IsVariable(0) && !ScriptLoader.Spot.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    for (int i = 1; i < content.Count; i++)
                    {
                        if (!content.IsVariable(i) && !ScriptLoader.GenericUnit.ContainsKey(content[i].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(i, Variable);
                    }
                    break;
                case "roamunit":
                    content.ThrowException(1);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    break;
                case "unionpower":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    if (!content.IsVariable(1) && !ScriptLoader.Power.ContainsKey(content[1].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    break;
                case "settraining":
                case "addtraining":
                case "settrainingup":
                case "addtrainingup":
                case "setbaselevel":
                case "addbaselevel":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    break;
                case "addpowermerce2":
                    content.ThrowException(2, int.MaxValue);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    for (int i = 1; i < content.Count; i++)
                        if (content.IsVariable(i)) Variable.Add(content[i].Content.ToLower());
                        else if (!ScriptLoader.GenericUnit.ContainsKey(content[i].ToLowerString())) throw new Exception();
                    break;
                case "addpowerstaff2":
                    content.ThrowException(2, int.MaxValue);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    for (int i = 1; i < content.Count; i++)
                        if (content.IsVariable(i)) Variable.Add(content[i].Content.ToLower());
                        else if (!ScriptLoader.GenericUnit.ContainsKey(content[i].ToLowerString()) && !ScriptLoader.Race.ContainsKey(content[i].ToLowerString())) throw new Exception();
                    break;
                case "changepowerfix":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.IsVariable(1)) Variable.Add(content[1].ToLowerString());
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
                                throw new Exception();
                        }
                    break;
                case "changepowername":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.IsVariable(1)) Variable.Add(content[1].Content.ToLower());
                    break;
                case "changepowerflag":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (!content.IsVariable(1) && !ScriptLoader.Folder.Flag_Bmp.Any(_ => System.IO.Path.GetFileNameWithoutExtension(_) == content[1].Content) && !ScriptLoader.Folder.Flag_Png.Any(_ => System.IO.Path.GetFileNameWithoutExtension(_) == content[1].Content) && !ScriptLoader.Folder.Flag_Jpg.Any(_ => System.IO.Path.GetFileNameWithoutExtension(_) == content[1].Content))
                        throw new Exception();
                    else Variable.Add(content[1].ToLowerString());
                    break;
                case "changemaster":
                    content.ThrowException(1, 3);
                    switch (content.Count)
                    {
                        case 1:
                            if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            break;
                        case 2:
                            if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                            if (!content.IsVariable(1) && !ScriptLoader.Unit.ContainsKey(content[1].ToLowerString())) throw new Exception();
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            content.AddVariable_NotAddIdentifier(1, Variable);
                            break;
                        case 3:
                            if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                            if (!content.IsVariable(1) && !ScriptLoader.Unit.ContainsKey(content[1].ToLowerString())) throw new Exception();
                            if (!content.IsVariable(2) && content[2].ToLowerString() != "flag") throw new Exception();
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            content.AddVariable_NotAddIdentifier(1, Variable);
                            content.AddVariable_NotAddIdentifier(2, Variable);
                            break;
                    }
                    break;
                case "changeplayer":
                    content.ThrowException(0, 1);
                    if (content.Count == 0) break;
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    break;
                case "eraseunit":
                case "eraseunittroop":
                case "roamtroop":
                    content.ThrowException(1);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    break;
                case "shifttroop":
                case "shifttroop2":
                    content.ThrowException(3, 4);
                    if (content.Count == 4)
                    {
                        if (content.IsVariable(3)) Variable.Add(content[3].ToLowerString());
                        else if (content[3].ToLowerString() != "on") throw new Exception();
                    }
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    content.AddIdentifierOrNumber(2, Identifier);
                    break;
                case "skilltroop":
                    content.ThrowException(2, 3);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.Count == 2)
                    {
                        if (content.IsVariable(1)) { Variable.Add(content[1].ToLowerString()); break; }
                        if (!ScriptLoader.Skill.ContainsKey(content[1].ToLowerString()) && !ScriptLoader.Unit.ContainsKey(content[1].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[1].ToLowerString())) throw new Exception();
                    }
                    else if (content.IsIdentifierOrNumber(1))
                    {
                        content.AddIdentifierOrNumber(1, Identifier);
                        content.AddIdentifierOrNumber(2, Identifier);
                    }
                    else if (content.IsVariable(2)) Variable.Add(content[2].Content.ToLower());
                    else switch (content[2].ToLowerString())
                        {
                            case "on":
                            case "dead_ok":
                                break;
                            default: throw new Exception();
                        }
                    break;
                case "formtroop":
                case "speedtroop":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
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
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.GenericUnit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    break;
                case "setgain":
                case "setcastle":
                case "setcapa":
                case "addgain":
                case "addcastle":
                case "addcapa":
                case "changecastle":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Spot.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    break;
                case "changemap":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Spot.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    if (!content.IsVariable(1) && !ScriptLoader.Folder.Stage_Map.Any((_) => System.IO.Path.GetFileNameWithoutExtension(_).ToLower() == content[1].Content.ToLower()))
                        throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    break;
                case "addpower":
                case "erasepower":
                case "erasepowermerce":
                case "erasepowerstaff":
                    content.ThrowException(1);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    break;
                case "addspot":
                    content.ThrowException(1, 2);
                    if (!content.IsVariable(0) && !ScriptLoader.Spot.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.Count == 2)
                    {
                        if (!content.IsVariable(1) && !ScriptLoader.Power.ContainsKey(content[1].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(1, Variable);
                    }
                    break;
                case "removespot":
                    content.ThrowException(1);
                    if (!content.IsVariable(0) && !ScriptLoader.Spot.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    break;
                case "setarbeit":
                    content.ThrowException(3);
                    if (!content.IsVariable(0) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    if (!content.IsVariable(1) && !ScriptLoader.Unit.ContainsKey(content[1].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    content.AddIdentifierOrNumber(2, Identifier);
                    break;
                case "scrollspeed":
                case "zoom":
                case "volume":
                case "setlimit":
                case "addlimit":
                    content.ThrowException(1);
                    content.AddIdentifierOrNumber(0, Identifier);
                    break;
                case "fadeout":
                case "fadein":
                case "shake":
                    content.ThrowException(0, 1);
                    if (content.Count == 1)
                        content.AddIdentifierOrNumber(0, Identifier);
                    break;
                case "playbgm":
                    content.ThrowException(0, 1);
                    if (content.Count == 0) break;
                    content.AddVariableOrString(0, Variable);
                    break;
                case "playese":
                    content.ThrowException(1);
                    content.AddVariableOrString(0, Variable);
                    break;
                case "showpolitics":
                case "showparty":
                    content.ThrowException(1);
                    switch (content[0].ToLowerString())
                    {
                        case "on":
                        case "off":
                            break;
                        default:
                            if (!content.IsVariable(0)) throw new Exception();
                            break;
                    }
                    break;
                case "changespotimage":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Spot.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    break;
                case "changedungeon":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Spot.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    if (!content.IsVariable(1) && !ScriptLoader.Dungeon.ContainsKey(content[1].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    break;
                case "setdungeonfloor":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Dungeon.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    break;
                case "setmoney":
                case "addmoney":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString()) && !ScriptLoader.Power.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    break;
                case "changeclass":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    if (!content.IsVariable(1) && !ScriptLoader.GenericUnit.ContainsKey(content[1].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    break;
                case "eraseskill":
                    content.ThrowException(1, int.MaxValue);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    for (int i = 1; i < content.Count; i++)
                    {
                        if (!content.IsVariable(i) && !ScriptLoader.Skill.ContainsKey(content[i].ToLowerString()) && !ScriptLoader.SkillSet.ContainsKey(content[1].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(i, Identifier);
                    }
                    break;
                case "addskill":
                case "addskill2":
                case "removeskill":
                    content.ThrowException(2, int.MaxValue);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    for (int i = 1; i < content.Count; i++)
                    {
                        if (!content.IsVariable(i) && !ScriptLoader.Skill.ContainsKey(content[i].ToLowerString()) && !ScriptLoader.SkillSet.ContainsKey(content[1].ToLowerString())) throw new Exception();
                        content.AddVariable_NotAddIdentifier(i, Identifier);
                    }
                    break;
                case "addstatus":
                case "setstatus":
                    content.ThrowException(3);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
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
                            default: throw new Exception();
                        }
                    content.AddIdentifierOrNumber(2, Identifier);
                    break;
                case "hidechara":
                case "reversechara":
                case "showdungeon":
                    content.ThrowException(1);
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    break;
                case "equipitem":
                    content.ThrowException(2, 3);
                    if (content.Count == 3)
                        if (content.IsVariable(2))
                        { if (content[2].ToLowerString() != "on") throw new Exception(); }
                        else Variable.Add(content[2].ToLowerString());
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.IsVariable(1)) Variable.Add(content[1].ToLowerString());
                    else
                    {
                        SkillData tmpSkill;
                        if (ScriptLoader.Skill.TryGetValue(content[1].ToLowerString(), out tmpSkill))
                        { if (tmpSkill.ItemType == null) throw new Exception(); }
                        else throw new Exception();
                    }
                    break;
                case "entryitem":
                case "exititem":
                case "additem":
                case "eraseitem":
                    content.ThrowException(1);
                    if (content.IsVariable(0))
                        Variable.Add(content[0].ToLowerString());
                    else
                    {
                        SkillData tmpSkill;
                        if (ScriptLoader.Skill.TryGetValue(content[0].ToLowerString(), out tmpSkill))
                        { if (tmpSkill.ItemType == null) throw new Exception(); }
                        else throw new Exception();
                    }
                    break;
                case "erasefriend":
                    content.ThrowException(1, int.MaxValue);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    for (int i = 1; i < content.Count; i++)
                    {
                        var coni = content[i].ToLowerString();
                        if (content.IsVariable(i))
                            Variable.Add(coni);
                        else if (!ScriptLoader.Unit.ContainsKey(coni) && !ScriptLoader.GenericUnit.ContainsKey(coni) && !ScriptLoader.Race.ContainsKey(coni))
                            throw new Exception();
                    }
                    break;
                case "addfriend":
                    content.ThrowException(2, int.MaxValue);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    for (int i = 1; i < content.Count; i++)
                    {
                        var coni = content[i].ToLowerString();
                        if (content.IsVariable(i))
                            Variable.Add(coni);
                        else if (!ScriptLoader.Unit.ContainsKey(coni) && !ScriptLoader.GenericUnit.ContainsKey(coni) && !ScriptLoader.Race.ContainsKey(coni))
                            throw new Exception();
                    }
                    break;
                case "changerace":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    if (!content.IsVariable(1) && !ScriptLoader.Race.ContainsKey(content[1].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddVariable_NotAddIdentifier(1, Variable);
                    break;
                case "setdone":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    if (content.IsVariable(1)) Variable.Add(content[1].ToLowerString());
                    switch (content[1].Content.ToLower())
                    {
                        case "on":
                        case "off":
                            break;
                        default: throw new Exception();
                    }
                    break;
                case "addloyal":
                case "addmerits":
                case "addtrust":
                case "addlevel":
                case "setlevel":
                    content.ThrowException(2);
                    if (!content.IsVariable(0) && !ScriptLoader.Unit.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    content.AddVariable_NotAddIdentifier(0, Variable);
                    content.AddIdentifierOrNumber(1, Identifier);
                    break;
                case "bg":
                case "bcg":
                    content.ThrowException(1, 2);
                    if (content.Count == 2)
                        if (content[1].ToLowerString() != "on") throw new Exception();
                    content.AddVariableOrString(0, Variable);
                    break;
                case "choice":
                    content.ThrowException(1, int.MaxValue);
                    content.AddIdentifier(0, Identifier);
                    break;
                case "choicetitle":
                    content.ThrowException(0, 1);
                    if (content.Count == 1)
                        content.AddVariableOrString(0, Variable);
                    break;
                case "select":
                    content.ThrowException(2);
                    content.AddIdentifier(0, Identifier);
                    break;
                case "dialog":
                    content.ThrowException(1, 2);
                    if (content.Count == 2)
                    {
                        content.AddVariableOrString(1, Variable);
                        content.AddVariable_NotAddIdentifier(0, Variable);
                    }
                    else content.AddVariableOrString(0, Variable);
                    break;
                case "fontc":
                    content.ThrowException(0, 3);
                    if (content.Count == 3)
                    {
                        content.AddIdentifierOrNumber(0, Identifier);
                        content.AddIdentifierOrNumber(1, Identifier);
                        content.AddIdentifierOrNumber(2, Identifier);
                    }
                    break;
                case "font":
                    content.ThrowException(0, 3);
                    if (content.Count == 3)
                    {
                        content.AddVariableOrString(0, Variable);
                        content.AddIdentifierOrNumber(1, Identifier);
                        content.AddIdentifierOrNumber(2, Identifier);
                    }
                    break;
                case "msg":
                case "msg2":
                    content.ThrowException(1, 3);
                    switch (content.Count)
                    {
                        case 1:
                            content.AddVariableOrString(0, Variable);
                            break;
                        case 2:
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            content.AddVariableOrString(1, Variable);
                            break;
                        case 3:
                            content.AddVariable_NotAddIdentifier(0, Variable);
                            content.AddVariableOrString(1, Variable);
                            content.AddVariableOrString(2, Variable);
                            break;
                    }
                    break;
                case "battleevent":
                    content.ThrowException(1);
                    var coni_routine = content[0].ToLowerString();
                    if (content.IsVariable(0)) Variable.Add(content[0].ToLowerString());
                    else if (!ScriptLoader.Event.ContainsKey(content[0].ToLowerString())) throw new Exception();
                    BattleEvent.Add(content[0].ToLowerString());
                    break;
                case "event":
                    content.ThrowException(1);
                    coni_routine = content[0].ToLowerString();
                    if (content.IsVariable(0)) Variable.Add(content[0].ToLowerString());
                    else if (!ScriptLoader.Event.ContainsKey(coni_routine) && coni_routine != "world_bgm" && coni_routine != "count") throw new Exception();
                    Event.Add(content[0].ToLowerString());
                    break;
                case "routine":
                    content.ThrowException(1);
                    coni_routine = content[0].ToLowerString();
                    if (content.IsVariable(0)) Variable.Add(coni_routine);
                    else if (!ScriptLoader.Event.ContainsKey(coni_routine) && coni_routine != "world_bgm" && coni_routine != "count") throw new Exception();
                    Routine.Add(content[0].ToLowerString());
                    break;
                    //未だ追加しきれていない関数を調べる際にはコメントインしてください。
                    //default: throw new Exception();
            }
        }
    }
}