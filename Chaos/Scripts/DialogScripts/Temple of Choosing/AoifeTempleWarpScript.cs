using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts
{
    public class AoifeTempleWarpScript : DialogScriptBase
    {
        private readonly ISimpleCache SimpleCache;

        public AoifeTempleWarpScript(Dialog subject, ISimpleCache simpleCache) : base(subject)
        {
            SimpleCache = simpleCache;
        }

        public override void OnDisplayed(Aisling source)
        {
            if (source.Flags.HasFlag(QuestFlag1.ChosenClass))
            {
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You have already chosen a class. Luck be with you.");
                return;
            }

            var mapInstance = SimpleCache.Get<MapInstance>("tocinner");
            var point = new Point(22, 13);
            source.TraverseMap(mapInstance, point);
        }
    }
}
