using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Networking.Abstractions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class ClassDedicationScript : DialogScriptBase
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    
    public ClassDedicationScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry) : base(subject)
    {
        ClientRegistry = clientRegistry;
    }


    private void SetUserToLevel1Stats(Aisling source, BaseClass baseClass)
    {
        source.Inventory.RemoveQuantity("ard ioc deum", 10);
        source.UserStatSheet.Assert(
            statref =>
            {
                statref.Level = 1;
                statref.TotalExp = 0;
                statref.UnspentPoints = 0;
                statref.ToNextLevel = 599;
            });
                
        source.UserStatSheet.Str = 1;
        source.UserStatSheet.Int = 1;
        source.UserStatSheet.Wis = 1;
        source.UserStatSheet.Con = 1;
        source.UserStatSheet.Dex = 1;
        source.UserStatSheet.SetMaxWeight(51);
        source.UserStatSheet.SetBaseClass(baseClass);
        var statBuyCost = new Attributes
        {
            MaximumHp = source.UserStatSheet.MaximumHp - 100,
            MaximumMp = source.UserStatSheet.MaximumMp - 100
        };
        source.UserStatSheet.Subtract(statBuyCost);
        source.Client.SendAttributes(StatUpdateType.Full);
        source.Refresh(true);
        
        var player = ClientRegistry
            .Select(c => c.Aisling)
            .Where(a => true);
        
        foreach (var aisling in player)
        {
            aisling.SendOrangeBarMessage($"{source.Name} has dedicated to the path of {baseClass.ToString()}.");
        }
    }
    
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "aoife_confirmrequirements":
            {
                if (source.Legend.ContainsKey("dedicated"))
                {
                    Subject.Text = "You have already dedicated yourself to a new path.";
                    Subject.PrevDialogKey = null;
                    Subject.NextDialogKey = null;
                    return;
                }
                if (source.UserStatSheet.Level != 99)
                {
                    Subject.Text = "You are not Level 99 and cannot dedicate your class.";
                    Subject.PrevDialogKey = null;
                    Subject.NextDialogKey = null;
                    return;
                }
                if (source.UserStatSheet.Master)
                {
                    Subject.Text = "You are already a Master of your class and cannot rededicate yourself.";
                    Subject.PrevDialogKey = null;
                    Subject.NextDialogKey = null;
                    return;
                }
                var requiredHealth = 0;
                var requiredMana = 0;

                switch (source.UserStatSheet.BaseClass)
                {
                    case BaseClass.Monk:
                        requiredHealth = 8400;
                        requiredMana = 3600;
                        break;
                    case BaseClass.Warrior:
                        requiredHealth = 8400;
                        requiredMana = 2000;
                        break;
                    case BaseClass.Rogue:
                        requiredHealth = 6600;
                        requiredMana = 3600;
                        break;
                    case BaseClass.Wizard:
                        requiredHealth = 6000;
                        requiredMana = 6600;
                        break;
                    case BaseClass.Priest:
                        requiredHealth = 6600;
                        requiredMana = 7250;
                        break;
                }
                if (source.UserStatSheet.MaximumHp >= requiredHealth && source.UserStatSheet.MaximumMp >= requiredMana)
                {
                    Subject.Text = "You have enough vitality to continue.";
                }
                else
                {
                    Subject.Text = $"You do not have the required {requiredHealth} health and {requiredMana} mana to continue.";
                    Subject.PrevDialogKey = null;
                    Subject.NextDialogKey = null;
                    return;
                }
                if (source.Inventory.CountOf("ard ioc deum") >= 10)
                {
                    Subject.Text += " Looks like you've also brought enough ard ioc deum";
                }
                else
                {
                    Subject.Text += " but you do not have the required ard ioc deum. Come back with what you need.";
                    Subject.PrevDialogKey = null;
                    Subject.NextDialogKey = null;
                    return;
                }
                if (source.UserStatSheet.TotalExp >= 60000000)
                {
                    Subject.Text += " and you've hunted enough experience! Let's continue.";
                    Subject.NextDialogKey = "aoife_chooseNewClass";
                }
                else
                {
                    Subject.Text += " but you do not have the required experience of 60 million.";
                    Subject.PrevDialogKey = null;
                    Subject.NextDialogKey = null;
                }
                break;
            }

            case "aoife_choosenewclass":
            {
                if (Subject.GetOptionIndex(source.UserStatSheet.BaseClass.ToString()).HasValue)
                {
                    var s = Subject.GetOptionIndex(source.UserStatSheet.BaseClass.ToString())!.Value;
                    Subject.Options.RemoveAt(s);
                }
                break;
            }
            
            case "aoife_selectwizard":
            {
                SetUserToLevel1Stats(source, BaseClass.Wizard);
                source.Legend.AddUnique(new LegendMark("Dedicated to Wizard", "dedicated", MarkIcon.Wizard, MarkColor.Cyan, 1, GameTime.Now));
                break;
            }
            
            case "aoife_selectwarrior":
            {
                SetUserToLevel1Stats(source, BaseClass.Warrior);
                source.Legend.AddUnique(new LegendMark("Dedicated to Warrior", "dedicated", MarkIcon.Warrior, MarkColor.Cyan, 1, GameTime.Now));
                break;
            }
            
            case "aoife_selectrogue":
            {
                SetUserToLevel1Stats(source, BaseClass.Rogue);
                source.Legend.AddUnique(new LegendMark("Dedicated to Rogue", "dedicated", MarkIcon.Rogue, MarkColor.Cyan, 1, GameTime.Now));
                break;
            }
            
            case "aoife_selectpriest":
            {
                SetUserToLevel1Stats(source, BaseClass.Priest);
                source.Legend.AddUnique(new LegendMark("Dedicated to Priest", "dedicated", MarkIcon.Priest, MarkColor.Cyan, 1, GameTime.Now));
                break;
            }
            
            case "aoife_selectmonk":
            {
                SetUserToLevel1Stats(source, BaseClass.Monk);
                source.Legend.AddUnique(new LegendMark("Dedicated to Monk", "dedicated", MarkIcon.Monk, MarkColor.Cyan, 1, GameTime.Now));
                break;
            }
        }
    }
}