using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Phoenix;

// phoenix will cast hail of feathers on clusters of aislings

public class PhoenixCastingScript : MonsterScriptBase
{
    private readonly Spell HailOfFeathers;

    /// <inheritdoc />
    public PhoenixCastingScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
        => HailOfFeathers = spellFactory.Create("phoenix_hailOfFeathers");

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        HailOfFeathers.Update(delta);
        
        if (Map.LoadedFromInstanceId.ContainsI("sky"))
            return;

        if (!ShouldUseSpell)
            return;
        
        //get nearby aislings
        var nearbyAislings = Map.GetEntitiesWithinRange<Aisling>(Subject)
                                .ToList();

        //group by cluster size
        var clusters = nearbyAislings.GroupBy(aisling => nearbyAislings.Count(other => aisling.WithinRange(other, 3)));

        var target = clusters.OrderByDescending(group => group.Key)
                             .ThenBy(_ => Random.Shared.Next())
                             .FirstOrDefault()
                             ?.FirstOrDefault();
        
        if (target is not { IsAlive: true } || !target.WithinRange(Subject))
            return;

        Subject.TryUseSpell(HailOfFeathers, target.Id);
    }
}