using System.Collections.Immutable;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.EffectScripts.HideEffects;

namespace Chaos.Scripting.Behaviors;

public class VisibilityBehavior
{
    private readonly ImmutableList<string> SeeHiddenEffects = ImmutableList.Create(
        EffectBase.GetEffectKey(typeof(SeeHideEffect)),
        EffectBase.GetEffectKey(typeof(SeeTrueHideEffect)));

    private readonly ImmutableList<string> SeeTrueHiddenEffects = ImmutableList.Create(EffectBase.GetEffectKey(typeof(SeeTrueHideEffect)));

    public virtual bool CanSee(Creature creature, VisibleEntity entity)
    {
        switch (entity.Visibility)
        {
            case VisibilityType.Normal:
                return true;
            
            case VisibilityType.Hidden:
            {
                if (creature is Aisling aisling && entity is Aisling)
                {
                    if ((aisling.Group != null) && aisling.Group.Contains(entity))
                        return true;
                }
                
                return SeeHiddenEffects.Any(key => creature.Effects.Contains(key));   
            }
            case VisibilityType.TrueHidden:
            {
                if (creature is Aisling aisling && entity is Aisling)
                {
                    if ((aisling.Group != null) && aisling.Group.Contains(entity))
                        return true;
                }
                return SeeTrueHiddenEffects.Any(key => creature.Effects.Contains(key));
            }
            default:
                return false;
        }
    }
}