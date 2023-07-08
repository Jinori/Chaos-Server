using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Casino;

public class ZertaScript : DialogScriptBase
{
    private readonly IScriptFactory<IMerchantScript, Merchant> ScriptFactory;

    /// <inheritdoc />
    public ZertaScript(Dialog subject, IScriptFactory<IMerchantScript, Merchant> scriptFactory)
        : base(subject) =>
        ScriptFactory = scriptFactory;

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
                    merchant.AddScript(typeof(MerchantScripts.Casino.ZertaScript), ScriptFactory);
                    script = merchant.Script.As<MerchantScripts.Casino.ZertaScript>();
                    script!.Source = source;
                    merchant.Say($"Thanks {source.Name}!");
                }
            }
    }
}