using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("learnall", true)]
public class LearnAllCommand : ICommand<Aisling>
{
    private readonly ISkillFactory _skillFactory;
    private readonly ISpellFactory _spellFactory;

    public LearnAllCommand(ISpellFactory spellFactory, ISkillFactory skillFactory)
    {
        _spellFactory = spellFactory;
        _skillFactory = skillFactory;
    }

    #region SkillSpellList
    private readonly string[] _priestSpells =
    {
        "aobeagcradh", "aocradh", "aomorcradh", "aoardcradh", "aopoison", "aosith", "dinarcoli", "armachd", "beannaich", "motivate", "zap",
        "poison", "suain", "beagioc", "ioc", "morioc", "ardioc", "nuadhaich", "salvation", "beagioccomlha", "ioccomlha", "morioccomlha",
        "beothaich", "revive", "selfrevive"
    };

    private readonly string[] _priestSkills = { "wieldholystaff" };

    private readonly string[] _monkSpells =
    {
        "gentletouch", "goad", "howl", "innerfire", "laststand", "dodge", "thunderstance", "smokestance", "earthenstance", "miststance"
    };

    private readonly string[] _monkSkills =
    {
        "airpunch", "firepunch", "rockpunch", "waterpunch", "ambush", "clawfist", "doublepunch", "dracotailkick", "eaglestrike", "highkick",
        "kick", "mantiskick",
        "poisonpunch", "punch", "roundhousekick", "triplekick", "wolffangfist"
    };

    private readonly string[] _rogueSpells =
    {
        "needleTrap", "stilettoTrap", "boltTrap", "coiledBoltTrap", "springTrap", "maidenTrap", "detectTraps"
    };

    private readonly string[] _rogueSkills =
    {
        "birthdaysuit", "pierce", "sapNeedle", "shadowFigure", "smokescreen", "stab", "stabAndTwist", "stabTwice",
        "studyCreature", "throwsmokebomb"
    };

    private readonly string[] _warriorSpells =
    {
        "asgallfaileas", "battlecry", "rage", "warcry", "wrath"
    };

    private readonly string[] _warriorSkills =
    {
        "assault", "clobber", "flank", "wallop", "aobeagsuain", "beagsuain", "charge", "cleave", "crasher", "execute", "sever", "slash",
        "windblade"
    };

    private readonly string[] _wizardSkills =
    {
        "wieldmagusstaff"
    };

    private readonly string[] _wizardSpells =
    {
        "rumination", "beagcradh", "cradh", "morcradh", "ardcradh", "beagpramh", "pramh", "arcaneblast", "arcanebolt"
    };
    #endregion

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        switch (source.UserStatSheet.BaseClass)
        {
            case BaseClass.Warrior:
                foreach (var skill in _warriorSkills)
                {
                    var temp = _skillFactory.Create(skill);
                    source.SkillBook.TryAddToNextSlot(temp);
                }

                foreach (var spell in _warriorSpells)
                {
                    var temp = _spellFactory.Create(spell);
                    source.SpellBook.TryAddToNextSlot(temp);
                }

                break;
            case BaseClass.Rogue:
                foreach (var rogueSkill in _rogueSkills)
                {
                    var temp = _skillFactory.Create(rogueSkill);
                    source.SkillBook.TryAddToNextSlot(temp);
                }

                foreach (var rogueSpell in _rogueSpells)
                {
                    var temp = _spellFactory.Create(rogueSpell);
                    source.SpellBook.TryAddToNextSlot(temp);
                }

                break;
            case BaseClass.Wizard:
                foreach (var skill in _wizardSkills)
                {
                    var temp = _skillFactory.Create(skill);
                    source.SkillBook.TryAddToNextSlot(temp);
                }

                foreach (var spell in _wizardSpells)
                {
                    var temp = _spellFactory.Create(spell);
                    source.SpellBook.TryAddToNextSlot(temp);
                }

                break;
            case BaseClass.Priest:
                foreach (var skill in _priestSkills)
                {
                    var temp = _skillFactory.Create(skill);
                    source.SkillBook.TryAddToNextSlot(temp);
                }

                foreach (var spell in _priestSpells)
                {
                    var temp = _spellFactory.Create(spell);
                    source.SpellBook.TryAddToNextSlot(temp);
                }

                break;
            case BaseClass.Monk:
                foreach (var skill in _monkSkills)
                {
                    var temp = _skillFactory.Create(skill);
                    source.SkillBook.TryAddToNextSlot(temp);
                }

                foreach (var spell in _monkSpells)
                {
                    var temp = _spellFactory.Create(spell);
                    source.SpellBook.TryAddToNextSlot(temp);
                }

                break;
        }

        return default;
    }
}