using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Suomi;

public class suomiFarmerScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    public suomiFarmerScript(
        Dialog subject,
        ISimpleCache simpleCache
    )
        : base(subject)
    {
        SimpleCache = simpleCache;
    }

    public override void OnDisplayed(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "grape_initial":
            {
                if (source.TimedEvents.TryGetNearestToCompletion(TimedEvent.TimedEventId.SuomiGrapeCd,
                        out var timedEvent))
                {
                    Subject.Text =
                        $"You've recently farmed Grapes, best to let them grow. Come back later. (({timedEvent.Remaining.ToReadableString()
                        }))";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                if (!source.TryTakeGold(5000))
                {
                    Subject.Text = "Excuse me, it's 5,000 gold to access my field. You don't have enough.";
                    Subject.Type = MenuOrDialogType.Normal;
                    return;
                }

                Subject.Text =
                    "Thank you Aisling, enjoy your grape farming! Once you're full up, I'll pull you from the field.";
                var mapInstance = SimpleCache.Get<MapInstance>("suomi");
                var point = new Point(8, 5);
                source.TraverseMap(mapInstance, point);
            }
                break;

            case "cherry_start":
            {
                if (source.TimedEvents.TryGetNearestToCompletion(TimedEvent.TimedEventId.SuomiCherryCd,
                        out var timedEvent))
                {
                    Subject.Text =
                        $"You've recently farmed Cherries, best to let them grow. Come back later. (({timedEvent.Remaining.ToReadableString()
                        }))";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                if (!source.TryTakeGold(5000))
                {
                    Subject.Text = "Excuse me, it's 5,000 gold to access my field. You don't have enough.";
                    Subject.Type = MenuOrDialogType.Normal;
                    return;
                }

                Subject.Text =
                    "Thank you Aisling, enjoy your cherry farming! Once you're full up, I'll pull you from the field.";
                var mapInstance = SimpleCache.Get<MapInstance>("suomi");
                var point = new Point(8, 5);
                source.TraverseMap(mapInstance, point);


            }
                break;

        }
    }
}