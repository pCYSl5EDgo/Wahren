using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wahren.Compiler;

public struct FormatArguments
{
    public string rootFolder;
    public bool @switch;
    public bool time;
    public bool forceUnicode;
    public bool deleteDiscardedToken;
    public bool ____help____;

    public FormatArguments()
    {
        rootFolder = ".";
        @switch = false;
        time = false;
        forceUnicode = false;
        deleteDiscardedToken = false;
        ____help____ = false;
    }

    public bool TryParse(ReadOnlySpan<string> ____span____)
    {
        bool __switch__ = true, __time__ = true, __forceUnicode__ = true, __deleteDiscardedToken__ = true;
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

            if (__forceUnicode__ && (____argument____.Equals("--forceUnicode", StringComparison.OrdinalIgnoreCase) || ____argument____.Equals("-f", StringComparison.OrdinalIgnoreCase)))
            {
                __forceUnicode__ = false;
                if (++____index____ >= ____span____.Length)
                {
                    forceUnicode = true;
                    return true;
                }

                if (bool.TryParse(____span____[____index____].AsSpan().Trim(), out forceUnicode))
                {
                    ++____index____;
                }
                else
                {
                    forceUnicode = true;
                }
                continue;
            }

            if (__deleteDiscardedToken__ && (____argument____.Equals("--delete-discarded-token", StringComparison.OrdinalIgnoreCase) || ____argument____.Equals("-d", StringComparison.OrdinalIgnoreCase)))
            {
                __deleteDiscardedToken__ = false;
                if (++____index____ >= ____span____.Length)
                {
                    deleteDiscardedToken = true;
                    return true;
                }

                if (bool.TryParse(____span____[____index____].AsSpan().Trim(), out deleteDiscardedToken))
                {
                    ++____index____;
                }
                else
                {
                    deleteDiscardedToken = true;
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
                    rootFolder = JsonSerializer.Deserialize(____argument____, SourceGenerationContext.Default.String) ?? rootFolder;
                    
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
