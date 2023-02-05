using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.DialogScripts.Mileth;

public class ArmsLoreGiveExpScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public ArmsLoreGiveExpScript(Dialog subject)
        : base(subject) => ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplayed(Aisling source)
    {
        if (source.Flags.HasFlag(QuestFlag1.Arms))
            return;

        source.Flags.AddFlag(QuestFlag1.Arms);
        ExperienceDistributionScript.GiveExp(source, 1000);
    }
}