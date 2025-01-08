using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Events;

public class EventTimingScript : MerchantScriptBase
{
    private readonly IIntervalTimer CheckEventActive;

    public EventTimingScript(Merchant subject)
        : base(subject)
        => CheckEventActive = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        CheckEventActive.Update(delta);

        if (!CheckEventActive.IntervalElapsed)
            return;

        var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, Subject.MapInstance.InstanceId);

        var gmPresent = Subject.MapInstance
                               .GetEntities<Aisling>()
                               .Where(x => x.IsGodModeEnabled());

        //Despawns if event is not active and no gm is on the map
        //Reload maps if you're wanting to test near an NPC that will despawn instantly
        if (!isEventActive && !gmPresent.Any())
            Subject.MapInstance.RemoveEntity(Subject);
    }
}