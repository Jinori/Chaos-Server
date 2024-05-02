using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts.MainStoryQuestItems;

public class MysteriousArtifactScript : ItemScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IItemFactory ItemFactory;
    private readonly IDialogFactory DialogFactory;

    public MysteriousArtifactScript(Item subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        MonsterFactory = monsterFactory;
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    private Animation GetPlayerAnimation() => new()
        { AnimationSpeed = 100, TargetAnimation = 22 };
    

    public override void OnUse(Aisling source)
    {
        var mysteriousartifact = ItemFactory.Create("mysteriousartifact");
        var mysteriousartifactdialog = DialogFactory.Create("mysteriousartifact_initial1", mysteriousartifact);
        mysteriousartifactdialog.Display(source);


        if (source.MapInstance.Name != "The God's Realm")
        {
            source.Animate(GetPlayerAnimation());
            source.SendOrangeBarMessage("You hold the artifact tight.");
            source.Animate(GetPlayerAnimation());
        }
    }
}