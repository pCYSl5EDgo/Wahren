namespace Wahren.AbstractSyntaxTree.Project;

using Parser;

public sealed partial class Project
{
    private void SpecialTreatment_unit_friend(ref Result result, ref UnitNode node, ref Pair_NullableString_NullableInt value)
    {
        SpecialTreatment_unit_class_friend(ref result, ref value, "Unit");
    }

    private void SpecialTreatment_class_friend(ref Result result, ref ClassNode node, ref Pair_NullableString_NullableInt value)
    {
        SpecialTreatment_unit_class_friend(ref result, ref value, "Class");
    }

    private void SpecialTreatment_unit_class_friend(ref Result result, ref Pair_NullableString_NullableInt value, string kind)
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
                value.ReferenceId = result.UnitSet.GetOrAdd(span, value.Text);
                value.ReferenceKind = ReferenceKind.Unit;
                value.HasReference = true;
                break;
            case ReferenceKind.Class:
                value.ReferenceId = result.ClassSet.GetOrAdd(span, value.Text);
                value.ReferenceKind = ReferenceKind.Class;
                value.HasReference = true;
                break;
            case ReferenceKind.Race:
                value.ReferenceId = result.RaceSet.GetOrAdd(span, value.Text);
                value.ReferenceKind = ReferenceKind.Race;
                value.HasReference = true;
                break;
            default:
                result.ErrorAdd_UnexpectedElementReferenceKind(kind, "friend", "Race, Unit, Class", value.Text);
                break;
        }
    }

    private void SpecialTreatment_skill_offset(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_homing(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_add2(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_add(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_attr(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_image(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }

    private void SpecialTreatment_skill_yorozu(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableInt value)
    {
    }
}
