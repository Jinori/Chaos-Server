using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Objects.World;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

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

                        aisling.Counters.AddOrIncrement("StartedRatQuest", 1);
                        aisling.Counters.TryGetValue("StartedRatQuest", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }

                    case "crypt_bat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Bat))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }

                    case "crypt_centipede":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Centipede1))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_centipede2":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Centipede2))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_giantbat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.GiantBat))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_kardi":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Kardi))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_marauder":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Marauder))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_mimic":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Mimic))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_rat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Rat))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_scorpion":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Scorpion))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_spider":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Spider1))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_spider2":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Spider2))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_succubus":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.Succubus))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "crypt_white_bat":
                    {
                        var hasStage = aisling.Enums.TryGetValue(out CryptSlayerStage stage);

                        if (!hasStage || (stage != CryptSlayerStage.WhiteBat))
                            return;

                        aisling.Counters.AddOrIncrement("CryptSlayer", 1);
                        aisling.Counters.TryGetValue("CryptSlayer", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-1":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Lower))
                            return;

                        aisling.Counters.AddOrIncrement("BunnyLower", 1);
                        aisling.Counters.TryGetValue("BunnyLower", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1-2":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.Higher))
                            return;

                        aisling.Counters.AddOrIncrement("BunnyHigher", 1);
                        aisling.Counters.TryGetValue("BunnyHigher", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "horse1_boss":
                    {
                        var hasBunny = aisling.Enums.TryGetValue(out MythicBunny bunny);

                        if (!hasBunny || (bunny != MythicBunny.BossStarted))
                            return;

                        aisling.Counters.AddOrIncrement("AppleJack", 1);
                        aisling.Counters.TryGetValue("AppleJack", out var value);
                        aisling.SendOrangeBarMessage($"You defeated Apple Jack {value} times!");

                        break;
                    }
                    case "bunny1-1":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Lower))
                            return;

                        aisling.Counters.AddOrIncrement("HorseLower", 1);
                        aisling.Counters.TryGetValue("HorseLower", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1-2":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.Higher))
                            return;

                        aisling.Counters.AddOrIncrement("HorseHigher", 1);
                        aisling.Counters.TryGetValue("HorseHigher", out var value);
                        aisling.SendOrangeBarMessage($"Killed {value} {Subject.Template.Name}.");

                        break;
                    }
                    case "bunny1_boss":
                    {
                        var hasHorse = aisling.Enums.TryGetValue(out MythicHorse horse);

                        if (!hasHorse || (horse != MythicHorse.BossStarted))
                            return;

                        aisling.Counters.AddOrIncrement("MrHopps", 1);
                        aisling.Counters.TryGetValue("MrHopps", out var value);
                        aisling.SendOrangeBarMessage($"You defeated Mr.Hopps ({value} times!");

                        break;
                    }
                }
    }
}