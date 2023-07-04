using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class AscendingScript : DialogScriptBase
{
    private static readonly int HealthGain = 50;
    private static readonly int ManaGain = 25;
    private static readonly int AscendLevelRequirement = 99;

    private readonly IExperienceDistributionScript ExperienceDistributionScript;

    public AscendingScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    private int CalculateAscensionsCount(Aisling source, string hPorMP)
    {
        var loopCounter = 0;
        var gains = hPorMP == "HP" ? HealthGain : ManaGain;
        var baseVal = hPorMP == "HP" ? source.StatSheet.MaximumHp : source.StatSheet.MaximumMp;
        float currentExp = source.UserStatSheet.TotalExp;

        for (; (currentExp -= (baseVal + gains * loopCounter) * 500) >= 0; loopCounter++) { }

        return loopCounter;
    }

    private void IncreaseAttribute(
        Aisling source,
        string attributeType,
        int timesToAscend,
        int gain,
        int beforeBaseValue
    )
    {
        var attribute = new Attributes();

        switch (attributeType)
        {
            case "HP":
                var hp = new Attributes
                {
                    MaximumHp = gain
                };

                source.StatSheet.Add(hp);

                break;
            case "MP":
                var mp = new Attributes
                {
                    MaximumMp = gain
                };

                source.StatSheet.Add(mp);

                break;
            default:
                return;
        }

        for (var i = 0; i < timesToAscend; i++)
        {
            var formula = (source.StatSheet.MaximumHp + gain * i) * 500;

            if (!ExperienceDistributionScript.TryTakeExp(source, formula))
                break;

            source.StatSheet.Add(attribute);
        }

        var newBaseValue = beforeBaseValue + timesToAscend * gain;
        source.Client.SendAttributes(StatUpdateType.Full);
        source.SendOrangeBarMessage($"You've increased to {newBaseValue} base {attributeType.ToLower()} from {beforeBaseValue}.");
    }

    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.Level <= AscendLevelRequirement)
        {
            Subject.Reply(source, "You must be level 99 to buy health and mana.");
            source.SendOrangeBarMessage("You may not ascend until level 99.");

            return;
        }

        var beforeBaseHealth = source.StatSheet.MaximumHp;
        var beforeBaseMana = source.StatSheet.MaximumMp;
        int timesToAscend;

        if (!ExperienceDistributionScript.TryTakeExp(source, source.StatSheet.MaximumHp * 500))
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
                    HealthGain,
                    beforeBaseHealth);

                break;

            case "aoife_buymanaforallexp":
                timesToAscend = CalculateAscensionsCount(source, "MP");

                IncreaseAttribute(
                    source,
                    "MP",
                    timesToAscend,
                    ManaGain,
                    beforeBaseMana);

                break;

            case "aoife_buyhealthonce":
                IncreaseAttribute(
                    source,
                    "HP",
                    1,
                    HealthGain,
                    beforeBaseHealth);

                break;

            case "aoife_buymanaonce":
                IncreaseAttribute(
                    source,
                    "MP",
                    1,
                    ManaGain,
                    beforeBaseMana);

                break;
        }
    }
}