using System;
using System.Buffers;
using System.Globalization;
using System.Text;

namespace Wahren.DebugPaper;

public sealed record class DebugPaper(string? Folder, bool IsDebugMode, bool Encode, bool Margin, bool TestMap, bool SkillCheckOff, uint? Zoom, uint? ZoomMax, uint? ZoomTick, uint? Width, uint? Height, uint? Midi)
{
    public Reinterpret Reinterpret;

    static DebugPaper()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static readonly DebugPaper DefaultDebug = new("a_default", true, false, false, false, false, default, default, default, default, default, default);
    public static readonly DebugPaper DefaultRelease = new("a_default", false, false, false, false, false, default, default, default, default, default, default);

    public static DebugPaper? CreateFromSpan(ReadOnlySpan<byte> span, bool isDebugMode)
    {
        if (span.IsEmpty)
        {
            return null;
        }

        var rental = ArrayPool<char>.Shared.Rent(span.Length);
        try
        {
            for (int i = 0; i < span.Length; i++)
            {
                rental[i] = (char)span[i];
                if (span[i] > 0x7f)
                {
                    return ParseNotAscii(span, isDebugMode);
                }
            }

            return Parse(rental.AsSpan(0, span.Length), isDebugMode);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rental);
        }
    }

    private static DebugPaper? Parse(ReadOnlySpan<char> span, bool isDebugMode)
    {
        string? scenario = null;
        Reinterpret reinterpret = new();
        bool encode = false;
        bool margin = false;
        bool testmap = false;
        bool skillcheckoff = false;
        uint? zoom = default;
        uint? zoomMax = default;
        uint? zoomTick = default;
        uint? height = default;
        uint? width = default;
        byte? midi = default;
        while (!span.IsEmpty)
        {
            var crlf = span.IndexOf("\r\n");
            if (crlf == 0 && span.Length == 2)
            {
                break;
            }

            var thisLine = span.Slice(0, crlf);
            span = span.Slice(crlf + 2);
            switch (thisLine.Length)
            {
                case 0:
                    continue;
                case 1:
                case 2:
                case 3:
                    scenario ??= thisLine.ToString();
                    continue;
                case 4:
                    if (thisLine.SequenceEqual("race"))
                    {
                        reinterpret.race = true;
                    }
                    else if (thisLine.SequenceEqual("spot"))
                    {
                        reinterpret.spot = true;
                    }
                    else if (thisLine.SequenceEqual("unit"))
                    {
                        reinterpret.unit = true;
                    }
                    scenario ??= thisLine.ToString();
                    continue;
                case 5:
                    if (thisLine.SequenceEqual("class"))
                    {
                        reinterpret.@class = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("event"))
                    {
                        reinterpret.@event = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("field"))
                    {
                        reinterpret.field = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("power"))
                    {
                        reinterpret.power = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("skill"))
                    {
                        reinterpret.skill = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("sound"))
                    {
                        reinterpret.sound = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("story"))
                    {
                        reinterpret.story = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("voice"))
                    {
                        reinterpret.voice = true;
                        continue;
                    }

                    goto CHECK_5;
                case 6:
                    if (thisLine.SequenceEqual("object"))
                    {
                        reinterpret.@object = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("detail"))
                    {
                        reinterpret.detail = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("encode"))
                    {
                        encode = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("margin"))
                    {
                        margin = true;
                        continue;
                    }

                    goto CHECK_6;
                case 7:
                    if (thisLine.SequenceEqual("context"))
                    {
                        reinterpret.context = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("dungeon"))
                    {
                        reinterpret.dungeon = true;
                        continue;
                    }
                    else if (thisLine.SequenceEqual("testmap"))
                    {
                        testmap = true;
                        continue;
                    }

                    goto CHECK_7;
                case 8:
                    if (thisLine.SequenceEqual("skillset"))
                    {
                        reinterpret.skillset = true;
                        continue;
                    }

                    goto CHECK_8;
                case 9:
                    if (thisLine.SequenceEqual("attribute"))
                    {
                        reinterpret.attribute = true;
                        continue;
                    }
                    break;
                case 13:
                    if (thisLine.SequenceEqual("skillcheckoff"))
                    {
                        skillcheckoff = true;
                        continue;
                    }

                    break;
            }

            if (thisLine.StartsWith("zoomtick"))
            {
                if (uint.TryParse(thisLine.Slice(8), out var output))
                {
                    zoomTick = output;
                }
                continue;
            }
        CHECK_8:
            if (thisLine.StartsWith("zoommax"))
            {
                if (uint.TryParse(thisLine.Slice(7), out var output))
                {
                    zoomMax = output;
                }
                continue;
            }
        CHECK_7:
            if (thisLine.StartsWith("height"))
            {
                if (ushort.TryParse(thisLine.Slice(6), out var output))
                {
                    height = output;
                }
                continue;
            }
        CHECK_6:
            if (thisLine.StartsWith("width"))
            {
                if (ushort.TryParse(thisLine.Slice(5), out var output))
                {
                    width = output;
                }
                continue;
            }
        CHECK_5:
            if (thisLine.StartsWith("midi"))
            {
                if (byte.TryParse(thisLine.Slice(4), out var output))
                {
                    midi = (byte)(output > 10 ? 10 : (output < 1 ? 1 : output));
                }
                continue;
            }
            else if (thisLine.StartsWith("zoom"))
            {
                if (uint.TryParse(thisLine.Slice(4), out var output))
                {
                    zoom = output > 150000 ? 150000 : output;
                    zoomMax = 1000;
                }
                continue;
            }

            scenario ??= thisLine.ToString();
        }

        return new DebugPaper(scenario, isDebugMode, encode, margin, testmap, skillcheckoff, zoom, zoomMax, zoomTick, width, height, midi)
        {
            Reinterpret = reinterpret,
        };
    }

    private static DebugPaper? ParseNotAscii(ReadOnlySpan<byte> span, bool isDebugMode)
    {
        var encoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage);
        var count = encoding.GetCharCount(span);
        if (count <= 0)
        {
            return null;
        }

        var rental = ArrayPool<char>.Shared.Rent(count);
        var chars = rental.AsSpan(0, count);
        try
        {
            encoding.GetChars(span, chars);
            return Parse(chars, isDebugMode);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rental);
        }
    }
}

public struct Reinterpret
{
    public bool race;
    public bool spot;
    public bool unit;
    public bool @class;
    public bool @event;
    public bool field;
    public bool power;
    public bool skill;
    public bool sound;
    public bool story;
    public bool voice;
    public bool @object;
    public bool detail;
    public bool context;
    public bool dungeon;
    public bool movetype;
    public bool scenario;
    public bool skillset;
    public bool attribute;
}
