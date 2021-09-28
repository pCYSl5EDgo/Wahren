﻿#nullable enable
// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE .tt FILE INSTEAD.
// </auto-generated>
namespace Wahren.AbstractSyntaxTree;

public enum FunctionKind : uint
{
    isSelect,
    isWhoDead,
    isGameOver,
    has,
    inVar,
    yet,
    rand,
    count,
    amount,
    equal,
    eqVar,
    isMap,
    isNpc,
    isNPM,
    isWar,
    ptest,
    conVar,
    inSpot,
    isDead,
    isDone,
    isJoin,
    isNext,
    reckon,
    getLife,
    getMode,
    getTime,
    getTurn,
    inPower,
    isAlive,
    isEnemy,
    isEvent,
    isPeace,
    isWorld,
    countVar,
    getLimit,
    inBattle,
    isActive,
    isArbeit,
    isEnable,
    isFriend,
    isInvade,
    isLeader,
    isLeague,
    isMaster,
    isPlayer,
    isPostIn,
    isRoamer,
    isTalent,
    isVassal,
    countGain,
    countPost,
    countSpot,
    countUnit,
    isAllDead,
    isAnyDead,
    isComTurn,
    isDungeon,
    isNewTurn,
    isNowSpot,
    istoWorld,
    countForce,
    countMoney,
    countPower,
    countSkill,
    getLifePer,
    inRoamSpot,
    isInterval,
    isRedAlive,
    isSameArmy,
    isScenario,
    isWatching,
    getDistance,
    getRedCount,
    isBlueAlive,
    isGameClear,
    isPlayerEnd,
    getBlueCount,
    isPlayerTurn,
    isRoamLeader,
    getClearFloor,
    isWorldMusicStop,
    None = uint.MaxValue,
}

