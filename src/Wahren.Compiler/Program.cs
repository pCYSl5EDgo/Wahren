﻿global using ConsoleAppFramework;
global using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

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
        var result = await task.ConfigureAwait(false);
        Console.WriteLine("Press Enter Key...");
        Console.ReadLine();
        return result;
    }

    [Command(new string[] {
        "help",
    })]
    public void Help()
    {
        Console.WriteLine("Use --help instead.\n\n");
        Console.WriteLine(@"Copyright 2017-2021 pCYSl5EDgo

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");
        Console.WriteLine("\n\n==== Dependency Licenses ====\n\n");
        Console.WriteLine(@"= CySharp/ConsoleAppFramework =
MIT License

Copyright (c) 2020 Cysharp, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.");
    }
}
