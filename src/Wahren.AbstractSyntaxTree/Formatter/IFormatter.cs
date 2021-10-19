using Wahren.AbstractSyntaxTree.Parser;

namespace Wahren.AbstractSyntaxTree.Formatter;

public interface IFormatter<T>
    where T : unmanaged
{
    abstract static bool TryFormat(ref Result result, ref List<T> destination);
}
