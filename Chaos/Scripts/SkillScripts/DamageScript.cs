using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
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
    private readonly ISkillFactory SkillFactory;

    /// <inheritdoc />
    public DamageScript(Skill subject, IEffectFactory effectFactory, ISkillFactory skillFactory)
        : base(subject) { EffectFactory = effectFactory; SkillFactory = skillFactory; }

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
            #region ChiReactions
            if (target is Aisling aisling 
                && !aisling.Effects.Contains("chiBlocker") 
                && aisling.Equipment.TryGetObject((byte)EquipmentSlot.Boots, out var boots) 
                && boots.Template.TemplateKey.EqualsI("chiAnklet") && !Subject.Template.IsAssail)
            {
                var chiBlock = (ChiAnkletFlags)aisling.Flags.GetFlag<ChiAnkletFlags>();
                chiBlock &= ChiAnkletFlags.Block1 | ChiAnkletFlags.Block2 | ChiAnkletFlags.Block3 | ChiAnkletFlags.Block4 | ChiAnkletFlags.Block5;
                switch (chiBlock)
                {
                    case ChiAnkletFlags.Block1:
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
                            aisling.MapInstance.ShowAnimation(ani.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                    case ChiAnkletFlags.Block2:
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
                            aisling.MapInstance.ShowAnimation(ani.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                    case ChiAnkletFlags.Block3:
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
                            aisling.MapInstance.ShowAnimation(ani.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                    case ChiAnkletFlags.Block4:
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
                            aisling.MapInstance.ShowAnimation(ani.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                    case ChiAnkletFlags.Block5:
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
                            aisling.MapInstance.ShowAnimation(ani.GetTargetedAnimation(aisling.Id));
                            return;
                        }
                        break;
                }
                var chiDtk = (ChiAnkletFlags)aisling.Flags.GetFlag<ChiAnkletFlags>();
                chiDtk &= ChiAnkletFlags.DTKProc1 | ChiAnkletFlags.DTKProc2 | ChiAnkletFlags.DTKProc3 | ChiAnkletFlags.DTKProc4 | ChiAnkletFlags.DTKProc5;
                switch (chiDtk)
                {
                    case ChiAnkletFlags.DTKProc1:
                        if (Randomizer.RollChance(1))
                        {
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var skill = SkillFactory.Create("roundhousekick");
                            aisling.TryUseSkill(skill);
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi reacted to {Subject.Template.Name} with a {skill.Template.Name}!");
                        }
                        break;
                    case ChiAnkletFlags.DTKProc2:
                        if (Randomizer.RollChance(2))
                        {
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var skill = SkillFactory.Create("roundhousekick");
                            aisling.TryUseSkill(skill);
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi reacted to {Subject.Template.Name} with a {skill.Template.Name}!");
                        }
                        break;
                    case ChiAnkletFlags.DTKProc3:
                        if (Randomizer.RollChance(3))
                        {
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var skill = SkillFactory.Create("roundhousekick");
                            aisling.TryUseSkill(skill);
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi reacted to {Subject.Template.Name} with a {skill.Template.Name}!");
                        }
                        break;
                    case ChiAnkletFlags.DTKProc4:
                        if (Randomizer.RollChance(4))
                        {
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var skill = SkillFactory.Create("roundhousekick");
                            aisling.TryUseSkill(skill);
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi reacted to {Subject.Template.Name} with a {skill.Template.Name}!");
                        }
                        break;
                    case ChiAnkletFlags.DTKProc5:
                        if (Randomizer.RollChance(5))
                        {
                            var effect = EffectFactory.Create("chiBlocker");
                            aisling.Effects.Apply(aisling, effect);
                            var skill = SkillFactory.Create("roundhousekick");
                            aisling.TryUseSkill(skill);
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your Chi reacted to {Subject.Template.Name} with a {skill.Template.Name}!");
                        }
                        break;
                }
            }
            #endregion ChiReactions

            ApplyDamageScripts.Default.ApplyDamage(
                context.Source,
                target,
                this,
                CalculateDamage(context, target));
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
		
    protected IApplyDamageScript ApplyDamageScript { get; }
    protected DamageComponent DamageComponent { get; }
    protected DamageComponent.DamageComponentOptions DamageComponentOptions { get; }

    /// <inheritdoc />
    public DamageScript(Skill subject)
        : base(subject)
    {
        ApplyDamageScript = DefaultApplyDamageScript.Create();
        DamageComponent = new DamageComponent();

        DamageComponentOptions = new DamageComponent.DamageComponentOptions
        {
            ApplyDamageScript = ApplyDamageScript,
            SourceScript = this,
            BaseDamage = BaseDamage,
            DamageMultiplier = DamageMultiplier,
            DamageStat = DamageStat
        };
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
        DamageComponent.ApplyDamage(context, targets.TargetEntities, DamageComponentOptions);
    }

    #region ScriptVars
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageMultiplier { get; init; }
    #endregion
}