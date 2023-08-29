using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Professions;

public class AcceptHobbyorCraftScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;

    public AcceptHobbyorCraftScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        source.Trackers.Flags.TryGetFlag(out Hobbies _);
        var hasCraft = source.Trackers.Enums.TryGetValue(out Crafts craft);
        var hasPenta = source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "kamel_initial":
            {
                if (!source.Trackers.Flags.HasFlag(Hobbies.Fishing))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "fishing_starthobby",
                        OptionText = "I love to fish."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }
            }

                break;
            case "fishing_accepthobby":
            {
                source.Trackers.Flags.AddFlag(Hobbies.Fishing);
                source.SendOrangeBarMessage("You are now a fisherman! Grab a pole and some bait from Kamel.");

                return;
            }
            case "gendusa_initial":
            {
                if (!source.Trackers.Flags.HasFlag(Hobbies.Cooking))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cooking_starthobby",
                        OptionText = "I am ready to be a Chef."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }
            }

                break;
            case "cooking_accepthobby":
            {
                source.Trackers.Flags.AddFlag(Hobbies.Cooking);
                source.Trackers.Flags.AddFlag(CookingRecipes.DinnerPlate);
                source.Trackers.Flags.AddFlag(CookingRecipes.FruitBasket);

                return;
            }
            case "thorin_initial":
            {
                if ((hasCraft && (craft != Crafts.Jewelcrafting)) && (stage != PentagramQuestStage.FoundPentagramPiece) && (stage != PentagramQuestStage.EmpoweringPentagramPiece))
                {
                    Subject.Reply(
                        source,
                        "You aren't a Weaponsmith, get out of my forge. I cannot have Aislings wandering around who don't know what they're doing. You could get hurt. Go see Riona to learn about crafts if you're curious.");

                    return;
                }

                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "weaponsmithing_startcraft",
                        OptionText = "I want to be a Weaponsmith."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);

                    return;
                }

                if (hasCraft && (craft == Crafts.Weaponsmithing))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be a Weaponsmith."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;
            }

            case "weaponsmithing_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Weaponsmithing);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Eppe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Saber);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.DullClaw);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SnowDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.CenterDagger);
                source.Trackers.Flags.AddFlag(RionaTutorialQuestFlags.Crafting);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Weaponsmith",
                        "wpnsmth",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                source.SendOrangeBarMessage("You are now a Beginner Weaponsmith!");

                return;
            }

            case "gwendolyn_initial":
            {
                if (hasCraft && (craft != Crafts.Armorsmithing))
                {
                    Subject.Reply(
                        source,
                        "Why are you here Aisling? You are not an Armorsmith, you could get hurt around my equipment here. Please, go see Riona if you want to learn about crafting.");

                    return;
                }

                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "armorsmith_startcraft",
                        OptionText = "I want to be an Armorsmith."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                if (hasCraft && (craft == Crafts.Armorsmithing))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be an Armorsmith."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;
            }

            case "armorsmith_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Armorsmithing);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherGauntlet);
                source.Trackers.Flags.AddFlag(RionaTutorialQuestFlags.Crafting);
                var book = ItemFactory.Create("recipe_basicarmors");
                source.TryGiveItem(ref book);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Armorsmith",
                        "armsmth",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                source.SendOrangeBarMessage("You are now an Armorsmith!");

                return;
            }
            case "mathis_initial":
            {
                if (hasCraft && (craft != Crafts.Alchemy))
                {
                    Subject.Reply(
                        source,
                        "You don't belong in my lab. This is a very dangerous area for those who don't know what they're doing. Go see Riona if you want to learn about crafting.");

                    return;
                }

                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "alchemy_startcraft",
                        OptionText = "I'd like to be an Alchemist."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                if (hasCraft && (craft == Crafts.Alchemy))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be an Alchemist."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;
            }

            case "alchemy_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Alchemy);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallHealthPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallManaPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallHasteBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallPowerBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallAccuracyPotion);
                source.Trackers.Flags.AddFlag(RionaTutorialQuestFlags.Crafting);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Alchemist",
                        "alch",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                source.SendOrangeBarMessage("You are now an Alchemist!");

                return;
            }
            case "elara_initial":
            {
                if (hasCraft && (craft != Crafts.Enchanting))
                {
                    Subject.Reply(
                        source,
                        "Shhh, don't bothe... Wait, what are you doing here? You don't know the first thing about enchanting. Get out, you're going to bother others. Go see Riona.");

                    return;
                }

                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "enchanting_startcraft",
                        OptionText = "Could you teach me Enchanting?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                if (hasCraft && (craft == Crafts.Enchanting))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be an Enchanter."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;
            }

            case "enchanting_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Enchanting);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithGratitude);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarEnvy);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisSerenity);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.TheseleneElusion);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.AquaedonClarity);
                source.Trackers.Flags.AddFlag(RionaTutorialQuestFlags.Crafting);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Enchanter",
                        "ench",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                source.SendOrangeBarMessage("You are now an Enchanter!");

                return;
            }
            case "celestia_initial":
            {
                if ((hasCraft && (craft != Crafts.Jewelcrafting)) && (stage != PentagramQuestStage.FoundPentagramPiece) && (stage != PentagramQuestStage.EmpoweringPentagramPiece))
                {
                    Subject.Reply(
                        source,
                        "The art of Jewelcrafting is delicate and precious. You are a distraction to those who are working. If you want to learn about crafting, go see Riona.");

                    return;
                }

                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "jewelcrafting_startcraft",
                        OptionText = "I'd like to be a Jewelcrafter."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);

                    return;
                }

                if (hasCraft && (craft == Crafts.Jewelcrafting))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be a Jewelcrafter."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;
            }

            case "jewelcrafting_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Jewelcrafting);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.SmallRubyRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BerylRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.EarthNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.SeaNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.FireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.WindNecklace);
                source.Trackers.Flags.AddFlag(RionaTutorialQuestFlags.Crafting);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Jewelcrafter",
                        "jwlcrftng",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                source.SendOrangeBarMessage("You are now a Jewelcrafter!");

                return;
            }
            case "craft_remove3":
            {
                if (hasCraft && (craft == Crafts.Armorsmithing))
                {
                    source.SendOrangeBarMessage("You are no longer an Armorsmith.");
                    source.Legend.Remove("armsmth", out _);
                }

                if (hasCraft && (craft == Crafts.Weaponsmithing))
                {
                    source.SendOrangeBarMessage("You are no longer a Weaponsmith.");
                    source.Legend.Remove("wpnsmth", out _);
                }

                if (hasCraft && (craft == Crafts.Jewelcrafting))
                {
                    source.SendOrangeBarMessage("You are no longer a Jewelcrafter.");
                    source.Legend.Remove("jwlcrftng", out _);
                }

                if (hasCraft && (craft == Crafts.Alchemy))
                {
                    source.SendOrangeBarMessage("You are no longer an Alchemist.");
                    source.Legend.Remove("alch", out _);
                }

                if (hasCraft && (craft == Crafts.Enchanting))
                {
                    source.SendOrangeBarMessage("You are no longer an Enchanter.");
                    source.Legend.Remove("ench", out _);
                }

                source.Trackers.Enums.Remove<Crafts>();

                return;
            }
        }
    }
}