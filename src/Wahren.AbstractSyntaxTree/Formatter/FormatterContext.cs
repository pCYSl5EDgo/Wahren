namespace Wahren.AbstractSyntaxTree;

internal ref struct FormatterContext
{
    public Span<char> Destination;
    public int Written;
    public bool JustChangeLine;
    public bool FormatFail;

    public FormatterContext(Span<char> destination)
    {
        Destination = destination;
        Written = 0;
        JustChangeLine = false;
        FormatFail = false;
    }
}
