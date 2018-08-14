using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wahren.Analysis.Unity;
using Wahren.Analysis.Specific;

using static Farmhash.Sharp.Farmhash;

namespace Wahren.Analysis
{
    static class Program
    {
        static readonly CommandLineApplication app;
        static Program()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            app = new CommandLineApplication();
            app.Name = nameof(Wahren);
            app.Command("decode", (decode) =>
            {
                var folderArgument = decode.Argument("folder", "", false);
                decode.OnExecute(() =>
                {
                    var sc = new ScenarioFolder(folderArgument.Value);
                    var facePath = Path.Combine(sc.FullName, "face");
                    var imagePath = Path.Combine(sc.FullName, "image");
                    byte[] tmp;
                    void Decode(List<string> list, string appendix, string path)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            FileInfo fileInfo = new FileInfo(Path.Combine(path, list[i] + appendix));
                            using (var input = fileInfo.OpenRead())
                            {
                                var _ = new byte[input.Length];
                                input.Read(_);
                                PicEncoder.ImageChg.Decode(_, out tmp);
                            }
                            using (var output = fileInfo.OpenWrite())
                                output.Write(tmp);
                        }
                    }
                    Decode(sc.Image_Png, ".png", imagePath);
                    Decode(sc.Face_Png, ".png", facePath);
                    Decode(sc.Image_Bmp, ".bmp", imagePath);
                    Decode(sc.Face_Bmp, ".bmp", facePath);
                    Decode(sc.Image_Jpg, ".jpg", imagePath);
                    return 0;
                });
            });
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
                                }
                            }
                        }
                    }
                    void AppendChip(string f, List<string> bmps, List<string> jpgs, List<string> pngs)
                    {
                        var chipDir = Path.Combine(destinationFolder, f);
                        var src = Path.Combine(sc.FullName, f);
                        for (int i = 0; i < bmps.Count; i++)
                            using (var input = Image.Load(Path.Combine(src, bmps[i] + ".bmp")))
                                input.Save(Path.Combine(chipDir, bmps[i] + ".png"));
                        for (int i = 0; i < jpgs.Count; i++)
                            using (var input = Image.Load(Path.Combine(src, jpgs[i] + ".jpg")))
                                input.Save(Path.Combine(chipDir, jpgs[i] + ".png"));
                        for (int i = 0; i < pngs.Count; i++)
                            File.Copy(Path.Combine(src, pngs[i] + ".png"), Path.Combine(chipDir, pngs[i] + ".png"), true);
                    }
                    Directory.CreateDirectory(Path.Combine(destinationFolder, "chip"));
                    Directory.CreateDirectory(Path.Combine(destinationFolder, "chip2"));
                    Directory.CreateDirectory(Path.Combine(destinationFolder, "Face"));
                    Directory.CreateDirectory(Path.Combine(destinationFolder, "Flag"));
                    Rgb24 faceTrans;
                    using (var faceInput = Image.Load(Path.Combine(sc.FullName, "Face/noface.png")))
                        faceTrans = faceInput[0, 0].Rgb;
                    void AddFlag(string flag, string append)
                    {
                        using (var input = Image.Load(Path.Combine(sc.FullName, "Flag/" + flag + append)))
                        using (var out0 = new Image<Rgba32>(32, 32))
                        using (var out1 = new Image<Rgba32>(32, 32))
                        {
                            for (int j = 0; j < 64; j++)
                                for (int k = 0; k < 32; k++)
                                    if (j < 32)
                                        out0[j, k] = input[j, k].Equals(Rgba32.Black) ? Rgba32.Transparent : input[j, k];
                                    else
                                        out1[j - 32, k] = input[j, k].Equals(Rgba32.Black) ? Rgba32.Transparent : input[j, k];
                            out0.Save(Path.Combine(destinationFolder, "Flag/" + flag + "_0.png"));
                            out1.Save(Path.Combine(destinationFolder, "Flag/" + flag + "_1.png"));
                        }
                    }
                    void AddFace(string face)
                    {
                        using (var input = Image.Load(Path.Combine(sc.FullName, "Face/" + face + ".png")))
                        {
                            for (int j = 0; j < input.Width; j++)
                                for (int k = 0; k < input.Height; k++)
                                    if (input[j, k].Rgb.Equals(faceTrans))
                                        input[j, k] = Rgba32.Transparent;
                            input.Save(Path.Combine(destinationFolder, "Face/" + face + ".png"));
                        }
                    }
                    for (int i = 0; i < sc.Flag_Bmp.Count; i++)
                        AddFlag(sc.Flag_Bmp[i], ".bmp");
                    for (int i = 0; i < sc.Flag_Png.Count; i++)
                        AddFlag(sc.Flag_Bmp[i], ".png");
                    for (int i = 0; i < sc.Face_Png.Count; i++)
                        AddFace(sc.Face_Png[i]);
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
            app.Command("map", (map) =>
            {
                var folderArgument = map.Argument("folder", "", false);
                map.OnExecute(() =>
                {
                    ScriptLoader.InitializeComponent(folderArgument.Value);
                    if (Map.MapHelper.TryLoad(Path.Combine(Path.Combine(folderArgument.Value, "stage"), "s25_kazeto.map"), out var w, out var h, out var chips))
                    {
                        // System.Console.WriteLine("s25_kazeto");
                        // Console.Write("Width:");
                        // Console.WriteLine(w);
                        // Console.Write("Height:");
                        // Console.WriteLine(h);
                        MoveTypeData moveType = ScriptLoader.MoveTypeDictionary.Values.Last();
                        var array = Map.MapHelper.DistanceList(Map.MapHelper.ConvertToFieldAttributeAndHeightArray(chips), w, moveType, out var valid);
                        // for (int i = 0; i < array.Length; i++)
                        // {
                        //     Console.Write("(");
                        //     Console.Write(array[i].Item1);
                        //     Console.Write(":");
                        //     Console.Write(array[i].Item2);
                        //     Console.Write(":");
                        //     Console.Write(array[i].Item3);
                        //     Console.WriteLine("), ");
                        // }
                    }
                    return 0;
                });
            });
            app.Command("parse", (parse) =>
            {
                var folderArgument = parse.Argument("folder", "", false);
                var getOnlyOption = parse.Option("--getOnly", "", CommandOptionType.NoValue);
                var setOnlyOption = parse.Option("--setOnly", "", CommandOptionType.NoValue);
                parse.OnExecute(() =>
                {
                    var isGetOnly = getOnlyOption.HasValue();
                    var isSetOnly = setOnlyOption.HasValue();
                    ScriptLoader.InitializeComponent(folderArgument.Value);
                    // FileCreator.StaticData();
                    // Console.WriteLine(FileCreator.Start);
                    // Console.WriteLine(FileCreator.ScenarioChoiceFunction());
                    // Console.WriteLine(ScriptLoader.Context.TitleFunction());
                    // Console.WriteLine(FileCreator.End);
                    // for (int i = 0; i < ScriptLoader.Scenarios.Length; i++)
                    // {
                    //     foreach (var spot in ScriptLoader.Scenarios[i].Spot.Values)
                    //         System.Console.WriteLine(FileCreator.SpotCreator(spot));
                    // }
                    #region GS
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
                    #endregion
                    return 0;
                });
            });
            app.Command("variable", (variable) =>
            {
                var folderArgument = variable.Argument("folder", "", false);
                var output = variable.Argument("output", "", false);
                variable.OnExecute(() =>
                {
                    ScriptLoader.InitializeComponent(folderArgument.Value);
                    var scenarios = ScriptLoader.Scenarios;
                    var variables = new HashSet<string>();
                    var identifiers = new HashSet<string>();
                    for (int i = 0; i < scenarios.Length; i++)
                    {
                        ref var sc = ref scenarios[i];
                        foreach (var item in sc.Variable_Get)
                            variables.Add(item);
                        foreach (var item in sc.Variable_Set)
                            variables.Add(item);
                        foreach (var item in sc.Identifier_Get)
                            identifiers.Add(item);
                        foreach (var item in sc.Identifier_Set)
                            identifiers.Add(item);
                    }
                    using (var sw = new StreamWriter(output.Value, append: false, Encoding.UTF8))
                    {
                        sw.Write("using System;\nusing System.Collections.Generic;\n\nnamespace Wahren.Analysis\n{\n\tpublic static class Variables\n\t{\n");
                        const string Value = "\t\tpublic static void Clear()\n\t\t{\n";
                        var buf = new StringBuilder(64 * variables.Count).Append(Value);
                        var reflection = new StringBuilder(32 * variables.Count).Append("\t\tpublic static ref List<ulong> GetRef(ulong name)\n\t\t{\n\t\t\tswitch(name)\n\t\t\t{\n");
                        foreach (var item in variables)
                        {
                            var _ = item.Substring(1);
                            sw.Write("\t\tpublic static List<ulong> ");
                            sw.Write(_);
                            sw.Write(";\n");
                            buf.Append("\t\t\tif(")
                            .Append(_)
                            .Append(" != null)")
                            .Append(_)
                            .Append(".Clear();\n");
                            reflection.Append("\t\t\t\tcase ").Append(Hash64(_)).Append("ul:\n\t\t\t\t\treturn ref ").Append(_).Append(";\n");
                        }
                        sw.Write(buf.Append("\t\t}\n").ToString());
                        sw.Write(reflection.Append("\t\t\t\tdefault: throw new ArgumentException(name.ToString());\n\t\t\t}\n\t\t}\n").ToString());
                        sw.Write("\t}\n\tpublic static class Identifiers\n\t{\n");
                        var save = new StringBuilder(buf.Capacity).Append("\t\tpublic static byte[] Save()\n\t\t{\n\t\t\tvar answer = new byte[8 * ").Append(identifiers.Count).Append("];\n");
                        var load = new StringBuilder(buf.Capacity).Append("\t\tpublic static void Load(byte[] input)\n\t\t{\n");
                        buf.Clear().Append(Value);
                        reflection.Clear().Append("\t\tpublic static ref long GetRef(ulong name)\n\t\t{\n\t\t\tswitch(name)\n\t\t\t{\n");
                        var i = 0;
                        var j = 0;
                        foreach (var item in identifiers)
                        {
                            sw.Write("\t\tpublic static long ");
                            sw.Write(item);
                            sw.Write(";\n");
                            buf.Append("\t\t\t")
                            .Append(item)
                            .Append(" = 0;\n");
                            reflection.Append("\t\t\t\tcase ").Append(Hash64(item)).Append("ul:\n\t\t\t\t\treturn ref ").Append(item).Append(";\n");
                            save
                            .Append("\t\t\tanswer[").Append(8 * i).Append("] = (byte)").Append(item).Append(";\n")
                            .Append("\t\t\tanswer[").Append(8 * i + 1).Append("] = (byte)((ulong)(").Append(item).Append(") >> 8);\n")
                            .Append("\t\t\tanswer[").Append(8 * i + 2).Append("] = (byte)((ulong)(").Append(item).Append(") >> 16);\n")
                            .Append("\t\t\tanswer[").Append(8 * i + 3).Append("] = (byte)((ulong)(").Append(item).Append(") >> 24);\n")
                            .Append("\t\t\tanswer[").Append(8 * i + 4).Append("] = (byte)((ulong)(").Append(item).Append(") >> 32);\n")
                            .Append("\t\t\tanswer[").Append(8 * i + 5).Append("] = (byte)((ulong)(").Append(item).Append(") >> 40);\n")
                            .Append("\t\t\tanswer[").Append(8 * i + 6).Append("] = (byte)((ulong)(").Append(item).Append(") >> 48);\n")
                            .Append("\t\t\tanswer[").Append(8 * i + 7).Append("] = (byte)((ulong)(").Append(item).Append(") >> 56);\n");
                            load.Append("\t\t\t").Append(item).Append(" = (long)((ulong)input[")
                            .Append(j++).Append("] + ((ulong)input[").Append(j++)
                            .Append("] << 8) + ((ulong)input[").Append(j++)
                            .Append("] << 16) + ((ulong)input[").Append(j++)
                            .Append("] << 24) + ((ulong)input[").Append(j++)
                            .Append("] << 32) + ((ulong)input[").Append(j++)
                            .Append("] << 40) + ((ulong)input[").Append(j++)
                            .Append("] << 48) + ((ulong)input[").Append(j++)
                            .Append("] << 56));\n");
                            ++i;
                        }
                        sw.Write(buf.Append("\t\t}\n").ToString());
                        sw.Write(save.Append("\t\t\treturn answer;\n\t\t}\n").ToString());
                        sw.Write(load.Append("\t\t}\n").ToString());
                        sw.Write(reflection.Append("\t\t\t\tdefault: throw new ArgumentException(name.ToString());\n\t\t\t}\n\t\t}\n").ToString());
                        sw.Write("\t}\n}");
                    }
                    return 0;
                });
            });
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