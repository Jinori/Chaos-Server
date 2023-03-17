using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class EquipmentScript : ConfigurableItemScriptBase
{
    public EquipmentScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        var template = Subject.Template;

        if (template.TemplateKey.ContainsI("magus"))
        {
            if (source.SkillBook["wieldmagusstaff"] != null)
            {
                source.Equip(template.EquipmentType, Subject);

                return;
            }
        }
        
        if (template.TemplateKey.ContainsI("holy"))
        {
            if (source.SkillBook["wieldholystaff"] != null)
            {
                source.Equip(template.EquipmentType, Subject);

                return;
            }
        }

        if (template.EquipmentType == EquipmentType.NotEquipment)
        {
            source.SendOrangeBarMessage("You can't equip that");

            return;
        }

        //gender check
        if (!template.Gender.HasFlag(source.Gender))
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

        if (StatRequired.HasValue
            && StatAmountRequired.HasValue
            && (source.StatSheet.GetBaseStat(StatRequired.Value) < StatAmountRequired.Value))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} requires {StatAmountRequired.Value} {StatRequired} to equip.");

            return;
        }

        source.Equip(template.EquipmentType, Subject);
    }

    #region ScriptVars
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }
    #endregion
}