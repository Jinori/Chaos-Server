using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Scripts.MerchantScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Templates.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.MerchantScripts.Components;

// ReSharper disable once ClassCanBeSealed.Global
public class WanderScript : ConfigurableMerchantScriptBase
{
    private MapInstance Map => Subject.MapInstance;

    private readonly IIntervalTimer WanderTimer;
    protected int WanderIntervalMs { get; init; }
    private bool ShouldWander => WanderTimer.IntervalElapsed;

    /// <inheritdoc />
    public WanderScript(Merchant subject)
    : base(subject) { WanderTimer = new IntervalTimer(TimeSpan.FromMilliseconds(WanderIntervalMs)); }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        WanderTimer.Update(delta);

        if (!ShouldWander)
            return;

        if (!Map.GetEntitiesWithinRange<Aisling>(Subject).Any())
            return;

        Subject.Wander();
    }
}