using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Objects.Legend;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripts.ItemScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.ItemScripts;

public class FishConsumableScript : ItemScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private protected int Percent { get; set; }

    public FishConsumableScript(Item subject)
        : base(subject) => ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnUse(Aisling source)
    {
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);

        switch (Subject.DisplayName)
        {
            case "Trout":
                Percent = Convert.ToInt32(0.006 * tnl);

                break;
            case "Bass":
                Percent = Convert.ToInt32(0.007 * tnl);

                break;
            case "Perch":
                Percent = Convert.ToInt32(0.008 * tnl);

                break;
            case "Pike":
                Percent = Convert.ToInt32(0.009 * tnl);

                break;
            case "Rock Fish":
                Percent = Convert.ToInt32(0.01 * tnl);

                break;
            case "Lion Fish":
                Percent = Convert.ToInt32(0.02 * tnl);

                break;
            case "Purple Whopper":
                Percent = Convert.ToInt32(0.03 * tnl);

                break;
        }

        ExperienceDistributionScript.GiveExp(source, Percent);
        source.Inventory.RemoveQuantity(Subject.DisplayName, 1, out _);
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You ate {Subject.DisplayName} and it gave you {Percent} exp.");

        source.Legend.AddOrAccumulate(
            new LegendMark(
                "Caught a fish and ate it",
                "fish",
                MarkIcon.Yay,
                MarkColor.White,
                1,
                GameTime.Now));
    }
}