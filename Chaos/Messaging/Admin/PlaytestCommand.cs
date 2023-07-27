using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.LevelUp;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Chaos.Messaging.Admin;

[Command("playtest", helpText: "<level>")]
public class PlaytestCommand : ICommand<Aisling>
{
    private readonly IConfiguration Configuration;
    private readonly IItemFactory ItemFactory;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    public ILevelUpScript LevelUpScript { get; set; }

    public PlaytestCommand(ISkillFactory skillFactory, ISpellFactory spellFactory, IItemFactory itemFactory)
    {
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        ItemFactory = itemFactory;
        LevelUpScript = DefaultLevelUpScript.Create();
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext(out int level))
            return default;

        for (var i = 0; i < level; i++)
            LevelUpScript.LevelUp(source);

        // Grant the player appropriate skills and spells for the new level

        foreach (var skill in GetSkillsForLevel(source, level))
            if (!source.SkillBook.ContainsByTemplateKey(skill.TemplateKey))
            {
                var newSkill = SkillFactory.Create(skill.TemplateKey);
                source.SkillBook.TryAddToNextSlot(newSkill);
            }

        foreach (var spell in GetSpellsForLevel(source, level))
            if (!source.SpellBook.ContainsByTemplateKey(spell.TemplateKey))
            {
                var newSpell = SpellFactory.Create(spell.TemplateKey);
                source.SpellBook.TryAddToNextSlot(newSpell);
            }

        // Equip the best-in-slot equipment for the new level
        EquipBestInSlotEquipment(source, level);

        // Send necessary updates to the client
        source.Client.SendAttributes(StatUpdateType.Full);

