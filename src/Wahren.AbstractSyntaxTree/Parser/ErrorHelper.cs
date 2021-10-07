namespace Wahren.AbstractSyntaxTree.Parser;

public static class ErrorHelper
{
    public static void ErrorAdd(ref this Result result, string text, uint tokenId)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new(text, token.Position, token.Length));
    }

    public static void WarningAdd(ref this Result result, string text, uint tokenId)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new(text, token.Position, token.Length, DiagnosticSeverity.Warning));
    }

    public static void InfoAdd(ref this Result result, string text, uint tokenId)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new(text, token.Position, token.Length, DiagnosticSeverity.Info));
    }

    public static void HintAdd(ref this Result result, string text, uint tokenId)
    {
        ref var token = ref result.TokenList[tokenId];
        result.ErrorList.Add(new(text, token.Position, token.Length, DiagnosticSeverity.Hint));
    }

    public static void WarningAdd_MultipleAssignment(ref this Result result, uint tokenId)
    {
#if JAPANESE
        var error = $"要素'{result.GetSpan(tokenId)}'への多重代入は深刻なエラーの原因となりえます。この代入は無視されます。";
#else
        var error = $"Multiple assignment to '{result.GetSpan(tokenId)}' can cause serious error.";
#endif
        result.WarningAdd(error, tokenId);
    }

    public static void ErrorAdd_LastAndLastBut1MustBeOneLine(ref this Result result)
    {
        var lastIndex = result.TokenList.LastIndex;
#if JAPANESE
        var error = $"最後のトークンとその１つ前のトークンは同じ行になくてはいけませんでした。最後のトークン: '{result.GetSpan(lastIndex)}'。その１つ前のトークン: '{result.GetSpan(lastIndex - 1U)}'。";
#else
        var error = $"The last token and the last but 1 token must be on the same line. Last Token: '{result.GetSpan(lastIndex)}'. Last But 1 Token: '{result.GetSpan(lastIndex - 1U)}'.";
#endif
        result.ErrorAdd(error, lastIndex);
    }

    public static void ErrorAdd_UnexpectedEndOfFile(ref this Result result, uint tokenId, ReadOnlySpan<char> text = default)
    {
#if JAPANESE
        var error = $"予期せぬファイル終端です。最後のトークン: {result.GetSpan(result.TokenList.LastIndex)}。{text}";
#else
        var error = $"Unexpected End Of File. Last Token: {result.GetSpan(result.TokenList.LastIndex)}. {text}";
#endif
        result.ErrorAdd(error, tokenId);
    }

    public static void ErrorAdd_UnexpectedEndOfFile_AssignmentOrParenLeftIsExpected(ref this Result result)
    {
#if JAPANESE
        var error = $"予期せぬファイル終端です。'='か'('が求められていました。最後のトークン: {result.GetSpan(result.TokenList.LastIndex)}。";
#else
        var error = $"Unexpected End Of File. '=' or '(' is expected. Last Token: {result.GetSpan(result.TokenList.LastIndex)}.";
#endif
        result.ErrorAdd(error, result.TokenList.LastIndex);
    }

    public static void ErrorAdd_UnexpectedOperatorToken(ref this Result result, uint elementId, string? text = null)
    {
        if (string.IsNullOrEmpty(text))
        {
#if JAPANESE
            text = $"予期せぬ演算子です。最後のトークン: '{result.GetSpan(result.TokenList.LastIndex)}'。";
#else
            text = $"Unexpected Operator. Last Token: {result.GetSpan(result.TokenList.LastIndex)}";
#endif
        }
        else
        {
#if JAPANESE
            text = $"予期せぬ演算子です。最後のトークン: '{result.GetSpan(result.TokenList.LastIndex)}'。{text}";
#else
            text = $"Unexpected Operator. Last Token: {result.GetSpan(result.TokenList.LastIndex)}. {text}";
#endif
        }

        result.ErrorAdd(text, elementId);
    }

    public static void ErrorAdd_AssignmentInConditionalBlock(ref this Result result, uint tokenId)
    {
#if JAPANESE
        var text = $"ブロック{{}}の中での要素'{result.GetSpan(tokenId)}'への代入は意図通りには動作しません。";
#else
        var text = $"Assignment to '{result.GetSpan(tokenId)}' in the conditional block does not behave as you expected.";
#endif
        result.ErrorAdd(text, tokenId);
    }

    public static void ErrorAdd_TooManyArguments(ref this Result result, ActionKind callName, int count, int max, uint callTokenId)
    {
#if JAPANESE
        var text = $"{callName}関数の引数の個数は最大{max}個までですが、{count}個もあります。";
#else
        var text = $"There are too many arguments({count}) for '{callName}'. Max: {max}";
#endif
        result.ErrorAdd(text, callTokenId);
    }

    public static void ErrorAdd_TooManyArguments(ref this Result result, FunctionKind callName, int count, int max, uint callTokenId)
    {
#if JAPANESE
        var text = $"{callName}関数の引数の個数は最大{max}個までですが、{count}個もあります。";
#else
        var text = $"There are too many arguments({count}) for '{callName}'. Max: {max}";
#endif
        result.ErrorAdd(text, callTokenId);
    }

    public static void ErrorAdd_TooLessArguments(ref this Result result, FunctionKind callName, int count, int min, uint callTokenId)
    {
#if JAPANESE
        var text = $"{callName}関数の引数の個数は最低{min}個必要ですが、{count}個しかありません。";
#else
        var text = $"There are too less arguments({count}) for '{callName}'. Min: {min}";
#endif
        result.ErrorAdd(text, callTokenId);
    }

    public static void ErrorAdd_TooLessArguments(ref this Result result, ActionKind callName, int count, int min, uint callTokenId)
    {
#if JAPANESE
        var text = $"{callName}関数の引数の個数は最低{min}個必要ですが、{count}個しかありません。";
#else
        var text = $"There are too less arguments({count}) for '{callName}'. Min: {min}";
#endif
        result.ErrorAdd(text, callTokenId);
    }

    public static void ErrorAdd_CoresspondingNextDoesNotExist(ref this Result result, uint tokenId)
    {
#if JAPANESE
        var text = $"battleブロックを記述する場合、先行してnext()関数の呼び出しが必要とwikiに記述されていますが見当たりません。";
#else
        var text = $"'battle{}' block does not have corresponding 'next()' statement.";
#endif
        result.ErrorAdd(text, tokenId);
    }

    public static void ErrorAdd_EssentialElementDoesNotExist(ref this Result result, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName, uint tokenId)
    {
#if JAPANESE
        var text = $"{nodeKind}構造体'{result.GetSpan(tokenId)}'に必須である要素'{elementName}'が記述されていません。";
#else
        var text = $"'{elementName}' does not exist in the {nodeKind} structure '{result.GetSpan(tokenId)}'.";
#endif
        result.ErrorAdd(text, tokenId);
    }

    public static void ErrorAdd_ArgumentDoesNotExist(ref this Result result, char c0, char c1, uint tokenId)
    {
#if JAPANESE
        var text = $"'{c0}'と'{c1}'の間に引数が記述されていません。";
#else
        var text = $"Argument does not exists between '{c0}' and '{c1}'.";
#endif
        result.ErrorAdd(text, tokenId);
    }

    public static void ErrorAdd_ValueDoesNotExistAfterAssignment(ref this Result result, uint elementId)
    {
#if JAPANESE
        var text = $"要素'{result.GetSpan(elementId)}'の'='の後に値が記述されていません。";
#else
        var text = $"Element must have value. There is no value text after '='.";
#endif
        result.ErrorAdd(text, elementId);
    }

    public static void ErrorAdd_StructureKindOrCommentIsExpected(ref this Result result)
    {
#if JAPANESE
        var text = $"構造体の種別か、あるいはコメント(//, /+, /*)が記述されているべきですが、実際は'{result.GetSpan(result.TokenList.LastIndex)}'と記述されていました。";
#else
        var text = $"Structure kind or comment start is necessary.";
#endif
        result.ErrorAdd(text, result.TokenList.LastIndex);
    }

    public static void ErrorAdd_TooManyBracketRight(ref this Result result)
    {
#if JAPANESE
        var text = $"'{{'と対応しない'}}'が見つかりました。括弧の対応関係を見直してください。";
#else
        var text = $"Too many '}}'. It does not have corresponding '{{'.";
#endif
        result.ErrorAdd(text, result.TokenList.LastIndex);
    }

    public static void ErrorAdd_UnexpectedElementName(ref this Result result, uint kindId, uint elementId)
    {
        string text;
        var name = result.GetSpan(elementId);
        if (name.Length != 0 && char.IsWhiteSpace(name[0]))
        {
#if JAPANESE
            text = $"'{name}'が空白文字列から始まってはいませんか？ {result.GetSpan(kindId)}構造体は要素'{name}'を持たないはずです。";
#else
            text = $"'{name}' starts with whitespace(s). '{result.GetSpan(kindId)}' structure cannot have element '{name}'.";
#endif
        }
        else
        {
#if JAPANESE
            text = $"{result.GetSpan(kindId)}構造体は要素'{name}'を持たないはずです。";
#else
            text = $"'{result.GetSpan(kindId)}' structure cannot have element '{name}'.";
#endif
        }

        result.ErrorAdd(text, elementId);
    }

    public static void ErrorAdd_UnexpectedCall(ref this Result result, uint callId)
    {
        string text;
        var name = result.GetSpan(callId);
        if (name.Length != 0 && char.IsWhiteSpace(name[0]))
        {
#if JAPANESE
            text = $"'{name}'が空白文字列から始まってはいませんか？ {name}関数という関数は存在しません。";
#else
            text = $"'{name}' starts with whitespace(s). function/action '{name}' does not exist.";
#endif
        }
        else
        {
#if JAPANESE
            text = $"{name}関数という関数は存在しません。";
#else
            text = $"function/action '{name}' does not exist.";
#endif
        }

        result.ErrorAdd(text, callId);
    }

    public static void ErrorAdd_UnexpectedElementReferenceKind(ref this Result result, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName, ReadOnlySpan<char> referenceKind, uint tokenId)
    {
#if JAPANESE
        var text = $"{nodeKind}構造体の要素'{elementName}'の値'{result.GetSpan(tokenId)}'は期待される型{referenceKind}ではありません。";
#else
        var text = $"Value '{result.GetSpan(tokenId)}' is not {referenceKind} required by element '{elementName}' of struct {nodeKind}.";
#endif
        result.ErrorAdd(text, tokenId);
    }

    public static void ErrorAdd_UnexpectedElementSpecialValue(ref this Result result, ReadOnlySpan<char> nodeKind, ReadOnlySpan<char> elementName, ReadOnlySpan<char> values, uint tokenId)
    {
#if JAPANESE
        var text = $"{nodeKind}構造体の要素'{elementName}'の値'{result.GetSpan(tokenId)}'は期待される値{values}ではありません。";
#else
        var text = $"Value '{result.GetSpan(tokenId)}' is not {values} required by element '{elementName}' of struct {nodeKind}.";
#endif
        result.ErrorAdd(text, tokenId);
    }

    public static void ErrorAdd_UnexpectedArgumentReferenceKind(ref this Result result, ReadOnlySpan<char> action, int argumentIndex, ReadOnlySpan<char> referenceKind, uint tokenId)
    {
#if JAPANESE
        var text = $"{action}関数の{argumentIndex}番目の引数の値'{result.GetSpan(tokenId)}'は期待される型{referenceKind}ではありません。";
#else
        var text = $"The value '{result.GetSpan(tokenId)}' of the action {action}'s {argumentIndex}-th argument is not {referenceKind}.";
#endif
        result.ErrorAdd(text, tokenId);
    }

    public static void WarningAdd_MustBeInWhileBlock(ref this Result result, ReadOnlySpan<char> kind, uint tokenId)
    {
#if JAPANESE
        var text = $"{kind}()はwhileブロック内に記述されるべきです。";
#else
        var text = $"{kind}() statement must be in while loop.";
#endif
        result.WarningAdd(text, tokenId);
    }

    public static void WarningAdd_UnexpectedElementName(ref this Result result, uint kindId, uint elementId)
    {
        string text;
        var name = result.GetSpan(elementId);
        if (name.Length != 0 && char.IsWhiteSpace(name[0]))
        {
#if JAPANESE
            text = $"'{name}'が空白文字列から始まってはいませんか？ {result.GetSpan(kindId)}構造体は要素'{name}'を持たないはずです。";
#else
            text = $"'{name}' starts with whitespace(s). '{result.GetSpan(kindId)}' structure cannot have element '{name}'.";
#endif
        }
        else
        {
#if JAPANESE
            text = $"{result.GetSpan(kindId)}構造体は要素'{name}'を持たないはずです。";
#else
            text = $"'{result.GetSpan(kindId)}' structure cannot have element '{name}'.";
#endif
        }
        result.WarningAdd(text, elementId);
    }

    public static void ErrorAdd_VariantNotAllowed(ref this Result result, uint kindId, uint elementId)
    {
#if JAPANESE
        var error = $"{result.GetSpan(kindId)}構造体の要素'{result.GetSpan(elementId)}'は'@'以下にバリエーションを持ってはなりません。";
#else
        var error = $"{result.GetSpan(kindId)}'s element '{result.GetSpan(elementId)}' must not have variation after '@'.";
#endif
        result.ErrorAdd(error, elementId);
    }

    public static void ErrorAdd_BracketRightNotFound(ref this Result result, uint kindId)
    {
#if JAPANESE
        var error = $"{result.GetSpan(kindId)}構造体の '}}' がファイル末尾まで探しても見つかりませんでした。";
#else
        var error = $"{result.GetSpan(kindId)}'s '}}' is not found. Unexpected End Of File.";
#endif
        result.ErrorAdd(error, kindId);
    }

    public static void ErrorAdd_BracketRightNotFound(ref this Result result, uint kindId, uint nameId)
    {
#if JAPANESE
        var error = $"{result.GetSpan(kindId)}構造体 '{result.GetSpan(nameId)}'の '}}' がファイル末尾まで探しても見つかりませんでした。";
#else
        var error = $"{result.GetSpan(kindId)} {result.GetSpan(nameId)}'s '}}' is not found. Unexpected End Of File.";
#endif
        ref var token = ref result.TokenList[kindId];
        result.ErrorAdd(error, kindId);
    }

    public static void ErrorAdd_Statement_BracketRightNotFound(ref this Result result, uint statementId, ReadOnlySpan<char> statement)
    {
#if JAPANESE
        var error = $"{statement}文の '}}' がファイル末尾まで探しても見つかりませんでした。";
#else
        var error = $"{statement}'s '}}' is not found. Unexpected End Of File.";
#endif
        result.ErrorAdd(error, statementId);
    }

    public static void ErrorAdd_NumberIsExpected(ref this Result result, uint tokenId, ReadOnlySpan<char> postText = default)
    {
#if JAPANESE
        var error = $"ここには数値が記述されるべきでしたが、実際は'{result.GetSpan(tokenId)}'と書いてありました。{postText}";
#else
        var error = $"Number text is expected but actually '{result.GetSpan(tokenId)}'.{postText}";
#endif
        result.ErrorAdd(error, tokenId);
    }

    public static void ErrorAdd_BooleanIsExpected(ref this Result result, uint tokenId, ReadOnlySpan<char> postText = default)
    {
#if JAPANESE
        var error = $"ここにはon/offが記述されるべきでしたが、実際は'{result.GetSpan(tokenId)}'と書いてありました。{postText}";
#else
        var error = $"Boolean text is expected but actually '{result.GetSpan(tokenId)}'.{postText}";
#endif
        result.ErrorAdd(error, tokenId);
    }

    public static void ErrorAdd_CommaIsExpected(ref this Result result, uint tokenId, ReadOnlySpan<char> postText = default)
    {
#if JAPANESE
        var error = $"ここには','が記述されるべきでしたが、実際は'{result.GetSpan(tokenId)}'と書いてありました。{postText}";
#else
        var error = $"',' is expected but actually '{result.GetSpan(tokenId)}'.{postText}";
#endif
        result.ErrorAdd(error, tokenId);
    }

    public static void ErrorAdd_ParenRightIsExpected(ref this Result result, uint tokenId, ReadOnlySpan<char> postText = default)
    {
#if JAPANESE
        var error = $"ここには')'が記述されるべきでしたが、実際は'{result.GetSpan(tokenId)}'と書いてありました。{postText}";
#else
        var error = $"')' is expected but actually '{result.GetSpan(tokenId)}'.{postText}";
#endif
        result.ErrorAdd(error, tokenId);
    }
}
