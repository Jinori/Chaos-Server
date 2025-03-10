using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CryptBosses.DeepCryptRat;

public sealed class DeepCryptRatDelayDefenseScript : MonsterScriptBase
{
    private readonly Dictionary<IEffect, DateTime> AppliedEffects = new();
    private readonly IIntervalTimer UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200));

    public DeepCryptRatDelayDefenseScript(Monster subject)
        : base(subject) { }

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

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        UpdateTimer.Update(delta);

        if (!Subject.Effects.Any())
            return;

        if (!UpdateTimer.IntervalElapsed)
            return;

        var currentTime = DateTime.UtcNow;
        var effectsToRemove = new List<IEffect>();

        foreach (var effect in Subject.Effects)
            if (!AppliedEffects.ContainsKey(effect))

                // Add new effect with the current time
                AppliedEffects[effect] = currentTime;
            else
            {
                var appliedTime = AppliedEffects[effect];

                if (((currentTime - appliedTime).TotalSeconds >= 3) && ((currentTime - appliedTime).TotalSeconds <= 5))
                {
                    switch (effect.Name.ToLowerInvariant())
                    {
                        case "beag pramh":
                        case "pramh":
                            RemoveEffectAndHeal(effect);

                            break;
                        case "Wolf Fang Fist":
                            RemoveEffectAndHeal(effect);

                            break;
                        case "suain":
                            RemoveEffectAndHeal(effect);

                            break;
                        case "Beag Suain":
                        case "blind":
                        case "dall":
                            RemoveEffect(effect);

                            break;
                    }

                    effectsToRemove.Add(effect);
                }
            }

        // Remove effects that have been processed
        foreach (var effect in effectsToRemove)
            AppliedEffects.Remove(effect);
    }
}