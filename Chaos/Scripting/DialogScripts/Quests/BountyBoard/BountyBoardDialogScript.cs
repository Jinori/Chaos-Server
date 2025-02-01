using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Quests.BountyBoard;

public class BountyBoardDialogScript(Dialog subject, ILogger<BountyBoardDialogScript> logger) : DialogScriptBase(subject)
{
    private static readonly Random Random = new();

    // Dictionary of bounty options
    private static readonly
        Dictionary<string, (string MonsterKey, int KillRequirement, BountyBoardKill1 KillEnum, BountyBoardFlags DifficultyFlag,
            BountyBoardOptions BountyOption )> BountyOptions1 = new()
        {
            // Easy Tier
            {
                "Hunt 100 Ancient Skeletons (Easy)",
                ("ancientskeleton", 100, BountyBoardKill1.AncientSkeleton, BountyBoardFlags.Easy1, BountyBoardOptions.EasyAncientSkeleton)
            },
            {
                "Hunt 100 Ancient Beetalics (Easy)",
                ("ancientbeetalic", 100, BountyBoardKill1.AncientBeetalic, BountyBoardFlags.Easy1, BountyBoardOptions.EasyAncientBeetalic)
            },
            {
                "Hunt 100 Losganns (Easy)",
                ("losgann", 100, BountyBoardKill1.Losgann, BountyBoardFlags.Easy1, BountyBoardOptions.EasyLosgann)
            },
            {
                "Hunt 100 Ruidhtears (Easy)",
                ("ruidhtear", 100, BountyBoardKill1.Ruidhtear, BountyBoardFlags.Easy1, BountyBoardOptions.EasyRuidhtear)
            },
            {
                "Hunt 100 Brown Mantises (Easy)",
                ("brownmantis", 100, BountyBoardKill1.BrownMantis, BountyBoardFlags.Easy1, BountyBoardOptions.EasyBrownMantis)
            },
            {
                "Hunt 100 Gold Beetalics (Easy)",
                ("goldbeetalic", 100, BountyBoardKill1.GoldBeetalic, BountyBoardFlags.Easy1, BountyBoardOptions.EasyGoldBeetalic)
            },
            {
                "Hunt 100 Red Shockers (Easy)",
                ("redshocker", 100, BountyBoardKill1.RedShocker, BountyBoardFlags.Easy1, BountyBoardOptions.EasyRedShocker)
            },
            {
                "Hunt 100 Black Shockers (Easy)",
                ("blackshocker", 100, BountyBoardKill1.BlackShocker, BountyBoardFlags.Easy1, BountyBoardOptions.EasyBlackShocker)
            },
            {
                "Hunt 100 Gold Shockers (Easy)",
                ("goldshocker", 100, BountyBoardKill1.GoldShocker, BountyBoardFlags.Easy1, BountyBoardOptions.EasyGoldShocker)
            },
            {
                "Hunt 100 Blue Shockers (Easy)",
                ("blueshocker", 100, BountyBoardKill1.BlueShocker, BountyBoardFlags.Easy1, BountyBoardOptions.EasyBlueShocker)
            },
            {
                "Hunt 100 Dire Wolves (Easy)",
                ("direwolf", 100, BountyBoardKill1.DireWolf, BountyBoardFlags.Easy1, BountyBoardOptions.EasyDireWolf)
            },
            {
                "Hunt 100 Ice Elementals (Easy)",
                ("iceelemental", 100, BountyBoardKill1.IceElemental, BountyBoardFlags.Easy1, BountyBoardOptions.EasyIceElemental)
            },
            {
                "Hunt 100 Ice Skeletons (Easy)",
                ("iceskeleton", 100, BountyBoardKill1.IceSkeleton, BountyBoardFlags.Easy1, BountyBoardOptions.EasyIceSkeleton)
            },
            {
                "Hunt 100 Ice Spores (Easy)",
                ("icespore", 100, BountyBoardKill1.IceSpore, BountyBoardFlags.Easy1, BountyBoardOptions.EasyIceSpore)
            },

            // Medium Tier
            {
                "Hunt 500 Ancient Skeletons (Medium)",
                ("ancientskeleton", 500, BountyBoardKill1.AncientSkeleton, BountyBoardFlags.Medium1,
                    BountyBoardOptions.MediumAncientSkeleton)
            },
            {
                "Hunt 500 Ancient Beetalics (Medium)",
                ("ancientbeetalic", 500, BountyBoardKill1.AncientBeetalic, BountyBoardFlags.Medium1,
                    BountyBoardOptions.MediumAncientBeetalic)
            },
            {
                "Hunt 500 Losganns (Medium)",
                ("losgann", 500, BountyBoardKill1.Losgann, BountyBoardFlags.Medium1, BountyBoardOptions.MediumLosgann)
            },
            {
                "Hunt 500 Ruidhtears (Medium)",
                ("ruidhtear", 500, BountyBoardKill1.Ruidhtear, BountyBoardFlags.Medium1, BountyBoardOptions.MediumRuidhtear)
            },
            {
                "Hunt 500 Brown Mantises (Medium)",
                ("brownmantis", 500, BountyBoardKill1.BrownMantis, BountyBoardFlags.Medium1, BountyBoardOptions.MediumBrownMantis)
            },
            {
                "Hunt 500 Gold Beetalics (Medium)",
                ("goldbeetalic", 500, BountyBoardKill1.GoldBeetalic, BountyBoardFlags.Medium1, BountyBoardOptions.MediumGoldBeetalic)
            },
            {
                "Hunt 500 Red Shockers (Medium)",
                ("redshocker", 500, BountyBoardKill1.RedShocker, BountyBoardFlags.Medium1, BountyBoardOptions.MediumRedShocker)
            },
            {
                "Hunt 500 Black Shockers (Medium)",
                ("blackshocker", 500, BountyBoardKill1.BlackShocker, BountyBoardFlags.Medium1, BountyBoardOptions.MediumBlackShocker)
            },
            {
                "Hunt 500 Gold Shockers (Medium)",
                ("goldshocker", 500, BountyBoardKill1.GoldShocker, BountyBoardFlags.Medium1, BountyBoardOptions.MediumGoldShocker)
            },
            {
                "Hunt 500 Blue Shockers (Medium)",
                ("blueshocker", 500, BountyBoardKill1.BlueShocker, BountyBoardFlags.Medium1, BountyBoardOptions.MediumBlueShocker)
            },
            {
                "Hunt 500 Dire Wolves (Medium)",
                ("direwolf", 500, BountyBoardKill1.DireWolf, BountyBoardFlags.Medium1, BountyBoardOptions.MediumDireWolf)
            },
            {
                "Hunt 500 Ice Elementals (Medium)",
                ("iceelemental", 500, BountyBoardKill1.IceElemental, BountyBoardFlags.Medium1, BountyBoardOptions.MediumIceElemental)
            },
            {
                "Hunt 500 Ice Skeletons (Medium)",
                ("iceskeleton", 500, BountyBoardKill1.IceSkeleton, BountyBoardFlags.Medium1, BountyBoardOptions.MediumIceSkeleton)
            },
            {
                "Hunt 500 Ice Spores (Medium)",
                ("icespore", 500, BountyBoardKill1.IceSpore, BountyBoardFlags.Medium1, BountyBoardOptions.MediumIceSpore)
            },

            // Hard Tier
            {
                "Hunt 1000 Ancient Skeletons (Hard)",
                ("ancientskeleton", 1000, BountyBoardKill1.AncientSkeleton, BountyBoardFlags.Hard1, BountyBoardOptions.HardAncientSkeleton)
            },
            {
                "Hunt 1000 Ancient Beetalics (Hard)",
                ("ancientbeetalic", 1000, BountyBoardKill1.AncientBeetalic, BountyBoardFlags.Hard1, BountyBoardOptions.HardAncientBeetalic)
            },
            {
                "Hunt 1000 Losganns (Hard)",
                ("losgann", 1000, BountyBoardKill1.Losgann, BountyBoardFlags.Hard1, BountyBoardOptions.HardLosgann)
            },
            {
                "Hunt 1000 Ruidhtears (Hard)",
                ("ruidhtear", 1000, BountyBoardKill1.Ruidhtear, BountyBoardFlags.Hard1, BountyBoardOptions.HardRuidhtear)
            },
            {
                "Hunt 1000 Brown Mantises (Hard)",
                ("brownmantis", 1000, BountyBoardKill1.BrownMantis, BountyBoardFlags.Hard1, BountyBoardOptions.HardBrownMantis)
            },
            {
                "Hunt 1000 Gold Beetalics (Hard)",
                ("goldbeetalic", 1000, BountyBoardKill1.GoldBeetalic, BountyBoardFlags.Hard1, BountyBoardOptions.HardGoldBeetalic)
            },
            {
                "Hunt 1000 Red Shockers (Hard)",
                ("redshocker", 1000, BountyBoardKill1.RedShocker, BountyBoardFlags.Hard1, BountyBoardOptions.HardRedShocker)
            },
            {
                "Hunt 1000 Black Shockers (Hard)",
                ("blackshocker", 1000, BountyBoardKill1.BlackShocker, BountyBoardFlags.Hard1, BountyBoardOptions.HardBlackShocker)
            },
            {
                "Hunt 1000 Gold Shockers (Hard)",
                ("goldshocker", 1000, BountyBoardKill1.GoldShocker, BountyBoardFlags.Hard1, BountyBoardOptions.HardGoldShocker)
            },
            {
                "Hunt 1000 Blue Shockers (Hard)",
                ("blueshocker", 1000, BountyBoardKill1.BlueShocker, BountyBoardFlags.Hard1, BountyBoardOptions.HardBlueShocker)
            },
            {
                "Hunt 1000 Dire Wolves (Hard)",
                ("direwolf", 1000, BountyBoardKill1.DireWolf, BountyBoardFlags.Hard1, BountyBoardOptions.HardDireWolf)
            },
            {
                "Hunt 1000 Ice Elementals (Hard)",
                ("iceelemental", 1000, BountyBoardKill1.IceElemental, BountyBoardFlags.Hard1, BountyBoardOptions.HardIceElemental)
            },
            {
                "Hunt 1000 Ice Skeletons (Hard)",
                ("iceskeleton", 1000, BountyBoardKill1.IceSkeleton, BountyBoardFlags.Hard1, BountyBoardOptions.HardIceSkeleton)
            },
            {
                "Hunt 1000 Ice Spores (Hard)",
                ("icespore", 1000, BountyBoardKill1.IceSpore, BountyBoardFlags.Hard1, BountyBoardOptions.HardIceSpore)
            },

            // Epic Tier
            {
                "Hunt 25 Kelberoths (Epic)",
                ("kelberoth", 25, BountyBoardKill1.Kelberoth, BountyBoardFlags.Epic1, BountyBoardOptions.EpicKelberoth)
            },
            {
                "Hunt 25 Ancient Dracos (Epic)",
                ("ancientdraco", 25, BountyBoardKill1.AncientDraco, BountyBoardFlags.Epic1, BountyBoardOptions.EpicAncientDraco)
            },
            {
                "Hunt 25 Green Mantises (Epic)",
                ("greenmantis", 25, BountyBoardKill1.GreenMantis, BountyBoardFlags.Epic1, BountyBoardOptions.EpicGreenMantis)
            },
            {
                "Hunt 25 Ancient Hydra (Epic)",
                ("ancienthydra", 25, BountyBoardKill1.AncientHydra, BountyBoardFlags.Epic1, BountyBoardOptions.EpicAncientHydra)
            }
        };

