using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts
{
    public class MountScript : ItemScriptBase
    {
        private readonly IEffectFactory _effectFactory;
        private static readonly Dictionary<CurrentMount, int> MountSprites = new()
        {
            { CurrentMount.WhiteHorse, 1296 },
            { CurrentMount.WhiteWolf, 1297 }
            // Add more mount types here as needed
        };

        public MountScript(Item subject, IEffectFactory effectFactory)
            : base(subject) => _effectFactory = effectFactory;

        public override void OnUse(Aisling source)
        {
            var effect = _effectFactory.Create("mount");

            if (source.Trackers.Enums.TryGetValue(out CurrentMount mount))
            {
                if (source.Sprite != 0)
                {
                    source.SendOrangeBarMessage("You jump off your mount.");
                    source.Effects.Dispel("mount");
                    return;
                }

                if (source.Trackers.TimedEvents.HasActiveEvent("mount", out var timedEvent))
                {
                    source.SendOrangeBarMessage($"You can mount again in {timedEvent.Remaining.ToReadableString()}");
                    return;
                }

                if (MountSprites.TryGetValue(mount, out var sprite))
                {
                    source.Sprite = (ushort)sprite;
                    source.Refresh(true);
                    source.SendOrangeBarMessage("You jump on your mount.");
                    source.Effects.Apply(source, effect);
                }
            }
        }
    }
}