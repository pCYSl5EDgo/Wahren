namespace Wahren.AbstractSyntaxTree.Project;

using Parser;

public sealed partial class Project
{
    private void SpecialTreatment_unit_picture(ref Result result, AnalysisResult analysisResult, ref UnitNode node, ref Pair_NullableString_NullableInt value)
    {
        SpecialTreatment_unit_class_picture(ref result, analysisResult, ref value, "unit");
    }

    private void SpecialTreatment_class_picture(ref Result result, AnalysisResult analysisResult, ref ClassNode node, ref Pair_NullableString_NullableInt value)
    {
        SpecialTreatment_unit_class_picture(ref result, analysisResult, ref value, "class");
    }

    private void SpecialTreatment_unit_class_picture(ref Result result, AnalysisResult analysisResult, ref Pair_NullableString_NullableInt value, string kind)
    {
    }

    private void SpecialTreatment_unit_friend(ref Result result, AnalysisResult analysisResult, ref UnitNode node, ref Pair_NullableString_NullableInt value)
    {
        SpecialTreatment_unit_class_friend(ref result, analysisResult, ref value, "unit");
    }

    private void SpecialTreatment_class_friend(ref Result result, AnalysisResult analysisResult, ref ClassNode node, ref Pair_NullableString_NullableInt value)
    {
        SpecialTreatment_unit_class_friend(ref result, analysisResult, ref value, "class");
    }

    private void SpecialTreatment_unit_class_friend(ref Result result, AnalysisResult analysisResult, ref Pair_NullableString_NullableInt value, string kind)
    {
        if (!value.HasText)
        {
            return;
        }
        var span = result.GetSpan(value.Text);
        if (value.TrailingTokenCount != 0)
        {
            result.ErrorAdd_UnexpectedElementReferenceKind(kind, "friend", "Race, Unit, Class", value.Text);
            return;
        }
        if (span.SequenceEqual("allclass"))
        {
            return;
        }
        else if (span.SequenceEqual("allrace"))
        {
            return;
        }
        ref var reference = ref AmbiguousDictionary_UnitClassPowerSpotRace.TryGet(span);
        if (Unsafe.IsNullRef(ref reference))
        {
            result.ErrorAdd_UnexpectedElementReferenceKind(kind, "friend", "Race, Unit, Class", value.Text);
            return;
        }
        switch (reference.Kind)
        {
            case ReferenceKind.Unit:
                value.ReferenceId = analysisResult.UnitSet.GetOrAdd(span, value.Text);
                value.ReferenceKind = ReferenceKind.Unit;
                value.HasReference = true;
                break;
            case ReferenceKind.Class:
                value.ReferenceId = analysisResult.ClassSet.GetOrAdd(span, value.Text);
                value.ReferenceKind = ReferenceKind.Class;
                value.HasReference = true;
                break;
            case ReferenceKind.Race:
                value.ReferenceId = analysisResult.RaceSet.GetOrAdd(span, value.Text);
                value.ReferenceKind = ReferenceKind.Race;
                value.HasReference = true;
                break;
            default:
                result.ErrorAdd_UnexpectedElementReferenceKind(kind, "friend", "Race, Unit, Class", value.Text);
                break;
        }
    }

