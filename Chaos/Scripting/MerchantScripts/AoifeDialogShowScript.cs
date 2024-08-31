using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class AoifeDialogShowScript(Dialog subject) : DialogScriptBase(subject)
{
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "aoife_initial":
            {
                if (source.UserStatSheet.BaseClass == BaseClass.Peasant)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "aoife_temple",
                        OptionText = "Enter Inner Sanctum"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                if (source.UserStatSheet.Level >= 99 && source.Trackers.Flags.HasFlag(MainstoryFlags.CompletedFloor3))
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "aoife_aetheriarealm",
                        OptionText = "Realm of Aetheria"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Add(option2);
                }
            }

                break;
        }
    }
}