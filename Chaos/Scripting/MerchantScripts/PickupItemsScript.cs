using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class PickupItemsScript : MerchantScriptBase
{
    private readonly List<string> _sayings = new()
    {
        "Oh, I could use one of these!", "Yoink!", "You missed the altar..", "King Bruce gives a good payment for {Item}!",
        "Daddy needs a new pair of {Item}!", "Glioca blessed be!"
    };

    public PickupItemsScript(Merchant subject)
        : base(subject) { }

    public override void Update(TimeSpan delta)
    {
        var now = DateTime.UtcNow;
        const double timeSpanSeconds = 3;
        foreach (var item in Subject.MapInstance.GetEntitiesWithinRange<GroundItem>(Subject).Where(x => now - x.Creation > TimeSpan.FromSeconds(timeSpanSeconds)))
        {
            Subject.MapInstance.RemoveObject(item);
            var saying = _sayings.PickRandom().Inject(item.Name);
            Subject.Say(saying);
        }
    }
}
