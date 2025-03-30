using System.Collections.Immutable;
using Chaos.Extensions.Common;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.EffectScripts.Abstractions;

public abstract class HierarchicalIntervalEffectBase : IntervalEffectBase
{
    protected abstract ImmutableArray<string> ReplaceHierarchy { get; }

    public override bool ShouldApply(Creature source, Creature target)
    {
        //check if any effect in the hierarchy is active
        var current = target.Effects.FirstOrDefault(e => ReplaceHierarchy.ContainsI(e.Name))
                            ?.Name;

        //determine via other rules if this effect should apply
        var shouldApply = base.ShouldApply(source, target);

        //if the effect should apply
        //check if we need to dispel a lower rank effect in the hierarchy
        if (shouldApply && current is not null)
        {
            if (!ShouldReplace(current))
                return false;

            target.Effects.Dispel(current);
        }

        return shouldApply;
    }

    protected virtual bool ShouldReplace(string current)
    {
        var currentRank = ReplaceHierarchy.IndexOf(current);
        var newRank = ReplaceHierarchy.IndexOf(Name);

        return newRank <= currentRank;
    }
}