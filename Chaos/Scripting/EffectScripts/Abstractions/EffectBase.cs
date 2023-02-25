using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.EffectScripts.Abstractions;

public abstract class EffectBase : ScriptBase, IEffect
{
    public EffectColor Color { get; set; }

    public TimeSpan Remaining
    {
        get => Duration - Elapsed;
        set => Elapsed = Duration - value;
    }

    public Creature Subject { get; set; } = null!;
    protected TimeSpan Elapsed { get; private set; }
    public abstract byte Icon { get; }
    public abstract string Name { get; }

    /// <inheritdoc />
#pragma warning disable CS0108, CS0114
    public string ScriptKey { get; }
#pragma warning restore CS0108, CS0114
    protected Aisling? AislingSubject => Subject as Aisling;
    protected abstract TimeSpan Duration { get; }

    protected EffectBase() => ScriptKey = GetEffectKey(GetType());

    public static string GetEffectKey(Type type) => type.Name.ReplaceI("effect", string.Empty);

    /// <inheritdoc />
    public virtual void OnApplied() { }

    public virtual void OnDispelled() { }

    /// <inheritdoc />
    public virtual void OnReApplied() => OnApplied();

    public virtual void OnTerminated() { }

    /// <inheritdoc />
    public virtual bool ShouldApply(Creature source, Creature target) => !target.Effects.Contains(Name);

    public virtual void Update(TimeSpan delta)
    {
        Elapsed += delta;

        var currentColor = this.GetColor();

        if (Color != currentColor)
        {
            Color = currentColor;
            AislingSubject?.Client.SendEffect(Color, Icon);
        }
    }
}