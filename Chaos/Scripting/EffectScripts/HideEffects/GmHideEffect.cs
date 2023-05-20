using Chaos.Definitions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.HideEffects;

public class GmHideEffect : EffectBase
{
    /// <inheritdoc />
    public override byte Icon { get; } = 131;
    /// <inheritdoc />
    public override string Name { get; } = "Gm Hide";
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromDays(100);

    /// <inheritdoc />
    public override void OnApplied() => Subject.SetVisibility(VisibilityType.GmHidden);

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