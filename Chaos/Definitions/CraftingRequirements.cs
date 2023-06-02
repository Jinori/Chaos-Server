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
    public static Dictionary<string, Tuple<List<Ingredient>, string, int>> AlchemyRequirements { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        {
            "Hemloch", new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                "Basic",
                3
            )
        },
        {
            "BetonyDeum", new Tuple<List<Ingredient>, string, int>(
                new List<Ingredient>()
                {
                    new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 2 },
                    new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
                },
                "Basic",
                8
            )
        }
    };
}