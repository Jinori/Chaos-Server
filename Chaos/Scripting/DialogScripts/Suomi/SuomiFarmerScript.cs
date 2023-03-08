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

    public override void OnDisplaying(Aisling source)
    {
        
        switch (Subject.Template.TemplateKey.ToLower())
        {

            case "grape_start":
            {
                if (source.UserStatSheet.Level < 11)
                {
                    Subject.Text = "You are too young to harvest my fields. I cannot watch you from here incase you get hurt.";
                    return;
                }
                
                if (source.TimedEvents.TryGetNearestToCompletion(TimedEvent.TimedEventId.SuomiGrapeCd,
                        out var timedEvent))
                {
                    Subject.Text =
                        $"You already picked grapes today, you can't do it again! (({timedEvent.Remaining.ToReadableString()
                        }))";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                if (source.UserStatSheet.Level < 11)
                {
                    Subject.Text = "You are too young to harvest my fields. I cannot watch you from here incase you get hurt.";
                    return;
                }

                if (!source.TryTakeGold(5000))
                {
                    Subject.Text = "You can't go in there for free! It's 5,000 coins, I have things to do if you're not going to pay.";
                    Subject.Type = MenuOrDialogType.Normal;
                    return;
                }

                Subject.Text =
                    "Take as long as you like, however, once you've picked alot, I'm going to pull you from the field.";
                var mapInstance = SimpleCache.Get<MapInstance>("suomi");
                var point = new Point(78, 40);
                source.TraverseMap(mapInstance, point);
                source.TimedEvents.AddEvent(TimedEvent.TimedEventId.SuomiGrapeCd, TimeSpan.FromHours(24), true);
            }
                break;

            case "cherry_start":
            {
                if (source.UserStatSheet.Level < 11)
                {
                    Subject.Text = "You are too young to harvest my fields. I cannot watch you from here incase you get hurt.";
                    return;
                }
                
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
                    return;
                }
                
                Subject.Text =
                    "Thank you Aisling, enjoy your cherry farming! Once you're full up, I'll pull you from the field.";
                var mapInstance = SimpleCache.Get<MapInstance>("suomi");
                var point = new Point(29, 71);
                source.TraverseMap(mapInstance, point);
                source.TimedEvents.AddEvent(TimedEvent.TimedEventId.SuomiCherryCd, TimeSpan.FromHours(24), true);
                
            }
                break;

        }
    }
}