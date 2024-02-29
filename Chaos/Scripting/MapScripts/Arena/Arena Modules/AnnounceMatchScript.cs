using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class AnnounceMatchScript : MapScriptBase
{
    private bool AnnounceStart;
    private readonly IIntervalTimer MessageTimer;
    
    /// <inheritdoc />
    public AnnounceMatchScript(MapInstance subject)
        : base(subject) =>
        MessageTimer = new PeriodicMessageTimer(
            TimeSpan.FromSeconds(35),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(1),
            "The match will begin in {Time}.",
            SendMessage);

    private void SendMessage(string message)
    {
        foreach (var player in Subject.GetEntities<Aisling>())
            player.SendServerMessage(ServerMessageType.ActiveMessage, message);
    }
    
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        MessageTimer.Update(delta);

        if (MessageTimer.IntervalElapsed)
        {
            if (!AnnounceStart)
            {
                if (Subject.Name == "Escort - Teams")
                    Subject.Morph("26017");

                if (Subject.Name == "Lava Arena - Teams")
                    Subject.Morph("26006");
                
                if (Subject.Name == "Color Clash - Teams")
                    Subject.Morph("26013");
            
                SendMessage("Let's go! Match start!");
                AnnounceStart = true;
            }
        }
    }
}