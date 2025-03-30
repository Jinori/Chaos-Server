using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Quests.PFQuest;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Events.Christmas;

public class ScChallengeScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public ScChallengeScript(
        Dialog subject,
        ISimpleCache simpleCache,
        ILogger<PFQuestScript> logger,
        IItemFactory itemFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        Logger = logger;
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    private List<Aisling>? GetNearbyGroupMembers(Aisling source)
        => source.Group
                 ?.Where(x => x.WithinRange(new Point(source.X, source.Y)))
                 .ToList();

    private bool IsGroupValid(Aisling source)
        => (source.Group != null) && !source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source));

    private bool IsGroupWithinLevelRange(Aisling source, List<Aisling> group)
        => group.All(member => member.WithinLevelRange(source) && member.UserStatSheet.Master);

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "elf4_initial":
            {
                if (!source.UserStatSheet.Master)
                    return;

                var option1 = new DialogOption
                {
                    DialogKey = "scchallenge_initial",
                    OptionText = "Santa's Challenge."
                };

                if (!Subject.HasOption(option1.OptionText))
                    Subject.Options.Insert(0, option1);
            }

                break;

            case "scchallenge_initial4":
            {
                var group = GetNearbyGroupMembers(source);

                if (group == null)
                {
                    Subject.Reply(source, "Santa's instructions says you must have a group to enter the challenge! No exceptions.");

                    return;
                }

                if (!IsGroupValid(source))
                {
                    SendGroupInvalidMessage(source);

                    return;
                }

                if (!IsGroupWithinLevelRange(source, group))
                {
                    SendLevelRangeInvalidMessage(source);

                    return;
                }

                TeleportGrouptoChallenge(source, group);

                break;
            }
        }
    }

    private void SendGroupInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "All of your group members are not here.");
        Subject.Reply(source, "Some of your companions are not here.");
    }

    private void SendLevelRangeInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "All of your group members must be masters.");
        Subject.Reply(source, "Some of your companions are not masters.");
    }

    private void TeleportGrouptoChallenge(Aisling source, List<Aisling> group)
    {
        var mapInstance = SimpleCache.Get<MapInstance>("santachallenge");

        foreach (var member in group)
        {
            var rectangle = new Rectangle(
                15,
                15,
                5,
                5);
            Point point;

            do
                point = rectangle.GetRandomPoint();
            while (!mapInstance.IsWalkable(point, collisionType: member.Type));

            member.Trackers.Counters.Set("scwave", 1);

            var dialog = member.ActiveDialog.Get();
            dialog?.Close(member);
            Subject.Close(source);
            member.TraverseMap(mapInstance, point);
        }
    }
}