using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Networking.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.AislingScripts.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.AislingScripts;

public sealed class DefaultAislingScript : AislingScriptBase
{
    private readonly IClientRegistry<IWorldClient> _clientRegistry;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private readonly List<string> _mapsToNotPunishDeathOn = new()
    {
        "tutorial_bossroom",
        "tutorial_farm"
    };

    private RestrictionComponent RestrictionComponent { get; }

    /// <inheritdoc />
    public DefaultAislingScript(Aisling subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject)
    {
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        RestrictionComponent = new RestrictionComponent();
        _clientRegistry = clientRegistry;
    }

    /// <inheritdoc />
    public override bool CanMove() => RestrictionComponent.CanMove(Subject);

    /// <inheritdoc />
    public override bool CanTalk() => RestrictionComponent.CanTalk(Subject);

    /// <inheritdoc />
    public override bool CanTurn() => RestrictionComponent.CanTurn(Subject);

    /// <inheritdoc />
    public override bool CanUseItem(Item item) => RestrictionComponent.CanUseItem(Subject, item);

    /// <inheritdoc />
    public override bool CanUseSkill(Skill skill) => RestrictionComponent.CanUseSkill(Subject, skill);

    /// <inheritdoc />
    public override bool CanUseSpell(Spell spell) => RestrictionComponent.CanUseSpell(Subject, spell);

    /// <inheritdoc />
    public override void OnDeath(Creature source)
    {
        Subject.IsDead = true;

        //Refresh to show ghost
        Subject.Refresh(true);

        //Remove all effects from the player
        var effects = Subject.Effects.ToList();

        foreach (var effect in effects)
            Subject.Effects.Dispel(effect.Name);

        if (_mapsToNotPunishDeathOn.Contains(Subject.MapInstance.InstanceId))
            return;

        foreach (var client in _clientRegistry)
            client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{Subject.Name} was killed at {Subject.MapInstance.Name} by {source.Name}.");

        //Let's break some items at 2% chance
        var itemsToBreak = Subject.Equipment.Where(x => x.Template.AccountBound is false);

        foreach (var item in itemsToBreak)
            if (Randomizer.RollChance(2))
            {
                Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{item.DisplayName} has been consumed by the Death God.");
                Subject.Equipment.TryGetRemove(item.Slot, out _);
            }
        
        var tenPercent = MathEx.GetPercentOf<int>((int)Subject.UserStatSheet.TotalExp, 10);
        if (ExperienceDistributionScript.TryTakeExp(Subject, tenPercent))
        {
            Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You have lost {tenPercent} experience.");   
        }
    }
}