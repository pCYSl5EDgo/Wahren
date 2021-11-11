using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Wahren.ArrayPoolCollections;

namespace Wahren.Imagedata;

public sealed class Imagedata : IDisposable
{
    public record struct Rect(uint X, uint Y, uint W, uint H)
    {
    }

    public byte[] ImageFileBinary = Array.Empty<byte>();
    public int ImageFileBinarySize;
    public byte[] DataFileBinary = Array.Empty<byte>();
    public int DataFileBinarySize;
    public (byte[]?, int)[] AdditionalImageFileBinaries = Array.Empty<(byte[]?, int)>();
    public string[] AdditionalImageFilePaths = Array.Empty<string>();

    public byte TransparentColorAlpha, TransparentColorRed, TransparentColorGreen, TransparentColorBlue;
    public uint Width, Height;

    public StringSpanKeySlowSet Set = new();
    public ArrayPoolList<Rect> RectList = new();

    public bool TryLoadDataFileBinary(byte[] dataFileBinary, int dataFileBinarySize)
    {
        if (dataFileBinary.Length < dataFileBinarySize)
        {
            return false;
        }

        if (DataFileBinary == dataFileBinary || DataFileBinary.AsSpan(0, DataFileBinarySize).SequenceEqual(dataFileBinary.AsSpan(0, dataFileBinarySize)))
        {
            return true;
        }

        if (dataFileBinarySize < 12)
        {
            return false;
        }

        Set.Dispose();
        RectList.Dispose();
        if (DataFileBinary != Array.Empty<byte>())
        {
            ArrayPool<byte>.Shared.Return(DataFileBinary);
        }

        DataFileBinary = dataFileBinary;
        DataFileBinarySize = dataFileBinarySize;

        Width = BinaryPrimitives.ReadUInt32LittleEndian(dataFileBinary);
        Height = BinaryPrimitives.ReadUInt32LittleEndian(dataFileBinary.AsSpan(4, 4));
        TransparentColorAlpha = dataFileBinary[8];
        TransparentColorRed = dataFileBinary[9];
        TransparentColorGreen = dataFileBinary[10];
        TransparentColorBlue = dataFileBinary[11];

        using ArrayPoolList<char> list = new();
        var span = dataFileBinary.AsSpan(12);
        do
        {
            var index = span.IndexOf<byte>(0);
            if (index <= 0)
            {
                return false;
            }

            if (index == 8 && Unsafe.As<byte, ulong>(ref span[0]) == 0x5f5f5f5f5f5f5f5fUL)
            {
                return true;
            }

            var name = span.Slice(0, index);
            list.Clear();
            var undefined = list.InsertUndefinedRange(0, (uint)name.Length);
            for (int i = 0; i < undefined.Length; i++)
            {
                undefined[i] = (char)name[i];
            }
            Set.InitialAdd(undefined);
            span = span.Slice(index + 1);
            if (span.Length < 16)
            {
                return false;
            }
            
            var x = BinaryPrimitives.ReadUInt32LittleEndian(span);
            var y = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4));
            var w = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(8));
            var h = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(12));
            RectList.Add(new(x, y, w, h));
            span = span.Slice(16);
        } while (true);
    }

    public void Dispose()
    {
        ImageFileBinarySize = 0;
        if (ImageFileBinary != Array.Empty<byte>())
        {
            ArrayPool<byte>.Shared.Return(ImageFileBinary);
            ImageFileBinary = Array.Empty<byte>();
        }

        if (DataFileBinary != Array.Empty<byte>())
        {
            ArrayPool<byte>.Shared.Return(DataFileBinary);
            DataFileBinary = Array.Empty<byte>();
        }

        if (AdditionalImageFileBinaries != Array.Empty<(byte[], int)>())
        {
            var span = AdditionalImageFileBinaries.AsSpan(0, AdditionalImageFilePaths.Length);
            foreach (ref var item in span)
            {
                if (item.Item1 is null)
                {
                    continue;
                }

                ArrayPool<byte>.Shared.Return(item.Item1);
            }

            span.Clear();
            ArrayPool<(byte[]?, int)>.Shared.Return(AdditionalImageFileBinaries);
            AdditionalImageFileBinaries = Array.Empty<(byte[]?, int)>();
        }

        AdditionalImageFilePaths = Array.Empty<string>();
        Set.Dispose();
        RectList.Dispose();
    }
}
