using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Wahren.Map
{
    public static class MapLoader
    {
        public static Panel LoadPanel(this string file)
        {
            var ans = new Panel();
            ans.Load(file);
            return ans;
        }

        public static ValueTuple<FieldData, List<ObjectData>>[][][] LoadMapAll(this ScenarioFolder sc)
        {
            var ans = new ValueTuple<FieldData, List<ObjectData>>[sc.Stage_Map.Count][][];
            for (int i = 0; i < sc.Stage_Map.Count; i++)
            {
                var p = sc.Stage_Map[i].LoadPanel();
                ans[i] = new ValueTuple<FieldData, List<ObjectData>>[p.Height][];
                for (int j = 0; j < ans.Length; j++)
                {
                    ans[i][j] = new ValueTuple<FieldData, List<ObjectData>>[p.Width];
                    for (int k = 0; k < p.Pool[j].Length; k++)
                    {
                        var objects = new List<ObjectData>(p.Pool[j][k].Count - 1);
                        FieldData field = null;
                        for (int l = 0; l < p.Pool[j][k].Count; l++)
                        {
                            if (!ScriptLoader.FieldDictionary.TryGetValue(p.Pool[j][k][l].Name, out field))
                            {
                                if (!ScriptLoader.ObjectDictionary.TryGetValue(p.Pool[j][k][l].Name, out var obj))
                                    throw new Exception();
                                objects.Add(obj);
                            }
                        }
                        if (field == null) throw new Exception();
                        ans[i][j][k] = new ValueTuple<FieldData, List<ObjectData>>(field, objects);
                    }
                }
            }
            return ans;
        }
    }
    public class Panel
    {
        public Panel() { }
        public void Load(string path) => Load(new FileInfo(path));
        public void Load(FileInfo fi)
        {
            if (fi == null)
                throw new ArgumentNullException(nameof(fi));

            using (var stream = fi.Open(FileMode.Open))
            {
                byte[] b2 = new byte[2];
                stream.Read(b2, 0, 2);
                Pool = new List<ChipData>[b2[1]][];
                int type = 0;
                int fx = 0;
                int fy = 0;
                for (int i = 0; i < Pool.Length; i++)
                    Pool[i] = new List<ChipData>[b2[0]];
                while (fy < b2[1])
                {
                    type = stream.ReadByte();
                    if (type == 0xff)
                    {
                        if (++fx >= b2[0])
                        {
                            fx = 0;
                            ++fy;
                        }
                        continue;
                    }
                    var namebuf = new List<byte>(50);
                    while (true)
                    {
                        var ch = stream.ReadByte();
                        if (ch == 0xff || ch == 0xfe) break;
                        namebuf.Add((byte)ch);
                        if (namebuf.Count > 50) throw new ApplicationException("mapに使用できるオブジェクトあるいはフィールド識別子の文字数は50字以内です。");
                    }
                    ChipData newcd;
                    if (type == 0 || type == 1)
                        newcd = new ChipData(Encoding.UTF8.GetString(namebuf.ToArray()), fx, fy, type);
                    else
                    {
                        int drt = (type & 0xf) - 2;
                        int fm = (type & 0xff) >> 4;
                        newcd = ChipData.MakeUnitNameChip(Encoding.UTF8.GetString(namebuf.ToArray()).ToLower(), fx, fy, drt, fm);
                    }
                    if (newcd != null)
                    {
                        if (Pool[fy][fx] == null)
                            Pool[fy][fx] = new List<ChipData>();
                        Pool[fy][fx].Add(newcd);
                    }
                }
            }
        }
        public Task Save(FileInfo fi)
        {
            if (fi == null)
                throw new ArgumentNullException(nameof(fi));

            var ans = new List<byte>();
            ans.Add((byte)Pool[0].Length);
            ans.Add((byte)Pool.Length);
            for (int y = 0; y < Pool.Length; y++)
                for (int x = 0; x < Pool[0].Length; x++)
                {
                    foreach (var cd in Pool[y][x])
                    {
                        if (cd.X != x || cd.Y != y)
                            continue;
                        ans.Add((byte)cd.ZOrder);
                        ans.AddRange(Encoding.UTF8.GetBytes(cd.Name));
                        ans.Add(0xfe);
                    }
                    ans.Add(0xff);
                }
            using (var stream = fi.Open(FileMode.OpenOrCreate))
                return stream.WriteAsync(ans.ToArray(), 0, ans.Count);
        }

        public void Select(Func<ChipData, ChipData> func)
        {
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                    if (Pool[y][x] == null) continue;
                    else
                        for (int i = 0; i < Pool[y][x].Count; ++i)
                            Pool[y][x][i] = func(Pool[y][x][i]);
        }
        public void Add(ChipData chip)
        {
            if (chip == null || chip.X >= Width || chip.Y >= Height) return;
            if (Pool[chip.Y][chip.X] == null)
                Pool[chip.Y][chip.X] = new List<ChipData>() { chip };
            else
                Pool[chip.Y][chip.X].Add(chip);
        }
        public void Clear(Func<ChipData, bool> match) => Clear(new Predicate<ChipData>(match));
        private void Clear(Predicate<ChipData> match)
        {
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                {
                    if (Pool[y][x] == null) continue;
                    Pool[y][x].RemoveAll(match);
                }
        }
        public int Height => Pool.Length;
        public int Width => Pool[0].Length;
        public List<ChipData>[][] Pool { get; protected set; }
    }
    public class ChipData
    {
        public string Name { get; protected set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public int ZOrder { get; protected set; }
        public int ZIndex { get; protected set; } = 0;

        public ChipData(string name, int x, int y, int zOrder) : this(name, x, y, 32, 32, zOrder) { }
        public ChipData(string name, int x, int y, int width, int height, int zOrder)
        {
            Name = name;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ZOrder = zOrder;
        }
        public bool IsUnit() => ZOrder != 0 && ZOrder != 1;
        public static ChipData MakeUnitNameChip(string name, int x, int y, int drt, int fm)
            => new ChipData(name, x, y, (drt + 2) | (fm << 4));

        public override bool Equals(object obj) => Equals(obj as ChipData);
        public bool Equals(ChipData obj)
            => (obj != null) && (Name == obj.Name) && X == obj.X && Y == obj.Y && Width == obj.Width && (Height == obj.Height) && ZOrder == obj.ZOrder;
        public override int GetHashCode() => throw new System.NotImplementedException();
    }

    public static class MapHelper
    {
        public static readonly short[] DistanceTable = new short[11] { short.MaxValue >> 1, 2520, 1260, 840, 630, 504, 420, 360, 315, 280, 252 };
        public static readonly short[] DistanceObliqueTable = new short[11] { short.MaxValue >> 1, (short)(2520 * Math.Pow(2, 0.5)), (short)(1260 * Math.Pow(2, 0.5)), (short)(840 * Math.Pow(2, 0.5)), (short)(630 * Math.Pow(2, 0.5)), (short)(504 * Math.Pow(2, 0.5)), (short)(420 * Math.Pow(2, 0.5)), (short)(360 * Math.Pow(2, 0.5)), (short)(315 * Math.Pow(2, 0.5)), (short)(280 * Math.Pow(2, 0.5)), (short)(252 * Math.Pow(2, 0.5)) };
        public static short Distance(MoveTypeData move, FieldData from, FieldData to)
        {
            short ans = 0;
            if (move.FieldMoveDictionary.TryGetValue(from.Attribute, out var mode1))
                ans = DistanceTable[mode1];
            else ans = DistanceTable[5];
            if (mode1 == 0) return short.MaxValue >> 1;
            if (move.FieldMoveDictionary.TryGetValue(from.Attribute, out var mode2))
            {
                if (mode2 == 0) return DistanceTable[0];
                return (short)(DistanceTable[mode2] + ans);
            }
            else return (short)(ans + DistanceTable[5]);
        }
        public static short DistanceOblique(MoveTypeData move, FieldData from, FieldData to)
        {
            short ans = 0;
            if (move.FieldMoveDictionary.TryGetValue(from.Attribute, out var mode1))
                ans = DistanceObliqueTable[mode1];
            else ans = DistanceObliqueTable[5];
            if (mode1 == 0) return DistanceObliqueTable[0];
            if (move.FieldMoveDictionary.TryGetValue(from.Attribute, out var mode2))
            {
                if (mode2 == 0) return DistanceObliqueTable[0];
                return (short)(DistanceObliqueTable[mode2] + ans);
            }
            else return (short)(ans + DistanceObliqueTable[5]);
        }

        public static int[][] Warshall(this short[][] edgeTable, int[][] next)
        {
            var ans = new int[edgeTable.Length][];
            for (int i = 0; i < ans.Length; i++)
            {
                ans[i] = new int[ans.Length];
                for (int j = 0; j < ans.Length; j++)
                {
                    ans[i][j] = edgeTable[i][j];
                    next[i][j] = j;
                }
            }
            for (int k = 0; k < ans.Length; k++)
            {
                for (int i = 0; i < ans.Length; i++)
                {
                    for (int j = 0; j < ans.Length; j++)
                    {
                        if (ans[i][j] > ans[i][k] + ans[k][j])
                        {
                            ans[i][j] = ans[i][k] + ans[k][j];
                            next[i][j] = next[i][k];
                        }
                    }
                }
            }
            return ans;
        }
    }
}