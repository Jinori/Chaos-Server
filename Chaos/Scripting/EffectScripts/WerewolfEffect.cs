using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class WerewolfEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    private Attributes NegativeAttributes = null!;

    public List<string> ConflictingEffectNames { get; init; } =
        [
            "Werewolf",
            "Mount"
        ];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(999);

    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 6,
        AnimationSpeed = 100
    };

    public override byte Icon => 85;
    public override string Name => "Werewolf";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        if (AislingSubject != null)
        {
            NegativeAttributes = new Attributes
            {
                Dmg = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveDmg * 0.25),
                Hit = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveHit * 0.25),
                Str = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveStr * 0.25),
                Ac = -Convert.ToInt32(AislingSubject.UserStatSheet.Ac * 0.25),
                MaximumHp = Convert.ToInt32(AislingSubject.UserStatSheet.MaximumHp * 0.25),
                MaximumMp = Convert.ToInt32(AislingSubject.UserStatSheet.MaximumMp * 0.25),
                Int = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveInt * 0.25),
                Dex = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveDex * 0.25),
                Con = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveCon * 0.25),
                Wis = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveWis * 0.25),
                MagicResistance = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveMagicResistance * 0.25),
                FlatSkillDamage = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveFlatSkillDamage * 0.25),
                FlatSpellDamage = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveFlatSpellDamage * 0.25),
                SpellDamagePct = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveSpellDamagePct * 0.25),
                SkillDamagePct = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveSkillDamagePct * 0.25),
                AtkSpeedPct = Convert.ToInt32(AislingSubject.UserStatSheet.EffectiveAttackSpeedPct * 0.25)
            };

            AislingSubject.StatSheet.SubtractBonus(NegativeAttributes);
            AislingSubject.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject.SetSprite(426);
        }
    }

    public override void OnTerminated()
    {
        if (AislingSubject != null)
        {
            AislingSubject.StatSheet.AddBonus(NegativeAttributes);
            AislingSubject.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject.SetSprite(0);
        }
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}