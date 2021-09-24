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

    public ref Result TryResolveDetail(ReadOnlySpan<char> name, out NodeKind kind, out uint index)
    {
        throw new NotImplementedException();
    }

    public ref Result TryResolveSkillOrSkillset(ReadOnlySpan<char> name, out NodeKind kind, out uint index)
    {
        throw new NotImplementedException();
    }

    public ref Result TryResolveUnitOrClass(ReadOnlySpan<char> name, out NodeKind kind, out uint index)
    {
        throw new NotImplementedException();
    }
}