    private void SpecialTreatment_skill_offset(ref Result result, AnalysisResult analysisResult, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_homing(ref Result result, AnalysisResult analysisResult, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_add2(ref Result result, AnalysisResult analysisResult, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_add(ref Result result, AnalysisResult analysisResult, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_attr(ref Result result, AnalysisResult analysisResult, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
        var kind = GetRecursive_SkillKind(ref result, ref node);
        var span = result.GetSpan(value.Text);
        switch (kind)
        {
            case SkillKind.heal:
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                if (PerResultValidator.IsStatus(span, out value.ReferenceId))
                {
                }
                else if (PerResultValidator.IsAbnormalAttribute(span, out value.ReferenceId))
                {
                    value.ReferenceId += 11;
                }
                else if (span.SequenceEqual("all"))
                {
                    value.ReferenceId = 18;
                }
                else
                {
                    value.HasReference = false;
                    result.ErrorAdd_UnexpectedElementReferenceKind("skill", "attr", "heal -> Status, AbnormalAttribute, all", value.Text);
                }
                break;
            case SkillKind.status:
                if (PerResultValidator.IsStatus(span, out value.ReferenceId))
                {
                }
                else if (PerResultValidator.IsAbnormalAttribute(span, out value.ReferenceId))
                {
                    value.ReferenceId += 11;
                }
                else if (span.SequenceEqual("summon_max"))
                {
                    value.ReferenceId = 18;
                }
                else if (span.SequenceEqual("training"))
                {
                    value.ReferenceId = 19;
                }
                else if (span.SequenceEqual("movetype"))
                {
                    value.ReferenceId = 20;
                }
                else
                {
                    value.HasReference = false;
                    result.ErrorAdd_UnexpectedElementReferenceKind("skill", "attr", "status -> Status, AbnormalAttribute, summon_max, training, movetype", value.Text);
                }
                break;
            default:
                value.ReferenceId = analysisResult.AttributeTypeSet.GetOrAdd(span, value.Text);
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.AttributeType;
                break;
        }
    }

    private void SpecialTreatment_skill_image(ref Result result, AnalysisResult analysisResult, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
        var isGround0 = GetRecursive_IsGround0(ref result, ref node);
        ref var set = ref (isGround0 ? ref analysisResult.imagedata2Set : ref analysisResult.imagedataSet);
        value.ReferenceId = set.GetOrAdd(result.GetSpan(value.Text), value.Text);
        value.HasReference = true;
        value.ReferenceKind = isGround0 ? ReferenceKind.imagedata2 : ReferenceKind.imagedata;
    }

    private void SpecialTreatment_skill_yorozu(ref Result result, AnalysisResult analysisResult, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private SkillMovetype GetRecursive_SkillMovetype(ref Result result, ref SkillNode node)
    {
        if (node.SkillMovetype != SkillMovetype.Unknown)
        {
            return node.SkillMovetype;
        }

        if (node.movetype is not null)
        {
            if (node.movetype.HasValue)
            {
                if (node.movetype.Value.ReferenceId == 1)
                {
                    var speed = GetRecursive_speed_Value(ref result, ref node);
                    if (!speed.HasValue || speed.Value == 0)
                    {
                        if (GetRecursive_SkillKind(ref result, ref node) != SkillKind.heal)
                        {
                            result.ErrorAdd_ElementValueInvalid_NotEqual("skill", "speed", 0, node.speed!.Value.Text);
                        }
                        return node.SkillMovetype = SkillMovetype.drop;
                    }
                    else if (speed.Value > 0)
                    {
                        return node.SkillMovetype = SkillMovetype.drop;
                    }
                    else
                    {
                        return node.SkillMovetype = SkillMovetype.DropUpper;
                    }
                }
                else
                {
                    result.ErrorAdd_ElementValueInvalid_Equal("skill", "movetype", "drop", node.movetype.Value.Text);
                    return default;
                }
            }
            else
            {
                var speed = GetRecursive_speed_Value(ref result, ref node);
                return node.SkillMovetype = !speed.HasValue || speed.Value == 0 ? SkillMovetype.Stop : SkillMovetype.Straight;
            }
        }

        if (!node.HasSuper)
        {
            return default;
        }

        var superSpan = result.GetSpan(node.Super);
        ref var track = ref AmbiguousDictionary_SkillSkillset.TryGet(superSpan);
        ref var superResult = ref Files[track.ResultId];
        return node.SkillMovetype = GetRecursive_SkillMovetype(ref superResult, ref superResult.SkillNodeList[track.NodeIndex]);
    }

    private int? GetRecursive_speed_Value(ref Result result, ref SkillNode node)
    {
        if (node.speed is not null)
        {
            if (!node.speed.HasValue || !node.speed.Value.HasNumber)
            {
                return default;
            }

            return node.speed.Value.Number;
        }

        if (!node.HasSuper)
        {
            return default;
        }

        var superSpan = result.GetSpan(node.Super);
        ref var track = ref AmbiguousDictionary_SkillSkillset.TryGet(superSpan);
        ref var superResult = ref Files[track.ResultId];
        return GetRecursive_speed_Value(ref superResult, ref superResult.SkillNodeList[track.NodeIndex]);
    }

    private SkillKind GetRecursive_SkillKind(ref Result result, ref SkillNode node)
    {
        if (node.SkillKind != SkillKind.Unknown)
        {
            return node.SkillKind;
        }

        if (!node.HasSuper)
        {
            return node.SkillKind = SkillKind.missile;
        }

        var superSpan = result.GetSpan(node.Super);
        ref var track = ref AmbiguousDictionary_SkillSkillset.TryGet(superSpan);
        ref var superResult = ref Files[track.ResultId];
        return node.SkillKind = GetRecursive_SkillKind(ref superResult, ref superResult.SkillNodeList[track.NodeIndex]);
    }

    private bool GetRecursive_IsGround0(ref Result result, ref SkillNode node)
    {
        if (node.ground is not null)
        {
            if (!node.ground.HasValue || !node.ground.Value.HasNumber)
            {
                return default;
            }

            return node.ground.Value.Number == 0;
        }

        if (!node.HasSuper)
        {
            return default;
        }

        var superSpan = result.GetSpan(node.Super);
        ref var track = ref AmbiguousDictionary_SkillSkillset.TryGet(superSpan);
        ref var superResult = ref Files[track.ResultId];
        return GetRecursive_IsGround0(ref superResult, ref superResult.SkillNodeList[track.NodeIndex]);
    }
}
