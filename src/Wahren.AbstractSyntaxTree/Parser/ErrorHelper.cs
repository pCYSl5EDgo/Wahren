namespace Wahren.AbstractSyntaxTree.Parser;

public static class ErrorHelper
{
    public static void ErrorAdd(ref this Result result, string text, uint tokenId)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new(text, token.Range.StartInclusive, token.Length));
    }

    public static void WarningAdd(ref this Result result, string text, uint tokenId)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new(text, token.Range.StartInclusive, token.Length, DiagnosticSeverity.Warning));
    }

    public static void WarningAdd_MultipleAssignment(ref this Result result, uint tokenId)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new("Multiple assignment can cause serious error.", token.Range.StartInclusive, token.Length, DiagnosticSeverity.Warning));
    }

    public static void ErrorAdd_UnexpectedEndOfFile(ref this Result result, uint tokenId, ReadOnlySpan<char> text = default)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new($"Unexpected End Of File. Last Token: {result.GetSpan(result.TokenList.LastIndex)}. {text}", token.Range.StartInclusive, token.Length));
    }

    public static void ErrorAdd_UnexpectedOperatorToken(ref this Result result, uint elementId, string? text = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            text = $"Unexpected Operator. Last Token: {result.GetSpan(result.TokenList.LastIndex)}";
        }
        else
        {
            text = $"Unexpected Operator. Last Token: {result.GetSpan(result.TokenList.LastIndex)}. {text}";
        }

        ref var token = ref result.TokenList[elementId];
        result.ErrorList.Add(new(text, token.Range.StartInclusive, token.Length));
    }

    public static void ErrorAdd_UnexpectedElementName(ref this Result result, uint kindId, uint elementId)
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

        ref var token = ref result.TokenList[elementId];
        result.ErrorList.Add(new(text, token.Range.StartInclusive, token.Length));
    }

    public static void WarningAdd_UnexpectedElementName(ref this Result result, uint kindId, uint elementId)
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
        ref var token = ref result.TokenList[elementId];
        result.ErrorList.Add(new(text, token.Range.StartInclusive, token.Length, DiagnosticSeverity.Warning));
    }

    public static void ErrorAdd_BracketRightNotFound(ref this Result result, uint kindId)
    {
        ref var token = ref result.TokenList[kindId];
        var text = $"{result.GetSpan(kindId)}'s '}}' is not found. Unexpected End Of File.";
        result.ErrorList.Add(new(text, token.Range.StartInclusive, token.Length));
    }

    public static void ErrorAdd_BracketRightNotFound(ref this Result result, uint kindId, uint nameId)
    {
        var text = $"{result.GetSpan(kindId)} {result.GetSpan(nameId)}'s '}}' is not found. Unexpected End Of File.";
        ref var token = ref result.TokenList[kindId];
        result.ErrorList.Add(new(text, token.Range.StartInclusive, token.Length));
    }

    public static void ErrorAdd_NumberIsExpected(ref this Result result, uint tokenId, ReadOnlySpan<char> postText = default)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new($"Number text is expected but actually \"{result.GetSpan(tokenId)}\".{postText}", token.Range.StartInclusive, token.Length));
    }

    public static void ErrorAdd_BooleanIsExpected(ref this Result result, uint tokenId, ReadOnlySpan<char> postText = default)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new($"Boolean text is expected but actually \"{result.GetSpan(tokenId)}\".{postText}", token.Range.StartInclusive, token.Length));
    }

    public static void ErrorAdd_CommaIsExpected(ref this Result result, uint tokenId, ReadOnlySpan<char> postText = default)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new($"',' is expected but actually \"{result.GetSpan(tokenId)}\".{postText}", token.Range.StartInclusive, token.Length));
    }
}