    private static readonly
        Dictionary<string, (string MonsterKey, int KillRequirement, BountyBoardKill2 KillEnum, BountyBoardFlags DifficultyFlag,
            BountyBoardOptions BountyOption )> BountyOptions2 = new()
        {
            // Easy Tier
            {
                "Hunt 100 Ancient Skeletons (Easy)",
                ("ancientskeleton", 100, BountyBoardKill2.AncientSkeleton, BountyBoardFlags.Easy2, BountyBoardOptions.EasyAncientSkeleton)
            },
            {
                "Hunt 100 Ancient Beetalics (Easy)",
                ("ancientbeetalic", 100, BountyBoardKill2.AncientBeetalic, BountyBoardFlags.Easy2, BountyBoardOptions.EasyAncientBeetalic)
            },
            {
                "Hunt 100 Losganns (Easy)",
                ("losgann", 100, BountyBoardKill2.Losgann, BountyBoardFlags.Easy2, BountyBoardOptions.EasyLosgann)
            },
            {
                "Hunt 100 Ruidhtears (Easy)",
                ("ruidhtear", 100, BountyBoardKill2.Ruidhtear, BountyBoardFlags.Easy2, BountyBoardOptions.EasyRuidhtear)
            },
            {
                "Hunt 100 Brown Mantises (Easy)",
                ("brownmantis", 100, BountyBoardKill2.BrownMantis, BountyBoardFlags.Easy2, BountyBoardOptions.EasyBrownMantis)
            },
            {
                "Hunt 100 Gold Beetalics (Easy)",
                ("goldbeetalic", 100, BountyBoardKill2.GoldBeetalic, BountyBoardFlags.Easy2, BountyBoardOptions.EasyGoldBeetalic)
            },
            {
                "Hunt 100 Red Shockers (Easy)",
                ("redshocker", 100, BountyBoardKill2.RedShocker, BountyBoardFlags.Easy2, BountyBoardOptions.EasyRedShocker)
            },
            {
                "Hunt 100 Black Shockers (Easy)",
                ("blackshocker", 100, BountyBoardKill2.BlackShocker, BountyBoardFlags.Easy2, BountyBoardOptions.EasyBlackShocker)
            },
            {
                "Hunt 100 Gold Shockers (Easy)",
                ("goldshocker", 100, BountyBoardKill2.GoldShocker, BountyBoardFlags.Easy2, BountyBoardOptions.EasyGoldShocker)
            },
            {
                "Hunt 100 Blue Shockers (Easy)",
                ("blueshocker", 100, BountyBoardKill2.BlueShocker, BountyBoardFlags.Easy2, BountyBoardOptions.EasyBlueShocker)
            },
            {
                "Hunt 100 Dire Wolves (Easy)",
                ("direwolf", 100, BountyBoardKill2.DireWolf, BountyBoardFlags.Easy2, BountyBoardOptions.EasyDireWolf)
            },
            {
                "Hunt 100 Ice Elementals (Easy)",
                ("iceelemental", 100, BountyBoardKill2.IceElemental, BountyBoardFlags.Easy2, BountyBoardOptions.EasyIceElemental)
            },
            {
                "Hunt 100 Ice Skeletons (Easy)",
                ("iceskeleton", 100, BountyBoardKill2.IceSkeleton, BountyBoardFlags.Easy2, BountyBoardOptions.EasyIceSkeleton)
            },
            {
                "Hunt 100 Ice Spores (Easy)",
                ("icespore", 100, BountyBoardKill2.IceSpore, BountyBoardFlags.Easy2, BountyBoardOptions.EasyIceSpore)
            },

            // Medium Tier
            {
                "Hunt 500 Ancient Skeletons (Medium)",
                ("ancientskeleton", 500, BountyBoardKill2.AncientSkeleton, BountyBoardFlags.Medium2,
                    BountyBoardOptions.MediumAncientSkeleton)
            },
            {
                "Hunt 500 Ancient Beetalics (Medium)",
                ("ancientbeetalic", 500, BountyBoardKill2.AncientBeetalic, BountyBoardFlags.Medium2,
                    BountyBoardOptions.MediumAncientBeetalic)
            },
            {
                "Hunt 500 Losganns (Medium)",
                ("losgann", 500, BountyBoardKill2.Losgann, BountyBoardFlags.Medium2, BountyBoardOptions.MediumLosgann)
            },
            {
                "Hunt 500 Ruidhtears (Medium)",
                ("ruidhtear", 500, BountyBoardKill2.Ruidhtear, BountyBoardFlags.Medium2, BountyBoardOptions.MediumRuidhtear)
            },
            {
                "Hunt 500 Brown Mantises (Medium)",
                ("brownmantis", 500, BountyBoardKill2.BrownMantis, BountyBoardFlags.Medium2, BountyBoardOptions.MediumBrownMantis)
            },
            {
                "Hunt 500 Gold Beetalics (Medium)",
                ("goldbeetalic", 500, BountyBoardKill2.GoldBeetalic, BountyBoardFlags.Medium2, BountyBoardOptions.MediumGoldBeetalic)
            },
            {
                "Hunt 500 Red Shockers (Medium)",
                ("redshocker", 500, BountyBoardKill2.RedShocker, BountyBoardFlags.Medium2, BountyBoardOptions.MediumRedShocker)
            },
            {
                "Hunt 500 Black Shockers (Medium)",
                ("blackshocker", 500, BountyBoardKill2.BlackShocker, BountyBoardFlags.Medium2, BountyBoardOptions.MediumBlackShocker)
            },
            {
                "Hunt 500 Gold Shockers (Medium)",
                ("goldshocker", 500, BountyBoardKill2.GoldShocker, BountyBoardFlags.Medium2, BountyBoardOptions.MediumGoldShocker)
            },
            {
                "Hunt 500 Blue Shockers (Medium)",
                ("blueshocker", 500, BountyBoardKill2.BlueShocker, BountyBoardFlags.Medium2, BountyBoardOptions.MediumBlueShocker)
            },
            {
                "Hunt 500 Dire Wolves (Medium)",
                ("direwolf", 500, BountyBoardKill2.DireWolf, BountyBoardFlags.Medium2, BountyBoardOptions.MediumDireWolf)
            },
            {
                "Hunt 500 Ice Elementals (Medium)",
                ("iceelemental", 500, BountyBoardKill2.IceElemental, BountyBoardFlags.Medium2, BountyBoardOptions.MediumIceElemental)
            },
            {
                "Hunt 500 Ice Skeletons (Medium)",
                ("iceskeleton", 500, BountyBoardKill2.IceSkeleton, BountyBoardFlags.Medium2, BountyBoardOptions.MediumIceSkeleton)
            },
            {
                "Hunt 500 Ice Spores (Medium)",
                ("icespore", 500, BountyBoardKill2.IceSpore, BountyBoardFlags.Medium2, BountyBoardOptions.MediumIceSpore)
            },

            // Hard Tier
            {
                "Hunt 1000 Ancient Skeletons (Hard)",
                ("ancientskeleton", 1000, BountyBoardKill2.AncientSkeleton, BountyBoardFlags.Hard2, BountyBoardOptions.HardAncientSkeleton)
            },
            {
                "Hunt 1000 Ancient Beetalics (Hard)",
                ("ancientbeetalic", 1000, BountyBoardKill2.AncientBeetalic, BountyBoardFlags.Hard2, BountyBoardOptions.HardAncientBeetalic)
            },
            {
                "Hunt 1000 Losganns (Hard)",
                ("losgann", 1000, BountyBoardKill2.Losgann, BountyBoardFlags.Hard2, BountyBoardOptions.HardLosgann)
            },
            {
                "Hunt 1000 Ruidhtears (Hard)",
                ("ruidhtear", 1000, BountyBoardKill2.Ruidhtear, BountyBoardFlags.Hard2, BountyBoardOptions.HardRuidhtear)
            },
            {
                "Hunt 1000 Brown Mantises (Hard)",
                ("brownmantis", 1000, BountyBoardKill2.BrownMantis, BountyBoardFlags.Hard2, BountyBoardOptions.HardBrownMantis)
            },
            {
                "Hunt 1000 Gold Beetalics (Hard)",
                ("goldbeetalic", 1000, BountyBoardKill2.GoldBeetalic, BountyBoardFlags.Hard2, BountyBoardOptions.HardGoldBeetalic)
            },
            {
                "Hunt 1000 Red Shockers (Hard)",
                ("redshocker", 1000, BountyBoardKill2.RedShocker, BountyBoardFlags.Hard2, BountyBoardOptions.HardRedShocker)
            },
            {
                "Hunt 1000 Black Shockers (Hard)",
                ("blackshocker", 1000, BountyBoardKill2.BlackShocker, BountyBoardFlags.Hard2, BountyBoardOptions.HardBlackShocker)
            },
            {
                "Hunt 1000 Gold Shockers (Hard)",
                ("goldshocker", 1000, BountyBoardKill2.GoldShocker, BountyBoardFlags.Hard2, BountyBoardOptions.HardGoldShocker)
            },
            {
                "Hunt 1000 Blue Shockers (Hard)",
                ("blueshocker", 1000, BountyBoardKill2.BlueShocker, BountyBoardFlags.Hard2, BountyBoardOptions.HardBlueShocker)
            },
            {
                "Hunt 1000 Dire Wolves (Hard)",
                ("direwolf", 1000, BountyBoardKill2.DireWolf, BountyBoardFlags.Hard2, BountyBoardOptions.HardDireWolf)
            },
            {
                "Hunt 1000 Ice Elementals (Hard)",
                ("iceelemental", 1000, BountyBoardKill2.IceElemental, BountyBoardFlags.Hard2, BountyBoardOptions.HardIceElemental)
            },
            {
                "Hunt 1000 Ice Skeletons (Hard)",
                ("iceskeleton", 1000, BountyBoardKill2.IceSkeleton, BountyBoardFlags.Hard2, BountyBoardOptions.HardIceSkeleton)
            },
            {
                "Hunt 1000 Ice Spores (Hard)",
                ("icespore", 1000, BountyBoardKill2.IceSpore, BountyBoardFlags.Hard2, BountyBoardOptions.HardIceSpore)
            },

            // Epic Tier
            {
                "Hunt 25 Kelberoths (Epic)",
                ("kelberoth", 25, BountyBoardKill2.Kelberoth, BountyBoardFlags.Epic2, BountyBoardOptions.EpicKelberoth)
            },
            {
                "Hunt 25 Ancient Dracos (Epic)",
                ("ancientdraco", 25, BountyBoardKill2.AncientDraco, BountyBoardFlags.Epic2, BountyBoardOptions.EpicAncientDraco)
            },
            {
                "Hunt 25 Green Mantis (Epic)",
                ("greenmantis", 25, BountyBoardKill2.GreenMantis, BountyBoardFlags.Epic2, BountyBoardOptions.EpicGreenMantis)
            },
            {
                "Hunt 25 Ancient Hydra (Epic)",
                ("ancienthydra", 25, BountyBoardKill2.AncientHydra, BountyBoardFlags.Epic2, BountyBoardOptions.EpicAncientHydra)
            }
        };

