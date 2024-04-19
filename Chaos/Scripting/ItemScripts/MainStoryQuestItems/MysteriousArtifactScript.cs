using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
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
        { AnimationSpeed = 100, TargetAnimation = 160 };
    

    public override void OnUse(Aisling source)
    {
        source.SendOrangeBarMessage("You hold the artifact tight.");
        source.Animate(GetPlayerAnimation());
        var mysteriousartifact = ItemFactory.Create("mysteriousartifact");
        var mysteriousartifactdialog = DialogFactory.Create("mysteriousartifact_initial1", mysteriousartifact);
        mysteriousartifactdialog.Display(source);
    }
}