using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.Components;

public class DurabilityComponent
{
    public virtual void TryApplyDurability(ActivationContext context, IReadOnlyCollection<Creature> targetEntities, IScript source)
    {
        if (source is not SubjectiveScriptBase<Skill> skillScript || !skillScript.Subject.Template.IsAssail)
            return;

        if (context.Source is Aisling)
        {
            var hasWeapon = context.SourceAisling!.Equipment.TryGetObject(1, out var weapon);
            if (hasWeapon && weapon?.CurrentDurability >= 1)
                weapon.CurrentDurability--;
            var hasNecklace = context.SourceAisling!.Equipment.TryGetObject(6, out var necklace);
            if (hasNecklace && necklace?.CurrentDurability >= 1)
                necklace.CurrentDurability--;
        }

        foreach (var aislingTarget in targetEntities)
        {
            if (aislingTarget is not Aisling aisling)
                continue;
            
            var equipment = aisling.Equipment.Where(x => x.CurrentDurability.HasValue).ToList();
            foreach (var item in equipment.Where(item => item.Slot is > 1 and < 14)) item.CurrentDurability--;

            var itemsToBreak = aisling.Equipment.Where(x => !x.Template.AccountBound && x.CurrentDurability <= 0);
            foreach (var item in itemsToBreak)
            {
                aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1,
                    $"{item.DisplayName} has reached zero durability and has broke.");
                aisling.Equipment.TryGetRemove(item.Slot, out _);
            }

            var itemsToBank = aisling.Equipment.Where(x => x.Template.AccountBound && x.CurrentDurability <= 0);
            foreach (var item in itemsToBank)
            {
                aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1,
                    $"{item.DisplayName} has reached zero durability and has been sent to your bank.");
                aisling.Equipment.TryGetRemove(item.Slot, out var items);
                if (items != null)
                    aisling.Bank.Deposit(items);
            }
        }
    }
}