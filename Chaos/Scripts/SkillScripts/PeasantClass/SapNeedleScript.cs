using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SkillScripts.PeasantClass
{
    public class SapNeedleScript : BasicSkillScriptBase
    {

        protected int? BaseDamage { get; init; }
        protected Stat? DamageStat { get; init; }
        protected decimal? DamageStatMultiplier { get; init; }

        public SapNeedleScript(Skill subject) : base(subject)
        {
        }

        public override void OnUse(SkillContext context)
        {
            ShowBodyAnimation(context);

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);
            ApplyDamage(context, affectedEntities);
        }

        protected virtual void ApplyDamage(SkillContext context, IEnumerable<Creature> targetEntities)
        {
            var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));
            foreach (var target in targetEntities)
            {
                if (group?.Count() > 1)
                {
                    foreach (var entity in group!)
                    {
                        int? manaToSap = targetEntities?.FirstOrDefault()?.StatSheet.CurrentMp;
                        if (manaToSap.HasValue)
                        {
                            int mana = manaToSap.Value / group.Count();
                            target.StatSheet.SubtractMp(manaToSap.Value);
                            entity.ApplyMana(entity, mana);
                            entity.Client?.SendAttributes(StatUpdateType.Vitality);
                            var ani = new Animation
                            {
                                TargetAnimation = 127,
                                AnimationSpeed = 100
                            };
                            entity.MapInstance.ShowAnimation(ani.GetTargetedAnimation(entity.Id));
                        }
                    }
                }
                else
                {
                    int? manaToSap = targetEntities?.FirstOrDefault()?.StatSheet.CurrentMp;
                    if (manaToSap.HasValue)
                    {
                        int mana = manaToSap.Value;
                        target.StatSheet.SubtractMp(manaToSap.Value);
                        context.Source!.ApplyMana(context.Source, mana);
                        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
                        var ani = new Animation
                        {
                            TargetAnimation = 127,
                            AnimationSpeed = 100
                        };
                        context.Source?.MapInstance.ShowAnimation(ani.GetTargetedAnimation(context.Source.Id));
                    }
                }

                var damage = CalculateDamage(context, target);
                target.ApplyDamage(context.Source!, damage);
            }
        }

        protected virtual int CalculateDamage(SkillContext context, Creature target)
        {
            var damage = BaseDamage ?? 0;

            if (DamageStat.HasValue)
            {
                var multiplier = DamageStatMultiplier ?? 1;

                damage += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(DamageStat.Value) * multiplier);
            }

            if (context.Source.Status.HasFlag(Status.ClawFist) && Subject.Template.IsAssail)
            {
                damage += Convert.ToInt32(damage * 0.3);
            }

            return DamageFormulae.Default.Calculate(context.Source, target, damage);
        }


    }
}
