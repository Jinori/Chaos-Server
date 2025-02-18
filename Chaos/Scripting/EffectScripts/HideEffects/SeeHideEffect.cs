using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.HideEffects;

public sealed class SeeHideEffect : EffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(30);

    public override byte Icon => 6;

    /// <inheritdoc />
    public override string Name => "See Hide";

    /// <inheritdoc />
    public override void OnApplied()
    {
        var ani = new Animation
        {
            TargetAnimation = 169,
            AnimationSpeed = 200,
            Priority = 1
        };

        Subject.Animate(ani);

        AislingSubject?.SendOrangeBarMessage("You can now detect hidden things");
        AislingSubject?.Refresh(true);
    }

    /// <inheritdoc />
    public override void OnTerminated()
    {
        AislingSubject?.SendOrangeBarMessage("You can no longer detect hidden things");
        AislingSubject?.Refresh(true);
    }
}