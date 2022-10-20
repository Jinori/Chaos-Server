using Chaos.Data;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class StatSheetMapperProfile : IMapperProfile<StatSheet, StatSheetSchema>
{
    public StatSheet Map(StatSheetSchema obj) => new()
    {
        AtkSpeedPct = obj.AtkSpeedPct,
        Ac = obj.Ac,
        Dmg = obj.Dmg,
        Hit = obj.Hit,
        Str = obj.Str,
        Int = obj.Int,
        Wis = obj.Wis,
        Con = obj.Con,
        Dex = obj.Dex,
        MagicResistance = obj.MagicResistance,
        MaximumHp = obj.MaximumHp,
        MaximumMp = obj.MaximumMp,
        CurrentHp = obj.CurrentHp,
        CurrentMp = obj.CurrentMp,
        Ability = obj.Ability,
        Level = obj.Level
    };

    public StatSheetSchema Map(StatSheet obj) => new()
    {
        AtkSpeedPct = obj.AtkSpeedPct,
        Ability = obj.Ability,
        Ac = obj.Ac,
        Con = obj.Con,
        CurrentHp = obj.CurrentHp,
        CurrentMp = obj.CurrentMp,
        Level = obj.Level,
        Dex = obj.Dex,
        Dmg = obj.Dmg,
        Hit = obj.Hit,
        Int = obj.Int,
        MagicResistance = obj.MagicResistance,
        MaximumHp = obj.MaximumHp,
        MaximumMp = obj.MaximumMp,
        Str = obj.Str,
        Wis = obj.Wis
    };
}