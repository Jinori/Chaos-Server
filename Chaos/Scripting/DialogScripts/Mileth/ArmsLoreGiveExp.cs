using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class ArmsLoreGiveExpScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public ArmsLoreGiveExpScript(Dialog subject)
        : base(subject) => ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplayed(Aisling source)
    {
        if (source.Trackers.Flags.HasFlag(QuestFlag1.Arms))
            return;

        source.Trackers.Flags.AddFlag(QuestFlag1.Arms);
        ExperienceDistributionScript.GiveExp(source, 1000);
    }
}