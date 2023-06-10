using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ItemScripts.Enchantments;
using Chaos.TypeMapper;
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
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
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
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Basic",
                Level = 24,
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
            EnchantingRecipes.IgnatarRegret,
            new Recipe()
            {
                Name = "Ignatar's Regret",
                TemplateKey = "ignatarregret",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 48,
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
            EnchantingRecipes.IgnatarJealousy,
            new Recipe()
            {
                Name = "Ignatar's Jealousy",
                TemplateKey = "ignatarjealousy",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 60,
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
            EnchantingRecipes.IgnatarDestruction,
            new Recipe()
            {
                Name = "Ignatar's Destruction",
                TemplateKey = "ignatardestruction",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 80,
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
            EnchantingRecipes.GeolithGratitude,
            new Recipe()
            {
                Name = "Geolith's Gratitude",
                TemplateKey = "geolithgratitude",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
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
            EnchantingRecipes.GeolithPride,
            new Recipe()
            {
                Name = "Geolith's Pride",
                TemplateKey = "geolithpride",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Basic",
                Level = 28,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SwiftPrefixScript)));
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
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 50,
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
            EnchantingRecipes.GeolithObsession,
            new Recipe()
            {
                Name = "Geolith's Obsession",
                TemplateKey = "geolithobsession",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 65,
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
            EnchantingRecipes.GeolithFortitude,
            new Recipe()
            {
                Name = "Geolith's Fortitude",
                TemplateKey = "geolithfortitude",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 83,
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
            EnchantingRecipes.MiraelisSerenity,
            new Recipe()
            {
                Name = "Miraelis' Serenity",
                TemplateKey = "miraelisserenity",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 5,
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
            EnchantingRecipes.MiraelisBlessing,
            new Recipe()
            {
                Name = "Miraelis' Blessing",
                TemplateKey = "miraelisblessing",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Basic",
                Level = 34,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SwiftPrefixScript)));
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
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 50,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SwiftPrefixScript)));
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
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 69,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SwiftPrefixScript)));
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
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 88,
                Difficulty = 2,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SwiftPrefixScript)));
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
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Beginner",
                Level = 5,
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
            EnchantingRecipes.TheseleneShadow,
            new Recipe()
            {
                Name = "Theselene's Shadow",
                TemplateKey = "theseleneshadow",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Basic",
                Level = 37,
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
            EnchantingRecipes.TheseleneDexterity,
            new Recipe()
            {
                Name = "Theselene's Dexterity",
                TemplateKey = "theselenedexterity",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Apprentice",
                Level = 50,
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
            EnchantingRecipes.TheseleneBalance,
            new Recipe()
            {
                Name = "Theselene's Balance",
                TemplateKey = "theselenebalance",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Journeyman",
                Level = 71,
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
            EnchantingRecipes.TheseleneRisk,
            new Recipe()
            {
                Name = "Theselene's Risk",
                TemplateKey = "theselenerisk",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                },
                Rank = "Adept",
                Level = 90,
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
            EnchantingRecipes.AquaedonClarity,
            new Recipe()
            {
                Name = "Aquaedon's Clarity",
                TemplateKey = "aquaedonclarity",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 5,
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
            EnchantingRecipes.AquaedonCalming,
            new Recipe()
            {
                Name = "Aquaedon's Calming",
                TemplateKey = "aquaedoncalming",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Basic",
                Level = 40,
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
            EnchantingRecipes.AquaedonWisdom,
            new Recipe()
            {
                Name = "Aquaedon's Wisdom",
                TemplateKey = "aquaedonwisdom",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Apprentice",
                Level = 50,
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
            EnchantingRecipes.AquaedonWill,
            new Recipe()
            {
                Name = "Aquaedon's Will",
                TemplateKey = "aquaedonwill",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Journeyman",
                Level = 71,
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
            EnchantingRecipes.AquaedonResolve,
            new Recipe()
            {
                Name = "Aquaedon's Resolve",
                TemplateKey = "aquaedonresolve",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Adept",
                Level = 90,
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
            EnchantingRecipes.SerendaelLuck,
            new Recipe()
            {
                Name = "Serendael's Luck",
                TemplateKey = "serendaelluck",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Basic",
                Level = 11,
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
            EnchantingRecipes.SerendaelMagic,
            new Recipe()
            {
                Name = "Serendael's Magic",
                TemplateKey = "serendaelmagic",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 41,
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
            EnchantingRecipes.SerendaelChance,
            new Recipe()
            {
                Name = "Serendael's Chance",
                TemplateKey = "serendaelchance",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 3 }
                },
                Rank = "Apprentice",
                Level = 55,
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
            EnchantingRecipes.SerendaelRoll,
            new Recipe()
            {
                Name = "Serendael's Roll",
                TemplateKey = "serendaelroll",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 4 }
                },
                Rank = "Journeyman",
                Level = 71,
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
            EnchantingRecipes.SerendaelAddiction,
            new Recipe()
            {
                Name = "Serendael's Addiction",
                TemplateKey = "serendaeladdiction",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 5 }
                },
                Rank = "Adept",
                Level = 90,
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
            EnchantingRecipes.SkandaraMight,
            new Recipe()
            {
                Name = "Skandara's Might",
                TemplateKey = "skandaramight",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Basic",
                Level = 16,
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
            EnchantingRecipes.SkandaraTriumph,
            new Recipe()
            {
                Name = "Skandara's Triumph",
                TemplateKey = "skandaratriumph",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 44,
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
            EnchantingRecipes.SkandaraStrength,
            new Recipe()
            {
                Name = "Skandara's Strength",
                TemplateKey = "skandarastrength",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 3 }
                },
                Rank = "Apprentice",
                Level = 50,
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
            EnchantingRecipes.SkandaraDrive,
            new Recipe()
            {
                Name = "Skandara's Drive",
                TemplateKey = "skandaradrive",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 4 }
                },
                Rank = "Journeyman",
                Level = 75,
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
            EnchantingRecipes.SkandaraPierce,
            new Recipe()
            {
                Name = "Skandara's Pierce",
                TemplateKey = "skandarapierce",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 5 }
                },
                Rank = "Adept",
                Level = 95,
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
            EnchantingRecipes.ZephyraSpirit,
            new Recipe()
            {
                Name = "Zephyra's Spirit",
                TemplateKey = "zephyraspirit",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 }
                },
                Rank = "Basic",
                Level = 20,
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
            EnchantingRecipes.ZephyraMist,
            new Recipe()
            {
                Name = "Zephyra's Mist",
                TemplateKey = "zephyramist",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 45,
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
            EnchantingRecipes.ZephyraWind,
            new Recipe()
            {
                Name = "Zephyra's Wind",
                TemplateKey = "zephyrawind",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 2 }
                },
                Rank = "Apprentice",
                Level = 58,
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
            EnchantingRecipes.ZephyraVortex,
            new Recipe()
            {
                Name = "Zephyra's Vortex",
                TemplateKey = "zephyravortex",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 3 }
                },
                Rank = "Journeyman",
                Level = 78,
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
            EnchantingRecipes.ZephyraGust,
            new Recipe()
            {
                Name = "Zephyra's Gust",
                TemplateKey = "zephyragust",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 2 }
                },
                Rank = "Adept",
                Level = 97,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SwiftPrefixScript)));
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
                Name = "Hemloch",
                TemplateKey = "hemloch",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Adept",
                Level = 1,
                Difficulty = 1
            }
        },
        {
            AlchemyRecipes.BetonyDeum,
            new Recipe()
            {
                Name = "Betony Deum",
                TemplateKey = "betonydeum",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Basic",
                Level = 8,
                Difficulty = 2
            }
        },
    };

    #region Jewelcrafting

    public static Dictionary<JewelcraftingRecipes, Recipe> JewelcraftingRequirements { get; } = new()
    {
        {
            JewelcraftingRecipes.None,
            new Recipe()
            {
                Name = "Miraelis Embrace",
                TemplateKey = "beginnerScroll",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                Rank = "Beginner",
                Level = 1,
                Difficulty = 1,
                Modification = ((mapper, item) =>
                {
                    item.ScriptKeys.Add(ScriptBase.GetScriptKey(typeof(SwiftPrefixScript)));
                    var schema = mapper.Map<ItemSchema>(item);
                    return mapper.Map<Item>(schema);
                })
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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
                Rank = "Basic",
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

    public static Dictionary<ArmorSmithRecipes, Recipe> ArmorSmithingGearRequirements { get; } = new()
    {
        {
            ArmorSmithRecipes.LeatherSapphireGauntlet,
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
    ArmorSmithRecipes.LeatherRubyGauntlet,
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
    ArmorSmithRecipes.LeatherEmeraldGauntlet,
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
    ArmorSmithRecipes.LeatherHeartstoneGauntlet,
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
    ArmorSmithRecipes.IronSapphireGauntlet,
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
    ArmorSmithRecipes.IronRubyGauntlet,
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
    ArmorSmithRecipes.IronEmeraldGauntlet,
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
    ArmorSmithRecipes.IronHeartstoneGauntlet,
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
    ArmorSmithRecipes.MythrilSapphireGauntlet,
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
    ArmorSmithRecipes.MythrilRubyGauntlet,
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
    ArmorSmithRecipes.MythrilEmeraldGauntlet,
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
    ArmorSmithRecipes.MythrilHeartstoneGauntlet,
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
    ArmorSmithRecipes.HybrasylSapphireGauntlet,
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
    ArmorSmithRecipes.HybrasylRubyGauntlet,
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
    ArmorSmithRecipes.HybrasylEmeraldGauntlet,
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
    ArmorSmithRecipes.HybrasylHeartstoneGauntlet,
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
            ArmorSmithRecipes.JeweledSeaBelt,
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
    ArmorSmithRecipes.JeweledFireBelt,
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
    ArmorSmithRecipes.JeweledWindBelt,
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
    ArmorSmithRecipes.JeweledEarthBelt,
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
    ArmorSmithRecipes.JeweledNatureBelt,
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
    ArmorSmithRecipes.JeweledMetalBelt,
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
    ArmorSmithRecipes.JeweledLightBelt,
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
    ArmorSmithRecipes.JeweledDarkBelt,
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