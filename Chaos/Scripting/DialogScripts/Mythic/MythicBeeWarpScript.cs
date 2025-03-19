using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicBeeWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "beewarp_initial":
            {
                if (source.StatSheet.Level >= 76)
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "beewarp_bee3",
                        OptionText = "Bee 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if (source.StatSheet.Level >= 46)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "beewarp_bee2",
                        OptionText = "Bee 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 14)
                {
                    source.SendOrangeBarMessage("You must be at least level 14 to enter bees.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "beewarp_bee1",
                    OptionText = "Bee 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "beewarp_bee1");

                break;
            }

            case "beewarp_bee1":
            {
                var bee1 = SimpleCache.Get<MapInstance>("bee1-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(18, 46);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 12);
                    source.TraverseMap(bee1, point);
                } else
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(bee1, point);
                }

                Subject.Close(source);

                break;
            }

            case "beewarp_bee2":
            {
                var bee1 = SimpleCache.Get<MapInstance>("bee2-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(18, 46);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 12);
                    source.TraverseMap(bee1, point);
                } else
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(bee1, point);
                }

                Subject.Close(source);

                break;
            }

            case "beewarp_bee3":
            {
                var bee1 = SimpleCache.Get<MapInstance>("bee3-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(18, 46);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 12);
                    source.TraverseMap(bee1, point);
                } else
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(bee1, point);
                }

                Subject.Close(source);

                break;
            }
        }
    }
}