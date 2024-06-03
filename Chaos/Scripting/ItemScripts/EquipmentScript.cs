using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class EquipmentScript(Item subject) : ConfigurableItemScriptBase(subject)
{
    private void EquipStaff(Aisling source, ItemTemplate template)
    {
        if (template.EquipmentType is not null)
            source.Equip(template.EquipmentType.Value, Subject);
    }
    
    private bool CanWieldStaff(Aisling source, string skillName)
    {
        if (source.UserStatSheet.BaseClass == BaseClass.Priest)
            return true;

        if (source.UserStatSheet.BaseClass == BaseClass.Wizard)
            return true;

        if (source.SkillBook.Contains(skillName)) return true;

        source.SendOrangeBarMessage($"You do not have the skill to wield it.");
        return false;
    }

    public override void OnUse(Aisling source)
    {
        var template = Subject.Template;
        
        // Check if the item is a shield
        if (template.Category.Contains("Shield"))
        {
            if (source.UserStatSheet.BaseClass == BaseClass.Monk)
            {
                source.SendOrangeBarMessage("Agility and discipline, not shields, for protection.");
                return;   
            }
            
            // Ensure the character is not already equipped with a staff
            if (source.Equipment.TryGetObject((byte)EquipmentSlot.Weapon, out var item))
            {
                if (item.Template.Category.ContainsI("Staff"))
                {
                    source.SendOrangeBarMessage("You cannot equip a shield while having a staff equipped."); 
                    return;   
                }

                if (item.Template.Category.ContainsI("2H"))
                {
                    source.SendOrangeBarMessage("You cannot equip a shield while wielding a two handed weapon."); 
                    return;
                }
            }
        }
        
        if (template.EquipmentType is null or EquipmentType.NotEquipment)
        {
            source.SendOrangeBarMessage("You can't equip that");

            return;
        }

        //gender check
        if (template.Gender.HasValue && !template.Gender.Value.HasFlag(source.Gender))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (template.Class.HasValue && !source.HasClass(template.Class.Value))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} is not for {source.UserStatSheet.BaseClass}.");

            return;
        }

        if (template.AdvClass.HasValue && (template.AdvClass.Value != source.UserStatSheet.AdvClass))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} is not for {source.UserStatSheet.AdvClass}.");

            return;
        }

        if (template.Level > source.UserStatSheet.Level)
        {
            source.SendOrangeBarMessage($"You are too inexperienced to equip {Subject.Template.Name}.");

            return;
        }

        if (template.RequiresMaster && !source.UserStatSheet.Master)
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} requires Master to wear.");

            return;
        }

        if (StatRequired.HasValue
            && StatAmountRequired.HasValue
            && (source.StatSheet.GetBaseStat(StatRequired.Value) < StatAmountRequired.Value))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} requires {StatAmountRequired.Value} {StatRequired} to equip.");

            return;
        }

        if (template.RequiresMaster && !source.UserStatSheet.Master)
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} requires Master to be able to be equipped.");

            return;
        }
        
        if (template.Category.Contains("Staff"))
        {
            if (source.Equipment.Contains((byte)EquipmentSlot.Shield)
                && source.Equipment.TryGetObject((byte)EquipmentSlot.Shield, out var shield))
            {
                source.SendOrangeBarMessage($"You cannot equip a staff while using {shield.DisplayName}.");
                return;
            }
            // Check specific conditions for equipping a staff
            if (!template.TemplateKey.ContainsI("magus") && !CanWieldStaff(source, "Wield Magus Staff"))
            {
                EquipStaff(source, template);
                return;
            }

            if (template.TemplateKey.ContainsI("holy") && CanWieldStaff(source, "Wield Holy Staff"))
            {
                EquipStaff(source, template);
                return;
            }
        }

        if (template.Category.ContainsI("2H"))
        {
            // Ensure the character is not already equipped with a staff
            if (source.Equipment.TryGetObject((byte)EquipmentSlot.Shield, out var item))
            {
                if (item.Template.Category.ContainsI("Shield"))
                {
                    source.SendOrangeBarMessage("You cannot equip a two handed weapon with a shield equipped."); 
                    return;   
                }
            }
        }

        source.Equip(template.EquipmentType.Value, Subject);
    }

    #region ScriptVars
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }
    #endregion
}