using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Objects.World;
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
                        var hasStage = aisling.Enums.TryGetValue(out RionaRatQuestStage stage);

                        if (!hasStage || (stage != RionaRatQuestStage.StartedRatQuest))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("StartedRatQuest", 5))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("StartedRatQuest");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }

                    case "crypt_bat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Bat))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }

                    case "crypt_centipede":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Centipede1))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_centipede2":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Centipede2))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_giantbat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.GiantBat))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_kardi":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Kardi))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_marauder":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Marauder))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_mimic":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Mimic))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_rat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Rat))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_scorpion":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Scorpion))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_spider":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Spider1))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_spider2":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Spider2))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_succubus":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Succubus))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_white_bat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.WhiteBat))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("CryptSlayer", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("CryptSlayer");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-1":
                    case "horse2-1":
                    case "horse3-1":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Lower))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("PurpleHorse", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("PurpleHorse");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                        
                    }
                    case "horse1-2":
                    case "horse2-2":
                    case "horse3-2":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("GrayHorse", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("GrayHorse");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-3":
                    case "horse2-3":
                    case "horse3-3":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("RedHorse", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("RedHorse");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1_boss":
                    case "horse2_boss":
                    case "horse3_boss":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("AppleJack", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Big Bunny.");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("AppleJack", 3))
                            {
                                aisling.Counters.AddOrIncrement("AppleJack");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("AppleJack");
                        aisling.SendOrangeBarMessage($"You defeated Apple Jack {value} times!");

                        break;
                    }
                    case "bunny1-1":
                    case "bunny2-1":
                    case "bunny3-1":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Lower))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("WhiteBunny", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("WhiteBunny");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1-2":
                    case "bunny2-2":
                    case "bunny3-2":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("BrownBunny", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("BrownBunny");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1-3":
                    case "bunny2-3":
                    case "bunny3-3":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("PurpleBunny", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("PurpleBunny");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1_boss":
                    case "bunny2_boss":
                    case "bunny3_boss":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("MrHopps", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Horse Leader.");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("MrHopps", 3))
                            {
                                aisling.Counters.AddOrIncrement("MrHopps");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("MrHopps");
                        aisling.SendOrangeBarMessage($"You defeated Mr.Hopps {value} times!");
                        
                        break;
                    }
                    case "wolf1-1":
                    case "wolf2-1":
                    case "wolf3-1":
                    {
                        var hasFrog = aisling.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.Lower))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("MythicWolf", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("MythicWolf");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wolf1-2":
                    case "wolf2-2":
                    case "wolf3-2":
                    {
                        var hasFrog = aisling.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("WhiteWolf", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("WhiteWolf");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wolf1-3":
                    case "wolf2-3":
                    case "wolf3-3":
                    {
                        var hasFrog = aisling.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.Higher))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("BeardedWolf", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("BeardedWolf");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wolf1_boss":
                    case "wolf2_boss":
                    case "wolf3_boss":
                    {
                        var hasFrog = aisling.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("Nymeria", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to King Frog.");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("Nymeria", 3))
                            {
                                aisling.Counters.AddOrIncrement("Nymeria");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("Nymeria");
                        aisling.SendOrangeBarMessage($"You defeated Nymeria {value} times!");
                        
                        break;
                    }
                    case "mantis1-1":
                    case "mantis2-1":
                    case "mantis3-1":
                    {
                        var hasBee = aisling.Enums.TryGetValue(out MythicBee bee);

                        if (!hasBee || (bee != MythicBee.Lower))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("MythicMantis", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("MythicMantis");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "mantis1-2":
                    case "mantis2-2":
                    case "mantis3-2":
                    {
                        var hasBee = aisling.Enums.TryGetValue(out MythicBee bee);

                        if (!hasBee || (bee != MythicBee.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("BrownMantis", 20))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("BrownMantis");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "mantis1_boss":
                    case "mantis2_boss":
                    case "mantis3_boss":
                    {
                        var hasBee = aisling.Enums.TryGetValue(out MythicBee bee);

                        if (!hasBee || (bee != MythicBee.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("FireTree", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Bee Queen.");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("FireTree", 3))
                            {
                                aisling.Counters.AddOrIncrement("FireTree");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("FireTree");
                        aisling.SendOrangeBarMessage($"You defeated Fire Tree {value} times!");
                        
                        break;
                    }
                    case "bee1-1":
                    case "bee2-1":
                    case "bee3-1":
                    {
                        var hasMantis = aisling.Enums.TryGetValue(out MythicMantis mantis);

                        if (!hasMantis || (mantis != MythicMantis.Lower))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("MythicBee", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("MythicBee");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bee1-2":
                    case "bee2-2":
                    case "bee3-2":
                    {
                        var hasMantis = aisling.Enums.TryGetValue(out MythicMantis mantis);

                        if (!hasMantis || (mantis != MythicMantis.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("GreenBee", 20))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("GreenBee");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bee1_boss":
                    case "bee2_boss":
                    case "bee3_boss":
                    {
                        var hasMantis = aisling.Enums.TryGetValue(out MythicMantis mantis);

                        if (!hasMantis || (mantis != MythicMantis.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("Carolina", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Mantis King.");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("Carolina", 3))
                            {
                                aisling.Counters.AddOrIncrement("Carolina");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("Carolina");
                        aisling.SendOrangeBarMessage($"You defeated Carolina {value} times!");
                        
                        break;
                    }
                    case "frog1-1":
                    case "frog2-1":
                    case "frog3-1":
                    {
                        var hasWolf = aisling.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.Lower))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("MythicFrog", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("MythicFrog");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "frog1-2":
                    case "frog2-2":
                    case "frog3-2":
                    {
                        var hasWolf = aisling.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("BlueFrog", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("BlueFrog");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "frog1-3":
                    case "frog2-3":
                    case "frog3-3":
                    {
                        var hasWolf = aisling.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("RedFrog", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("RedFrog");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "frog1_boss":
                    case "frog2_boss":
                    case "frog3_boss":
                    {
                        var hasWolf = aisling.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("Frogger", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Wolf Pack Leader.");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("Frogger", 3))
                            {
                                aisling.Counters.AddOrIncrement("Frogger");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("Frogger");
                        aisling.SendOrangeBarMessage($"You defeated Frogger {value} times!");
                        
                        break;
                    } 
                    case "gargoyle1-1":
                    case "gargoyle2-1":
                    case "gargoyle3-1":
                    {
                        var hasZombie = aisling.Enums.TryGetValue(out MythicZombie zombie);

                        if (!hasZombie || (zombie != MythicZombie.Lower))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("MythicDunan", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("MythicDunan");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "gargoyle1-2":
                    case "gargoyle2-2":
                    case "gargoyle3-2":
                    {
                        var hasZombie = aisling.Enums.TryGetValue(out MythicZombie zombie);

                        if (!hasZombie || (zombie != MythicZombie.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("GargoyleServant", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("GargoyleServant");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "gargoyle1-3":
                    case "gargoyle2-3":
                    case "gargoyle3-3":
                    {
                        var hasZombie = aisling.Enums.TryGetValue(out MythicZombie zombie);

                        if (!hasZombie || (zombie != MythicZombie.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("GargoyleGuard", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("GargoyleGuard");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "gargoyle1_boss":
                    case "gargoyle2_boss":
                    case "gargoyle3_boss":
                    {
                        var hasZombie = aisling.Enums.TryGetValue(out MythicZombie zombie);

                        if (!hasZombie || (zombie != MythicZombie.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("GargoyleFiend", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Superior Zombie");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("GargoyleFiend", 3))
                            {
                                aisling.Counters.AddOrIncrement("GargoyleFiend");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("GargoyleFiend");
                        aisling.SendOrangeBarMessage($"You defeated Gargoyle Fiend {value} times!");
                        
                        break;
                    }
                    case "zombie1-1":
                    case "zombie2-1":
                    case "zombie3-1":
                    {
                        var hasGargoyle = aisling.Enums.TryGetValue(out MythicGargoyle gargoyle);

                        if (!hasGargoyle || (gargoyle != MythicGargoyle.Lower))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("ZombieGrunt", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("ZombieGrunt");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "zombie1-2":
                    case "zombie2-2":
                    case "zombie3-2":
                    {
                        var hasGargoyle = aisling.Enums.TryGetValue(out MythicGargoyle gargoyle);

                        if (!hasGargoyle || (gargoyle != MythicGargoyle.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("ZombieSoldier", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("ZombieSoldier");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "zombie1-3":
                    case "zombie2-3":
                    case "zombie3-3":
                    {
                        var hasGargoyle = aisling.Enums.TryGetValue(out MythicGargoyle gargoyle);

                        if (!hasGargoyle || (gargoyle != MythicGargoyle.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("ZombieLumberjack", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("ZombieLumberjack");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "zombie1_boss":
                    case "zombie2_boss":
                    case "zombie3_boss":
                    {
                        var hasGargoyle = aisling.Enums.TryGetValue(out MythicGargoyle gargoyle);

                        if (!hasGargoyle || (gargoyle != MythicGargoyle.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("Brains", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Lord Gargoyle.");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("Brains", 3))
                            {
                                aisling.Counters.AddOrIncrement("Brains");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("Brains");
                        aisling.SendOrangeBarMessage($"You defeated Brains {value} times!");
                        
                        break;
                    }
                    case "grimlock1-1":
                    case "grimlock2-1":
                    case "grimlock3-1":
                    {
                        var hasKobold = aisling.Enums.TryGetValue(out MythicKobold kobold);

                        if (!hasKobold || (kobold != MythicKobold.Lower))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("GrimlockWorker", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("GrimlockWorker");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "grimlock1-2":
                    case "grimlock2-2":
                    case "grimlock3-2":
                    {
                        var hasKobold = aisling.Enums.TryGetValue(out MythicKobold kobold);

                        if (!hasKobold || (kobold != MythicKobold.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("GrimlockRogue", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("GrimlockRogue");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "grimlock1-3":
                    case "grimlock2-3":
                    case "grimlock3-3":
                    {
                        var hasKobold = aisling.Enums.TryGetValue(out MythicKobold kobold);

                        if (!hasKobold || (kobold != MythicKobold.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("GrimlockGuard", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("GrimlockGuard");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "grimlock1_boss":
                    case "grimlock2_boss":
                    case "grimlock3_boss":
                    {
                        var hasKobold = aisling.Enums.TryGetValue(out MythicKobold kobold);

                        if (!hasKobold || (kobold != MythicKobold.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("GrimlockPrincess", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Kobold Pack Leader.");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("GrimlockPrincess", 3))
                            {
                                aisling.Counters.AddOrIncrement("GrimlockPrincess");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("GrimlockPrincess");
                        aisling.SendOrangeBarMessage($"You defeated Grimlock Princess {value} times!");
                        
                        break;
                    }
                    case "kobold1-1":
                    case "kobold2-1":
                    case "kobold3-1":
                    {
                        var hasGrimlock = aisling.Enums.TryGetValue(out MythicGrimlock grimlock);

                        if (!hasGrimlock || (grimlock != MythicGrimlock.Lower))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("KoboldWorker", 15))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("KoboldWorker");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "kobold3-2":
                    case "kobold2-2":
                    case "kobold1-2":
                    {
                        var hasGrimlock = aisling.Enums.TryGetValue(out MythicGrimlock grimlock);

                        if (!hasGrimlock || (grimlock != MythicGrimlock.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("KoboldSoldier", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("KoboldSoldier");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "kobold1-3":
                    case "kobold2-3":
                    case "kobold3-3":
                    {
                        var hasGrimlock = aisling.Enums.TryGetValue(out MythicGrimlock grimlock);

                        if (!hasGrimlock || (grimlock != MythicGrimlock.Higher))
                            return;

                        if (aisling.Counters.CounterGreaterThanOrEqualTo("KoboldWarrior", 10))
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("KoboldWarrior");
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "kobold1_boss":
                    case "kobold2_boss":
                    case "kobold3_boss":
                    {
                        var hasGrimlock = aisling.Enums.TryGetValue(out MythicGrimlock grimlock);

                        if (!hasGrimlock || (grimlock != MythicGrimlock.BossStarted))
                            return;
                        
                        if (aisling.Counters.CounterGreaterThanOrEqualTo("Shank", 2))
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}. Return to Grimlock Queen.");

                            if (!aisling.Counters.CounterGreaterThanOrEqualTo("Shank", 3))
                            {
                                aisling.Counters.AddOrIncrement("Shank");
                            }

                            return;
                        }

                        var value = aisling.Counters.AddOrIncrement("Shank");
                        aisling.SendOrangeBarMessage($"You defeated Shank {value} times!");
                        
                        break;
                    }
                    
                }
    }
}