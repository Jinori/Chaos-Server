using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class ThrowWeaponComponent : IComponent
{
    
    
    //Animations
    //10000 - Single Wooden Arrow
    //10001 - Double Crystal Arrow
    //10002 - Triple Wooden Arrow
    //10003 - Single Fire Arrow
    //10004 - Double Fire Arrow
    //10005 - Triple Fire Arrow
    //10006 - Single Crystal Arrow
    //10007 - Double Wooden Arrow
    //10008 - Triple Crystal Arrow
    //10009 - Daggers
    //10010 - Apples
    //10011 - Surigum   
    //10012 - Big Surigum
    //10013 - Rock    
    
    public void Execute(ActivationContext context, ComponentVars vars)
    {
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
}