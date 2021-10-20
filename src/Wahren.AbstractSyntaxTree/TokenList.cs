namespace Wahren.AbstractSyntaxTree;

public struct TokenList : IDisposable
{
    private uint[] arrayUInt32 = Array.Empty<uint>();
    private TokenKind[] arrayTokenKind = Array.Empty<TokenKind>();
    private int count = 0;
    private int eachLimit = 0;
    
    public int Count => count;
    public uint LastIndex => (uint)(count - 1);

    public Span<uint> Line => arrayUInt32.AsSpan(0, count);
    public Span<uint> Offset => arrayUInt32.AsSpan(eachLimit, count);
    public Span<uint> Length => arrayUInt32.AsSpan(eachLimit << 1, count);
    public Span<uint> PrecedingNewLineCount => arrayUInt32.AsSpan(eachLimit * 3, count);
    public Span<uint> PrecedingWhitespaceCount => arrayUInt32.AsSpan(eachLimit << 2, count);
    public Span<uint> Other => arrayUInt32.AsSpan(eachLimit * 5, count);
    public Span<TokenKind> Kind => arrayTokenKind.AsSpan(0, count);

    public ref uint GetLine(uint index) => ref arrayUInt32[index];
    public ref uint GetOffset(uint index) => ref arrayUInt32[eachLimit + index];
    public ref uint GetLength(uint index) => ref arrayUInt32[(eachLimit << 1) + index];
    public ref uint GetPrecedingNewLineCount(uint index) => ref arrayUInt32[(eachLimit * 3) + index];
    public ref uint GetPrecedingWhitespaceCount(uint index) => ref arrayUInt32[(eachLimit << 2) + index];
    public ref uint GetOther(uint index) => ref arrayUInt32[(eachLimit * 5) + index];
    public ref TokenKind GetKind(uint index) => ref arrayTokenKind[index];

    public bool IsFirstTokenInTheLine(uint index) => arrayUInt32[eachLimit + index] == arrayUInt32[(eachLimit << 2) + index];

    public void Add(ref Token token)
    {
        PrepareAddRange(1);
        var lastIndex = (uint)count++;
        GetLine(lastIndex) = token.Position.Line;
        GetOffset(lastIndex) = token.Position.Offset;
        GetLength(lastIndex) = token.Length;
        GetPrecedingNewLineCount(lastIndex) = token.PrecedingNewLineCount;
        GetPrecedingWhitespaceCount(lastIndex) = token.PrecedingWhitespaceCount;
        GetOther(lastIndex) = token.Other;
        GetKind(lastIndex) = token.Kind;
    }

    public void RemoveLast()
    {
        if (count != 0)
        {
            --count;
        }
    }

    public void RemoveRange(int index, int removeCount)
    {
        if (removeCount == 0)
        {
            return;
        }
        
        if (index < 0 || removeCount < 0 || index + removeCount > count)
        {
            throw new ArgumentOutOfRangeException();
        }
        
        if (index + removeCount != count)
        {
            Line.Slice(index + removeCount).CopyTo(Line.Slice(index));
            Offset.Slice(index + removeCount).CopyTo(Offset.Slice(index));
            Length.Slice(index + removeCount).CopyTo(Length.Slice(index));
            PrecedingNewLineCount.Slice(index + removeCount).CopyTo(PrecedingNewLineCount.Slice(index));
            PrecedingWhitespaceCount.Slice(index + removeCount).CopyTo(PrecedingWhitespaceCount.Slice(index));
            Other.Slice(index + removeCount).CopyTo(Other.Slice(index));
            Kind.Slice(index + removeCount).CopyTo(Kind.Slice(index));
        }

        count -= removeCount;
    }

