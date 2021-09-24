namespace Wahren.AbstractSyntaxTree.Parser;

public static class ErrorHelper
{
    public static void WarningAdd_MultipleAssignment(ref this Result result, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        result.ErrorList.Add(new("Multiple assignment can cause serious error.", result.TokenList[elementId].Range, DiagnosticSeverity.Warning, ErrorCode.Syntax, callerFilePath, callerLineNumber));
    }

    public static void ErrorAdd_UnexpectedEndOfFile(ref this Result result, uint tokenId, string? text = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        if (string.IsNullOrEmpty(text))
        {
            text = $"Unexpected End Of File. Last Token: {result.GetSpan(result.TokenList.LastIndex)}";
        }
        else
        {
            text = $"Unexpected End Of File. Last Token: {result.GetSpan(result.TokenList.LastIndex)}. {text}";
        }

        result.ErrorList.Add(new(text, result.TokenList[tokenId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_UnexpectedOperatorToken(ref this Result result, uint elementId, string? text = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        if (string.IsNullOrEmpty(text))
        {
            text = $"Unexpected Operator. Last Token: {result.GetSpan(result.TokenList.LastIndex)}";
        }
        else
        {
            text = $"Unexpected Operator. Last Token: {result.GetSpan(result.TokenList.LastIndex)}. {text}";
        }

        result.ErrorList.Add(new(text, result.TokenList[elementId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_UnexpectedElementName(ref this Result result, uint kindId, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        string text;
        var name = result.GetSpan(elementId);
        if (name.Length != 0 && char.IsWhiteSpace(name[0]))
        {
            text = $"'{name}' starts with whitespace(s). '{result.GetSpan(kindId)}' structure cannot have element '{name}'.";
        }
        else
        {
            text = $"'{result.GetSpan(kindId)}' structure cannot have element '{name}'.";
        }
        result.ErrorList.Add(new(text, result.TokenList[elementId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void WarningAdd_UnexpectedElementName(ref this Result result, uint kindId, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        string text;
        var name = result.GetSpan(elementId);
        if (name.Length != 0 && char.IsWhiteSpace(name[0]))
        {
            text = $"'{name}' starts with whitespace(s). '{result.GetSpan(kindId)}' structure cannot have element '{name}'.";
        }
        else
        {
            text = $"'{result.GetSpan(kindId)}' structure cannot have element '{name}'.";
        }
        result.ErrorList.Add(new(text, result.TokenList[elementId].Range, DiagnosticSeverity.Warning, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_BracketRightNotFound(ref this Result result, uint kindId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        var text = $"{result.GetSpan(kindId)}'s '}}' is not found. Unexpected End Of File.";
        result.ErrorList.Add(new(text, result.TokenList[kindId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_BracketRightNotFound(ref this Result result, uint kindId, uint nameId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        var text = $"{result.GetSpan(kindId)} {result.GetSpan(nameId)}'s '}}' is not found. Unexpected End Of File.";
        result.ErrorList.Add(new(text, result.TokenList[nameId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_NumberIsExpected(ref this Result result, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        result.ErrorList.Add(new($"Number text is expected but actually \"{result.GetSpan(result.TokenList.LastIndex)}\"", result.TokenList[elementId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_CommaIsExpected(ref this Result result, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        result.ErrorList.Add(new($"',' is expected but actually \"{result.GetSpan(result.TokenList.LastIndex)}\"", result.TokenList[elementId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }
}
