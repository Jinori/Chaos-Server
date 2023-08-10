using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class SummonPetScript : ConfigurableSkillScriptBase
{
    private readonly IMonsterFactory _monsterFactory;
    private readonly ISkillFactory _skillFactory;
    private readonly ISpellFactory _spellFactory;

    /// <inheritdoc />
    public SummonPetScript(Skill subject, IMonsterFactory monsterFactory, ISkillFactory skillFactory, ISpellFactory spellFactory)
        : base(subject)
    {
        _spellFactory = spellFactory;
        _skillFactory = skillFactory;
        _monsterFactory = monsterFactory;   
    }

    private Element[] Elements { get; } =
    {
        Element.Fire,
        Element.Water,
        Element.Wind,
        Element.Earth
    };
    
    public override void OnUse(ActivationContext context)
    {
        RemoveExistingPets(context);
        SpawnNewPet(context);
    }

    private void RemoveExistingPets(ActivationContext context)
    {
        var monsters = context.Source.MapInstance.GetEntities<Monster>();

        foreach (var monster in monsters)
            if (monster.Name.Contains(context.Source.Name))
                monster.MapInstance.RemoveObject(monster);
    }

    private void SpawnNewPet(ActivationContext context)
    {
        context.Source.Trackers.Enums.TryGetValue(out SummonChosenPet petKey);

        if (petKey is SummonChosenPet.None)
        {
            context.SourceAisling?.SendActiveMessage("You have not selected a pet type with Areini!");

            return;
        }

        var newMonster = _monsterFactory.Create(petKey + "pet", context.SourceMap, context.SourcePoint);
        newMonster.Name = $"{context.Source.Name}'s {petKey}";
        newMonster.PetOwner = context.SourceAisling;

        if (newMonster.PetOwner != null)
        {
            var attrib = new Attributes
            {
                Ac = newMonster.PetOwner.StatSheet.Ac,
                Con = (newMonster.PetOwner.StatSheet.EffectiveCon + newMonster.PetOwner.StatSheet.Level) / 2,
                Dex = (newMonster.PetOwner.StatSheet.EffectiveDex + newMonster.PetOwner.StatSheet.Level) / 2,
                Int = (newMonster.PetOwner.StatSheet.EffectiveInt + newMonster.PetOwner.StatSheet.Level) / 2,
                Str = (newMonster.PetOwner.StatSheet.EffectiveStr + newMonster.PetOwner.StatSheet.Level) / 2,
                Wis = (newMonster.PetOwner.StatSheet.EffectiveWis + newMonster.PetOwner.StatSheet.Level) / 2,
                AtkSpeedPct = newMonster.PetOwner.StatSheet.Level,
                MaximumHp = newMonster.PetOwner.StatSheet.Level * 1000 / 9,
                MaximumMp = newMonster.PetOwner.StatSheet.Level * 500 / 9
            };
            
            newMonster.StatSheet.SetOffenseElement(Elements.PickRandom());
            newMonster.StatSheet.SetDefenseElement(Elements.PickRandom());
            newMonster.StatSheet.SetLevel(newMonster.PetOwner.StatSheet.Level);
            newMonster.StatSheet.AddBonus(attrib);
            newMonster.StatSheet.SetHealthPct(100);
            newMonster.StatSheet.SetManaPct(100);
            
            var has10Skill = newMonster.PetOwner.Trackers.Enums.TryGetValue(out Level10PetSkills level10Skill);
            var has25Skill = newMonster.PetOwner.Trackers.Enums.TryGetValue(out Level25PetSkills level25Skill);
            
            if (has10Skill)
            {
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
            }
            if (has25Skill)
            {
                switch (level25Skill)
                {
                    
                    case Level25PetSkills.TailSweep:
                    {
                        var skillToAdd = _skillFactory.Create("dracotailkick");
                        newMonster.Skills.Add(skillToAdd);
                        break;
                    }
                    case Level25PetSkills.Enrage:
                    {
                        var skillToAdd = _skillFactory.Create("dracotailkick");
                        newMonster.Skills.Add(skillToAdd);
                        break;
                    }
                    case Level25PetSkills.Swift:
                    {
                        var skillToAdd = _skillFactory.Create("blitz");
                        newMonster.Skills.Add(skillToAdd);
                        break;
                    }
                }
            }
        }

        context.Source.MapInstance.AddObject(newMonster, new Point(context.Source.X, context.Source.Y));
    }
}