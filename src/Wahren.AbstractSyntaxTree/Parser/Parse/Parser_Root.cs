namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    public static bool Parse(ref Context context, ref Result result, AnalysisResult analysisResult)
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

            ref var line = ref source[tokenList.GetLine(tokenList.LastIndex)];
            var nextIndex = tokenList.GetOffset(tokenList.LastIndex) + 1;
            if (nextIndex >= line.Count)
            {
                if (result.IsBracketRight(tokenList.LastIndex))
                {
                    result.ErrorAdd_TooManyBracketRight();
                }
                else
                {
                    result.ErrorAdd_StructureKindOrCommentIsExpected();
                }

                return false;
            }

            var next = line[nextIndex];
            ref var lastKind = ref tokenList.GetKind(tokenList.LastIndex);
            switch (line[nextIndex - 1])
            {
                case 'a' when next == 't' && result.Is_attribute_Skip2(tokenList.LastIndex):
                    lastKind = TokenKind.attribute;
                    if (ParseAttribute(ref context, ref result, analysisResult))
                    {
                        continue;
                    }

                    goto FALSE;
                case 'c':
                    switch (next)
                    {
                        case 'o' when result.Is_context_Skip2(tokenList.LastIndex):
                            lastKind = TokenKind.context;
                            if (ParseContext(ref context, ref result, analysisResult))
                            {
                                continue;
                            }

                            goto FALSE;
                        case 'l' when result.Is_class_Skip2(tokenList.LastIndex):
                            lastKind = TokenKind.@class;
                            success &= ParseClass(ref context, ref result, analysisResult, out canContinue);
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
                        case 'e' when result.Is_detail_Skip2(tokenList.LastIndex):
                            lastKind = TokenKind.detail;
                            if (ParseDetail(ref context, ref result, analysisResult))
                            {
                                continue;
                            }

                            goto FALSE;
                        case 'u' when result.Is_dungeon_Skip2(tokenList.LastIndex):
                            lastKind = TokenKind.dungeon;
                            success &= ParseDungeon(ref context, ref result, analysisResult, out canContinue);
                            if (canContinue)
                            {
                                continue;
                            }

                            goto FALSE;
                    }

                    goto default;
                case 'e' when next == 'v' && result.Is_event_Skip2(tokenList.LastIndex):
                    lastKind = TokenKind.@event;
                    success &= ParseEvent(ref context, ref result, analysisResult, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    return false;
                case 'f' when next == 'i':
                    if (result.Is_field_Skip2(tokenList.LastIndex))
                    {
                        lastKind = TokenKind.field;
                        success &= ParseField(ref context, ref result, analysisResult, out canContinue);
                        if (canContinue)
                        {
                            continue;
                        }

                        goto FALSE;
                    }
                    else if (result.Is_fight_Skip2(tokenList.LastIndex))
                    {
                        lastKind = TokenKind.fight;
                        success &= ParseEvent(ref context, ref result, analysisResult, out canContinue);
                        if (canContinue)
                        {
                            continue;
                        }

                        goto FALSE;
                    }

                    goto default;
                case 'm' when next == 'o' && result.Is_movetype_Skip2(tokenList.LastIndex):
                    lastKind = TokenKind.movetype;
                    success &= ParseMovetype(ref context, ref result, analysisResult, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 'o' when next == 'b' && result.Is_object_Skip2(tokenList.LastIndex):
                    lastKind = TokenKind.@object;
                    success &= ParseObject(ref context, ref result, analysisResult, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 'p' when next == 'o' && result.Is_power_Skip2(tokenList.LastIndex):
                    lastKind = TokenKind.power;
                    success &= ParsePower(ref context, ref result, analysisResult, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 'r' when next == 'a' && result.Is_race_Skip2(tokenList.LastIndex):
                    lastKind = TokenKind.race;
                    success &= ParseRace(ref context, ref result, analysisResult, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 's':
                    switch (next)
                    {
                        case 'c' when result.Is_scenario_Skip2(tokenList.LastIndex):
                            lastKind = TokenKind.scenario;
                            success &= ParseScenario(ref context, ref result, analysisResult, out canContinue);
                            if (canContinue)
                            {
                                continue;
                            }

                            goto FALSE;
                        case 'k':
                            if (result.Is_skill_Skip2(tokenList.LastIndex))
                            {
                                lastKind = TokenKind.skill;
                                success &= ParseSkill(ref context, ref result, analysisResult, out canContinue);
                                if (canContinue)
                                {
                                    continue;
                                }

                                goto FALSE;
                            }
                            else if (result.Is_skillset_Skip2(tokenList.LastIndex))
                            {
                                lastKind = TokenKind.skillset;
                                success &= ParseSkillset(ref context, ref result, analysisResult, out canContinue);
                                if (canContinue)
                                {
                                    continue;
                                }

                                goto FALSE;
                            }

                            break;
                        case 'o' when result.Is_sound_Skip2(tokenList.LastIndex):
                            lastKind = TokenKind.sound;
                            if (ParseSound(ref context, ref result, analysisResult))
                            {
                                continue;
                            }

                            goto FALSE;
                        case 'p' when result.Is_spot_Skip2(tokenList.LastIndex):
                            lastKind = TokenKind.spot;
                            success &= ParseSpot(ref context, ref result, analysisResult, out canContinue);
                            if (canContinue)
                            {
                                continue;
                            }

                            goto FALSE;
                        case 't' when result.Is_story_Skip2(tokenList.LastIndex):
                            lastKind = TokenKind.story;
                            success &= ParseStory(ref context, ref result, analysisResult, out canContinue);
                            if (canContinue)
                            {
                                continue;
                            }

                            goto FALSE;
                    }

                    goto default;
                case 'u' when next == 'n' && result.Is_unit_Skip2(tokenList.LastIndex):
                    lastKind = TokenKind.unit;
                    success &= ParseUnit(ref context, ref result, analysisResult, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 'v' when next == 'o' && result.Is_voice_Skip2(tokenList.LastIndex):
                    lastKind = TokenKind.voice;
                    success &= ParseVoice(ref context, ref result, analysisResult, out canContinue);
                    if (canContinue)
                    {
                        continue;
                    }

                    goto FALSE;
                case 'w' when next == 'o':
                    if (result.Is_workspace_Skip2(tokenList.LastIndex))
                    {
                        lastKind = TokenKind.workspace;
                        if (ParseWorkspace(ref context, ref result, analysisResult))
                        {
                            continue;
                        }

                        goto FALSE;
                    }
                    else if (result.Is_world_Skip2(tokenList.LastIndex))
                    {
                        lastKind = TokenKind.world;
                        success &= ParseEvent(ref context, ref result, analysisResult, out canContinue);
                        if (canContinue)
                        {
                            continue;
                        }

                        goto FALSE;
                    }

                    goto default;
                default:
                    result.ErrorAdd_StructureKindOrCommentIsExpected();
                    return false;
            }

        FALSE:
            return false;
        } while (true);
    }
}
