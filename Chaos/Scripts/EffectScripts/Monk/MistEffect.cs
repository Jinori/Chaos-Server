﻿using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.EffectScripts.Priest
{
    public class MistEffect : EffectBase
    {
        public override byte Icon => 55;
        public override string Name => "mist";

        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(10);

        public override void OnApplied()
        {
            base.OnApplied();

            var attributesToSubtract = new Attributes
            {
                Ac = 20,
            };
            var attributesToAdd = new Attributes
            {
                MagicResistance = 20,
            };

            Subject?.StatSheet.SubtractMp(100);
            Subject?.StatSheet.SubtractBonus(attributesToSubtract);
            Subject?.StatSheet.AddBonus(attributesToAdd);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor increased while MR decreased.");
        }

        public override void OnTerminated()
        {
            var attributesToSubtract = new Attributes
            {
                MagicResistance = 20,
            };
            var attributesToAdd = new Attributes
            {
                Ac = 20,
            };

            Subject?.StatSheet.SubtractMp(100);
            Subject?.StatSheet.SubtractBonus(attributesToSubtract);
            Subject?.StatSheet.AddBonus(attributesToAdd);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor and MR has returned to normal.");
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            if (target.Effects.Contains("mist"))
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Mist has already been applied.");
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