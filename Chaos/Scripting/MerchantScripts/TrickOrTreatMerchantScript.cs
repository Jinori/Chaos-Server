using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class TrickOrTreatMerchantScript : MerchantScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly Dictionary<int, List<string>> LevelBasedMonsters;
    private readonly IMonsterFactory MonsterFactory;
    private readonly List<string> RandomMaps;
    private readonly ISimpleCache SimpleCache;
    private readonly Dictionary<string, int> TreatItems; // Dictionary of treat items with weights
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public TrickOrTreatMerchantScript(
        Merchant subject,
        IItemFactory itemFactory,
        ISimpleCache simpleCache,
        IMonsterFactory monsterFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        SimpleCache = simpleCache;
        MonsterFactory = monsterFactory;

        // Dictionary of monsters based on level ranges
        LevelBasedMonsters = new Dictionary<int, List<string>>
        {
            {
                1, new List<string>
                {
                    "crypt_spider3",
                    "wilderness_chicken1",
                    "bunny1-1"
                }
            },
            {
                11, new List<string>
                {
                    "ebee1_boss",
                    "eWilderness_cat",
                    "ecrypt_boss5",
                    "ebunny1_boss",
                    "ewilderness_abomination"
                }
            },
            {
                41, new List<string>
                {
                    "ekarlopos_crab",
                    "efrog1_boss",
                    "ebunny2_boss",
                    "ekarlopos_gog",
                    "eSewer_skrull5",
                    "esewer_skrull10",
                    "ecrypt_boss10"
                }
            },
            {
                71, new List<string>
                {
                    "ead_boss10",
                    "ead_dwarf10",
                    "ead_polyp9",
                    "emantis2_boss",
                    "ewolf2_boss"
                }
            },
            {
                99, new List<string>
                {
                    "ezombie2_boss",
                    "esoul_collector",
                    "emummy10",
                    "eflesh_golem10",
                    "emehadi_frog",
                    "emehadi_shrieker",
                    "echandi_mukul",
                    "eundine_field_brute"
                }
            },
            {
                100, new List<string>
                {
                    "eshinewood_brownmantis",
                    "enm_blackshocker",
                    "eor_hydra",
                    "eor_losgann",
                    "emg_kelberoth",
                    "emg_direwolf",
                    "ekm_draco2",
                    "ekm_orcwarlord",
                    "ecd_darkmonk",
                    "ecd_darkpriest",
                    "ecd_darkrogue",
                    "ecd_darkwarrior",
                    "edarkmasterpam",
                    "edarkmasterphil",
                    "edarkmastermary",
                    "edarkmastermike",
                    "edarkmasterwanda",
                    "edarkmasterroy",
                    "edarkmasterray",
                    "edarkmasterjane",
                    "edarkmasterjohn",
                    "ecthonic_drake",
                    "ecthonic_hydra",
                    "edc_gargoylelord",
                    "edc_basement_gargoyleboss",
                    "eundead_king"
                }
            }
        };

        // List of random maps for teleporting players
        RandomMaps = new List<string>
        {
            "Mileth",
            "Abel",
            "Rucesion",
            "Piet",
            "Mythic",
            "arena_entrance",
            "Nobis",
            "Undine",
            "Suomi",
            "pf_entrance",
            "tagor"
        };

        // Dictionary of treat items with assigned weights
        TreatItems = new Dictionary<string, int>
        {
            {
                "wine", 30
            },
            {
                "rum", 30
            },
            {
                "strongjuggernautbrew", 10
            },
            {
                "strongastralbrew", 10
            },
            {
                "EvilHorns", 8
            },
            {
                "radiantpearl", 1
            },
            {
                "eclipsepearl", 1
            },
            {
                "strangestone", 2
            },
            {
                "macabrebox", 25
            }, // Common item
            {
                "strongknowledgeelixir", 25
            }, // Less common item
            {
                "batwings", 8
            } // Rare item
        };
    }

    private List<string> GetMonsterListForLevel(int level, Aisling source)
    {
        switch (level)
        {
            case <= 10:
                return LevelBasedMonsters[1];
            case <= 40:
                return LevelBasedMonsters[11];
            case <= 70:
                return LevelBasedMonsters[41];
            case <= 98:
                return LevelBasedMonsters[71];
            case 99 when !source.UserStatSheet.Master:
                return LevelBasedMonsters[99];
        }

        if (source.UserStatSheet.Master)
            return LevelBasedMonsters[100];

        throw new InvalidOperationException("Unknown level");
    }

    public override void OnPublicMessage(Creature source, string message)
    {
        if (!message.EqualsI("Trick or Treat!"))
            return;

        if (source is not Aisling aisling)
            return;

        if (aisling.Trackers.TimedEvents.HasActiveEvent($"{Subject.Template.Name}", out _))
        {
            Subject.Say("You've already been here! Try somewhere else.");

            return;
        }

        aisling.Trackers.TimedEvents.AddEvent($"{Subject.Template.Name}", TimeSpan.FromHours(3), true);

        // 50% chance for treat (item or experience) or trick (teleport)
        if (IntegerRandomizer.RollChance(70))
        {
            Subject.Say("You got a treat!");

            var randomNumber = IntegerRandomizer.RollSingle(101);

            // Randomly decide between an item reward or experience reward
            if (randomNumber < 80)
            {
                // Give random item from TreatItems dictionary using weights
                var randomItem = TreatItems.PickRandomWeighted();
                var item = ItemFactory.Create(randomItem);
                aisling.GiveItemOrSendToBank(item);
                aisling.SendOrangeBarMessage($"You received a {item.DisplayName} from {Subject.Name}!");

                return;
            }

            if (randomNumber < 101)
            {
                if (aisling.StatSheet.Level < 99)
                {
                    var tnl = LevelUpFormulae.Default.CalculateTnl(aisling);
                    var twentyPercent = MathEx.GetPercentOf<int>(tnl, 20);
                    ExperienceDistributionScript.GiveExp(aisling, twentyPercent);
                    aisling.SendOrangeBarMessage($"{Subject.Name} grants you experience!");

                    return;
                }

                if (aisling.StatSheet.Level == 99)
                {
                    ExperienceDistributionScript.GiveExp(aisling, 10000000);
                    aisling.SendOrangeBarMessage($"{Subject.Name} grants you experience!");
                }
            }
        } else
        {
            Subject.Say("Ha! Trick!");

            var randomRoll = IntegerRandomizer.RollSingle(101);

            if (randomRoll < 50)
            {
                var randomMapName = RandomMaps.PickRandom();
                var mapInstance = SimpleCache.Get<MapInstance>(randomMapName);

                // Try to get a random walkable point on the map for teleportation
                if (mapInstance.TryGetRandomWalkablePoint(out var mapPoint, CreatureType.Aisling))
                {
                    aisling.TraverseMap(mapInstance, mapPoint.Value);
                    aisling.SendOrangeBarMessage("That was a trick! Oh no.");

                    return;
                }

                aisling.SendOrangeBarMessage("Couldn't find a safe place to teleport. Try again later!");

                return;
            }

            var randomMonsterList = GetMonsterListForLevel(aisling.StatSheet.Level, aisling);
            var randomMonster = randomMonsterList.PickRandom();

            var rectangle = new Rectangle(
                aisling.X,
                aisling.Y,
                2,
                2);

            var randomPoint = rectangle.GetRandomPoint();

            var monster = MonsterFactory.Create(randomMonster, Subject.MapInstance, randomPoint);
            Subject.MapInstance.AddEntity(monster, randomPoint);
            aisling.SendOrangeBarMessage("You got a trick! Watch out!");
        }
    }
}