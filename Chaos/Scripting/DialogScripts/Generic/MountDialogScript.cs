using Chaos.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Generic;

public class MountDialogScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MountDialogScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
                var hasFlag = source.Trackers.Flags.TryGetFlag(out AvailableMounts mount);

                if (hasFlag)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mount_initial",
                        OptionText = "Mounts"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }
            }

                break;
            
            case "mount_initial":
            {
                if (source.Trackers.Flags.TryGetFlag(out AvailableMounts mount))
                {
                    if (source.Trackers.Flags.HasFlag(AvailableMounts.WhiteHorse))
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "mount_whitehorse",
                            OptionText = "White Horse"
                        };

                        if (!Subject.HasOption(option))
                            Subject.Options.Insert(0, option);

                        return;
                    }
                }
            }

                break;

            case "mount_whitehorse":
            {
                if (source.Sprite != 0)
                {
                    source.Sprite = 0;
                    source.Refresh(true);
                    source.SendOrangeBarMessage("You jump off your mount.");
                    Subject.Reply(source, "Skip", "Close");

                    return;
                }
                source.Sprite = 1;
                source.Refresh(true);
                source.SendOrangeBarMessage("You jump on your mount.");
                Subject.Reply(source, "Skip", "Close");
            }

                break;
        }
    }
}