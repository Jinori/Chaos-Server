namespace Chaos.Definitions;

public class BountyBoardDictionary
{
     public static readonly
        Dictionary<string, (string MonsterKey, int KillRequirement, BountyBoardKill1 KillEnum, BountyBoardFlags DifficultyFlag,
            BountyBoardOptions BountyOption )> BountyOptions1 = new()
        {
            // Easy Tier
            {
                "Hunt 100 Ancient Skeletons ({=qEasy{=x{=h)",
                ("ancientskeleton", 100, BountyBoardKill1.AncientSkeleton, BountyBoardFlags.Easy1, BountyBoardOptions.EasyAncientSkeleton)
            },
            {
                "Hunt 100 Ancient Beetalics ({=qEasy{=x{=h)",
                ("ancientbeetalic", 100, BountyBoardKill1.AncientBeetalic, BountyBoardFlags.Easy1, BountyBoardOptions.EasyAncientBeetalic)
            },
            {
                "Hunt 100 Losganns ({=qEasy{=x{=h)",
                ("losgann", 100, BountyBoardKill1.Losgann, BountyBoardFlags.Easy1, BountyBoardOptions.EasyLosgann)
            },
            {
                "Hunt 100 Ruidhtears ({=qEasy{=x{=h)",
                ("ruidhtear", 100, BountyBoardKill1.Ruidhtear, BountyBoardFlags.Easy1, BountyBoardOptions.EasyRuidhtear)
            },
            {
                "Hunt 100 Brown Mantises ({=qEasy{=x{=h)",
                ("brownmantis", 100, BountyBoardKill1.BrownMantis, BountyBoardFlags.Easy1, BountyBoardOptions.EasyBrownMantis)
            },
            {
                "Hunt 100 Gold Beetalics ({=qEasy{=x{=h)",
                ("goldbeetalic", 100, BountyBoardKill1.GoldBeetalic, BountyBoardFlags.Easy1, BountyBoardOptions.EasyGoldBeetalic)
            },
            {
                "Hunt 100 Red Shockers ({=qEasy{=x{=h)",
                ("redshocker", 100, BountyBoardKill1.RedShocker, BountyBoardFlags.Easy1, BountyBoardOptions.EasyRedShocker)
            },
            {
                "Hunt 100 Black Shockers ({=qEasy{=x{=h)",
                ("blackshocker", 100, BountyBoardKill1.BlackShocker, BountyBoardFlags.Easy1, BountyBoardOptions.EasyBlackShocker)
            },
            {
                "Hunt 100 Gold Shockers ({=qEasy{=x{=h)",
                ("goldshocker", 100, BountyBoardKill1.GoldShocker, BountyBoardFlags.Easy1, BountyBoardOptions.EasyGoldShocker)
            },
            {
                "Hunt 100 Blue Shockers ({=qEasy{=x{=h)",
                ("blueshocker", 100, BountyBoardKill1.BlueShocker, BountyBoardFlags.Easy1, BountyBoardOptions.EasyBlueShocker)
            },
            {
                "Hunt 100 Dire Wolves ({=qEasy{=x{=h)",
                ("direwolf", 100, BountyBoardKill1.DireWolf, BountyBoardFlags.Easy1, BountyBoardOptions.EasyDireWolf)
            },
            {
                "Hunt 100 Ice Elementals ({=qEasy{=x{=h)",
                ("iceelemental", 100, BountyBoardKill1.IceElemental, BountyBoardFlags.Easy1, BountyBoardOptions.EasyIceElemental)
            },
            {
                "Hunt 100 Ice Skeletons ({=qEasy{=x{=h)",
                ("iceskeleton", 100, BountyBoardKill1.IceSkeleton, BountyBoardFlags.Easy1, BountyBoardOptions.EasyIceSkeleton)
            },
            {
                "Hunt 100 Ice Spores ({=qEasy{=x{=h)",
                ("icespore", 100, BountyBoardKill1.IceSpore, BountyBoardFlags.Easy1, BountyBoardOptions.EasyIceSpore)
            },

            // Medium Tier
            {
                "Hunt 250 Ancient Skeletons ({=cMedium{=x{=h)",
                ("ancientskeleton", 250, BountyBoardKill1.AncientSkeleton, BountyBoardFlags.Medium1,
                    BountyBoardOptions.MediumAncientSkeleton)
            },
            {
                "Hunt 250 Ancient Beetalics ({=cMedium{=x{=h)",
                ("ancientbeetalic", 250, BountyBoardKill1.AncientBeetalic, BountyBoardFlags.Medium1,
                    BountyBoardOptions.MediumAncientBeetalic)
            },
            {
                "Hunt 250 Losganns ({=cMedium{=x{=h)",
                ("losgann", 250, BountyBoardKill1.Losgann, BountyBoardFlags.Medium1, BountyBoardOptions.MediumLosgann)
            },
            {
                "Hunt 250 Ruidhtears ({=cMedium{=x{=h)",
                ("ruidhtear", 250, BountyBoardKill1.Ruidhtear, BountyBoardFlags.Medium1, BountyBoardOptions.MediumRuidhtear)
            },
            {
                "Hunt 250 Brown Mantises ({=cMedium{=x{=h)",
                ("brownmantis", 250, BountyBoardKill1.BrownMantis, BountyBoardFlags.Medium1, BountyBoardOptions.MediumBrownMantis)
            },
            {
                "Hunt 250 Gold Beetalics ({=cMedium{=x{=h)",
                ("goldbeetalic", 250, BountyBoardKill1.GoldBeetalic, BountyBoardFlags.Medium1, BountyBoardOptions.MediumGoldBeetalic)
            },
            {
                "Hunt 250 Red Shockers ({=cMedium{=x{=h)",
                ("redshocker", 250, BountyBoardKill1.RedShocker, BountyBoardFlags.Medium1, BountyBoardOptions.MediumRedShocker)
            },
            {
                "Hunt 250 Black Shockers ({=cMedium{=x{=h)",
                ("blackshocker", 250, BountyBoardKill1.BlackShocker, BountyBoardFlags.Medium1, BountyBoardOptions.MediumBlackShocker)
            },
            {
                "Hunt 250 Gold Shockers ({=cMedium{=x{=h)",
                ("goldshocker", 250, BountyBoardKill1.GoldShocker, BountyBoardFlags.Medium1, BountyBoardOptions.MediumGoldShocker)
            },
            {
                "Hunt 250 Blue Shockers ({=cMedium{=x{=h)",
                ("blueshocker", 250, BountyBoardKill1.BlueShocker, BountyBoardFlags.Medium1, BountyBoardOptions.MediumBlueShocker)
            },
            {
                "Hunt 250 Dire Wolves ({=cMedium{=x{=h)",
                ("direwolf", 250, BountyBoardKill1.DireWolf, BountyBoardFlags.Medium1, BountyBoardOptions.MediumDireWolf)
            },
            {
                "Hunt 250 Ice Elementals ({=cMedium{=x{=h)",
                ("iceelemental", 250, BountyBoardKill1.IceElemental, BountyBoardFlags.Medium1, BountyBoardOptions.MediumIceElemental)
            },
            {
                "Hunt 250 Ice Skeletons ({=cMedium{=x{=h)",
                ("iceskeleton", 250, BountyBoardKill1.IceSkeleton, BountyBoardFlags.Medium1, BountyBoardOptions.MediumIceSkeleton)
            },
            {
                "Hunt 250 Ice Spores ({=cMedium{=x{=h)",
                ("icespore", 250, BountyBoardKill1.IceSpore, BountyBoardFlags.Medium1, BountyBoardOptions.MediumIceSpore)
            },

            // Hard Tier
            {
                "Hunt 400 Ancient Skeletons ({=bHard{=x{=h)",
                ("ancientskeleton", 400, BountyBoardKill1.AncientSkeleton, BountyBoardFlags.Hard1, BountyBoardOptions.HardAncientSkeleton)
            },
            {
                "Hunt 400 Ancient Beetalics ({=bHard{=x{=h)",
                ("ancientbeetalic", 400, BountyBoardKill1.AncientBeetalic, BountyBoardFlags.Hard1, BountyBoardOptions.HardAncientBeetalic)
            },
            {
                "Hunt 400 Losganns ({=bHard{=x{=h)",
                ("losgann", 400, BountyBoardKill1.Losgann, BountyBoardFlags.Hard1, BountyBoardOptions.HardLosgann)
            },
            {
                "Hunt 400 Ruidhtears ({=bHard{=x{=h)",
                ("ruidhtear", 400, BountyBoardKill1.Ruidhtear, BountyBoardFlags.Hard1, BountyBoardOptions.HardRuidhtear)
            },
            {
                "Hunt 400 Brown Mantises ({=bHard{=x{=h)",
                ("brownmantis", 400, BountyBoardKill1.BrownMantis, BountyBoardFlags.Hard1, BountyBoardOptions.HardBrownMantis)
            },
            {
                "Hunt 400 Gold Beetalics ({=bHard{=x{=h)",
                ("goldbeetalic", 400, BountyBoardKill1.GoldBeetalic, BountyBoardFlags.Hard1, BountyBoardOptions.HardGoldBeetalic)
            },
            {
                "Hunt 400 Red Shockers ({=bHard{=x{=h)",
                ("redshocker", 400, BountyBoardKill1.RedShocker, BountyBoardFlags.Hard1, BountyBoardOptions.HardRedShocker)
            },
            {
                "Hunt 400 Black Shockers ({=bHard{=x{=h)",
                ("blackshocker", 400, BountyBoardKill1.BlackShocker, BountyBoardFlags.Hard1, BountyBoardOptions.HardBlackShocker)
            },
            {
                "Hunt 400 Gold Shockers ({=bHard{=x{=h)",
                ("goldshocker", 400, BountyBoardKill1.GoldShocker, BountyBoardFlags.Hard1, BountyBoardOptions.HardGoldShocker)
            },
            {
                "Hunt 400 Blue Shockers ({=bHard{=x{=h)",
                ("blueshocker", 400, BountyBoardKill1.BlueShocker, BountyBoardFlags.Hard1, BountyBoardOptions.HardBlueShocker)
            },
            {
                "Hunt 400 Dire Wolves ({=bHard{=x{=h)",
                ("direwolf", 400, BountyBoardKill1.DireWolf, BountyBoardFlags.Hard1, BountyBoardOptions.HardDireWolf)
            },
            {
                "Hunt 400 Ice Elementals ({=bHard{=x{=h)",
                ("iceelemental", 400, BountyBoardKill1.IceElemental, BountyBoardFlags.Hard1, BountyBoardOptions.HardIceElemental)
            },
            {
                "Hunt 400 Ice Skeletons ({=bHard{=x{=h)",
                ("iceskeleton", 400, BountyBoardKill1.IceSkeleton, BountyBoardFlags.Hard1, BountyBoardOptions.HardIceSkeleton)
            },
            {
                "Hunt 400 Ice Spores ({=bHard{=x{=h)",
                ("icespore", 400, BountyBoardKill1.IceSpore, BountyBoardFlags.Hard1, BountyBoardOptions.HardIceSpore)
            },

            // Epic Tier
            {
                "Hunt 10 Kelberoths ({=pEpic{=x{=h)",
                ("kelberoth", 10, BountyBoardKill1.Kelberoth, BountyBoardFlags.Epic1, BountyBoardOptions.EpicKelberoth)
            },
            {
                "Hunt 10 Ancient Dracos ({=pEpic{=x{=h)",
                ("ancientdraco", 10, BountyBoardKill1.AncientDraco, BountyBoardFlags.Epic1, BountyBoardOptions.EpicAncientDraco)
            },
            {
                "Hunt 10 Green Mantises ({=pEpic{=x{=h)",
                ("greenmantis", 10, BountyBoardKill1.GreenMantis, BountyBoardFlags.Epic1, BountyBoardOptions.EpicGreenMantis)
            },
            {
                "Hunt 10 Ancient Hydra ({=pEpic{=x{=h)",
                ("ancienthydra", 10, BountyBoardKill1.AncientHydra, BountyBoardFlags.Epic1, BountyBoardOptions.EpicAncientHydra)
            }
        };

