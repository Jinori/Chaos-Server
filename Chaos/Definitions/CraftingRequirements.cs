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
    
    #region WeaponSmithing
    public static Dictionary<string, Tuple<List<Ingredient>, string, int>> WeaponSmithingCraftRequirements { get; } =
        new()
        {
            {
                "Eppe", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                        new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                    },
                    "Basic",
                    2)
            },
            {
                "Saber", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                        new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                    },
                    "Basic",
                    7)
            },
            {
                "Claidheamh", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                        new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                    },
                    "Basic",
                    11)
            },
            {
                "Broadsword", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                        new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                    },
                    "Basic",
                    17)
            },
            {
                "Battlesword", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    26)
            },
            {
                "Masquerade", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    31)
            },
            {
                "Bramble", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                        new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                    },
                    "Apprentice",
                    41)
            },
            {
                "Longsword", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    56)
            },
            {
                "Claidhmore", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    71)
            },
            {
                "Emeraldsword", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    77)
            },
            {
                "Kindjal", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Adept",
                    90)
            },
            {
                "Dragonslayer", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Adept",
                    99)
            },
            {
                "Hatchet", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    11)
            },
            {
                "Harpoon", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    21)
            },
            {
                "Club", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    41)
            },
            {
                "Spikedclub", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    50)
            },
            {
                "Chainmace", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    60)
            },
            {
                "Handaxe", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    71)
            },
            {
                "Cutlass", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    80)
            },
            {
                "Talgoniteaxe", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Adept",
                    95)
            },
            {
                "Hybrasylaxe", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Adept",
                    99)
            },
            {
                "Magusares", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    11)
            },
            {
                "Holyhermes", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    11)
            },
            {
                "Maguszeus", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    41)
            },
            {
                "Holykronos", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    41)
            },
            {
                "Magusdiana", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    71)
            },
            {
                "Holydiana", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    71)
            },
            {
                "Stonecross", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Adept",
                    90)
            },
            {
                "Oakstaff", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Adept",
                    97)
            },
            {
                "Staffofwisdom", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Adept",
                    97)
            },
            {
                "Snowdagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    2)
            },
            {
                "Centerdagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    4)
            },
            {
                "Blossomdagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    14)
            },
            {
                "Curveddagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    30)
            },
            {
                "Moondagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    31)
            },
            {
                "Lightdagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    42)
            },
            {
                "Sundagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    62)
            },
            {
                "Lotusdagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    75)
            },
            {
                "Blooddagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    89)
            },
            {
                "Nagetierdagger", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Adept",
                    97)
            },
            {
                "Dullclaw", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    3)
            },
            {
                "Wolfclaw", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    11)
            },
            {
                "Eagletalon", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    41)
            },
            {
                "Phoenixclaw", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    71)
            },
            {
                "Nunchaku", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Adept",
                    97)
            },
            {
                "Woodenshield", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    3)
            },
            {
                "Leathershield", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    15)
            },
            {
                "Bronzeshield", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Basic",
                    31)
            },
            {
                "Gravelshield", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    41)
            },
            {
                "Lightshield", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    50)
            },
            {
                "Ironshield", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    45)
            },
            {
                "Mythrilshield", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Apprentice",
                    61)
            },
            {
                "Hybrasylshield", new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    },
                    "Journeyman",
                    77)
            },
        };
    #endregion
}