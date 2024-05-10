using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MonsterScripts.Kill_Counters;

public class KillCounterScript : MonsterScriptBase
{
    /// <inheritdoc />
    public KillCounterScript(Monster subject)
        : base(subject) { }

    private void HandleAbominationKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out IceWallQuest abomination) || (abomination != IceWallQuest.KillBoss))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("wilderness_abomination", 1))
        {
            aisling.SendOrangeBarMessage("You defeated the Abomination!.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleManorGhostKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out ManorLouegieStage louegieStage) || (louegieStage != ManorLouegieStage.AcceptedQuestBanshee))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo($"{Subject.Template.TemplateKey}", 100))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");
            aisling.Trackers.Enums.Set(ManorLouegieStage.CompletedQuest);

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleSnowWolfKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out IceWallQuest wolf) || (wolf != IceWallQuest.KillWolves))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("wolf", 10))
        {
            aisling.SendOrangeBarMessage("You've killed enough Snow Wolves.");

            return;
        }

        SnowWolfIncrementCounter(aisling);
    }

    private void HandleTavernRatKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out RionaTutorialQuestStage ratquest) || (ratquest != RionaTutorialQuestStage.StartedRatQuest))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("tavern_rat", 5))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}s.");

            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("tavern_rat", 5))
                aisling.Trackers.Counters.AddOrIncrement("tavern_rat");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleUndeadKingKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CrHorror undeadKing) || (undeadKing != CrHorror.Start))
            return;

        IncrementCounter(aisling);
        aisling.SendOrangeBarMessage("You defeated the Undead King!");
    }

    private void HandleWildernessBeeKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out BeeProblem bee) || (bee != BeeProblem.Started))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("wilderness_Bee", 5))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleWolfKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out WolfProblemStage wolf) || (wolf != WolfProblemStage.Start))
            return;

        IncrementCounter(aisling);
        aisling.SendOrangeBarMessage("You defeated the Wolf.");
    }

    private void HandleZombieKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicGargoyle gargoyle) || (gargoyle != MythicGargoyle.LowerGargoyle))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 15))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void IncrementCounter(Aisling aisling)
    {
        var value = aisling.Trackers.Counters.AddOrIncrement(Subject.Template.TemplateKey);
        aisling.Client.SendServerMessage(ServerMessageType.PersistentMessage, $"{value.ToWords().Titleize()} - {Subject.Template.Name}");
    }

    /// <inheritdoc />
    public override void OnDeath() => ProcessKillCount();

    public void ProcessKillCount()
    {
        var rewardTarget = Subject.Contribution
                                  .OrderByDescending(kvp => kvp.Value)
                                  .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
                                  .FirstOrDefault(a => a is not null)
                           ?? Subject.AggroList
                                     .OrderByDescending(kvp => kvp.Value)
                                     .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
                                     .FirstOrDefault(a => a is not null);

        Aisling[]? rewardTargets = null;

        if (rewardTarget != null)
            rewardTargets = (rewardTarget.Group ?? (IEnumerable<Aisling>)new[] { rewardTarget })
                            .ThatAreWithinRange(rewardTarget)
                            .ToArray();

        if (rewardTargets is not null)
            foreach (var aisling in rewardTargets)
                switch (Subject.Template.TemplateKey.ToLowerInvariant())
                {
                    case "wilderness_questwolf":
                        HandleWolfKill(aisling);

                        break;
                    case "undead_king":
                        HandleUndeadKingKill(aisling);

                        break;
                    case "wilderness_bee":
                        HandleWildernessBeeKill(aisling);

                        break;
                    case "wilderness_snowwolf1":
                    case "wilderness_snowwolf2":
                        HandleSnowWolfKill(aisling);

                        break;
                    case "wilderness_abomination":
                        HandleAbominationKill(aisling);

                        break;
                    case "manorghost":
                        HandleManorGhostKill(aisling);

                        break;
                    case "tavern_rat":
                        HandleTavernRatKill(aisling);

                        break;
                }
    }

    private void SnowWolfIncrementCounter(Aisling aisling)
    {
        var value = aisling.Trackers.Counters.AddOrIncrement("wolf");
        aisling.Client.SendServerMessage(ServerMessageType.PersistentMessage, $"{value.ToWords().Titleize()} - Snow Wolf");
    }
}