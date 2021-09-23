namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct FunctionInfo
{
    public string Name;
    public int Min;
    public int Max;
    public Dictionary<int, ArgumentInfo[]> Dictionary;

    public FunctionInfo(string name, int min, int max, Dictionary<int, ArgumentInfo[]>? dictionary = null)
    {
        Name = name;
        Min = min;
        Max = max;
        Dictionary = dictionary ?? new();
    }

    public void Deconstruct(out string name, out int min, out int max)
    {
        name = Name;
        min = Min;
        max = Max;
    }

    public void Deconstruct(out string name, out int min, out int max, out Dictionary<int, ArgumentInfo[]> dictionary)
    {
        name = Name;
        min = Min;
        max = Max;
        dictionary = Dictionary;
    }

    public static readonly FunctionInfo[] FunctionInfoArray = new FunctionInfo[]
    {
        new("has", 2, int.MaxValue),
        new("yet", 1, 1),
        new("rand", 0, 0),
        new("count", 1, 1),
        new("equal", 2, 2),
        new("eqVar", 0, int.MaxValue),
        new("inVar", 0, int.MaxValue),
        new("isMap", 0, 0),
        new("isNpc", 1, int.MaxValue),
        new("isNPM", 0, 0),
        new("isWar", 2, 2),
        new("ptest", 2, 2),
        new("amount", 0, int.MaxValue),
        new("conVar", 0, int.MaxValue),
        new("inSpot", 2, int.MaxValue),
        new("isDead", 1, int.MaxValue),
        new("isDone", 1, 1),
        new("isJoin", 2, 3),
        new("isNext", 2, 3),
        new("reckon", 2, 2),
        new("getLife", 1, 1),
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
        new("isArbeit", 1, 1),
        new("isEnable", 1, 1),
        new("isFriend", 2, 2),
        new("isInvade", 1, 1),
        new("isLeader", 1, 1),
        new("isLeague", 2, 2),
        new("isMaster", 1, 1),
        new("isPlayer", 1, 1),
        new("isPostIn", 3, 5),
        new("isRoamer", 1, 1),
        new("isSelect", 0, int.MaxValue),
        new("isTalent", 1, 1),
        new("isVassal", 1, 1),
        new("countGain", 1, 1),
        new("countPost", 0, int.MaxValue),
        new("countSpot", 1, 1),
        new("countUnit", 1, 1),
        new("isAllDead", 1, 1),
        new("isAnyDead", 2, int.MaxValue),
        new("isComTurn", 0, 1),
        new("isDungeon", 0, 2),
        new("isNewTurn", 0, 0),
        new("isNowSpot", 1, 1),
        new("istoWorld", 0, 0),
        new("isWhoDead", 0, int.MaxValue),
        new("countForce", 1, 1),
        new("countMoney", 1, 1),
        new("countPower", 0, 0),
        new("countSkill", 0, int.MaxValue),
        new("getLifePer", 0, int.MaxValue),
        new("inRoamSpot", 2, int.MaxValue),
        new("isGameOver", 0, int.MaxValue),
        new("isInterval", 1, 1),
        new("isRedAlive", 0, 0),
        new("isSameArmy", 2, 2),
        new("isScenario", 1, 1),
        new("isWatching", 0, 0),
        new("getDistance", 2, 3),
        new("getRedCount", 0, 0),
        new("isBlueAlive", 0, 0),
        new("isGameClear", 0, 0),
        new("isPlayerEnd", 0, 0),
        new("getBlueCount", 0, 0),
        new("isPlayerTurn", 0, 0),
        new("isRoamLeader", 1, 1),
        new("getClearFloor", 1, 1),
        new("isWorldMusicStop", 0, 0),
    };
}
