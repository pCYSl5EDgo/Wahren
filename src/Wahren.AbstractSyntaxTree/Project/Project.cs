using Wahren.AbstractSyntaxTree.Statement;
using Wahren.AbstractSyntaxTree.Statement.Expression;

namespace Wahren.AbstractSyntaxTree.Project;

using Parser;

public sealed partial class Project : IDisposable
{
    public DiagnosticSeverity RequiredSeverity;
    public DisposableList<Result> Files = new();
    public List<ProjectError> ErrorList = new();

    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Scenario = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Event = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Story = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Movetype = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Skill = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Skillset = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Race = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Unit = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Class = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Power = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Spot = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Field = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Object = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Dungeon = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> Voice = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> AttributeType = new();

    public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_SkillSkillset = default;
    public StringSpanKeyTrackableSet<AmbiguousNameReference>.Single AmbiguousDictionary_UnitClassPowerSpotRace = default;

    public void Dispose()
    {
        Files.Dispose();
        ErrorList.Dispose();

        Scenario.Dispose();
        Story.Dispose();
        Movetype.Dispose();
        Event.Dispose();
        Power.Dispose();
        Race.Dispose();
        Spot.Dispose();
        Unit.Dispose();
        Class.Dispose();
        Skill.Dispose();
        Skillset.Dispose();
        Voice.Dispose();
        Field.Dispose();
        Object.Dispose();
        Dungeon.Dispose();
        AttributeType.Dispose();

        AmbiguousDictionary_SkillSkillset.Dispose();
        AmbiguousDictionary_UnitClassPowerSpotRace.Dispose();
    }

    public bool ContainsAttributeType(ReadOnlySpan<char> name)
    {
        if (name.IsEmpty)
        {
            return false;
        }

        foreach (ref var file in Files)
        {
            var node = file.AttributeNode;
            if (node is null)
            {
                continue;
            }


            foreach (ref var other in node.Others)
            {
                if (other.Value is not null)
                {
                    ref var range = ref other.Value.ElementKeyRange;
                    var span = file.Source[range.Line].AsSpan(range.Offset, range.Length);
                    if (name.SequenceEqual(span))
                    {
                        return true;
                    }

                    continue;
                }

                if (other.VariantArray is null)
                {
                    continue;
                }

                foreach (var item in other.VariantArray)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    ref var range = ref item.ElementKeyRange;
                    var span = file.Source[range.Line].AsSpan(range.Offset, range.Length);
                    if (name.SequenceEqual(span))
                    {
                        return true;
                    }

                    break;
                }
            }
            break;
        }

