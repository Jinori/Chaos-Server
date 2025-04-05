using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class StudyCreatureScript : ConfigurableSkillScriptBase, GenericAbilityComponent<Creature>.IAbilityComponentOptions
{
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    public List<string>? EffectKeysToBreak { get; set; }

    /// <inheritdoc />

    public int? ExclusionRange { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    public int? HealthCost { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    public decimal PctHealthCost { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public bool StopOnFirstHit { get; init; }
    public bool StopOnWalls { get; init; }

    /// <inheritdoc />
    public StudyCreatureScript(Skill subject)
        : base(subject) { }

    private string GetElementColor(Element element)
        => element switch
        {
            Element.Fire     => $"{MessageColor.Red.ToPrefix()}FIRE{MessageColor.Orange.ToPrefix()}",
            Element.Water    => $"{MessageColor.Silver.ToPrefix()}WATER{MessageColor.Orange.ToPrefix()}",
            Element.Earth    => $"{MessageColor.Yellow.ToPrefix()}EARTH{MessageColor.Orange.ToPrefix()}",
            Element.Wind     => $"{MessageColor.NeonGreen.ToPrefix()}WIND{MessageColor.Orange.ToPrefix()}",
            Element.None     => $"{MessageColor.Gray.ToPrefix()}NONE{MessageColor.Orange.ToPrefix()}",
            Element.Holy     => $"{MessageColor.HotPink.ToPrefix()}HOLY{MessageColor.Orange.ToPrefix()}",
            Element.Darkness => $"{MessageColor.Black.ToPrefix()}DARK{MessageColor.Orange.ToPrefix()}",
            Element.Wood     => $"{MessageColor.Brown.ToPrefix()}WOOD{MessageColor.Orange.ToPrefix()}",
            Element.Metal    => $"{MessageColor.Silver.ToPrefix()}METAL{MessageColor.Orange.ToPrefix()}",
            Element.Undead   => $"{MessageColor.DarkGreen.ToPrefix()}UNDEAD{MessageColor.Orange.ToPrefix()}",
            _                => throw new ArgumentOutOfRangeException(nameof(element), element, null)
        };

    private string GetElementScrollWindowColor(Element element)
        => element switch
        {
            Element.Fire     => $"{MessageColor.Red.ToPrefix()}FIRE{MessageColor.Gray.ToPrefix()}",
            Element.Water    => $"{MessageColor.Silver.ToPrefix()}WATER{MessageColor.Gray.ToPrefix()}",
            Element.Earth    => $"{MessageColor.Yellow.ToPrefix()}EARTH{MessageColor.Gray.ToPrefix()}",
            Element.Wind     => $"{MessageColor.NeonGreen.ToPrefix()}WIND{MessageColor.Gray.ToPrefix()}",
            Element.None     => $"{MessageColor.Gray.ToPrefix()}NONE{MessageColor.Gray.ToPrefix()}",
            Element.Holy     => $"{MessageColor.HotPink.ToPrefix()}HOLY{MessageColor.Gray.ToPrefix()}",
            Element.Darkness => $"{MessageColor.Black.ToPrefix()}DARK{MessageColor.Gray.ToPrefix()}",
            Element.Wood     => $"{MessageColor.Brown.ToPrefix()}WOOD{MessageColor.Gray.ToPrefix()}",
            Element.Metal    => $"{MessageColor.Silver.ToPrefix()}METAL{MessageColor.Gray.ToPrefix()}",
            Element.Undead   => $"{MessageColor.DarkGreen.ToPrefix()}UNDEAD{MessageColor.Gray.ToPrefix()}",
            _                => throw new ArgumentOutOfRangeException(nameof(element), element, null)
        };

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        if (context.SourceAisling is not null)
        {
            var options = new AoeShapeOptions
            {
                Direction = context.SourceAisling.Direction,
                Source = new Point(context.SourceAisling.X, context.SourceAisling.Y),
                Range = 3
            };

            var points = AoeShape.Front.ResolvePoints(options);

            var entities = context.SourceMap.GetEntitiesAtPoints<Creature>(points.OfType<IPoint>());

            var mob = entities.FirstOrDefault(entity => entity is not Aisling && !entity.Script.Is<PetScript>());

            if (mob is not null)
            {
                var offenseColor = GetElementColor(mob.StatSheet.OffenseElement);
                var defenseColor = GetElementColor(mob.StatSheet.DefenseElement);
                var offenseScrollColor = GetElementScrollWindowColor(mob.StatSheet.OffenseElement);
                var defenseScrollColor = GetElementScrollWindowColor(mob.StatSheet.DefenseElement);

                context.SourceAisling?.Client.SendServerMessage(
                    ServerMessageType.ScrollWindow,
                    $"Name: {mob.Name}\nLevel: {mob.StatSheet.Level}\nHealth: {mob.StatSheet.CurrentHp}\nArmor Class: {
                        mob.StatSheet.EffectiveAc}\nCurrent Mana: {mob.StatSheet.CurrentMp}\nOffensive Element: {offenseScrollColor
                        }\nDefensive Element: {defenseScrollColor}");

                var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));
                var message = $"{mob.Name}: Hp:{mob.StatSheet.HealthPercent:F0}% OFF: {offenseColor} DEF: {defenseColor}";

                if (group == null)
                    context.SourceAisling?.SendOrangeBarMessage(message);

                if (group != null)
                    foreach (var entity in group)
                    {
                        var showMobEle = entity.MapInstance
                                               .GetEntities<Creature>()
                                               .FirstOrDefault(x => x.Equals(mob));
                        showMobEle?.Chant(showMobEle.StatSheet.DefenseElement.ToString());
                        entity.SendOrangeBarMessage(message);
                    }
            }

            if (mob == null)
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your attempt to examine failed.");
        }
    }
}