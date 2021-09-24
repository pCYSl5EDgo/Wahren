namespace Wahren.AbstractSyntaxTree.Parser;

public interface ISolutionResolver
{
    ref Result TryGetScenarioNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetEventNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetStoryNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetMovetypeNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetSkillNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetSkillsetNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetRaceNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetUnitNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetClassNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetPowerNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetSpotNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetFieldNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetObjectNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetDungeonNode(ReadOnlySpan<char> name, out uint index);
    ref Result TryGetVoiceNode(ReadOnlySpan<char> name, out uint index);

    ref Result TryResolveDetail(ReadOnlySpan<char> name, out NodeKind kind, out uint index);
    ref Result TryResolveUnitOrClass(ReadOnlySpan<char> name, out NodeKind kind, out uint index);
    ref Result TryResolveSkillOrSkillset(ReadOnlySpan<char> name, out NodeKind kind, out uint index);
}
