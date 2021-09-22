namespace Wahren.AbstractSyntaxTree;

public enum ActionKind : uint
{
	@if,
	rif,
	@while,
	next,
	@return,
	@break,
	@continue,
	bg,
	vc,
	add,
	div,
	mod,
	msg,
	mul,
	per,
	set,
	sub,
	win,
	addv,
	call,
	chat,
	exit,
	face,
	font,
	msg2,
	play,
	ppl1,
	save,
	setv,
	stop,
	subv,
	talk,
	wait,
	zoom,
	chat2,
	citom,
	clear,
	erase,
	@event,
	face2,
	focus,
	fontc,
	gread,
	image,
	index,
	pushv,
	setPM,
	setud,
	shake,
	talk2,
	title,
	addstr,
	addVar,
	choice,
	dialog,
	fadein,
	gwrite,
	locate,
	playSE,
	scroll,
	select,
	setbcg,
	setVar,
	shadow,
	subVar,
	title2,
	volume,
	addCapa,
	addGain,
	addItem,
	addSpot,
	addUnit,
	dialogF,
	doskill,
	fadeout,
	levelup,
	loopBGM,
	minimap,
	picture,
	playBGM,
	pushCon,
	pushSex,
	pushVar,
	routine,
	scroll2,
	setCapa,
	setDone,
	setGain,
	shuffle,
	stopBGM,
	storePM,
	storeud,
	addDiplo,
	addLevel,
	addLimit,
	addLoyal,
	addMoney,
	addPower,
	addSkill,
	addTroop,
	addTrust,
	aimTroop,
	clearVar,
	darkness,
	exitItem,
	hideLink,
	hideSpot,
	linkSpot,
	openGoal,
	picture2,
	pushCapa,
	pushGain,
	pushItem,
	pushRand,
	pushRank,
	pushSpot,
	pushTurn,
	roamUnit,
	setDiplo,
	setLevel,
	setLimit,
	setMoney,
	setTruce,
	showCamp,
	showFace,
	showPict,
	showSpot,
	spotmark,
	addCastle,
	addFriend,
	addMerits,
	addSkill2,
	addStatus,
	changeMap,
	clickWait,
	closeGoal,
	ctrlTroop,
	entryItem,
	equipItem,
	eraseItem,
	eraseUnit,
	formTroop,
	freeTroop,
	haltTroop,
	hideBlind,
	hideChara,
	hideImage,
	moveTroop,
	playWorld,
	pushDeath,
	pushDiplo,
	pushForce,
	pushLevel,
	pushLimit,
	pushLoyal,
	pushMoney,
	pushRand2,
	pushTrain,
	pushTrust,
	resetTime,
	resetZone,
	roamUnit2,
	setArbeit,
	setCastle,
	setLeague,
	setStatus,
	showBlind,
	showChara,
	showImage,
	stopTroop,
	terminate,
	worldskin,
	backScroll,
	changeRace,
	endingRoll,
	erasePower,
	eraseSkill,
	eraseTroop,
	eraseUnit2,
	hideEscape,
	linkEscape,
	playBattle,
	pushCastle,
	pushMerits,
	pushStatus,
	reloadMenu,
	removeSpot,
	resetTruce,
	setDungeon,
	shiftTroop,
	shuffleVar,
	skillTroop,
	sleepTroop,
	smoveTroop,
	speedTroop,
	storeDeath,
	storeIndex,
	unionPower,
	activeTroop,
	addTraining,
	battleEvent,
	changeClass,
	choiceTitle,
	eraseFriend,
	hidePicture,
	pushSpotPos,
	pushTrainUp,
	removeSkill,
	removeTroop,
	resetLeague,
	scrollSpeed,
	setTraining,
	shiftTroop2,
	showDungeon,
	showPicture,
	unctrlTroop,
	addBaseLevel,
	changeCastle,
	changeMaster,
	changePlayer,
	darkness_off,
	doGameEnding,
	hideSpotMark,
	moveTroopFix,
	retreatTroop,
	reverseChara,
	setBaseLevel,
	setGameClear,
	setPowerHome,
	showPolitics,
	showSpotMark,
	storeAllSpot,
	addPowerMerce,
	addPowerStaff,
	addTrainingUp,
	changeDungeon,
	pushBaseLevel,
	setEnemyPower,
	setTrainingUp,
	setWorldMusic,
	smoveTroopFix,
	storeAllPower,
	storeComPower,
	storeIndexVar,
	storeNextSpot,
	storeNowPower,
	storeRectUnit,
	storeSkillset,
	storeTodoUnit,
	addPowerMerce2,
	addPowerStaff2,
	changePowerFix,
	eraseUnitTroop,
	pushBattleHome,
	pushBattleRect,
	pushCountPower,
	storeAliveUnit,
	storeAllTalent,
	changePowerFlag,
	changePowerName,
	changeSpotImage,
	erasePowerMerce,
	erasePowerStaff,
	resetEnemyPower,
	resetWorldMusic,
	setDungeonFloor,
	storeBattleSpot,
	storePlayerUnit,
	storeRaceOfUnit,
	storeSpotOfUnit,
	storeUnitOfSpot,
	storeAttackPower,
	storeClassOfUnit,
	storeNeutralSpot,
	storePlayerPower,
	storePowerOfSpot,
	storePowerOfUnit,
	storeSkillOfUnit,
	storeSpotOfPower,
	storeTalentPower,
	storeUnitOfPower,
	clearBattleRecord,
	storeDefensePower,
	storeLeaderOfSpot,
	storeMasterOfUnit,
	storeMemberOfUnit,
	storePowerOfForce,
	storeSpotOfBattle,
	storeLeaderOfPower,
	storeMasterOfPower,
	storePowerOfAttack,
	storeNonPlayerPower,
	storePowerOfDefense,
	storeRoamUnitOfSpot,
	storeBaseClassOfUnit,
	None = uint.MaxValue
}

