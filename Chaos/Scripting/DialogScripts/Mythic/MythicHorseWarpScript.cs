using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicHorseWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "horsewarp_initial":
            {
                var vitality = source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2;
                
                if ((source.StatSheet.Level >= 99) && (vitality >= 50000))
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "horsewarp_horse3",
                        OptionText = "Horse 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if ((source.StatSheet.Level >= 80) && (vitality >= 20000))
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "horsewarp_horse2",
                        OptionText = "Horse 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 80)
                {
                    source.SendOrangeBarMessage("You must be at least level 80 to enter horses.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "horsewarp_horse1",
                    OptionText = "Horse 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "horsewarp_horse1");

                break;
            }

            case "horsewarp_horse1":
            {
                var horse1 = SimpleCache.Get<MapInstance>("horse1-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(7, 46);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(12, 1);
                    source.TraverseMap(horse1, point);
                } else
                {
                    var point = new Point(13, 1);
                    source.TraverseMap(horse1, point);
                }

                Subject.Close(source);

                break;
            }

            case "horsewarp_horse2":
            {
                var horse1 = SimpleCache.Get<MapInstance>("horse2-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(7, 46);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(12, 1);
                    source.TraverseMap(horse1, point);
                } else
                {
                    var point = new Point(13, 1);
                    source.TraverseMap(horse1, point);
                }


                Subject.Close(source);

                break;
            }

            case "horsewarp_horse3":
            {
                var horse1 = SimpleCache.Get<MapInstance>("horse3-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(7, 46);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(12, 1);
                    source.TraverseMap(horse1, point);
                } else
                {
                    var point = new Point(13, 1);
                    source.TraverseMap(horse1, point);
                }


                Subject.Close(source);

                break;
            }
        }
    }
}