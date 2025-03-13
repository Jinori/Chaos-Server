using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicBunnyWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "bunnywarp_initial":
            {
                if (source.StatSheet.Level >= 71)
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "bunnywarp_bunny3",
                        OptionText = "Bunny 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if (source.StatSheet.Level >= 41)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "bunnywarp_bunny2",
                        OptionText = "Bunny 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 9)
                {
                    source.SendOrangeBarMessage("You must be atleast level 9 to enter bunnys.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "bunnywarp_bunny1",
                    OptionText = "Bunny 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "bunnywarp_bunny1");

                break;
            }

            case "bunnywarp_bunny1":
            {
                var bunny1 = SimpleCache.Get<MapInstance>("bunny1-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(9, 2);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(12, 23);
                    source.TraverseMap(bunny1, point);
                } else
                {
                    var point = new Point(13, 23);
                    source.TraverseMap(bunny1, point);
                }

                Subject.Close(source);

                break;
            }

            case "bunnywarp_bunny2":
            {
                var bunny1 = SimpleCache.Get<MapInstance>("bunny2-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(9, 2);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(12, 23);
                    source.TraverseMap(bunny1, point);
                } else
                {
                    var point = new Point(13, 23);
                    source.TraverseMap(bunny1, point);
                }

                Subject.Close(source);

                break;
            }

            case "bunnywarp_bunny3":
            {
                var bunny1 = SimpleCache.Get<MapInstance>("bunny3-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(9, 2);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(12, 23);
                    source.TraverseMap(bunny1, point);
                } else
                {
                    var point = new Point(13, 23);
                    source.TraverseMap(bunny1, point);
                }

                Subject.Close(source);

                break;
            }
        }
    }
}