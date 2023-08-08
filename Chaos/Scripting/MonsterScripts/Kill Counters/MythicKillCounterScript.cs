using System.Reflection;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MonsterScripts.Kill_Counters;

public class MythicKillCounterScript : ConfigurableMonsterScriptBase
{
    #region ScriptVars
    public string? QuestEnum { get; init; }

    public string? Leader { get; init; }

    public string Counter { get; init; }

    public int QuantityReq { get; set; }

    public bool IsMythicBoss { get; set; }

    #endregion

    /// <inheritdoc />
    public MythicKillCounterScript(Monster subject)
        : base(subject)
    {
    }

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
            foreach (var aisling in rewardTargets)
            {
                var stageType = GetEnumType(QuestEnum);
                if (stageType is null)
                {
                    // Handle the case where Quest value is not a valid enum value
                    // You can decide what action to take or skip the iteration
                    continue;
                }

                if (Enum.TryParse(stageType, QuestEnum, out var currentStage) && (currentStage.ToString() == QuestEnum))
                {
                    aisling.Trackers.Counters.TryGetValue(Counter, out var killedamt);
                    aisling.Trackers.Enums.TryGetValue(stageType, out var stage);

                    if (!Equals(stage, currentStage))
                        continue;

                    if (killedamt < QuantityReq)
                    {
                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Counter, QuantityReq))
                            continue;
                        
                        var value = aisling.Trackers.Counters.AddOrIncrement(Counter); 
                        aisling.Client.SendServerMessage(
                            ServerMessageType.PersistentMessage,
                            $"{value.ToWords().Titleize()} - {Subject.Template.Name}");
                    }
                    
                    if (IsMythicBoss)
                    {
                        if (aisling.Trackers.Counters.CounterLessThanOrEqualTo(Counter, QuantityReq - 1))
                        {
                            aisling.SendOrangeBarMessage($"{Subject.Template.Name} has retreated.");
                        }
                        else
                        {
                            aisling.SendOrangeBarMessage(
                                $"{Subject.Template.Name} has been defeated. Return to {Leader}.");
                            continue;
                        }
                    }
                    else // Move the else statement here
                    {
                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Counter, QuantityReq))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");
                            continue;
                        }
                    }
                }

            }
    }
    
    private static Type? GetEnumType(string? enumValue)
    {
        if (enumValue is null)
            return null;

        var enumTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum);
        foreach (var enumType in enumTypes)
        {
            if (Enum.IsDefined(enumType, enumValue))
                return enumType;
        }

        return null;
    }
}