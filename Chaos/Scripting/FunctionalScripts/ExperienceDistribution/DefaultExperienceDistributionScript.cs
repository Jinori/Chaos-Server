#region
using System.Diagnostics;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.LevelUp;
using Chaos.Scripting.WorldScripts.WorldBuffs.Guild;
using Chaos.Scripting.WorldScripts.WorldBuffs.Religion;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

public class DefaultExperienceDistributionScript : ScriptBase, IExperienceDistributionScript
{
    public DefaultExperienceDistributionScript(ILogger<DefaultExperienceDistributionScript> logger, IStorage<GuildBuffs> guildBuffStorage, IStorage<ReligionBuffs> religionBuffStorage)
    {
        Logger = logger;
        GuildBuffStorage = guildBuffStorage;
        ReligionBuffStorage = religionBuffStorage;
    }
    private const double EXPERIENCE_MULTIPLIER = 1.0; // Default is 1.0, can be lowered or raised as needed
    public virtual IExperienceFormula ExperienceFormula { get; set; } = ExperienceFormulae.Default;
    public virtual ILevelUpScript LevelUpScript { get; set; } = DefaultLevelUpScript.Create();

    protected ILogger Logger { get; }
    protected IStorage<GuildBuffs> GuildBuffStorage { get; }
    protected IStorage<ReligionBuffs> ReligionBuffStorage { get; }

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultExperienceDistributionScript));

    /// <inheritdoc />
    public static IExperienceDistributionScript Create() => FunctionalScriptRegistry.Instance.Get<IExperienceDistributionScript>(Key);

    /// <inheritdoc />
    public virtual void DistributeExperience(Creature killedCreature, params ICollection<Aisling> aislings)
    {
        var baseExp = ExperienceFormula.Calculate(killedCreature, aislings);

        // Distribute the experience to each Aisling
        foreach (var aisling in aislings)
        {
            var finalExp = ApplyBonuses(baseExp, aisling);
            GiveExp(aisling, finalExp);
        }
    }

    protected virtual long ApplyBonuses(long exp, Aisling aisling)
    {
        var totalBonus = 1m; // Start with 1 (no bonus)

        // Apply a 5% experience bonus for "Knowledge"
        if (HasKnowledgeEffect(aisling))
            totalBonus += 0.05m;

        // Apply a 10% experience bonus for "Strong Knowledge"
        if (HasStrongKnowledgeEffect(aisling))
            totalBonus += 0.10m;

        // Apply a 25% experience bonus for "Skandara's World Buff" or "GM Trinket, but won't allow them to stack
        if (HasGMKnowledgeEffect(aisling) || HasReligionBuff(ReligionScriptBase.SKANDARA_GLOBAL_BUFF_NAME))
            totalBonus += 0.25m;

        if (HasValentinesCandyEffect(aisling))
            totalBonus += 0.4m;

        // Apply an additional 5% bonus for mythic completion
        if (HasCompletedMythic(aisling))
            totalBonus += 0.05m;

        if (HasGuildBuff(aisling, GuildBuffScript.GUILD_25_EXP_BUFF_NAME))
            totalBonus += 0.25m;
            
        if (HasEpicMaster(aisling))
            totalBonus += 0.05m;

        // Calculate the final experience with bonuses applied
        return (long)(exp * totalBonus);
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

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Experience, Topics.Actions.Remove)
                  .WithProperty(aisling)
                  .WithProperty(stackTrace)
                  .LogError("Tried to take {Amount:N0} experience from {@AislingName}", amount, aisling.Name);

            return false;
        }

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

    private bool HasCompletedMythic(Aisling aisling)
    {
        if (aisling.Trackers.Enums.HasValue(MythicQuestMain.CompletedMythic))
            return true;

        return false;
    }

    private bool HasGuildBuff(Aisling aisling, string buffName)
    {
        if (aisling.Guild != null)
            if (GuildBuffStorage.Value.ActiveBuffs.Any(
                    buff => buff.GuildName.EqualsI(aisling.Guild.Name) && buff.BuffName.EqualsI(buffName)))
                return true;

        return false;
    }
    
    private bool HasReligionBuff(string buffName)
    {
        if (ReligionBuffStorage.Value.ActiveBuffs.Any(buff => buff.BuffName.EqualsI(buffName)))
            return true;

        return false;
    }
    
    private bool HasEpicMaster(Aisling aisling)
    {
        if (aisling.Legend.ContainsKey("epicbountymark"))
            return true;

        return false;
    }

    private bool HasGMKnowledgeEffect(Aisling aisling)
    {
        // Check if any of the Aislings have the "Strong Knowledge" effect
        if (aisling.Effects.Contains("GMKnowledge"))
            return true;

        return false;
    }

    private bool HasKnowledgeEffect(Aisling aisling)
    {
        if (aisling.Effects.Contains("Knowledge"))
            return true;

        return false;
    }

    private bool HasStrongKnowledgeEffect(Aisling aisling)
    {
        if (aisling.Effects.Contains("Strong Knowledge"))
            return true;

        return false;
    }

    private bool HasValentinesCandyEffect(Aisling aisling)
    {
        if (aisling.Effects.Contains("ValentinesCandy"))
            return true;

        return false;
    }
}