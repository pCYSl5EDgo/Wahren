namespace Wahren.AbstractSyntaxTree.Parser;

public static class ErrorHelper
{
    public static void WarningAdd_MultipleAssignment(ref this Result result, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        var text = "Multiple assignment can cause serious error.";
        result.ErrorList.Add(new(text, result.TokenList[elementId].Range, DiagnosticSeverity.Warning, ErrorCode.Syntax, callerFilePath, callerLineNumber));
    }

    public static void ErrorAdd_UnexpectedEndOfFile(ref this Result result, uint tokenId, string? text = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        if (string.IsNullOrEmpty(text))
        {
            text = "Unexpected End Of File. Last Token: " + result.TokenList.Last.ToString(ref result.Source);
        }
        else
        {
            text = "Unexpected End Of File. Last Token: " + result.TokenList.Last.ToString(ref result.Source) + text;
        }

        result.ErrorList.Add(new(text, result.TokenList[tokenId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_UnexpectedOperatorToken(ref this Result result, uint elementId, string? text = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        if (string.IsNullOrEmpty(text))
        {
            text = "Unexpected Operator. Last Token: " + result.TokenList.Last.ToString(ref result.Source);
        }
        else
        {
            text = "Unexpected Operator. Last Token: " + result.TokenList.Last.ToString(ref result.Source) + text;
        }

        result.ErrorList.Add(new(text, result.TokenList[elementId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_UnexpectedElementName(ref this Result result, uint kindId, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        ref var tokenList = ref result.TokenList;
        ref var element = ref tokenList[elementId];
        string text;
        var name = element.ToString(ref result.Source);
        if (name.Length != 0 && char.IsWhiteSpace(name[0]))
        {
            text = $"'{name}' starts with whitespace(s). '{tokenList[kindId].ToString(ref result.Source)}' structure cannot have element '{name}'.";
        }
        else
        {
            text = $"'{tokenList[kindId].ToString(ref result.Source)}' structure cannot have element '{name}'.";
        }
        result.ErrorList.Add(new(text, element.Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void WarningAdd_UnexpectedElementName(ref this Result result, uint kindId, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        ref var tokenList = ref result.TokenList;
        ref var element = ref tokenList[elementId];
        string text;
        var name = element.ToString(ref result.Source);
        if (name.Length != 0 && char.IsWhiteSpace(name[0]))
        {
            text = $"'{name}' starts with whitespace(s). '{tokenList[kindId].ToString(ref result.Source)}' structure cannot have element '{name}'.";
        }
        else
        {
            text = $"'{tokenList[kindId].ToString(ref result.Source)}' structure cannot have element '{name}'.";
        }
        result.ErrorList.Add(new(text, element.Range, DiagnosticSeverity.Warning, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_BracketRightNotFound(ref this Result result, uint kindId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        var text = $"{result.TokenList[kindId].ToString(ref result.Source)}'s '}}' is not found. Unexpected End Of File.";
        result.ErrorList.Add(new(text, result.TokenList[kindId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_BracketRightNotFound(ref this Result result, uint kindId, uint nameId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        var text = $"{result.TokenList[kindId].ToString(ref result.Source)} {result.TokenList[nameId].ToString(ref result.Source)}'s '}}' is not found. Unexpected End Of File.";
        result.ErrorList.Add(new(text, result.TokenList[nameId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_NumberIsExpected(ref this Result result, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        ref var last = ref result.TokenList.Last;
        var text = PooledStringBuilder.Rent()
            .Append("Number text is expected but actually \"")
            .Append(result.Source[last.Range.StartInclusive.Line].AsSpan(last.Range.StartInclusive.Offset, last.LengthInFirstLine))
            .Append('\"').ToString();

        result.ErrorList.Add(new(text, result.TokenList[elementId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void ErrorAdd_CommaIsExpected(ref this Result result, uint elementId, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        ref var last = ref result.TokenList.Last;
        var text = PooledStringBuilder.Rent()
            .Append("',' is expected but actually \"")
            .Append(result.Source[last.Range.StartInclusive.Line].AsSpan(last.Range.StartInclusive.Offset, last.LengthInFirstLine))
            .Append('\"').ToString();

        result.ErrorList.Add(new(text, result.TokenList[elementId].Range, InternalCSharpFilePath: callerFilePath, InternalCSharpLineNumber: callerLineNumber));
    }

    public static void WarningAdd_SpacesBetweenFunctionNameAndLeftParenAreNotNecessary(ref this Result result, uint functionTokenId)
    {

    }
}
