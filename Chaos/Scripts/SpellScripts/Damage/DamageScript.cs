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
using Chaos.Scripts.RuntimeScripts;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts.Damage;

public class DamageScript : BasicSpellScriptBase
{
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }
    protected Element? OffensiveElement { get; init; }
    protected int? ManaSpent { get; init; }
    protected bool Missed { get; set; }
    
    protected readonly Animation MissedAnimation = new Animation { AnimationSpeed = 100, TargetAnimation = 115 };
    
    protected readonly IEffectFactory EffectFactory;
    
    protected readonly ISkillFactory SkillFactory;


    /// <inheritdoc />
    public DamageScript(Spell subject, IEffectFactory effectFactory, ISkillFactory skillFactory)
        : base(subject) { EffectFactory = effectFactory; SkillFactory = skillFactory; }
    

    protected virtual void ApplyDamage(SpellContext context, IEnumerable<Creature> targetEntities)
    {
        foreach (var target in targetEntities)
        {
            if (!Randomizer.RollChance(100 - target.StatSheet.EffectiveMagicResistance / 2))
            {
                target.Animate(MissedAnimation);
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your {Subject.Template.Name} has missed!");
                Missed = true;
                return;
            }
            #region ChiReactions
            if (target is Aisling aisling
                && !aisling.Effects.Contains("chiBlocker") 
                && aisling.Equipment.TryGetObject((byte)EquipmentSlot.Boots, out var boots) 
                && boots.Template.TemplateKey.EqualsI("chiAnklet"))
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
            context.Target,
            this,
            CalculateDamage(context, target));
        }
    }

    protected virtual int CalculateDamage(SpellContext context, Creature target)
    {
        var damage = BaseDamage ?? 0;

        if (DamageStat.HasValue)
        {
            var multiplier = DamageStatMultiplier ?? 1;

            damage += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(DamageStat.Value) * multiplier);
        }
        if (OffensiveElement.HasValue)
        {
            return DamageFormulae.Default.CalculateElemental(context.Source, target, damage, OffensiveElement.Value, target.StatSheet.DefenseElement);
        }
        else
            return DamageFormulae.Default.Calculate(context.Source, target, damage);
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

        PlaySound(context, affectedPoints);
        ApplyDamage(context, affectedEntities);

        if (!Missed)
            ShowAnimation(context, affectedPoints);
        else
            Missed = false;
    }
}