    public static readonly
        Dictionary<string, (string MonsterKey, int KillRequirement, BountyBoardKill2 KillEnum, BountyBoardFlags DifficultyFlag,
            BountyBoardOptions BountyOption )> BountyOptions2 = new()
        {
            // Easy Tier
            {
                "Hunt 100 Ancient Skeletons ({=qEasy{=k{=h)",
                ("ancientskeleton", 100, BountyBoardKill2.AncientSkeleton, BountyBoardFlags.Easy2, BountyBoardOptions.EasyAncientSkeleton)
            },
            {
                "Hunt 100 Ancient Beetalics ({=qEasy{=k{=h)",
                ("ancientbeetalic", 100, BountyBoardKill2.AncientBeetalic, BountyBoardFlags.Easy2, BountyBoardOptions.EasyAncientBeetalic)
            },
            {
                "Hunt 100 Losganns ({=qEasy{=k{=h)",
                ("losgann", 100, BountyBoardKill2.Losgann, BountyBoardFlags.Easy2, BountyBoardOptions.EasyLosgann)
            },
            {
                "Hunt 100 Ruidhtears ({=qEasy{=k{=h)",
                ("ruidhtear", 100, BountyBoardKill2.Ruidhtear, BountyBoardFlags.Easy2, BountyBoardOptions.EasyRuidhtear)
            },
            {
                "Hunt 100 Brown Mantises ({=qEasy{=k{=h)",
                ("brownmantis", 100, BountyBoardKill2.BrownMantis, BountyBoardFlags.Easy2, BountyBoardOptions.EasyBrownMantis)
            },
            {
                "Hunt 100 Gold Beetalics ({=qEasy{=k{=h)",
                ("goldbeetalic", 100, BountyBoardKill2.GoldBeetalic, BountyBoardFlags.Easy2, BountyBoardOptions.EasyGoldBeetalic)
            },
            {
                "Hunt 100 Red Shockers ({=qEasy{=k{=h)",
                ("redshocker", 100, BountyBoardKill2.RedShocker, BountyBoardFlags.Easy2, BountyBoardOptions.EasyRedShocker)
            },
            {
                "Hunt 100 Black Shockers ({=qEasy{=k{=h)",
                ("blackshocker", 100, BountyBoardKill2.BlackShocker, BountyBoardFlags.Easy2, BountyBoardOptions.EasyBlackShocker)
            },
            {
                "Hunt 100 Gold Shockers ({=qEasy{=k{=h)",
                ("goldshocker", 100, BountyBoardKill2.GoldShocker, BountyBoardFlags.Easy2, BountyBoardOptions.EasyGoldShocker)
            },
            {
                "Hunt 100 Blue Shockers ({=qEasy{=k{=h)",
                ("blueshocker", 100, BountyBoardKill2.BlueShocker, BountyBoardFlags.Easy2, BountyBoardOptions.EasyBlueShocker)
            },
            {
                "Hunt 100 Dire Wolves ({=qEasy{=k{=h)",
                ("direwolf", 100, BountyBoardKill2.DireWolf, BountyBoardFlags.Easy2, BountyBoardOptions.EasyDireWolf)
            },
            {
                "Hunt 100 Ice Elementals ({=qEasy{=k{=h)",
                ("iceelemental", 100, BountyBoardKill2.IceElemental, BountyBoardFlags.Easy2, BountyBoardOptions.EasyIceElemental)
            },
            {
                "Hunt 100 Ice Skeletons ({=qEasy{=k{=h)",
                ("iceskeleton", 100, BountyBoardKill2.IceSkeleton, BountyBoardFlags.Easy2, BountyBoardOptions.EasyIceSkeleton)
            },
            {
                "Hunt 100 Ice Spores ({=qEasy{=k{=h)",
                ("icespore", 100, BountyBoardKill2.IceSpore, BountyBoardFlags.Easy2, BountyBoardOptions.EasyIceSpore)
            },

            // Medium Tier
            {
                "Hunt 250 Ancient Skeletons ({=cMedium{=k{=h)",
                ("ancientskeleton", 250, BountyBoardKill2.AncientSkeleton, BountyBoardFlags.Medium2,
                    BountyBoardOptions.MediumAncientSkeleton)
            },
            {
                "Hunt 250 Ancient Beetalics ({=cMedium{=k{=h)",
                ("ancientbeetalic", 250, BountyBoardKill2.AncientBeetalic, BountyBoardFlags.Medium2,
                    BountyBoardOptions.MediumAncientBeetalic)
            },
            {
                "Hunt 250 Losganns ({=cMedium{=k{=h)",
                ("losgann", 250, BountyBoardKill2.Losgann, BountyBoardFlags.Medium2, BountyBoardOptions.MediumLosgann)
            },
            {
                "Hunt 250 Ruidhtears ({=cMedium{=k{=h)",
                ("ruidhtear", 250, BountyBoardKill2.Ruidhtear, BountyBoardFlags.Medium2, BountyBoardOptions.MediumRuidhtear)
            },
            {
                "Hunt 250 Brown Mantises ({=cMedium{=k{=h)",
                ("brownmantis", 250, BountyBoardKill2.BrownMantis, BountyBoardFlags.Medium2, BountyBoardOptions.MediumBrownMantis)
            },
            {
                "Hunt 250 Gold Beetalics ({=cMedium{=k{=h)",
                ("goldbeetalic", 250, BountyBoardKill2.GoldBeetalic, BountyBoardFlags.Medium2, BountyBoardOptions.MediumGoldBeetalic)
            },
            {
                "Hunt 250 Red Shockers ({=cMedium{=k{=h)",
                ("redshocker", 250, BountyBoardKill2.RedShocker, BountyBoardFlags.Medium2, BountyBoardOptions.MediumRedShocker)
            },
            {
                "Hunt 250 Black Shockers ({=cMedium{=k{=h)",
                ("blackshocker", 250, BountyBoardKill2.BlackShocker, BountyBoardFlags.Medium2, BountyBoardOptions.MediumBlackShocker)
            },
            {
                "Hunt 250 Gold Shockers ({=cMedium{=k{=h)",
                ("goldshocker", 250, BountyBoardKill2.GoldShocker, BountyBoardFlags.Medium2, BountyBoardOptions.MediumGoldShocker)
            },
            {
                "Hunt 250 Blue Shockers ({=cMedium{=k{=h)",
                ("blueshocker", 250, BountyBoardKill2.BlueShocker, BountyBoardFlags.Medium2, BountyBoardOptions.MediumBlueShocker)
            },
            {
                "Hunt 250 Dire Wolves ({=cMedium{=k{=h)",
                ("direwolf", 250, BountyBoardKill2.DireWolf, BountyBoardFlags.Medium2, BountyBoardOptions.MediumDireWolf)
            },
            {
                "Hunt 250 Ice Elementals ({=cMedium{=k{=h)",
                ("iceelemental", 250, BountyBoardKill2.IceElemental, BountyBoardFlags.Medium2, BountyBoardOptions.MediumIceElemental)
            },
            {
                "Hunt 250 Ice Skeletons ({=cMedium{=k{=h)",
                ("iceskeleton", 250, BountyBoardKill2.IceSkeleton, BountyBoardFlags.Medium2, BountyBoardOptions.MediumIceSkeleton)
            },
            {
                "Hunt 250 Ice Spores ({=cMedium{=k{=h)",
                ("icespore", 250, BountyBoardKill2.IceSpore, BountyBoardFlags.Medium2, BountyBoardOptions.MediumIceSpore)
            },

            // Hard Tier
            {
                "Hunt 400 Ancient Skeletons ({=bHard{=k{=h)",
                ("ancientskeleton", 400, BountyBoardKill2.AncientSkeleton, BountyBoardFlags.Hard2, BountyBoardOptions.HardAncientSkeleton)
            },
            {
                "Hunt 400 Ancient Beetalics ({=bHard{=k{=h)",
                ("ancientbeetalic", 400, BountyBoardKill2.AncientBeetalic, BountyBoardFlags.Hard2, BountyBoardOptions.HardAncientBeetalic)
            },
            {
                "Hunt 400 Losganns ({=bHard{=k{=h)",
                ("losgann", 400, BountyBoardKill2.Losgann, BountyBoardFlags.Hard2, BountyBoardOptions.HardLosgann)
            },
            {
                "Hunt 400 Ruidhtears ({=bHard{=k{=h)",
                ("ruidhtear", 400, BountyBoardKill2.Ruidhtear, BountyBoardFlags.Hard2, BountyBoardOptions.HardRuidhtear)
            },
            {
                "Hunt 400 Brown Mantises ({=bHard{=k{=h)",
                ("brownmantis", 400, BountyBoardKill2.BrownMantis, BountyBoardFlags.Hard2, BountyBoardOptions.HardBrownMantis)
            },
            {
                "Hunt 400 Gold Beetalics ({=bHard{=k{=h)",
                ("goldbeetalic", 400, BountyBoardKill2.GoldBeetalic, BountyBoardFlags.Hard2, BountyBoardOptions.HardGoldBeetalic)
            },
            {
                "Hunt 400 Red Shockers ({=bHard{=k{=h)",
                ("redshocker", 400, BountyBoardKill2.RedShocker, BountyBoardFlags.Hard2, BountyBoardOptions.HardRedShocker)
            },
            {
                "Hunt 400 Black Shockers ({=bHard{=k{=h)",
                ("blackshocker", 400, BountyBoardKill2.BlackShocker, BountyBoardFlags.Hard2, BountyBoardOptions.HardBlackShocker)
            },
            {
                "Hunt 400 Gold Shockers ({=bHard{=k{=h)",
                ("goldshocker", 400, BountyBoardKill2.GoldShocker, BountyBoardFlags.Hard2, BountyBoardOptions.HardGoldShocker)
            },
            {
                "Hunt 400 Blue Shockers ({=bHard{=k{=h)",
                ("blueshocker", 400, BountyBoardKill2.BlueShocker, BountyBoardFlags.Hard2, BountyBoardOptions.HardBlueShocker)
            },
            {
                "Hunt 400 Dire Wolves ({=bHard{=k{=h)",
                ("direwolf", 400, BountyBoardKill2.DireWolf, BountyBoardFlags.Hard2, BountyBoardOptions.HardDireWolf)
            },
            {
                "Hunt 400 Ice Elementals ({=bHard{=k{=h)",
                ("iceelemental", 400, BountyBoardKill2.IceElemental, BountyBoardFlags.Hard2, BountyBoardOptions.HardIceElemental)
            },
            {
                "Hunt 400 Ice Skeletons ({=bHard{=k{=h)",
                ("iceskeleton", 400, BountyBoardKill2.IceSkeleton, BountyBoardFlags.Hard2, BountyBoardOptions.HardIceSkeleton)
            },
            {
                "Hunt 400 Ice Spores ({=bHard{=k{=h)",
                ("icespore", 400, BountyBoardKill2.IceSpore, BountyBoardFlags.Hard2, BountyBoardOptions.HardIceSpore)
            },

            // Epic Tier
            {
                "Hunt 10 Kelberoths ({=pEpic{=k{=h)",
                ("kelberoth", 10, BountyBoardKill2.Kelberoth, BountyBoardFlags.Epic2, BountyBoardOptions.EpicKelberoth)
            },
            {
                "Hunt 10 Ancient Dracos ({=pEpic{=k{=h)",
                ("ancientdraco", 10, BountyBoardKill2.AncientDraco, BountyBoardFlags.Epic2, BountyBoardOptions.EpicAncientDraco)
            },
            {
                "Hunt 10 Green Mantis ({=pEpic{=k{=h)",
                ("greenmantis", 10, BountyBoardKill2.GreenMantis, BountyBoardFlags.Epic2, BountyBoardOptions.EpicGreenMantis)
            },
            {
                "Hunt 10 Ancient Hydra ({=pEpic{=k{=h)",
                ("ancienthydra", 10, BountyBoardKill2.AncientHydra, BountyBoardFlags.Epic2, BountyBoardOptions.EpicAncientHydra)
            }
        };

