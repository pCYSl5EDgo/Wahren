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
    }
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
    public sealed class LexicalTree_BoolParen : LexicalTree_Block
    {
        public LexicalTree_BoolParen() : base("") { Type = LexicalTree.TreeType.BoolParen; }
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