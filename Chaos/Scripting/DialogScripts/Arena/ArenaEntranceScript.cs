using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaEntranceScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private readonly IDialogFactory DialogFactory;
    private readonly IShardGenerator ShardGenerator;

    /// <inheritdoc />
    public ArenaEntranceScript(Dialog subject, ISimpleCache simpleCache, IDialogFactory dialogFactory, IShardGenerator shardGenerator)
        : base(subject)
    {
        ShardGenerator = shardGenerator;
        DialogFactory = dialogFactory;
        SimpleCache = simpleCache;
    }


    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "rlyeh_pitfightconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(1, out var name))
                {
                    Subject.ReplyToUnknownInput(source);
                    return;
                }
                
                var aislingChallenged = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x =>
                    string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
                
                if ((aislingChallenged == null) || !aislingChallenged.OnSameMapAs(source))
                {
                    source.SendOrangeBarMessage("It does not look like they are here.");
                    Subject.Close(source);
                    return;
                }
                
                switch (optionIndex)
                {
                    case 1:
                    {
                        var dialog = DialogFactory.Create("rlyeh_denypitfight", Subject.DialogSource);
                        dialog.InjectTextParameters(source.Name);
                        dialog.Display(aislingChallenged);
                        Subject.Reply(source, $"You have denied {aislingChallenged.Name}'s challenge to a pit fight.");
                        aislingChallenged.Trackers.TimedEvents.AddEvent($"{source.Name.ToLower()}" + "denyPitFight", TimeSpan.FromMinutes(5), true);
                        break;
                    }
                    case 2:
                    {
                        var shard = ShardGenerator.CreateShardOfInstance("arena_pitfight");
                        shard.Shards.TryAdd(shard.InstanceId, shard);
 
                        source.TraverseMap(shard, new Point(3, 18));
                        source.SendOrangeBarMessage($"You accepted {aislingChallenged.Name}'s challenge. Get ready!");
                        aislingChallenged.SendOrangeBarMessage($"{source.Name} accepted your challenge. Get ready!");
                        aislingChallenged.TraverseMap(shard, new Point(17, 3));
                        Subject.Close(source);
                        break;
                    }
                }

                break;
            }
            case "rlyeh_pitfightstart":
            {
                if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
                {
                    Subject.ReplyToUnknownInput(source);
                    return;
                }

                var aislingToChallenge = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x =>
                    string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

                if (aislingToChallenge?.Name == source.Name)
                {
                    source.SendOrangeBarMessage("You poke yourself in the eye. Challenge accepted.");
                    source.AnimateBody(BodyAnimation.Ouch);
                    Subject.Close(source);
                    return;
                }
                
                if (Equals(aislingToChallenge?.Client.RemoteIp, source.Client.RemoteIp))
                {
                    source.SendOrangeBarMessage($"You cannot challenge yourself to a pit fight. (( Same IP ))");
                    Subject.Close(source);
                    return;
                }
                
                if ((aislingToChallenge == null) || !aislingToChallenge.OnSameMapAs(source))
                {
                    source.SendOrangeBarMessage("It does not look like they are here.");
                    Subject.Close(source);
                    return;
                }

                if (source.Trackers.TimedEvents.HasActiveEvent($"{aislingToChallenge.Name.ToLower()}_pitfight", out var @pitfight))
                {
                    Subject.Reply(source, $"You can challenge {aislingToChallenge.Name} in {@pitfight.Remaining.Humanize()}. You previously fought them in a pit fight.");
                    return;
                }
                
                if (source.Trackers.TimedEvents.HasActiveEvent($"{aislingToChallenge.Name.ToLower()}denyPitFight", out var @event))
                {
                   Subject.Reply(source, $"You can challenge {aislingToChallenge.Name} in {@event.Remaining.Humanize()}. They denied you previously.");
                   return;
                }
                
                if (!aislingToChallenge.WithinLevelRange(source))
                {
                    Subject.Reply(source,$"{aislingToChallenge.Name} is not in your level range. Pit fights must be balanced." );
                    return;
                }
                
                var dialog = DialogFactory.Create("rlyeh_pitfightconfirm", Subject.DialogSource);
                dialog.MenuArgs = Subject.MenuArgs;
                dialog.MenuArgs.Add(source.Name);
                dialog.InjectTextParameters(source.Name);
                dialog.Display(aislingToChallenge);
                Subject.Close(source);
                break;
            }
        }
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "rlyeh_revive":
            {
                Subject.Close(source);

                if (!source.IsAlive)
                {
                    source.IsDead = false;
                    source.StatSheet.SetHealthPct(25);
                    source.StatSheet.SetManaPct(25);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendActiveMessage("R'lyeh has revived you.");
                    source.Refresh();
                    source.Display();
                }

                break;
            }
            
            case "rlyeh_hostedstaging":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
                source.TraverseMap(mapInstance, new Point(12, 10));
                Subject.Close(source);

                break;
            }
            
            case "rlyeh_battlering":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
                source.TraverseMap(mapInstance, new Point(13, 13));
                Subject.Close(source);
                break;
            }
            
            case "rlyeh_leave":
            {
                Subject.Close(source);
                var worldMap = SimpleCache.Get<WorldMap>("field001");

                //if we cant set the active object, return
                if (!source.ActiveObject.SetIfNull(worldMap))
                    return;

                source.MapInstance.RemoveEntity(source);
                source.Client.SendWorldMap(worldMap);

                break;
            }
        }
    }
}