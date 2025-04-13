using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class PowerLimitEffect : IntervalEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromDays(9_999);

    /// <inheritdoc />
    public override byte Icon => 41;

    /// <inheritdoc />
    public override string Name => "Power Limit";

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(250), false);

    private Attributes MaxAttributes = null!;
    private Attributes CurrentDebuff = new();

    /// <inheritdoc />
    public override void OnApplied()
        => MaxAttributes = new Attributes
        {
            Str = GetVarOrDefault("maxStr", 180),
            Int = GetVarOrDefault("maxInt", 180),
            Wis = GetVarOrDefault("maxWis", 180),
            Con = GetVarOrDefault("maxCon", 180),
            Dex = GetVarOrDefault("maxDex", 180),
            AtkSpeedPct = GetVarOrDefault("maxAtkSpeedPct", 150),
            Ac = GetVarOrDefault("maxAc", -15),
            Dmg = GetVarOrDefault("maxDmg", 150),
            SkillDamagePct = GetVarOrDefault("maxSkillDamagePct", 80),
            SpellDamagePct = GetVarOrDefault("maxSpellDamagePct", 80),
            HealBonusPct = GetVarOrDefault("maxHealBonusPct", 80),
            FlatSkillDamage = GetVarOrDefault("maxFlatSkillDamage", 1_200),
            FlatSpellDamage = GetVarOrDefault("maxFlatSpellDamage", 1_200),
            HealBonus = GetVarOrDefault("maxHealBonus", 1_200),
            MaximumHp = GetVarOrDefault("maxMaxHp", 40_000),
            MaximumMp = GetVarOrDefault("maxMaxMp", 30_000)
        };
    
    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var currentAttributes = Subject.StatSheet;

        Subject.StatSheet.AddBonus(CurrentDebuff);
        
        CurrentDebuff = new Attributes
        {
            Str = currentAttributes.EffectiveStr > MaxAttributes.Str ? currentAttributes.EffectiveStr - MaxAttributes.Str : 0,
            Int = currentAttributes.EffectiveInt > MaxAttributes.Int ? currentAttributes.EffectiveInt - MaxAttributes.Int : 0,
            Wis = currentAttributes.EffectiveWis > MaxAttributes.Wis ? currentAttributes.EffectiveWis - MaxAttributes.Wis : 0,
            Con = currentAttributes.EffectiveCon > MaxAttributes.Con ? currentAttributes.EffectiveCon - MaxAttributes.Con : 0,
            Dex = currentAttributes.EffectiveDex > MaxAttributes.Dex ? currentAttributes.EffectiveDex - MaxAttributes.Dex : 0,
            AtkSpeedPct = currentAttributes.EffectiveAttackSpeedPct > MaxAttributes.AtkSpeedPct
                ? currentAttributes.EffectiveAttackSpeedPct - MaxAttributes.AtkSpeedPct
                : 0,
            Ac = currentAttributes.EffectiveAc < MaxAttributes.Ac ? currentAttributes.EffectiveAc + MaxAttributes.Ac : 0,
            Dmg = currentAttributes.EffectiveDmg > MaxAttributes.Dmg ? currentAttributes.EffectiveDmg - MaxAttributes.Dmg : 0,
            SkillDamagePct = currentAttributes.EffectiveSkillDamagePct > MaxAttributes.SkillDamagePct
                ? currentAttributes.EffectiveSkillDamagePct - MaxAttributes.SkillDamagePct
                : 0,
            SpellDamagePct = currentAttributes.EffectiveSpellDamagePct > MaxAttributes.SpellDamagePct
                ? currentAttributes.EffectiveSpellDamagePct - MaxAttributes.SpellDamagePct
                : 0,
            HealBonusPct = currentAttributes.EffectiveHealBonusPct > MaxAttributes.HealBonusPct
                ? currentAttributes.EffectiveHealBonusPct - MaxAttributes.HealBonusPct
                : 0,
            FlatSkillDamage = currentAttributes.EffectiveFlatSkillDamage > MaxAttributes.FlatSkillDamage
                ? currentAttributes.EffectiveFlatSkillDamage - MaxAttributes.FlatSkillDamage
                : 0,
            FlatSpellDamage = currentAttributes.EffectiveFlatSpellDamage > MaxAttributes.FlatSpellDamage
                ? currentAttributes.EffectiveFlatSpellDamage - MaxAttributes.FlatSpellDamage
                : 0,
            HealBonus = currentAttributes.EffectiveHealBonus > MaxAttributes.HealBonus
                ? currentAttributes.EffectiveHealBonus - MaxAttributes.HealBonus
                : 0,
            MaximumHp = (int)currentAttributes.EffectiveMaximumHp > MaxAttributes.MaximumHp
                ? (int)currentAttributes.EffectiveMaximumHp - MaxAttributes.MaximumHp
                : 0,
            MaximumMp = (int)currentAttributes.EffectiveMaximumMp > MaxAttributes.MaximumMp
                ? (int)currentAttributes.EffectiveMaximumMp - MaxAttributes.MaximumMp
                : 0
        };
        
        Subject.StatSheet.SubtractBonus(CurrentDebuff);
        
        if(Subject.StatSheet.CurrentHp > Subject.StatSheet.EffectiveMaximumHp)
            Subject.StatSheet.SetHp((int)Subject.StatSheet.EffectiveMaximumHp);
        
        if(Subject.StatSheet.CurrentMp > Subject.StatSheet.EffectiveMaximumMp)
            Subject.StatSheet.SetMp((int)Subject.StatSheet.EffectiveMaximumMp);
        
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    /// <inheritdoc />
    public override void OnTerminated()
    {
        Subject.StatSheet.AddBonus(CurrentDebuff);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);

        base.OnTerminated();
    }
}