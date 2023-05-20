using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class AssassinStrikeEffect : EffectBase
{
    protected int ArmorClassSaved;
    public override byte Icon => 109;
    public override string Name => "AssassinStrike";
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(12);

    public override void OnApplied()
    {
        base.OnApplied();

        //Reduce Armor of Subject by Equipment Slot Armor
        if (AislingSubject is not null && AislingSubject.Equipment.TryGetObject((byte)EquipmentSlot.Armor, out var obj))
        {
            if (obj.Template.Modifiers?.Ac != null)
            {
                var armorClass = obj.Template.Modifiers.Ac;

                var attributes = new Attributes
                {
                    Ac = armorClass
                };

                ArmorClassSaved = armorClass;
                AislingSubject?.StatSheet.AddBonus(attributes);
            }
        }
        //If creature or no armor, lets pass a base value?
        else
        {
            var attributes = new Attributes
            {
                Ac = 80
            };

            Subject.StatSheet.AddBonus(attributes);
            ArmorClassSaved = 80;
        }

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Ack! Armor class has been lowered for a short period.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        if (AislingSubject is not null)
        {
            var attributes = new Attributes
            {
                Ac = ArmorClassSaved
            };

            AislingSubject?.StatSheet.SubtractBonus(attributes);
        }
        //If creature or no armor, lets pass a base value?
        else
        {
            var attributes = new Attributes
            {
                Ac = ArmorClassSaved
            };

            Subject.StatSheet.SubtractBonus(attributes);
        }

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor class has returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("AssassinStrike"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Their armor class has already been lowered.");

            return false;
        }

        return true;
    }
}