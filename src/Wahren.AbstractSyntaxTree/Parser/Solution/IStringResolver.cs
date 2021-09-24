namespace Wahren.AbstractSyntaxTree.Parser;

public interface IStringResolver<T>
    where T : IIdEnumerator
{
    void Register(nuint resultId, uint id, ReadOnlySpan<char> key);
    ref Result TryGet(ReadOnlySpan<char> key, out uint value);
    bool Exists(ReadOnlySpan<char> key);

    T GetIdEnumerator();
}
