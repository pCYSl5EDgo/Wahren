using System;
using System.Collections.Generic;

namespace Wahren
{
    public static partial class ScriptLoader
    {
        static IEnumerable<LexicalTree_Assign> SelectAssign(LexicalTree_Block input)
        {
            for (int i = 0; i < input.Children.Count; i++)
            {
                var assign = input.Children[i] as LexicalTree_Assign;
                if (assign == null) continue;
                yield return assign;
            }
        }
        static string Intern(string str) => str == null ? null : string.Intern(str);
        static string InternLower(string str) => str == null ? null : string.Intern(str.ToLower());

        static string InsertString(LexicalTree_Assign assign, List<string> fillWithNull)
        {
            if (assign.Content.Count == 0 || assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return null;
            }
            fillWithNull.Remove(assign.Name);
            return assign.Content[0].ToString();
        }
        static byte? InsertByte(LexicalTree_Assign assign, List<string> fillWithNull)
        {
            if (assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return null;
            }
            fillWithNull.Remove(assign.Name);
            return (byte)assign.Content[0].Number;
        }
        static int? InsertInt(LexicalTree_Assign assign, List<string> fillWithNull)
        {
            if (assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return null;
            }
            fillWithNull.Remove(assign.Name);
            return (int)assign.Content[0].Number;
        }
        static bool? InsertBool(LexicalTree_Assign assign, List<string> fillWithNull)
        {
            if (assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return null;
            }
            fillWithNull.Remove(assign.Name);
            switch (assign.Content[0].ToLowerString())
            {
                case "on":
                    return true;
                case "off":
                    return false;
                default:
                    throw new ScriptLoadingException(assign.Content[0]);
            }
        }
        static void InsertStringOnlyList(LexicalTree_Assign assign, List<string> fillWithNull, List<string> list)
        {
            list.Clear();
            var content = assign.Content;
            if (content.Count == 1 && content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return;
            }
            fillWithNull.Remove(assign.Name);
            for (int i = 0; i < content.Count; i++)
            {
                switch (content[i].Type)
                {
                    case 0:
                        list.Add(Intern(content[i].ToString()));
                        break;
                    default: throw new Exception(content[i].DebugInfo);
                }
            }
        }
        static void InsertStringList(LexicalTree_Assign assign, List<string> fillWithNull, List<string> list)
        {
            list.Clear();
            var content = assign.Content;
            if (content.Count == 1 && content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return;
            }
            fillWithNull.Remove(assign.Name);
            for (int i = 0; i < content.Count; i++)
            {
                switch (content[i].Type)
                {
                    case 0:
                        list.Add(Intern(content[i].ToString()));
                        break;
                    case 1:
                        if (content[i].IsSingleSymbol && content[i].Symbol1 == '@')
                            list.Add("");
                        else throw new Exception(content[i].DebugInfo);
                        break;
                    case 2:
                        for (int j = 1; j < content[i].Number; j++)
                            list.Add(Intern(content[i - 1].ToString()));
                        break;
                }
            }
        }
        static void InsertStringIntPair(LexicalTree_Assign assign, List<string> fillWithNull, Dictionary<string, int> dic)
        {
            dic.Clear();
            if (assign.Content.Count == 0 || assign.Content[0].Symbol1 == '@' || assign.Content[0].ToLowerString() == "none")
            {
                fillWithNull.Add(assign.Name);
                return;
            }
            fillWithNull.Remove(assign.Name);
            for (int i = 0; (i << 1) < assign.Content.Count; i++)
            {
                dic[Intern(assign.Content[i << 1].ToString())] = (int)assign.Content[(i << 1) + 1].Number;
            }
            return;
        }
        static void InsertStringBytePair(LexicalTree_Assign assign, List<string> fillWithNull, Dictionary<string, byte> dic)
        {
            dic.Clear();
            if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
            {
                fillWithNull.Add(assign.Name);
                return;
            }
            fillWithNull.Remove(assign.Name);
            for (int i = 0; (i << 1) < assign.Content.Count; i++)
            {
                dic[Intern(assign.Content[i << 1].ToString())] = (byte)assign.Content[(i << 1) + 1].Number;
            }
            return;
        }
        static void AddStringIntPair(LexicalTree_Assign assign, List<string> filledWithNull, List<string> monsters)
        {
            monsters.Clear();
            if (assign.Content.Count == 1 && assign.Content[0].Symbol1 == '@')
            {
                filledWithNull.Add(assign.Name);
                return;
            }
            filledWithNull.Remove(assign.Name);
            foreach (var item in assign.Content)
            {
                if (item.Type == 0)
                    monsters.Add(Intern(item.Content));
                else if (item.Symbol1 == '@')
                    monsters.Add("");
                else if (item.Type == 2)
                {
                    var lst = monsters[monsters.Count - 1];
                    for (long i = 1; i < item.Number; ++i)
                        monsters.Add(lst);
                }
            }
        }
        static void ScenarioVariantRoutine(LexicalTree_Assign assign, Dictionary<string, Dictionary<string, LexicalTree_Assign>> VariantData)
        {
            if (assign.Name[assign.Name.Length - 1] == '@')
            {
                var assignName = string.Intern(assign.Name.Substring(0, assign.Name.Length - 1));
                if (VariantData.ContainsKey(""))
                    VariantData[""][assignName] = assign;
                else
                    VariantData[""] = new Dictionary<string, LexicalTree_Assign>() { { assignName, assign } };
                return;
            }
            var array = assign.Name.Split(new char[1] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            switch (array.Length)
            {
                case 1:
                    assign.Name = array[0];
                    if (VariantData.ContainsKey(""))
                        VariantData[""][assign.Name] = assign;
                    else
                        VariantData[""] = new Dictionary<string, LexicalTree_Assign>() { { assign.Name, assign } };
                    break;
                case 2:
                    assign.Name = array[0];
                    if (VariantData.ContainsKey(array[1]))
                        VariantData[array[1]][assign.Name] = assign;
                    else
                        VariantData[array[1]] = new Dictionary<string, LexicalTree_Assign>() { { assign.Name, assign } };
                    break;
                default: throw new ScriptLoadingException(assign);
            }
        }
    }
}