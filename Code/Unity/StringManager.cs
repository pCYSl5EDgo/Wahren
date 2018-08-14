using System;
using System.Text;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace Wahren.Analysis.Unity
{
    using static Farmhash.Sharp.Farmhash;
    public static class StringManager
    {
        public static string String()
        {
            var buf = new StringBuilder();
            var set = new Dictionary<ulong, string>();
            buf.Append(@"using System.Collections.Immutable;

namespace Wahren.Analysis{
    public static class StringManager{
        public static readonly ImmutableDictionary<ulong, string> StringMap;
        static StringManager(){
            var dic = new System.Collections.Generic.Dictionary<ulong, string>();");
            void Proc(string key)
            {
                var hash = Hash64(key);
                if (set.ContainsKey(hash))
                {
                    if (!Object.ReferenceEquals(set[hash], key))
                        throw new Exception(key);
                }
                else buf.Append("\t\t\tdic.Add(" + hash + "ul,\"" + key.Escape() + "\");\n");
            }
            foreach (var key in ScriptLoader.Attribute.Keys)
                Proc(key);
            foreach (var key in ScriptLoader.BaseClassKeyDictionary.Keys)
                Proc(key);
            foreach (var key in ScriptLoader.DungeonDictionary.Keys)
                Proc(key);
            foreach (var key in ScriptLoader.EventDictionary.Keys)
                Proc(key);
            buf.Append(@"               StringMap = ImmutableDictionary.CreateRange(dic);
        }
    }
}");
            return buf.ToString();
        }
    }
}