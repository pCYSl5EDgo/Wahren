namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    /// <summary>
    /// Already read '='.
    /// </summary>
    private static bool Parse_Voice_ROAM_EnglishMode(ref Context context, ref Result result, Pair_NullableString_NullableInt_ArrayElement element)
    {
        if (!context.IsEnglishMode)
        {
            return Parse_Element_ROAM(ref context, ref result, element);
        }

        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[element.ElementTokenId].Kind = TokenKind.ROAM;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd("Element must have value. There is no value text after '='.", element.ElementTokenId);
            return false;
        }

        tokenList.Last.Kind = TokenKind.Content;
        element.HasValue = true;
        element.Value.Add(new(tokenList.LastIndex));

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (tokenList.Last.IsSemicolon(ref source))
            {
                if (element.Value.Count == 1 && tokenList[element.Value[0].Text].IsAtmark(ref source))
                {
                    element.Value.Clear();
                    element.HasValue = false;
                }

                return true;
            }

            if (tokenList.Last.IsComma(ref source) && result.IsEndOfLine(tokenList.LastIndex))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                    return false;
                }

                tokenList.Last.Kind = TokenKind.Content;
                element.Value.Add(new(tokenList.LastIndex));
            }
            else
            {
                result.UnionLast2Tokens();
            }
        } while (true);
    }
}
