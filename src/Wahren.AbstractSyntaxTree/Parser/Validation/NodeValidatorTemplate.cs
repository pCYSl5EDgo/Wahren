﻿// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE .tt FILE INSTEAD.
// </auto-generated>
#nullable enable

namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class NodeValidator
{
	public static bool AddReferenceAndValidate(this ref Result result, ref VoiceNode node)
	{
		bool success = true;
		AddReference(ref result, ref node.voice_type, ref result.VoiceTypeReaderSet, ReferenceKind.VoiceType);
		AddReference(ref result, ref node.delskill, ref result.VoiceTypeReaderSet, ReferenceKind.VoiceType);
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref SpotNode node)
	{
		bool success = true;
		if (!ValidateNumber(ref result, ref node.value, " 'value' of spot requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.x, " 'x' of spot requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.y, " 'y' of spot requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.w, " 'w' of spot requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.h, " 'h' of spot requires Number."))
		{
			success = false;
		}
		if (!ValidateBoolean(ref result, ref node.castle_battle, " 'castle_battle' of spot requires Boolean."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.limit, " 'limit' of spot requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.gain, " 'gain' of spot requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.castle, " 'castle' of spot requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.capacity, " 'capacity' of spot requires Number."))
		{
			success = false;
		}
		AddReference(ref result, ref node.dungeon, ref result.DungeonSet, ReferenceKind.Dungeon);
		if (!ValidateNumber(ref result, ref node.castle_lot, " 'castle_lot' of spot requires Number."))
		{
			success = false;
		}
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref UnitNode node)
	{
		bool success = true;
		AddReference(ref result, ref node.dead_event, ref result.EventSet, ReferenceKind.Event);
		AddReference(ref result, ref node.race, ref result.RaceSet, ReferenceKind.Race);
		AddReference(ref result, ref node.@class, ref result.ClassSet, ReferenceKind.Class);
		if (!ValidateNumber(ref result, ref node.w, " 'w' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.h, " 'h' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.a, " 'a' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.level, " 'level' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hp, " 'hp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mp, " 'mp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.attack, " 'attack' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.defense, " 'defense' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magic, " 'magic' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magdef, " 'magdef' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.speed, " 'speed' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.dext, " 'dext' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.move, " 'move' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hprec, " 'hprec' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mprec, " 'mprec' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.summon_max, " 'summon_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.summon_level, " 'summon_level' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.heal_max, " 'heal_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.attack_max, " 'attack_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.defense_max, " 'defense_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magic_max, " 'magic_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magdef_max, " 'magdef_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.speed_max, " 'speed_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.dext_max, " 'dext_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.move_max, " 'move_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hprec_max, " 'hprec_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mprec_max, " 'mprec_max' of unit requires Number."))
		{
			success = false;
		}
		AddReference(ref result, ref node.consti, ref result.AttributeTypeSet, ReferenceKind.AttributeType);
		AddReference(ref result, ref node.movetype, ref result.MovetypeSet, ReferenceKind.Movetype);
		if (!ValidateNumber(ref result, ref node.attack_range, " 'attack_range' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.escape_range, " 'escape_range' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.escape_run, " 'escape_run' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hand_range, " 'hand_range' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.wake_range, " 'wake_range' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.view_range, " 'view_range' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.cavalry_range, " 'cavalry_range' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.level_max, " 'level_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.exp, " 'exp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.exp_mul, " 'exp_mul' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.exp_max, " 'exp_max' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hpUp, " 'hpUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mpUp, " 'mpUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.attackUp, " 'attackUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.defenseUp, " 'defenseUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magicUp, " 'magicUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magdefUp, " 'magdefUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.speedUp, " 'speedUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.dextUp, " 'dextUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.moveUp, " 'moveUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hprecUp, " 'hprecUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mprecUp, " 'mprecUp' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hpMax, " 'hpMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mpMax, " 'mpMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.attackMax, " 'attackMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.defenseMax, " 'defenseMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magicMax, " 'magicMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magdefMax, " 'magdefMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.speedMax, " 'speedMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.dextMax, " 'dextMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.moveMax, " 'moveMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hprecMax, " 'hprecMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mprecMax, " 'mprecMax' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.fkey, " 'fkey' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.yabo, " 'yabo' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.kosen, " 'kosen' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.brave, " 'brave' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.align, " 'align' of unit requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.loyal, " 'loyal' of unit requires Number."))
		{
			success = false;
		}
		AddReference(ref result, ref node.home, ref result.SpotSet, ReferenceKind.Spot);
		AddReference(ref result, ref node.voice_type, ref result.VoiceTypeReaderSet, ReferenceKind.VoiceType);
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref RaceNode node)
	{
		bool success = true;
		if (!ValidateNumber(ref result, ref node.align, " 'align' of race requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.brave, " 'brave' of race requires Number."))
		{
			success = false;
		}
		AddReference(ref result, ref node.movetype, ref result.MovetypeSet, ReferenceKind.Movetype);
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref ClassNode node)
	{
		bool success = true;
		if (!ValidateNumber(ref result, ref node.medical, " 'medical' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.a, " 'a' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.h, " 'h' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.w, " 'w' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.exp, " 'exp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.cost, " 'cost' of class requires Number."))
		{
			success = false;
		}
		AddReference(ref result, ref node.race, ref result.RaceSet, ReferenceKind.Race);
		if (!ValidateNumber(ref result, ref node.price, " 'price' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.level, " 'level' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hp, " 'hp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mp, " 'mp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.attack, " 'attack' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.defense, " 'defense' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magic, " 'magic' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magdef, " 'magdef' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.speed, " 'speed' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.dext, " 'dext' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.move, " 'move' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hprec, " 'hprec' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mprec, " 'mprec' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.summon_max, " 'summon_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.summon_level, " 'summon_level' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.heal_max, " 'heal_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.attack_max, " 'attack_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.defense_max, " 'defense_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magic_max, " 'magic_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magdef_max, " 'magdef_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.speed_max, " 'speed_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.dext_max, " 'dext_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.move_max, " 'move_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hprec_max, " 'hprec_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mprec_max, " 'mprec_max' of class requires Number."))
		{
			success = false;
		}
		AddReference(ref result, ref node.movetype, ref result.MovetypeSet, ReferenceKind.Movetype);
		if (!ValidateNumber(ref result, ref node.attack_range, " 'attack_range' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.escape_range, " 'escape_range' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.escape_run, " 'escape_run' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hand_range, " 'hand_range' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.wake_range, " 'wake_range' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.view_range, " 'view_range' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.cavalry_range, " 'cavalry_range' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.level_max, " 'level_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.exp_mul, " 'exp_mul' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.exp_max, " 'exp_max' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hpUp, " 'hpUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mpUp, " 'mpUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.attackUp, " 'attackUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.defenseUp, " 'defenseUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magicUp, " 'magicUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magdefUp, " 'magdefUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.speedUp, " 'speedUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.dextUp, " 'dextUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.moveUp, " 'moveUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hprecUp, " 'hprecUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mprecUp, " 'mprecUp' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hpMax, " 'hpMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mpMax, " 'mpMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.attackMax, " 'attackMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.defenseMax, " 'defenseMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magicMax, " 'magicMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.magdefMax, " 'magdefMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.speedMax, " 'speedMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.dextMax, " 'dextMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.moveMax, " 'moveMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.hprecMax, " 'hprecMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.mprecMax, " 'mprecMax' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.brave, " 'brave' of class requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.sortkey, " 'sortkey' of class requires Number."))
		{
			success = false;
		}
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref FieldNode node)
	{
		bool success = true;
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref SkillNode node)
	{
		bool success = true;
		if (!ValidateNumber(ref result, ref node.sortkey, " 'sortkey' of skill requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.w, " 'w' of skill requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.h, " 'h' of skill requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.a, " 'a' of skill requires Number."))
		{
			success = false;
		}
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref PowerNode node)
	{
		bool success = true;
		AddReference(ref result, ref node.master, ref result.UnitSet, ReferenceKind.Unit);
		if (!ValidateNumber(ref result, ref node.money, " 'money' of power requires Number."))
		{
			success = false;
		}
		AddReference(ref result, ref node.home, ref result.SpotSet, ReferenceKind.Spot);
		if (!ValidateNumber(ref result, ref node.training_average, " 'training_average' of power requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.base_loyal, " 'base_loyal' of power requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.yabo, " 'yabo' of power requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.kosen, " 'kosen' of power requires Number."))
		{
			success = false;
		}
		AddReference(ref result, ref node.member, ref result.SpotSet, ReferenceKind.Spot);
		if (!ValidateNumber(ref result, ref node.training_up, " 'training_up' of power requires Number."))
		{
			success = false;
		}
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref ObjectNode node)
	{
		bool success = true;
		if (!ValidateNumber(ref result, ref node.w, " 'w' of object requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.h, " 'h' of object requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.a, " 'a' of object requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.image2_w, " 'image2_w' of object requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.image2_h, " 'image2_h' of object requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.image2_a, " 'image2_a' of object requires Number."))
		{
			success = false;
		}
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref DungeonNode node)
	{
		bool success = true;
		if (!ValidateNumber(ref result, ref node.move_speed, " 'move_speed' of dungeon requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.limit, " 'limit' of dungeon requires Number."))
		{
			success = false;
		}
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref MovetypeNode node)
	{
		bool success = true;
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref SkillsetNode node)
	{
		bool success = true;
		AddReference(ref result, ref node.member, ref result.SkillSet, ReferenceKind.Skill);
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref EventNode node)
	{
		bool success = true;
		if (!ValidateBoolean(ref result, ref node.disperse, " 'disperse' of event requires Boolean."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.w, " 'w' of event requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.h, " 'h' of event requires Number."))
		{
			success = false;
		}
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref ScenarioNode node)
	{
		bool success = true;
		if (!ValidateNumber(ref result, ref node.ws_red, " 'ws_red' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.ws_blue, " 'ws_blue' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.ws_green, " 'ws_green' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.ws_alpha, " 'ws_alpha' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.ws_light, " 'ws_light' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.ws_light_range, " 'ws_light_range' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.max_unit, " 'max_unit' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.locate_x, " 'locate_x' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.locate_y, " 'locate_y' of scenario requires Number."))
		{
			success = false;
		}
		AddReference(ref result, ref node.world, ref result.EventSet, ReferenceKind.Event);
		AddReference(ref result, ref node.fight, ref result.EventSet, ReferenceKind.Event);
		AddReference(ref result, ref node.politics, ref result.EventSet, ReferenceKind.Event);
		if (!ValidateNumber(ref result, ref node.war_capacity, " 'war_capacity' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.spot_capacity, " 'spot_capacity' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.gain_per, " 'gain_per' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.support_range, " 'support_range' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.my_range, " 'my_range' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.myhelp_range, " 'myhelp_range' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.base_level, " 'base_level' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.monster_level, " 'monster_level' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.training_up, " 'training_up' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.actor_per, " 'actor_per' of scenario requires Number."))
		{
			success = false;
		}
		if (!ValidateNumber(ref result, ref node.sortkey, " 'sortkey' of scenario requires Number."))
		{
			success = false;
		}
		return success;
	}

	public static bool AddReferenceAndValidate(this ref Result result, ref StoryNode node)
	{
		bool success = true;
		if (!ValidateBoolean(ref result, ref node.fight, " 'fight' of story requires Boolean."))
		{
			success = false;
		}
		return success;
	}
}
