using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.DialogScripts.Quests;

public class ALittleBitofThatScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public ALittleBitofThatScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Enums.TryGetValue(out ALittleBitofThatStage stage);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = Convert.ToInt32(.20 * tnl);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "egil_initial":
            case "appie_initial":
            case "matei_initial":
            {
                if (source.UserStatSheet.Level < 21)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_initial",
                        OptionText = "A Little Bit of That"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }
            }

                break;

            case "alittlebitofthat_initial":
                if (!hasStage || (stage == ALittleBitofThatStage.None))
                {
                    if (source.TimedEvents.TryGetNearestToCompletion(TimedEvent.TimedEventId.ALittleBitofThatCd, out var timedEvent))
                    {
                        Subject.Text = $"I'm still working on this, come back later. (({timedEvent.Remaining.ToReadableString()}))";

                        return;
                    }

                    var option = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_yes",
                        OptionText = "What can I do to help?"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_no",
                        OptionText = "Can't do. Good luck!"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);
                }

                if (stage == ALittleBitofThatStage.StartedApple)
                {
                    Subject.Text = "Oh, back so soon? Did you get it?";

                    var option = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_apple",
                        OptionText = "I found you an Apple."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_where",
                        OptionText = "Where do you expect me to look?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedBaguette)
                {
                    Subject.Text = "Oh, back so soon? Did you get it?";

                    var option = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_baguette",
                        OptionText = "I found you some Baguette."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_where",
                        OptionText = "Where do you expect me to look?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedCherry)
                {
                    Subject.Text = "Oh, back so soon? Did you get it?";

                    var option = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_cherry",
                        OptionText = "I found you some Cherries."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_where",
                        OptionText = "Where do you expect me to look?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedGrapes)
                {
                    Subject.Text = "Oh, back so soon? Did you get it?";

                    var option = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_grapes",
                        OptionText = "I found you some Grapes."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_where",
                        OptionText = "Where do you expect me to look?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedMold)
                {
                    Subject.Text = "Oh, back so soon? Did you get it?";

                    var option = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_mold",
                        OptionText = "I found you some Mold."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_where",
                        OptionText = "Where do you expect me to look?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedTomato)
                {
                    Subject.Text = "Oh, back so soon? Did you get it?";

                    var option = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_tomato",
                        OptionText = "I found you a Tomato."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_where",
                        OptionText = "Where do you expect me to look?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);
                }

                break;

            case "alittlebitofthat_yes":
                if (!hasStage || (stage == ALittleBitofThatStage.None))
                {
                    var randomALittleBitofThatStage = new[]
                    {
                        ALittleBitofThatStage.StartedApple,
                        ALittleBitofThatStage.StartedBaguette,
                        ALittleBitofThatStage.StartedCherry,
                        ALittleBitofThatStage.StartedGrapes,
                        ALittleBitofThatStage.StartedMold,
                        ALittleBitofThatStage.StartedTomato
                    }.PickRandom();

                    source.Enums.Set(randomALittleBitofThatStage);

                    switch (randomALittleBitofThatStage)
                    {
                        case ALittleBitofThatStage.StartedApple:
                        {
                            Subject.Text = "I really appreciate your help Aisling, please bring me five Apples.";
                        }

                            break;

                        case ALittleBitofThatStage.StartedBaguette:
                        {
                            Subject.Text = "I really appreciate your help Aisling, please bring me three Baguette.";
                        }

                            break;

                        case ALittleBitofThatStage.StartedCherry:
                        {
                            Subject.Text = "I really appreciate your help Aisling, please bring me ten Cherries.";
                        }

                            break;
                        case ALittleBitofThatStage.StartedMold:
                        {
                            Subject.Text = "I really appreciate your help Aisling, please bring me three Mold.";
                        }

                            break;

                        case ALittleBitofThatStage.StartedTomato:
                        {
                            Subject.Text = "I really appreciate your help Aisling, please bring me a Tomato.";
                        }

                            break;
                        case ALittleBitofThatStage.StartedGrapes:
                        {
                            Subject.Text = "I really appreciate your help Aisling, please bring me ten Grapes.";
                        }

                            break;
                    }
                }

                break;

            case "alittlebitofthat_apple":

                if (stage == ALittleBitofThatStage.StartedApple)
                {
                    if (!source.Inventory.HasCount("Apple", 5))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some apples, you remember he needs five.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("Apple", 5);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.ALittleBitofThatCd, TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_baguette":
                if (stage == ALittleBitofThatStage.StartedBaguette)
                {
                    if (!source.Inventory.HasCount("Baguette", 3))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some baguettes, you remember he needs three.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("baguette", 3);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.ALittleBitofThatCd, TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_cherry":
                if (stage == ALittleBitofThatStage.StartedCherry)
                {
                    if (!source.Inventory.HasCount("Cherry", 10))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some cherries, you remember he needs ten.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("Cherry", 10);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.ALittleBitofThatCd, TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_grape":
                if (stage == ALittleBitofThatStage.StartedGrapes)
                {
                    if (!source.Inventory.HasCount("Grape", 10))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some grapes, you remember he needs ten.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("Grape", 10);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.ALittleBitofThatCd, TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_mold":
                if (stage == ALittleBitofThatStage.StartedMold)
                {
                    if (!source.Inventory.HasCount("Mold", 3))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some mold, you remember he needs three.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("Mold", 3);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.ALittleBitofThatCd, TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_tomato":
                if (stage == ALittleBitofThatStage.StartedTomato)
                {
                    if (!source.Inventory.HasCount("Tomato", 1))
                    {
                        source.SendOrangeBarMessage("You seem to be missing a Tomato.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("Tomato", 1);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.ALittleBitofThatCd, TimeSpan.FromHours(2), true);
                }

                break;
        }
    }
}