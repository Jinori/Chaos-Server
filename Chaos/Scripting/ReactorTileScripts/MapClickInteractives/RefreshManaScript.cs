using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MapClickInteractives;

public sealed class RefreshManaScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public RefreshManaScript(ReactorTile subject)
        : base(subject) {}

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {   
        if (!source.WithinRange(Subject, 3))
            return;
        
        var ani = new Animation
        {
            TargetAnimation = 58,
            AnimationSpeed = 100
        };
        
        source.SendOrangeBarMessage("You scoop some water and splash your face feeling refreshed.");
        source.UserStatSheet.SetManaPct(100);
        source.Animate(ani);
        source.Client.SendAttributes(StatUpdateType.Vitality);
    }
}