using System;
using System.Text;
using static Farmhash.Sharp.Farmhash;

namespace Wahren.Analysis.Unity
{
    public static partial class FileCreator
    {
        public static string ScenarioCreator(this Wahren.Analysis.Specific.ScenarioData2 scenario)
        {
            var buf = new StringBuilder()
            .Append("new ScenarioData(")
            .Append(Hash64(scenario.Scenario.Name))
            .Append("ul,")
            .Append(Hash64(scenario.Scenario.DisplayName ?? ""))
            .Append("ul,")
            .Append(Hash64(scenario.Scenario.WorldMapPath ?? ""))
            .Append("ul,")
            .Append(Hash64(scenario.Scenario.DescriptionText ?? ""))
            .Append("ul,")
            .Append(Hash64(scenario.Scenario.BeginText ?? ""))
            .Append("ul,")
            .Append(Hash64(scenario.Scenario.ZoneName ?? ""))
            .Append("ul,new int2(")
            .Append(scenario.Scenario.X ?? 0)
            .Append(',')
            .Append(scenario.Scenario.Y ?? 0)
            .Append("),")
            ;
            return buf.ToString();
        }
    }
}