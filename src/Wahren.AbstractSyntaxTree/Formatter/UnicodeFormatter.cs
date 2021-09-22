namespace Wahren.AbstractSyntaxTree.Parser;

public partial struct Result : ISpanFormattable
{
    public override string ToString() => toString ??= ToString("", null);

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        if (Source.Count == 0 || (Source.Count == 1 && Source[0].IsEmpty))
        {
            return string.Empty;
        }

        int count = (int)Source.AddAllCount() + (Source.Count << 1);
        ReadOnlySpan<char> formatSpan = format;
        var isCrLf = formatSpan.Contains('r');
        var isLf = formatSpan.Contains('n');
        if (!isCrLf && !isLf)
        {
            isCrLf = Environment.NewLine == "\r\n";
        }

        var rental = ArrayPool<char>.Shared.Rent(count << 1);
        FormatterContext context;
        var isBeautify = formatSpan.Contains('b');
        if (isBeautify)
        {
            for (var i = 0; i < 3; i++)
            {
                context = new(rental);
                var answer = isCrLf ? TryFormatBeautifyCrLf(ref context) : TryFormatBeautifyLf(ref context);
                if (answer)
                {
                    goto RETURN;
                }
                else if (!context.FormatFail)
                {
                    break;
                }

                var newOne = ArrayPool<char>.Shared.Rent(rental.Length << 1);
                ArrayPool<char>.Shared.Return(rental);
                rental = newOne;
            }
        }

        context = new(rental);
        var success = isCrLf ? TryFormat_AsIs_CrLf(ref context) : TryFormat_AsIs_Lf(ref context);
        if (!success)
        {
            ArrayPool<char>.Shared.Return(rental);
            throw new InvalidOperationException();
        }

    RETURN:
        var returnValue = new string(rental.AsSpan(0, context.Written));
        ArrayPool<char>.Shared.Return(rental);
        return returnValue;
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? formatProvider = null)
    {
        charsWritten = 0;
        if (Source.Count == 0 || (Source.Count == 1 && Source[0].IsEmpty))
        {
            return true;
        }

        var isCrLf = format.Contains('r');
        var isLf = format.Contains('n');
        if (!isCrLf && !isLf)
        {
            isCrLf = Environment.NewLine == "\r\n";
        }

        FormatterContext context;
        var isBeautify = format.Contains('b');
        if (isBeautify)
        {
            context = new(destination);
            var answer = isCrLf ? TryFormatBeautifyCrLf(ref context) : TryFormatBeautifyLf(ref context);
            if (answer)
            {
                charsWritten = context.Written;
                return true;
            }
            else if (!context.FormatFail)
            {
                return false;
            }
        }

        context = new(destination);
        var answer2 = isCrLf ? TryFormat_AsIs_CrLf(ref context) : TryFormat_AsIs_Lf(ref context);
        if (!answer2)
        {
            return false;
        }

        charsWritten = context.Written;
        return true;
    }

    private static bool FillSpaces(ref FormatterContext context, int spaces)
    {
        if (spaces > context.Destination.Length)
        {
            return false;
        }

        context.Destination.Slice(0, spaces).Fill(' ');
        context.Destination = context.Destination.Slice(spaces);
        context.Written += spaces;
        return true;
    }
}
