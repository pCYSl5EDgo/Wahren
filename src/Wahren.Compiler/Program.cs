using System.Net.Http;
using System.Reflection;

namespace Wahren.Compiler;

public partial class Program
{
    public static async Task<int> Main(string[] args)
    {
        var (commandKind, readCount) = CommandKindHelper.DecideCommandKind(args);
        switch (commandKind)
        {
            case CommandKind.Analyze:
                {
                    var arguments = new AnalyzeArguments();
                    if (!arguments.TryParse(args.AsSpan(readCount)))
                    {
                        Console.WriteLine(args.ToString());
                        break;
                    }

                    return await AnalyzeAsync(arguments.rootFolder, arguments.@switch, arguments.severity, arguments.time).ConfigureAwait(false);
                }
            case CommandKind.Format:
                {
                    var arguments = new FormatArguments();
                    if (!arguments.TryParse(args.AsSpan(readCount)))
                    {
                        Console.WriteLine(args.ToString());
                        break;
                    }

                    return await FormatAsync(arguments.rootFolder, arguments.@switch, arguments.time, arguments.forceUnicode, arguments.deleteDiscardedToken).ConfigureAwait(false);
                }
            case CommandKind.Update:
                try
                {
                    await CheckUpdateAsync().ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return 1;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                    return 1;
                }
                break;
            case CommandKind.Version:
                if (Assembly.GetExecutingAssembly().GetName() is not { Version: { } version })
                {
                    Console.WriteLine("Version: Unknown");
                    break;
                }

                Console.WriteLine($"Version: {version.Major}.{version.Minor}.{version.Revision}");
                break;
            case CommandKind.License:
                Console.WriteLine("""
                Copyright 2017-2022 pCYSl5EDgo

                Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

                The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

                THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
                """);
                break;
            case CommandKind.Help:
            default:
                Console.WriteLine("""
                Wahren.Compiler.exe

                使用可能なコマンド
                - analyze
                - format
                - update
                - help
                - version
                - license

                各コマンドの詳細については --helpオプションを付与して確認してください
                """);
                break;
        }

        return 0;
    }

    private static async ValueTask CheckUpdateAsync()
    {
        var processPath = Environment.ProcessPath;
        if (processPath is null || processPath.Contains("Wahren.new"))
        {
            return;
        }

        var processDirPath = Path.GetDirectoryName(processPath);
        if (processDirPath is null)
        {
            return;
        }

        var newPath = Path.Combine(processDirPath, "Wahren.new.exe");
        if (File.Exists(newPath))
        {
            return;
        }

        var version = typeof(Program).Assembly.GetName().Version;
        if (version is null)
        {
            return;
        }

        using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromMinutes(1));
        Console.CancelKeyPress += new((object? _, ConsoleCancelEventArgs args) =>
        {
            cancellationTokenSource?.Cancel();
            args.Cancel = true;
        });

        using var client = new HttpClient();
        using var getMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/pCYSl5EDgo/Wahren/releases/latest");
        getMessage.Headers.Accept.Add(new("application/vnd.github.v3+json"));
        getMessage.Headers.UserAgent.Add(new("Wahren", null));
        using var responseMessage = await client.SendAsync(getMessage, cancellationTokenSource.Token).ConfigureAwait(false);
        if (!responseMessage.IsSuccessStatusCode)
        {
            return;
        }


        using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationTokenSource.Token).ConfigureAwait(false);
        var latestResponse = await System.Text.Json.JsonSerializer.DeserializeAsync(stream, SourceGenerationContext.Default.GitHubLatestReleaseResponse, cancellationToken: cancellationTokenSource.Token).ConfigureAwait(false);
        if (latestResponse is null)
        {
            return;
        }

        static bool IsValidTag(ReadOnlySpan<char> tag, out int major, out int minor, out int build)
        {
            major = minor = build = 0;
            if (tag.IsEmpty || tag[0] != 'v')
            {
                return false;
            }

            tag = tag.Slice(1);
            var firstDot = tag.IndexOf('.');
            if (firstDot <= 0)
            {
                return false;
            }

            if (!int.TryParse(tag.Slice(0, firstDot), out major))
            {
                return false;
            }

            tag = tag.Slice(firstDot + 1);
            var secondDot = tag.IndexOf('.');
            if (secondDot <= 0)
            {
                return false;
            }

            return int.TryParse(tag.Slice(0, secondDot), out minor) && int.TryParse(tag.Slice(secondDot + 1), out build);
        }
        if (!IsValidTag(latestResponse.tag_name.AsSpan(), out var major, out var minor, out var build))
        {
            return;
        }

        if (major < version.Major)
        {
            return;
        }
        else if (major == version.Major)
        {
            if (minor < version.Minor)
            {
                return;
            }
            else if (minor == version.Minor)
            {
                if (build <= version.Build)
                {
                    return;
                }
            }
        }

        var currentTag = $"v{version.Major}.{version.Minor}.{version.Build}";
        var fileName = Path.GetFileName(processPath);
        foreach (var asset in latestResponse.assets)
        {
            if (asset.name == fileName)
            {
                await LoadNewFileAsync(processPath, processDirPath, newPath, client, asset, cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }
    }

    private static async ValueTask LoadNewFileAsync(string processPath, string processDirPath, string newPath, HttpClient client, GitHubLatestReleaseResponseAsset asset, CancellationToken token)
    {
        const string batFileName = "WahrenUpdatePatch.bat";

        using var getMessage = new HttpRequestMessage(HttpMethod.Get, asset.url);
        getMessage.Headers.Accept.Add(new("application/octet-stream"));
        getMessage.Headers.UserAgent.Add(new("Wahren", null));
        using var responseMessage = await client.SendAsync(getMessage, token).ConfigureAwait(false);
        if ((int)responseMessage.StatusCode != 200)
        {
            return;
        }

        var contents = await responseMessage.Content.ReadAsByteArrayAsync(token).ConfigureAwait(false);
        File.WriteAllBytes(newPath, contents);
        Console.WriteLine($"Update Exists: {newPath}");
        var builder = new StringBuilder();
        builder.AppendLine("timeout /t 5 /nobreak > nul");
        builder.Append("del ").Append(processPath).AppendLine(" > nul");
        builder.AppendLine($"del \"{batFileName}\" > nul");
        builder.AppendLine("exit");
        File.WriteAllText(Path.Combine(processDirPath, batFileName), builder.ToString());
        using Process process = new();
        process.StartInfo.FileName = batFileName;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        process.Close();
    }
}

#pragma warning disable IDE1006
internal class GitHubLatestReleaseResponse
{
    public string tag_name { get; set; } = "";
    public GitHubLatestReleaseResponseAsset[] assets { get; set; } = Array.Empty<GitHubLatestReleaseResponseAsset>();
}

internal class GitHubLatestReleaseResponseAsset
{
    public string name { get; set; } = "";
    public string url { get; set; } = "";
    public int size { get; set; }
}
#pragma warning restore IDE1006
