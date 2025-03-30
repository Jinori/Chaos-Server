using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public sealed class BossGroupScalingScript(Monster subject, ISpellFactory spellFactory) : MonsterScriptBase(subject)
{
    private static bool GroupBonusApplied;

    private void AddSpellsBasedOnGroupLevel(IEnumerable<int> groupLevel)
    {
        var averageLevel = groupLevel.Average();

        switch (averageLevel)
        {
            case > 10 and < 24:
                AddSpellsToBoss("beagsradlamh", "beagcradh");

                break;
            case > 25:
                AddSpellsToBoss("srad", "beagcradh");

                break;
        }
    }

    private void AddSpellsToBoss(string spellName1, string spellName2)
    {
        Subject.Spells.Add(spellFactory.Create(spellName1));
        Subject.Spells.Add(spellFactory.Create(spellName2));
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
            AtkSpeedPct = groupCount * 3,
            MaximumHp = averageLevel * groupCount * 500,
            MaximumMp = averageLevel * groupCount * 500,
            SkillDamagePct = groupCount * 2,
            SpellDamagePct = groupCount * 2
        };
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        // If bonus is already applied or there's no valid target, return
        if (GroupBonusApplied
            || (Target == null)
            || !ShouldMove
            || !Map.GetEntities<Aisling>()
                   .Any())
            return;

        var groupLevel = Map.GetEntitiesWithinRange<Aisling>(Subject, 12)
                            .Select(aisling => aisling.StatSheet.Level)
                            .ToList();

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