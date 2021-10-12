namespace Wahren.AbstractSyntaxTree.Parser;

using Node;
using System;

public struct Result : IDisposable
{
    public DualList<char> Source = new();
    public TokenList TokenList = new();
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

    public AttributeNode? AttributeNode = null;

    public ContextNode? ContextNode = null;
    public SoundNode? SoundNode = null;

    public uint Id = default;
    public bool Success = default;
    public string? FilePath = default;

    public Result(uint id)
    {
        Id = id;
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

        Source.Dispose();
        Success = false;
        FilePath = null;
    }

    public void UnionLast2Tokens()
    {
        if (TokenList.GetPrecedingNewLineCount(TokenList.LastIndex) == 0)
        {
            TokenList.GetLength(TokenList.LastIndex - 1) += TokenList.GetLength(TokenList.LastIndex) + TokenList.GetPrecedingWhitespaceCount(TokenList.LastIndex);
            TokenList.RemoveLast();
        }
        else
        {
            this.ErrorAdd_LastAndLastBut1MustBeOneLine();
        }
    }

    public bool IsEndOfLine(uint tokenIndex)
    {
        uint lineIndex = TokenList.GetLine(tokenIndex);
        if (lineIndex >= Source.Count)
        {
            return true;
        }

        return TokenList.GetOffset(tokenIndex) + TokenList.GetLength(tokenIndex) >= Source[lineIndex].Count;
    }

    public Span<char> GetSpan(uint tokenIndex) => Source[TokenList.GetLine(tokenIndex)].AsSpan(TokenList.GetOffset(tokenIndex), TokenList.GetLength(tokenIndex));
    public Span<char> GetSpan(uint tokenIndex, uint trailingTokenCount)
    {
        if (trailingTokenCount == 0)
        {
            return GetSpan(tokenIndex);
        }

        var line = TokenList.GetLine(tokenIndex);
        var offset = TokenList.GetOffset(tokenIndex);
        var lastIndex = tokenIndex + trailingTokenCount;
        var offsetLast = TokenList.GetOffset(lastIndex);
        var lengthLast = TokenList.GetLength(lastIndex);
        return Source[line].AsSpan(offset, offsetLast + lengthLast - offset);
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
