using Chaos.Models.Panel;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ItemScripts.Enchantments;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Definitions;

public static class CraftingRequirements
{
    public sealed class Recipe
    {
        public string Name { get; set; } = null!;
        public string TemplateKey { get; set; } = null!;
        public List<Ingredient> Ingredients { get; set; } = null!;
        public string Rank { get; set; } = null!;
        public int Level { get; set; }
        public int Difficulty { get; set; }
        public Func<ITypeMapper, Item, Item>? Modification { get; set; }
    }

    public sealed class Ingredient
    {
        public string TemplateKey { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public int Amount { get; set; }
    }

    #region Enchanting Recipes
    public static Dictionary<EnchantingRecipes, Recipe> EnchantingRequirements { get; } = new()
    {
        {
            EnchantingRecipes.IgnatarEnvy,
            new Recipe()
            {
                Name = "Ignatar's Envy",
                TemplateKey = "ignatarenvy",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofignatar", DisplayName = "Essence of Ignatar", Amount = 1 },
                },
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SwiftPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
        },
        {
            EnchantingRecipes.IgnatarGrief,
            new Recipe()
            {
                Name = "Ignatar's Grief",
                TemplateKey = "ignatargrief",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofignatar", DisplayName = "Essence of Ignatar", Amount = 1 },
                },
                Rank = "Basic",
                Level = 24,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(MinorPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
        },
        {
            EnchantingRecipes.IgnatarRegret,
            new Recipe()
            {
                Name = "Ignatar's Regret",
                TemplateKey = "ignatarregret",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofignatar", DisplayName = "Essence of Ignatar", Amount = 3 },
                },
                Rank = "Apprentice",
                Level = 48,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(FieryPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
        },
        {
            EnchantingRecipes.IgnatarJealousy,
            new Recipe()
            {
                Name = "Ignatar's Jealousy",
                TemplateKey = "ignatarjealousy",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofignatar", DisplayName = "Essence of Ignatar", Amount = 5 },
                },
                Rank = "Apprentice",
                Level = 60,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(CripplingPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
        },
        {
            EnchantingRecipes.IgnatarDestruction,
            new Recipe()
            {
                Name = "Ignatar's Destruction",
                TemplateKey = "ignatardestruction",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofignatar", DisplayName = "Essence of Ignatar", Amount = 7 },
                },
                Rank = "Journeyman",
                Level = 80,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(RuthlessPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        
        {
            EnchantingRecipes.GeolithGratitude,
            new Recipe()
            {
                Name = "Geolith's Gratitude",
                TemplateKey = "geolithgratitude",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofgeolith", DisplayName = "Essence of Geolith", Amount = 1 },
                },
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SkillfulPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.GeolithPride,
            new Recipe()
            {
                Name = "Geolith's Pride",
                TemplateKey = "geolithpride",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofgeolith", DisplayName = "Essence of Geolith", Amount = 3 },
                },
                Rank = "Basic",
                Level = 28,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(ModestPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.GeolithConstitution,
            new Recipe()
            {
                Name = "Geolith's Constitution",
                TemplateKey = "geolithconstitution",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofgeolith", DisplayName = "Essence of Geolith", Amount = 5},
                },
                Rank = "Apprentice",
                Level = 50,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(HalePrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.GeolithObsession,
            new Recipe()
            {
                Name = "Geolith's Obsession",
                TemplateKey = "geolithobsession",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofgeolith", DisplayName = "Essence of Geolith", Amount = 5 },
                },
                Rank = "Apprentice",
                Level = 65,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(PowerfulPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.GeolithFortitude,
            new Recipe()
            {
                Name = "Geolith's Fortitude",
                TemplateKey = "geolithfortitude",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofgeolith", DisplayName = "Essence of Geolith", Amount = 7 },
                },
                Rank = "Journeyman",
                Level = 83,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(EternalPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.MiraelisSerenity,
            new Recipe()
            {
                Name = "Miraelis' Serenity",
                TemplateKey = "miraelisserenity",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofmiraelis", DisplayName = "Essence of Miraelis", Amount = 1 },
                },
                Rank = "Beginner",
                Level = 5,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(MysticalPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.MiraelisBlessing,
            new Recipe()
            {
                Name = "Miraelis' Blessing",
                TemplateKey = "miraelisblessing",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofmiraelis", DisplayName = "Essence of Miraelis", Amount = 3 },
                },
                Rank = "Basic",
                Level = 34,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(TinyPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.MiraelisIntellect,
            new Recipe()
            {
                Name = "Miraelis' Intellect",
                TemplateKey = "miraelisintellect",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofmiraelis", DisplayName = "Essence of Miraelis", Amount = 5 },
                },
                Rank = "Apprentice",
                Level = 50,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(BrilliantPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.MiraelisHarmony,
            new Recipe()
            {
                Name = "Miraelis' Harmony",
                TemplateKey = "miraelisharmony",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofmiraelis", DisplayName = "Essence of Miraelis", Amount = 5 },
                },
                Rank = "Apprentice",
                Level = 69,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(BrightPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.MiraelisNurturing,
            new Recipe()
            {
                Name = "Miraelis' Nurturing",
                TemplateKey = "miraelisnurturing",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofmiraelis", DisplayName = "Essence of Miraelis", Amount = 7 },
                },
                Rank = "Journeyman",
                Level = 88,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(AncientPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.TheseleneElusion,
            new Recipe()
            {
                Name = "Theselene's Elusion",
                TemplateKey = "theseleneelusion",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceoftheselene", DisplayName = "Essence of Theselene", Amount = 1 },
                },
                Rank = "Beginner",
                Level = 5,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(ShroudedPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.TheseleneShadow,
            new Recipe()
            {
                Name = "Theselene's Shadow",
                TemplateKey = "theseleneshadow",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceoftheselene", DisplayName = "Essence of Theselene", Amount = 3 }
                },
                Rank = "Basic",
                Level = 37,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(DarkPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.TheseleneDexterity,
            new Recipe()
            {
                Name = "Theselene's Dexterity",
                TemplateKey = "theselenedexterity",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceoftheselene", DisplayName = "Essence of Theselene", Amount = 5 }
                },
                Rank = "Apprentice",
                Level = 50,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(NimblePrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.TheseleneBalance,
            new Recipe()
            {
                Name = "Theselene's Balance",
                TemplateKey = "theselenebalance",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceoftheselene", DisplayName = "Essence of Theselene", Amount = 7 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(TightPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.TheseleneRisk,
            new Recipe()
            {
                Name = "Theselene's Risk",
                TemplateKey = "theselenerisk",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceoftheselene", DisplayName = "Essence of Theselene", Amount = 10 }
                },
                Rank = "Adept",
                Level = 90,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(CursedPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.AquaedonClarity,
            new Recipe()
            {
                Name = "Aquaedon's Clarity",
                TemplateKey = "aquaedonclarity",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofaquaedon", DisplayName = "Essence of Aquaedon", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 5,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(MeagerPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.AquaedonCalming,
            new Recipe()
            {
                Name = "Aquaedon's Calming",
                TemplateKey = "aquaedoncalming",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofaquaedon", DisplayName = "Essence of Aquaedon", Amount = 3 }
                },
                Rank = "Basic",
                Level = 40,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SerenePrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.AquaedonWisdom,
            new Recipe()
            {
                Name = "Aquaedon's Wisdom",
                TemplateKey = "aquaedonwisdom",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofaquaedon", DisplayName = "Essence of Aquaedon", Amount = 5 }
                },
                Rank = "Apprentice",
                Level = 50,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(WisePrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.AquaedonWill,
            new Recipe()
            {
                Name = "Aquaedon's Will",
                TemplateKey = "aquaedonwill",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofaquaedon", DisplayName = "Essence of Aquaedon", Amount = 7 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(PotentPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.AquaedonResolve,
            new Recipe()
            {
                Name = "Aquaedon's Resolve",
                TemplateKey = "aquaedonresolve",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofaquaedon", DisplayName = "Essence of Aquaedon", Amount = 10 }
                },
                Rank = "Adept",
                Level = 90,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SoothingPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SerendaelLuck,
            new Recipe()
            {
                Name = "Serendael's Luck",
                TemplateKey = "serendaelluck",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofserendael", DisplayName = "Essence of Serendael", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(LuckyPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SerendaelMagic,
            new Recipe()
            {
                Name = "Serendael's Magic",
                TemplateKey = "serendaelmagic",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofserendael", DisplayName = "Essence of Serendael", Amount = 3 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(LightPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SerendaelChance,
            new Recipe()
            {
                Name = "Serendael's Chance",
                TemplateKey = "serendaelchance",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofserendael", DisplayName = "Essence of Serendael", Amount = 5 }
                },
                Rank = "Apprentice",
                Level = 55,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(FocusedPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SerendaelRoll,
            new Recipe()
            {
                Name = "Serendael's Roll",
                TemplateKey = "serendaelroll",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofserendael", DisplayName = "Essence of Serendael", Amount = 7 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(PrecisionPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SerendaelAddiction,
            new Recipe()
            {
                Name = "Serendael's Addiction",
                TemplateKey = "serendaeladdiction",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofserendael", DisplayName = "Essence of Serendael", Amount = 10 }
                },
                Rank = "Adept",
                Level = 90,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(PersistingPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SkandaraMight,
            new Recipe()
            {
                Name = "Skandara's Might",
                TemplateKey = "skandaramight",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofskandara", DisplayName = "Essence of Skandara", Amount = 1 }
                },
                Rank = "Basic",
                Level = 16,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(MightyPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SkandaraTriumph,
            new Recipe()
            {
                Name = "Skandara's Triumph",
                TemplateKey = "skandaratriumph",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofskandara", DisplayName = "Essence of Skandara", Amount = 3 }
                },
                Rank = "Apprentice",
                Level = 44,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(ValiantPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SkandaraStrength,
            new Recipe()
            {
                Name = "Skandara's Strength",
                TemplateKey = "skandarastrength",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofskandara", DisplayName = "Essence of Skandara", Amount = 5 }
                },
                Rank = "Apprentice",
                Level = 50,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(ToughPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SkandaraDrive,
            new Recipe()
            {
                Name = "Skandara's Drive",
                TemplateKey = "skandaradrive",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofskandara", DisplayName = "Essence of Skandara", Amount = 7 }
                },
                Rank = "Journeyman",
                Level = 75,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SavagePrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.SkandaraPierce,
            new Recipe()
            {
                Name = "Skandara's Pierce",
                TemplateKey = "skandarapierce",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofskandara", DisplayName = "Essence of Skandara", Amount = 10 }
                },
                Rank = "Adept",
                Level = 95,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(BlazingPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.ZephyraSpirit,
            new Recipe()
            {
                Name = "Zephyra's Spirit",
                TemplateKey = "zephyraspirit",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofzephyra", DisplayName = "Essence of Zephyra", Amount = 1 }
                },
                Rank = "Basic",
                Level = 20,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(BreezyPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.ZephyraMist,
            new Recipe()
            {
                Name = "Zephyra's Mist",
                TemplateKey = "zephyramist",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofzephyra", DisplayName = "Essence of Zephyra", Amount = 3 }
                },
                Rank = "Apprentice",
                Level = 45,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SoftPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
            },
        {
            EnchantingRecipes.ZephyraWind,
            new Recipe()
            {
                Name = "Zephyra's Wind",
                TemplateKey = "zephyrawind",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofzephyra", DisplayName = "Essence of Zephyra", Amount = 5 }
                },
                Rank = "Apprentice",
                Level = 58,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(HazyPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
        },
        {
            EnchantingRecipes.ZephyraVortex,
            new Recipe()
            {
                Name = "Zephyra's Vortex",
                TemplateKey = "zephyravortex",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofzephyra", DisplayName = "Essence of Zephyra", Amount = 7 }
                },
                Rank = "Journeyman",
                Level = 78,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(WhirlingPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
        },
        {
            EnchantingRecipes.ZephyraGust,
            new Recipe()
            {
                Name = "Zephyra's Gust",
                TemplateKey = "zephyragust",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "essenceofzephyra", DisplayName = "Essence of Zephyra", Amount = 10 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(HowlingPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);

                    return mapper.Map<Item>(schema);
                })
            }
        },
    };

    #endregion
    public static Dictionary<AlchemyRecipes, Recipe> AlchemyRequirements { get; } = new()
    {
        {
            AlchemyRecipes.Hemloch,
            new Recipe()
            {
                Name = "Hemloch Formula",
                TemplateKey = "hemlochformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallHealthPotion,
            new Recipe()
            {
                Name = "Small Health Potion Formula",
                TemplateKey = "smallhealthpotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
        AlchemyRecipes.SmallManaPotion,
        new Recipe()
        {
            Name = "Small Mana Potion Formula",
            TemplateKey = "smallmanapotionformula",
            Ingredients = new List<Ingredient>()
            {
                new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            },
            Rank = "Beginner",
            Level = 1,
            Difficulty = 1
        }
    },
        {
        AlchemyRecipes.SmallRejuvenationPotion,
        new Recipe()
        {
            Name = "Small Rejuvenation Potion Formula",
            TemplateKey = "smallrejuvenationpotionformula",
            Ingredients = new List<Ingredient>()
            {
                new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            },
            Rank = "Beginner",
            Level = 1,
            Difficulty = 1
        }
    },
        {
            AlchemyRecipes.SmallHasteBrew,
            new Recipe()
            {
                Name = "Small Haste Brew Formula",
                TemplateKey = "smallhastebrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallPowerBrew,
            new Recipe()
            {
                Name = "Small Power Brew Formula",
                TemplateKey = "smallpowerbrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallAccuracyPotion,
            new Recipe()
            {
                Name = "Small Accuracy Potion Formula",
                TemplateKey = "smallaccuracypotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.JuggernautBrew,
            new Recipe()
            {
                Name = "Juggernaut Brew Formula",
                TemplateKey = "juggernautbrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.AstralBrew,
            new Recipe()
            {
                Name = "Astral Brew Formula",
                TemplateKey = "Astralbrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.AntidotePotion,
            new Recipe()
            {
                Name = "Antidote Potion Formula",
                TemplateKey = "antidotepotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallFirestormTonic,
            new Recipe()
            {
                Name = "Small Firestorm Tonic Formula",
                TemplateKey = "smallfirestormtonicformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.SmallStunTonic,
            new Recipe()
            {
                Name = "Small Stun Tonic Formula",
                TemplateKey = "smallstuntonicformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.HealthPotion,
            new Recipe()
            {
                Name = "Health Potion Formula",
                TemplateKey = "healthpotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.ManaPotion,
            new Recipe()
            {
                Name = "Mana Potion Formula",
                TemplateKey = "manapotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.RejuvenationPotion,
            new Recipe()
            {
                Name = "Rejuvenation Potion Formula",
                TemplateKey = "rejuvenationpotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.HasteBrew,
            new Recipe()
            {
                Name = "Haste Brew Formula",
                TemplateKey = "hastebrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.PowerBrew,
            new Recipe()
            {
                Name = "Power Brew Formula",
                TemplateKey = "powerbrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.AccuracyPotion,
            new Recipe()
            {
                Name = "Accuracy Potion Formula",
                TemplateKey = "accuracypotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.RevivePotion,
            new Recipe()
            {
                Name = "Revive Potion Formula",
                TemplateKey = "revivepotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StrongJuggernautBrew,
            new Recipe()
            {
                Name = "Strong Juggernaut Brew Formula",
                TemplateKey = "strongjuggernautbrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StrongAstralBrew,
            new Recipe()
            {
                Name = "Strong Astral Brew Formula",
                TemplateKey = "strongAstralbrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.CleansingBrew,
            new Recipe()
            {
                Name = "Cleansing Brew Formula",
                TemplateKey = "cleansingbrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.FirestormTonic,
            new Recipe()
            {
                Name = "Firestorm Tonic Formula",
                TemplateKey = "firestormtonicformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StunTonic,
            new Recipe()
            {
                Name = "Stun Tonic Formula",
                TemplateKey = "stuntonicformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.WarmthPotion,
            new Recipe()
            {
                Name = "Warmth Potion Formula",
                TemplateKey = "warmthpotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.AmnesiaBrew,
            new Recipe()
            {
                Name = "Amnesia Brew Formula",
                TemplateKey = "amnesiaBrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StrongHealthPotion,
            new Recipe()
            {
                Name = "Strong Health Potion Formula",
                TemplateKey = "stronghealthpotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StrongManaPotion,
            new Recipe()
            {
                Name = "Strong Mana Potion Formula",
                TemplateKey = "strongmanapotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StrongRejuvenationPotion,
            new Recipe()
            {
                Name = "Strong Rejuvenation Potion Formula",
                TemplateKey = "strongrejuvenationpotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StrongHasteBrew,
            new Recipe()
            {
                Name = "Strong Haste Brew Formula",
                TemplateKey = "stronghastebrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StrongPowerBrew,
            new Recipe()
            {
                Name = "Strong Power Brew Formula",
                TemplateKey = "strongpowerbrewformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StrongAccuracyPotion,
            new Recipe()
            {
                Name = "Strong Accuracy Potion Formula",
                TemplateKey = "strongaccuracypotionformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.StatBoostElixir,
            new Recipe()
            {
                Name = "Stat Boost Elixir Formula",
                TemplateKey = "statboostelixirformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.KnowledgeElixir,
            new Recipe()
            {
                Name = "Knowledge Elixir Formula",
                TemplateKey = "knowledgeelixirformula",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1
            }
        },
    };

    #region Jewelcrafting

    public static Dictionary<JewelcraftingRecipes, Recipe> JewelcraftingRequirements { get; } = new()
    {
        {
            JewelcraftingRecipes.BasicBerylEarrings,
            new Recipe()
            {
                Name = "Basic Beryl Earrings",
                TemplateKey = "basicberylearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedberyl", DisplayName = "Flawed Beryl", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BasicRubyEarrings,
            new Recipe()
            {
                Name = "Basic Ruby Earrings",
                TemplateKey = "basicrubyearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedruby", DisplayName = "Flawed Ruby", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BasicSapphireEarrings,
            new Recipe()
            {
                Name = "Basic Sapphire Earrings",
                TemplateKey = "basicsapphireearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedsapphire", DisplayName = "Flawed Sapphire", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BasicEmeraldEarrings,
            new Recipe()
            {
                Name = "Basic Emerald Earrings",
                TemplateKey = "basicemeraldearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedemerald", DisplayName = "Flawed Emerald", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BasicHeartstoneEarrings,
            new Recipe()
            {
                Name = "Basic Heartstone Earrings",
                TemplateKey = "basicheartstoneearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedheartstone", DisplayName = "Flawed Heartstone", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronBerylEarrings,
            new Recipe()
            {
                Name = "Iron Beryl Earrings",
                TemplateKey = "ironberylearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutberyl", DisplayName = "Uncut Beryl", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedberyl", DisplayName = "Finished Beryl", Amount = 1 },
                new Ingredient { TemplateKey = "flawedberyl", DisplayName = "Flawed Beryl", Amount = 1 },
            new Ingredient { TemplateKey = "mantiseye", DisplayName = "Mantis Eye", Amount = 1 }
        
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronRubyEarrings,
            new Recipe()
            {
                Name = "Iron Ruby Earrings",
                TemplateKey = "ironrubyearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutruby", DisplayName = "Uncut Ruby", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronSapphireEarrings,
            new Recipe()
            {
                Name = "Iron Sapphire Earrings",
                TemplateKey = "ironsapphireearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutsapphire", DisplayName = "Uncut Sapphire", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronEmeraldEarrings,
            new Recipe()
            {
                Name = "Iron Emerald Earrings",
                TemplateKey = "ironemeraldearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutemerald", DisplayName = "Uncut Emerald", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronHeartstoneEarrings,
            new Recipe()
            {
                Name = "Iron Heartstone Earrings",
                TemplateKey = "ironheartstoneearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutheartstone", DisplayName = "Uncut Heartstone", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
                {
            JewelcraftingRecipes.MythrilBerylEarrings,
            new Recipe()
            {
                Name = "Mythril Beryl Earrings",
                TemplateKey = "mythrilberylearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedberyl", DisplayName = "Finished Beryl", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.MythrilRubyEarrings,
            new Recipe()
            {
                Name = "Mythril Ruby Earrings",
                TemplateKey = "mythrilrubyearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedruby", DisplayName = "Finished Ruby", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.MythrilSapphireEarrings,
            new Recipe()
            {
                Name = "Mythril Sapphire Earrings",
                TemplateKey = "mythrilsapphireearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedsapphire", DisplayName = "Finished Sapphire", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.MythrilEmeraldEarrings,
            new Recipe()
            {
                Name = "Mythril Emerald Earrings",
                TemplateKey = "mythrilemeraldearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedemerald", DisplayName = "Finished Emerald", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.MythrilHeartstoneEarrings,
            new Recipe()
            {
                Name = "Mythril Heartstone Earrings",
                TemplateKey = "mythrilheartstoneearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedheartstone", DisplayName = "Finished Heartstone", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
                        {
            JewelcraftingRecipes.HybrasylBerylEarrings,
            new Recipe()
            {
                Name = "Hybrasyl Beryl Earrings",
                TemplateKey = "hybrasylberylearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedberyl", DisplayName = "Finished Beryl", Amount = 3 }
                },
                Rank = "Adept",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.HybrasylRubyEarrings,
            new Recipe()
            {
                Name = "Hybrasyl Ruby Earrings",
                TemplateKey = "hybrasylrubyearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedruby", DisplayName = "Finished Ruby", Amount = 3 }
                },
                Rank = "Adept",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.HybrasylSapphireEarrings,
            new Recipe()
            {
                Name = "Hybrasyl Sapphire Earrings",
                TemplateKey = "hybrasylsapphireearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedsapphire", DisplayName = "Finished Sapphire", Amount = 3 }
                },
                Rank = "Adept",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.HybrasylEmeraldEarrings,
            new Recipe()
            {
                Name = "Hybrasyl Emerald Earrings",
                TemplateKey = "hybrasylemeraldearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedemerald", DisplayName = "Finished Emerald", Amount = 3 }
                },
                Rank = "Adept",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.HybrasylHeartstoneEarrings,
            new Recipe()
            {
                Name = "Hybrasyl Heartstone Earrings",
                TemplateKey = "hybrasylheartstoneearrings",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedheartstone", DisplayName = "Finished Heartstone", Amount = 3 }
                },
                Rank = "Adept",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.SmallRubyRing,
            new Recipe()
            {
                Name = "Small Ruby Ring",
                TemplateKey = "smallrubyring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "rawruby", DisplayName = "Raw Ruby", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BerylRing,
            new Recipe()
            {
                Name = "Beryl Ring",
                TemplateKey = "berylring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "rawberyl", DisplayName = "Raw Beryl", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BronzeBerylRing,
            new Recipe()
            {
                Name = "Bronze Beryl Ring",
                TemplateKey = "bronzeberylring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedberyl", DisplayName = "Flawed Beryl", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BronzeRubyRing,
            new Recipe()
            {
                Name = "Bronze Ruby Ring",
                TemplateKey = "bronzerubyring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedruby", DisplayName = "Flawed Ruby", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BronzeSapphireRing,
            new Recipe()
            {
                Name = "Bronze Sapphire Ring",
                TemplateKey = "bronzesapphirering",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedsapphire", DisplayName = "Flawed Sapphire", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BronzeEmeraldRing,
            new Recipe()
            {
                Name = "Bronze Emerald Ring",
                TemplateKey = "bronzeemeraldring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedemerald", DisplayName = "Flawed Emerald", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BronzeHeartstoneRing,
            new Recipe()
            {
                Name = "Bronze Heartstone Ring",
                TemplateKey = "bronzeheartstonering",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedheartstone", DisplayName = "Flawed Heartstone", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronBerylRing,
            new Recipe()
            {
                Name = "Iron Beryl Ring",
                TemplateKey = "ironberylring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutberyl", DisplayName = "Uncut Beryl", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronRubyRing,
            new Recipe()
            {
                Name = "Iron Ruby Ring",
                TemplateKey = "ironrubyring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutruby", DisplayName = "Uncut Ruby", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronSapphireRing,
            new Recipe()
            {
                Name = "Iron Sapphire Ring",
                TemplateKey = "ironsapphirering",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutsapphire", DisplayName = "Uncut Sapphire", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronEmeraldRing,
            new Recipe()
            {
                Name = "Iron Emerald Ring",
                TemplateKey = "ironemeraldring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutemerald", DisplayName = "Uncut Emerald", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.IronHeartstoneRing,
            new Recipe()
            {
                Name = "Iron Heartstone Ring",
                TemplateKey = "ironheartstonering",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutheartstone", DisplayName = "Uncut Heartstone", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
                {
            JewelcraftingRecipes.MythrilBerylRing,
            new Recipe()
            {
                Name = "Mythril Beryl Ring",
                TemplateKey = "mythrilberylring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedberyl", DisplayName = "Finished Beryl", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.MythrilRubyRing,
            new Recipe()
            {
                Name = "Mythril Ruby Ring",
                TemplateKey = "mythrilrubyring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedruby", DisplayName = "Finished Ruby", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.MythrilSapphireRing,
            new Recipe()
            {
                Name = "Mythril Sapphire Ring",
                TemplateKey = "mythrilsapphirering",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedsapphire", DisplayName = "Finished Sapphire", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.MythrilEmeraldRing,
            new Recipe()
            {
                Name = "Mythril Emerald Ring",
                TemplateKey = "mythrilemeraldring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedemerald", DisplayName = "Finished Emerald", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.MythrilHeartstoneRing,
            new Recipe()
            {
                Name = "Mythril Heartstone Ring",
                TemplateKey = "mythrilheartstonering",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedheartstone", DisplayName = "Finished Heartstone", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
                        {
            JewelcraftingRecipes.HybrasylBerylRing,
            new Recipe()
            {
                Name = "Hy-Brasyl Beryl Ring",
                TemplateKey = "hybrasylberylring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedberyl", DisplayName = "Finished Beryl", Amount = 3 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.HybrasylRubyRing,
            new Recipe()
            {
                Name = "Hy-Brasyl Ruby Ring",
                TemplateKey = "hybrasylrubyring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedruby", DisplayName = "Finished Ruby", Amount = 3 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.HybrasylSapphireRing,
            new Recipe()
            {
                Name = "Hy-Brasyl Sapphire Ring",
                TemplateKey = "hybrasylsapphirering",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedsapphire", DisplayName = "Finished Sapphire", Amount = 3 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.HybrasylEmeraldRing,
            new Recipe()
            {
                Name = "Hy-Brasyl Emerald Ring",
                TemplateKey = "hybrasylemeraldring",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedemerald", DisplayName = "Finished Emerald", Amount = 3 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.HybrasylHeartstoneRing,
            new Recipe()
            {
                Name = "Hy-Brasyl Heartstone Ring",
                TemplateKey = "hybrasylheartstonering",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedheartstone", DisplayName = "Finished Heartstone", Amount = 3 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.FireNecklace,
            new Recipe()
            {
                Name = "Fire Necklace",
                TemplateKey = "firenecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "rawruby", DisplayName = "Raw Ruby", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.SeaNecklace,
            new Recipe()
            {
                Name = "Sea Necklace",
                TemplateKey = "seanecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "rawsapphire", DisplayName = "Raw Sapphire", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.WindNecklace,
            new Recipe()
            {
                Name = "Wind Necklace",
                TemplateKey = "windnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "rawemerald", DisplayName = "Raw Emerald", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.EarthNecklace,
            new Recipe()
            {
                Name = "Earth Necklace",
                TemplateKey = "earthnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "rawberyl", DisplayName = "Raw Beryl", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BoneEarthNecklace,
            new Recipe()
            {
                Name = "Bone Earth Necklace",
                TemplateKey = "boneearthnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedberyl", DisplayName = "Flawed Beryl", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BoneFireNecklace,
            new Recipe()
            {
                Name = "Bone Fire Necklace",
                TemplateKey = "bonefirenecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedruby", DisplayName = "Flawed Ruby", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BoneSeaNecklace,
            new Recipe()
            {
                Name = "Bone Sea Necklace",
                TemplateKey = "boneseanecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedsapphire", DisplayName = "Flawed Sapphire", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.BoneWindNecklace,
            new Recipe()
            {
                Name = "Bone Wind Necklace",
                TemplateKey = "bonewindnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedbronze", DisplayName = "Polished Bronze", Amount = 1 },
                    new Ingredient { TemplateKey = "flawedemerald", DisplayName = "Flawed Emerald", Amount = 1}
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.KannaEarthNecklace,
            new Recipe()
            {
                Name = "Kanna Earth Necklace",
                TemplateKey = "kannaearthnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishediron", DisplayName = "Polished Iron", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutberyl", DisplayName = "Uncut Beryl", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.KannaWindNecklace,
            new Recipe()
            {
                Name = "Kanna Wind Necklace",
                TemplateKey = "kannawindnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishediron", DisplayName = "Polished Iron", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutemerald", DisplayName = "Uncut Emerald", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.KannaFireNecklace,
            new Recipe()
            {
                Name = "Kanna Fire Necklace",
                TemplateKey = "kannafirenecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishediron", DisplayName = "Polished Iron", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutruby", DisplayName = "Uncut Ruby", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.KannaSeaNecklace,
            new Recipe()
            {
                Name = "Kanna Sea Necklace",
                TemplateKey = "kannaseanecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishediron", DisplayName = "Polished Iron", Amount = 1 },
                    new Ingredient { TemplateKey = "uncutsapphire", DisplayName = "Uncut Sapphire", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.PolishedEarthNecklace,
            new Recipe()
            {
                Name = "Polished Earth Necklace",
                TemplateKey = "polishedearthnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedmythril", DisplayName = "Polished Mythril", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedberyl", DisplayName = "Finished Beryl", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.PolishedWindNecklace,
            new Recipe()
            {
                Name = "Polished Wind Necklace",
                TemplateKey = "polishedwindnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedmythril", DisplayName = "Polished Mythril", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedemerald", DisplayName = "Finished Emerald", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.PolishedFireNecklace,
            new Recipe()
            {
                Name = "Polished Fire Necklace",
                TemplateKey = "polishedfirenecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedmythril", DisplayName = "Polished Mythril", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedruby", DisplayName = "Finished Ruby", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.PolishedSeaNecklace,
            new Recipe()
            {
                Name = "Polished Sea Necklace",
                TemplateKey = "polishedseanecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedmythril", DisplayName = "Polished Mythril", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedsapphire", DisplayName = "Finished Sapphire", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.StarSeaNecklace,
            new Recipe()
            {
                Name = "Star Sea Necklace",
                TemplateKey = "starseanecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedhybrasyl", DisplayName = "Polished Hy-Brasyl", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedsapphire", DisplayName = "Finished Sapphire", Amount = 3 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.StarFireNecklace,
            new Recipe()
            {
                Name = "Star Fire Necklace",
                TemplateKey = "starfirenecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedhybrasyl", DisplayName = "Polished Hy-Brasyl", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedruby", DisplayName = "Finished Ruby", Amount = 3 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.StarEarthNecklace,
            new Recipe()
            {
                Name = "Star Earth Necklace",
                TemplateKey = "starearthnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedhybrasyl", DisplayName = "Polished Hy-Brasyl", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedberyl", DisplayName = "Finished Beryl", Amount = 3 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
            }
        },
        {
            JewelcraftingRecipes.StarWindNecklace,
            new Recipe()
            {
                Name = "Star Wind Necklace",
                TemplateKey = "starwindnecklace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "polishedhybrasyl", DisplayName = "Polished Hy-Brasyl", Amount = 1 },
                    new Ingredient { TemplateKey = "finishedemerald", DisplayName = "Finished Emerald", Amount = 3 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
            }
        },
    };


    #endregion

    #region Weapon Smithing
    public static Dictionary<WeaponSmithingRecipes, Recipe> WeaponSmithingCraftRequirements { get; } = new()
    {
        {
            WeaponSmithingRecipes.Eppe,
            new Recipe()
            {
                Name = "Eppe",
                TemplateKey = "eppe",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 2,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Saber,
            new Recipe()
            {
                Name = "Saber",
                TemplateKey = "saber",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 7,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Claidheamh,
            new Recipe()
            {
                Name = "Claidheamh",
                TemplateKey = "claidheamh",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BroadSword,
            new Recipe()
            {
                Name = "Broad Sword",
                TemplateKey = "broadsword",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 17,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BattleSword,
            new Recipe()
            {
                Name = "Battle Sword",
                TemplateKey = "battlesword",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 26,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Masquerade,
            new Recipe()
            {
                Name = "Masquerade",
                TemplateKey = "masquerade",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 31,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Bramble,
            new Recipe()
            {
                Name = "Bramble",
                TemplateKey = "bramble",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.LongSword,
            new Recipe()
            {
                Name = "Long Sword",
                TemplateKey = "longsword",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 56,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Claidhmore,
            new Recipe()
            {
                Name = "Claidhmore",
                TemplateKey = "claidhmore",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.EmeraldSword,
            new Recipe()
            {
                Name = "Emerald Sword",
                TemplateKey = "emeraldsword",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 77,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Gladius,
            new Recipe()
            {
                Name = "Gladius",
                TemplateKey = "gladius",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 85,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Kindjal,
            new Recipe()
            {
                Name = "Kindjal",
                TemplateKey = "kindjal",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 90,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.DragonSlayer,
            new Recipe()
            {
                Name = "Dragon Slayer",
                TemplateKey = "dragonslayer",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 99,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Hatchet,
            new Recipe()
            {
                Name = "Hatchet",
                TemplateKey = "hatchet",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Harpoon,
            new Recipe()
            {
                Name = "Harpoon",
                TemplateKey = "harpoon",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 21,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Scimitar,
            new Recipe()
            {
                Name = "Scimitar",
                TemplateKey = "scimitar",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 31,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Club,
            new Recipe()
            {
                Name = "Club",
                TemplateKey = "club",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.SpikedClub,
            new Recipe()
            {
                Name = "Spiked Club",
                TemplateKey = "spikedclub",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 50,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.ChainMace,
            new Recipe()
            {
                Name = "Chain Mace",
                TemplateKey = "chainmace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 60,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.HandAxe,
            new Recipe()
            {
                Name = "Handaxe",
                TemplateKey = "handaxe",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 70,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Cutlass,
            new Recipe()
            {
                Name = "Cutlass",
                TemplateKey = "cutlass",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Journeyman",
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.TalgoniteAxe,
            new Recipe()
            {
                Name = "Talgonite Axe",
                TemplateKey = "talgoniteaxe",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 95,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.HybrasylBattleAxe,
            new Recipe()
            {
                Name = "Hy-brasyl Battle Axe",
                TemplateKey = "hybrasylbattleaxe",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 99,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.MagusAres,
            new Recipe()
            {
                Name = "Magus Ares",
                TemplateKey = "magusares",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.HolyHermes,
            new Recipe()
            {
                Name = "Holy Hermes",
                TemplateKey = "holyhermes",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.MagusZeus,
            new Recipe()
            {
                Name = "Magus Zeus",
                TemplateKey = "maguszeus",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.HolyKronos,
            new Recipe()
            {
                Name = "Holy Kronos",
                TemplateKey = "holykronos",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.MagusDiana,
            new Recipe()
            {
                Name = "Magus Diana",
                TemplateKey = "magusdiana",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.HolyDiana,
            new Recipe()
            {
                Name = "Holy Diana",
                TemplateKey = "holydiana",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.StoneCross,
            new Recipe()
            {
                Name = "Stone Cross",
                TemplateKey = "stonecross",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 90,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.OakStaff,
            new Recipe()
            {
                Name = "Oak Staff",
                TemplateKey = "oakstaff",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.StaffOfWisdom,
            new Recipe()
            {
                Name = "Staff of Wisdom",
                TemplateKey = "staffofwisdom",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.SnowDagger,
            new Recipe()
            {
                Name = "Snow Dagger",
                TemplateKey = "snowdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Beginner",
                Level = 2,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.CenterDagger,
            new Recipe()
            {
                Name = "Center Dagger",
                TemplateKey = "centerdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Beginner",
                Level = 4,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BlossomDagger,
            new Recipe()
            {
                Name = "Blossom Dagger",
                TemplateKey = "blossomdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 14,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.CurvedDagger,
            new Recipe()
            {
                Name = "Curved Dagger",
                TemplateKey = "curveddagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 30,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.MoonDagger,
            new Recipe()
            {
                Name = "Moon Dagger",
                TemplateKey = "moondagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 31,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.LightDagger,
            new Recipe()
            {
                Name = "Light Dagger",
                TemplateKey = "lightdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.SunDagger,
            new Recipe()
            {
                Name = "Sun Dagger",
                TemplateKey = "sundagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 62,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.LotusDagger,
            new Recipe()
            {
                Name = "Lotus Dagger",
                TemplateKey = "lotusdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 75,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.BloodDagger,
            new Recipe()
            {
                Name = "Blood Dagger",
                TemplateKey = "blooddagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 89,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.NagetierDagger,
            new Recipe()
            {
                Name = "Nagetierdagger",
                TemplateKey = "nagetierdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.DullClaw,
            new Recipe()
            {
                Name = "Dull Claw",
                TemplateKey = "dullclaw",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.WolfClaw,
            new Recipe()
            {
                Name = "Wolf Claw",
                TemplateKey = "wolfclaw",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.EagleTalon,
            new Recipe()
            {
                Name = "Eagle Talon",
                TemplateKey = "eagletalon",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.PhoenixClaw,
            new Recipe()
            {
                Name = "Phoenix Claw",
                TemplateKey = "phoenixclaw",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.Nunchaku,
            new Recipe()
            {
                Name = "Nunchaku",
                TemplateKey = "nunchaku",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.WoodenShield,
            new Recipe()
            {
                Name = "Wooden Shield",
                TemplateKey = "woodenshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.LeatherShield,
            new Recipe()
            {
                Name = "Leather Shield",
                TemplateKey = "leathershield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 15,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BronzeShield,
            new Recipe()
            {
                Name = "Bronze Shield",
                TemplateKey = "bronzeshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 31,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.IronShield,
            new Recipe()
            {
                Name = "Iron Shield",
                TemplateKey = "ironshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 45,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.MythrilShield,
            new Recipe()
            {
                Name = "Mythril Shield",
                TemplateKey = "mythrilshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 61,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.HybrasylShield,
            new Recipe()
            {
                Name = "Hybrasyl Shield",
                TemplateKey = "hybrasylshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 77,
                Difficulty = 3
            }
        }
    };

    public static Dictionary<WeaponSmithingRecipes, Recipe> WeaponSmithingUpgradeRequirements { get; } = new()
    {
        {
            WeaponSmithingRecipes.Eppe,
            new Recipe()
            {
                Name = "Eppe",
                TemplateKey = "eppe",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "eppe", DisplayName = "Eppe", Amount = 1 },
                    new Ingredient { TemplateKey = "rawbronze", DisplayName = "Raw Bronze", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 2,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Saber,
            new Recipe()
            {
                Name = "Saber",
                TemplateKey = "saber",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 7,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Claidheamh,
            new Recipe()
            {
                Name = "Claidheamh",
                TemplateKey = "claidheamh",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BroadSword,
            new Recipe()
            {
                Name = "Broad Sword",
                TemplateKey = "broadsword",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 17,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BattleSword,
            new Recipe()
            {
                Name = "Battle Sword",
                TemplateKey = "battlesword",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 26,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Masquerade,
            new Recipe()
            {
                Name = "Masquerade",
                TemplateKey = "masquerade",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 31,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Bramble,
            new Recipe()
            {
                Name = "Bramble",
                TemplateKey = "bramble",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.LongSword,
            new Recipe()
            {
                Name = "Longsword",
                TemplateKey = "longsword",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 56,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Claidhmore,
            new Recipe()
            {
                Name = "Claidhmore",
                TemplateKey = "claidhmore",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.EmeraldSword,
            new Recipe()
            {
                Name = "Emerald Sword",
                TemplateKey = "emeraldsword",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 77,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Gladius,
            new Recipe()
            {
                Name = "Gladius",
                TemplateKey = "gladius",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 85,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Kindjal,
            new Recipe()
            {
                Name = "Kindjal",
                TemplateKey = "kindjal",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 90,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.DragonSlayer,
            new Recipe()
            {
                Name = "Dragon Slayer",
                TemplateKey = "dragonslayer",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 99,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Hatchet,
            new Recipe()
            {
                Name = "Hatchet",
                TemplateKey = "hatchet",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Harpoon,
            new Recipe()
            {
                Name = "Harpoon",
                TemplateKey = "harpoon",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 21,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Scimitar,
            new Recipe()
            {
                Name = "Scimitar",
                TemplateKey = "scimitar",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 31,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Club,
            new Recipe()
            {
                Name = "Club",
                TemplateKey = "club",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.SpikedClub,
            new Recipe()
            {
                Name = "Spiked Club",
                TemplateKey = "spikedclub",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 50,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.ChainMace,
            new Recipe()
            {
                Name = "Chain Mace",
                TemplateKey = "chainmace",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 60,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.HandAxe,
            new Recipe()
            {
                Name = "Handaxe",
                TemplateKey = "handaxe",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 70,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.Cutlass,
            new Recipe()
            {
                Name = "Cutlass",
                TemplateKey = "cutlass",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Journeyman",
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.TalgoniteAxe,
            new Recipe()
            {
                Name = "Talgonite Axe",
                TemplateKey = "talgoniteaxe",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 95,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.HybrasylBattleAxe,
            new Recipe()
            {
                Name = "Hy-brasyl Battle Axe",
                TemplateKey = "hybrasylbattleaxe",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 99,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.MagusAres,
            new Recipe()
            {
                Name = "Magus Ares",
                TemplateKey = "magusares",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.HolyHermes,
            new Recipe()
            {
                Name = "Holy Hermes",
                TemplateKey = "holyhermes",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.MagusZeus,
            new Recipe()
            {
                Name = "Magus Zeus",
                TemplateKey = "maguszeus",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.HolyKronos,
            new Recipe()
            {
                Name = "Holy Kronos",
                TemplateKey = "holykronos",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.MagusDiana,
            new Recipe()
            {
                Name = "Magus Diana",
                TemplateKey = "magusdiana",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.HolyDiana,
            new Recipe()
            {
                Name = "Holy Diana",
                TemplateKey = "holydiana",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.StoneCross,
            new Recipe()
            {
                Name = "Stone Cross",
                TemplateKey = "stonecross",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 90,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.OakStaff,
            new Recipe()
            {
                Name = "Oak Staff",
                TemplateKey = "oakstaff",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.StaffOfWisdom,
            new Recipe()
            {
                Name = "Staff of Wisdom",
                TemplateKey = "staffofwisdom",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.SnowDagger,
            new Recipe()
            {
                Name = "Snow Dagger",
                TemplateKey = "snowdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Beginner",
                Level = 2,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.CenterDagger,
            new Recipe()
            {
                Name = "Center Dagger",
                TemplateKey = "centerdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Beginner",
                Level = 4,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BlossomDagger,
            new Recipe()
            {
                Name = "Blossom Dagger",
                TemplateKey = "blossomdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 14,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.CurvedDagger,
            new Recipe()
            {
                Name = "Curved Dagger",
                TemplateKey = "curveddagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 30,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.MoonDagger,
            new Recipe()
            {
                Name = "Moon Dagger",
                TemplateKey = "moondagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 31,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.LightDagger,
            new Recipe()
            {
                Name = "Light Dagger",
                TemplateKey = "lightdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.SunDagger,
            new Recipe()
            {
                Name = "Sun Dagger",
                TemplateKey = "sundagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 62,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.LotusDagger,
            new Recipe()
            {
                Name = "Lotus Dagger",
                TemplateKey = "lotusdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 75,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.BloodDagger,
            new Recipe()
            {
                Name = "Blood Dagger",
                TemplateKey = "blooddagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 89,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.NagetierDagger,
            new Recipe()
            {
                Name = "Nagetierdagger",
                TemplateKey = "nagetierdagger",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.DullClaw,
            new Recipe()
            {
                Name = "Dull Claw",
                TemplateKey = "dullclaw",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.WolfClaw,
            new Recipe()
            {
                Name = "Wolf Claw",
                TemplateKey = "wolfclaw",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.EagleTalon,
            new Recipe()
            {
                Name = "Eagle Talon",
                TemplateKey = "eagletalon",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.PhoenixClaw,
            new Recipe()
            {
                Name = "Phoenix Claw",
                TemplateKey = "phoenixclaw",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 71,
                Difficulty = 3
            }
        },
        {
            WeaponSmithingRecipes.Nunchaku,
            new Recipe()
            {
                Name = "Nunchaku",
                TemplateKey = "nunchaku",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 4
            }
        },
        {
            WeaponSmithingRecipes.WoodenShield,
            new Recipe()
            {
                Name = "Wooden Shield",
                TemplateKey = "woodenshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Beginner",
                Level = 3,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.LeatherShield,
            new Recipe()
            {
                Name = "Leather Shield",
                TemplateKey = "leathershield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 15,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.BronzeShield,
            new Recipe()
            {
                Name = "Bronze Shield",
                TemplateKey = "bronzeshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Basic",
                Level = 31,
                Difficulty = 1
            }
        },
        {
            WeaponSmithingRecipes.GravelShield,
            new Recipe()
            {
                Name = "Gravel Shield",
                TemplateKey = "gravelshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.IronShield,
            new Recipe()
            {
                Name = "Iron Shield",
                TemplateKey = "ironshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 45,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.LightShield,
            new Recipe()
            {
                Name = "Light Shield",
                TemplateKey = "lightshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 50,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.MythrilShield,
            new Recipe()
            {
                Name = "Mythril Shield",
                TemplateKey = "mythrilshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 61,
                Difficulty = 2
            }
        },
        {
            WeaponSmithingRecipes.HybrasylShield,
            new Recipe()
            {
                Name = "Hybrasyl Shield",
                TemplateKey = "hybrasylshield",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 }
                },
                Rank = "Journeyman",
                Level = 77,
                Difficulty = 3
            }
        }
    };
    #endregion

    #region Armor Smithing

    public static Dictionary<CraftedArmors, Recipe> ArmorSmithingArmorRequirements { get; } = new()
    {
        {
            CraftedArmors.RefinedScoutLeather,
            new Recipe()
            {
                Name = "Refined Scout Leather",
                TemplateKey = "refinedscoutleather",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "basic",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedDwarvishLeather,
            new Recipe()
            {
                Name = "Refined Dwarvish Leather",
                TemplateKey = "refineddwarvishleather",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedPaluten,
            new Recipe()
            {
                Name = "Refined Paluten",
                TemplateKey = "refinedpaluten",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedKeaton,
            new Recipe()
            {
                Name = "Refined Keaton",
                TemplateKey = "refinedkeaton",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Adept",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedBardocle,
            new Recipe()
            {
                Name = "Refined Bardocle",
                TemplateKey = "refinedbardocle",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Advanced",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedGardcorp,
            new Recipe()
            {
                Name = "Refined Gardcorp",
                TemplateKey = "refinedgardcorp",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "refined",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedJourneyman,
            new Recipe()
            {
                Name = "Refined Journeyman",
                TemplateKey = "refinedjourneyman",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedLorum,
            new Recipe()
            {
                Name = "Refined Lorum",
                TemplateKey = "refinedlorum",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedMane,
            new Recipe()
            {
                Name = "Refined Mane",
                TemplateKey = "refinedmane",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Adept",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedDuinUasal,
            new Recipe()
            {
                Name = "Refined Duin-Uasal",
                TemplateKey = "refinedduinuasal",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Advanced",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedCowl,
            new Recipe()
            {
                Name = "Refined Cowl",
                TemplateKey = "refinedcowl",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedGaluchatCoat,
            new Recipe()
            {
                Name = "Refined Galuchat Coat",
                TemplateKey = "refinedgaluchatcoat",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedMantle,
            new Recipe()
            {
                Name = "Refined Mantle",
                TemplateKey = "refinedmantle",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedHierophant,
            new Recipe()
            {
                Name = "Refined Hierophant",
                TemplateKey = "refinedhierophant",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Adept",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedDalmatica,
            new Recipe()
            {
                Name = "Refined Dalmatica",
                TemplateKey = "refineddalmatica",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Advanced",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedDobok,
            new Recipe()
            {
                Name = "Refined Dobok",
                TemplateKey = "refineddobok",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedCulotte,
            new Recipe()
            {
                Name = "Refined Culotte",
                TemplateKey = "refinedculotte",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedEarthGarb,
            new Recipe()
            {
                Name = "Refined Earth Garb",
                TemplateKey = "refinedearthgarb",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedWindGarb,
            new Recipe()
            {
                Name = "Refined Wind Garb",
                TemplateKey = "refinedwindgarb",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Adept",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedMountainGarb,
            new Recipe()
            {
                Name = "Refined Mountain Garb",
                TemplateKey = "refinedmountaingarb",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Advanced",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedLeatherTunic,
            new Recipe()
            {
                Name = "Refined Leather Tunic",
                TemplateKey = "refinedleathertunic",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedLorica,
            new Recipe()
            {
                Name = "Refined Lorica",
                TemplateKey = "refinedlorica",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedKasmaniumArmor,
            new Recipe()
            {
                Name = "Refined Kasmanium Armor",
                TemplateKey = "refinedkasmaniumarmor",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedIpletMail,
            new Recipe()
            {
                Name = "Refined Iplet Mail",
                TemplateKey = "refinedipletmail",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Adept",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedHybrasylPlate,
            new Recipe()
            {
                Name = "Refined Hy-brasyl Plate",
                TemplateKey = "refinedhybrasylplate",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Advanced",
                Level = 99,
                Difficulty = 5
            }
        },

        {
            CraftedArmors.RefinedCotte,
            new Recipe()
            {
                Name = "Refined Cotte",
                TemplateKey = "refinedcotte",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedBrigandine,
            new Recipe()
            {
                Name = "Refined Brigandine",
                TemplateKey = "refinedbrigandine",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedCorsette,
            new Recipe()
            {
                Name = "Refined Corsette",
                TemplateKey = "refinedcorsette",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedPebbleRose,
            new Recipe()
            {
                Name = "Refined Pebble Rose",
                TemplateKey = "refinedpebblerose",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Adept",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedKagum,
            new Recipe()
            {
                Name = "Refined Kagum",
                TemplateKey = "refinedkagum",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Advanced",
                Level = 99,
                Difficulty = 5
            }
        },
        {
            CraftedArmors.RefinedMagiSkirt,
            new Recipe()
            {
                Name = "Refined Magi Skirt",
                TemplateKey = "refinedmagiskirt",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 8,
                Difficulty = 1
            }
        },
        {
            CraftedArmors.RefinedBenusta,
            new Recipe()
            {
                Name = "Refined Benusta",
                TemplateKey = "refinedbenusta",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 26,
                Difficulty = 2
            }
        },
        {
            CraftedArmors.RefinedStoller,
            new Recipe()
            {
                Name = "Refined Stoller",
                TemplateKey = "refinedstoller",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 56,
                Difficulty = 3
            }
        },
        {
            CraftedArmors.RefinedClymouth,
            new Recipe()
            {
                Name = "Refined Clymouth",
                TemplateKey = "refinedclymouth",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Adept",
                Level = 86,
                Difficulty = 4
            }
        },
        {
            CraftedArmors.RefinedClamyth,
            new Recipe()
            {
                Name = "Refined Clamyth",
                TemplateKey = "refinedclamyth",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Advanced",
                Level = 99,
                Difficulty = 5
            }
        },
        {
    CraftedArmors.RefinedGorgetGown,
    new Recipe()
    {
        Name = "Refined Gorget Gown",
        TemplateKey = "refinedgorgetgown",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Basic",
        Level = 8,
        Difficulty = 1
    }
},
{
    CraftedArmors.RefinedMysticGown,
    new Recipe()
    {
        Name = "Refined Mystic Gown",
        TemplateKey = "refinedmysticgown",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Apprentice",
        Level = 26,
        Difficulty = 2
    }
},
{
    CraftedArmors.RefinedElle,
    new Recipe()
    {
        Name = "Refined Elle",
        TemplateKey = "refinedelle",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Journeyman",
        Level = 56,
        Difficulty = 3
    }
},
{
    CraftedArmors.RefinedDolman,
    new Recipe()
    {
        Name = "Refined Dolman",
        TemplateKey = "refineddolman",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Adept",
        Level = 86,
        Difficulty = 4
    }
},
{
    CraftedArmors.RefinedBansagart,
    new Recipe()
    {
        Name = "Refined Bansagart",
        TemplateKey = "refinedbansagart",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Advanced",
        Level = 99,
        Difficulty = 5
    }
},
{
    CraftedArmors.RefinedEarthBodice,
    new Recipe()
    {
        Name = "Refined Earth Bodice",
        TemplateKey = "refinedearthbodice",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Basic",
        Level = 8,
        Difficulty = 1
    }
},
{
    CraftedArmors.RefinedLotusBodice,
    new Recipe()
    {
        Name = "Refined Lotus Bodice",
        TemplateKey = "refinedlotusbodice",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Apprentice",
        Level = 26,
        Difficulty = 2
    }
},
{
    CraftedArmors.RefinedMoonBodice,
    new Recipe()
    {
        Name = "Refined Moon Bodice",
        TemplateKey = "refinedmoonbodice",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Journeyman",
        Level = 56,
        Difficulty = 3
    }
},
{
    CraftedArmors.RefinedLightningGarb,
    new Recipe()
    {
        Name = "Refined Lightning Garb",
        TemplateKey = "refinedlightninggarb",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Adept",
        Level = 86,
        Difficulty = 4
    }
},
{
    CraftedArmors.RefinedSeaGarb,
    new Recipe()
    {
        Name = "Refined Sea Garb",
        TemplateKey = "refinedseagarb",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Advanced",
        Level = 99,
        Difficulty = 5
    }
},
{
    CraftedArmors.RefinedLeatherBliaut,
    new Recipe()
    {
        Name = "Refined Leather Bliaut",
        TemplateKey = "refinedleatherbliaut",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Basic",
        Level = 8,
        Difficulty = 1
    }
},
{
    CraftedArmors.RefinedCuirass,
    new Recipe()
    {
        Name = "Refined Cuirass",
        TemplateKey = "refinedcuirass",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Apprentice",
        Level = 26,
        Difficulty = 2
    }
},
{
    CraftedArmors.RefinedKasmaniumHauberk,
    new Recipe()
    {
        Name = "Refined Kasmanium Hauberk",
        TemplateKey = "refinedkasmaniumhauberk",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Journeyman",
        Level = 56,
        Difficulty = 3
    }
},
{
    CraftedArmors.RefinedPhoenixMail,
    new Recipe()
    {
        Name = "Refined Phoenix Mail",
        TemplateKey = "refinedphoenixmail",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Adept",
        Level = 86,
        Difficulty = 4
    }
},
{
    CraftedArmors.RefinedHybrasylArmor,
    new Recipe()
    {
        Name = "Refined Hybrasyl Armor",
        TemplateKey = "refinedhybrasylarmor",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Advanced",
        Level = 99,
        Difficulty = 5
    }
}

    };

    public static Dictionary<ArmorsmithingRecipes, Recipe> ArmorSmithingGearRequirements { get; } = new()
    {
        {
            ArmorsmithingRecipes.LeatherSapphireGauntlet,
            new Recipe()
            {
                Name = "Leather Sapphire Gauntlet",
                TemplateKey = "leathersapphiregauntlet",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
    ArmorsmithingRecipes.LeatherRubyGauntlet,
    new Recipe()
    {
        Name = "Leather Ruby Gauntlet",
        TemplateKey = "leatherrubygauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Basic",
        Level = 11,
        Difficulty = 1
    }
},
{
    ArmorsmithingRecipes.LeatherEmeraldGauntlet,
    new Recipe()
    {
        Name = "Leather Emerald Gauntlet",
        TemplateKey = "leatheremeraldgauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Basic",
        Level = 11,
        Difficulty = 1
    }
},
{
    ArmorsmithingRecipes.LeatherHeartstoneGauntlet,
    new Recipe()
    {
        Name = "Leather Heartstone Gauntlet",
        TemplateKey = "leatherheartstonegauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Basic",
        Level = 11,
        Difficulty = 1
    }
},
{
    ArmorsmithingRecipes.IronSapphireGauntlet,
    new Recipe()
    {
        Name = "Iron Sapphire Gauntlet",
        TemplateKey = "ironsapphiregauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Apprentice",
        Level = 41,
        Difficulty = 2
    }
},
{
    ArmorsmithingRecipes.IronRubyGauntlet,
    new Recipe()
    {
        Name = "Iron Ruby Gauntlet",
        TemplateKey = "ironrubygauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Apprentice",
        Level = 41,
        Difficulty = 2
    }
},
{
    ArmorsmithingRecipes.IronEmeraldGauntlet,
    new Recipe()
    {
        Name = "Iron Emerald Gauntlet",
        TemplateKey = "ironemeraldgauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Apprentice",
        Level = 41,
        Difficulty = 2
    }
},
{
    ArmorsmithingRecipes.IronHeartstoneGauntlet,
    new Recipe()
    {
        Name = "Iron Heartstone Gauntlet",
        TemplateKey = "ironheartstonegauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Apprentice",
        Level = 41,
        Difficulty = 2
    }
},
{
    ArmorsmithingRecipes.MythrilSapphireGauntlet,
    new Recipe()
    {
        Name = "Mythril Sapphire Gauntlet",
        TemplateKey = "mythrilsapphiregauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Journeyman",
        Level = 71,
        Difficulty = 3
    }
},
{
    ArmorsmithingRecipes.MythrilRubyGauntlet,
    new Recipe()
    {
        Name = "Mythril Ruby Gauntlet",
        TemplateKey = "mythrilrubygauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Journeyman",
        Level = 71,
        Difficulty = 3
    }
},
{
    ArmorsmithingRecipes.MythrilEmeraldGauntlet,
    new Recipe()
    {
        Name = "Mythril Emerald Gauntlet",
        TemplateKey = "mythrilemeraldgauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Journeyman",
        Level = 71,
        Difficulty = 3
    }
},
{
    ArmorsmithingRecipes.MythrilHeartstoneGauntlet,
    new Recipe()
    {
        Name = "Mythril Heartstone Gauntlet",
        TemplateKey = "mythrilheartstonegauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Journeyman",
        Level = 71,
        Difficulty = 3
    }
},
{
    ArmorsmithingRecipes.HybrasylSapphireGauntlet,
    new Recipe()
    {
        Name = "Hy-brasyl Sapphire Gauntlet",
        TemplateKey = "hybrasylsapphiregauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Adept",
        Level = 97,
        Difficulty = 5
    }
},
{
    ArmorsmithingRecipes.HybrasylRubyGauntlet,
    new Recipe()
    {
        Name = "Hy-brasyl Ruby Gauntlet",
        TemplateKey = "hybrasylrubygauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Adept",
        Level = 97,
        Difficulty = 5
    }
},
{
    ArmorsmithingRecipes.HybrasylEmeraldGauntlet,
    new Recipe()
    {
        Name = "Hy-brasyl Emerald Gauntlet",
        TemplateKey = "hybrasylemeraldgauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Adept",
        Level = 97,
        Difficulty = 5
    }
},
{
    ArmorsmithingRecipes.HybrasylHeartstoneGauntlet,
    new Recipe()
    {
        Name = "Hy-brasyl Heartstone Gauntlet",
        TemplateKey = "hybrasylheartstonegauntlet",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Adept",
        Level = 97,
        Difficulty = 5
    }
},
        {
            ArmorsmithingRecipes.JeweledSeaBelt,
            new Recipe()
            {
                Name = "Jeweled Sea Belt",
                TemplateKey = "jeweledseabelt",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
                Difficulty = 1
            }
        },
        {
    ArmorsmithingRecipes.JeweledFireBelt,
    new Recipe()
    {
        Name = "Jeweled Fire Belt",
        TemplateKey = "jeweledfirebelt",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Basic",
        Level = 11,
        Difficulty = 1
    }
},
{
    ArmorsmithingRecipes.JeweledWindBelt,
    new Recipe()
    {
        Name = "Jeweled Wind Belt",
        TemplateKey = "jeweledwindbelt",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Basic",
        Level = 11,
        Difficulty = 1
    }
},
{
    ArmorsmithingRecipes.JeweledEarthBelt,
    new Recipe()
    {
        Name = "Jeweled Earth Belt",
        TemplateKey = "jeweledearthbelt",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Basic",
        Level = 11,
        Difficulty = 1
    }
},
{
    ArmorsmithingRecipes.JeweledNatureBelt,
    new Recipe()
    {
        Name = "Jeweled Nature Belt",
        TemplateKey = "jewelednaturebelt",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Apprentice",
        Level = 41,
        Difficulty = 2
    }
},
{
    ArmorsmithingRecipes.JeweledMetalBelt,
    new Recipe()
    {
        Name = "Jeweled Metal Belt",
        TemplateKey = "jeweledmetalbelt",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Apprentice",
        Level = 41,
        Difficulty = 2
    }
},
{
    ArmorsmithingRecipes.JeweledLightBelt,
    new Recipe()
    {
        Name = "Jeweled Light Belt",
        TemplateKey = "jeweledlightbelt",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Journeyman",
        Level = 71,
        Difficulty = 3
    }
},
{
    ArmorsmithingRecipes.JeweledDarkBelt,
    new Recipe()
    {
        Name = "Jeweled Dark Belt",
        TemplateKey = "jeweleddarkbelt",
        Ingredients = new List<Ingredient>()
        {
            new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
            new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
        },
        Rank = "Journeyman",
        Level = 71,
        Difficulty = 3
    }
}
        };

        #endregion
}