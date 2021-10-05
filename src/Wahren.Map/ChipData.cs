namespace Wahren.Map;

public record struct ChipData
{
    public uint NameId;
    public ChipKind Kind;
    public UnitDirection Direction;
    public UnitFormation Formation;
    public UnitKind UnitKind;

    public ChipData(byte type, uint nameId)
    {
        NameId = nameId;
        UnitKind = default;
        Direction = default;
        Formation = default;
        switch (type)
        {
            case 0:
                Kind = ChipKind.Field;
                break;
            case 1:
                Kind = ChipKind.Building;
                break;
            default:
                Kind = ChipKind.Unit;
                Direction = (UnitDirection)((0xf & type) - 2);
                Formation = (UnitFormation)((0xff & type) >> 4);
                break;
        }
    }
}

public enum ChipKind : byte
{
    Field = 0,
    Building = 1,
    Unit = 2,
}

public enum UnitDirection : byte
{

}

public enum UnitFormation : byte
{

}

public enum UnitKind : byte
{
    None,
    Sharp_Sharp,
    Atmark_ESC_Atmark,
}
