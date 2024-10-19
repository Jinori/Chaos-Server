using System.Collections.Frozen;
using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class GlobalDefaultMapScript : MapScriptBase
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly IReactorTileFactory ReactorTileFactory;
    private static readonly FrozenSet<ushort> HEMP_TILE_IDS = new ushort[] { 755, 756 }.ToFrozenSet();
    
    public GlobalDefaultMapScript(MapInstance subject, IReactorTileFactory reactorTileFactory) : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        
        for(var x = 0; x <= Subject.Template.Tiles.GetUpperBound(0); x++)
            for (var y = 0; y <= Subject.Template.Tiles.GetUpperBound(1); y++)
            {
                var tile = Subject.Template.Tiles[x, y];
                var point = new Point(x, y);
            
                // ReSharper disable once InvertIf
                if (HEMP_TILE_IDS.Contains(tile.LeftForeground) && HEMP_TILE_IDS.Contains(tile.RightForeground))
                {
                    var hempReactor = ReactorTileFactory.Create("hemp", Subject, point);
                
                    Subject.SimpleAdd(hempReactor);
                }
            }
    }
}