using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monsters;

public class DrowningEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(45);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 295
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 66;

    /// <inheritdoc />
    public override string Name => "Drowning";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject.MapInstance.Template.TemplateKey != "6599")
        {
            Subject.Effects.Dispel(Name);

            return;
        }

        var safeRectangle = new Rectangle(
            9,
            7,
            7,
            7);
        
        var subjectPoint = new Point(Subject.X, Subject.Y);

        if (safeRectangle.Contains(subjectPoint))
        {
            AislingSubject?.SendOrangeBarMessage("You are no longer drowning!");
            Subject.Effects.Dispel(Name);

            return;
        }

        double maxHp = Subject.StatSheet.EffectiveMaximumHp;
        const double DAMAGE_PERCENTAGE = 0.3;
        const int DAMAGE_CAP = 1000000;

        var damage = (int)Math.Min(maxHp * DAMAGE_PERCENTAGE, DAMAGE_CAP);

        if (Subject.IsGodModeEnabled())
            return;

        if (Subject.IsDead)
            return;

        if (Subject.StatSheet.CurrentHp <= damage)
        {
            Subject.StatSheet.SetHealthPct(0);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.Script.OnDeath();

            return;
        }

        if (Subject.StatSheet.TrySubtractHp(damage))
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are drowning!");

        Subject.Animate(Animation);
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target is not Aisling)
            return false;
        
        if (target.Script.Is<ThisIsABossScript>())
            return false;

        if (target.IsGodModeEnabled())
            return false;

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You cast {Name} on {target.Name}.");
        ;
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{source.Name} casted {Name} on you.");

        return true;
    }
}