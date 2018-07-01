using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Wahren
{
    public class ScenarioFolder
    {
        public string Name { get; protected set; }
        private enum EncType : byte { UTF8, Unicode, Shift_JIS };
        private EncType encType = EncType.Shift_JIS;
        public bool IsShiftJis => EncType.Shift_JIS == encType;
        public bool IsUnicode => encType == EncType.Unicode;
        public bool IsUTF8 => EncType.UTF8 == encType;
        public bool IsEnglishMode { get; protected set; } = false;
        public bool IsDebug { get; set; } = false;
        private static readonly Encoding shift_jis_encoding = Encoding.GetEncoding("Shift_JIS");
        public Encoding Encoding
        {
            get
            {
                switch (encType)
                {
                    case EncType.Unicode:
                        return Encoding.Unicode;
                    case EncType.UTF8:
                        return Encoding.UTF8;
                    default:
                        return shift_jis_encoding;
                }
            }
        }
        public readonly HashSet<string> Bgm = new HashSet<string>();
        public List<string> Bgm_Mid { get; protected set; } = new List<string>();
        public List<string> Bgm_Mp3 { get; protected set; } = new List<string>();
        public List<string> Bgm_Ogg { get; protected set; } = new List<string>();
        public readonly HashSet<string> Chip = new HashSet<string>();
        public List<string> Chip_Png { get; protected set; } = new List<string>();
        public List<string> Chip_Jpg { get; protected set; } = new List<string>();
        public List<string> Chip_Bmp { get; protected set; } = new List<string>();
        public readonly HashSet<string> Chip2 = new HashSet<string>();
        public List<string> Chip2_Png { get; protected set; } = new List<string>();
        public List<string> Chip2_Jpg { get; protected set; } = new List<string>();
        public List<string> Chip2_Bmp { get; protected set; } = new List<string>();
        public readonly HashSet<string> Face = new HashSet<string>();
        public List<string> Face_Png { get; protected set; } = new List<string>();
        public List<string> Face_Jpg { get; protected set; } = new List<string>();
        public List<string> Face_Bmp { get; protected set; } = new List<string>();
        public readonly HashSet<string> Flag = new HashSet<string>();
        public List<string> Flag_Png { get; protected set; } = new List<string>();
        public List<string> Flag_Jpg { get; protected set; } = new List<string>();
        public List<string> Flag_Bmp { get; protected set; } = new List<string>();
        public readonly HashSet<string> Icon = new HashSet<string>();
        public List<string> Icon_Png { get; protected set; } = new List<string>();
        public List<string> Icon_Jpg { get; protected set; } = new List<string>();
        public List<string> Icon_Bmp { get; protected set; } = new List<string>();
        public readonly HashSet<string> Image = new HashSet<string>();
        public List<string> Image_Png { get; protected set; } = new List<string>();
        public List<string> Image_Jpg { get; protected set; } = new List<string>();
        public List<string> Image_Bmp { get; protected set; } = new List<string>();
        public readonly HashSet<string> Picture = new HashSet<string>();
        public List<string> Picture_Png { get; protected set; } = new List<string>();
        public List<string> Picture_Jpg { get; protected set; } = new List<string>();
        public List<string> Picture_Bmp { get; protected set; } = new List<string>();
        public List<string> Script_Dat { get; protected set; } = new List<string>();
        public List<string> Script_Language { get; protected set; } = new List<string>();
        public List<string> Sound_Wav { get; protected set; } = new List<string>();
        public List<string> Stage_Map { get; protected set; } = new List<string>();
        //image.dat, imagedata.dat
        public readonly string ImageData1;
        public readonly (byte R, byte G, byte B, byte A) ImageData1TransparentColor;
        public readonly Dictionary<string, (int left, int top, int right, int bottom)> ImageData1Dictionary = new Dictionary<string, (int left, int top, int right, int bottom)>();
        //image2.dat, imagedata2.dat
        public readonly string ImageData2;
        public readonly (byte R, byte G, byte B, byte A) ImageData2TransparentColor;
        public readonly Dictionary<string, (int left, int top, int right, int bottom)> ImageData2Dictionary = new Dictionary<string, (int left, int top, int right, int bottom)>();

        public ScenarioFolder(string folderPath, bool isDebug = false)
        {
            IsDebug = isDebug;
            Name = new DirectoryInfo(folderPath).Name;

            foreach (var folder in Directory.GetDirectories(folderPath))
            {
                void AddFilesWithoutExtensionLower(List<string> list, string pattern, HashSet<string> group = null)
                {
                    var collection = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories).Select(_ => string.Intern(Path.GetFileNameWithoutExtension(_).ToLower()));
                    if (group == null)
                        list.AddRange(collection);
                    else foreach (var item in collection)
                        {
                            list.Add(item);
                            group.Add(item);
                        }
                }
                void AddFilesWithoutExtension(List<string> list, string pattern, HashSet<string> group = null)
                {
                    var collection = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories).Select(_ => string.Intern(Path.GetFileNameWithoutExtension(_)));
                    if (group == null)
                        list.AddRange(collection);
                    else foreach (var item in collection)
                        {
                            list.Add(item);
                            group.Add(item);
                        }
                }
                switch (new DirectoryInfo(folder).Name)
                {
                    case "script":
                        Script_Dat.AddRange(Directory.GetFiles(folder, "*.dat", SearchOption.AllDirectories));
                        AddFilesWithoutExtension(Script_Language, "language*.txt");
                        //UTF8モード
                        var utf8FileInfo = new FileInfo(Path.Combine(folder, "utf8.txt"));
                        if (utf8FileInfo.Exists)
                        {
                            using (var sr = utf8FileInfo.OpenText())
                            {
                                encType = EncType.UTF8;
                                var line = sr.ReadLine();
                                if (!string.IsNullOrWhiteSpace(line) && line.Trim().ToLower() == "foreign")
                                    IsEnglishMode = true;
                            }
                            break;
                        }
                        //英文モード
                        var englishFileInfo = new FileInfo(Path.Combine(folder, "english.txt"));
                        if (englishFileInfo.Exists)
                        {
                            encType = EncType.Unicode;
                            IsEnglishMode = true;
                            break;
                        }
                        //ユニコード(UTF16)モード
                        var unicodeFileInfo = new FileInfo(Path.Combine(folder, "unicode.txt"));
                        if (!unicodeFileInfo.Exists) break;
                        encType = EncType.Unicode;
                        using (var sr = new StreamReader(unicodeFileInfo.OpenRead(), Encoding.Unicode))
                        {
                            var line = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(line) && line.Trim().ToLower() == "foreign")
                                IsEnglishMode = true;
                        }
                        break;
                    case "stage":
                        AddFilesWithoutExtension(Stage_Map, "*.map");
                        break;
                    case "bgm":
                        AddFilesWithoutExtension(Bgm_Mid, "*.mid", Bgm);
                        AddFilesWithoutExtension(Bgm_Mp3, "*.mp3", Bgm);
                        AddFilesWithoutExtension(Bgm_Ogg, "*.ogg", Bgm);
                        break;
                    case "sound":
                        AddFilesWithoutExtension(Sound_Wav, "*.wav");
                        break;
                    case "image":
                        AddFilesWithoutExtension(Image_Bmp, "*.bmp", Image);
                        AddFilesWithoutExtension(Image_Jpg, "*.jpg", Image);
                        AddFilesWithoutExtension(Image_Png, "*.png", Image);
                        (byte, byte, byte, byte) ReadImageData(string filePath, Dictionary<string, (int left, int top, int right, int bottom)> dictionary)
                        {
#if NETCOREAPP2_1
                            Span<byte> tmp = stackalloc byte[12];
#else
                            var tmp = new byte[12];
                            byte[] _tmpFile;
#endif
                            Span<byte> file;
                            if (!new FileInfo(filePath).Exists) return default;
                            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false))
                            {
#if NETCOREAPP2_1
                                fs.Read(tmp);
                                file = new byte[fs.Length - 12];
                                fs.Read(file);
                                tmp = tmp.Slice(8, 4);
#else
                                fs.Read(tmp, 0, tmp.Length);
                                _tmpFile = new byte[fs.Length - 12];
                                fs.Read(_tmpFile, 0, _tmpFile.Length);
                                file = _tmpFile;
#endif
                            }

                            int ReadInt32(Span<byte> input)
                            {
                                return input[0] + (input[1] << 8) + (input[2] << 16) + (input[3] << 24);
                            }
                            Span<byte> ___ = stackalloc byte[9];
                            for (int i = 0; i < 8; i++)
                                ___[i] = 0x5f;
                            ___[8] = 0;
                            ReadOnlySpan<byte> endOfFile = ___;
                            Span<char> nameSpan = stackalloc char[256];
                            int index;
                            while (true)
                            {
                                if (file.StartsWith(endOfFile))
                                    return (tmp[2], tmp[1], tmp[0], tmp[3]);
                                index = file.IndexOf<byte>(0);
                                if (index > nameSpan.Length)
                                    throw new IndexOutOfRangeException();
                                for (int i = 0; i < index; i++)
                                    nameSpan[i] = (char)file[i];
                                var rect = file.Slice(index + 1, 16);
#if NETCOREAPP2_1
                                dictionary[String.Intern(new string(nameSpan.Slice(0, index)))] = (ReadInt32(rect.Slice(0, 4)), ReadInt32(rect.Slice(4, 4)), ReadInt32(rect.Slice(8, 4)), ReadInt32(rect.Slice(12, 4)));
#else
                                dictionary[String.Intern(new string(lower.Slice(0, index).ToArray()))] = (ReadInt32(rect.Slice(0, 4)), ReadInt32(rect.Slice(4, 4)), ReadInt32(rect.Slice(8, 4)), ReadInt32(rect.Slice(12, 4)));
#endif
                                file = file.Slice(index + 17);
                            }
                        }
                        ImageData1 = Path.Combine(folder, "image.dat");
                        ImageData2 = Path.Combine(folder, "image2.dat");
                        ImageData1TransparentColor = ReadImageData(Path.Combine(folder, "imagedata.dat"), ImageData1Dictionary);
                        ImageData2TransparentColor = ReadImageData(Path.Combine(folder, "imagedata2.dat"), ImageData2Dictionary);
                        foreach (var key in ImageData1Dictionary.Keys)
                            Chip.Add(key);
                        foreach (var key in ImageData2Dictionary.Keys)
                            Chip2.Add(key);
                        break;
                    case "icon":
                        AddFilesWithoutExtension(Icon_Bmp, "*.bmp", Icon);
                        AddFilesWithoutExtension(Icon_Jpg, "*.jpg", Icon);
                        AddFilesWithoutExtension(Icon_Png, "*.png", Icon);
                        break;
                    case "flag":
                        AddFilesWithoutExtensionLower(Flag_Bmp, "*.bmp", Flag);
                        AddFilesWithoutExtensionLower(Flag_Jpg, "*.jpg", Flag);
                        AddFilesWithoutExtensionLower(Flag_Png, "*.png", Flag);
                        break;
                    case "face":
                        AddFilesWithoutExtension(Face_Bmp, "*.bmp", Face);
                        AddFilesWithoutExtension(Face_Jpg, "*.jpg", Face);
                        AddFilesWithoutExtension(Face_Png, "*.png", Face);
                        break;
                    case "picture":
                        AddFilesWithoutExtension(Picture_Bmp, "*.bmp", Picture);
                        AddFilesWithoutExtension(Picture_Jpg, "*.jpg", Picture);
                        AddFilesWithoutExtension(Picture_Png, "*.png", Picture);
                        break;
                    case "chip":
                        AddFilesWithoutExtension(Chip_Bmp, "*.bmp", Chip);
                        AddFilesWithoutExtension(Chip_Jpg, "*.jpg", Chip);
                        AddFilesWithoutExtension(Chip_Png, "*.png", Chip);
                        break;
                    case "chip2":
                        AddFilesWithoutExtension(Chip2_Bmp, "*.bmp", Chip2);
                        AddFilesWithoutExtension(Chip2_Jpg, "*.jpg", Chip2);
                        AddFilesWithoutExtension(Chip2_Png, "*.png", Chip2);
                        break;
                }
            }
            if (Script_Dat.Count == 0)
                throw new FileNotFoundException();
        }
    }
}