using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.LevelUp;
using Chaos.Scripting.WorldScripts.WorldBuffs.Guild;
using Chaos.Scripting.WorldScripts.WorldBuffs.Religion;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

public class NonMasterScalingExperienceDistributionScript : DefaultExperienceDistributionScript, IExperienceDistributionScript
{
    /// <inheritdoc />
    public NonMasterScalingExperienceDistributionScript(
        ILogger<NonMasterScalingExperienceDistributionScript> logger,
        IStorage<GuildBuffs> guildBuffStorage,
        IStorage<ReligionBuffs> religionBuffStorage)
        : base(logger, guildBuffStorage, religionBuffStorage) { }

    /// <inheritdoc />
    public override IExperienceFormula ExperienceFormula { get; set; } = ExperienceFormulae.Pure;

    /// <inheritdoc />
    public override ILevelUpScript LevelUpScript { get; set; } = DefaultLevelUpScript.Create();

    /// <inheritdoc />
    public override void DistributeExperience(Creature killedCreature, params ICollection<Aisling> aislings)
    {
        var baseExp = ExperienceFormula.Calculate(killedCreature, aislings);

        var fauxAisling = Aisling.Faux;
        fauxAisling.StatSheet.SetLevel(99);
        var tnlAt98 = LevelUpScript.LevelUpFormula.CalculateTnl(fauxAisling);
        var pctOfTnl = (decimal)baseExp / tnlAt98;
        
        foreach (var aisling in aislings)
        {
            var tnl = LevelUpScript.LevelUpFormula.CalculateTnl(aisling);
            var expFromTnl = (long)(tnl * pctOfTnl);
            var bonusedExp = ApplyBonuses(expFromTnl, aisling);

            GiveExp(aisling, bonusedExp);
        }
    }
    
    /// <inheritdoc />
    public static new string Key { get; } = GetScriptKey(typeof(NonMasterScalingExperienceDistributionScript));

    /// <inheritdoc />
    public static new IExperienceDistributionScript Create() => FunctionalScriptRegistry.Instance.Get<IExperienceDistributionScript>(Key);
}