using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MainStory.FireGuardian;

public sealed class FireGuardianBossDefenseScript : MonsterScriptBase
{
    private readonly ISpellFactory SpellFactory;

    /// <inheritdoc />
    public FireGuardianBossDefenseScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
        => SpellFactory = spellFactory;

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
                    Subject.Say("*burns hotter*");

                    break;
                case "pramh":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*burns hotter*");

                    break;
                case "Wolf Fang Fist":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*heats up*");

                    break;
                case "suain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*heats up*");

                    break;

                case "Beag Suain":
                    RemoveEffectAndHeal(effect);
                    Subject.Say("*unaffected*");

                    break;

                case "dall":
                    RemoveEffect(effect);
                    Subject.Say("*unaffected*");

                    break;
            }
    }
}