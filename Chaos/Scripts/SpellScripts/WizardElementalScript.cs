﻿using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Objects;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Extensions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SpellScripts
{
    public class WizardElementalScript : BasicSpellScriptBase
    {
        protected int? BaseDamage { get; init; }
        protected Stat? DamageStat { get; init; }
        protected decimal? DamageStatMultiplier { get; init; }
        protected Element? OffensiveElement { get; init; }
        protected int? ManaSpent { get; init; }

        /// <inheritdoc />
        public WizardElementalScript(Spell subject)
            : base(subject) { }

        protected virtual void ApplyDamage(SpellContext context, IEnumerable<Creature> targetEntities)
        {
            foreach (var target in targetEntities)
            {
                var damage = CalculateDamage(context, target);
                target.ApplyDamage(context.Source, damage);
            }
        }

        protected virtual int CalculateDamage(SpellContext context, Creature target)
        {
            var damage = BaseDamage ?? 0;
            var elementaldamage = 0;

            if (DamageStat.HasValue)
            {
                var multiplier = DamageStatMultiplier ?? 1;

                damage += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(DamageStat.Value) * multiplier);
            }

            if (OffensiveElement.HasValue)
            {
                elementaldamage = Chaos.Extensions.ElementalDamageExtensions.ConvertElementalAttackDamage(OffensiveElement.Value, target.StatSheet.DefenseElement, damage);
            }

            int sum = damage + elementaldamage;
            return DamageFormulae.Default.Calculate(context.Source, target, sum);
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
            if (ManaSpent.HasValue)
            {
                //Require mana
                if (context.Source.StatSheet.CurrentMp < ManaSpent.Value)
                {
                    context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                    return;
                }
                //Subtract mana and update user
                context.Source.StatSheet.SubtractMp(ManaSpent.Value);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
            }

            ShowBodyAnimation(context);

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);
            ApplyDamage(context, affectedEntities);
        }
    }
}