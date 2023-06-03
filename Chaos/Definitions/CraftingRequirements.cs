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
}