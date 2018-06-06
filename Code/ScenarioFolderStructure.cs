using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        private static readonly System.Text.Encoding shift_jis_encoding = System.Text.Encoding.GetEncoding("Shift_JIS");
        public System.Text.Encoding Encoding
        {
            get
            {
                switch (encType)
                {
                    case EncType.Unicode:
                        return System.Text.Encoding.Unicode;
                    case EncType.UTF8:
                        return System.Text.Encoding.UTF8;
                    default:
                        return shift_jis_encoding;
                }
            }
        }
        public List<string> Bgm_Mid { get; protected set; } = new List<string>();
        public List<string> Bgm_Mp3 { get; protected set; } = new List<string>();
        public List<string> Bgm_Ogg { get; protected set; } = new List<string>();
        public List<string> Chip_Png { get; protected set; } = new List<string>();
        public List<string> Chip_Jpg { get; protected set; } = new List<string>();
        public List<string> Chip_Bmp { get; protected set; } = new List<string>();
        public List<string> Chip2_Png { get; protected set; } = new List<string>();
        public List<string> Chip2_Jpg { get; protected set; } = new List<string>();
        public List<string> Chip2_Bmp { get; protected set; } = new List<string>();
        public List<string> Face_Png { get; protected set; } = new List<string>();
        public List<string> Face_Jpg { get; protected set; } = new List<string>();
        public List<string> Face_Bmp { get; protected set; } = new List<string>();
        public List<string> Flag_Png { get; protected set; } = new List<string>();
        public List<string> Flag_Jpg { get; protected set; } = new List<string>();
        public List<string> Flag_Bmp { get; protected set; } = new List<string>();
        public List<string> Icon_Png { get; protected set; } = new List<string>();
        public List<string> Icon_Jpg { get; protected set; } = new List<string>();
        public List<string> Icon_Bmp { get; protected set; } = new List<string>();
        public List<string> Image_Png { get; protected set; } = new List<string>();
        public List<string> Image_Jpg { get; protected set; } = new List<string>();
        public List<string> Image_Bmp { get; protected set; } = new List<string>();
        public List<string> Picture_Png { get; protected set; } = new List<string>();
        public List<string> Picture_Jpg { get; protected set; } = new List<string>();
        public List<string> Picture_Bmp { get; protected set; } = new List<string>();
        public List<string> Script_Dat { get; protected set; } = new List<string>();
        public List<string> Script_Language { get; protected set; } = new List<string>();
        public List<string> Sound_Wav { get; protected set; } = new List<string>();
        public List<string> Stage_Map { get; protected set; } = new List<string>();
        //image.dat, imagedata.dat
        public readonly (byte R, byte G, byte B, byte A) ImageData1TransparentColor;
        public readonly Dictionary<string, (int left, int top, int right, int bottom)> ImageData1 = new Dictionary<string, (int left, int top, int right, int bottom)>();
        //image2.dat, imagedata2.dat
        public readonly (byte R, byte G, byte B, byte A) ImageData2TransparentColor;
        public readonly Dictionary<string, (int left, int top, int right, int bottom)> ImageData2 = new Dictionary<string, (int left, int top, int right, int bottom)>();

        public ScenarioFolder(string folderPath, bool isDebug = false)
        {
            IsDebug = isDebug;
            Name = new DirectoryInfo(folderPath).Name;
            foreach (var folder in Directory.GetDirectories(folderPath))
            {
                switch (new DirectoryInfo(folder).Name.ToLower())
                {
                    case "script":
                        Script_Dat.AddRange(Directory.GetFiles(folder, "*.dat", SearchOption.AllDirectories).Select(String.Intern));
                        Script_Language.AddRange(Directory.GetFiles(folder, "language*.txt"));
                        //UTF8モード
                        var utf8FileInfo = new FileInfo(Path.Combine(folder, "utf8.txt"));
                        if (utf8FileInfo.Exists)
                            using (var sr = utf8FileInfo.OpenText())
                            {
                                encType = EncType.UTF8;
                                var line = sr.ReadLine();
                                if (!string.IsNullOrWhiteSpace(line) && line.Trim().ToLower() == "foreign")
                                    IsEnglishMode = true;
                            }
                        else
                        {
                            //英文モード
                            var englishFileInfo = new FileInfo(Path.Combine(folder, "english.txt"));
                            if (englishFileInfo.Exists)
                            {
                                encType = EncType.Unicode;
                                IsEnglishMode = true;
                            }
                            else
                            {
                                //ユニコードモード
                                var unicodeFileInfo = new FileInfo(Path.Combine(folder, "unicode.txt"));
                                if (unicodeFileInfo.Exists)
                                {
                                    encType = EncType.Unicode;
                                    using (var sr = new StreamReader(unicodeFileInfo.OpenRead(), System.Text.Encoding.Unicode))
                                    {
                                        var line = sr.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(line) && line.Trim().ToLower() == "foreign")
                                            IsEnglishMode = true;
                                    }
                                }
                            }
                        }
                        break;
                    case "stage":
                        Stage_Map.AddRange(Directory.GetFiles(folder, "*.map"));
                        break;
                    case "bgm":
                        Bgm_Mid.AddRange(Directory.GetFiles(folder, "*.mid", SearchOption.AllDirectories));
                        Bgm_Mp3.AddRange(Directory.GetFiles(folder, "*.mp3", SearchOption.AllDirectories));
                        Bgm_Ogg.AddRange(Directory.GetFiles(folder, "*.ogg", SearchOption.AllDirectories));
                        break;
                    case "sound":
                        Sound_Wav.AddRange(Directory.GetFiles(folder, "*.wav", SearchOption.AllDirectories));
                        break;
                    case "image":
                        Image_Bmp.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.AllDirectories));
                        Image_Jpg.AddRange(Directory.GetFiles(folder, "*.jpg", SearchOption.AllDirectories));
                        Image_Png.AddRange(Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories));
                        (byte, byte, byte, byte) ReadImageData(string filePath, Dictionary<string, (int left, int top, int right, int bottom)> dictionary)
                        {
                            Span<byte> tmp = stackalloc byte[12];
                            Span<byte> file;
                            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false))
                            {
                                fs.Read(tmp);
                                file = new byte[fs.Length - 12];
                                fs.Read(file);
                                tmp = tmp.Slice(8, 4);
                            }
                            int ReadInt32(Span<byte> input)
                            {
                                return input[0] + (input[1] << 8) + (input[2] << 16) + (input[3] << 24);
                            }
                            ReadOnlySpan<byte> endOfFile = stackalloc byte[] { 0x5f, 0x5f, 0x5f, 0x5f, 0x5f, 0x5f, 0x5f, 0x5f, 0x00 };
                            Span<char> lower = stackalloc char[256];
                            int index;
                            while (true)
                            {
                                if (file.StartsWith(endOfFile))
                                    return (tmp[0], tmp[1], tmp[2], tmp[3]);
                                index = file.IndexOf<byte>(0);
                                if (index > lower.Length)
                                    throw new IndexOutOfRangeException();
                                for (int i = 0; i < index; i++)
                                    lower[i] = (char)(file[i] >= 0x41 && file[i] <= 0x5a ? (file[i] + 0x20) : file[i]);
                                var rect = file.Slice(index + 1, 16);
                                dictionary[String.Intern(new string(lower.Slice(0, index)))] = (ReadInt32(rect.Slice(0, 4)), ReadInt32(rect.Slice(4, 4)), ReadInt32(rect.Slice(8, 4)), ReadInt32(rect.Slice(12, 4)));
                                file = file.Slice(index + 17);
                            }
                        }
                        ImageData1TransparentColor = ReadImageData(Path.Combine(folder, "imagedata.dat"), ImageData1);
                        ImageData2TransparentColor = ReadImageData(Path.Combine(folder, "imagedata2.dat"), ImageData2);
                        break;
                    case "icon":
                        Icon_Bmp.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.AllDirectories));
                        Icon_Jpg.AddRange(Directory.GetFiles(folder, "*.jpg", SearchOption.AllDirectories));
                        Icon_Png.AddRange(Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories));
                        break;
                    case "flag":
                        Flag_Bmp.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.AllDirectories));
                        Flag_Jpg.AddRange(Directory.GetFiles(folder, "*.jpg", SearchOption.AllDirectories));
                        Flag_Png.AddRange(Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories));
                        break;
                    case "face":
                        Face_Bmp.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.AllDirectories));
                        Face_Jpg.AddRange(Directory.GetFiles(folder, "*.jpg", SearchOption.AllDirectories));
                        Face_Png.AddRange(Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories));
                        break;
                    case "picture":
                        Picture_Bmp.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.AllDirectories));
                        Picture_Jpg.AddRange(Directory.GetFiles(folder, "*.jpg", SearchOption.AllDirectories));
                        Picture_Png.AddRange(Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories));
                        break;
                    case "chip":
                        Chip_Bmp.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.AllDirectories));
                        Chip_Jpg.AddRange(Directory.GetFiles(folder, "*.jpg", SearchOption.AllDirectories));
                        Chip_Png.AddRange(Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories));
                        break;
                    case "chip2":
                        Chip2_Bmp.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.AllDirectories));
                        Chip2_Jpg.AddRange(Directory.GetFiles(folder, "*.jpg", SearchOption.AllDirectories));
                        Chip2_Png.AddRange(Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories));
                        break;
                }
            }
        }
    }
}