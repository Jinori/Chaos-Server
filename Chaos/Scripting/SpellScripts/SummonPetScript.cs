using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class SummonPetScript : ConfigurableSpellScriptBase
{
    private const string MONSTER_KEY = "Gloop";
    private readonly IMonsterFactory _monsterFactory;

    public SummonPetScript(Spell subject, IMonsterFactory monsterFactory)
        : base(subject) => _monsterFactory = monsterFactory;

    public override void OnUse(SpellContext context)
    {
        RemoveExistingPets(context);
        SpawnNewPet(context);
    }

    private void RemoveExistingPets(SpellContext context)
    {
        var monsters = context.Source.MapInstance.GetEntities<Monster>();

        foreach (var monster in monsters)
        {
            if (string.Equals(monster.Template.TemplateKey, MONSTER_KEY, StringComparison.OrdinalIgnoreCase) &&
                monster.Name.Contains(context.Source.Name))
            {
                monster.MapInstance.RemoveObject(monster);
            }
        }
    }

    private void SpawnNewPet(SpellContext context)
    {
        var newMonster = _monsterFactory.Create(MONSTER_KEY, context.SourceMap, context.SourcePoint);
        newMonster.Name = $"{context.Source.Name}'s {MONSTER_KEY}";
        context.Source.MapInstance.AddObject(newMonster, new Point(context.Source.X, context.Source.Y));
    }
}