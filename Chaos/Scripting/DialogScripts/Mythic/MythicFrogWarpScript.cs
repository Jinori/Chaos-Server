using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicFrogWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "frogwarp_initial":
            {
                if (source.StatSheet.Level >= 99)
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "frogwarp_frog3",
                        OptionText = "Frog 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if (source.StatSheet.Level >= 80)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "frogwarp_frog2",
                        OptionText = "Frog 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 50)
                {
                    source.SendOrangeBarMessage("You must be at least level 50 to enter frogs.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "frogwarp_frog1",
                    OptionText = "Frog 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "frogwarp_frog1");

                break;
            }

            case "frogwarp_frog1":
            {
                var frog1 = SimpleCache.Get<MapInstance>("frog1-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(3, 19);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 9);
                    source.TraverseMap(frog1, point);
                } else
                {
                    var point = new Point(23, 10);
                    source.TraverseMap(frog1, point);
                }

                Subject.Close(source);

                break;
            }

            case "frogwarp_frog2":
            {
                var frog1 = SimpleCache.Get<MapInstance>("frog2-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(3, 19);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 9);
                    source.TraverseMap(frog1, point);
                } else
                {
                    var point = new Point(23, 10);
                    source.TraverseMap(frog1, point);
                }

                Subject.Close(source);

                break;
            }

            case "frogwarp_frog3":
            {
                var frog1 = SimpleCache.Get<MapInstance>("frog3-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(3, 19);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 9);
                    source.TraverseMap(frog1, point);
                } else
                {
                    var point = new Point(23, 10);
                    source.TraverseMap(frog1, point);
                }

                Subject.Close(source);

                break;
            }
        }
    }
}