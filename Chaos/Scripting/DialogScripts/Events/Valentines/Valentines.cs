using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Events.Valentines;

public class Valentines(Dialog subject, IEffectFactory effectFactory) : DialogScriptBase(subject)
{
    public override void OnDisplaying(Aisling source)
    {
        if (Subject.DialogSource is Merchant merchant)
        {
            var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, merchant.MapInstance.InstanceId);

            if (!isEventActive)
            {
                Subject.Reply(source, "Looks like I'm all out of candies..");
                return;
            }

            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "nadia_initial":
                    var effect = effectFactory.Create("ValentinesCandy");
                    source.SendOrangeBarMessage("Nadia stuffs a chocolate in your face. Knowledge rates increased!");
                    source.Effects.Apply(source, effect);
                    break;
            }
        }
    }

}