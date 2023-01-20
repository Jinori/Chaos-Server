using Chaos.Common.Definitions;
using Chaos.Common.Identity;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MerchantScripts.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World;

public sealed class Merchant : Creature, IScripted<IMerchantScript>
{
    /// <inheritdoc />
    public override int AssailIntervalMs => 500;
    public override bool IsAlive => true;
    public override ILogger<Merchant> Logger { get; }
    /// <inheritdoc />
    public override IMerchantScript Script { get; }
    /// <inheritdoc />
    public override ISet<string> ScriptKeys { get; }
    public override StatSheet StatSheet { get; }
    public MerchantTemplate Template { get; }

    public override CreatureType Type { get; }

    /// <inheritdoc />
    public override void ApplyDamage(Creature source, int amount, byte? hitSound = 1)
    {
        StatSheet.SubtractHp(amount);
        ShowHealth(hitSound);
    }

    public override void ApplyHealing(Creature source, int amount)
    {
        StatSheet.AddHp(amount);
        ShowHealth();
    }

    public override void ApplyMana(Creature source, int amount)
    {
        StatSheet.AddMp(amount);
    }

    public Merchant(
        MerchantTemplate template,
        MapInstance mapInstance,
        IPoint point,
        ILogger<Merchant> logger,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys = null
    )
        : base(
            template.Name,
            template.Sprite,
            mapInstance,
            point)
    {
        extraScriptKeys ??= Array.Empty<string>();

        Template = template;
        Logger = logger;
        StatSheet = StatSheet.Maxed;
        Type = CreatureType.Merchant;
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IMerchantScript, Merchant>(ScriptKeys, this);
    }

    /// <inheritdoc />
    public override void OnApproached(Creature creature) => Script.OnApproached(creature);

    public override void OnClicked(Aisling source) => Script.OnClicked(source);

    /// <inheritdoc />
    public override void OnDeparture(Creature creature) => Script.OnDeparture(creature);

    public override void OnGoldDroppedOn(Aisling source, int amount) => Script.OnGoldDroppedOn(source, amount);

    public override void OnItemDroppedOn(Aisling source, byte slot, byte count) => Script.OnItemDroppedOn(source, slot, count);

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Script.Update(delta);
    }
}