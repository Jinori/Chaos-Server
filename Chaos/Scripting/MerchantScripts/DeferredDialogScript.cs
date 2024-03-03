using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class DeferredDialogScript(Merchant subject, IMerchantFactory merchantFactory) : MerchantScriptBase(subject)
{
    public override void OnClicked(Aisling source)
    {
        var merchant = merchantFactory.Create("terrorChest", source.MapInstance, new Point(source.X, source.Y));
        merchant.OnClicked(source);
    }
}