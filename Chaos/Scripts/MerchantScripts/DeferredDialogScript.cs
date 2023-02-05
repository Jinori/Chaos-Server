using Chaos.Objects.World;
using Chaos.Scripts.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.MerchantScripts;

public class DeferredDialogScript : MerchantScriptBase
{
    private readonly IMerchantFactory MerchantFactory;

    public DeferredDialogScript(Merchant subject, IMerchantFactory merchantFactory)
        : base(subject) => MerchantFactory = merchantFactory;

    public override void OnClicked(Aisling source)
    {
        var merchant = MerchantFactory.Create("terrorChest", source.MapInstance, new Point(source.X, source.Y));
        merchant.OnClicked(source);
    }
}