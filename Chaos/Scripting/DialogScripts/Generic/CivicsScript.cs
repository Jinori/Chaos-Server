using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class CivicsScript : DialogScriptBase
{
    private readonly Dictionary<string, Action<Aisling>> TemplateActions;

    public CivicsScript(Dialog subject) : base(subject) =>
        TemplateActions = new Dictionary<string, Action<Aisling>>(StringComparer.OrdinalIgnoreCase)
        {
            // Void / Arena
            {"rlyeh_civics", source => HandleCivicOptions(source, "Become Drowned Labyrinth Citizen")},
            {"rlyeh_renounceyes", source => HandleRenounceCitizenship(source, Nation.Labyrinth)},
            {"rlyeh_becomecitizenyes", source => HandleBecomeCitizen(source, Nation.Labyrinth)},
            
            // Undine
            {"ayumi_civics", source => HandleCivicOptions(source, "Become Undine Citizen")},
            {"ayumi_renounceyes", source => HandleRenounceCitizenship(source, Nation.Undine)},
            {"ayumi_becomecitizenyes", source => HandleBecomeCitizen(source, Nation.Undine)},
            
            // Abel
            {"runa_civics", source => HandleCivicOptions(source, "Become Abel Citizen")},
            {"runa_renounceyes", source => HandleRenounceCitizenship(source, Nation.Abel)},
            {"runa_becomecitizenyes", source => HandleBecomeCitizen(source, Nation.Abel)},

            // Tagor
            {"dorina_civics", source => HandleCivicOptions(source, "Become Tagor Citizen")},
            {"dorina_renounceyes", source => HandleRenounceCitizenship(source, Nation.Tagor)},
            {"dorina_becomecitizenyes", source => HandleBecomeCitizen(source, Nation.Tagor)},

            // Piet
            {"saskia_civics", source => HandleCivicOptions(source, "Become Piet Citizen")},
            {"saskia_renounceyes", source => HandleRenounceCitizenship(source, Nation.Piet)},
            {"saskia_becomecitizenyes", source => HandleBecomeCitizen(source, Nation.Piet)},

            // Suomi
            {"eeva_civics", source => HandleCivicOptions(source, "Become Suomi Citizen")},
            {"eeva_renounceyes", source => HandleRenounceCitizenship(source, Nation.Suomi)},
            {"eeva_becomecitizenyes", source => HandleBecomeCitizen(source, Nation.Suomi)},

            // Rucesion
            {"angelo_civics", source => HandleCivicOptions(source, "Become Rucesion Citizen")},
            {"angelo_renounceyes", source => HandleRenounceCitizenship(source, Nation.Rucesion)},
            {"angelo_becomecitizenyes", source => HandleBecomeCitizen(source, Nation.Rucesion)},

            // Mileth
            {"aingeal_civics", source => HandleCivicOptions(source, "Become Mileth Citizen")},
            {"aingeal_renounceyes", source => HandleRenounceCitizenship(source, Nation.Mileth)},
            {"aingeal_becomecitizenyes", source => HandleBecomeCitizen(source, Nation.Mileth)},
        };

    private void HandleBecomeCitizen(Aisling source, Nation nation)
    {
        if (source.Nation == nation)
        {
            source.SendOrangeBarMessage("Citizen? You already have the seal upon your legend.");
            Subject.Close(source);
            return;
        }
        
        if (source.Nation != Nation.Exile)
        {
            source.SendOrangeBarMessage("You are already of another Nation. Begone.");
            Subject.Close(source);
            return;
        }

        source.Nation = nation;
        source.SendOrangeBarMessage($"You become a {nation} Citizen.");
        source.Animate(new Animation { AnimationSpeed = 100, TargetAnimation = 78 }, source.Id);
    }

    private void HandleRenounceCitizenship(Aisling source, Nation nation)
    {
        if (source.Nation != nation)
        {
            source.SendOrangeBarMessage("You are of a different Nation. Begone.");
            Subject.Close(source);
        }
        else
        {
            source.Nation = Nation.Exile;
            source.SendOrangeBarMessage("You renounce your citizenship and turn to Exile.");
        }
    }

    private void HandleCivicOptions(Aisling source, string optionName)
    {
        if (source.Nation != Nation.Exile)
        {
            var optionIndex = Subject.GetOptionIndex(optionName);
            if (optionIndex.HasValue)
                Subject.Options.RemoveAt(optionIndex.Value);
        }
    }

    public override void OnDisplaying(Aisling source)
    {
        if (TemplateActions.TryGetValue(Subject.Template.TemplateKey, out var action))
            action(source);
    }
}