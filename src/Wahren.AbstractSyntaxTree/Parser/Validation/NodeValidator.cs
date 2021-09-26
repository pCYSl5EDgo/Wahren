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

    private static bool ValidateBooleanNumber(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> postText)
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

    private static bool SpecialTreatment_unit_sex(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_unit_class_sex(ref result, ref pair, "unit", severity);
    private static bool SpecialTreatment_class_sex(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_unit_class_sex(ref result, ref pair, "class", severity);
    private static bool SpecialTreatment_unit_class_sex(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> kind, DiagnosticSeverity severity)
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

    private static bool SpecialTreatment_class_yorozu(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> yorozu, DiagnosticSeverity severity) => SpecialTreatment_unit_class_yorozu(ref result, ref yorozu, "class", severity);
    private static bool SpecialTreatment_unit_yorozu(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> yorozu, DiagnosticSeverity severity) => SpecialTreatment_unit_class_yorozu(ref result, ref yorozu, "unit", severity);
    private static bool SpecialTreatment_unit_class_yorozu(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, ReadOnlySpan<char> kind, DiagnosticSeverity severity)
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


    private static bool SpecialTreatment_event_handle(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
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

    private static bool SpecialTreatment_unit_arbeit(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
            if (span.SequenceEqual("on"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 0;
            }
            else if (span.SequenceEqual("power"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 1;
            }
            else if (span.SequenceEqual("fix"))
            {
                value.HasReference = true;
                value.ReferenceKind = ReferenceKind.Special;
                value.ReferenceId = 2;
            }
            else
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

    private static bool SpecialTreatment_unit_fix(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
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

    private static bool SpecialTreatment_unit_add_vassal(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_add_vassal(ref result, ref pair, "unit");
    private static bool SpecialTreatment_class_add_vassal(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_add_vassal(ref result, ref pair, "class");
    private static bool SpecialTreatment_add_vassal(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> kind)
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

    private static bool SpecialTreatment_unit_multi(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, DiagnosticSeverity severity) => SpecialTreatment_multi(ref result, ref pair, severity, "unit");
    private static bool SpecialTreatment_class_multi(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, DiagnosticSeverity severity) => SpecialTreatment_multi(ref result, ref pair, severity, "class");
    private static bool SpecialTreatment_multi(ref Result result, ref VariantPair<Pair_NullableString_NullableInt_ArrayElement> pair, DiagnosticSeverity severity, ReadOnlySpan<char> kind)
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
                switch (text.Length)
                {
                    case 2 when text[1] == 'p':
                        if (text[0] == 'h')
                        {
                            const int id = 0;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'hp'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                            break;
                        }
                        else if (text[0] == 'm')
                        {
                            const int id = 1;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'hp'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                            break;
                        }
                        else
                        {
                            goto default;
                        }
                    case 4 when text.SequenceEqual("dext"):
                        {
                            const int id = 2;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'dext'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                        }
                        break;
                    case 4 when text.SequenceEqual("move"):
                        {
                            const int id = 3;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'move'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                        }
                        break;
                    case 5 when text.SequenceEqual("magic"):
                        {
                            const int id = 4;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'magic'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                        }
                        break;
                    case 5 when text.SequenceEqual("speed"):
                        {
                            const int id = 5;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'speed'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                        }
                        break;
                    case 5 when text.SequenceEqual("hprec"):
                        {
                            const int id = 6;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'hprec'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                        }
                        break;
                    case 5 when text.SequenceEqual("mprec"):
                        {
                            const int id = 7;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'mprec'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                        }
                        break;
                    case 6 when text.SequenceEqual("attack"):
                        {
                            const int id = 8;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'attack'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                        }
                        break;
                    case 6 when text.SequenceEqual("magdef"):
                        {
                            const int id = 9;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'magdef'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                        }
                        break;
                    case 7 when text.SequenceEqual("defense"):
                        {
                            const int id = 10;
                            if (statuses[id] != -1)
                            {
                                result.ErrorList.Add(new($"'multi' of struct {kind} already have 'defense'.", result.TokenList[item.Text].Range));
                                success = false;
                            }
                            statuses[id] = i;
                        }
                        break;
                    default:
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

    private static bool SpecialTreatment_unit_politics(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_politics(ref result, ref pair, "unit");
    private static bool SpecialTreatment_class_politics(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_politics(ref result, ref pair, "class");
    private static bool SpecialTreatment_politics(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, ReadOnlySpan<char> kind)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value, ReadOnlySpan<char> kind)
        {
            var span = result.GetSpan(value.Text);
            switch (span.Length)
            {
                case 2 when span.SequenceEqual("on"):
                    value.ReferenceId = 0;
                    break;
                case 3 when span.SequenceEqual("fix"):
                    value.ReferenceId = 1;
                    break;
                case 5 when span.SequenceEqual("erase"):
                    value.ReferenceId = 2;
                    break;
                case 6 when span.SequenceEqual("unique"):
                    value.ReferenceId = 3;
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

    private static bool SpecialTreatment_unit_line(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_line(ref result, ref pair, severity, "unit");
    private static bool SpecialTreatment_class_line(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_line(ref result, ref pair, severity, "class");
    private static bool SpecialTreatment_line(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity, ReadOnlySpan<char> kind)
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

    private static bool SpecialTreatment_unit_picture_floor(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_picture_floor(ref result, ref pair, severity, "unit");
    private static bool SpecialTreatment_class_picture_floor(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity) => SpecialTreatment_picture_floor(ref result, ref pair, severity, "class");
    private static bool SpecialTreatment_picture_floor(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity, ReadOnlySpan<char> kind)
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

    private static bool SpecialTreatment_spot_politics(ref Result result, ref VariantPair<Pair_NullableString_NullableIntElement> pair, DiagnosticSeverity severity)
    {
        static bool Validate(ref Result result, ref Pair_NullableString_NullableInt value)
        {
            var span = result.GetSpan(value.Text);
            if (!span.SequenceEqual("on"))
            {
                result.ErrorList.Add(new($"'politics' of struct spot must be 'top', 'msg', 'base' or 'bottom'.", result.TokenList[value.Text].Range));
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
