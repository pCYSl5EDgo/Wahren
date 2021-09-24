namespace Wahren.AbstractSyntaxTree.Parser;

public interface IIdEnumerator : IDisposable
{
    bool MoveNext(out ReadOnlySpan<char> key);
}
