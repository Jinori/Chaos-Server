using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicMantisWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "mantiswarp_initial":
            {
                var vitality = source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2;
                
                if (source.StatSheet.Level >= 80)
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "mantiswarp_mantis3",
                        OptionText = "Mantis 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if (source.StatSheet.Level >= 51)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "mantiswarp_mantis2",
                        OptionText = "Mantis 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 21)
                {
                    source.SendOrangeBarMessage("You must be atleast level 21 to enter mantis.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "mantiswarp_mantis1",
                    OptionText = "Mantis 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "mantiswarp_mantis1");

                break;
            }

            case "mantiswarp_mantis1":
            {
                var mantis1 = SimpleCache.Get<MapInstance>("mantis1-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(2, 10);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(mantis1, point);
                } else
                {
                    var point = new Point(23, 12);
                    source.TraverseMap(mantis1, point);
                }

                Subject.Close(source);

                break;
            }

            case "mantiswarp_mantis2":
            {
                var mantis1 = SimpleCache.Get<MapInstance>("mantis2-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(2, 10);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(mantis1, point);
                } else
                {
                    var point = new Point(23, 12);
                    source.TraverseMap(mantis1, point);
                }


                Subject.Close(source);

                break;
            }

            case "mantiswarp_mantis3":
            {
                var mantis1 = SimpleCache.Get<MapInstance>("mantis3-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(2, 10);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(23, 13);
                    source.TraverseMap(mantis1, point);
                } else
                {
                    var point = new Point(23, 12);
                    source.TraverseMap(mantis1, point);
                }


                Subject.Close(source);

                break;
            }
        }
    }
}