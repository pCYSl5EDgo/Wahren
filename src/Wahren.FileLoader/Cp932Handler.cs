using System.Text;

namespace Wahren.FileLoader;

public static class Cp932Handler
{
    static Cp932Handler()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding = Encoding.GetEncoding(932);
    }

    private static readonly Encoding Encoding;

    public static void Load(Span<byte> content, out DualList<char> source)
    {
        if (content.Length >= 32 && Unsafe.As<byte, uint>(ref MemoryMarshall.GetReference(content)) == 0x04030201U)
        {
            CryptUtility.Decrypt(content.Slice(4, 28), content.Slice(32));
            content = content.Slice(32);
        }

        ReadOnlySpan<byte> span = content;
        var rental = ArrayPool<char>.Shared.Rent(Encoding.GetCharCount(span));
        var count = Encoding.GetChars(span, rental.AsSpan());
        UnicodeHandler.LoadFromString(rental.AsSpan(0, count), out source);
        ArrayPool<char>.Shared.Return(rental);
    }
}
