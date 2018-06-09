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

        protected LexicalTree(string name)
        {
            Name = name.ToLower();
        }
    }
    public sealed class LexicalTreeConstructionException : ApplicationException
    {
        public LexicalTreeConstructionException(in Token token) : base($"{token.File}/{token.Line + 1}/{token.Column + 1}") { }
    }
    public class LexicalTree_Content : LexicalTree
    {
        public List<Token> Content { get; } = new List<Token>();
        protected LexicalTree_Content(string name) : base(name) { }
    }
    public sealed class LexicalTree_VariableParen : LexicalTree_Content
    {
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
            public SingleContent(in Token token) : base(token.ToString())
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
                            answer.Children.Add(new LexicalTree_Function(content) { File = token.File, Line = token.Line, Column = token.Column, Variable = c.ParseVariableParen() });
                        }
                        else
                        {
                            answer.Children.Add(new SingleContent(token));
                            goto Top;
                        }
                    }
                    else throw new LexicalTreeConstructionException(token);
                }
                else if (token.Type == 2)
                    answer.Children.Add(new SingleContent(token));
                else if (token.IsSingleSymbol)
                {
                    switch (token.Symbol1)
                    {
                        case ')':
                            --count;
                            if (count != 0)
                                answer.Children.Add(new SingleContent(token));
                            else return answer;
                            break;
                        case '(':
                            ++count;
                            answer.Children.Add(new SingleContent(token));
                            break;
                        case '+':
                            var last = answer.Children.LastOrDefault() as SingleContent;
                            if (last != null)
                            {
                                var last_token = last.Content;
                                if (last_token.Symbol1 == '+' || last_token.Symbol1 == '-' || last_token.Symbol1 == '*' || last_token.Symbol1 == '/' || last_token.Symbol1 == '%' || last_token.Symbol1 == '(' || last_token.Symbol1 == '=' || last_token.Symbol1 == '!' || last_token.Symbol1 == '<' || last_token.Symbol1 == '>')
                                {
                                    if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                    next = c.Current;
                                    if (token.IsNext(ref next) && next.Type == 2)
                                    {
                                        token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, token.Number);
                                        answer.Children.Add(new SingleContent(token));
                                    }
                                    else goto Top;
                                }
                                else answer.Children.Add(new SingleContent(token));
                            }
                            else if (answer.Children.Count == 0)
                            {
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                next = c.Current;
                                if (token.IsNext(ref next) && next.Type == 2)
                                {
                                    token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, token.Number);
                                    answer.Children.Add(new SingleContent(token));
                                }
                                else goto Top;
                            }
                            else answer.Children.Add(new SingleContent(token));
                            break;
                        case '-':
                            var last2 = answer.Children.LastOrDefault() as SingleContent;
                            if (last2 != null)
                            {
                                var last_token2 = last2.Content;
                                if (last_token2.Symbol1 == '+' || last_token2.Symbol1 == '-' || last_token2.Symbol1 == '*' || last_token2.Symbol1 == '/' || last_token2.Symbol1 == '%' || last_token2.Symbol1 == '(' || last_token2.Symbol1 == '=' || last_token2.Symbol1 == '!' || last_token2.Symbol1 == '<' || last_token2.Symbol1 == '>')
                                {
                                    if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                    next = c.Current;
                                    if (token.IsNext(ref next) && next.Type == 2)
                                    {
                                        token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, -token.Number);
                                        answer.Children.Add(new SingleContent(token));
                                    }
                                    else goto Top;
                                }
                                else answer.Children.Add(new SingleContent(token));
                            }
                            else if (answer.Children.Count == 0)
                            {
                                if (!c.MoveNext()) throw new LexicalTreeConstructionException(token);
                                next = c.Current;
                                if (token.IsNext(ref next) && next.Type == 2)
                                {
                                    token = new Token(token.File, token.Line, token.Column, token.IsDebug, token.IsMemo, -token.Number);
                                    answer.Children.Add(new SingleContent(token));
                                }
                                else goto Top;
                            }
                            else answer.Children.Add(new SingleContent(token));
                            break;
                        case '@':
                            if (c.MoveNext())
                            {
                                next = c.Current;
                                if (next.Type != 0 || !token.IsNext(ref next))
                                {
                                    answer.Children.Add(new SingleContent(token));
                                    goto Top;
                                }
                                token = token.Merge(ref next);
                                answer.Children.Add(new SingleContent(token));
                            }
                            else throw new LexicalTreeConstructionException(token);
                            break;
                        default:
                            answer.Children.Add(new SingleContent(token));
                            break;
                    }
                }
                else answer.Children.Add(new SingleContent(token));
            }
            throw new LexicalTreeConstructionException(token);
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
    }
    public sealed class LexicalTree_NameStructure : LexicalTree_Block, IInherit, IStructureDataType
    {
        public LexicalTree_NameStructure(string name) : base(name)
        {
            Type = TreeType.NameStructure;
        }
        public string Inherit { get; set; }
        public StructureDataType Structure { get; set; }
    }
}