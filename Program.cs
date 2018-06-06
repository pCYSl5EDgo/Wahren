using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
            app.Command("image", (image) =>
            {
                var folderArgument = image.Argument("folder", "", false);
                var destArgument = image.Argument("destination", "", false);
                image.OnExecute(() =>
                {
                    var sc = new ScenarioFolder(folderArgument.Value);
                    var dest = destArgument.Value;
                    if (Directory.Exists(dest))
                        Directory.Delete(dest, true);
                    Directory.CreateDirectory(dest);
                    void ProcessChip(string f, (byte R, byte G, byte B, byte A) transparent, string imagedata, Dictionary<string, (int left, int top, int right, int bottom)> dictionary)
                    {
                        var chipDir = Path.Combine(dest, f);
                        Directory.CreateDirectory(chipDir);
                        var trans = new Rgba32(transparent.R, transparent.G, transparent.B, transparent.A);
                        using (var input = Image.Load(imagedata))
                        {
                            foreach (var (name, (left, top, right, bottom)) in dictionary)
                            {
                                using (var destination = new Image<Rgba32>(right - left, bottom - top))
                                {
                                    for (int i = 0; i < right - left; i++)
                                        for (int j = 0; j < bottom - top; j++)
                                            destination[i, j] = input[i + left, j + top].Equals(trans) ? Rgba32.Transparent : input[i + left, j + top];
                                    destination.Save(Path.Combine(chipDir, name + ".png"));
                                }
                            }
                        }
                    }
                    ProcessChip("chip", sc.ImageData1TransparentColor, sc.ImageData1, sc.ImageData1Dictionary);
                    ProcessChip("chip2", sc.ImageData2TransparentColor, sc.ImageData2, sc.ImageData2Dictionary);
                    return 0;
                });
            });
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