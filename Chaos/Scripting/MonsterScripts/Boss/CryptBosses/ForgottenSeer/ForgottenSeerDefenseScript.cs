using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CryptBosses.ForgottenSeer;

public sealed class ForgottenSeerDefenseScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ForgottenSeerDefenseScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override bool CanSee(VisibleEntity entity)
    {
        //Can see all persons except GMs
        if (entity.Visibility is VisibilityType.Hidden or VisibilityType.TrueHidden or VisibilityType.Normal)
            return true;

        return false;
    }

    private Aisling? FindLowestAggro()
        => Subject.MapInstance
                  .GetEntitiesWithinRange<Aisling>(Subject, AggroRange)
                  .ThatAreObservedBy(Subject)
                  .FirstOrDefault(
                      obj => !obj.Equals(Subject)
                             && obj.IsAlive
                             && (obj.Id
                                 == Subject.AggroList.FirstOrDefault(a => a.Value == Subject.AggroList.Values.Min())
                                           .Key));

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

        if (!Subject.Effects.Any())
            return;

        foreach (var effect in Subject.Effects)
            switch (effect.Name.ToLowerInvariant())
            {
                case "beag pramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("Sleep is for the weak!");

                    break;
                case "pramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("Sleep is for the weak!");

                    break;
                case "Wolf Fang Fist":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("Don't bother..");

                    break;
                case "suain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("Fool! You cannot stop me!");

                    break;

                case "Beag Suain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("Not a chance!");

                    break;
                case "blind":
                    RemoveEffect(effect);
                    Subject.Say("You cannot blind the blind!");

                    break;
                case "dall":
                    RemoveEffect(effect);
                    Subject.Say("You cannot blind the blind!");

                    break;
            }
    }
}