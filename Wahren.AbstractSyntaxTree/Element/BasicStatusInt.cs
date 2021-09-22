namespace Wahren.AbstractSyntaxTree.Element;

public unsafe struct BasicStatusInt
{
    public fixed int Value[11];
    public fixed bool Defined[11];

    public ref int Hp => ref Value[0];
    public ref int Mp => ref Value[1];
    public ref int Attack => ref Value[2];
    public ref int Defense => ref Value[3];
    public ref int Magic => ref Value[4];
    public ref int MagicDefense => ref Value[5];
    public ref int Speed => ref Value[6];
    public ref int Dexterity => ref Value[7];
    public ref int Move => ref Value[8];
    public ref int Hprec => ref Value[9];
    public ref int Mprec => ref Value[10];

    public ref bool DefinedHp => ref Defined[0];
    public ref bool DefinedMp => ref Defined[1];
    public ref bool DefinedAttack => ref Defined[2];
    public ref bool DefinedDefense => ref Defined[3];
    public ref bool DefinedMagic => ref Defined[4];
    public ref bool DefinedMagicDefense => ref Defined[5];
    public ref bool DefinedSpeed => ref Defined[6];
    public ref bool DefinedDexterity => ref Defined[7];
    public ref bool DefinedMove => ref Defined[8];
    public ref bool DefinedHprec => ref Defined[9];
    public ref bool DefinedMprec => ref Defined[10];
}
