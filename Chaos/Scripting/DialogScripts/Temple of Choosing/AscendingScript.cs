using System.Globalization;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class AscendingScript(Dialog subject, ILogger<AscendingScript> logger) : DialogScriptBase(subject)
{
    private const int HEALTH_GAIN = 50;
    private const int MANA_GAIN = 25;
    private const int ASCEND_LEVEL_REQUIREMENT = 99;

    private readonly IExperienceDistributionScript ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    private int CalculateAscensionsCount(Aisling source, string hPorMp)
    {
        var loopCounter = 0;
        var gains = hPorMp == "HP" ? HEALTH_GAIN : MANA_GAIN;
        var baseVal = hPorMp == "HP" ? source.StatSheet.MaximumHp : source.StatSheet.MaximumMp;
        float currentExp = source.UserStatSheet.TotalExp;

        for (; (currentExp -= (baseVal + gains * loopCounter) * 500) >= 0; loopCounter++) { }

        return loopCounter;
    }

    private void IncreaseAttribute(
        Aisling source,
        string attributeType,
        int timesToAscend,
        int gain)
    {
        var statBeforeStarting = attributeType == "HP" ? source.StatSheet.MaximumHp : source.StatSheet.MaximumMp;
        var totalExpSpent = 0f;

        for (var i = 0; i < timesToAscend; i++)
        {
            var formula = (statBeforeStarting + gain * i) * 500;

            if (!ExperienceDistributionScript.TryTakeExp(source, formula))
                break;

            totalExpSpent += formula;

            switch (attributeType)
            {
                case "HP":
                {
                    var hp = new Attributes
                    {
                        MaximumHp = gain
                    };

                    source.StatSheet.Add(hp);

                    logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Dialog, Topics.Actions.Reward)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has bought {@Attribute} health", source.Name, hp);

                    break;
                }
                case "MP":
                {
                    var mp = new Attributes
                    {
                        MaximumMp = gain
                    };

                    source.StatSheet.Add(mp);

                    logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Dialog, Topics.Actions.Reward)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has bought {@Attribute} mana", source.Name, mp);

                    break;
                }
            }
        }

        var newBaseValue = statBeforeStarting + timesToAscend * gain;
        source.Client.SendAttributes(StatUpdateType.Full);

        source.SendOrangeBarMessage(
            $"Your {attributeType.ToLower()} is now {newBaseValue} base from {statBeforeStarting}, spending {totalExpSpent:N0} Exp.");

        var totalExp = totalExpSpent.ToString("N0");
        Subject.InjectTextParameters(attributeType, newBaseValue, statBeforeStarting, totalExp);
    }

    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.Level < ASCEND_LEVEL_REQUIREMENT)
        {
            Subject.Reply(source, "You must be level 99 to buy health and mana.");
            source.SendOrangeBarMessage("You may not ascend until level 99.");

            return;
        }


        if (source.HasClass(BaseClass.Priest) && source.UserStatSheet.Master && !source.Trackers.Enums.HasValue(MasterPriestPath.Light) && !source.Trackers.Enums.HasValue(MasterPriestPath.Dark))
        {
            Subject.Reply(source, "You haven't picked a priest path yet Aisling. To ascend any further, you must pick Dark or Light path.");
            source.SendOrangeBarMessage("You must pick Dark or Light path.");

            return;
        }

        int timesToAscend;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "skandaragod_buyhealthforallexp":
                if (source.UserStatSheet.TotalExp < (source.StatSheet.MaximumHp * 500))
                {
                    source.SendOrangeBarMessage("You do not have enough experience to buy health.");
                    Subject.Reply(source, "You do not have enough experience to buy vitality. Go increase your knowledge.");

                    return;
                }

                timesToAscend = CalculateAscensionsCount(source, "HP");

                IncreaseAttribute(
                    source,
                    "HP",
                    timesToAscend,
                    HEALTH_GAIN);

                break;

            case "theselenegod_buymanaforallexp":
                if (source.UserStatSheet.TotalExp < (source.StatSheet.MaximumMp * 500))
                {
                    source.SendOrangeBarMessage("You do not have enough experience to buy mana.");

                    Subject.Reply(
                        source,
                        "Your journey through the veiled paths of experience is yet incomplete, seeker. Wander further into the realm of shadows, gather your tales and trials, then return when you are ready to trade them for the whispers of power.");

                    return;
                }

                timesToAscend = CalculateAscensionsCount(source, "MP");

                IncreaseAttribute(
                    source,
                    "MP",
                    timesToAscend,
                    MANA_GAIN);

                break;

            case "skandaragod_buyhealthonce":
                if (source.UserStatSheet.TotalExp < (source.StatSheet.MaximumHp * 500))
                {
                    source.SendOrangeBarMessage("You do not have enough experience to buy health.");
                    Subject.Reply(source, "You do not have enough experience to buy vitality. Go increase your knowledge.");

                    return;
                }

                IncreaseAttribute(
                    source,
                    "HP",
                    1,
                    HEALTH_GAIN);

                break;

            case "theselenegod_buymanaonce":
                if (source.UserStatSheet.TotalExp < (source.StatSheet.MaximumMp * 500))
                {
                    source.SendOrangeBarMessage("You do not have enough experience to buy mana.");

                    Subject.Reply(
                        source,
                        "Your journey through the veiled paths of experience is yet incomplete, seeker. Wander further into the realm of shadows, gather your tales and trials, then return when you are ready to trade them for the whispers of power.");

                    return;
                }

                IncreaseAttribute(
                    source,
                    "MP",
                    1,
                    MANA_GAIN);

                break;
        }
    }
}