using System.Text;

namespace Wahren.PooledList;

public static class PooledStringBuilder
{
    [System.ThreadStatic]
    private static StringBuilder? builder;

    public static StringBuilder Rent() => (builder ??= new StringBuilder()).Clear();
}
