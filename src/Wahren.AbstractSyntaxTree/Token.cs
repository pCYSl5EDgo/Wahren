namespace Wahren.AbstractSyntaxTree;

public struct Token
{
    public Range Range;
    public uint Length;
    public uint PrecedingNewLineCount;
    public uint PrecedingWhitespaceCount;
    /// <summary>
    /// Kind switch CallAction => ActionKind, CallFunction => FunctionKind
    /// </summary>
    public uint Other;
    public TokenKind Kind;

    public override string ToString()
    {
        return $"Start: {{{Range.StartInclusive.Line}, {Range.StartInclusive.Offset}}}, Kind: {Kind}, End: {{{Range.EndExclusive.Line}, {Range.EndExclusive.Offset}}}";
    }

    public bool IsFirstTokenInTheLine => PrecedingWhitespaceCount == Range.StartInclusive.Offset;
}
