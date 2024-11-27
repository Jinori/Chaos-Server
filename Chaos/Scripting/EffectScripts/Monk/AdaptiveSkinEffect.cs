using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public sealed class AdaptiveSkinEffect : ContinuousAnimationEffectBase
{
    private int AcBonus;
    private int LastMonsterCount;
    private int MagicResistBonus;

    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(30);

    protected override Animation Animation { get; } = new();

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    /// <inheritdoc />
    public override byte Icon => 2;

    /// <inheritdoc />
    public override string Name => "Adaptive Skin";

    public override void OnApplied()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You glance around then your skin hardens.");
        UpdateStats();
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() => UpdateStats();

    public override void OnTerminated()
    {
        // Remove the current bonuses
        if (LastMonsterCount >= 0)
            Subject.StatSheet.SubtractBonus(
                new Attributes
                {
                    Ac = -AcBonus,
                    MagicResistance = MagicResistBonus
                });

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your skin returns to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("IronSkin"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You already have Adaptive Skin applied.");

            return false;
        }

        return true;
    }

    private void UpdateStats()
    {
        var monstercount = Subject.MapInstance
                                  .GetEntities<Creature>()
                                  .Count(x => x.WithinRange(Subject, 8) && x.IsHostileTo(Subject));

        if (monstercount != LastMonsterCount)
        {
            // Remove the previous bonuses
            if (LastMonsterCount >= 0)
                Subject.StatSheet.SubtractBonus(
                    new Attributes
                    {
                        Ac = -AcBonus,
                        MagicResistance = MagicResistBonus
                    });

            if (LastMonsterCount > 12)
                LastMonsterCount = 12;

            // Calculate the new bonuses
            AcBonus = monstercount + 1;
            MagicResistBonus = monstercount + 10;
            LastMonsterCount = monstercount;

            // Apply the new bonuses
            Subject.StatSheet.AddBonus(
                new Attributes
                {
                    Ac = -AcBonus,
                    MagicResistance = MagicResistBonus
                });

            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        }
    }
}