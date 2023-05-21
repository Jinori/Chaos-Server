namespace Chaos.Definitions;

public static class CraftingRequirements
{
    public sealed class Ingredient
    {
        public string? TemplateKey { get; set; }
        public string? DisplayName { get; set; }
        public int Amount { get; set; }
    }


    public static Dictionary<AlchemyRecipes, List<Ingredient>> AlchemyRequirements { get; } = new()
    {
        {
            AlchemyRecipes.Hemloch, new List<Ingredient>()
            {
                new Ingredient { TemplateKey = "mold", DisplayName = "Mold", Amount = 1 },
                new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
            }
        },
        {
            AlchemyRecipes.BetonyDeum, new List<Ingredient>()
            {
                new Ingredient { TemplateKey = "petunia", DisplayName = "Petunia", Amount = 5 },
                new Ingredient { TemplateKey = "emptybottle", DisplayName = "Empty Bottle", Amount = 1 }
            }
        }
    };


    public static List<string> Meats { get; } = new()
    {
        "lobstertail",
        "beef",
        "chicken",
        "beefslices",
        "clam",
        "egg",
        "liver",
        "rawmeat"
        // Add more meat types as needed...
    };


    public static Dictionary<string, List<(string TemplateKey, int Amount)>> FoodRequirements { get; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            {
                "fruitbasket", new List<(string TemplateKey, int Amount)>()
                {
                    ("cherry", 15),
                    ("grape", 5),
                    ("tangerines", 3)
                }
            },

            {
                "fruitbasket2", new List<(string TemplateKey, int Amount)>()
                {
                    ("apple", 10),
                    ("greengrapes", 5),
                    ("strawberry", 3)
                }
            },

            {
                "salad", new List<(string TemplateKey, int Amount)>()
                {
                    ("acorn", 10),
                    ("tomato", 5),
                    ("rambutan", 3)
                }
            },

            {
                "salad2", new List<(string TemplateKey, int Amount)>()
                {
                    ("carrot", 10),
                    ("vegetable", 5),
                    ("rambutan", 3)
                }
            },
            {
                "dinnerplate", new List<(string TemplateKey, int Amount)>()
                {
                    ("any_meat", 1),
                    ("vegetable", 10),
                    ("salt", 1),
                    ("apple", 5)
                }
            },
            {
                "lobsterdinner", new List<(string TemplateKey, int Amount)>()
                {
                    ("carrot", 10),
                    ("vegetable", 5),
                    ("rambutan", 3)
                }
            },
            {
                "steakmeal", new List<(string TemplateKey, int Amount)>()
                {
                    ("any_meat", 1),
                    ("carrot", 10),
                    ("vegetable", 5),
                    ("rambutan", 3)
                }
            },
            {
                "sweetbuns", new List<(string TemplateKey, int Amount)>()
                {
                    ("carrot", 10),
                    ("vegetable", 5),
                    ("rambutan", 3)
                }
            },
            {
                "sandwich", new List<(string TemplateKey, int Amount)>()
                {
                    ("any_meat", 1),
                    ("carrot", 10),
                    ("vegetable", 5),
                    ("rambutan", 3)
                }
            },
            {
                "soup", new List<(string TemplateKey, int Amount)>()
                {
                    ("carrot", 10),
                    ("vegetable", 5),
                    ("rambutan", 3)
                }
            }
        };
}