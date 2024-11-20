using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class BlackRoseScript : ItemScriptBase
{
    private readonly ISimpleCache _simpleCache;
    private readonly IItemFactory ItemFactory;
    private readonly IDialogFactory DialogFactory;

    public BlackRoseScript(Item subject, ISimpleCache simpleCache, IItemFactory itemFactory,
        IDialogFactory dialogFactory
    )
        : base(subject)
    {
        _simpleCache = simpleCache;
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    private bool CanDream(Aisling source, out MapInstance mapInstance)
    {
        mapInstance = _simpleCache.Get<MapInstance>("mehadi_heart_east");
        source.Trackers.Enums.TryGetValue(out NightmareQuestStage stage);
        
        if (stage is not (NightmareQuestStage.Started or NightmareQuestStage.MetRequirementsToEnter1 or NightmareQuestStage.EnteredDream or NightmareQuestStage.SpawnedNightmare))
            return false;

        if (!source.IsAlive || !source.MapInstance.Name.EqualsI(mapInstance.Name))
            return false;

        return true;
    }

    private void Dream(Aisling source)
    {
        source.SendOrangeBarMessage("The Black Rose causes you to hallucinate.");
        var item = ItemFactory.Create("blackrose");
        var classDialog = DialogFactory.Create("nightmareblackrose1", item);
        classDialog.Display(source);
    }
    
    public override void OnUse(Aisling source)
    {
        if (CanDream(source, out var mapInstance))
        {
            Dream(source);
            return;
        }
        
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "This would surely make you sick to consume.");
    }
}