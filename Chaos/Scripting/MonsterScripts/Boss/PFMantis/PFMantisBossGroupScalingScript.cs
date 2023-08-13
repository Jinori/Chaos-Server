using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.PFMantis;

public sealed class PFMantisBossGroupScalingScript : MonsterScriptBase
{
    private static bool GroupBonusApplied;
    private readonly ISpellFactory _spellFactory;

    public PFMantisBossGroupScalingScript(Monster subject, ISpellFactory spellFactory)
        : base(subject) =>
        _spellFactory = spellFactory;

    private void AddSpellsBasedOnGroupLevel(IEnumerable<int> groupLevel)
    {
        var averageLevel = groupLevel.Average();

        switch (averageLevel)
        {
            case > 11 and < 30:
                AddSpellsToBoss("beagsallamh", "beagatharlamh", "beagcradh");

                break;
            case > 31:
                AddSpellsToBoss("atharlamh", "sallamh", "cradh");

                break;
        }
    }

    private void AddSpellsToBoss(string spellName1, string spellName2, string spellName3)
    {
        Subject.Spells.Add(_spellFactory.Create(spellName1));
        Subject.Spells.Add(_spellFactory.Create(spellName2));
        Subject.Spells.Add(_spellFactory.Create(spellName3));
    }

    private Attributes CreateGroupBonusAttributes(IReadOnlyCollection<int> groupLevel)
    {
        var averageLevel = (int)groupLevel.Average();
        var groupCount = groupLevel.Count;

        return new Attributes
        {
            Con = averageLevel,
            Dex = averageLevel,
            Int = averageLevel,
            Str = averageLevel,
            Wis = averageLevel,
            AtkSpeedPct = groupCount * 5,
            MaximumHp = averageLevel * groupCount * 900,
            MaximumMp = averageLevel * groupCount * 900,
            SkillDamagePct = groupCount * 5,
            SpellDamagePct = groupCount * 5
        };
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        // If bonus is already applied or there's no valid target, return
        if (GroupBonusApplied || (Target == null) || !ShouldMove || !Map.GetEntities<Aisling>().Any())
            return;

        var groupLevel = Map.GetEntitiesWithinRange<Aisling>(Subject, 12).Select(aisling => aisling.StatSheet.Level).ToList();

        // Create bonus attributes based on the group level
        var attrib = CreateGroupBonusAttributes(groupLevel);

        // Check the group level and add spells accordingly
        AddSpellsBasedOnGroupLevel(groupLevel);

        // Add the attributes to the monster
        Subject.StatSheet.AddBonus(attrib);
        // Add HP and MP to the monster
        Subject.StatSheet.SetHealthPct(100);
        Subject.StatSheet.SetManaPct(100);
        GroupBonusApplied = true;
    }
}