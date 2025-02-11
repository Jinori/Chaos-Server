namespace Chaos.Definitions;

public sealed class BountyDetails
{
    public required Enum AvailableQuestFlag { get; init; }
    public required Enum BountyQuestFlag { get; init; }
    public int KillRequirement { get; init; }
    public required string MonsterTemplateKey { get; init; }
    public required string QuestText { get; init; }
}

public class BountyBoardQuests
{
    public static readonly List<BountyDetails> PossibleQuestDetails =
    [
        //EASY
        new()
        {
            MonsterTemplateKey = "ancientskeleton",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyAncientSkeleton,
            AvailableQuestFlag = AvailableQuestFlags1.EasyAncientSkeleton,
            QuestText = "Hunt 100 Ancient Skeletons ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "ancientbeetalic",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyAncientBeetalic,
            QuestText = "Hunt 100 Ancient Beetalics ({=qEasy{=h)",
            AvailableQuestFlag = AvailableQuestFlags1.EasyAncientBeetalic
        },
        new()
        {
            MonsterTemplateKey = "losgann",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyLosgann,
            AvailableQuestFlag = AvailableQuestFlags1.EasyLosgann,
            QuestText = "Hunt 100 Losganns ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "ruidhtear",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyRuidhtear,
            AvailableQuestFlag = AvailableQuestFlags1.EasyRuidhtear,
            QuestText = "Hunt 100 Ruidhtears ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "brownmantis",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyBrownMantis,
            AvailableQuestFlag = AvailableQuestFlags1.EasyBrownMantis,
            QuestText = "Hunt 100 Brown Mantises ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "goldbeetalic",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyGoldBeetalic,
            AvailableQuestFlag = AvailableQuestFlags1.EasyGoldBeetalic,
            QuestText = "Hunt 100 Gold Beetalics ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "redshocker",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyRedShocker,
            AvailableQuestFlag = AvailableQuestFlags1.EasyRedShocker,
            QuestText = "Hunt 100 Red Shockers ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "blackshocker",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyBlackShocker,
            AvailableQuestFlag = AvailableQuestFlags1.EasyBlackShocker,
            QuestText = "Hunt 100 Black Shockers ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "goldshocker",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyGoldShocker,
            AvailableQuestFlag = AvailableQuestFlags1.EasyGoldShocker,
            QuestText = "Hunt 100 Gold Shockers ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "blueshocker",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyBlueShocker,
            AvailableQuestFlag = AvailableQuestFlags1.EasyBlueShocker,
            QuestText = "Hunt 100 Blue Shockers ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "direwolf",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyDireWolf,
            AvailableQuestFlag = AvailableQuestFlags1.EasyDireWolf,
            QuestText = "Hunt 100 Dire Wolves ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "iceelemental",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyIceElemental,
            AvailableQuestFlag = AvailableQuestFlags1.EasyIceElemental,
            QuestText = "Hunt 100 Ice Elementals ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "iceskeleton",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyIceSkeleton,
            AvailableQuestFlag = AvailableQuestFlags1.EasyIceSkeleton,
            QuestText = "Hunt 100 Ice Skeletons ({=qEasy{=h)"
        },
        new()
        {
            MonsterTemplateKey = "icespore",
            KillRequirement = 100,
            BountyQuestFlag = BountyQuestFlags1.EasyIceSpore,
            AvailableQuestFlag = AvailableQuestFlags1.EasyIceSpore,
            QuestText = "Hunt 100 Ice Spore ({=qEasy{=h)"
        },

        //MEDIUM
        new()
        {
            MonsterTemplateKey = "ancientskeleton",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumAncientSkeleton,
            AvailableQuestFlag = AvailableQuestFlags1.MediumAncientSkeleton,
            QuestText = "Hunt 250 Ancient Skeletons ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "ancientbeetalic",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumAncientBeetalic,
            AvailableQuestFlag = AvailableQuestFlags1.MediumAncientBeetalic,
            QuestText = "Hunt 250 Ancient Beetalics ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "losgann",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumLosgann,
            AvailableQuestFlag = AvailableQuestFlags1.MediumLosgann,
            QuestText = "Hunt 250 Losganns ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "ruidhtear",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumRuidhtear,
            AvailableQuestFlag = AvailableQuestFlags1.MediumRuidhtear,
            QuestText = "Hunt 250 Ruidhtears ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "brownmantis",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumBrownMantis,
            AvailableQuestFlag = AvailableQuestFlags1.MediumBrownMantis,
            QuestText = "Hunt 250 Brown Mantises ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "goldbeetalic",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumGoldBeetalic,
            AvailableQuestFlag = AvailableQuestFlags1.MediumGoldBeetalic,
            QuestText = "Hunt 250 Gold Beetalics ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "redshocker",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumRedShocker,
            AvailableQuestFlag = AvailableQuestFlags1.MediumRedShocker,
            QuestText = "Hunt 250 Red Shockers ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "blackshocker",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumBlackShocker,
            AvailableQuestFlag = AvailableQuestFlags1.MediumBlackShocker,
            QuestText = "Hunt 250 Black Shockers ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "goldshocker",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumGoldShocker,
            AvailableQuestFlag = AvailableQuestFlags1.MediumGoldShocker,
            QuestText = "Hunt 250 Gold Shockers ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "blueshocker",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumBlueShocker,
            AvailableQuestFlag = AvailableQuestFlags1.MediumBlueShocker,
            QuestText = "Hunt 250 Blue Shockers ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "direwolf",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumDireWolf,
            AvailableQuestFlag = AvailableQuestFlags1.MediumDireWolf,
            QuestText = "Hunt 250 Dire Wolves ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "iceelemental",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumIceElemental,
            AvailableQuestFlag = AvailableQuestFlags1.MediumIceElemental,
            QuestText = "Hunt 250 Ice Elementals ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "iceskeleton",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumIceSkeleton,
            AvailableQuestFlag = AvailableQuestFlags1.MediumIceSkeleton,
            QuestText = "Hunt 250 Ice Skeletons ({=cMedium{=h)"
        },
        new()
        {
            MonsterTemplateKey = "icespore",
            KillRequirement = 250,
            BountyQuestFlag = BountyQuestFlags1.MediumIceSpore,
            AvailableQuestFlag = AvailableQuestFlags1.MediumIceSpore,
            QuestText = "Hunt 250 Ice Spores ({=cMedium{=h)"
        },

        //HARD
        new()
        {
            MonsterTemplateKey = "ancientskeleton",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardAncientSkeleton,
            AvailableQuestFlag = AvailableQuestFlags1.HardAncientSkeleton,
            QuestText = "Hunt 400 Ancient Skeletons ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "ancientbeetalic",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardAncientBeetalic,
            AvailableQuestFlag = AvailableQuestFlags1.HardAncientBeetalic,
            QuestText = "Hunt 400 Ancient Beetalics ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "losgann",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardLosgann,
            AvailableQuestFlag = AvailableQuestFlags1.HardLosgann,
            QuestText = "Hunt 400 Losganns ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "ruidhtear",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardRuidhtear,
            AvailableQuestFlag = AvailableQuestFlags1.HardRuidhtear,
            QuestText = "Hunt 400 Ruidhtears ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "brownmantis",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardBrownMantis,
            AvailableQuestFlag = AvailableQuestFlags1.HardBrownMantis,
            QuestText = "Hunt 400 Brown Mantises ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "goldbeetalic",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardGoldBeetalic,
            AvailableQuestFlag = AvailableQuestFlags1.HardGoldBeetalic,
            QuestText = "Hunt 400 Gold Beetalics ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "redshocker",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardRedShocker,
            AvailableQuestFlag = AvailableQuestFlags1.HardRedShocker,
            QuestText = "Hunt 400 Red Shockers ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "blackshocker",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardBlackShocker,
            AvailableQuestFlag = AvailableQuestFlags1.HardBlackShocker,
            QuestText = "Hunt 400 Black Shockers ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "goldshocker",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardGoldShocker,
            AvailableQuestFlag = AvailableQuestFlags1.HardGoldShocker,
            QuestText = "Hunt 400 Gold Shockers ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "blueshocker",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardBlueShocker,
            AvailableQuestFlag = AvailableQuestFlags1.HardBlueShocker,
            QuestText = "Hunt 400 Blue Shockers ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "direwolf",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardDireWolf,
            AvailableQuestFlag = AvailableQuestFlags1.HardDireWolf,
            QuestText = "Hunt 400 Dire Wolves ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "iceelemental",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardIceElemental,
            AvailableQuestFlag = AvailableQuestFlags1.HardIceElemental,
            QuestText = "Hunt 400 Ice Elementals ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "iceskeleton",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardIceSkeleton,
            AvailableQuestFlag = AvailableQuestFlags1.HardIceSkeleton,
            QuestText = "Hunt 400 Ice Skeletons ({=bHard{=h)"
        },
        new()
        {
            MonsterTemplateKey = "icespore",
            KillRequirement = 400,
            BountyQuestFlag = BountyQuestFlags1.HardIceSpore,
            AvailableQuestFlag = AvailableQuestFlags1.HardIceSpore,
            QuestText = "Hunt 400 Ice Spores ({=bHard{=h)"
        },

        // Epic Tier
        new()
        {
            MonsterTemplateKey = "kelberoth",
            KillRequirement = 10,
            BountyQuestFlag = BountyQuestFlags1.EpicKelberoth,
            AvailableQuestFlag = AvailableQuestFlags1.EpicKelberoth,
            QuestText = "Hunt 10 Kelberoths ({=pEpic{=h)"
        },
        new()
        {
            MonsterTemplateKey = "ancientdraco",
            KillRequirement = 10,
            BountyQuestFlag = BountyQuestFlags1.EpicAncientDraco,
            AvailableQuestFlag = AvailableQuestFlags1.EpicAncientDraco,
            QuestText = "Hunt 10 Ancient Dracos ({=pEpic{=h)"
        },
        new()
        {
            MonsterTemplateKey = "greenmantis",
            KillRequirement = 10,
            BountyQuestFlag = BountyQuestFlags1.EpicGreenMantis,
            AvailableQuestFlag = AvailableQuestFlags1.EpicGreenMantis,
            QuestText = "Hunt 10 Green Mantises ({=pEpic{=h)"
        },
        new()
        {
            MonsterTemplateKey = "hydra",
            KillRequirement = 10,
            BountyQuestFlag = BountyQuestFlags1.EpicHydra,
            AvailableQuestFlag = AvailableQuestFlags1.EpicHydra,
            QuestText = "Hunt 10 Hydras ({=pEpic{=h)"
        }
    ];

    //new() { MonsterTemplateKey = , KillRequirement = , BountyQuestFlag =  , QuestText = },
}