    private static readonly
        Dictionary<string, (string MonsterKey, int KillRequirement, BountyBoardKill3 KillEnum, BountyBoardFlags DifficultyFlag,
            BountyBoardOptions BountyOption )> BountyOptions3 = new()
        {
            // Easy Tier
            {
                "Hunt 100 Ancient Skeletons (Easy)",
                ("ancientskeleton", 100, BountyBoardKill3.AncientSkeleton, BountyBoardFlags.Easy3, BountyBoardOptions.EasyAncientSkeleton)
            },
            {
                "Hunt 100 Ancient Beetalics (Easy)",
                ("ancientbeetalic", 100, BountyBoardKill3.AncientBeetalic, BountyBoardFlags.Easy3, BountyBoardOptions.EasyAncientBeetalic)
            },
            {
                "Hunt 100 Losganns (Easy)",
                ("losgann", 100, BountyBoardKill3.Losgann, BountyBoardFlags.Easy3, BountyBoardOptions.EasyLosgann)
            },
            {
                "Hunt 100 Ruidhtears (Easy)",
                ("ruidhtear", 100, BountyBoardKill3.Ruidhtear, BountyBoardFlags.Easy3, BountyBoardOptions.EasyRuidhtear)
            },
            {
                "Hunt 100 Brown Mantises (Easy)",
                ("brownmantis", 100, BountyBoardKill3.BrownMantis, BountyBoardFlags.Easy3, BountyBoardOptions.EasyBrownMantis)
            },
            {
                "Hunt 100 Gold Beetalics (Easy)",
                ("goldbeetalic", 100, BountyBoardKill3.GoldBeetalic, BountyBoardFlags.Easy3, BountyBoardOptions.EasyGoldBeetalic)
            },
            {
                "Hunt 100 Red Shockers (Easy)",
                ("redshocker", 100, BountyBoardKill3.RedShocker, BountyBoardFlags.Easy3, BountyBoardOptions.EasyRedShocker)
            },
            {
                "Hunt 100 Black Shockers (Easy)",
                ("blackshocker", 100, BountyBoardKill3.BlackShocker, BountyBoardFlags.Easy3, BountyBoardOptions.EasyBlackShocker)
            },
            {
                "Hunt 100 Gold Shockers (Easy)",
                ("goldshocker", 100, BountyBoardKill3.GoldShocker, BountyBoardFlags.Easy3, BountyBoardOptions.EasyGoldShocker)
            },
            {
                "Hunt 100 Blue Shockers (Easy)",
                ("blueshocker", 100, BountyBoardKill3.BlueShocker, BountyBoardFlags.Easy3, BountyBoardOptions.EasyBlueShocker)
            },
            {
                "Hunt 100 Dire Wolves (Easy)",
                ("direwolf", 100, BountyBoardKill3.DireWolf, BountyBoardFlags.Easy3, BountyBoardOptions.EasyDireWolf)
            },
            {
                "Hunt 100 Ice Elementals (Easy)",
                ("iceelemental", 100, BountyBoardKill3.IceElemental, BountyBoardFlags.Easy3, BountyBoardOptions.EasyIceElemental)
            },
            {
                "Hunt 100 Ice Skeletons (Easy)",
                ("iceskeleton", 100, BountyBoardKill3.IceSkeleton, BountyBoardFlags.Easy3, BountyBoardOptions.EasyIceSkeleton)
            },
            {
                "Hunt 100 Ice Spores (Easy)",
                ("icespore", 100, BountyBoardKill3.IceSpore, BountyBoardFlags.Easy3, BountyBoardOptions.EasyIceSpore)
            },

            //Medium Tier
            {
                "Hunt 500 Ancient Skeletons (Medium)",
                ("ancientskeleton", 500, BountyBoardKill3.AncientSkeleton, BountyBoardFlags.Medium3,
                    BountyBoardOptions.MediumAncientSkeleton)
            },
            {
                "Hunt 500 Ancient Beetalics (Medium)",
                ("ancientbeetalic", 500, BountyBoardKill3.AncientBeetalic, BountyBoardFlags.Medium3,
                    BountyBoardOptions.MediumAncientBeetalic)
            },
            {
                "Hunt 500 Losganns (Medium)",
                ("losgann", 500, BountyBoardKill3.Losgann, BountyBoardFlags.Medium3, BountyBoardOptions.MediumLosgann)
            },
            {
                "Hunt 500 Ruidhtears (Medium)",
                ("ruidhtear", 500, BountyBoardKill3.Ruidhtear, BountyBoardFlags.Medium3, BountyBoardOptions.MediumRuidhtear)
            },
            {
                "Hunt 500 Brown Mantises (Medium)",
                ("brownmantis", 500, BountyBoardKill3.BrownMantis, BountyBoardFlags.Medium3, BountyBoardOptions.MediumBrownMantis)
            },
            {
                "Hunt 500 Gold Beetalics (Medium)",
                ("goldbeetalic", 500, BountyBoardKill3.GoldBeetalic, BountyBoardFlags.Medium3, BountyBoardOptions.MediumGoldBeetalic)
            },
            {
                "Hunt 500 Red Shockers (Medium)",
                ("redshocker", 500, BountyBoardKill3.RedShocker, BountyBoardFlags.Medium3, BountyBoardOptions.MediumRedShocker)
            },
            {
                "Hunt 500 Black Shockers (Medium)",
                ("blackshocker", 500, BountyBoardKill3.BlackShocker, BountyBoardFlags.Medium3, BountyBoardOptions.MediumBlackShocker)
            },
            {
                "Hunt 500 Gold Shockers (Medium)",
                ("goldshocker", 500, BountyBoardKill3.GoldShocker, BountyBoardFlags.Medium3, BountyBoardOptions.MediumGoldShocker)
            },
            {
                "Hunt 500 Blue Shockers (Medium)",
                ("blueshocker", 500, BountyBoardKill3.BlueShocker, BountyBoardFlags.Medium3, BountyBoardOptions.MediumBlueShocker)
            },
            {
                "Hunt 500 Dire Wolves (Medium)",
                ("direwolf", 500, BountyBoardKill3.DireWolf, BountyBoardFlags.Medium3, BountyBoardOptions.MediumDireWolf)
            },
            {
                "Hunt 500 Ice Elementals (Medium)",
                ("iceelemental", 500, BountyBoardKill3.IceElemental, BountyBoardFlags.Medium3, BountyBoardOptions.MediumIceElemental)
            },
            {
                "Hunt 500 Ice Skeletons (Medium)",
                ("iceskeleton", 500, BountyBoardKill3.IceSkeleton, BountyBoardFlags.Medium3, BountyBoardOptions.MediumIceSkeleton)
            },
            {
                "Hunt 500 Ice Spores (Medium)",
                ("icespore", 500, BountyBoardKill3.IceSpore, BountyBoardFlags.Medium3, BountyBoardOptions.MediumIceSpore)
            },

            //Hard Tier
            {
                "Hunt 1000 Ancient Skeletons (Hard)",
                ("ancientskeleton", 1000, BountyBoardKill3.AncientSkeleton, BountyBoardFlags.Hard3, BountyBoardOptions.HardAncientSkeleton)
            },
            {
                "Hunt 1000 Ancient Beetalics (Hard)",
                ("ancientbeetalic", 1000, BountyBoardKill3.AncientBeetalic, BountyBoardFlags.Hard3, BountyBoardOptions.HardAncientBeetalic)
            },
            {
                "Hunt 1000 Losganns (Hard)",
                ("losgann", 1000, BountyBoardKill3.Losgann, BountyBoardFlags.Hard3, BountyBoardOptions.HardLosgann)
            },
            {
                "Hunt 1000 Ruidhtears (Hard)",
                ("ruidhtear", 1000, BountyBoardKill3.Ruidhtear, BountyBoardFlags.Hard3, BountyBoardOptions.HardRuidhtear)
            },
            {
                "Hunt 1000 Brown Mantises (Hard)",
                ("brownmantis", 1000, BountyBoardKill3.BrownMantis, BountyBoardFlags.Hard3, BountyBoardOptions.HardBrownMantis)
            },
            {
                "Hunt 1000 Gold Beetalics (Hard)",
                ("goldbeetalic", 1000, BountyBoardKill3.GoldBeetalic, BountyBoardFlags.Hard3, BountyBoardOptions.HardGoldBeetalic)
            },
            {
                "Hunt 1000 Red Shockers (Hard)",
                ("redshocker", 1000, BountyBoardKill3.RedShocker, BountyBoardFlags.Hard3, BountyBoardOptions.HardRedShocker)
            },
            {
                "Hunt 1000 Black Shockers (Hard)",
                ("blackshocker", 1000, BountyBoardKill3.BlackShocker, BountyBoardFlags.Hard3, BountyBoardOptions.HardBlackShocker)
            },
            {
                "Hunt 1000 Gold Shockers (Hard)",
                ("goldshocker", 1000, BountyBoardKill3.GoldShocker, BountyBoardFlags.Hard3, BountyBoardOptions.HardGoldShocker)
            },
            {
                "Hunt 1000 Blue Shockers (Hard)",
                ("blueshocker", 1000, BountyBoardKill3.BlueShocker, BountyBoardFlags.Hard3, BountyBoardOptions.HardBlueShocker)
            },
            {
                "Hunt 1000 Dire Wolves (Hard)",
                ("direwolf", 1000, BountyBoardKill3.DireWolf, BountyBoardFlags.Hard3, BountyBoardOptions.HardDireWolf)
            },
            {
                "Hunt 1000 Ice Elementals (Hard)",
                ("iceelemental", 1000, BountyBoardKill3.IceElemental, BountyBoardFlags.Hard3, BountyBoardOptions.HardIceElemental)
            },
            {
                "Hunt 1000 Ice Skeletons (Hard)",
                ("iceskeleton", 1000, BountyBoardKill3.IceSkeleton, BountyBoardFlags.Hard3, BountyBoardOptions.HardIceSkeleton)
            },
            {
                "Hunt 1000 Ice Spores (Hard)",
                ("icespore", 1000, BountyBoardKill3.IceSpore, BountyBoardFlags.Hard3, BountyBoardOptions.HardIceSpore)
            },

            {
                "Hunt 25 Kelberoths (Epic)",
                ("kelberoth", 25, BountyBoardKill3.Kelberoth, BountyBoardFlags.Epic3, BountyBoardOptions.EpicKelberoth)
            },
            {
                "Hunt 25 Ancient Dracos (Epic)",
                ("ancientdraco", 25, BountyBoardKill3.AncientDraco, BountyBoardFlags.Epic3, BountyBoardOptions.EpicAncientDraco)
            },
            {
                "Hunt 25 Green Mantis (Epic)",
                ("greenmantis", 25, BountyBoardKill3.GreenMantis, BountyBoardFlags.Epic3, BountyBoardOptions.EpicGreenMantis)
            },
            {
                "Hunt 25 Ancient Hydra (Epic)",
                ("ancienthydra", 25, BountyBoardKill3.AncientHydra, BountyBoardFlags.Epic3, BountyBoardOptions.EpicAncientHydra)
            }
        };

    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    private void CompleteBounty<T>(Aisling source, T bounty, Type bountyEnumType) where T: Enum
    {
        var bountyDictionary = GetBountyDictionary(bountyEnumType);
        var bountyEntry = bountyDictionary.FirstOrDefault(x => x.Value.KillEnum.Equals(bounty));

        if (bountyEntry.Key == null)
        {
            Subject.Reply(source, "Error: Unable to find the corresponding bounty in the system.");

            return;
        }

        (var monsterKey, var killRequirement, _, var difficultyFlag, _) = bountyEntry.Value;

        // ✅ Use `BountyMonsterKey` instead of the default counter key
        if (!source.Trackers.Counters.TryGetValue(monsterKey, out var currentKills) || (currentKills < killRequirement))
        {
            Subject.Reply(source, $"You have not completed the bounty for {bountyEntry.Key} yet.");

            return;
        }

        // ✅ Grant rewards
        GrantBountyRewards(source, bountyEntry.Key, killRequirement);

        // ✅ Remove the bounty slot
        source.Trackers.Enums.Remove(bountyEnumType);

        // ✅ Remove the kill counter
        source.Trackers.Counters.Remove(monsterKey, out _);

        // ✅ Remove only the specific difficulty flag
        source.Trackers.Flags.RemoveFlag(typeof(BountyBoardFlags), difficultyFlag);

        Subject.Reply(source, $"You have completed the bounty: {bountyEntry.Key}! Well done.");
    }

