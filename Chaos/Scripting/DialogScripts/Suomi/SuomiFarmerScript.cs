using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
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
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "grape_start":
            {
                if (source.UserStatSheet.Level < 11)
                {
                    Subject.Reply(source, "You are too young to harvest my fields. I cannot watch you from here incase you get hurt.");

                    return;
                }

                if (source.Trackers.TimedEvents.HasActiveEvent(
                        "SuomiGrapeCd",
                        out var timedEvent))
                {
                    Subject.Reply(
                        source,
                        $"You already picked grapes today, you can't do it again! (({timedEvent.Remaining.ToReadableString()
                        }))");

                    return;
                }

                if (!source.TryTakeGold(5000))
                {
                    Subject.Reply(
                        source,
                        "You can't go in there for free! It's 5,000 coins, I have things to do if you're not going to pay.");

                    return;
                }

                Subject.Reply(
                    source,
                    "Take as long as you like, however, once you've picked alot, I'm going to pull you from the field.");

                var mapInstance = SimpleCache.Get<MapInstance>("suomi");
                var point = new Point(78, 40);
                source.TraverseMap(mapInstance, point);
                source.Trackers.TimedEvents.AddEvent("SuomiGrapeCd", TimeSpan.FromHours(22), true);
            }

                break;

            case "cherry_start":
            {
                if (source.UserStatSheet.Level < 11)
                {
                    Subject.Reply(source, "You are too young to harvest my fields. I cannot watch you from here incase you get hurt.");

                    return;
                }

                if (source.Trackers.TimedEvents.HasActiveEvent(
                        "SuomiCherryCd",
                        out var timedEvent))
                {
                    Subject.Reply(
                        source,
                        $"You've recently farmed Cherries, best to let them grow. Come back later. (({
                            timedEvent.Remaining.ToReadableString()
                        }))");

                    return;
                }

                if (!source.TryTakeGold(5000))
                {
                    Subject.Reply(source, "Excuse me, it's 5,000 gold to access my field. You don't have enough.");

                    return;
                }

                Subject.Reply(
                    source,
                    "Thank you Aisling, enjoy your cherry farming! Once you're full up, I'll pull you from the field.");

                var mapInstance = SimpleCache.Get<MapInstance>("suomi");
                var point = new Point(29, 71);
                source.TraverseMap(mapInstance, point);
                source.Trackers.TimedEvents.AddEvent("SuomiCherryCd", TimeSpan.FromHours(22), true);
            }

                break;
        }
    }
}