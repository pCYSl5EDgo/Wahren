namespace Wahren.AbstractSyntaxTree.Parser;

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

        bool success = true;
        bool canContinue;
        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                return success;
            }

            ref var last = ref tokenList.Last;
            ref var line = ref source[last.Position.Line];
            var nextIndex = last.Position.Offset + 1;
            if (nextIndex >= line.Count)
            {
                if (last.IsBracketRight(ref source))
                {
                    result.ErrorAdd("Too many '}'. It does not have corresponding '{'.", tokenList.LastIndex);
                }
                else
                {
                    result.ErrorAdd("Structure kind or comment start is necessary. This is too short.", tokenList.LastIndex);
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
                            success &= ParseClass(ref context, ref result, out canContinue);
                            if (canContinue)
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
                            success &= ParseDungeon(ref context, ref result, out canContinue);
                            if (canContinue)
                            {
                                continue;
                            }

                            goto FALSE;
                    }

                    goto default;
                case 'e' when next == 'v' && last.Is_event_Skip2(ref source):
                    last.Kind = TokenKind.@event;
                    success &= ParseEvent(ref context, ref result, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    return false;
                case 'f' when next == 'i':
                    if (last.Is_field_Skip2(ref source))
                    {
                        last.Kind = TokenKind.field;
                        success &= ParseField(ref context, ref result, out canContinue);
                        if (canContinue)
                        {
                            continue;
                        }

                        goto FALSE;
                    }
                    else if (last.Is_fight_Skip2(ref source))
                    {
                        last.Kind = TokenKind.fight;
                        success &= ParseEvent(ref context, ref result, out canContinue);
                        if (canContinue)
                        {
                            continue;
                        }

                        goto FALSE;
                    }

                    goto default;
                case 'm' when next == 'o' && last.Is_movetype_Skip2(ref source):
                    last.Kind = TokenKind.movetype;
                    success &= ParseMovetype(ref context, ref result, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 'o' when next == 'b' && last.Is_object_Skip2(ref source):
                    last.Kind = TokenKind.@object;
                    success &= ParseObject(ref context, ref result, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 'p' when next == 'o' && last.Is_power_Skip2(ref source):
                    last.Kind = TokenKind.power;
                    success &= ParsePower(ref context, ref result, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 'r' when next == 'a' && last.Is_race_Skip2(ref source):
                    last.Kind = TokenKind.race;
                    success &= ParseRace(ref context, ref result, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 's':
                    switch (next)
                    {
                        case 'c' when last.Is_scenario_Skip2(ref source):
                            last.Kind = TokenKind.scenario;
                            success &= ParseScenario(ref context, ref result, out canContinue);
                            if (canContinue)
                            {
                                continue;
                            }

                            goto FALSE;
                        case 'k':
                            if (last.Is_skill_Skip2(ref source))
                            {
                                last.Kind = TokenKind.skill;
                                success &= ParseSkill(ref context, ref result, out canContinue);
                                if (canContinue)
                                {
                                    continue;
                                }

                                goto FALSE;
                            }
                            else if (last.Is_skillset_Skip2(ref source))
                            {
                                last.Kind = TokenKind.skillset;
                                success &= ParseSkillset(ref context, ref result, out canContinue);
                                if (canContinue)
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
                            success &= ParseSpot(ref context, ref result, out canContinue);
                            if (canContinue)
                            {
                                continue;
                            }

                            goto FALSE;
                        case 't' when last.Is_story_Skip2(ref source):
                            last.Kind = TokenKind.story;
                            success &= ParseStory(ref context, ref result, out canContinue);
                            if (canContinue)
                            {
                                continue;
                            }

                            goto FALSE;
                    }

                    goto default;
                case 'u' when next == 'n' && last.Is_unit_Skip2(ref source):
                    last.Kind = TokenKind.unit;
                    success &= ParseUnit(ref context, ref result, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 'v' when next == 'o' && last.Is_voice_Skip2(ref source):
                    last.Kind = TokenKind.voice;
                    success &= ParseVoice(ref context, ref result, out canContinue);
                    if (canContinue)
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
                        success &= ParseEvent(ref context, ref result, out canContinue);
                        if (canContinue)
                        {
                            continue;
                        }

                        goto FALSE;
                    }

                    goto default;
                default:
                    result.ErrorAdd($"Structure kind or comment start is necessary. {result.GetSpan(tokenList.LastIndex)}.", tokenList.LastIndex);
                    return false;
            }

        FALSE:
            return false;
        } while (true);
    }
}