        return Enum.TryParse<AttributeTypeKind>(name, out _);
    }

    public void AddReferenceAndValidate()
    {
        var fileSpan = Files.AsSpan();
        StringSpanKeyTrackableSet<AmbiguousNameReference> ambiguousDictionary_UnitClassPowerSpotRace = new(), ambiguousDictionary_SkillSkillset = new();
        try
        {
            ambiguousDictionary_UnitClassPowerSpotRace.TryRegisterTrack("dead_event_a", new(0, 0, ReferenceKind.Unit, 0));
            ambiguousDictionary_UnitClassPowerSpotRace.TryRegisterTrack("dead_event_d", new(0, 0, ReferenceKind.Unit, 0));
            if (PerResultValidator.CollectNames(fileSpan, ref ambiguousDictionary_UnitClassPowerSpotRace, ref ambiguousDictionary_SkillSkillset))
            {
                AmbiguousDictionary_UnitClassPowerSpotRace = ambiguousDictionary_UnitClassPowerSpotRace.ToSingle();
                AmbiguousDictionary_SkillSkillset = ambiguousDictionary_SkillSkillset.ToSingle();
            }
            else
            {
                PerResultValidator.CollectError(fileSpan, ref ErrorList, ref ambiguousDictionary_UnitClassPowerSpotRace);
                PerResultValidator.CollectError(fileSpan, ref ErrorList, ref ambiguousDictionary_SkillSkillset);
                return;
            }
        }
        finally
        {
            ambiguousDictionary_UnitClassPowerSpotRace.Dispose();
            ambiguousDictionary_SkillSkillset.Dispose();
        }

        for (int fileIndex = 0; fileIndex < fileSpan.Length; ++fileIndex)
        {
            ref var file = ref fileSpan[fileIndex];
            {
                var nodes = file.ScenarioNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.EventNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.StoryNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.MovetypeNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.SkillNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.SkillsetNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.RaceNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.UnitNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.ClassNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.PowerNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.SpotNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.FieldNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.ObjectNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.DungeonNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
            {
                var nodes = file.VoiceNodeList.AsSpan();
                for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
                {
                    ref var node = ref nodes[nodeIndex];
                    AddReferenceAndValidate(ref file, ref node);
                }
            }
        }
    }

    private void AddReferenceAndValidate_CompoundText(ref Result result, int argumentIndex, ref Argument argument)
    {
        if (argument.IsNumber)
        {
            return;
        }

        for (uint i = argument.TokenId + 2U, end = argument.TokenId + argument.TrailingTokenCount + 1U; i < end; ++i)
        {
            var span = result.GetSpan(i);
            if (span.Length != 1 || span[0] != '&')
            {
                continue;
            }

            span = result.GetSpan(i - 2U);
            if (span.Length != 1 || span[0] != '&')
            {
                continue;
            }

            ref var tokenList = ref result.TokenList;
            ref var latter = ref tokenList[i++];
            if (latter.PrecedingWhitespaceCount != 0 || latter.PrecedingNewLineCount != 0)
            {
                continue;
            }
            ref var token = ref tokenList[i - 2U];
            if (token.PrecedingWhitespaceCount != 0 || token.PrecedingNewLineCount != 0)
            {
                continue;
            }
            span = result.GetSpan(i - 2U);
            if (span.IsEmpty)
            {
                continue;
            }

            static bool IsIdentifier(ReadOnlySpan<char> span)
            {
                foreach (var item in span)
                {
                    if ((item < '0' || item > '9') && (item < 'A' || item > 'Z') && (item < 'a' || item > 'z') && item != '_')
                    {
                        return false;
                    }
                }

                return true;
            }
            if (span[0] == '@')
            {
                if (span.Length == 1)
                {
                    continue;
                }

                span = span.Slice(1);
                if (span.IsEmpty || !IsIdentifier(span))
                {
                    continue;
                }

                argument.HasReference = true;
                result.StringVariableReaderSet.GetOrAdd(span, i - 2U);
                i++;
                continue;
            }
            else if (!IsIdentifier(span))
            {
                continue;
            }

            argument.HasReference = true; 
            ref var reference = ref AmbiguousDictionary_UnitClassPowerSpotRace.TryGet(span);
            if (!Unsafe.IsNullRef(ref reference))
            {
                switch (reference.Kind)
                {
                    case ReferenceKind.Unit:
                        result.UnitSet.GetOrAdd(span, i - 2U);
                        continue;
                    case ReferenceKind.Class:
                        result.ClassSet.GetOrAdd(span, i - 2U);
                        continue;
                    case ReferenceKind.Power:
                        result.PowerSet.GetOrAdd(span, i - 2U);
                        continue;
                    case ReferenceKind.Spot:
                        result.SpotSet.GetOrAdd(span, i - 2U);
                        continue;
                }
            }

            result.NumberVariableReaderSet.GetOrAdd(span, i - 2U);
        }
    }

    private void AddReferenceAndValidate_Statement(ref Result result, IStatement statement)
    {
        switch (statement)
        {
            case CallActionStatement call:
                AddReferenceAndValidate_Call(ref result, call);
                break;
            case WhileStatement @while:
                AddReferenceAndValidate_Condition(ref result, @while.Condition);
                foreach (var item in @while.Statements.AsSpan())
                {
                    AddReferenceAndValidate_Statement(ref result, item);
                }
                break;
            case IfStatement @if:
                AddReferenceAndValidate_Condition(ref result, @if.Condition);
                foreach (var item in @if.Statements.AsSpan())
                {
                    AddReferenceAndValidate_Statement(ref result, item);
                }
                if (@if.HasElseStatement)
                {
                    foreach (var item in @if.ElseStatements.AsSpan())
                    {
                        AddReferenceAndValidate_Statement(ref result, item);
                    }
                }
                break;
            case BattleStatement battle:
                foreach (var item in battle.Statements.AsSpan())
                {
                    AddReferenceAndValidate_Statement(ref result, item);
                }
                break;
        }
    }

    private void AddReferenceAndValidate_Condition(ref Result result, IReturnBooleanExpression? expression)
    {
        switch (expression)
        {
            case CallFunctionExpression call:
                AddReferenceAndValidate_Call(ref result, call);
                break;
            case LogicOperatorExpression logic:
                AddReferenceAndValidate_Condition(ref result, logic.Left);
                AddReferenceAndValidate_Condition(ref result, logic.Right);
                break;
            case NumberComparerExpression numberCompare:
                AddReferenceAndValidate_Number(ref result, numberCompare.Left);
                AddReferenceAndValidate_Number(ref result, numberCompare.Right);
                break;
        }
    }

    private void AddReferenceAndValidate_Number(ref Result result, IReturnNumberExpression? expression)
    {
        switch (expression)
        {
            case CallFunctionExpression call:
                AddReferenceAndValidate_Call(ref result, call);
                break;
            case NumberCalculatorOperatorExpression numberCalculator:
                AddReferenceAndValidate_Number(ref result, numberCalculator.Left);
                AddReferenceAndValidate_Number(ref result, numberCalculator.Right);
                break;
        }
    }

    private void SpecialTreatment_unit_friend(ref Result result, ref Pair_NullableString_NullableInt value)
    {
        SpecialTreatment_unit_class_friend(ref result, ref value, "Unit");
    }

    private void SpecialTreatment_class_friend(ref Result result, ref Pair_NullableString_NullableInt value)
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
            result.ErrorAdd($"Value '{span}...' is not Race, Unit, Class required by element 'friend' of struct {kind}. ET0", value.Text);
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
            result.ErrorAdd($"Value '{span}' is not Race, Unit, Class required by element 'friend' of struct {kind}. ET2", value.Text);
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
                result.ErrorAdd($"Value '{span}' is not Race, Unit, Class required by element 'friend' of struct {kind}. ET3", value.Text);
                break;
        }
    }
}
