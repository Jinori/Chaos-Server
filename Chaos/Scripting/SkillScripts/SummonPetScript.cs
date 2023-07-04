using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class SummonPetScript : ConfigurableSkillScriptBase
{
    private const string MONSTER_KEY = "Pet";
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
            if (string.Equals(monster.Template.TemplateKey, MONSTER_KEY, StringComparison.OrdinalIgnoreCase)
                && monster.Name.Contains(context.Source.Name))
                monster.MapInstance.RemoveObject(monster);
    }

    private void SpawnNewPet(ActivationContext context)
    {
        var newMonster = _monsterFactory.Create(MONSTER_KEY, context.SourceMap, context.SourcePoint);
        newMonster.Name = $"{context.Source.Name}'s {MONSTER_KEY}";
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

            context.Source.Trackers.Enums.TryGetValue(out SummonChosenPet pet);

            if (SpriteDictionary.SpriteIds.TryGetValue(pet, out var id))
                newMonster.Template.Sprite = (ushort)id;

            newMonster.StatSheet.SetLevel(newMonster.PetOwner.StatSheet.Level);
            newMonster.StatSheet.AddBonus(attrib);
            newMonster.StatSheet.SetHealthPct(100);
            newMonster.StatSheet.SetManaPct(100);
        }

        context.Source.MapInstance.AddObject(newMonster, new Point(context.Source.X, context.Source.Y));
    }

    public static class SpriteDictionary
    {
        public static readonly Dictionary<SummonChosenPet, int> SpriteIds = new()
        {
            { SummonChosenPet.None, 1 },
            { SummonChosenPet.Gloop, 845 },
            { SummonChosenPet.Bunny, 846 },
            { SummonChosenPet.Faerie, 848 },
            { SummonChosenPet.Dog, 849 },
            { SummonChosenPet.Ducklings, 839 },
            { SummonChosenPet.Cat, 841 }
        };
    }
}