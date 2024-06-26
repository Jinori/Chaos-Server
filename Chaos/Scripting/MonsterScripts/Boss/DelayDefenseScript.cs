using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss
{
    public sealed class DelayDefenseScript : MonsterScriptBase
    {
        private readonly IIntervalTimer UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200));
        private readonly Dictionary<IEffect, DateTime> AppliedEffects = new();

        public DelayDefenseScript(Monster subject)
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
            {
                if (!AppliedEffects.ContainsKey(effect))
                {
                    // Add new effect with the current time
                    AppliedEffects[effect] = currentTime;
                }
                else
                {
                    var appliedTime = AppliedEffects[effect];
                    if ((currentTime - appliedTime).TotalSeconds >= 3 && (currentTime - appliedTime).TotalSeconds <= 5)
                    {
                        switch (effect.Name.ToLowerInvariant())
                        {
                            case "beagpramh":
                            case "pramh":
                                RemoveEffectAndHeal(effect);
                                Subject.Say("Nice try!");
                                break;
                            case "wolffangfist":
                                RemoveEffectAndHeal(effect);
                                Subject.Say("Don't bother..");
                                break;
                            case "suain":
                                RemoveEffectAndHeal(effect);
                                Subject.Say("Not a chance!");
                                break;
                            case "beagsuain":
                            case "blind":
                            case "dall":
                                RemoveEffect(effect);
                                Subject.Say("Not a chance!");
                                break;
                        }
                        effectsToRemove.Add(effect);
                    }
                }
            }

            // Remove effects that have been processed
            foreach (var effect in effectsToRemove)
            {
                AppliedEffects.Remove(effect);
            }
        }
    }
}
