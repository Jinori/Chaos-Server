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
    /// <inheritdoc />
    public UpsideDownCastingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />

    private string[] Heals = { "ardioc", "beagioc", "ioc", "morioc", "nuadhaich", "salvation" };

    private string[] Cradhs = { "ardcradh", "morcradh", "cradh", "beagcradh" };

    private string[] Debuffs = { "poison", "suain", "beagpramh", "pramh" };

    private string[] Buffs = { "armachd", "innerfire", "mist", "battlecry", "rage", "warcry" };

    private string[] Attacks =
    {
        "ardathar", "athar", "atharlamh", "atharmeall", "morathar", "beagathar", "beagatharlamh", 
        "ardcreag", "creag", "creaglamh", "creagmeall", "morcreag", "beagcreag", "beagcreaglamh",
        "ardsal", "beagsal", "beagsallamh", "morsal", "sal", "sallamh", "salmeall",
        "ardsrad", "beagsrad", "beagsradlamh", "morsrad", "srad", "sradlamh", "sradmeall",
        "arcaneblast", "arcanebolt", "zap"
    };

        public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Target is not { IsAlive: true } || !ShouldUseSpell || !Target.WithinRange(Subject))
            return;
        
        var hostileAislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject).Where(x => x.WithinRange(Subject, 6)).ToList();
        var friendlyCreatures = Subject.MapInstance.GetEntitiesWithinRange<Creature>(Subject)
            .Where(x => x.WithinRange(Subject, 6) && x is not Aisling).ToList();
        
        if (!Subject.SpellTimer.IntervalElapsed)
            return;
        
        foreach (var spell in Spells)
        {
            //Monsters will heal when under 30% Health
            if (Heals.ContainsI(spell.Template.TemplateKey))
            {
                if (Subject.StatSheet.HealthPercent <= 30)
                {
                    Subject.TryUseSpell(spell, Subject.Id);
                    break;
                }

                foreach (var creature in friendlyCreatures.Where(creature => creature.StatSheet.HealthPercent <= 30))
                {
                    Subject.TryUseSpell(spell, creature.Id);
                    break;
                }

                break;
            }

            //Buffs
            if (Buffs.ContainsI(spell.Template.TemplateKey))
            {
                switch (spell.Template.TemplateKey)
                {
                    case "armachd":
                    {
                        if (!Subject.Effects.Contains("armachd"))
                        {
                            Subject.TryUseSpell(spell, Subject.Id);
                            break;
                        }

                        foreach (var creature in friendlyCreatures.Where(creature => !creature.Effects.Contains("armachd")))
                        {
                            Subject.TryUseSpell(spell, creature.Id);
                            break;
                        }
                        break;
                    }
                    case "innerfire":
                    {
                        if (!Subject.Effects.Contains("innerFire"))
                        {
                            Subject.TryUseSpell(spell, Subject.Id);
                        }

                        break;
                    }
                    case "mist":
                    {
                        if (!Subject.Effects.Contains("mist"))
                        {
                            Subject.TryUseSpell(spell, Subject.Id);
                        }
                        break;
                    }
                    case "battlecry":
                    {
                        if (!Subject.Effects.Contains("battlecry"))
                            Subject.TryUseSpell(spell, Subject.Id);
                        
                        foreach (var creature in friendlyCreatures.Where(creature => !creature.Effects.Contains("battlecry")))
                        {
                            Subject.TryUseSpell(spell, creature.Id);
                            break;
                        }
                        break;
                    }
                    case "warcry":
                    {
                        if (!Subject.Effects.Contains("warcry"))
                            Subject.TryUseSpell(spell, Subject.Id);
                        
                        foreach (var creature in friendlyCreatures.Where(creature => !creature.Effects.Contains("warcry")))
                        {
                            Subject.TryUseSpell(spell, creature.Id);
                            break;
                        }
                        break;
                    }
                    case "rage":
                    {
                        if (!Subject.Effects.Contains("rage"))
                            Subject.TryUseSpell(spell, Subject.Id);
                        
                        foreach (var creature in friendlyCreatures.Where(creature => !creature.Effects.Contains("rage")))
                        {
                            Subject.TryUseSpell(spell, creature.Id);
                            break;
                        }
                        break;
                    }
                }

                break;
            }
            
            
            //Monster will apply any debuffs before using damage spells
            if (Cradhs.ContainsI(spell.Template.TemplateKey) && Subject.Effects.Contains("ard cradh") || Subject.Effects.Contains("mor cradh") || Subject.Effects.Contains("cradh") || Subject.Effects.Contains("beag cradh"))
            {
                foreach (var aisling in hostileAislings)
                {
                    Subject.TryUseSpell(spell, aisling.Id);
                }

                break;
            }

            if (Debuffs.ContainsI(spell.Template.TemplateKey))
            {
                foreach (var aisling in hostileAislings)
                {
                    if (!aisling.Status.HasFlag(Status.Pramh) && spell.Template.TemplateKey.EqualsI("pramh") ||
                        spell.Template.TemplateKey.EqualsI("beagpramh"))
                    {
                        Subject.TryUseSpell(spell, aisling.Id);
                        break;
                    }

                    if (aisling.Status.HasFlag(Status.Suain) && spell.Template.TemplateKey.EqualsI("pramh"))
                    {
                        Subject.TryUseSpell(spell, aisling.Id);
                        break;
                    }

                    if (aisling.UserStatSheet.BaseClass == BaseClass.Wizard && spell.Template.TemplateKey.EqualsI("preventaffliction"))
                    {
                        Subject.TryUseSpell(spell, aisling.Id);
                        break;
                    }
                }

                break;
            }
            
            //Lets only do major attacks if mana is more than 50%
            if (Subject.StatSheet.ManaPercent >= 35 && Attacks.ContainsI(spell.Template.TemplateKey))
            {
                Subject.TryUseSpell(spell, Target.Id);
                Subject.WanderTimer.Reset();
                Subject.MoveTimer.Reset();
                Subject.SkillTimer.Reset();
                break;
            }
        }

        if (Subject.SpellTimer.IntervalElapsed)
        {
            Subject.SpellTimer.Reset();
        }
    }
}