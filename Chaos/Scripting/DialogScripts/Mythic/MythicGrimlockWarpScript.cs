using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicGrimlockWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "grimlockwarp_initial":
            {
                var vitality = source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2;
                
                if ((source.StatSheet.Level >= 99) && (vitality >= 30000))
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "grimlockwarp_grimlock3",
                        OptionText = "Grimlock 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if (source.StatSheet.Level >= 97)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "grimlockwarp_grimlock2",
                        OptionText = "Grimlock 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 71)
                {
                    source.SendOrangeBarMessage("You must be at least level 71 to enter grimlocks.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "grimlockwarp_grimlock1",
                    OptionText = "Grimlock 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "grimlockwarp_grimlock1");

                break;
            }

            case "grimlockwarp_grimlock1":
            {
                var grimlock1 = SimpleCache.Get<MapInstance>("grimlock1-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(19, 4);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(17, 23);
                    source.TraverseMap(grimlock1, point);
                } else
                {
                    var point = new Point(18, 23);
                    source.TraverseMap(grimlock1, point);
                }

                Subject.Close(source);

                break;
            }

            case "grimlockwarp_grimlock2":
            {
                var grimlock1 = SimpleCache.Get<MapInstance>("grimlock2-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(19, 4);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(17, 23);
                    source.TraverseMap(grimlock1, point);
                } else
                {
                    var point = new Point(18, 23);
                    source.TraverseMap(grimlock1, point);
                }


                Subject.Close(source);

                break;
            }

            case "grimlockwarp_grimlock3":
            {
                var grimlock1 = SimpleCache.Get<MapInstance>("grimlock3-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(19, 4);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(17, 23);
                    source.TraverseMap(grimlock1, point);
                } else
                {
                    var point = new Point(18, 23);
                    source.TraverseMap(grimlock1, point);
                }


                Subject.Close(source);

                break;
            }
        }
    }
}