using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.EffectScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.EffectScripts
{
    public class AssassinStrikeEffect : EffectBase
    {
        public override byte Icon => 109;
        public override string Name => "AssassinStrike";

        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(12);

        public override void OnApplied()
        {

            base.OnApplied();

            //Reduce Armor of Subject by Equipment Slot Armor
            if (AislingSubject?.Equipment[EquipmentSlot.Armor]?.Count > 0)
            {
                if (AislingSubject?.Equipment[EquipmentSlot.Armor]?.Template?.Modifiers?.Ac is not null)
                {
                    int s = (int)(AislingSubject?.Equipment[EquipmentSlot.Armor]?.Template!.Modifiers?.Ac);
                    var attributes = new Attributes
                    {
                        Ac = -s,
                    };

                    AislingSubject?.StatSheet.SubtractBonus(attributes);
                }
            }
            //If creature or no armor, lets pass a base value?
            else
            {
                var attributes = new Attributes
                {
                    Ac = -80,
                };
                Subject.StatSheet.SubtractBonus(attributes);
            }
            
          
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Ack! Armor class has been lowered for a short period.");
        }

        public override void OnTerminated()
        {
            //Reduce Armor of Subject by Equipment Slot Armor
            if (AislingSubject?.Equipment[EquipmentSlot.Armor]?.Count > 0)
            {
                if (AislingSubject?.Equipment[EquipmentSlot.Armor]?.Template?.Modifiers?.Ac is not null)
                {
                    int s = (int)(AislingSubject?.Equipment[EquipmentSlot.Armor]?.Template!.Modifiers?.Ac);
                    var attributes = new Attributes
                    {
                        Ac = -s,
                    };

                    AislingSubject?.StatSheet.AddBonus(attributes);
                }
            }
            //If creature or no armor, lets pass a base value?
            else
            {
                var attributes = new Attributes
                {
                    Ac = -80,
                };
                Subject.StatSheet.AddBonus(attributes);
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
            else
                return true;
        }

        public override void OnDispelled()
        {
            OnTerminated();
        }
    }
}
