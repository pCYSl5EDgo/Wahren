﻿global using System.Collections.Generic;
namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct CallableInfo
{
    public string Name;
    public int Min;
    public int Max;
    public ReferenceKind[][] KindArray;
    public string[]?[]?[]? SpecialArray;

    public CallableInfo(string name, int min, int max)
    {
        Name = name;
        Min = min;
        Max = max;
        KindArray = System.Array.Empty<ReferenceKind[]>();
        SpecialArray = null;
    }

    public CallableInfo(string name, int min, int max, params ReferenceKind[] kinds)
    {
        Name = name;
        Min = min;
        Max = max;
        KindArray = new ReferenceKind[][] { kinds };
        SpecialArray = null;
    }

    public CallableInfo(string name, int min, int max, params ReferenceKind[][] kinds)
    {
        Name = name;
        Min = min;
        Max = max;
        KindArray = kinds;
        SpecialArray = null;
    }

    public void Deconstruct(out string name, out int min, out int max)
    {
        name = Name;
        min = Min;
        max = Max;
    }

    public new ulong GetHashCode() => StringHashUtility.Calc(Name);

    public static readonly CallableInfo[] Specials = new CallableInfo[]
    {
        new("if", 0, 0),
        new("rif", 0, 0),
        new("while", 0, 0),
        new("next", 0, 0),
        new("return", 0, 0),
        new("break", 0, 0),
        new("continue", 0, 0),
    };

    private const ReferenceKind NumReader = ReferenceKind.NumberVariableReader | ReferenceKind.Number;
    private const ReferenceKind StrReader = ReferenceKind.StringVariableReader | ReferenceKind.Text;
    private const ReferenceKind SPow = ReferenceKind.StringVariableReader | ReferenceKind.Power;
    private const ReferenceKind SUni = ReferenceKind.StringVariableReader | ReferenceKind.Unit;
    private const ReferenceKind SUniCla = ReferenceKind.StringVariableReader | ReferenceKind.Unit | ReferenceKind.Class;
    private const ReferenceKind SPowUni = SPow | ReferenceKind.Unit;
    private const ReferenceKind SPowUniCla = SPowUni | ReferenceKind.Class;
    private const ReferenceKind SSki = ReferenceKind.StringVariableReader | ReferenceKind.Skill;
    private const ReferenceKind SSpo = ReferenceKind.StringVariableReader | ReferenceKind.Spot;
    private const ReferenceKind SSpoUni = ReferenceKind.StringVariableReader | ReferenceKind.Spot | ReferenceKind.Unit;
    private const ReferenceKind SSpoUniCla = ReferenceKind.StringVariableReader | ReferenceKind.Spot | ReferenceKind.Unit | ReferenceKind.Class;
    private const ReferenceKind SSpoPow = ReferenceKind.StringVariableReader | ReferenceKind.Spot | ReferenceKind.Power;
    private const ReferenceKind SSpoPowUni = ReferenceKind.StringVariableReader | ReferenceKind.Spot | ReferenceKind.Power | ReferenceKind.Unit;
    private const ReferenceKind SFriendship = ReferenceKind.StringVariableReader | ReferenceKind.Race | ReferenceKind.Class | ReferenceKind.Unit;

    public static readonly CallableInfo[] ActionInfoNormals = new CallableInfo[]
    {
        new("vc", 0, int.MaxValue),
        new("play", 0, int.MaxValue),
        new("ppl1", 0, 0),
        new("citom", 0, int.MaxValue),
        new("setbcg", 0, int.MaxValue),
        new("showCamp", 0, int.MaxValue),
        new("clickWait", 0, int.MaxValue),
        new("worldskin", 0, 0),
        new("darkness_off", 0, int.MaxValue),
        new("doGameEnding", 0, int.MaxValue),
        new("storeDeath", 0, int.MaxValue),
        new("pushDeath", 6, 6, StrReader, SPow, StrReader, StrReader, StrReader, ReferenceKind.NumberVariableWriter),
        new("setPowerHome", 1, 2, SPow, SSpo),
        new("msg", 1, 3, new[] { ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.face, ReferenceKind.CompoundText }),
        new("msg2", 1, 3, new[] { ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.face, ReferenceKind.CompoundText }),
        new("talk", 1, 3, new[] { ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.face, ReferenceKind.CompoundText }),
        new("talk2", 1, 3, new[] { ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.face, ReferenceKind.CompoundText }),
        new("chat", 1, 3, new[] { ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.face, ReferenceKind.CompoundText }),
        new("chat2", 1, 3, new[] { ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.face, ReferenceKind.CompoundText }),
        new("dialog", 1, 2, new[] { ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.CompoundText }),
        new("dialogF", 1, 2, new[] { ReferenceKind.CompoundText }, new[] { SUni | ReferenceKind.Text, ReferenceKind.CompoundText }),
        new("select", 2, 2, ReferenceKind.NumberVariableWriter, ReferenceKind.CompoundText),
        new("choice", 2, int.MaxValue, ReferenceKind.NumberVariableWriter, ReferenceKind.CompoundText),
        new("exit", 0, 1, ReferenceKind.Number | ReferenceKind.CompoundText),
        new("image", 1, 8, ReferenceKind.image, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader),
        new("image2", 1, 8, ReferenceKind.image, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader),
        new("showImage", 1, 8, ReferenceKind.image, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader),
        new("hideImage", 1, 2, ReferenceKind.image, NumReader),
        new("face", 1, 8, ReferenceKind.face, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader),
        new("face2", 1, 8, ReferenceKind.face, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader),
        new("showFace", 1, 8, ReferenceKind.face, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader),
        new("hideFace", 1, 2, ReferenceKind.face, NumReader),
        new("picture", 1, 8, ReferenceKind.picture, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader),
        new("picture2", 1, 8, ReferenceKind.picture, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader),
        new("showPict", 1, 8, ReferenceKind.picture, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader, NumReader),
        new("showPicture", 2, 3, ReferenceKind.picture, NumReader, ReferenceKind.Special) { SpecialArray = new[] { new[] { null, null, new[] { "on", "center", "right" } } } },
        new("hidePicture", 1, 2, ReferenceKind.picture, NumReader),
        new("stop", 0, 0),
        new("bg", 1, 2, ReferenceKind.image, ReferenceKind.Boolean),
        new("add", 2, 2, ReferenceKind.NumberVariableWriter, NumReader),
        new("div", 2, 2, ReferenceKind.NumberVariableWriter, NumReader),
        new("mod", 2, 2, ReferenceKind.NumberVariableWriter, NumReader),
        new("mul", 2, 2, ReferenceKind.NumberVariableWriter, NumReader),
        new("per", 2, 2, ReferenceKind.NumberVariableWriter, NumReader),
        new("set", 2, 2, ReferenceKind.NumberVariableWriter, NumReader),
        new("sub", 2, 2, ReferenceKind.NumberVariableWriter, NumReader),
        new("win", 0, 0),
        new("addv", 2, 2, ReferenceKind.StringVariableWriter, StrReader),
        new("call", 1, 1, ReferenceKind.Event),
        new("font", 0, 3, ReferenceKind.font, ReferenceKind.Number, ReferenceKind.Number),
        new("save", 0, 0),
        new("setv", 2, 2, ReferenceKind.StringVariableWriter, StrReader),
        new("subv", 2, 2, ReferenceKind.StringVariableWriter, StrReader),
        new("wait", 0, 1, NumReader),
        new("zoom", 1, 1, NumReader),
        new("clear", 1, 1, ReferenceKind.StringVariableWriter),
        new("erase", 0, 0),
        new("event", 1, 3, ReferenceKind.Event, ReferenceKind.Power, ReferenceKind.Power),
        new("focus", 0, 1, SUni),
        new("fontc", 0, 3, ReferenceKind.Number, ReferenceKind.Number, ReferenceKind.Number),
        new("gread", 2, 3, ReferenceKind.GlobalVariableReader | ReferenceKind.StringVariableReader, ReferenceKind.NumberVariableWriter, ReferenceKind.StringVariableWriter),
        new("gwrite", 2, 3, ReferenceKind.GlobalVariableWriter | ReferenceKind.StringVariableReader, NumReader, StrReader),
        new("index", 3, 3, ReferenceKind.StringVariableReader, NumReader, ReferenceKind.StringVariableWriter),
        new("storeIndex", 3, 3, ReferenceKind.StringVariableReader, NumReader, ReferenceKind.StringVariableWriter),
        new("storeIndexVar", 3, 3, ReferenceKind.StringVariableReader, NumReader, ReferenceKind.StringVariableWriter),
        new("pushv", 2, 3, new[] { ReferenceKind.StringVariableReader, ReferenceKind.NumberVariableWriter }, new[] { ReferenceKind.StringVariableReader, StrReader , ReferenceKind.NumberVariableWriter }),
        new("setPM", 2, 2, SUni, SFriendship),
        new("setud", 2, 2, ReferenceKind.GlobalStringVariableWriter, StrReader),
        new("storeud", 2, 2, ReferenceKind.GlobalStringVariableReader, ReferenceKind.StringVariableWriter),
        new("shake", 0, 1, NumReader),
        new("title", 2, 2, ReferenceKind.Text, NumReader),
        new("addstr", 2, 2, ReferenceKind.StringVariableWriter, ReferenceKind.Text),
        new("addVar", 2, 2, ReferenceKind.StringVariableWriter, StrReader),
        new("fadein", 0, 1, NumReader),
        new("locate", 1, 2, new[] { SSpoUni }, new[] { NumReader, NumReader }),
        new("playSE", 1, 1, ReferenceKind.sound),
        new("scroll", 1, 2, new[] { SSpoUniCla }, new[] { NumReader, NumReader }),
        new("scroll2", 1, 2, new[] { SSpoUniCla }, new[] { NumReader, NumReader }),
        new("setVar", 2, 2, ReferenceKind.StringVariableWriter, StrReader),
        new("shadow", 0, 6, ReferenceKind.Number, ReferenceKind.Number, ReferenceKind.Number, ReferenceKind.Number, ReferenceKind.Number, ReferenceKind.Number),
        new("subVar", 2, 2, ReferenceKind.StringVariableWriter, StrReader),
        new("title2", 2, 2, ReferenceKind.Text, NumReader),
        new("volume", 1, 1, NumReader),
        new("addCapa", 2, 2, SSpo, NumReader),
        new("addGain", 2, 2, SSpo, NumReader),
        new("addItem", 1, 1, SSki),
        new("addSpot", 1, 2, SSpo, SPow),
        new("addUnit", 2, 3, new[] { SUniCla, SSpoPowUni }, new[] { SUni, SSpo, ReferenceKind.Special }) { SpecialArray = new[] { null, new[] { null, null, new[] { "roam" } } } },
        new("doskill", 5, 5, ReferenceKind.Skill, NumReader, NumReader, NumReader, ReferenceKind.Boolean),
        new("fadeout", 0, 1, NumReader),
        new("loopBGM", 1, int.MaxValue, ReferenceKind.bgm),
        new("minimap", 1, 1, ReferenceKind.Boolean),
        new("playBGM", 0, 1, ReferenceKind.bgm),
        new("pushCon", 3, 3, SSpo, SUni | ReferenceKind.Class, ReferenceKind.NumberVariableWriter),
        new("pushSex", 2, 2, SUni, ReferenceKind.NumberVariableWriter),
        new("pushVar", 2, 3, new[] { ReferenceKind.StringVariableReader, ReferenceKind.NumberVariableWriter }, new[] { ReferenceKind.StringVariableReader, StrReader , ReferenceKind.NumberVariableWriter }),
        new("routine", 1, 1, ReferenceKind.Event),
        new("setCapa", 2, 2, SSpo, NumReader),
        new("setDone", 2, 2, SUni, ReferenceKind.Boolean),
        new("setGain", 2, 2, SSpo, NumReader),
        new("shuffle", 1, 1, ReferenceKind.StringVariableReader),
        new("stopBGM", 0, 0),
        new("storePM", 2, 2, SUni, ReferenceKind.StringVariableWriter),
        new("addDiplo", 2, 3, new[] { ReferenceKind.StringVariableReader, NumReader }, new[] { SPow, SPow, NumReader }),
        new("levelup", 2, 2, SUni, NumReader),
        new("addLevel", 2, 2, SUni, NumReader),
        new("addLimit", 1, 1, NumReader),
        new("addLoyal", 2, 2, SUni, NumReader),
        new("addMoney", 2, 2, SPowUni, NumReader),
        new("addPower", 1, 1, SPow),
        new("addSkill", 2, int.MaxValue, SUni, SSki),
        new("addTroop", 5, 5, SUniCla, NumReader, NumReader, NumReader, ReferenceKind.RedBlue | ReferenceKind.Boolean),
        new("stopTroop", 1, 1, SUniCla),
        new("addTrust", 2, 2, SUni, NumReader),
        new("aimTroop", 2, 3, new[] { SUniCla, SUniCla }, new[]{ SUniCla, NumReader, NumReader }),
        new("clearVar", 1, 1, ReferenceKind.StringVariableReader),
        new("darkness", 0, 4, ReferenceKind.Number, ReferenceKind.Number, ReferenceKind.Number, ReferenceKind.Number),
        new("exitItem", 1, 1, SSki),
        new("hideLink", 2, 2, SSpo, SSpo),
        new("hideSpot", 1, 1, SSpo),
        new("linkSpot", 2, 4, SSpo, SSpo, ReferenceKind.imagedata, ReferenceKind.Number),
        new("openGoal", 0, 0),
        new("pushCapa", 2, 2, SSpo, ReferenceKind.NumberVariableWriter),
        new("pushGain", 2, 2, SPow | ReferenceKind.Spot, ReferenceKind.NumberVariableWriter),
        new("pushItem", 2, 2, SSki, ReferenceKind.NumberVariableWriter),
        new("pushRand", 1, 1, ReferenceKind.NumberVariableWriter),
        new("pushRank", 2, 2, SUni, ReferenceKind.NumberVariableWriter),
        new("pushSpot", 2, 2, SPow, ReferenceKind.NumberVariableWriter),
        new("pushTurn", 1, 1, ReferenceKind.NumberVariableWriter),
        new("roamUnit", 1, 2, SUni, SSpo),
        new("roamUnit2", 1, 2, SUni, SSpo),
        new("setDiplo", 2, 3, new[] { ReferenceKind.StringVariableReader, NumReader }, new[] { SPow, SPow, NumReader }),
        new("setLevel", 2, 2, SUni, NumReader),
        new("setLimit", 1, 1, NumReader),
        new("setMoney", 2, 2, SPowUni, NumReader),
        new("setTruce", 2, 3, new[] { ReferenceKind.StringVariableReader, NumReader }, new[] { SPow, SPow, NumReader }),
        new("showSpot", 1, 1, SSpo),
        new("spotmark", 0, 2, SSpoPowUni, ReferenceKind.Number),
        new("showSpotMark", 1, 2, SSpoPowUni, ReferenceKind.Number),
        new("hideSpotMark", 0, 0),
        new("hideEscape", 2, 2, SSpo, SSpo),
        new("showParty", 1, 1, ReferenceKind.Boolean),
        new("addCastle", 2, 2, SSpo, NumReader),
        new("addFriend", 2, int.MaxValue, SUni, SFriendship),
        new("addMerits", 2, 2, SUni, NumReader),
        new("addSkill2", 2, int.MaxValue, SUni, SSki),
        new("addStatus", 3, 3, SUni, ReferenceKind.StringVariableReader | ReferenceKind.Status, NumReader),
        new("changeMap", 2, 2, SSpo, ReferenceKind.map),
        new("closeGoal", 0, 0),
        new("ctrlTroop", 1, 1, SUniCla),
        new("entryItem", 1, 1, SSki),
        new("equipItem", 2, 3, SUni, SSki, ReferenceKind.Boolean),
        new("eraseItem", 1, 1, SSki),
        new("eraseUnit", 1, 1, SUni),
        new("formTroop", 2, 2, SUniCla, NumReader),
        new("freeTroop", 1, 1, SUniCla),
        new("haltTroop", 1, 1, SUniCla),
        new("hideBlind", 0, 0),
        new("hideChara", 1, 1, ReferenceKind.imagedata),
        new("moveTroop", 2, 4, new[] { SUniCla, SUniCla }, new[] { SUniCla, NumReader, NumReader }, new[] { SUniCla, NumReader, NumReader, NumReader }),
        new("moveTroopFix", 2, 4, new[] { SUniCla, SUniCla }, new[] { SUniCla, NumReader, NumReader }, new[] { SUniCla, NumReader, NumReader, NumReader }),
        new("smoveTroop", 2, 4, new[] { SUniCla, SUniCla }, new[] { SUniCla, NumReader, NumReader }, new[] { SUniCla, NumReader, NumReader, NumReader }),
        new("smoveTroopFix", 2, 4, new[] { SUniCla, SUniCla }, new[] { SUniCla, NumReader, NumReader }, new[] { SUniCla, NumReader, NumReader, NumReader }),
        new("playWorld", 0, 0),
        new("pushDiplo", 3, 3, SPow, SPow, ReferenceKind.NumberVariableWriter),
        new("pushForce", 2, 2, SPowUni, ReferenceKind.NumberVariableWriter),
        new("pushLevel", 2, 2, SUni, ReferenceKind.NumberVariableWriter),
        new("pushLimit", 1, 1, ReferenceKind.NumberVariableWriter),
        new("pushLoyal", 2, 2, SUni, ReferenceKind.NumberVariableWriter),
        new("pushMoney", 2, 2, SPowUni, ReferenceKind.NumberVariableWriter),
        new("pushRand2", 1, 1, ReferenceKind.NumberVariableWriter),
        new("pushTrain", 2, 2, SPow, ReferenceKind.NumberVariableWriter),
        new("pushTrust", 2, 2, SPowUni, ReferenceKind.NumberVariableWriter),
        new("resetTime", 0, 0),
        new("resetZone", 0, 0),
        new("setArbeit", 3, 3, SPow, SUni, NumReader),
        new("setCastle", 2, 2, SSpo, NumReader),
        new("setLeague", 2, 3, new[] { ReferenceKind.StringVariableReader, NumReader }, new[] { SPow, SPow, NumReader }),
        new("setStatus", 3, 3, SUni, ReferenceKind.StringVariableReader | ReferenceKind.Status, NumReader),
        new("showBlind", 0, 0),
        new("showChara", 3, 5, ReferenceKind.imagedata, NumReader, NumReader, NumReader, NumReader),
        new("terminate", 0, 0),
        new("backScroll", 0, 0),
        new("changeRace", 2, 2, SUni, ReferenceKind.StringVariableReader | ReferenceKind.Race),
        new("endingRoll", 1, 1, ReferenceKind.Event),
        new("erasePower", 1, 1, SPow),
        new("eraseSkill", 1, int.MaxValue, SUni, SSki | ReferenceKind.Skillset),
        new("eraseUnit2", 2, int.MaxValue, SSpoPow, SUni),
        new("eraseTroop", 1, 1, SUniCla),
        new("linkEscape", 2, 4, SSpo, SSpo, ReferenceKind.imagedata, ReferenceKind.Number),
        new("playBattle", 0, 0),
        new("pushCastle", 2, 2, SSpo, ReferenceKind.NumberVariableWriter),
        new("pushMerits", 2, 2, SUni, ReferenceKind.NumberVariableWriter),
        new("pushStatus", 3, 3, SUni, ReferenceKind.Status, ReferenceKind.NumberVariableWriter),
        new("reloadMenu", 0, 1, ReferenceKind.Boolean),
        new("removeSpot", 1, 1, SSpo),
        new("resetTruce", 1, 2, new[] { ReferenceKind.StringVariableReader }, new[] { SPow, SPow }),
        new("setDungeon", 2, 2, SSpo, ReferenceKind.Boolean),
        new("shiftTroop", 3, 4, SUniCla, NumReader, NumReader, ReferenceKind.Boolean),
        new("shuffleVar", 1, 1, ReferenceKind.StringVariableReader),
        new("skillTroop", 2, 3, SUniCla, ReferenceKind.Skill, ReferenceKind.Special) { SpecialArray = new[] { new[] { null, null, new[] { "on", "dead_ok" } } } },
        new("sleepTroop", 1, 1, SUniCla),
        new("speedTroop", 2, 2, SUniCla, NumReader),
        new("unionPower", 2, 2, SPow, SPow),
        new("activeTroop", 1, 1, SUniCla),
        new("addTraining", 2, 2, SPow, NumReader),
        new("battleEvent", 1, 1, ReferenceKind.Event),
        new("changeClass", 2, 2, SUni, ReferenceKind.StringVariableReader | ReferenceKind.Class),
        new("choiceTitle", 0, 1, ReferenceKind.Text),
        new("eraseFriend", 1, int.MaxValue, SUni, SFriendship),
        new("pushSpotPos", 3, 3, SSpo, ReferenceKind.NumberVariableWriter, ReferenceKind.NumberVariableWriter),
        new("pushTrainUp", 2, 2, SPow, ReferenceKind.NumberVariableWriter),
        new("removeSkill", 2, 2, SUni, SSki | ReferenceKind.Skillset),
        new("removeTroop", 1, 1, SUniCla),
        new("resetLeague", 1, 2, new[] { ReferenceKind.StringVariableReader }, new[] { SPow, SPow }),
        new("scrollSpeed", 1, 1, NumReader),
        new("setTraining", 2, 2, SPow, NumReader),
        new("shiftTroop2", 4, 4, SUni, NumReader, NumReader, ReferenceKind.Boolean),
        new("showDungeon", 1, 1, ReferenceKind.StringVariableReader | ReferenceKind.Dungeon),
        new("unctrlTroop", 0, 1, SUniCla),
        new("addBaseLevel", 2, 2, SPow, NumReader),
        new("changeCastle", 2, 2, SSpo, NumReader),
        new("changeMaster", 1, 3, new[] { SUni }, new[] { SPowUni, SUni }, new[] { SUni, SUni, ReferenceKind.flag }),
        new("changePlayer", 0, 1, SUni),
        new("retreatTroop", 1, 1, SUniCla),
        new("reverseChara", 1, 1, ReferenceKind.imagedata),
        new("setBaseLevel", 2, 2, SPow, NumReader),
        new("setGameClear", 0, 1, ReferenceKind.Boolean),
        new("showPolitics", 1, 1, ReferenceKind.Boolean),
        new("storeAllSpot", 1, 1, ReferenceKind.StringVariableWriter),
        new("addPowerMerce", 2, int.MaxValue, SPow, SFriendship),
        new("addPowerStaff", 2, int.MaxValue, SPow, SFriendship),
        new("addPowerMerce2", 2, int.MaxValue, SPow, SFriendship),
        new("addPowerStaff2", 2, int.MaxValue, SPow, SFriendship),
        new("addTrainingUp", 2, 2, SPow, NumReader),
        new("changeDungeon", 2, 2, SPow, ReferenceKind.Dungeon),
        new("pushBaseLevel", 2, 2, SPow, ReferenceKind.NumberVariableWriter),
        new("setEnemyPower", 3, 3, SPow, SPow, NumReader),
        new("setTrainingUp", 2, 2, SPow, NumReader),
        new("setWorldMusic", 0, 0),
        new("storeAllPower", 1, 1, ReferenceKind.StringVariableWriter),
        new("storeComPower", 1, 1, ReferenceKind.StringVariableWriter),
        new("storeNextSpot", 2, 2, SSpo, ReferenceKind.StringVariableWriter),
        new("storeNowPower", 1, 1, ReferenceKind.StringVariableWriter),
        new("storeRectUnit", 5, 5, ReferenceKind.RedBlue, NumReader, NumReader, NumReader, NumReader, ReferenceKind.NumberVariableWriter),
        new("storeSkillset", 2, 2, ReferenceKind.StringVariableReader | ReferenceKind.Skillset, ReferenceKind.StringVariableWriter),
        new("storeTodoUnit", 2, 2, SUniCla, ReferenceKind.StringVariableWriter),
        new("changePowerFix", 2, 2, SPow, ReferenceKind.Text),
        new("eraseUnitTroop", 1, 1, SUniCla),
        new("pushBattleHome", 2, 2, ReferenceKind.NumberVariableWriter, ReferenceKind.NumberVariableWriter),
        new("pushBattleRect", 2, 2, ReferenceKind.NumberVariableWriter, ReferenceKind.NumberVariableWriter),
        new("pushCountPower", 1, 1, ReferenceKind.NumberVariableWriter),
        new("storeAliveUnit", 2, 2, SUniCla, ReferenceKind.StringVariableWriter),
        new("storeAllTalent", 1, 1, ReferenceKind.StringVariableWriter),
        new("changePowerFlag", 2, 2, SPow, ReferenceKind.StringVariableReader | ReferenceKind.flag),
        new("changePowerName", 2, 2, SPow, ReferenceKind.Text),
        new("changeSpotImage", 2, 2, SSpo, ReferenceKind.imagedata),
        new("erasePowerMerce", 1, 1, SPow),
        new("erasePowerStaff", 1, 1, SPow),
        new("resetEnemyPower", 1, 2, SPow, SPow),
        new("resetWorldMusic", 0, 0),
        new("setDungeonFloor", 2, 2, ReferenceKind.StringVariableReader | ReferenceKind.Dungeon, NumReader),
        new("storeBattleSpot", 1, 1, ReferenceKind.StringVariableWriter),
        new("storePlayerUnit", 1, 1, ReferenceKind.StringVariableWriter),
        new("storeRaceOfUnit", 2, 2, SUni, ReferenceKind.StringVariableWriter),
        new("storeSpotOfUnit", 2, 2, SUni, ReferenceKind.StringVariableWriter),
        new("storeUnitOfSpot", 2, 2, SSpo, ReferenceKind.StringVariableWriter),
        new("storeAttackPower", 1, 1, ReferenceKind.StringVariableWriter),
        new("storeClassOfUnit", 2, 2, SUni, ReferenceKind.StringVariableWriter),
        new("storeNeutralSpot", 1, 1, ReferenceKind.StringVariableWriter),
        new("storePlayerPower", 1, 1, ReferenceKind.StringVariableWriter),
        new("storePowerOfSpot", 2, 2, SSpo, ReferenceKind.StringVariableWriter),
        new("storePowerOfUnit", 2, 2, SUni, ReferenceKind.StringVariableWriter),
        new("storeSkillOfUnit", 2, 2, SUni, ReferenceKind.StringVariableWriter),
        new("storeSpotOfPower", 2, 2, SPow, ReferenceKind.StringVariableWriter),
        new("storeTalentPower", 2, 2, ReferenceKind.Unit, ReferenceKind.StringVariableWriter),
        new("storeUnitOfPower", 2, 2, SPow, ReferenceKind.StringVariableWriter),
        new("clearBattleRecord", 0, 0),
        new("storeDefensePower", 1, 1, ReferenceKind.StringVariableWriter),
        new("storeLeaderOfSpot", 2, 2, SSpo, ReferenceKind.StringVariableWriter),
        new("storeMasterOfUnit", 2, 2, SUni, ReferenceKind.StringVariableWriter),
        new("storeMemberOfUnit", 2, 2, SUni, ReferenceKind.StringVariableWriter),
        new("storePowerOfForce", 2, 2, NumReader, ReferenceKind.StringVariableWriter),
        new("storeSpotOfBattle", 1, 1, ReferenceKind.StringVariableWriter),
        new("storeLeaderOfPower", 2, 2, SPow, ReferenceKind.StringVariableWriter),
        new("storeMasterOfPower", 2, 2, SPow, ReferenceKind.StringVariableWriter),
        new("storePowerOfAttack", 1, 1, ReferenceKind.StringVariableWriter),
        new("storeNonPlayerPower", 1, 1, ReferenceKind.StringVariableWriter),
        new("storePowerOfDefense", 1, 1, ReferenceKind.StringVariableWriter),
        new("storeRoamUnitOfSpot", 2, 2, SSpo, ReferenceKind.StringVariableWriter),
        new("storeBaseClassOfUnit", 2, 2, SUni, ReferenceKind.StringVariableWriter),
    };

    public static readonly CallableInfo[] FunctionInfoNormals = new CallableInfo[]
    {
        new("isSelect", 0, int.MaxValue),
        new("isWhoDead", 0, int.MaxValue),
        new("isGameOver", 0, int.MaxValue),
        new("has", 2, int.MaxValue, ReferenceKind.StringVariableReader, StrReader),
        new("inVar", 2, int.MaxValue, ReferenceKind.StringVariableReader, StrReader),
        new("yet", 1, 1, ReferenceKind.Event),
        new("rand", 0, 0),
        new("count", 1, 1, ReferenceKind.StringVariableReader),
        new("amount", 1, 1, ReferenceKind.StringVariableReader),
        new("equal", 2, 2, ReferenceKind.StringVariableReader, StrReader),
        new("eqVar", 2, 2, ReferenceKind.StringVariableReader, StrReader),
        new("isMap", 0, 0),
        new("isNpc", 1, int.MaxValue, SPowUni),
        new("isNPM", 0, 0),
        new("isWar", 2, 2, SPow, ReferenceKind.Power),
        new("ptest", 2, 2, ReferenceKind.Spot, ReferenceKind.Unit),
        new("conVar", 1, 1, ReferenceKind.StringVariableReader),
        new("inSpot", 2, int.MaxValue, SSpo, SUni),
        new("isDead", 1, int.MaxValue, SUni),
        new("isDone", 1, 1, SUni),
        new("isJoin", 2, 3, SSpoPow, SSpoPow, ReferenceKind.Boolean),
        new("isNext", 2, 3, SSpo, SSpo, NumReader),
        new("reckon", 2, 2, ReferenceKind.StringVariableReader, StrReader),
        new("getLife", 1, 1, SUni),
        new("getMode", 0, 0),
        new("getTime", 0, 0),
        new("getTurn", 0, 0),
        new("inPower", 2, int.MaxValue, SPow, SSpoUni),
        new("isAlive", 1, int.MaxValue, SPowUni),
        new("isEnemy", 2, 2, SSpoUni, SSpoUni),
        new("isEvent", 0, 0),
        new("isPeace", 0, 1, ReferenceKind.Boolean),
        new("isWorld", 0, 0),
        new("countVar", 1, 1, ReferenceKind.StringVariableReader),
        new("getLimit", 0, 0),
        new("inBattle", 1, int.MaxValue, SPowUniCla),
        new("isActive", 1, 1, SUni),
        new("isArbeit", 1, 1, SUni),
        new("isEnable", 1, 1, SUni),
        new("isFriend", 2, 2, SSpoUni, SSpoUni),
        new("isInvade", 1, 1, SPow),
        new("isLeader", 1, 1, SUni),
        new("isLeague", 2, 2, SPow, SPow),
        new("isMaster", 1, 1, SUni),
        new("isPlayer", 1, 1, SPowUni),
        new("isPostIn", 3, 5, new[] { ReferenceKind.RedBlue, SUniCla, NumReader }, new[] { ReferenceKind.RedBlue, NumReader, NumReader, NumReader }, new[] { ReferenceKind.RedBlue, NumReader, NumReader, NumReader, NumReader }),
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
        new("countSkill", 1, 1, SSki),
        new("getLifePer", 1, 1, SUni),
        new("inRoamSpot", 2, int.MaxValue, SSpo, SUni),
        new("isInterval", 1, 1, NumReader),
        new("isRedAlive", 0, 0),
        new("isSameArmy", 2, 2, SUni),
        new("isScenario", 1, 1, ReferenceKind.Scenario),
        new("isWatching", 0, 0),
        new("getDistance", 2, 3, new[] { SUniCla, SUniCla }, new[] { SUniCla, NumReader, NumReader }),
        new("getRedCount", 0, 0),
        new("isBlueAlive", 0, 0),
        new("isGameClear", 0, 0),
        new("isPlayerEnd", 0, 0),
        new("getBlueCount", 0, 0),
        new("isPlayerTurn", 0, 0),
        new("isRoamLeader", 1, 1, SUni),
        new("getClearFloor", 1, 1, ReferenceKind.Dungeon),
        new("isWorldMusicStop", 0, 0),
    };
}