public static class FunctionKindHelper
{
    public static FunctionKind Convert(ReadOnlySpan<char> key)
    {
        if (key.Length < 3 || key.Length > 16)
        {
            return FunctionKind.None;
        }

        Span<char> span = stackalloc char[key.Length];
        key.CopyTo(span);
        for (var i = 0; i < key.Length; ++i)
        {
            if (span[i] >= 'a')
            {
                span[i] = (char)(ushort)(span[i] - 32);
            }
        }

        switch (span.Length)
        {
            case 3:
                if (span.SequenceEqual("HAS")) { return FunctionKind.has; }
                else if (span.SequenceEqual("YET")) { return FunctionKind.yet; }
                else { return FunctionKind.None; }
            case 4:
                if (span.SequenceEqual("RAND")) { return FunctionKind.rand; }
                else { return FunctionKind.None; }
        }
        ulong key4 = System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(System.Runtime.InteropServices.MemoryMarshal.Cast<char, byte>(span));
        span = span.Slice(4);
        switch (span.Length - 1)
        {
            case 3 when (key4 == 0x0045005300530049UL) && span.SequenceEqual("LECT"): return FunctionKind.isSelect;
            case 3 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("TVAR"): return FunctionKind.countVar;
            case 3 when (key4 == 0x004C005400450047UL) && span.SequenceEqual("IMIT"): return FunctionKind.getLimit;
            case 3 when (key4 == 0x00410042004E0049UL) && span.SequenceEqual("TTLE"): return FunctionKind.inBattle;
            case 3 when (key4 == 0x0043004100530049UL) && span.SequenceEqual("TIVE"): return FunctionKind.isActive;
            case 3 when (key4 == 0x0052004100530049UL) && span.SequenceEqual("BEIT"): return FunctionKind.isArbeit;
            case 3 when (key4 == 0x004E004500530049UL) && span.SequenceEqual("ABLE"): return FunctionKind.isEnable;
            case 3 when (key4 == 0x0052004600530049UL) && span.SequenceEqual("IEND"): return FunctionKind.isFriend;
            case 3 when (key4 == 0x004E004900530049UL) && span.SequenceEqual("VADE"): return FunctionKind.isInvade;
            case 3 when (key4 == 0x0045004C00530049UL) && span.SequenceEqual("ADER"): return FunctionKind.isLeader;
            case 3 when (key4 == 0x0045004C00530049UL) && span.SequenceEqual("AGUE"): return FunctionKind.isLeague;
            case 3 when (key4 == 0x0041004D00530049UL) && span.SequenceEqual("STER"): return FunctionKind.isMaster;
            case 3 when (key4 == 0x004C005000530049UL) && span.SequenceEqual("AYER"): return FunctionKind.isPlayer;
            case 3 when (key4 == 0x004F005000530049UL) && span.SequenceEqual("STIN"): return FunctionKind.isPostIn;
            case 3 when (key4 == 0x004F005200530049UL) && span.SequenceEqual("AMER"): return FunctionKind.isRoamer;
            case 3 when (key4 == 0x0041005400530049UL) && span.SequenceEqual("LENT"): return FunctionKind.isTalent;
            case 3 when (key4 == 0x0041005600530049UL) && span.SequenceEqual("SSAL"): return FunctionKind.isVassal;
            case 4 when (key4 == 0x0048005700530049UL) && span.SequenceEqual("ODEAD"): return FunctionKind.isWhoDead;
            case 4 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("TGAIN"): return FunctionKind.countGain;
            case 4 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("TPOST"): return FunctionKind.countPost;
            case 4 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("TSPOT"): return FunctionKind.countSpot;
            case 4 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("TUNIT"): return FunctionKind.countUnit;
            case 4 when (key4 == 0x004C004100530049UL) && span.SequenceEqual("LDEAD"): return FunctionKind.isAllDead;
            case 4 when (key4 == 0x004E004100530049UL) && span.SequenceEqual("YDEAD"): return FunctionKind.isAnyDead;
            case 4 when (key4 == 0x004F004300530049UL) && span.SequenceEqual("MTURN"): return FunctionKind.isComTurn;
            case 4 when (key4 == 0x0055004400530049UL) && span.SequenceEqual("NGEON"): return FunctionKind.isDungeon;
            case 4 when (key4 == 0x0045004E00530049UL) && span.SequenceEqual("WTURN"): return FunctionKind.isNewTurn;
            case 4 when (key4 == 0x004F004E00530049UL) && span.SequenceEqual("WSPOT"): return FunctionKind.isNowSpot;
            case 4 when (key4 == 0x004F005400530049UL) && span.SequenceEqual("WORLD"): return FunctionKind.istoWorld;
            case 5 when (key4 == 0x0041004700530049UL) && span.SequenceEqual("MEOVER"): return FunctionKind.isGameOver;
            case 5 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("TFORCE"): return FunctionKind.countForce;
            case 5 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("TMONEY"): return FunctionKind.countMoney;
            case 5 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("TPOWER"): return FunctionKind.countPower;
            case 5 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("TSKILL"): return FunctionKind.countSkill;
            case 5 when (key4 == 0x004C005400450047UL) && span.SequenceEqual("IFEPER"): return FunctionKind.getLifePer;
            case 5 when (key4 == 0x004F0052004E0049UL) && span.SequenceEqual("AMSPOT"): return FunctionKind.inRoamSpot;
            case 5 when (key4 == 0x004E004900530049UL) && span.SequenceEqual("TERVAL"): return FunctionKind.isInterval;
            case 5 when (key4 == 0x0045005200530049UL) && span.SequenceEqual("DALIVE"): return FunctionKind.isRedAlive;
            case 5 when (key4 == 0x0041005300530049UL) && span.SequenceEqual("MEARMY"): return FunctionKind.isSameArmy;
            case 5 when (key4 == 0x0043005300530049UL) && span.SequenceEqual("ENARIO"): return FunctionKind.isScenario;
            case 5 when (key4 == 0x0041005700530049UL) && span.SequenceEqual("TCHING"): return FunctionKind.isWatching;
            case 0 when (key4 == 0x00410056004E0049UL) && span.SequenceEqual("R"): return FunctionKind.inVar;
            case 0 when (key4 == 0x004E0055004F0043UL) && span.SequenceEqual("T"): return FunctionKind.count;
            case 0 when (key4 == 0x0041005500510045UL) && span.SequenceEqual("L"): return FunctionKind.equal;
            case 0 when (key4 == 0x0041005600510045UL) && span.SequenceEqual("R"): return FunctionKind.eqVar;
            case 0 when (key4 == 0x0041004D00530049UL) && span.SequenceEqual("P"): return FunctionKind.isMap;
            case 0 when (key4 == 0x0050004E00530049UL) && span.SequenceEqual("C"): return FunctionKind.isNpc;
            case 0 when (key4 == 0x0050004E00530049UL) && span.SequenceEqual("M"): return FunctionKind.isNPM;
            case 0 when (key4 == 0x0041005700530049UL) && span.SequenceEqual("R"): return FunctionKind.isWar;
            case 0 when (key4 == 0x0053004500540050UL) && span.SequenceEqual("T"): return FunctionKind.ptest;
            case 1 when (key4 == 0x0055004F004D0041UL) && span.SequenceEqual("NT"): return FunctionKind.amount;
            case 1 when (key4 == 0x0056004E004F0043UL) && span.SequenceEqual("AR"): return FunctionKind.conVar;
            case 1 when (key4 == 0x00500053004E0049UL) && span.SequenceEqual("OT"): return FunctionKind.inSpot;
            case 1 when (key4 == 0x0045004400530049UL) && span.SequenceEqual("AD"): return FunctionKind.isDead;
            case 1 when (key4 == 0x004F004400530049UL) && span.SequenceEqual("NE"): return FunctionKind.isDone;
            case 1 when (key4 == 0x004F004A00530049UL) && span.SequenceEqual("IN"): return FunctionKind.isJoin;
            case 1 when (key4 == 0x0045004E00530049UL) && span.SequenceEqual("XT"): return FunctionKind.isNext;
            case 1 when (key4 == 0x004B004300450052UL) && span.SequenceEqual("ON"): return FunctionKind.reckon;
            case 2 when (key4 == 0x004C005400450047UL) && span.SequenceEqual("IFE"): return FunctionKind.getLife;
            case 2 when (key4 == 0x004D005400450047UL) && span.SequenceEqual("ODE"): return FunctionKind.getMode;
            case 2 when (key4 == 0x0054005400450047UL) && span.SequenceEqual("IME"): return FunctionKind.getTime;
            case 2 when (key4 == 0x0054005400450047UL) && span.SequenceEqual("URN"): return FunctionKind.getTurn;
            case 2 when (key4 == 0x004F0050004E0049UL) && span.SequenceEqual("WER"): return FunctionKind.inPower;
            case 2 when (key4 == 0x004C004100530049UL) && span.SequenceEqual("IVE"): return FunctionKind.isAlive;
            case 2 when (key4 == 0x004E004500530049UL) && span.SequenceEqual("EMY"): return FunctionKind.isEnemy;
            case 2 when (key4 == 0x0056004500530049UL) && span.SequenceEqual("ENT"): return FunctionKind.isEvent;
            case 2 when (key4 == 0x0045005000530049UL) && span.SequenceEqual("ACE"): return FunctionKind.isPeace;
            case 2 when (key4 == 0x004F005700530049UL) && span.SequenceEqual("RLD"): return FunctionKind.isWorld;
            case 6 when (key4 == 0x0044005400450047UL) && span.SequenceEqual("ISTANCE"): return FunctionKind.getDistance;
            case 6 when (key4 == 0x0052005400450047UL) && span.SequenceEqual("EDCOUNT"): return FunctionKind.getRedCount;
            case 6 when (key4 == 0x004C004200530049UL) && span.SequenceEqual("UEALIVE"): return FunctionKind.isBlueAlive;
            case 6 when (key4 == 0x0041004700530049UL) && span.SequenceEqual("MECLEAR"): return FunctionKind.isGameClear;
            case 6 when (key4 == 0x004C005000530049UL) && span.SequenceEqual("AYEREND"): return FunctionKind.isPlayerEnd;
            case 7 when (key4 == 0x0042005400450047UL) && span.SequenceEqual("LUECOUNT"): return FunctionKind.getBlueCount;
            case 7 when (key4 == 0x004C005000530049UL) && span.SequenceEqual("AYERTURN"): return FunctionKind.isPlayerTurn;
            case 7 when (key4 == 0x004F005200530049UL) && span.SequenceEqual("AMLEADER"): return FunctionKind.isRoamLeader;
            case 8 when (key4 == 0x0043005400450047UL) && span.SequenceEqual("LEARFLOOR"): return FunctionKind.getClearFloor;
            case 11 when (key4 == 0x004F005700530049UL) && span.SequenceEqual("RLDMUSICSTOP"): return FunctionKind.isWorldMusicStop;
        }

        return FunctionKind.None;
    }
}
