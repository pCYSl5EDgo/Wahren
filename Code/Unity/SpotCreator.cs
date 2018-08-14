using System;
using System.Text;
using static Farmhash.Sharp.Farmhash;

namespace Wahren.Analysis.Unity
{
    public static partial class FileCreator
    {
        public static string SpotCreator(this SpotData spot)
        {
            var buf = new StringBuilder()
            .Append("new Spot(")
            .Append(Hash64(spot.Name ?? ""))
            .Append("ul,")
            .Append(Hash64(spot.DisplayName ?? ""))
            .Append("ul,")
            .Append(Hash64(spot.Image ?? ""))
            .Append("ul,")
            .Append(Hash64(spot.Map ?? ""))
            .Append("ul,")
            .Append(Hash64(spot.BGM ?? ""))
            .Append("ul,")
            .Append(Hash64(spot.Dungeon ?? ""))
            .Append("ul,new uint2(").Append(spot.X ?? 0).Append(',').Append(spot.Y ?? 0)
            .Append("),new uint2(").Append(spot.Width ?? 32).Append(',').Append(spot.Height ?? 32)
            .Append("),")
            .Append(spot.Gain ?? 0)
            .Append("ul,")
            .Append(spot.Castle ?? 0)
            .Append("ul,")
            .Append(spot.Limit ?? ScriptLoader.Context.btl_limit)
            .Append("ul,(ushort)")
            .Append(spot.CastleLot ?? ScriptLoader.Context.BattleCastleLot)
            .Append(",(byte)")
            .Append(spot.Volume ?? ScriptLoader.Context.BGMVolume)
            .Append(",(byte)")
            .Append(spot.Capacity ?? ScriptLoader.Context.SpotCapacity)
            .Append(",(byte)")
            .Append((spot.IsCastleBattle ?? false ? 1 : 0) | (spot.IsNotHome ?? false ? 2 : 0) | (spot.IsNotRaisableSpot ?? false ? 4 : 0) | (spot.Politics ?? false ? 8 : 0))
            .Append(");");
            return buf.ToString();
        }
    }
}