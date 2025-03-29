using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class UndineChickenEggsScript : ItemScriptBase
{
    /// <inheritdoc />
    public UndineChickenEggsScript(Item subject)
        : base(subject) { }

    /// <inheritdoc />
    public override bool CanBePickedUp(Aisling source, Point sourcePoint) => false;
}