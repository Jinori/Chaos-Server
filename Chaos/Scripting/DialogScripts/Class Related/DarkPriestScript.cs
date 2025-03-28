﻿using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class DarkPriestScript(Dialog subject, ISpellFactory spellFactory)
    : DialogScriptBase(subject)
{
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out MasterPriestPath stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "morgoth_initial":
            {
                if (source.UserStatSheet.Level < 99)
                {
                    Subject.Reply(source, "You are a little young to be here, there's nothing I can do for you. Get out.");

                    return;
                }

                if (source.UserStatSheet.BaseClass != BaseClass.Priest)
                {
                    Subject.Reply(source, "Only priest are welcome here, there's nothing I can do for someone like you.");

                    return;
                }

                if (!source.UserStatSheet.Master)
                {
                    Subject.Reply(
                        source,
                        "You must master your class before I could possibly teach you the dark ways. Return to me once you master, we have much to learn.");

                    return;
                }

                if (!hasStage)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "darkpriest_initial",
                        OptionText = "Dark Priest"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (hasStage && (stage == MasterPriestPath.Dark))
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

                if (hasStage && (stage == MasterPriestPath.Light))
                    Subject.Reply(source, "The Light has already shown through you, you are no use to me, get lost.");

                break;
            }

            case "darkpriest_initial5":
            {
                var animation = new Animation
                {
                    TargetAnimation = 104,
                    AnimationSpeed = 100
                };

                source.Trackers.Enums.Set(MasterPriestPath.Dark);

                source.Legend.AddUnique(
                    new LegendMark(
                        "Walked the path of Dark Priest",
                        "darkpriest",
                        MarkIcon.Priest,
                        MarkColor.DarkPurple,
                        1,
                        GameTime.Now));

                if (source.Bank.Contains("Pet Collar"))
                    source.Bank.TryWithdraw("Pet Collar", 1, out _);

                if (source.Inventory.Contains("Pet Collar"))
                    source.Inventory.Remove("Pet Collar");

                source.SpellBook.Remove("zap");
                var spell = spellFactory.Create("voidjolt");
                var spell2 = spellFactory.Create("auraoftorment");
                source.SpellBook.TryAddToNextSlot(spell);
                source.SpellBook.TryAddToNextSlot(spell2);
                source.SendOrangeBarMessage("You feel a surge of power, Darkness has consumed your body.");
                source.UserStatSheet.SetHp(1);
                source.UserStatSheet.SetMp(1);
                source.Animate(animation);

                return;
            }
        }
    }
}