    private List<string> GetAllBounties()
        => GetBountyDictionary(typeof(BountyBoardKill1))
           .Keys
           .Concat(
               GetBountyDictionary(typeof(BountyBoardKill2))
                   .Keys)
           .Concat(
               GetBountyDictionary(typeof(BountyBoardKill3))
                   .Keys)
           .ToList();

    public static Dictionary<string, (string MonsterKey, int KillRequirement, Enum KillEnum, BountyBoardFlags DifficultyFlag,
        BountyBoardOptions BountyOption)> GetBountyDictionary(Type bountyEnumType)
        => bountyEnumType switch
        {
            _ when bountyEnumType == typeof(BountyBoardKill1) => BountyOptions1.ToDictionary(
                x => x.Key,
                x => (x.Value.MonsterKey, x.Value.KillRequirement, (Enum)x.Value.KillEnum, x.Value.DifficultyFlag, x.Value.BountyOption)),
            _ when bountyEnumType == typeof(BountyBoardKill2) => BountyOptions2.ToDictionary(
                x => x.Key,
                x => (x.Value.MonsterKey, x.Value.KillRequirement, (Enum)x.Value.KillEnum, x.Value.DifficultyFlag, x.Value.BountyOption)),
            _ when bountyEnumType == typeof(BountyBoardKill3) => BountyOptions3.ToDictionary(
                x => x.Key,
                x => (x.Value.MonsterKey, x.Value.KillRequirement, (Enum)x.Value.KillEnum, x.Value.DifficultyFlag, x.Value.BountyOption)),
            _ => throw new InvalidOperationException("Invalid bounty enum type.")
        };

