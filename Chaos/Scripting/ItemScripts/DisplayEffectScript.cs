using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class DisplayEffectScript : ConfigurableItemScriptBase
{
    protected Animation? Animation { get; init; }
    protected BodyAnimation? BodyAnimation { get; init; }
    protected byte? Sound { get; init; }

    public DisplayEffectScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        if (source.IsAlive)
        {
            if (Animation != null)
                source.MapInstance.ShowAnimation(Animation.GetTargetedAnimation(source.Id));

            if (Sound.HasValue)
                source.MapInstance.PlaySound(Sound.Value, source);

            if (BodyAnimation.HasValue)
                source.AnimateBody(BodyAnimation.Value);

            source.Inventory.RemoveQuantity(Subject.DisplayName, 1, out _);
        }
    }
}