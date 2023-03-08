using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.Components;

public class DurabilityComponent
{
    public void TryApplyDurability(ActivationContext context, IReadOnlyCollection<Creature> targetEntities, IScript source)
    {
        if (source is not SubjectiveScriptBase<Skill> skillScript)
            return;

        if (!skillScript.Subject.Template.IsAssail)
            return;

        if (context.Source is Aisling)
        {
            //Aisling Attacker
            var hasWeapon = context.SourceAisling!.Equipment.TryGetObject(1, out var weapon);
            if (hasWeapon && (weapon?.CurrentDurability >= 1))
                weapon.CurrentDurability--;
            var hasNecklace = context.SourceAisling!.Equipment.TryGetObject(6, out var necklace);
            if (hasNecklace && (necklace?.CurrentDurability >= 1))
                necklace.CurrentDurability--;   
        }

        //Works but lets clean this up?
        foreach (var creature in targetEntities)
        {
            if (creature is not Aisling aisling) 
                continue;
            
            var equipment = aisling.Equipment.Where(x => x.CurrentDurability.HasValue ).ToList();
            //Aisling Defender, Let's hurt everything but weapon, accessories, overcoats
            foreach (var item in equipment!.Where(item => item.Slot is > 1 and < 14))
            {
                item.CurrentDurability--;
            }
        }
    }
}