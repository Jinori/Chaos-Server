using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class PetCollarScript : DialogScriptBase
{
    /// <inheritdoc />
    public PetCollarScript(Dialog subject, IMonsterFactory monsterFactory, ISkillFactory skillFactory,
        ISpellFactory spellFactory
    )
        : base(subject)
    {
        _monsterFactory = monsterFactory;
        _skillFactory = skillFactory;
        _spellFactory = spellFactory;
    }

    
    private readonly IMonsterFactory _monsterFactory;
    private readonly ISkillFactory _skillFactory;
    private readonly ISpellFactory _spellFactory;

    private Element[] Elements { get; } =
    {
        Element.Fire,
        Element.Water,
        Element.Wind,
        Element.Earth
    };
    
    private void RemoveExistingPets(Aisling source)
    {
        var monsters = source.MapInstance.GetEntities<Monster>().Where(x => x.Script.Is<PetScript>());

        foreach (var monster in monsters)
            if (monster.Name.Contains(source.Name))
                monster.MapInstance.RemoveEntity(monster);
    }
    
    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        if (string.Equals(Subject.Template.TemplateKey, "petcollar_home", StringComparison.OrdinalIgnoreCase))
        {
            var monsters = source.MapInstance.GetEntities<Monster>().Where(x => x.Script.Is<PetScript>());

            foreach (var monster in monsters)
                if (monster.Name.Contains(source.Name))
                    monster.MapInstance.RemoveEntity(monster);
        }

        if (string.Equals(Subject.Template.TemplateKey, "petcollar_summonpet", StringComparison.OrdinalIgnoreCase))
        {
            if (source.Trackers.TimedEvents.HasActiveEvent("PetDeath", out var timedEvent))
            {
                var timeLeft = timedEvent.Remaining.ToReadableString();
                source.SendActiveMessage($"Your pet recently died. Please wait {timeLeft}."); 
                return;
            }

            
            RemoveExistingPets(source);
            source.Trackers.Enums.TryGetValue(out SummonChosenPet petKey);

            if (petKey is SummonChosenPet.None)
            {
                source.SendActiveMessage("You have not selected a pet type with Areini!");
                return;
            }

            var newMonster = _monsterFactory.Create(petKey + "pet", source.MapInstance, new Point(source.X, source.Y));
            newMonster.Name = $"{source.Name}'s {petKey}";
            newMonster.PetOwner = source;

            if (newMonster.PetOwner != null)
            {
                var attrib = new Attributes
                {
                    Ac = 100 - newMonster.PetOwner.StatSheet.Level,
                    Con = newMonster.PetOwner.StatSheet.EffectiveCon + newMonster.PetOwner.StatSheet.Level,
                    Dex = newMonster.PetOwner.StatSheet.EffectiveDex + newMonster.PetOwner.StatSheet.Level,
                    Int = newMonster.PetOwner.StatSheet.EffectiveInt + newMonster.PetOwner.StatSheet.Level,
                    Str = newMonster.PetOwner.StatSheet.EffectiveStr + newMonster.PetOwner.StatSheet.Level,
                    Wis = newMonster.PetOwner.StatSheet.EffectiveWis + newMonster.PetOwner.StatSheet.Level,
                    AtkSpeedPct = newMonster.PetOwner.StatSheet.Level,
                    MaximumHp = newMonster.PetOwner.StatSheet.Level * 1000 / 7 + 1000,
                    MaximumMp = newMonster.PetOwner.StatSheet.Level * 500 / 7 + 1000
                };

                newMonster.StatSheet.SetOffenseElement(Elements.PickRandom());
                newMonster.StatSheet.SetDefenseElement(Elements.PickRandom());
                newMonster.StatSheet.SetLevel(newMonster.PetOwner.StatSheet.Level);
                newMonster.StatSheet.AddBonus(attrib);
                newMonster.StatSheet.SetHealthPct(100);
                newMonster.StatSheet.SetManaPct(100);

                var has10Skill = newMonster.PetOwner.Trackers.Enums.TryGetValue(out Level10PetSkills level10Skill);
                var has25Skill = newMonster.PetOwner.Trackers.Enums.TryGetValue(out Level25PetSkills level25Skill);
                var has40Skill = newMonster.PetOwner.Trackers.Enums.TryGetValue(out Level40PetSkills level40Skill);
                var has55Skill = newMonster.PetOwner.Trackers.Enums.TryGetValue(out Level55PetSkills level55Skill);

                if (has10Skill)
                    switch (level10Skill)
                    {
                        case Level10PetSkills.RabidBite:
                        {
                            var skillToAdd = _skillFactory.Create("poisonpunch");
                            newMonster.Skills.Add(skillToAdd);

                            break;
                        }
                        case Level10PetSkills.Growl:
                        {
                            var spellToAdd = _spellFactory.Create("howl");
                            newMonster.Spells.Add(spellToAdd);

                            break;
                        }
                        case Level10PetSkills.QuickAttack:
                        {
                            var skillToAdd = _skillFactory.Create("stab");
                            newMonster.Skills.Add(skillToAdd);

                            break;
                        }
                    }

                if (has25Skill)
                    switch (level25Skill)
                    {
                        case Level25PetSkills.PawStrike:
                        {
                            var skillToAdd = _skillFactory.Create("mantiskick");
                            newMonster.Skills.Add(skillToAdd);

                            break;
                        }
                        case Level25PetSkills.Enrage:
                        {
                            var skillToAdd = _skillFactory.Create("clawfist");
                            newMonster.Skills.Add(skillToAdd);

                            break;
                        }
                        case Level25PetSkills.WindStrike:
                        {
                            var skillToAdd = _skillFactory.Create("windblade");
                            newMonster.Skills.Add(skillToAdd);

                            break;
                        }
                    }

                if (has40Skill)
                    switch (level40Skill)
                    {
                        case Level40PetSkills.Blitz:
                        {
                            var skillToAdd = _skillFactory.Create("blitz");
                            newMonster.Skills.Add(skillToAdd);

                            break;
                        }
                        case Level40PetSkills.Slobber:
                        {
                            var skillToAdd = _skillFactory.Create("clobber");
                            newMonster.Skills.Add(skillToAdd);

                            break;
                        }
                        case Level40PetSkills.DoubleLick:
                        {
                            var skillToAdd = _skillFactory.Create("doublepunch");
                            newMonster.Skills.Add(skillToAdd);

                            break;
                        }
                    }

                if (has55Skill)
                    switch (level55Skill)
                    {
                        case Level55PetSkills.Frenzy:
                        {
                            var skillToAdd = _spellFactory.Create("focus");
                            newMonster.Spells.Add(skillToAdd);

                            break;
                        }
                        case Level55PetSkills.Spit:
                        {
                            var skillToAdd = _spellFactory.Create("arcanebolt");
                            newMonster.Spells.Add(skillToAdd);

                            break;
                        }
                        case Level55PetSkills.Evade:
                        {
                            var skillToAdd = _spellFactory.Create("dodge");
                            newMonster.Spells.Add(skillToAdd);

                            break;
                        }
                    }
            }

            source.MapInstance.AddEntity(newMonster, new Point(source.X, source.Y));
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {

        if (string.Equals(Subject.Template.TemplateKey, "petcollar_changeappearance", StringComparison.OrdinalIgnoreCase))
            if (optionIndex.HasValue)
            {
                var optionText = Subject.GetOptionText((int)optionIndex - 1);

                if (Enum.TryParse<SummonChosenPet>(optionText, true, out var chosenPet))
                    switch (chosenPet)
                    {
                        case SummonChosenPet.None:
                            break;
                        case SummonChosenPet.Gloop:
                        case SummonChosenPet.Bunny:
                        case SummonChosenPet.Faerie:
                        case SummonChosenPet.Dog:
                        case SummonChosenPet.Ducklings:
                        case SummonChosenPet.Cat:
                        case SummonChosenPet.Smoldy:
                        case SummonChosenPet.Penguin:
                            source.Trackers.Enums.Set(chosenPet);
                            source.SendActiveMessage($"You've chosen {chosenPet}!");

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                else
                    source.SendActiveMessage("Something went wrong!");

                return;
            }

        if (string.Equals(Subject.Template.TemplateKey, "petcollar_combatstance", StringComparison.OrdinalIgnoreCase))
            if (optionIndex.HasValue)
            {
                var optionText = Subject.GetOptionText((int)optionIndex - 1);

                if (Enum.TryParse<PetMode>(optionText, true, out var chosenMode))
                    switch (chosenMode)
                    {
                        case PetMode.Defensive:
                            source.Trackers.Enums.Set(chosenMode);
                            source.SendActiveMessage($"You've chosen {chosenMode} pet behaviors.");
                            Subject.Reply(source, "Your pet will watch your back for anything aggressive to you.");

                            break;
                        case PetMode.Offensive:
                            source.Trackers.Enums.Set(chosenMode);
                            source.SendActiveMessage($"You've chosen {chosenMode} pet behavior.");
                            Subject.Reply(source, "Your pet will attack anything that comes within range.");

                            break;
                        case PetMode.Assist:
                            source.Trackers.Enums.Set(chosenMode);
                            source.SendActiveMessage($"You've chosen {chosenMode} pet behavior.");
                            Subject.Reply(source, "Your pet will attack the most aggressive monster in group range.");
                            break;
                        
                        case PetMode.Passive:
                            source.Trackers.Enums.Set(chosenMode);
                            source.SendActiveMessage($"You've chosen {chosenMode} pet behavior.");
                            Subject.Reply(source, "Your pet will not move to attack anything.");
                            break;
                    }
            }

        if (string.Equals(Subject.Template.TemplateKey, "petcollar_petfollow", StringComparison.OrdinalIgnoreCase))
            if (optionIndex.HasValue)
            {
                var optionText = Subject.GetOptionText((int)optionIndex - 1);

                if (!string.IsNullOrEmpty(optionText))
                {
                    var chosenMode = ParseFollowMode(optionText);

                    if (chosenMode.HasValue)
                        switch (chosenMode.Value)
                        {
                            case PetFollowMode.Wander:
                                source.Trackers.Enums.Set(chosenMode.Value);
                                source.SendActiveMessage($"You tell your pet it can {chosenMode.Value}.");
                                Subject.Reply(source, "Your pet will now wander around.");

                                break;
                            case PetFollowMode.AtFeet:
                                source.Trackers.Enums.Set(chosenMode.Value);
                                source.SendActiveMessage("You tell your pet to stay close.");
                                Subject.Reply(source, "Your pet will now stay near your side.");

                                break;
                            case PetFollowMode.FollowAtDistance:
                                source.Trackers.Enums.Set(chosenMode.Value);
                                source.SendActiveMessage("You tell your pet to follow behind you.");
                                Subject.Reply(source, "Your pet will now follow you at a short distance.");

                                break;
                            case PetFollowMode.DontMove:
                                source.Trackers.Enums.Set(chosenMode.Value);
                                source.SendActiveMessage("You tell your pet not to move.");
                                Subject.Reply(source, "Your pet will now stay unless provoked.");

                                break;
                        }
                }
            }
    }

    private PetFollowMode? ParseFollowMode(string optionText) =>
        optionText.Replace(" ", "") switch
        {
            "AtFeet"       => PetFollowMode.AtFeet,
            "FollowBehind" => PetFollowMode.FollowAtDistance,
            "Stay"         => PetFollowMode.DontMove,
            "Wander"       => PetFollowMode.Wander,
            _              => null
        };
}