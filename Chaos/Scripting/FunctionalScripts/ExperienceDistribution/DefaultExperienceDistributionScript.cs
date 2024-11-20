#region
using System.Diagnostics;
using Chaos.DarkAges.Definitions;
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
#endregion

namespace Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

public class DefaultExperienceDistributionScript(ILogger<DefaultExperienceDistributionScript> logger)
    : ScriptBase, IExperienceDistributionScript
{
    private const double EXPERIENCE_MULTIPLIER = 1.0; // Default is 1.0, can be lowered or raised as needed
    public IExperienceFormula ExperienceFormula { get; set; } = ExperienceFormulae.Default;
    public ILevelUpScript LevelUpScript { get; set; } = DefaultLevelUpScript.Create();
    public ILogger<DefaultExperienceDistributionScript> Logger { get; set; } = logger;

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultExperienceDistributionScript));

    /// <inheritdoc />
    public static IExperienceDistributionScript Create() => FunctionalScriptRegistry.Instance.Get<IExperienceDistributionScript>(Key);

    /// <inheritdoc />
    public virtual void DistributeExperience(Creature killedCreature, params ICollection<Aisling> aislings)
    {
        var baseExp = ExperienceFormula.Calculate(killedCreature, aislings);
        var totalBonus = 1m; // Start with 1 (no bonus)

        // Apply a 5% experience bonus for "Knowledge"
        if (HasKnowledgeEffect(aislings))
            totalBonus += 0.05m;

        // Apply a 10% experience bonus for "Strong Knowledge"
        if (HasStrongKnowledgeEffect(aislings))
            totalBonus += 0.10m;

        if (HasGMKnowledgeEffect(aislings))
            totalBonus += 0.25m;

        // Apply an additional 5% bonus for mythic completion
        if (HasCompletedMythic(aislings))
            totalBonus += 0.05m;

        // Calculate the final experience with bonuses applied
        var finalExp = baseExp * totalBonus;

        // Distribute the experience to each Aisling
        foreach (var aisling in aislings)
            GiveExp(aisling, (long)finalExp);
    }

    public virtual void GiveExp(Aisling aisling, long amount)
    {
        if (aisling.Trackers.Enums.TryGetValue(out GainExp gainExpOption) && (gainExpOption == GainExp.No))
            return;

        if (aisling.IsDead)
            return;

        if (amount < 0)
        {
            var stackTrace = new StackTrace(true).ToString();

            Logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Entities.Experience,
                          Topics.Actions.Add
                      ])
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

        Logger.WithTopics(
                  [
                      Topics.Entities.Aisling,
                      Topics.Entities.Experience,
                      Topics.Actions.Add
                  ])
              .WithProperty(aisling)
              .LogInformation("Aisling {@AislingName} has gained {Amount:N0} experience", aisling.Name, amount);

        while (amount > 0)
            if (aisling.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel)
            {
                aisling.UserStatSheet.AddTotalExp(amount);
                amount = 0;
            } else
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

            Logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Entities.Experience,
                          Topics.Actions.Remove
                      ])
                  .WithProperty(aisling)
                  .WithProperty(stackTrace)
                  .LogError("Tried to take {Amount:N0} experience from {@AislingName}", amount, aisling.Name);

            return false;
        }

        amount = amount;

        if (aisling.UserStatSheet.TotalExp < amount)
            return false;

        if (!aisling.UserStatSheet.TrySubtractTotalExp(amount))
            return false;

        Logger.WithTopics(
                  [
                      Topics.Entities.Aisling,
                      Topics.Entities.Experience,
                      Topics.Actions.Remove
                  ])
              .WithProperty(aisling)
              .LogInformation("Aisling {@AislingName} has lost {Amount:N0} experience", aisling.Name, amount);

        aisling.Client.SendAttributes(StatUpdateType.ExpGold);

        return true;
    }

    private bool HasCompletedMythic(Aisling[] aislings)
    {
        foreach (var aisling in aislings)
            if (aisling.Trackers.Enums.HasValue(MythicQuestMain.CompletedMythic))
                return true;

        return false;
    }

    private bool HasGMKnowledgeEffect(Aisling[] aislings)
    {
        // Check if any of the Aislings have the "Strong Knowledge" effect
        foreach (var aisling in aislings)
            if (aisling.Effects.Contains("GM Knowledge"))
                return true;

        return false;
    }

    private bool HasKnowledgeEffect(Aisling[] aislings)
    {
        // Check if any of the Aislings have the "Knowledge" effect
        foreach (var aisling in aislings)
            if (aisling.Effects.Contains("Knowledge"))
                return true;

        return false;
    }

    private bool HasStrongKnowledgeEffect(Aisling[] aislings)
    {
        // Check if any of the Aislings have the "Strong Knowledge" effect
        foreach (var aisling in aislings)
            if (aisling.Effects.Contains("Strong Knowledge"))
                return true;

        return false;
    }
}