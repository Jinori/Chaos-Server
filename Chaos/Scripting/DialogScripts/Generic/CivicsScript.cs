using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class CivicsScript : DialogScriptBase
{
    public CivicsScript(Dialog subject)
        : base(subject) { }

    private void HandleBecomeCitizen(Aisling source, Nation nation)
    {
        if ((source.Nation != Nation.Exile) && (source.Nation != nation))
        {
            source.SendOrangeBarMessage("You are already of another Nation. Begone.");
            Subject.Close(source);
        }

        if ((source.Nation != Nation.Exile) && (source.Nation == nation))
        {
            source.SendOrangeBarMessage("Citizen? You already have the seal upon your legend.");
            Subject.Close(source);
        }

        if (source.Nation == Nation.Exile)
        {
            var animation = new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 78
            };

            source.Nation = nation;
            source.SendOrangeBarMessage($"You become a {nation} Citizen. Hooray!");
            source.Animate(animation, source.Id);
        }
    }

    private void HandleMilethCivics(Aisling source)
    {
        if ((source.Nation != Nation.Exile) && Subject.GetOptionIndex("Become Mileth Citizen") is { } optionIndex)
            Subject.Options.RemoveAt(optionIndex);
    }

    private void HandleRenounceCitizenship(Aisling source, Nation nation)
    {
        if (source.Nation != nation)
        {
            source.SendOrangeBarMessage("You are of a different Nation. Begone.");
            Subject.Close(source);
        }

        if (source.Nation == nation)
        {
            source.Nation = Nation.Exile;
            source.SendOrangeBarMessage("You renounce your citizenship and turn to Exile.");
        }
    }

    private void HandleRucesionCivics(Aisling source)
    {
        if ((source.Nation != Nation.Exile) && Subject.GetOptionIndex("Become Rucesion Citizen") is { } optionIndex)
            Subject.Options.RemoveAt(optionIndex);
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            // Rucesion
            case "angelo_civics":
                HandleRucesionCivics(source);

                break;

            case "angelo_renounceyes":
                HandleRenounceCitizenship(source, Nation.Rucesion);

                break;

            case "angelo_becomecitizenyes":
                HandleBecomeCitizen(source, Nation.Rucesion);

                break;

            // Mileth
            case "aingeal_civics":
                HandleMilethCivics(source);

                break;

            case "aingeal_renounceyes":
                HandleRenounceCitizenship(source, Nation.Mileth);

                break;

            case "aingeal_becomecitizenyes":
                HandleBecomeCitizen(source, Nation.Mileth);

                break;
        }
    }
}