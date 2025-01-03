using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class EquipmentScript(Item subject) : ConfigurableItemScriptBase(subject)
{
    private Attributes? MysticAttributes { get; set; }

    private bool CanWieldStaff(Aisling source, string skillName)
    {
        // Check if the character's base class and skill match the traditional rules.
        var isPriestWithHolyStaff = source.UserStatSheet.BaseClass == BaseClass.Priest && skillName == "Wield Holy Staff";
        var isWizardWithMagusStaff = source.UserStatSheet.BaseClass == BaseClass.Wizard && skillName == "Wield Magus Staff";

        // Check if the character has the skill, even if it's not typical for their class.
        var hasSkill = source.SkillBook.Contains(skillName);

        if (isPriestWithHolyStaff || isWizardWithMagusStaff || hasSkill)
        {
            return true;
        }
    
        source.SendOrangeBarMessage("You do not have the skill to wield this staff.");
        return false;
    }

    private void EquipStaff(Aisling source, ItemTemplate template)
    {
        if (template.EquipmentType is not null)
            source.Equip(template.EquipmentType.Value, Subject);
    }

    public override void OnUnEquipped(Aisling aisling)
    {
        base.OnUnEquipped(aisling);

        if (!Subject.Template.TemplateKey.StartsWithI("mystic"))
            return;

        if (MysticAttributes != null)
            Subject.Modifiers.Subtract(MysticAttributes);

        MysticAttributes = null;
    }

    public override void OnUse(Aisling source)
    {
        if (source.Options.LockHands)
        {
            switch (Subject.Template.EquipmentType)
            {
                case EquipmentType.Weapon:
                    source.SendOrangeBarMessage($"Lock Hands is stopping you from equipping {Subject.DisplayName}.");
                    return;
                case EquipmentType.Shield:
                    source.SendOrangeBarMessage($"Lock Hands is stopping you from equipping {Subject.DisplayName}.");
                    return;
            }
        }
        
        var template = Subject.Template;

        if (Subject.CurrentDurability is < 1)
        {
            source.SendOrangeBarMessage("That item is broken and needs to be repaired.");

            return;
        }

        if (template.Category.Contains("Staff"))
        {
            // Check for shield usage which prevents staff wielding
            if (source.Equipment.Contains((byte)EquipmentSlot.Shield) &&
                source.Equipment.TryGetObject((byte)EquipmentSlot.Shield, out var shield))
            {
                source.SendOrangeBarMessage($"You cannot equip a staff while using {shield.DisplayName}.");
                return;
            }

            switch (template.Class)
            {
                // Handle equipping a Magus Staff
                case BaseClass.Wizard:
                {
                    if (!CanWieldStaff(source, "Wield Magus Staff"))
                    {
                        return;
                    }

                    if (template.Level > source.UserStatSheet.Level)
                    {
                        source.SendOrangeBarMessage($"You are too inexperienced to equip {template.Name}.");
                        return;
                    }

                    EquipStaff(source, template);
                    return;
                }
                // Handle equipping a Holy Staff
                case BaseClass.Priest:
                {
                    if (!CanWieldStaff(source, "Wield Holy Staff"))
                    {
                        return;
                    }

                    if (template.Level > source.UserStatSheet.Level)
                    {
                        source.SendOrangeBarMessage($"You are too inexperienced to equip {template.Name}.");
                        return;
                    }

                    EquipStaff(source, template);
                    return;
                }
            }
        }

        // Check if the item is a shield
        if (template.Category.Contains("Shield"))

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

        if (template.Category.ContainsI("2H"))
        {
            if (!source.SkillBook.Contains("Two Handed Attack"))
            {
                source.SendOrangeBarMessage("You cannot equip a Two Handed Weapon without the skill.");

                return;
            }

            // Ensure the character is not already equipped with a staff
            if (source.Equipment.TryGetObject((byte)EquipmentSlot.Shield, out var item))
                if (item.Template.Category.ContainsI("Shield"))
                {
                    source.SendOrangeBarMessage("You cannot equip a two handed weapon with a shield equipped.");

                    return;
                }
        }

        if (Subject.Template.TemplateKey.StartsWithI("mystic"))
        {
            if (Subject.Template.TemplateKey.EqualsI("mysticclub"))
            {
                var attributes = new Attributes
                {
                    Ac = -1,
                    AtkSpeedPct = 20,
                    FlatSkillDamage = (int)(8 + (source.StatSheet.Level - 3) * 1.4),
                    SkillDamagePct = (int)(3 + (source.StatSheet.Level - 3) * 0.2),
                    MagicResistance = source.StatSheet.Level >= 40 ? 5 : 0
                };

                MysticAttributes = attributes;
                subject.Modifiers.Add(MysticAttributes);
            }

            if (Subject.Template.TemplateKey.EqualsI("mysticknife"))
            {
                var attributes = new Attributes
                {
                    AtkSpeedPct = 7 + source.StatSheet.Level / 10,
                    FlatSkillDamage = 6 + (int)(source.StatSheet.Level * 1.3),
                    SkillDamagePct = 6 + source.StatSheet.Level / 10
                };

                MysticAttributes = attributes;
                subject.Modifiers.Add(MysticAttributes);
            }

            if (Subject.Template.TemplateKey.EqualsI("mysticsword"))
            {
                var attributes = new Attributes
                {
                    AtkSpeedPct = 3 + source.StatSheet.Level / 10,
                    FlatSkillDamage = 15 + (int)(source.StatSheet.Level * 1.5),
                    SkillDamagePct = source.StatSheet.Level / 10
                };

                MysticAttributes = attributes;
                subject.Modifiers.Add(MysticAttributes);
            }
        }

        source.Equip(template.EquipmentType.Value, Subject);
    }

    #region ScriptVars
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }
    #endregion
}