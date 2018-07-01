using System;
using System.Collections.Generic;
using System.Linq;

namespace Wahren
{
    public static partial class ScriptLoader
    {
        internal static void Parse<T>(this IEnumerable<LexicalTree_Assign> enumerable, T data) where T : ScenarioVariantData
        {
            switch (data)
            {
                case SpotData _1:
                    enumerable.Parse(_1);
                    break;
                case PowerData _2:
                    enumerable.Parse(_2);
                    break;
                case GenericUnitData _3:
                    Parse(_3);
                    break;
                case UnitData _4:
                    Parse(_4);
                    break;
                case RaceData _5:
                    enumerable.Parse(_5);
                    break;
                case SkillData _6:
                    enumerable.Parse(_6);
                    break;
                case SkillSetData _7:
                    enumerable.Parse(_7);
                    break;
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree_Assign> enumerable, SkillData skill)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        skill.DisplayName = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "icon":
                        skill.Icon.Clear();
                        skill.IconAlpha.Clear();
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add("icon");
                            break;
                        }
                        skill.FilledWithNull.Remove("icon");
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            if (i + 1 < assign.Content.Count && assign.Content[i + 1].Type == 2)
                            {
                                skill.Icon.Add(Intern(assign.Content[i].Content));
                                skill.IconAlpha.Add((byte)assign.Content[i + 1].Number);
                                ++i;
                            }
                            else
                            {
                                skill.Icon.Add(Intern(assign.Content[i].Content));
                                skill.IconAlpha.Add(255);
                            }
                        }
                        break;
                    case "fkey":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Fkey = null;
                            skill.FkeyNumber = null;
                            skill.FilledWithNull.Add("fkey");
                            continue;
                        }
                        skill.FilledWithNull.Remove("fkey");
                        skill.Fkey = assign.Content[0].ToLowerString();
                        if (assign.Content.Count == 2)
                        { skill.FkeyNumber = (int)assign.Content[1].Number; }
                        else { skill.FkeyNumber = 0; }
                        break;
                    case "sortkey":
                        skill.SortKey = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "special":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Special = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                            skill.Special = (int)assign.Content[0].Number;
                        else if (assign.Content[0].ToLowerString() == "on")
                            skill.Special = 1;
                        else skill.Special = 0;
                        break;
                    case "gun_delay":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.GunDelay = null;
                            skill.GunDelayName = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.GunDelay = (int)assign.Content[0].Number;
                            skill.GunDelayName = skill.Name;
                        }
                        else
                        {
                            skill.GunDelayName = assign.Content[0].Content;
                            skill.GunDelay = (int)assign.Content[1].Number;
                        }
                        break;
                    case "quickreload":
                        skill.QuickReload = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "help":
                        skill.Help = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "hide_help":
                        skill.HideHelp = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "sound":
                        InsertStringOnlyList(assign, skill.FilledWithNull, skill.Sound);
                        break;
                    case "msg":
                        skill.Message = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "cutin":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.CutinAlpha = null;
                            skill.CutinFlashR = null;
                            skill.CutinFlashG = null;
                            skill.CutinFlashB = null;
                            skill.CutinInflate = null;
                            skill.CutinOn = null;
                            skill.CutinPhantom = null;
                            skill.CutinSlide = null;
                            skill.CutinStop = null;
                            skill.CutinTop = null;
                            skill.CutinTrans = null;
                            skill.CutinType = null;
                            skill.CutinWakeTime = null;
                            skill.CutinY = null;
                            skill.CutinY2 = null;
                            skill.CutinZoom = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i < assign.Content.Count; i += 2)
                        {
                            switch (assign.Content[i].ToLowerString())
                            {
                                case "type":
                                    skill.CutinType = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "top":
                                    skill.CutinTop = (int)assign.Content[i + 1].Number;
                                    break;
                                case "y":
                                    skill.CutinY = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "y2":
                                    skill.CutinY2 = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "stop":
                                    skill.CutinStop = true;
                                    --i;
                                    break;
                                case "wake_time":
                                    skill.CutinWakeTime = (int)assign.Content[i + 1].Number;
                                    break;
                                case "flash":
                                    var n = (int)assign.Content[i + 1].Number;
                                    var r = n / 100;
                                    var g = (n - 100 * r) / 10;
                                    var b = n - 100 * r - 10 * g;
                                    switch (r)
                                    {
                                        case 0:
                                            skill.CutinFlashR = 0;
                                            break;
                                        case 1:
                                            skill.CutinFlashR = 28;
                                            break;
                                        case 2:
                                            skill.CutinFlashR = 56;
                                            break;
                                        case 3:
                                            skill.CutinFlashR = 85;
                                            break;
                                        case 4:
                                            skill.CutinFlashR = 113;
                                            break;
                                        case 5:
                                            skill.CutinFlashR = 141;
                                            break;
                                        case 6:
                                            skill.CutinFlashR = 170;
                                            break;
                                        case 7:
                                            skill.CutinFlashR = 198;
                                            break;
                                        case 8:
                                            skill.CutinFlashR = 227;
                                            break;
                                        case 9:
                                            skill.CutinFlashR = 255;
                                            break;
                                    }
                                    switch (g)
                                    {
                                        case 0:
                                            skill.CutinFlashG = 0;
                                            break;
                                        case 1:
                                            skill.CutinFlashG = 28;
                                            break;
                                        case 2:
                                            skill.CutinFlashG = 56;
                                            break;
                                        case 3:
                                            skill.CutinFlashG = 85;
                                            break;
                                        case 4:
                                            skill.CutinFlashG = 113;
                                            break;
                                        case 5:
                                            skill.CutinFlashG = 141;
                                            break;
                                        case 6:
                                            skill.CutinFlashG = 170;
                                            break;
                                        case 7:
                                            skill.CutinFlashG = 198;
                                            break;
                                        case 8:
                                            skill.CutinFlashG = 227;
                                            break;
                                        case 9:
                                            skill.CutinFlashG = 255;
                                            break;
                                    }
                                    switch (b)
                                    {
                                        case 0:
                                            skill.CutinFlashB = 0;
                                            break;
                                        case 1:
                                            skill.CutinFlashB = 28;
                                            break;
                                        case 2:
                                            skill.CutinFlashB = 56;
                                            break;
                                        case 3:
                                            skill.CutinFlashB = 85;
                                            break;
                                        case 4:
                                            skill.CutinFlashB = 113;
                                            break;
                                        case 5:
                                            skill.CutinFlashB = 141;
                                            break;
                                        case 6:
                                            skill.CutinFlashB = 170;
                                            break;
                                        case 7:
                                            skill.CutinFlashB = 198;
                                            break;
                                        case 8:
                                            skill.CutinFlashB = 227;
                                            break;
                                        case 9:
                                            skill.CutinFlashR = 255;
                                            break;
                                    }
                                    break;
                                case "phantom":
                                    skill.CutinPhantom = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "alpha":
                                    skill.CutinAlpha = (byte)assign.Content[i + 1].Number;
                                    break;
                                case "zoom":
                                    skill.CutinZoom = (int)assign.Content[i + 1].Number;
                                    break;
                                case "inflate":
                                    skill.CutinInflate = (int)assign.Content[i + 1].Number;
                                    break;
                                case "slide":
                                    skill.CutinSlide = (int)assign.Content[i + 1].Number;
                                    break;
                                case "trans":
                                    skill.CutinTrans = (int)assign.Content[i + 1].Number;
                                    break;
                            }
                        }
                        break;
                    case "value":
                        skill.Value = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "talent":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Talent = null;
                            skill.TalentSkill = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (assign.Content[0].Content.ToLower())
                        {
                            case "off":
                                skill.Talent = false;
                                skill.TalentSkill = null;
                                break;
                            case "on":
                                skill.Talent = true;
                                skill.TalentSkill = null;
                                break;
                            default:
                                skill.Talent = true;
                                skill.TalentSkill = assign.Content[0].Content;
                                break;
                        }
                        break;
                    case "exp_per":
                        skill.ExpPercentage = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "mp":
                        skill.MP = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "image":
                        skill.Image = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "w":
                        skill.Width = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "h":
                        skill.Height = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "a":
                        skill.Alpha = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "alpha_tip":
                        skill.AlphaTip = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "alpha_butt":
                        skill.AlphaButt = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "anime":
                        skill.Anime = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "anime_interval":
                        skill.AnimeInterval = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "center":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Center = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (assign.Content[0].Content.ToLower())
                        {
                            case "on":
                                skill.Center = 1;
                                break;
                            case "end":
                                skill.Center = 2;
                                break;
                            default:
                                skill.Center = 0;
                                break;
                        }
                        break;
                    case "ground":
                        skill.Ground = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "d360":
                        skill.D360 = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "rotate":
                        skill.Rotate = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "d360_adj":
                        skill.D360Adjust = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "direct":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Direct = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (assign.Content[0].Content.ToLower())
                        {
                            case "on":
                                skill.Direct = 1;
                                break;
                            case "roll":
                                skill.Direct = 2;
                                break;
                            default:
                                skill.Direct = 0;
                                break;
                        }
                        break;
                    case "resize_a":
                        skill.ResizeA = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "resize_h":
                        skill.ResizeH = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_interval":
                        skill.ResizeInterval = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_reverse":
                        skill.ResizeReverse = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_s":
                        skill.ResizeS = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_start":
                        skill.ResizeStart = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "resize_w":
                        skill.ResizeW = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "force_fire":
                        skill.ForceFire = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "slow_per":
                        skill.SlowPercentage = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slow_time":
                        skill.SlowTime = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slide":
                        skill.Slide = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slide_delay":
                        skill.SlideDelay = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slide_speed":
                        skill.SlideSpeed = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "slide_stamp":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.SlideStamp = null;
                            skill.SlideStampOn = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.SlideStampOn = true;
                            skill.Slide = (int)assign.Content[0].Number;
                        }
                        else if (assign.Content[0].Content.ToLower() == "on")
                        {
                            skill.SlideStampOn = true;
                        }
                        break;
                    case "wait_time":
                        skill.WaitTimeMin = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "wait_time2":
                        skill.WaitTimeMax = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "shake":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Shake = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                            skill.Shake = (int)assign.Content[0].Number;
                        else if (assign.Content[0].Content.ToLower() == "on")
                            skill.Shake = 30;
                        else skill.Shake = 0;
                        break;
                    case "ray":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.RayA = null;
                            skill.RayR = null;
                            skill.RayG = null;
                            skill.RayB = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content.Count == 4)
                        {
                            skill.RayA = (byte)assign.Content[0].Number;
                            skill.RayR = (byte)assign.Content[1].Number;
                            skill.RayG = (byte)assign.Content[2].Number;
                            skill.RayB = (byte)assign.Content[3].Number;
                        }
                        else
                        {
                            skill.RayA = 255;
                            skill.RayR = (byte)assign.Content[0].Number;
                            skill.RayG = (byte)assign.Content[1].Number;
                            skill.RayB = (byte)assign.Content[2].Number;
                        }
                        break;
                    case "force_ray":
                        skill.ForceRay = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "flash":
                        skill.Flash = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "flash_anime":
                        skill.FlashAnime = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "flash_image":
                        skill.FlashImage = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "collision":
                        skill.Collision = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "afterdeath":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.AfterDeath = null;
                            skill.AfterDeathType = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        skill.AfterDeath = assign.Content[0].Content;
                        if (assign.Content.Count == 2) skill.AfterDeathType = (byte)assign.Content[1].Number;
                        break;
                    case "afterhit":
                        skill.AfterHit = null;
                        skill.AfterHitType = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        skill.AfterHit = assign.Content[0].Content;
                        if (assign.Content.Count == 2) skill.AfterHitType = (byte)assign.Content[1].Number;
                        break;
                    case "yorozu":
                        skill.YorozuAttribute.Clear();
                        skill.YorozuRadius = null;
                        skill.YorozuThrowMax = null;
                        skill.YorozuTurn = null;
                        skill.TroopType = null;
                        skill.EffectHeight = null;
                        skill.EffectWidth = null;
                        skill.YorozuAttack = null;
                        skill.YorozuConf = null;
                        skill.YorozuDeath = null;
                        skill.YorozuDefense = null;
                        skill.YorozuDext = null;
                        skill.YorozuDrain = null;
                        skill.YorozuFear = null;
                        skill.YorozuHp = null;
                        skill.YorozuHprec = null;
                        skill.YorozuIll = null;
                        skill.YorozuMagdef = null;
                        skill.YorozuMagic = null;
                        skill.YorozuMagsuck = null;
                        skill.YorozuMove = null;
                        skill.YorozuMp = null;
                        skill.YorozuMprec = null;
                        skill.YorozuPara = null;
                        skill.YorozuPoi = null;
                        skill.YorozuRadius = null;
                        skill.YorozuSil = null;
                        skill.YorozuSpeed = null;
                        skill.YorozuStone = null;
                        skill.YorozuSuck = null;
                        skill.YorozuSummonMax = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            if (assign.Content[i].Type != 0) continue;
                            try
                            {
                                switch (assign.Content[i].Content)
                                {
                                    case "type":
                                        skill.YorozuTurn = assign.Content[i + 1].Number == 2;
                                        break;
                                    case "radius":
                                        skill.YorozuRadius = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "thm":
                                        skill.YorozuThrowMax = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "w":
                                        skill.EffectWidth = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "h":
                                        skill.EffectHeight = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "troop":
                                        skill.TroopType = 0;
                                        break;
                                    case "troop2":
                                        skill.TroopType = 1;
                                        break;
                                    case "troop3":
                                        skill.TroopType = 2;
                                        break;
                                    case "hp":
                                        skill.YorozuHp = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "mp":
                                        skill.YorozuMp = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "attack":
                                        skill.YorozuAttack = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "defense":
                                        skill.YorozuDefense = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "magic":
                                        skill.YorozuMagic = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "magdef":
                                        skill.YorozuMagdef = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "speed":
                                        skill.YorozuSpeed = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "dext":
                                        skill.YorozuDext = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "move":
                                        skill.YorozuMove = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "hprec":
                                        skill.YorozuHprec = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "mprec":
                                        skill.YorozuMprec = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "summon_max":
                                        skill.YorozuSummonMax = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "poi":
                                        skill.YorozuPoi = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "para":
                                        skill.YorozuPara = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "ill":
                                        skill.YorozuIll = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "conf":
                                        skill.YorozuConf = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "sil":
                                        skill.YorozuSil = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "stone":
                                        skill.YorozuStone = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "fear":
                                        skill.YorozuFear = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "suck":
                                        skill.YorozuSuck = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "magsuck":
                                        skill.YorozuMagsuck = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "drain":
                                        skill.YorozuDrain = (int)assign.Content[i + 1].Number;
                                        break;
                                    case "death":
                                        skill.YorozuDeath = (int)assign.Content[i + 1].Number;
                                        break;
                                    default:
                                        skill.YorozuAttribute[assign.Content[i].ToLowerString()] = (int)assign.Content[i + 1].Number;
                                        break;
                                }
                            }
                            catch
                            {
                                Console.Error.WriteLine(i + 1);
                                Console.Error.WriteLine(assign.DebugInfo);
                            }
                        }
                        break;
                    case "str":
                        skill.Strength = StrType.None;
                        skill.StrPercent = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (assign.Content[0].Content)
                        {
                            case "attack":
                                skill.Strength = StrType.Attack;
                                break;
                            case "attack_dext":
                                skill.Strength = StrType.AttackDext;
                                break;
                            case "attack_magic":
                                skill.Strength = StrType.AttackMagic;
                                break;
                            case "fix":
                                skill.Strength = StrType.Fix;
                                break;
                            case "magic_dext":
                                skill.Strength = StrType.MagicDext;
                                break;
                            case "magic":
                                skill.Strength = StrType.Magic;
                                break;
                            case "none":
                                skill.Strength = StrType.None;
                                skill.StrPercent = null;
                                continue;
                            default: throw new ApplicationException();
                        }
                        if (assign.Content.Count >= 2)
                            skill.StrPercent = (int)assign.Content[1].Number;
                        break;
                    case "str_ratio":
                        skill.StrRatio = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "attr":
                        skill.Attribute = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "add":
                    case "add2":
                        InsertStringList(assign, skill.FilledWithNull, skill.Add);
                        break;
                    case "add_all":
                        skill.AddAll = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "add_per":
                        skill.AddPercentage = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "damage":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.DamageType = null;
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.DamageType = (int)assign.Content[0].Number;
                            continue;
                        }
                        switch (assign.Content[0].Content)
                        {
                            case "off":
                                skill.DamageType = -2;
                                break;
                            case "pass":
                                skill.DamageType = -3;
                                break;
                            default: throw new ScriptLoadingException(assign);
                        }
                        break;
                    case "damage_range_adjust":
                        skill.DamageRangeAdjust = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "attack_us":
                        skill.AttackUs = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "allfunc":
                        skill.AllFunc = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "bom":
                        skill.Bom = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "homing":
                        skill.HomingNumber = null;
                        skill.HomingOn = null;
                        skill.HomingType = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.HomingOn = true;
                            skill.HomingNumber = (int)assign.Content[0].Number;
                            skill.HomingType = 0;
                            continue;
                        }
                        switch (assign.Content[0].Content)
                        {
                            case "on":
                                skill.HomingType = 1;
                                skill.HomingOn = true;
                                break;
                            case "origin":
                                skill.HomingType = 2;
                                skill.HomingOn = true;
                                break;
                            case "fix":
                                skill.HomingType = 3;
                                skill.HomingOn = true;
                                break;
                            case "hold":
                                skill.HomingType = 4;
                                skill.HomingOn = true;
                                break;
                            case "off":
                                skill.HomingOn = false;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "forward":
                        skill.Forward = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "far":
                        skill.Far = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "hard":
                        skill.Hard = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "onehit":
                        skill.OneHit = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "hard2":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.Hard2 = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.Hard2 = (byte)assign.Content[0].Number;
                            continue;
                        }
                        switch (assign.Content[0].Content)
                        {
                            case "on":
                                skill.Hard2 = 1;
                                break;
                            default:
                                skill.Hard2 = 0;
                                break;
                        }
                        break;
                    case "offset":
                        skill.Offset.Clear();
                        skill.OffsetOn = false;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (assign.Content[0].Content)
                        {
                            case "on":
                                skill.OffsetOn = true;
                                continue;
                            case "off":
                                continue;
                        }
                        skill.OffsetOn = true;
                        foreach (var item in assign.Content)
                            skill.Offset.Add(item.ToLowerString());
                        break;
                    case "offset_attr":
                        InsertStringOnlyList(assign, skill.FilledWithNull, skill.OffsetAttribute);
                        break;
                    case "knock":
                        skill.Knock = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "knock_speed":
                        skill.KnockSpeed = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "knock_power":
                        skill.KnockPower = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "range":
                        skill.Range = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "range_min":
                        skill.RangeMin = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "check":
                        skill.Check = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "speed":
                        skill.Speed = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "origin":
                        skill.Origin = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "random_space":
                        skill.RandomSpace = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "random_space_min":
                        skill.RandomSpaceMin = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "time":
                        skill.Time = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "height":
                        skill.Height_Charge_Arc = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "rush":
                        skill.Rush = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "rush_degree":
                        skill.RushDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "rush_interval":
                        skill.RushInterval = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "rush_random_degree":
                        skill.RushRandomDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "follow":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.FollowOn = null;
                            skill.Follow = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        if (assign.Content[0].Type == 2)
                        {
                            skill.FollowOn = true;
                            skill.Follow = (int)assign.Content[0].Number;
                        }
                        else if (assign.Content[0].Content == "on")
                        {
                            skill.FollowOn = true;
                            skill.Follow = null;
                        }
                        else
                        {
                            skill.FollowOn = false;
                            skill.Follow = null;
                        }
                        break;
                    case "start_degree":
                        skill.StartDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "start_random_degree":
                        skill.StartRandomDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "start_degree_type":
                        skill.StartDegreeType = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "start_degree_turnunit":
                        skill.StartDegreeTurnUnit = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "start_degree_fix":
                        skill.StartDegreeFix = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "homing2":
                        skill.Homing2 = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "drop_degree":
                        skill.DropDegree = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "drop_degree2":
                        skill.DropDegreeMax = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "joint_skill":
                        skill.JointSkill = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "send_target":
                        skill.SendTarget = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "send_image_degree":
                        skill.SendImageDegree = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "next":
                        skill.Next = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "next2":
                        InsertStringList(assign, skill.FilledWithNull, skill.Next2);
                        break;
                    case "next3":
                        InsertStringList(assign, skill.FilledWithNull, skill.Next3);
                        break;
                    case "next4":
                        skill.Next4 = InsertString(assign, skill.FilledWithNull);
                        break;
                    case "next_order":
                        skill.NextOrder = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "next_last":
                        skill.NextLast = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "next_first":
                        skill.NextFirst = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "just_next":
                        InsertStringList(assign, skill.FilledWithNull, skill.JustNext);
                        break;
                    case "pair_next":
                        InsertStringList(assign, skill.FilledWithNull, skill.PairNext);
                        break;
                    case "item_type":
                        skill.ItemType = InsertByte(assign, skill.FilledWithNull);
                        break;
                    case "item_sort":
                        skill.ItemSort = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "item_nosell":
                        skill.ItemNoSell = InsertBool(assign, skill.FilledWithNull);
                        break;
                    case "price":
                        skill.Price = InsertInt(assign, skill.FilledWithNull);
                        break;
                    case "friend":
                        InsertStringOnlyList(assign, skill.FilledWithNull, skill.Friend);
                        break;
                    case "movetype":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            skill.MoveType = null;
                            skill.FilledWithNull.Add(assign.Name);
                            continue;
                        }
                        skill.FilledWithNull.Remove(assign.Name);
                        switch (assign.Content[0].Content)
                        {
                            case "arc":
                                skill.MoveType = 1;
                                break;
                            case "drop":
                                skill.MoveType = 3;
                                break;
                            case "throw":
                                skill.MoveType = 2;
                                break;
                            case "circle":
                                skill.MoveType = 4;
                                break;
                            case "swing":
                                skill.MoveType = 5;
                                break;
                            case "missile":
                                skill.MoveType = 6;
                                break;
                            case "normal":
                                skill.MoveType = 7;
                                break;
                            case "none":
                                skill.MoveType = 0;
                                break;
                            default: throw new ScriptLoadingException(assign);
                        }
                        break;
                    case "type":
                        skill.StatusType = InsertByte(assign, skill.FilledWithNull);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, skill.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(this List<LexicalTree> children, StoryData story)
        {
            foreach (var tree in children)
            {
                var assign = tree as LexicalTree_Assign;
                if (assign == null)
                {
                    story.Script.Add(tree);
                    continue;
                }
                switch (assign.Name)
                {
                    case "friend":
                        InsertStringOnlyList(assign, story.FilledWithNull, story.Friend);
                        break;
                    case "fight":
                        story.Fight = InsertBool(assign, story.FilledWithNull);
                        break;
                    case "politics":
                        story.Politics = InsertBool(assign, story.FilledWithNull);
                        break;
                    case "pre":
                        story.Pre = InsertBool(assign, story.FilledWithNull);
                        break;
                    default: throw new ApplicationException();
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree_Assign> enumerable, FieldData field)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "type":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            field.Type = FieldData.ChipType.None;
                            continue;
                        }
                        switch (assign.Content[0].Content)
                        {
                            case "coll":
                                field.Type = FieldData.ChipType.coll;
                                break;
                            case "wall":
                                field.Type = FieldData.ChipType.wall;
                                break;
                            case "wall2":
                                field.Type = FieldData.ChipType.wall2;
                                break;
                        }
                        break;
                    case "attr":
                        field.Attribute = InsertString(assign, field.FilledWithNull);
                        break;
                    case "color":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            field.ColorB = null;
                            field.ColorG = null;
                            field.ColorR = null;
                            field.FilledWithNull.Add("color");
                            continue;
                        }
                        field.FilledWithNull.Remove("color");
                        field.ColorR = (byte)assign.Content[0].Number;
                        field.ColorG = (byte)assign.Content[1].Number;
                        field.ColorB = (byte)assign.Content[2].Number;
                        break;
                    case "id":
                        field.BaseId = InsertString(assign, field.FilledWithNull);
                        break;
                    case "edge":
                        field.IsEdge = InsertBool(assign, field.FilledWithNull);
                        break;
                    case "joint":
                        field.JointChip.Clear();
                        field.JointImage.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            field.FilledWithNull.Add("joint");
                            continue;
                        }
                        field.FilledWithNull.Remove("joint");
                        if (assign.Content.Count == 1)
                        {
                            field.JointChip.Add(assign.Content[0].ToLowerString());
                        }
                        else
                        {
                            for (int i = 0; i < assign.Content.Count; i += 2)
                            {
                                field.JointChip.Add(assign.Content[i].ToLowerString());
                                field.JointImage.Add(assign.Content[i + 1].ToLowerString());
                            }
                        }
                        break;
                    case "image":
                        field.ImageNameRandomList.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            field.FilledWithNull.Add("image");
                            continue;
                        }
                        field.FilledWithNull.Remove("image");
                        field.ImageNameRandomList.Add(assign.Content[0].Content);
                        break;
                    case "add2":
                        AddStringIntPair(assign, field.FilledWithNull, field.ImageNameRandomList);
                        break;
                    case "member":
                        AddStringIntPair(assign, field.FilledWithNull, field.MemberRandomList);
                        break;
                    case "alt":
                        field.Alt_min = InsertInt(assign, field.FilledWithNull);
                        break;
                    case "alt_max":
                        field.Alt_max = InsertInt(assign, field.FilledWithNull);
                        break;
                    case "smooth":
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "@":
                            case "on":
                                field.SmoothType = FieldData.Smooth.on;
                                break;
                            case "off":
                                field.SmoothType = FieldData.Smooth.off;
                                break;
                            case "step":
                                field.SmoothType = FieldData.Smooth.step;
                                break;
                        }
                        break;
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree_Assign> enumerable, ObjectData object1)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "land_base":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            object1.FilledWithNull.Add("land_base");
                            object1.IsLandBase = null;
                            object1.LandBase = null;
                            continue;
                        }
                        object1.FilledWithNull.Add("land_base");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                object1.IsLandBase = true;
                                object1.LandBase = 0;
                                break;
                            case "off":
                                object1.IsLandBase = false;
                                break;
                            default:
                                object1.IsLandBase = false;
                                object1.LandBase = (int)assign.Content[0].Number;
                                break;
                        }
                        break;
                    case "type":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            object1.Type = ObjectData.ChipType.None;
                            continue;
                        }
                        switch (assign.Content[0].Content)
                        {
                            case "coll":
                                object1.Type = ObjectData.ChipType.coll;
                                break;
                            case "wall":
                                object1.Type = ObjectData.ChipType.wall;
                                break;
                            case "wall2":
                                object1.Type = ObjectData.ChipType.wall2;
                                break;
                            case "break":
                                object1.Type = ObjectData.ChipType.breakable;
                                break;
                            case "gate":
                            case "break2":
                                object1.Type = ObjectData.ChipType.gate;
                                break;
                            case "floor":
                                object1.Type = ObjectData.ChipType.floor;
                                break;
                            case "start":
                                object1.Type = ObjectData.ChipType.start;
                                break;
                            case "goal":
                                object1.Type = ObjectData.ChipType.goal;
                                break;
                            case "box":
                                object1.Type = ObjectData.ChipType.box;
                                break;
                            case "cover":
                                object1.Type = ObjectData.ChipType.cover;
                                break;
                            default: throw new ScriptLoadingException(assign);
                        }
                        break;
                    case "no_stop":
                        object1.NoStop = InsertBool(assign, object1.FilledWithNull);
                        break;
                    case "no_wall2":
                        object1.NoWall2 = InsertByte(assign, object1.FilledWithNull);
                        break;
                    case "no_arc_hit":
                        object1.NoArcHit = InsertBool(assign, object1.FilledWithNull);
                        break;
                    case "radius":
                        object1.Radius = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "blk":
                        object1.Blk = InsertBool(assign, object1.FilledWithNull);
                        break;
                    case "w":
                        object1.Width = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "h":
                        object1.Height = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "a":
                        object1.Alpha = InsertByte(assign, object1.FilledWithNull);
                        break;
                    case "image":
                        object1.ImageNameRandomList.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            object1.FilledWithNull.Add("image");
                            continue;
                        }
                        object1.FilledWithNull.Remove("image");
                        object1.ImageNameRandomList.Add(assign.Content[0].ToLowerString());
                        break;
                    case "member":
                        AddStringIntPair(assign, object1.FilledWithNull, object1.ImageNameRandomList);
                        break;
                    case "image2":
                        object1.Image2Name = InsertString(assign, object1.FilledWithNull);
                        break;
                    case "image2_w":
                        object1.Image2Width = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "image2_h":
                        object1.Image2Height = InsertInt(assign, object1.FilledWithNull);
                        break;
                    case "image2_a":
                        object1.Image2Alpha = InsertByte(assign, object1.FilledWithNull);
                        break;
                    case "ground":
                        object1.IsGound = InsertBool(assign, object1.FilledWithNull);
                        break;
                    default:
                        break;
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree_Assign> enumerable, PowerData power)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {

                    case "name":
                        power.DisplayName = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master":
                        power.Master = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master2":
                        power.Master2 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master3":
                        power.Master3 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master4":
                        power.Master4 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master5":
                        power.Master5 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "master6":
                        power.Master6 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head":
                        power.Head = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head2":
                        power.Head2 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head3":
                        power.Head3 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head4":
                        power.Head4 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head5":
                        power.Head5 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "head6":
                        power.Head6 = InsertString(assign, power.FilledWithNull);
                        break;
                    case "member":
                        InsertStringList(assign, power.FilledWithNull, power.MemberSpot);
                        break;
                    case "friend":
                        InsertStringOnlyList(assign, power.FilledWithNull, power.Friend);
                        break;
                    case "flag":
                        power.FlagPath = InternLower(InsertString(assign, power.FilledWithNull));
                        break;
                    case "event":
                        power.IsEvent = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "bgm":
                        power.BGM = Intern(InsertString(assign, power.FilledWithNull));
                        break;
                    case "volume":
                        power.Volume = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "diplomacy":
                        power.DoDiplomacy = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "enable_select":
                        power.IsSelectable = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "enable_talent":
                        power.IsPlayableAsTalent = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "free_raise":
                        power.IsRaisable = InsertBool(assign, power.FilledWithNull);
                        break;
                    case "money":
                        power.Money = InsertInt(assign, power.FilledWithNull);
                        break;
                    case "home":
                        InsertStringOnlyList(assign, power.FilledWithNull, power.HomeSpot);
                        break;
                    case "fix":
                        power.FilledWithNull.Remove("fix");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "@":
                                power.FilledWithNull.Add("fix");
                                power.Fix = null;
                                break;
                            case "on":
                                power.Fix = PowerData.FixType.on;
                                break;
                            case "off":
                                power.Fix = PowerData.FixType.off;
                                break;
                            case "home":
                                power.Fix = PowerData.FixType.home;
                                break;
                            case "warlike":
                                power.Fix = PowerData.FixType.warlike;
                                break;
                            case "freeze":
                                power.Fix = PowerData.FixType.freeze;
                                break;
                            case "hold":
                                power.Fix = PowerData.FixType.hold;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "diplo":
                        InsertStringIntPair(assign, power.FilledWithNull, power.Diplo);
                        break;
                    case "league":
                        InsertStringIntPair(assign, power.FilledWithNull, power.League);
                        break;
                    case "enemy":
                        InsertStringIntPair(assign, power.FilledWithNull, power.EnemyPower);
                        break;
                    case "staff":
                        InsertStringOnlyList(assign, power.FilledWithNull, power.Staff);
                        break;
                    case "merce":
                        AddStringIntPair(assign, power.FilledWithNull, power.Merce);
                        break;
                    case "training_average":
                        power.TrainingAveragePercent = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "training_up":
                        power.TrainingUp = InsertInt(assign, power.FilledWithNull);
                        break;
                    case "base_merits":
                        power.BaseMerit = InsertInt(assign, power.FilledWithNull);
                        break;
                    case "merits":
                        InsertStringIntPair(assign, power.FilledWithNull, power.Merits);
                        break;
                    case "loyals":
                        InsertStringIntPair(assign, power.FilledWithNull, power.Loyals);
                        break;
                    case "base_loyal":
                        power.BaseLoyal = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "diff":
                        power.Difficulty = InsertString(assign, power.FilledWithNull);
                        break;
                    case "yabo":
                        power.Yabo = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "kosen":
                        power.Kosen = InsertByte(assign, power.FilledWithNull);
                        break;
                    case "text":
                        if (assign.Content.Count == 0)
                        {
                            power.Description = "";
                            power.FilledWithNull.Remove("text");
                            break;
                        }
                        power.Description = InsertString(assign, power.FilledWithNull);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, power.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree_Assign> enumerable, RaceData race)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        race.DisplayName = InsertString(assign, race.FilledWithNull);
                        break;
                    case "align":
                        race.Align = InsertByte(assign, race.FilledWithNull);
                        break;
                    case "brave":
                        race.Brave = InsertByte(assign, race.FilledWithNull);
                        break;
                    case "movetype":
                        race.MoveType = InsertString(assign, race.FilledWithNull);
                        break;
                    case "consti":
                        InsertStringBytePair(assign, race.FilledWithNull, race.Consti);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, race.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree_Assign> enumerable, SkillSetData skillset)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        skillset.DisplayName = InsertString(assign, skillset.FilledWithNull);
                        break;
                    case "member":
                        InsertStringOnlyList(assign, skillset.FilledWithNull, skillset.MemberSkill);
                        break;
                    case "back":
                        skillset.BackIconPath = InsertString(assign, skillset.FilledWithNull);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, skillset.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(this GenericUnitData genericunit)
        {
            if (!genericunit.VariantData.ContainsKey("")) return;
            var removeList = new List<string>();
            foreach (var keyVal in genericunit.VariantData[""])
            {
                switch (keyVal.Key)
                {
                    case "fkey":
                        ScriptLoader.BaseClassKeyDictionary.AddOrUpdate(genericunit.BaseClassKey = InsertString(keyVal.Value, genericunit.FilledWithNull), 0, (_1, _2) => _2);
                        break;
                    case "unique":
                        genericunit.IsUnique = InsertBool(keyVal.Value, genericunit.FilledWithNull);
                        break;
                    case "change":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            genericunit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        genericunit.FilledWithNull.Remove("change");
                        if (keyVal.Value.Content.Count != 2 || keyVal.Value.Content[1].Type != 2)
                            throw new ApplicationException();
                        genericunit.Change = new Tuple<string, int>(keyVal.Value.Content[0].ToLowerString(), (int)keyVal.Value.Content[1].Number);
                        break;
                }
            }
            foreach (var item in removeList)
                genericunit.VariantData[""].Remove(item);
        }
        internal static void Parse(this UnitData unit)
        {
            if (!unit.VariantData.ContainsKey("")) return;
            var removeList = new List<string>();
            foreach (var keyVal in unit.VariantData[""])
            {
                switch (keyVal.Key)
                {
                    case "class":
                        unit.Class = Intern(InsertString(keyVal.Value, unit.FilledWithNull)); removeList.Add(keyVal.Key);
                        break;
                    case "talent":
                        unit.IsTalent = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "bgm":
                        unit.BGM = Intern(InsertString(keyVal.Value, unit.FilledWithNull)); removeList.Add(keyVal.Key);
                        break;
                    case "volume":
                        unit.Volume = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture":
                        unit.Picture = Intern(InsertString(keyVal.Value, unit.FilledWithNull)); removeList.Add(keyVal.Key);
                        break;
                    case "picture_detail":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("picture_detail");
                            unit.PictureDetail = null;
                            break;
                        }
                        unit.FilledWithNull.Remove("picture_detail");
                        switch (keyVal.Value.Content[0].ToLowerString())
                        {
                            case "off":
                                unit.PictureDetail = 0;
                                break;
                            case "on":
                                unit.PictureDetail = 1;
                                break;
                            case "on1":
                                unit.PictureDetail = 2;
                                break;
                            case "on2":
                                unit.PictureDetail = 3;
                                break;
                            case "on3":
                                unit.PictureDetail = 4;
                                break;
                            default: throw new Exception();
                        }
                        break;
                    case "picture_menu":
                        unit.PictureMenu = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_floor":
                        unit.PictureFloor = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_shift":
                        unit.PictureShift = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_shift_up":
                        unit.PictureShiftUp = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_center":
                        unit.PictureCenter = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "picture_back":
                        unit.PictureBack = Intern(InsertString(keyVal.Value, unit.FilledWithNull)); removeList.Add(keyVal.Key);
                        break;
                    case "alive_per":
                        unit.AlivePercentage = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "medical":
                        unit.Medical = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "leader_skill":
                        removeList.Add(keyVal.Key);
                        unit.LeaderSkill.Clear();
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("leader_skill");
                            break;
                        }
                        unit.FilledWithNull.Remove("leader_skill");
                        int tmplevel = 0;
                        List<string> list = null;
                        for (int i = 0; i < keyVal.Value.Content.Count; i++)
                        {
                            if (keyVal.Value.Content[i].Type == 2) { tmplevel = (int)keyVal.Value.Content[i].Number; }
                            else if (unit.LeaderSkill.TryGetValue(tmplevel, out list)) { list.Add(keyVal.Value.Content[i].ToLowerString()); }
                            else { unit.LeaderSkill[tmplevel] = new List<string>() { keyVal.Value.Content[i].ToLowerString() }; }
                        }
                        break;
                    case "assist_skill":
                        removeList.Add(keyVal.Key);
                        unit.AssistSkill.Clear();
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("assist_skill");
                            break;
                        }
                        unit.FilledWithNull.Remove("assist_skill");
                        tmplevel = 0;
                        list = null;
                        for (int i = 0; i < keyVal.Value.Content.Count; i++)
                        {
                            if (keyVal.Value.Content[i].Type == 2) { tmplevel = (int)keyVal.Value.Content[i].Number; }
                            else if (unit.AssistSkill.TryGetValue(tmplevel, out list)) { list.Add(Intern(keyVal.Value.Content[i].ToLowerString())); }
                            else { unit.AssistSkill[tmplevel] = new List<string>() { Intern(keyVal.Value.Content[i].ToLowerString()) }; }
                        }
                        break;
                    case "yabo":
                        unit.Yabo = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "kosen":
                        unit.Kosen = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "align":
                        unit.Align = InsertByte(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "enemy":
                        InsertStringOnlyList(keyVal.Value, unit.FilledWithNull, unit.Enemy); removeList.Add(keyVal.Key);
                        break;
                    case "loyal":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.Loyal = null;
                            unit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        unit.FilledWithNull.Remove("loyal");
                        unit.Loyal = new Tuple<string, byte>(keyVal.Value.Content[0].ToLowerString(), (byte)(keyVal.Value.Content.Count == 2 ? keyVal.Value.Content[0].Number : 0));
                        break;
                    case "power_name":
                        unit.PowerDisplayName = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "flag":
                        unit.Flag = InternLower(InsertString(keyVal.Value, unit.FilledWithNull)); removeList.Add(keyVal.Key);
                        break;
                    case "staff":
                        InsertStringOnlyList(keyVal.Value, unit.FilledWithNull, unit.Staff); removeList.Add(keyVal.Key);
                        break;
                    case "diplomacy":
                        unit.Diplomacy = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "castle_guard":
                        removeList.Add(keyVal.Key);
                        unit.CastleGuard.Clear();
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("castle_guard");
                            break;
                        }
                        unit.FilledWithNull.Remove("castle_guard");
                        for (int i = 0; (i << 1) < keyVal.Value.Content.Count; i++)
                            unit.CastleGuard[keyVal.Value.Content[i << 1].ToLowerString()] = (int)keyVal.Value.Content[(i << 1) + 1].Number;
                        break;
                    case "actor":
                        unit.IsActor = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "enable_select":
                        unit.IsEnableSelect = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "enable":
                        unit.EnableTurn = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "enable_max":
                        unit.EnableTurnMax = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "fix":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.Fix = null;
                            unit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        unit.FilledWithNull.Remove("fix");
                        switch (keyVal.Value.Content[0].ToLowerString())
                        {
                            case "on":
                                unit.Fix = 1;
                                break;
                            case "off":
                                unit.Fix = 0;
                                break;
                            case "home":
                                unit.Fix = 2;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "home":
                        InsertStringOnlyList(keyVal.Value, unit.FilledWithNull, unit.Home); removeList.Add(keyVal.Key);
                        break;
                    case "no_escape":
                        unit.IsNoEscape = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "noremove_unit":
                        unit.IsNoRemoveUnit = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "noemploy_unit":
                        unit.IsNoEmployUnit = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "noitem_unit":
                        unit.IsNoItemUnit = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "arbeit":
                        removeList.Add(keyVal.Key);
                        unit.FilledWithNull.Remove("arbeit");
                        switch (keyVal.Value.Content.Count)
                        {
                            case 1:
                                if (byte.TryParse(keyVal.Value.Content[0].Content, out var arbeitPercentage))
                                {
                                    unit.ArbeitPercentage = arbeitPercentage;
                                    unit.ArbeitType = 0;
                                }
                                else if (keyVal.Value.Content[0].Symbol1 == '@')
                                {
                                    unit.ArbeitPercentage = null;
                                    unit.ArbeitType = null;
                                }
                                else
                                {
                                    unit.ArbeitPercentage = 100;
                                    switch (keyVal.Value.Content[0].ToLowerString())
                                    {
                                        case "on":
                                            unit.ArbeitType = 0;
                                            break;
                                        case "power":
                                            unit.ArbeitType = 1;
                                            break;
                                        case "fix":
                                            unit.ArbeitType = 2;
                                            break;
                                        default: throw new ApplicationException();
                                    }
                                }
                                break;
                            case 2:
                                switch (keyVal.Value.Content[0].ToLowerString())
                                {
                                    case "on":
                                        unit.ArbeitType = 0;
                                        break;
                                    case "power":
                                        unit.ArbeitType = 1;
                                        break;
                                    case "fix":
                                        unit.ArbeitType = 2;
                                        break;
                                    default: throw new ApplicationException();
                                }
                                unit.ArbeitPercentage = (byte)(keyVal.Value.Content[1].Number);
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "arbeit_capacity":
                        unit.ArbeitCapacity = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "help":
                        unit.Help = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "join":
                        unit.Join = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "dead":
                        unit.Dead = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "retreat":
                        unit.Retreat = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "break":
                        unit.Break = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "voice_type":
                        unit.VoiceType = InsertString(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "red":
                        removeList.Add(keyVal.Key);
                        if (keyVal.Value.Content.Count == 1 && keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        switch (keyVal.Value.Content[0].ToLowerString())
                        {
                            case "off":
                                unit.Team = 0;
                                break;
                            case "on":
                                unit.Team = 1;
                                break;
                            default:
                                if (keyVal.Value.Content[0].Type != 2)
                                    throw new ApplicationException();
                                unit.Team = (byte)keyVal.Value.Content[0].Number;
                                break;
                        }
                        break;
                    case "keep_form":
                        unit.KeepForm = InsertBool(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "breast_width":
                        unit.BreastWidth = InsertInt(keyVal.Value, unit.FilledWithNull); removeList.Add(keyVal.Key);
                        break;
                    case "active":
                        if (keyVal.Value.Content[0].Symbol1 == '@')
                        {
                            unit.ActiveType = null;
                            unit.FilledWithNull.Add(keyVal.Key);
                            break;
                        }
                        unit.FilledWithNull.Remove("active");
                        switch (keyVal.Value.Content[0].ToLowerString())
                        {
                            case "never":
                                unit.ActiveType = 0;
                                break;
                            case "rect":
                                unit.ActiveType = 2;
                                break;
                            case "range":
                                unit.ActiveType = 4;
                                break;
                            case "time":
                                unit.ActiveType = 6;
                                break;
                            case "troop":
                                unit.ActiveType = 8;
                                break;
                            case "never2":
                                unit.ActiveType = 1;
                                break;
                            case "rect2":
                                unit.ActiveType = 3;
                                break;
                            case "range2":
                                unit.ActiveType = 5;
                                break;
                            case "time2":
                                unit.ActiveType = 7;
                                break;
                            case "troop2":
                                unit.ActiveType = 9;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "activenum":
                        unit.FilledWithNull.Remove("activenum");
                        unit.ActiveRange = null;
                        unit.ActiveRect = null;
                        unit.ActiveTime = null;
                        unit.ActiveTroop = null;
                        switch (keyVal.Value.Content.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                int parse = (int)keyVal.Value.Content[0].Number;
                                if (keyVal.Value.Content[0].Symbol1 == '@')
                                {
                                    unit.FilledWithNull.Add(keyVal.Key);
                                    break;
                                }
                                else if (keyVal.Value.Content[0].Type == 2)
                                    if (!unit.ActiveType.HasValue)
                                    {
                                        unit.ActiveTime = parse;
                                        unit.ActiveRange = parse;
                                    }
                                    else if (unit.ActiveType <= 5)
                                        unit.ActiveRange = parse;
                                    else unit.ActiveTime = parse;
                                else
                                    unit.ActiveTroop = keyVal.Value.Content[0].ToLowerString();
                                break;
                            case 4:
                                unit.ActiveRect = new Tuple<int, int, int, int>((int)keyVal.Value.Content[0].Number, (int)keyVal.Value.Content[1].Number, (int)keyVal.Value.Content[2].Number, (int)keyVal.Value.Content[3].Number);
                                break;
                            default: throw new ApplicationException();
                        }
                        break;

                }
            }
            foreach (var item in removeList)
                unit.VariantData[""].Remove(item);
        }
        static void CommonParse<T>(IEnumerable<LexicalTree_Assign> enumerable, T unit) where T : CommonUnitData
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        unit.DisplayName = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "sex":
                        unit.FilledWithNull.Remove("sex");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "male":
                                unit.Sex = 1;
                                break;
                            case "female":
                                unit.Sex = 2;
                                break;
                            case "@":
                                unit.Sex = null;
                                unit.FilledWithNull.Add(assign.Name);
                                break;
                            default:
                                unit.Sex = 0;
                                break;
                        }
                        break;
                    case "race":
                        unit.Race = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "radius":
                        unit.Radius = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "radius_press":
                        unit.RadiusPress = InsertByte(assign, unit.FilledWithNull);
                        break;
                    case "w":
                        unit.Width = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "h":
                        unit.Height = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "a":
                        unit.Alpha = InsertByte(assign, unit.FilledWithNull);
                        break;
                    case "image":
                        unit.Image = Intern(InsertString(assign, unit.FilledWithNull));
                        break;
                    case "image2":
                        unit.Image2 = Intern(InsertString(assign, unit.FilledWithNull));
                        break;
                    case "tkool":
                        unit.IsTkool = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "face":
                        unit.Face = Intern(InsertString(assign, unit.FilledWithNull));
                        break;
                    case "price":
                        unit.Price = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "cost":
                        unit.Cost = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hasexp":
                        unit.HasEXP = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "friend":
                        unit.IsFriendAllRace = null;
                        unit.IsFriendAllClass = null;
                        InsertStringOnlyList(assign, unit.FilledWithNull, unit.Friends);
                        if (unit.Friends.Contains("allrace"))
                            unit.IsFriendAllRace = true;
                        if (unit.Friends.Contains("allclass"))
                            unit.IsFriendAllClass = true;
                        break;
                    case "friend_ex":
                        unit.FriendEx1.Clear();
                        unit.FriendEx2.Clear();
                        unit.FriendExCount = null;
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("friend_ex");
                        for (int splitIndex = 0; splitIndex < assign.Content.Count; ++splitIndex)
                        {
                            if (unit.FriendExCount == null)
                                if (assign.Content[splitIndex].Type == 2)
                                {
                                    unit.FriendExCount = (int)assign.Content[splitIndex].Number;
                                }
                                else { unit.FriendEx1.Add(assign.Content[splitIndex].ToLowerString()); }
                            else { unit.FriendEx2.Add(assign.Content[splitIndex].ToLowerString()); }
                        }
                        break;
                    case "merce":
                        InsertStringList(assign, unit.FilledWithNull, unit.Merce);
                        break;
                    case "same_friend":
                        unit.IsSameFriend = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "same_call":
                        unit.IsSameCall = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "samecall_baseup":
                        unit.IsSameCallBaseUp = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "same_sex":
                        unit.IsSameSex = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "member":
                        AddStringIntPair(assign, unit.FilledWithNull, unit.Member);
                        break;
                    case "level":
                        unit.Level = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hp":
                        unit.Hp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mp":
                        unit.Mp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "attack":
                        unit.Attack = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "defense":
                        unit.Defense = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magic":
                        unit.Magic = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magdef":
                        unit.Magdef = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dext":
                        unit.Dext = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "speed":
                        unit.Speed = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "move":
                        unit.Move = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hprec":
                        unit.HpRec = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mprec":
                        unit.MpRec = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "summon_max":
                        unit.summon_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "summon_level":
                        unit.summon_level = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "heal_max":
                        unit.heal_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "attack_max":
                        unit.attack_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "defense_max":
                        unit.defense_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magic_max":
                        unit.magic_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magdef_max":
                        unit.magdef_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dext_max":
                        unit.dext_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "speed_max":
                        unit.speed_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "move_max":
                        unit.move_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hprec_max":
                        unit.hprec_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mprec_max":
                        unit.mprec_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "skill":
                        InsertStringOnlyList(assign, unit.FilledWithNull, unit.Skill);
                        break;
                    case "skill2":
                        unit.Skill2.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove(assign.Name);
                        int _tmplevel = 0;
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            if (assign.Content[i].Type == 2)
                                unit.Skill2[(_tmplevel = (int)assign.Content[i].Number)] = new List<string>();
                            else if (!unit.Skill2.ContainsKey(_tmplevel))
                                unit.Skill2[_tmplevel] = new List<string>() { assign.Content[i].ToLowerString() };
                            else
                                unit.Skill2[_tmplevel].Add(assign.Content[i].ToLowerString());
                        }
                        break;
                    case "delskill":
                        InsertStringList(assign, unit.FilledWithNull, unit.DeleteSkill);
                        break;
                    case "learn":
                        unit.Learn.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("learn");
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            var tmplevel = (int)assign.Content[i].Number;
                            if (assign.Content[i].Type == 2)
                                unit.Learn[(int)assign.Content[i].Number] = new List<string>();
                            else if (!unit.Learn.ContainsKey(tmplevel))
                                unit.Learn[tmplevel] = new List<string>() { assign.Content[i].ToLowerString() };
                            else
                                unit.Learn[tmplevel].Add(assign.Content[i].ToLowerString());
                        }
                        break;
                    case "delskill2":
                        InsertStringList(assign, unit.FilledWithNull, unit.DeleteSkill2);
                        break;
                    case "consti":
                        InsertStringIntPair(assign, unit.FilledWithNull, unit.Consti);
                        foreach (var (key, value) in unit.Consti)
                            if (value < 0 || value > 10)
                                throw new IndexOutOfRangeException("Consti Error\n" + unit.DebugInfo);
                        break;
                    case "movetype":
                        unit.MoveType = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "line":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.DefenseLine = null;
                            unit.FilledWithNull.Add("line");
                            break;
                        }
                        unit.FilledWithNull.Remove("line");
                        byte line = (byte)assign.Content[0].Number;
                        if (assign.Content[0].Type == 1)
                            unit.DefenseLine = line;
                        else if (assign.Content[0].ToLowerString() == "front")
                            unit.DefenseLine = 10;
                        else unit.DefenseLine = 0;
                        break;
                    case "satellite":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.IsSatellite = null;
                            unit.Satellite = null;
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("satellite");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                unit.IsSatellite = true;
                                break;
                            case "off":
                                unit.IsSatellite = false;
                                break;
                            default:
                                if (assign.Content[0].Type == 2)
                                {
                                    unit.IsSatellite = true;
                                    unit.Satellite = (int)assign.Content[0].Number;
                                }
                                break;
                        }
                        break;
                    case "beast_unit":
                        unit.IsBeast = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "no_knock":
                        if (assign.Content[0].ToLowerString() == "on")
                            unit.NoKnock = 1;
                        else unit.NoKnock = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "no_cover":
                        unit.IsNoCover = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "view_unit":
                        unit.IsViewUnit = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "element_lost":
                        unit.IsElementLost = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "attack_range":
                        unit.AttackRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "escape_range":
                        unit.EscapeRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hand_range":
                        unit.HandRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "wake_range":
                        unit.WakeRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "view_range":
                        unit.ViewRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "cavalary_range":
                        unit.CavalryRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "escape_run":
                        unit.EscapeRange = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "multi":
                        unit.Multi.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.FilledWithNull.Add("multi");
                            break;
                        }
                        unit.FilledWithNull.Remove("multi");
                        int multiLevel = 0;
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            if (assign.Content[i].Type == 2)
                                multiLevel = (int)assign.Content[i].Number;
                            else unit.Multi[assign.Content[i].ToLowerString()] = multiLevel;
                        }
                        break;
                    case "exp":
                        unit.exp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "exp_mul":
                        unit.exp_mul = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "level_max":
                        unit.level_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "exp_max":
                        unit.exp_max = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hpup":
                        unit.hpUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mpup":
                        unit.mpUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "attackup":
                        unit.attackUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "defenseup":
                        unit.defenseUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magicup":
                        unit.magicUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magdefup":
                        unit.magdefUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dextup":
                        unit.dextUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "speedup":
                        unit.speedUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "moveup":
                        unit.moveUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hprecup":
                        unit.hprecUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mprecup":
                        unit.mprecUp = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hpmax":
                        unit.hpMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mpmax":
                        unit.mpMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "attackmax":
                        unit.attackMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "defensemax":
                        unit.defenseMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magicmax":
                        unit.magicMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "magdefmax":
                        unit.magdefMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dextmax":
                        unit.dextMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "speedmax":
                        unit.speedMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "movemax":
                        unit.moveMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "hprecmax":
                        unit.hprecMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "mprecmax":
                        unit.mprecMax = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "brave":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.IsBrave = null;
                            unit.Brave = null;
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("brave");
                        if (assign.Content.Count == 2)
                        {
                            if (assign.Content[0].ToLowerString() != "on") throw new ApplicationException();
                            unit.IsBrave = true;
                            unit.Brave = (byte)assign.Content[1].Number;
                            break;
                        }
                        unit.Brave = (byte)assign.Content[0].Number;
                        break;
                    case "handle":
                        unit.IsHandle = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "sortkey":
                        unit.SortKey = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "item":
                        InsertStringList(assign, unit.FilledWithNull, unit.Item);
                        break;
                    case "no_training":
                        unit.IsNoTraining = InsertBool(assign, unit.FilledWithNull);
                        break;
                    case "scream":
                        unit.Scream = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "lost_corpse":
                        unit.LostCorpse = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "dead_event":
                        unit.DeadEvent = InsertString(assign, unit.FilledWithNull);
                        break;
                    case "free_move":
                        unit.FreeMove = InsertInt(assign, unit.FilledWithNull);
                        break;
                    case "add_vassal":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.AddVassal = null;
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("add_vassal");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "off":
                                unit.AddVassal = 0;
                                break;
                            case "on":
                                unit.AddVassal = 1;
                                break;
                            case "roam":
                                unit.AddVassal = 2;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    case "politics":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            unit.Politics = null;
                            unit.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        unit.FilledWithNull.Remove("politics");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                unit.Politics = 1;
                                break;
                            case "fix":
                                unit.Politics = 2;
                                break;
                            case "erase":
                                unit.Politics = 3;
                                break;
                            case "unique":
                                unit.Politics = 4;
                                break;
                            default: throw new ApplicationException();
                        }
                        break;
                    default:
                        ScenarioVariantRoutine(assign, unit.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree> enumerable, EventData eventData)
        {
            foreach (var item in enumerable)
            {
                if (item.Type != LexicalTree.TreeType.Assign)
                {
                    eventData.Script.Add(item);
                    continue;
                }
                var assign = item as LexicalTree_Assign;
                switch (assign.Name)
                {
                    case "bg":
                    case "bcg":
                        assign.Name = "bg";
                        eventData.BackGround = InsertString(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "bgm":
                        eventData.BGM = InsertString(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "w":
                        eventData.Width = InsertInt(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "h":
                        eventData.Height = InsertInt(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "map":
                        eventData.Map = InsertString(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "handle":
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            eventData.FilledWithNull.Add("handle");
                            eventData.Handle = null;
                            break;
                        }
                        eventData.FilledWithNull.Remove("handle");
                        if (assign.Content[0].Type == 2)
                            eventData.Handle = (byte)assign.Content[0].Number;
                        else if (assign.Content[0].ToLowerString() == "red")
                            eventData.Handle = 1;
                        else eventData.Handle = 0;
                        break;
                    case "disperse":
                        eventData.Disperse = InsertBool(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "castle_battle":
                        eventData.CastleBattle = InsertBool(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "castle":
                        eventData.Castle = InsertInt(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "title":
                        eventData.Title = InsertString(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "limit":
                        eventData.Limit = InsertInt(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "volume":
                        eventData.Volume = InsertByte(assign as LexicalTree_Assign, eventData.FilledWithNull);
                        break;
                    case "blind":
                        eventData.Blind = null;
                        eventData.IsBlind = null;
                        if (assign.Content[0].Symbol1 == '@')
                        {
                            eventData.FilledWithNull.Add("blind");
                        }
                        eventData.FilledWithNull.Remove("blind");
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                eventData.IsBlind = true;
                                break;
                            case "off":
                                eventData.IsBlind = false;
                                break;
                            default:
                                eventData.IsBlind = true;
                                eventData.Blind = (byte)assign.Content[0].Number;
                                break;
                        }
                        break;
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree_Assign> enumerable, SpotData spot)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        spot.DisplayName = InsertString(assign, spot.FilledWithNull);
                        break;
                    case "image":
                        spot.Image = Intern(InsertString(assign, spot.FilledWithNull));
                        if (spot.Image != null)
                            spot.Image = String.Intern(spot.Image);
                        break;
                    case "x":
                        spot.X = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "y":
                        spot.Y = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "w":
                        spot.Width = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "h":
                        spot.Height = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "big":
                        spot.Width = spot.Height = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "map":
                        spot.Map = Intern(InsertString(assign, spot.FilledWithNull));
                        if (spot.Map != null)
                            spot.Map = String.Intern(spot.Map);
                        break;
                    case "castle_battle":
                        spot.IsCastleBattle = InsertBool(assign, spot.FilledWithNull);
                        break;
                    case "yorozu":
                        InsertStringList(assign, spot.FilledWithNull, spot.Yorozu);
                        break;
                    case "limit":
                        spot.Limit = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "bgm":
                        spot.BGM = Intern(InsertString(assign, spot.FilledWithNull));
                        if (spot.BGM != null)
                            spot.BGM = String.Intern(spot.BGM);
                        break;
                    case "volume":
                        spot.Volume = InsertByte(assign, spot.FilledWithNull);
                        break;
                    case "gain":
                        spot.Gain = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "castle":
                        spot.Castle = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "capacity":
                        spot.Capacity = InsertByte(assign, spot.FilledWithNull);
                        break;
                    case "monster":
                        AddStringIntPair(assign, spot.FilledWithNull, spot.Monsters);
                        break;
                    case "merce":
                        InsertStringList(assign, spot.FilledWithNull, spot.Merce);
                        break;
                    case "member":
                        AddStringIntPair(assign, spot.FilledWithNull, spot.Members);
                        break;
                    case "dungeon":
                        spot.Dungeon = InsertString(assign, spot.FilledWithNull);
                        break;
                    case "no_home":
                        spot.IsNotHome = InsertBool(assign, spot.FilledWithNull);
                        break;
                    case "no_raise":
                        spot.IsNotRaisableSpot = InsertBool(assign, spot.FilledWithNull);
                        break;
                    case "castle_lot":
                        spot.CastleLot = InsertInt(assign, spot.FilledWithNull);
                        break;
                    case "politics":
                        spot.Politics = InsertBool(assign, spot.FilledWithNull);
                        break;
                    default:
                        ScenarioVariantRoutine(assign, spot.VariantData);
                        break;
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree> enumerable, ScenarioData scenario)
        {
            int? locate_x = null;
            int? locate_y = null;
            foreach (var item in enumerable)
            {
                var assign = item as LexicalTree_Assign;
                if (assign == null)
                {
                    scenario.Script.Add(item);
                    continue;
                }
                switch (assign.Name)
                {
                    case "name":
                        scenario.DisplayName = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "map":
                        scenario.WorldMapPath = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "locate_x":
                        locate_x = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "locate_y":
                        locate_y = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "text":
                        scenario.DescriptionText = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "begin_text":
                        scenario.BeginText = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "power":
                        InsertStringOnlyList(assign, scenario.FilledWithNull, scenario.Power);
                        break;
                    case "spot":
                        InsertStringOnlyList(assign, scenario.FilledWithNull, scenario.Spot);
                        break;
                    case "roam":
                        InsertStringOnlyList(assign, scenario.FilledWithNull, scenario.Roamer);
                        break;
                    case "world":
                        scenario.WorldEvent = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "fight":
                        scenario.FightEvent = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "politics":
                        scenario.PoliticsEvent = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "war_capcity":
                        scenario.WarCapacity = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "spot_capacity":
                        scenario.SpotCapacity = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "gain_per":
                        scenario.GainPercent = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "support_range":
                        scenario.SupportRange = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "my_range":
                        scenario.MyRange = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "myhelp_range":
                        scenario.MyHelpRange = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "base_level":
                        scenario.BaseLevelUp = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "monster_level":
                        scenario.MonsterLevel = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "training_up":
                        scenario.TrainingUp = InsertInt(assign, scenario.FilledWithNull);
                        break;
                    case "actor_per":
                        scenario.ActorPercent = InsertByte(assign, scenario.FilledWithNull);
                        break;
                    case "sortkey":
                        if (assign.Content[0].Symbol1 == '@') scenario.SortKey = 0;
                        else scenario.SortKey = (int)assign.Content[0].Number;
                        break;
                    case "default_ending":
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                scenario.IsDefaultEnding = true;
                                break;
                            case "@":
                                scenario.FilledWithNull.Add("default_ending");
                                scenario.IsDefaultEnding = null;
                                break;
                            case "off":
                                scenario.IsDefaultEnding = false;
                                break;
                        }
                        break;
                    case "power_order":
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "normal":
                                scenario.Order = ContextData.Order.Normal;
                                break;
                            case "dash":
                                scenario.Order = ContextData.Order.Dash;
                                break;
                            case "test":
                            case "dashtest":
                                scenario.Order = ContextData.Order.DashTest;
                                break;
                            default:
                                throw new ApplicationException();
                        }
                        break;
                    case "enable":
                        scenario.IsEnable = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "enable_talent":
                        scenario.IsPlayableAsTalent = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "party":
                        scenario.IsAbleToMerce = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "no_autosave":
                        scenario.IsNoAutoSave = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "offset":
                        foreach (var remove in assign.Content)
                            for (int i = 0; i < scenario.MenuButtonPower.Length; ++i)
                            {
                                if (scenario.MenuButtonPower[i] == remove.Content)
                                    scenario.MenuButtonPower[i] = "";
                                if (scenario.MenuButtonTalent[i] == remove.Content)
                                    scenario.MenuButtonTalent[i] = "";
                            }
                        break;
                    case "zone":
                        scenario.ZoneName = InsertString(assign, scenario.FilledWithNull);
                        break;
                    case "nozone":
                        scenario.IsNoZone = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "poli":
                        scenario.PoliticsData.Clear();
                        if (assign.Content[0].Symbol1 == '@' || assign.Content[0].ToLowerString() == "none")
                        {
                            scenario.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        scenario.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i * 3 < assign.Content.Count; i++)
                        {
                            var con1 = assign.Content[i * 3].ToString();
                            var con2 = assign.Content[i * 3 + 1].ToString();
                            var con3 = assign.Content[i * 3 + 2].ToString();
                            scenario.PoliticsData[con1] = new ValueTuple<string, string>(con2 == "@" ? "" : con2, con3 == "@" ? "" : con3);
                        }
                        break;
                    case "camp":
                        scenario.CampingData.Clear();
                        if (assign.Content[0].Symbol1 == '@' || assign.Content[0].ToLowerString() == "none")
                        {
                            scenario.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        scenario.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i * 3 < assign.Content.Count; i++)
                        {
                            var con1 = assign.Content[i * 3].Content;
                            var con2 = assign.Content[i * 3 + 1].Content;
                            var con3 = assign.Content[i * 3 + 2].Content;
                            scenario.CampingData[con1] = new ValueTuple<string, string>(con2 == "@" ? "" : con2, con3 == "@" ? "" : con3);
                        }
                        break;
                    case "item":
                        for (int i = 0; i < scenario.ItemWindowTab.Length; i++)
                        {
                            scenario.ItemWindowTab[i] = (null, 0);
                        }
                        if (assign.Content[0].Symbol1 == '@' || assign.Content[0].ToLowerString() == "none")
                        {
                            scenario.FilledWithNull.Add(assign.Name);
                            break;
                        }
                        scenario.FilledWithNull.Remove(assign.Name);
                        for (int i = 0; i < assign.Content.Count; i++)
                            scenario.ItemWindowTab[i] = (assign.Content[i].ToLowerString(), 1);
                        break;
                    case "item0":
                    case "item1":
                    case "item2":
                    case "item3":
                    case "item4":
                    case "item5":
                    case "item6":
                        var itemIndex = int.Parse(assign.Name.Last().ToString());
                        var itemTabName = scenario.ItemWindowTab[itemIndex].Item1;
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "on":
                                scenario.ItemWindowTab[itemIndex] = (itemTabName, int.MaxValue);
                                break;
                            case "@":
                            case "off":
                                scenario.ItemWindowTab[itemIndex] = (itemTabName, 0);
                                break;
                            default:
                                scenario.ItemWindowTab[itemIndex] = (itemTabName, (int)assign.Content[0].Number);
                                break;
                        }
                        break;
                    case "item_sale":
                        InsertStringOnlyList(assign, scenario.FilledWithNull, scenario.ItemSale);
                        break;
                    case "item_limit":
                        scenario.IsItemLimit = InsertBool(assign, scenario.FilledWithNull);
                        break;
                    case "item_hold":
                        InsertStringOnlyList(assign, scenario.FilledWithNull, scenario.PlayerInitialItem);
                        break;
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree_Assign> enumerable, VoiceData voice)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "voice_type":
                        InsertStringOnlyList(assign, voice.FilledWithNull, voice.VoiceType);
                        break;
                    case "delskill":
                        InsertStringOnlyList(assign, voice.FilledWithNull, voice.DeleteVoiceType);
                        break;
                    case "power":
                        InsertStringOnlyList(assign, voice.FilledWithNull, voice.PowerVoice);
                        if (voice.PowerVoice.Count != 0)
                        {
                            voice.NoPower = false;
                        }
                        break;
                    case "spot":
                        InsertStringOnlyList(assign, voice.FilledWithNull, voice.SpotVoice);
                        break;
                }
            }
        }
        internal static void Parse(this IEnumerable<LexicalTree_Assign> enumerable, DungeonData answer)
        {
            foreach (var assign in enumerable)
            {
                switch (assign.Name)
                {
                    case "name":
                        answer.DisplayName = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "prefix":
                        answer.Prefix = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "suffix":
                        answer.Suffix = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "bgm":
                        answer.BGM = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "map":
                        answer.MapFileName = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "floor":
                        answer.Floor = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "wall":
                        answer.Wall = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "start":
                        answer.Start = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "goal":
                        answer.Goal = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "box":
                        answer.Box = InsertString(assign, answer.FilledWithNull);
                        break;
                    case "open":
                        answer.IsOpened = InsertBool(assign, answer.FilledWithNull);
                        break;
                    case "item_text":
                        answer.ItemText = InsertBool(assign, answer.FilledWithNull);
                        break;
                    case "volume":
                        answer.Volume = InsertByte(assign, answer.FilledWithNull);
                        break;
                    case "blind":
                        answer.Blind = InsertByte(assign, answer.FilledWithNull);
                        break;
                    case "max":
                        answer.Max = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "move_speed":
                        answer.MoveSpeedRatio = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "lv_adjust":
                        answer.LevelAdjust = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "limit":
                        answer.Limit = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "base_level":
                        answer.BaseLevel = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "monster_num":
                        answer.MonsterNumber = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "item_num":
                        answer.ItemNumber = InsertInt(assign, answer.FilledWithNull);
                        break;
                    case "color":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.ColorDirection = null;
                            answer.ForeColorA = null;
                            answer.ForeColorB = null;
                            answer.ForeColorG = null;
                            answer.ForeColorR = null;
                            answer.BackColorA = null;
                            answer.BackColorB = null;
                            answer.BackColorG = null;
                            answer.BackColorR = null;
                            answer.Dense = null;
                            answer.FilledWithNull.Add("color");
                            break;
                        }
                        answer.FilledWithNull.Remove("color");
                        answer.ColorDirection = (byte)assign.Content[0].Number;
                        answer.ForeColorB = (byte)assign.Content[3].Number;
                        answer.ForeColorG = (byte)assign.Content[2].Number;
                        answer.ForeColorR = (byte)assign.Content[1].Number;
                        answer.BackColorB = (byte)assign.Content[6].Number;
                        answer.BackColorG = (byte)assign.Content[5].Number;
                        answer.BackColorR = (byte)assign.Content[4].Number;
                        answer.ForeColorA = (byte)assign.Content[7].Number;
                        answer.BackColorA = (byte)assign.Content[8].Number;
                        answer.Dense = (byte)assign.Content[9].Number;
                        break;
                    case "ray":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.Ray = null;
                            answer.Way1 = null;
                            answer.Way2 = null;
                            answer.Way3 = null;
                            answer.FilledWithNull.Add("ray");
                            break;
                        }
                        answer.FilledWithNull.Remove("ray");
                        answer.Ray = (byte)assign.Content[0].Number;
                        answer.Way1 = (byte)assign.Content[1].Number;
                        answer.Way2 = (byte)assign.Content[2].Number;
                        answer.Way3 = (byte)assign.Content[3].Number;
                        break;
                    case "home":
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.Width = null;
                            answer.Height = null;
                            answer.HallwayWidth = null;
                            answer.Seed = null;
                            answer.FilledWithNull.Add("home");
                            break;
                        }
                        answer.FilledWithNull.Remove("home");
                        if (assign.Content.Count < 3 || assign.Content.Count > 4) throw new ApplicationException();
                        answer.Width = (int)assign.Content[0].Number;
                        answer.Height = (int)assign.Content[1].Number;
                        answer.HallwayWidth = (int)assign.Content[2].Number;
                        if (assign.Content.Count == 4)
                            answer.Seed = (long)assign.Content[3].Number;
                        break;
                    case "monster":
                        answer.Monsters.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.FilledWithNull.Add("monster");
                            break;
                        }
                        answer.FilledWithNull.Remove("monster");
                        for (int i = 0; i < assign.Content.Count; i++)
                        {
                            if (i + 1 < assign.Content.Count && int.TryParse(assign.Content[i + 1].Content, out var count))
                                answer.Monsters[assign.Content[i].ToLowerString()] = count;
                            else if (answer.Monsters.TryGetValue(assign.Content[i].ToLowerString(), out count))
                                answer.Monsters[assign.Content[i].ToLowerString()] = count + 1;
                            else answer.Monsters[assign.Content[i].ToLowerString()] = 1;
                        }
                        break;
                    case "item":
                        answer.Item.Clear();
                        if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
                        {
                            answer.FilledWithNull.Add("item");
                            break;
                        }
                        answer.FilledWithNull.Remove("item");
                        answer.Item.AddRange(assign.Content.Select(_ => _.ToLowerString()));
                        break;
                    default:
                        var split = assign.Name.Split(new char[1] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        if (split.Length == 1)
                            answer.VariantData[int.Parse(split[1])][split[0]] = assign.Content;
                        else
                            answer.VariantData[0][split[0]] = assign.Content;
                        break;
                }
            }
        }
        static void ContextParse(IEnumerable<LexicalTree_Assign> enumerable)
        {
            foreach (var assign in enumerable)
            {
                byte firstByte = (byte)assign.Content[0].Number;
                int firstInt = (int)assign.Content[0].Number;
                string firstString = assign.Content[0].ToString();
                string firstIdentifier = assign.Content[0].ToLowerString();
                if (assign.Content[0].Symbol1 == '@') continue;
                switch (assign.Name)
                {
                    case "title_name":
                        Context.TitleName = firstString;
                        break;
                    case "title_menu_right":
                        Context.TitleMenuRight = firstInt;
                        break;
                    case "title_menu_top":
                        Context.TitleMenuTop = firstInt;
                        break;
                    case "title_menu_space":
                        Context.TitleMenuSpace = firstInt;
                        break;
                    case "event_bg_size":
                        Context.event_bg_size = firstInt;
                        break;
                    case "member":
                        Context.Member = firstByte;
                        break;
                    case "movement_num":
                        if (assign.Content[0].Symbol1 == '@') continue;
                        Context.MovementNumber = firstByte;
                        break;
                    case "employ_range":
                        Context.EmployRange = firstByte;
                        break;
                    case "battle_fast":
                        Context.BattleFast = firstString == "on";
                        break;
                    case "support_range":
                        Context.SupportRange = firstByte;
                        break;
                    case "my_range":
                        Context.MyRange = firstByte;
                        break;
                    case "myhelp_range":
                        Context.MyHelpRange = firstByte;
                        break;
                    case "mode_easy":
                        foreach (var token in assign.Content)
                        {
                            switch (token.ToLowerString())
                            {
                                case "samecall":
                                    Context.SameCall_easy = true;
                                    break;
                                case "isblind":
                                    Context.IsBlind_easy = true;
                                    break;
                                case "dead":
                                    Context.Dead_easy = true;
                                    break;
                                case "dead2":
                                    Context.Dead2_easy = true;
                                    break;
                                case "escape":
                                    Context.Escape_easy = true;
                                    break;
                                case "auto":
                                    Context.Auto_easy = true;
                                    break;
                                case "sdown":
                                    Context.SDown_easy = true;
                                    break;
                                case "steal":
                                    Context.Steal_easy = true;
                                    break;
                                case "hostil":
                                    Context.Hostil_easy = true;
                                    break;
                                case "cover_fire":
                                    Context.CoverFire_easy = true;
                                    break;
                                case "force_fire":
                                    Context.ForceFire_easy = true;
                                    break;
                                case "nozone":
                                    Context.NoZone_easy = true;
                                    break;
                            }
                        }
                        break;
                    case "mode_normal":
                        foreach (var token in assign.Content)
                        {
                            switch (token.ToLowerString())
                            {
                                case "samecall":
                                    Context.SameCall_normal = true;
                                    break;
                                case "isblind":
                                    Context.IsBlind_normal = true;
                                    break;
                                case "dead":
                                    Context.Dead_normal = true;
                                    break;
                                case "dead2":
                                    Context.Dead2_normal = true;
                                    break;
                                case "escape":
                                    Context.Escape_normal = true;
                                    break;
                                case "auto":
                                    Context.Auto_normal = true;
                                    break;
                                case "sdown":
                                    Context.SDown_normal = true;
                                    break;
                                case "steal":
                                    Context.Steal_normal = true;
                                    break;
                                case "hostil":
                                    Context.Hostil_normal = true;
                                    break;
                                case "cover_fire":
                                    Context.CoverFire_normal = true;
                                    break;
                                case "force_fire":
                                    Context.ForceFire_normal = true;
                                    break;
                                case "nozone":
                                    Context.NoZone_normal = true;
                                    break;
                            }
                        }
                        break;
                    case "mode_hard":
                        foreach (var token in assign.Content)
                        {
                            switch (token.ToLowerString())
                            {
                                case "samecall":
                                    Context.SameCall_hard = true;
                                    break;
                                case "isblind":
                                    Context.IsBlind_hard = true;
                                    break;
                                case "dead":
                                    Context.Dead_hard = true;
                                    break;
                                case "dead2":
                                    Context.Dead2_hard = true;
                                    break;
                                case "escape":
                                    Context.Escape_hard = true;
                                    break;
                                case "auto":
                                    Context.Auto_hard = true;
                                    break;
                                case "sdown":
                                    Context.SDown_hard = true;
                                    break;
                                case "steal":
                                    Context.Steal_hard = true;
                                    break;
                                case "hostil":
                                    Context.Hostil_hard = true;
                                    break;
                                case "cover_fire":
                                    Context.CoverFire_hard = true;
                                    break;
                                case "force_fire":
                                    Context.ForceFire_hard = true;
                                    break;
                                case "nozone":
                                    Context.NoZone_hard = true;
                                    break;
                            }
                        }
                        break;
                    case "mode_luna":
                        foreach (var token in assign.Content)
                        {
                            switch (token.ToLowerString())
                            {
                                case "samecall":
                                    Context.SameCall_luna = true;
                                    break;
                                case "isblind":
                                    Context.IsBlind_luna = true;
                                    break;
                                case "dead":
                                    Context.Dead_luna = true;
                                    break;
                                case "dead2":
                                    Context.Dead2_luna = true;
                                    break;
                                case "escape":
                                    Context.Escape_luna = true;
                                    break;
                                case "auto":
                                    Context.Auto_luna = true;
                                    break;
                                case "sdown":
                                    Context.SDown_luna = true;
                                    break;
                                case "steal":
                                    Context.Steal_luna = true;
                                    break;
                                case "hostil":
                                    Context.Hostil_luna = true;
                                    break;
                                case "cover_fire":
                                    Context.CoverFire_luna = true;
                                    break;
                                case "force_fire":
                                    Context.ForceFire_luna = true;
                                    break;
                                case "nozone":
                                    Context.NoZone_luna = true;
                                    break;
                            }
                        }
                        break;
                    case "mode_easy_text":
                        Context.ModeText[0] = assign.Content[0].Content;
                        break;
                    case "mode_normal_text":
                        Context.ModeText[1] = assign.Content[0].Content;
                        break;
                    case "mode_hard_text":
                        Context.ModeText[2] = assign.Content[0].Content;
                        break;
                    case "mode_luna_text":
                        Context.ModeText[3] = assign.Content[0].Content;
                        break;
                    case "fv_hp_per":
                        Context.fv_hp_per = firstInt;
                        break;
                    case "fv_mp_per":
                        Context.fv_mp_per = firstInt;
                        break;
                    case "fv_attack_per":
                        Context.fv_attack_per = firstInt;
                        break;
                    case "fv_speed_per":
                        Context.fv_speed_per = firstInt;
                        break;
                    case "fv_move_per":
                        Context.fv_move_per = firstInt;
                        break;
                    case "fv_hprec_per":
                        Context.fv_hprec_per = firstInt;
                        break;
                    case "fv_consti_mul":
                        Context.fv_consti_mul = firstInt;
                        break;
                    case "fv_summon_mul":
                        Context.fv_summon_mul = firstInt;
                        break;
                    case "fv_level_per":
                        Context.fv_level_per = firstInt;
                        break;
                    case "power_order":
                        switch (assign.Content[0].ToLowerString())
                        {
                            case "normal":
                                Context.PowerOrder = ContextData.Order.Normal;
                                break;
                            case "dash":
                                Context.PowerOrder = ContextData.Order.Dash;
                                break;
                            case "test":
                            case "dashtest":
                                Context.PowerOrder = ContextData.Order.DashTest;
                                break;
                            case "normaltest":
                                Context.PowerOrder = ContextData.Order.NormalTest;
                                break;
                        }
                        break;
                    case "talent_mode":
                        Context.TalentMode = firstString == "on";
                        break;
                    case "npm_play":
                        Context.NonPlayerMode = firstString== "on";
                        break;
                    case "default_ending":
                        Context.DefaultEnding = firstString== "on";
                        break;
                    case "picture_floor":
                        if (assign.Content[0].ToLowerString() == "bottom")
                            Context.PictureFloorMsg = false;
                        break;
                    case "race_suffix":
                        Context.RaceSuffix = assign.Content[0].Content;
                        break;
                    case "race_label":
                        Context.RaceLabel = firstString;
                        break;
                    case "sound_volume":
                        Context.SoundVolume = firstInt;
                        break;
                    case "bgm_volume":
                        Context.BGMVolume = firstByte;
                        break;
                    case "gain_per":
                        Context.GainPer = firstInt;
                        break;
                    case "spot_capacity":
                        Context.SpotCapacity = firstByte;
                        break;
                    case "war_capacity":
                        Context.WarCapacity = firstByte;
                        break;
                    case "max_unit":
                        Context.MaxUnit = firstInt;
                        break;
                    case "dead_penalty":
                        Context.DeadPenalty = firstByte;
                        break;
                    case "win_gain":
                        Context.WinGain = firstInt;
                        break;
                    case "training_average":
                        Context.TrainingAverage = firstInt;
                        break;
                    case "leave_period":
                        Context.LeavePeriod = firstByte;
                        break;
                    case "merits_bonus":
                        Context.MeritsBonus = firstInt;
                        break;
                    case "caution_adjust":
                        Context.CautionAdjust = firstByte;
                        break;
                    case "notalent_down":
                        Context.NoTalentAbility = firstInt;
                        break;
                    case "unit_castle_forcefire":
                        Context.unit_castle_forcefire = firstString== "on";
                        break;
                    case "unit_attack_range":
                        Context.unit_attack_range = firstInt;
                        break;
                    case "unit_battle_cram":
                        Context.unit_battle_cram = firstString== "on";
                        break;
                    case "unit_catle_cram":
                        Context.unit_castle_cram = firstString== "on";
                        break;
                    case "unit_drain_mul":
                        Context.unit_drain_mul = firstInt;
                        break;
                    case "unit_element_heal":
                        Context.unit_element_heal = firstString== "on";
                        break;
                    case "unit_escape_range":
                        Context.unit_escape_range = firstInt;
                        break;
                    case "unit_escape_run":
                        Context.unit_escape_run = firstInt;
                        break;
                    case "unit_hand_range":
                        Context.unit_hand_range = firstInt;
                        break;
                    case "unit_keep_form":
                        Context.unit_keep_form = firstString== "on";
                        break;
                    case "unit_level_max":
                        Context.unit_level_max = firstInt;
                        break;
                    case "unit_poison_per":
                        Context.unit_poison_per = firstInt;
                        break;
                    case "unit_retreat_damage":
                        Context.unit_retreat_damage = firstInt;
                        break;
                    case "unit_retreat_speed":
                        Context.unit_retreat_speed = firstInt;
                        break;
                    case "unit_slow_per":
                        Context.unit_slow_per = firstInt;
                        break;
                    case "unit_status_death":
                        Context.unit_status_death = firstInt;
                        break;
                    case "unit_summon_level":
                        Context.unit_summon_level = firstInt;
                        break;
                    case "unit_summon_min":
                        Context.unit_summon_min = firstInt;
                        break;
                    case "unit_view_range":
                        Context.unit_view_range = firstInt;
                        break;
                    case "btl_auto":
                        Context.btl_auto = firstInt;
                        break;
                    case "btl_blind_alpha":
                        Context.btl_blind_alpha = firstByte;
                        break;
                    case "btl_breast_width":
                        Context.btl_breast_width = firstInt;
                        break;
                    case "btl_limit":
                        Context.btl_limit = firstInt;
                        break;
                    case "btl_limit_c":
                        Context.btl_limit_c = firstInt;
                        break;
                    case "btl_lineshift":
                        Context.btl_lineshift = firstInt;
                        break;
                    case "btl_min_damage":
                        Context.btl_min_damage = firstInt;
                        break;
                    case "btl_msgtime":
                        Context.btl_msgtime = firstInt;
                        break;
                    case "btl_nocastle_bdr":
                        Context.btl_nocastle_bdr = firstInt;
                        break;
                    case "btl_prepare_max":
                        Context.btl_prepare_max = firstInt;
                        break;
                    case "btl_prepare_min":
                        Context.btl_prepare_min = firstInt;
                        break;
                    case "btl_replace_line":
                        Context.btl_replace_line = firstByte;
                        break;
                    case "btl_unitmax":
                        Context.btl_unitmax = firstInt;
                        break;
                    case "btl_wingmax":
                        Context.btl_wingmax = firstInt;
                        break;
                    case "neutral_max":
                        Context.neutral_max = firstByte;
                        break;
                    case "neutral_member_max":
                        Context.neutral_member_max = firstByte;
                        break;
                    case "neutral_member_min":
                        Context.neutral_member_min = firstByte;
                        break;
                    case "neutral_min":
                        Context.neutral_min = firstByte;
                        break;
                    case "skillicon_leader":
                        Context.SkillIcon[0] = assign.Content[0].Content;
                        break;
                    case "skillicon_all":
                        Context.SkillIcon[1] = assign.Content[0].Content;
                        break;
                    case "skillicon_add":
                        Context.SkillIcon[2] = assign.Content[0].Content;
                        break;
                    case "skillicon_special":
                        Context.SkillIcon[3] = assign.Content[0].Content;
                        break;
                    case "item_sell_per":
                        Context.ItemSellPercentage = firstInt;
                        break;
                    case "executive_bdr":
                        Context.ExecutiveBorder = firstByte;
                        break;
                    case "senior_bdr":
                        Context.SeniorBorder = firstByte;
                        break;
                    case "employ_price_coe":
                        Context.EmployPrice = firstInt;
                        break;
                    case "vassal_price_coe":
                        Context.VassalPrice = firstInt;
                        break;
                    case "support_gain":
                        Context.support_gain = firstInt;
                        break;
                    case "compati_vassal_bdr":
                        Context.compati_vassal_bdr = firstByte;
                        break;
                    case "compati_bad":
                        Context.compati_bad = firstByte;
                        break;
                    case "loyal_down":
                        Context.loyal_down = firstByte;
                        break;
                    case "loyal_up":
                        Context.loyal_up = firstByte;
                        break;
                    case "loyal_border":
                        Context.loyal_border = firstInt;
                        break;
                    case "loyal_escape_border":
                        Context.loyal_escape_border = firstInt;
                        break;
                    case "arbeit_gain":
                        Context.arbeit_gain = firstInt;
                        break;
                    case "arbeit_player":
                        Context.arbeit_player = firstString== "on";
                        break;
                    case "arbeit_price_coe":
                        Context.arbeit_price_coe = firstInt;
                        break;
                    case "arbeit_turn":
                        Context.arbeit_turn = firstByte;
                        break;
                    case "arbeit_vassal_coe":
                        Context.arbeit_vassal_coe = firstInt;
                        break;
                    case "raid_border":
                        Context.RaidBorder = firstInt;
                        break;
                    case "raid_coe":
                        Context.RaidCoe = firstInt;
                        break;
                    case "raid_max":
                        Context.RaidMax = firstInt;
                        break;
                    case "raid_min":
                        Context.RaidMin = firstInt;
                        break;
                    case "btl_castle_lot":
                        Context.BattleCastleLot = firstInt;
                        break;
                    case "scenario_select2":
                        Context.ScenarioSelect2On = true;
                        Context.ScenarioSelect2Percentage = firstByte;
                        Context.ScenarioSelect2 = assign.Content[1].Content;
                        break;
                    case "unit_image_right":
                        Context.UnitImageRight = firstString== "on";
                        break;
                    case "font_file":
                        foreach (var item in assign.Content)
                            Context.FontFiles.Add(item.Content);
                        break;
                    case "color_blue":
                        Context.ColorBlue[0] = (byte)assign.Content[0].Number;
                        Context.ColorBlue[1] = (byte)assign.Content[1].Number;
                        Context.ColorBlue[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_red":
                        Context.ColorRed[0] = (byte)assign.Content[0].Number;
                        Context.ColorRed[1] = (byte)assign.Content[1].Number;
                        Context.ColorRed[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_text":
                        Context.ColorText[0] = (byte)assign.Content[0].Number;
                        Context.ColorText[1] = (byte)assign.Content[1].Number;
                        Context.ColorText[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_name":
                        Context.ColorName[0] = (byte)assign.Content[0].Number;
                        Context.ColorName[1] = (byte)assign.Content[1].Number;
                        Context.ColorName[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_nameHelp":
                        Context.ColorNameHelp[0] = (byte)assign.Content[0].Number;
                        Context.ColorNameHelp[1] = (byte)assign.Content[1].Number;
                        Context.ColorNameHelp[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_white":
                        Context.ColorWhite[0] = (byte)assign.Content[0].Number;
                        Context.ColorWhite[1] = (byte)assign.Content[1].Number;
                        Context.ColorWhite[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_magenta":
                        Context.ColorMagenta[0] = (byte)assign.Content[0].Number;
                        Context.ColorMagenta[1] = (byte)assign.Content[1].Number;
                        Context.ColorMagenta[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_green":
                        Context.ColorGreen[0] = (byte)assign.Content[0].Number;
                        Context.ColorGreen[1] = (byte)assign.Content[1].Number;
                        Context.ColorGreen[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_cyan":
                        Context.ColorCyan[0] = (byte)assign.Content[0].Number;
                        Context.ColorCyan[1] = (byte)assign.Content[1].Number;
                        Context.ColorCyan[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_yellow":
                        Context.ColorYellow[0] = (byte)assign.Content[0].Number;
                        Context.ColorYellow[1] = (byte)assign.Content[1].Number;
                        Context.ColorYellow[2] = (byte)assign.Content[2].Number;
                        break;
                    case "color_orange":
                        Context.ColorOrange[0] = (byte)assign.Content[0].Number;
                        Context.ColorOrange[1] = (byte)assign.Content[1].Number;
                        Context.ColorOrange[2] = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_button":
                        Context.WindowBottom = (sbyte)assign.Content[0].Number;
                        break;
                    case "wnd_menu":
                        Context.Window_menu[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_menu[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_menu = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_heromenu":
                        Context.Window_heromenu[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_heromenu[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_heromenu = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_commenu":
                        Context.Window_commenu[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_commenu[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_commenu = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_spot":
                        Context.Window_spot[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_spot[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_spot = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_spot2":
                        Context.Window_spot2[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_spot2[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_spot2 = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_status":
                        Context.Window_status[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_status[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_status = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_merce":
                        Context.Window_merce[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_merce[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_merce = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_war":
                        Context.Window_war[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_war[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_war = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_diplo":
                        Context.Window_diplo[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_diplo[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_diplo = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_powerinfo":
                        Context.Window_powerinfo[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_powerinfo[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_powerinfo = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_personinfo":
                        Context.Window_personinfo[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_personinfo[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_personinfo = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_save":
                        Context.Window_save[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_save[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_save = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_load":
                        Context.Window_load[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_load[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_load = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_tool":
                        Context.Window_tool[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_tool[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_tool = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_battle":
                        Context.Window_battle[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_battle[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_battle = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_message":
                        Context.Window_message[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_message[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_message = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_name":
                        Context.Window_name[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_name[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_name = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_dialog":
                        Context.Window_dialog[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_dialog[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_dialog = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_help":
                        Context.Window_help[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_help[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_help = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_detail":
                        Context.Window_detail[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_detail[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_detail = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_corps":
                        Context.Window_corps[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_corps[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_corps = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_bstatus":
                        Context.Window_bstatus[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_bstatus[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_bstatus = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_powerselect":
                        Context.Window_powerselect[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_powerselect[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_powerselect = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_personselect":
                        Context.Window_personselect[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_personselect[1] = (sbyte)assign.Content[1].Number;
                        Context.Alpha_Window_personselect = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_scenario":
                        Context.Window_scenario[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_scenario[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_scenario = (byte)assign.Content[2].Number;
                        break;
                    case "wnd_scenariotext":
                        Context.Window_scenariotext[0] = (sbyte)assign.Content[0].Number;
                        if (assign.Content.Count > 1)
                            Context.Window_scenariotext[1] = (sbyte)assign.Content[1].Number;
                        if (assign.Content.Count > 2)
                            Context.Alpha_Window_scenariotext = (byte)assign.Content[2].Number;
                        break;
                    case "fullbody_detail":
                        Context.FullBodyDetail = firstString== "on";
                        break;
                    default:
                        Context.VariantData[assign.Name] = assign.Content;
                        break;
                }
            }
        }
    }
}