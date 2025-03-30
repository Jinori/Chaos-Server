using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Professions;

internal class AlchemyTrinketScript : DialogScriptBase
{
    private const string LEGENDMARK_KEY = "alch";

    private static readonly Dictionary<string, int> RankMappings = new()
    {
        {
            "Master Alchemist", 8
        },
        {
            "Expert Alchemist", 7
        },
        {
            "Advanced Alchemist", 6
        },
        {
            "Adept Alchemist", 5
        },
        {
            "Artisan Alchemist", 4
        },
        {
            "Initiate Alchemist", 3
        },
        {
            "Novice Alchemist", 2
        },
        {
            "Beginner Alchemist", 1
        }
    };

    private static readonly Dictionary<string, int> StatusMappings = new()
    {
        {
            "Master", 8
        },
        {
            "Expert", 7
        },
        {
            "Advanced", 6
        },
        {
            "Adept", 5
        },
        {
            "Artisan", 4
        },
        {
            "Initiate", 3
        },
        {
            "Basic", 2
        },
        {
            "Beginner", 1
        }
    };

    private readonly IEffectFactory EffectFactory;
    private readonly IItemFactory ItemFactory;

    private readonly ISimpleCache SimpleCache;

    private Animation AlcUseAnimation { get; } = new()
    {
        TargetAnimation = 653,
        AnimationSpeed = 100
    };

    /// <inheritdoc />
    public AlchemyTrinketScript(
        Dialog subject,
        IEffectFactory effectFactory,
        ISimpleCache simpleCache,
        IItemFactory itemFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        SimpleCache = simpleCache;
        ItemFactory = itemFactory;
    }

    private static void AddOption(Dialog subject, string optionName, string templateKey)
    {
        if (!subject.GetOptionIndex(optionName)
                    .HasValue)
            subject.AddOption(optionName, templateKey);
    }

    private static int GetRankAsInt(string rank) => RankMappings.TryGetValue(rank, out var value) ? value : -1;

    private static int GetStatusAsInt(string status) => StatusMappings.TryGetValue(status, out var value) ? value : 0;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "alchemytrinket_useongroup":
            {
                if (source.Group is null)
                {
                    source.SendActiveMessage("You are not currently in a group.");

                    return;
                }

                source.Trackers.Enums.TryGetValue(out AlchemyTrinketSelection potion);

                foreach (var person in source.Group)
                {
                    var newCraft = ItemFactory.Create(
                        potion.ToString()
                              .ToLower());
                    newCraft.Use(person);
                    person.Animate(AlcUseAnimation);
                    person.SendOrangeBarMessage($"{source.Name} shared a sip of Bialann Sruthmhor with you.");
                }

                Subject.InjectTextParameters(
                    potion.Humanize()
                          .Titleize());
                source.Trackers.TimedEvents.AddEvent("AlchemyTrinketUsage", TimeSpan.FromHours(3), true);

                break;
            }
            case "alchemytrinket_selfuse":
            {
                source.Trackers.Enums.TryGetValue(out AlchemyTrinketSelection potion);

                var newCraft = ItemFactory.Create(
                    potion.ToString()
                          .ToLower());
                newCraft.Use(source);
                source.Animate(AlcUseAnimation);

                Subject.InjectTextParameters(
                    potion.Humanize()
                          .Titleize());
                source.Trackers.TimedEvents.AddEvent("AlchemyTrinketUsage", TimeSpan.FromMinutes(10), true);

                break;
            }
            case "alchemytrinket_portaltolab":
            {
                var targetMap = SimpleCache.Get<MapInstance>("piet_alchemy_lab");
                source.TraverseMap(targetMap, new Point(8, 8));
                source.SendOrangeBarMessage("You take a sip and you feel matter displace you.");

                break;
            }
            case "alchemytrinket_userselectedapotion":
            {
                if (Subject.Context != null)
                {
                    Subject.InjectTextParameters(Subject.Context);

                    var cleanedContext = Subject.Context
                                                .ToString()
                                                ?.Replace(" ", "");

                    if (Enum.TryParse(cleanedContext, true, out AlchemyTrinketSelection value))
                    {
                        source.Trackers.Enums.Set(value);
                        source.Trackers.Enums.Set(AlchemyTrinketUsage.SelectedPotion);
                        source.Trackers.TimedEvents.AddEvent("AlchemyTrinketSetPotion", TimeSpan.FromHours(12), true);
                    }
                }

                break;
            }

