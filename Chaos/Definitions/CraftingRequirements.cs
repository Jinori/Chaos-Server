namespace Chaos.Definitions;

public static class CraftingRequirements
{

    public sealed class Recipe
    {
        public string? Name { get; set; }
        public string? TemplateKey { get; set; }
        public List<Ingredient>? Ingredients { get; set; }
        public string? Rank { get; set; }
        public int Level { get; set; }
        public int Difficulty { get; set; }
    }

    public sealed class Ingredient
    {
        public string? TemplateKey { get; set; }
        public string? DisplayName { get; set; }
        public int Amount { get; set; }
    }



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
            WeaponSmithingRecipes.Broadsword,
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
            WeaponSmithingRecipes.Battlesword,
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
            WeaponSmithingRecipes.Longsword,
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
            WeaponSmithingRecipes.Emeraldsword,
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
            WeaponSmithingRecipes.Dragonslayer,
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
            WeaponSmithingRecipes.Spikedclub,
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
            WeaponSmithingRecipes.Chainmace,
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
            WeaponSmithingRecipes.Handaxe,
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
            WeaponSmithingRecipes.Talgoniteaxe,
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
            WeaponSmithingRecipes.Hybrasylaxe,
            new Recipe()
            {
                Name = "Hy-brasyl Axe",
                TemplateKey = "hybrasylaxe",
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
            WeaponSmithingRecipes.Magusares,
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
            WeaponSmithingRecipes.Holyhermes,
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
            WeaponSmithingRecipes.Maguszeus,
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
            WeaponSmithingRecipes.Holykronos,
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
            WeaponSmithingRecipes.Magusdiana,
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
            WeaponSmithingRecipes.Holydiana,
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
            WeaponSmithingRecipes.Stonecross,
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
            WeaponSmithingRecipes.Oakstaff,
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
            WeaponSmithingRecipes.Staffofwisdom,
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
            WeaponSmithingRecipes.Snowdagger,
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
            WeaponSmithingRecipes.Centerdagger,
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
            WeaponSmithingRecipes.Blossomdagger,
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
            WeaponSmithingRecipes.Curveddagger,
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
            WeaponSmithingRecipes.Moondagger,
            new Recipe()
            {
                Name = "Moondagger",
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
            WeaponSmithingRecipes.Lightdagger,
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
            WeaponSmithingRecipes.Sundagger,
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
            WeaponSmithingRecipes.Lotusdagger,
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
            WeaponSmithingRecipes.Blooddagger,
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
            WeaponSmithingRecipes.Nagetierdagger,
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
            WeaponSmithingRecipes.Dullclaw,
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
            WeaponSmithingRecipes.Wolfclaw,
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
            WeaponSmithingRecipes.Eagletalon,
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
            WeaponSmithingRecipes.Phoenixclaw,
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
            WeaponSmithingRecipes.Woodenshield,
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
            WeaponSmithingRecipes.Leathershield,
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
            WeaponSmithingRecipes.Bronzeshield,
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
            WeaponSmithingRecipes.Gravelshield,
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
            WeaponSmithingRecipes.Ironshield,
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
            WeaponSmithingRecipes.Lightshield,
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
            WeaponSmithingRecipes.Mythrilshield,
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
            WeaponSmithingRecipes.Hybrasylshield,
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
            WeaponSmithingRecipes.Broadsword,
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
            WeaponSmithingRecipes.Battlesword,
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
            WeaponSmithingRecipes.Longsword,
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
            WeaponSmithingRecipes.Emeraldsword,
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
            WeaponSmithingRecipes.Dragonslayer,
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
            WeaponSmithingRecipes.Spikedclub,
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
            WeaponSmithingRecipes.Chainmace,
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
            WeaponSmithingRecipes.Handaxe,
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
            WeaponSmithingRecipes.Talgoniteaxe,
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
            WeaponSmithingRecipes.Hybrasylaxe,
            new Recipe()
            {
                Name = "Hy-brasyl Axe",
                TemplateKey = "hybrasylaxe",
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
            WeaponSmithingRecipes.Magusares,
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
            WeaponSmithingRecipes.Holyhermes,
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
            WeaponSmithingRecipes.Maguszeus,
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
            WeaponSmithingRecipes.Holykronos,
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
            WeaponSmithingRecipes.Magusdiana,
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
            WeaponSmithingRecipes.Holydiana,
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
            WeaponSmithingRecipes.Stonecross,
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
            WeaponSmithingRecipes.Oakstaff,
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
            WeaponSmithingRecipes.Staffofwisdom,
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
            WeaponSmithingRecipes.Snowdagger,
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
            WeaponSmithingRecipes.Centerdagger,
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
            WeaponSmithingRecipes.Blossomdagger,
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
            WeaponSmithingRecipes.Curveddagger,
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
            WeaponSmithingRecipes.Moondagger,
            new Recipe()
            {
                Name = "Moondagger",
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
            WeaponSmithingRecipes.Lightdagger,
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
            WeaponSmithingRecipes.Sundagger,
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
            WeaponSmithingRecipes.Lotusdagger,
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
            WeaponSmithingRecipes.Blooddagger,
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
            WeaponSmithingRecipes.Nagetierdagger,
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
            WeaponSmithingRecipes.Dullclaw,
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
            WeaponSmithingRecipes.Wolfclaw,
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
            WeaponSmithingRecipes.Eagletalon,
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
            WeaponSmithingRecipes.Phoenixclaw,
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
            WeaponSmithingRecipes.Woodenshield,
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
            WeaponSmithingRecipes.Leathershield,
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
            WeaponSmithingRecipes.Bronzeshield,
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
            WeaponSmithingRecipes.Gravelshield,
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
            WeaponSmithingRecipes.Ironshield,
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
            WeaponSmithingRecipes.Lightshield,
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
            WeaponSmithingRecipes.Mythrilshield,
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
            WeaponSmithingRecipes.Hybrasylshield,
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
            CraftedArmors.Refinedscoutleather,
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
            CraftedArmors.Refineddwarvishleather,
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
            CraftedArmors.Refinedpaluten,
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
            CraftedArmors.Refinedkeaton,
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
            CraftedArmors.Refinedbardocle,
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
            CraftedArmors.Refinedgardcorp,
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
            CraftedArmors.Refinedjourneyman,
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
            CraftedArmors.Refinedlorum,
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
            CraftedArmors.Refinedmane,
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
            CraftedArmors.Refinedduinuasal,
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
            CraftedArmors.Refinedcowl,
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
            CraftedArmors.Refinedgaluchatcoat,
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
            CraftedArmors.Refinedmantle,
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
            CraftedArmors.Refinedhierophant,
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
            CraftedArmors.Refineddalmatica,
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
            CraftedArmors.Refineddobok,
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
            CraftedArmors.Refinedculotte,
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
            CraftedArmors.Refinedearthgarb,
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
            CraftedArmors.Refinedwindgarb,
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
            CraftedArmors.Refinedmountaingarb,
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
            CraftedArmors.Refinedleathertunic,
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
            CraftedArmors.Refinedlorica,
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
            CraftedArmors.Refinedkasmaniumarmor,
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
            CraftedArmors.Refinedipletmail,
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
            CraftedArmors.Refinedhybrasylplate,
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
            CraftedArmors.Refinedcotte,
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
            CraftedArmors.Refinedbrigandine,
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
            CraftedArmors.Refinedcorsette,
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
            CraftedArmors.Refinedpebblerose,
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
            CraftedArmors.Refinedkagum,
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
            CraftedArmors.Refinedmagiskirt,
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
            CraftedArmors.Refinedbenusta,
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
            CraftedArmors.Refinedstoller,
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
            CraftedArmors.Refinedclymouth,
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
            CraftedArmors.Refinedclamyth,
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
    CraftedArmors.Refinedgorgetgown,
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
    CraftedArmors.Refinedmysticgown,
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
    CraftedArmors.Refinedelle,
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
    CraftedArmors.Refineddolman,
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
    CraftedArmors.Refinedbansagart,
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
    CraftedArmors.Refinedearthbodice,
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
    CraftedArmors.Refinedlotusbodice,
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
    CraftedArmors.Refinedmoonbodice,
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
    CraftedArmors.Refinedlightninggarb,
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
    CraftedArmors.Refinedseagarb,
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
    CraftedArmors.Refinedleatherbliaut,
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
    CraftedArmors.Refinedcuirass,
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
    CraftedArmors.Refinedkasmaniumhauberk,
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
    CraftedArmors.Refinedphoenixmail,
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
    CraftedArmors.Refinedhybrasylarmor,
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
            ArmorSmithRecipes.Leathersapphiregauntlet,
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
    ArmorSmithRecipes.Leatherrubygauntlet,
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
    ArmorSmithRecipes.Leatheremeraldgauntlet,
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
    ArmorSmithRecipes.Leatherheartstonegauntlet,
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
    ArmorSmithRecipes.Ironsapphiregauntlet,
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
    ArmorSmithRecipes.Ironrubygauntlet,
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
    ArmorSmithRecipes.Ironemeraldgauntlet,
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
    ArmorSmithRecipes.Ironheartstonegauntlet,
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
    ArmorSmithRecipes.Mythrilsapphiregauntlet,
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
    ArmorSmithRecipes.Mythrilrubygauntlet,
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
    ArmorSmithRecipes.Mythrilemeraldgauntlet,
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
    ArmorSmithRecipes.Mythrilheartstonegauntlet,
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
    ArmorSmithRecipes.Hybrasylsapphiregauntlet,
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
    ArmorSmithRecipes.Hybrasylrubygauntlet,
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
    ArmorSmithRecipes.Hybrasylemeraldgauntlet,
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
    ArmorSmithRecipes.Hybrasylheartstonegauntlet,
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
            ArmorSmithRecipes.Jeweledseabelt,
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
    ArmorSmithRecipes.Jeweledfirebelt,
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
    ArmorSmithRecipes.Jeweledwindbelt,
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
    ArmorSmithRecipes.Jeweledearthbelt,
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
    ArmorSmithRecipes.Jewelednaturebelt,
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
    ArmorSmithRecipes.Jeweledmetalbelt,
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
    ArmorSmithRecipes.Jeweledlightbelt,
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
    ArmorSmithRecipes.Jeweleddarkbelt,
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