using System.Reflection;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MonsterScripts.Kill_Counters;

public class ChristmasKillCounterScript(Monster subject) : ConfigurableMonsterScriptBase(subject)
{
    private static Type? GetEnumType(string? enumValue)
    {
        if (enumValue is null)
            return null;

        var enumTypes = Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(t => t.IsEnum);

        foreach (var enumType in enumTypes)
            if (Enum.IsDefined(enumType, enumValue))
                return enumType;

        return null;
    }

    public override void OnDeath()
    {
        // Get the primary reward target based on contribution
        var rewardTarget = Subject.Contribution
                                  .OrderByDescending(kvp => kvp.Value)
                                  .Select(kvp => Subject.MapInstance.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
                                  .FirstOrDefault(a => a is not null);

        Aisling[]? rewardTargets = null;

        if (rewardTarget != null)

            // Get all players within range 13 of the reward target
            rewardTargets = Subject.MapInstance
                                   .GetEntities<Aisling>()
                                   .Where(a => a.WithinRange(rewardTarget, 13))
                                   .ToArray();

        if (rewardTargets is not null)
            foreach (var aisling in rewardTargets)
            {
                var stageType = GetEnumType(QuestEnum);

                if (stageType is null)

                    // Skip if QuestEnum does not resolve to a valid enum type
                    continue;

                if (Enum.TryParse(stageType, QuestEnum, out var currentStage) && (currentStage.ToString() == QuestEnum))
                {
                    aisling.Trackers.Counters.TryGetValue(Counter, out var killedamt);
                    aisling.Trackers.Enums.TryGetValue(stageType, out var stage);

                    if (!Equals(stage, currentStage))
                        continue;

                    if (killedamt < QuantityReq)
                    {
                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Counter, QuantityReq))
                            return;

                        var value = aisling.Trackers.Counters.AddOrIncrement(Counter);

                        aisling.Client.SendServerMessage(
                            ServerMessageType.PersistentMessage,
                            $"{value.ToWords().Titleize()} - {Subject.Template.Name}");
                    } else
                    {
                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Counter, QuantityReq))
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");
                    }
                }
            }
    }

    #region ScriptVars
    public string? QuestEnum { get; init; }
    public string Counter { get; init; } = null!;
    public int QuantityReq { get; set; }
    #endregion
}