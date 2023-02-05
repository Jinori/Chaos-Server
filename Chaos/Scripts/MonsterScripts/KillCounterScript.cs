using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Objects.World;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class KillCounterScript : MonsterScriptBase
{
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public KillCounterScript(Monster subject)
        : base(subject) => ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    /// <inheritdoc />
    public override void OnDeath()
    {
        var rewardTarget = Subject.Contribution
                                  .OrderByDescending(kvp => kvp.Value)
                                  .Select(kvp => Map.TryGetObject<Aisling>(kvp.Key, out var a) ? a : null)
                                  .FirstOrDefault(a => a is not null)
                           ?? Subject.AggroList
                                     .OrderByDescending(kvp => kvp.Value)
                                     .Select(kvp => Map.TryGetObject<Aisling>(kvp.Key, out var a) ? a : null)
                                     .FirstOrDefault(a => a is not null);

        Aisling[]? rewardTargets = null;

        if (rewardTarget != null)
            rewardTargets = (rewardTarget.Group ?? (IEnumerable<Aisling>)new[] { rewardTarget })
                            .ThatAreWithinRange(rewardTarget)
                            .ToArray();

        if (rewardTargets is not null)
        {
            foreach (var aisling in rewardTargets)
            {
                if (aisling.Counters.ContainsKey(Subject.Template.TemplateKey))
                {
                    aisling.Counters[Subject.Template.TemplateKey] += 1;

                    var hasRatStage = aisling.Enums.TryGetValue(out RionaRatQuestStage stage);
                    if (hasRatStage && (stage == RionaRatQuestStage.StartedRatQuest))
                    {
                        aisling.SendServerMessage(ServerMessageType.PersistentMessage, $"Killed " + aisling.Counters[Subject.Template.TemplateKey] + " " +Subject.Template.Name);
                    }
                }
                else
                {
                    aisling.Counters.TryAdd(Subject.Template.TemplateKey, 1);
                    aisling.SendServerMessage(ServerMessageType.PersistentMessage, $"Killed " + aisling.Counters[Subject.Template.TemplateKey] + " " +Subject.Template.Name);
                }
            }
        }
    }
}