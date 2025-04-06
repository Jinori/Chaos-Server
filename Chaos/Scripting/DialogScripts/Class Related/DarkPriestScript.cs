using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class DarkPriestScript : DialogScriptBase
{
    private readonly ILogger<DarkPriestScript> Logger;
    private readonly ISpellFactory SpellFactory;

    public DarkPriestScript(Dialog subject, ISpellFactory spellFactory, ILogger<DarkPriestScript> logger)
        : base(subject)
    {
        Logger = logger;
        SpellFactory = spellFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        if (!source.Trackers.Enums.TryGetValue(out MasterPriestPath stage))
            stage = MasterPriestPath.None;

        switch (Subject.Template.TemplateKey.ToLowerInvariant())
        {
            case "morgoth_initial":
                HandleInitialMorgothInteraction(source, stage);
                break;

            case "darkpriest_initial5":
                PromoteToDarkPriest(source);
                break;
        }
    }

    private void HandleInitialMorgothInteraction(Aisling source, MasterPriestPath stage)
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
            Subject.Reply(source, "You must master your class before I could possibly teach you the dark ways. Return to me once you master, we have much to learn.");

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
                  .WithProperty(Subject)
                  .LogInformation("{@AislingName} tried to become Dark Priest before Mastering", source.Name);

            return;
        }

        switch (stage)
        {
            case MasterPriestPath.None:
                AddDialogOption("Dark Priest", "darkpriest_initial");

                break;
            
            case MasterPriestPath.Dark:
                AddDialogOption("Forget Spells", "generic_forgetspell_initial");
                AddDialogOption("Learn Spells", "generic_learnspell_initial");

                break;
            
            case MasterPriestPath.Light:
                Subject.Reply(source, "The Light has already shown through you, you are no use to me, get lost.");

                Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
                      .WithProperty(Subject)
                      .LogInformation("{@AislingName} is Light Priest and spoke to Morgoth", source.Name);

                break;
        }
    }

    private void PromoteToDarkPriest(Aisling source)
    {
        source.Trackers.Enums.Set(MasterPriestPath.Dark);

        source.Legend.AddUnique(new LegendMark(
            "Walked the path of Dark Priest",
            "darkpriest",
            MarkIcon.Priest,
            MarkColor.DarkPurple,
            1,
            GameTime.Now
        ));

        // Remove pet collar if present
        source.Bank.TryWithdraw("Pet Collar", 1, out _);
        if (source.Inventory.Contains("Pet Collar"))
            source.Inventory.Remove("Pet Collar");

        // Spellbook update
        source.SpellBook.Remove("zap");
        source.SpellBook.TryAddToNextSlot(SpellFactory.Create("voidjolt"));
        source.SpellBook.TryAddToNextSlot(SpellFactory.Create("auraoftorment"));

        // Visual + stat feedback
        source.UserStatSheet.SetHp(1);
        source.UserStatSheet.SetMp(1);
        source.SendOrangeBarMessage("You feel a surge of power, Darkness has consumed your body.");
        source.Animate(new Animation { TargetAnimation = 104, AnimationSpeed = 100 });

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
              .WithProperty(Subject)
              .LogInformation("{@AislingName} became a Dark Priest through Morgoth", source.Name);
    }

    private void AddDialogOption(string optionText, string dialogKey)
    {
        if (!Subject.HasOption(optionText))
        {
            Subject.Options.Insert(0, new DialogOption
            {
                OptionText = optionText,
                DialogKey = dialogKey
            });
        }
    }
}