public static class ActionKindHelper
{
	public static ActionKind Convert(ReadOnlySpan<char> key)
	{
		if (key.Length < 2 || key.Length > 20)
		{
			return ActionKind.None;
		}

		Span<char> span = stackalloc char[key.Length < 4 ? 4 : key.Length];
        key.CopyTo(span);
		span.Slice(key.Length).Clear();
        for (var i = 0; i < key.Length; ++i)
        {
            if (span[i] >= 'a')
            {
                span[i] = (char)(ushort)(span[i] - 32);
            }
        }

        ulong key4 = System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(System.Runtime.InteropServices.MemoryMarshal.Cast<char, byte>(span));
        span = span.Slice(4);
		switch (span.Length)
        {
			case 0 when key4 == 0x0000000000460049UL: return ActionKind.@if;
			case 0 when key4 == 0x0000004600490052UL: return ActionKind.rif;
			case 1 when (key4 == 0x004C004900480057UL) && (span[0] == 'E'): return ActionKind.@while;
			case 0 when key4 == 0x005400580045004EUL: return ActionKind.next;
			case 2 when (key4 == 0x0055005400450052UL) && span.SequenceEqual("RN"): return ActionKind.@return;
			case 1 when (key4 == 0x0041004500520042UL) && (span[0] == 'K'): return ActionKind.@break;
			case 4 when (key4 == 0x0054004E004F0043UL) && span.SequenceEqual("INUE"): return ActionKind.@continue;
			case 0 when key4 == 0x0000000000470042UL: return ActionKind.bg;
			case 0 when key4 == 0x0000000000430056UL: return ActionKind.vc;
			case 0 when key4 == 0x0000004400440041UL: return ActionKind.add;
			case 0 when key4 == 0x0000005600490044UL: return ActionKind.div;
			case 0 when key4 == 0x00000044004F004DUL: return ActionKind.mod;
			case 0 when key4 == 0x000000470053004DUL: return ActionKind.msg;
			case 0 when key4 == 0x0000004C0055004DUL: return ActionKind.mul;
			case 0 when key4 == 0x0000005200450050UL: return ActionKind.per;
			case 0 when key4 == 0x0000005400450053UL: return ActionKind.set;
			case 0 when key4 == 0x0000004200550053UL: return ActionKind.sub;
			case 0 when key4 == 0x0000004E00490057UL: return ActionKind.win;
			case 0 when key4 == 0x0056004400440041UL: return ActionKind.addv;
			case 0 when key4 == 0x004C004C00410043UL: return ActionKind.call;
			case 0 when key4 == 0x0054004100480043UL: return ActionKind.chat;
			case 0 when key4 == 0x0054004900580045UL: return ActionKind.exit;
			case 0 when key4 == 0x0045004300410046UL: return ActionKind.face;
			case 0 when key4 == 0x0054004E004F0046UL: return ActionKind.font;
			case 0 when key4 == 0x003200470053004DUL: return ActionKind.msg2;
			case 0 when key4 == 0x00590041004C0050UL: return ActionKind.play;
			case 0 when key4 == 0x0031004C00500050UL: return ActionKind.ppl1;
			case 0 when key4 == 0x0045005600410053UL: return ActionKind.save;
			case 0 when key4 == 0x0056005400450053UL: return ActionKind.setv;
			case 0 when key4 == 0x0050004F00540053UL: return ActionKind.stop;
			case 0 when key4 == 0x0056004200550053UL: return ActionKind.subv;
			case 0 when key4 == 0x004B004C00410054UL: return ActionKind.talk;
			case 0 when key4 == 0x0054004900410057UL: return ActionKind.wait;
			case 0 when key4 == 0x004D004F004F005AUL: return ActionKind.zoom;
			case 1 when (key4 == 0x0054004100480043UL) && (span[0] == '2'): return ActionKind.chat2;
			case 1 when (key4 == 0x004F005400490043UL) && (span[0] == 'M'): return ActionKind.citom;
			case 1 when (key4 == 0x00410045004C0043UL) && (span[0] == 'R'): return ActionKind.clear;
			case 1 when (key4 == 0x0053004100520045UL) && (span[0] == 'E'): return ActionKind.erase;
			case 1 when (key4 == 0x004E004500560045UL) && (span[0] == 'T'): return ActionKind.@event;
			case 1 when (key4 == 0x0045004300410046UL) && (span[0] == '2'): return ActionKind.face2;
			case 1 when (key4 == 0x00550043004F0046UL) && (span[0] == 'S'): return ActionKind.focus;
			case 1 when (key4 == 0x0054004E004F0046UL) && (span[0] == 'C'): return ActionKind.fontc;
			case 1 when (key4 == 0x0041004500520047UL) && (span[0] == 'D'): return ActionKind.gread;
			case 1 when (key4 == 0x00470041004D0049UL) && (span[0] == 'E'): return ActionKind.image;
			case 1 when (key4 == 0x00450044004E0049UL) && (span[0] == 'X'): return ActionKind.index;
			case 1 when (key4 == 0x0048005300550050UL) && (span[0] == 'V'): return ActionKind.pushv;
			case 1 when (key4 == 0x0050005400450053UL) && (span[0] == 'M'): return ActionKind.setPM;
			case 1 when (key4 == 0x0055005400450053UL) && (span[0] == 'D'): return ActionKind.setud;
			case 1 when (key4 == 0x004B004100480053UL) && (span[0] == 'E'): return ActionKind.shake;
			case 1 when (key4 == 0x004B004C00410054UL) && (span[0] == '2'): return ActionKind.talk2;
			case 1 when (key4 == 0x004C005400490054UL) && (span[0] == 'E'): return ActionKind.title;
			case 2 when (key4 == 0x0053004400440041UL) && span.SequenceEqual("TR"): return ActionKind.addstr;
			case 2 when (key4 == 0x0056004400440041UL) && span.SequenceEqual("AR"): return ActionKind.addVar;
			case 2 when (key4 == 0x0049004F00480043UL) && span.SequenceEqual("CE"): return ActionKind.choice;
			case 2 when (key4 == 0x004C004100490044UL) && span.SequenceEqual("OG"): return ActionKind.dialog;
			case 2 when (key4 == 0x0045004400410046UL) && span.SequenceEqual("IN"): return ActionKind.fadein;
			case 2 when (key4 == 0x0049005200570047UL) && span.SequenceEqual("TE"): return ActionKind.gwrite;
			case 2 when (key4 == 0x00410043004F004CUL) && span.SequenceEqual("TE"): return ActionKind.locate;
			case 2 when (key4 == 0x00590041004C0050UL) && span.SequenceEqual("SE"): return ActionKind.playSE;
			case 2 when (key4 == 0x004F005200430053UL) && span.SequenceEqual("LL"): return ActionKind.scroll;
			case 2 when (key4 == 0x0045004C00450053UL) && span.SequenceEqual("CT"): return ActionKind.select;
			case 2 when (key4 == 0x0042005400450053UL) && span.SequenceEqual("CG"): return ActionKind.setbcg;
			case 2 when (key4 == 0x0056005400450053UL) && span.SequenceEqual("AR"): return ActionKind.setVar;
			case 2 when (key4 == 0x0044004100480053UL) && span.SequenceEqual("OW"): return ActionKind.shadow;
			case 2 when (key4 == 0x0056004200550053UL) && span.SequenceEqual("AR"): return ActionKind.subVar;
			case 2 when (key4 == 0x004C005400490054UL) && span.SequenceEqual("E2"): return ActionKind.title2;
			case 2 when (key4 == 0x0055004C004F0056UL) && span.SequenceEqual("ME"): return ActionKind.volume;
			case 3 when (key4 == 0x0043004400440041UL) && span.SequenceEqual("APA"): return ActionKind.addCapa;
			case 3 when (key4 == 0x0047004400440041UL) && span.SequenceEqual("AIN"): return ActionKind.addGain;
			case 3 when (key4 == 0x0049004400440041UL) && span.SequenceEqual("TEM"): return ActionKind.addItem;
			case 3 when (key4 == 0x0053004400440041UL) && span.SequenceEqual("POT"): return ActionKind.addSpot;
			case 3 when (key4 == 0x0055004400440041UL) && span.SequenceEqual("NIT"): return ActionKind.addUnit;
			case 3 when (key4 == 0x004C004100490044UL) && span.SequenceEqual("OGF"): return ActionKind.dialogF;
			case 3 when (key4 == 0x004B0053004F0044UL) && span.SequenceEqual("ILL"): return ActionKind.doskill;
			case 3 when (key4 == 0x0045004400410046UL) && span.SequenceEqual("OUT"): return ActionKind.fadeout;
			case 3 when (key4 == 0x004500560045004CUL) && span.SequenceEqual("LUP"): return ActionKind.levelup;
			case 3 when (key4 == 0x0050004F004F004CUL) && span.SequenceEqual("BGM"): return ActionKind.loopBGM;
			case 3 when (key4 == 0x0049004E0049004DUL) && span.SequenceEqual("MAP"): return ActionKind.minimap;
			case 3 when (key4 == 0x0054004300490050UL) && span.SequenceEqual("URE"): return ActionKind.picture;
			case 3 when (key4 == 0x00590041004C0050UL) && span.SequenceEqual("BGM"): return ActionKind.playBGM;
			case 3 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("CON"): return ActionKind.pushCon;
			case 3 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("SEX"): return ActionKind.pushSex;
			case 3 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("VAR"): return ActionKind.pushVar;
			case 3 when (key4 == 0x00540055004F0052UL) && span.SequenceEqual("INE"): return ActionKind.routine;
			case 3 when (key4 == 0x004F005200430053UL) && span.SequenceEqual("LL2"): return ActionKind.scroll2;
			case 3 when (key4 == 0x0043005400450053UL) && span.SequenceEqual("APA"): return ActionKind.setCapa;
			case 3 when (key4 == 0x0044005400450053UL) && span.SequenceEqual("ONE"): return ActionKind.setDone;
			case 3 when (key4 == 0x0047005400450053UL) && span.SequenceEqual("AIN"): return ActionKind.setGain;
			case 3 when (key4 == 0x0046005500480053UL) && span.SequenceEqual("FLE"): return ActionKind.shuffle;
			case 3 when (key4 == 0x0050004F00540053UL) && span.SequenceEqual("BGM"): return ActionKind.stopBGM;
			case 3 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EPM"): return ActionKind.storePM;
			case 3 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EUD"): return ActionKind.storeud;
			case 4 when (key4 == 0x0044004400440041UL) && span.SequenceEqual("IPLO"): return ActionKind.addDiplo;
			case 4 when (key4 == 0x004C004400440041UL) && span.SequenceEqual("EVEL"): return ActionKind.addLevel;
			case 4 when (key4 == 0x004C004400440041UL) && span.SequenceEqual("IMIT"): return ActionKind.addLimit;
			case 4 when (key4 == 0x004C004400440041UL) && span.SequenceEqual("OYAL"): return ActionKind.addLoyal;
			case 4 when (key4 == 0x004D004400440041UL) && span.SequenceEqual("ONEY"): return ActionKind.addMoney;
			case 4 when (key4 == 0x0050004400440041UL) && span.SequenceEqual("OWER"): return ActionKind.addPower;
			case 4 when (key4 == 0x0053004400440041UL) && span.SequenceEqual("KILL"): return ActionKind.addSkill;
			case 4 when (key4 == 0x0054004400440041UL) && span.SequenceEqual("ROOP"): return ActionKind.addTroop;
			case 4 when (key4 == 0x0054004400440041UL) && span.SequenceEqual("RUST"): return ActionKind.addTrust;
			case 4 when (key4 == 0x0054004D00490041UL) && span.SequenceEqual("ROOP"): return ActionKind.aimTroop;
			case 4 when (key4 == 0x00410045004C0043UL) && span.SequenceEqual("RVAR"): return ActionKind.clearVar;
			case 4 when (key4 == 0x004B005200410044UL) && span.SequenceEqual("NESS"): return ActionKind.darkness;
			case 4 when (key4 == 0x0054004900580045UL) && span.SequenceEqual("ITEM"): return ActionKind.exitItem;
			case 4 when (key4 == 0x0045004400490048UL) && span.SequenceEqual("LINK"): return ActionKind.hideLink;
			case 4 when (key4 == 0x0045004400490048UL) && span.SequenceEqual("SPOT"): return ActionKind.hideSpot;
			case 4 when (key4 == 0x004B004E0049004CUL) && span.SequenceEqual("SPOT"): return ActionKind.linkSpot;
			case 4 when (key4 == 0x004E00450050004FUL) && span.SequenceEqual("GOAL"): return ActionKind.openGoal;
			case 4 when (key4 == 0x0054004300490050UL) && span.SequenceEqual("URE2"): return ActionKind.picture2;
			case 4 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("CAPA"): return ActionKind.pushCapa;
			case 4 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("GAIN"): return ActionKind.pushGain;
			case 4 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("ITEM"): return ActionKind.pushItem;
			case 4 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("RAND"): return ActionKind.pushRand;
			case 4 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("RANK"): return ActionKind.pushRank;
			case 4 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("SPOT"): return ActionKind.pushSpot;
			case 4 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("TURN"): return ActionKind.pushTurn;
			case 4 when (key4 == 0x004D0041004F0052UL) && span.SequenceEqual("UNIT"): return ActionKind.roamUnit;
			case 4 when (key4 == 0x0044005400450053UL) && span.SequenceEqual("IPLO"): return ActionKind.setDiplo;
			case 4 when (key4 == 0x004C005400450053UL) && span.SequenceEqual("EVEL"): return ActionKind.setLevel;
			case 4 when (key4 == 0x004C005400450053UL) && span.SequenceEqual("IMIT"): return ActionKind.setLimit;
			case 4 when (key4 == 0x004D005400450053UL) && span.SequenceEqual("ONEY"): return ActionKind.setMoney;
			case 4 when (key4 == 0x0054005400450053UL) && span.SequenceEqual("RUCE"): return ActionKind.setTruce;
			case 4 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("CAMP"): return ActionKind.showCamp;
			case 4 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("FACE"): return ActionKind.showFace;
			case 4 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("PICT"): return ActionKind.showPict;
			case 4 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("SPOT"): return ActionKind.showSpot;
			case 4 when (key4 == 0x0054004F00500053UL) && span.SequenceEqual("MARK"): return ActionKind.spotmark;
			case 5 when (key4 == 0x0043004400440041UL) && span.SequenceEqual("ASTLE"): return ActionKind.addCastle;
			case 5 when (key4 == 0x0046004400440041UL) && span.SequenceEqual("RIEND"): return ActionKind.addFriend;
			case 5 when (key4 == 0x004D004400440041UL) && span.SequenceEqual("ERITS"): return ActionKind.addMerits;
			case 5 when (key4 == 0x0053004400440041UL) && span.SequenceEqual("KILL2"): return ActionKind.addSkill2;
			case 5 when (key4 == 0x0053004400440041UL) && span.SequenceEqual("TATUS"): return ActionKind.addStatus;
			case 5 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GEMAP"): return ActionKind.changeMap;
			case 5 when (key4 == 0x00430049004C0043UL) && span.SequenceEqual("KWAIT"): return ActionKind.clickWait;
			case 5 when (key4 == 0x0053004F004C0043UL) && span.SequenceEqual("EGOAL"): return ActionKind.closeGoal;
			case 5 when (key4 == 0x004C005200540043UL) && span.SequenceEqual("TROOP"): return ActionKind.ctrlTroop;
			case 5 when (key4 == 0x00520054004E0045UL) && span.SequenceEqual("YITEM"): return ActionKind.entryItem;
			case 5 when (key4 == 0x0049005500510045UL) && span.SequenceEqual("PITEM"): return ActionKind.equipItem;
			case 5 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("EITEM"): return ActionKind.eraseItem;
			case 5 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("EUNIT"): return ActionKind.eraseUnit;
			case 5 when (key4 == 0x004D0052004F0046UL) && span.SequenceEqual("TROOP"): return ActionKind.formTroop;
			case 5 when (key4 == 0x0045004500520046UL) && span.SequenceEqual("TROOP"): return ActionKind.freeTroop;
			case 5 when (key4 == 0x0054004C00410048UL) && span.SequenceEqual("TROOP"): return ActionKind.haltTroop;
			case 5 when (key4 == 0x0045004400490048UL) && span.SequenceEqual("BLIND"): return ActionKind.hideBlind;
			case 5 when (key4 == 0x0045004400490048UL) && span.SequenceEqual("CHARA"): return ActionKind.hideChara;
			case 5 when (key4 == 0x0045004400490048UL) && span.SequenceEqual("IMAGE"): return ActionKind.hideImage;
			case 5 when (key4 == 0x00450056004F004DUL) && span.SequenceEqual("TROOP"): return ActionKind.moveTroop;
			case 5 when (key4 == 0x00590041004C0050UL) && span.SequenceEqual("WORLD"): return ActionKind.playWorld;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("DEATH"): return ActionKind.pushDeath;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("DIPLO"): return ActionKind.pushDiplo;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("FORCE"): return ActionKind.pushForce;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("LEVEL"): return ActionKind.pushLevel;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("LIMIT"): return ActionKind.pushLimit;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("LOYAL"): return ActionKind.pushLoyal;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("MONEY"): return ActionKind.pushMoney;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("RAND2"): return ActionKind.pushRand2;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("TRAIN"): return ActionKind.pushTrain;
			case 5 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("TRUST"): return ActionKind.pushTrust;
			case 5 when (key4 == 0x0045005300450052UL) && span.SequenceEqual("TTIME"): return ActionKind.resetTime;
			case 5 when (key4 == 0x0045005300450052UL) && span.SequenceEqual("TZONE"): return ActionKind.resetZone;
			case 5 when (key4 == 0x004D0041004F0052UL) && span.SequenceEqual("UNIT2"): return ActionKind.roamUnit2;
			case 5 when (key4 == 0x0041005400450053UL) && span.SequenceEqual("RBEIT"): return ActionKind.setArbeit;
			case 5 when (key4 == 0x0043005400450053UL) && span.SequenceEqual("ASTLE"): return ActionKind.setCastle;
			case 5 when (key4 == 0x004C005400450053UL) && span.SequenceEqual("EAGUE"): return ActionKind.setLeague;
			case 5 when (key4 == 0x0053005400450053UL) && span.SequenceEqual("TATUS"): return ActionKind.setStatus;
			case 5 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("BLIND"): return ActionKind.showBlind;
			case 5 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("CHARA"): return ActionKind.showChara;
			case 5 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("IMAGE"): return ActionKind.showImage;
			case 5 when (key4 == 0x0050004F00540053UL) && span.SequenceEqual("TROOP"): return ActionKind.stopTroop;
			case 5 when (key4 == 0x004D005200450054UL) && span.SequenceEqual("INATE"): return ActionKind.terminate;
			case 5 when (key4 == 0x004C0052004F0057UL) && span.SequenceEqual("DSKIN"): return ActionKind.worldskin;
			case 6 when (key4 == 0x004B004300410042UL) && span.SequenceEqual("SCROLL"): return ActionKind.backScroll;
			case 6 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GERACE"): return ActionKind.changeRace;
			case 6 when (key4 == 0x00490044004E0045UL) && span.SequenceEqual("NGROLL"): return ActionKind.endingRoll;
			case 6 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("EPOWER"): return ActionKind.erasePower;
			case 6 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("ESKILL"): return ActionKind.eraseSkill;
			case 6 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("ETROOP"): return ActionKind.eraseTroop;
			case 6 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("EUNIT2"): return ActionKind.eraseUnit2;
			case 6 when (key4 == 0x0045004400490048UL) && span.SequenceEqual("ESCAPE"): return ActionKind.hideEscape;
			case 6 when (key4 == 0x004B004E0049004CUL) && span.SequenceEqual("ESCAPE"): return ActionKind.linkEscape;
			case 6 when (key4 == 0x00590041004C0050UL) && span.SequenceEqual("BATTLE"): return ActionKind.playBattle;
			case 6 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("CASTLE"): return ActionKind.pushCastle;
			case 6 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("MERITS"): return ActionKind.pushMerits;
			case 6 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("STATUS"): return ActionKind.pushStatus;
			case 6 when (key4 == 0x004F004C00450052UL) && span.SequenceEqual("ADMENU"): return ActionKind.reloadMenu;
			case 6 when (key4 == 0x004F004D00450052UL) && span.SequenceEqual("VESPOT"): return ActionKind.removeSpot;
			case 6 when (key4 == 0x0045005300450052UL) && span.SequenceEqual("TTRUCE"): return ActionKind.resetTruce;
			case 6 when (key4 == 0x0044005400450053UL) && span.SequenceEqual("UNGEON"): return ActionKind.setDungeon;
			case 6 when (key4 == 0x0046004900480053UL) && span.SequenceEqual("TTROOP"): return ActionKind.shiftTroop;
			case 6 when (key4 == 0x0046005500480053UL) && span.SequenceEqual("FLEVAR"): return ActionKind.shuffleVar;
			case 6 when (key4 == 0x004C0049004B0053UL) && span.SequenceEqual("LTROOP"): return ActionKind.skillTroop;
			case 6 when (key4 == 0x00450045004C0053UL) && span.SequenceEqual("PTROOP"): return ActionKind.sleepTroop;
			case 6 when (key4 == 0x0056004F004D0053UL) && span.SequenceEqual("ETROOP"): return ActionKind.smoveTroop;
			case 6 when (key4 == 0x0045004500500053UL) && span.SequenceEqual("DTROOP"): return ActionKind.speedTroop;
			case 6 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EDEATH"): return ActionKind.storeDeath;
			case 6 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EINDEX"): return ActionKind.storeIndex;
			case 6 when (key4 == 0x004F0049004E0055UL) && span.SequenceEqual("NPOWER"): return ActionKind.unionPower;
			case 7 when (key4 == 0x0049005400430041UL) && span.SequenceEqual("VETROOP"): return ActionKind.activeTroop;
			case 7 when (key4 == 0x0054004400440041UL) && span.SequenceEqual("RAINING"): return ActionKind.addTraining;
			case 7 when (key4 == 0x0054005400410042UL) && span.SequenceEqual("LEEVENT"): return ActionKind.battleEvent;
			case 7 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GECLASS"): return ActionKind.changeClass;
			case 7 when (key4 == 0x0049004F00480043UL) && span.SequenceEqual("CETITLE"): return ActionKind.choiceTitle;
			case 7 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("EFRIEND"): return ActionKind.eraseFriend;
			case 7 when (key4 == 0x0045004400490048UL) && span.SequenceEqual("PICTURE"): return ActionKind.hidePicture;
			case 7 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("SPOTPOS"): return ActionKind.pushSpotPos;
			case 7 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("TRAINUP"): return ActionKind.pushTrainUp;
			case 7 when (key4 == 0x004F004D00450052UL) && span.SequenceEqual("VESKILL"): return ActionKind.removeSkill;
			case 7 when (key4 == 0x004F004D00450052UL) && span.SequenceEqual("VETROOP"): return ActionKind.removeTroop;
			case 7 when (key4 == 0x0045005300450052UL) && span.SequenceEqual("TLEAGUE"): return ActionKind.resetLeague;
			case 7 when (key4 == 0x004F005200430053UL) && span.SequenceEqual("LLSPEED"): return ActionKind.scrollSpeed;
			case 7 when (key4 == 0x0054005400450053UL) && span.SequenceEqual("RAINING"): return ActionKind.setTraining;
			case 7 when (key4 == 0x0046004900480053UL) && span.SequenceEqual("TTROOP2"): return ActionKind.shiftTroop2;
			case 7 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("DUNGEON"): return ActionKind.showDungeon;
			case 7 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("PICTURE"): return ActionKind.showPicture;
			case 7 when (key4 == 0x00540043004E0055UL) && span.SequenceEqual("RLTROOP"): return ActionKind.unctrlTroop;
			case 8 when (key4 == 0x0042004400440041UL) && span.SequenceEqual("ASELEVEL"): return ActionKind.addBaseLevel;
			case 8 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GECASTLE"): return ActionKind.changeCastle;
			case 8 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GEMASTER"): return ActionKind.changeMaster;
			case 8 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GEPLAYER"): return ActionKind.changePlayer;
			case 8 when (key4 == 0x004B005200410044UL) && span.SequenceEqual("NESS_OFF"): return ActionKind.darkness_off;
			case 8 when (key4 == 0x00410047004F0044UL) && span.SequenceEqual("MEENDING"): return ActionKind.doGameEnding;
			case 8 when (key4 == 0x0045004400490048UL) && span.SequenceEqual("SPOTMARK"): return ActionKind.hideSpotMark;
			case 8 when (key4 == 0x00450056004F004DUL) && span.SequenceEqual("TROOPFIX"): return ActionKind.moveTroopFix;
			case 8 when (key4 == 0x0052005400450052UL) && span.SequenceEqual("EATTROOP"): return ActionKind.retreatTroop;
			case 8 when (key4 == 0x0045005600450052UL) && span.SequenceEqual("RSECHARA"): return ActionKind.reverseChara;
			case 8 when (key4 == 0x0042005400450053UL) && span.SequenceEqual("ASELEVEL"): return ActionKind.setBaseLevel;
			case 8 when (key4 == 0x0047005400450053UL) && span.SequenceEqual("AMECLEAR"): return ActionKind.setGameClear;
			case 8 when (key4 == 0x0050005400450053UL) && span.SequenceEqual("OWERHOME"): return ActionKind.setPowerHome;
			case 8 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("POLITICS"): return ActionKind.showPolitics;
			case 8 when (key4 == 0x0057004F00480053UL) && span.SequenceEqual("SPOTMARK"): return ActionKind.showSpotMark;
			case 8 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EALLSPOT"): return ActionKind.storeAllSpot;
			case 9 when (key4 == 0x0050004400440041UL) && span.SequenceEqual("OWERMERCE"): return ActionKind.addPowerMerce;
			case 9 when (key4 == 0x0050004400440041UL) && span.SequenceEqual("OWERSTAFF"): return ActionKind.addPowerStaff;
			case 9 when (key4 == 0x0054004400440041UL) && span.SequenceEqual("RAININGUP"): return ActionKind.addTrainingUp;
			case 9 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GEDUNGEON"): return ActionKind.changeDungeon;
			case 9 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("BASELEVEL"): return ActionKind.pushBaseLevel;
			case 9 when (key4 == 0x0045005400450053UL) && span.SequenceEqual("NEMYPOWER"): return ActionKind.setEnemyPower;
			case 9 when (key4 == 0x0054005400450053UL) && span.SequenceEqual("RAININGUP"): return ActionKind.setTrainingUp;
			case 9 when (key4 == 0x0057005400450053UL) && span.SequenceEqual("ORLDMUSIC"): return ActionKind.setWorldMusic;
			case 9 when (key4 == 0x0056004F004D0053UL) && span.SequenceEqual("ETROOPFIX"): return ActionKind.smoveTroopFix;
			case 9 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EALLPOWER"): return ActionKind.storeAllPower;
			case 9 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ECOMPOWER"): return ActionKind.storeComPower;
			case 9 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EINDEXVAR"): return ActionKind.storeIndexVar;
			case 9 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ENEXTSPOT"): return ActionKind.storeNextSpot;
			case 9 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ENOWPOWER"): return ActionKind.storeNowPower;
			case 9 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ERECTUNIT"): return ActionKind.storeRectUnit;
			case 9 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ESKILLSET"): return ActionKind.storeSkillset;
			case 9 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ETODOUNIT"): return ActionKind.storeTodoUnit;
			case 10 when (key4 == 0x0050004400440041UL) && span.SequenceEqual("OWERMERCE2"): return ActionKind.addPowerMerce2;
			case 10 when (key4 == 0x0050004400440041UL) && span.SequenceEqual("OWERSTAFF2"): return ActionKind.addPowerStaff2;
			case 10 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GEPOWERFIX"): return ActionKind.changePowerFix;
			case 10 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("EUNITTROOP"): return ActionKind.eraseUnitTroop;
			case 10 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("BATTLEHOME"): return ActionKind.pushBattleHome;
			case 10 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("BATTLERECT"): return ActionKind.pushBattleRect;
			case 10 when (key4 == 0x0048005300550050UL) && span.SequenceEqual("COUNTPOWER"): return ActionKind.pushCountPower;
			case 10 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EALIVEUNIT"): return ActionKind.storeAliveUnit;
			case 10 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EALLTALENT"): return ActionKind.storeAllTalent;
			case 11 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GEPOWERFLAG"): return ActionKind.changePowerFlag;
			case 11 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GEPOWERNAME"): return ActionKind.changePowerName;
			case 11 when (key4 == 0x004E004100480043UL) && span.SequenceEqual("GESPOTIMAGE"): return ActionKind.changeSpotImage;
			case 11 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("EPOWERMERCE"): return ActionKind.erasePowerMerce;
			case 11 when (key4 == 0x0053004100520045UL) && span.SequenceEqual("EPOWERSTAFF"): return ActionKind.erasePowerStaff;
			case 11 when (key4 == 0x0045005300450052UL) && span.SequenceEqual("TENEMYPOWER"): return ActionKind.resetEnemyPower;
			case 11 when (key4 == 0x0045005300450052UL) && span.SequenceEqual("TWORLDMUSIC"): return ActionKind.resetWorldMusic;
			case 11 when (key4 == 0x0044005400450053UL) && span.SequenceEqual("UNGEONFLOOR"): return ActionKind.setDungeonFloor;
			case 11 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EBATTLESPOT"): return ActionKind.storeBattleSpot;
			case 11 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EPLAYERUNIT"): return ActionKind.storePlayerUnit;
			case 11 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ERACEOFUNIT"): return ActionKind.storeRaceOfUnit;
			case 11 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ESPOTOFUNIT"): return ActionKind.storeSpotOfUnit;
			case 11 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EUNITOFSPOT"): return ActionKind.storeUnitOfSpot;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EATTACKPOWER"): return ActionKind.storeAttackPower;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ECLASSOFUNIT"): return ActionKind.storeClassOfUnit;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ENEUTRALSPOT"): return ActionKind.storeNeutralSpot;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EPLAYERPOWER"): return ActionKind.storePlayerPower;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EPOWEROFSPOT"): return ActionKind.storePowerOfSpot;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EPOWEROFUNIT"): return ActionKind.storePowerOfUnit;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ESKILLOFUNIT"): return ActionKind.storeSkillOfUnit;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ESPOTOFPOWER"): return ActionKind.storeSpotOfPower;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ETALENTPOWER"): return ActionKind.storeTalentPower;
			case 12 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EUNITOFPOWER"): return ActionKind.storeUnitOfPower;
			case 13 when (key4 == 0x00410045004C0043UL) && span.SequenceEqual("RBATTLERECORD"): return ActionKind.clearBattleRecord;
			case 13 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EDEFENSEPOWER"): return ActionKind.storeDefensePower;
			case 13 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ELEADEROFSPOT"): return ActionKind.storeLeaderOfSpot;
			case 13 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EMASTEROFUNIT"): return ActionKind.storeMasterOfUnit;
			case 13 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EMEMBEROFUNIT"): return ActionKind.storeMemberOfUnit;
			case 13 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EPOWEROFFORCE"): return ActionKind.storePowerOfForce;
			case 13 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ESPOTOFBATTLE"): return ActionKind.storeSpotOfBattle;
			case 14 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ELEADEROFPOWER"): return ActionKind.storeLeaderOfPower;
			case 14 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EMASTEROFPOWER"): return ActionKind.storeMasterOfPower;
			case 14 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EPOWEROFATTACK"): return ActionKind.storePowerOfAttack;
			case 15 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("ENONPLAYERPOWER"): return ActionKind.storeNonPlayerPower;
			case 15 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EPOWEROFDEFENSE"): return ActionKind.storePowerOfDefense;
			case 15 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EROAMUNITOFSPOT"): return ActionKind.storeRoamUnitOfSpot;
			case 16 when (key4 == 0x0052004F00540053UL) && span.SequenceEqual("EBASECLASSOFUNIT"): return ActionKind.storeBaseClassOfUnit;
		}

		return ActionKind.None;
	}
}
