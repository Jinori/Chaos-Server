using System.Diagnostics;
using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

/// <summary>
///     Manages save files for Aislings
/// </summary>
public sealed class AislingStore : BackedUpFileStoreBase<Aisling, AislingStoreOptions>, IAsyncStore<Aisling>
{
    private readonly IEntityRepository EntityRepository;
    private readonly ICloningService<Item> ItemCloningService;

    public AislingStore(
        IEntityRepository entityRepository,
        IOptions<AislingStoreOptions> options,
        ILogger<AislingStore> logger,
        ICloningService<Item> itemCloningService
    )
        : base(options, logger)
    {
        ItemCloningService = itemCloningService;
        EntityRepository = entityRepository;
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string key)
    {
        var directory = Path.Combine(Options.Directory, key.ToLower());

        return Task.FromResult(Directory.Exists(directory));
    }

    private async Task<Aisling> InnerLoadAsync(string name, string directory)
    {
        if (!Directory.Exists(directory))
            throw new InvalidOperationException($"No aisling data exists for the key \"{name}\" at the specified path \"{directory}\"");

        var aislingPath = Path.Combine(directory, "aisling.json");
        var bankPath = Path.Combine(directory, "bank.json");
        var trackersPath = Path.Combine(directory, "trackers.json");
        var legendPath = Path.Combine(directory, "legend.json");
        var inventoryPath = Path.Combine(directory, "inventory.json");
        var skillsPath = Path.Combine(directory, "skills.json");
        var spellsPath = Path.Combine(directory, "spells.json");
        var equipmentPath = Path.Combine(directory, "equipment.json");
        var effectsPath = Path.Combine(directory, "effects.json");

        var aislingTask = EntityRepository.LoadAndMapAsync<Aisling, AislingSchema>(aislingPath);
        var bankTask = EntityRepository.LoadAndMapAsync<Bank, BankSchema>(bankPath);
        var trackersTask = EntityRepository.LoadAndMapAsync<Trackers, TrackersSchema>(trackersPath);
        var effectsTask = EntityRepository.LoadAndMapManyAsync<IEffect, EffectSchema>(effectsPath).ToListAsync();
        var equipmentTask = EntityRepository.LoadAndMapManyAsync<Item, ItemSchema>(equipmentPath).ToListAsync();
        var inventoryTask = EntityRepository.LoadAndMapManyAsync<Item, ItemSchema>(inventoryPath).ToListAsync();
        var skillsTask = EntityRepository.LoadAndMapManyAsync<Skill, SkillSchema>(skillsPath).ToListAsync();
        var spellsTask = EntityRepository.LoadAndMapManyAsync<Spell, SpellSchema>(spellsPath).ToListAsync();
        var legendTask = EntityRepository.LoadAndMapManyAsync<LegendMark, LegendMarkSchema>(legendPath).ToListAsync();

        var aisling = await aislingTask;
        var bank = await bankTask;
        var trackers = await trackersTask;

        var effectsBar = new EffectsBar(aisling, await effectsTask);
        var equipment = new Equipment(await equipmentTask);
        var inventory = new Inventory(ItemCloningService, await inventoryTask);
        var skillBook = new SkillBook(await skillsTask);
        var spellBook = new SpellBook(await spellsTask);
        var legend = new Legend(await legendTask);

        aisling.Initialize(
            name,
            bank,
            equipment,
            inventory,
            skillBook,
            spellBook,
            legend,
            effectsBar,
            trackers);

        return aisling;
    }

    private Task InnerSaveAsync(string directory, Aisling aisling)
    {
        var aislingPath = Path.Combine(directory, "aisling.json");
        var bankPath = Path.Combine(directory, "bank.json");
        var trackersPath = Path.Combine(directory, "trackers.json");
        var legendPath = Path.Combine(directory, "legend.json");
        var inventoryPath = Path.Combine(directory, "inventory.json");
        var skillsPath = Path.Combine(directory, "skills.json");
        var spellsPath = Path.Combine(directory, "spells.json");
        var equipmentPath = Path.Combine(directory, "equipment.json");
        var effectsPath = Path.Combine(directory, "effects.json");

        return Task.WhenAll(
            EntityRepository.SaveAsync<Aisling, AislingSchema>(aisling, aislingPath),
            EntityRepository.SaveAsync<Bank, BankSchema>(aisling.Bank, bankPath),
            EntityRepository.SaveAsync<Trackers, TrackersSchema>(aisling.Trackers, trackersPath),
            EntityRepository.SaveAsync<LegendMark, LegendMarkSchema>(aisling.Legend, legendPath),
            EntityRepository.SaveAsync<Item, ItemSchema>(aisling.Inventory, inventoryPath),
            EntityRepository.SaveAsync<Skill, SkillSchema>(aisling.SkillBook, skillsPath),
            EntityRepository.SaveAsync<Spell, SpellSchema>(aisling.SpellBook, spellsPath),
            EntityRepository.SaveAsync<Item, ItemSchema>(aisling.Equipment, equipmentPath),
            EntityRepository.SaveAsync<IEffect, EffectSchema>(aisling.Effects, effectsPath));
    }

    public async Task<Aisling> LoadAsync(string name)
    {
        Logger.LogTrace("Loading aisling {@AislingName}", name);

        var directory = Path.Combine(Options.Directory, name.ToLower());

        var aisling = await SafeExecuteDirectoryActionAsync(directory, () => InnerLoadAsync(name, directory));

        Logger.WithProperty(aisling)
              .LogDebug("Loaded aisling {@AislingName}", aisling.Name);

        return aisling;
    }

    public async Task SaveAsync(Aisling aisling)
    {
        Logger.WithProperty(aisling)
              .LogTrace("Saving {@AislingName}", aisling.Name);

        var start = Stopwatch.GetTimestamp();

        try
        {
            var directory = Path.Combine(Options.Directory, aisling.Name.ToLower());

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await SafeExecuteDirectoryActionAsync(directory, () => InnerSaveAsync(directory, aisling));

            Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);

            Logger.WithProperty(aisling)
                  .LogDebug("Saved aisling {@AislingName}, took {@Elapsed}", aisling.Name, Stopwatch.GetElapsedTime(start));
        } catch (Exception e)
        {
            Logger.WithProperty(aisling)
                  .LogCritical(
                      e,
                      "Failed to save aisling {@AislingName} in {@Elapsed}",
                      aisling.Name,
                      Stopwatch.GetElapsedTime(start));
        }
    }
}