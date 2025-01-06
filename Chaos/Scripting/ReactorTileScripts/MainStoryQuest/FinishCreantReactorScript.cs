using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.MainStoryLine;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class FinishCreantReactorScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public FinishCreantReactorScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;


        var mapScript = Subject.MapInstance.Script.As<CreantBossMapScript>();

        if (mapScript == null)
            return;

        if (mapScript.State != CreantBossMapScript.ScriptState.CreantKilled)
        {
            aisling.SendOrangeBarMessage("The Creant is not ready to be sealed.");

            return;
        }
        
        var point1 = new Point(source.X, source.Y);
        var blankmerchant1 = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point1);
        var dialog1 = DialogFactory.Create("creant_reward_initial", blankmerchant1);
        dialog1.Display(aisling);
    }
}