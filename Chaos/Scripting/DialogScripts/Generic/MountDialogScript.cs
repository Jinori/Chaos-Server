using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class MountDialogScript : DialogScriptBase
{
    /// <inheritdoc />
    public MountDialogScript(Dialog subject, IEffectFactory effectFactory)
        : base(subject)
    { }

    public override void OnDisplaying(Aisling source)
    {
        var hasFlag = source.Trackers.Flags.TryGetFlag(out AvailableMounts _);
        source.Trackers.Enums.TryGetValue(out CurrentMount _);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
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
                if (source.Effects.TryGetEffect("mount", out var effect) && source.Effects.Contains(effect))
                {
                    Subject.Reply(source, "Please get off your mount first.");

                    return;
                }

                if (source.Trackers.Flags.HasFlag(AvailableMounts.Horse))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mount_horse",
                        OptionText = "Horse"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableMounts.Wolf))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mount_wolf",
                        OptionText = "Wolf"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableMounts.Dunan))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mount_dunan",
                        OptionText = "Dunan"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableMounts.Kelberoth))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mount_kelberoth",
                        OptionText = "Kelberoth"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableMounts.Ant))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mount_ant",
                        OptionText = "Ant"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableMounts.Bee))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mount_bee",
                        OptionText = "Bee"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
            }

                break;

            case "mount_horse":
            {
                source.Trackers.Enums.Set(CurrentMount.Horse);

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Blue))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_blue",
                        OptionText = "Blue Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Red))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_red",
                        OptionText = "Red Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Black))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_black",
                        OptionText = "Black Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Green))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_green",
                        OptionText = "Green Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Purple))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_purple",
                        OptionText = "Purple Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                return;
            }

            case "mount_wolf":
            {
                source.Trackers.Enums.Set(CurrentMount.Wolf);

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Blue))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_blue",
                        OptionText = "Blue Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Red))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_red",
                        OptionText = "Red Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Black))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_black",
                        OptionText = "Black Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Green))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_green",
                        OptionText = "Green Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Purple))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_purple",
                        OptionText = "Purple Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                return;
            }

            case "mount_kelberoth":
            {
                source.Trackers.Enums.Set(CurrentMount.Kelberoth);

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Blue))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_blue",
                        OptionText = "Blue Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Red))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_red",
                        OptionText = "Red Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Black))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_black",
                        OptionText = "Black Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Green))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_green",
                        OptionText = "Green Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Purple))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_purple",
                        OptionText = "Purple Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                return;
            }
            case "mount_bee":
            {
                source.Trackers.Enums.Set(CurrentMount.Bee);

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Blue))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_blue",
                        OptionText = "Blue Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Red))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_red",
                        OptionText = "Red Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Black))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_black",
                        OptionText = "Black Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Green))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_green",
                        OptionText = "Green Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Purple))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_purple",
                        OptionText = "Purple Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                return;
            }

            case "mount_ant":
            {
                source.Trackers.Enums.Set(CurrentMount.Ant);

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Blue))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_blue",
                        OptionText = "Blue Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Red))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_red",
                        OptionText = "Red Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Black))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_black",
                        OptionText = "Black Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Green))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_green",
                        OptionText = "Green Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Purple))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_purple",
                        OptionText = "Purple Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                return;
            }
            case "mount_dunan":
            {
                source.Trackers.Enums.Set(CurrentMount.Dunan);

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Blue))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_blue",
                        OptionText = "Blue Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Red))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_red",
                        OptionText = "Red Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Black))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_black",
                        OptionText = "Black Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Green))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_green",
                        OptionText = "Green Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(AvailableCloaks.Purple))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cloak_purple",
                        OptionText = "Purple Cloak"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                return;
            }
            case "cloak_blue":
            {
                source.Trackers.Enums.Set(CurrentCloak.Blue);

                break;
            }
            case "cloak_red":
            {
                source.Trackers.Enums.Set(CurrentCloak.Red);

                break;
            }
            case "cloak_purple":
            {
                source.Trackers.Enums.Set(CurrentCloak.Purple);

                break;
            }
            case "cloak_black":
            {
                source.Trackers.Enums.Set(CurrentCloak.Black);

                break;
            }
            case "cloak_green":
            {
                source.Trackers.Enums.Set(CurrentCloak.Green);
            }

                break;
        }
    }
}