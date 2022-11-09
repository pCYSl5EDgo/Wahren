using System.Text.Json.Serialization;

namespace Wahren.Compiler;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(GitHubLatestReleaseResponse))]
[JsonSerializable(typeof(GitHubLatestReleaseResponseAsset[]))]
[JsonSerializable(typeof(GitHubLatestReleaseResponseAsset))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
