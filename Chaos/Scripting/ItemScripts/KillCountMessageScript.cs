using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
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
            var killCounts = source.Counters.Select(x => string.Join(" - ", x.Key, x.Value));

            var enumerable = killCounts as string[] ?? killCounts.ToArray();

            if (enumerable.Any())
                foreach (var kill in enumerable)
                    source.Client.SendServerMessage(ServerMessageType.WoodenBoard, kill);
            else
                source.Client.SendServerMessage(ServerMessageType.WoodenBoard, "No currently recorded kills.");
        }
    }
}