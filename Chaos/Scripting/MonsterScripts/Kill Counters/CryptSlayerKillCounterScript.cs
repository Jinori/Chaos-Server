using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MonsterScripts.Kill_Counters;

public class CryptSlayerKillCounterScript : ConfigurableMonsterScriptBase
{
    #region ScriptVars
    public string? QuestEnum { get; init; }
    #endregion
    
    /// <inheritdoc />
    public CryptSlayerKillCounterScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnDeath()
    {
        var rewardTarget = Subject.Contribution
                                  .OrderByDescending(kvp => kvp.Value)
                                  .Select(kvp => Subject.MapInstance.TryGetObject<Aisling>(kvp.Key, out var a) ? a : null)
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
                CryptSlayerStage stage;

                if (Enum.TryParse(QuestEnum, out CryptSlayerStage parsedStage))
                {
                    stage = parsedStage;
                }
                else
                {
                    // Handle the case where Quest value is not a valid CryptSlayerStage enum value
                    // You can decide what action to take or skip the iteration
                    continue;
                }

                if (aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage currentStage) && (currentStage == stage))
                {
                    if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                    {
                        aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");
                        return;
                    }
                    
                    var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                    aisling.Client.SendServerMessage(ServerMessageType.PersistentMessage, $"{value.ToWords().Titleize()} - {Subject.Template.Name}");
                }
            }
        }
    }
}