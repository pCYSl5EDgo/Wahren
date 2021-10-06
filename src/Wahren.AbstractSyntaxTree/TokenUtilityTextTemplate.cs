namespace Wahren.AbstractSyntaxTree;
public static partial class TokenUtility
{
    public static bool TryParse(ref this Token token, ref DualList<char> source, out int result)
    {
        if (token.Length != 0)
        {
            var span = source[token.Position.Line].AsSpan(token.Position.Offset, token.Length);
            return int.TryParse(span, out result);
        }

        result = default;
        return false;
    }

    public static bool Is_attribute(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'a', 't', 't', 'r', 'i', 'b', 'u', 't', 'e');

    public static bool Is_attribute_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 't', 'r', 'i', 'b', 'u', 't', 'e');

    public static bool Is_battle(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'b', 'a', 't', 't', 'l', 'e');

    public static bool Is_battle_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 't', 't', 'l', 'e');

    public static bool Is_class(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'c', 'l', 'a', 's', 's');

    public static bool Is_class_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'a', 's', 's');

    public static bool Is_context(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'c', 'o', 'n', 't', 'e', 'x', 't');

    public static bool Is_context_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'n', 't', 'e', 'x', 't');

    public static bool Is_delskill(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'd', 'e', 'l', 's', 'k', 'i', 'l', 'l');

    public static bool Is_delskill_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'l', 's', 'k', 'i', 'l', 'l');

    public static bool Is_detail(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'd', 'e', 't', 'a', 'i', 'l');

    public static bool Is_detail_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 't', 'a', 'i', 'l');

    public static bool Is_dungeon(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'd', 'u', 'n', 'g', 'e', 'o', 'n');

    public static bool Is_dungeon_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'n', 'g', 'e', 'o', 'n');

    public static bool Is_else(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'e', 'l', 's', 'e');

    public static bool Is_else_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 's', 'e');

    public static bool Is_event(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'e', 'v', 'e', 'n', 't');

    public static bool Is_event_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'e', 'n', 't');

    public static bool Is_field(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'f', 'i', 'e', 'l', 'd');

    public static bool Is_field_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'e', 'l', 'd');

    public static bool Is_fight(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'f', 'i', 'g', 'h', 't');

    public static bool Is_fight_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'g', 'h', 't');

    public static bool Is_friend(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'f', 'r', 'i', 'e', 'n', 'd');

    public static bool Is_friend_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'i', 'e', 'n', 'd');

    public static bool Is_if(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'i', 'f');

    public static bool Is_movetype(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'm', 'o', 'v', 'e', 't', 'y', 'p', 'e');

    public static bool Is_movetype_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'v', 'e', 't', 'y', 'p', 'e');

    public static bool Is_multi(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'm', 'u', 'l', 't', 'i');

    public static bool Is_multi_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'l', 't', 'i');

    public static bool Is_object(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'o', 'b', 'j', 'e', 'c', 't');

    public static bool Is_object_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'j', 'e', 'c', 't');

    public static bool Is_on(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'o', 'n');

    public static bool Is_power(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'p', 'o', 'w', 'e', 'r');

    public static bool Is_power_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'w', 'e', 'r');

    public static bool Is_race(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'r', 'a', 'c', 'e');

    public static bool Is_race_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'c', 'e');

    public static bool Is_rif(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'r', 'i', 'f');

    public static bool Is_rif_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'f');

    public static bool Is_roam(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'r', 'o', 'a', 'm');

    public static bool Is_roam_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'a', 'm');

    public static bool Is_scenario(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 's', 'c', 'e', 'n', 'a', 'r', 'i', 'o');

    public static bool Is_scenario_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'e', 'n', 'a', 'r', 'i', 'o');

    public static bool Is_skill(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 's', 'k', 'i', 'l', 'l');

    public static bool Is_skill_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'i', 'l', 'l');

    public static bool Is_skillset(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 's', 'k', 'i', 'l', 'l', 's', 'e', 't');

    public static bool Is_skillset_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'i', 'l', 'l', 's', 'e', 't');

    public static bool Is_sound(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 's', 'o', 'u', 'n', 'd');

    public static bool Is_sound_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'u', 'n', 'd');

    public static bool Is_spot(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 's', 'p', 'o', 't');

    public static bool Is_spot_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'o', 't');

    public static bool Is_story(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 's', 't', 'o', 'r', 'y');

    public static bool Is_story_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'o', 'r', 'y');

    public static bool Is_unit(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'u', 'n', 'i', 't');

    public static bool Is_unit_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'i', 't');

    public static bool Is_voice(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'v', 'o', 'i', 'c', 'e');

    public static bool Is_voice_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'i', 'c', 'e');

    public static bool Is_voice_type(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'v', 'o', 'i', 'c', 'e', '_', 't', 'y', 'p', 'e');

    public static bool Is_voice_type_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'i', 'c', 'e', '_', 't', 'y', 'p', 'e');

    public static bool Is_while(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'w', 'h', 'i', 'l', 'e');

    public static bool Is_while_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'i', 'l', 'e');

    public static bool Is_world(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'w', 'o', 'r', 'l', 'd');

    public static bool Is_world_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'r', 'l', 'd');

    public static bool Is_workspace(ref this Token token, ref DualList<char> source) => Equals(ref token, ref source, 'w', 'o', 'r', 'k', 's', 'p', 'a', 'c', 'e');

    public static bool Is_workspace_Skip2(ref this Token token, ref DualList<char> source) => EqualsSkip2(ref token, ref source, 'r', 'k', 's', 'p', 'a', 'c', 'e');

    public static bool Equals(ref this Token token, ref DualList<char> source, char other0, char other1)
    {
        const int size = 2;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[1] == other1 && span[0] == other0;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, char other0, char other1, char other2)
    {
        const int size = 3;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[2] == other2 && span[1] == other1 && span[0] == other0;
    }

    public static bool EqualsSkip2(ref this Token token, ref DualList<char> source, char other2)
    {
        const int size = 3;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[2] == other2;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, char other0, char other1, char other2, char other3)
    {
        const int size = 4;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[3] == other3 && span[2] == other2 && span[1] == other1 && span[0] == other0;
    }

    public static bool EqualsSkip2(ref this Token token, ref DualList<char> source, char other2, char other3)
    {
        const int size = 4;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[3] == other3 && span[2] == other2;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, char other0, char other1, char other2, char other3, char other4)
    {
        const int size = 5;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[4] == other4 && span[3] == other3 && span[2] == other2 && span[1] == other1 && span[0] == other0;
    }

    public static bool EqualsSkip2(ref this Token token, ref DualList<char> source, char other2, char other3, char other4)
    {
        const int size = 5;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[4] == other4 && span[3] == other3 && span[2] == other2;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, char other0, char other1, char other2, char other3, char other4, char other5)
    {
        const int size = 6;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2 && span[1] == other1 && span[0] == other0;
    }

    public static bool EqualsSkip2(ref this Token token, ref DualList<char> source, char other2, char other3, char other4, char other5)
    {
        const int size = 6;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, char other0, char other1, char other2, char other3, char other4, char other5, char other6)
    {
        const int size = 7;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[6] == other6 && span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2 && span[1] == other1 && span[0] == other0;
    }

    public static bool EqualsSkip2(ref this Token token, ref DualList<char> source, char other2, char other3, char other4, char other5, char other6)
    {
        const int size = 7;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[6] == other6 && span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, char other0, char other1, char other2, char other3, char other4, char other5, char other6, char other7)
    {
        const int size = 8;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[7] == other7 && span[6] == other6 && span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2 && span[1] == other1 && span[0] == other0;
    }

    public static bool EqualsSkip2(ref this Token token, ref DualList<char> source, char other2, char other3, char other4, char other5, char other6, char other7)
    {
        const int size = 8;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[7] == other7 && span[6] == other6 && span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, char other0, char other1, char other2, char other3, char other4, char other5, char other6, char other7, char other8)
    {
        const int size = 9;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[8] == other8 && span[7] == other7 && span[6] == other6 && span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2 && span[1] == other1 && span[0] == other0;
    }

    public static bool EqualsSkip2(ref this Token token, ref DualList<char> source, char other2, char other3, char other4, char other5, char other6, char other7, char other8)
    {
        const int size = 9;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[8] == other8 && span[7] == other7 && span[6] == other6 && span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2;
    }

    public static bool Equals(ref this Token token, ref DualList<char> source, char other0, char other1, char other2, char other3, char other4, char other5, char other6, char other7, char other8, char other9)
    {
        const int size = 10;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[9] == other9 && span[8] == other8 && span[7] == other7 && span[6] == other6 && span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2 && span[1] == other1 && span[0] == other0;
    }

    public static bool EqualsSkip2(ref this Token token, ref DualList<char> source, char other2, char other3, char other4, char other5, char other6, char other7, char other8, char other9)
    {
        const int size = 10;
        if (token.Length != size)
        {
            return false;
        }

        ref var start = ref token.Position;
        var span = source[start.Line].AsSpan(start.Offset, size);
        return span[9] == other9 && span[8] == other8 && span[7] == other7 && span[6] == other6 && span[5] == other5 && span[4] == other4 && span[3] == other3 && span[2] == other2;
    }

}
