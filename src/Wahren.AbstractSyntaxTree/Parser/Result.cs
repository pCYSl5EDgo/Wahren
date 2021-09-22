namespace Wahren.AbstractSyntaxTree.Parser;

using Node;
using System;

public partial struct Result : IDisposable
{
    public DualList<char> Source;
    public List<Token> TokenList;
    public List<Error> ErrorList;
    public DisposableList<INode> NodeList;
    public StringSpanKeySlowSet ScenarioSet;
    public StringSpanKeySlowSet MovetypeSet;
    public StringSpanKeySlowSet SkillOrSkillsetSet;
    public StringSpanKeySlowSet EventSet;
    public StringSpanKeySlowSet RaceSet;
    public StringSpanKeySlowSet UnitSet;
    public StringSpanKeySlowSet ClassSet;
    public StringSpanKeySlowSet PowerSet;
    public StringSpanKeySlowSet SpotSet;
    public StringSpanKeySlowSet VoiceTypeSet;
    public StringSpanKeySlowSet NumberVariableSet;
    public StringSpanKeySlowSet StringVariableSet;

    public AttributeNode? AttributeNode;
    public ContextNode? ContextNode;
    public SoundNode? SoundNode;

    public nuint Id;
    public bool Success;
    public object? Data;
    private string? toString;

    public Result()
    {
        Source = new();
        TokenList = new();
        ErrorList = new();
        NodeList = new();
        ScenarioSet = new();
        ScenarioSet.GetOrAdd("a");
        ScenarioSet.GetOrAdd("b");
        ScenarioSet.GetOrAdd("c");
        ScenarioSet.GetOrAdd("d");
        ScenarioSet.GetOrAdd("e");
        ScenarioSet.GetOrAdd("f");
        ScenarioSet.GetOrAdd("g");
        ScenarioSet.GetOrAdd("h");
        ScenarioSet.GetOrAdd("i");
        ScenarioSet.GetOrAdd("j");

        MovetypeSet = new();
        EventSet = new();
        PowerSet = new();
        SpotSet = new();
        RaceSet = new();
        UnitSet = new();
        ClassSet = new();
        SkillOrSkillsetSet = new();

        VoiceTypeSet = new();
        VoiceTypeSet.GetOrAdd("male");
        VoiceTypeSet.GetOrAdd("female");
        VoiceTypeSet.GetOrAdd("hold");
        VoiceTypeSet.GetOrAdd("advance");
        VoiceTypeSet.GetOrAdd("even");
        VoiceTypeSet.GetOrAdd("push");
        VoiceTypeSet.GetOrAdd("back");

        NumberVariableSet = new();
        StringVariableSet = new();

        AttributeNode = null;
        ContextNode = null;
        SoundNode = null;

        Id = 0;
        Success = false;
        Data = null;
        toString = null;
    }

    public Result(nuint id) : this()
    {
        Id = id;
    }

    public void Dispose()
    {
        TokenList.Dispose();
        ErrorList.Dispose();
        NodeList.Dispose();
        ScenarioSet.Dispose();
        MovetypeSet.Dispose();
        EventSet.Dispose();
        PowerSet.Dispose();
        RaceSet.Dispose();
        SpotSet.Dispose();
        UnitSet.Dispose();
        ClassSet.Dispose();
        SkillOrSkillsetSet.Dispose();
        VoiceTypeSet.Dispose();
        NumberVariableSet.Dispose();
        StringVariableSet.Dispose();
        Source.Dispose();
        Success = false;
        toString = null;
        if (Data is IDisposable disposable)
        {
            disposable.Dispose();
        }
        Data = null;
    }

    public void Reset()
    {
        Dispose();
        ScenarioSet.GetOrAdd("a");
        ScenarioSet.GetOrAdd("b");
        ScenarioSet.GetOrAdd("c");
        ScenarioSet.GetOrAdd("d");
        ScenarioSet.GetOrAdd("e");
        ScenarioSet.GetOrAdd("f");
        ScenarioSet.GetOrAdd("g");
        ScenarioSet.GetOrAdd("h");
        ScenarioSet.GetOrAdd("i");
        ScenarioSet.GetOrAdd("j");

        VoiceTypeSet.GetOrAdd("male");
        VoiceTypeSet.GetOrAdd("female");
        VoiceTypeSet.GetOrAdd("hold");
        VoiceTypeSet.GetOrAdd("advance");
        VoiceTypeSet.GetOrAdd("even");
        VoiceTypeSet.GetOrAdd("push");
        VoiceTypeSet.GetOrAdd("back");
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
}
