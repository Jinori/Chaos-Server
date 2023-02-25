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

        if (template.Class.HasValue && !template.Class.Value.ContainsClass(source.UserStatSheet.BaseClass))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (template.AdvClass.HasValue && (template.AdvClass.Value != source.UserStatSheet.AdvClass))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (template.Level > source.UserStatSheet.Level)
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        if (StatRequired.HasValue
            && StatAmountRequired.HasValue
            && (source.StatSheet.GetBaseStat(StatRequired.Value) < StatAmountRequired.Value))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        source.Equip(template.EquipmentType, Subject);
    }

    #region ScriptVars
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }
    #endregion
}