using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MainStory.SeaGuardian;

public sealed class SeaGuardianBossDefenseScript : MonsterScriptBase
{
    private readonly ISpellFactory SpellFactory;

    /// <inheritdoc />
    public SeaGuardianBossDefenseScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
    }

    /// <inheritdoc />
    public override bool CanSee(VisibleEntity entity)
    {
        //Can see all persons except GMs
        if (entity.Visibility is VisibilityType.Hidden or VisibilityType.TrueHidden or VisibilityType.Normal)
            return true;

        return false;
    }

    private void RemoveEffect(IEffect effect)
    {
        Subject.Effects.Dispel(effect.Name);
    }

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
                case "beagpramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*washes off*");

                    break;
                case "pramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*washes off*");

                    break;
                case "wolffangfist":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*washes off*");

                    break;
                case "suain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*brrr*");

                    break;

                case "beagsuain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*unaffected*");

                    break;
                case "blind":
                    RemoveEffect(effect);
                    Subject.Say("*unaffected*");

                    break;
                case "dall":
                    RemoveEffect(effect);
                    Subject.Say("*unaffected*");

                    break;
            }
    }
}