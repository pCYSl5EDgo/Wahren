using Wahren.AbstractSyntaxTree.Statement;
using Wahren.AbstractSyntaxTree.Statement.Expression;

namespace Wahren.AbstractSyntaxTree.Project;

using Parser;

public sealed partial class Project : IDisposable
{
    public DiagnosticSeverity RequiredSeverity;
    public DisposableList<Result> Files = new();
    public List<SolutionError> SolutionErrorList = new();

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
        SolutionErrorList.Dispose();

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
            if (PerResultValidator.CollectNames(fileSpan, ref ambiguousDictionary_UnitClassPowerSpotRace, ref ambiguousDictionary_SkillSkillset))
            {
                AmbiguousDictionary_UnitClassPowerSpotRace = ambiguousDictionary_UnitClassPowerSpotRace.ToSingle();
                AmbiguousDictionary_SkillSkillset = ambiguousDictionary_SkillSkillset.ToSingle();
            }
            else
            {
                PerResultValidator.CollectError(fileSpan, ref SolutionErrorList, ref ambiguousDictionary_UnitClassPowerSpotRace);
                PerResultValidator.CollectError(fileSpan, ref SolutionErrorList, ref ambiguousDictionary_SkillSkillset);
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

        ref var range = ref result.TokenList[argument.TokenId].Range;
        if (range.OneLine)
        {
            AddReferenceAndValidate_CompoundText_Line(ref result, argumentIndex, ref argument, result.GetSpan(argument.TokenId));
            return;
        }

        ref var source = ref result.Source;
        
        AddReferenceAndValidate_CompoundText_Line(ref result, argumentIndex, ref argument, source[range.StartInclusive.Line].AsSpan(range.StartInclusive.Offset));
        for (uint lineIndex = range.StartInclusive.Line + 1, lineEnd = range.EndExclusive.Line; lineIndex < lineEnd; lineIndex++)
        {
            AddReferenceAndValidate_CompoundText_Line(ref result, argumentIndex, ref argument, source[lineIndex].AsSpan());
        }
        if (range.EndExclusive.Offset != 0)
        {
            AddReferenceAndValidate_CompoundText_Line(ref result, argumentIndex, ref argument, source[range.EndExclusive.Line].AsSpan(0, range.EndExclusive.Offset));
        }
    }

    private void AddReferenceAndValidate_CompoundText_Line(ref Result result, int argumentIndex, ref Argument argument, ReadOnlySpan<char> line)
    {
        var andIndex = line.IndexOf('&');
        if (andIndex == -1)
        {
            return;
        }

        do
        {
            line = line.Slice(andIndex + 1);
            andIndex = line.IndexOf('&');
            if (andIndex == -1)
            {
                return;
            }
            if (andIndex == 0)
            {
                continue;
            }

            var span = line.Slice(0, andIndex);
            if (span[0] == '@')
            {
                span = span.Slice(1);
                if (span.IsEmpty)
                {
                    continue;
                }
                for (int i = 0; i < span.Length; ++i)
                {
                    var c = span[i];
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                    {

                    }
                    else
                    {
                        goto FAIL;
                    }
                }
                
                argument.ReferenceId = result.StringVariableReaderSet.GetOrAdd(span, argument.TokenId);
                argument.ReferenceKind = ReferenceKind.StringVariableReader;
                argument.HasReference = true;
            }
            else
            {
                for (int i = 0; i < span.Length; ++i)
                {
                    var c = span[i];
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                    {

                    }
                    else
                    {
                        goto FAIL;
                    }
                }

                ref var track = ref AmbiguousDictionary_UnitClassPowerSpotRace.TryGet(span);
                if (Unsafe.IsNullRef(ref track))
                {
                    argument.ReferenceId = result.NumberVariableReaderSet.GetOrAdd(span, argument.TokenId);
                    argument.ReferenceKind = ReferenceKind.NumberVariableReader;
                    argument.HasReference = true;
                }
                else
                {
                    switch (track.Kind)
                    {
                        case ReferenceKind.Spot:
                            argument.ReferenceId = result.SpotSet.GetOrAdd(span, argument.TokenId);
                            argument.ReferenceKind = ReferenceKind.Spot;
                            argument.HasReference = true;
                            break;
                        case ReferenceKind.Power:
                            argument.ReferenceId = result.PowerSet.GetOrAdd(span, argument.TokenId);
                            argument.ReferenceKind = ReferenceKind.Power;
                            argument.HasReference = true;
                            break;
                        case ReferenceKind.Class:
                            argument.ReferenceId = result.ClassSet.GetOrAdd(span, argument.TokenId);
                            argument.ReferenceKind = ReferenceKind.Class;
                            argument.HasReference = true;
                            break;
                        case ReferenceKind.Unit:
                            argument.ReferenceId = result.UnitSet.GetOrAdd(span, argument.TokenId);
                            argument.ReferenceKind = ReferenceKind.Unit;
                            argument.HasReference = true;
                            break;
                        default:
                            argument.ReferenceId = result.NumberVariableReaderSet.GetOrAdd(span, argument.TokenId);
                            argument.ReferenceKind = ReferenceKind.NumberVariableReader;
                            argument.HasReference = true;
                            break;
                    }
                }
            }

            line = line.Slice(andIndex + 1);
            andIndex = line.IndexOf('&');
            if (andIndex == -1)
            {
                return;
            }

        FAIL:
            continue;
        } while (true);
    }

    private void AddReferenceAndValidate(ref Result result, IStatement statement)
    {
        switch (statement)
        {
            case CallActionStatement call:
                AddReferenceAndValidate(ref result, call);
                break;
            case WhileStatement @while:
                AddReferenceAndValidate(ref result, @while.Condition);
                foreach (var item in @while.Statements.AsSpan())
                {
                    AddReferenceAndValidate(ref result, item);
                }
                break;
            case IfStatement @if:
                AddReferenceAndValidate(ref result, @if.Condition);
                foreach (var item in @if.Statements.AsSpan())
                {
                    AddReferenceAndValidate(ref result, item);
                }
                if (@if.HasElseStatement)
                {
                    foreach (var item in @if.ElseStatements.AsSpan())
                    {
                        AddReferenceAndValidate(ref result, item);
                    }
                }
                break;
            case BattleStatement battle:
                foreach (var item in battle.Statements.AsSpan())
                {
                    AddReferenceAndValidate(ref result, item);
                }
                break;
        }
    }

    private void AddReferenceAndValidate(ref Result result, IReturnBooleanExpression? expression)
    {
        switch (expression)
        {
            case CallFunctionExpression call:
                AddReferenceAndValidate(ref result, call);
                break;
            case LogicOperatorExpression logic:
                AddReferenceAndValidate(ref result, logic.Left);
                AddReferenceAndValidate(ref result, logic.Right);
                break;
            case NumberComparerExpression numberCompare:
                AddReferenceAndValidate(ref result, numberCompare.Left);
                AddReferenceAndValidate(ref result, numberCompare.Right);
                break;
        }
    }

    private void AddReferenceAndValidate(ref Result result, IReturnNumberExpression? expression)
    {
        switch (expression)
        {
            case CallFunctionExpression callFunction:
                AddReferenceAndValidate(ref result, callFunction);
                break;
            case NumberCalculatorOperatorExpression numberCalculator:
                AddReferenceAndValidate(ref result, numberCalculator.Left);
                AddReferenceAndValidate(ref result, numberCalculator.Right);
                break;
        }
    }
}
