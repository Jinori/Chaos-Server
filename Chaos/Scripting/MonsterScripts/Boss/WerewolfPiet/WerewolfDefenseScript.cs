using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.WerewolfPiet;

public sealed class WerewolfDefenseScript : MonsterScriptBase
{
    /// <inheritdoc />
    public WerewolfDefenseScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override bool CanSee(VisibleEntity entity)
    {
        //Can see all persons except GMs
        if (entity.Visibility is VisibilityType.Hidden or VisibilityType.TrueHidden or VisibilityType.Normal)
            return true;

        return false;
    }

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

        if (!Subject.Effects.Any())
            return;

        foreach (var effect in Subject.Effects)
            switch (effect.Name.ToLowerInvariant())
            {
                case "beag pramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("Nice try!");

                    break;
                case "pramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("Nice try!");

                    break;
                case "Wolf Fang Fist":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("Don't bother..");

                    break;
                case "suain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("Not a chance!");

                    break;

                case "Beag Suain":
                    RemoveEffect(effect);
                    Subject.Say("Not a chance!");

                    break;
                case "blind":
                    RemoveEffect(effect);
                    Subject.Say("Not a chance!");

                    break;
                case "dall":
                    RemoveEffect(effect);
                    Subject.Say("Not a chance!");

                    break;
            }
    }
}