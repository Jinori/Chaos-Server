using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mythic;

public class MythicGargoyleWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "gargoylewarp_initial":
            {
                var vitality = source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2;
                
                if ((source.StatSheet.Level >= 99) && (vitality >= 80000))
                {
                    var option3 = new DialogOption
                    {
                        DialogKey = "gargoylewarp_gargoyle3",
                        OptionText = "Gargoyle 3"
                    };

                    if (!Subject.HasOption(option3.OptionText))
                        Subject.Options.Insert(0, option3);
                }

                if ((source.StatSheet.Level >= 99) && (vitality >= 40000))
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "gargoylewarp_gargoyle2",
                        OptionText = "Gargoyle 2"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                }

                if (source.UserStatSheet.Level < 93)
                {
                    source.SendOrangeBarMessage("You must be atleast level 93 to enter gargoyles.");
                    Subject.Close(source);
                    var point2 = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point2);

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "gargoylewarp_gargoyle1",
                    OptionText = "Gargoyle 1"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (Subject.Options.Count == 1)
                    Subject.Reply(source, "Skip", "gargoylewarp_gargoyle1");

                break;
            }

            case "gargoylewarp_gargoyle1":
            {
                var gargoyle1 = SimpleCache.Get<MapInstance>("gargoyle1-1");
                var point = new Point(2, 10);
                source.TraverseMap(gargoyle1, point);

                Subject.Close(source);

                break;
            }

            case "gargoylewarp_gargoyle2":
            {
                var gargoyle1 = SimpleCache.Get<MapInstance>("gargoyle2-1");
                var point = new Point(2, 10);
                source.TraverseMap(gargoyle1, point);

                Subject.Close(source);

                break;
            }

            case "gargoylewarp_gargoyle3":
            {
                var gargoyle1 = SimpleCache.Get<MapInstance>("gargoyle3-1");
                var point = new Point(2, 10);
                source.TraverseMap(gargoyle1, point);

                Subject.Close(source);

                break;
            }
        }
    }
}