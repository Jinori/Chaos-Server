using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class StudyCreatureScript : ConfigurableSkillScriptBase, AbilityComponent<Creature>.IAbilityComponentOptions
{
    /// <inheritdoc />
    public StudyCreatureScript(Skill subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var points = AoeShape.Front.ResolvePoints(
            context.Source,
            3,
            context.Direction,
            excludeSource: true);

        var mob = context.SourceMap.GetEntitiesAtPoints<Creature>(points.OfType<IPoint>()).FirstOrDefault();
        
        if (mob is not null)
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

    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }
    /// <inheritdoc />
    public TargetFilter Filter { get; init; }
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    /// <inheritdoc />
    public int Range { get; init; }
    /// <inheritdoc />
    public AoeShape Shape { get; init; }
    /// <inheritdoc />
    public byte? Sound { get; init; }
    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    /// <inheritdoc />
    public Animation? Animation { get; init; }
    /// <inheritdoc />
    public int? ManaCost { get; init; }
    /// <inheritdoc />
    public decimal PctManaCost { get; init; }
    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }
}