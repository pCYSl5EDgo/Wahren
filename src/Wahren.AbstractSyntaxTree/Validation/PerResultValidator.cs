namespace Wahren.AbstractSyntaxTree.Parser;

using Element;
using Statement;
using Statement.Expression;

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
        if (pair is { Value.HasValue: true })
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
            if (item is not { HasValue: true, Value.HasNumber: false })
            {
                continue;
            }

            ref var value = ref item.Value;
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

    internal static void AddReference(ref Result result, ref VariantPair<StringArrayElement> pair, ref StringSpanKeySlowSet set, ReferenceKind kind)
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

    internal static bool ValidateBooleanNumber(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> postText)
    {
        bool success = true;
        if (pair is { Value.HasValue: true })
        {
            ref var value = ref pair.Value.Value;
            if (!value.HasNumber)
            {
                var span = result.GetSpan(value.Text);
                if (!span.SequenceEqual("on") && !span.SequenceEqual("off"))
                {
                    result.ErrorList.Add(new($"Boolean text or Number text is expected but actually \"{result.GetSpan(value.Text)}\".{postText}", result.TokenList[value.Text].Range));
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

            result.ErrorList.Add(new($"Boolean text or Number text is expected but actually \"{result.GetSpan(value.Text)}\".{postText}", result.TokenList[value.Text].Range));
            success = false;
        }

        return success;
    }

    internal static bool IsStatus(ReadOnlySpan<char> span, out uint referenceId)
    {
        // hp 0
        // mp 1
        // dext 7
        // move 8
        // magic 4
        // speed 6
        // hprec 9
        // mprec a
        // attack 2
        // magdef 5
        // defense 3
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

    internal static bool SpecialTreatment_unit_sex(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_unit_class_sex(ref result, ref pair, "unit", severity);
    internal static bool SpecialTreatment_class_sex(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_unit_class_sex(ref result, ref pair, "class", severity);
    internal static bool SpecialTreatment_unit_class_sex(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> kind, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value, ReadOnlySpan<char> kind, DiagnosticSeverity severity)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("male"))
            {
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("female"))
            {
                value.ReferenceId = 2;
            }
            else
            {
                value.ReferenceId = 0;
                if (DiagnosticSeverity.Warning <= severity && !span.IsEmpty && !span.SequenceEqual("neuter"))
                {
                    result.ErrorList.Add(new($"'sex' of struct {kind} must be 'neuter', 'male' or 'female'.", result.TokenList[value.Text].Range, DiagnosticSeverity.Warning));
                }
            }

            value.HasReference = true;
            value.ReferenceKind = ReferenceKind.Special;
            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value, kind, severity);
        }

        if (pair.ScenarioVariant is null || pair.ScenarioVariant.Length == 0)
        {
            return success;
        }

        foreach (ref var item in pair.ScenarioVariant.AsSpan())
        {
            if (item is { HasValue: true, Value.HasText: true })
            {
                success &= Validate(ref result, ref item.Value, kind, severity);
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_class_yorozu(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> yorozu, DiagnosticSeverity severity) => SpecialTreatment_unit_class_yorozu(ref result, ref yorozu, "class", severity);
    internal static bool SpecialTreatment_unit_yorozu(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> yorozu, DiagnosticSeverity severity) => SpecialTreatment_unit_class_yorozu(ref result, ref yorozu, "unit", severity);
    internal static bool SpecialTreatment_unit_class_yorozu(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, ReadOnlySpan<char> kind, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, Span<Pair_NullableString_NullableInt> list, ReadOnlySpan<char> kind, DiagnosticSeverity severity)
        {
            if (list.Length > 4 && DiagnosticSeverity.Warning <= severity)
            {
                result.ErrorList.Add(new($"'yorozu' of struct {kind} can have up to 4 kind of values ('keep_direct', 'no_circle', 'base', or 'keep_color').", result.TokenList[list[4].Text].Range, DiagnosticSeverity.Warning));
            }

            int keep_direct = -1;
            int keep_color = -1;
            int no_circle = -1;
            int @base = -1;

            bool success = true;
            for (int i = 0; i < list.Length; i++)
            {
                ref var item = ref list[i];
                if (!item.HasText)
                {
                    continue;
                }

                var text = result.GetSpan(item.Text);
                switch (text.Length)
                {
                    case 4 when text.SequenceEqual("base"):
                        if (@base != -1)
                        {
                            result.ErrorList.Add(new($"'yorozu' of struct {kind} already have 'base'.", result.TokenList[item.Text].Range));
                            success = false;
                        }
                        @base = i;
                        break;
                    case 9 when text.SequenceEqual(nameof(no_circle)):
                        if (no_circle != -1)
                        {
                            result.ErrorList.Add(new($"'yorozu' of struct {kind} already have 'no_circle'.", result.TokenList[item.Text].Range));
                            success = false;
                        }
                        no_circle = i;
                        break;
                    case 10 when text.SequenceEqual(nameof(keep_color)):
                        if (keep_color != -1)
                        {
                            result.ErrorList.Add(new($"'yorozu' of struct {kind} already have 'keep_color'.", result.TokenList[item.Text].Range));
                            success = false;
                        }
                        keep_color = i;
                        break;
                    case 11 when text.SequenceEqual(nameof(keep_direct)):
                        if (keep_direct != -1)
                        {
                            result.ErrorList.Add(new($"'yorozu' of struct {kind} already have 'keep_direct'.", result.TokenList[item.Text].Range));
                            success = false;
                        }
                        keep_direct = i;
                        break;
                    default:
                        result.ErrorList.Add(new($"'{text}' is not valid value. 'yorozu' of struct {kind} can have up to 4 kind of values ('keep_direct', 'no_circle', 'base', or 'keep_color').", result.TokenList[item.Text].Range));
                        success = false;
                        break;
                }
            }

            return success;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.IsEmpty: false })
        {
            success &= Validate(ref result, pair.Value.Value.AsSpan(), kind, severity);
        }

        if (pair.ScenarioVariant is not null)
        {
            foreach (var item in pair.ScenarioVariant)
            {
                if (item is { HasValue: true, Value.IsEmpty: false })
                {
                    success &= Validate(ref result, item.Value.AsSpan(), kind, severity);
                }
            }
        }

        return success;
    }


    internal static bool SpecialTreatment_event_handle(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("red"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 0;
            }
            else if (span.SequenceEqual("blue"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 1;
            }
            else
            {
                value.HasReference = false;
                result.ErrorList.Add(new("'handle' of struct event must be 'red' or 'blue'.", result.TokenList[value.Text].Range));
                return false;
            }

            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value);
        }

        if (pair.ScenarioVariant is not null)
        {
            foreach (var item in pair.ScenarioVariant)
            {
                if (item is { HasValue: true, Value.HasText: true })
                {
                    success &= Validate(ref result, ref item.Value);
                }
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_unit_arbeit(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("on"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("power"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 2;
            }
            else if (span.SequenceEqual("fix"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 3;
            }
            else if (span.SequenceEqual("off"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 0;
            }
            else if (!value.HasNumber)
            {
                value.HasReference = false;
                result.ErrorList.Add(new("'arbeit' of struct unit must be 'on', 'power' or 'fix'.", result.TokenList[value.Text].Range));
                return false;
            }

            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value);
        }

        if (pair.ScenarioVariant is not null)
        {
            foreach (var item in pair.ScenarioVariant)
            {
                if (item is { HasValue: true, Value.HasText: true })
                {
                    success &= Validate(ref result, ref item.Value);
                }
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_power_fix(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("off"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 0;
            }
            else if (span.SequenceEqual("on"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("home"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 2;
            }
            else if (span.SequenceEqual("hold"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 3;
            }
            else if (span.SequenceEqual("warlike"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 4;
            }
            else if (span.SequenceEqual("freeze"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 5;
            }
            else
            {
                value.HasReference = false;
                result.ErrorList.Add(new("'fix' of struct power must be 'off', 'on', 'home', 'hold', 'warlike', 'freeze'.", result.TokenList[value.Text].Range));
                return false;
            }

            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value);
        }

        if (pair.ScenarioVariant is not null)
        {
            foreach (var item in pair.ScenarioVariant)
            {
                if (item is { HasValue: true, Value.HasText: true })
                {
                    success &= Validate(ref result, ref item.Value);
                }
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_unit_fix(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("off"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 0;
            }
            else if (span.SequenceEqual("on"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("home"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 2;
            }
            else
            {
                value.HasReference = false;
                result.ErrorList.Add(new("'fix' of struct unit must be 'off', 'on' or 'home'.", result.TokenList[value.Text].Range));
                return false;
            }

            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value);
        }

        if (pair.ScenarioVariant is not null)
        {
            foreach (var item in pair.ScenarioVariant)
            {
                if (item is { HasValue: true, Value.HasText: true })
                {
                    success &= Validate(ref result, ref item.Value);
                }
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_scenario_power_order(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("normal"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 0;
            }
            else if (span.SequenceEqual("test"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("dash"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 2;
            }
            else
            {
                value.HasReference = false;
                result.ErrorList.Add(new("'power_order' of struct scenario must be 'normal', 'test' or 'dash'.", result.TokenList[value.Text].Range));
                return false;
            }

            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value);
        }

        if (pair.ScenarioVariant is not null)
        {
            foreach (var item in pair.ScenarioVariant)
            {
                if (item is { HasValue: true, Value.HasText: true })
                {
                    success &= Validate(ref result, ref item.Value);
                }
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_unit_active(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_active(ref result, ref pair, "unit");
    internal static bool SpecialTreatment_class_active(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_active(ref result, ref pair, "class");
    internal static bool SpecialTreatment_active(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> kind)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value, ReadOnlySpan<char> kind)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("never"))
            {
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("rect"))
            {
                value.ReferenceId = 2;
            }
            else if (span.SequenceEqual("range"))
            {
                value.ReferenceId = 3;
            }
            else if (span.SequenceEqual("time"))
            {
                value.ReferenceId = 4;
            }
            else if (span.SequenceEqual("troop"))
            {
                value.ReferenceId = 5;
            }
            else if (span.SequenceEqual("never2"))
            {
                value.ReferenceId = 6;
            }
            else if (span.SequenceEqual("rect2"))
            {
                value.ReferenceId = 7;
            }
            else if (span.SequenceEqual("range2"))
            {
                value.ReferenceId = 8;
            }
            else if (span.SequenceEqual("time2"))
            {
                value.ReferenceId = 9;
            }
            else if (span.SequenceEqual("troop2"))
            {
                value.ReferenceId = 10;
            }
            else
            {
                value.ReferenceId = 0;
                result.ErrorList.Add(new($"'active' of struct {kind} must be 'never', 'rect', 'range', 'time', 'troop', 'never2', 'rect2', 'range2', 'time2', or 'troop2'.", result.TokenList[value.Text].Range));
            }

            value.HasReference = true;
            value.ReferenceKind = ReferenceKind.Special;
            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value, kind);
        }

        if (pair.ScenarioVariant is null || pair.ScenarioVariant.Length == 0)
        {
            return success;
        }

        foreach (ref var item in pair.ScenarioVariant.AsSpan())
        {
            if (item is { HasValue: true, Value.HasText: true })
            {
                success &= Validate(ref result, ref item.Value, kind);
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_unit_picture_detail(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_picture_detail(ref result, ref pair, "unit");
    internal static bool SpecialTreatment_class_picture_detail(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_picture_detail(ref result, ref pair, "class");
    internal static bool SpecialTreatment_picture_detail(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> kind)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value, ReadOnlySpan<char> kind)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("on"))
            {
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("off"))
            {
                value.ReferenceId = 0;
            }
            else if (span.SequenceEqual("on1"))
            {
                value.ReferenceId = 2;
            }
            else if (span.SequenceEqual("on2"))
            {
                value.ReferenceId = 3;
            }
            else if (span.SequenceEqual("on3"))
            {
                value.ReferenceId = 4;
            }
            else
            {
                value.ReferenceId = 0;
                result.ErrorList.Add(new($"'picture_detail' of struct {kind} must be 'off', 'on', 'on1', 'on2', or 'on3'.", result.TokenList[value.Text].Range));
            }

            value.HasReference = true;
            value.ReferenceKind = ReferenceKind.Special;
            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value, kind);
        }

        if (pair.ScenarioVariant is null || pair.ScenarioVariant.Length == 0)
        {
            return success;
        }

        foreach (ref var item in pair.ScenarioVariant.AsSpan())
        {
            if (item is { HasValue: true, Value.HasText: true })
            {
                success &= Validate(ref result, ref item.Value, kind);
            }
        }

        return success;
    }


    internal static bool SpecialTreatment_unit_add_vassal(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_add_vassal(ref result, ref pair, "unit");
    internal static bool SpecialTreatment_class_add_vassal(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_add_vassal(ref result, ref pair, "class");
    internal static bool SpecialTreatment_add_vassal(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> kind)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value, ReadOnlySpan<char> kind)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("on"))
            {
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("roam"))
            {
                value.ReferenceId = 2;
            }
            else if (span.SequenceEqual("off"))
            {
                value.ReferenceId = 0;
            }
            else
            {
                value.ReferenceId = 0;
                result.ErrorList.Add(new($"'add_vassal' of struct {kind} must be 'off', 'on' or 'roam'.", result.TokenList[value.Text].Range));
            }

            value.HasReference = true;
            value.ReferenceKind = ReferenceKind.Special;
            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value, kind);
        }

        if (pair.ScenarioVariant is null || pair.ScenarioVariant.Length == 0)
        {
            return success;
        }

        foreach (ref var item in pair.ScenarioVariant.AsSpan())
        {
            if (item is { HasValue: true, Value.HasText: true })
            {
                success &= Validate(ref result, ref item.Value, kind);
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_unit_multi(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, DiagnosticSeverity severity) => SpecialTreatment_multi(ref result, ref pair, severity, "unit");
    internal static bool SpecialTreatment_class_multi(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, DiagnosticSeverity severity) => SpecialTreatment_multi(ref result, ref pair, severity, "class");
    internal static bool SpecialTreatment_multi(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, DiagnosticSeverity severity, ReadOnlySpan<char> kind)
    {
        static bool Validate(ref Result result, Span<Pair_NullableString_NullableInt> list, ReadOnlySpan<char> kind, DiagnosticSeverity severity)
        {
            if (list.Length > 11 && DiagnosticSeverity.Warning <= severity)
            {
                result.ErrorList.Add(new($"'multi' of struct {kind} can have up to 11 kind of values ('hp', 'mp', 'attack', 'defense', 'magic', 'magdef', 'speed', 'dext', 'move', 'hprec', 'mprec').", result.TokenList[list[4].Text].Range, DiagnosticSeverity.Warning));
            }

            Span<int> statuses = stackalloc int[11];
            statuses.Fill(-1);

            bool success = true;
            for (int i = 0; i < list.Length; i++)
            {
                ref var item = ref list[i];
                if (!item.HasText)
                {
                    continue;
                }

                var text = result.GetSpan(item.Text);
                if (IsStatus(text, out uint index))
                {
                    if (statuses[(int)index] == -1)
                    {
                        statuses[(int)index] = i;
                    }
                    else
                    {
                        result.ErrorList.Add(new($"'multi' of struct {kind} already have '{text}'.", result.TokenList[item.Text].Range));
                        success = false;
                        break;
                    }
                }
                else
                {
                    result.ErrorList.Add(new($"'{text}' is not valid value. 'multi' of struct {kind} must be one of the 11 status text('hp', 'mp', 'attack', 'defense', 'magic', 'magdef', 'speed', 'dext', 'move', 'hprec', 'mprec').", result.TokenList[item.Text].Range));
                    success = false;
                    break;
                }
            }

            return success;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.IsEmpty: false })
        {
            success &= Validate(ref result, pair.Value.Value.AsSpan(), kind, severity);
        }

        if (pair.ScenarioVariant is not null)
        {
            foreach (var item in pair.ScenarioVariant)
            {
                if (item is { HasValue: true, Value.IsEmpty: false })
                {
                    success &= Validate(ref result, item.Value.AsSpan(), kind, severity);
                }
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_unit_politics(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_politics(ref result, ref pair, "unit");
    internal static bool SpecialTreatment_class_politics(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_politics(ref result, ref pair, "class");
    internal static bool SpecialTreatment_politics(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> kind)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value, ReadOnlySpan<char> kind)
        {
            var span = result.GetSpan(value.Text);
            switch (span.Length)
            {
                case 2 when span.SequenceEqual("on"):
                    value.ReferenceId = 1;
                    break;
                case 3 when span.SequenceEqual("fix"):
                    value.ReferenceId = 2;
                    break;
                case 5 when span.SequenceEqual("erase"):
                    value.ReferenceId = 3;
                    break;
                case 6 when span.SequenceEqual("unique"):
                    value.ReferenceId = 4;
                    break;
                default:
                    result.ErrorList.Add(new($"'politics' of struct {kind} must be 'on', 'fix', 'erase' or 'unique'.", result.TokenList[value.Text].Range));
                    return false;
            }

            value.HasReference = true;
            value.ReferenceKind = ReferenceKind.Special;
            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value, kind);
        }

        if (pair.ScenarioVariant is null || pair.ScenarioVariant.Length == 0)
        {
            return success;
        }

        foreach (ref var item in pair.ScenarioVariant.AsSpan())
        {
            if (item is { HasValue: true, Value.HasText: true })
            {
                success &= Validate(ref result, ref item.Value, kind);
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_unit_line(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_line(ref result, ref pair, severity, "unit");
    internal static bool SpecialTreatment_class_line(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_line(ref result, ref pair, severity, "class");
    internal static bool SpecialTreatment_line(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity, ReadOnlySpan<char> kind)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value, ReadOnlySpan<char> kind)
        {
            var span = result.GetSpan(value.Text);
            switch (span.Length)
            {
                case 4 when span.SequenceEqual("back"):
                    value.ReferenceId = 2;
                    break;
                case 5 when span.SequenceEqual("front"):
                    value.ReferenceId = 1;
                    break;
                default:
                    value.ReferenceId = 0;
                    result.ErrorList.Add(new($"'line' of struct {kind} must be 'front', or 'back'.", result.TokenList[value.Text].Range));
                    return false;
            }

            value.HasReference = true;
            value.ReferenceKind = ReferenceKind.Special;
            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true })
        {
            success &= Validate(ref result, ref pair.Value.Value, kind);
        }

        if (pair.ScenarioVariant is null || pair.ScenarioVariant.Length == 0)
        {
            return success;
        }

        foreach (ref var item in pair.ScenarioVariant.AsSpan())
        {
            if (item is { HasValue: true, Value.HasText: true })
            {
                success &= Validate(ref result, ref item.Value, kind);
            }
        }

        return success;
    }

    internal static bool SpecialTreatment_unit_picture_floor(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_picture_floor(ref result, ref pair, severity, "unit");
    internal static bool SpecialTreatment_class_picture_floor(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_picture_floor(ref result, ref pair, severity, "class");
    internal static bool SpecialTreatment_picture_floor(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity, ReadOnlySpan<char> kind)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value, ReadOnlySpan<char> kind)
        {
            var span = result.GetSpan(value.Text);
            switch (span.Length)
            {
                case 3 when span.SequenceEqual("top"):
                    value.ReferenceId = 0;
                    break;
                case 3 when span.SequenceEqual("msg"):
                    value.ReferenceId = 1;
                    break;
                case 4 when span.SequenceEqual("base"):
                    value.ReferenceId = 3;
                    break;
                case 6 when span.SequenceEqual("bottom"):
                    value.ReferenceId = 2;
                    break;
                default:
                    result.ErrorList.Add(new($"'picture_floor' of struct {kind} must be 'top', 'msg', 'base' or 'bottom'.", result.TokenList[value.Text].Range));
                    return false;
            }

            value.HasReference = true;
            value.ReferenceKind = ReferenceKind.Special;
            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true, Value.HasNumber: false })
        {
            success &= Validate(ref result, ref pair.Value.Value, kind);
        }

        if (pair.ScenarioVariant is null || pair.ScenarioVariant.Length == 0)
        {
            return success;
        }

        foreach (ref var item in pair.ScenarioVariant.AsSpan())
        {
            if (item is { HasValue: true, Value.HasText: true, Value.HasNumber: false })
            {
                success &= Validate(ref result, ref item.Value, kind);
            }
        }

        return success;
    }
    
    internal static bool SpecialTreatment_spot_politics(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
            if (!span.SequenceEqual("on"))
            {
                result.ErrorList.Add(new($"'politics' of struct spot must be 'on'.", result.TokenList[value.Text].Range));
                return false;
            }

            value.HasReference = true;
            value.ReferenceKind = ReferenceKind.Special;
            return true;
        }

        bool success = true;
        if (pair.Value is { HasValue: true, Value.HasText: true, Value.HasNumber: false })
        {
            success &= Validate(ref result, ref pair.Value.Value);
        }

        if (pair.ScenarioVariant is null || pair.ScenarioVariant.Length == 0)
        {
            return success;
        }

        foreach (ref var item in pair.ScenarioVariant.AsSpan())
        {
            if (item is { HasValue: true, Value.HasText: true, Value.HasNumber: false })
            {
                success &= Validate(ref result, ref item.Value);
            }
        }

        return success;
    }
}
