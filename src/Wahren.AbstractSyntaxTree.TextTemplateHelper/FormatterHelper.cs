using System.Linq;

namespace Wahren.AbstractSyntaxTree.TextTemplateHelper;

public struct NameContentPair
{
    public string name;
    public string content;

    public NameContentPair(string name, string content) : this()
    {
        this.name = name;
        this.content = content;
    }
}

public struct NameContentPair2
{
    public string name0;
    public string content0;
    public string name1;
    public string content1;

    public NameContentPair2(string name0, string content0, string name1, string content1) : this()
    {
        this.name0 = name0;
        this.content0 = content0;
        this.name1 = name1;
        this.content1 = content1;
    }
}

public static class FormatterHelper
{
    public static readonly string[] NameNodes = new string[]
    {
        "spot",
        "unit",
        "race",
        "class",
        "field", 
        "skill",
        "power",
        "voice",
        "object",
        "dungeon",
        "movetype",
        "skillset",
        "story",
        "fight",
        "world",
        "event",
        "scenario",
    };

    public static readonly string[] BlockNodes = new string[] {
        "context",
        "workspace",
        "attribute",
        "sound",
        "detail",
    };

    public static readonly NameContentPair[] Operators = new NameContentPair[] {
        new("Mul", "*"),
        new("Add", "+"),
        new("Sub", "-"),
        new("Div", "/"),
        new("Percent", "%"),
        new("And", "&&"),
        new("Or", "||"),
        new("CompareEqual", "=="),
        new("CompareNotEqual", "!="),
        new("CompareGreaterThan", ">"),
        new("CompareGreaterThanOrEqualTo", ">="),
        new("CompareLessThan", "<"),
        new("CompareLessThanOrEqualTo", "<="),
    };

    static FormatterHelper()
    {
        var pairs = new List<NameContentPair>
        {
            new("Space", " "),
            new("Space_Assign", " ="),
            new("Semicolon", ";"),
            new("ParenLeft", "("),
            new("BracketLeft", "{"),
            new("ParenRight", ")"),
            new("else_Space_if_ParenLeft", "else if ("),
            new("else_Space_rif_ParenLeft", "else rif ("),
            new("Comma", ","),
            new("Comma_Space", ", ")
        };
        foreach (var x in new NameContentPair[]{
            new("Assign", "="),
            new("Colon", ":"),
        }.Concat(Operators))
        {
            pairs.Add(new("Space_" + x.name + "_Space", " " + x.content + " "));
        }
        foreach (var name in new string[]{
            "if",
            "rif",
            "while",
        })
        {
            pairs.Add(new(name + "_Space_ParenLeft", name + " ("));
        }
        foreach (var name in NameNodes)
        {
            pairs.Add(new(name + "_Space", name + " "));
        };
        foreach (var x in CallableInfo.ActionInfoNormals)
        {
            pairs.Add(new(x.Name + "_ParenLeft", x.Name + "("));
        }
        foreach (var x in CallableInfo.FunctionInfoNormals)
        {
            pairs.Add(new(x.Name + "_ParenLeft", x.Name + "("));
        }
        
        Pairs = pairs.ToArray();

        var pairs_NewLine = new List<NameContentPair>
        {
            new("BracketRight", "}"),
            new("BracketLeft", "{"),
            new("else", "else"),
            new("battle", "battle")
        };
        foreach (var name in new string[] {
            "next",
            "return",
            "continue",
            "break",
        })
        {
            pairs_NewLine.Add(new(name + "_ParenLeft_ParenRight", name + "()"));
        }

        Pairs_NewLine = pairs_NewLine.ToArray();

        var pairs_NewLine_Pairs = new List<NameContentPair2>();
        foreach (var name in BlockNodes)
        {
            pairs_NewLine_Pairs.Add(new(name, name, "BracketLeft", "{"));
        };

        Pairs_NewLine_Pairs = pairs_NewLine_Pairs.ToArray();
    }
    
    public static readonly NameContentPair[] Pairs;
    public static readonly NameContentPair[] Pairs_NewLine;
    public static readonly NameContentPair2[] Pairs_NewLine_Pairs;
}
