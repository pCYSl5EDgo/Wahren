namespace Wahren.AbstractSyntaxTree.Modifier;

public struct InsertCallActionInfo : IDisposable
{
    public ActionKind Kind;

    public DualList<char> arguments;

    public InsertCallActionInfo(ActionKind kind)
    {
        Kind = kind;
        arguments = new();
    }

    public void Dispose()
    {
        Kind = default;
        arguments.Dispose();
    }
}
