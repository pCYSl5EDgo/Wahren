namespace Wahren.AbstractSyntaxTree.Parser;

public sealed partial class Solution : IDisposable
{
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
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> FieldAttributeTypeWriter = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> FieldAttributeTypeReader = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> FieldIdWriter = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> FieldIdReader = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> VoiceTypeWriter = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> VoiceTypeReader = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> ClassTypeWriter = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> ClassTypeReader = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> map = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> bgm = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> imagedata = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> face = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> se = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> picture = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> image_file = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> flag = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> font = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> NumberVariableWriter = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> NumberVariableReader = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> StringVariableWriter = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> StringVariableReader = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> GlobalVariableWriter = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> GlobalVariableReader = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> GlobalStringVariableWriter = new();
    public StringSpanKeyTrackableDictionary<(uint FileId, uint Id), uint> GlobalStringVariableReader = new();

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
        FieldAttributeTypeWriter.Dispose();
        FieldAttributeTypeReader.Dispose();
        FieldIdWriter.Dispose();
        FieldIdReader.Dispose();
        VoiceTypeWriter.Dispose();
        VoiceTypeReader.Dispose();
        ClassTypeWriter.Dispose();
        ClassTypeReader.Dispose();
        map.Dispose();
        bgm.Dispose();
        imagedata.Dispose();
        face.Dispose();
        se.Dispose();
        picture.Dispose();
        image_file.Dispose();
        flag.Dispose();
        font.Dispose();
        NumberVariableWriter.Dispose();
        StringVariableWriter.Dispose();
        NumberVariableReader.Dispose();
        StringVariableReader.Dispose();
        GlobalVariableWriter.Dispose();
        GlobalVariableReader.Dispose();
        GlobalStringVariableWriter.Dispose();
        GlobalStringVariableReader.Dispose();
    }

    public ref Result TryResolveDetail(uint queryFileId, ReadOnlySpan<char> name, out ReferenceKind kind, out uint index)
    {
        throw new NotImplementedException();
    }

    public ref Result TryResolveSkillOrSkillset(uint queryFileId, ReadOnlySpan<char> name, out ReferenceKind kind, out uint index)
    {
        throw new NotImplementedException();
    }

    public ref Result TryResolveUnitOrClass(uint queryFileId, ReadOnlySpan<char> name, out ReferenceKind kind, out uint index)
    {
        throw new NotImplementedException();
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

                if (other.ScenarioVariant is null)
                {
                    continue;
                }

                foreach (var item in other.ScenarioVariant)
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
}
