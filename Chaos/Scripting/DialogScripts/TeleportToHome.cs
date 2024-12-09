using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts;

public class TeleportToHome : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToHome(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source) => TeleportPlayerToHome(source);

    private void TeleportPlayerToHome(Aisling source)
    {
        switch (source.Nation)
        {
            case Nation.Rucesion:
            {
                var point = new Point(25, 36);
                var mapInstance = SimpleCache.Get<MapInstance>("rucesion");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Mileth:
            {
                var point = new Point(23, 18);
                var mapInstance = SimpleCache.Get<MapInstance>("mileth");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Exile:
            {
                var point = new Point(3, 13);
                var mapInstance = SimpleCache.Get<MapInstance>("mileth_village_way");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Suomi:
            {
                var point = new Point(16, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("suomi_village_way");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Loures:
            {
                var point = new Point(10, 6);
                var mapInstance = SimpleCache.Get<MapInstance>("mileth_village_way");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Tagor:
            {
                var point = new Point(22, 94);
                var mapInstance = SimpleCache.Get<MapInstance>("tagor");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Piet:
            {
                var point = new Point(16, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("piet_village_way");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Abel:
            {
                var point = new Point(11, 13);
                var mapInstance = SimpleCache.Get<MapInstance>("abel_port_way");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Void:
            {
                var point = new Point(12, 16);
                var mapInstance = SimpleCache.Get<MapInstance>("arena_entrance");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Undine:
            {
                var point = new Point(11, 12);
                var mapInstance = SimpleCache.Get<MapInstance>("undine_village_way");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
        }
    }
}