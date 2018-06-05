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
        public Tuple<string, string> ImageData1 { get; protected set; }
        //image2.dat, imagedata2.dat
        public Tuple<string, string> ImageData2 { get; protected set; }

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