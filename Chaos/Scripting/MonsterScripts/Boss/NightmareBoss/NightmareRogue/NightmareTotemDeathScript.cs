using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.MonsterScripts.Boss.NightmareBoss.NightmareRogue;

public sealed class NightmareTotemDeathScript : MonsterScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public NightmareTotemDeathScript(Monster subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveObject(Subject))
            return;

        var nightmaregearDictionary = new Dictionary<(BaseClass, Gender), string[]>
        {
            { (BaseClass.Warrior, Gender.Male), new[] { "malecarnunplate", "carnunhelmet" } },
            { (BaseClass.Warrior, Gender.Female), new[] { "femalecarnunplate", "carnunhelmet" } },
            { (BaseClass.Monk, Gender.Male), new[] { "maleaosdicpatternwalker" } },
            { (BaseClass.Monk, Gender.Female), new[] { "femaleaosdicpatternwalker" } },
            { (BaseClass.Rogue, Gender.Male), new[] { "malemarauderhide", "maraudermask" } },
            { (BaseClass.Rogue, Gender.Female), new[] { "femalemarauderhide", "maraudermask" } },
            { (BaseClass.Priest, Gender.Male), new[] { "malecthonicdisciplerobes", "cthonicdisciplecaputium" } },
            { (BaseClass.Priest, Gender.Female), new[] { "morrigudisciplepellison", "holyhairband" } },
            { (BaseClass.Wizard, Gender.Male), new[] { "malecthonicmagusrobes", "cthonicmaguscaputium" } },
            { (BaseClass.Wizard, Gender.Female), new[] { "morrigumaguspellison", "magushairband" } }
        };

        foreach (var aisling in Map.GetEntities<Aisling>())
        {
            aisling.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareLoss1);
            aisling.SendOrangeBarMessage("You succumb to your nightmare... and wake up in the Inn.");

            aisling.Legend.AddOrAccumulate(
                new LegendMark(
                    "Succumbed to their Nightmares",
                    "Nightmare",
                    MarkIcon.Victory,
                    MarkColor.Brown,
                    1,
                    GameTime.Now));

            ExperienceDistributionScript.GiveExp(aisling, 250000);

            var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
            var pointS = new Point(5, 7);
            aisling.TraverseMap(mapInstance, pointS);

            var gearKey = (aisling.UserStatSheet.BaseClass, aisling.Gender);

            if (nightmaregearDictionary.TryGetValue(gearKey, out var nightmaregear))
            {
                var hasGear = nightmaregear.All(
                    gearItemName =>
                        aisling.Inventory.ContainsByTemplateKey(gearItemName)
                        || aisling.Bank.Contains(gearItemName)
                        || aisling.Equipment.ContainsByTemplateKey(gearItemName));

                if (!hasGear)
                    foreach (var gearItemName in nightmaregear)
                    {
                        var gearItem = ItemFactory.Create(gearItemName);
                        aisling.GiveItemOrSendToBank(gearItem);
                    }
            }
        }
    }
}