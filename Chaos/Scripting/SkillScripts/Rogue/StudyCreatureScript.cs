using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class StudyCreatureScript : BasicSkillScriptBase
{
    protected IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public StudyCreatureScript(Skill subject)
        : base(subject) =>
        ApplyDamageScript = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        var mob = targets.TargetEntities.FirstOrDefault();
        if (mob != null)
        {
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.ScrollWindow,
                $"Name: {mob.Name}\nLevel: {mob.StatSheet.Level}\nCurrent Health: {mob.StatSheet.CurrentHp}\nArmor Class: {mob.StatSheet.EffectiveAc}\nCurrent Mana: {mob.StatSheet.CurrentMp}\nOffensive Element: {mob.StatSheet.OffenseElement}\nDefensive Element: {mob.StatSheet.DefenseElement}"
            );
            var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));
            if (group != null)
            {
                foreach (var entity in group)
                {
                    var showMobEle = entity.MapInstance.GetEntities<Creature>().FirstOrDefault(x => x.Equals(mob));
                    showMobEle?.Chant(showMobEle.StatSheet.DefenseElement.ToString());
                }
            }
        }
        if (mob == null)
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your attempt to examine failed.");
    }
}