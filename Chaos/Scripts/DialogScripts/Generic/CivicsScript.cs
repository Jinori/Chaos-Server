using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts.Generic;

public class CivicsScript : DialogScriptBase
{
    public CivicsScript(Dialog subject) : base(subject)
    {
    }

    
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            //Rucesion
            case "angelo_civics":
            {
                if (source.Nation != Nation.Exile)
                {
                    if (Subject.GetOptionIndex("Become Rucesion Citizen").HasValue)
                    {
                        var s = Subject.GetOptionIndex("Become Rucesion Citizen")!.Value;
                        Subject.Options.RemoveAt(s);
                    }
                }

                break;
            }
            case "angelo_renounceyes":
            {
                if (source.Nation != Nation.Rucesion)
                {
                    source.SendOrangeBarMessage("You are of a different Nation. Begone.");
                    Subject.Close(source);
                }
                if (source.Nation == Nation.Rucesion)
                {
                    source.Nation = Nation.Exile;
                    source.SendOrangeBarMessage("You renounce your citizenship and turn to Exile.");   
                }
            }
                break;
            
            case "angelo_becomecitizenyes":
            {
                if (source.Nation != Nation.Exile && source.Nation != Nation.Rucesion)
                {
                    source.SendOrangeBarMessage("You are already of another Nation. Begone.");
                    Subject.Close(source);
                }
                if (source.Nation != Nation.Exile && source.Nation == Nation.Rucesion)
                {
                    source.SendOrangeBarMessage("Citizen? You already have the seal upon your legend.");
                    Subject.Close(source);
                }
                if (source.Nation == Nation.Exile)
                {
                    var ani = new Animation
                    {
                        AnimationSpeed = 100,
                        TargetAnimation = 78
                    };
                    source.Nation = Nation.Rucesion;
                    source.SendOrangeBarMessage("You become a Rucesion Citizen. Hooray!"); 
                    source.Animate(ani, source.Id);
                }
            }
                break;
            //Mileth
            case "aingeal_civics":
            {
                if (source.Nation != Nation.Exile)
                {
                    if (Subject.GetOptionIndex("Become Mileth Citizen").HasValue)
                    {
                        var s = Subject.GetOptionIndex("Become Mileth Citizen")!.Value;
                        Subject.Options.RemoveAt(s);
                    }
                }

                break;
            }
            case "aingeal_renounceyes":
            {
                if (source.Nation != Nation.Mileth)
                {
                    source.SendOrangeBarMessage("You are of a different Nation. Begone.");
                    Subject.Close(source);
                }
                if (source.Nation == Nation.Mileth)
                {
                    source.Nation = Nation.Exile;
                    source.SendOrangeBarMessage("You renounce your citizenship and turn to Exile.");   
                }
            }
                break;
            case "aingeal_becomecitizenyes":
            {
                if (source.Nation != Nation.Exile && source.Nation != Nation.Mileth)
                {
                    source.SendOrangeBarMessage("You are already of another Nation. Begone.");
                    Subject.Close(source);
                }
                if (source.Nation != Nation.Exile && source.Nation == Nation.Mileth)
                {
                    source.SendOrangeBarMessage("Citizen? You already have the seal upon your legend.");
                    Subject.Close(source);
                }
                if (source.Nation == Nation.Exile)
                {
                    var ani = new Animation
                    {
                        AnimationSpeed = 100,
                        TargetAnimation = 78
                    };
                    source.Nation = Nation.Mileth;
                    source.SendOrangeBarMessage("You become a Mileth Citizen. Hooray!"); 
                    source.Animate(ani, source.Id);
                }
            }
                break;
        }
    }
}