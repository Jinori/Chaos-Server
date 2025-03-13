using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicZombieWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "zombiewarp_initial":
            {
                if (source.StatSheet.Level >= 97)
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "zombiewarp_zombie3",
                        OptionText = "Zombie 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if (source.StatSheet.Level >= 71)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "zombiewarp_zombie2",
                        OptionText = "Zombie 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 41)
                {
                    source.SendOrangeBarMessage("You must be atleast level 41 to enter zombies.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "zombiewarp_zombie1",
                    OptionText = "Zombie 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "zombiewarp_zombie1");

                break;
            }

            case "zombiewarp_zombie1":
            {
                var zombie1 = SimpleCache.Get<MapInstance>("zombie1-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(3, 27);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(zombie1, point);
                } else
                {
                    var point = new Point(23, 14);
                    source.TraverseMap(zombie1, point);
                }

                Subject.Close(source);

                break;
            }

            case "zombiewarp_zombie2":
            {
                var zombie1 = SimpleCache.Get<MapInstance>("zombie2-1");
                var pointOne = new Point(3, 27);
                var sourcePoint = new Point(source.X, source.Y);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(zombie1, point);
                } else
                {
                    var point = new Point(23, 14);
                    source.TraverseMap(zombie1, point);
                }

                Subject.Close(source);

                break;
            }

            case "zombiewarp_zombie3":
            {
                var zombie1 = SimpleCache.Get<MapInstance>("zombie3-1");
                var pointOne = new Point(3, 27);
                var sourcePoint = new Point(source.X, source.Y);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(zombie1, point);
                } else
                {
                    var point = new Point(23, 14);
                    source.TraverseMap(zombie1, point);
                }

                Subject.Close(source);

                break;
            }
        }
    }
}