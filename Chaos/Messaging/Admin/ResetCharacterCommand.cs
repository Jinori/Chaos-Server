using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("resetchar")]
public class ResetCharacterCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        source.UserStatSheet.SetLevel(1);
        source.UserStatSheet.SubtractTotalExp(source.UserStatSheet.TotalExp);

        if (source.UserStatSheet.ToNextLevel >= 1)
            source.UserStatSheet.SubtractTnl(source.UserStatSheet.ToNextLevel);

        var baseStats = UserStatSheet.NewCharacter;
        
        foreach (var item in source.Inventory)
            source.Inventory.Remove(item.Slot);

        foreach (var item in source.Equipment)
            source.Equipment.Remove(item.Slot);

        foreach (var skill in source.SkillBook)
            source.SkillBook.Remove(skill.Slot);
        
        foreach (var spell in source.SpellBook)
            source.SpellBook.Remove(spell.Slot);
        
        var diff = new Attributes()
        {
            Ac = source.StatSheet.Ac - baseStats.Ac,
            MaximumHp = source.StatSheet.MaximumHp - baseStats.MaximumHp,
            MaximumMp = source.StatSheet.MaximumMp - baseStats.MaximumMp,
            Hit = source.StatSheet.Hit - baseStats.Hit,
            Dmg = source.StatSheet.Dmg - baseStats.Dmg,
            MagicResistance = source.StatSheet.MagicResistance - baseStats.MagicResistance,
            AtkSpeedPct = source.StatSheet.AtkSpeedPct - baseStats.AtkSpeedPct,
            FlatSkillDamage = source.StatSheet.FlatSkillDamage - baseStats.FlatSkillDamage,
            FlatSpellDamage = source.StatSheet.FlatSpellDamage - baseStats.FlatSpellDamage,
            SkillDamagePct = source.StatSheet.SkillDamagePct - baseStats.SkillDamagePct,
            SpellDamagePct = source.StatSheet.SpellDamagePct - baseStats.SpellDamagePct,
            Str = source.StatSheet.Str - baseStats.Str,
            Int = source.StatSheet.Int - baseStats.Int,
            Wis = source.StatSheet.Wis - baseStats.Wis,
            Con = source.StatSheet.Con - baseStats.Con,
            Dex = source.StatSheet.Dex - baseStats.Dex
        };

        source.UserStatSheet.Subtract(diff);
        source.UserStatSheet.AddTnl(599);
        source.UserStatSheet.SetMaxWeight(44);
        source.UserStatSheet.UnspentPoints = 0;
        source.Client.SendAttributes(StatUpdateType.Full);
        source.Refresh(true);

        return default;
    }
}