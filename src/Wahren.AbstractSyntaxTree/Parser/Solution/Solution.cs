namespace Wahren.AbstractSyntaxTree.Parser;

public sealed partial class Solution : IDisposable
{
    public DisposableList<Result> Files = new();
    public List<SolutionError> SolutionErrorList = new();

    public void Dispose()
    {
        Files.Dispose();
        SolutionErrorList.Dispose();
    }

    public ref Result TryResolveDetail(ReadOnlySpan<char> name, out ReferenceKind kind, out uint index)
    {
        throw new NotImplementedException();
    }

    public ref Result TryResolveSkillOrSkillset(ReadOnlySpan<char> name, out ReferenceKind kind, out uint index)
    {
        throw new NotImplementedException();
    }

    public ref Result TryResolveUnitOrClass(ReadOnlySpan<char> name, out ReferenceKind kind, out uint index)
    {
        throw new NotImplementedException();
    }

    public bool ContainsAttributeType(ReadOnlySpan<char> name)
    {
        if (name.IsEmpty)
        {
            return false;
        }

        foreach (ref var file in Files)
        {
            var node = file.AttributeNode;
            if (node is null)
            {
                continue;
            }


            foreach (ref var other in node.Others)
            {
                if (other.Value is not null)
                {
                    ref var range = ref other.Value.ElementKeyRange;
                    var span = file.Source[range.Line].AsSpan(range.Offset, range.Length);
                    if (name.SequenceEqual(span))
                    {
                        return true;
                    }

                    continue;
                }

                if (other.ScenarioVariant is null)
                {
                    continue;
                }

                foreach (var item in other.ScenarioVariant)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    ref var range = ref item.ElementKeyRange;
                    var span = file.Source[range.Line].AsSpan(range.Offset, range.Length);
                    if (name.SequenceEqual(span))
                    {
                        return true;
                    }

                    break;
                }
            }
            break;
        }

        return Enum.TryParse<AttributeTypeKind>(name, out _);
    }
}
