namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct FunctionInfo
{
    public string Name;
    public int Min;
    public int Max;
    public ReferenceKind[] ReferenceKinds;

    public FunctionInfo(string name, int min, int max)
    {
        Name = name;
        Min = min;
        Max = max;
        ReferenceKinds = System.Array.Empty<ReferenceKind>();
    }

    public FunctionInfo(string name, int min, int max, params ReferenceKind[] referenceKinds)
    {
        Name = name;
        Min = min;
        Max = max;
        ReferenceKinds = referenceKinds;
    }

    public void Deconstruct(out string name, out int min, out int max)
    {
        name = Name;
        min = Min;
        max = Max;
    }

    public static readonly FunctionInfo[] FunctionInfoArray = new FunctionInfo[]
    {
        new("has", 2, int.MaxValue),
        new("yet", 1, 1, ReferenceKind.Event),
        new("rand", 0, 0),
        new("count", 1, 1, ReferenceKind.StringVariableReader),
        new("equal", 2, 2),
        new("eqVar", 0, int.MaxValue),
        new("inVar", 0, int.MaxValue),
        new("isMap", 0, 0),
        new("isNpc", 1, int.MaxValue),
        new("isNPM", 0, 0),
        new("isWar", 2, 2, ReferenceKind.Power, ReferenceKind.Power),
        new("ptest", 2, 2),
        new("amount", 0, int.MaxValue),
        new("conVar", 0, int.MaxValue),
        new("inSpot", 2, int.MaxValue),
        new("isDead", 1, int.MaxValue),
        new("isDone", 1, 1, ReferenceKind.Unit | ReferenceKind.StringVariableReader),
        new("isJoin", 2, 3),
        new("isNext", 2, 3),
        new("reckon", 2, 2),
        new("getLife", 1, 1, ReferenceKind.Unit | ReferenceKind.StringVariableReader),
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
        new("isArbeit", 1, 1, ReferenceKind.Unit | ReferenceKind.StringVariableReader),
        new("isEnable", 1, 1),
        new("isFriend", 2, 2),
        new("isInvade", 1, 1, ReferenceKind.Power | ReferenceKind.StringVariableReader),
        new("isLeader", 1, 1),
        new("isLeague", 2, 2),
        new("isMaster", 1, 1, ReferenceKind.Unit | ReferenceKind.StringVariableReader),
        new("isPlayer", 1, 1, ReferenceKind.Unit | ReferenceKind.StringVariableReader),
        new("isPostIn", 3, 5),
        new("isRoamer", 1, 1, ReferenceKind.Unit | ReferenceKind.StringVariableReader),
        new("isSelect", 0, int.MaxValue),
        new("isTalent", 1, 1, ReferenceKind.Unit | ReferenceKind.StringVariableReader),
        new("isVassal", 1, 1),
        new("countGain", 1, 1, ReferenceKind.Power | ReferenceKind.StringVariableReader),
        new("countPost", 0, int.MaxValue),
        new("countSpot", 1, 1, ReferenceKind.Power | ReferenceKind.StringVariableReader),
        new("countUnit", 1, 1, ReferenceKind.Power | ReferenceKind.StringVariableReader),
        new("isAllDead", 1, 1),
        new("isAnyDead", 2, int.MaxValue),
        new("isComTurn", 0, 1, ReferenceKind.Power | ReferenceKind.StringVariableReader),
        new("isDungeon", 0, 2),
        new("isNewTurn", 0, 0),
        new("isNowSpot", 1, 1),
        new("istoWorld", 0, 0),
        new("isWhoDead", 0, int.MaxValue),
        new("countForce", 1, 1, ReferenceKind.Power | ReferenceKind.StringVariableReader),
        new("countMoney", 1, 1, ReferenceKind.Power | ReferenceKind.StringVariableReader),
        new("countPower", 0, 0),
        new("countSkill", 0, int.MaxValue),
        new("getLifePer", 0, int.MaxValue),
        new("inRoamSpot", 2, int.MaxValue),
        new("isGameOver", 0, int.MaxValue),
        new("isInterval", 1, 1, ReferenceKind.Number | ReferenceKind.NumberVariableReader),
        new("isRedAlive", 0, 0),
        new("isSameArmy", 2, 2, ReferenceKind.Unit | ReferenceKind.StringVariableReader),
        new("isScenario", 1, 1, ReferenceKind.Scenario),
        new("isWatching", 0, 0),
        new("getDistance", 2, 3),
        new("getRedCount", 0, 0),
        new("isBlueAlive", 0, 0),
        new("isGameClear", 0, 0),
        new("isPlayerEnd", 0, 0),
        new("getBlueCount", 0, 0),
        new("isPlayerTurn", 0, 0),
        new("isRoamLeader", 1, 1, ReferenceKind.Unit | ReferenceKind.StringVariableReader),
        new("getClearFloor", 1, 1),
        new("isWorldMusicStop", 0, 0),
    };
}
