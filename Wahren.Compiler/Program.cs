global using System.Threading.Tasks;
global using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;

namespace Wahren.Compiler;

public partial class Program : ConsoleAppBase
{
    public async static Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
    }    
}
