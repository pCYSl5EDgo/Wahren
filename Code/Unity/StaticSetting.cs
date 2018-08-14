using System.Text;
using System.Collections.Generic;
using static Farmhash.Sharp.Farmhash;

namespace Wahren.Analysis.Unity
{
    public static partial class FileCreator
    {
        public static string StaticData()
        {
            var buf = new StringBuilder();
            var dicIdentifier = new SortedList<ulong, string>();
            var dicVariable = new SortedList<ulong, string>();
            var scs = ScriptLoader.Scenarios;
            for (int i = 0; i < scs.Length; i++)
            {
                var vg = scs[i].Variable_Get;
                foreach (var item in vg)
                    dicVariable.TryAdd(Hash64(item), item);
                var vs = scs[i].Variable_Set;
                foreach (var item in vs)
                    dicVariable.TryAdd(Hash64(item), item);
                var ig = scs[i].Identifier_Get;
                foreach (var str in ig)
                    dicIdentifier.TryAdd(Hash64(str), str);
                var ise = scs[i].Identifier_Set;
                foreach (var str in ise)
                    dicIdentifier.TryAdd(Hash64(str), str);
            }
            buf.Append(@"using System;
using System.Collections.Generic;

public static class CommonSetting
{
    //255: undefined
    //0: easy
    //1: normal
    //2: hard
    //3: luna
    public static byte Difficulty = 255;

    //255: undefinded
    public static byte ScenarioNumber = 255;
	public static readonly Dictionary<string, int> IdentifierDictionary = new Dictionary<string, int>();
	public static readonly Dictionary<string, string[]> VariableDictionary = new Dictionary<string, string[]>();
}");
            return buf.ToString();
        }
    }
}