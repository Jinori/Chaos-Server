using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicWolfWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "wolfwarp_initial":
            {
                var vitality = source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2;
                
                if (source.StatSheet.Level >= 90)
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "wolfwarp_wolf3",
                        OptionText = "Wolf 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if (source.StatSheet.Level >= 60)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "wolfwarp_wolf2",
                        OptionText = "Wolf 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 30)
                {
                    source.SendOrangeBarMessage("You must be atleast level 30 to enter wolf.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "wolfwarp_wolf1",
                    OptionText = "Wolf 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "wolfwarp_wolf1");

                break;
            }

            case "wolfwarp_wolf1":
            {
                var wolf1 = SimpleCache.Get<MapInstance>("wolf1-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(3, 39);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 14);
                    source.TraverseMap(wolf1, point);
                } else
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(wolf1, point);
                }

                Subject.Close(source);

                break;
            }

            case "wolfwarp_wolf2":
            {
                var wolf1 = SimpleCache.Get<MapInstance>("wolf2-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(3, 39);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 14);
                    source.TraverseMap(wolf1, point);
                } else
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(wolf1, point);
                }


                Subject.Close(source);

                break;
            }

            case "wolfwarp_wolf3":
            {
                var wolf1 = SimpleCache.Get<MapInstance>("wolf3-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(3, 39);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 14);
                    source.TraverseMap(wolf1, point);
                } else
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(wolf1, point);
                }


                Subject.Close(source);

                break;
            }
        }
    }
}