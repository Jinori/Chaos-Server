using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class SummonPetScript : ConfigurableSkillScriptBase
{
    private readonly IMonsterFactory _monsterFactory;
    private readonly ISkillFactory _skillFactory;

    /// <inheritdoc />
    public SummonPetScript(Skill subject, IMonsterFactory monsterFactory, ISkillFactory skillFactory)
        : base(subject)
    {
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
            
            var hasSkill = newMonster.PetOwner.Trackers.Enums.TryGetValue(out Level10PetSkills skill);

            if (hasSkill)
            {
                switch (skill)
                {
                    case Level10PetSkills.RabidBite:
                    {
                        var skillToAdd = _skillFactory.Create("poisonpunch");
                        newMonster.Skills.Add(skillToAdd);
                        break;
                    }
                    case Level10PetSkills.Growl:
                    {
                        var skillToAdd = _skillFactory.Create("howl");
                        newMonster.Skills.Add(skillToAdd);
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
        }

        context.Source.MapInstance.AddObject(newMonster, new Point(context.Source.X, context.Source.Y));
    }
}