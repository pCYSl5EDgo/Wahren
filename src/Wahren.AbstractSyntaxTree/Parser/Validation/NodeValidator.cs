namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class NodeValidator
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

        if (pair.ScenarioVariant is null)
        {
            return;
        }

        foreach (var item in pair.ScenarioVariant)
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

        if (pair.ScenarioVariant is null)
        {
            return;
        }

        foreach (var item in pair.ScenarioVariant)
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

    public static bool ValidateNumber(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> postText)
    {
        bool success = true;
        if (pair.Value is not null && pair.Value.HasValue)
        {
            ref var value = ref pair.Value.Value;
            if (!value.HasNumber)
            {
                result.ErrorAdd_NumberIsExpected(value.Text, postText);
                success = false;
            }
        }

        if (pair.ScenarioVariant is null)
        {
            return success;
        }

        foreach (var item in pair.ScenarioVariant)
        {
            if (item is null || !item.HasValue)
            {
                continue;
            }

            ref var value = ref item.Value;
            if (value.HasNumber)
            {
                continue;
            }

            result.ErrorAdd_NumberIsExpected(value.Text, postText);
            success = false;
        }

        return success;
    }

    public static bool ValidateNumber(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, ReadOnlySpan<char> postText)
    {
        bool success = true;
        if (pair.Value is not null && pair.Value.HasValue)
        {
            foreach (ref var value in pair.Value.Value)
            {
                if (value.HasNumber)
                {
                    continue;
                }

                result.ErrorAdd_NumberIsExpected(value.Text, postText);
                success = false;
            }
        }

        if (pair.ScenarioVariant is null)
        {
            return success;
        }

        foreach (var item in pair.ScenarioVariant)
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

                result.ErrorAdd_NumberIsExpected(value.Text, postText);
                success = false;
            }
        }

        return success;
    }

    public static bool ValidateBoolean(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> postText)
    {
        bool success = true;
        if (pair.Value is not null && pair.Value.HasValue)
        {
            ref var value = ref pair.Value.Value;
            if (success = !value.HasNumber)
            {
                var span = result.GetSpan(value.Text);
                if (!span.SequenceEqual("on") && !span.SequenceEqual("off"))
                {
                    result.ErrorAdd_BooleanIsExpected(value.Text, postText);
                    success = false;
                }
            }
        }

        if (pair.ScenarioVariant is null)
        {
            return success;
        }

        foreach (var item in pair.ScenarioVariant)
        {
            if (item is null || !item.HasValue)
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

            result.ErrorAdd_BooleanIsExpected(value.Text, postText);
            success = false;
        }

        return success;
    }

    public static bool ValidateBoolean(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, ReadOnlySpan<char> postText)
    {
        bool success = true;
        if (pair.Value is not null && pair.Value.HasValue)
        {
            foreach (ref var value in pair.Value.Value)
            {
                if (!value.HasNumber)
                {
                    var span = result.GetSpan(value.Text);
                    if (span.SequenceEqual("on") || span.SequenceEqual("off"))
                    {
                        continue;
                    }
                }

                result.ErrorAdd_BooleanIsExpected(value.Text, postText);
                success = false;
                break;
            }
        }

        if (pair.ScenarioVariant is null)
        {
            return success;
        }

        foreach (var item in pair.ScenarioVariant)
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
                    if (span.SequenceEqual("on") || span.SequenceEqual("off"))
                    {
                        continue;
                    }
                }

                result.ErrorAdd_BooleanIsExpected(value.Text, postText);
                success = false;
                break;
            }
        }

        return success;
    }

    private static void AddReference(ref Result result, ref VariantPair<StringArrayElement> pair, ref StringSpanKeySlowSet set, ReferenceKind kind)
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

        if (pair.ScenarioVariant is null)
        {
            return;
        }

        foreach (var item in pair.ScenarioVariant)
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
}
