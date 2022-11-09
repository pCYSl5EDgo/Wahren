namespace Wahren.Compiler;

public struct AnalyzeArguments
{
    public string rootFolder;
    public bool @switch;
    public PseudoDiagnosticSeverity severity;
    public bool time;
    public bool ____help____;

    public AnalyzeArguments()
    {
        rootFolder = ".";
        @switch = false;
        severity = PseudoDiagnosticSeverity.Error;
        time = false;
        ____help____ = false;
    }

    public bool TryParse(ReadOnlySpan<string> ____span____)
    {
        bool __switch__ = true, __severity__ = true, __time__ = true;
        for (int ____index____ = 0, ____argumentIndex____ = 0; ____index____ < ____span____.Length;)
        {
            var ____argument____ = ____span____[____index____].AsSpan().Trim();
            if (____argument____.IsEmpty)
            {
                ++____index____;
                continue;
            }

            if (__switch__ && ____argument____.Equals("--switch", StringComparison.OrdinalIgnoreCase))
            {
                __switch__ = false;
                if (++____index____ >= ____span____.Length)
                {
                    @switch = true;
                    return true;
                }

                if (bool.TryParse(____span____[____index____].AsSpan().Trim(), out @switch))
                {
                    ++____index____;
                }
                else
                {
                    @switch = true;
                }
                continue;
            }

            if (__severity__ && (____argument____.Equals("--severity", StringComparison.OrdinalIgnoreCase) || ____argument____.Equals("-s", StringComparison.OrdinalIgnoreCase)))
            {
                __severity__ = false;
                if (++____index____ < ____span____.Length && Enum.TryParse(____span____[____index____++].AsSpan().Trim(), out severity))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            if (__time__ && (____argument____.Equals("--time", StringComparison.OrdinalIgnoreCase) || ____argument____.Equals("-t", StringComparison.OrdinalIgnoreCase)))
            {
                __time__ = false;
                if (++____index____ >= ____span____.Length)
                {
                    time = true;
                    return true;
                }

                if (bool.TryParse(____span____[____index____].AsSpan().Trim(), out time))
                {
                    ++____index____;
                }
                else
                {
                    time = true;
                }
                continue;
            }

            if (____argument____.Equals("--help", StringComparison.OrdinalIgnoreCase) || ____argument____.Equals("-h", StringComparison.OrdinalIgnoreCase))
            {
                ____help____ = true;
                return true;
            }

            switch (____argumentIndex____++)
            {
                case 0:
                    rootFolder = ____argument____.IsEmpty ? string.Empty : new(____argument____);
                    ++____index____;
                    break;
                default:
                    return false;
            }
        }

        return true;
    }

    public override string ToString() => """

        """;
}
