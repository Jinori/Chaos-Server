using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class EvaluateScript : BasicSkillScriptBase
{
    /// <inheritdoc />
    public EvaluateScript(Skill subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);

        var item = context.SourceAisling?.Inventory.FirstOrDefault();

        if (item is null)
        {
            context.SourceAisling?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                "You should seek to put the item first in your hands.");

            return;
        }

        context.SourceAisling?.Client.SendServerMessage(
                ServerMessageType.ScrollWindow,
                "Name: "
                + item?.DisplayName
                + "\nClass: "
                + item?.Template.Class
                + "\nLevel: " 
                + item?.Template.Level
                + "\nWeight: "
                + item?.Template.Weight
                + "\nMax Stacks: "
                + item?.Template.MaxStacks
                + "\nSell Value: "
                + item?.Template.SellValue
                + "\nBuy Value: "
                + item?.Template.BuyCost
                + "\nDescription: "
                + item?.Template.Description);
    }

    #region ScriptVars
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }
    #endregion
}