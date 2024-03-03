using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class AscendingScript(Dialog subject) : DialogScriptBase(subject)
{
    private const int HEALTH_GAIN = 50;
    private const int MANA_GAIN = 25;
    private const int ASCEND_LEVEL_REQUIREMENT = 99;

    private readonly IExperienceDistributionScript ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    private int CalculateAscensionsCount(Aisling source, string hPorMP)
    {
        var loopCounter = 0;
        var gains = hPorMP == "HP" ? HEALTH_GAIN : MANA_GAIN;
        var baseVal = hPorMP == "HP" ? source.StatSheet.MaximumHp : source.StatSheet.MaximumMp;
        float currentExp = source.UserStatSheet.TotalExp;

        for (; (currentExp -= (baseVal + gains * loopCounter) * 500) >= 0; loopCounter++) { }

        return loopCounter;
    }

    private void IncreaseAttribute(
        Aisling source,
        string attributeType,
        int timesToAscend,
        int gain
    )
    {
        var statBeforeStarting = attributeType == "HP" ? source.StatSheet.MaximumHp : source.StatSheet.MaximumMp;
        
        for (var i = 0; i < timesToAscend; i++)
        {
            var formula = (statBeforeStarting + gain * i) * 500;

            if (!ExperienceDistributionScript.TryTakeExp(source, formula))
                break;

            switch (attributeType)
            {
                case "HP":
                {
                    var hp = new Attributes
                    {
                        MaximumHp = gain
                    };

                    source.StatSheet.Add(hp);

                    break;
                }
                case "MP":
                {
                    var mp = new Attributes
                    {
                        MaximumMp = gain
                    };

                    source.StatSheet.Add(mp);

                    break;
                }
            }
        }

        var newBaseValue = statBeforeStarting + timesToAscend * gain;
        source.Client.SendAttributes(StatUpdateType.Full);
        source.SendOrangeBarMessage($"You've increased to {newBaseValue} base {attributeType.ToLower()} from {statBeforeStarting}.");
    }

    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.Level < ASCEND_LEVEL_REQUIREMENT)
        {
            Subject.Reply(source, "You must be level 99 to buy health and mana.");
            source.SendOrangeBarMessage("You may not ascend until level 99.");

            return;
        }
        
        int timesToAscend;

        if (!ExperienceDistributionScript.TryTakeExp(source, source.StatSheet.MaximumHp * 500) || (!ExperienceDistributionScript.TryTakeExp(source, source.StatSheet.MaximumMp * 250)))
        {
            Subject.Reply(source, "You do not have enough experience to buy vitality. Go increase your knowledge.");

            return;
        }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "aoife_buyhealthforallexp":
                timesToAscend = CalculateAscensionsCount(source, "HP");

                IncreaseAttribute(
                    source,
                    "HP",
                    timesToAscend,
                    HEALTH_GAIN);

                break;

            case "aoife_buymanaforallexp":
                timesToAscend = CalculateAscensionsCount(source, "MP");

                IncreaseAttribute(
                    source,
                    "MP",
                    timesToAscend,
                    MANA_GAIN);

                break;

            case "aoife_buyhealthonce":
                IncreaseAttribute(
                    source,
                    "HP",
                    1,
                    HEALTH_GAIN);

                break;

            case "aoife_buymanaonce":
                IncreaseAttribute(
                    source,
                    "MP",
                    1,
                    MANA_GAIN);

                break;
        }
    }
}