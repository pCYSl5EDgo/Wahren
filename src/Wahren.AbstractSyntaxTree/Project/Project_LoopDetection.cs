namespace Wahren.AbstractSyntaxTree.Project;

using Parser;
using Statement;
using Statement.Expression;

public sealed partial class Project
{
    public bool EnsureNoLoop()
    {
        int[] offsets = ArrayPool<int>.Shared.Rent(Files.Count);
        try
        {
            var total = 0;
            foreach (ref var file in Files.AsSpan())
            {
            }
        }
        finally
        {
            ArrayPool<int>.Shared.Return(offsets);
        }

        return true;
    }
}
