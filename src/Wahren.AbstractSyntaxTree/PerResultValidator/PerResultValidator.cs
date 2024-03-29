﻿namespace Wahren.AbstractSyntaxTree.Parser;

using Element;

public static partial class PerResultValidator
{
    private static void Collect<T>(ref Result result, ref StringSpanKeySlowSet set, ref VariantPair<T> pair)
        where T : class, IElement
    {
        foreach (ref T variant in pair.Variants)
        {
            set.GetOrAdd(result.GetSpan(variant.ElementTokenId).Slice(variant.ElementKeyLength + 1), variant.ElementTokenId);
        }
    }

    private static void CollectNumber<T>(ref Result result, ref StringSpanKeySlowSet set, ref VariantPair<T> pair)
        where T : class, IElement
    {
        foreach (ref T variant in pair.Variants)
        {
            var variantSpan = result.GetSpan(variant.ElementTokenId).Slice(variant.ElementKeyLength + 1);
            if (!int.TryParse(variantSpan, out _))
            {
                set.GetOrAdd(variantSpan, variant.ElementTokenId);
            }
        }
    }

    private static void CollectSpecial_sub_image<T>(ref Result result, ref StringSpanKeySlowSet set, ref VariantPair<T> pair)
        where T : class, IElement
    {
        foreach (ref T variant in pair.Variants)
        {
            var rest = result.GetSpan(variant.ElementTokenId).Slice(variant.ElementKeyLength + 1);
            if (rest.IsEmpty)
            {
                continue;
            }

            if (rest[0] == '@')
            {
                var atmarkIndex = rest.Slice(1).IndexOf('@');
                if (atmarkIndex == -1)
                {
                    if (!int.TryParse(rest.Slice(1), out _))
                    {
                        result.ErrorAdd_sub_image_InvalidNumber(variant.ElementTokenId);
                    }
                }
                else if (atmarkIndex == 0)
                {
                    result.ErrorAdd_sub_image_Invalid(variant.ElementTokenId);
                }
                else if (int.TryParse(rest.Slice(1, atmarkIndex), out _))
                {
                    set.GetOrAdd(rest.Slice(2 + atmarkIndex), variant.ElementTokenId);
                }
                else
                {
                    result.ErrorAdd_sub_image_InvalidNumber(variant.ElementTokenId);
                }
            }
            else
            {
                var atmarkIndex = rest.IndexOf("@@");
                if (atmarkIndex == -1)
                {
                    set.GetOrAdd(rest, variant.ElementTokenId);
                }
                else
                {
                    result.ErrorAdd_sub_image_InvalidOrder(variant.ElementTokenId);
                }
            }
        }
    }

    public static void AddReference(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ref StringSpanKeySlowSet set, ReferenceKind kind)
    {
        if (pair.Value is not null && pair.Value.HasValue)
        {
            ref var value = ref pair.Value.Value;
            value.ReferenceKind = kind;
            value.ReferenceId = set.GetOrAdd(result.GetSpan(value.Text), value.Text);
            value.HasReference = true;
        }

        foreach (var item in pair.Variants)
        {
            if (!item.HasValue)
            {
                continue;
            }

            ref var value = ref item.Value;
            value.ReferenceKind = kind;
            var span = result.GetSpan(value.Text);
            if (span.Length != 1 || span[0] != '@')
            {
                value.ReferenceId = set.GetOrAdd(span, value.Text);
            }
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
                var span = result.GetSpan(value.Text);
                if (span.Length != 1 || span[0] != '@')
                {
                    value.ReferenceId = set.GetOrAdd(span, value.Text);
                }
                value.HasReference = true;
            }
        }

        foreach (var item in pair.Variants)
        {
            if (!item.HasValue)
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
                var span = result.GetSpan(value.Text);
                if (span.Length != 1 || span[0] != '@')
                {
                    value.ReferenceId = set.GetOrAdd(span, value.Text);
                }
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
                var span = result.GetSpan(value.Text);
                if (span.Length != 1 || span[0] != '@')
                {
                    value.ReferenceId = set.GetOrAdd(span, value.Text);
                }
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
        var span = result.GetSpan(v.Text);
        if (span.Length != 1 || span[0] != '@')
        {
            v.ReferenceId = set.GetOrAdd(span, v.Text);
        }
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
            var span = result.GetSpan(v.Text);
            if (span.Length != 1 || span[0] != '@')
            {
                v.ReferenceId = set.GetOrAdd(span, v.Text);
            }
        }
    }

    public static void AddReferenceSkillBoolean(ref Result result, AnalysisResult analysisResult, ref Pair_NullableString_NullableIntElement? value, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName)
    {
        if (value is not { HasValue: true })
        {
            return;
        }

        ref var v = ref value.Value;
        if (v.HasNumber)
        {
            result.ErrorAdd_UnexpectedElementReferenceKind(nodeKind, elementName, "Skill, Boolean", v.Text);
            return;
        }

        var span = result.GetSpan(v.Text);
        v.HasReference = true;
        if (IsBoolean(span, out v.ReferenceId))
        {
            v.ReferenceKind = ReferenceKind.Boolean;
        }
        else
        {
            v.ReferenceKind = ReferenceKind.Skill;
            if (span.Length != 1 || span[0] != '@')
            {
                v.ReferenceId = analysisResult.SkillSet.GetOrAdd(span, v.Text);
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
        if (pair.Value is not null)
        {
            ValidateNumber(ref result, ref pair.Value, nodeKind, elementName);
        }

        foreach (ref var item in pair.Variants)
        {
            ValidateNumber(ref result, ref item!, nodeKind, elementName);
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

        foreach (var item in pair.Variants)
        {
            if (!item.HasValue)
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

        foreach (var item in pair.Variants)
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

        foreach (var item in pair.Variants)
        {
            if (!item.HasValue)
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

        foreach (var item in pair.Variants)
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

        foreach (var item in pair.Variants)
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

        foreach (var item in pair.Variants)
        {
            if (!item.HasValue)
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

        foreach (var item in pair.Variants)
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

    internal static bool IsAbnormalAttribute(ReadOnlySpan<char> span, out uint referenceId)
    {
        if (span.SequenceEqual("poi"))
        {
            referenceId = 0;
            return true;
        }
        else if (span.SequenceEqual("para"))
        {
            referenceId = 1;
            return true;
        }
        else if (span.SequenceEqual("ill"))
        {
            referenceId = 2;
            return true;
        }
        else if (span.SequenceEqual("conf"))
        {
            referenceId = 3;
            return true;
        }
        else if (span.SequenceEqual("sil"))
        {
            referenceId = 4;
            return true;
        }
        else if (span.SequenceEqual("stone"))
        {
            referenceId = 5;
            return true;
        }
        else if (span.SequenceEqual("fear"))
        {
            referenceId = 6;
            return true;
        }
        else
        {
            referenceId = 0;
            return false;
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

        foreach (var item in pair.Variants)
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
}
