using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Casino;

public class ZertaScript : DialogScriptBase
{
    /// <inheritdoc />
    public ZertaScript(Dialog subject, IScriptProvider scriptProvider)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "zerta_receivedgold":
            {
                OnDisplayingReceivedGold(source);

                break;
            }
        }
    }

    private void OnDisplayingReceivedGold(Aisling source)
    {
        if (source.TryTakeGold(5000))
            if (Subject.DialogSource is Merchant merchant)
            {
                var script = merchant.Script.As<MerchantScripts.Casino.ZertaScript>();

                if (script == null)
                {
                    merchant.AddScript<MerchantScripts.Casino.ZertaScript>();
                    script = merchant.Script.As<MerchantScripts.Casino.ZertaScript>();
                    script!.Source = source;
                    merchant.Say($"Thanks {source.Name}!");
                }
            }
    }
}