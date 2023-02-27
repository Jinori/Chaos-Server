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
                        aisling.Counters.TryGetValue("StartedRatQuest", out var value);

                        if (!hasStage || (stage != RionaRatQuestStage.StartedRatQuest))
                            return;

                        if (value >= 5)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("StartedRatQuest", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }

                    case "crypt_bat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (!hasStage || (stage != CryptSlayerStage.Bat))
                            return;

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }

                    case "crypt_centipede":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Centipede1))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_centipede2":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Centipede2))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_giantbat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.GiantBat))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_kardi":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Kardi))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_marauder":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Marauder))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_mimic":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Mimic))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_rat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Rat))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_scorpion":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Scorpion))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_spider":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Spider1))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_spider2":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Spider2))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_succubus":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Succubus))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_white_bat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.WhiteBat))
                            return;

                        aisling.Counters.TryGetValue("CryptSlayer", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-1":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Lower))
                            return;

                        aisling.Counters.TryGetValue("PurpleHorse", out var value);

                        if (value >= 15)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("purplehorse", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-2":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Higher))
                            return;

                        aisling.Counters.TryGetValue("grayhorse", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("grayhorse", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-3":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Higher))
                            return;

                        aisling.Counters.TryGetValue("redhorse", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("redhorse", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1_boss":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.BossStarted))
                            return;

                        aisling.Counters.TryGetValue("AppleJack", out var value);

                        if (value >= 3)
                        {
                            aisling.SendOrangeBarMessage($"You've defeated {Subject.Template.Name} Return to Big Bunny.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("AppleJack", 1);
                        aisling.SendOrangeBarMessage($"You defeated Apple Jack {value} times!");

                        break;
                    }
                    case "bunny1-1":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Lower))
                            return;

                        aisling.Counters.TryGetValue("whitebunny", out var value);

                        if (value >= 15)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("whitebunny", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1-2":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Higher))
                            return;

                        aisling.Counters.TryGetValue("brownbunny", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("brownbunny", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1-3":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Higher))
                            return;

                        aisling.Counters.TryGetValue("purplebunny", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("purplebunny", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1_boss":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.BossStarted))
                            return;

                        aisling.Counters.TryGetValue("MrHopps", out var value);

                        if (value >= 3)
                        {
                            aisling.SendOrangeBarMessage($"You've already defeated {Subject.Template.Name}. Return to the Horse Leader.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("MrHopps", 1);
                        aisling.SendOrangeBarMessage($"You defeated Mr.Hopps ({value} times!");

                        break;
                    }
                    case "wolf1-1":
                    {
                        var hasFrog = aisling.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.Lower))
                            return;
                        
                        aisling.Counters.TryGetValue("mythicwolf", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("MythicWolf", 1);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wolf1-2":
                    {
                        var hasFrog = aisling.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.Higher))
                            return;

                        aisling.Counters.TryGetValue("WhiteWolf", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }
                        
                        aisling.Counters.AddOrIncrement("WhiteWolf", 1);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wolf1-3":
                    {
                        var hasFrog = aisling.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.Higher))
                            return;
                        aisling.Counters.TryGetValue("beardedwolf", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("BeardedWolf", 1);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "wolf1_boss":
                    {
                        var hasFrog = aisling.Enums.TryGetValue(out MythicFrog frog);

                        if (!hasFrog || (frog != MythicFrog.BossStarted))
                            return;
                        
                        aisling.Counters.TryGetValue("nymeria", out var value);

                        if (value >= 3)
                        {
                            aisling.SendOrangeBarMessage($"You've already defeated {Subject.Template.Name}. Return to the King Frog.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("Nymeria", 1);
                        aisling.SendOrangeBarMessage($"You defeated Nymeria ({value} times!");

                        break;
                    }
                    case "mantis1-1":
                    {
                        var hasBee = aisling.Enums.TryGetValue(out MythicBee bee);

                        if (!hasBee || (bee != MythicBee.Lower))
                            return;

                        aisling.Counters.TryGetValue("MythicMantis", out var value);

                        if (value >= 15)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("MythicMantis", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "mantis1-2":
                    {
                        var hasBee = aisling.Enums.TryGetValue(out MythicBee bee);

                        if (!hasBee || (bee != MythicBee.Higher))
                            return;

                        aisling.Counters.TryGetValue("BrownMantis", out var value);

                        if (value >= 20)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("BrownMantis", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "mantis1_boss":
                    {
                        var hasBee = aisling.Enums.TryGetValue(out MythicBee bee);

                        if (!hasBee || (bee != MythicBee.BossStarted))
                            return;
                        
                        aisling.Counters.TryGetValue("firetree", out var value);

                        if (value >= 3)
                        {
                            aisling.SendOrangeBarMessage($"You've already defeated {Subject.Template.Name}. Return to the Queen Bee.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("firetree", 1);
                        aisling.SendOrangeBarMessage($"You defeated Fire Tree ({value} times!");

                        break;
                    }
                    case "bee1-1":
                    {
                        var hasMantis = aisling.Enums.TryGetValue(out MythicMantis mantis);

                        if (!hasMantis || (mantis != MythicMantis.Lower))
                            return;

                        aisling.Counters.TryGetValue("mythicbee", out var value);

                        if (value >= 15)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("mythicbee", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bee1-2":
                    {
                        var hasMantis = aisling.Enums.TryGetValue(out MythicMantis mantis);

                        if (!hasMantis || (mantis != MythicMantis.Higher))
                            return;

                        aisling.Counters.TryGetValue("greenbee", out var value);

                        if (value >= 20)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("greenbee", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bee1_boss":
                    {
                        var hasMantis = aisling.Enums.TryGetValue(out MythicMantis mantis);

                        if (!hasMantis || (mantis != MythicMantis.BossStarted))
                            return;
                        
                        aisling.Counters.TryGetValue("Carolina", out var value);

                        if (value >= 3)
                        {
                            aisling.SendOrangeBarMessage($"You've already defeated {Subject.Template.Name}. Return to the King Mantis.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("carolina", 1);
                        aisling.SendOrangeBarMessage($"You defeated Carolina ({value} times!");

                        break;
                    }
                    case "frog1-1":
                    {
                        var hasWolf = aisling.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.Lower))
                            return;

                        aisling.Counters.TryGetValue("mythicfrog", out var value);

                        if (value >= 15)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("mythicfrog", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "frog1-2":
                    {
                        var hasWolf = aisling.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.Higher))
                            return;

                        aisling.Counters.TryGetValue("bluefrog", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("bluefrog", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "frog1-3":
                    {
                        var hasWolf = aisling.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.Higher))
                            return;

                        aisling.Counters.TryGetValue("redfrog", out var value);

                        if (value >= 10)
                        {
                            aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("redfrog", 1);
                        aisling.SendOrangeBarMessage($"You've killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "frog1_boss":
                    {
                        var hasWolf = aisling.Enums.TryGetValue(out MythicWolf wolf);

                        if (!hasWolf || (wolf != MythicWolf.BossStarted))
                            return;
                        
                        aisling.Counters.TryGetValue("frogger", out var value);

                        if (value >= 3)
                        {
                            aisling.SendOrangeBarMessage($"You've already defeated {Subject.Template.Name}. Return to the Wolf Pack Leader.");

                            return;
                        }

                        aisling.Counters.AddOrIncrement("frogger", 1);
                        aisling.SendOrangeBarMessage($"You defeated Frogger ({value} times!");

                        break;
                    }
                    
                }
    }
}