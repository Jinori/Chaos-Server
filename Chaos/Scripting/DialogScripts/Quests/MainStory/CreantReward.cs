using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class CreantReward : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;

    /// <inheritdoc />
    public CreantReward(
        Dialog subject,
        ISimpleCache simpleCache,
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        ItemFactory = itemFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "creant_reward_initial":
            {
                if ((source.Trackers.Flags.HasFlag(CreantEnums.KilledMedusa) && (source.MapInstance.Template.TemplateKey == "6599"))
                    || (source.Trackers.Flags.HasFlag(CreantEnums.KilledPhoenix) && (source.MapInstance.Template.TemplateKey == "989"))
                    || (source.Trackers.Flags.HasFlag(CreantEnums.KilledTauren) && (source.MapInstance.Template.TemplateKey == "19522"))
                    || (source.Trackers.Flags.HasFlag(CreantEnums.KilledSham) && (source.MapInstance.Template.TemplateKey == "31010")))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "creant_reward_initial2",
                        OptionText = "Speak the Incantation"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                var option2 = new DialogOption
                {
                    DialogKey = "creant_return",
                    OptionText = "Take me out of here."
                };

                if (!Subject.HasOption(option2.OptionText))
                    Subject.Options.Insert(0, option2);

                break;
            }

            case "creant_reward_initial2":
            {
                source.Say("Le solas na lasair sìorraidh,");
                source.SendOrangeBarMessage("By the light of the eternal flame,");

                break;
            }

            case "creant_reward_initial3":
            {
                source.Say("Le slabhraidhean na cloiche neo-throma,");
                source.SendOrangeBarMessage("By the chains of unyielding stone,");

                break;
            }

            case "creant_reward_initial4":
            {
                source.Say("Tha mi gad cheangal, sgàil na doimhneachd,");
                source.SendOrangeBarMessage("I bind thee, shadow of the abyss,");

                break;
            }

            case "creant_reward_initial5":
            {
                source.Say("Gu domhainn, far nach till neach sam bith.");
                source.SendOrangeBarMessage("To the depths, from which none may return.");

                break;
            }

            case "creant_reward_initial6":
            {
                source.Say("Cadal a-nis, ann an sàmhchair is dorchadas,");
                source.SendOrangeBarMessage("Sleep now, in silence and darkness,");

                break;
            }

            case "creant_reward_initial7":
            {
                source.Say("Ceangailte le toil an dìonadair,");
                source.SendOrangeBarMessage("Bound by the will of the protector,");

                break;
            }

            case "creant_reward_initial8":
            {
                source.Say("Le solas na lasair sìorraidh,");
                source.SendOrangeBarMessage("By the light of the eternal flame,");

                break;
            }

            case "creant_reward_final":

                var pureGmGearDictionary = new Dictionary<(string mapTemplateKey, BaseClass baseClass, Gender gender), string[]>
                {
                    {
                        ("19522", BaseClass.Warrior, Gender.Male), new[]
                        {
                            "duskrenderbattleaxe"
                        }
                    },
                    {
                        ("19522", BaseClass.Warrior, Gender.Female), new[]
                        {
                            "duskrenderbattleaxe"
                        }
                    },
                    {
                        ("19522", BaseClass.Priest, Gender.Male), new[]
                        {
                            "PureMaleGMPriestArmor"
                        }
                    },
                    {
                        ("19522", BaseClass.Priest, Gender.Female), new[]
                        {
                            "PureFemaleGMPriestArmor"
                        }
                    },
                    {
                        ("19522", BaseClass.Wizard, Gender.Male), new[]
                        {
                            "PureMaleGMWizardArmor"
                        }
                    },
                    {
                        ("19522", BaseClass.Wizard, Gender.Female), new[]
                        {
                            "PureFemaleGMWizardArmor"
                        }
                    },

                    {
                        ("989", BaseClass.Warrior, Gender.Male), new[]
                        {
                            "PureMaleWarriorGMHelm"
                        }
                    },
                    {
                        ("989", BaseClass.Warrior, Gender.Female), new[]
                        {
                            "PureFemaleWarriorGMHelm"
                        }
                    },
                    {
                        ("989", BaseClass.Priest, Gender.Male), new[]
                        {
                            "DivineStaff"
                        }
                    },
                    {
                        ("989", BaseClass.Priest, Gender.Female), new[]
                        {
                            "DivineStaff"
                        }
                    },
                    {
                        ("989", BaseClass.Monk, Gender.Male), new[]
                        {
                            "PureMaleGMMonkArmor"
                        }
                    },
                    {
                        ("989", BaseClass.Monk, Gender.Female), new[]
                        {
                            "PureFemaleGMMonkArmor"
                        }
                    },
                    {
                        ("989", BaseClass.Rogue, Gender.Male), new[]
                        {
                            "PureGMShadowMask"
                        }
                    },
                    {
                        ("989", BaseClass.Rogue, Gender.Female), new[]
                        {
                            "PureGMShadowHood"
                        }
                    },

                    {
                        ("6599", BaseClass.Warrior, Gender.Male), new[]
                        {
                            "PureMaleGMWarArmor"
                        }
                    },
                    {
                        ("6599", BaseClass.Warrior, Gender.Female), new[]
                        {
                            "PureFemaleGMWarArmor"
                        }
                    },
                    {
                        ("6599", BaseClass.Wizard, Gender.Male), new[]
                        {
                            "mysticgreaves"
                        }
                    },
                    {
                        ("6599", BaseClass.Wizard, Gender.Female), new[]
                        {
                            "mysticgreaves"
                        }
                    },
                    {
                        ("6599", BaseClass.Monk, Gender.Male), new[]
                        {
                            "PureMaleGMDugon"
                        }
                    },
                    {
                        ("6599", BaseClass.Monk, Gender.Female), new[]
                        {
                            "PureFemaleGMDugon"
                        }
                    },
                    {
                        ("6599", BaseClass.Rogue, Gender.Male), new[]
                        {
                            "dragonfang"
                        }
                    },
                    {
                        ("6599", BaseClass.Rogue, Gender.Female), new[]
                        {
                            "dragonfang"
                        }
                    },

                    {
                        ("31010", BaseClass.Priest, Gender.Male), new[]
                        {
                            "PureGMDivineMitre"
                        }
                    },
                    {
                        ("31010", BaseClass.Priest, Gender.Female), new[]
                        {
                            "PureGMDivineBand"
                        }
                    },
                    {
                        ("31010", BaseClass.Wizard, Gender.Male), new[]
                        {
                            "PureMaleGMWizardHelm"
                        }
                    },
                    {
                        ("31010", BaseClass.Wizard, Gender.Female), new[]
                        {
                            "PureFemaleGMWizardHelm"
                        }
                    },
                    {
                        ("31010", BaseClass.Monk, Gender.Male), new[]
                        {
                            "obsidian"
                        }
                    },
                    {
                        ("31010", BaseClass.Monk, Gender.Female), new[]
                        {
                            "obsidian"
                        }
                    },
                    {
                        ("31010", BaseClass.Rogue, Gender.Male), new[]
                        {
                            "PureMaleGMRogueArmor"
                        }
                    },
                    {
                        ("31010", BaseClass.Rogue, Gender.Female), new[]
                        {
                            "PureFemaleGMRogueArmor"
                        }
                    }
                };

                var subGmGearDictionary = new Dictionary<(string mapTemplateKey, BaseClass baseClass, Gender gender), string[]>
                {
                    {
                        ("19522", BaseClass.Warrior, Gender.Male), new[]
                        {
                            "dawnreaverbattleaxe"
                        }
                    },
                    {
                        ("19522", BaseClass.Warrior, Gender.Female), new[]
                        {
                            "dawnreaverbattleaxe"
                        }
                    },
                    {
                        ("19522", BaseClass.Priest, Gender.Male), new[]
                        {
                            "SubMaleGMPriestArmor"
                        }
                    },
                    {
                        ("19522", BaseClass.Priest, Gender.Female), new[]
                        {
                            "SubFemaleGMPriestArmor"
                        }
                    },
                    {
                        ("19522", BaseClass.Wizard, Gender.Male), new[]
                        {
                            "SubMaleGMWizardArmor"
                        }
                    },
                    {
                        ("19522", BaseClass.Wizard, Gender.Female), new[]
                        {
                            "SubFemaleGMWizardArmor"
                        }
                    },

                    {
                        ("989", BaseClass.Warrior, Gender.Male), new[]
                        {
                            "SubMaleWarriorGMHelm"
                        }
                    },
                    {
                        ("989", BaseClass.Warrior, Gender.Female), new[]
                        {
                            "SubFemaleWarriorGMHelm"
                        }
                    },
                    {
                        ("989", BaseClass.Priest, Gender.Male), new[]
                        {
                            "CelestialStaff"
                        }
                    },
                    {
                        ("989", BaseClass.Priest, Gender.Female), new[]
                        {
                            "CelestialStaff"
                        }
                    },
                    {
                        ("989", BaseClass.Monk, Gender.Male), new[]
                        {
                            "SubMaleGMMonkArmor"
                        }
                    },
                    {
                        ("989", BaseClass.Monk, Gender.Female), new[]
                        {
                            "SubFemaleGMMonkArmor"
                        }
                    },
                    {
                        ("989", BaseClass.Rogue, Gender.Male), new[]
                        {
                            "SubGMShadowMask"
                        }
                    },
                    {
                        ("989", BaseClass.Rogue, Gender.Female), new[]
                        {
                            "SubGMShadowHood"
                        }
                    },

                    {
                        ("6599", BaseClass.Warrior, Gender.Male), new[]
                        {
                            "SubMaleGMWarArmor"
                        }
                    },
                    {
                        ("6599", BaseClass.Warrior, Gender.Female), new[]
                        {
                            "SubFemaleGMWarArmor"
                        }
                    },
                    {
                        ("6599", BaseClass.Wizard, Gender.Male), new[]
                        {
                            "mysticgreaves"
                        }
                    },
                    {
                        ("6599", BaseClass.Wizard, Gender.Female), new[]
                        {
                            "mysticgreaves"
                        }
                    },
                    {
                        ("6599", BaseClass.Monk, Gender.Male), new[]
                        {
                            "SubMaleGMDugon"
                        }
                    },
                    {
                        ("6599", BaseClass.Monk, Gender.Female), new[]
                        {
                            "SubFemaleGMDugon"
                        }
                    },
                    {
                        ("6599", BaseClass.Rogue, Gender.Male), new[]
                        {
                            "skean"
                        }
                    },
                    {
                        ("6599", BaseClass.Rogue, Gender.Female), new[]
                        {
                            "skean"
                        }
                    },

                    {
                        ("31010", BaseClass.Priest, Gender.Male), new[]
                        {
                            "SubGMCelestialMitre"
                        }
                    },
                    {
                        ("31010", BaseClass.Priest, Gender.Female), new[]
                        {
                            "SubGMCelestialBand"
                        }
                    },
                    {
                        ("31010", BaseClass.Wizard, Gender.Male), new[]
                        {
                            "SubMaleGMWizardHelm"
                        }
                    },
                    {
                        ("31010", BaseClass.Wizard, Gender.Female), new[]
                        {
                            "SubFemaleGMWizardHelm"
                        }
                    },
                    {
                        ("31010", BaseClass.Monk, Gender.Male), new[]
                        {
                            "tilianclaw"
                        }
                    },
                    {
                        ("31010", BaseClass.Monk, Gender.Female), new[]
                        {
                            "tilianclaw"
                        }
                    },
                    {
                        ("31010", BaseClass.Rogue, Gender.Male), new[]
                        {
                            "SubMaleGMRogueArmor"
                        }
                    },
                    {
                        ("31010", BaseClass.Rogue, Gender.Female), new[]
                        {
                            "SubFemaleGMRogueArmor"
                        }
                    }
                };

                // Example: Retrieve the map's template key
                var mapTemplateKey = source.MapInstance.Template.TemplateKey;
                var baseClass = source.UserStatSheet.BaseClass;
                var gender = source.Gender;

                if (source.IsPureMonkMaster()
                    || source.IsPurePriestMaster()
                    || source.IsPureWizardMaster()
                    || source.IsPureRogueMaster()
                    || source.IsPureWarriorMaster())
                {
                    if (pureGmGearDictionary.TryGetValue((mapTemplateKey, baseClass, gender), out var purerewards))

                        // Grant rewards to the player
                        foreach (var reward in purerewards)
                        {
                            var item = ItemFactory.Create(reward);
                            source.GiveItemOrSendToBank(item);
                            source.SendOrangeBarMessage($"You received the {item.DisplayName}!");
                        }
                } else if (subGmGearDictionary.TryGetValue((mapTemplateKey, baseClass, gender), out var rewards))

                    // Grant rewards to the player
                    foreach (var reward in rewards)
                    {
                        var item = ItemFactory.Create(reward);
                        source.GiveItemOrSendToBank(item);
                        source.SendOrangeBarMessage($"You received the {item.DisplayName}!");
                    }

                var animation = new Animation
                {
                    TargetAnimation = 52,
                    AnimationSpeed = 200
                };

                source.Say("Air a seuladh gu bràth, gus deireadh nan làithean.");
                source.SendOrangeBarMessage("Forever sealed, until the end of days.");
                source.Animate(animation);

                switch (mapTemplateKey)
                {
                    case "6599":

                        if (source.HasClass(BaseClass.Priest))
                        {
                            if (source.Trackers.Enums.HasValue(MasterPriestPath.Light))
                            {
                                var spell = SpellFactory.Create("aosith");
                                source.SpellBook.TryAddToNextSlot(spell);
                            }

                            if (source.Trackers.Enums.HasValue(MasterPriestPath.Dark))
                            {
                                var spell = SpellFactory.Create("shadowtouch");
                                source.SpellBook.TryAddToNextSlot(spell);
                            }
                        }

                        source.Legend.AddUnique(
                            new LegendMark(
                                "Sealed away the Medusa Creant",
                                "medusa",
                                MarkIcon.Heart,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));

                        source.Trackers.Flags.RemoveFlag(CreantEnums.KilledMedusa);
                        source.Trackers.Flags.AddFlag(CreantEnums.CompletedMedusa);

                        if (source.Trackers.Flags.HasFlag(CreantEnums.CompletedTauren)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedSham)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedMedusa)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedPhoenix))
                        {
                            source.Trackers.Enums.Set(MainstoryMasterEnums.KilledCreants);
                            source.SendOrangeBarMessage("You've sealed away all Creants. Speak to Miraelis.");
                        }

                        break;
                    case "31010":

                        if (source.HasClass(BaseClass.Warrior))
                        {
                            if (source.IsPureWarriorMaster())
                            {
                                var skill1 = SkillFactory.Create("devour");
                                source.SkillBook.TryAddToNextSlot(skill1);
                                var skill2 = SkillFactory.Create("paralyzeforce");
                                source.SkillBook.TryAddToNextSlot(skill2);
                                var spell = SpellFactory.Create("inferno");
                                source.SpellBook.TryAddToNextSlot(spell);

                                if (source.SpellBook.ContainsByTemplateKey("wrath"))
                                    source.SpellBook.RemoveByTemplateKey("wrath");

                                if (source.SpellBook.ContainsByTemplateKey("whirlwind"))
                                    source.SpellBook.RemoveByTemplateKey("whirlwind");
                            } else
                            {
                                var skill1 = SkillFactory.Create("devour");
                                source.SkillBook.TryAddToNextSlot(skill1);
                                var skill2 = SkillFactory.Create("groundshattering");
                                source.SkillBook.TryAddToNextSlot(skill2);
                            }

                            if (source.SkillBook.ContainsByTemplateKey("groundstomp"))
                                source.SkillBook.RemoveByTemplateKey("groundstomp");

                            source.SendOrangeBarMessage("You learned new abilities!");
                        }

                        source.Legend.AddUnique(
                            new LegendMark(
                                "Sealed away the Shamensyth Creant",
                                "shamensyth",
                                MarkIcon.Heart,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));

                        source.Trackers.Flags.RemoveFlag(CreantEnums.KilledSham);
                        source.Trackers.Flags.AddFlag(CreantEnums.CompletedSham);

                        if (source.Trackers.Flags.HasFlag(CreantEnums.CompletedTauren)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedSham)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedMedusa)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedPhoenix))
                        {
                            source.Trackers.Enums.Set(MainstoryMasterEnums.KilledCreants);
                            source.SendOrangeBarMessage("You've sealed away all Creants. Speak to Miraelis.");
                        }

                        break;
                    case "989":

                        if (source.HasClass(BaseClass.Wizard))
                        {
                            if (source.IsPureWizardMaster())
                            {
                                var spell = SpellFactory.Create("diacradh");
                                source.SpellBook.TryAddToNextSlot(spell);
                                var spell2 = SpellFactory.Create("meteor");
                                source.SpellBook.TryAddToNextSlot(spell2);
                            } else
                            {
                                var spell = SpellFactory.Create("meteor");
                                source.SpellBook.TryAddToNextSlot(spell);
                            }
                        }

                        source.Legend.AddUnique(
                            new LegendMark(
                                "Sealed away the Lady Phoenix Creant",
                                "phoenix",
                                MarkIcon.Heart,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));

                        source.Trackers.Flags.RemoveFlag(CreantEnums.KilledPhoenix);
                        source.Trackers.Flags.AddFlag(CreantEnums.CompletedPhoenix);

                        if (source.Trackers.Flags.HasFlag(CreantEnums.CompletedTauren)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedSham)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedMedusa)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedPhoenix))
                        {
                            source.Trackers.Enums.Set(MainstoryMasterEnums.KilledCreants);
                            source.SendOrangeBarMessage("You've sealed away all Creants. Speak to Miraelis.");
                        }

                        break;
                    case "19522":

                        if (source.HasClass(BaseClass.Rogue))
                        {
                            if (source.IsPureRogueMaster())
                            {
                                var skill1 = SkillFactory.Create("rupture");
                                source.SkillBook.TryAddToNextSlot(skill1);
                                var spell = SpellFactory.Create("cunning");
                                source.SpellBook.TryAddToNextSlot(spell);
                            } else
                            {
                                var spell = SpellFactory.Create("cunning");
                                source.SpellBook.TryAddToNextSlot(spell);
                            }

                            source.SendOrangeBarMessage("You learned new abilities!");
                        }

                        if (source.HasClass(BaseClass.Monk))
                        {
                            if (source.IsPureMonkMaster())
                            {
                                var skill1 = SkillFactory.Create("soulfultouch");
                                source.SkillBook.TryAddToNextSlot(skill1);
                                var spell = SpellFactory.Create("evasion");
                                source.SpellBook.TryAddToNextSlot(spell);

                                if (source.SpellBook.ContainsByTemplateKey("dodge"))
                                    source.SpellBook.RemoveByTemplateKey("dodge");
                            } else
                            {
                                var skill1 = SkillFactory.Create("soulfultouch");
                                source.SkillBook.TryAddToNextSlot(skill1);
                            }

                            source.SendOrangeBarMessage("You learned new abilities!");
                        }

                        source.Legend.AddUnique(
                            new LegendMark(
                                "Sealed away the Tauren Creant",
                                "tauren",
                                MarkIcon.Heart,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));

                        source.Trackers.Flags.RemoveFlag(CreantEnums.KilledTauren);
                        source.Trackers.Flags.AddFlag(CreantEnums.CompletedTauren);

                        if (source.Trackers.Flags.HasFlag(CreantEnums.CompletedTauren)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedSham)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedMedusa)
                            && source.Trackers.Flags.HasFlag(CreantEnums.CompletedPhoenix))
                        {
                            source.Trackers.Enums.Set(MainstoryMasterEnums.KilledCreants);
                            source.SendOrangeBarMessage("You've sealed away all Creants. Speak to Miraelis.");
                        }

                        break;
                }

                break;

            case "creant_return":
            {
                var mapTemplateKey2 = source.MapInstance.Template.TemplateKey;

                switch (mapTemplateKey2)
                {
                    case "6599":
                        var point1 = new Point(13, 17);
                        var medusaMapInstance = SimpleCache.Get<MapInstance>("oren_ruins_altar");
                        source.TraverseMap(medusaMapInstance, point1);
                        Subject.Close(source);

                        break;
                    case "31010":
                        var point2 = new Point(12, 17);
                        var shamMapInstance = SimpleCache.Get<MapInstance>("nobis_maze1-9");
                        source.TraverseMap(shamMapInstance, point2);
                        Subject.Close(source);

                        break;
                    case "989":
                        var point3 = new Point(5, 5);
                        var phoenixMapInstance = SimpleCache.Get<MapInstance>("shinewood_forest18");
                        source.TraverseMap(phoenixMapInstance, point3);
                        Subject.Close(source);

                        break;
                    case "19522":
                        var point4 = new Point(8, 10);
                        var taurenMapInstance = SimpleCache.Get<MapInstance>("giragan_cave13");
                        source.TraverseMap(taurenMapInstance, point4);
                        Subject.Close(source);

                        break;
                }

                break;
            }
        }
    }
}