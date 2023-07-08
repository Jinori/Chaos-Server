using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MonsterScripts;

public class KillCounterScript : MonsterScriptBase
{
    /// <inheritdoc />
    public KillCounterScript(Monster subject)
        : base(subject) { }

    private void HandleAbominationKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out IceWallQuest abomination) || (abomination != IceWallQuest.KillBoss))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("abomination", 1))
        {
            aisling.SendOrangeBarMessage("You didn't defeat the Abomination yet.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleTavernRatKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out RionaRatQuestStage ratquest) || (ratquest != RionaRatQuestStage.StartedRatQuest))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("tavern_rat", 4))
        {
            aisling.SendOrangeBarMessage($"You've killed {Subject.Template.Name}.");

            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("tavern_rat", 5))
                aisling.Trackers.Counters.AddOrIncrement("tavern_rat");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleBeeBossKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicMantis mantis) || (mantis != MythicMantis.BossStarted))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Carolina", 2))
        {
            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Mantis King.");

            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Carolina", 3))
                aisling.Trackers.Counters.AddOrIncrement("Carolina");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleBeeKill(Aisling aisling)
    {
        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 15))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleFrogBossKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicWolf wolf) || (wolf != MythicWolf.BossStarted))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Frogger", 2))
        {
            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Wolf Pack Leader.");

            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Frogger", 3))
                aisling.Trackers.Counters.AddOrIncrement("Frogger");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleFrogKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicWolf wolf) || (wolf != MythicWolf.Lower))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 15))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleGargoyleBossKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicZombie zombie) || (zombie != MythicZombie.BossStarted))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GargoyleFiend", 2))
        {
            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Superior Zombie");

            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GargoyleFiend", 3))
                aisling.Trackers.Counters.AddOrIncrement("GargoyleFiend");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleGargoyleKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicZombie zombie) || (zombie != MythicZombie.Lower))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 15))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleGrimlockBossKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicKobold kobold) || (kobold != MythicKobold.BossStarted))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GrimlockPrincess", 2))
        {
            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Kobold Pack Leader.");

            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GrimlockPrincess", 3))
                aisling.Trackers.Counters.AddOrIncrement("GrimlockPrincess");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleGrimlockKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicKobold kobold) || (kobold != MythicKobold.Lower))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 15))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleKoboldBossKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicGrimlock grimlock) || (grimlock != MythicGrimlock.BossStarted))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Shank", 2))
        {
            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Grimlock Queen.");

            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Shank", 3))
                aisling.Trackers.Counters.AddOrIncrement("Shank");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleKoboldKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicGrimlock grimlock) || (grimlock != MythicGrimlock.Lower))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 15))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleManorGhostKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out ManorLouegieStage louegieStage) || (louegieStage != ManorLouegieStage.AcceptedQuest))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("manorghost", 100))
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

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Bee", 5))
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

    private void HandleZombieBossKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicGargoyle gargoyle) || (gargoyle != MythicGargoyle.BossStarted))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Brains", 2))
        {
            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Lord Gargoyle.");

            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Brains", 3))
                aisling.Trackers.Counters.AddOrIncrement("Brains");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleZombieKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out MythicGargoyle gargoyle) || (gargoyle != MythicGargoyle.Lower))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 15))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptSuccubusKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage suc) || (suc != CryptSlayerStage.Succubus))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptMimicKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage mimic) || (mimic != CryptSlayerStage.Mimic))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptSpider2Kill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage spider2) || (spider2 != CryptSlayerStage.Spider2))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptSpiderKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage spider) || (spider != CryptSlayerStage.Spider1))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptScorpionKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage scorpion) || (scorpion != CryptSlayerStage.Scorpion))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptMarauderKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage mar) || (mar != CryptSlayerStage.Marauder))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptKardiKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage kardi) || (kardi != CryptSlayerStage.Kardi))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptWhiteBatKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage whitebat) || (whitebat != CryptSlayerStage.WhiteBat))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptGiantBatKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage giantbat) || (giantbat != CryptSlayerStage.GiantBat))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptCentipede2Kill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage centipede2) || (centipede2 != CryptSlayerStage.Centipede2))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptCentipedeKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage centipede) || (centipede != CryptSlayerStage.Centipede1))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptRatKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage rat) || (rat != CryptSlayerStage.Rat))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
        {
            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

            return;
        }

        IncrementCounter(aisling);
    }

    private void HandleCryptBatKill(Aisling aisling)
    {
        if (!aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage bat) || (bat != CryptSlayerStage.Bat))
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(Subject.Template.TemplateKey, 10))
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
            foreach (var aisling in rewardTargets)
                switch (Subject.Template.TemplateKey.ToLowerInvariant())
                {
                    case "bee1-1":
                    case "bee2-1":
                    case "bee3-1":
                        HandleBeeKill(aisling);

                        break;
                    case "bee1-2":
                    case "bee2-2":
                    case "bee3-2":
                        HandleBeeKill(aisling);

                        break;
                    case "bee1-3":
                    case "bee2-3":
                    case "bee3-3":
                        HandleBeeKill(aisling);

                        break;
                    case "bee1_boss":
                    case "bee2_boss":
                    case "bee3_boss":
                        HandleBeeBossKill(aisling);

                        break;
                    case "frog1-1":
                    case "frog2-1":
                    case "frog3-1":
                        HandleFrogKill(aisling);

                        break;
                    case "frog1-2":
                    case "frog2-2":
                    case "frog3-2":
                        HandleFrogKill(aisling);

                        break;
                    case "frog1-3":
                    case "frog2-3":
                    case "frog3-3":
                        HandleFrogKill(aisling);

                        break;
                    case "frog1_boss":
                    case "frog2_boss":
                    case "frog3_boss":
                        HandleFrogBossKill(aisling);

                        break;
                    case "gargoyle1-1":
                    case "gargoyle2-1":
                    case "gargoyle3-1":
                        HandleGargoyleKill(aisling);

                        break;
                    case "gargoyle1-2":
                    case "gargoyle2-2":
                    case "gargoyle3-2":
                        HandleGargoyleKill(aisling);

                        break;
                    case "gargoyle1-3":
                    case "gargoyle2-3":
                    case "gargoyle3-3":
                        HandleGargoyleKill(aisling);

                        break;
                    case "gargoyle1_boss":
                    case "gargoyle2_boss":
                    case "gargoyle3_boss":
                        HandleGargoyleBossKill(aisling);

                        break;
                    case "zombie1-1":
                    case "zombie2-1":
                    case "zombie3-1":
                        HandleZombieKill(aisling);

                        break;
                    case "zombie1-2":
                    case "zombie2-2":
                    case "zombie3-2":
                        HandleZombieKill(aisling);

                        break;
                    case "zombie1-3":
                    case "zombie2-3":
                    case "zombie3-3":
                        HandleZombieKill(aisling);

                        break;
                    case "zombie1_boss":
                    case "zombie2_boss":
                    case "zombie3_boss":
                        HandleZombieBossKill(aisling);

                        break;
                    case "grimlock1-1":
                    case "grimlock2-1":
                    case "grimlock3-1":
                        HandleGrimlockKill(aisling);

                        break;
                    case "grimlock1-2":
                    case "grimlock2-2":
                    case "grimlock3-2":
                        HandleGrimlockKill(aisling);

                        break;
                    case "grimlock1-3":
                    case "grimlock2-3":
                    case "grimlock3-3":
                        HandleGrimlockKill(aisling);

                        break;
                    case "grimlock1_boss":
                    case "grimlock2_boss":
                    case "grimlock3_boss":
                        HandleGrimlockBossKill(aisling);

                        break;
                    case "kobold1-1":
                    case "kobold2-1":
                    case "kobold3-1":
                        HandleKoboldKill(aisling);

                        break;
                    case "kobold1-2":
                    case "kobold2-2":
                    case "kobold3-2":
                        HandleKoboldKill(aisling);

                        break;
                    case "kobold1-3":
                    case "kobold2-3":
                    case "kobold3-3":
                        HandleKoboldKill(aisling);

                        break;
                    case "kobold1_boss":
                    case "kobold2_boss":
                    case "kobold3_boss":
                        HandleKoboldBossKill(aisling);

                        break;
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
                    case "crypt_rat":
                        HandleCryptRatKill(aisling);

                        break;
                    case "crypt_bat":
                        HandleCryptBatKill(aisling);

                        break;
                    case "crypt_centipede":
                        HandleCryptCentipedeKill(aisling);

                        break;
                    case "crypt_centipede2":
                        HandleCryptCentipede2Kill(aisling);

                        break;
                    case "crypt_giantbat":
                        HandleCryptGiantBatKill(aisling);

                        break;
                    case "crypt_whitebat":
                        HandleCryptWhiteBatKill(aisling);

                        break;
                    case "crypt_kardi":
                        HandleCryptKardiKill(aisling);

                        break;
                    case "crypt_marauder":
                        HandleCryptMarauderKill(aisling);

                        break;
                    case "crypt_scorpion":
                        HandleCryptScorpionKill(aisling);

                        break;
                    case "crypt_spider":
                        HandleCryptSpiderKill(aisling);

                        break;
                    case "crypt_spider2":
                        HandleCryptSpider2Kill(aisling);

                        break;
                    case "crypt_mimic":
                        HandleCryptMimicKill(aisling);

                        break;
                    case "crypt_succubus":
                        HandleCryptSuccubusKill(aisling);

                        break;
                }
    }
}