    public static readonly
        Dictionary<string, (string MonsterKey, int KillRequirement, BountyBoardKill3 KillEnum, BountyBoardFlags DifficultyFlag,
            BountyBoardOptions BountyOption )> BountyOptions3 = new()
        {
            // Easy Tier
            {
                "Hunt 100 Ancient Skeletons ({=qEasy{=h{=h)",
                ("ancientskeleton", 100, BountyBoardKill3.AncientSkeleton, BountyBoardFlags.Easy3, BountyBoardOptions.EasyAncientSkeleton)
            },
            {
                "Hunt 100 Ancient Beetalics ({=qEasy{=h{=h)",
                ("ancientbeetalic", 100, BountyBoardKill3.AncientBeetalic, BountyBoardFlags.Easy3, BountyBoardOptions.EasyAncientBeetalic)
            },
            {
                "Hunt 100 Losganns ({=qEasy{=h{=h)",
                ("losgann", 100, BountyBoardKill3.Losgann, BountyBoardFlags.Easy3, BountyBoardOptions.EasyLosgann)
            },
            {
                "Hunt 100 Ruidhtears ({=qEasy{=h{=h)",
                ("ruidhtear", 100, BountyBoardKill3.Ruidhtear, BountyBoardFlags.Easy3, BountyBoardOptions.EasyRuidhtear)
            },
            {
                "Hunt 100 Brown Mantises ({=qEasy{=h{=h)",
                ("brownmantis", 100, BountyBoardKill3.BrownMantis, BountyBoardFlags.Easy3, BountyBoardOptions.EasyBrownMantis)
            },
            {
                "Hunt 100 Gold Beetalics ({=qEasy{=h{=h)",
                ("goldbeetalic", 100, BountyBoardKill3.GoldBeetalic, BountyBoardFlags.Easy3, BountyBoardOptions.EasyGoldBeetalic)
            },
            {
                "Hunt 100 Red Shockers ({=qEasy{=h{=h)",
                ("redshocker", 100, BountyBoardKill3.RedShocker, BountyBoardFlags.Easy3, BountyBoardOptions.EasyRedShocker)
            },
            {
                "Hunt 100 Black Shockers ({=qEasy{=h{=h)",
                ("blackshocker", 100, BountyBoardKill3.BlackShocker, BountyBoardFlags.Easy3, BountyBoardOptions.EasyBlackShocker)
            },
            {
                "Hunt 100 Gold Shockers ({=qEasy{=h{=h)",
                ("goldshocker", 100, BountyBoardKill3.GoldShocker, BountyBoardFlags.Easy3, BountyBoardOptions.EasyGoldShocker)
            },
            {
                "Hunt 100 Blue Shockers ({=qEasy{=h{=h)",
                ("blueshocker", 100, BountyBoardKill3.BlueShocker, BountyBoardFlags.Easy3, BountyBoardOptions.EasyBlueShocker)
            },
            {
                "Hunt 100 Dire Wolves ({=qEasy{=h{=h)",
                ("direwolf", 100, BountyBoardKill3.DireWolf, BountyBoardFlags.Easy3, BountyBoardOptions.EasyDireWolf)
            },
            {
                "Hunt 100 Ice Elementals ({=qEasy{=h{=h)",
                ("iceelemental", 100, BountyBoardKill3.IceElemental, BountyBoardFlags.Easy3, BountyBoardOptions.EasyIceElemental)
            },
            {
                "Hunt 100 Ice Skeletons ({=qEasy{=h{=h)",
                ("iceskeleton", 100, BountyBoardKill3.IceSkeleton, BountyBoardFlags.Easy3, BountyBoardOptions.EasyIceSkeleton)
            },
            {
                "Hunt 100 Ice Spores ({=qEasy{=h{=h)",
                ("icespore", 100, BountyBoardKill3.IceSpore, BountyBoardFlags.Easy3, BountyBoardOptions.EasyIceSpore)
            },

            //Medium Tier
            {
                "Hunt 250 Ancient Skeletons ({=cMedium{=h{=h)",
                ("ancientskeleton", 250, BountyBoardKill3.AncientSkeleton, BountyBoardFlags.Medium3,
                    BountyBoardOptions.MediumAncientSkeleton)
            },
            {
                "Hunt 250 Ancient Beetalics ({=cMedium{=h{=h)",
                ("ancientbeetalic", 250, BountyBoardKill3.AncientBeetalic, BountyBoardFlags.Medium3,
                    BountyBoardOptions.MediumAncientBeetalic)
            },
            {
                "Hunt 250 Losganns ({=cMedium{=h{=h)",
                ("losgann", 250, BountyBoardKill3.Losgann, BountyBoardFlags.Medium3, BountyBoardOptions.MediumLosgann)
            },
            {
                "Hunt 250 Ruidhtears ({=cMedium{=h{=h)",
                ("ruidhtear", 250, BountyBoardKill3.Ruidhtear, BountyBoardFlags.Medium3, BountyBoardOptions.MediumRuidhtear)
            },
            {
                "Hunt 250 Brown Mantises ({=cMedium{=h{=h)",
                ("brownmantis", 250, BountyBoardKill3.BrownMantis, BountyBoardFlags.Medium3, BountyBoardOptions.MediumBrownMantis)
            },
            {
                "Hunt 250 Gold Beetalics ({=cMedium{=h{=h)",
                ("goldbeetalic", 250, BountyBoardKill3.GoldBeetalic, BountyBoardFlags.Medium3, BountyBoardOptions.MediumGoldBeetalic)
            },
            {
                "Hunt 250 Red Shockers ({=cMedium{=h{=h)",
                ("redshocker", 250, BountyBoardKill3.RedShocker, BountyBoardFlags.Medium3, BountyBoardOptions.MediumRedShocker)
            },
            {
                "Hunt 250 Black Shockers ({=cMedium{=h{=h)",
                ("blackshocker", 250, BountyBoardKill3.BlackShocker, BountyBoardFlags.Medium3, BountyBoardOptions.MediumBlackShocker)
            },
            {
                "Hunt 250 Gold Shockers ({=cMedium{=h{=h)",
                ("goldshocker", 250, BountyBoardKill3.GoldShocker, BountyBoardFlags.Medium3, BountyBoardOptions.MediumGoldShocker)
            },
            {
                "Hunt 250 Blue Shockers ({=cMedium{=h{=h)",
                ("blueshocker", 250, BountyBoardKill3.BlueShocker, BountyBoardFlags.Medium3, BountyBoardOptions.MediumBlueShocker)
            },
            {
                "Hunt 250 Dire Wolves ({=cMedium{=h{=h)",
                ("direwolf", 250, BountyBoardKill3.DireWolf, BountyBoardFlags.Medium3, BountyBoardOptions.MediumDireWolf)
            },
            {
                "Hunt 250 Ice Elementals ({=cMedium{=h{=h)",
                ("iceelemental", 250, BountyBoardKill3.IceElemental, BountyBoardFlags.Medium3, BountyBoardOptions.MediumIceElemental)
            },
            {
                "Hunt 250 Ice Skeletons ({=cMedium{=h{=h)",
                ("iceskeleton", 250, BountyBoardKill3.IceSkeleton, BountyBoardFlags.Medium3, BountyBoardOptions.MediumIceSkeleton)
            },
            {
                "Hunt 250 Ice Spores ({=cMedium{=h{=h)",
                ("icespore", 250, BountyBoardKill3.IceSpore, BountyBoardFlags.Medium3, BountyBoardOptions.MediumIceSpore)
            },

            //Hard Tier
            {
                "Hunt 400 Ancient Skeletons ({=bHard{=h{=h)",
                ("ancientskeleton", 400, BountyBoardKill3.AncientSkeleton, BountyBoardFlags.Hard3, BountyBoardOptions.HardAncientSkeleton)
            },
            {
                "Hunt 400 Ancient Beetalics ({=bHard{=h{=h)",
                ("ancientbeetalic", 400, BountyBoardKill3.AncientBeetalic, BountyBoardFlags.Hard3, BountyBoardOptions.HardAncientBeetalic)
            },
            {
                "Hunt 400 Losganns ({=bHard{=h{=h)",
                ("losgann", 400, BountyBoardKill3.Losgann, BountyBoardFlags.Hard3, BountyBoardOptions.HardLosgann)
            },
            {
                "Hunt 400 Ruidhtears ({=bHard{=h{=h)",
                ("ruidhtear", 400, BountyBoardKill3.Ruidhtear, BountyBoardFlags.Hard3, BountyBoardOptions.HardRuidhtear)
            },
            {
                "Hunt 400 Brown Mantises ({=bHard{=h{=h)",
                ("brownmantis", 400, BountyBoardKill3.BrownMantis, BountyBoardFlags.Hard3, BountyBoardOptions.HardBrownMantis)
            },
            {
                "Hunt 400 Gold Beetalics ({=bHard{=h{=h)",
                ("goldbeetalic", 400, BountyBoardKill3.GoldBeetalic, BountyBoardFlags.Hard3, BountyBoardOptions.HardGoldBeetalic)
            },
            {
                "Hunt 400 Red Shockers ({=bHard{=h{=h)",
                ("redshocker", 400, BountyBoardKill3.RedShocker, BountyBoardFlags.Hard3, BountyBoardOptions.HardRedShocker)
            },
            {
                "Hunt 400 Black Shockers ({=bHard{=h{=h)",
                ("blackshocker", 400, BountyBoardKill3.BlackShocker, BountyBoardFlags.Hard3, BountyBoardOptions.HardBlackShocker)
            },
            {
                "Hunt 400 Gold Shockers ({=bHard{=h{=h)",
                ("goldshocker", 400, BountyBoardKill3.GoldShocker, BountyBoardFlags.Hard3, BountyBoardOptions.HardGoldShocker)
            },
            {
                "Hunt 400 Blue Shockers ({=bHard{=h{=h)",
                ("blueshocker", 400, BountyBoardKill3.BlueShocker, BountyBoardFlags.Hard3, BountyBoardOptions.HardBlueShocker)
            },
            {
                "Hunt 400 Dire Wolves ({=bHard{=h{=h)",
                ("direwolf", 400, BountyBoardKill3.DireWolf, BountyBoardFlags.Hard3, BountyBoardOptions.HardDireWolf)
            },
            {
                "Hunt 400 Ice Elementals ({=bHard{=h{=h)",
                ("iceelemental", 400, BountyBoardKill3.IceElemental, BountyBoardFlags.Hard3, BountyBoardOptions.HardIceElemental)
            },
            {
                "Hunt 400 Ice Skeletons ({=bHard{=h{=h)",
                ("iceskeleton", 400, BountyBoardKill3.IceSkeleton, BountyBoardFlags.Hard3, BountyBoardOptions.HardIceSkeleton)
            },
            {
                "Hunt 400 Ice Spores ({=bHard{=h{=h)",
                ("icespore", 400, BountyBoardKill3.IceSpore, BountyBoardFlags.Hard3, BountyBoardOptions.HardIceSpore)
            },

            {
                "Hunt 10 Kelberoths ({=pEpic{=h{=h)",
                ("kelberoth", 10, BountyBoardKill3.Kelberoth, BountyBoardFlags.Epic3, BountyBoardOptions.EpicKelberoth)
            },
            {
                "Hunt 10 Ancient Dracos ({=pEpic{=h{=h)",
                ("ancientdraco", 10, BountyBoardKill3.AncientDraco, BountyBoardFlags.Epic3, BountyBoardOptions.EpicAncientDraco)
            },
            {
                "Hunt 10 Green Mantis ({=pEpic{=h{=h)",
                ("greenmantis", 10, BountyBoardKill3.GreenMantis, BountyBoardFlags.Epic3, BountyBoardOptions.EpicGreenMantis)
            },
            {
                "Hunt 10 Ancient Hydra ({=pEpic{=h{=h)",
                ("ancienthydra", 10, BountyBoardKill3.AncientHydra, BountyBoardFlags.Epic3, BountyBoardOptions.EpicAncientHydra)
            }
        };
}