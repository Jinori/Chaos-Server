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

public class LightPriestScript : DialogScriptBase
{
    private readonly ISpellFactory SpellFactory;
    private readonly ILogger<LightPriestScript> Logger;

    public LightPriestScript(Dialog subject, ISpellFactory spellFactory, ILogger<LightPriestScript> logger)
        : base(subject)
    {
        SpellFactory = spellFactory;
        Logger = logger;
    }

    public override void OnDisplaying(Aisling source)
    {
        if (!source.Trackers.Enums.TryGetValue(out MasterPriestPath stage))
            stage = MasterPriestPath.None;

        switch (Subject.Template.TemplateKey.ToLowerInvariant())
        {
            case "saintaugustine_initial":
                HandleInitialLightPriestDialog(source, stage);
                break;

            case "lightpriest_initial5":
                PromoteToLightPriest(source);
                break;
        }
    }

    private void HandleInitialLightPriestDialog(Aisling source, MasterPriestPath stage)
    {
        if (source.UserStatSheet.Level < 99)
        {
            Subject.Reply(source, "You so far to be here, you must be tired little thing. I'm sorry, but I cannot help you in any way.");
            return;
        }

        if (source.UserStatSheet.BaseClass != BaseClass.Priest)
        {
            Subject.Reply(source, "I see you are another class, I cannot teach you the magic I possess. Only a master priest may grasp my knowledge.");
            return;
        }

        if (!source.UserStatSheet.Master)
        {
            Subject.Reply(source, "I'm sorry dear, only a master priest would understand these secrets... Please return to me when you are a master of your class.");
            
            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
                  .WithProperty(Subject)
                  .LogInformation("{@AislingName} tried to become Light Priest before Mastering", source.Name);

            return;
        }

        if (stage == MasterPriestPath.None)
        {
            AddDialogOption("Light Priest", "lightpriest_initial");
        }

        if (stage == MasterPriestPath.Light)
        {
            AddDialogOption("Forget Spells", "generic_forgetspell_initial");
            AddDialogOption("Learn Spells", "generic_learnspell_initial");
        }

        if (stage == MasterPriestPath.Dark)
        {
            Subject.Reply(source, "I can see the darkness inside of you, begone!");

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
                  .WithProperty(Subject)
                  .LogInformation("{@AislingName} is a Dark Priest and spoke to Saint Augustine. Rejected", source.Name);
        }
    }

    private void PromoteToLightPriest(Aisling source)
    {
        source.Trackers.Enums.Set(MasterPriestPath.Light);

        source.Legend.AddUnique(new LegendMark(
            "Walked the path of Light Priest",
            "lightpriest",
            MarkIcon.Priest,
            MarkColor.Pink,
            1,
            GameTime.Now
        ));

        // Remove pet collar if present
        source.Bank.TryWithdraw("Pet Collar", 1, out _);
        if (source.Inventory.Contains("Pet Collar"))
            source.Inventory.Remove("Pet Collar");

        // Add Light Priest spell
        var spell = SpellFactory.Create("auraofblessing");
        source.SpellBook.TryAddToNextSlot(spell);

        // Animate and give feedback
        source.SendOrangeBarMessage("You feel a burst of energy, Light fills your body.");
        source.Animate(new Animation { TargetAnimation = 93, AnimationSpeed = 100 });

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
              .WithProperty(Subject)
              .LogInformation("{@AislingName} became a Light Priest through Saint Augustine", source.Name);
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
