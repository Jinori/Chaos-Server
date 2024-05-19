using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public sealed class ArenaHostComponent : IComponent
{
    private readonly Point CenterWarp = new(11, 10);
    private Point CenterWarpPlayer;

    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Aisling>().ToList();
        var options = vars.GetOptions<IArenaHostComponentOptions>();

        foreach (var target in targets.ToList())
        {
            if (target.MapInstance.Template.TemplateKey != "arenaunderground")
                return; 
            
            switch (options.ArenaTeamColor)
            {
                case ArenaTeam.Blue:
                    target.Trackers.Enums.Set(ArenaTeam.Blue);
                    var rectBlue = new Rectangle(new Point(12, 4), 4, 3);
                    target.WarpTo(rectBlue.GetRandomPoint());
                    target.SendActiveMessage("You've been placed on the Blue team!");

                    break;
                case ArenaTeam.Green:
                    target.Trackers.Enums.Set(ArenaTeam.Green);
                    var rectGreen = new Rectangle(new Point(18, 10), 3, 4);
                    target.WarpTo(rectGreen.GetRandomPoint());
                    target.SendActiveMessage("You've been placed on the Green team!");

                    break;
                case ArenaTeam.Gold:
                    target.Trackers.Enums.Set(ArenaTeam.Gold);
                    var rectGold = new Rectangle(new Point(5, 10), 4, 3);
                    target.WarpTo(rectGold.GetRandomPoint());
                    target.SendActiveMessage("You've been placed on the Gold team!");

                    break;
                case ArenaTeam.Red:
                    target.Trackers.Enums.Set(ArenaTeam.Red);
                    var rectRed = new Rectangle(new Point(11, 17), 4, 3);
                    target.WarpTo(rectRed.GetRandomPoint());
                    target.SendActiveMessage("You've been placed on the Red team!");

                    break;
                case ArenaTeam.None:
                    var rect = new Rectangle(new Point(11, 10), 3, 4);

                    do
                        CenterWarpPlayer = rect.GetRandomPoint();
                    while (CenterWarp == CenterWarpPlayer);

                    target.Trackers.Enums.Remove<ArenaTeam>();
                    target.WarpTo(CenterWarpPlayer);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public interface IArenaHostComponentOptions
    {
        ArenaTeam ArenaTeamColor { get; init; }
    }
}