using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class MountScript : ItemScriptBase
{
    private static readonly Dictionary<(CurrentMount, CurrentCloak), int> MountAndCloakSprites = new()
    {
        { (CurrentMount.Horse, CurrentCloak.Red), 1334 },
        { (CurrentMount.Horse, CurrentCloak.Blue), 1332 },
        { (CurrentMount.Horse, CurrentCloak.Black), 1331 },
        { (CurrentMount.Horse, CurrentCloak.Green), 1296 },
        { (CurrentMount.Horse, CurrentCloak.Purple), 1333},
        { (CurrentMount.Wolf, CurrentCloak.Red), 1326 },
        { (CurrentMount.Wolf, CurrentCloak.Blue), 1324 },
        { (CurrentMount.Wolf, CurrentCloak.Black), 1323 },
        { (CurrentMount.Wolf, CurrentCloak.Green), 1297 },
        { (CurrentMount.Wolf, CurrentCloak.Purple), 1325},
        { (CurrentMount.Kelberoth, CurrentCloak.Red), 1330 },
        { (CurrentMount.Kelberoth, CurrentCloak.Blue), 1328 },
        { (CurrentMount.Kelberoth, CurrentCloak.Black), 1327 },
        { (CurrentMount.Kelberoth, CurrentCloak.Green), 1312 },
        { (CurrentMount.Kelberoth, CurrentCloak.Purple), 1329},
        { (CurrentMount.Ant, CurrentCloak.Red), 1339 },
        { (CurrentMount.Ant, CurrentCloak.Blue), 1337 },
        { (CurrentMount.Ant, CurrentCloak.Black), 1336 },
        { (CurrentMount.Ant, CurrentCloak.Green), 1335 },
        { (CurrentMount.Ant, CurrentCloak.Purple), 1338},
        { (CurrentMount.Dunan, CurrentCloak.Red), 1322 },
        { (CurrentMount.Dunan, CurrentCloak.Blue), 1320 },
        { (CurrentMount.Dunan, CurrentCloak.Black), 1319 },
        { (CurrentMount.Dunan, CurrentCloak.Green), 1318 },
        { (CurrentMount.Dunan, CurrentCloak.Purple), 1321 },
        { (CurrentMount.Bee, CurrentCloak.Red), 1316 },
        { (CurrentMount.Bee, CurrentCloak.Blue), 1313 },
        { (CurrentMount.Bee, CurrentCloak.Black), 1317 },
        { (CurrentMount.Bee, CurrentCloak.Green), 1314 },
        { (CurrentMount.Bee, CurrentCloak.Purple), 1315 },
        // Add more mount and cloak combinations here as needed
    };
    private readonly IEffectFactory _effectFactory;

    public MountScript(Item subject, IEffectFactory effectFactory)
        : base(subject) => _effectFactory = effectFactory;

    public override void OnUse(Aisling source)
    {
        var effect = _effectFactory.Create("mount");

        if (source.Effects.Contains("hide"))
        {
            source.SendOrangeBarMessage("You cannot mount while hidden.");

            return;
        }
        
        if (source.Trackers.TimedEvents.HasActiveEvent("mount", out var timedEvent))
        {
            source.SendOrangeBarMessage($"You can mount again in {timedEvent.Remaining.ToReadableString()}");

            return;
        }

        if (source.Trackers.Enums.TryGetValue(out CurrentMount mount) && source.Trackers.Enums.TryGetValue(out CurrentCloak cloak))
        {
            if (source.Sprite != 0)
            {
                source.SendOrangeBarMessage("You jump off your mount.");
                source.Effects.Dispel("mount");

                return;
            }

            if (MountAndCloakSprites.TryGetValue((mount, cloak), out var sprite))
            {
                source.Sprite = (ushort)sprite;
                source.Refresh(true);
                source.SendOrangeBarMessage("You jump on your mount.");
                source.Effects.Apply(source, effect);
            }
        }
    }
}