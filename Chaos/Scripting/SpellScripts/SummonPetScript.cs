using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class SummonPetScript : ConfigurableSpellScriptBase
{
    private readonly IMonsterFactory MonsterFactory;

    /// <inheritdoc />
    public SummonPetScript(Spell subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    public override void OnUse(SpellContext context)
    {
        var checkForOldPets = context.Source.MapInstance.GetEntities<Monster>();

        foreach (var mob in checkForOldPets)
        {
            if (mob.Template.TemplateKey == "gloop".ToLower())
            {
                if (mob.Name.Contains(context.Source.Name))
                    mob.MapInstance.RemoveObject(mob);
            }
        }

        var monster = MonsterFactory.Create("gloop", context.SourceMap, context.SourcePoint);
        monster.Name = context.Source.Name + "'s Gloop";
        context.Source.MapInstance.AddObject(monster, new Point(context.Source.X, context.Source.Y));
    }
}