    private (string, int, Enum, BountyBoardFlags, BountyBoardOptions) GetBountyFromAnyDictionary(string bounty)
        => GetBountyDictionary(typeof(BountyBoardKill1))
            .TryGetValue(bounty, out var bountyData)
            ? bountyData
            : GetBountyDictionary(typeof(BountyBoardKill2))
                .TryGetValue(bounty, out bountyData)
                ? bountyData
                : GetBountyDictionary(typeof(BountyBoardKill3))
                    .TryGetValue(bounty, out bountyData)
                    ? bountyData
                    : throw new KeyNotFoundException($"Bounty '{bounty}' not found in any dictionary.");

    private BountyBoardFlags GetMatchingDifficultyFlag(BountyBoardFlags originalFlag, int slot)
    {
        var flagName = originalFlag.ToString();

        if (flagName.EndsWith('1'))
            return Enum.Parse<BountyBoardFlags>(flagName.Replace("1", slot.ToString()));

        if (flagName.EndsWith('2'))
            return Enum.Parse<BountyBoardFlags>(flagName.Replace("2", slot.ToString()));

        if (flagName.EndsWith('3'))
            return Enum.Parse<BountyBoardFlags>(flagName.Replace("3", slot.ToString()));

        return originalFlag; // Default to original if no match
    }

