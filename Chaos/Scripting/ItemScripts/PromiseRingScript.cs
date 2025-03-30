using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class PromiseRingScript(Item subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory) : ItemScriptBase(subject)
{
    private static bool AreFacingEachOther(Aisling source, Aisling target)
    {
        var sourceDirectionToTarget = source.DirectionalRelationTo(target);
        var targetDirectionToSource = target.DirectionalRelationTo(source);

        return (source.Direction == targetDirectionToSource) && (target.Direction == sourceDirectionToTarget);
    }

    private static Aisling? FindTargetPlayer(Aisling source)
        => source.MapInstance
                 .GetEntitiesWithinRange<Aisling>(source, 1)
                 .FirstOrDefault(target => source.Trackers.Counters.ContainsKey($"ValentinesPromise[{target.Name.ToLower()}]"));

    private void InitiateRingGivingDialog(Aisling source, Aisling targetPlayer)
    {
        var merchant = merchantFactory.Create("cueti", source.MapInstance, source);
        var dialog = dialogFactory.Create("cueti_givering", merchant);

        dialog.InjectTextParameters(source.Name);
        dialog.Display(targetPlayer);
    }

    public override void OnUse(Aisling source)
    {
        var targetPlayer = FindTargetPlayer(source);

        if (targetPlayer == null)
        {
            source.SendMessage("Where is your lover? They must be infront of you.");

            return;
        }

        if (!AreFacingEachOther(source, targetPlayer))
        {
            source.SendMessage("You must be facing each other to use the promise ring.");

            return;
        }

        InitiateRingGivingDialog(source, targetPlayer);
    }
}