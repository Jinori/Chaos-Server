using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts.Healing
{
    public class VialSmashScript : BasicSpellScriptBase
    {
        private readonly IEffectFactory EffectFactory;
        //Healing
        protected int? GiveHeal { get; init; }
        protected Stat? HealStat { get; init; }
        protected decimal? HealStatMultiplier { get; init; }
        //Mana Regen
        protected int? GiveMana { get; init; }
        protected Stat? ManaStat { get; init; }
        protected decimal? ManaStatMultiplier { get; init; }


        public VialSmashScript(Spell subject, IEffectFactory effectFactory) : base(subject)
        {
            EffectFactory = effectFactory;
        }

        protected virtual void ApplyHealing(SpellContext context, IEnumerable<Creature> targetEntities)
        {
            foreach (var target in targetEntities)
            {
                var heals = CalculateHealing(context, target);
                target.ApplyHealing(target, heals);
            }
        }

        protected virtual int CalculateHealing(SpellContext context, Creature target)
        {
            var heals = GiveHeal ?? 0;

            if (HealStat.HasValue)
            {
                if (context.Source == target)
                {
                    var multiplier = HealStatMultiplier ?? 1;

                    heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(HealStat.Value) * multiplier);
                    //Divide by two for healing yourself
                    heals = Convert.ToInt32(heals / 2);
                }
                else
                {
                    var multiplier = HealStatMultiplier ?? 1;

                    heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(HealStat.Value) * multiplier);
                }
            }

            return heals;
        }

        protected virtual void ApplyMana(SpellContext context, IEnumerable<Creature> targetEntities)
        {
            foreach (var target in targetEntities)
            {
                var heals = CalculateMana(context, target);
                target.ApplyHealing(target, heals);
            }
        }

        protected virtual int CalculateMana(SpellContext context, Creature target)
        {
            var heals = GiveMana ?? 0;

            if (ManaStat.HasValue)
            {
                if (context.Source == target)
                {
                    var multiplier = ManaStatMultiplier ?? 1;

                    heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(ManaStat.Value) * multiplier);
                    //Divide by two for healing yourself
                    heals = Convert.ToInt32(heals / 2);
                }
                else
                {
                    var multiplier = ManaStatMultiplier ?? 1;

                    heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(ManaStat.Value) * multiplier);
                }
            }

            return heals;
        }

        /// <inheritdoc />
        protected override IEnumerable<T> GetAffectedEntities<T>(SpellContext context, IEnumerable<IPoint> affectedPoints)
        {
            var entities = base.GetAffectedEntities<T>(context, affectedPoints);

            return entities;
        }

        /// <inheritdoc />
        public override void OnUse(SpellContext context)
        {
            if (context.SourceAisling?.Inventory.HasCount("Crafted Health Pot", 1) is false && GiveHeal.HasValue)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have the required health potion.");
                return;
            }

            if (context.SourceAisling?.Inventory.HasCount("Crafted Mana Pot", 1) is false && GiveMana.HasValue)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have the required mana potion.");
                return;
            }

            ShowBodyAnimation(context);


            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);

            if (GiveHeal.HasValue)
            {
                ApplyHealing(context, affectedEntities);
                foreach (var entity in affectedEntities)
                {
                    var effect = EffectFactory.Create("smashVialHeal");
                    entity.Effects.Apply(context.Source, effect);
                }
                context.SourceAisling?.Inventory.RemoveQuantity("Crafted Health Pot", 1, out _);
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You smash a Crafted Health Pot on the ground.");
            }

            if (GiveMana.HasValue)
            {
                ApplyMana(context, affectedEntities);
                foreach (var entity in affectedEntities)
                {
                    var effect = EffectFactory.Create("smashVialMana");
                    entity.Effects.Apply(context.Source, effect);
                }
                context.SourceAisling?.Inventory.RemoveQuantity("Crafted Mana Pot", 1, out _);
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You smash a Crafted Mana Pot on the ground.");
            }
        }

    }
}
