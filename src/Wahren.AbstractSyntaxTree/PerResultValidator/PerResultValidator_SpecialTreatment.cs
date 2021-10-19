namespace Wahren.AbstractSyntaxTree.Parser;

using Element;

public static partial class PerResultValidator
{
    private static void SpecialTreatment_skill_gun_delay(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableIntElement? value)
    {
        if (value is { HasValue: true })
        {
            ref var v = ref value.Value;
            if (v.HasText)
            {
                var span = result.GetSpan(v.Text);
                if (int.TryParse(span, out _))
                {
                    return;
                }
            }
            if (!v.HasNumber)
            {
                result.ErrorAdd_UnexpectedElementReferenceKind("skill", "gun_delay", "Number", v.Text);
            }
        }
    }

    private static void SpecialTreatment_skill_func(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableIntElement? value)
    {
        if (value is null)
        {
            if (node.HasSuper)
            {
                var superSpan = result.GetSpan(node.Super);
                node.SkillKind = SkillKind.Unknown;
                foreach (ref var other in result.SkillNodeList.AsSpan())
                {
                    if (other.Name == node.Name || !superSpan.SequenceEqual(result.GetSpan(other.Name)))
                    {
                        continue;
                    }

                    node.SkillKind = other.SkillKind;
                    break;
                }
            }
            else
            {
                node.SkillKind = SkillKind.missile;
            }
        }
        else
        {
            if (value.HasValue)
            {
                ref var v = ref value.Value;
                var span = result.GetSpan(v.Text);
                v.HasReference = true;
                v.ReferenceKind = ReferenceKind.Special;
                if (span.SequenceEqual("missile"))
                {
                    v.ReferenceId = 0;
                    node.SkillKind = SkillKind.missile;
                }
                else if (span.SequenceEqual("sword"))
                {
                    v.ReferenceId = 1;
                    node.SkillKind = SkillKind.sword;
                }
                else if (span.SequenceEqual("heal"))
                {
                    v.ReferenceId = 2;
                    node.SkillKind = SkillKind.heal;
                }
                else if (span.SequenceEqual("summon"))
                {
                    v.ReferenceId = 3;
                    node.SkillKind = SkillKind.summon;
                }
                else if (span.SequenceEqual("charge"))
                {
                    v.ReferenceId = 4;
                    node.SkillKind = SkillKind.charge;
                }
                else if (span.SequenceEqual("status"))
                {
                    v.ReferenceId = 5;
                    node.SkillKind = SkillKind.status;
                }
                else
                {
                    v.HasReference = false;
                    result.ErrorAdd_UnexpectedElementSpecialValue("skill", "func", "missile, sword, heal, summon, charge, status", v.Text);
                    node.SkillKind = SkillKind.Unknown;
                }
            }
            else
            {
                node.SkillKind = SkillKind.missile;
            }
        }
    }

    private static void SpecialTreatment_skill_movetype(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableIntElement? value)
    {
        node.SkillMovetype = SkillMovetype.Unknown;
        if (value is not { HasValue: true })
        {
            if (node.speed is { HasValue: true, Value.HasNumber: true })
            {
                node.SkillMovetype = node.speed.Value.Number == 0 ? SkillMovetype.Stop : SkillMovetype.Straight;
            }
            return;
        }

        ref var v = ref value.Value;
        var span = result.GetSpan(v.Text);
        v.HasReference = true;
        v.ReferenceKind = ReferenceKind.Special;
        if (span.SequenceEqual("arc"))
        {
            v.ReferenceId = 0;
            node.SkillMovetype = SkillMovetype.arc;
        }
        else if (span.SequenceEqual("drop"))
        {
            v.ReferenceId = 1;
            if (node.speed is { HasValue: true, Value.HasNumber: true })
            {
                var speed = node.speed.Value.Number;
                if (speed != 0)
                {
                    node.SkillMovetype = speed > 0 ? SkillMovetype.drop : SkillMovetype.DropUpper;
                }
            }
        }
        else if (span.SequenceEqual("throw"))
        {
            v.ReferenceId = 2;
            node.SkillMovetype = SkillMovetype.@throw;
        }
        else if (span.SequenceEqual("circle"))
        {
            v.ReferenceId = 3;
            node.SkillMovetype = SkillMovetype.circle;
        }
        else if (span.SequenceEqual("swing"))
        {
            v.ReferenceId = 4;
            node.SkillMovetype = SkillMovetype.swing;
        }
        else
        {
            v.HasReference = false;
            result.ErrorAdd_UnexpectedElementSpecialValue("skill", "movetype", "arc, drop, throw, circle, swing", v.Text);
        }
    }
}
