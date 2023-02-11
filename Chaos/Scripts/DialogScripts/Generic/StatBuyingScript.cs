using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts.Generic;

public class StatBuyingScript : DialogScriptBase
{

    public StatBuyingScript(Dialog subject)
        : base(subject) { }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is null)
            return;

        switch (source.UserStatSheet.BaseClass)
        {
            case BaseClass.Warrior:
            {
                if (source.StatSheet.MaximumHp <= 4149)
                {
                    source.SendOrangeBarMessage("Doing so would make you too feeble to stand. Return with more health.");
                    Subject.Close(source);
                    return;
                }
            }
                break;
            
            case BaseClass.Wizard:
            {
                if (source.StatSheet.MaximumHp <= 3899)
                {
                    source.SendOrangeBarMessage("Doing so would make you too feeble to stand. Return with more health.");
                    Subject.Close(source);
                    return;
                }
            }
                break;
            
            case BaseClass.Priest:
            {
                if (source.StatSheet.MaximumHp <= 3649)
                {
                    source.SendOrangeBarMessage("Doing so would make you too feeble to stand. Return with more health.");
                    Subject.Close(source);
                    return;
                }
            }
                break;
            
            case BaseClass.Monk:
            {
                if (source.StatSheet.MaximumHp <= 6149)
                {
                    source.SendOrangeBarMessage("Doing so would make you too feeble to stand. Return with more health.");
                    Subject.Close(source);
                    return;
                }
            }
                break;
            
            case BaseClass.Rogue:
            {
                if (source.StatSheet.MaximumHp <= 4399)
                {
                    source.SendOrangeBarMessage("Doing so would make you too feeble to stand. Return with more health.");
                    Subject.Close(source);
                    return;
                }
            }
                break;
        }

        var statBuyCost = new Attributes
        {
            MaximumHp = 150
        };
        
        source.StatSheet.Subtract(statBuyCost);

        switch (optionIndex)
        {
            case 1:
            {
                var str = new Attributes { Str = 1 };
                source.UserStatSheet.Add(str);
                source.SendOrangeBarMessage($"Strength increased by one to {source.UserStatSheet.Str}. 150 Health taken.");

                break;
            }
            case 2:
            {
                var intel = new Attributes { Int = 1 };
                source.UserStatSheet.Add(intel);
                source.SendOrangeBarMessage($"Intelligence increased by one to {source.UserStatSheet.Int}. 150 Health taken.");

                break;
            }
            case 3:
            {
                var wis = new Attributes { Wis = 1 };
                source.UserStatSheet.Add(wis);
                source.SendOrangeBarMessage($"Wisdom increased by one to {source.UserStatSheet.Wis}. 150 Health taken.");

                break;
            }
            case 4:
            {
                var con = new Attributes { Con = 1 };
                source.UserStatSheet.Add(con);
                source.SendOrangeBarMessage($"Constitution increased by one to {source.UserStatSheet.Con}. 150 Health taken.");

                break;
            }
            case 5:
            {
                var dex = new Attributes { Dex = 1 };
                source.UserStatSheet.Add(dex);
                source.SendOrangeBarMessage($"Dexterity increased by one to {source.UserStatSheet.Dex}. 150 Health taken.");

                break;
            }
        }
        source.Client.SendAttributes(StatUpdateType.Primary);
    }
    
    
    public override void OnDisplaying(Aisling source)
    {
        switch (source.UserStatSheet.BaseClass)
        {
            case BaseClass.Warrior:
            {
               var formula = (source.StatSheet.MaximumHp - 4000) / 150;
                Subject.Text = $"Looks like you can get {formula} stats. Which one did you want?";
            }

                break;
            
            case BaseClass.Wizard:
            {
                var formula = (source.StatSheet.MaximumHp - 3750) / 150;
                Subject.Text = $"Looks like you can get {formula} stats. Which one did you want?";
            }

                break;
                
            case BaseClass.Priest:
            {
                var formula = (source.StatSheet.MaximumHp - 3500) / 150;
                Subject.Text = $"Looks like you can get {formula} stats. Which one did you want?";
            }

                break;
            
            case BaseClass.Monk:
            {
                var formula = (source.StatSheet.MaximumHp - 6000) / 150;
                Subject.Text = $"Looks like you can get {formula} stats. Which one did you want?";
            }

                break;
            
            case BaseClass.Rogue:
            {
                var formula = (source.StatSheet.MaximumHp - 4250) / 150;
                Subject.Text = $"Looks like you can get {formula} stats. Which one did you want?";
            }

                break;
        }
    }
}