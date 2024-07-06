using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Upside_Down
{
    public sealed class UpsideCastingScript : MonsterScriptBase
    {
        private readonly IEffectFactory _effectFactory;

        public UpsideCastingScript(Monster subject, IEffectFactory effectFactory)
            : base(subject) =>
            _effectFactory = effectFactory;

        private static readonly string[] HealingSpells =
        [
            "ardioc", "morioc", "nuadhaich", "salvation", "morioccomlha", "ioccomlha"
        ];

        private static readonly string[] AoSpells =
        [
            "aoardcradh", "aomorcradh", "aodall", "aopoison", "aosuain", "dinarcoli"
        ];

        private static readonly string[] SelfBuffSpells =
        [
            "dodge", "earthenstance", "innerfire", "laststand", "miststance", "smokestance", "thunderstance", 
            "battlecry", "warcry", "wrath", "rage", "asgallfaileas", "focus", "laststand", "armachd"
        ];
        
        private static readonly string[] TargetedDebuffSpells =
        [
            "ardcradh", "morcradh", "aosith", "dall", "poison", "suain", "pramh", "preventaffliction"
        ];

        private static readonly Dictionary<string, string> AoDebuffMap = new()
        {
            { "aoardcradh", "ardcradh" },
            { "aomorcradh", "morcradh" },
            { "aodall", "dall" },
            { "aosuain", "suain" },
            { "dinarcoli", "pramh" },
            { "aopoison", "poison" }
        };
        
        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            if ((Subject.Spells.Count == 0) || !Subject.SpellTimer.IntervalElapsed)
                return;

            var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 12).ToList();
            var friendlyCreatures = Subject.MapInstance.GetEntitiesWithinRange<Creature>(Subject, 12)
                                        .Where(x => x.IsFriendlyTo(Subject) && !x.IsDead).ToList();

            if (aislings.Count <= 0)
                return;

            foreach (var spell in Subject.Spells)
            {
                if (TryCastHealingSpell(spell, friendlyCreatures))
                    return;

                if (TryCastAoSpell(spell, friendlyCreatures))
                    return;

                if (TryCastSelfBuffSpell(spell, friendlyCreatures))
                    return;
            }
        }

        private bool TryCastHealingSpell(Spell spell, List<Creature> friendlyCreatures)
        {
            if (!HealingSpells.Contains(spell.Template.TemplateKey) || (friendlyCreatures.Count == 0))
                return false;

            var needsHeal = friendlyCreatures.FirstOrDefault(x => x.StatSheet.HealthPercent <= 50);
            if (needsHeal != null)
            {
                Subject.TryUseSpell(spell, needsHeal.Id);
                return true;
            }

            return false;
        }


        private bool TryCastAoSpell(Spell spell, List<Creature> friendlyCreatures)
        {
            if (!AoSpells.Contains(spell.Template.TemplateKey))
                return false;

            var debuffToRemove = AoDebuffMap.GetValueOrDefault(spell.Template.TemplateKey);
            if (debuffToRemove != null)
            {
                var targetWithDebuff = friendlyCreatures.FirstOrDefault(creature =>
                    creature.Effects.Any(effect => effect.Name == debuffToRemove));

                if (targetWithDebuff != null)
                {
                    Subject.TryUseSpell(spell, targetWithDebuff.Id);
                    return true;
                }
            }

            return false;
        }

        private bool TryCastSelfBuffSpell(Spell spell, List<Creature> friendlyCreatures)
        {
            if (!SelfBuffSpells.Contains(spell.Template.TemplateKey))
                return false;

            if ((spell.Template.TemplateKey == "laststand") && (Subject.StatSheet.HealthPercent <= 10))
            {
                Subject.TryUseSpell(spell, Subject.Id);
                return true;
            }

            if (spell.Template.TemplateKey == "warcry")
            {
                ApplyWarCryBuff(friendlyCreatures);
                return true;
            }

            Subject.TryUseSpell(spell, Subject.Id);
            return true;
        }

        private void ApplyWarCryBuff(List<Creature> friendlyCreatures)
        {
            foreach (var friend in friendlyCreatures)
            {
                var effect = _effectFactory.Create("battlecry");
                friend.Effects.Apply(Subject, effect);
            }
        }
    }
}