using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Wahren.AbstractSyntaxTree.Parser;

public static partial class Parser
{
    private static bool ParseDungeon(ref Context context, ref Result result)
    {
        result.DungeonNodeList.Add(new());
        ref var tokenList = ref result.TokenList;
        ref var node = ref result.DungeonNodeList.Last;
        node.Kind = tokenList.LastIndex;
        if (!ParseNameAndSuperAndBracketLeft(ref context, ref result, ref node, ref result.DungeonSet))
        {
            return false;
        }

        var createErrorWarning = context.CreateError(DiagnosticSeverity.Warning);
        ref var source = ref result.Source;
        ref var pair_DEFAULT = ref Unsafe.NullRef<Pair_NullableString_NullableIntElement?>();
        ref var pair_RAY = ref Unsafe.NullRef<Pair_NullableString_NullableInt_ArrayElement?>();
        ref var pair_MEMBER = ref Unsafe.NullRef<Pair_NullableString_NullableInt_ArrayElement?>();
        ulong key = 0UL;
        do
        {
            if (!ReadUsefulToken(ref context, ref result))
            {
                result.ErrorAdd_BracketRightNotFound(node.Kind, node.Name);
                return false;
            }

            if (tokenList.Last.IsBracketRight(ref source))
            {
                node.BracketRight = tokenList.LastIndex;
                return true;
            }

            var currentIndex = tokenList.LastIndex;
            if (!result.SplitElement(currentIndex, out var span, out var scenarioVariant))
            {
                return false;
            }
            if (!ReadToken(ref context, ref result))
            {
                result.ErrorAdd_UnexpectedEndOfFile(tokenList.LastIndex, "'=' is expected but not found.");
                return false;
            }
            
            if (!tokenList.Last.IsAssign(ref source))
            {
                result.ErrorAdd_UnexpectedOperatorToken(tokenList.LastIndex, "'=' is expected but not found.");
                return false;
            }

            var byteSpan = MemoryMarshal.Cast<char, byte>(span);
            var originalLength = span.Length;
            switch (originalLength)
            {
                case 0: return false;
                case 1:
                    key = ((ulong)byteSpan[0]) | ((ulong)byteSpan[1] << 8);
                    span = Span<char>.Empty;
                    goto DISCARD;
                case 2:
                    key = BinaryPrimitives.ReadUInt32LittleEndian(byteSpan);
                    span = Span<char>.Empty;
                    goto DISCARD;
                case 3:
                    key = BinaryPrimitives.ReadUInt32LittleEndian(byteSpan) | (((ulong)BinaryPrimitives.ReadUInt16LittleEndian(byteSpan.Slice(4))) << 32);
                    switch (key)
                    {
                        case 0x00780061006DUL: pair_DEFAULT = ref node.max.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x006D00670062UL: pair_DEFAULT = ref node.bgm.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x00700061006DUL: pair_DEFAULT = ref node.map.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x0078006F0062UL: pair_DEFAULT = ref node.box.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x007900610072UL: pair_RAY = ref node.ray.EnsureGet(scenarioVariant); goto RAY;
                    }
                    span = Span<char>.Empty;
                    goto DISCARD;
            }

            key = BinaryPrimitives.ReadUInt64LittleEndian(byteSpan);
            span = span.Slice(4);
            switch (span.Length)
            {
                case 0:
                    switch (key)
                    {
                        case 0x0065006D0061006EUL: pair_DEFAULT = ref node.name.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x006E00650070006FUL: pair_DEFAULT = ref node.open.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x006C006C00610077UL: pair_DEFAULT = ref node.wall.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x006C0061006F0067UL: pair_DEFAULT = ref node.goal.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x006D006500740069UL: pair_MEMBER = ref node.item.EnsureGet(scenarioVariant); goto MEMBER;
                        case 0x0065006D006F0068UL: pair_RAY = ref node.home.EnsureGet(scenarioVariant); goto RAY;
                    }
                    goto default;
                case 1:
                    switch (key)
                    {
                        case 0x0069006D0069006CUL when span[0] == 't': pair_DEFAULT = ref node.limit.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x006E0069006C0062UL when span[0] == 'd': pair_DEFAULT = ref node.blind.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x006F006C006F0063UL when span[0] == 'r': pair_RAY = ref node.color.EnsureGet(scenarioVariant); goto RAY;
                        case 0x006F006F006C0066UL when span[0] == 'r': pair_DEFAULT = ref node.floor.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x0072006100740073UL when span[0] == 't': pair_DEFAULT = ref node.start.EnsureGet(scenarioVariant); goto DEFAULT;
                    }
                    goto default;
                case 2:
                    switch (key)
                    {
                        case 0x0066006500720070UL when span.SequenceEqual("ix"): pair_DEFAULT = ref node.prefix.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x0066006600750073UL when span.SequenceEqual("ix"): pair_DEFAULT = ref node.suffix.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x0075006C006F0076UL when span.SequenceEqual("me"): pair_DEFAULT = ref node.volume.EnsureGet(scenarioVariant); goto DEFAULT;
                    }
                    goto default;
                case 3:
                    switch (key)
                    {
                        case 0x0073006E006F006DUL when span.SequenceEqual("ter"): pair_MEMBER = ref node.monster.EnsureGet(scenarioVariant); goto MEMBER;
                    }
                    goto default;
                case 4:
                    switch (key)
                    {
                        case 0x006D006500740069UL when span.SequenceEqual("_num"): pair_DEFAULT = ref node.item_num.EnsureGet(scenarioVariant); goto DEFAULT;
                    }
                    goto default;
                case 5:
                    switch (key)
                    {
                        case 0x0061005F0076006CUL when span.SequenceEqual("djust"): pair_DEFAULT = ref node.lv_adjust.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x006D006500740069UL when span.SequenceEqual("_text"): pair_DEFAULT = ref node.item_text.EnsureGet(scenarioVariant); goto DEFAULT;
                    }
                    goto default;
                case 6:
                    switch (key)
                    {
                        case 0x00650076006F006DUL when span.SequenceEqual("_speed"): pair_DEFAULT = ref node.move_speed.EnsureGet(scenarioVariant); goto DEFAULT;
                        case 0x0065007300610062UL when span.SequenceEqual("_level"): pair_DEFAULT = ref node.base_level.EnsureGet(scenarioVariant); goto DEFAULT;
                    }
                    goto default;
                case 7:
                    switch (key)
                    {
                        case 0x0073006E006F006DUL when span.SequenceEqual("ter_num"): pair_DEFAULT = ref node.monster_num.EnsureGet(scenarioVariant); goto DEFAULT;
                    }
                    goto default;
                default:
                    span = MemoryMarshal.Cast<byte, char>(byteSpan);
                    goto DISCARD;
            }

        DEFAULT:
            if (pair_DEFAULT is null)
            {
                pair_DEFAULT = new(currentIndex);
                pair_DEFAULT.ElementScenarioId = scenarioVariant;
                pair_DEFAULT.ElementKeyRange.Length = (uint)originalLength;
                {
                    ref var start = ref tokenList[currentIndex].Range.StartInclusive;
                    pair_DEFAULT.ElementKeyRange.Line = start.Line;
                    pair_DEFAULT.ElementKeyRange.Offset = start.Offset;
                }
                if (Parse_Element_DEFAULT(ref context, ref result, pair_DEFAULT))
                {
                   continue;
                }

                return false;
            }

            if (createErrorWarning)
            {
                result.WarningAdd_MultipleAssignment(currentIndex);
            }
                
            if (Parse_Discard_DEFAULT(ref context, ref result, currentIndex))
            {
                continue;
            }
            else
            {
                return false;
            }
        RAY:
            if (pair_RAY is null)
            {
                pair_RAY = new(currentIndex);
                pair_RAY.ElementScenarioId = scenarioVariant;
                pair_RAY.ElementKeyRange.Length = (uint)originalLength;
                {
                    ref var start = ref tokenList[currentIndex].Range.StartInclusive;
                    pair_RAY.ElementKeyRange.Line = start.Line;
                    pair_RAY.ElementKeyRange.Offset = start.Offset;
                }
                if (Parse_Element_RAY(ref context, ref result, pair_RAY))
                {
                   continue;
                }

                return false;
            }

            if (createErrorWarning)
            {
                result.WarningAdd_MultipleAssignment(currentIndex);
            }
                
            if (Parse_Discard_RAY(ref context, ref result, currentIndex))
            {
                continue;
            }
            else
            {
                return false;
            }
        MEMBER:
            if (pair_MEMBER is null)
            {
                pair_MEMBER = new(currentIndex);
                pair_MEMBER.ElementScenarioId = scenarioVariant;
                pair_MEMBER.ElementKeyRange.Length = (uint)originalLength;
                {
                    ref var start = ref tokenList[currentIndex].Range.StartInclusive;
                    pair_MEMBER.ElementKeyRange.Line = start.Line;
                    pair_MEMBER.ElementKeyRange.Offset = start.Offset;
                }
                if (Parse_Element_MEMBER(ref context, ref result, pair_MEMBER))
                {
                   continue;
                }

                return false;
            }

            if (createErrorWarning)
            {
                result.WarningAdd_MultipleAssignment(currentIndex);
            }
                
            if (Parse_Discard_MEMBER(ref context, ref result, currentIndex))
            {
                continue;
            }
            else
            {
                return false;
            }
        DISCARD:
            if (Parse_Discard(ref context, ref result, currentIndex, span, key))
            {
                if (createErrorWarning)
                {
                    result.WarningAdd_UnexpectedElementName(node.Kind, currentIndex);
                }
                continue;
            }
            else
            {
                return false;
            }
        } while (true);
    }
}
