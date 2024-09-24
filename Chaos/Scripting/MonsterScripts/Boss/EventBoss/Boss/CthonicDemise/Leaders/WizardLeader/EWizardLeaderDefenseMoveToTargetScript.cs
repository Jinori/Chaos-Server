using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CthonicDemise.Leaders.WizardLeader;

public class EWizardLeaderDefenseMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public EWizardLeaderDefenseMoveToTargetScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        
        if (Target == null || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);
        
        if (Subject.Template.TemplateKey.Contains("edarkmasterwanda") || Subject.Template.TemplateKey.Contains("edarkmasterwilliam"))
        {
            if (distance <= 5)
            {
                var pathtopoint = Subject.SpiralSearch(3).OrderByDescending(point => point.ManhattanDistanceFrom(Target))
                    .FirstOrDefault(point => Subject.MapInstance.IsWalkable(point, Subject.Type));
            
                Subject.Pathfind(pathtopoint);
            }
        }
        else
        {
            if (distance != 1)
                Subject.Pathfind(Target);
            else
            {
                var direction = Target.DirectionalRelationTo(Subject);
                Subject.Turn(direction);
            } 
        }

        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}