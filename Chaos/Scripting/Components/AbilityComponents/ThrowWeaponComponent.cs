using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class ThrowWeaponComponent : IComponent
{
    
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IThrowWeaponComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        
        foreach (var target in targets)
        {
            if (context.TargetMap.IsWall(target) || context.TargetMap.IsBlockingReactor(target))
                return;

            var weaponType = context.SourceAisling?.Equipment[1]?.Template.Category;

            switch (weaponType)
            {
                case "Dagger":
                {
                    var ani = new Animation()
                    {
                        AnimationSpeed = 100,
                        SourceAnimation = 10009,
                        TargetAnimation = 10009,
                        SourceId = context.Source.Id,
                        TargetId = target.Id
                    };
                    context.TargetMap.ShowAnimation(ani);
                    break;
                }
                case "Secret":
                {
                    var ani = new Animation()
                    {
                        AnimationSpeed = 100,
                        SourceAnimation = 10011,
                        TargetAnimation = 10011,
                        SourceId = context.Source.Id,
                        TargetId = target.Id
                    };
                    context.TargetMap.ShowAnimation(ani);
                    break;
                }
                case "Surigum":
                {
                    var ani = new Animation()
                    {
                        AnimationSpeed = 100,
                        SourceAnimation = 10011,
                        TargetAnimation = 10011,
                        SourceId = context.Source.Id,
                        TargetId = target.Id
                    };
                    context.TargetMap.ShowAnimation(ani);
                    break;
                }
                case null:
                    return;
            }
            
            return;
        }
    }

    
    public interface IThrowWeaponComponentOptions
    {
        int DistanceToThrow { get; }
    }
}