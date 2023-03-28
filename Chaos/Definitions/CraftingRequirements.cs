namespace Chaos.Definitions;

public static class CraftingRequirements
{
    public static Dictionary<string, List<(string TemplateKey, int Amount)>> FoodRequirements { get; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            {"fruitbasket", new List<(string TemplateKey, int Amount)>()
            {
                ("cherry", 15),
                ("grape", 5),
                ("tangerines", 3)
            }},
            
            {"fruitbasket2", new List<(string TemplateKey, int Amount)>()
            {
                ("apple", 10),
                ("greengrapes", 5),
                ("strawberry", 3)
            }},
            
            {"salad", new List<(string TemplateKey, int Amount)>()
            {
                ("acorn", 10),
                ("tomato", 5),
                ("rambutan", 3)
            }},
            
            {"salad2", new List<(string TemplateKey, int Amount)>()
            {
                ("carrot", 10),
                ("vegetable", 5),
                ("rambutan", 3)
            }},
            {"dinnerplate", new List<(string TemplateKey, int Amount)>()
            {
                ("beef", 5),
                ("vegetable", 10),
                ("salt", 1),
                ("apple", 5)
            }},
            {"lobsterdinner", new List<(string TemplateKey, int Amount)>()
            {
                ("carrot", 10),
                ("vegetable", 5),
                ("rambutan", 3)
            }},
            {"steakmeal", new List<(string TemplateKey, int Amount)>()
            {
                ("carrot", 10),
                ("vegetable", 5),
                ("rambutan", 3)
            }},
            {"sweetbuns", new List<(string TemplateKey, int Amount)>()
            {
                ("carrot", 10),
                ("vegetable", 5),
                ("rambutan", 3)
            }},
            {"sandwich", new List<(string TemplateKey, int Amount)>()
            {
                ("carrot", 10),
                ("vegetable", 5),
                ("rambutan", 3)
            }},
            {"soup", new List<(string TemplateKey, int Amount)>()
            {
                ("carrot", 10),
                ("vegetable", 5),
                ("rambutan", 3)
            }}, 
            {"meats", new List<(string TemplateKey, int Amount)>()
                {
                    ("beef", 1),
                    ("chicken", 1),
                    ("beefslices", 1)
                }}
        };
}