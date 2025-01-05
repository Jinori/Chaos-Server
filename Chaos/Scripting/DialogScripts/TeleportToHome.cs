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
                var point = new Point(7, 3);
                var mapInstance = SimpleCache.Get<MapInstance>("rucesion_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Mileth:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Exile:
            {
                var point = new Point(7, 5);
                var mapInstance = SimpleCache.Get<MapInstance>("toc");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Suomi:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("suomi_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Loures:
            {
                var point = new Point(21, 16);
                var mapInstance = SimpleCache.Get<MapInstance>("loures_castle");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Tagor:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("tagor_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Piet:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("piet_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Abel:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("abel_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Labyrinth:
            {
                var point = new Point(6, 7);
                var mapInstance = SimpleCache.Get<MapInstance>("arena_entrance");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Undine:
            {
                var point = new Point(5, 11);
                var mapInstance = SimpleCache.Get<MapInstance>("undine_village_way");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
        }
    }
}