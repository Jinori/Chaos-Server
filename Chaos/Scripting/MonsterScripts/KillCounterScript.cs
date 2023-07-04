using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
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
            foreach (var aisling in rewardTargets)
                switch (Subject.Template.TemplateKey.ToLower())
                {
                    case "tavern_rat":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out RionaRatQuestStage stage);

                        if (!hasStage || (stage != RionaRatQuestStage.StartedRatQuest))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("StartedRatQuest", 5))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("StartedRatQuest");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }

                    case "crypt_bat":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Bat))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }

                    case "crypt_centipede":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Centipede1))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_centipede2":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Centipede2))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_giantbat":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.GiantBat))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_kardi":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Kardi))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_marauder":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Marauder))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_mimic":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Mimic))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_rat":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Rat))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_scorpion":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Scorpion))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_spider":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Spider1))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_spider2":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Spider2))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_succubus":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Succubus))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_white_bat":
                    {
                        var hasStage = aisling.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.WhiteBat))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-1":
                    case "horse2-1":
                    case "horse3-1":
                    {
                        var hasBunny = aisling.Trackers.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("PurpleHorse", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("PurpleHorse");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-2":
                    case "horse2-2":
                    case "horse3-2":
                    {
                        var hasBunny = aisling.Trackers.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GrayHorse", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("GrayHorse");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-3":
                    case "horse2-3":
                    case "horse3-3":
                    {
                        var hasBunny = aisling.Trackers.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("RedHorse", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("RedHorse");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1_boss":
                    case "horse2_boss":
                    case "horse3_boss":
                    {
                        var hasBunny = aisling.Trackers.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("AppleJack", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Big Bunny.");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("AppleJack", 3))
                                aisling.Trackers.Counters.AddOrIncrement("AppleJack");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("AppleJack");
                        aisling.SendOrangeBarMessage($"You defeated Apple Jack {value} times!");

                        break;
                    }
                    case "bunny1-1":
                    case "bunny2-1":
                    case "bunny3-1":
                    {
                        var hasHorse = aisling.Trackers.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("WhiteBunny", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("WhiteBunny");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1-2":
                    case "bunny2-2":
                    case "bunny3-2":
                    {
                        var hasHorse = aisling.Trackers.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("BrownBunny", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("BrownBunny");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1-3":
                    case "bunny2-3":
                    case "bunny3-3":
                    {
                        var hasHorse = aisling.Trackers.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("PurpleBunny", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("PurpleBunny");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1_boss":
                    case "bunny2_boss":
                    case "bunny3_boss":
                    {
                        var hasHorse = aisling.Trackers.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("MrHopps", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Horse Leader.");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("MrHopps", 3))
                                aisling.Trackers.Counters.AddOrIncrement("MrHopps");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("MrHopps");
                        aisling.SendOrangeBarMessage($"You defeated Mr.Hopps {value} times!");

                        break;
                    }
                    case "wolf1-1":
                    case "wolf2-1":
                    case "wolf3-1":
                    {
                        var hasFrog = aisling.Trackers.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("MythicWolf", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("MythicWolf");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wolf1-2":
                    case "wolf2-2":
                    case "wolf3-2":
                    {
                        var hasFrog = aisling.Trackers.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("WhiteWolf", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("WhiteWolf");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wolf1-3":
                    case "wolf2-3":
                    case "wolf3-3":
                    {
                        var hasFrog = aisling.Trackers.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("BeardedWolf", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("BeardedWolf");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wolf1_boss":
                    case "wolf2_boss":
                    case "wolf3_boss":
                    {
                        var hasFrog = aisling.Trackers.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Nymeria", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to King Frog.");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Nymeria", 3))
                                aisling.Trackers.Counters.AddOrIncrement("Nymeria");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("Nymeria");
                        aisling.SendOrangeBarMessage($"You defeated Nymeria {value} times!");

                        break;
                    }
                    case "mantis1-1":
                    case "mantis2-1":
                    case "mantis3-1":
                    {
                        var hasBee = aisling.Trackers.Enums.TryGetValue(out MythicBee bee);

                        if (!hasBee || (bee != MythicBee.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("MythicMantis", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("MythicMantis");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "mantis1-2":
                    case "mantis2-2":
                    case "mantis3-2":
                    {
                        var hasBee = aisling.Trackers.Enums.TryGetValue(out MythicBee bee);

                        if (!hasBee || (bee != MythicBee.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("BrownMantis", 20))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("BrownMantis");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "mantis1_boss":
                    case "mantis2_boss":
                    case "mantis3_boss":
                    {
                        var hasBee = aisling.Trackers.Enums.TryGetValue(out MythicBee bee);

                        if (!hasBee || (bee != MythicBee.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("FireTree", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Bee Queen.");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("FireTree", 3))
                                aisling.Trackers.Counters.AddOrIncrement("FireTree");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("FireTree");
                        aisling.SendOrangeBarMessage($"You defeated Fire Tree {value} times!");

                        break;
                    }
                    case "bee1-1":
                    case "bee2-1":
                    case "bee3-1":
                    {
                        var hasMantis = aisling.Trackers.Enums.TryGetValue(out MythicMantis mantis);

                        if (!hasMantis || (mantis != MythicMantis.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("MythicBee", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("MythicBee");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bee1-2":
                    case "bee2-2":
                    case "bee3-2":
                    {
                        var hasMantis = aisling.Trackers.Enums.TryGetValue(out MythicMantis mantis);

                        if (!hasMantis || (mantis != MythicMantis.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GreenBee", 20))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("GreenBee");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bee1_boss":
                    case "bee2_boss":
                    case "bee3_boss":
                    {
                        var hasMantis = aisling.Trackers.Enums.TryGetValue(out MythicMantis mantis);

                        if (!hasMantis || (mantis != MythicMantis.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Carolina", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Mantis King.");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Carolina", 3))
                                aisling.Trackers.Counters.AddOrIncrement("Carolina");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("Carolina");
                        aisling.SendOrangeBarMessage($"You defeated Carolina {value} times!");

                        break;
                    }
                    case "frog1-1":
                    case "frog2-1":
                    case "frog3-1":
                    {
                        var hasWolf = aisling.Trackers.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("MythicFrog", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("MythicFrog");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "frog1-2":
                    case "frog2-2":
                    case "frog3-2":
                    {
                        var hasWolf = aisling.Trackers.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("BlueFrog", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("BlueFrog");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "frog1-3":
                    case "frog2-3":
                    case "frog3-3":
                    {
                        var hasWolf = aisling.Trackers.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("RedFrog", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("RedFrog");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "frog1_boss":
                    case "frog2_boss":
                    case "frog3_boss":
                    {
                        var hasWolf = aisling.Trackers.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Frogger", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Wolf Pack Leader.");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Frogger", 3))
                                aisling.Trackers.Counters.AddOrIncrement("Frogger");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("Frogger");
                        aisling.SendOrangeBarMessage($"You defeated Frogger {value} times!");

                        break;
                    }
                    case "gargoyle1-1":
                    case "gargoyle2-1":
                    case "gargoyle3-1":
                    {
                        var hasZombie = aisling.Trackers.Enums.TryGetValue(out MythicZombie zombie);

                        if (!hasZombie || (zombie != MythicZombie.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("MythicDunan", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("MythicDunan");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "gargoyle1-2":
                    case "gargoyle2-2":
                    case "gargoyle3-2":
                    {
                        var hasZombie = aisling.Trackers.Enums.TryGetValue(out MythicZombie zombie);

                        if (!hasZombie || (zombie != MythicZombie.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GargoyleServant", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("GargoyleServant");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "gargoyle1-3":
                    case "gargoyle2-3":
                    case "gargoyle3-3":
                    {
                        var hasZombie = aisling.Trackers.Enums.TryGetValue(out MythicZombie zombie);

                        if (!hasZombie || (zombie != MythicZombie.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GargoyleGuard", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("GargoyleGuard");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "gargoyle1_boss":
                    case "gargoyle2_boss":
                    case "gargoyle3_boss":
                    {
                        var hasZombie = aisling.Trackers.Enums.TryGetValue(out MythicZombie zombie);

                        if (!hasZombie || (zombie != MythicZombie.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GargoyleFiend", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Superior Zombie");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GargoyleFiend", 3))
                                aisling.Trackers.Counters.AddOrIncrement("GargoyleFiend");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("GargoyleFiend");
                        aisling.SendOrangeBarMessage($"You defeated Gargoyle Fiend {value} times!");

                        break;
                    }
                    case "zombie1-1":
                    case "zombie2-1":
                    case "zombie3-1":
                    {
                        var hasGargoyle = aisling.Trackers.Enums.TryGetValue(out MythicGargoyle gargoyle);

                        if (!hasGargoyle || (gargoyle != MythicGargoyle.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("ZombieGrunt", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("ZombieGrunt");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "zombie1-2":
                    case "zombie2-2":
                    case "zombie3-2":
                    {
                        var hasGargoyle = aisling.Trackers.Enums.TryGetValue(out MythicGargoyle gargoyle);

                        if (!hasGargoyle || (gargoyle != MythicGargoyle.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("ZombieSoldier", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("ZombieSoldier");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "zombie1-3":
                    case "zombie2-3":
                    case "zombie3-3":
                    {
                        var hasGargoyle = aisling.Trackers.Enums.TryGetValue(out MythicGargoyle gargoyle);

                        if (!hasGargoyle || (gargoyle != MythicGargoyle.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("ZombieLumberjack", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("ZombieLumberjack");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "zombie1_boss":
                    case "zombie2_boss":
                    case "zombie3_boss":
                    {
                        var hasGargoyle = aisling.Trackers.Enums.TryGetValue(out MythicGargoyle gargoyle);

                        if (!hasGargoyle || (gargoyle != MythicGargoyle.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Brains", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Lord Gargoyle.");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Brains", 3))
                                aisling.Trackers.Counters.AddOrIncrement("Brains");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("Brains");
                        aisling.SendOrangeBarMessage($"You defeated Brains {value} times!");

                        break;
                    }
                    case "grimlock1-1":
                    case "grimlock2-1":
                    case "grimlock3-1":
                    {
                        var hasKobold = aisling.Trackers.Enums.TryGetValue(out MythicKobold kobold);

                        if (!hasKobold || (kobold != MythicKobold.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GrimlockWorker", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("GrimlockWorker");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "grimlock1-2":
                    case "grimlock2-2":
                    case "grimlock3-2":
                    {
                        var hasKobold = aisling.Trackers.Enums.TryGetValue(out MythicKobold kobold);

                        if (!hasKobold || (kobold != MythicKobold.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GrimlockRogue", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("GrimlockRogue");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "grimlock1-3":
                    case "grimlock2-3":
                    case "grimlock3-3":
                    {
                        var hasKobold = aisling.Trackers.Enums.TryGetValue(out MythicKobold kobold);

                        if (!hasKobold || (kobold != MythicKobold.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GrimlockGuard", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("GrimlockGuard");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "grimlock1_boss":
                    case "grimlock2_boss":
                    case "grimlock3_boss":
                    {
                        var hasKobold = aisling.Trackers.Enums.TryGetValue(out MythicKobold kobold);

                        if (!hasKobold || (kobold != MythicKobold.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GrimlockPrincess", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Kobold Pack Leader.");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("GrimlockPrincess", 3))
                                aisling.Trackers.Counters.AddOrIncrement("GrimlockPrincess");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("GrimlockPrincess");
                        aisling.SendOrangeBarMessage($"You defeated Grimlock Princess {value} times!");

                        break;
                    }
                    case "kobold1-1":
                    case "kobold2-1":
                    case "kobold3-1":
                    {
                        var hasGrimlock = aisling.Trackers.Enums.TryGetValue(out MythicGrimlock grimlock);

                        if (!hasGrimlock || (grimlock != MythicGrimlock.Lower))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("KoboldWorker", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("KoboldWorker");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "kobold3-2":
                    case "kobold2-2":
                    case "kobold1-2":
                    {
                        var hasGrimlock = aisling.Trackers.Enums.TryGetValue(out MythicGrimlock grimlock);

                        if (!hasGrimlock || (grimlock != MythicGrimlock.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("KoboldSoldier", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("KoboldSoldier");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "kobold1-3":
                    case "kobold2-3":
                    case "kobold3-3":
                    {
                        var hasGrimlock = aisling.Trackers.Enums.TryGetValue(out MythicGrimlock grimlock);

                        if (!hasGrimlock || (grimlock != MythicGrimlock.Higher))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("KoboldWarrior", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("KoboldWarrior");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "kobold1_boss":
                    case "kobold2_boss":
                    case "kobold3_boss":
                    {
                        var hasGrimlock = aisling.Trackers.Enums.TryGetValue(out MythicGrimlock grimlock);

                        if (!hasGrimlock || (grimlock != MythicGrimlock.BossStarted))
                            continue;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Shank", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Grimlock Queen.");

                            if (!aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Shank", 3))
                                aisling.Trackers.Counters.AddOrIncrement("Shank");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("Shank");
                        aisling.SendOrangeBarMessage($"You defeated Shank {value} times!");

                        break;
                    }
                    case "wilderness_questwolf":
                    {
                        var hasWolf = aisling.Trackers.Enums.TryGetValue(out WolfProblemStage wolf);

                        if (!hasWolf || (wolf != WolfProblemStage.Start))
                            return;

                        var value = aisling.Trackers.Counters.AddOrIncrement("Wolf");
                        aisling.SendOrangeBarMessage("You defeated the Wolf.");

                        break;
                    }
                    case "undead_king":
                    {
                        var hasUndeadking = aisling.Trackers.Enums.TryGetValue(out CrHorror Undeadking);

                        if (!hasUndeadking || (Undeadking != CrHorror.Start))
                            return;

                        var value = aisling.Trackers.Counters.AddOrIncrement("Undead_king");
                        aisling.SendOrangeBarMessage("You defeated the Undead King!");

                        break;
                    }
                    case "wilderness_bee":
                    {
                        var hasBee = aisling.Trackers.Enums.TryGetValue(out BeeProblem Bee);

                        if (!hasBee || (Bee != BeeProblem.Started))
                            return;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("Bee", 5))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("wilderness_bee");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wilderness_snowwolf1":
                    case "wilderness_snowwolf2":
                    {
                        var haswolf = aisling.Trackers.Enums.TryGetValue(out IceWallQuest wolf);

                        if (!haswolf || (wolf != IceWallQuest.KillWolves))
                            return;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("wolf", 10))
                        {
                            aisling.SendOrangeBarMessage("You've killed enough Snow Wolves.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("wolf");
                        aisling.SendOrangeBarMessage($"You've killed {value} Snow wolves.");

                        break;
                    }
                    case "wilderness_abomination":
                    {
                        var hasabomination = aisling.Trackers.Enums.TryGetValue(out IceWallQuest abomination);

                        if (!hasabomination || (abomination != IceWallQuest.KillBoss))
                            return;

                        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("abomination", 1))
                        {
                            aisling.SendOrangeBarMessage("You didn't defeat the Abomination yet.");

                            continue;
                        }

                        var value = aisling.Trackers.Counters.AddOrIncrement("abomination");
                        aisling.SendOrangeBarMessage("You've Slain the Abomination!");

                        break;
                    }
                }
    }
}