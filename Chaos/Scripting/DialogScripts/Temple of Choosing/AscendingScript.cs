using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class AscendingScript : DialogScriptBase
{

    private readonly IExperienceDistributionScript ExperienceDistributionScript;

    public AscendingScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();


    private int CheckTimesToAscend(Aisling source, string hPorMP)
    {
        var loopCounter = 0;
        var gains = hPorMP == "HP" ? 50 : 25;
        var baseVal = hPorMP == "HP" ? source.StatSheet.MaximumHp : source.StatSheet.MaximumMp;
        float currentExp = source.UserStatSheet.TotalExp;

        for (; (currentExp -= (baseVal + gains * loopCounter) * 500) >= 0; loopCounter++) { }

        return loopCounter;
    }
    
    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.Level <= 98)
        {
            Subject.Reply(source, "You must be level 99 to buy health and mana.");
            source.SendOrangeBarMessage("You may not ascend until level 99.");
            return;
        }

        var beforeBaseHealth = source.StatSheet.MaximumHp;
        var beforeBaseMana = source.StatSheet.MaximumMp;

        if (!ExperienceDistributionScript.TryTakeExp(source, source.StatSheet.MaximumHp * 500))
        {
            Subject.Reply(source, "You do not have enough experience to buy vitality. Go increase your knowledge.");
            return;
        }
        

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "aoife_buyhealthforallexp":
            {
                var timesToAscend = CheckTimesToAscend(source, "HP");
                var hp = new Attributes
                {
                    MaximumHp = 50
                };

                if (timesToAscend < 1)
                {
                   
                    Subject.Reply(source, "You do not have enough experience to buy vitality. Go increase your knowledge.");
                    return;
                }

                for (var i = 0; i < timesToAscend; i++)
                {
                    //Get Current Base Health
                    var hpFormula = source.StatSheet.MaximumHp * 500;
                    //Take Exp
                    ExperienceDistributionScript.TryTakeExp(source, hpFormula);
                    //Add Health
                    source.StatSheet.Add(hp);
                }
                
                //Calculate old value to new health and show user
                var newHealth = beforeBaseMana + (timesToAscend * 50);
                source.Client.SendAttributes(StatUpdateType.Full);
                source.SendOrangeBarMessage($"You've increased to {newHealth} base health from {beforeBaseMana}.");
            }

                break;
            
            case "aoife_buymanaforallexp":
            {
                var timesToAscend = CheckTimesToAscend(source, "MP");
                var mp = new Attributes
                {
                    MaximumMp = 25
                };
                
                if (timesToAscend < 1)
                {
                   
                    Subject.Reply(source, "You do not have enough experience to buy vitality. Go increase your knowledge.");
                    return;
                }

                for (var i = 0; i < timesToAscend; i++)
                {
                    //Get Current Base Mana
                    var mpFormula = source.StatSheet.MaximumMp * 500;
                    //Take Exp
                    ExperienceDistributionScript.TryTakeExp(source, mpFormula);
                    //Add Mana
                    source.StatSheet.Add(mp);
                }
                
                //Calculate old value to new mana and show user
                var newMana = beforeBaseMana + (timesToAscend * 25);
                source.Client.SendAttributes(StatUpdateType.Full);
                source.SendOrangeBarMessage($"You've increased to {newMana} base mana from {beforeBaseMana}.");
            }

                break;
            
            case "aoife_buyhealthonce":
            {
                if (ExperienceDistributionScript.TryTakeExp(source, source.StatSheet.MaximumHp * 500))
                {
                    var hp = new Attributes
                    {
                        MaximumHp = 50
                    };
                    
                    source.StatSheet.Add(hp);
                    var newHealth = beforeBaseHealth + (1 * 50);
                    source.Client.SendAttributes(StatUpdateType.Full);
                    source.SendOrangeBarMessage($"Health increased to {newHealth} from {beforeBaseHealth}.");
                }
            }

                break;

            case "aoife_buymanaonce":
            {
                if (ExperienceDistributionScript.TryTakeExp(source, source.StatSheet.MaximumMp * 500))
                {
                    var mp = new Attributes
                    {
                        MaximumMp = 25
                    };
                    
                    source.StatSheet.Add(mp);
                    var newMana = beforeBaseMana + (1 * 50);
                    source.Client.SendAttributes(StatUpdateType.Full);
                    source.SendOrangeBarMessage($"Health increased to {newMana} from {beforeBaseMana}.");
                }
            }

                break;
        }
    }
}