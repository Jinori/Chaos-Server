using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class AmnesiaEffect : ContinuousAnimationEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);

    /// <inheritdoc />
    private Aisling? SourceOfEffect { get; set; }

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 42
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(700));

    /// <inheritdoc />
    public override byte Icon => 15;

    /// <inheritdoc />
    public override string Name => "Amnesia";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject is not Monster monster)
            return;

        monster.ResetAggro();
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source as Aisling;

        if (Subject is not Monster monster)
            return false;

        if (Subject.Script.Is<ThisIsABossScript>())
        {
            SourceOfEffect?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The target is not affected by Amnesia.");

            return false;
        }

        if (Subject.Effects.Contains("Amnesia"))
            return false;

        monster.ResetAggro();

        return true;
    }
}