namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public static class ReferenceKindHelper
{
    public static bool CanProcessArgument(this ReferenceKind[][] referencesArray)
    {
        foreach (var references in referencesArray)
        {
            if (!references.CanProcessArgument())
            {
                return false;
            }
        }

        return true;
    }

    public static bool CanProcessArgument(this ReferenceKind[] references)
    {
        foreach (var reference in references)
        {
            if (!reference.CanProcessArgument())
            {
                return false;
            }
        }

        return true;
    }

    public static bool CanProcessArgument(this ReferenceKind reference)
    {
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
            case ReferenceKind.Number:
            case ReferenceKind.Boolean:
            case ReferenceKind.Status:
            case ReferenceKind.RedBlue:
            case ReferenceKind.NumberVariableReader | ReferenceKind.Number:
            case ReferenceKind.StringVariableReader:
            case ReferenceKind.StringVariableWriter:
            case ReferenceKind.StringVariableReader | ReferenceKind.Spot:
            case ReferenceKind.StringVariableReader | ReferenceKind.Unit:
            case ReferenceKind.StringVariableReader | ReferenceKind.Power:
            case ReferenceKind.StringVariableReader | ReferenceKind.Class:
            case ReferenceKind.StringVariableReader | ReferenceKind.Skill:
            case ReferenceKind.StringVariableReader | ReferenceKind.Skillset:
            case ReferenceKind.StringVariableReader | ReferenceKind.Dungeon:
            case ReferenceKind.StringVariableReader | ReferenceKind.Race:
            case ReferenceKind.StringVariableReader | ReferenceKind.Text:
                return true;
        }

        return false;
    }

    public static string ProcessArgument(this ReferenceKind reference, int i, string name, int indent)
    {
        var builder = new System.Text.StringBuilder();
        System.Text.StringBuilder I()
        {
            for (int j = 0; j < indent; ++j) { builder.Append("    "); }
            return builder;
        }
        System.Text.StringBuilder F()
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
                F().Append("if (!argument.IsNumber && (argument.HasReference = Is").Append(reference).AppendLine("(result.GetSpan(argument.TokenId), out argument.ReferenceId)))");
                I().Append("{").AppendLine();
                I().Append("    argument.ReferenceKind = ReferenceKind.").Append(reference).AppendLine(";");
                I().Append("}").AppendLine();
                I().Append("else").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorList.Add(new($\"The "); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument of action '").Append(name).Append("' must be ").Append(reference).AppendLine(".\", result.TokenList[argument.TokenId].Range));");
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
                I().Append("    result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument is empty. String Variable is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
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
                I().Append("    if (context.CreateError(DiagnosticSeverity.Warning))").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument '{span}' is String Variable. '@' should be written.\", result.TokenList[argument.TokenId].Range));").AppendLine();
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
                I().Append("    result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument is empty. String Variable is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
                I().Append("}").AppendLine();
                I().Append("else if (span[0] == '@')").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    if (span.Length == 1)").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument '@' must be String Variable.\", result.TokenList[argument.TokenId].Range));").AppendLine();
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
                I().Append("    if (context.CreateError(DiagnosticSeverity.Warning))").AppendLine();
                I().Append("    {").AppendLine();
                I().Append("        result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument '{span}' is String Variable. '@' should be written.\", result.TokenList[argument.TokenId].Range));").AppendLine();
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
                reference ^= ReferenceKind.StringVariableReader;
                F().Append("span = result.GetSpan(argument.TokenId);").AppendLine();
                I().Append("if (span.IsEmpty)").AppendLine();
                I().Append("{").AppendLine();
                I().Append("    result.ErrorList.Add(new($\""); if (i < 0) { builder.Append("{i}"); } else { builder.Append(i); } builder.Append("-th argument is empty. String Variable is required by action '").Append(name).Append("'.\", result.TokenList[argument.TokenId].Range));").AppendLine();
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
        }
        return builder.ToString();
    }
}
