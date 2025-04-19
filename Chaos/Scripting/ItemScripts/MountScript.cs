using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Humanizer;

namespace Chaos.Scripting.ItemScripts;

public class MountScript(Item subject, IEffectFactory effectFactory) : ItemScriptBase(subject)
{
    private static readonly Dictionary<(CurrentMount, CurrentCloak), int> MountAndCloakSprites = new()
    {
        {
            (CurrentMount.Horse, CurrentCloak.Red), 1334
        },
        {
            (CurrentMount.Horse, CurrentCloak.Blue), 1332
        },
        {
            (CurrentMount.Horse, CurrentCloak.Black), 1331
        },
        {
            (CurrentMount.Horse, CurrentCloak.Green), 1296
        },
        {
            (CurrentMount.Horse, CurrentCloak.Purple), 1333
        },
        {
            (CurrentMount.Wolf, CurrentCloak.Red), 1326
        },
        {
            (CurrentMount.Wolf, CurrentCloak.Blue), 1324
        },
        {
            (CurrentMount.Wolf, CurrentCloak.Black), 1323
        },
        {
            (CurrentMount.Wolf, CurrentCloak.Green), 1297
        },
        {
            (CurrentMount.Wolf, CurrentCloak.Purple), 1325
        },
        {
            (CurrentMount.Kelberoth, CurrentCloak.Red), 1330
        },
        {
            (CurrentMount.Kelberoth, CurrentCloak.Blue), 1328
        },
        {
            (CurrentMount.Kelberoth, CurrentCloak.Black), 1327
        },
        {
            (CurrentMount.Kelberoth, CurrentCloak.Green), 1312
        },
        {
            (CurrentMount.Kelberoth, CurrentCloak.Purple), 1329
        },
        {
            (CurrentMount.Ant, CurrentCloak.Red), 1339
        },
        {
            (CurrentMount.Ant, CurrentCloak.Blue), 1337
        },
        {
            (CurrentMount.Ant, CurrentCloak.Black), 1336
        },
        {
            (CurrentMount.Ant, CurrentCloak.Green), 1335
        },
        {
            (CurrentMount.Ant, CurrentCloak.Purple), 1338
        },
        {
            (CurrentMount.Dunan, CurrentCloak.Red), 1322
        },
        {
            (CurrentMount.Dunan, CurrentCloak.Blue), 1320
        },
        {
            (CurrentMount.Dunan, CurrentCloak.Black), 1319
        },
        {
            (CurrentMount.Dunan, CurrentCloak.Green), 1318
        },
        {
            (CurrentMount.Dunan, CurrentCloak.Purple), 1321
        },
        {
            (CurrentMount.Bee, CurrentCloak.Red), 1316
        },
        {
            (CurrentMount.Bee, CurrentCloak.Blue), 1313
        },
        {
            (CurrentMount.Bee, CurrentCloak.Black), 1317
        },
        {
            (CurrentMount.Bee, CurrentCloak.Green), 1314
        },
        {
            (CurrentMount.Bee, CurrentCloak.Purple), 1315
        }

        // Add more mount and cloak combinations here as needed
    };
    public override void OnUse(Aisling source)
    {
        if (source.Effects.Contains("Hide"))
        {
            source.SendOrangeBarMessage("You cannot mount while hidden.");

            return;
        }

        if (source.Effects.Contains("Werewolf"))
        {
            source.SendOrangeBarMessage("You cannot mount while you are a werewolf.");

            return;
        }

        if (source.Trackers.TimedEvents.HasActiveEvent("Mount", out var timedEvent))
        {
            source.SendOrangeBarMessage($"You can mount again in {timedEvent.Remaining.Humanize()}.");

            return;
        }

        if (source.IsOnArenaMap())
        {
            source.SendOrangeBarMessage("You cannot mount on an arena map.");

            return;
        }

        if (source.Trackers.Enums.TryGetValue(out CurrentMount mount) && source.Trackers.Enums.TryGetValue(out CurrentCloak cloak))
        {
            if (source.Effects.Contains("Mount"))
            {
                source.SendOrangeBarMessage("You jump off your mount.");
                source.Effects.Dispel("Mount");

                return;
            }

            if (MountAndCloakSprites.TryGetValue((mount, cloak), out var sprite))
            {
                source.SetSprite((ushort)sprite);
                source.SendOrangeBarMessage("You jump on your mount.");
                var effect = effectFactory.Create("Mount");
                source.Effects.Apply(source, effect, this);
            }
        }
    }
}