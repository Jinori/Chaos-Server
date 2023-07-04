using Chaos.Common.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class KillCountMessageScript : ConfigurableItemScriptBase
{
    protected bool Admin { get; init; }

    public KillCountMessageScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        if (!Admin)
        {
            var killCounts = source.Trackers.Counters.Select(x => $"{x.Key} - {x.Value}").ToArray();

            if (killCounts.Length > 0)
                foreach (var kill in killCounts)
                    source.Client.SendServerMessage(ServerMessageType.WoodenBoard, kill);
            else
                source.Client.SendServerMessage(ServerMessageType.WoodenBoard, "No currently recorded kills.");
        }
    }
}