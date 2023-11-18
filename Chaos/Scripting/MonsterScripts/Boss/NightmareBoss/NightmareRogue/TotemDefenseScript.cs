using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.NightmareBoss.NightmareRogue;

public sealed class TotemDefenseScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TotemDefenseScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override bool CanSee(VisibleEntity entity)
    {
        //Can see all persons except GMs
        if (entity.Visibility is VisibilityType.Hidden or VisibilityType.TrueHidden or VisibilityType.Normal)
            return true;

        return false;
    }

    private Aisling? FindLowestAggro() =>
        Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, AggroRange)
               .ThatAreObservedBy(Subject)
               .FirstOrDefault(
                   obj => !obj.Equals(Subject)
                          && obj.IsAlive
                          && (obj.Id
                              == Subject.AggroList.FirstOrDefault(a => a.Value == Subject.AggroList.Values.Min()).Key));

    private void RemoveEffect(IEffect effect) => Subject.Effects.Dispel(effect.Name);

    private void RemoveEffectAndHeal(IEffect effect) => Subject.Effects.Dispel(effect.Name);

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (!Subject.Effects.Any())
            return;

        foreach (var effect in Subject.Effects)
            switch (effect.Name.ToLowerInvariant())
            {
                case "beagpramh":
                    RemoveEffectAndHeal(effect);

                    break;
                case "pramh":
                    RemoveEffectAndHeal(effect);

                    break;
                case "wolffangfist":
                    RemoveEffectAndHeal(effect);

                    break;
                case "suain":
                    RemoveEffectAndHeal(effect);

                    break;

                case "beagsuain":
                    RemoveEffectAndHeal(effect);

                    break;
                case "blind":
                    RemoveEffect(effect);

                    break;
                case "dall":
                    RemoveEffect(effect);

                    break;

                case "poison":
                    RemoveEffect(effect);

                    break;

                case "amnesia":
                    RemoveEffect(effect);

                    break;
            }
    }
}