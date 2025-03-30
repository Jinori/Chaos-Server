using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Events.StPatricks;

public class LuckyCharmsScript : DialogScriptBase
{
    private readonly List<string> CharmList =
    [
        "bluemooncharm",
        "clovercharm",
        "heartcharm",
        "horseshoecharm",
        "potofgoldcharm",
        "rainbowcharm",
        "redballooncharm",
        "starcharm"
    ];

    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public LuckyCharmsScript(Dialog subject, IItemFactory itemFactory, IClientRegistry<IChaosWorldClient> clientRegistry)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ClientRegistry = clientRegistry;
    }

    private void AddOption(Dialog subject, string optionName, string templateKey)
    {
        if (!subject.GetOptionIndex(optionName)
                    .HasValue)
            subject.AddOption(optionName, templateKey);
    }

    /// <summary>
    ///     Checks if the player has all required charms in their inventory.
    /// </summary>
    private bool HasAllCharms(Aisling source) => CharmList.All(charm => source.Inventory.HasCountByTemplateKey(charm, 1));

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "lucky_initial":
            {
                source.Trackers.Enums.TryGetValue(out LuckyCharmsQuest quest);

                switch (quest)
                {
                    case LuckyCharmsQuest.TurnedIn:
                        Subject.Reply(source, "Thank ye for returning all of my charms aislin'!");

                        return;

                    case LuckyCharmsQuest.Accepted:
                        RemoveOption(Subject, "What charms are you missing?");
                        RemoveOption(Subject, "Sounds like a personal problem...");
                        AddOption(Subject, "I've found all of the charms!", "lucky_return");
                        Subject.Text = "Ah-ha! Back so soon, are ye? Tell me now, did ye find me precious charms?";

                        break;
                }

                break;
            }

            case "lucky_accept":
            {
                source.Trackers.Enums.Set(LuckyCharmsQuest.Accepted);
                source.SendActiveMessage("You've accepted Lucky's quest to find his charms!");

                break;
            }

            case "lucky_return":
            {
                if (HasAllCharms(source))
                {
                    RemoveCharmsFromInventory(source);
                    source.Trackers.Enums.Set(LuckyCharmsQuest.TurnedIn);
                    var item = ItemFactory.Create("fannypouch");
                    source.GiveItemOrSendToBank(item);

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Found Lucky's Charms",
                            "luckycharms",
                            MarkIcon.Yay,
                            MarkColor.LightGreen,
                            1,
                            GameTime.Now));

                    foreach (var aisling in ClientRegistry)
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"{source.Name} has found all eight of Lucky's Charms!");

                    source.SendActiveMessage("You've recieved a statted Fanny Pouch for helping Lucky!");
                } else
                    Subject.Reply(source, "Eh? Ye tryin' to trick ol' Lucky? Ye don't have all me charms yet!");

                break;
            }
        }
    }

    /// <summary>
    ///     Removes the charms from the player's inventory once they complete the quest.
    /// </summary>
    private void RemoveCharmsFromInventory(Aisling source)
    {
        foreach (var charm in CharmList)
            source.Inventory.TryGetRemoveByTemplateKey(charm, out _);
    }

    private void RemoveOption(Dialog subject, string optionName)
    {
        if (subject.GetOptionIndex(optionName)
                   .HasValue)
        {
            var s = subject.GetOptionIndex(optionName)!.Value;
            subject.Options.RemoveAt(s);
        }
    }
}