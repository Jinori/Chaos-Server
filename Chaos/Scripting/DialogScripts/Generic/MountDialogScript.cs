using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class MountDialogScript : DialogScriptBase
{
    private readonly IEffectFactory _effectFactory;

    /// <inheritdoc />
    public MountDialogScript(Dialog subject, IEffectFactory effectFactory)
        : base(subject) =>
        _effectFactory = effectFactory;

    public override void OnDisplaying(Aisling source)
    {
        var effect = _effectFactory.Create("mount");

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

                    if (!Subject.HasOption(option.OptionText))
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

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                    }
                    if (source.Trackers.Flags.HasFlag(AvailableMounts.WhiteWolf))
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "mount_whitewolf",
                            OptionText = "White Wolf"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);

                    }
                }
            }

                break;

            case "mount_whitehorse":
            {
                source.Trackers.Enums.Set(CurrentMount.WhiteHorse);
                Subject.Reply(source, "Skip", "Close");
                source.SendOrangeBarMessage("You've equipped your White Horse.");

                return;
            }
                
            case "mount_whitewolf":
            {
                source.Trackers.Enums.Set(CurrentMount.WhiteWolf);
                Subject.Reply(source, "Skip", "Close");
                source.SendOrangeBarMessage("You've equipped your White Wolf.");
            }

                break;
        }
    }
}