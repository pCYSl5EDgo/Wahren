namespace Wahren.AbstractSyntaxTree.Parser;
using Wahren.AbstractSyntaxTree.Project;

public static partial class PerResultValidator
{
    public static void CollectError(Span<Result> results, System.Collections.Concurrent.ConcurrentBag<ProjectError> errorBag, ref StringSpanKeyTrackableSet<AmbiguousNameReference> set)
    {
        var enumerator = set.GetEnumerator();
        System.Text.StringBuilder? builder = default;
        while (enumerator.MoveNext(out var key, out var tracks))
        {
            if (tracks.Length < 2)
            {
                continue;
            }

            if (builder is null)
            {
                builder = new(256);
            }
            else
            {
                builder.Clear();
            }
            builder.Append($"Duplicate name '{key}' of struct {tracks[0].Kind}.");
            foreach ((int resultId, _, var kind, uint index) in tracks)
            {
                ref var result = ref results[resultId];
                ref var tokenList = ref result.TokenList;
                builder.Append($"\n    {result.FilePath}({tokenList.GetLine(index) + 1}, {tokenList.GetOffset(index) + 1})");
            }
            errorBag.Add(new(builder.ToString()));
        }
    }
}
