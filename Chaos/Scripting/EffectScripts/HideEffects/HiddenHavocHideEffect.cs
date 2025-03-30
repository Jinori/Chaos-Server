using Chaos.Definitions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.HideEffects;

public sealed class HiddenHavocHideEffect : EffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(100);

    /// <inheritdoc />
    public override byte Icon => 8;

    /// <inheritdoc />
    public override string Name => "HiddenHavocHide";

    /// <inheritdoc />
    public override void OnApplied()
    {
        foreach (var effect in Subject.Effects)
            if (effect != this)
                Subject.Effects.Dispel(effect.Name);

        Subject.SetVisibility(VisibilityType.Hidden);
    }

    /// <inheritdoc />
    public override void OnTerminated() => Subject.SetVisibility(VisibilityType.Normal);
}