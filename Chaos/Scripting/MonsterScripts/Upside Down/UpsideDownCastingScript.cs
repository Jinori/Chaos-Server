using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Upside_Down;

// ReSharper disable once ClassCanBeSealed.Global
public class UpsideDownCastingScript : MonsterScriptBase
{
    private const int HEALTH_THRESHOLD = 30;
    private const int MANA_THRESHOLD = 35;

    private static readonly string[] HealingSpells =
    {
        "ardioc", "beagioc", "ioc", "morioc", "nuadhaich", "salvation"
    };

    private static readonly string[] DebuffSpells =
    {
        "poison", "suain", "beagpramh", "pramh"
    };

    private static readonly string[] BuffSpells =
    {
        "armachd", "innerfire", "mist", "battlecry", "rage", "warcry"
    };

    private static readonly string[] AttackSpells =
    {
        "ardathar", "athar", "atharlamh", "atharmeall", "morathar", "beagathar", "beagatharlamh",
        "ardcreag", "creag", "creaglamh", "creagmeall", "morcreag", "beagcreag", "beagcreaglamh",
        "ardsal", "beagsal", "beagsallamh", "morsal", "sal", "sallamh", "salmeall",
        "ardsrad", "beagsrad", "beagsradlamh", "morsrad", "srad", "sradlamh", "sradmeall",
        "arcaneblast", "arcanebolt", "zap"
    };

    /// <inheritdoc />
    public UpsideDownCastingScript(Monster subject)
        : base(subject) { }

    private bool ShouldApplyBuff(string buffSpell, Creature creature) => !creature.Effects.Contains(buffSpell);

    private bool ShouldApplyDebuff(string debuffSpell, Aisling aisling)
    {
        if (debuffSpell.EqualsI("pramh") || debuffSpell.EqualsI("beagpramh"))
        {
            if (!aisling.Status.HasFlag(Status.Pramh))
                return true;

            if (aisling.Status.HasFlag(Status.Suain) && debuffSpell.EqualsI("pramh"))
                return true;

            if ((aisling.UserStatSheet.BaseClass == BaseClass.Wizard) && debuffSpell.EqualsI("preventaffliction"))
                return true;
        }

        return false;
    }

    private bool ShouldHeal(int healthPercent) => healthPercent <= HEALTH_THRESHOLD;

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Target is not { IsAlive: true } || !ShouldUseSpell || !Target.WithinRange(Subject))
            return;

        var hostileAislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 6).ToList();
        var friendlyCreatures = Subject.MapInstance.GetEntitiesWithinRange<Creature>(Subject, 6).Where(x => x is not Aisling).ToList();

        if (!Subject.SpellTimer.IntervalElapsed)
            return;

        foreach (var spell in Spells)
        {
            // Healing Spells
            if (HealingSpells.ContainsI(spell.Template.TemplateKey))
            {
                if (ShouldHeal(Subject.StatSheet.HealthPercent))
                {
                    Subject.TryUseSpell(spell, Subject.Id);

                    break;
                }

                var creatureToHeal = friendlyCreatures.FirstOrDefault(
                    creature =>
                        ShouldHeal(creature.StatSheet.HealthPercent));

                if (creatureToHeal != null)
                {
                    Subject.TryUseSpell(spell, creatureToHeal.Id);

                    break;
                }
            }

            // Buff Spells
            if (BuffSpells.ContainsI(spell.Template.TemplateKey))
            {
                if (ShouldApplyBuff(spell.Template.TemplateKey, Subject))
                    Subject.TryUseSpell(spell, Subject.Id);

                var creatureToBuff = friendlyCreatures.FirstOrDefault(
                    creature =>
                        ShouldApplyBuff(spell.Template.TemplateKey, creature));

                if (creatureToBuff != null)
                {
                    Subject.TryUseSpell(spell, creatureToBuff.Id);

                    break;
                }
            }

            // Debuff Spells
            if (DebuffSpells.ContainsI(spell.Template.TemplateKey))
            {
                var aislingToDebuff = hostileAislings.FirstOrDefault(
                    aisling =>
                        ShouldApplyDebuff(spell.Template.TemplateKey, aisling));

                if (aislingToDebuff != null)
                {
                    Subject.TryUseSpell(spell, aislingToDebuff.Id);

                    break;
                }
            }

            // Attack Spells
            if (AttackSpells.ContainsI(spell.Template.TemplateKey))
                if (Subject.StatSheet.ManaPercent >= MANA_THRESHOLD)
                {
                    Subject.TryUseSpell(spell, Target.Id);
                    Subject.WanderTimer.Reset();
                    Subject.MoveTimer.Reset();
                    Subject.SkillTimer.Reset();

                    break;
                }
        }

        if (Subject.SpellTimer.IntervalElapsed)
            Subject.SpellTimer.Reset();
    }
}