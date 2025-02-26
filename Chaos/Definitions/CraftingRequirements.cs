using Chaos.Extensions;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Enchantments;

namespace Chaos.Definitions;

public static class CraftingRequirements
{
    #region Alchemy Recipes
    public static Dictionary<AlchemyRecipes, Recipe> AlchemyRequirements { get; } = new()
    {
        {
            AlchemyRecipes.Hemloch, new Recipe
            {
                Name = "Hemloch Formula",
                TemplateKey = "hemlochformula",
                Ingredients =
                [
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 2),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            AlchemyRecipes.SmallHealthPotion, new Recipe
            {
                Name = "Small Health Potion Formula",
                TemplateKey = "smallhealthpotionformula",
                Ingredients =
                [
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallManaPotion, new Recipe
            {
                Name = "Small Mana Potion Formula",
                TemplateKey = "smallmanapotionformula",
                Ingredients =
                [
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallRejuvenationPotion, new Recipe
            {
                Name = "Small Rejuvenation Potion Formula",
                TemplateKey = "smallrejuvenationpotionformula",
                Ingredients =
                [
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallHasteBrew, new Recipe
            {
                Name = "Small Haste Brew Formula",
                TemplateKey = "smallhastebrewformula",
                Ingredients =
                [
                    new Ingredient("sparkflower", "Spark Flower", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallPowerBrew, new Recipe
            {
                Name = "Small Power Brew Formula",
                TemplateKey = "smallpowerbrewformula",
                Ingredients =
                [
                    new Ingredient("cactusflower", "Cactus Flower", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallAccuracyPotion, new Recipe
            {
                Name = "Small Accuracy Potion Formula",
                TemplateKey = "smallaccuracypotionformula",
                Ingredients =
                [
                    new Ingredient("kabineblossom", "Kabine Blossom", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.JuggernautBrew, new Recipe
            {
                Name = "Juggernaut Brew Formula",
                TemplateKey = "juggernautbrewformula",
                Ingredients =
                [
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 2),
                    new Ingredient("koboldtail", "Kobold Tail", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            AlchemyRecipes.AstralBrew, new Recipe
            {
                Name = "Astral Brew Formula",
                TemplateKey = "astralbrewformula",
                Ingredients =
                [
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 2),
                    new Ingredient("koboldtail", "Kobold Tail", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            AlchemyRecipes.AntidotePotion, new Recipe
            {
                Name = "Antidote Potion Formula",
                TemplateKey = "antidotepotionformula",
                Ingredients =
                [
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 2),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            AlchemyRecipes.SmallFirestormTonic, new Recipe
            {
                Name = "Small Firestorm Tonic Formula",
                TemplateKey = "smallfirestormtonicformula",
                Ingredients =
                [
                    new Ingredient("cactusflower", "Cactus Flower", 1),
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 2),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            AlchemyRecipes.SmallStunTonic, new Recipe
            {
                Name = "Small Stun Tonic Formula",
                TemplateKey = "smallstuntonicformula",
                Ingredients =
                [
                    new Ingredient("dochasbloom", "Dochas Bloom", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 2)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            AlchemyRecipes.HealthPotion, new Recipe
            {
                Name = "Health Potion Formula",
                TemplateKey = "healthpotionformula",
                Ingredients =
                [
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 3)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            AlchemyRecipes.ManaPotion, new Recipe
            {
                Name = "Mana Potion Formula",
                TemplateKey = "manapotionformula",
                Ingredients =
                [
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 3),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            AlchemyRecipes.RejuvenationPotion, new Recipe
            {
                Name = "Rejuvenation Potion Formula",
                TemplateKey = "rejuvenationpotionformula",
                Ingredients =
                [
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 3)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            AlchemyRecipes.HasteBrew, new Recipe
            {
                Name = "Haste Brew Formula",
                TemplateKey = "hastebrewformula",
                Ingredients =
                [
                    new Ingredient("sparkflower", "Spark Flower", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("basicmonsterextract", "Basic Monster Extract", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            AlchemyRecipes.PowerBrew, new Recipe
            {
                Name = "Power Brew Formula",
                TemplateKey = "powerbrewformula",
                Ingredients =
                [
                    new Ingredient("cactusflower", "Cactus Flower", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("basicmonsterextract", "Basic Monster Extract", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            AlchemyRecipes.AccuracyPotion, new Recipe
            {
                Name = "Accuracy Potion Formula",
                TemplateKey = "accuracypotionformula",
                Ingredients =
                [
                    new Ingredient("kabineblossom", "Kabine Blossom", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("basicmonsterextract", "Basic Monster Extract", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            AlchemyRecipes.RevivePotion, new Recipe
            {
                Name = "Revive Potion Formula",
                TemplateKey = "revivepotionformula",
                Ingredients =
                [
                    new Ingredient("BlossomofBetrayal", "Blossom of Betrayal", 1),
                    new Ingredient("greatermonsterextract", "Greater Monster Extract", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            AlchemyRecipes.StrongJuggernautBrew, new Recipe
            {
                Name = "Strong Juggernaut Brew Formula",
                TemplateKey = "strongjuggernautbrewformula",
                Ingredients =
                [
                    new Ingredient("greatermonsterextract", "Greater Monster Extract", 1),
                    new Ingredient("koboldtail", "Kobold Tail", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            AlchemyRecipes.StrongAstralBrew, new Recipe
            {
                Name = "Strong Astral Brew Formula",
                TemplateKey = "strongAstralbrewformula",
                Ingredients =
                [
                    new Ingredient("greatermonsterextract", "Greater Monster Extract", 1),
                    new Ingredient("koboldtail", "Kobold Tail", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            AlchemyRecipes.CleansingBrew, new Recipe
            {
                Name = "Cleansing Brew Formula",
                TemplateKey = "cleansingbrewformula",
                Ingredients =
                [
                    new Ingredient("waterlily", "Water Lily", 1),
                    new Ingredient("greatermonsterextract", "Greater Monster Extract", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            AlchemyRecipes.FirestormTonic, new Recipe
            {
                Name = "Firestorm Tonic Formula",
                TemplateKey = "firestormtonicformula",
                Ingredients =
                [
                    new Ingredient("cactusflower", "Cactus Flower", 1),
                    new Ingredient("greatermonsterextract", "Greater Monster Extract", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            AlchemyRecipes.StunTonic, new Recipe
            {
                Name = "Stun Tonic Formula",
                TemplateKey = "stuntonicformula",
                Ingredients =
                [
                    new Ingredient("dochasbloom", "Dochas Bloom", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("greatermonsterextract", "Greater Monster Extract", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            AlchemyRecipes.WarmthPotion, new Recipe
            {
                Name = "Warmth Potion Formula",
                TemplateKey = "warmthpotionformula",
                Ingredients =
                [
                    new Ingredient("bocanbough", "Bocan Bough", 2),
                    new Ingredient("greatermonsterextract", "Greater Monster Extract", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            AlchemyRecipes.AmnesiaBrew, new Recipe
            {
                Name = "Amnesia Brew Formula",
                TemplateKey = "amnesiaBrewformula",
                Ingredients =
                [
                    new Ingredient("blossomofbetrayal", "Blossom of Betrayal", 1),
                    new Ingredient("greatermonsterextract", "Greater Monster Extract", 1),
                    new Ingredient("Empty Bottle", "Empty Bottle", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            AlchemyRecipes.StrongHealthPotion, new Recipe
            {
                Name = "Strong Health Potion Formula",
                TemplateKey = "stronghealthpotionformula",
                Ingredients =
                [
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 5)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            AlchemyRecipes.StrongManaPotion, new Recipe
            {
                Name = "Strong Mana Potion Formula",
                TemplateKey = "strongmanapotionformula",
                Ingredients =
                [
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 5),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            AlchemyRecipes.StrongRejuvenationPotion, new Recipe
            {
                Name = "Strong Rejuvenation Potion Formula",
                TemplateKey = "strongrejuvenationpotionformula",
                Ingredients =
                [
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 5)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            AlchemyRecipes.StrongHasteBrew, new Recipe
            {
                Name = "Strong Haste Brew Formula",
                TemplateKey = "stronghastebrewformula",
                Ingredients =
                [
                    new Ingredient("sparkflower", "Spark Flower", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 1)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            AlchemyRecipes.StrongPowerBrew, new Recipe
            {
                Name = "Strong Power Brew Formula",
                TemplateKey = "strongpowerbrewformula",
                Ingredients =
                [
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 1),
                    new Ingredient("cactusflower", "Cactus Flower", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            AlchemyRecipes.StrongAccuracyPotion, new Recipe
            {
                Name = "Strong Accuracy Potion Formula",
                TemplateKey = "strongaccuracypotionformula",
                Ingredients =
                [
                    new Ingredient("kabineblossom", "Kabine Blossom", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            AlchemyRecipes.StatBoostElixir, new Recipe
            {
                Name = "Stat Boost Elixir Formula",
                TemplateKey = "statboostelixirformula",
                Ingredients =
                [
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 1),
                    new Ingredient("marauderspine", "Marauder's Spine", 1),
                    new Ingredient("satyrhoof", "Satyr's Hoof", 1),
                    new Ingredient("polypsac", "Polyp Sac", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            AlchemyRecipes.KnowledgeElixir, new Recipe
            {
                Name = "Knowledge Elixir Formula",
                TemplateKey = "knowledgeelixirformula",
                Ingredients =
                [
                    new Ingredient("greatermonsterextract", "Greater Monster Extract", 1),
                    new Ingredient("lionfish", "Lion Fish", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.PotentHealthPotion, new Recipe
            {
                Name = "Potent Health Potion Formula",
                TemplateKey = "potenthealthpotionformula",
                Ingredients =
                [
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 8)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.PotentManaPotion, new Recipe
            {
                Name = "Potent Mana Potion Formula",
                TemplateKey = "potentmanapotionformula",
                Ingredients =
                [
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 8)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.PotentRejuvenationPotion, new Recipe
            {
                Name = "Potent Rejuvenation Potion Formula",
                TemplateKey = "potentrejuvenationpotionformula",
                Ingredients =
                [
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("lessermonsterextract", "Lesser Monster Extract", 8)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.PotentHasteBrew, new Recipe
            {
                Name = "Potent Haste Brew Formula",
                TemplateKey = "potenthastebrewformula",
                Ingredients =
                [
                    new Ingredient("sparkflower", "Spark Flower", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 2)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.PotentPowerBrew, new Recipe
            {
                Name = "Potent Power Brew Formula",
                TemplateKey = "potentpowerbrewformula",
                Ingredients =
                [
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 2),
                    new Ingredient("cactusflower", "Cactus Flower", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.PotentAccuracyPotion, new Recipe
            {
                Name = "Potent Accuracy Potion Formula",
                TemplateKey = "potentaccuracypotionformula",
                Ingredients =
                [
                    new Ingredient("kabineblossom", "Kabine Blossom", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 2),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.PotentJuggernautBrew, new Recipe
            {
                Name = "Potent Juggernaut Brew Formula",
                TemplateKey = "potentjuggernautbrewformula",
                Ingredients =
                [
                    new Ingredient("blossomofbetrayal", "Blossom of Betrayal", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 3),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.PotentAstralBrew, new Recipe
            {
                Name = "Potent Astral Brew Formula",
                TemplateKey = "potentastralbrewformula",
                Ingredients =
                [
                    new Ingredient("passionflower", "Passion Flower", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 3),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.InvisibilityPotion, new Recipe
            {
                Name = "Invisibility Potion Formula",
                TemplateKey = "invisibilitypotionformula",
                Ingredients =
                [
                    new Ingredient("koboldtail", "Kobold Tail", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 3),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.PoisonImmunityElixir, new Recipe
            {
                Name = "Poison Immunity Elixir Formula",
                TemplateKey = "poisonimmunityelixirformula",
                Ingredients =
                [
                    new Ingredient("raineach", "Raineach", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            AlchemyRecipes.PotionOfStrength, new Recipe
            {
                Name = "Potion of Strength Formula",
                TemplateKey = "potionofstrengthformula",
                Ingredients =
                [
                    new Ingredient("cactusflower", "Cactus Flower", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 2),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            AlchemyRecipes.PotionOfIntellect, new Recipe
            {
                Name = "Potion of Intellect Formula",
                TemplateKey = "potionofintellectformula",
                Ingredients =
                [
                    new Ingredient("blossomofbetrayal", "Blossom of Betrayal", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 2),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            AlchemyRecipes.PotionOfWisdom, new Recipe
            {
                Name = "Potion of Wisdom Formula",
                TemplateKey = "potionofwisdomformula",
                Ingredients =
                [
                    new Ingredient("lilypad", "Lily Pad", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 2),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            AlchemyRecipes.PotionOfConstitution, new Recipe
            {
                Name = "Potion of Constitution Formula",
                TemplateKey = "potionofconstitutionformula",
                Ingredients =
                [
                    new Ingredient("bocanbough", "Bocan Bough", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 2),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            AlchemyRecipes.PotionOfDexterity, new Recipe
            {
                Name = "Potion of Dexterity Formula",
                TemplateKey = "potionofdexterityformula",
                Ingredients =
                [
                    new Ingredient("dochasbloom", "Dochas Bloom", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 2),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            AlchemyRecipes.StrongStatBoostElixir, new Recipe
            {
                Name = "Strong Stat Boost Elixir Formula",
                TemplateKey = "strongstatboostelixirformula",
                Ingredients =
                [
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 3),
                    new Ingredient("losganntail", "Losgann Tail", 1),
                    new Ingredient("redshockerpiece", "Red Shocker Piece", 1),
                    new Ingredient("iceskeletonskull", "Ice Skeleton Skull", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            AlchemyRecipes.StrongKnowledgeElixir, new Recipe
            {
                Name = "Strong Knowledge Elixir Formula",
                TemplateKey = "strongknowledgeelixirformula",
                Ingredients =
                [
                    new Ingredient("blackshockerpiece", "Black Shocker Piece", 1),
                    new Ingredient("iceelementalflame", "Ice Elemental Flame", 1),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 2),
                    new Ingredient("rockfish", "Rock Fish", 1),
                    new Ingredient("emptybottle", "Empty Bottle", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 2
            }
        }
    };
    #endregion

    #region Enchanting Recipes
    public static Dictionary<EnchantingRecipes, Recipe> EnchantingRequirements { get; } = new()
    {
        {
            EnchantingRecipes.AquaedonCalming, new Recipe
            {
                Name = "Aquaedon's Calming",
                TemplateKey = "aquaedoncalming",
                Ingredients = [new Ingredient("essenceofaquaedon", "Essence of Aquaedon", 3)],
                Rank = "Basic",
                Prefix = "Serene",
                Level = 40,
                Difficulty = 2,
                Modification = item => item.AddScript<SerenePrefixScript>()
            }
        },
        {
            EnchantingRecipes.AquaedonClarity, new Recipe
            {
                Name = "Aquaedon's Clarity",
                TemplateKey = "aquaedonclarity",
                Ingredients = [new Ingredient("essenceofaquaedon", "Essence of Aquaedon", 1)],
                Rank = "Beginner",
                Prefix = "Meager",
                Level = 5,
                Difficulty = 1,
                Modification = item => item.AddScript<MeagerPrefixScript>()
            }
        },
        {
            EnchantingRecipes.AquaedonResolve, new Recipe
            {
                Name = "Aquaedon's Resolve",
                TemplateKey = "aquaedonresolve",
                Ingredients = [new Ingredient("essenceofaquaedon", "Essence of Aquaedon", 10)],
                Rank = "Adept",
                Prefix = "Soothing",
                Level = 90,
                Difficulty = 5,
                Modification = item => item.AddScript<SoothingPrefixScript>()
            }
        },
        {
            EnchantingRecipes.AquaedonWill, new Recipe
            {
                Name = "Aquaedon's Will",
                TemplateKey = "aquaedonwill",
                Ingredients = [new Ingredient("essenceofaquaedon", "Essence of Aquaedon", 7)],
                Rank = "Artisan",
                Prefix = "Potent",
                Level = 71,
                Difficulty = 4,
                Modification = item => item.AddScript<PotentPrefixScript>()
            }
        },
        {
            EnchantingRecipes.AquaedonWisdom, new Recipe
            {
                Name = "Aquaedon's Wisdom",
                TemplateKey = "aquaedonwisdom",
                Ingredients = [new Ingredient("essenceofaquaedon", "Essence of Aquaedon", 5)],
                Rank = "Initiate",
                Prefix = "Wise",
                Level = 50,
                Difficulty = 3,
                Modification = item => item.AddScript<WisePrefixScript>()
            }
        },
        {
            EnchantingRecipes.IgnatarDestruction, new Recipe
            {
                Name = "Ignatar's Destruction",
                TemplateKey = "ignatardestruction",
                Ingredients = [new Ingredient("essenceofignatar", "Essence of Ignatar", 7)],
                Rank = "Artisan",
                Prefix = "Ruthless",
                Level = 80,
                Difficulty = 4,
                Modification = item => item.AddScript<RuthlessPrefixScript>()
            }
        },
        {
            EnchantingRecipes.IgnatarEnvy, new Recipe
            {
                Name = "Ignatar's Envy",
                TemplateKey = "ignatarenvy",
                Ingredients = [new Ingredient("essenceofignatar", "Essence of Ignatar", 1)],
                Rank = "Beginner",
                Prefix = "Swift",
                Level = 3,
                Difficulty = 1,
                Modification = item => item.AddScript<SwiftPrefixScript>()
            }
        },
        {
            EnchantingRecipes.IgnatarGrief, new Recipe
            {
                Name = "Ignatar's Grief",
                TemplateKey = "ignatargrief",
                Ingredients = [new Ingredient("essenceofignatar", "Essence of Ignatar", 1)],
                Rank = "Basic",
                Prefix = "Minor",
                Level = 24,
                Difficulty = 2,
                Modification = item => item.AddScript<MinorPrefixScript>()
            }
        },
        {
            EnchantingRecipes.IgnatarJealousy, new Recipe
            {
                Name = "Ignatar's Jealousy",
                TemplateKey = "ignatarjealousy",
                Ingredients = [new Ingredient("essenceofignatar", "Essence of Ignatar", 5)],
                Rank = "Initiate",
                Prefix = "Crippling",
                Level = 60,
                Difficulty = 3,
                Modification = item => item.AddScript<CripplingPrefixScript>()
            }
        },
        {
            EnchantingRecipes.IgnatarRegret, new Recipe
            {
                Name = "Ignatar's Regret",
                TemplateKey = "ignatarregret",
                Ingredients = [new Ingredient("essenceofignatar", "Essence of Ignatar", 3)],
                Rank = "Initiate",
                Prefix = "Hasty",
                Level = 48,
                Difficulty = 3,
                Modification = item => item.AddScript<HastyPrefixScript>()
            }
        },
        {
            EnchantingRecipes.GeolithConstitution, new Recipe
            {
                Name = "Geolith's Constitution",
                TemplateKey = "geolithconstitution",
                Ingredients = [new Ingredient("essenceofgeolith", "Essence of Geolith", 5)],
                Rank = "Initiate",
                Prefix = "Hale",
                Level = 50,
                Difficulty = 3,
                Modification = item => item.AddScript<HalePrefixScript>()
            }
        },
        {
            EnchantingRecipes.GeolithFortitude, new Recipe
            {
                Name = "Geolith's Fortitude",
                TemplateKey = "geolithfortitude",
                Ingredients = [new Ingredient("essenceofgeolith", "Essence of Geolith", 7)],
                Rank = "Artisan",
                Prefix = "Eternal",
                Level = 83,
                Difficulty = 4,
                Modification = item => item.AddScript<EternalPrefixScript>()
            }
        },
        {
            EnchantingRecipes.GeolithGratitude, new Recipe
            {
                Name = "Geolith's Gratitude",
                TemplateKey = "geolithgratitude",
                Ingredients = [new Ingredient("essenceofgeolith", "Essence of Geolith", 1)],
                Rank = "Beginner",
                Prefix = "Skillful",
                Level = 3,
                Difficulty = 1,
                Modification = item => item.AddScript<SkillfulPrefixScript>()
            }
        },
        {
            EnchantingRecipes.GeolithPride, new Recipe
            {
                Name = "Geolith's Pride",
                TemplateKey = "geolithpride",
                Ingredients = [new Ingredient("essenceofgeolith", "Essence of Geolith", 3)],
                Rank = "Basic",
                Prefix = "Modest",
                Level = 28,
                Difficulty = 2,
                Modification = item => item.AddScript<ModestPrefixScript>()
            }
        },
        {
            EnchantingRecipes.GeolithObsession, new Recipe
            {
                Name = "Geolith's Obsession",
                TemplateKey = "geolithobsession",
                Ingredients = [new Ingredient("essenceofgeolith", "Essence of Geolith", 5)],
                Rank = "Initiate",
                Prefix = "Powerful",
                Level = 65,
                Difficulty = 3,
                Modification = item => item.AddScript<PowerfulPrefixScript>()
            }
        },
        {
            EnchantingRecipes.MiraelisBlessing, new Recipe
            {
                Name = "Miraelis' Blessing",
                TemplateKey = "miraelisblessing",
                Ingredients = [new Ingredient("essenceofmiraelis", "Essence of Miraelis", 3)],
                Rank = "Basic",
                Prefix = "Tiny",
                Level = 34,
                Difficulty = 2,
                Modification = item => item.AddScript<TinyPrefixScript>()
            }
        },
        {
            EnchantingRecipes.MiraelisHarmony, new Recipe
            {
                Name = "Miraelis' Harmony",
                TemplateKey = "miraelisharmony",
                Ingredients = [new Ingredient("essenceofmiraelis", "Essence of Miraelis", 5)],
                Rank = "Initiate",
                Prefix = "Bright",
                Level = 69,
                Difficulty = 3,
                Modification = item => item.AddScript<BrightPrefixScript>()
            }
        },
        {
            EnchantingRecipes.MiraelisIntellect, new Recipe
            {
                Name = "Miraelis' Intellect",
                TemplateKey = "miraelisintellect",
                Ingredients = [new Ingredient("essenceofmiraelis", "Essence of Miraelis", 5)],
                Rank = "Initiate",
                Prefix = "Brilliant",
                Level = 50,
                Difficulty = 3,
                Modification = item => item.AddScript<BrilliantPrefixScript>()
            }
        },
        {
            EnchantingRecipes.MiraelisNurturing, new Recipe
            {
                Name = "Miraelis' Nurturing",
                TemplateKey = "miraelisnurturing",
                Ingredients = [new Ingredient("essenceofmiraelis", "Essence of Miraelis", 7)],
                Rank = "Artisan",
                Prefix = "Ancient",
                Level = 88,
                Difficulty = 4,
                Modification = item => item.AddScript<AncientPrefixScript>()
            }
        },
        {
            EnchantingRecipes.MiraelisSerenity, new Recipe
            {
                Name = "Miraelis' Serenity",
                TemplateKey = "miraelisserenity",
                Ingredients = [new Ingredient("essenceofmiraelis", "Essence of Miraelis", 1)],
                Rank = "Beginner",
                Prefix = "Mystical",
                Level = 5,
                Difficulty = 1,
                Modification = item => item.AddScript<MysticalPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SerendaelAddiction, new Recipe
            {
                Name = "Serendael's Addiction",
                TemplateKey = "serendaeladdiction",
                Ingredients = [new Ingredient("essenceofserendael", "Essence of Serendael", 10)],
                Rank = "Adept",
                Prefix = "Persisting",
                Level = 90,
                Difficulty = 5,
                Modification = item => item.AddScript<PersistingPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SerendaelChance, new Recipe
            {
                Name = "Serendael's Chance",
                TemplateKey = "serendaelchance",
                Ingredients = [new Ingredient("essenceofserendael", "Essence of Serendael", 5)],
                Rank = "Initiate",
                Prefix = "Focused",
                Level = 55,
                Difficulty = 3,
                Modification = item => item.AddScript<FocusedPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SerendaelLuck, new Recipe
            {
                Name = "Serendael's Luck",
                TemplateKey = "serendaelluck",
                Ingredients = [new Ingredient("essenceofserendael", "Essence of Serendael", 1)],
                Rank = "Basic",
                Prefix = "Lucky",
                Level = 11,
                Difficulty = 2,
                Modification = item => item.AddScript<LuckyPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SerendaelMagic, new Recipe
            {
                Name = "Serendael's Magic",
                TemplateKey = "serendaelmagic",
                Ingredients = [new Ingredient("essenceofserendael", "Essence of Serendael", 3)],
                Rank = "Initiate",
                Prefix = "Airy",
                Level = 41,
                Difficulty = 3,
                Modification = item => item.AddScript<AiryPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SerendaelRoll, new Recipe
            {
                Name = "Serendael's Roll",
                TemplateKey = "serendaelroll",
                Ingredients = [new Ingredient("essenceofserendael", "Essence of Serendael", 7)],
                Rank = "Artisan",
                Prefix = "Precision",
                Level = 71,
                Difficulty = 4,
                Modification = item => item.AddScript<PrecisionPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SkandaraDrive, new Recipe
            {
                Name = "Skandara's Drive",
                TemplateKey = "skandaradrive",
                Ingredients = [new Ingredient("essenceofskandara", "Essence of Skandara", 7)],
                Rank = "Artisan",
                Prefix = "Savage",
                Level = 75,
                Difficulty = 4,
                Modification = item => item.AddScript<SavagePrefixScript>()
            }
        },
        {
            EnchantingRecipes.SkandaraMight, new Recipe
            {
                Name = "Skandara's Might",
                TemplateKey = "skandaramight",
                Ingredients = [new Ingredient("essenceofskandara", "Essence of Skandara", 1)],
                Rank = "Basic",
                Prefix = "Mighty",
                Level = 16,
                Difficulty = 2,
                Modification = item => item.AddScript<MightyPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SkandaraPierce, new Recipe
            {
                Name = "Skandara's Pierce",
                TemplateKey = "skandarapierce",
                Ingredients = [new Ingredient("essenceofskandara", "Essence of Skandara", 10)],
                Rank = "Adept",
                Prefix = "Blazing",
                Level = 95,
                Difficulty = 5,
                Modification = item => item.AddScript<BlazingPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SkandaraStrength, new Recipe
            {
                Name = "Skandara's Strength",
                TemplateKey = "skandarastrength",
                Ingredients = [new Ingredient("essenceofskandara", "Essence of Skandara", 5)],
                Rank = "Initiate",
                Prefix = "Tough",
                Level = 50,
                Difficulty = 3,
                Modification = item => item.AddScript<ToughPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SkandaraTriumph, new Recipe
            {
                Name = "Skandara's Triumph",
                TemplateKey = "skandaratriumph",
                Ingredients = [new Ingredient("essenceofskandara", "Essence of Skandara", 3)],
                Rank = "Initiate",
                Prefix = "Valiant",
                Level = 44,
                Difficulty = 3,
                Modification = item => item.AddScript<ValiantPrefixScript>()
            }
        },
        {
            EnchantingRecipes.TheseleneBalance, new Recipe
            {
                Name = "Theselene's Balance",
                TemplateKey = "theselenebalance",
                Ingredients = [new Ingredient("essenceoftheselene", "Essence of Theselene", 7)],
                Rank = "Artisan",
                Prefix = "Tight",
                Level = 71,
                Difficulty = 4,
                Modification = item => item.AddScript<TightPrefixScript>()
            }
        },
        {
            EnchantingRecipes.TheseleneDexterity, new Recipe
            {
                Name = "Theselene's Dexterity",
                TemplateKey = "theselenedexterity",
                Ingredients = [new Ingredient("essenceoftheselene", "Essence of Theselene", 5)],
                Rank = "Initiate",
                Prefix = "Nimble",
                Level = 50,
                Difficulty = 3,
                Modification = item => item.AddScript<NimblePrefixScript>()
            }
        },
        {
            EnchantingRecipes.TheseleneElusion, new Recipe
            {
                Name = "Theselene's Elusion",
                TemplateKey = "theseleneelusion",
                Ingredients = [new Ingredient("essenceoftheselene", "Essence of Theselene", 1)],
                Rank = "Beginner",
                Prefix = "Shrouded",
                Level = 5,
                Difficulty = 1,
                Modification = item => item.AddScript<ShroudedPrefixScript>()
            }
        },
        {
            EnchantingRecipes.TheseleneRisk, new Recipe
            {
                Name = "Theselene's Risk",
                TemplateKey = "theselenerisk",
                Ingredients = [new Ingredient("essenceoftheselene", "Essence of Theselene", 10)],
                Rank = "Adept",
                Prefix = "Cursed",
                Level = 90,
                Difficulty = 5,
                Modification = item => item.AddScript<CursedPrefixScript>()
            }
        },
        {
            EnchantingRecipes.TheseleneShadow, new Recipe
            {
                Name = "Theselene's Shadow",
                TemplateKey = "theseleneshadow",
                Ingredients = [new Ingredient("essenceoftheselene", "Essence of Theselene", 3)],
                Rank = "Basic",
                Prefix = "Darkened",
                Level = 37,
                Difficulty = 2,
                Modification = item => item.AddScript<DarkenedPrefixScript>()
            }
        },
        {
            EnchantingRecipes.ZephyraGust, new Recipe
            {
                Name = "Zephyra's Gust",
                TemplateKey = "zephyragust",
                Ingredients = [new Ingredient("essenceofzephyra", "Essence of Zephyra", 10)],
                Rank = "Adept",
                Prefix = "Howling",
                Level = 97,
                Difficulty = 5,
                Modification = item => item.AddScript<HowlingPrefixScript>()
            }
        },
        {
            EnchantingRecipes.ZephyraMist, new Recipe
            {
                Name = "Zephyra's Mist",
                TemplateKey = "zephyramist",
                Ingredients = [new Ingredient("essenceofzephyra", "Essence of Zephyra", 3)],
                Rank = "Initiate",
                Prefix = "Soft",
                Level = 45,
                Difficulty = 3,
                Modification = item => item.AddScript<SoftPrefixScript>()
            }
        },
        {
            EnchantingRecipes.ZephyraSpirit, new Recipe
            {
                Name = "Zephyra's Spirit",
                TemplateKey = "zephyraspirit",
                Ingredients = [new Ingredient("essenceofzephyra", "Essence of Zephyra", 1)],
                Rank = "Basic",
                Prefix = "Breezy",
                Level = 20,
                Difficulty = 2,
                Modification = item => item.AddScript<BreezyPrefixScript>()
            }
        },
        {
            EnchantingRecipes.ZephyraVortex, new Recipe
            {
                Name = "Zephyra's Vortex",
                TemplateKey = "zephyravortex",
                Ingredients = [new Ingredient("essenceofzephyra", "Essence of Zephyra", 7)],
                Rank = "Artisan",
                Prefix = "Whirling",
                Level = 78,
                Difficulty = 4,
                Modification = item => item.AddScript<WhirlingPrefixScript>()
            }
        },
        {
            EnchantingRecipes.ZephyraWind, new Recipe
            {
                Name = "Zephyra's Wind",
                TemplateKey = "zephyrawind",
                Ingredients = [new Ingredient("essenceofzephyra", "Essence of Zephyra", 5)],
                Rank = "Initiate",
                Prefix = "Hazy",
                Level = 58,
                Difficulty = 3,
                Modification = item => item.AddScript<HazyPrefixScript>()
            }
        },
        {
            EnchantingRecipes.IgnatarDominance, new Recipe
            {
                Name = "Ignatar's Dominance",
                TemplateKey = "ignatardominance",
                Ingredients = [new Ingredient("essenceofignatar", "Essence of Ignatar", 10)],
                Rank = "Advanced",
                Prefix = "Fury",
                Level = 99,
                Difficulty = 6,
                Modification = item => item.AddScript<FuryPrefixScript>()
            }
        },
        {
            EnchantingRecipes.GeolithTestimony, new Recipe
            {
                Name = "Geolith's Testimony",
                TemplateKey = "geolithtestimony",
                Ingredients = [new Ingredient("essenceofgeolith", "Essence of Geolith", 10)],
                Rank = "Advanced",
                Prefix = "Sturdy",
                Level = 99,
                Difficulty = 6,
                Modification = item => item.AddScript<SturdyPrefixScript>()
            }
        },
        {
            EnchantingRecipes.MiraelisBrace, new Recipe
            {
                Name = "Miraelis' Brace",
                TemplateKey = "miraelisbrace",
                Ingredients = [new Ingredient("essenceofmiraelis", "Essence of Miraelis", 10)],
                Rank = "Advanced",
                Prefix = "Resilient",
                Level = 99,
                Difficulty = 6,
                Modification = item => item.AddScript<ResilientPrefixScript>()
            }
        },
        {
            EnchantingRecipes.AquaedonCommitment, new Recipe
            {
                Name = "Aquaedon's Commitment",
                TemplateKey = "aquaedoncommitment",
                Ingredients = [new Ingredient("essenceofaquaedon", "Essence of Aquaedon", 10)],
                Rank = "Advanced",
                Prefix = "Spirited",
                Level = 99,
                Difficulty = 6,
                Modification = item => item.AddScript<SpiritedPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SerendaelSuccess, new Recipe
            {
                Name = "Serendael's Success",
                TemplateKey = "serendaelsuccess",
                Ingredients = [new Ingredient("essenceofserendael", "Essence of Serendael", 10)],
                Rank = "Advanced",
                Prefix = "Pristine",
                Level = 99,
                Difficulty = 6,
                Modification = item => item.AddScript<PristinePrefixScript>()
            }
        },
        {
            EnchantingRecipes.TheseleneSorrow, new Recipe
            {
                Name = "Theselene's Sorrow",
                TemplateKey = "theselenesorrow",
                Ingredients = [new Ingredient("essenceoftheselene", "Essence of Theselene", 10)],
                Rank = "Advanced",
                Prefix = "Shaded",
                Level = 99,
                Difficulty = 6,
                Modification = item => item.AddScript<ShadedPrefixScript>()
            }
        },
        {
            EnchantingRecipes.SkandaraOffense, new Recipe
            {
                Name = "Skandara's Offense",
                TemplateKey = "skandaraoffense",
                Ingredients = [new Ingredient("essenceofskandara", "Essence of Skandara", 15)],
                Rank = "Expert",
                Prefix = "Sinister",
                Level = 99,
                Difficulty = 7,
                Modification = item => item.AddScript<SinisterPrefixScript>()
            }
        },
        {
            EnchantingRecipes.ZephyraPower, new Recipe
            {
                Name = "Zephyra's Power",
                TemplateKey = "zephyrapower",
                Ingredients = [new Ingredient("essenceofzephyra", "Essence of Zephyra", 15)],
                Rank = "Expert",
                Prefix = "Gleaming",
                Level = 99,
                Difficulty = 7,
                Modification = item => item.AddScript<GleamingPrefixScript>()
            }
        },
        {
            EnchantingRecipes.IgnatarDeficit, new Recipe
            {
                Name = "Ignatar's Deficit",
                TemplateKey = "ignatardeficit",
                Ingredients = [new Ingredient("essenceofignatar", "Essence of Ignatar", 15)],
                Rank = "Expert",
                Prefix = "Infernal",
                Level = 99,
                Difficulty = 7,
                Modification = item => item.AddScript<InfernalPrefixScript>()
            }
        },
        {
            EnchantingRecipes.GeolithBarrier, new Recipe
            {
                Name = "Geolith's Barrier",
                TemplateKey = "geolithbarrier",
                Ingredients = [new Ingredient("essenceofgeolith", "Essence of Geolith", 15)],
                Rank = "Expert",
                Prefix = "Primal",
                Level = 99,
                Difficulty = 7,
                Modification = item => item.AddScript<PrimalPrefixScript>()
            }
        },
        {
            EnchantingRecipes.MiraelisRoots, new Recipe
            {
                Name = "Miraelis' Roots",
                TemplateKey = "miraelisroots",
                Ingredients = [new Ingredient("essenceofmiraelis", "Essence of Miraelis", 15)],
                Rank = "Expert",
                Prefix = "Thick",
                Level = 99,
                Difficulty = 7,
                Modification = item => item.AddScript<ThickPrefixScript>()
            }
        }
    };
    #endregion

    public sealed class Ingredient(string templateKey, string displayName, int amount)
    {
        public int Amount { get; set; } = amount;
        public string DisplayName { get; set; } = displayName;
        public string TemplateKey { get; set; } = templateKey;
    }

    public sealed class Recipe
    {
        public int Difficulty { get; set; }
        public List<Ingredient> Ingredients { get; set; } = null!;
        public int Level { get; set; }
        public Action<Item>? Modification { get; set; }
        public string Name { get; set; } = null!;

        public string Prefix { get; set; } = null!;
        public string Rank { get; set; } = null!;
        public string TemplateKey { get; set; } = null!;
    }

    #region Jewelcrafting
    public static Dictionary<JewelcraftingRecipes, Recipe> JewelcraftingRequirements { get; } = new()
    {
        {
            JewelcraftingRecipes.BasicBerylEarrings, new Recipe
            {
                Name = "Basic Beryl Earrings",
                TemplateKey = "basicberylearrings",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedberyl", "Flawed Beryl", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BasicRubyEarrings, new Recipe
            {
                Name = "Basic Ruby Earrings",
                TemplateKey = "basicrubyearrings",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedruby", "Flawed Ruby", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BasicSapphireEarrings, new Recipe
            {
                Name = "Basic Sapphire Earrings",
                TemplateKey = "basicsapphireearrings",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedsapphire", "Flawed Sapphire", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BasicEmeraldEarrings, new Recipe
            {
                Name = "Basic Emerald Earrings",
                TemplateKey = "basicemeraldearrings",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedemerald", "Flawed Emerald", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BasicHeartstoneEarrings, new Recipe
            {
                Name = "Basic Heartstone Earrings",
                TemplateKey = "basicheartstoneearrings",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedheartstone", "Flawed Heartstone", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.IronBerylEarrings, new Recipe
            {
                Name = "Iron Beryl Earrings",
                TemplateKey = "ironberylearrings",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutberyl", "Uncut Beryl", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.IronRubyEarrings, new Recipe
            {
                Name = "Iron Ruby Earrings",
                TemplateKey = "ironrubyearrings",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutruby", "Uncut Ruby", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.IronSapphireEarrings, new Recipe
            {
                Name = "Iron Sapphire Earrings",
                TemplateKey = "ironsapphireearrings",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutsapphire", "Uncut Sapphire", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.IronEmeraldEarrings, new Recipe
            {
                Name = "Iron Emerald Earrings",
                TemplateKey = "ironemeraldearrings",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutemerald", "Uncut Emerald", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.IronHeartstoneEarrings, new Recipe
            {
                Name = "Iron Heartstone Earrings",
                TemplateKey = "ironheartstoneearrings",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutheartstone", "Uncut Heartstone", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.MythrilBerylEarrings, new Recipe
            {
                Name = "Mythril Beryl Earrings",
                TemplateKey = "mythrilberylearrings",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineberyl", "Pristine Beryl", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.MythrilRubyEarrings, new Recipe
            {
                Name = "Mythril Ruby Earrings",
                TemplateKey = "mythrilrubyearrings",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineruby", "Pristine Ruby", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.MythrilSapphireEarrings, new Recipe
            {
                Name = "Mythril Sapphire Earrings",
                TemplateKey = "mythrilsapphireearrings",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.MythrilEmeraldEarrings, new Recipe
            {
                Name = "Mythril Emerald Earrings",
                TemplateKey = "mythrilemeraldearrings",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineemerald", "Pristine Emerald", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.MythrilHeartstoneEarrings, new Recipe
            {
                Name = "Mythril Heartstone Earrings",
                TemplateKey = "mythrilheartstoneearrings",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.HybrasylBerylEarrings, new Recipe
            {
                Name = "Hy-brasyl Beryl Earrings",
                TemplateKey = "hybrasylberylearrings",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristineberyl", "Pristine Beryl", 3)
                ],
                Rank = "Adept",
                Level = 71,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.HybrasylRubyEarrings, new Recipe
            {
                Name = "Hy-brasyl Ruby Earrings",
                TemplateKey = "hybrasylrubyearrings",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristineruby", "Pristine Ruby", 3)
                ],
                Rank = "Adept",
                Level = 71,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.HybrasylSapphireEarrings, new Recipe
            {
                Name = "Hy-brasyl Sapphire Earrings",
                TemplateKey = "hybrasylsapphireearrings",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 3)
                ],
                Rank = "Adept",
                Level = 71,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.HybrasylEmeraldEarrings, new Recipe
            {
                Name = "Hy-brasyl Emerald Earrings",
                TemplateKey = "hybrasylemeraldearrings",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristineemerald", "Pristine Emerald", 3)
                ],
                Rank = "Adept",
                Level = 71,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.HybrasylHeartstoneEarrings, new Recipe
            {
                Name = "Hy-brasyl Heartstone Earrings",
                TemplateKey = "hybrasylheartstoneearrings",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 3)
                ],
                Rank = "Adept",
                Level = 71,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.SmallRubyRing, new Recipe
            {
                Name = "Small Ruby Ring",
                TemplateKey = "smallrubyring",
                Ingredients =
                [
                    new Ingredient("rawruby", "Raw Ruby", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            JewelcraftingRecipes.BerylRing, new Recipe
            {
                Name = "Beryl Ring",
                TemplateKey = "berylring",
                Ingredients =
                [
                    new Ingredient("rawberyl", "Raw Beryl", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            JewelcraftingRecipes.BronzeBerylRing, new Recipe
            {
                Name = "Bronze Beryl Ring",
                TemplateKey = "bronzeberylring",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedberyl", "Flawed Beryl", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BronzeRubyRing, new Recipe
            {
                Name = "Bronze Ruby Ring",
                TemplateKey = "bronzerubyring",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedruby", "Flawed Ruby", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BronzeSapphireRing, new Recipe
            {
                Name = "Bronze Sapphire Ring",
                TemplateKey = "bronzesapphirering",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedsapphire", "Flawed Sapphire", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BronzeEmeraldRing, new Recipe
            {
                Name = "Bronze Emerald Ring",
                TemplateKey = "bronzeemeraldring",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedemerald", "Flawed Emerald", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BronzeHeartstoneRing, new Recipe
            {
                Name = "Bronze Heartstone Ring",
                TemplateKey = "bronzeheartstonering",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedheartstone", "Flawed Heartstone", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.IronBerylRing, new Recipe
            {
                Name = "Iron Beryl Ring",
                TemplateKey = "ironberylring",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutberyl", "Uncut Beryl", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.IronRubyRing, new Recipe
            {
                Name = "Iron Ruby Ring",
                TemplateKey = "ironrubyring",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutruby", "Uncut Ruby", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.IronSapphireRing, new Recipe
            {
                Name = "Iron Sapphire Ring",
                TemplateKey = "ironsapphirering",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutsapphire", "Uncut Sapphire", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.IronEmeraldRing, new Recipe
            {
                Name = "Iron Emerald Ring",
                TemplateKey = "ironemeraldring",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutemerald", "Uncut Emerald", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.IronHeartstoneRing, new Recipe
            {
                Name = "Iron Heartstone Ring",
                TemplateKey = "ironheartstonering",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutheartstone", "Uncut Heartstone", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.MythrilBerylRing, new Recipe
            {
                Name = "Mythril Beryl Ring",
                TemplateKey = "mythrilberylring",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineberyl", "Pristine Beryl", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.MythrilRubyRing, new Recipe
            {
                Name = "Mythril Ruby Ring",
                TemplateKey = "mythrilrubyring",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineruby", "Pristine Ruby", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.MythrilSapphireRing, new Recipe
            {
                Name = "Mythril Sapphire Ring",
                TemplateKey = "mythrilsapphirering",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.MythrilEmeraldRing, new Recipe
            {
                Name = "Mythril Emerald Ring",
                TemplateKey = "mythrilemeraldring",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineemerald", "Pristine Emerald", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.MythrilHeartstoneRing, new Recipe
            {
                Name = "Mythril Heartstone Ring",
                TemplateKey = "mythrilheartstonering",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.HybrasylBerylRing, new Recipe
            {
                Name = "Hy-Brasyl Beryl Ring",
                TemplateKey = "hybrasylberylring",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-Brasyl Bar", 1),
                    new Ingredient("pristineberyl", "Pristine Beryl", 3)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.HybrasylRubyRing, new Recipe
            {
                Name = "Hy-Brasyl Ruby Ring",
                TemplateKey = "hybrasylrubyring",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-Brasyl Bar", 1),
                    new Ingredient("pristineruby", "Pristine Ruby", 3)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.HybrasylSapphireRing, new Recipe
            {
                Name = "Hy-Brasyl Sapphire Ring",
                TemplateKey = "hybrasylsapphirering",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-Brasyl Bar", 1),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 3)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.HybrasylEmeraldRing, new Recipe
            {
                Name = "Hy-Brasyl Emerald Ring",
                TemplateKey = "hybrasylemeraldring",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-Brasyl Bar", 1),
                    new Ingredient("pristineemerald", "Pristine Emerald", 3)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.HybrasylHeartstoneRing, new Recipe
            {
                Name = "Hy-Brasyl Heartstone Ring",
                TemplateKey = "hybrasylheartstonering",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-Brasyl Bar", 1),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 3)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.FireNecklace, new Recipe
            {
                Name = "Fire Necklace",
                TemplateKey = "firenecklace",
                Ingredients =
                [
                    new Ingredient("rawruby", "Raw Ruby", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            JewelcraftingRecipes.SeaNecklace, new Recipe
            {
                Name = "Sea Necklace",
                TemplateKey = "seanecklace",
                Ingredients =
                [
                    new Ingredient("rawsapphire", "Raw Sapphire", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            JewelcraftingRecipes.WindNecklace, new Recipe
            {
                Name = "Wind Necklace",
                TemplateKey = "windnecklace",
                Ingredients =
                [
                    new Ingredient("rawemerald", "Raw Emerald", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            JewelcraftingRecipes.EarthNecklace, new Recipe
            {
                Name = "Earth Necklace",
                TemplateKey = "earthnecklace",
                Ingredients =
                [
                    new Ingredient("rawberyl", "Raw Beryl", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1)
                ],
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            JewelcraftingRecipes.BoneEarthNecklace, new Recipe
            {
                Name = "Bone Earth Necklace",
                TemplateKey = "boneearthnecklace",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedberyl", "Flawed Beryl", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BoneFireNecklace, new Recipe
            {
                Name = "Bone Fire Necklace",
                TemplateKey = "bonefirenecklace",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedruby", "Flawed Ruby", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BoneSeaNecklace, new Recipe
            {
                Name = "Bone Sea Necklace",
                TemplateKey = "boneseanecklace",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedsapphire", "Flawed Sapphire", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.BoneWindNecklace, new Recipe
            {
                Name = "Bone Wind Necklace",
                TemplateKey = "bonewindnecklace",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedemerald", "Flawed Emerald", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            JewelcraftingRecipes.KannaEarthNecklace, new Recipe
            {
                Name = "Kanna Earth Necklace",
                TemplateKey = "kannaearthnecklace",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutberyl", "Uncut Beryl", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.KannaWindNecklace, new Recipe
            {
                Name = "Kanna Wind Necklace",
                TemplateKey = "kannawindnecklace",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutemerald", "Uncut Emerald", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.KannaFireNecklace, new Recipe
            {
                Name = "Kanna Fire Necklace",
                TemplateKey = "kannafirenecklace",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutruby", "Uncut Ruby", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.KannaSeaNecklace, new Recipe
            {
                Name = "Kanna Sea Necklace",
                TemplateKey = "kannaseanecklace",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutsapphire", "Uncut Sapphire", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            JewelcraftingRecipes.PolishedEarthNecklace, new Recipe
            {
                Name = "Polished Earth Necklace",
                TemplateKey = "polishedearthnecklace",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineberyl", "Pristine Beryl", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.PolishedWindNecklace, new Recipe
            {
                Name = "Polished Wind Necklace",
                TemplateKey = "polishedwindnecklace",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineemerald", "Pristine Emerald", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.PolishedFireNecklace, new Recipe
            {
                Name = "Polished Fire Necklace",
                TemplateKey = "polishedfirenecklace",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineruby", "Pristine Ruby", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.PolishedSeaNecklace, new Recipe
            {
                Name = "Polished Sea Necklace",
                TemplateKey = "polishedseanecklace",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            JewelcraftingRecipes.StarSeaNecklace, new Recipe
            {
                Name = "Star Sea Necklace",
                TemplateKey = "starseanecklace",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-Brasyl Bar", 1),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 3)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.StarFireNecklace, new Recipe
            {
                Name = "Star Fire Necklace",
                TemplateKey = "starfirenecklace",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-Brasyl Bar", 1),
                    new Ingredient("pristineruby", "Pristine Ruby", 3)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.StarEarthNecklace, new Recipe
            {
                Name = "Star Earth Necklace",
                TemplateKey = "starearthnecklace",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-Brasyl Bar", 1),
                    new Ingredient("pristineberyl", "Pristine Beryl", 3)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            JewelcraftingRecipes.StarWindNecklace, new Recipe
            {
                Name = "Star Wind Necklace",
                TemplateKey = "starwindnecklace",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-Brasyl Bar", 1),
                    new Ingredient("pristineemerald", "Pristine Emerald", 3)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        }
    };

    public static Dictionary<JewelcraftingRecipes2, Recipe> JewelcraftingRequirements2 { get; } = new()
    {
        {
            JewelcraftingRecipes2.CrimsoniteBerylRing, new Recipe
            {
                Name = "Crimsonite Beryl Ring",
                TemplateKey = "crimsoniteberylring",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("pristineberyl", "Pristine Beryl", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteRubyRing, new Recipe
            {
                Name = "Crimsonite Ruby Ring",
                TemplateKey = "crimsoniterubyring",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("pristineruby", "Pristine Ruby", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteSapphireRing, new Recipe
            {
                Name = "Crimsonite Sapphire Ring",
                TemplateKey = "crimsonitesapphirering",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteEmeraldRing, new Recipe
            {
                Name = "Crimsonite Emerald Ring",
                TemplateKey = "crimsoniteemeraldring",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("pristineemerald", "Pristine Emerald", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteHeartstoneRing, new Recipe
            {
                Name = "Crimsonite Heartstone Ring",
                TemplateKey = "crimsoniteheartstonering",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },

        {
            JewelcraftingRecipes2.AzuriumBerylRing, new Recipe
            {
                Name = "Azurium Beryl Ring",
                TemplateKey = "azuriumberylring",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("pristineberyl", "Pristine Beryl", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumRubyRing, new Recipe
            {
                Name = "Azurium Ruby Ring",
                TemplateKey = "azuriumrubyring",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("pristineruby", "Pristine Ruby", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumSapphireRing, new Recipe
            {
                Name = "Azurium Sapphire Ring",
                TemplateKey = "azuriumsapphirering",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumEmeraldRing, new Recipe
            {
                Name = "Azurium Emerald Ring",
                TemplateKey = "azuriumemeraldring",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("pristineemerald", "Pristine Emerald", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumHeartstoneRing, new Recipe
            {
                Name = "Azurium Heartstone Ring",
                TemplateKey = "azuriumheartstonering",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },

        {
            JewelcraftingRecipes2.CrimsoniteBerylEarrings, new Recipe
            {
                Name = "Crimsonite Beryl Earrings",
                TemplateKey = "crimsoniteberylearrings",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 4),
                    new Ingredient("pristineberyl", "Pristine Beryl", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteRubyEarrings, new Recipe
            {
                Name = "Crimsonite Ruby Earrings",
                TemplateKey = "crimsoniterubyearrings",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 4),
                    new Ingredient("pristineruby", "Pristine Ruby", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteSapphireEarrings, new Recipe
            {
                Name = "Crimsonite Sapphire Earrings",
                TemplateKey = "crimsonitesapphireearrings",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 4),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteEmeraldEarrings, new Recipe
            {
                Name = "Crimsonite Emerald Earrings",
                TemplateKey = "crimsoniteemeraldearrings",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 4),
                    new Ingredient("pristineemerald", "Pristine Emerald", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteHeartstoneEarrings, new Recipe
            {
                Name = "Crimsonite Heartstone Earrings",
                TemplateKey = "crimsoniteheartstoneearrings",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 4),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },

        {
            JewelcraftingRecipes2.AzuriumBerylEarrings, new Recipe
            {
                Name = "Azurium Beryl Earrings",
                TemplateKey = "azuriumberylearrings",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 4),
                    new Ingredient("pristineberyl", "Pristine Beryl", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumRubyEarrings, new Recipe
            {
                Name = "Azurium Ruby Earrings",
                TemplateKey = "azuriumrubyearrings",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 4),
                    new Ingredient("pristineruby", "Pristine Ruby", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumSapphireEarrings, new Recipe
            {
                Name = "Azurium Sapphire Earrings",
                TemplateKey = "azuriumsapphireearrings",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 4),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumEmeraldEarrings, new Recipe
            {
                Name = "Azurium Emerald Earrings",
                TemplateKey = "azuriumemeraldearrings",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 4),
                    new Ingredient("pristineemerald", "Pristine Emerald", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumHeartstoneEarrings, new Recipe
            {
                Name = "Azurium Heartstone Earrings",
                TemplateKey = "azuriumheartstoneearrings",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 4),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },

        {
            JewelcraftingRecipes2.CrimsoniteEarthNecklace, new Recipe
            {
                Name = "Crimsonite Earth Necklace",
                TemplateKey = "crimsoniteearthnecklace",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 5),
                    new Ingredient("pristineberyl", "Pristine Beryl", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteFireNecklace, new Recipe
            {
                Name = "Crimsonite Fire Necklace",
                TemplateKey = "crimsonitefirenecklace",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 5),
                    new Ingredient("pristineruby", "Pristine Ruby", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteSeaNecklace, new Recipe
            {
                Name = "Crimsonite Sea Necklace",
                TemplateKey = "crimsoniteseanecklace",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 5),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteWindNecklace, new Recipe
            {
                Name = "Crimsonite Wind Necklace",
                TemplateKey = "crimsonitewindnecklace",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 5),
                    new Ingredient("pristineemerald", "Pristine Emerald", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteLightNecklace, new Recipe
            {
                Name = "Crimsonite Light Necklace",
                TemplateKey = "crimsonitelightnecklace",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 20),
                    new Ingredient("radiantpearl", "Radiant Pearl", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 8
            }
        },
        {
            JewelcraftingRecipes2.CrimsoniteDarkNecklace, new Recipe
            {
                Name = "Crimsonite Dark Necklace",
                TemplateKey = "crimsonitedarknecklace",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 20),
                    new Ingredient("eclipsepearl", "Eclipse Pearl", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 8
            }
        },

        {
            JewelcraftingRecipes2.AzuriumEarthNecklace, new Recipe
            {
                Name = "Azurium Earth Necklace",
                TemplateKey = "azuriumearthnecklace",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 5),
                    new Ingredient("pristineberyl", "Pristine Beryl", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumFireNecklace, new Recipe
            {
                Name = "Azurium Fire Necklace",
                TemplateKey = "azuriumfirenecklace",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 5),
                    new Ingredient("pristineruby", "Pristine Ruby", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumSeaNecklace, new Recipe
            {
                Name = "Azurium Sea Necklace",
                TemplateKey = "azuriumseanecklace",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 5),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumWindNecklace, new Recipe
            {
                Name = "Azurium Wind Necklace",
                TemplateKey = "azuriumwindnecklace",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 5),
                    new Ingredient("pristineemerald", "Pristine Emerald", 4)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        },
        {
            JewelcraftingRecipes2.AzuriumLightNecklace, new Recipe
            {
                Name = "Azurium Light Necklace",
                TemplateKey = "azuriumlightnecklace",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 20),
                    new Ingredient("radiantpearl", "Radiant Pearl", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 8
            }
        },
        {
            JewelcraftingRecipes2.AzuriumDarkNecklace, new Recipe
            {
                Name = "Azurium Dark Necklace",
                TemplateKey = "azuriumdarknecklace",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 20),
                    new Ingredient("eclipsepearl", "Eclipse Pearl", 1)
                ],
                Rank = "Expert",
                Level = 99,
                Difficulty = 8
            }
        }
    };
    #endregion

    #region Weapon Smithing
    public static Dictionary<WeaponSmithingRecipes, Recipe> WeaponSmithingCraftRequirements { get; } = new()
    {
        {
            WeaponSmithingRecipes.Eppe, new Recipe
            {
                Name = "Eppe",
                TemplateKey = "eppe",
                Ingredients =
                [
                    new Ingredient("rawbronze", "Raw Bronze", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 2,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Saber, new Recipe
            {
                Name = "Saber",
                TemplateKey = "saber",
                Ingredients =
                [
                    new Ingredient("rawbronze", "Raw Bronze", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 7,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Claidheamh, new Recipe
            {
                Name = "Claidheamh",
                TemplateKey = "claidheamh",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.BroadSword, new Recipe
            {
                Name = "Broad Sword",
                TemplateKey = "broadsword",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 17,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.BattleSword, new Recipe
            {
                Name = "Battle Sword",
                TemplateKey = "battlesword",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.Masquerade, new Recipe
            {
                Name = "Masquerade",
                TemplateKey = "masquerade",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 4),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 31,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.Bramble, new Recipe
            {
                Name = "Bramble",
                TemplateKey = "bramble",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.Longsword, new Recipe
            {
                Name = "Longsword",
                TemplateKey = "longsword",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.Claidhmore, new Recipe
            {
                Name = "Claidhmore",
                TemplateKey = "claidhmore",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.EmeraldSword, new Recipe
            {
                Name = "Emerald Sword",
                TemplateKey = "emeraldsword",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Artisan",
                Level = 77,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.Gladius, new Recipe
            {
                Name = "Gladius",
                TemplateKey = "gladius",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 4),
                    new Ingredient("coal", "Coal", 4)
                ],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.Kindjal, new Recipe
            {
                Name = "Kindjal",
                TemplateKey = "kindjal",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 2),
                    new Ingredient("coal", "Coal", 5)
                ],
                Rank = "Adept",
                Level = 90,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.Hatchet, new Recipe
            {
                Name = "Hatchet",
                TemplateKey = "hatchet",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.SpikedClub, new Recipe
            {
                Name = "Spiked Club",
                TemplateKey = "spikedclub",
                Ingredients =
                [
                    new Ingredient("club", "Club", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 50,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.ChainMace, new Recipe
            {
                Name = "Chain Mace",
                TemplateKey = "chainmace",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 3)
                ],
                Rank = "Initiate",
                Level = 60,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.HandAxe, new Recipe
            {
                Name = "Handaxe",
                TemplateKey = "handaxe",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.TalgoniteAxe, new Recipe
            {
                Name = "Talgonite Axe",
                TemplateKey = "talgoniteaxe",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 3),
                    new Ingredient("coal", "Coal", 5)
                ],
                Rank = "Adept",
                Level = 95,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.MagusAres, new Recipe
            {
                Name = "Magus Ares",
                TemplateKey = "magusares",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.HolyHermes, new Recipe
            {
                Name = "Holy Hermes",
                TemplateKey = "holyhermes",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.MagusZeus, new Recipe
            {
                Name = "Magus Zeus",
                TemplateKey = "maguszeus",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.HolyKronos, new Recipe
            {
                Name = "Holy Kronos",
                TemplateKey = "holykronos",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.SnowDagger, new Recipe
            {
                Name = "Snow Dagger",
                TemplateKey = "snowdagger",
                Ingredients =
                [
                    new Ingredient("rawbronze", "Raw Bronze", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 2,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.CenterDagger, new Recipe
            {
                Name = "Center Dagger",
                TemplateKey = "centerdagger",
                Ingredients =
                [
                    new Ingredient("rawbronze", "Raw Bronze", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 4,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BlossomDagger, new Recipe
            {
                Name = "Blossom Dagger",
                TemplateKey = "blossomdagger",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 14,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.MoonDagger, new Recipe
            {
                Name = "Moon Dagger",
                TemplateKey = "moondagger",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 31,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.LightDagger, new Recipe
            {
                Name = "Light Dagger",
                TemplateKey = "lightdagger",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.SunDagger, new Recipe
            {
                Name = "Sun Dagger",
                TemplateKey = "sundagger",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 62,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.LotusDagger, new Recipe
            {
                Name = "Lotus Dagger",
                TemplateKey = "lotusdagger",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Artisan",
                Level = 75,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.BloodDagger, new Recipe
            {
                Name = "Blood Dagger",
                TemplateKey = "blooddagger",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 3),
                    new Ingredient("coal", "Coal", 3)
                ],
                Rank = "Artisan",
                Level = 89,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.DullClaw, new Recipe
            {
                Name = "Dull Claw",
                TemplateKey = "dullclaw",
                Ingredients =
                [
                    new Ingredient("rawbronze", "Raw Bronze", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.WolfClaw, new Recipe
            {
                Name = "Wolf Claw",
                TemplateKey = "wolfclaw",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.EagleTalon, new Recipe
            {
                Name = "Eagle Talon",
                TemplateKey = "eagletalon",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.PhoenixClaw, new Recipe
            {
                Name = "Phoenix Claw",
                TemplateKey = "phoenixclaw",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.WoodenShield, new Recipe
            {
                Name = "Wooden Shield",
                TemplateKey = "woodenshield",
                Ingredients =
                [
                    new Ingredient("rawbronze", "Raw Bronze", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.LeatherShield, new Recipe
            {
                Name = "Leather Shield",
                TemplateKey = "leathershield",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 15,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.BronzeShield, new Recipe
            {
                Name = "Bronze Shield",
                TemplateKey = "bronzeshield",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 31,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.IronShield, new Recipe
            {
                Name = "Iron Shield",
                TemplateKey = "ironshield",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Initiate",
                Level = 45,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.MythrilShield, new Recipe
            {
                Name = "Mythril Shield",
                TemplateKey = "mythrilshield",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 61,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.HybrasylShield, new Recipe
            {
                Name = "Hy-brasyl Shield",
                TemplateKey = "hybrasylshield",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Artisan",
                Level = 77,
                Difficulty = 4
            }
        }
    };

    public static Dictionary<WeaponSmithingRecipes, Recipe> WeaponSmithingUpgradeRequirements { get; } = new()
    {
        {
            WeaponSmithingRecipes.Eppe, new Recipe
            {
                Name = "Eppe",
                TemplateKey = "eppe",
                Ingredients =
                [
                    new Ingredient("eppe", "Eppe", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 2,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Saber, new Recipe
            {
                Name = "Saber",
                TemplateKey = "saber",
                Ingredients =
                [
                    new Ingredient("Saber", "Saber", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 7,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Claidheamh, new Recipe
            {
                Name = "Claidheamh",
                TemplateKey = "claidheamh",
                Ingredients =
                [
                    new Ingredient("claidheamh", "Claidheamh", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.BroadSword, new Recipe
            {
                Name = "Broad Sword",
                TemplateKey = "broadsword",
                Ingredients =
                [
                    new Ingredient("broadsword", "Broad Sword", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 17,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.BattleSword, new Recipe
            {
                Name = "Battle Sword",
                TemplateKey = "battlesword",
                Ingredients =
                [
                    new Ingredient("battlesword", "Battle Sword", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.Masquerade, new Recipe
            {
                Name = "Masquerade",
                TemplateKey = "masquerade",
                Ingredients =
                [
                    new Ingredient("masquerade", "Masquerade", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 31,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.Bramble, new Recipe
            {
                Name = "Bramble",
                TemplateKey = "bramble",
                Ingredients =
                [
                    new Ingredient("bramble", "Bramble", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("coal", "Coal", 3)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.Longsword, new Recipe
            {
                Name = "Longsword",
                TemplateKey = "longsword",
                Ingredients =
                [
                    new Ingredient("Longsword", "Longsword", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("coal", "Coal", 3)
                ],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.Claidhmore, new Recipe
            {
                Name = "Claidhmore",
                TemplateKey = "claidhmore",
                Ingredients =
                [
                    new Ingredient("Claidhmore", "Claidhmore", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 3)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.EmeraldSword, new Recipe
            {
                Name = "Emerald Sword",
                TemplateKey = "emeraldsword",
                Ingredients =
                [
                    new Ingredient("emeraldsword", "Emerald Sword", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 3)
                ],
                Rank = "Artisan",
                Level = 77,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.Gladius, new Recipe
            {
                Name = "Gladius",
                TemplateKey = "gladius",
                Ingredients =
                [
                    new Ingredient("Gladius", "Gladius", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 3),
                    new Ingredient("coal", "Coal", 4)
                ],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.Kindjal, new Recipe
            {
                Name = "Kindjal",
                TemplateKey = "kindjal",
                Ingredients =
                [
                    new Ingredient("kindjal", "Kindjal", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 3),
                    new Ingredient("coal", "Coal", 4)
                ],
                Rank = "Adept",
                Level = 90,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.DragonSlayer, new Recipe
            {
                Name = "Dragon Slayer",
                TemplateKey = "dragonslayer",
                Ingredients =
                [
                    new Ingredient("dragonslayer", "Dragon Slayer", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 5),
                    new Ingredient("coal", "Coal", 5)
                ],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.Hatchet, new Recipe
            {
                Name = "Hatchet",
                TemplateKey = "hatchet",
                Ingredients =
                [
                    new Ingredient("hatchet", "Hatchet", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.Harpoon, new Recipe
            {
                Name = "Harpoon",
                TemplateKey = "harpoon",
                Ingredients =
                [
                    new Ingredient("harpoon", "Harpoon", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 21,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.Scimitar, new Recipe
            {
                Name = "Scimitar",
                TemplateKey = "scimitar",
                Ingredients =
                [
                    new Ingredient("Scimitar", "Scimitar", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 31,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.Club, new Recipe
            {
                Name = "Club",
                TemplateKey = "club",
                Ingredients =
                [
                    new Ingredient("Club", "Club", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.SpikedClub, new Recipe
            {
                Name = "Spiked Club",
                TemplateKey = "spikedclub",
                Ingredients =
                [
                    new Ingredient("SpikedClub", "Spiked Club", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 50,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.ChainMace, new Recipe
            {
                Name = "Chain Mace",
                TemplateKey = "chainmace",
                Ingredients =
                [
                    new Ingredient("chainmace", "Chain Mace", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 3)
                ],
                Rank = "Initiate",
                Level = 60,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.HandAxe, new Recipe
            {
                Name = "Handaxe",
                TemplateKey = "handaxe",
                Ingredients =
                [
                    new Ingredient("handaxe", "Handaxe", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.Cutlass, new Recipe
            {
                Name = "Cutlass",
                TemplateKey = "cutlass",
                Ingredients =
                [
                    new Ingredient("cutlass", "Cutlass", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Artisan",
                Level = 80,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.TalgoniteAxe, new Recipe
            {
                Name = "Talgonite Axe",
                TemplateKey = "talgoniteaxe",
                Ingredients =
                [
                    new Ingredient("Talgoniteaxe", "Talgonite Axe", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 4),
                    new Ingredient("coal", "Coal", 4)
                ],
                Rank = "Adept",
                Level = 95,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.HybrasylBattleAxe, new Recipe
            {
                Name = "Hy-brasyl Battle Axe",
                TemplateKey = "hybrasylbattleaxe",
                Ingredients =
                [
                    new Ingredient("hybrasylbattleaxe", "Hy-brasyl Battle Axe", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 5),
                    new Ingredient("coal", "Coal", 5)
                ],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.MagusAres, new Recipe
            {
                Name = "Magus Ares",
                TemplateKey = "magusares",
                Ingredients =
                [
                    new Ingredient("magusares", "Magus Ares", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.HolyHermes, new Recipe
            {
                Name = "Holy Hermes",
                TemplateKey = "holyhermes",
                Ingredients =
                [
                    new Ingredient("holyhermes", "Holy Hermes", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.MagusZeus, new Recipe
            {
                Name = "Magus Zeus",
                TemplateKey = "maguszeus",
                Ingredients =
                [
                    new Ingredient("maguszeus", "Magus Zeus", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.HolyKronos, new Recipe
            {
                Name = "Holy Kronos",
                TemplateKey = "holykronos",
                Ingredients =
                [
                    new Ingredient("holykronos", "Holy Kronos", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.MagusDiana, new Recipe
            {
                Name = "Magus Diana",
                TemplateKey = "magusdiana",
                Ingredients =
                [
                    new Ingredient("magusdiana", "Magus Diana", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.HolyDiana, new Recipe
            {
                Name = "Holy Diana",
                TemplateKey = "holydiana",
                Ingredients =
                [
                    new Ingredient("holydiana", "Holy Diana", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.StoneCross, new Recipe
            {
                Name = "Stone Cross",
                TemplateKey = "stonecross",
                Ingredients =
                [
                    new Ingredient("stonecross", "Stone Cross", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 4),
                    new Ingredient("coal", "Coal", 3)
                ],
                Rank = "Adept",
                Level = 90,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.OakStaff, new Recipe
            {
                Name = "Oak Staff",
                TemplateKey = "oakstaff",
                Ingredients =
                [
                    new Ingredient("oakstaff", "Oak Staff", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 5),
                    new Ingredient("coal", "Coal", 4)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.StaffOfWisdom, new Recipe
            {
                Name = "Staff of Wisdom",
                TemplateKey = "staffofwisdom",
                Ingredients =
                [
                    new Ingredient("staffofwisdom", "Staff of Wisdom", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 5),
                    new Ingredient("coal", "Coal", 4)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.SnowDagger, new Recipe
            {
                Name = "Snow Dagger",
                TemplateKey = "snowdagger",
                Ingredients =
                [
                    new Ingredient("snowdagger", "Snow Dagger", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 2,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.CenterDagger, new Recipe
            {
                Name = "Center Dagger",
                TemplateKey = "centerdagger",
                Ingredients =
                [
                    new Ingredient("centerdagger", "Center Dagger", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 4,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BlossomDagger, new Recipe
            {
                Name = "Blossom Dagger",
                TemplateKey = "blossomdagger",
                Ingredients =
                [
                    new Ingredient("blossomdagger", "Blossom Dagger", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 14,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.CurvedDagger, new Recipe
            {
                Name = "Curved Dagger",
                TemplateKey = "curveddagger",
                Ingredients =
                [
                    new Ingredient("curveddagger", "Curved Dagger", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 3),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 30,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.MoonDagger, new Recipe
            {
                Name = "Moon Dagger",
                TemplateKey = "moondagger",
                Ingredients =
                [
                    new Ingredient("moondagger", "Moon Dagger", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 31,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.LightDagger, new Recipe
            {
                Name = "Light Dagger",
                TemplateKey = "lightdagger",
                Ingredients =
                [
                    new Ingredient("lightdagger", "Light Dagger", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.SunDagger, new Recipe
            {
                Name = "Sun Dagger",
                TemplateKey = "sundagger",
                Ingredients =
                [
                    new Ingredient("sundagger", "Sun Dagger", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 62,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.LotusDagger, new Recipe
            {
                Name = "Lotus Dagger",
                TemplateKey = "lotusdagger",
                Ingredients =
                [
                    new Ingredient("lotusdagger", "Lotus Dagger", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Artisan",
                Level = 75,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.BloodDagger, new Recipe
            {
                Name = "Blood Dagger",
                TemplateKey = "blooddagger",
                Ingredients =
                [
                    new Ingredient("blooddagger", "Blood Dagger", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Artisan",
                Level = 89,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.NagetierDagger, new Recipe
            {
                Name = "Nagetier Dagger",
                TemplateKey = "nagetierdagger",
                Ingredients =
                [
                    new Ingredient("nagetierdagger", "Nagetier Dagger", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 5),
                    new Ingredient("coal", "Coal", 4)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.DullClaw, new Recipe
            {
                Name = "Dull Claw",
                TemplateKey = "dullclaw",
                Ingredients =
                [
                    new Ingredient("dullclaw", "Dull Claw", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.WolfClaw, new Recipe
            {
                Name = "Wolf Claw",
                TemplateKey = "wolfclaw",
                Ingredients =
                [
                    new Ingredient("wolfclaw", "Wolf Claw", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.EagleTalon, new Recipe
            {
                Name = "Eagle Talon",
                TemplateKey = "eagletalon",
                Ingredients =
                [
                    new Ingredient("eagletalon", "Eagle Talon", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.StoneFist, new Recipe
            {
                Name = "Stone Fist",
                TemplateKey = "stonefist",
                Ingredients =
                [
                    new Ingredient("stonefist", "Stone Fist", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.PhoenixClaw, new Recipe
            {
                Name = "Phoenix Claw",
                TemplateKey = "phoenixclaw",
                Ingredients =
                [
                    new Ingredient("phoenixclaw", "Phoenix Claw", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.Nunchaku, new Recipe
            {
                Name = "Nunchaku",
                TemplateKey = "nunchaku",
                Ingredients =
                [
                    new Ingredient("Nunchaku", "Nunchaku", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 5),
                    new Ingredient("coal", "Coal", 4)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes.WoodenShield, new Recipe
            {
                Name = "Wooden Shield",
                TemplateKey = "woodenshield",
                Ingredients =
                [
                    new Ingredient("woodenshield", "Wooden Shield", 1),
                    new Ingredient("rawbronze", "Raw Bronze", 1),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.LeatherShield, new Recipe
            {
                Name = "Leather Shield",
                TemplateKey = "leathershield",
                Ingredients =
                [
                    new Ingredient("leathershield", "Leather Shield", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Basic",
                Level = 15,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.BronzeShield, new Recipe
            {
                Name = "Bronze Shield",
                TemplateKey = "bronzeshield",
                Ingredients =
                [
                    new Ingredient("bronzeshield", "Bronze Shield", 1),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Basic",
                Level = 31,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.GravelShield, new Recipe
            {
                Name = "Gravel Shield",
                TemplateKey = "gravelshield",
                Ingredients =
                [
                    new Ingredient("gravelshield", "Gravel Shield", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 2),
                    new Ingredient("coal", "Coal", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.IronShield, new Recipe
            {
                Name = "Iron Shield",
                TemplateKey = "ironshield",
                Ingredients =
                [
                    new Ingredient("ironshield", "Iron Shield", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 45,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.LightShield, new Recipe
            {
                Name = "Light Shield",
                TemplateKey = "lightshield",
                Ingredients =
                [
                    new Ingredient("lightshield", "Light Shield", 1),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 50,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.MythrilShield, new Recipe
            {
                Name = "Mythril Shield",
                TemplateKey = "mythrilshield",
                Ingredients =
                [
                    new Ingredient("mythrilshield", "Mythril Shield", 1),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 3),
                    new Ingredient("coal", "Coal", 2)
                ],
                Rank = "Initiate",
                Level = 61,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.HybrasylShield, new Recipe
            {
                Name = "Hy-brasyl Shield",
                TemplateKey = "hybrasylshield",
                Ingredients =
                [
                    new Ingredient("hybrasylshield", "Hy-brasyl Shield", 1),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 4),
                    new Ingredient("coal", "Coal", 3)
                ],
                Rank = "Artisan",
                Level = 77,
                Difficulty = 4
            }
        }
    };

    public static Dictionary<WeaponSmithingRecipes2, Recipe> WeaponSmithingUpgradeRequirements2 { get; } = new()
    {
        {
            WeaponSmithingRecipes2.TalgoniteShield, new Recipe
            {
                Name = "Talgonite Shield",
                TemplateKey = "talgoniteshield",
                Ingredients =
                [
                    new Ingredient("talgoniteshield", "Talgonite Shield", 1),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 5),
                    new Ingredient("coal", "Coal", 4)
                ],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes2.CaptainShield, new Recipe
            {
                Name = "Captain's Shield",
                TemplateKey = "captainsshield",
                Ingredients =
                [
                    new Ingredient("captainsshield", "Captain's Shield", 1),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("coal", "Coal", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.CursedShield, new Recipe
            {
                Name = "Cursed Shield",
                TemplateKey = "cursedshield",
                Ingredients =
                [
                    new Ingredient("cursedshield", "Cursed Shield", 1),
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("coal", "Coal", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            WeaponSmithingRecipes2.CathonicShield, new Recipe
            {
                Name = "Cathonic Shield",
                TemplateKey = "cathonicshield",
                Ingredients =
                [
                    new Ingredient("cathonicshield", "Cathonic Shield", 1),
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("coal", "Coal", 5)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 5
            }
        }
    };

    public static Dictionary<WeaponSmithingRecipes2, Recipe> WeaponSmithingCraftRequirements2 { get; } = new()
    {
        {
            WeaponSmithingRecipes2.EnchantStone, new Recipe
            {
                Name = "Enchanting Stone",
                TemplateKey = "enchantingstone",
                Ingredients =
                [
                    new Ingredient("strangestone", "Strange Stone", 1),
                    new Ingredient("radiantpearl", "Radiant Pearl", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.EmpowerStone, new Recipe
            {
                Name = "Empowering Stone",
                TemplateKey = "empoweringstone",
                Ingredients =
                [
                    new Ingredient("strangestone", "Strange Stone", 1),
                    new Ingredient("eclipsepearl", "Eclipse Pearl", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.PolishingStone, new Recipe
            {
                Name = "Polishing Stone",
                TemplateKey = "polishingstone",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 2),
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 2),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 10)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes2.EmeraldTonfa, new Recipe
            {
                Name = "Emerald Tonfa",
                TemplateKey = "emeraldtonfa",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("pristineemerald", "Pristine Emerald", 25)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.RubyTonfa, new Recipe
            {
                Name = "Ruby Tonfa",
                TemplateKey = "rubytonfa",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("pristineruby", "Pristine Ruby", 25)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.SapphireTonfa, new Recipe
            {
                Name = "Sapphire Tonfa",
                TemplateKey = "sapphiretonfa",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 25)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.StaffofAquaedon, new Recipe
            {
                Name = "Staff of Aquaedon",
                TemplateKey = "staffofaquaedon",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("essenceofaquaedon", "Essence of Aquaedon", 20)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.StaffofMiraelis, new Recipe
            {
                Name = "Staff of Miraelis",
                TemplateKey = "staffofmiraelis",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("essenceofmiraelis", "Essence of Miraelis", 20)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.StaffofZephyra, new Recipe
            {
                Name = "Staff of Zephyra",
                TemplateKey = "staffofzephyra",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("essenceofzephyra", "Essence of Zephyra", 20)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.StaffofIgnatar, new Recipe
            {
                Name = "Staff of Ignatar",
                TemplateKey = "staffofignatar",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("essenceofignatar", "Essence of Ignatar", 20)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.StaffofGeolith, new Recipe
            {
                Name = "Staff of Geolith",
                TemplateKey = "staffofgeolith",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("essenceofgeolith", "Essence of Geolith", 20)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.StaffofTheselene, new Recipe
            {
                Name = "Staff of Theselene",
                TemplateKey = "staffoftheselene",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("essenceoftheselene", "Essence of Theselene", 20)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.ChainWhip, new Recipe
            {
                Name = "Chain Whip",
                TemplateKey = "chainwhip",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("eclipsepearl", "Eclipse Pearl", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.Kris, new Recipe
            {
                Name = "Kris",
                TemplateKey = "kris",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("radiantpearl", "Radiant Pearl", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.ScurvyDagger, new Recipe
            {
                Name = "Scurvy Dagger",
                TemplateKey = "scurvydagger",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 15)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.RubySaber, new Recipe
            {
                Name = "Ruby Saber",
                TemplateKey = "rubysaber",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("pristineruby", "Pristine Ruby", 25)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.SapphireSaber, new Recipe
            {
                Name = "Sapphire Saber",
                TemplateKey = "sapphiresaber",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 25)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.BerylSaber, new Recipe
            {
                Name = "Beryl Saber",
                TemplateKey = "berylsaber",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("pristineberyl", "Pristine Beryl", 25)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.ForestEscalon, new Recipe
            {
                Name = "Forest Escalon",
                TemplateKey = "forestescalon",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("superiormonsterextract", "Superior Monster Extract", 15)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.ShadowEscalon, new Recipe
            {
                Name = "Shadow Escalon",
                TemplateKey = "shadowescalon",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("essenceoftheselene", "Essence of Theselene", 25)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            WeaponSmithingRecipes2.MoonEscalon, new Recipe
            {
                Name = "Moon Escalon",
                TemplateKey = "moonescalon",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 10),
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 10),
                    new Ingredient("eclipsepearl", "Eclipse Pearl", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        }
    };
    #endregion

    #region Armor Smithing
    public static Dictionary<CraftedArmors, Recipe> ArmorSmithingArmorRequirements { get; } = new()
    {
        {
            CraftedArmors.RefinedScoutLeather, new Recipe
            {
                Name = "Scout Leather Pattern",
                TemplateKey = "scoutleatherpattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedDwarvishLeather, new Recipe
            {
                Name = "Dwarvish Leather Pattern",
                TemplateKey = "dwarvishleatherpattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedPaluten, new Recipe
            {
                Name = "Paluten Pattern",
                TemplateKey = "palutenpattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedKeaton, new Recipe
            {
                Name = "Keaton Pattern",
                TemplateKey = "keatonpattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedBardocle, new Recipe
            {
                Name = "Bardocle Pattern",
                TemplateKey = "bardoclepattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedGardcorp, new Recipe
            {
                Name = "Gardcorp Pattern",
                TemplateKey = "gardcorppattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedJourneyman, new Recipe
            {
                Name = "Journeyman Pattern",
                TemplateKey = "journeymanpattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedLorum, new Recipe
            {
                Name = "Lorum Pattern",
                TemplateKey = "lorumpattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedMane, new Recipe
            {
                Name = "Mane Pattern",
                TemplateKey = "manepattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedDuinUasal, new Recipe
            {
                Name = "Duin-Uasal Pattern",
                TemplateKey = "duinuasalpattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedCowl, new Recipe
            {
                Name = "Cowl Pattern",
                TemplateKey = "cowlpattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedGaluchatCoat, new Recipe
            {
                Name = "Galuchat Coat Pattern",
                TemplateKey = "galuchatcoatpattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedMantle, new Recipe
            {
                Name = "Mantle Pattern",
                TemplateKey = "mantlepattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedHierophant, new Recipe
            {
                Name = "Hierophant Pattern",
                TemplateKey = "hierophantpattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedDalmatica, new Recipe
            {
                Name = "Dalmatica Pattern",
                TemplateKey = "dalmaticapattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedDobok, new Recipe
            {
                Name = "Dobok Pattern",
                TemplateKey = "dobokpattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedCulotte, new Recipe
            {
                Name = "Culotte Pattern",
                TemplateKey = "culottepattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedEarthGarb, new Recipe
            {
                Name = "Earth Garb Pattern",
                TemplateKey = "earthgarbpattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedWindGarb, new Recipe
            {
                Name = "Wind Garb Pattern",
                TemplateKey = "windgarbpattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedMountainGarb, new Recipe
            {
                Name = "Mountain Garb Pattern",
                TemplateKey = "mountaingarbpattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedLeatherTunic, new Recipe
            {
                Name = "Leather Tunic Pattern",
                TemplateKey = "leathertunicpattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedLorica, new Recipe
            {
                Name = "Lorica Pattern",
                TemplateKey = "loricapattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedKasmaniumArmor, new Recipe
            {
                Name = "Kasmanium Armor Pattern",
                TemplateKey = "kasmaniumarmorpattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedIpletMail, new Recipe
            {
                Name = "Iplet Mail Pattern",
                TemplateKey = "ipletmailpattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedHybrasylPlate, new Recipe
            {
                Name = "Hy-brasyl Plate Pattern",
                TemplateKey = "hybrasylplatepattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },

        {
            CraftedArmors.RefinedCotte, new Recipe
            {
                Name = "Cotte Pattern",
                TemplateKey = "cottepattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedBrigandine, new Recipe
            {
                Name = "Brigandine Pattern",
                TemplateKey = "brigandinepattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedCorsette, new Recipe
            {
                Name = "Corsette Pattern",
                TemplateKey = "corsettepattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedPebbleRose, new Recipe
            {
                Name = "Pebble Rose Pattern",
                TemplateKey = "pebblerosepattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedKagum, new Recipe
            {
                Name = "Kagum Pattern",
                TemplateKey = "kagumpattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedMagiSkirt, new Recipe
            {
                Name = "Magi Skirt Pattern",
                TemplateKey = "magiskirtpattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedBenusta, new Recipe
            {
                Name = "Benusta Pattern",
                TemplateKey = "benustapattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedStoller, new Recipe
            {
                Name = "Stoller Pattern",
                TemplateKey = "stollerpattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedClymouth, new Recipe
            {
                Name = "Clymouth Pattern",
                TemplateKey = "clymouthpattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedClamyth, new Recipe
            {
                Name = "Clamyth Pattern",
                TemplateKey = "clamythpattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedGorgetGown, new Recipe
            {
                Name = "Gorget Gown Pattern",
                TemplateKey = "gorgetgownpattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedMysticGown, new Recipe
            {
                Name = "Mystic Gown Pattern",
                TemplateKey = "mysticgownpattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedElle, new Recipe
            {
                Name = "Elle Pattern",
                TemplateKey = "ellepattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedDolman, new Recipe
            {
                Name = "Dolman Pattern",
                TemplateKey = "dolmanpattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedBansagart, new Recipe
            {
                Name = "Bansagart Pattern",
                TemplateKey = "bansagartpattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedEarthBodice, new Recipe
            {
                Name = "Earth Bodice Pattern",
                TemplateKey = "earthbodicepattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedLotusBodice, new Recipe
            {
                Name = "Lotus Bodice Pattern",
                TemplateKey = "lotusbodicepattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedMoonBodice, new Recipe
            {
                Name = "Moon Bodice Pattern",
                TemplateKey = "moonbodicepattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedLightningGarb, new Recipe
            {
                Name = "Lightning Garb Pattern",
                TemplateKey = "lightninggarbpattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedSeaGarb, new Recipe
            {
                Name = "Sea Garb Pattern",
                TemplateKey = "seagarbpattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedLeatherBliaut, new Recipe
            {
                Name = "Leather Bliaut Pattern",
                TemplateKey = "leatherbliautpattern",
                Ingredients = [new Ingredient("finelinen", "Fine Linen", 3)],
                Rank = "Beginner",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedCuirass, new Recipe
            {
                Name = "Cuirass Pattern",
                TemplateKey = "cuirasspattern",
                Ingredients = [new Ingredient("exquisitelinen", "Exquisite Linen", 2)],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedKasmaniumHauberk, new Recipe
            {
                Name = "Kasmanium Hauberk Pattern",
                TemplateKey = "kasmaniumhauberkpattern",
                Ingredients = [new Ingredient("exquisitecotton", "Exquisite Cotton", 3)],
                Rank = "Initiate",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedPhoenixMail, new Recipe
            {
                Name = "Phoenix Mail Pattern",
                TemplateKey = "phoenixmailpattern",
                Ingredients = [new Ingredient("exquisitewool", "Exquisite Wool", 4)],
                Rank = "Artisan",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedHybrasylArmor, new Recipe
            {
                Name = "Hy-brasyl Armor Pattern",
                TemplateKey = "hybrasylarmorpattern",
                Ingredients = [new Ingredient("exquisitesilk", "Exquisite Silk", 5)],
                Rank = "Adept",
                Level = 99,
                Difficulty = 5
            }
        }
    };

    public static Dictionary<CraftedArmors2, Recipe> ArmorSmithingArmorRequirements2 { get; } = new()
    {
        {
            CraftedArmors2.RefiningKit, new Recipe
            {
                Name = "Refining Kit",
                TemplateKey = "refiningkit",
                Ingredients =
                [
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 25),
                    new Ingredient("strangestone", "Strange Stone", 1)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 7
            }
        }
    };

    public static Dictionary<ArmorsmithingRecipes, Recipe> ArmorSmithingGearRequirements { get; } = new()
    {
        {
            ArmorsmithingRecipes.EarthBelt, new Recipe
            {
                Name = "Earth Belt",
                TemplateKey = "earthbelt",
                Ingredients =
                [
                    new Ingredient("linen", "Linen", 5),
                    new Ingredient("rawberyl", "Raw Beryl", 1)
                ],
                Rank = "Beginner",
                Level = 5,
                Difficulty = 1
            }
        },
        {
            ArmorsmithingRecipes.WindBelt, new Recipe
            {
                Name = "Wind Belt",
                TemplateKey = "windbelt",
                Ingredients =
                [
                    new Ingredient("linen", "Linen", 5),
                    new Ingredient("rawemerald", "Raw Emerald", 1)
                ],
                Rank = "Beginner",
                Level = 5,
                Difficulty = 1
            }
        },
        {
            ArmorsmithingRecipes.SeaBelt, new Recipe
            {
                Name = "Sea Belt",
                TemplateKey = "seabelt",
                Ingredients =
                [
                    new Ingredient("linen", "Linen", 5),
                    new Ingredient("rawsapphire", "Raw Sapphire", 1)
                ],
                Rank = "Beginner",
                Level = 5,
                Difficulty = 1
            }
        },
        {
            ArmorsmithingRecipes.FireBelt, new Recipe
            {
                Name = "Fire Belt",
                TemplateKey = "firebelt",
                Ingredients =
                [
                    new Ingredient("linen", "Linen", 5),
                    new Ingredient("rawruby", "Raw Ruby", 1)
                ],
                Rank = "Beginner",
                Level = 5,
                Difficulty = 1
            }
        },
        {
            ArmorsmithingRecipes.FireBronzeBelt, new Recipe
            {
                Name = "Fire Bronze Belt",
                TemplateKey = "firebronzebelt",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedruby", "Flawed Ruby", 2)
                ],
                Rank = "Basic",
                Level = 20,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.SeaBronzeBelt, new Recipe
            {
                Name = "Sea Bronze Belt",
                TemplateKey = "seabronzebelt",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedsapphire", "Flawed Sapphire", 2)
                ],
                Rank = "Basic",
                Level = 20,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.EarthBronzeBelt, new Recipe
            {
                Name = "Earth Bronze Belt",
                TemplateKey = "earthbronzebelt",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedberyl", "Flawed Beryl", 2)
                ],
                Rank = "Basic",
                Level = 20,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.WindBronzeBelt, new Recipe
            {
                Name = "Wind Bronze Belt",
                TemplateKey = "windbronzebelt",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedemerald", "Flawed Emerald", 2)
                ],
                Rank = "Basic",
                Level = 20,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.DarkBronzeBelt, new Recipe
            {
                Name = "Dark Bronze Belt",
                TemplateKey = "darkbronzebelt",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedheartstone", "Flawed Heartstone", 2)
                ],
                Rank = "Basic",
                Level = 30,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.LightBronzeBelt, new Recipe
            {
                Name = "Light Bronze Belt",
                TemplateKey = "lightbronzebelt",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("flawedheartstone", "Flawed Heartstone", 2)
                ],
                Rank = "Basic",
                Level = 30,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.FireIronBelt, new Recipe
            {
                Name = "Fire Iron Belt",
                TemplateKey = "fireironbelt",
                Ingredients =
                [
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutruby", "Uncut Ruby", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.SeaIronBelt, new Recipe
            {
                Name = "Sea Iron Belt",
                TemplateKey = "seaironbelt",
                Ingredients =
                [
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutsapphire", "Uncut Sapphire", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.WindIronBelt, new Recipe
            {
                Name = "Wind Iron Belt",
                TemplateKey = "windironbelt",
                Ingredients =
                [
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutemerald", "Uncut Emerald", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.EarthIronBelt, new Recipe
            {
                Name = "Earth Iron Belt",
                TemplateKey = "earthironbelt",
                Ingredients =
                [
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutberyl", "Uncut Beryl", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.LightIronBelt, new Recipe
            {
                Name = "Light Iron Belt",
                TemplateKey = "lightironbelt",
                Ingredients =
                [
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutheartstone", "Uncut Heartstone", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.DarkIronBelt, new Recipe
            {
                Name = "Dark Iron Belt",
                TemplateKey = "darkironbelt",
                Ingredients =
                [
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("uncutheartstone", "Uncut Heartstone", 2)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.FireMythrilBraidBelt, new Recipe
            {
                Name = "Fire Mythril Braid Belt",
                TemplateKey = "firemythrilbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineruby", "Pristine Ruby", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.EarthMythrilBraidBelt, new Recipe
            {
                Name = "Earth Mythril Braid Belt",
                TemplateKey = "earthmythrilbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineberyl", "Pristine Beryl", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.WindMythrilBraidBelt, new Recipe
            {
                Name = "Wind Mythril Braid Belt",
                TemplateKey = "windmythrilbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineemerald", "Pristine Emerald", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.SeaMythrilBraidBelt, new Recipe
            {
                Name = "Sea Mythril Braid Belt",
                TemplateKey = "seamythrilbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitewool", "Exquisite wool", 2),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.DarkMythrilBraidBelt, new Recipe
            {
                Name = "Dark Mythril Braid Belt",
                TemplateKey = "darkmythrilbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.LightMythrilBraidBelt, new Recipe
            {
                Name = "Light Mythril Braid Belt",
                TemplateKey = "lightmythrilbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 2)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.FireHybrasylBraidBelt, new Recipe
            {
                Name = "Fire Hy-brasyl Braid Belt",
                TemplateKey = "firehybrasylbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristineruby", "Pristine Ruby", 5)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            ArmorsmithingRecipes.EarthHybrasylBraidBelt, new Recipe
            {
                Name = "Earth Hy-brasyl Braid Belt",
                TemplateKey = "earthhybrasylbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristineberyl", "Pristine Beryl", 5)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            ArmorsmithingRecipes.WindHybrasylBraidBelt, new Recipe
            {
                Name = "Wind Hy-brasyl Braid Belt",
                TemplateKey = "windhybrasylbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristineemerald", "Pristine Emerald", 5)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            ArmorsmithingRecipes.SeaHybrasylBraidBelt, new Recipe
            {
                Name = "Sea Hybrasyl Braid Belt",
                TemplateKey = "seahybrasylbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 5)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            ArmorsmithingRecipes.DarkHybrasylBraidBelt, new Recipe
            {
                Name = "Dark Hy-brasyl Braid Belt",
                TemplateKey = "darkhybrasylbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 5)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes.LightHybrasylBraidBelt, new Recipe
            {
                Name = "Light Hy-brasyl Braid Belt",
                TemplateKey = "lighthybrasylbraidbelt",
                Ingredients =
                [
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 1),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 5)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            ArmorsmithingRecipes.LeatherGauntlet, new Recipe
            {
                Name = "Leather Gauntlet",
                TemplateKey = "leathergauntlet",
                Ingredients = [new Ingredient("linen", "Linen", 8)],
                Rank = "Beginner",
                Level = 5,
                Difficulty = 1
            }
        },
        {
            ArmorsmithingRecipes.LeatherSapphireGauntlet, new Recipe
            {
                Name = "Leather Sapphire Gauntlet",
                TemplateKey = "leathersapphiregauntlet",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.LeatherRubyGauntlet, new Recipe
            {
                Name = "Leather Ruby Gauntlet",
                TemplateKey = "leatherrubygauntlet",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("pristineruby", "Pristine Ruby", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.LeatherEmeraldGauntlet, new Recipe
            {
                Name = "Leather Emerald Gauntlet",
                TemplateKey = "leatheremeraldgauntlet",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("pristineemerald", "Pristine Emerald", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.LeatherHeartstoneGauntlet, new Recipe
            {
                Name = "Leather Heartstone Gauntlet",
                TemplateKey = "leatherheartstonegauntlet",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.LeatherBerylGauntlet, new Recipe
            {
                Name = "Leather Beryl Gauntlet",
                TemplateKey = "leatherberylgauntlet",
                Ingredients =
                [
                    new Ingredient("exquisitelinen", "Exquisite Linen", 2),
                    new Ingredient("pristineberyl", "Pristine Beryl", 1)
                ],
                Rank = "Basic",
                Level = 11,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.BronzeSapphireGauntlet, new Recipe
            {
                Name = "Bronze Sapphire Gauntlet",
                TemplateKey = "bronzesapphiregauntlet",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("exquisitelinen", "Exquisite Linen", 1),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 1)
                ],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.BronzeRubyGauntlet, new Recipe
            {
                Name = "Bronze Ruby Gauntlet",
                TemplateKey = "bronzerubygauntlet",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("exquisitelinen", "Exquisite Linen", 1),
                    new Ingredient("pristineruby", "Pristine Ruby", 1)
                ],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.BronzeEmeraldGauntlet, new Recipe
            {
                Name = "Bronze Emerald Gauntlet",
                TemplateKey = "bronzeemeraldgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("exquisitelinen", "Exquisite Linen", 1),
                    new Ingredient("pristineemerald", "Pristine Emerald", 1)
                ],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.BronzeHeartstoneGauntlet, new Recipe
            {
                Name = "Bronze Heartstone Gauntlet",
                TemplateKey = "bronzeheartstonegauntlet",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("exquisitelinen", "Exquisite Linen", 1),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 1)
                ],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.BronzeBerylGauntlet, new Recipe
            {
                Name = "Bronze Beryl Gauntlet",
                TemplateKey = "bronzeberylgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedbronzebar", "Polished Bronze Bar", 1),
                    new Ingredient("exquisitelinen", "Exquisite Linen", 1),
                    new Ingredient("pristineberyl", "Pristine Beryl", 1)
                ],
                Rank = "Basic",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            ArmorsmithingRecipes.IronSapphireGauntlet, new Recipe
            {
                Name = "Iron Sapphire Gauntlet",
                TemplateKey = "ironsapphiregauntlet",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.IronRubyGauntlet, new Recipe
            {
                Name = "Iron Ruby Gauntlet",
                TemplateKey = "ironrubygauntlet",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("pristineruby", "Pristine Ruby", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.IronEmeraldGauntlet, new Recipe
            {
                Name = "Iron Emerald Gauntlet",
                TemplateKey = "ironemeraldgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("pristineemerald", "Pristine Emerald", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.IronHeartstoneGauntlet, new Recipe
            {
                Name = "Iron Heartstone Gauntlet",
                TemplateKey = "ironheartstonegauntlet",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.IronBerylGauntlet, new Recipe
            {
                Name = "Iron Beryl Gauntlet",
                TemplateKey = "ironberylgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedironbar", "Polished Iron Bar", 1),
                    new Ingredient("exquisitecotton", "Exquisite Cotton", 2),
                    new Ingredient("pristineberyl", "Pristine Beryl", 1)
                ],
                Rank = "Initiate",
                Level = 41,
                Difficulty = 3
            }
        },
        {
            ArmorsmithingRecipes.MythrilSapphireGauntlet, new Recipe
            {
                Name = "Mythril Sapphire Gauntlet",
                TemplateKey = "mythrilsapphiregauntlet",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.MythrilRubyGauntlet, new Recipe
            {
                Name = "Mythril Ruby Gauntlet",
                TemplateKey = "mythrilrubygauntlet",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("pristineruby", "Pristine Ruby", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.MythrilEmeraldGauntlet, new Recipe
            {
                Name = "Mythril Emerald Gauntlet",
                TemplateKey = "mythrilemeraldgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("pristineemerald", "Pristine Emerald", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.MythrilHeartstoneGauntlet, new Recipe
            {
                Name = "Mythril Heartstone Gauntlet",
                TemplateKey = "mythrilheartstonegauntlet",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.MythrilBerylGauntlet, new Recipe
            {
                Name = "Mythril Beryl Gauntlet",
                TemplateKey = "mythrilberylgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedmythrilbar", "Polished Mythril Bar", 1),
                    new Ingredient("exquisitewool", "Exquisite Wool", 2),
                    new Ingredient("pristineberyl", "Pristine Beryl", 1)
                ],
                Rank = "Artisan",
                Level = 71,
                Difficulty = 4
            }
        },
        {
            ArmorsmithingRecipes.HybrasylSapphireGauntlet, new Recipe
            {
                Name = "Hy-brasyl Sapphire Gauntlet",
                TemplateKey = "hybrasylsapphiregauntlet",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 2),
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 2)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            ArmorsmithingRecipes.HybrasylRubyGauntlet, new Recipe
            {
                Name = "Hy-brasyl Ruby Gauntlet",
                TemplateKey = "hybrasylrubygauntlet",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 2),
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("pristineruby", "Pristine Ruby", 2)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            ArmorsmithingRecipes.HybrasylEmeraldGauntlet, new Recipe
            {
                Name = "Hy-brasyl Emerald Gauntlet",
                TemplateKey = "hybrasylemeraldgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 2),
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("pristineemerald", "Pristine Emerald", 2)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            ArmorsmithingRecipes.HybrasylHeartstoneGauntlet, new Recipe
            {
                Name = "Hy-brasyl Heartstone Gauntlet",
                TemplateKey = "hybrasylheartstonegauntlet",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 2),
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 2)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        },
        {
            ArmorsmithingRecipes.HybrasylBerylGauntlet, new Recipe
            {
                Name = "Hy-brasyl Beryl Gauntlet",
                TemplateKey = "hybrasylberylgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedhybrasylbar", "Polished Hy-brasyl Bar", 2),
                    new Ingredient("exquisitesilk", "Exquisite Silk", 3),
                    new Ingredient("pristineberyl", "Pristine Beryl", 2)
                ],
                Rank = "Adept",
                Level = 97,
                Difficulty = 5
            }
        }
    };

    public static Dictionary<ArmorsmithingRecipes2, Recipe> ArmorSmithingGearRequirements2 { get; } = new()
    {
        {
            ArmorsmithingRecipes2.WindCrimsoniteBelt, new Recipe
            {
                Name = "Wind Crimsonite Belt",
                TemplateKey = "windcrimsonitebelt",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineemerald", "Pristine Emerald", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.EarthCrimsoniteBelt, new Recipe
            {
                Name = "Earth Crimsonite Belt",
                TemplateKey = "earthcrimsonitebelt",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineberyl", "Pristine Beryl", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.FireCrimsoniteBelt, new Recipe
            {
                Name = "Fire Crimsonite Belt",
                TemplateKey = "firecrimsonitebelt",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineruby", "Pristine Ruby", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.SeaCrimsoniteBelt, new Recipe
            {
                Name = "Sea Crimsonite Belt",
                TemplateKey = "seacrimsonitebelt",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.DarkCrimsoniteBelt, new Recipe
            {
                Name = "Dark Crimsonite Belt",
                TemplateKey = "darkcrimsonitebelt",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.LightCrimsoniteBelt, new Recipe
            {
                Name = "Light Crimsonite Belt",
                TemplateKey = "lightcrimsonitebelt",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.WindAzuriumBelt, new Recipe
            {
                Name = "Wind Azurium Belt",
                TemplateKey = "windazuriumbelt",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineemerald", "Pristine Emerald", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.EarthAzuriumBelt, new Recipe
            {
                Name = "Earth Azurium Belt",
                TemplateKey = "earthazuriumbelt",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineberyl", "Pristine Beryl", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.FireAzuriumBelt, new Recipe
            {
                Name = "Fire Azurium Belt",
                TemplateKey = "fireazuriumbelt",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineruby", "Pristine Ruby", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.SeaAzuriumBelt, new Recipe
            {
                Name = "Sea Azurium Belt",
                TemplateKey = "seaazuriumbelt",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.DarkAzuriumBelt, new Recipe
            {
                Name = "Dark Azurium Belt",
                TemplateKey = "darkazuriumbelt",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.LightAzuriumBelt, new Recipe
            {
                Name = "Light Azurium Belt",
                TemplateKey = "lightazuriumbelt",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 1),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 7)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.CrimsoniteRubyGauntlet, new Recipe
            {
                Name = "Crimsonite Ruby Gauntlet",
                TemplateKey = "crimsoniterubygauntlet",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineruby", "Pristine Ruby", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.CrimsoniteSapphireGauntlet, new Recipe
            {
                Name = "Crimsonite Sapphire Gauntlet",
                TemplateKey = "crimsonitesapphiregauntlet",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.CrimsoniteEmeraldGauntlet, new Recipe
            {
                Name = "Crimsonite Emerald Gauntlet",
                TemplateKey = "crimsoniteemeraldgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineemerald", "Pristine Emerald", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.CrimsoniteHeartstoneGauntlet, new Recipe
            {
                Name = "Crimsonite Heartstone Gauntlet",
                TemplateKey = "crimsoniteheartstonegauntlet",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.CrimsoniteBerylGauntlet, new Recipe
            {
                Name = "Crimsonite Beryl Gauntlet",
                TemplateKey = "crimsoniteberylgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedcrimsonitebar", "Polished Crimsonite Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineberyl", "Pristine Beryl", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.AzuriumRubyGauntlet, new Recipe
            {
                Name = "Azurium Ruby Gauntlet",
                TemplateKey = "azuriumrubygauntlet",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineruby", "Pristine Ruby", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.AzuriumSapphireGauntlet, new Recipe
            {
                Name = "Azurium Sapphire Gauntlet",
                TemplateKey = "azuriumsapphiregauntlet",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristinesapphire", "Pristine Sapphire", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.AzuriumEmeraldGauntlet, new Recipe
            {
                Name = "Azurium Emerald Gauntlet",
                TemplateKey = "azuriumemeraldgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineemerald", "Pristine Emerald", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.AzuriumHeartstoneGauntlet, new Recipe
            {
                Name = "Azurium Heartstone Gauntlet",
                TemplateKey = "azuriumheartstonegauntlet",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineheartstone", "Pristine Heartstone", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        },
        {
            ArmorsmithingRecipes2.AzuriumBerylGauntlet, new Recipe
            {
                Name = "Azurium Beryl Gauntlet",
                TemplateKey = "azuriumberylgauntlet",
                Ingredients =
                [
                    new Ingredient("polishedazuriumbar", "Polished Azurium Bar", 3),
                    new Ingredient("exquisitehemp", "Exquisite Hemp", 4),
                    new Ingredient("pristineberyl", "Pristine Beryl", 3)
                ],
                Rank = "Advanced",
                Level = 99,
                Difficulty = 6
            }
        }
    };
    #endregion
}