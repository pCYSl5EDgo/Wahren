﻿namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    public static bool Parse(ref Context context, ref Result result)
    {
        ref var source = ref result.Source;
        ref var position = ref context.Position;
        ref var tokenList = ref result.TokenList;
        if (source.Count != 0 && source[0].Count != 0 && source[0][0] == '\ufeff')
        {
            source[0].RemoveAt(0);
        }

        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                return true;
            }

            ref var last = ref tokenList.Last;
            ref var lastRange = ref last.Range;
            ref var line = ref source[lastRange.StartInclusive.Line];
            var nextIndex = lastRange.StartInclusive.Offset + 1;
            if (nextIndex >= line.Count)
            {
                if (last.IsBracketRight(ref source))
                {
                    result.ErrorList.Add(new("Too many '}'. It does not have corresponding '{'.", lastRange));
                }
                else
                {
                    result.ErrorList.Add(new("Structure kind or comment start is necessary. This is too short.", lastRange));
                }

                return false;
            }

            var next = line[nextIndex];
            switch (line[nextIndex - 1])
            {
                case 'a' when next == 't' && last.Is_attribute_Skip2(ref source):
                    last.Kind = TokenKind.attribute;
                    if (ParseAttribute(ref context, ref result))
                    {
                        continue;
                    }

                    goto FALSE;
                case 'c':
                    switch (next)
                    {
                        case 'o' when last.Is_context_Skip2(ref source):
                            last.Kind = TokenKind.context;
                            if (ParseContext(ref context, ref result))
                            {
                                continue;
                            }

                            goto FALSE;
                        case 'l' when last.Is_class_Skip2(ref source):
                            last.Kind = TokenKind.@class;
                            if (ParseClass(ref context, ref result))
                            {
                                continue;
                            }

                            goto FALSE;
                    }

                    goto default;
                case 'd':
                    switch (next)
                    {
                        case 'e' when last.Is_detail_Skip2(ref source):
                            last.Kind = TokenKind.detail;
                            if (ParseDetail(ref context, ref result))
                            {
                                continue;
                            }

                            goto FALSE;
                        case 'u' when last.Is_dungeon_Skip2(ref source):
                            last.Kind = TokenKind.dungeon;
                            if (ParseDungeon(ref context, ref result))
                            {
                                continue;
                            }

                            goto FALSE;
                    }

                    goto default;
                case 'e' when next == 'v' && last.Is_event_Skip2(ref source):
                    last.Kind = TokenKind.@event;
                    if (ParseEvent(ref context, ref result))
                    {
                        continue;
                    }

                    return false;
                case 'f' when next == 'i':
                    if (last.Is_field_Skip2(ref source))
                    {
                        last.Kind = TokenKind.field;
                        if (ParseField(ref context, ref result))
                        {
                            continue;
                        }

                        goto FALSE;
                    }
                    else if (last.Is_fight_Skip2(ref source))
                    {
                        last.Kind = TokenKind.fight;
                        if (ParseEvent(ref context, ref result))
                        {
                            continue;
                        }

                        goto FALSE;
                    }

                    goto default;
                case 'm' when next == 'o' && last.Is_movetype_Skip2(ref source):
                    last.Kind = TokenKind.movetype;
                    if (ParseMovetype(ref context, ref result))
                    {
                        continue;
                    }

                    goto FALSE;
                case 'o' when next == 'b' && last.Is_object_Skip2(ref source):
                    last.Kind = TokenKind.@object;
                    if (ParseObject(ref context, ref result))
                    {
                        continue;
                    }

                    goto FALSE;
                case 'p' when next == 'o' && last.Is_power_Skip2(ref source):
                    last.Kind = TokenKind.power;
                    if (ParsePower(ref context, ref result))
                    {
                        continue;
                    }

                    goto FALSE;
                case 'r' when next == 'a' && last.Is_race_Skip2(ref source):
                    last.Kind = TokenKind.race;
                    if (ParseRace(ref context, ref result))
                    {
                        continue;
                    }

                    goto FALSE;
                case 's':
                    switch (next)
                    {
                        case 'c' when last.Is_scenario_Skip2(ref source):
                            last.Kind = TokenKind.scenario;
                            if (ParseScenario(ref context, ref result))
                            {
                                continue;
                            }

                            goto FALSE;
                        case 'k':
                            if (last.Is_skill_Skip2(ref source))
                            {
                                last.Kind = TokenKind.skill;
                                if (ParseSkill(ref context, ref result))
                                {
                                    continue;
                                }

                                goto FALSE;
                            }
                            else if (last.Is_skillset_Skip2(ref source))
                            {
                                last.Kind = TokenKind.skillset;
                                if (ParseSkillset(ref context, ref result))
                                {
                                    continue;
                                }

                                goto FALSE;
                            }

                            break;
                        case 'o' when last.Is_sound_Skip2(ref source):
                            last.Kind = TokenKind.sound;
                            if (ParseSound(ref context, ref result))
                            {
                                continue;
                            }

                            goto FALSE;
                        case 'p' when last.Is_spot_Skip2(ref source):
                            last.Kind = TokenKind.spot;
                            if (ParseSpot(ref context, ref result))
                            {
                                continue;
                            }

                            goto FALSE;
                        case 't' when last.Is_story_Skip2(ref source):
                            last.Kind = TokenKind.story;
                            if (ParseStory(ref context, ref result))
                            {
                                continue;
                            }

                            goto FALSE;
                    }

                    goto default;
                case 'u' when next == 'n' && last.Is_unit_Skip2(ref source):
                    last.Kind = TokenKind.unit;
                    if (ParseUnit(ref context, ref result))
                    {
                        continue;
                    }

                    goto FALSE;
                case 'v' when next == 'o' && last.Is_voice_Skip2(ref source):
                    last.Kind = TokenKind.voice;
                    if (ParseVoice(ref context, ref result))
                    {
                        continue;
                    }

                    goto FALSE;
                case 'w' when next == 'o':
                    if (last.Is_workspace_Skip2(ref source))
                    {
                        last.Kind = TokenKind.workspace;
                        if (ParseWorkspace(ref context, ref result))
                        {
                            continue;
                        }

                        goto FALSE;
                    }
                    else if (last.Is_world_Skip2(ref source))
                    {
                        last.Kind = TokenKind.world;
                        if (ParseEvent(ref context, ref result))
                        {
                            continue;
                        }

                        goto FALSE;
                    }

                    goto default;
                default:
                    result.ErrorList.Add(new($"Structure kind or comment start is necessary. {last.ToString(ref source)}.", lastRange));
                    return false;
            }

        FALSE:
            return false;
        } while (true);
    }
}
