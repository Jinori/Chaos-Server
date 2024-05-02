using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MainStory.EarthGuardian;

public sealed class EarthGuardianBossDefenseScript : MonsterScriptBase
{
    private IIntervalTimer PullLowestTarget { get; }
    private readonly ISpellFactory SpellFactory;

    /// <inheritdoc />
    public EarthGuardianBossDefenseScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        PullLowestTarget = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(20),
            45,
            RandomizationType.Positive,
            false);
    }

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
        Subject.StatSheet.AddHealthPct(35);
        Subject.ShowHealth();
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        PullLowestTarget.Update(delta);

        if (PullLowestTarget.IntervalElapsed)
        {
            var aislings = Map.GetEntitiesWithinRange<Aisling>(Subject, AggroRange).Where(x => x.DistanceFrom(Subject) <= 1).ToList();

            if (aislings.Count >= 2)
            {
                var target = FindLowestAggro();

                if (target != null)
                {
                    var bossPoint = new Point(Subject.X + 1, Subject.Y);
                    target.WarpTo(bossPoint);
                    var spelltocast = SpellFactory.Create("morcreaglamh");
                    Subject.TryUseSpell(spelltocast);
                }
            }
        }

        if (!Subject.Effects.Any())
            return;

        foreach (var effect in Subject.Effects)
            switch (effect.Name.ToLowerInvariant())
            {
                case "beagpramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*murmurs*");

                    break;
                case "pramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*murmurs*");

                    break;
                case "wolffangfist":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*grunts*");

                    break;
                case "suain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*grunts*");

                    break;

                case "beagsuain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*squish*");

                    break;
                case "blind":
                    RemoveEffect(effect);
                    Subject.Say("*squish*");

                    break;
                case "dall":
                    RemoveEffect(effect);
                    Subject.Say("*squish*");

                    break;
            }
    }
}