    public Span<TokenKind> InsertUndefinedRange(int index, int additiveCount, out Span<uint> line, out Span<uint> offset, out Span<uint> length, out Span<uint> precedingNewLineCount, out Span<uint> precedingWhitespaceCount, out Span<uint> other)
    {
        if (index == count)
        {
            PrepareAddRange(additiveCount);
            var answer = arrayTokenKind.AsSpan(count, additiveCount);
            line = arrayUInt32.AsSpan(count, additiveCount);
            offset = arrayUInt32.AsSpan(eachLimit + count, additiveCount);
            length = arrayUInt32.AsSpan(eachLimit * 2 + count, additiveCount);
            precedingNewLineCount = arrayUInt32.AsSpan(eachLimit * 3 + count, additiveCount);
            precedingWhitespaceCount = arrayUInt32.AsSpan(eachLimit * 4 + count, additiveCount);
            other = arrayUInt32.AsSpan(eachLimit * 5 + count, additiveCount);
            count += additiveCount;
            return answer;
        }

        if (additiveCount <= 0)
        {
            line = Span<uint>.Empty;
            offset = Span<uint>.Empty;
            length = Span<uint>.Empty;
            precedingNewLineCount = Span<uint>.Empty;
            precedingWhitespaceCount = Span<uint>.Empty;
            other = Span<uint>.Empty;
            return Span<TokenKind>.Empty;
        }

        var rest = count - index;
        var dest = index + additiveCount;
        if (count + additiveCount > arrayTokenKind.Length)
        {
            var tmp = ArrayPool<TokenKind>.Shared.Rent(count + additiveCount);
            arrayTokenKind.AsSpan(0, index).CopyTo(tmp);
            arrayTokenKind.AsSpan(index, rest).CopyTo(tmp.AsSpan(dest));
            if (arrayTokenKind != Array.Empty<TokenKind>())
            {
                ArrayPool<TokenKind>.Shared.Return(arrayTokenKind);
            }
            arrayTokenKind = tmp;
        }
        else
        {
            arrayTokenKind.AsSpan(index, rest).CopyTo(arrayTokenKind.AsSpan(dest));
        }

        if (count + additiveCount > eachLimit)
        {
            var tmp = ArrayPool<uint>.Shared.Rent((count + additiveCount) * 6);
            var eachLimitNew = tmp.Length / 6;
            if (arrayUInt32 != Array.Empty<uint>())
            {
                arrayUInt32.AsSpan(0, index).CopyTo(tmp);
                arrayUInt32.AsSpan(index, rest).CopyTo(tmp.AsSpan(dest));
                arrayUInt32.AsSpan(eachLimit, index).CopyTo(tmp.AsSpan(eachLimitNew));
                arrayUInt32.AsSpan(eachLimit + index, rest).CopyTo(tmp.AsSpan(eachLimitNew + dest));
                arrayUInt32.AsSpan(eachLimit << 1, index).CopyTo(tmp.AsSpan(eachLimitNew << 1));
                arrayUInt32.AsSpan((eachLimit << 1) + index, rest).CopyTo(tmp.AsSpan((eachLimitNew << 1) + dest));
                arrayUInt32.AsSpan(eachLimit * 3, index).CopyTo(tmp.AsSpan(eachLimitNew * 3));
                arrayUInt32.AsSpan(eachLimit * 3 + index, rest).CopyTo(tmp.AsSpan(eachLimitNew * 3 + dest));
                arrayUInt32.AsSpan(eachLimit << 2, index).CopyTo(tmp.AsSpan(eachLimitNew << 2));
                arrayUInt32.AsSpan((eachLimit << 2) + index, rest).CopyTo(tmp.AsSpan((eachLimitNew << 2) + dest));
                arrayUInt32.AsSpan(eachLimit * 5, index).CopyTo(tmp.AsSpan(eachLimitNew * 5));
                arrayUInt32.AsSpan(eachLimit * 5 + index, rest).CopyTo(tmp.AsSpan(eachLimitNew * 5 + dest));
                ArrayPool<uint>.Shared.Return(arrayUInt32);
            }
            arrayUInt32 = tmp;
            eachLimit = eachLimitNew;
        }

        count += additiveCount;
        
        line = arrayUInt32.AsSpan(index, additiveCount);
        offset = arrayUInt32.AsSpan(eachLimit + index, additiveCount);
        length = arrayUInt32.AsSpan(eachLimit * 2 + index, additiveCount);
        precedingNewLineCount = arrayUInt32.AsSpan(eachLimit * 3 + index, additiveCount);
        precedingWhitespaceCount = arrayUInt32.AsSpan(eachLimit * 4 + index, additiveCount);
        other = arrayUInt32.AsSpan(eachLimit * 5 + index, additiveCount);
        return arrayTokenKind.AsSpan(index, additiveCount);
    }

    public void PrepareAddRange(int additiveCount)
    {
        if (additiveCount <= 0)
        {
            return;
        }

        if (count + additiveCount > arrayTokenKind.Length)
        {
            var tmp = ArrayPool<TokenKind>.Shared.Rent(count + additiveCount);
            Kind.CopyTo(tmp);
            if (arrayTokenKind != Array.Empty<TokenKind>())
            {
                ArrayPool<TokenKind>.Shared.Return(arrayTokenKind);
            }
            arrayTokenKind = tmp;
        }

        if (count + additiveCount > eachLimit)
        {
            var tmp = ArrayPool<uint>.Shared.Rent((count + additiveCount) * 6);
            var eachLimitNew = tmp.Length / 6;
            if (arrayUInt32 != Array.Empty<uint>())
            {
                Line.CopyTo(tmp.AsSpan());
                Offset.CopyTo(tmp.AsSpan(eachLimitNew));
                Length.CopyTo(tmp.AsSpan(eachLimitNew << 1));
                PrecedingNewLineCount.CopyTo(tmp.AsSpan(eachLimitNew * 3));
                PrecedingWhitespaceCount.CopyTo(tmp.AsSpan(eachLimitNew << 2));
                Other.CopyTo(tmp.AsSpan(eachLimitNew * 5));
                ArrayPool<uint>.Shared.Return(arrayUInt32);
            }
            arrayUInt32 = tmp;
            eachLimit = eachLimitNew;
        }
    }

    public void Dispose()
    {
        if (arrayUInt32 is { Length: > 0 })
        {
            ArrayPool<uint>.Shared.Return(arrayUInt32);
        }
        arrayUInt32 = Array.Empty<uint>();

        if (arrayTokenKind is { Length: > 0 })
        {
            ArrayPool<TokenKind>.Shared.Return(arrayTokenKind);
        }
        arrayTokenKind = Array.Empty<TokenKind>();
        count = 0;
        eachLimit = 0;
    }
}
