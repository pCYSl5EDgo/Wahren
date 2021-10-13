global using ConsoleAppFramework;
global using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace Wahren.Compiler;

public partial class Program : ConsoleAppBase
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args).ConfigureAwait(false);
    }


    public async ValueTask<int> Run()
    {
        var task = Analyze(".", false, PseudoDiagnosticSeverity.Error, true);
        try
        {
            await CheckUpdateAsync().ConfigureAwait(false);
        }
        catch
        {
        }
        var result = await task.ConfigureAwait(false);
        Console.WriteLine("Press Enter Key...");
        Console.ReadLine();
        return result;
    }

    private static async ValueTask CheckUpdateAsync()
    {
        var processPath = Environment.ProcessPath;
        if (processPath is null)
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

        var currentTag = $"v{version.Major}.{version.Minor}.{version.Build}";

        using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationTokenSource.Token).ConfigureAwait(false);
        var latestResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<GitHubLatestReleaseResponse>(stream, cancellationToken: cancellationTokenSource.Token).ConfigureAwait(false);
        if (latestResponse is null || latestResponse.tag_name == currentTag)
        {
            return;
        }

        var fileName = Path.GetFileName(processPath);
        foreach (var asset in latestResponse.assets)
        {
            if (asset.name == fileName)
            {
                await LoadNewFileAsync(processPath, client, asset, cancellationTokenSource.Token).ConfigureAwait(false);
                break;
            }
        }
    }

    private static async ValueTask LoadNewFileAsync(string processPath, HttpClient client, GitHubLatestReleaseResponseAsset asset, CancellationToken token)
    {
        using var getMessage = new HttpRequestMessage(HttpMethod.Get, asset.url);
        getMessage.Headers.Accept.Add(new("application/octet-stream"));
        getMessage.Headers.UserAgent.Add(new("Wahren", null));
        using var responseMessage = await client.SendAsync(getMessage, token).ConfigureAwait(false);
        if ((int)responseMessage.StatusCode != 200)
        {
            return;
        }

        var contents = await responseMessage.Content.ReadAsByteArrayAsync(token).ConfigureAwait(false);
        File.WriteAllBytes(processPath, contents);
        Console.WriteLine($"Update: {processPath}");
    }
}

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
