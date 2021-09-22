﻿using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    /// <summary>
    /// Already read '='.
    /// </summary>
    private static bool Parse_Discard(ref Context context, ref Result result, uint elementTokenId)
    {
        if (!SplitElement(ref result, elementTokenId, out var span, out _))
        {
            return false;
        }

        Span<byte> byteSpan = MemoryMarshal.Cast<char, byte>(span);
        switch (span.Length)
        {
            case 0: return false;
            case 1: return Parse_Discard(ref context, ref result, elementTokenId, ReadOnlySpan<char>.Empty, BinaryPrimitives.ReadUInt16LittleEndian(byteSpan));
            case 2: return Parse_Discard(ref context, ref result, elementTokenId, ReadOnlySpan<char>.Empty, BinaryPrimitives.ReadUInt32LittleEndian(byteSpan));
            case 3: return Parse_Discard(ref context, ref result, elementTokenId, ReadOnlySpan<char>.Empty, BinaryPrimitives.ReadUInt32LittleEndian(byteSpan) | ((ulong)BinaryPrimitives.ReadUInt16LittleEndian(byteSpan.Slice(4)) << 32));
            default: return Parse_Discard(ref context, ref result, elementTokenId, span.Slice(4), BinaryPrimitives.ReadUInt64LittleEndian(byteSpan));
        }
    }

    /// <summary>
    /// Already read '='. Already split element.
    /// element: [ ray, poli, camp, home, multi, learn, skill, color, joint, weapon, skill2, weapon2, activenum, friend_ex ]
    /// </summary>
    private static bool Parse_Discard_RAY(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList[elementTokenId].Kind = TokenKind.RAY;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorList.Add(new("Element must have value. There is no value text after '='.", result.TokenList[elementTokenId].Range));
            return false;
        }

        ref var source = ref result.Source;
        ref var thisLine = ref source[tokenList.Last.Range.StartInclusive.Line];
        do
        {
            tokenList.Last.Kind = TokenKind.Content;
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (tokenList.Last.Range.StartInclusive.Line != tokenList[tokenList.LastIndex - 1].Range.StartInclusive.Line)
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            if (!tokenList.Last.IsComma(ref source))
            {
                result.ErrorAdd_UnexpectedOperatorToken(elementTokenId);
                return false;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }
        } while (true);
    }

    /// <summary>
    /// Already read '='.
    /// structure: attribute
    /// element: [ loyal, change, fkey, str, arbeit, brave, ground, gun_delay ]
    /// </summary>
    private static bool Parse_Discard_LOYAL(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[elementTokenId].Kind = TokenKind.LOYAL;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorList.Add(new("Element must have value. There is no value text after '='.", result.TokenList[elementTokenId].Range));
            return false;
        }

        tokenList.Last.Kind = TokenKind.Content;
        if (tokenList.Last.TryParse(ref source, out _))
        {
            if (tokenList.Last.Range.EndExclusive.Offset != 0)
            {
                result.ErrorList.Add(new("Line feed must be next to number value in this kind of element. Neither text nor comment are allowed.", tokenList.Last.Range));
                return false;
            }
        }

        var textIndex = tokenList.LastIndex;
        while (tokenList.Last.Range.EndExclusive.Offset != 0)
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(textIndex);
                return false;
            }

            if (!tokenList.Last.IsMul(ref source))
            {
                result.UnionLast2Tokens();
                continue;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(textIndex, "Number text is expected after '*'.");
                return false;
            }

            tokenList.Last.Kind = TokenKind.Content;
            if (!tokenList.Last.TryParse(ref source, out _))
            {
                result.ErrorList.Add(new("Number text must follows '*'.", tokenList[textIndex].Range));
            }

            if (tokenList.Last.Range.EndExclusive.Offset == 0)
            {
                break;
            }

            result.ErrorList.Add(new("Line feed must be next to number value in this kind of element. Neither text nor comment are allowed.", tokenList[textIndex].Range));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Already read '='. Already Split.
    /// detail
    /// element: [ text ]
    /// </summary>
    private static bool Parse_Discard_TEXT(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[elementTokenId].Kind = TokenKind.TEXT;
        tokenList.Add(new());
        if (!Lexer.ReadTokenSemicolon(ref source, ref context.Position, ref tokenList.Last))
        {
            tokenList.RemoveLast();
            result.ErrorAdd_UnexpectedEndOfFile(elementTokenId, "';' is expected.");
            return false;
        }

        tokenList.Last.Kind = TokenKind.Content;
        if (!ReadUsefulToken(ref context, ref result) || !tokenList.Last.IsSemicolon(ref source))
        {
            throw new InvalidOperationException();
        }

        return true;
    }

    /// <summary>
    /// Already read '='. Already Split.
    /// element [ consti, icon, leader_skill, assist_skill, diplo, league, enemy_power, merits, loyals, wave, cutin, yorozu ]
    /// </summary>
    private static bool Parse_Discard_CONSTI(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var source = ref result.Source;
        ref var tokenList = ref result.TokenList;
        tokenList[elementTokenId].Kind = TokenKind.CONSTI;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            tokenList.Last.Kind = TokenKind.Content;
            do
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                    return false;
                }

                if (tokenList.Last.IsMul(ref source))
                {
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                        return false;
                    }

                    tokenList.Last.Kind = TokenKind.Content;
                    if (!tokenList.Last.TryParse(ref source, out _))
                    {
                        result.ErrorAdd_NumberIsExpected(elementTokenId);
                    }

                    var processingLine = tokenList.Last.Range.StartInclusive.Line;
                    if (!ReadToken(ref context, ref result))
                    {
                        result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                        return false;
                    }

                    if (tokenList.Last.Range.StartInclusive.Line != processingLine)
                    {
                        CancelTokenReadback(ref context, ref result);
                        return true;
                    }

                    if (tokenList.Last.IsComma(ref source))
                    {
                        break;
                    }

                    result.ErrorAdd_CommaIsExpected(elementTokenId);
                    return false;
                }

                if (tokenList.Last.IsComma(ref source))
                {
                    break;
                }

                if (tokenList.Last.Range.StartInclusive.Line != tokenList[tokenList.LastIndex - 1].Range.StartInclusive.Line)
                {
                    CancelTokenReadback(ref context, ref result);
                    return true;
                }

                result.UnionLast2Tokens();
            } while (true);
        } while (true);
    }

    /// <summary>
    /// Already read '='. Already split.
    /// element: [ roam, power, spot ]
    /// </summary>
    private static bool Parse_Discard_ROAM(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList[elementTokenId].Kind = TokenKind.ROAM;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorList.Add(new("Element must have value. There is no value text after '='.", result.TokenList[elementTokenId].Range));
            return false;
        }

        tokenList.Last.Kind = TokenKind.Content;
        ref var source = ref result.Source;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (tokenList.Last.IsSemicolon(ref source))
            {
                return true;
            }

            if (tokenList.Last.IsComma(ref source))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                    return false;
                }

                if (tokenList.Last.IsSemicolon(ref source))
                {
                    return true;
                }
                
                tokenList.Last.Kind = TokenKind.Content;
            }
            else
            {
                result.UnionLast2Tokens();
            }
        } while (true);
    }

    /// <summary>
    /// Already read '='. Already split.
    /// struct: [ workspace, context ]
    /// element: [ member, merce, add2, next2, next3, just_next, monster, sound, item, castle_guard, item_sale, item_hold ]
    /// </summary>
    private static bool Parse_Discard_MEMBER(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList[elementTokenId].Kind = TokenKind.MEMBER;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorList.Add(new("Element must have value. There is no value text after '='.", result.TokenList[elementTokenId].Range));
            return false;
        }

        ref var source = ref result.Source;
        uint lastAddIndex = tokenList.LastIndex;
        tokenList.Last.Kind = TokenKind.Content;
        if (tokenList.Last.IsOperator(ref source))
        {
            result.ErrorList.Add(new($"Unexpected operator char{source[tokenList.Last.Range.StartInclusive.Line][tokenList.Last.Range.StartInclusive.Offset]} appears. Text is expected.", tokenList.Last.Range));
            return false;
        }

        var createWarning = context.CreateError(DiagnosticSeverity.Warning);
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (tokenList[lastAddIndex].Range.StartInclusive.Line != tokenList.Last.Range.StartInclusive.Line)
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            if (tokenList.Last.IsComma(ref source))
            {
                if (!ReadToken(ref context, ref result))
                {
                    result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                    return false;
                }

                lastAddIndex = tokenList.LastIndex;
                tokenList.Last.Kind = TokenKind.Content;
                if (tokenList.Last.IsOperator(ref source))
                {
                    result.ErrorList.Add(new($"Unexpected operator char{source[tokenList.Last.Range.StartInclusive.Line][tokenList.Last.Range.StartInclusive.Offset]} appears. Text is expected.", tokenList.Last.Range));
                    return false;
                }

                continue;
            }

            if (!tokenList.Last.IsMul(ref source))
            {
                result.UnionLast2Tokens();
                continue;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            tokenList.Last.Kind = TokenKind.Content;
            if (tokenList.Last.TryParse(ref source, out int repeatCount))
            {
                if (createWarning)
                {
                    if (repeatCount < 0)
                    {
                        result.ErrorList.Add(new($"Repeat count({repeatCount}) should be greater than -1.", tokenList.Last.Range, DiagnosticSeverity.Warning, ErrorCode.Semantics));
                    }
                    else if (repeatCount == 0)
                    {
                        result.ErrorList.Add(new("Repeat count is 0. I recommend you not to write \"*0\".", tokenList.Last.Range, DiagnosticSeverity.Warning, ErrorCode.Semantics));
                    }
                    else if (repeatCount == 1)
                    {
                        result.ErrorList.Add(new("Repeat count is 1. I recommend you not to write \"*1\".", tokenList.Last.Range, DiagnosticSeverity.Warning, ErrorCode.Semantics));
                    }
                }
            }
            else
            {
                result.ErrorAdd_NumberIsExpected(elementTokenId);
            }

            var processingLineIndex = tokenList.Last.Range.StartInclusive.Line;
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (processingLineIndex != tokenList.Last.Range.StartInclusive.Line)
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            if (!tokenList.Last.IsComma(ref source))
            {
                result.ErrorAdd_CommaIsExpected(elementTokenId);
                return false;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            tokenList.Last.Kind = TokenKind.Content;
            lastAddIndex = tokenList.LastIndex;
            if (tokenList.Last.IsOperator(ref source))
            {
                result.ErrorList.Add(new($"Unexpected operator char{source[tokenList.Last.Range.StartInclusive.Line][tokenList.Last.Range.StartInclusive.Offset]} appears. Text is expected.", tokenList.Last.Range));
                return false;
            }
        } while (true);
    }

    /// <summary>
    /// Already read '='. Already split.
    /// element: [ voice_type, delskill, delskill2, friend, enemy, staff, offset ]
    /// </summary>
    private static bool Parse_Discard_OFFSET(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList[elementTokenId].Kind = TokenKind.OFFSET;
        if (!ReadUsefulToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(elementTokenId, "Element must have value. There is no value text after '='.");
            return false;
        }

        tokenList.Last.Kind = TokenKind.Content;
        ref var source = ref result.Source;
        var processingLineIndex = tokenList.Last.Range.StartInclusive.Line;
        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (tokenList.Last.Range.StartInclusive.Line != processingLineIndex)
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            if (!tokenList.Last.IsComma(ref source))
            {
                result.UnionLast2Tokens();
                continue;
            }

            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            tokenList.Last.Kind = TokenKind.Content;
            if (tokenList.Last.Range.StartInclusive.Line != processingLineIndex)
            {
                result.ErrorList.Add(new("Unexpected Line Feed. ',' needs succeding token on the same line.", tokenList[tokenList.LastIndex - 1].Range));
                return false;
            }
        } while (true);
    }

    /// <summary>
    /// Default parsing function.
    /// '=' is already read. Already split.
    /// </summary>
    private static bool Parse_Discard_DEFAULT(ref Context context, ref Result result, uint elementTokenId)
    {
        ref var tokenList = ref result.TokenList;
        tokenList[elementTokenId].Kind = TokenKind.DEFAULT;
        if (!ReadToken(ref context, ref result))
        {
            result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
            return false;
        }

        ref var source = ref result.Source;
        tokenList.Last.Kind = TokenKind.Content;
        var processingLine = tokenList.Last.Range.StartInclusive.Line;
        var hasNumber = tokenList.Last.TryParse(ref source, out var number);
        if (hasNumber)
        {
            ref var end = ref tokenList.Last.Range.EndExclusive;
            if (processingLine == end.Line)
            {
                var span = source[end.Line].AsSpan(end.Offset);
                if (!span.IsEmpty)
                {
                    foreach (var item in span)
                    {
                        if (item != ' ' && item != '\t')
                        {
                            result.ErrorList.Add(new($"Line feed is needed immediately after number text({number}).", tokenList[elementTokenId].Range));
                            return false;
                        }
                    }

                    if (context.CreateError(DiagnosticSeverity.Hint))
                    {
                        result.ErrorList.Add(new($"Line feed is needed immediately after number text({number}). Trailing whitespaces are allowed, but not recommended.", tokenList[elementTokenId].Range, DiagnosticSeverity.Hint));
                    }
                }
            }

            return true;
        }

        do
        {
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(elementTokenId);
                return false;
            }

            if (processingLine != tokenList.Last.Range.StartInclusive.Line)
            {
                CancelTokenReadback(ref context, ref result);
                return true;
            }

            result.UnionLast2Tokens();
        } while (true);
    }
}
