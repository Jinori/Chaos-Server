using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Models.Legend;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.ItemScripts;

public class FishConsumableScript : ItemScriptBase
{
    private static readonly Dictionary<string, double> FishExperienceMultipliers = new()
    {
        { "Trout", 0.006 },
        { "Bass", 0.007 },
        { "Perch", 0.008 },
        { "Pike", 0.009 },
        { "Rock Fish", 0.01 },
        { "Lion Fish", 0.02 },
        { "Purple Whopper", 0.03 }
    };

    private readonly IExperienceDistributionScript ExperienceDistributionScript;

    public FishConsumableScript(Item subject)
        : base(subject)
        => ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    private int CalculateExperienceGain(Aisling source, int tnl)
    {
        if (!FishExperienceMultipliers.TryGetValue(Subject.DisplayName, out var multiplier))
        {
            source.SendActiveMessage("Something went wrong when trying to eat the fish!");

            return 0;
        }

        return Convert.ToInt32(multiplier * tnl);
    }

    private void NotifyPlayer(Aisling source, int expGain) => source.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        $"You ate {Subject.DisplayName} and it gave you {expGain} exp.");

    public override void OnUse(Aisling source)
    {
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var expGain = CalculateExperienceGain(source, tnl);

        ExperienceDistributionScript.GiveExp(source, expGain);
        RemoveItemFromInventory(source);
        NotifyPlayer(source, expGain);
        UpdatePlayerLegend(source);
    }

    private void RemoveItemFromInventory(Aisling source) => source.Inventory.RemoveQuantity(Subject.DisplayName, 1, out _);

    private void UpdatePlayerLegend(Aisling source) =>
        source.Legend.AddOrAccumulate(
            new LegendMark(
                "Caught a fish and ate it.",
                "fish",
                MarkIcon.Yay,
                MarkColor.White,
                1,
                GameTime.Now));
}