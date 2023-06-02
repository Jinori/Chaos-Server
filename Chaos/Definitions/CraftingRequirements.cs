namespace Chaos.Definitions;

public static class CraftingRequirements
{
    public sealed class Ingredient
    {
        public string? TemplateKey { get; set; }
        public string? DisplayName { get; set; }
        public int Amount { get; set; }
    }


    //Recipe, Ingredient, Status and Player Level Requirement
    public static Dictionary<AlchemyRecipes, Tuple<List<Ingredient>, string, int>> AlchemyRequirements { get; } = new()
    {
        {
            AlchemyRecipes.Hemloch, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                "Basic",
                3)
        },
        {
            AlchemyRecipes.BetonyDeum, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                "Basic",
                8)
        }
    };

    public static Dictionary<EnchantingRecipes, Tuple<List<Ingredient>, string, int>> EnchantingRequirements { get; } = new()
        {
            {
                EnchantingRecipes.MiraelisEmbrace, new Tuple<List<Ingredient>, string, int>(
                    new List<Ingredient>()
                    {
                        new Ingredient { TemplateKey = "magicalessence", DisplayName = "Magical Essence", Amount = 5 },
                        new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                    },
                    "Basic",
                    3)
            }
        };

    #region WeaponSmithing
    public static Dictionary<WeaponSmithingRecipes, Tuple<List<Ingredient>, string, int>> WeaponSmithingCraftRequirements { get; } = new()
    {
        {
            WeaponSmithingRecipes.Eppe, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                },
                "Basic",
                2)
        },
        {
            WeaponSmithingRecipes.Saber, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                },
                "Basic",
                7)
        },
        {
            WeaponSmithingRecipes.Claidheamh, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                },
                "Basic",
                11)
        },
        {
            WeaponSmithingRecipes.Broadsword, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                },
                "Basic",
                17)
        },
        {
            WeaponSmithingRecipes.Battlesword, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                26)
        },
        {
            WeaponSmithingRecipes.Masquerade, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                31)
        },
        {
            WeaponSmithingRecipes.Bramble, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "ruby", DisplayName = "Ruby", Amount = 1 }
                },
                "Apprentice",
                41)
        },
        {
            WeaponSmithingRecipes.Longsword, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                56)
        },
        {
        WeaponSmithingRecipes.Claidhmore, new Tuple<List<Ingredient>, string, int>(
            new List<Ingredient>()
            {
                new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
            },
            "Journeyman",
            71)
        },
        {
            WeaponSmithingRecipes.Emeraldsword, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Journeyman",
                77)
        },
        {
            WeaponSmithingRecipes.Kindjal, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Adept",
                90)
        },
        {
            WeaponSmithingRecipes.Dragonslayer, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Adept",
                99)
        },
        {
            WeaponSmithingRecipes.Hatchet, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                11)
        },
        {
            WeaponSmithingRecipes.Harpoon, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                21)
        },
        {
            WeaponSmithingRecipes.Club, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                41)
        },
        {
            WeaponSmithingRecipes.Spikedclub, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                50)
        },
        {
            WeaponSmithingRecipes.Chainmace, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                60)
        },
        {
            WeaponSmithingRecipes.Handaxe, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Journeyman",
                71)
        },
        {
            WeaponSmithingRecipes.Cutlass, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Journeyman",
                80)
        },
        {
            WeaponSmithingRecipes.Talgoniteaxe, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Adept",
                95)
        },
        {
            WeaponSmithingRecipes.Hybrasylaxe, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Adept",
                99)
        },
        {
            WeaponSmithingRecipes.Magusares, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                11)
        },
        {
            WeaponSmithingRecipes.Holyhermes, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                11)
        },
        {
            WeaponSmithingRecipes.Maguszeus, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                41)
        },
        {
            WeaponSmithingRecipes.Holykronos, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                41)
        },
        {
            WeaponSmithingRecipes.Magusdiana, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Journeyman",
                71)
        },
        {
            WeaponSmithingRecipes.Holydiana, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Journeyman",
                71)
        },
        {
            WeaponSmithingRecipes.Stonecross, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Adept",
                90)
        },
        {
            WeaponSmithingRecipes.Oakstaff, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Adept",
                97)
        },
        {
            WeaponSmithingRecipes.Staffofwisdom, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Adept",
                97)
        },
        {
            WeaponSmithingRecipes.Snowdagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                2)
        },
        {
            WeaponSmithingRecipes.Centerdagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                4)
        },
        {
            WeaponSmithingRecipes.Blossomdagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                14)
        },
        {
            WeaponSmithingRecipes.Curveddagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                30)
        },
        {
            WeaponSmithingRecipes.Moondagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                31)
        },
        {
            WeaponSmithingRecipes.Lightdagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                42)
        },
        {
            WeaponSmithingRecipes.Sundagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                62)
        },
        {
            WeaponSmithingRecipes.Lotusdagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Journeyman",
                75)
        },
        {
            WeaponSmithingRecipes.Blooddagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Journeyman",
                89)
        },
        {
            WeaponSmithingRecipes.Nagetierdagger, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Adept",
                97)
        },
        {
            WeaponSmithingRecipes.Dullclaw, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                3)
        },
        {
            WeaponSmithingRecipes.Wolfclaw, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                11)
        },
        {
            WeaponSmithingRecipes.Eagletalon, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                41)
        },
        {
            WeaponSmithingRecipes.Phoenixclaw, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Journeyman",
                71)
        },
        {
            WeaponSmithingRecipes.Nunchaku, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Adept",
                97)
        },
        {
            WeaponSmithingRecipes.Woodenshield, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                3)
        },
        {
            WeaponSmithingRecipes.Leathershield, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                15)
        },
        {
            WeaponSmithingRecipes.Bronzeshield, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Basic",
                31)
        },
        {
            WeaponSmithingRecipes.Gravelshield, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                41)
        },
        {
            WeaponSmithingRecipes.Lightshield, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                50)
        },
        {
            WeaponSmithingRecipes.Ironshield, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                45)
        },
        {
            WeaponSmithingRecipes.Mythrilshield, new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                },
                "Apprentice",
                61)
        },
        {
            WeaponSmithingRecipes.Hybrasylshield, new Tuple<List<Ingredient>, string, int>(
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