using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts
{
    public class ApplyCradhScript : BasicSpellScriptBase
    {
        private readonly IEffectFactory EffectFactory;
        protected string EffectKey { get; init; } = null!;
        protected int? manaSpent { get; init; }


        public ApplyCradhScript(Spell subject, IEffectFactory effectFactory) : base(subject)
        {
            EffectFactory = effectFactory;
        }

        /// <inheritdoc />
        public override void OnUse(SpellContext context)
        {
            if (manaSpent.HasValue)
            {
                //Require mana
                if (context.Source.StatSheet.CurrentMp < manaSpent.Value)
                {
                    context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                    return;
                }
                //Subtract mana and update user
                context.Source.StatSheet.SubtractMp(manaSpent.Value);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
            }

            if (context.Target?.Status.HasFlag(Status.PreventAffliction) is true)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your magic temporarily fizzles. (Prevent Affliction)");
                return;
            }


            ShowBodyAnimation(context);

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);

            foreach (var entity in affectedEntities)
            {
                var effect = EffectFactory.Create(EffectKey);
                entity.Effects.Apply(context.Source, effect);
            }
        }

    }
}
