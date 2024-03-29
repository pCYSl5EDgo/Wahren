﻿namespace Wahren.AbstractSyntaxTree.Parser;

using Node;
using System;

public struct Result : IDisposable, ITokenIdModifiable
{
    public DualList<char> Source = new();
    public TokenList TokenList = new();
    public ArrayPoolList<Error> ErrorList = new();

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

    public void DecrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        foreach (ref var node in ScenarioNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in EventNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in StoryNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in MovetypeNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in FieldNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in ObjectNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in SkillNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in SkillsetNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in UnitNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in ClassNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in SpotNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in PowerNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in RaceNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in DungeonNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in VoiceNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in WorkspaceNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in DetailNodeList.AsSpan())
        {
            node.DecrementToken(indexEqualToOrGreaterThan, count);
        }
        AttributeNode?.DecrementToken(indexEqualToOrGreaterThan, count);
        ContextNode?.DecrementToken(indexEqualToOrGreaterThan, count);
        SoundNode?.DecrementToken(indexEqualToOrGreaterThan, count);
    }

    public void IncrementToken(uint indexEqualToOrGreaterThan, uint count)
    {
        foreach (ref var node in ScenarioNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in EventNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in StoryNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in MovetypeNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in FieldNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in ObjectNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in SkillNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in SkillsetNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in UnitNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in ClassNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in SpotNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in PowerNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in RaceNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in DungeonNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in VoiceNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in WorkspaceNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        foreach (ref var node in DetailNodeList.AsSpan())
        {
            node.IncrementToken(indexEqualToOrGreaterThan, count);
        }
        AttributeNode?.IncrementToken(indexEqualToOrGreaterThan, count);
        ContextNode?.IncrementToken(indexEqualToOrGreaterThan, count);
        SoundNode?.IncrementToken(indexEqualToOrGreaterThan, count);
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
        var lineLast = TokenList.GetLine(lastIndex);
        if (line != lineLast)
        {
#if JAPANESE
            var text = "改行してはなりません。";
#else
            var text = "Invalid Line Feed.";
#endif
            ErrorList.Add(new(text, line, offset, TokenList.GetLength(tokenIndex)));
            return GetSpan(tokenIndex);
        }
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

        ArrayPoolList<char> list = new();
        try
        {
            var isBeautify = formatSpan.Contains('b');
            if (isBeautify)
            {
                if (Formatter.UnicodeFormatter.TryFormat(ref this, ref list))
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
