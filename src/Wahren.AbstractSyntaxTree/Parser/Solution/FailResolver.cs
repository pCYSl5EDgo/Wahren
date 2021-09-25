namespace Wahren.AbstractSyntaxTree.Parser;

public sealed class FailResolver : ISolutionResolver
{
    public static readonly FailResolver Default = new();

    public ref Result TryGetClassNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetDungeonNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetEventNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetFieldNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetMovetypeNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetObjectNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetPowerNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetRaceNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetScenarioNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetSkillNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetSkillsetNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetSpotNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetStoryNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetUnitNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryGetVoiceNode(ReadOnlySpan<char> name, out uint index)
    {
        index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryResolveDetail(ReadOnlySpan<char> name, out ReferenceKind kind, out uint index)
    {
        kind = ReferenceKind.Scenario; index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryResolveSkillOrSkillset(ReadOnlySpan<char> name, out ReferenceKind kind, out uint index)
    {
        kind = ReferenceKind.Skill; index = 0; return ref Unsafe.NullRef<Result>();
    }

    public ref Result TryResolveUnitOrClass(ReadOnlySpan<char> name, out ReferenceKind kind, out uint index)
    {
        kind = ReferenceKind.Unit; index = 0; return ref Unsafe.NullRef<Result>();
    }
}