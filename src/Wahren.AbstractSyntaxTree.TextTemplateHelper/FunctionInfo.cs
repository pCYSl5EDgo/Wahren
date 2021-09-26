namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct FunctionInfo
{
    public string Name;
    public int Min;
    public int Max;
    public SortedDictionary<int, ArgumentInfo[]> Dictionary;

    public FunctionInfo(string name, int min, int max)
    {
        Name = name;
        Min = min;
        Max = max;
        Dictionary = new();
    }

    public FunctionInfo(string name, int min, int max, ArgumentInfo argument0)
    {
        Name = name;
        Min = min;
        Max = max;
        Dictionary = new();
        Dictionary.Add(1, new ArgumentInfo[] { argument0 });
    }

    public FunctionInfo(string name, int min, int max, ArgumentInfo argument0, ArgumentInfo argument1)
    {
        Name = name;
        Min = min;
        Max = max;
        Dictionary = new();
        Dictionary.Add(2, new ArgumentInfo[] { argument0, argument1 });
    }

    public void Deconstruct(out string name, out int min, out int max)
    {
        name = Name;
        min = Min;
        max = Max;
    }

    public void Deconstruct(out string name, out int min, out int max, out SortedDictionary<int, ArgumentInfo[]> dictionary)
    {
        name = Name;
        min = Min;
        max = Max;
        dictionary = Dictionary;
    }

    public static readonly FunctionInfo[] FunctionInfoArray = new FunctionInfo[]
    {
        new("has", 2, int.MaxValue),
        new("yet", 1, 1, new(ReferenceKind.Event)),
        new("rand", 0, 0),
        new("count", 1, 1, new(ReferenceKind.StringVariable)),
        new("equal", 2, 2),
        new("eqVar", 0, int.MaxValue),
        new("inVar", 0, int.MaxValue),
        new("isMap", 0, 0),
        new("isNpc", 1, int.MaxValue),
        new("isNPM", 0, 0),
        new("isWar", 2, 2, new(ReferenceKind.Power), new(ReferenceKind.Power)),
        new("ptest", 2, 2),
        new("amount", 0, int.MaxValue),
        new("conVar", 0, int.MaxValue),
        new("inSpot", 2, int.MaxValue),
        new("isDead", 1, int.MaxValue),
        new("isDone", 1, 1, new(ReferenceKind.Unit | ReferenceKind.StringVariable)),
        new("isJoin", 2, 3),
        new("isNext", 2, 3),
        new("reckon", 2, 2),
        new("getLife", 1, 1, new(ReferenceKind.Unit | ReferenceKind.StringVariable)),
        new("getMode", 0, 0),
        new("getTime", 0, 0),
        new("getTurn", 0, 0),
        new("inPower", 2, int.MaxValue),
        new("isAlive", 1, int.MaxValue),
        new("isEnemy", 2, 2),
        new("isEvent", 0, 0),
        new("isPeace", 0, 0),
        new("isWorld", 0, 0),
        new("countVar", 1, 1),
        new("getLimit", 0, 0),
        new("inBattle", 1, int.MaxValue),
        new("isActive", 1, 1),
        new("isArbeit", 1, 1, new(ReferenceKind.Unit | ReferenceKind.StringVariable)),
        new("isEnable", 1, 1),
        new("isFriend", 2, 2),
        new("isInvade", 1, 1, new(ReferenceKind.Power | ReferenceKind.StringVariable)),
        new("isLeader", 1, 1),
        new("isLeague", 2, 2),
        new("isMaster", 1, 1, new(ReferenceKind.Unit | ReferenceKind.StringVariable)),
        new("isPlayer", 1, 1, new(ReferenceKind.Unit | ReferenceKind.StringVariable)),
        new("isPostIn", 3, 5),
        new("isRoamer", 1, 1, new(ReferenceKind.Unit | ReferenceKind.StringVariable)),
        new("isSelect", 0, int.MaxValue),
        new("isTalent", 1, 1, new(ReferenceKind.Unit | ReferenceKind.StringVariable)),
        new("isVassal", 1, 1),
        new("countGain", 1, 1, new(ReferenceKind.Power | ReferenceKind.StringVariable)),
        new("countPost", 0, int.MaxValue),
        new("countSpot", 1, 1, new(ReferenceKind.Power | ReferenceKind.StringVariable)),
        new("countUnit", 1, 1, new(ReferenceKind.Power | ReferenceKind.StringVariable)),
        new("isAllDead", 1, 1),
        new("isAnyDead", 2, int.MaxValue),
        new("isComTurn", 0, 1, new(ReferenceKind.Power | ReferenceKind.StringVariable)),
        new("isDungeon", 0, 2),
        new("isNewTurn", 0, 0),
        new("isNowSpot", 1, 1),
        new("istoWorld", 0, 0),
        new("isWhoDead", 0, int.MaxValue),
        new("countForce", 1, 1, new(ReferenceKind.Power | ReferenceKind.StringVariable)),
        new("countMoney", 1, 1, new(ReferenceKind.Power | ReferenceKind.StringVariable)),
        new("countPower", 0, 0),
        new("countSkill", 0, int.MaxValue),
        new("getLifePer", 0, int.MaxValue),
        new("inRoamSpot", 2, int.MaxValue),
        new("isGameOver", 0, int.MaxValue),
        new("isInterval", 1, 1, new(ReferenceKind.Number | ReferenceKind.NumberVariable)),
        new("isRedAlive", 0, 0),
        new("isSameArmy", 2, 2, new(ReferenceKind.Unit | ReferenceKind.StringVariable)),
        new("isScenario", 1, 1, new(ReferenceKind.Scenario)),
        new("isWatching", 0, 0),
        new("getDistance", 2, 3),
        new("getRedCount", 0, 0),
        new("isBlueAlive", 0, 0),
        new("isGameClear", 0, 0),
        new("isPlayerEnd", 0, 0),
        new("getBlueCount", 0, 0),
        new("isPlayerTurn", 0, 0),
        new("isRoamLeader", 1, 1, new(ReferenceKind.Unit | ReferenceKind.StringVariable)),
        new("getClearFloor", 1, 1),
        new("isWorldMusicStop", 0, 0),
    };
}
