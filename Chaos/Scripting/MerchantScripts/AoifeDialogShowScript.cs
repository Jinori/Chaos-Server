using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class AoifeDialogShowScript : DialogScriptBase
{
    public AoifeDialogShowScript(Dialog subject) : base(subject)
    {
    }

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

                if (source.UserStatSheet.Level >= 99)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "aoife_stats",
                        OptionText = "Purchase Stats"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                    
                    var option1 = new DialogOption
                    {
                        DialogKey = "aoife_ascend",
                        OptionText = "Ascension"
                    };

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Add(option1);
                    
                    var option2 = new DialogOption
                    {
                        DialogKey = "aoife_classdedication",
                        OptionText = "Class Dedication"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Add(option2);
                }
            }
                break;
            
        }
    }
}