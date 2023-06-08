using System.Text;
using System.Text.RegularExpressions;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic
{
    public class RecipeLibraryScript : DialogScriptBase
    {
        public RecipeLibraryScript(Dialog subject)
            : base(subject) {}

        public override void OnDisplaying(Aisling source)
        {
            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "terminus_initial":
                    var hasFlag = source.Trackers.Flags.TryGetFlag(out CookingRecipes _);
                    if (hasFlag)
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "recipelibrary_initial",
                            OptionText = "Recipe Book"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                    }
                    break;

                case "cookinglibrary":
                    Subject.Close(source);
                    var sb = new StringBuilder();
                    var recipeNames = Enum.GetNames(typeof(CookingRecipes));

                    
                    foreach (var recipe in recipeNames)
                    {
                        if (recipe is not "None")
                        {
                            var spacedName = AddSpaces(recipe);
                            sb.Append(spacedName);
                            sb.AppendLine();   
                        }
                    }
                    source.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());
                    break;
            }
        }
        public static string AddSpaces(string text) => Regex.Replace(text, "(\\B[A-Z])", " $1");
    }
}