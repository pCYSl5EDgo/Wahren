using System.Text;

namespace Wahren.FileLoader;

public static unsafe class Utf8Handler
{
    private static readonly ArrayPoolList<char>.AddConverter<byte> GetChars = Encoding.UTF8.GetChars;

    public static void Load(Span<byte> content, out DualList<char> source) => AsciiCompatibleHandler.Load(content, GetChars, out source);
}
