using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class RecipeItemScript : ItemScriptBase
{

    public RecipeItemScript(Item subject)
        : base(subject) {}
    

    public override void OnUse(Aisling source)
    {
        var ani = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 21
        };

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "recipe_dinnerplate":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.dinnerplate))
                {
                    source.Animate(ani);
                    source.Trackers.Flags.AddFlag(CookingRecipes.dinnerplate);
                    source.SendOrangeBarMessage($"You've learned {Subject.Template.Name}.");
                    source.Inventory.RemoveQuantity($"{Subject.Template.TemplateKey}", 1);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_sweetbuns":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.sweetbuns))
                {
                    source.Animate(ani);
                    source.Trackers.Flags.AddFlag(CookingRecipes.sweetbuns);
                    source.SendOrangeBarMessage($"You've learned {Subject.Template.Name}.");
                    source.Inventory.RemoveQuantity($"{Subject.Template.TemplateKey}", 1);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_fruitbasket":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.fruitbasket))
                {
                    source.Animate(ani);
                    source.Trackers.Flags.AddFlag(CookingRecipes.fruitbasket);
                    source.SendOrangeBarMessage($"You've learned {Subject.Template.Name}.");
                    source.Inventory.RemoveQuantity($"{Subject.Template.TemplateKey}", 1);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_salad":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.salad))
                {
                    source.Animate(ani);
                    source.Trackers.Flags.AddFlag(CookingRecipes.salad);
                    source.SendOrangeBarMessage($"You've learned {Subject.Template.Name}.");
                    source.Inventory.RemoveQuantity($"{Subject.Template.TemplateKey}", 1);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_lobsterdinner":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.lobsterdinner))
                {
                    source.Animate(ani);
                    source.Trackers.Flags.AddFlag(CookingRecipes.lobsterdinner);
                    source.SendOrangeBarMessage($"You've learned {Subject.Template.Name}.");
                    source.Inventory.RemoveQuantity($"{Subject.Template.TemplateKey}", 1);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_pie":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.pie))
                {
                    source.Animate(ani);
                    source.Trackers.Flags.AddFlag(CookingRecipes.pie);
                    source.SendOrangeBarMessage($"You've learned {Subject.Template.Name}.");
                    source.Inventory.RemoveQuantity($"{Subject.Template.TemplateKey}", 1);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_sandwich":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.sandwich))
                {
                    source.Animate(ani);
                    source.Trackers.Flags.AddFlag(CookingRecipes.sandwich);
                    source.SendOrangeBarMessage($"You've learned {Subject.Template.Name}.");
                    source.Inventory.RemoveQuantity($"{Subject.Template.TemplateKey}", 1);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_soup":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.soup))
                {
                    source.Animate(ani);
                    source.Trackers.Flags.AddFlag(CookingRecipes.soup);
                    source.SendOrangeBarMessage($"You've learned {Subject.Template.Name}.");
                    source.Inventory.RemoveQuantity($"{Subject.Template.TemplateKey}", 1);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_steakmeal":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.steakmeal))
                {
                    source.Animate(ani);
                    source.Trackers.Flags.AddFlag(CookingRecipes.steakmeal);
                    source.SendOrangeBarMessage($"You've learned {Subject.Template.Name}.");
                    source.Inventory.RemoveQuantity($"{Subject.Template.TemplateKey}", 1);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
        }
    }
}