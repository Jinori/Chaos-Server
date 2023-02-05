using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts.Rogue;

public class StudyCreatureScript : BasicSkillScriptBase
{
    protected IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public StudyCreatureScript(Skill subject)
        : base(subject) =>
        ApplyDamageScript = DefaultApplyDamageScript.Create();

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        var mob = targets.TargetEntities.FirstOrDefault();
        
        if (mob is not null)
        {
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.ScrollWindow, "Name: " + mob.Name + "\nLevel: " + mob.StatSheet.Level + "\nCurrent Health: {=b" + mob.StatSheet.CurrentHp + "\n{=hCurrent Mana: {=v" + mob.StatSheet.CurrentMp
                + "\n{=hOffensive Element: " + mob.StatSheet.OffenseElement + "\nDefensive Element: " + mob.StatSheet.DefenseElement);
            var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));
            if (group is not null)
            {
                foreach (var entity in group)
                {
                    var showMobEle = entity.MapInstance.GetEntities<Creature>().FirstOrDefault(x => x.Equals(mob));
                    showMobEle?.Chant(showMobEle.StatSheet.DefenseElement.ToString());
                }
            }
        }
        if (mob is null)
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your attempt to examine failed.");
    }
}