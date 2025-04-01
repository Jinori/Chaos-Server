using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
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
        : base(subject)
        => MonsterFactory = monsterFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if ((context.SourceAisling != null) && !context.SourceAisling.StatSheet.TrySubtractMp(1200))
        {
            context.SourceAisling.SendMessage("You don't have enough mana.");

            return;
        }

        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
        var newMonster = MonsterFactory.Create("shadowPet", context.SourceMap, context.Source);
        newMonster.PetOwner = context.SourceAisling;

        newMonster.Direction = context.Source.Direction;

        var shadowTouchScript = newMonster.Script.As<MonsterScripts.ShadowTouchScript>();

        if (shadowTouchScript is null)
            return;

        shadowTouchScript.SourceScript = this;
        
        context.SourceMap.AddEntity(newMonster, context.Source);
    }
}