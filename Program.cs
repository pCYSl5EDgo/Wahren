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
                var metaOption = image.Option("-m|--meta", "generate meta files", CommandOptionType.NoValue);
                image.OnExecute(() =>
                {
                    var isMeta = metaOption.HasValue();
                    var sc = new ScenarioFolder(folderArgument.Value);
                    var destinationFolder = destArgument.Value;
                    if (Directory.Exists(destinationFolder))
                        Directory.Delete(destinationFolder, true);
                    Directory.CreateDirectory(destinationFolder);
                    var imageDirectory = Path.Combine(destinationFolder, "image");
                    Directory.CreateDirectory(imageDirectory);
                    void ProcessChip(string f, (byte R, byte G, byte B, byte A) transparent, string imagedata, Dictionary<string, (int left, int top, int right, int bottom)> dictionary)
                    {
                        var chipDir = Path.Combine(destinationFolder, f);
                        var transColor = new Rgb24(transparent.R, transparent.G, transparent.B);
                        using (var input = Image.Load(imagedata))
                        {
                            foreach (var (name, (left, top, right, bottom)) in dictionary)
                            {
                                using (var destination = new Image<Rgba32>(right - left, bottom - top))
                                {
                                    for (int i = 0; i < right - left; i++)
                                        for (int j = 0; j < bottom - top; j++)
                                        {
                                            var tmp = input[i + left, j + top];
                                            destination[i, j] = tmp.Rgb.Equals(transColor) ? Rgba32.Transparent : tmp;
                                        }
                                    var outputFilePath = Path.Combine(chipDir, name + ".png");
                                    destination.Save(outputFilePath);
                                    if (isMeta)
                                    {
                                    }
                                }
                            }
                        }
                    }
                    void AppendChip(string f, List<string> bmps, List<string> jpgs, List<string> pngs)
                    {
                        var chipDir = Path.Combine(destinationFolder, f);
                        for (int i = 0; i < bmps.Count; i++)
                        {
                            using (var input = Image.Load(bmps[i]))
                            {
                                input.Save(Path.Combine(chipDir, Path.GetFileNameWithoutExtension(bmps[i]).ToLower() + ".png"));
                            }
                        }
                        for (int i = 0; i < jpgs.Count; i++)
                            using (var input = Image.Load(jpgs[i]))
                            {
                                input.Save(Path.Combine(chipDir, Path.GetFileNameWithoutExtension(jpgs[i]).ToLower() + ".png"));
                            }
                        for (int i = 0; i < pngs.Count; i++)
                            File.Copy(pngs[i], Path.Combine(chipDir, Path.GetFileName(pngs[i]).ToLower()), true);
                    }
                    Directory.CreateDirectory(Path.Combine(destinationFolder, "chip"));
                    Directory.CreateDirectory(Path.Combine(destinationFolder, "chip2"));
                    if (sc.ImageData1Dictionary.Count != 0)
                        ProcessChip("chip", sc.ImageData1TransparentColor, sc.ImageData1, sc.ImageData1Dictionary);
                    if (sc.ImageData2Dictionary.Count != 0)
                        ProcessChip("chip2", sc.ImageData2TransparentColor, sc.ImageData2, sc.ImageData2Dictionary);
                    AppendChip("chip", sc.Chip_Bmp, sc.Chip_Jpg, sc.Chip_Png);
                    AppendChip("chip2", sc.Chip2_Bmp, sc.Chip2_Jpg, sc.Chip2_Png);
                    #region ウィンドウスキンやタイトル画面関連の画像処理
                    {
                        Span<char> span = stackalloc char[8];
                        "wnd0.png".AsSpan().CopyTo(span);
                        var inputDirectory = Path.Combine(folderArgument.Value, "image");
                        for (int i = 0; i < 5; i++)
                        {
                            span[3] = (char)('0' + i);
                            using (var img = Image.Load(Path.Combine(inputDirectory, span.ToString())))
                            using (var destimg = new Image<Rgba32>(64, 64))
                            {
                                for (int j = 0; j < 64; j++)
                                    for (int k = 0; k < 64; k++)
                                        destimg[k, j] = img[k, j];
                                for (int j = 0; j < 16; j++)
                                    for (int k = 64; k < 128; k++)
                                        if (img[k, j].A != 0)
                                            destimg[k - 64, j] = img[k, j];
                                for (int j = 16; j < 48; j++)
                                {
                                    for (int k = 64; k < 80; k++)
                                        if (img[k, j].A != 0)
                                            destimg[k - 64, j] = img[k, j];
                                    for (int k = 112; k < 128; k++)
                                        if (img[k, j].A != 0)
                                            destimg[k - 64, j] = img[k, j];
                                }
                                for (int j = 48; j < 64; j++)
                                    for (int k = 64; k < 128; k++)
                                        if (img[k, j].A != 0)
                                            destimg[k - 64, j] = img[k, j];
                                destimg.Save(Path.Combine(imageDirectory, span.ToString()));
                            }
                        }
                        var titleImages = new string[] {
                            "easy.png",
                            "normal.png",
                            "hard.png",
                            "luna.png",
                            "continue.png",
                            "tool.png",
                            "wnd5.png",
                        };
                        for (int i = 0; i < titleImages.Length; i++)
                        {
                            using (var img = Image.Load(Path.Combine(inputDirectory, titleImages[i])))
                            using (var destimg = new Image<Rgba32>(img.Width, img.Height))
                            {
                                var transparent = img[0, 0];
                                for (int j = 0; j < destimg.Height; j++)
                                    for (int k = 0; k < destimg.Width; k++)
                                        destimg[k, j] = img[k, j].Equals(transparent) ? Rgba32.Transparent : img[k, j];
                                destimg.Save(Path.Combine(imageDirectory, titleImages[i]));
                            }
                        }
                    }
                    #endregion
                    return 0;
                });
            });
            {
                var folderArgument = app.Argument("folder", "", false);
                var getOnlyOption = app.Option("--getOnly", "", CommandOptionType.NoValue);
                var setOnlyOption = app.Option("--setOnly", "", CommandOptionType.NoValue);
                app.OnExecute(() =>
                {
                    var isGetOnly = getOnlyOption.HasValue();
                    var isSetOnly = setOnlyOption.HasValue();
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
                        HashSet<string> go = new HashSet<string>(GetOnly(i)), so = new HashSet<string>(SetOnly(i));
                        if (isGetOnly)
                        {
                            foreach (var item in go)
                                buf.Append("Only Get: ").AppendLine(item);
                        }
                        if (isSetOnly)
                        {
                            foreach (var item in so)
                                buf.Append("Only Set: ").AppendLine(item);
                        }
                        System.Console.WriteLine(buf.ToString());
                        return (go, so);
                    }
                    if (isGetOnly || isSetOnly)
                    {
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
                        if (isGetOnly)
                        {
                            foreach (var item in getonly)
                                System.Console.Error.WriteLine("Get Only:" + item);
                        }
                        if (isSetOnly)
                        {
                            foreach (var item in setonly)
                                System.Console.Error.WriteLine("Set Only:" + item);
                        }
                    }
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