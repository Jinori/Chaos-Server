using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Commands.Admin;

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
    private string[] _priestSpells =
    {
        "aobeagcradh", "aocradh", "aomorcradh", "aoardcradh", "aopoison", "aosith", "dinarcoli", "armachd", "beannaich", "motivate", "zap",
        "poison", "suain", "beagioc", "ioc", "morioc", "ardioc", "nuadhaich", "salvation", "beagioccomlha", "ioccomlha", "morioccomlha",
        "beothaich", "revive", "selfrevive"
    };

    private string[] _priestSkills = { "wieldholystaff" };

    private string[] _monkSpells =
    {
        "gentletouch", "goad", "howl", "innerfire", "laststand", "dodge", "thunderstance", "smokestance", "earthenstance", "miststance"
    };

    private string[] _monkSkills =
    {
        "airpunch", "firepunch", "rockpunch", "waterpunch", "ambush", "clawfist", "doublepunch", "dracotailkick", "eaglestrike", "highkick", "kick", "mantiskick",
        "poisonpunch", "punch", "roundhousekick", "triplekick", "wolffangfist"
    };

    private string[] _rogueSpells =
    {
        "needleTrap", "stilettoTrap", "boltTrap", "coiledBoltTrap", "springTrap", "maidenTrap", "detectTraps"
    };

    private string[] _rogueSkills =
    {
        "assassinStrike", "pierce", "sapNeedle", "shadowFigure", "smokescreen", "stab", "stabAndTwist", "stabTwice",
        "studyCreature", "throwsmokebomb"
    };

    private string[] _warriorSpells =
    {
        "asgallfaileas", "battlecry", "rage", "warcry", "wrath"
    };

    private string[] _warriorSkills =
    {
        "assault", "clobber", "flank", "wallop", "aobeagsuain", "beagsuain", "charge", "cleave", "crasher", "execute", "sever", "slash", "windblade"
    };

    private string[] _wizardSkills =
    {
        "wieldmagusstaff"
    };

    private string[] _wizardSpells =
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
                foreach (var Skill in _warriorSkills)
                {
                    var temp = _skillFactory.Create(Skill);
                    source.SkillBook.TryAddToNextSlot(temp);
                }
                foreach (var Spell in _warriorSpells)
                {
                    var temp = _spellFactory.Create(Spell);
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
                foreach (var Skill in _wizardSkills)
                {
                    var temp = _skillFactory.Create(Skill);
                    source.SkillBook.TryAddToNextSlot(temp);
                }
                foreach (var Spell in _wizardSpells)
                {
                    var temp = _spellFactory.Create(Spell);
                    source.SpellBook.TryAddToNextSlot(temp);
                }
                break;
            case BaseClass.Priest:
                foreach (var Skill in _priestSkills)
                {
                    var temp = _skillFactory.Create(Skill);
                    source.SkillBook.TryAddToNextSlot(temp);
                }
                foreach (var Spell in _priestSpells)
                {
                    var temp = _spellFactory.Create(Spell);
                    source.SpellBook.TryAddToNextSlot(temp);
                }
                break;
            case BaseClass.Monk:
                foreach (var Skill in _monkSkills)
                {
                    var temp = _skillFactory.Create(Skill);
                    source.SkillBook.TryAddToNextSlot(temp);
                }
                foreach (var Spell in _monkSpells)
                {
                    var temp = _spellFactory.Create(Spell);
                    source.SpellBook.TryAddToNextSlot(temp);
                }
                break;
        }
        return default;
    }
}