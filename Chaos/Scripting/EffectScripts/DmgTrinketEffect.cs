using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class DmgTrinketEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(6);
    public override byte Icon => 87;
    public override string Name => "Dmg Trinket";

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Dmg = 10,
            SkillDamagePct = 5,
            SpellDamagePct = 5
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A weaponsmith blesses your instrument of battle.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Dmg = 10,
            SkillDamagePct = 5,
            SpellDamagePct = 5
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The blessing of the weaponsmith has subsided.");
    }
}