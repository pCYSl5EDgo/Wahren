﻿namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct FunctionInfo
{
    public string Name;
    public int Min;
    public int Max;
    public ReferenceKind[] KindArray;

    public FunctionInfo(string name, int min, int max)
    {
        Name = name;
        Min = min;
        Max = max;
        KindArray = System.Array.Empty<ReferenceKind>();
    }

    public FunctionInfo(string name, int min, int max, params ReferenceKind[] kinds)
    {
        Name = name;
        Min = min;
        Max = max;
        KindArray = kinds;
    }

    public void Deconstruct(out string name, out int min, out int max)
    {
        name = Name;
        min = Min;
        max = Max;
    }

    private const ReferenceKind NumReader = ReferenceKind.NumberVariableReader | ReferenceKind.Number;
    private const ReferenceKind SPow = ReferenceKind.StringVariableReader | ReferenceKind.Power;
    private const ReferenceKind SUni = ReferenceKind.StringVariableReader | ReferenceKind.Unit;
    private const ReferenceKind SPowUni = SPow | ReferenceKind.Unit;
    private const ReferenceKind SSpoUni = SUni | ReferenceKind.Spot;
    private const ReferenceKind SSpo = ReferenceKind.StringVariableReader | ReferenceKind.Spot;

    public static readonly FunctionInfo[] FunctionInfoArray = new FunctionInfo[]
    {
        new("isSelect", 0, int.MaxValue),
        new("isWhoDead", 0, int.MaxValue),
        new("isGameOver", 0, int.MaxValue),
        new("has", 2, int.MaxValue, ReferenceKind.StringVariableReader, ReferenceKind.StringVariableReader | ReferenceKind.Text),
        new("inVar", 2, int.MaxValue, ReferenceKind.StringVariableReader, ReferenceKind.StringVariableReader | ReferenceKind.Text),
        new("yet", 1, 1, ReferenceKind.Event),
        new("rand", 0, 0),
        new("count", 1, 1, ReferenceKind.StringVariableReader),
        new("amount", 1, 1, ReferenceKind.StringVariableReader),
        new("equal", 2, 2, ReferenceKind.StringVariableReader, ReferenceKind.StringVariableReader | ReferenceKind.Text),
        new("eqVar", 2, 2, ReferenceKind.StringVariableReader, ReferenceKind.StringVariableReader | ReferenceKind.Text),
        new("isMap", 0, 0),
        new("isNpc", 1, int.MaxValue, SPowUni),
        new("isNPM", 0, 0),
        new("isWar", 2, 2, SPow, ReferenceKind.Power),
        new("ptest", 2, 2, ReferenceKind.Spot, ReferenceKind.Unit),
        new("conVar", 1, 1, ReferenceKind.StringVariableReader),
        new("inSpot", 2, int.MaxValue, SSpo, SUni),
        new("isDead", 1, int.MaxValue, SUni),
        new("isDone", 1, 1, SUni),
        new("isJoin", 2, 3),
        new("isNext", 2, 3),
        new("reckon", 2, 2, ReferenceKind.StringVariableReader, ReferenceKind.StringVariableReader | ReferenceKind.Text),
        new("getLife", 1, 1, SUni),
        new("getMode", 0, 0),
        new("getTime", 0, 0),
        new("getTurn", 0, 0),
        new("inPower", 2, int.MaxValue, SPow, SSpoUni),
        new("isAlive", 1, int.MaxValue, SPowUni),
        new("isEnemy", 2, 2, SSpoUni, SSpoUni),
        new("isEvent", 0, 0),
        new("isPeace", 0, 0),
        new("isWorld", 0, 0),
        new("countVar", 1, 1, ReferenceKind.StringVariableReader),
        new("getLimit", 0, 0),
        new("inBattle", 1, int.MaxValue, SPowUni),
        new("isActive", 1, 1, SUni),
        new("isArbeit", 1, 1, SUni),
        new("isEnable", 1, 1, SUni),
        new("isFriend", 2, 2, SSpoUni, SSpoUni),
        new("isInvade", 1, 1, SPow),
        new("isLeader", 1, 1, SUni),
        new("isLeague", 2, 2, SPow, SPow),
        new("isMaster", 1, 1, SUni),
        new("isPlayer", 1, 1, SPowUni),
        new("isPostIn", 3, 5),
        new("isRoamer", 1, 1, SUni),
        new("isTalent", 1, 1, SUni),
        new("isVassal", 1, 1, SUni),
        new("countGain", 1, 1, SPow),
        new("countPost", 6, 6, ReferenceKind.RedBlue, SUni, NumReader, NumReader, NumReader, NumReader),
        new("countSpot", 1, 1, SPow),
        new("countUnit", 1, 1, SPowUni),
        new("isAllDead", 1, 1, SUni),
        new("isAnyDead", 1, int.MaxValue, SUni),
        new("isComTurn", 0, 1, SPow),
        new("isDungeon", 0, 2, ReferenceKind.Dungeon, ReferenceKind.Number),
        new("isNewTurn", 0, 0),
        new("isNowSpot", 1, 1, SSpo),
        new("istoWorld", 0, 0),
        new("countForce", 1, 1, SPow),
        new("countMoney", 1, 1, SPow),
        new("countPower", 0, 0),
        new("countSkill", 1, 1, ReferenceKind.Skill | ReferenceKind.StringVariableReader),
        new("getLifePer", 1, 1, SUni),
        new("inRoamSpot", 2, int.MaxValue, SSpo, SUni),
        new("isInterval", 1, 1, NumReader),
        new("isRedAlive", 0, 0),
        new("isSameArmy", 2, 2, SUni),
        new("isScenario", 1, 1, ReferenceKind.Scenario),
        new("isWatching", 0, 0),
        new("getDistance", 2, 3),
        new("getRedCount", 0, 0),
        new("isBlueAlive", 0, 0),
        new("isGameClear", 0, 0),
        new("isPlayerEnd", 0, 0),
        new("getBlueCount", 0, 0),
        new("isPlayerTurn", 0, 0),
        new("isRoamLeader", 1, 1, SUni),
        new("getClearFloor", 1, 1),
        new("isWorldMusicStop", 0, 0),
    };
}
