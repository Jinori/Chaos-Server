using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class RecipeLibraryScript : DialogScriptBase
{
    /// <inheritdoc />
    public RecipeLibraryScript(Dialog subject)
        : base(subject) {}

    public override void OnDisplaying(Aisling source)
    {

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
                var hasFlag = source.Trackers.Flags.TryGetFlag(out CookingRecipes cookingrecipes);

                if (hasFlag)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "recipelibrary_initial",
                        OptionText = "Cooking Recipes"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }
            }

                break;
            
            case "cookinglibrary":
            {
                
            }

                break;
        }
    }
}