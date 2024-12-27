using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Events;

public class EventTimingScript(Merchant subject) : MerchantScriptBase(subject)
{
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        
        var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, Subject.MapInstance.InstanceId);
        var gmPresent = Subject.MapInstance.GetEntities<Aisling>().Where(x => x.IsAdmin);
        
        //Despawns if event is not active and no gm is on the map
        //Reload maps if you're wanting to test near an NPC that will despawn instantly
        if (!isEventActive && !gmPresent.Any())
        {
            Subject.MapInstance.RemoveEntity(Subject);
        }
    }
}