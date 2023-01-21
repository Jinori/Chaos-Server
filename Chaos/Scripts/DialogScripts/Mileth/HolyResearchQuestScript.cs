using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts;

public class HolyResearchQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public HolyResearchQuestScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory,
        ISimpleCache simpleCache
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {

        var hasStage = source.Enums.TryGetValue(out HolyResearchStage stage);

        Skill? assail;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "berteli_initial":
            {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_initial",
                        OptionText = "Holy Research"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }
                

                break;

            case "holyresearch_initial":
                if (!hasStage || (stage == HolyResearchStage.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_yes",
                        OptionText = "Yes I will help you."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "HolyResearch_no",
                        OptionText = "I have better things to do."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);
                }


                if (stage == HolyResearchStage.StartedRawHoney)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_startedrawhoney",
                        OptionText = "I have your Raw Honey here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (stage == HolyResearchStage.StartedRawWax)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_startedrawwax",
                        OptionText = "I have your Raw Wax here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (stage == HolyResearchStage.StartedRoyalWax)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_startedroyalwax",
                        OptionText = "I have your Royal Wax here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "holyresearch_yes":
                if (!hasStage || (stage == HolyResearchStage.None))
                {
                    var randomHolyResearchStage = new[]
                    {
                        HolyResearchStage.StartedRawHoney, HolyResearchStage.StartedRawWax, HolyResearchStage.StartedRoyalWax
                    }.PickRandom();

                    source.Enums.Set(randomHolyResearchStage);

                    switch (randomHolyResearchStage)
                    {
                        case HolyResearchStage.StartedRawHoney:
                        {
                            Subject.Text = $"Thank you so much Aisling, bring me Raw Honey to me.";
                        }

                            break;

                        case HolyResearchStage.StartedRawWax:
                        {
                            Subject.Text = $"Thank you so much Aisling, bring me Raw Wax to me.";
                        }

                            break;

                        case HolyResearchStage.StartedRoyalWax:
                        {
                            Subject.Text = $"Thank you so much Aisling, bring me Royal Wax to me.";
                        }

                            break;
                    }

                }

                break;

            case "holyresearch_startedrawhoney":
                if (stage == HolyResearchStage.StartedRawHoney)
                {
                    if (!source.Inventory.HasCount("raw honey", 1))
                    {
                        source.SendOrangeBarMessage("Berteli sighs and looks away. You don't have any.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("raw honey", 1);
                    ExperienceDistributionScript.GiveExp(source, 8000);
                    source.Enums.Set(HolyResearchStage.None);
                    Subject.Close(source);
                }

                break;

            case "holyresearch_startedrawwax":
                if (stage == HolyResearchStage.StartedRawWax)
                {
                    if (!source.Inventory.HasCount("raw wax", 1))
                    {
                        source.SendOrangeBarMessage("Berteli sighs and looks away. You don't have any.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("raw wax", 1);
                    ExperienceDistributionScript.GiveExp(source, 8000);
                    source.Enums.Set(HolyResearchStage.None);
                    Subject.Close(source);
                }

                break;

            case "holyresearch_startedroyalwax":
                if (stage == HolyResearchStage.StartedRoyalWax)
                {
                    if (!source.Inventory.HasCount("royal wax", 1))
                    {
                        source.SendOrangeBarMessage("Berteli sighs and looks away. You don't have any.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("royal wax", 1);
                    ExperienceDistributionScript.GiveExp(source, 8000);
                    source.Enums.Set(HolyResearchStage.None);
                    Subject.Close(source);
                }

                break;
        }
    }
}