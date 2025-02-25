using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusHideEquipmentScript : DialogScriptBase
{
    /// <inheritdoc />
    public TerminusHideEquipmentScript(Dialog subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        source.Trackers.Flags.TryGetFlag(out InvisibleGear gear);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "terminus_setinvisiblegear",
                    OptionText = "Set Invisible Equipment"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Add(option);

                break;
            }

            case "terminus_setinvisiblegear":
            {
                AddGearToggleOption(InvisibleGear.HideHelmet, "Helmet", gear, "terminus_toggle_helmet");
                AddGearToggleOption(InvisibleGear.HideWeapon, "Weapon", gear, "terminus_toggle_weapon");
                AddGearToggleOption(InvisibleGear.HideShield, "Shield", gear, "terminus_toggle_shield");
                AddGearToggleOption(InvisibleGear.HideBoots, "Boots", gear, "terminus_toggle_boots");
                AddGearToggleOption(InvisibleGear.HideArmor, "Armor", gear, "terminus_toggle_armor");
                AddGearToggleOption(InvisibleGear.HideAccessoryOne, "Accessory One", gear, "terminus_toggle_accessory1");
                AddGearToggleOption(InvisibleGear.HideAccessoryTwo, "Accessory Two", gear, "terminus_toggle_accessory2");
                AddGearToggleOption(InvisibleGear.HideAccessoryThree, "Accessory Three", gear, "terminus_toggle_accessory3");

                Subject.Options.Add(new DialogOption
                {
                    DialogKey = "terminus_initial",
                    OptionText = "Back"
                });

                break;
            }

            case "terminus_toggle_helmet":
                ToggleGearFlag(source, InvisibleGear.HideHelmet);
                break;

            case "terminus_toggle_weapon":
                ToggleGearFlag(source, InvisibleGear.HideWeapon);
                break;

            case "terminus_toggle_shield":
                ToggleGearFlag(source, InvisibleGear.HideShield);
                break;

            case "terminus_toggle_boots":
                ToggleGearFlag(source, InvisibleGear.HideBoots);
                break;

            case "terminus_toggle_armor":
                ToggleGearFlag(source, InvisibleGear.HideArmor);
                break;
                
            case "terminus_toggle_accessory1":
                ToggleGearFlag(source, InvisibleGear.HideAccessoryOne);
                break;
                            
            case "terminus_toggle_accessory2":
                ToggleGearFlag(source, InvisibleGear.HideAccessoryTwo);
                break;
            
            case "terminus_toggle_accessory3":
                ToggleGearFlag(source, InvisibleGear.HideAccessoryThree);
                break;
        }
    }

    private void AddGearToggleOption(InvisibleGear flag, string gearName, InvisibleGear currentGear, string dialogKey)
    {
        var isHidden = currentGear.HasFlag(flag);
        var option = new DialogOption
        {
            DialogKey = dialogKey,
            OptionText = isHidden ? $"Show {gearName}" : $"Hide {gearName}"
        };

        if (!Subject.HasOption(option.OptionText))
            Subject.Options.Add(option);
    }

    private void ToggleGearFlag(Aisling source, InvisibleGear flag)
    {
        source.Trackers.Flags.TryGetFlag(out InvisibleGear currentGear);

        if (currentGear.HasFlag(flag))
            source.Trackers.Flags.RemoveFlag(flag);
        else
            source.Trackers.Flags.AddFlag(flag);

        source.Display();
    }
}
