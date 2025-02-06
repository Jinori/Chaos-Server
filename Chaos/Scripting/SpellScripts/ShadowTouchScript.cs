using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class ShadowTouchScript : SpellScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    /// <inheritdoc />
    public ShadowTouchScript(Spell subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        context.SourceAisling?.StatSheet.SubtractMp(1200);
        var newMonster = MonsterFactory.Create("shadowPet", context.SourceMap, context.Source);
        newMonster.PetOwner = context.SourceAisling;
        
        newMonster.Direction = context.Source.Direction;
        context.SourceMap.AddEntity(newMonster, context.Source);
    }
}