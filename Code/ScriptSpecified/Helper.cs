using System;
using System.Collections.Generic;
namespace Wahren.Specific
{
    internal static class ScenarioData2Helper
    {
        internal static readonly System.Text.RegularExpressions.Regex Variable = new System.Text.RegularExpressions.Regex(@"^@[a-zA-Z0-9_]*$");
        internal static readonly System.Text.RegularExpressions.Regex Identifier = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_]*$");
        internal static readonly System.Text.RegularExpressions.Regex VariableOrIdentifier = new System.Text.RegularExpressions.Regex(@"^@?[a-zA-Z0-9_]*$");
        internal static readonly System.Text.RegularExpressions.Regex Text = new System.Text.RegularExpressions.Regex(@"^(@|[^0-9]|.+)$");
        internal static bool IsVariable(this List<Token> list, int index) => list[index].Type == 0 && Variable.IsMatch(list[index].Content);
        internal static bool IsIdentifier(this List<Token> list, int index) => list[index].Type == 0 && Identifier.IsMatch(list[index].Content);
        internal static bool IsVariableOrIdentifier(this List<Token> list, int index) => list[index].Type == 0 && VariableOrIdentifier.IsMatch(list[index].Content);
        internal static bool IsIdentifierOrNumber(this List<Token> list, int index) => list[index].Type == 2 || (list[index].Type == 0 && Identifier.IsMatch(list[index].Content));
        internal static bool IsText(this List<Token> list, int index) => list[index].Type != 2 && Text.IsMatch(list[index].ToLowerString());

        internal static void NotBoolIdentifier(this List<Token> list, SortedSet<string> notset, SortedSet<string> set)
        {
            var identifierName = list[0].Content.ToLower();
            if (!notset.Contains(identifierName))
            {
                set.Remove(identifierName);
                notset.Add(identifierName);
            }
        }

        internal static void IsSpot(this Token spot)
        {
            if (spot.Type == 1)
                if (spot.Symbol1 == '@') return;
                else throw new Exception(spot.DebugInfo);
            if (spot.Type == 2) throw new Exception(spot.DebugInfo);
            if (Identifier.IsMatch(spot.Content) && ScriptLoader.SpotDictionary.ContainsKey(spot.Content.ToLower())) return;
            if (Variable.IsMatch(spot.Content)) return;
            throw new SpotNotFoundException(spot.DebugInfo);
        }

        internal static void AddIdentifier(this List<Token> list, int index, SortedSet<string> set)
        {
            if (list.Count <= index)
                throw new Exception(list[0].DebugInfo);
            else if (list[index].Type == 1 && list[index].Symbol1 != '@')
                throw new Exception(list[index].DebugInfo);
            var content = list[index].Content ?? "";
            if (list[index].Type != 0 || !Identifier.IsMatch(content))
                throw new Exception(list[index].DebugInfo);
            set.Add(content.ToLower());
        }
        internal static void AddIdentifierOrNumber(this List<Token> list, int index, SortedSet<string> set)
        {
            if (list.Count <= index) throw new Exception(list[0].DebugInfo);
            else if (list[index].Type == 1 && list[index].Symbol1 != '@') throw new Exception(list[index].DebugInfo);
            else if (list[index].Type == 0 && Identifier.IsMatch(list[index].Content ?? ""))
                set.Add(list[index].Content.ToLower());
            else if (list[index].Type != 2) throw new Exception(list[index].DebugInfo);
        }
        internal static void AddVariableOnly(this List<Token> list, int index, SortedSet<string> set, SortedSet<string> set2)
        {
            if (list.Count <= index) throw new IndexOutOfRangeException();
            else if (list[index].Type == 1 && list[index].Symbol1 != '@')
                throw new Exception(list[index].DebugInfo);
            else
            {
                var item = (list[index].Content ?? "").ToLower();
                if (!Variable.IsMatch(item))
                    item = '@' + item;
                set.Add(item);
                set2.Add(item);
            }
        }
        internal static void AddVariableOnly(this List<Token> list, int index, SortedSet<string> set)
        {
            if (list.Count <= index) throw new IndexOutOfRangeException();
            else if (list[index].Type == 1 && list[index].Symbol1 != '@')
                throw new Exception(list[index].DebugInfo);
            else
            {
                var item = (list[index].Content ?? "").ToLower();
                if (!Variable.IsMatch(item))
                    set.Add('@' + item);
                else set.Add(item);
            }
        }
        internal static void AddVariable(this List<Token> list, int index, SortedSet<string> set, params SortedSet<string>[] sets)
        {
            if (list.Count <= index) throw new IndexOutOfRangeException();
            else if (list[index].Type == 1 && list[index].Symbol1 != '@')
                throw new Exception(list[index].DebugInfo);
            else if (!Variable.IsMatch(list[index].Content ?? ""))
                throw new Exception(list[index].DebugInfo);
            var name = list[index].Content.ToLower();
            set.Add(name);
            for (int i = 0; i < sets.Length; i++)
                sets[i].Add(name);
        }
        internal static void AddVariableOrIdentifier(this List<Token> list, int index, SortedSet<string> variable, SortedSet<string> identifier)
        {
            if (list.Count <= index) throw new Exception();
            else if (list[index].Type == 1)
            {
                if (list[index].Symbol1 != '@')
                    throw new Exception(list[index].DebugInfo);
            }
            else if (Variable.IsMatch(list[index].Content ?? ""))
                variable.Add(list[index].Content.ToLower());
            else if (Identifier.IsMatch(list[index].Content ?? ""))
                identifier.Add(list[index].Content.ToLower());
        }
        internal static void AddVariable_NotAddIdentifier(this List<Token> list, int index, SortedSet<string> variable)
        {
            if (list.Count <= index) throw new Exception();
            else if (list[index].Type == 1)
            {
                if (list[index].Symbol1 != '@')
                    throw new Exception(list[index].DebugInfo);
            }
            else if (Variable.IsMatch(list[index].Content ?? ""))
                variable.Add(list[index].Content.ToLower());
            else if (!Identifier.IsMatch(list[index].Content ?? ""))
                throw new Exception(list[index].DebugInfo);
        }
        internal static void AddVariableOrString(this List<Token> list, int index, SortedSet<string> set)
        {
            if (list.Count <= index) throw new Exception(list[0].DebugInfo);
            else if (Variable.IsMatch(list[index].Content ?? ""))
                set.Add(list[index].Content.ToLower());
        }

        internal static void ThrowException(this List<Token> list, int min, int max)
        {
            if (list.Count < min || list.Count > max)
                throw new ArgumentOutOfRangeException(list[0].DebugInfo);
        }
        internal static void ThrowException(this List<Token> list, int count)
        {
            if (list.Count != count)
                throw new ArgumentOutOfRangeException(list[0].DebugInfo);
        }
    }
}