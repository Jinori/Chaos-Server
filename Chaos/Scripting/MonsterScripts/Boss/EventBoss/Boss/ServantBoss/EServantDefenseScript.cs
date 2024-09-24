using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.ServantBoss;

public sealed class EServantDefenseScript : MonsterScriptBase
{
    private IIntervalTimer AvoidBashers { get; }

    /// <inheritdoc />
    public EServantDefenseScript(Monster subject)
        : base(subject) =>
        AvoidBashers = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(25),
            25,
            RandomizationType.Positive,
            false);

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
                          && (obj.Id == Subject.AggroList.FirstOrDefault(a => a.Value == Subject.AggroList.Values.Min()).Key));

    private void RemoveEffect(IEffect effect) => Subject.Effects.Dispel(effect.Name);

    private void RemoveEffectAndHeal(IEffect effect)
    {
        Subject.Effects.Dispel(effect.Name);
        Subject.StatSheet.AddHealthPct(15);
        Subject.ShowHealth();
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        AvoidBashers.Update(delta);

        if (AvoidBashers.IntervalElapsed)
        {
            var aislings = Map.GetEntitiesWithinRange<Aisling>(Subject, AggroRange).Where(x => x.ManhattanDistanceFrom(Subject) <= 1).ToList();

            if (aislings.Count >= 2)
            {
                var target = FindLowestAggro();

                if (target != null)
                {
                    var targetPoint = new Point(target.X, target.Y);
                    var bossPoint = new Point(Subject.X, Subject.Y);
                    target.WarpTo(bossPoint);
                    Subject.WarpTo(targetPoint);
                }
            }
        }

        if (!Subject.Effects.Any())
            return;

        foreach (var effect in Subject.Effects)
            switch (effect.Name.ToLowerInvariant())
            {
                case "burn":
                    RemoveEffect(effect);
                    Subject.Say("I've lived in the heat, that does nothing to me.");
                    
                    break;
                case "beagpramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("I won't fall for your weak magic. Only makes me stronger.");

                    break;
                case "pramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("I won't fall for your weak magic. Only makes me stronger.");

                    break;
                case "wolffangfist":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("What a joke, pity Aisling.");

                    break;
                case "suain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("The cold has no effect on me.");

                    break;

                case "beagsuain":
                    RemoveEffect(effect);
                    Subject.Say("What a joke.");

                    break;
                case "blind":
                    RemoveEffect(effect);
                    Subject.Say("I have too many eyes for your nonsense.");

                    break;
                case "dall":
                    RemoveEffect(effect);
                    Subject.Say("I have too many eyes for your nonsense.");

                    break;
            }
    }
}