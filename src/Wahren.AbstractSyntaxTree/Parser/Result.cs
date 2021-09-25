namespace Wahren.AbstractSyntaxTree.Parser;

using Node;
using System;

public struct Result : IDisposable
{
    public DualList<char> Source = new();
    public List<Token> TokenList = new();
    public List<Error> ErrorList = new();

    public DisposableList<ScenarioNode> ScenarioNodeList = new();
    public DisposableList<EventNode> EventNodeList = new();
    public DisposableList<StoryNode> StoryNodeList = new();
    public DisposableList<MovetypeNode> MovetypeNodeList = new();
    public DisposableList<SkillNode> SkillNodeList = new();
    public DisposableList<SkillsetNode> SkillsetNodeList = new();
    public DisposableList<RaceNode> RaceNodeList = new();
    public DisposableList<UnitNode> UnitNodeList = new();
    public DisposableList<ClassNode> ClassNodeList = new();
    public DisposableList<PowerNode> PowerNodeList = new();
    public DisposableList<SpotNode> SpotNodeList = new();
    public DisposableList<FieldNode> FieldNodeList = new();
    public DisposableList<ObjectNode> ObjectNodeList = new();
    public DisposableList<DungeonNode> DungeonNodeList = new();
    public DisposableList<VoiceNode> VoiceNodeList = new();

    public DisposableList<DetailNode> DetailNodeList = new();
    public DisposableList<WorkspaceNode> WorkspaceNodeList = new();

    public StringSpanKeySlowSet ScenarioSet = new();
    public StringSpanKeySlowSet EventSet = new();
    public StringSpanKeySlowSet StorySet = new();
    public StringSpanKeySlowSet MovetypeSet = new();
    public StringSpanKeySlowSet SkillSet = new();
    public StringSpanKeySlowSet SkillsetSet = new();
    public StringSpanKeySlowSet RaceSet = new();
    public StringSpanKeySlowSet UnitSet = new();
    public StringSpanKeySlowSet ClassSet = new();
    public StringSpanKeySlowSet PowerSet = new();
    public StringSpanKeySlowSet SpotSet = new();
    public StringSpanKeySlowSet FieldSet = new();
    public StringSpanKeySlowSet ObjectSet = new();
    public StringSpanKeySlowSet DungeonSet = new();
    public StringSpanKeySlowSet VoiceSet = new();

    public StringSpanKeySlowSet AttributeTypeSet = new();
    public StringSpanKeySlowSet NumberVariableSet = new();
    public StringSpanKeySlowSet StringVariableSet = new();

    public AttributeNode? AttributeNode = null;
    public ContextNode? ContextNode = null;
    public SoundNode? SoundNode = null;
    public readonly ISolutionResolver Resolver = FailResolver.Default;

    public uint Id = default;
    public bool Success = default;
    public string? FilePath = default;

    public Result()
    {
        ScenarioSet.GetOrAdd("a", uint.MaxValue);
        ScenarioSet.GetOrAdd("b", uint.MaxValue);
        ScenarioSet.GetOrAdd("c", uint.MaxValue);
        ScenarioSet.GetOrAdd("d", uint.MaxValue);
        ScenarioSet.GetOrAdd("e", uint.MaxValue);
        ScenarioSet.GetOrAdd("f", uint.MaxValue);
        ScenarioSet.GetOrAdd("g", uint.MaxValue);
        ScenarioSet.GetOrAdd("h", uint.MaxValue);
        ScenarioSet.GetOrAdd("i", uint.MaxValue);
        ScenarioSet.GetOrAdd("j", uint.MaxValue);

        foreach (var attribute in Enum.GetNames<AttributeTypeKind>())
        {
            AttributeTypeSet.GetOrAdd(attribute, uint.MaxValue);
        }
    }

    public Result(uint id) : this()
    {
        Id = id;
    }

    public Result(ISolutionResolver resolver, uint id) : this(id)
    {
        Resolver = resolver;
    }