    private void GrantBountyRewards(Aisling source, string bountyName, int killRequirement)
    {
        var baseExp = 500; // Base XP per bounty
        var goldReward = 1000; // Base Gold Reward

        // Adjust reward based on difficulty
        if (killRequirement == 100)
        {
            baseExp = 500;
            goldReward = 1000;
        } else if (killRequirement == 500)
        {
            baseExp = 2500;
            goldReward = 5000;
        } else if (killRequirement == 1000)
        {
            baseExp = 5000;
            goldReward = 10000;
        }

        // Grant EXP and Gold
        ExperienceDistributionScript.GiveExp(source, baseExp);
        source.TryGiveGold(goldReward);

        Subject.Reply(source, $"You received {baseExp} experience and {goldReward} gold for completing {bountyName}.");
    }

    private bool HasBountyCompleted(Aisling source, Type bountyEnumType)
    {
        if (!source.Trackers.Enums.TryGetValue(bountyEnumType, out var activeBounty) || activeBounty.Equals(default(Enum)))
            return false; // ✅ No active bounty in this slot, so no completion check needed.

        var bountyDictionary = GetBountyDictionary(bountyEnumType);

        foreach ((_, (var monsterKey, var killRequirement, var killEnum, var difficultyFlag, _)) in bountyDictionary)
        {
            if (string.IsNullOrEmpty(monsterKey) || !killEnum.Equals(activeBounty))
                continue; // ✅ Ensure we only check the active bounty

            // ✅ Validate Kill Requirement Based on Difficulty
            if (source.Trackers.Flags.HasFlag(difficultyFlag)
                && source.Trackers.Counters.TryGetValue(monsterKey, out var currentKills)
                && (currentKills >= killRequirement))
                return true;
        }

        return false;
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage1 = source.Trackers.Enums.TryGetValue(out BountyBoardKill1 _);
        var hasStage2 = source.Trackers.Enums.TryGetValue(out BountyBoardKill2 _);
        var hasStage3 = source.Trackers.Enums.TryGetValue(out BountyBoardKill3 _);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "bountyboard_initial":
            {
                if (!source.UserStatSheet.Master)
                {
                    Subject.Reply(source, "The bounty board doesn't make any sense to you. (Master Only)");

                    return;
                }

                Subject.Options.Insert(
                    0,
                    new DialogOption
                    {
                        DialogKey = "bountyboard_questinitial",
                        OptionText = "Browse Posted Bounties"
                    });

                if (hasStage1 && HasBountyCompleted(source, typeof(BountyBoardKill1)))
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_turnin1",
                            OptionText = "Turn in Bounty"
                        });

