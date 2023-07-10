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

    /// <inheritdoc />
    public SummonPetScript(Skill subject, IMonsterFactory monsterFactory)
        : base(subject) => _monsterFactory = monsterFactory;

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
                Con = newMonster.PetOwner.StatSheet.EffectiveCon / 2,
                Dex = newMonster.PetOwner.StatSheet.EffectiveDex / 2,
                Int = newMonster.PetOwner.StatSheet.EffectiveInt / 2,
                Str = newMonster.PetOwner.StatSheet.EffectiveStr / 2,
                Wis = newMonster.PetOwner.StatSheet.EffectiveWis / 2,
                AtkSpeedPct = newMonster.PetOwner.StatSheet.Level,
                MaximumHp = (int)newMonster.PetOwner.StatSheet.EffectiveMaximumHp / 2,
                MaximumMp = (int)newMonster.PetOwner.StatSheet.EffectiveMaximumMp / 2
            };

            newMonster.StatSheet.SetLevel(newMonster.PetOwner.StatSheet.Level);
            newMonster.StatSheet.AddBonus(attrib);
            newMonster.StatSheet.SetHealthPct(100);
            newMonster.StatSheet.SetManaPct(100);
        }

        context.Source.MapInstance.AddObject(newMonster, new Point(context.Source.X, context.Source.Y));
    }
}