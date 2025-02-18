using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class LightPriestScript(Dialog subject, ISpellFactory spellFactory)
    : DialogScriptBase(subject)
{
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out MasterPriestPath stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "saintaugustine_initial":
            {
                if (source.UserStatSheet.Level < 99)
                {
                    Subject.Reply(
                        source,
                        "You so far to be here, you must be tired little thing. I'm sorry, but I cannot help you in any way.");

                    return;
                }

                if (source.UserStatSheet.BaseClass != BaseClass.Priest)
                {
                    Subject.Reply(
                        source,
                        "I see you are another class, I cannot teach you the magic I possess. Only a master priest may grasp my knowledge.");

                    return;
                }

                if (!source.UserStatSheet.Master)
                {
                    Subject.Reply(
                        source,
                        "I'm sorry dear, only a master priest would understand these secrets... Please return to me when you are a master of your class.");

                    return;
                }

                if (!hasStage)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "lightpriest_initial",
                        OptionText = "Light Priest"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (hasStage && (stage == MasterPriestPath.Light))
                {
                    var option1 = new DialogOption
                    {
                        DialogKey = "generic_forgetspell_initial",
                        OptionText = "Forget Spells"
                    };

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Insert(0, option1);

                    var option = new DialogOption
                    {
                        DialogKey = "generic_learnspell_initial",
                        OptionText = "Learn Spells"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (hasStage && (stage == MasterPriestPath.Dark))
                    Subject.Reply(source, "I can see the darkness inside of you, begone!");

                break;
            }

            case "lightpriest_initial5":
            {
                var animation = new Animation
                {
                    TargetAnimation = 93,
                    AnimationSpeed = 100
                };

                source.Trackers.Enums.Set(MasterPriestPath.Light);

                source.Legend.AddUnique(
                    new LegendMark(
                        "Walked the path of Light Priest",
                        "lightpriest",
                        MarkIcon.Priest,
                        MarkColor.Pink,
                        1,
                        GameTime.Now));

                if (source.Bank.Contains("Pet Collar"))
                    source.Bank.TryWithdraw("Pet Collar", 1, out _);

                if (source.Inventory.Contains("Pet Collar"))
                    source.Inventory.Remove("Pet Collar");

                var spell = spellFactory.Create("auraofblessing");
                source.SpellBook.TryAddToNextSlot(spell);
                source.SendOrangeBarMessage("You feel a burst of energy, Light fills your body.");
                source.Animate(animation);

                return;
            }
        }
    }
}