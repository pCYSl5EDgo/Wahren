using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagePack;

namespace Wahren
{
    [MessagePackObject]
    public class LexicalTree : IName, IDebugInfo
    {
        [Key(0)]
        private string _name;
        [Key(1)]
        private string _file;

        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
        public string File { get => _file; set => _file = String.Intern(value); }

        [Key(2)]
        public int Line { get; set; }

        [Key(3)]
        public int Column { get; set; }
        [IgnoreMember][System.Runtime.Serialization.IgnoreDataMember]
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

        [Key(4)]
        public TreeType Type { get; set; }

        [Key(5)]
        public string Name { get => _name; set => _name = String.Intern(value.ToLower()); }

        protected LexicalTree(string name) => Name = name;
        [SerializationConstructor]
        public LexicalTree() { }
    }
    public sealed class LexicalTreeConstructionException : ApplicationException
    {
        public LexicalTreeConstructionException(in Token token) : base($"{token.File}/{token.Line + 1}/{token.Column + 1}") { }
    }
    [MessagePackObject]
    public class LexicalTree_Content : LexicalTree
    {
        [Key(6)]
        public List<Token> Content { get; } = new List<Token>();
        protected LexicalTree_Content(string name) : base(name) { }
    }
    [MessagePackObject]
    public sealed class LexicalTree_VariableParen : LexicalTree_Content
    {
        public LexicalTree_VariableParen(string name) : base(name)
        {
            Type = TreeType.VariableParen;
        }
    }
    [MessagePackObject]
    public sealed class LexicalTree_Assign : LexicalTree_Content
    {
        public LexicalTree_Assign(string name) : base(name)
        {
            Type = TreeType.Assign;
        }
        [SerializationConstructor]
        public LexicalTree_Assign() : base("") { }
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
    [MessagePackObject]
    public sealed class SingleContent : LexicalTree
    {
        [Key(6)]
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
    [MessagePackObject]
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
    [MessagePackObject]
    public sealed class LexicalTree_Function : LexicalTree
    {
        public LexicalTree_Function(string name) : base(name)
        {
            Type = TreeType.Function;
        }
        [Key(6)]
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
    [MessagePackObject]
    public class LexicalTree_Block : LexicalTree
    {
        public LexicalTree_Block(string name) : base(name)
        {
            Type = TreeType.Block;
        }
        [Key(6)]
        public List<LexicalTree> Children { get; } = new List<LexicalTree>();
    }
    [MessagePackObject]
    public sealed class LexicalTree_Statement : LexicalTree_Block
    {
        public LexicalTree_Statement(string name) : base(name)
        {
            Type = TreeType.Statement;
        }
        [Key(7)]
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
    [Union(0, typeof(LexicalTree_NoNameStructure))]
    [Union(1, typeof(LexicalTree_NameStructure))]
    public interface IStructureDataType
    {
        StructureDataType Structure { get; }
    }
    [MessagePackObject]
    public sealed class LexicalTree_NoNameStructure : LexicalTree_Block, IStructureDataType
    {
        public LexicalTree_NoNameStructure(string name) : base(name)
        {
            Type = TreeType.NoNameStructure;
        }
        [Key(7)]
        public StructureDataType Structure { get; set; }
    }
    [MessagePackObject]
    public sealed class LexicalTree_NameStructure : LexicalTree_Block, IInherit, IStructureDataType
    {
        public LexicalTree_NameStructure(string name) : base(name)
        {
            Type = TreeType.NameStructure;
        }
        [Key(7)]
        public StructureDataType Structure { get; set; }
        [Key(8)]
        public string Inherit { get; set; }
    }
}