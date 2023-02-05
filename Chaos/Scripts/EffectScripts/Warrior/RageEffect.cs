using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.EffectScripts.Abstractions;

namespace Chaos.Scripts.EffectScripts.Warrior
{
    public class RageEffect : EffectBase
    {
        public override byte Icon => 135;
        public override string Name => "rage";

        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(25);

        public override void OnApplied()
        {
            base.OnApplied();

            var attributes = new Attributes
            {
                Dmg = 10
            };

            AislingSubject?.StatSheet.SubtractMp(50);
            Subject.StatSheet.AddBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bA violent rage builds up inside you.");
        }

        public override void OnTerminated()
        {
            var attributes = new Attributes
            {
                Dmg = 10,
            };
            Subject.StatSheet.SubtractBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your rage has subsided.");
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            if (target.Effects.Contains("rage"))
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You couldn't possibly get any more pissed.");
                return false;
            }
            else
                return true;
        }

        public override void OnDispelled() => OnTerminated();
    }
}
