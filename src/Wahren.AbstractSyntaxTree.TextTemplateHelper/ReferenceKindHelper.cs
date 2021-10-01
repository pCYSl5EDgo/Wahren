using System;
using System.Text;

namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public static class ReferenceKindHelper
{
    public static bool CanProcessEarly(this ReferenceKind[][] referencesArray)
    {
        foreach (var references in referencesArray)
        {
            if (!references.CanProcessEarly())
            {
                return false;
            }
        }

        return true;
    }

    public static bool CanProcessEarly(this ReferenceKind[] references)
    {
        foreach (var reference in references)
        {
            if (!reference.CanProcessEarly())
            {
                return false;
            }
        }

        return true;
    }

    public static bool CanProcessEarly(this ReferenceKind reference)
    {
        return reference switch
        {
            ReferenceKind.Unknown or
            ReferenceKind.Special or
            ReferenceKind.Scenario or
            ReferenceKind.Event or
            ReferenceKind.Story or
            ReferenceKind.Movetype or
            ReferenceKind.Skill or
            ReferenceKind.Skillset or
            ReferenceKind.Race or
            ReferenceKind.Unit or
            ReferenceKind.Class or
            ReferenceKind.Power or
            ReferenceKind.Spot or
            ReferenceKind.Field or
            ReferenceKind.Object or
            ReferenceKind.Dungeon or
            ReferenceKind.Voice or
            ReferenceKind.AttributeType or
            ReferenceKind.VoiceTypeReader or
            ReferenceKind.VoiceTypeWriter or
            ReferenceKind.NumberVariableReader or
            ReferenceKind.NumberVariableWriter or
            ReferenceKind.GlobalVariableReader or
            ReferenceKind.GlobalVariableWriter or
            ReferenceKind.FieldAttributeTypeReader or
            ReferenceKind.FieldAttributeTypeWriter or
            ReferenceKind.FieldIdReader or
            ReferenceKind.FieldIdWriter or
            ReferenceKind.ClassTypeReader or
            ReferenceKind.ClassTypeWriter or
            ReferenceKind.GlobalStringVariableReader or
            ReferenceKind.GlobalStringVariableWriter or
            ReferenceKind.map or
            ReferenceKind.bgm or
            ReferenceKind.imagedata or
            ReferenceKind.face or
            ReferenceKind.se or
            ReferenceKind.picture or
            ReferenceKind.image_file or
            ReferenceKind.flag or
            ReferenceKind.font or
            ReferenceKind.Number or
            ReferenceKind.Boolean or
            ReferenceKind.Status or
            ReferenceKind.RedBlue or
            ReferenceKind.Text or
            ReferenceKind.Boolean | ReferenceKind.Number or
            ReferenceKind.NumberVariableReader | ReferenceKind.Number or
            ReferenceKind.StringVariableReader or
            ReferenceKind.StringVariableWriter or
            ReferenceKind.StringVariableReader | ReferenceKind.GlobalVariableWriter or
            ReferenceKind.StringVariableReader | ReferenceKind.GlobalVariableReader or
            ReferenceKind.StringVariableReader | ReferenceKind.Spot or
            ReferenceKind.StringVariableReader | ReferenceKind.Unit or
            ReferenceKind.StringVariableReader | ReferenceKind.Power or
            ReferenceKind.StringVariableReader | ReferenceKind.Class or
            ReferenceKind.StringVariableReader | ReferenceKind.Skill or
            ReferenceKind.StringVariableReader | ReferenceKind.Skillset or
            ReferenceKind.StringVariableReader | ReferenceKind.Dungeon or
            ReferenceKind.StringVariableReader | ReferenceKind.Race or
            ReferenceKind.StringVariableReader | ReferenceKind.Text or
            ReferenceKind.StringVariableReader | ReferenceKind.Status or
            ReferenceKind.StringVariableReader | ReferenceKind.RedBlue or
            ReferenceKind.StringVariableReader | ReferenceKind.Boolean
            => true,
            _ => false,
        };
    }

    public static string ProcessLate(this ReferenceKind reference, int i, string name, int indent)
    {
        StringBuilder builder = new(256);
        if (reference.CanProcessEarly())
        {
            return reference.ProcessEarly(i, name, indent, builder, "DiagnosticSeverity.Warning <= RequiredSeverity");
        }

        if (reference == (ReferenceKind.Number | ReferenceKind.CompoundText))
        {
            ProcessLateNumberOrCompoundText(i, name, indent, builder);
            return builder.ToString();
        }
        if ((reference & ReferenceKind.StringVariableReader) == ReferenceKind.StringVariableReader)
        {
            ProcessLateString(reference, i, name, indent, builder);
            return builder.ToString();
        }
        if (reference == ReferenceKind.CompoundText)
        {
            ProcessLateCompoundText(i, name, indent, builder, true);
            return builder.ToString();
        }

        return "// ERROR 1";
    }

    private static void ProcessLateCompoundText(int i, string name, int indent, StringBuilder builder, bool processArgument)
    {
        StringBuilder Inden()
        {
            for (int j = 0; j < indent; ++j) { builder.Append("    "); }
            return builder;
        }
        if (i != 0 && processArgument)
        {
            builder.Append("argument = ref arguments[");
            if (i < 0)
            {
                builder.Append('i');
            }
            else
            {
                builder.Append(i);
            }
            builder.AppendLine("];");
            Inden();
        }
        if (i < -1)
        {
            builder.Append("AddReferenceAndValidate_CompoundText(ref result, i, ref argument);").AppendLine();
        }
        else
        {
            builder.Append("AddReferenceAndValidate_CompoundText(ref result, ").Append(i).Append(", ref argument);").AppendLine();
        }
    }

    private static void ProcessLateSkillSkillset(ReferenceKind reference, bool hasSkill, bool hasSkillset, int i, string name, int indent, StringBuilder builder)
    {
        StringBuilder Inden()
        {
            for (int j = 0; j < indent; ++j) { builder.Append("    "); }
            return builder;
        }
        if (i != 0)
        {
            builder.Append("argument = ref arguments[");
            if (i < 0)
            {
                builder.Append('i');
            }
            else
            {
                builder.Append(i);
            }
            builder.AppendLine("];");
            Inden();
        }
        builder.Append("span = result.GetSpan(argument.TokenId);").AppendLine();
        Inden().Append("if (span.IsEmpty)").AppendLine();
        Inden().Append("{").AppendLine();
        Inden().Append("    result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument is empty.").Append(reference).Append(" is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
        Inden().Append("}").AppendLine();
        Inden().Append("else if (span[0] == '@')").AppendLine();
        Inden().Append("{").AppendLine();
        Inden().Append("    if (span.Length == 1)").AppendLine();
        Inden().Append("    {").AppendLine();
        Inden().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument is empty string '@'. ").Append(reference).Append(" is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
        Inden().Append("    }").AppendLine();
        Inden().Append("    else").AppendLine();
        Inden().Append("    {").AppendLine();
        Inden().Append("        argument.ReferenceId = result.StringVariableReaderSet.GetOrAdd(span.Slice(1), argument.TokenId);").AppendLine();
        Inden().Append("        argument.ReferenceKind = ReferenceKind.StringVariableReader;").AppendLine();
        Inden().Append("        argument.HasReference = true;").AppendLine();
        Inden().Append("    }").AppendLine();
        Inden().Append("}").AppendLine();
        Inden().Append("else").AppendLine();
        Inden().Append("{").AppendLine();
        Inden().Append("    ref var track = ref AmbiguousDictionary_SkillSkillset.TryGet(span);").AppendLine();
        Inden().Append("    if (Unsafe.IsNullRef(ref track))").AppendLine();
        Inden().Append("    {").AppendLine();
        Inden().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument {span} is not ").Append(reference).Append(" required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
        Inden().Append("    }").AppendLine();
        Inden().Append("    else").AppendLine();
        Inden().Append("    {").AppendLine();
        Inden().Append("        switch (track.Kind)").AppendLine();
        Inden().Append("        {").AppendLine();
        if (hasSkill)
        {
            Inden().Append("            case ReferenceKind.Skill:").AppendLine();
            Inden().Append("                argument.ReferenceId = result.SkillSet.GetOrAdd(span, argument.TokenId);").AppendLine();
            Inden().Append("                argument.ReferenceKind = ReferenceKind.Skill;").AppendLine();
            Inden().Append("                argument.HasReference = true;").AppendLine();
            Inden().Append("                break;").AppendLine();
        }
        if (hasSkillset)
        {
            Inden().Append("            case ReferenceKind.Skillset:").AppendLine();
            Inden().Append("                argument.ReferenceId = result.SkillsetSet.GetOrAdd(span, argument.TokenId);").AppendLine();
            Inden().Append("                argument.ReferenceKind = ReferenceKind.Skillset;").AppendLine();
            Inden().Append("                argument.HasReference = true;").AppendLine();
            Inden().Append("                break;").AppendLine();
        }

        Inden().Append("          default:").AppendLine();
        Inden().Append("              result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument {span} is not ").Append(reference).Append(" required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
        Inden().Append("              argument.HasReference = false;").AppendLine();
        Inden().Append("              break;").AppendLine();
        Inden().Append("        }").AppendLine();
        Inden().Append("    }").AppendLine();
        Inden().Append("}").AppendLine();
    }

    private static void ProcessLateUnitClassPowerSpotRace(ReferenceKind reference, bool hasUnit, bool hasClass, bool hasPower, bool hasSpot, bool hasRace, int i, string name, int indent, StringBuilder builder)
    {
        StringBuilder Inden()
        {
            for (int j = 0; j < indent; ++j) { builder.Append("    "); }
            return builder;
        }
        if (i != 0)
        {
            builder.Append("argument = ref arguments[");
            if (i < 0)
            {
                builder.Append('i');
            }
            else
            {
                builder.Append(i);
            }
            builder.AppendLine("];");
            Inden();
        }
        builder.Append("span = result.GetSpan(argument.TokenId);").AppendLine();
        Inden().Append("if (span.IsEmpty)").AppendLine();
        Inden().Append("{").AppendLine();
        Inden().Append("    result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument is empty.").Append(reference).Append(" is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
        Inden().Append("}").AppendLine();
        Inden().Append("else if (span[0] == '@')").AppendLine();
        Inden().Append("{").AppendLine();
        Inden().Append("    if (span.Length == 1)").AppendLine();
        Inden().Append("    {").AppendLine();
        Inden().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument is empty string '@'. ").Append(reference).Append(" is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
        Inden().Append("    }").AppendLine();
        Inden().Append("    else").AppendLine();
        Inden().Append("    {").AppendLine();
        Inden().Append("        argument.ReferenceId = result.StringVariableReaderSet.GetOrAdd(span.Slice(1), argument.TokenId);").AppendLine();
        Inden().Append("        argument.ReferenceKind = ReferenceKind.StringVariableReader;").AppendLine();
        Inden().Append("        argument.HasReference = true;").AppendLine();
        Inden().Append("    }").AppendLine();
        Inden().Append("}").AppendLine();
        Inden().Append("else").AppendLine();
        Inden().Append("{").AppendLine();
        Inden().Append("    ref var track = ref AmbiguousDictionary_UnitClassPowerSpotRace.TryGet(span);").AppendLine();
        Inden().Append("    if (Unsafe.IsNullRef(ref track))").AppendLine();
        Inden().Append("    {").AppendLine();
        Inden().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument {span} is not ").Append(reference).Append(" required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
        Inden().Append("    }").AppendLine();
        Inden().Append("    else").AppendLine();
        Inden().Append("    {").AppendLine();
        Inden().Append("        switch (track.Kind)").AppendLine();
        Inden().Append("        {").AppendLine();
        if (hasUnit)
        {
            Inden().Append("            case ReferenceKind.Unit:").AppendLine();
            Inden().Append("                argument.ReferenceId = result.UnitSet.GetOrAdd(span, argument.TokenId);").AppendLine();
            Inden().Append("                argument.ReferenceKind = ReferenceKind.Unit;").AppendLine();
            Inden().Append("                argument.HasReference = true;").AppendLine();
            Inden().Append("                break;").AppendLine();
        }
        if (hasClass)
        {
            Inden().Append("            case ReferenceKind.Class:").AppendLine();
            Inden().Append("                argument.ReferenceId = result.ClassSet.GetOrAdd(span, argument.TokenId);").AppendLine();
            Inden().Append("                argument.ReferenceKind = ReferenceKind.Class;").AppendLine();
            Inden().Append("                argument.HasReference = true;").AppendLine();
            Inden().Append("                break;").AppendLine();
        }
        if (hasPower)
        {
            Inden().Append("            case ReferenceKind.Power:").AppendLine();
            Inden().Append("                argument.ReferenceId = result.PowerSet.GetOrAdd(span, argument.TokenId);").AppendLine();
            Inden().Append("                argument.ReferenceKind = ReferenceKind.Power;").AppendLine();
            Inden().Append("                argument.HasReference = true;").AppendLine();
            Inden().Append("                break;").AppendLine();
        }
        if (hasSpot)
        {
            Inden().Append("            case ReferenceKind.Spot:").AppendLine();
            Inden().Append("                argument.ReferenceId = result.SpotSet.GetOrAdd(span, argument.TokenId);").AppendLine();
            Inden().Append("                argument.ReferenceKind = ReferenceKind.Spot;").AppendLine();
            Inden().Append("                argument.HasReference = true;").AppendLine();
            Inden().Append("                break;").AppendLine();
        }
        if (hasRace)
        {
            Inden().Append("            case ReferenceKind.Race:").AppendLine();
            Inden().Append("                argument.ReferenceId = result.RaceSet.GetOrAdd(span, argument.TokenId);").AppendLine();
            Inden().Append("                argument.ReferenceKind = ReferenceKind.Race;").AppendLine();
            Inden().Append("                argument.HasReference = true;").AppendLine();
            Inden().Append("                break;").AppendLine();
        }

        Inden().Append("          default:").AppendLine();
        Inden().Append("              result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument {span} is not ").Append(reference).Append(" required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
        Inden().Append("              argument.HasReference = false;").AppendLine();
        Inden().Append("              break;").AppendLine();
        Inden().Append("        }").AppendLine();
        Inden().Append("    }").AppendLine();
        Inden().Append("}").AppendLine();
    }

    private static void ProcessLateNumberOrCompoundText(int i, string name, int indent, StringBuilder builder)
    {
        StringBuilder Inden()
        {
            for (int j = 0; j < indent; ++j) { builder.Append("    "); }
            return builder;
        }

        if (i != 0)
        {
            builder.Append("argument = ref arguments[");
            if (i < 0)
            {
                builder.Append('i');
            }
            else
            {
                builder.Append(i);
            }
            builder.AppendLine("];");
            Inden();
        }

        builder.Append("if (argument.IsNumber)").AppendLine();
        Inden().Append("{").AppendLine();
        Inden().Append("    if (argument.Number != 0)").AppendLine();
        Inden().Append("    {").AppendLine();
        Inden().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument is not Number, CompoundText required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
        Inden().Append("    }").AppendLine();
        Inden().Append("}").AppendLine();
        Inden().Append("else").AppendLine();
        Inden().Append("{").AppendLine();
        for (int j = 0; j < indent + 1; ++j) { builder.Append("    "); } ProcessLateCompoundText(i, name, indent + 1, builder, false);
        Inden().Append("}").AppendLine();
    }

    private static void ProcessLateString(ReferenceKind reference, int i, string name, int indent, StringBuilder builder)
    {
        bool hasSpot = (reference & ReferenceKind.Spot) == ReferenceKind.Spot;
        bool hasPower = (reference & ReferenceKind.Power) == ReferenceKind.Power;
        bool hasUnit = (reference & ReferenceKind.Unit) == ReferenceKind.Unit;
        bool hasClass = (reference & ReferenceKind.Class) == ReferenceKind.Class;
        bool hasRace = (reference & ReferenceKind.Race) == ReferenceKind.Race;
        if (hasSpot || hasPower || hasUnit || hasClass || hasRace)
        {
            ProcessLateUnitClassPowerSpotRace(reference, hasUnit, hasClass, hasPower, hasSpot, hasRace, i, name, indent, builder);
            return;
        }

        bool hasSkill = (reference & ReferenceKind.Skill) == ReferenceKind.Skill;
        bool hasSkillset = (reference & ReferenceKind.Skillset) == ReferenceKind.Skillset;
        if (hasSkill || hasSkillset)
        {
            ProcessLateSkillSkillset(reference, hasSkill, hasSkillset, i, name, indent, builder);
            return;
        }

        builder.AppendLine("// ERROR 0");
    }

    public static string ProcessEarly(this ReferenceKind reference, int i, string name, int indent, StringBuilder? builder = null, string severityCheck = "context.CreateError(DiagnosticSeverity.Warning)")
    {
        builder ??= new(256);
        StringBuilder I()
        {
            for (int j = 0; j < indent; ++j) { builder.Append("    "); }
            return builder;
        }
        StringBuilder F()
        {
            if (i == 0)
            {
                return builder;
            }
            builder.Append("argument = ref arguments[");
            if (i < 0)
            {
                builder.Append('i');
            }
            else
            {
                builder.Append(i);
            }
            builder.AppendLine("];");
            return I();
        }

        switch (reference)
        {
            case ReferenceKind.Scenario:
            case ReferenceKind.Event:
            case ReferenceKind.Story:
            case ReferenceKind.Movetype:
            case ReferenceKind.Skill:
            case ReferenceKind.Skillset:
            case ReferenceKind.Race:
            case ReferenceKind.Unit:
            case ReferenceKind.Class:
            case ReferenceKind.Power:
            case ReferenceKind.Spot:
            case ReferenceKind.Field:
            case ReferenceKind.Object:
            case ReferenceKind.Dungeon:
            case ReferenceKind.Voice:
            case ReferenceKind.AttributeType:
            case ReferenceKind.VoiceTypeReader:
            case ReferenceKind.VoiceTypeWriter:
            case ReferenceKind.NumberVariableReader:
            case ReferenceKind.NumberVariableWriter:
            case ReferenceKind.GlobalVariableReader:
            case ReferenceKind.GlobalVariableWriter:
            case ReferenceKind.FieldAttributeTypeReader:
            case ReferenceKind.FieldAttributeTypeWriter:
            case ReferenceKind.FieldIdReader:
            case ReferenceKind.FieldIdWriter:
            case ReferenceKind.ClassTypeReader:
            case ReferenceKind.ClassTypeWriter:
            case ReferenceKind.GlobalStringVariableReader:
            case ReferenceKind.GlobalStringVariableWriter:
            case ReferenceKind.map:
            case ReferenceKind.bgm:
            case ReferenceKind.imagedata:
            case ReferenceKind.face:
            case ReferenceKind.se:
            case ReferenceKind.picture:
            case ReferenceKind.image_file:
            case ReferenceKind.flag:
            case ReferenceKind.font:
                F().Append("argument.ReferenceKind = ReferenceKind.").Append(reference).AppendLine(";");
                I().Append("argument.ReferenceId = result.").Append(reference).AppendLine("Set.GetOrAdd(result.GetSpan(argument.TokenId), argument.TokenId);");
                I().Append("argument.HasReference = true;").AppendLine();
                break;
            case ReferenceKind.Number:
                F().Append("if (!argument.IsNumber)").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorAdd_NumberIsExpected(argument.TokenId, $\" The "); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }; builder.Append("-th argument of action '").Append(name).AppendLine(@"' must be Number."");");
                I().Append("}").AppendLine();
                break;
            case ReferenceKind.Boolean:
            case ReferenceKind.Status:
            case ReferenceKind.RedBlue:
                F().Append("if (!argument.IsNumber && (argument.HasReference = NodeValidator.Is").Append(reference).AppendLine("(result.GetSpan(argument.TokenId), out argument.ReferenceId)))");
                I().Append("{").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.").Append(reference).AppendLine(";");
                I().Append("}").AppendLine();
                I().Append("else").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorList.Add(new($\"The "); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument of action '").Append(name).Append("' must be ").Append(reference).AppendLine(".\", result.TokenList[argument.TokenId].Range));");
                I().Append("}").AppendLine();
                break;
            case ReferenceKind.NumberVariableReader | ReferenceKind.Number:
                F().Append("if (!argument.IsNumber)").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.NumberVariableReader;").AppendLine();
                I().Append("    argument.ReferenceId = result.NumberVariableReaderSet.GetOrAdd(result.GetSpan(argument.TokenId), argument.TokenId);").AppendLine();
                I().Append("    argument.HasReference = true;").AppendLine();
                I().Append("}").AppendLine();
                break;
            case ReferenceKind.StringVariableReader:
                F().Append("span = result.GetSpan(argument.TokenId);").AppendLine();
                I().Append("if (span.IsEmpty)").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument is empty. String Variable is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
                I().Append("}").AppendLine();
                I().Append("else if (span[0] == '@')").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    if (span.Length != 1)").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        argument.ReferenceId = result.").Append(reference).Append("Set.GetOrAdd(span.Slice(1), argument.TokenId);").AppendLine();
                I().Append("        argument.HasReference = true;").AppendLine();
                I().Append("    }").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.").Append(reference).Append(";").AppendLine();
                I().Append("}").AppendLine();
                I().Append("else").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    if (").Append(severityCheck).Append(")").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument '{span}' is String Variable. '@' should be written.\", result.TokenList[argument.TokenId].Range));").AppendLine();
                I().Append("    }").AppendLine();
                I().Append("    argument.ReferenceId = result.").Append(reference).Append("Set.GetOrAdd(span, argument.TokenId);").AppendLine();
                I().Append("    argument.HasReference = true;").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.").Append(reference).Append(";").AppendLine();
                I().Append("}").AppendLine();
                break;
            case ReferenceKind.StringVariableWriter:
                F().Append("span = result.GetSpan(argument.TokenId);").AppendLine();
                I().Append("if (span.IsEmpty)").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument is empty. String Variable is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
                I().Append("}").AppendLine();
                I().Append("else if (span[0] == '@')").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    if (span.Length == 1)").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument '@' must be String Variable.\", result.TokenList[argument.TokenId].Range));").AppendLine();
                I().Append("    }").AppendLine();
                I().Append("    else").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        argument.ReferenceId = result.").Append(reference).Append("Set.GetOrAdd(span.Slice(1), argument.TokenId);").AppendLine();
                I().Append("        argument.HasReference = true;").AppendLine();
                I().Append("    }").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.").Append(reference).Append(";").AppendLine();
                I().Append("}").AppendLine();
                I().Append("else").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    if (").Append(severityCheck).Append(")").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument '{span}' is String Variable. '@' should be written.\", result.TokenList[argument.TokenId].Range));").AppendLine();
                I().Append("    }").AppendLine();
                I().Append("    argument.ReferenceId = result.").Append(reference).Append("Set.GetOrAdd(span, argument.TokenId);").AppendLine();
                I().Append("    argument.HasReference = true;").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.").Append(reference).Append(";").AppendLine();
                I().Append("}").AppendLine();
                break;
            case ReferenceKind.StringVariableReader | ReferenceKind.Spot:
            case ReferenceKind.StringVariableReader | ReferenceKind.Unit:
            case ReferenceKind.StringVariableReader | ReferenceKind.Power:
            case ReferenceKind.StringVariableReader | ReferenceKind.Class:
            case ReferenceKind.StringVariableReader | ReferenceKind.Skill:
            case ReferenceKind.StringVariableReader | ReferenceKind.Skillset:
            case ReferenceKind.StringVariableReader | ReferenceKind.Dungeon:
            case ReferenceKind.StringVariableReader | ReferenceKind.Race:
            case ReferenceKind.StringVariableReader | ReferenceKind.GlobalVariableWriter:
            case ReferenceKind.StringVariableReader | ReferenceKind.GlobalVariableReader:
                reference ^= ReferenceKind.StringVariableReader;
                F().Append("span = result.GetSpan(argument.TokenId);").AppendLine();
                I().Append("if (span.IsEmpty)").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument is empty. String Variable is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
                I().Append("}").AppendLine();
                I().Append("else if (span[0] == '@')").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    if (span.Length != 1)").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        argument.ReferenceId = result.StringVariableReaderSet.GetOrAdd(span.Slice(1), argument.TokenId);").AppendLine();
                I().Append("        argument.ReferenceKind = ReferenceKind.StringVariableReader;").AppendLine();
                I().Append("        argument.HasReference = true;").AppendLine();
                I().Append("    }").AppendLine();
                I().Append("}").AppendLine();
                I().Append("else").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    argument.ReferenceId = result.").Append(reference).Append("Set.GetOrAdd(span, argument.TokenId);").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.").Append(reference).Append(";").AppendLine();
                I().Append("    argument.HasReference = true;").AppendLine();
                I().Append("}").AppendLine();
                break;
            case ReferenceKind.StringVariableReader | ReferenceKind.Text:
                F().Append("span = result.GetSpan(argument.TokenId);").AppendLine();
                I().Append("if (span.Length > 1 && span[0] == '@')").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    argument.ReferenceId = result.StringVariableReaderSet.GetOrAdd(span.Slice(1), argument.TokenId);").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.StringVariableReader;").AppendLine();
                I().Append("    argument.HasReference = true;").AppendLine();
                I().Append("}").AppendLine();
                break;
            case ReferenceKind.StringVariableReader | ReferenceKind.Status:
            case ReferenceKind.StringVariableReader | ReferenceKind.RedBlue:
            case ReferenceKind.StringVariableReader | ReferenceKind.Boolean:
                F().Append("span = result.GetSpan(argument.TokenId);").AppendLine();
                I().Append("if (argument.IsNumber)").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorList.Add(new($\"The "); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument of action '").Append(name).Append("' must be ").Append(reference).AppendLine(". Actually Number appears.\", result.TokenList[argument.TokenId].Range));");
                I().Append("}").AppendLine();
                I().Append("else if (span.IsEmpty)").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorList.Add(new($\"The "); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument of action '").Append(name).Append("' must be ").Append(reference).AppendLine(". Actually empty string appears.\", result.TokenList[argument.TokenId].Range));");
                I().Append("}").AppendLine();
                I().Append("else if (span[0] == '@')").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    if (span.Length == 1)").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        result.ErrorList.Add(new($\"The "); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument of action '").Append(name).Append("' must be ").Append(reference).AppendLine(". Actually empty string('@') appears.\", result.TokenList[argument.TokenId].Range));");
                I().Append("    }").AppendLine();
                I().Append("    else").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        argument.ReferenceId = result.StringVariableReaderSet.GetOrAdd(span.Slice(1), argument.TokenId);").AppendLine();
                I().Append("        argument.ReferenceKind = ReferenceKind.StringVariableReader;").AppendLine();
                I().Append("        argument.HasReference = true;").AppendLine();
                I().Append("    }").AppendLine();
                I().Append("}").AppendLine();
                I().Append("else if (argument.HasReference = NodeValidator.Is").Append(reference ^ ReferenceKind.StringVariableReader).AppendLine("(span, out argument.ReferenceId))");
                I().Append("{").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.").Append(reference ^ ReferenceKind.StringVariableReader).AppendLine(";");
                I().Append("}").AppendLine();
                I().Append("else").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorList.Add(new($\"The "); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); }
                builder.Append("-th argument of action '").Append(name).Append("' must be ").Append(reference).AppendLine(".\", result.TokenList[argument.TokenId].Range));");
                I().Append("}").AppendLine();
                break;
        }
        return builder.ToString();
    }

    public static string ProcessElementLate(string nodeKind, ref ElementInfo element, int indent)
    {
        var builder = new StringBuilder();
        var refkind = element.referenceKind;
        bool HasFlag(ReferenceKind kind)
        {
            return 0 != (refkind & kind);
        }

        var hasSpot = HasFlag(ReferenceKind.Spot);
        var hasUnit = HasFlag(ReferenceKind.Unit);
        var hasClass = HasFlag(ReferenceKind.Class);
        var hasPower = HasFlag(ReferenceKind.Power);
        var hasRace = HasFlag(ReferenceKind.Race);
        var hasSkill = HasFlag(ReferenceKind.Skill);
        var hasSkillset = HasFlag(ReferenceKind.Skillset);
        return "// ERROR  " + nodeKind + ", " + element.name + " of " + element.referenceKind;
    }
}
