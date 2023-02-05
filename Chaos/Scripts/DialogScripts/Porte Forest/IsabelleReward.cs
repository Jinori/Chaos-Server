using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.DialogScripts;

public class IsabelleRewardScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public IsabelleRewardScript(Dialog subject)
        : base(subject) => ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        if (source.Flags.HasFlag(QuestFlag1.IsabelleMantisDead))
        {
            source.Flags.RemoveFlag(QuestFlag1.IsabelleMantisDead);
            source.Flags.AddFlag(QuestFlag1.IsabelleComplete);
            ExperienceDistributionScript.GiveExp(source, 150000);
            source.TryGiveGold(25000);
            source.TryGiveGamePoints(25);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've received 25,000 coins and 25 game points!");
        } else
        {
            Subject.Text = "I can still see it from here! Please take care of it.";
            Subject.Options.Clear();
        }
    }
}