            case "alchemytrinket_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("AlchemyTrinketUsage", out var @event))
                {
                    Subject.Reply(
                        source,
                        $"Alchemy bends the laws of nature, but even mastery has its cadence. The brew must settle before it flows again.\n                               {{=c    Try again in {
                            @event.Remaining.Humanize()}.");

                    return;
                }

                if (source.Trackers.TimedEvents.HasActiveEvent("AlchemyTrinketSetPotion", out var event1)
                    && source.Trackers.Enums.TryGetValue(out AlchemyTrinketSelection selection))
                {
                    AddOption(Subject, $"Use {selection.Humanize().Titleize()}", "alchemytrinket_useselected");

                    Subject.Text
                        = $"What is alchemy if not the art of defying limits? Choose your draught, master of the arcane, and drink without fear, for this vessel knows no end.\n                             {{=cYou can set a new potion in {
                            event1.Remaining.Humanize()}.";
                } else
                    AddOption(Subject, "Select A Potion", "alchemytrinket_selectapotion");

                break;
            }

            case "alchemytrinket_selectapotion":
            {
                if (!source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark))
                {
                    Subject.Reply(source, "You don't have the legend mark. Something went wrong.");

                    return;
                }

                var playerRank = GetRankAsInt(existingMark.Text);

                if (source.Trackers.Flags.TryGetFlag(out AlchemyRecipes recipes))
                    foreach ((var recipeKey, var recipeValue) in CraftingRequirements.AlchemyRequirements)
                    {
                        var requiredRank = GetStatusAsInt(recipeValue.Rank);

                        if (recipes.HasFlag(recipeKey) && (playerRank == requiredRank) && (source.UserStatSheet.Level >= recipeValue.Level))
                        {
                            var potionName = recipeValue.Name.Replace(" Formula", "");
                            AddOption(Subject, potionName, "alchemytrinket_userselectedapotion");
                        }
                    }

                break;
            }
        }
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Subject.Template.TemplateKey.Equals("alchemytrinket_selectapotion", StringComparison.OrdinalIgnoreCase))
            Subject.Context = Subject.GetOptionText(optionIndex!.Value);

        if (Subject.Template.TemplateKey.Equals("alchemytrinket_useonanother", StringComparison.CurrentCultureIgnoreCase))
        {
            if (!TryFetchArgs<string>(out var name))
            {
                Subject.ReplyToUnknownInput(source);

                return;
            }

            var aisling = source.MapInstance
                                .GetEntitiesWithinRange<Aisling>(source, 12)
                                .FirstOrDefault(x => x.Name == name);

            if (aisling != null)
            {
                source.Trackers.Enums.TryGetValue(out AlchemyTrinketSelection potion);

                var newCraft = ItemFactory.Create(
                    potion.ToString()
                          .ToLower());
                aisling.TryUseItem(newCraft);
                aisling.Animate(AlcUseAnimation);

                Subject.InjectTextParameters(
                    potion.Humanize()
                          .Titleize());
                source.SendOrangeBarMessage($"You share a sip of Bialann Sruthmhor with {aisling.Name}.");
                source.Trackers.TimedEvents.AddEvent("AlchemyTrinketUsage", TimeSpan.FromMinutes(60), true);
                aisling.SendOrangeBarMessage($"{source.Name} shared a sip of Bialann Sruthmhor with you.");
            } else
                source.SendActiveMessage("No aisling by that name can be sensed in your vicinity.");
        }
    }

    private static void RemoveOption(Dialog subject, string optionName)
    {
        if (subject.GetOptionIndex(optionName) is { } index)
            subject.Options.RemoveAt(index);
    }
}