using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.ReactorTileScripts.Creants.Shamensyth;

namespace Chaos.Scripting.Components.AbilityComponents;

public sealed class RemoveShamBurningGroundComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        if (!ShouldActivate(context, vars))
            return;

        var pointsForStage = vars.GetPoints();

        var shamBurningGrounds = context.TargetMap
                                        .GetEntitiesAtPoints<ReactorTile>(pointsForStage)
                                        .Where(rt => rt.Script.Is<BurningGroundScript>())
                                        .ToList();

        foreach (var bg in shamBurningGrounds)
            context.TargetMap.RemoveEntity(bg);
    }

    private bool ShouldActivate(ActivationContext context, ComponentVars vars)
    {
        if (!context.TargetMap.LoadedFromInstanceId.EqualsI("shamensythbossroom"))
            return false;

        var sourceScript = vars.GetSourceScript();

        return ShouldActivateForScript(sourceScript);
    }

    private bool ShouldActivateForScript(IScript sourceScript)
    {
        switch (sourceScript)
        {
            case SubjectiveScriptBase<Spell> spellScript:
                var templateKey = spellScript.Subject.Template.TemplateKey;

                if (templateKey.EqualsI("tidalbreeze")
                    || templateKey.EqualsI("tidalwave")
                    || templateKey.ContainsI("ardsal")
                    || templateKey.ContainsI("morsal"))
                    return true;

                break;
            case SubjectiveScriptBase<Skill> skillScript:
                break;
            case SubjectiveScriptBase<ReactorTile> reactorTileScript:
                // ReSharper disable once TailRecursiveCall
                if (reactorTileScript.Subject.SourceScript is not null)
                    return ShouldActivateForScript(reactorTileScript.Subject.SourceScript);

                break;
        }

        return false;
    }
}