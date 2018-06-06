using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Wahren
{
    using Specific;
    static class Program
    {
        static readonly CommandLineApplication app;
        static Program()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            app = new CommandLineApplication();
            app.Name = nameof(Wahren);
            {
                var folderArgument = app.Argument("folder", "", false);
                app.OnExecute(() =>
                {
                    ScriptLoader.InitializeComponent(folderArgument.Value);
                    IEnumerable<string> GetOnly(int index)
                    {
                        return ScriptLoader.Scenarios[index].Variable_Get.Except(ScriptLoader.Scenarios[index].Variable_Set);
                    }
                    IEnumerable<string> SetOnly(int index)
                    {
                        return ScriptLoader.Scenarios[index].Variable_Set.Except(ScriptLoader.Scenarios[index].Variable_Get);
                    }
                    (HashSet<string> GetOnly, HashSet<string> SetOnly) Write(int i)
                    {
                        if (ScriptLoader.Scenarios[i].Variable_Get.Intersect(ScriptLoader.Scenarios[i].Variable_Set).Count() == ScriptLoader.Scenarios[i].Variable_Get.Count)
                            return default;
                        var buf = new StringBuilder()
                        .Append(ScriptLoader.Scenarios[i].Variable_Get.Count)
                        .Append("...")
                        .Append(ScriptLoader.Scenarios[i].Name)
                        .Append("...")
                        .Append(ScriptLoader.Scenarios[i].Variable_Set.Count)
                        .Append('\n');
                        HashSet<string> go, so;
                        foreach (var item in (go = new HashSet<string>(GetOnly(i))))
                            buf.Append("Only Get: ").AppendLine(item);
                        foreach (var item in (so = new HashSet<string>(SetOnly(i))))
                            buf.Append("Only Set: ").AppendLine(item);
                        System.Console.WriteLine(buf.ToString());
                        return (go, so);
                    }
                    HashSet<string> getonly = null, setonly = null;
                    int j = 0;
                    for (j = 0; j < ScriptLoader.Scenarios.Length; j++)
                    {
                        (getonly, setonly) = Write(j);
                        if (getonly != null)
                        {
                            ++j;
                            break;
                        }
                    }
                    for (; j < ScriptLoader.Scenarios.Length; j++)
                    {
                        var (_g, _s) = Write(j);
                        if (_g == null)
                            continue;
                        getonly.IntersectWith(_g);
                        setonly.IntersectWith(_s);
                    }
                    foreach (var item in getonly)
                        System.Console.Error.WriteLine("Get Only:" + item);
                    foreach (var item in setonly)
                        System.Console.Error.WriteLine("Set Only:" + item);
                    return 0;
                });
            }
        }
        static void Main(string[] args)
        {
            try
            {
                app.Execute(args);
            }
            catch (AggregateException e)
            {
                Console.Error.WriteLine(e.InnerException.ToString());
                return;
            }
        }
    }
}