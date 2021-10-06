namespace Wahren.AbstractSyntaxTree;

public struct Token
{
    public Position Position;
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
        return $"Start: {{{Position.Line}, {Position.Offset}}}, Kind: {Kind}}}";
    }

    public bool IsFirstTokenInTheLine => PrecedingWhitespaceCount == Position.Offset;
}
