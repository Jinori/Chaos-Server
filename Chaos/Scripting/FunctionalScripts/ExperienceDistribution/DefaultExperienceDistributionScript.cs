using System.Diagnostics;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.LevelUp;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

public class DefaultExperienceDistributionScript(ILogger<DefaultExperienceDistributionScript> logger)
    : ScriptBase, IExperienceDistributionScript
{
    public IExperienceFormula ExperienceFormula { get; set; } = ExperienceFormulae.Default;
    public ILevelUpScript LevelUpScript { get; set; } = DefaultLevelUpScript.Create();
    public ILogger<DefaultExperienceDistributionScript> Logger { get; set; } = logger;

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultExperienceDistributionScript));

    /// <inheritdoc />
    public static IExperienceDistributionScript Create() => FunctionalScriptRegistry.Instance.Get<IExperienceDistributionScript>(Key);

    private const double EXPERIENCE_MULTIPLIER = 1.0; // Default is 1.0, can be lowered or raised as needed

    /// <inheritdoc />
    public virtual void DistributeExperience(Creature killedCreature, params Aisling[] aislings)
    {
        var exp = ExperienceFormula.Calculate(killedCreature, aislings);

        foreach (var aisling in aislings)
            GiveExp(aisling, exp);
    }

    public virtual void GiveExp(Aisling aisling, long amount)
    {
        if (aisling.Trackers.Enums.TryGetValue(out GainExp gainExpOption) && (gainExpOption == GainExp.No))
            return;

        if (amount < 0)
        {
            var stackTrace = new StackTrace(true).ToString();

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Experience, Topics.Actions.Add)
                  .WithProperty(aisling)
                  .WithProperty(stackTrace)
                  .LogError("Tried to give {Amount:N0} experience to {@AislingName}", amount, aisling.Name);

            return;
        }

        if ((amount + aisling.UserStatSheet.TotalExp) > uint.MaxValue)
            amount = uint.MaxValue - aisling.UserStatSheet.TotalExp;

        amount = (long)(amount * EXPERIENCE_MULTIPLIER);
        
        if (amount <= 0)
            return;

        aisling.SendActiveMessage($"You have gained {amount:N0} experience!");

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Experience, Topics.Actions.Add)
              .WithProperty(aisling)
              .LogInformation("Aisling {@AislingName} has gained {Amount:N0} experience", aisling.Name, amount);

        while (amount > 0)
            if (aisling.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel)
            {
                aisling.UserStatSheet.AddTotalExp(amount);
                amount = 0;
            }
            else
            {
                var expToGive = Math.Min(amount, aisling.UserStatSheet.ToNextLevel);
                aisling.UserStatSheet.AddTotalExp(expToGive);
                aisling.UserStatSheet.SubtractTnl(expToGive);

                amount -= expToGive;

                if (aisling.UserStatSheet.ToNextLevel <= 0)
                    LevelUpScript.LevelUp(aisling);
            }

        aisling.Client.SendAttributes(StatUpdateType.ExpGold);

        if (aisling.UserStatSheet.TotalExp == uint.MaxValue)
            aisling.SendActiveMessage("You cannot gain any more experience");
    }

    public virtual bool TryTakeExp(Aisling aisling, long amount)
    {
        if (amount < 0)
        {
            var stackTrace = new StackTrace(true).ToString();

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Experience, Topics.Actions.Remove)
                  .WithProperty(aisling)
                  .WithProperty(stackTrace)
                  .LogError("Tried to take {Amount:N0} experience from {@AislingName}", amount, aisling.Name);

            return false;
        }

        //amount = (long)(amount * EXPERIENCE_MULTIPLIER); //Change back from exp
        amount *= 1; //Change back from exp
        
        if (aisling.UserStatSheet.TotalExp < amount)
            return false;

        if (!aisling.UserStatSheet.TrySubtractTotalExp(amount))
            return false;

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Experience, Topics.Actions.Remove)
              .WithProperty(aisling)
              .LogInformation("Aisling {@AislingName} has lost {Amount:N0} experience", aisling.Name, amount);

        aisling.Client.SendAttributes(StatUpdateType.ExpGold);

        return true;
    }
}