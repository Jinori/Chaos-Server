using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class LanternDialogScript : DialogScriptBase
{

    public LanternDialogScript(Dialog subject)
        : base(subject)
    {
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {

            case "terminus_initial":
                if (source.Trackers.Flags.HasFlag(LanternSizes.SmallLantern) ||
                    source.Trackers.Flags.HasFlag(LanternSizes.LargeLantern))
                {
                    Subject.Options.Insert(
                        0,
                        new DialogOption
                        {
                            DialogKey = "lantern_initial",
                            OptionText = "Lantern"
                        });
                }

                break;

            case "lantern_initial":
            {
                Subject.Options.Insert(
                    0,
                    new DialogOption
                    {
                        DialogKey = "lantern_none",
                        OptionText = "Unequip Lantern"
                    });
                
                if (source.Trackers.Flags.HasFlag(LanternSizes.SmallLantern))
                {
                    Subject.Options.Insert(
                        0,
                        new DialogOption
                        {
                            DialogKey = "lantern_small",
                            OptionText = "Small Lantern"
                        });
                }

                if (source.Trackers.Flags.HasFlag(LanternSizes.LargeLantern))
                {
                    Subject.Options.Insert(
                        0,
                        new DialogOption
                        {
                            DialogKey = "lantern_large",
                            OptionText = "Large Lantern"
                        });
                }

                break;
            }

            case "lantern_small":
            {
                source.LanternSize = LanternSize.Small;
                source.Refresh(true);
                break;
            }

            case "lantern_large":
            {
                source.LanternSize = LanternSize.Large;
                source.Refresh(true);
                break;
            }
            case "lantern_none":
            {
                source.LanternSize = LanternSize.None;
                source.Refresh(true);
                break;
            }
        }
    }
}