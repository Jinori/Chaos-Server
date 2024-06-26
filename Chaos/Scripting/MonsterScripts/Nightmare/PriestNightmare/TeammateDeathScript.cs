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

namespace Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;

// ReSharper disable once ClassCanBeSealed.Global
public class TeammateDeathScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private readonly IItemFactory ItemFactory;
    

    /// <inheritdoc />
    public TeammateDeathScript(Monster subject, ISimpleCache simpleCache, IItemFactory itemFactory)
        : base(subject)
    {
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        SimpleCache = simpleCache;
        ItemFactory = itemFactory;
    }

    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;

        var teammates = Map.GetEntities<Monster>().Where(m => m.Name == "Teammate");

        var players = Map.GetEntities<Aisling>().Where(x => x.IsAlive).ToList();

        foreach (var player in players)
            player.SendOrangeBarMessage($"{Subject.Name} has been killed! Your nightmare gets worse.");

        if (teammates.Any())
            return;

        foreach (var player in players)
        {
            var nightmaregearDictionary = new Dictionary<(BaseClass, Gender), string[]>
            {
                { (BaseClass.Warrior, Gender.Male), ["malecarnunplate", "carnunhelmet"] },
                { (BaseClass.Warrior, Gender.Female), ["femalecarnunplate", "carnunhelmet"] },
                { (BaseClass.Monk, Gender.Male), ["maleaosdicpatternwalker"] },
                { (BaseClass.Monk, Gender.Female), ["femaleaosdicpatternwalker"] },
                { (BaseClass.Rogue, Gender.Male), ["malemarauderhide", "maraudermask"] },
                { (BaseClass.Rogue, Gender.Female), ["femalemarauderhide", "maraudermask"] },
                { (BaseClass.Priest, Gender.Male), ["malecthonicdisciplerobes", "cthonicdisciplecaputium"] },
                { (BaseClass.Priest, Gender.Female), ["morrigudisciplepellison", "holyhairband"] },
                { (BaseClass.Wizard, Gender.Male), ["cthonicmagusrobes", "cthonicmaguscaputium"] },
                { (BaseClass.Wizard, Gender.Female), ["morrigumaguspellison", "magushairband"] }
            };
            
            var gearKey = (player.UserStatSheet.BaseClass, player.Gender);

            if (nightmaregearDictionary.TryGetValue(gearKey, out var nightmaregear))
            {
                var hasGear = nightmaregear.All(
                    gearItemName =>
                        player.Inventory.ContainsByTemplateKey(gearItemName)
                        || player.Bank.Contains(gearItemName)
                        || player.Equipment.ContainsByTemplateKey(gearItemName));

                if (!hasGear)
                    foreach (var gearItemName in nightmaregear)
                    {
                        var gearItem = ItemFactory.Create(gearItemName);
                        player.GiveItemOrSendToBank(gearItem);
                    }
            }
            var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
            var pointS = new Point(5, 7);

            player.TraverseMap(mapInstance, pointS);
            player.StatSheet.AddHp(1);
            player.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareLoss1);
            player.Client.SendAttributes(StatUpdateType.Vitality);
            player.SendOrangeBarMessage("You have been defeated by your Nightmares");
            player.Legend.AddOrAccumulate(
                new LegendMark(
                    "Succumbed to their Nightmares.",
                    "Nightmare",
                    MarkIcon.Victory,
                    MarkColor.White,
                    1,
                    GameTime.Now));

            player.Refresh(true);
        }
    }
}