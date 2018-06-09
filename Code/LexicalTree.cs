using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Wahren
{
    public class LexicalTree : IName, IDebugInfo
    {
        private string _name;
        private string _file;

        public string File { get => _file; set => _file = String.Intern(value); }
        public int Line { get; set; }
        public int Column { get; set; }
        public string DebugInfo => File + '/' + (Line + 1) + '/' + Column;
        public enum TreeType
        {
            Block,
            Assign,
            Function,
            Statement,
            BoolParen,
            VariableParen,
            NameStructure,
            NoNameStructure
        }
        public TreeType Type { get; set; }
        public string Name { get => _name; set => _name = String.Intern(value); }

        //Unit構造体やContext構造体
        public static IEnumerable<LexicalTree_Block> Parse(List<Token> input, bool isDebug)
        {
            IEnumerator<Token> c;
            if (isDebug) c = input.RemoveCommentWhiteSpace().GetEnumerator();
            else c = input.RemoveDebugCommentWhiteSpace().GetEnumerator();
            while (c.MoveNext())
            {
                var token = c.Current;
                switch (token.Type)
                {
                    case 0:
                        switch (token.ToLowerString())
                        {
                            case "power":
                            case "spot":
                            case "unit":
                            case "class":
                            case "skill":
                            case "skillset":
                            case "race":
                            case "field":
                            case "movetype":
                            case "dungeon":
                            case "voice":
                            case "object":
                                yield return LexicalTree_NameStructure.ParseNoFunction(ref c);
                                break;
                            case "world":
                            case "scenario":
                            case "event":
                            case "story":
                            case "fight":
                            case "function":
                            case "deploy":
                                yield return LexicalTree_NameStructure.ParseYesFunction(ref c);
                                break;
                            case "attribute":
                            case "context":
                            case "sound":
                            case "workspace":
                            case "detail":
                                yield return LexicalTree_NoNameStructure.Parse(ref c);
                                break;
                            default: throw new LexicalTreeConstructionException(ref token);
                        }
                        break;
                    default:
                        throw new LexicalTreeConstructionException(ref token);
                }
            }
        }

        public class LexicalTreeConstructionException : ApplicationException
        {
            public LexicalTreeConstructionException(ref Token token) : base($"{token.File}/{token.Line + 1}/{token.Column + 1}") { }
        }

        public LexicalTree(string name)
        {
            Name = name.ToLower();
        }
    }
    public class LexicalTree_Content : LexicalTree
    {
        public List<Token> Content { get; } = new List<Token>();
        public LexicalTree_Content(string name) : base(name) { }
    }
    public sealed class LexicalTree_VariableParen : LexicalTree_Content
    {
        public static LexicalTree_VariableParen Parse(ref IEnumerator<Token> c)
        {
            var token = c.Current;
            Token? old = null;
            var answer = new LexicalTree_VariableParen("(") { File = token.File, Line = token.Line, Column = token.Column };
            while (c.MoveNext())
            {
                token = c.Current;
                if (old == null)
                {
                    if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == ')') return answer;
                    else if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == ',')
                        continue;
                    else
                        old = token;
                }
                else
                {
                    var _tmpOld = old.Value;
                    if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == ')')
                    {
                        answer.Content.Add(_tmpOld);
                        return answer;
                    }
                    else if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == ',')
                    {
                        answer.Content.Add(_tmpOld);
                        old = null;
                        continue;
                    }
                    else if (token.Type == 2 && _tmpOld.Type == 1 && _tmpOld.IsSingleSymbol && _tmpOld.IsNext(ref token))
                    {
                        if (_tmpOld.Symbol1 == '+')
                            old = token;
                        else if (old.Value.Symbol1 == '-')
                            old = new Token(token.File, token.Line, _tmpOld.Column, _tmpOld.IsDebug, false, -token.Number);
                        else old = old.Value.Merge(ref token);
                    }
                    else old = old.Value.Merge(ref token);
                }
            }
            throw new LexicalTreeConstructionException(ref token);
        }
        public LexicalTree_VariableParen(string name) : base(name)
        {
            Type = TreeType.VariableParen;
        }
    }
    public sealed class LexicalTree_Assign : LexicalTree_Content
    {
        public LexicalTree_Assign(string name) : base(name)
        {
            Type = TreeType.Assign;
        }
        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append(this.Name).Append(" = ");
            if (Content.Count == 0)
            {
                buf.Append('@');
                return buf.ToString();
            }
            buf.Append(Content[0].ToString());
            foreach (var item in Content.Skip(1))
                buf.Append(", ").Append(item.ToString());
            return buf.ToString();
        }
        public void ForEach(Action<Token> act)
        {
            foreach (var item in Content)
                act(item);
        }
        public void ForEach(Action<string> act)
        {
            foreach (var item in Content)
                act(item.ToString());
        }
        public void ForEach(Action<long> act)
        {
            foreach (var item in Content)
                act(item.Number);
        }
        public void ForEach(Action<int> act)
        {
            foreach (var item in Content)
                act((int)item.Number);
        }
    }
    public sealed class LexicalTree_BoolParen : LexicalTree_Block
    {
        public sealed class SingleContent : LexicalTree
        {
            public Token Content;
            public SingleContent(ref Token token) : base(token.ToString())
            {
                Line = token.Line;
                File = token.File;
                Column = token.Column;
                Type = TreeType.BoolParen;
                Content = token;
            }
            public override string ToString() => Content.ToString();
        }
        public LexicalTree_BoolParen() : base("") { Type = LexicalTree.TreeType.BoolParen; }
        internal static LexicalTree_BoolParen ParseBool_Inner(ref IEnumerator<Token> c)
        {
            var token = c.Current;
            var answer = new LexicalTree_BoolParen() { File = token.File, Line = token.Line, Column = token.Column };
            string content;
            Token next;
            int count = 1;
            while (c.MoveNext())
            {
            Top:
                token = c.Current;
                content = token.Content?.ToLower();
                if (token.Type == 0)
                {
                    if (c.MoveNext())
                    {
                        next = c.Current;
                        if (next.Symbol1 == '(' && token.IsNext(ref next))
                        {
                            answer.Children.Add(new LexicalTree_Function(content) { File = token.File, Line = token.Line, Column = token.Column, Variable = LexicalTree_VariableParen.Parse(ref c) });
                        }
                        else
                        {
                            answer.Children.Add(new SingleContent(ref token));
                            goto Top;
                        }
                    }
                    else throw new LexicalTreeConstructionException(ref token);
                }
                else if (token.Type == 2)
                    answer.Children.Add(new SingleContent(ref token));
                else if (token.IsSingleSymbol)
                {
                    switch (token.Symbol1)
                    {
                        case ')':
                            --count;
                            if (count != 0)
                                answer.Children.Add(new SingleContent(ref token));
                            else return answer;
                            break;
                        case '(':
                            ++count;
                            answer.Children.Add(new SingleContent(ref token));
                            break;
                        case '+':
                            var last = answer.Children.LastOrDefault() as SingleContent;
                            if (last != null)
                            {
                                var last_token = last.Content;
                                if (last_token.Symbol1 == '+' || last_token.Symbol1 == '-' || last_token.Symbol1 == '*' || last_token.Symbol1 == '/' || last_token.Symbol1 == '%' || last_token.Symbol1 == '(' || last_token.Symbol1 == '=' || last_token.Symbol1 == '!' || last_token.Symbol1 == '<' || last_token.Symbol1 == '>')
                                {
                                    if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                                    next = c.Current;
                                    if (token.IsNext(ref next) && next.Type == 2)
                                    {
                                        token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, token.Number);
                                        answer.Children.Add(new SingleContent(ref token));
                                    }
                                    else goto Top;
                                }
                                else answer.Children.Add(new SingleContent(ref token));
                            }
                            else if (answer.Children.Count == 0)
                            {
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                                next = c.Current;
                                if (token.IsNext(ref next) && next.Type == 2)
                                {
                                    token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, token.Number);
                                    answer.Children.Add(new SingleContent(ref token));
                                }
                                else goto Top;
                            }
                            else answer.Children.Add(new SingleContent(ref token));
                            break;
                        case '-':
                            var last2 = answer.Children.LastOrDefault() as SingleContent;
                            if (last2 != null)
                            {
                                var last_token2 = last2.Content;
                                if (last_token2.Symbol1 == '+' || last_token2.Symbol1 == '-' || last_token2.Symbol1 == '*' || last_token2.Symbol1 == '/' || last_token2.Symbol1 == '%' || last_token2.Symbol1 == '(' || last_token2.Symbol1 == '=' || last_token2.Symbol1 == '!' || last_token2.Symbol1 == '<' || last_token2.Symbol1 == '>')
                                {
                                    if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                                    next = c.Current;
                                    if (token.IsNext(ref next) && next.Type == 2)
                                    {
                                        token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, -token.Number);
                                        answer.Children.Add(new SingleContent(ref token));
                                    }
                                    else goto Top;
                                }
                                else answer.Children.Add(new SingleContent(ref token));
                            }
                            else if (answer.Children.Count == 0)
                            {
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                                next = c.Current;
                                if (token.IsNext(ref next) && next.Type == 2)
                                {
                                    token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, -token.Number);
                                    answer.Children.Add(new SingleContent(ref token));
                                }
                                else goto Top;
                            }
                            else answer.Children.Add(new SingleContent(ref token));
                            break;
                        case '@':
                            if (c.MoveNext())
                            {
                                next = c.Current;
                                if (next.Type != 0 || !token.IsNext(ref next))
                                {
                                    answer.Children.Add(new SingleContent(ref token));
                                    goto Top;
                                }
                                token = token.Merge(ref next);
                                answer.Children.Add(new SingleContent(ref token));
                            }
                            else throw new LexicalTreeConstructionException(ref token);
                            break;
                        default:
                            answer.Children.Add(new SingleContent(ref token));
                            break;
                    }
                }
                else answer.Children.Add(new SingleContent(ref token));
            }
            throw new LexicalTreeConstructionException(ref token);
        }
        internal static LexicalTree_BoolParen ParseBool(ref IEnumerator<Token> c)
        {
            return ParseBool_Inner(ref c);
        }
        public override string ToString()
        {
            var buf = new StringBuilder();
            for (int i = 0; i < Children.Count; i++)
                buf.Append(Children[i].ToString());
            return buf.ToString();
        }
    }
    public sealed class LexicalTree_Function : LexicalTree
    {
        public LexicalTree_Function(string name) : base(name)
        {
            Type = TreeType.Function;
        }
        public LexicalTree_VariableParen Variable { get; set; }
        public static LexicalTree_Function ParseFunction(ref Token old, ref IEnumerator<Token> c)
        {
            var answer = new LexicalTree_Function(old.Content) { File = old.File, Line = old.Line, Column = old.Column };
            var token = c.Current;
            if (token.Type != 1 || token.IsDoubleSymbol || token.Symbol1 != '(') throw new LexicalTreeConstructionException(ref token);
            answer.Variable = LexicalTree_VariableParen.Parse(ref c);
            answer.Variable.Type = TreeType.BoolParen;
            return answer;
        }

        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append(Name).Append('(');
            if (this.Variable.Content.Count == 0)
            {
                buf.Append(')');
                return buf.ToString();
            }
            buf.Append(Variable.Content[0].ToString());
            foreach (var item in this.Variable.Content.Skip(1))
                buf.Append(", ").Append(item.ToString());
            buf.Append(')');
            return buf.ToString();
        }
    }
    public class LexicalTree_Block : LexicalTree
    {
        public LexicalTree_Block(string name) : base(name)
        {
            Type = TreeType.Block;
        }
        public List<LexicalTree> Children { get; } = new List<LexicalTree>();

        public static List<LexicalTree> Parse(ref IEnumerator<Token> c)
        {
            var answer = new List<LexicalTree>();
            int leftParenCount = 0;
            var token = c.Current;
            if (token.Symbol1 == '{') ++leftParenCount;
            else throw new LexicalTreeConstructionException(ref token);
            var tmp = new Stack<Token>(1024);
            while (c.MoveNext())
            {
            Top:
                token = c.Current;
                if (token.Type == 1)
                {
                    if (token.IsDoubleSymbol)
                    {
                        tmp.Push(token);
                    }
                    else
                    {
                        if (token.Symbol1 == '{')
                        {
                            ++leftParenCount;
                            var block = new LexicalTree_Block("{") { File = token.File, Line = token.Line, Column = token.Column };
                            block.Children.AddRange(Parse(ref c));
                            answer.Add(block);
                        }
                        else if (token.Symbol1 == '}') { if (--leftParenCount == 0) break; }
                        else tmp.Push(token);
                    }
                }
                else
                {
                    var content = token.ToString().ToLower();
                    switch (content)
                    {
                        case "if":
                        case "rif":
                        case "while":
                        case "switch":
                        case "case":
                        case "foreach":
                            var old = token;
                            if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                            token = c.Current;
                            if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == '(')
                            {
                                if (tmp.Count > 0) answer.AddRange(LexicalTree_NameStructure.ParseAssigns(tmp));
                                var stmt = new LexicalTree_Statement(content) { File = old.File, Line = old.Line, Column = old.Column };
                                stmt.Paren = LexicalTree_BoolParen.ParseBool(ref c);
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                                token = c.Current;
                                if (token.Type != 1 || token.IsDoubleSymbol || token.Symbol1 != '{') throw new LexicalTreeConstructionException(ref token);
                                stmt.Children.AddRange(LexicalTree_Block.Parse(ref c));
                                answer.Add(stmt);
                            }
                            else { tmp.Push(old); tmp.Push(token); }
                            break;
                        case "battle":
                        case "default":
                            old = token;
                            if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                            token = c.Current;
                            if (token.Symbol1 == '{')
                            {
                                if (tmp.Count > 0)
                                    answer.AddRange(LexicalTree_NameStructure.ParseAssigns(tmp));
                                var block = new LexicalTree_Statement(content) { File = old.File, Line = old.Line, Column = old.Column };
                                block.Children.AddRange(LexicalTree_Block.Parse(ref c));
                            }
                            else if (token.Symbol1 == '=')
                            {
                                tmp.Push(old);
                                tmp.Push(token);
                            }
                            else
                            {
                                tmp.Push(old);
                                goto Top;
                            }
                            break;
                        case "else":
                            if (tmp.Count > 0)
                                answer.AddRange(LexicalTree_NameStructure.ParseAssigns(tmp));
                            if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                            token = c.Current;
                            if (token.Type == 1 && token.IsSingleSymbol && token.Symbol1 == '{')
                            {
                                var _else = new LexicalTree_Statement("else") { File = token.File, Line = token.Line, Column = token.Column };
                                _else.Children.AddRange(LexicalTree_Block.Parse(ref c));
                                answer.Add(_else);
                            }
                            else if (token.Type == 0 && token.ToLowerString() == "if")
                            {
                                var _else = new LexicalTree_Statement("elseif") { File = token.File, Line = token.Line, Column = token.Column };
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                                _else.Paren = LexicalTree_BoolParen.ParseBool(ref c);
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                                _else.Children.AddRange(LexicalTree_Block.Parse(ref c));
                                answer.Add(_else);
                            }
                            else throw new LexicalTreeConstructionException(ref token);
                            break;
                        default:
                            old = token;
                            if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                            token = c.Current;
                            if (token.Symbol1 != '(' || !old.IsNext(ref token))
                            {
                                tmp.Push(old);
                                goto Top;
                            }
                            if (tmp.Count > 0)
                                answer.AddRange(LexicalTree_NameStructure.ParseAssigns(tmp));
                            answer.Add(LexicalTree_Function.ParseFunction(ref old, ref c));
                            break;
                    }
                }
            }
            return answer;
        }
    }
    public sealed class LexicalTree_Statement : LexicalTree_Block
    {
        public LexicalTree_Statement(string name) : base(name)
        {
            Type = TreeType.Statement;
        }
        public LexicalTree_BoolParen Paren { get; set; }
        public override string ToString()
        {
            var buf = new StringBuilder()
            .Append(Name)
            .Append('(')
            .Append(Paren.ToString())
            .Append("){");
            for (int i = 0; i < Children.Count; i++)
                buf.Append("\n\t").Append(Children[i].ToString());
            return buf.Append("\n}").ToString();
        }
    }
    public interface IStructureDataType
    {
        StructureDataType Structure { get; }
    }
    public sealed class LexicalTree_NoNameStructure : LexicalTree_Block, IStructureDataType
    {
        public LexicalTree_NoNameStructure(string name) : base(name)
        {
            Type = TreeType.NoNameStructure;
        }
        public StructureDataType Structure { get; set; }
        public static new LexicalTree_NoNameStructure Parse(ref IEnumerator<Token> c)
        {
            Token token = c.Current;
            LexicalTree_NoNameStructure answer = new LexicalTree_NoNameStructure(token.Content);
            switch (answer.Name)
            {
                case "attribute":
                    answer.Structure = StructureDataType.Attribute;
                    break;
                case "context":
                    answer.Structure = StructureDataType.Context;
                    break;
                case "sound":
                    answer.Structure = StructureDataType.Sound;
                    break;
                case "workspace":
                    answer.Structure = StructureDataType.Workspace;
                    break;
                case "detail":
                    answer.Structure = StructureDataType.Detail;
                    break;
            }
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
            token = c.Current;
            if (token.Type != 1 || token.Symbol1 != '{') throw new LexicalTreeConstructionException(ref token);
            var stack = new Stack<Token>();
            int leftCount = 1;
            while (c.MoveNext())
            {
                token = c.Current;
                if (token.Symbol1 == '{') { ++leftCount; stack.Push(token); }
                else if (token.Symbol1 == '}') { if (--leftCount == 0) break; stack.Push(token); }
                else stack.Push(token);
            }
            answer.Children.AddRange(LexicalTree_NameStructure.ParseAssigns(stack));
            return answer;
        }
    }
    public sealed class LexicalTree_NameStructure : LexicalTree_Block, IInherit, IStructureDataType
    {
        public LexicalTree_NameStructure(string name) : base(name)
        {
            Type = TreeType.NameStructure;
        }
        public string Inherit { get; set; }
        public StructureDataType Structure { get; set; }
        public static LexicalTree_NameStructure ParseYesFunction(ref IEnumerator<Token> c)
        {
            Token token = c.Current;
            LexicalTree_NameStructure answer = new LexicalTree_NameStructure(token.Content) { File = token.File, Line = token.Line, Column = token.Column };
            switch (token.ToLowerString())
            {
                case "world":
                    answer.Structure = StructureDataType.World;
                    break;
                case "scenario":
                    answer.Structure = StructureDataType.Scenario;
                    break;
                case "event":
                    answer.Structure = StructureDataType.Event;
                    break;
                case "story":
                    answer.Structure = StructureDataType.Story;
                    break;
                case "fight":
                    answer.Structure = StructureDataType.Fight;
                    break;
                case "function":
                    answer.Structure = StructureDataType.Function;
                    break;
                case "deploy":
                    answer.Structure = StructureDataType.Deploy;
                    break;
            }
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
            token = c.Current;
            if (token.Type == 1) throw new LexicalTreeConstructionException(ref token);
            answer.Name = token.ToString().ToLower();
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
            token = c.Current;
            if (token.Type != 1 || token.IsDoubleSymbol) throw new LexicalTreeConstructionException(ref token);
            if (token.Symbol1 == ':')
            {
                if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                token = c.Current;
                answer.Inherit = token.ToString().ToLower();
                if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                token = c.Current;
                if (token.Type != 1 || token.IsDoubleSymbol || token.Symbol1 != '{') throw new LexicalTreeConstructionException(ref token);
                answer.Children.AddRange(LexicalTree_Block.Parse(ref c));
                return answer;
            }
            else if (token.Symbol1 == '{')
            {
                answer.Inherit = "";
                answer.Children.AddRange(LexicalTree_Block.Parse(ref c));
                return answer;
            }
            else throw new LexicalTreeConstructionException(ref token);
        }
        public static LexicalTree_NameStructure ParseNoFunction(ref IEnumerator<Token> c)
        {
            var token = c.Current;
            var Block = new LexicalTree_NameStructure(token.Content) { File = token.File, Line = token.Line, Column = token.Column };
            switch (Block.Name)
            {
                case "power":
                    Block.Structure = StructureDataType.Power;
                    break;
                case "spot":
                    Block.Structure = StructureDataType.Spot;
                    break;
                case "unit":
                    Block.Structure = StructureDataType.Unit;
                    break;
                case "class":
                    Block.Structure = StructureDataType.Class;
                    break;
                case "skill":
                    Block.Structure = StructureDataType.Skill;
                    break;
                case "skillset":
                    Block.Structure = StructureDataType.Skillset;
                    break;
                case "race":
                    Block.Structure = StructureDataType.Race;
                    break;
                case "field":
                    Block.Structure = StructureDataType.Field;
                    break;
                case "movetype":
                    Block.Structure = StructureDataType.Movetype;
                    break;
                case "dungeon":
                    Block.Structure = StructureDataType.Dungeon;
                    break;
                case "voice":
                    Block.Structure = StructureDataType.Voice;
                    break;
                case "object":
                    Block.Structure = StructureDataType.Object;
                    break;
            }
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
            token = c.Current;
            if (token.Type == 1) throw new LexicalTreeConstructionException(ref token);
            Block.Name = token.ToString().ToLower();
            if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
            token = c.Current;
            if (token.Type != 1 || token.IsDoubleSymbol) throw new LexicalTreeConstructionException(ref token);
            if (token.Symbol1 == ':')
            {
                if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                token = c.Current;
                if (token.Type == 1) throw new LexicalTreeConstructionException(ref token);
                Block.Inherit = token.ToString().ToLower();
                if (!c.MoveNext()) throw new LexicalTreeConstructionException(ref token);
                token = c.Current;
                if (token.Type != 1 || token.IsDoubleSymbol || token.Symbol1 != '{') throw new LexicalTreeConstructionException(ref token);
            }
            else if (token.Symbol1 == '{')
                Block.Inherit = "";
            else throw new LexicalTreeConstructionException(ref token);
            var stack = new Stack<Token>();
            int leftCount = 1;
            while (c.MoveNext())
            {
                token = c.Current;
                if (token.Symbol1 == '{') { ++leftCount; stack.Push(token); }
                else if (token.Symbol1 == '}') { if (--leftCount == 0) break; stack.Push(token); }
                else stack.Push(token);
            }
            Block.Children.AddRange(ParseAssigns(stack));
            return Block;
        }
        //副作用として引数が�?�消費され�?
        public static Stack<LexicalTree_Assign> ParseAssigns(Stack<Token> stack)
        {
            Token token;
            var answer = new Stack<LexicalTree_Assign>();
            bool separated = false;
            var tmp = new Stack<Token>();
            while (stack.Count > 0)
            {
                token = stack.Pop();
                switch (token.Type)
                {
                    case 0:
                    case 2:
                        if (!separated && tmp.Count != 0)
                        {
                            var tmpToken = tmp.Pop();
                            tmp.Push(token.Merge(ref tmpToken));
                        }
                        else tmp.Push(token);
                        separated = false;
                        break;
                    case 1:
                        if (token.Symbol1 == ',' || token.Symbol1 == '*' || token.Symbol1 == ';')
                            separated = true;
                        else if (token.Symbol1 == '=')
                        {
                            Token name;
                            if (stack.Count > 0)
                            {
                                name = stack.Pop();
                                Token tmpName;
                                if (stack.Count != 0)
                                {
                                    tmpName = stack.Pop();
                                    if (tmpName.Type == 1 && tmpName.Symbol1 == '@' && tmpName.IsNext(ref name))
                                    {
                                        Token beforeIt;
                                        if (stack.Count != 0)
                                        {
                                            beforeIt = stack.Pop();
                                            if (beforeIt.Symbol1 != '=')
                                            {
                                                name = beforeIt.ForceMerge(ref tmpName).ForceMerge(ref name);
                                            }
                                            else
                                            {
                                                stack.Push(beforeIt);
                                                stack.Push(tmpName);
                                            }
                                        }
                                        else throw new LexicalTreeConstructionException(ref tmpName);
                                    }
                                    else
                                        stack.Push(tmpName);
                                    var tmpAdd = new LexicalTree_Assign(name.ToString()) { File = name.File, Line = name.Line, Column = name.Column };
                                    tmpAdd.Content.AddRange(tmp);
                                    tmp.Clear();
                                    answer.Push(tmpAdd);
                                }
                                else
                                {
                                    var tmpAdd = new LexicalTree_Assign(name.Content) { File = name.File, Line = name.Line, Column = name.Column };
                                    tmpAdd.Content.AddRange(tmp);
                                    tmp.Clear();
                                    answer.Push(tmpAdd);
                                }
                            }
                        }
                        else
                        {
                            if (!separated && tmp.Count != 0)
                            {
                                Token tmpToken = tmp.Pop();
                                tmp.Push(token.Merge(ref tmpToken));
                            }
                            else tmp.Push(token);
                            separated = false;
                        }
                        break;
                }
            }
            return answer;
        }
    }
}