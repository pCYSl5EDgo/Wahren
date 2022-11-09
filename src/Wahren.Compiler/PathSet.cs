namespace Wahren.Compiler;

internal class PathSet
{
    public readonly string Contents;
    public readonly string Script;
    public readonly string Chip;
    public readonly string Chip2;
    public readonly string Picture;
    public readonly string Image;
    public readonly string Face;
    public readonly string Image_dat;
    public readonly string Image2_dat;
    public readonly string Imagedata_dat;
    public readonly string Imagedata2_dat;

    public PathSet(string contents)
    {
        Contents = contents;
        Script = Path.Combine(Contents, "script");
        Chip = Path.Combine(Contents, "chip");
        Chip2 = Path.Combine(Contents, "chip2");
        Image = Path.Combine(Contents, "image");
        Image_dat = Path.Combine(Image, "image.dat");
        Image2_dat = Path.Combine(Image, "image2.dat");
        Imagedata_dat = Path.Combine(Image, "imagedata.dat");
        Imagedata2_dat = Path.Combine(Image, "imagedata2.dat");
        Picture = Path.Combine(Contents, "picture");
        Face = Path.Combine(Contents, "face");
    }

    public string[] GetScriptDatArray()
    {
        return Directory.GetFiles(Script, "*.dat", SearchOption.AllDirectories);
    }

    public TripleImageSet GetImages()
    {
        return new(Directory.GetFiles(Image, "*.png"), Directory.GetFiles(Image, "*.bmp"), Directory.GetFiles(Image, "*.jpg"));
    }

    public record struct TripleImageSet(string[] Png, string[] Bmp, string[] Jpg)
    {
        public Enumerator GetEnumerator() => new(ref this);

        public struct Enumerator
        {
            private readonly TripleImageSet set;

            private string[] array;
            private int index;

            public Enumerator(ref TripleImageSet set)
            {
                this.set = set;
                array = set.Png;
                index = -1;
            }

            public bool MoveNext()
            {
                if (++index >= array.Length)
                {
                    if (array == set.Jpg)
                    {
                        return false;
                    }
                    else if (array == set.Bmp)
                    {
                        array = set.Jpg;
                    }
                    else
                    {
                        array = set.Bmp;
                    }

                    index = 0;
                }

                return true;
            }

            public ReadOnlySpan<char> Current => Path.GetFileNameWithoutExtension(array[index].AsSpan());
        }
    }
}
