using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class MountDialogScript : DialogScriptBase
{
    private readonly IEffectFactory _effectFactory;

    public MountDialogScript(Dialog subject, IEffectFactory effectFactory)
        : base(subject)
        => _effectFactory = effectFactory;

    private void AddCloakOptions(Dialog dialog, Aisling source)
    {
        foreach (AvailableCloaks cloakFlag in Enum.GetValues(typeof(AvailableCloaks)))
            if (source.Trackers.Flags.HasFlag(cloakFlag))
            {
                var option = new DialogOption
                {
                    DialogKey = $"cloak_{cloakFlag.ToString().ToLower()}",
                    OptionText = $"{cloakFlag} Cloak"
                };

                if (!dialog.HasOption(option.OptionText))
                    dialog.Options.Insert(0, option);
            }
    }

    private void AddMountOptionIfFlag(
        Dialog dialog,
        Aisling source,
        AvailableMounts mountFlag,
        string optionKey)
    {
        if (source.Trackers.Flags.HasFlag(mountFlag))
        {
            var option = new DialogOption
            {
                DialogKey = optionKey,
                OptionText = mountFlag.ToString()
            };

            if (!dialog.HasOption(option.OptionText))
                dialog.Options.Insert(0, option);
        }
    }

    public override void OnDisplaying(Aisling source)
    {
        var templateKey = Subject.Template.TemplateKey.ToLower();
        var flags = source.Trackers.Flags;

        switch (templateKey)
        {
            case "terminus_initial":
                if (flags.TryGetFlag(out AvailableMounts _))
                    Subject.Options.Insert(
                        0,
                        new DialogOption
                        {
                            DialogKey = "mount_initial",
                            OptionText = "Mounts"
                        });

                break;

            case "mount_initial":
                if (source.Effects.TryGetEffect("Mount", out var effect) && source.Effects.Contains(effect))
                {
                    Subject.Reply(source, "Please get off your mount first.");

                    return;
                }

                AddMountOptionIfFlag(
                    Subject,
                    source,
                    AvailableMounts.Horse,
                    "mount_horse");

                AddMountOptionIfFlag(
                    Subject,
                    source,
                    AvailableMounts.Wolf,
                    "mount_wolf");

                AddMountOptionIfFlag(
                    Subject,
                    source,
                    AvailableMounts.Dunan,
                    "mount_dunan");

                AddMountOptionIfFlag(
                    Subject,
                    source,
                    AvailableMounts.Kelberoth,
                    "mount_kelberoth");

                AddMountOptionIfFlag(
                    Subject,
                    source,
                    AvailableMounts.Ant,
                    "mount_ant");

                AddMountOptionIfFlag(
                    Subject,
                    source,
                    AvailableMounts.Bee,
                    "mount_bee");

                break;

            case "mount_horse":
                SetMount(Subject, source, CurrentMount.Horse);

                break;

            case "mount_wolf":
                SetMount(Subject, source, CurrentMount.Wolf);

                break;

            case "mount_kelberoth":
                SetMount(Subject, source, CurrentMount.Kelberoth);

                break;

            case "mount_bee":
                SetMount(Subject, source, CurrentMount.Bee);

                break;

            case "mount_ant":
                SetMount(Subject, source, CurrentMount.Ant);

                break;

            case "mount_dunan":
                SetMount(Subject, source, CurrentMount.Dunan);

                break;

            case "cloak_blue":
                source.Trackers.Enums.Set(CurrentCloak.Blue);

                break;

            case "cloak_red":
                source.Trackers.Enums.Set(CurrentCloak.Red);

                break;

            case "cloak_purple":
                source.Trackers.Enums.Set(CurrentCloak.Purple);

                break;

            case "cloak_black":
                source.Trackers.Enums.Set(CurrentCloak.Black);

                break;

            case "cloak_green":
                source.Trackers.Enums.Set(CurrentCloak.Green);

                break;
        }
    }

    private void SetMount(Dialog dialog, Aisling source, CurrentMount mount)
    {
        source.Trackers.Enums.Set(mount);
        AddCloakOptions(dialog, source);
    }
}