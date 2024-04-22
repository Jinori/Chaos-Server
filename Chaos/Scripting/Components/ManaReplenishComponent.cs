using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.MonsterScripts.Pet;

namespace Chaos.Scripting.Components;

public class ManaReplenishComponent : IComponent
{
    private readonly Animation Sap = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 61
    };

    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IManaReplenishComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        var replenish = options.ManaReplenish ?? 0;

        foreach (var target in targets)
        {
            var finalReplenish = replenish + MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumMp, options.PctManaReplenish);

            if (options.ReplenishGroup)
            {
                if (context.Source is Monster monster && context.Source.Script.Is<PetScript>())
                {
                    var petGroup = monster.PetOwner?.Group?.Where(x => x.WithinRange(target));
                    if (petGroup != null)
                        foreach (var member in petGroup)
                        {
                            member.StatSheet.AddMp(finalReplenish);
                            member.Client.SendAttributes(StatUpdateType.Vitality);
                            member.Animate(Sap);
                        }
                    else
                    {
                        monster.PetOwner?.StatSheet.AddMp(finalReplenish);
                        monster.PetOwner?.Client.SendAttributes(StatUpdateType.Vitality);
                        monster.PetOwner?.Animate(Sap);
                    }
                }
                else
                {
                    var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(target));

                    if (group != null)
                        foreach (var member in group)
                        {
                            member.StatSheet.AddMp(finalReplenish);
                            member.Client.SendAttributes(StatUpdateType.Vitality);
                            member.Animate(Sap);
                        }
                }

                return;
            }

            context.Source.StatSheet.AddMp(finalReplenish);
            context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
        }
    }

    public interface IManaReplenishComponentOptions
    {
        int? ManaReplenish { get; init; }
        decimal PctManaReplenish { get; init; }
        bool ReplenishGroup { get; init; }
    }
}