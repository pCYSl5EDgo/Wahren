namespace Wahren.AbstractSyntaxTree.Parser;

using Element;

public static partial class PerResultValidator
{
    public static void AddReference(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ref StringSpanKeySlowSet set, ReferenceKind kind)
    {
        if (pair.Value is not null && pair.Value.HasValue)
        {
            ref var value = ref pair.Value.Value;
            value.ReferenceKind = kind;
            value.ReferenceId = set.GetOrAdd(result.GetSpan(value.Text), value.Text);
            value.HasReference = true;
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is null || !item.HasValue)
            {
                continue;
            }

            ref var value = ref item.Value;
            value.ReferenceKind = kind;
            value.ReferenceId = set.GetOrAdd(result.GetSpan(value.Text), value.Text);
            value.HasReference = true;
        }
    }

    public static void AddReference(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, ref StringSpanKeySlowSet set, ReferenceKind kind)
    {
        if (pair.Value is not null && pair.Value.HasValue)
        {
            ref var list = ref pair.Value.Value;
            if (!list.IsEmpty)
            {
                ref var value = ref list[0];
                value.ReferenceKind = kind;
                value.ReferenceId = set.GetOrAdd(result.GetSpan(value.Text), value.Text);
                value.HasReference = true;
            }

            for (int i = 1; i < list.Count; i++)
            {
                ref var prev = ref list[i - 1];
                ref var value = ref list[i];
                if (value.Text == prev.Text)
                {
                    continue;
                }

                value.ReferenceKind = kind;
                value.ReferenceId = set.GetOrAdd(result.GetSpan(value.Text), value.Text);
                value.HasReference = true;
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is null || !item.HasValue)
            {
                continue;
            }

            ref var list = ref item.Value;
            if (list.IsEmpty)
            {
                continue;
            }

            {
                ref var value = ref list[0];
                value.ReferenceKind = kind;
                value.ReferenceId = set.GetOrAdd(result.GetSpan(value.Text), value.Text);
                value.HasReference = true;
            }

            for (int i = 1; i < list.Count; i++)
            {
                ref var prev = ref list[i - 1];
                ref var value = ref list[i];
                if (value.Text == prev.Text)
                {
                    continue;
                }

                value.ReferenceKind = kind;
                value.ReferenceId = set.GetOrAdd(result.GetSpan(value.Text), value.Text);
                value.HasReference = true;
            }
        }
    }

    private static void AddReference(ref Result result, ref Pair_NullableString_NullableIntElement? value, ref StringSpanKeySlowSet set, ReferenceKind kind)
    {
        if (value is not { HasValue: true })
        {
            return;
        }

        ref var v = ref value.Value;
        v.ReferenceKind = kind;
        v.HasReference = true;
        v.ReferenceId = set.GetOrAdd(result.GetSpan(v.Text), v.Text);
    }

    private static void ValidateNumber(ref Result result, ref Pair_NullableString_NullableInt_ArrayElement? value, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (value is not { HasValue: true, Value.Count: > 0 })
        {
            return;
        }

        foreach (ref var v in value.Value.AsSpan())
        {
            if (!v.HasNumber)
            {
                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Number", v.Text);
                return;
            }
        }
    }


    private static void ValidateBooleanNumber(ref Result result, ref Pair_NullableString_NullableIntElement? value, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (value is not { HasValue: true })
        {
            return;
        }

        ref var v = ref value.Value;
        if (IsBoolean(result.GetSpan(v.Text), out v.ReferenceId))
        {
            v.ReferenceKind = ReferenceKind.Boolean;
            v.HasReference = true;
        }
        else if (!v.HasNumber)
        {
            result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Boolean, Number", v.Text);
        }
    }

    private static void ValidateBoolean(ref Result result, ref Pair_NullableString_NullableIntElement? value, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (value is not { HasValue: true })
        {
            return;
        }

        ref var v = ref value.Value;
        if (v.HasReference = IsBoolean(result.GetSpan(v.Text), out v.ReferenceId))
        {
            v.ReferenceKind = ReferenceKind.Boolean;
        }
        else
        {
            result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Boolean", v.Text);
        }
    }

    private static void AddReference(ref Result result, ref Pair_NullableString_NullableInt_ArrayElement? value, ref StringSpanKeySlowSet set, ReferenceKind kind)
    {
        if (value is not { HasValue: true, Value.Count: > 0 })
        {
            return;
        }

        foreach (ref var v in value.Value.AsSpan())
        {
            v.ReferenceKind = kind;
            v.HasReference = true;
            v.ReferenceId = set.GetOrAdd(result.GetSpan(v.Text), v.Text);
        }
    }

    public static void AddReferenceSkillBoolean(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (pair.Value is { HasValue: true })
        {
            ref var value = ref pair.Value.Value;
            if (value.HasNumber)
            {
                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Skill, Boolean", value.Text);
                return;
            }

            var span = result.GetSpan(value.Text);
            value.HasReference = true;
            if (IsBoolean(span, out value.ReferenceId))
            {
                value.ReferenceKind = ReferenceKind.Boolean;
            }
            else
            {
                value.ReferenceKind = ReferenceKind.Skill;
                value.ReferenceId = result.SkillSet.GetOrAdd(span, value.Text);
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is null || !item.HasValue)
            {
                continue;
            }

            ref var value = ref item.Value;
            if (value.HasNumber)
            {
                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Skill, Boolean", value.Text);
                return;
            }

            var span = result.GetSpan(value.Text);
            value.HasReference = true;
            if (IsBoolean(span, out value.ReferenceId))
            {
                value.ReferenceKind = ReferenceKind.Boolean;
            }
            else
            {
                value.ReferenceKind = ReferenceKind.Skill;
                value.ReferenceId = result.SkillSet.GetOrAdd(span, value.Text);
            }
        }
    }

    private static void ValidateNumber(ref Result result, ref Pair_NullableString_NullableIntElement? value, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (value is not { HasValue: true })
        {
            return;
        }

        if (!value.Value.HasNumber)
        {
            result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Number", value.Value.Text);
        }
    }

    private static void ValidateNumber(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        ValidateNumber(ref result, ref pair.Value, nodeKind, elementName);

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (ref var item in pair.VariantArray.AsSpan())
        {
            ValidateNumber(ref result, ref item, nodeKind, elementName);
        }
    }

    private static void ValidateNumber(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (pair.Value is not null && pair.Value.HasValue)
        {
            foreach (ref var value in pair.Value.Value)
            {
                if (value.HasNumber)
                {
                    continue;
                }

                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Number", value.Text);
                return;
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is null || !item.HasValue)
            {
                continue;
            }

            foreach (ref var value in item.Value)
            {
                if (value.HasNumber)
                {
                    continue;
                }

                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Number", value.Text);
                return;
            }
        }
    }

    public static void ValidateBoolean(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (pair is { Value.HasValue: true })
        {
            ref var value = ref pair.Value.Value;
            if (!value.HasNumber)
            {
                var span = result.GetSpan(value.Text);
                if (IsBoolean(span, out value.ReferenceId))
                {
                    value.HasReference = true;
                    value.ReferenceKind = ReferenceKind.Boolean;
                }
                else
                {
                    result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Boolean", value.Text);
                    return;
                }
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is not { HasValue: true, Value.HasNumber: false })
            {
                continue;
            }

            ref var value = ref item.Value;
            var span = result.GetSpan(value.Text);
            if (IsBoolean(span, out value.ReferenceId))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Boolean;
                continue;
            }

            result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Boolean", value.Text);
            return;
        }
    }

    public static void ValidateBoolean(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (pair.Value is not null && pair.Value.HasValue)
        {
            foreach (ref var value in pair.Value.Value)
            {
                if (!value.HasNumber)
                {
                    var span = result.GetSpan(value.Text);
                    if (IsBoolean(span, out value.ReferenceId))
                    {
                        value.HasReference = true;
                        value.ReferenceKind = ReferenceKind.Boolean;
                        continue;
                    }
                }

                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Boolean", value.Text);
                return;
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is null || !item.HasValue)
            {
                continue;
            }

            foreach (ref var value in item.Value)
            {
                if (!value.HasNumber)
                {
                    var span = result.GetSpan(value.Text);
                    if (IsBoolean(span, out value.ReferenceId))
                    {
                        value.HasReference = true;
                        value.ReferenceKind = ReferenceKind.Boolean;
                        continue;
                    }
                }

                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Boolean", value.Text);
                return;
            }
        }
    }

    internal static void ValidateBooleanNumber(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (pair is { Value.HasValue: true })
        {
            ref var value = ref pair.Value.Value;
            if (!value.HasNumber)
            {
                var span = result.GetSpan(value.Text);
                if (!span.SequenceEqual("on") && !span.SequenceEqual("off"))
                {
                    result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Boolean, Number", value.Text);
                }
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is not { HasValue: true })
            {
                continue;
            }

            ref var value = ref item.Value;
            if (value.HasNumber)
            {
                continue;
            }

            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("on") || span.SequenceEqual("off"))
            {
                continue;
            }

            result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Boolean, Number", value.Text);
        }
    }

    public static void ValidateStatus(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (pair is { Value.HasValue: true })
        {
            ref var value = ref pair.Value.Value;
            var span = result.GetSpan(value.Text);
            if (IsStatus(span, out _))
            {
                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Status", value.Text);
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is not { HasValue: true })
            {
                continue;
            }

            ref var value = ref item.Value;
            var span = result.GetSpan(value.Text);
            if (IsStatus(span, out _))
            {
                continue;
            }

            result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Status", value.Text);
        }
    }

    public static void ValidateStatus(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (pair.Value is not null && pair.Value.HasValue)
        {
            foreach (ref var value in pair.Value.Value)
            {
                var span = result.GetSpan(value.Text);
                if (IsStatus(span, out _))
                {
                    continue;
                }

                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Status", value.Text);
                break;
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is null || !item.HasValue)
            {
                continue;
            }

            foreach (ref var value in item.Value)
            {
                var span = result.GetSpan(value.Text);
                if (IsStatus(span, out _))
                {
                    continue;
                }

                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Status", value.Text);
                break;
            }
        }
    }

    public static void ValidateStatusNumber(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (pair.Value is not null && pair.Value.HasValue)
        {
            foreach (ref var value in pair.Value.Value)
            {
                var span = result.GetSpan(value.Text);
                if (value.HasNumber || IsStatus(span, out _))
                {
                    continue;
                }

                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Status, Number", value.Text);
                break;
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is null || !item.HasValue)
            {
                continue;
            }

            foreach (ref var value in item.Value)
            {
                var span = result.GetSpan(value.Text);
                if (value.HasNumber || IsStatus(span, out _))
                {
                    continue;
                }

                result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Status, Number", value.Text);
                break;
            }
        }
    }

    internal static bool IsStatus(ReadOnlySpan<char> span, out uint referenceId)
    {
        switch (span.Length)
        {
            case 2 when span[1] == 'p':
                switch (span[0])
                {
                    case 'h':
                        referenceId = 0;
                        return true;
                    case 'm':
                        referenceId = 1;
                        return true;
                }
                goto default;
            case 4 when span.SequenceEqual("dext"):
                referenceId = 7;
                return true;
            case 4 when span.SequenceEqual("move"):
                referenceId = 8;
                return true;
            case 5 when span.Slice(1).SequenceEqual("prec"):
                switch (span[0])
                {
                    case 'h':
                        referenceId = 9;
                        return true;
                    case 'm':
                        referenceId = 10;
                        return true;
                }
                goto default;
            case 5 when span.SequenceEqual("magic"):
                referenceId = 4;
                return true;
            case 5 when span.SequenceEqual("speed"):
                referenceId = 6;
                return true;
            case 6 when span.SequenceEqual("attack"):
                referenceId = 2;
                return true;
            case 6 when span.SequenceEqual("magdef"):
                referenceId = 5;
                return true;
            case 7 when span.SequenceEqual("defense"):
                referenceId = 3;
                return true;
            default:
                referenceId = 0;
                return false;
        }
    }

    internal static bool IsBoolean(ReadOnlySpan<char> span, out uint referenceId)
    {
        switch (span.Length)
        {
            case 2 when span.SequenceEqual("on"):
                referenceId = 1;
                return true;
            case 3 when span.SequenceEqual("off"):
                referenceId = 0;
                return true;
            default:
                referenceId = 0;
                return false;
        }
    }

    public static void ValidateRedBlue(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (pair is { Value.HasValue: true })
        {
            ref var value = ref pair.Value.Value;
            if (!value.HasNumber)
            {
                var span = result.GetSpan(value.Text);
                if (!IsRedBlue(span, out _))
                {
                    result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "RedBlue", value.Text);
                }
            }
        }

        if (pair.VariantArray is null)
        {
            return;
        }

        foreach (var item in pair.VariantArray)
        {
            if (item is not { HasValue: true, Value.HasNumber: false })
            {
                continue;
            }

            ref var value = ref item.Value;
            var span = result.GetSpan(value.Text);
            if (IsRedBlue(span, out _))
            {
                continue;
            }

            result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "RedBlue", value.Text);
        }
    }

    internal static bool IsRedBlue(ReadOnlySpan<char> span, out uint referenceId)
    {
        switch (span.Length)
        {
            case 3 when span.SequenceEqual("red"):
                referenceId = 0;
                return true;
            case 4 when span.SequenceEqual("blue"):
                referenceId = 1;
                return true;
            default:
                referenceId = 0;
                return false;
        }
    }

    private static void SpecialTreatment_skill_gun_delay(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableIntElement? value)
    {
        static void Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            if (value.HasText)
            {
                var span = result.GetSpan(value.Text);
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Skill;
                value.ReferenceId = result.SkillSet.GetOrAdd(span, value.Text);
            }
            if (!value.HasNumber)
            {
                result.ErrorAdd_UnexpectedElementReferenceKind("Skill", "gun_delay", "Skill, Number", value.Text);
            }
        }

        if (value is { HasValue: true })
        {
            Validate(ref result, ref value.Value);
        }
    }

    private static void SpecialTreatment_skill_func(ref Result result, ref SkillNode node, ref Pair_NullableString_NullableIntElement? value)
	{
		static void Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
			value.HasReference = true;
			value.ReferenceKind = ReferenceKind.Special;
            if (span.SequenceEqual("missile"))
            {
                value.ReferenceId = 0;
            }
            else if (span.SequenceEqual("sword"))
            {
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("heal"))
            {
                value.ReferenceId = 2;
            }
            else if (span.SequenceEqual("summon"))
            {
                value.ReferenceId = 3;
            }
            else if (span.SequenceEqual("charge"))
            {
                value.ReferenceId = 4;
            }
            else if (span.SequenceEqual("status"))
            {
                value.ReferenceId = 5;
            }
            else
            {
                value.HasReference = false;
                result.ErrorAdd_UnexpectedElementSpecialValue("Skill", "func", "missile, sword, heal, summon, charge, status", value.Text);
            }
        }

        if (value is null)
        {
            if (node.Super.HasValue)
            {
                var superSpan = result.SkillSet[node.Super.Value];
                node.SkillKind = SkillKind.Unknown;
                foreach(ref var other in result.SkillNodeList.AsSpan())
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
                Validate(ref result, ref value.Value);
                node.SkillKind = (SkillKind)value.Value.ReferenceId;
            }
            else
            {
                node.SkillKind = SkillKind.missile;
            }
        }
	}

    private static void SpecialTreatment_skill_movetype(ref Result result, ref SkillNode node, ref VariantPair<Pair_NullableString_NullableIntElement> pair)
	{
		static void Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
			value.HasReference = true;
			value.ReferenceKind = ReferenceKind.Special;
            if (span.SequenceEqual("arc"))
            {
                value.ReferenceId = 0;
            }
            else if (span.SequenceEqual("drop"))
            {
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("throw"))
            {
                value.ReferenceId = 2;
            }
            else if (span.SequenceEqual("circle"))
            {
                value.ReferenceId = 3;
            }
            else if (span.SequenceEqual("swing"))
            {
                value.ReferenceId = 4;
            }
            else
            {
                value.HasReference = false;
                result.ErrorAdd_UnexpectedElementSpecialValue("Skill", "movetype", "arc, drop, throw, circle, swing", value.Text);
            }
        }

        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            Validate(ref result, ref pair.Value.Value);
        }

        if (pair.VariantArray is not null)
        {
            foreach (var item in pair.VariantArray)
            {
                if (item is { HasValue: true, Value.HasText: true })
                {
                    Validate(ref result, ref item.Value);
                }
            }
        }
	}
}