    public void Dispose()
    {
        TokenList.Dispose();
        ErrorList.Dispose();

        ScenarioNodeList.Dispose();
        EventNodeList.Dispose();
        StoryNodeList.Dispose();
        MovetypeNodeList.Dispose();
        SkillNodeList.Dispose();
        SkillsetNodeList.Dispose();
        RaceNodeList.Dispose();
        UnitNodeList.Dispose();
        ClassNodeList.Dispose();
        PowerNodeList.Dispose();
        SpotNodeList.Dispose();
        FieldNodeList.Dispose();
        ObjectNodeList.Dispose();
        DungeonNodeList.Dispose();
        VoiceNodeList.Dispose();

        DetailNodeList.Dispose();
        WorkspaceNodeList.Dispose();

        ScenarioSet.Dispose();
        StorySet.Dispose();
        MovetypeSet.Dispose();
        EventSet.Dispose();
        PowerSet.Dispose();
        RaceSet.Dispose();
        SpotSet.Dispose();
        UnitSet.Dispose();
        ClassSet.Dispose();
        SkillSet.Dispose();
        SkillsetSet.Dispose();
        VoiceSet.Dispose();
        FieldSet.Dispose();
        ObjectSet.Dispose();
        DungeonSet.Dispose();

        AttributeTypeSet.Dispose();
        NumberVariableSet.Dispose();
        StringVariableSet.Dispose();
        
        Source.Dispose();
        Success = false;
        FilePath = null;
    }

    public void Reset()
    {
        Dispose();
        ScenarioSet.GetOrAdd("a", uint.MaxValue);
        ScenarioSet.GetOrAdd("b", uint.MaxValue);
        ScenarioSet.GetOrAdd("c", uint.MaxValue);
        ScenarioSet.GetOrAdd("d", uint.MaxValue);
        ScenarioSet.GetOrAdd("e", uint.MaxValue);
        ScenarioSet.GetOrAdd("f", uint.MaxValue);
        ScenarioSet.GetOrAdd("g", uint.MaxValue);
        ScenarioSet.GetOrAdd("h", uint.MaxValue);
        ScenarioSet.GetOrAdd("i", uint.MaxValue);
        ScenarioSet.GetOrAdd("j", uint.MaxValue);

        foreach (var attribute in Enum.GetNames<AttributeTypeKind>())
        {
            AttributeTypeSet.GetOrAdd(attribute, uint.MaxValue);
        }
    }

    public void UnionLast2Tokens()
    {
        ref var last = ref TokenList.Last;
        ref var lastBut1 = ref TokenList[TokenList.LastIndex - 1];
        lastBut1.Range.EndExclusive = last.Range.EndExclusive;
        if (lastBut1.Range.StartInclusive.Line == last.Range.StartInclusive.Line)
        {
            if (lastBut1.Range.StartInclusive.Line == lastBut1.Range.EndExclusive.Line)
            {
                lastBut1.LengthInFirstLine = lastBut1.Range.EndExclusive.Offset - lastBut1.Range.StartInclusive.Offset;
            }
            else
            {
                lastBut1.LengthInFirstLine = (uint)Source[last.Range.StartInclusive.Line].Count - lastBut1.Range.StartInclusive.Offset;
            }
        }

        TokenList.RemoveLast();
    }

    public Span<char> GetSpan(uint tokenIndex)
    {
        ref var token = ref TokenList[tokenIndex];
        ref var start = ref token.Range.StartInclusive;
        return Source[start.Line].AsSpan(start.Offset, token.LengthInFirstLine);
    }

    public override string ToString() => ToString("", null);

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        if (Source.Count == 0 || (Source.Count == 1 && Source[0].IsEmpty))
        {
            return string.Empty;
        }

        ReadOnlySpan<char> formatSpan = format;
        var isCrLf = formatSpan.Contains('r');
        var isLf = formatSpan.Contains('n');
        if (!isCrLf && !isLf)
        {
            isCrLf = Environment.NewLine == "\r\n";
        }

        List<char> list = new();
        try
        {
            var isBeautify = formatSpan.Contains('b');
            if (isBeautify)
            {
                Formatter.IFormatter<char> formatter = Formatter.UnicodeFormatter.GetDefault(isCrLf);
                if (formatter.TryFormat(ref this, ref list))
                {
                    return new string(list.AsSpan());
                }
            }

            list.Clear();
            for (uint i = 0, end = (uint)Source.Count; i < end; ++i)
            {
                if (i != 0)
                {
                    if (isCrLf)
                    {
                        list.AddRange("\r\n");
                    }
                    else
                    {
                        list.Add('\n');
                    }
                }
                list.AddRange(Source[i].AsSpan());
            }
            return new string(list.AsSpan());
        }
        finally
        {
            list.Dispose();
        }
    }
}
