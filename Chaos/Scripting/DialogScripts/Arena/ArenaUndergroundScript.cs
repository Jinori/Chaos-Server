using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaUndergroundScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public ArenaUndergroundScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public void HideDialogOptions(Aisling source) => source.Trackers.Enums.TryGetValue(out ArenaHost stage);

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ophie_initial":
                HideDialogOptions(source);

                break;

            case "ophie_battlering":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
                source.TraverseMap(mapInstance, new Point(13, 13));
                Subject.Close(source);

                break;
            }

            case "ophie_revive":
            {
                Subject.Close(source);

                if (!source.IsAlive)
                {
                    source.IsDead = false;
                    source.StatSheet.SetHealthPct(25);
                    source.StatSheet.SetManaPct(25);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendActiveMessage("Ophie has revived you.");
                    source.Refresh();
                }

                break;
            }

            case "ophie_leave":
            {
                Subject.Close(source);
                var worldMap = SimpleCache.Get<WorldMap>("field001");

                //if we cant set the active object, return
                if (!source.ActiveObject.SetIfNull(worldMap))
                    return;

                source.MapInstance.RemoveObject(source);
                source.Client.SendWorldMap(worldMap);

                break;
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ophie_placeonredconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Red);
                aisling?.WarpTo(new Point(6, 6));
                aisling?.SendActiveMessage("You've been placed on the Red team!");

                break;
            }
            case "ophie_placeongreenconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Green);
                aisling?.WarpTo(new Point(15, 6));
                aisling?.SendActiveMessage("You've been placed on the Green team!");

                break;
            }
            case "ophie_placeongoldconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Gold);
                aisling?.WarpTo(new Point(15, 15));
                aisling?.SendActiveMessage("You've been placed on the Gold team!");

                break;
            }
            case "ophie_placeonblueconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Blue);
                aisling?.WarpTo(new Point(6, 15));
                aisling?.SendActiveMessage("You've been placed on the Blue team!");

                break;
            }
        }
    }

    //if ((stage != ArenaHost.Host) && (stage != ArenaHost.MasterHost))
    //  RemoveOption(Subject, "Host Options");   
    private void RemoveOption(Dialog subject, string optionName)
    {
        if (subject.GetOptionIndex(optionName).HasValue)
        {
            var s = subject.GetOptionIndex(optionName)!.Value;
            subject.Options.RemoveAt(s);
        }
    }
}