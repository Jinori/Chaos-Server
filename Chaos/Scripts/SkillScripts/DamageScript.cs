using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class DamageScript : BasicSkillScriptBase
{
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }
    private readonly IEffectFactory EffectFactory;

    /// <inheritdoc />
    public DamageScript(Skill subject, IEffectFactory effectFactory)
        : base(subject) { EffectFactory = effectFactory; }

    protected virtual void ApplyDamage(SkillContext context, IEnumerable<Creature> targetEntities)
    {
        foreach (var target in targetEntities)
        {
            if (target.Status.HasFlag(Status.AsgallFaileas))
            {
                //Let's reflect damage back at a 70% chance and take no damage ourselves.
                if (Randomizer.RollChance(70))
                {
                    var reflectDamage = CalculateDamage(context, target);
                    context.Source.ApplyDamage(context.Source, reflectDamage);
                    return;
                }
            }
            if (target is Aisling aisling && !aisling.Effects.Contains("chiBlocker") && aisling.Equipment[EquipmentSlot.Boots]!.Template.TemplateKey.EqualsI("chiAnklet"))
            {
                var chiBlock = (ChiAnkletFlags)aisling.Flags.GetFlag<ChiAnkletFlags>();
                chiBlock &= ChiAnkletFlags.BlockSkill1 | ChiAnkletFlags.BlockSkill2 | ChiAnkletFlags.BlockSkill3 | ChiAnkletFlags.BlockSkill4 | ChiAnkletFlags.BlockSkill5;
                switch (chiBlock)
                {
                    case ChiAnkletFlags.BlockSkill1:
                        if (Randomizer.RollChance(1))
                        {
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi blocked {Subject.Template.Name}.");
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var ani = new Animation
                            {
                                TargetAnimation = 339,
                                AnimationSpeed = 100
                            };
                            aisling.MapInstance.ShowAnimation(Animation.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                    case ChiAnkletFlags.BlockSkill2:
                        if (Randomizer.RollChance(2))
                        {
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi blocked {Subject.Template.Name}.");
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var ani = new Animation
                            {
                                TargetAnimation = 339,
                                AnimationSpeed = 100
                            };
                            aisling.MapInstance.ShowAnimation(Animation.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                    case ChiAnkletFlags.BlockSkill3:
                        if (Randomizer.RollChance(3))
                        {
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi blocked {Subject.Template.Name}.");
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var ani = new Animation
                            {
                                TargetAnimation = 339,
                                AnimationSpeed = 100
                            };
                            aisling.MapInstance.ShowAnimation(Animation.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                    case ChiAnkletFlags.BlockSkill4:
                        if (Randomizer.RollChance(4))
                        {
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi blocked {Subject.Template.Name}.");
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var ani = new Animation
                            {
                                TargetAnimation = 339,
                                AnimationSpeed = 100
                            };
                            aisling.MapInstance.ShowAnimation(Animation.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                    case ChiAnkletFlags.BlockSkill5:
                        if (Randomizer.RollChance(5))
                        {
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi blocked {Subject.Template.Name}.");
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var ani = new Animation
                            {
                                TargetAnimation = 339,
                                AnimationSpeed = 100
                            };
                            aisling.MapInstance.ShowAnimation(Animation.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                }
            }
            var damage = CalculateDamage(context, target);
            target.ApplyDamage(context.Source, damage);
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

    /// <inheritdoc />
    public override void OnUse(SkillContext context)
    {
        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

        ShowAnimation(context, affectedPoints);
        PlaySound(context, affectedPoints);
        ApplyDamage(context, affectedEntities);
    }
}