        return default;
    }

    private void EquipBestInSlotEquipment(Aisling player, int level)
    {
        var equipmentDirectory = Configuration.GetSection("Options:ItemTemplateCacheOptions:Directory").Value;
        var rootDirectory = Configuration.GetSection("Options:ChaosOptions:StagingDirectory").Value;
        var equipmentFolderPath = Path.Combine(rootDirectory!, equipmentDirectory!);

        var bestInSlotEquipment = new Dictionary<string, EquipmentInfo>();

        // Get all JSON files in the specified equipment directory and its subdirectories.
        var equipmentJsonFiles = Directory.GetFiles(equipmentFolderPath, "*.json", SearchOption.AllDirectories);

        foreach (var equipmentJsonFile in equipmentJsonFiles)
        {
            var json = File.ReadAllText(equipmentJsonFile);

            // Manually parse the JSON content to extract the required information.
            var jsonObject = JObject.Parse(json);
            var equipmentType = jsonObject["equipmentType"]?.Value<string>();
            var gender = jsonObject["gender"]?.Value<string>();
            var classValue = jsonObject["class"]?.Value<string>();
            var levelValue = jsonObject["level"]?.Value<int>();
            var templateKeyValue = jsonObject["templateKey"]?.Value<string>();

            // Check if the equipment's level matches the requested level and the player's class.
            if (!string.IsNullOrEmpty(equipmentType)
                && !string.IsNullOrEmpty(classValue)
                && levelValue.HasValue
                && !string.IsNullOrEmpty(templateKeyValue)
                && ((classValue == player.UserStatSheet.BaseClass.ToString()) || (classValue == "Peasant"))
                && (levelValue.Value <= level))
            {
                // Check if the equipment is better than the current best-in-slot for the equipment type.
                if (!bestInSlotEquipment.ContainsKey(equipmentType))
                    bestInSlotEquipment[equipmentType] = new EquipmentInfo
                    {
                        EquipmentType = equipmentType,
                        Gender = gender!,
                        Class = classValue,
                        Level = levelValue.Value,
                        TemplateKey = templateKeyValue
                    };
                else if (levelValue.Value > bestInSlotEquipment[equipmentType].Level)
                    bestInSlotEquipment[equipmentType] = new EquipmentInfo
                    {
                        EquipmentType = equipmentType,
                        Gender = gender!,
                        Class = classValue,
                        Level = levelValue.Value,
                        TemplateKey = templateKeyValue
                    };
            }
            else
                // Log the problematic equipment data for troubleshooting.
                Console.WriteLine($"Invalid equipment data found in {equipmentJsonFile}.");
        }

        // Equip the best-in-slot equipment for the player.
        foreach (var equipmentInfo in bestInSlotEquipment.Values)
        {
            var item = ItemFactory.Create(equipmentInfo.TemplateKey);
            player.Inventory.TryAddToNextSlot(item);
        }
    }

    private IEnumerable<SkillInfo> GetSkillsForLevel(Aisling source, int level)
    {
        var skillsDirectory = Configuration.GetSection("Options:SkillTemplateCacheOptions:Directory").Value;
        var rootDirectory = Configuration.GetSection("Options:ChaosOptions:StagingDirectory").Value;
        var skillsFolderPath = Path.Combine(rootDirectory!, skillsDirectory!);

        var skillInfoList = new List<SkillInfo>();

        // Get all JSON files in the specified skills directory and its subdirectories.
        var skillJsonFiles = Directory.GetFiles(skillsFolderPath, "*.json", SearchOption.AllDirectories);

        foreach (var skillJsonFile in skillJsonFiles)
        {
            var json = File.ReadAllText(skillJsonFile);

            // Manually parse the JSON content to extract the required information.
            var jsonObject = JObject.Parse(json);
            var levelValue = jsonObject["level"]?.Value<int>();
            var classValue = jsonObject["class"]?.Value<string>();
            var templateKeyValue = jsonObject["templateKey"]?.Value<string>();

            // Check if the skill's level matches the requested level.
            if (!string.IsNullOrEmpty(classValue)
                && levelValue.HasValue
                && !string.IsNullOrEmpty(templateKeyValue)
                && (classValue == source.UserStatSheet.BaseClass.ToString())
                && (levelValue.Value <= level))
            {
                var skillInfo = new SkillInfo
                {
                    Level = levelValue.Value,
                    BaseClass = classValue,
                    TemplateKey = templateKeyValue
                };

                skillInfoList.Add(skillInfo);
            }
        }

        return skillInfoList;
    }

    private IEnumerable<SpellInfo> GetSpellsForLevel(Aisling source, int level)
    {
        var spellsDirectory = Configuration.GetSection("Options:SpellTemplateCacheOptions:Directory").Value;
        var rootDirectory = Configuration.GetSection("Options:ChaosOptions:StagingDirectory").Value;
        var spellsFolderPath = Path.Combine(rootDirectory!, spellsDirectory!);

        var spellInfoList = new List<SpellInfo>();

        // Get all JSON files in the specified skills directory and its subdirectories.
        var spellJsonFiles = Directory.GetFiles(spellsFolderPath, "*.json", SearchOption.AllDirectories);

        foreach (var spellJsonFile in spellJsonFiles)
        {
            var json = File.ReadAllText(spellJsonFile);

            // Manually parse the JSON content to extract the required information.
            var jsonObject = JObject.Parse(json);
            var levelValue = jsonObject["level"]?.Value<int>();
            var classValue = jsonObject["class"]?.Value<string>();
            var templateKeyValue = jsonObject["templateKey"]?.Value<string>();

            // Check if the skill's level matches the requested level.
            if (!string.IsNullOrEmpty(classValue)
                && levelValue.HasValue
                && !string.IsNullOrEmpty(templateKeyValue)
                && (classValue == source.UserStatSheet.BaseClass.ToString())
                && (levelValue.Value <= level))
            {
                var skillInfo = new SpellInfo
                {
                    Level = levelValue.Value,
                    BaseClass = classValue,
                    TemplateKey = templateKeyValue
                };

                spellInfoList.Add(skillInfo);
            }
        }

        return spellInfoList;
    }

    public sealed class EquipmentInfo
    {
        public string Class { get; set; } = null!;
        public string EquipmentType { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public int Level { get; set; }
        public string TemplateKey { get; set; } = null!;
    }

    public sealed class SkillInfo
    {
        public string BaseClass { get; set; } = null!;
        public int Level { get; set; }
        public string TemplateKey { get; set; } = null!;
    }

    public sealed class SpellInfo
    {
        public string BaseClass { get; set; } = null!;
        public int Level { get; set; }
        public string TemplateKey { get; set; } = null!;
    }
}