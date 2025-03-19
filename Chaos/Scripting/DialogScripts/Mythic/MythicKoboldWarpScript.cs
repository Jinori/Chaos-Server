using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicKoboldWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "koboldwarp_initial":
            {
                var vitality = source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2;
                
                if ((source.StatSheet.Level >= 99) && (vitality >= 25000))
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "koboldwarp_kobold3",
                        OptionText = "Kobold 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if (source.StatSheet.Level >= 90)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "koboldwarp_kobold2",
                        OptionText = "Kobold 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 60)
                {
                    source.SendOrangeBarMessage("You must be at least level 60 to enter kobolds.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "koboldwarp_kobold1",
                    OptionText = "Kobold 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "koboldwarp_kobold1");

                break;
            }

            case "koboldwarp_kobold1":
            {
                var kobold1 = SimpleCache.Get<MapInstance>("kobold1-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(26, 33);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(1, 11);
                    source.TraverseMap(kobold1, point);
                } else
                {
                    var point = new Point(1, 11);
                    source.TraverseMap(kobold1, point);
                }

                Subject.Close(source);

                break;
            }

            case "koboldwarp_kobold2":
            {
                var kobold1 = SimpleCache.Get<MapInstance>("kobold2-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(26, 33);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(1, 11);
                    source.TraverseMap(kobold1, point);
                } else
                {
                    var point = new Point(1, 11);
                    source.TraverseMap(kobold1, point);
                }


                Subject.Close(source);

                break;
            }

            case "koboldwarp_kobold3":
            {
                var kobold1 = SimpleCache.Get<MapInstance>("kobold3-1");
                var sourcePoint = new Point(source.X, source.Y);
                var pointOne = new Point(26, 33);

                if (sourcePoint == pointOne)
                {
                    var point = new Point(1, 11);
                    source.TraverseMap(kobold1, point);
                } else
                {
                    var point = new Point(1, 11);
                    source.TraverseMap(kobold1, point);
                }


                Subject.Close(source);

                break;
            }
        }
    }
}