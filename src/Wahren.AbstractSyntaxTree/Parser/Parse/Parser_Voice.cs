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

        ref var tokenList = ref result.TokenList;
        tokenList.GetKind(element.ElementTokenId) = TokenKind.ROAM;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_ValueDoesNotExistAfterAssignment(element.ElementTokenId);
            return false;
        }

        tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
        element.HasValue = true;
        element.Value.Add(new(tokenList.LastIndex));

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                return false;
            }

            if (result.IsSemicolon(tokenList.LastIndex))
            {
                if (element.Value.Count == 1 && result.IsAtmark(element.Value[0].Text))
                {
                    element.Value.Clear();
                    element.HasValue = false;
                }

                return true;
            }

            if (result.IsComma(tokenList.LastIndex) && result.IsEndOfLine(tokenList.LastIndex))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(element.ElementTokenId);
                    return false;
                }

                tokenList.GetKind(tokenList.LastIndex) = TokenKind.Content;
                element.Value.Add(new(tokenList.LastIndex));
            }
            else
            {
                result.UnionLast2Tokens();
            }
        } while (true);
    }
}
