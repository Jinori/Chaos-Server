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

    private int GoldAmountTotal { get; set; }

    public PickupItemsScript(Merchant subject)
        : base(subject) { }

    public override void Update(TimeSpan delta)
    {
        var now = DateTime.UtcNow;
        const double TIME_SPAN_SECONDS = 5;
        
        foreach (var item in Subject.MapInstance.GetEntitiesWithinRange<GroundItem>(Subject).Where(x => now - x.Creation > TimeSpan.FromSeconds(TIME_SPAN_SECONDS)))
        {
            if (Subject.Name.EndsWith("'s Gloop", StringComparison.Ordinal))
            {
                Subject.MapInstance.RemoveObject(item);
                var substringEndIndex = Subject.Name.IndexOf("'s Gloop", StringComparison.Ordinal);
                var substring = Subject.Name.Substring(0, substringEndIndex);
                var aisling = Map.GetEntitiesWithinRange<Aisling>(Subject).ToList();
                var gold = item.Item.Template.SellValue * 0.1;

                foreach (var person in aisling)
                {
                    if (person.Name == substring)
                        person.TryGiveGold((int)gold);

                    GoldAmountTotal += (int)gold;
                    person.SendActiveMessage($"Gloop munched {gold} gold. {GoldAmountTotal} total!");
                }
            } 
            else
            {
                Subject.MapInstance.RemoveObject(item);
                var saying = _sayings.PickRandom().Inject(item.Name);
                Subject.Say(saying);
            }
        }
    }
}