                if (hasStage2 && HasBountyCompleted(source, typeof(BountyBoardKill2)))
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_turnin2",
                            OptionText = "Turn in Bounty"
                        });

                if (hasStage3 && HasBountyCompleted(source, typeof(BountyBoardKill3)))
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_turnin3",
                            OptionText = "Turn in Bounty"
                        });

                break;
            }

            case "bountyboard_turnin1":
                ProcessBountyTurnIn(source, typeof(BountyBoardKill1));

                break;

            case "bountyboard_turnin2":
                ProcessBountyTurnIn(source, typeof(BountyBoardKill2));

                break;

            case "bountyboard_turnin3":
                ProcessBountyTurnIn(source, typeof(BountyBoardKill3));

                break;

            case "bountyboard_questinitial":
            {
                List<string> availableBounties;

                if (hasStage1 && hasStage2 && hasStage3)
                {
                    Subject.Reply(source, "You're currently on three bounties, you can't take anymore!");

                    return;
                }

                if (!source.Trackers.TimedEvents.HasActiveEvent("bountyboardreset", out _))
                {
                    if (source.Trackers.Flags.TryGetFlag(out BountyBoardOptions flags))
                        source.Trackers.Flags.RemoveFlag(flags);

                    var allBounties = GetAllBounties();
                    availableBounties = SelectRandomBounties(allBounties, 5);

                    source.Trackers.Counters.Remove("maxBountiesAccepted", out _);

                    foreach (var bounty in availableBounties)
                    {
                        (_, _, _, _, var bountyOption) = GetBountyFromAnyDictionary(bounty);
                        source.Trackers.Flags.AddFlag(bountyOption);
                    }

                    source.Trackers.TimedEvents.AddEvent("bountyboardreset", TimeSpan.FromHours(8), true);
                } else if (source.Trackers.Counters.TryGetValue("maxBountiesAccepted", out var count) && (count >= 3))
                {
                    Subject.Reply(source, "You can only accept three bounties per 8 hours.");

                    return;
                }

                availableBounties = RetrieveExistingBounties(source);

                if (availableBounties.Count == 0)
                {
                    Subject.Reply(source, "No bounties are available at this time.");

                    return;
                }

                foreach (var bounty in availableBounties)
                    if (!Subject.HasOption(bounty))
                        Subject.Options.Add(
                            new DialogOption
                            {
                                DialogKey = "bountyboard_acceptbounty",
                                OptionText = bounty
                            });

                break;
            }

            case "bountyboard_acceptbounty":
            {
                // Retrieve the selected bounty from the Context
                var selectedBounty = Subject.Context as string;

                if (string.IsNullOrEmpty(selectedBounty))
                {
                    Subject.Reply(source, "Invalid bounty selection.");

                    return;
                }

                // Merge all bounty dictionaries into a single one, preventing duplicates
                var bountyDictionary
                    = new Dictionary<string, (string MonsterKey, int KillRequirement, Enum KillEnum, BountyBoardFlags DifficultyFlag,
                        BountyBoardOptions BountyOption)>();

                foreach (var dict in new[]
                         {
                             GetBountyDictionary(typeof(BountyBoardKill1)),
                             GetBountyDictionary(typeof(BountyBoardKill2)),
                             GetBountyDictionary(typeof(BountyBoardKill3))
                         })
                {
                    foreach (var kvp in dict)
                        if (!bountyDictionary.ContainsKey(kvp.Key)) // Prevent duplicates
                            bountyDictionary[kvp.Key] = kvp.Value;
                }

                // Ensure the selected bounty exists
                if (!bountyDictionary.TryGetValue(selectedBounty, out var bountyData))
                {
                    Subject.Reply(source, "Invalid bounty selection.");

                    return;
                }

                // Extract data from the bounty
                (_, var killRequirement, var killEnum, var difficultyFlag, var bountyOption) = bountyData;

                // Determine which slot to use
                var hasBounty1 = source.Trackers.Enums.TryGetValue(out BountyBoardKill1 bounty1) && !bounty1.Equals(default);
                var hasBounty2 = source.Trackers.Enums.TryGetValue(out BountyBoardKill2 bounty2) && !bounty2.Equals(default);
                var hasBounty3 = source.Trackers.Enums.TryGetValue(out BountyBoardKill3 bounty3) && !bounty3.Equals(default);

                if (!hasBounty1)
                {
                    source.Trackers.Enums.Set(typeof(BountyBoardKill1), killEnum);
                    difficultyFlag = GetMatchingDifficultyFlag(difficultyFlag, 1);
                } else if (!hasBounty2)
                {
                    source.Trackers.Enums.Set(typeof(BountyBoardKill2), killEnum);
                    difficultyFlag = GetMatchingDifficultyFlag(difficultyFlag, 2);
                } else if (!hasBounty3)
                {
                    source.Trackers.Enums.Set(typeof(BountyBoardKill3), killEnum);
                    difficultyFlag = GetMatchingDifficultyFlag(difficultyFlag, 3);
                } else
                {
                    Subject.Reply(source, "You already have three active bounties. Complete one before accepting another.");

                    return;
                }

                // Apply the appropriate flag
                source.Trackers.Flags.AddFlag(difficultyFlag);
                source.Trackers.Counters.AddOrIncrement("maxBountiesAccepted");

                // Remove the bounty option flag so it does not reappear
                if (bountyOption != BountyBoardOptions.None)
                    source.Trackers.Flags.RemoveFlag(bountyOption);

                Subject.Reply(source, $"You've accepted the bounty: {selectedBounty}. Kill {killRequirement} to complete it!");

                break;
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is > 0 && (optionIndex.Value <= Subject.Options.Count))

            // Store the selected bounty in Context
            Subject.Context = Subject.Options[optionIndex.Value - 1].OptionText;

        base.OnNext(source, optionIndex);
    }

    private void ProcessBountyTurnIn(Aisling source, Type bountyEnumType)
    {
        if ((bountyEnumType == typeof(BountyBoardKill1)) && source.Trackers.Enums.TryGetValue(out BountyBoardKill1 bounty1))
            CompleteBounty(source, bounty1, bountyEnumType);
        else if ((bountyEnumType == typeof(BountyBoardKill2)) && source.Trackers.Enums.TryGetValue(out BountyBoardKill2 bounty2))
            CompleteBounty(source, bounty2, bountyEnumType);
        else if ((bountyEnumType == typeof(BountyBoardKill3)) && source.Trackers.Enums.TryGetValue(out BountyBoardKill3 bounty3))
            CompleteBounty(source, bounty3, bountyEnumType);
        else
            Subject.Reply(source, "You have no completed bounties to turn in.");
    }

    private List<string> RetrieveExistingBounties(Aisling source)
    {
        var availableBounties = new List<string>();

        if (!source.Trackers.Flags.TryGetFlag<BountyBoardOptions>(out var playerBountyOptions))
        {
            Subject.Reply(source, "No active bounties found.");

            return availableBounties;
        }

        foreach (var bountyDict in new[]
                 {
                     GetBountyDictionary(typeof(BountyBoardKill1)),
                     GetBountyDictionary(typeof(BountyBoardKill2)),
                     GetBountyDictionary(typeof(BountyBoardKill3))
                 })
        {
            foreach ((var bountyName, (_, _, _, _, var bountyOption)) in bountyDict)
                if (playerBountyOptions.HasFlag(bountyOption))
                    availableBounties.Add(bountyName);
        }

        return availableBounties;
    }

    private List<string> SelectRandomBounties(List<string> allBounties, int count)
    {
        var selectedBounties = new List<string>();
        var selectedMonsters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Filter out epic quests BEFORE shuffling
        var filteredBounties = allBounties
                               .Where(bounty => !bounty.Contains("(Epic)", StringComparison.OrdinalIgnoreCase)) // Exclude Epic bounties
                               .ToList();

        // Shuffle filtered list before selection
        var shuffledBounties = filteredBounties.OrderBy(_ => Random.Next())
                                               .ToList();

        foreach (var bounty in shuffledBounties)
        {
            (var monsterKey, _, _, _, _) = GetBountyFromAnyDictionary(bounty);

            // Skip if this monster was already selected at another difficulty
            if (selectedMonsters.Contains(monsterKey))
                continue;

            // Add to selected lists
            selectedBounties.Add(bounty);
            selectedMonsters.Add(monsterKey);

            // Stop when reaching the required count
            if (selectedBounties.Count == count)
                break;
        }

        return selectedBounties;
    }
}