using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.HideEffects;

public sealed class TrueHideEffect : EffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(120);

    /// <inheritdoc />
    public override byte Icon => 8;

    /// <inheritdoc />
    public override string Name => "True Hide";

    /// <inheritdoc />
    public override void OnApplied()
    {
        var monstersnearby = Subject.MapInstance.GetEntitiesWithinRange<Monster>(Subject);

        Subject.SetVisibility(VisibilityType.TrueHidden);

        foreach (var monster in monstersnearby)
            monster.AggroList.Remove(Subject.Id, out _);
    }

    /// <inheritdoc />
    public override void OnTerminated() => Subject.SetVisibility(VisibilityType.Normal);

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Visibility is not VisibilityType.Normal)
        {
            AislingSubject?.SendOrangeBarMessage("You are already hidden.");

            return false;
        }

        return base.ShouldApply(source, target);
    }
}