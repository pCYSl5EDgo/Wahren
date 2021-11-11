using System;
using Wahren.ArrayPoolCollections;

namespace Wahren.Map;

public struct MapInfo : IDisposable
{
    public string? Path;
    public byte Width;
    public byte Height;
    public DualList<ChipData> Board;
    public StringSpanKeySlowSet NameSet;

    public MapInfo()
    {
        Path = null;
        Width = 0;
        Height = 0;
        Board = new();
        NameSet = new();
    }

    public MapInfo(string path) : this()
    {
        Path = path;
    }

    public ref ArrayPoolList<ChipData> this[uint x, uint y] => ref Board[x + y * Width];

    public void Dispose()
    {
        Path = null;
        Width = Height = 0;
        Board.Dispose();
        NameSet.Dispose();
    }
}
