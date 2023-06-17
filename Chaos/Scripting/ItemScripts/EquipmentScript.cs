using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class EquipmentScript : ConfigurableItemScriptBase
{
    public EquipmentScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        var template = Subject.Template;

        if (template.Category.Contains("Staff"))
        {
            if (template.TemplateKey.ContainsI("magus") && source.SkillBook.Contains("wieldmagusstaff"))
            {
                EquipStaff(source, template);
                return;
            }

            if (template.TemplateKey.ContainsI("holy") && source.SkillBook.Contains("wieldholystaff"))
            {
                EquipStaff(source, template);
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
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you, but you could grow into it");
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
        source.Equip(template.EquipmentType.Value, Subject);
    }
    
    private void EquipStaff(Aisling source, ItemTemplate template)
    {
        if (template.EquipmentType is not null)
        {
            source.Equip(template.EquipmentType.Value, Subject);
        }
    }

    #region ScriptVars
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }
    #endregion
}