using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class GreetingScript : MerchantScriptBase
{
    public GreetingScript(Merchant subject) : base(subject)
    {
    }

    public override void OnApproached(Creature source)
    {
        Subject.Say($"Hello {source.Name}!");
    }
}