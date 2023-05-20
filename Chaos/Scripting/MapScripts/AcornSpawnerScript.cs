using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class AcornSpawnerScript : ItemSpawnerScript
{
    
    public override string ItemTemplateKey { get; set; } = "acorn";
    public override int MaxAmount { get; set; } = 30;
    public override int MaxPerSpawn { get; set; } = 8;
    public override int SpawnIntervalMs { get; set; } = 300000;
    
    public AcornSpawnerScript(MapInstance subject, IItemFactory itemFactory)
        : base(subject, itemFactory)
    {
        
    }
}