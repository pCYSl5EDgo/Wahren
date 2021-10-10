namespace Wahren.AbstractSyntaxTree.Parser;
using Wahren.AbstractSyntaxTree.Project;

public static partial class PerResultValidator
{
    public static bool CollectNames(Span<Result> results, ref StringSpanKeyTrackableSet<AmbiguousNameReference> setUnitClassPowerSpotRace, ref StringSpanKeyTrackableSet<AmbiguousNameReference> setSkillSkillset)
    {
        bool noDuplication = true;
        for (int resultId = 0; resultId < results.Length; ++resultId)
        {
            ref var result = ref results[resultId];
            {
                var span = result.UnitNodeList.AsSpan();
                for (int i = 0; i < span.Length; ++i)
                {
                    ref var node = ref span[i];
                    noDuplication &= setUnitClassPowerSpotRace.TryRegisterTrack(result.GetSpan(node.Name), new(resultId, i, ReferenceKind.Unit, node.Name));
                }
            }
            {
                var span = result.ClassNodeList.AsSpan();
                for (int i = 0; i < span.Length; ++i)
                {
                    ref var node = ref span[i];
                    noDuplication &= setUnitClassPowerSpotRace.TryRegisterTrack(result.GetSpan(node.Name), new(resultId, i, ReferenceKind.Class, node.Name));
                }
            }
            {
                var span = result.PowerNodeList.AsSpan();
                for (int i = 0; i < span.Length; ++i)
                {
                    ref var node = ref span[i];
                    noDuplication &= setUnitClassPowerSpotRace.TryRegisterTrack(result.GetSpan(node.Name), new(resultId, i, ReferenceKind.Power, node.Name));
                }
            }
            {
                var span = result.SpotNodeList.AsSpan();
                for (int i = 0; i < span.Length; ++i)
                {
                    ref var node = ref span[i];
                    noDuplication &= setUnitClassPowerSpotRace.TryRegisterTrack(result.GetSpan(node.Name), new(resultId, i, ReferenceKind.Spot, node.Name));
                }
            }
            {
                var span = result.RaceNodeList.AsSpan();
                for (int i = 0; i < span.Length; ++i)
                {
                    ref var node = ref span[i];
                    noDuplication &= setUnitClassPowerSpotRace.TryRegisterTrack(result.GetSpan(node.Name), new(resultId, i, ReferenceKind.Race, node.Name));
                }
            }
            {
                var span = result.SkillNodeList.AsSpan();
                for (int i = 0; i < span.Length; ++i)
                {
                    ref var node = ref span[i];
                    noDuplication &= setSkillSkillset.TryRegisterTrack(result.GetSpan(node.Name), new(resultId, i, ReferenceKind.Skill, node.Name));
                }
            }
            {
                var span = result.SkillsetNodeList.AsSpan();
                for (int i = 0; i < span.Length; ++i)
                {
                    ref var node = ref span[i];
                    noDuplication &= setSkillSkillset.TryRegisterTrack(result.GetSpan(node.Name), new(resultId, i, ReferenceKind.Skillset, node.Name));
                }
            }
        }

        return noDuplication;
    }

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
