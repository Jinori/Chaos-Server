using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.BankScripts;

public class GoldExchangeScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public GoldExchangeScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "goldexchange_platachequebuy":
            {
                OnDisplayingPlataChequeBuy(source);

                break;
            }            
            case "goldexchange_platachequesell":
            {
                OnDisplayingPlataChequeSell(source);

                break;
            }
            case "goldexchange_verdechequebuy":
            {
                OnDisplayingVerdeChequeBuy(source);

                break;
            }
            case "goldexchange_verdechequesell":
            {
                OnDisplayingVerdeChequeSell(source);

                break;
            }
            case "goldexchange_carmesichequebuy":
            {
                OnDisplayingCarmeChequeBuy(source);

                break;
            }
            case "goldexchange_carmesichequesell":
            {
                OnDisplayingCarmesiChequeSell(source);

                break;
            }
        }
    }

    private void OnDisplayingCarmesiChequeSell(Aisling source)
    {
        if (source.Inventory.CountOfByTemplateKey("carmesicheque") == 0)
        {
            Subject.Reply(source, "You do not have any Carmesi cheques to sell.");

            return;
        }

        if (!source.TryGiveGold(15000000))
        {
            Subject.Reply(source, "You cannot hold this much gold in your inventory.");
            
        }
        
        source.Inventory.RemoveQuantityByTemplateKey("carmesicheque", 1);
    }

    private void OnDisplayingVerdeChequeSell(Aisling source)
    {
        if (source.Inventory.CountOfByTemplateKey("verdecheque") == 0)
        {
            Subject.Reply(source, "You do not have any Verde cheques to sell.");

            return;
        }

        if (!source.TryGiveGold(10000000))
        {
            Subject.Reply(source, "You cannot hold this much gold in your inventory.");
            
        }
        
        source.Inventory.RemoveQuantityByTemplateKey("verdecheque", 1);
    }

    private void OnDisplayingPlataChequeSell(Aisling source)
    {
        if (source.Inventory.CountOfByTemplateKey("platacheque") == 0)
        {
            Subject.Reply(source, "You do not have any Plata cheques to sell.");

            return;
        }

        if (!source.TryGiveGold(5000000))
        {
            Subject.Reply(source, "You cannot hold this much gold in your inventory.");
            
        }
        
        source.Inventory.RemoveQuantityByTemplateKey("platacheque", 1);
    }

    private void OnDisplayingCarmeChequeBuy(Aisling source)
    {
        if (source.Gold < 15000000)
        {
            Subject.Reply(source, "You don't have that much gold on you.");
            
            return;
        }

        if (source.Inventory.IsFull)
        {
            Subject.Reply(source, "You don't have enough inventory space. Please make room.");

            return;
        }

        if (source.TryTakeGold(15000000))
        {
            var cheque = ItemFactory.Create("carmesicheque");
            source.Inventory.TryAddToNextSlot(cheque);
        }
    }

    private void OnDisplayingVerdeChequeBuy(Aisling source)
    {
        if (source.Gold < 10000000)
        {
            Subject.Reply(source, "You don't have that much gold on you.");
            
            return;
        }

        if (source.Inventory.IsFull)
        {
            Subject.Reply(source, "You don't have enough inventory space. Please make room.");

            return;
        }

        if (source.TryTakeGold(10000000))
        {
            var cheque = ItemFactory.Create("verdecheque");
            source.Inventory.TryAddToNextSlot(cheque);
        }
    }

    private void OnDisplayingPlataChequeBuy(Aisling source)
    {
        if (source.Gold < 5000000)
        {
            Subject.Reply(source, "You don't have that much gold on you.");
            
            return;
        }

        if (source.Inventory.IsFull)
        {
            Subject.Reply(source, "You don't have enough inventory space. Please make room.");

            return;
        }

        if (source.TryTakeGold(5000000))
        {
            var cheque = ItemFactory.Create("platacheque");
            source.Inventory.TryAddToNextSlot(cheque);
        }
    }
}