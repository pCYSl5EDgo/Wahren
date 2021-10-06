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
    public StringSpanKeySlowSet FieldAttributeTypeWriterSet = new();
    public StringSpanKeySlowSet FieldAttributeTypeReaderSet = new();
    public StringSpanKeySlowSet FieldIdWriterSet = new();
    public StringSpanKeySlowSet FieldIdReaderSet = new();
    public StringSpanKeySlowSet VoiceTypeWriterSet = new();
    public StringSpanKeySlowSet VoiceTypeReaderSet = new();
    public StringSpanKeySlowSet ClassTypeWriterSet = new();
    public StringSpanKeySlowSet ClassTypeReaderSet = new();
    public StringSpanKeySlowSet SkillGroupReaderSet = new();

    public StringSpanKeySlowSet imagedataSet = new();
    public StringSpanKeySlowSet imagedata2Set = new();
    public StringSpanKeySlowSet mapSet = new();
    public StringSpanKeySlowSet bgmSet = new();
    public StringSpanKeySlowSet iconSet = new();
    public StringSpanKeySlowSet faceSet = new();
    public StringSpanKeySlowSet soundSet = new();
    public StringSpanKeySlowSet pictureSet = new();
    public StringSpanKeySlowSet image_fileSet = new();
    public StringSpanKeySlowSet flagSet = new();
    public StringSpanKeySlowSet fontSet = new();

    public StringSpanKeySlowSet NumberVariableWriterSet = new();
    public StringSpanKeySlowSet NumberVariableReaderSet = new();
    public StringSpanKeySlowSet StringVariableWriterSet = new();
    public StringSpanKeySlowSet StringVariableReaderSet = new();
    public StringSpanKeySlowSet GlobalVariableWriterSet = new();
    public StringSpanKeySlowSet GlobalVariableReaderSet = new();
    public StringSpanKeySlowSet GlobalStringVariableWriterSet = new();
    public StringSpanKeySlowSet GlobalStringVariableReaderSet = new();

    public AttributeNode? AttributeNode = null;

    public ContextNode? ContextNode = null;
    public SoundNode? SoundNode = null;

    public uint Id = default;
    public bool Success = default;
    public string? FilePath = default;

    public Result(uint id)
    {
        Initialize();
        Id = id;
    }

    private void Initialize()
    {
        UnitSet.InitialAdd("dead_event_a");
        UnitSet.InitialAdd("dead_event_d");
        ScenarioSet.InitialAdd("a");
        ScenarioSet.InitialAdd("b");
        ScenarioSet.InitialAdd("c");
        ScenarioSet.InitialAdd("d");
        ScenarioSet.InitialAdd("e");
        ScenarioSet.InitialAdd("f");
        ScenarioSet.InitialAdd("g");
        ScenarioSet.InitialAdd("h");
        ScenarioSet.InitialAdd("i");
        ScenarioSet.InitialAdd("j");
        VoiceTypeWriterSet.InitialAdd("hold");
        VoiceTypeWriterSet.InitialAdd("advance");
        VoiceTypeWriterSet.InitialAdd("even");
        VoiceTypeWriterSet.InitialAdd("push");
        VoiceTypeWriterSet.InitialAdd("back");
        VoiceTypeReaderSet.InitialAdd("hold");
        VoiceTypeReaderSet.InitialAdd("advance");
        VoiceTypeReaderSet.InitialAdd("even");
        VoiceTypeReaderSet.InitialAdd("push");
        VoiceTypeReaderSet.InitialAdd("back");

        foreach (var attribute in Enum.GetNames<AttributeTypeKind>())
        {
            AttributeTypeSet.InitialAdd(attribute);
        }

        imagedata2Set.InitialAdd("@@");
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
        FieldAttributeTypeWriterSet.Dispose();
        FieldAttributeTypeReaderSet.Dispose();
        FieldIdWriterSet.Dispose();
        FieldIdReaderSet.Dispose();
        VoiceTypeWriterSet.Dispose();
        VoiceTypeReaderSet.Dispose();
        ClassTypeWriterSet.Dispose();
        ClassTypeReaderSet.Dispose();

        NumberVariableWriterSet.Dispose();
        StringVariableWriterSet.Dispose();
        NumberVariableReaderSet.Dispose();
        StringVariableReaderSet.Dispose();
        GlobalVariableWriterSet.Dispose();
        GlobalVariableReaderSet.Dispose();
        GlobalStringVariableWriterSet.Dispose();
        GlobalStringVariableReaderSet.Dispose();
        SkillGroupReaderSet.Dispose();

        imagedataSet.Dispose();
        imagedata2Set.Dispose();
        mapSet.Dispose();
        bgmSet.Dispose();
        iconSet.Dispose();
        faceSet.Dispose();
        soundSet.Dispose();
        pictureSet.Dispose();
        image_fileSet.Dispose();
        flagSet.Dispose();
        fontSet.Dispose();

        Source.Dispose();
        Success = false;
        FilePath = null;
    }

    public void Reset()
    {
        Dispose();
        Initialize();
    }

    public void UnionLast2Tokens()
    {
        ref var last = ref TokenList.Last;
        if (last.PrecedingNewLineCount == 0)
        {
            TokenList[TokenList.LastIndex - 1].Length += last.Length + last.PrecedingWhitespaceCount;
            TokenList.RemoveLast();
        }
        else
        {
            this.ErrorAdd_LastAndLastBut1MustBeOneLine();
        }
    }

    public bool IsEndOfLine(uint tokenIndex)
    {
        ref var token = ref TokenList[tokenIndex];
        ref var position = ref token.Position;
        if (position.Line >= Source.Count)
        {
            return true;
        }

        return position.Offset + token.Length >= Source[position.Line].Count;
    }

    public Span<char> GetSpan(uint tokenIndex)
    {
        ref var token = ref TokenList[tokenIndex];
        ref var start = ref token.Position;
        return Source[start.Line].AsSpan(start.Offset, token.Length);
    }

    public override string ToString() => ToString("");

    public string ToString(string? format)
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
                var formatter = Formatter.UnicodeFormatter.GetDefault(isCrLf);
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

    public bool NoError()
    {
        foreach (var error in ErrorList.AsSpan())
        {
            if (error.Severity == DiagnosticSeverity.Error)
            {
                return false;
            }
        }

        return true;
    }
}
