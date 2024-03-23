using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("learnall")]
public class LearnAllCommand : ICommand<Aisling>
{
    private readonly Dictionary<BaseClass, (string[] skills, string[] spells)> _classSkillsAndSpells;
    private readonly ISkillFactory _skillFactory;
    private readonly ISpellFactory _spellFactory;

    public LearnAllCommand(ISpellFactory spellFactory, ISkillFactory skillFactory)
    {
        _spellFactory = spellFactory;
        _skillFactory = skillFactory;

        _classSkillsAndSpells = new Dictionary<BaseClass, (string[] skills, string[] spells)>
        {
            {
                BaseClass.Warrior,
                (
                    new[]
                    {
                        "aoBeagSuain", "beagSuain", "charge", "cleave", "clobber", "crasher", "execute", "flank", "MadSoul", "sever",
                        "slash", "slaughter", "Thrash", "wallop", "windblade", "pulverize", "tempestblade", "strike"
                    },
                    new[] { "asgallfaileas", "battlecry", "rage", "warcry", "wrath" }
                )
            },
            {
                BaseClass.Rogue,
                (
                    new[]
                    {
                        "assassinStrike", "amnesia", "assault", "birthdaySuit", "midnightSlash", "pierce", "PoisonBomb", "sapNeedle",
                        "shadowFigure", "smokescreen", "stab", "stabAndTwist", "stabTwice", "studyCreature", "throwSmokeBomb",
                        "throwSurigum"
                    },
                    new[]
                    {
                        "bolttrap", "coiledBolttrap", "detectTraps", "focus", "hide", "maidentrap", "needletrap", "see_hide", "springtrap",
                        "stilettotrap"
                    }
                )
            },
            {
                BaseClass.Wizard,
                (
                    new[] { "wieldmagusstaff", "rumination", "assail" },
                    new[]
                    {
                        "arcaneBlast", "arcaneBolt", "ardathar", "athar", "atharlamh", "atharmeall", "beagathar", "beagatharlamh",
                        "morathar", "ardcreag", "beagcreag", "beagcreaglamh", "creag", "creaglamh", "creagmeall", "morcreag", "ardcradh",
                        "beagcradh", "cradh", "morcradh", "beagPramh", "pramh", "ardsal", "beagsal", "beagsallamh", "morsal", "sal",
                        "sallamh", "salmeall", "ardsrad", "beagsrad", "beagsradlamh", "morsrad", "srad", "sradlamh", "sradmeall",
                        "homeWizard"
                    }
                )
            },
            {
                BaseClass.Priest,
                (
                    new[] { "wieldholystaff", "summonpet", "assail" },
                    new[]
                    {
                        "aoardcradh", "aobeagcradh", "aocradh", "aomorcradh", "aopoison", "aosith", "dinarcoli", "ardnaomhaite", "armachd",
                        "beagnaomhaite", "beannaich", "mornaomhaite", "motivate", "naomhaite", "zap", "poison", "preventAffliction",
                        "suain", "ardioc", "beagIoc", "beagIocComlha", "ioc", "IocComlha", "morioc", "morIocComlha", "nuadhaich",
                        "salvation", "beothaich", "revive", "selfrevive", "fasdeireas", "morfasdeireas", "quake", "morbeannaich",
                        "homePriest"
                    }
                )
            },
            {
                BaseClass.Monk,
                (
                    new[]
                    {
                        "Airpunch", "ambush", "clawfist", "doublePunch", "dracotailkick", "eaglestrike", "firepunch", "highkick", "kick",
                        "mantiskick", "poisonpunch", "punch", "earthpunch", "roundhousekick", "tripleKick", "waterpunch", "wolfFangFist"
                    },
                    new[]
                    {
                        "dodge", "earthenstance", "gentleTouch", "goad", "howl", "innerFire", "laststand", "miststance", "smokestance",
                        "taunt", "thunderstance"
                    }
                )
            }
        };
    }

    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!_classSkillsAndSpells.TryGetValue(source.UserStatSheet.BaseClass, out var skillsAndSpells))
            return default;

        AddSkillsAndSpells(source, skillsAndSpells.skills, skillsAndSpells.spells);

        return default;
    }

    private void AddSkillsAndSpells(Aisling source, string[] skills, string[] spells)
    {
        foreach (var skill in skills)
        {
            var temp = _skillFactory.Create(skill);
            source.SkillBook.TryAddToNextSlot(temp);
        }

        foreach (var spell in spells)
        {
            var temp = _spellFactory.Create(spell);
            source.SpellBook.TryAddToNextSlot(temp);
        }
    }
}