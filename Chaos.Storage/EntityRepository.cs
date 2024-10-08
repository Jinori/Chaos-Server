using System.Runtime.Serialization;
using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

/// <summary>
///     Represents a store that can load and save objects that map to a different type when serialized
/// </summary>
public sealed class EntityRepository : IEntityRepository
{
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger<EntityRepository> Logger;
    private readonly ITypeMapper Mapper;
    private readonly EntityRepositoryOptions Options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityRepository" /> class
    /// </summary>
    public EntityRepository(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonserializerOptions,
        IOptionsSnapshot<EntityRepositoryOptions> options,
        ILogger<EntityRepository> logger)
    {
        Mapper = mapper;
        Logger = logger;
        JsonSerializerOptions = jsonserializerOptions.Value;
        Options = options.Value;
    }

    /// <inheritdoc />
    public TSchema Load<TSchema>(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading new unmapped {@TypeName} object from path {@Path}", typeof(TSchema).Name, path);

        var schema = JsonSerializerEx.Deserialize<TSchema>(path, JsonSerializerOptions);

        if (schema is null)
            throw new SerializationException($"Failed to serialize unmapped {typeof(TSchema).Name} from path \"{path}\"");

        return schema;
    }

    /// <inheritdoc />
    public T LoadAndMap<T, TSchema>(string path, Action<TSchema>? preMapAction = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading new {@TypeName} object from path {@Path}", typeof(T).Name, path);

        var schema = JsonSerializerEx.Deserialize<TSchema>(path, JsonSerializerOptions);

        if (schema is null)
            throw new SerializationException($"Failed to serialize {typeof(TSchema).Name} from path \"{path}\"");

        preMapAction?.Invoke(schema);

        return Mapper.Map<T>(schema);
    }

    /// <inheritdoc />
    public async Task<T> LoadAndMapAsync<T, TSchema>(string path, Func<TSchema, Task>? preMapAction = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading new {@TypeName} object from path {@Path}", typeof(T).Name, path);

        var schema = await JsonSerializerEx.DeserializeAsync<TSchema>(path, JsonSerializerOptions);

        if (schema is null)
            throw new SerializationException($"Failed to serialize {typeof(TSchema).Name} from path \"{path}\"");

        if (preMapAction is not null)
            await preMapAction(schema);

        return Mapper.Map<T>(schema);
    }

    /// <inheritdoc />
    public IEnumerable<T> LoadAndMapMany<T, TSchema>(string path, Action<TSchema>? preMapAction = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading collection of {@TypeName} objects from path {@Path}", typeof(T).Name, path);

        var schemas = JsonSerializerEx.Deserialize<ICollection<TSchema>>(path, JsonSerializerOptions)!;

        if (schemas is null)
            throw new SerializationException($"Failed to serialize collection of {typeof(TSchema).Name} from path \"{path}\"");

        foreach (var schema in schemas)
            preMapAction?.Invoke(schema);

        return Mapper.MapMany<TSchema, T>(schemas);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<T> LoadAndMapManyAsync<T, TSchema>(string path, Func<TSchema, Task>? preMapAction = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading collection of {@TypeName} objects from path {@Path}", typeof(T).Name, path);

        var schemas = await JsonSerializerEx.DeserializeAsync<ICollection<TSchema>>(path, JsonSerializerOptions);

        if (schemas is null)
            throw new SerializationException($"Failed to serialize collection of {typeof(TSchema).Name} from path \"{path}\"");

        if (preMapAction is not null)
            foreach (var schema in schemas)
            {
                await preMapAction(schema);

                yield return Mapper.Map<T>(schema!);
            }
        else
            foreach (var obj in Mapper.MapMany<TSchema, T>(schemas))
                yield return obj;
    }

    /// <inheritdoc />
    public async Task<TSchema> LoadAsync<TSchema>(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading new unmapped {@TypeName} object from path {@Path}", typeof(TSchema).Name, path);

        var schema = await JsonSerializerEx.DeserializeAsync<TSchema>(path, JsonSerializerOptions);

        if (schema is null)
            throw new SerializationException($"Failed to serialize unmapped {typeof(TSchema).Name} from path \"{path}\"");

        return schema;
    }

    /// <inheritdoc />
    public IEnumerable<TSchema> LoadMany<TSchema>(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading collection of unmapped {@TypeName} objects from path {@Path}", typeof(TSchema).Name, path);

        var schemas = JsonSerializerEx.Deserialize<IEnumerable<TSchema>>(path, JsonSerializerOptions);

        if (schemas is null)
            throw new SerializationException($"Failed to serialize collection of unmapped {typeof(TSchema).Name} from path \"{path}\"");

        return schemas;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TSchema> LoadManyAsync<TSchema>(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading collection of unmapped {@TypeName} objects from path {@Path}", typeof(TSchema).Name, path);

        var schemas = await JsonSerializerEx.DeserializeAsync<IEnumerable<TSchema>>(path, JsonSerializerOptions);

        if (schemas is null)
            throw new SerializationException($"Failed to serialize collection of unmapped {typeof(TSchema).Name} from path \"{path}\"");

        foreach (var schema in schemas)
            yield return schema;
    }

    /// <inheritdoc />
    public void Save<TSchema>(TSchema obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving {@TypeName} to path {@Path}", typeof(TSchema).Name, path);

        JsonSerializerEx.Serialize(
            path,
            obj,
            JsonSerializerOptions,
            Options.SafeSaves);
    }

    /// <inheritdoc />
    public void SaveAndMap<T, TSchema>(T obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving {@TypeName} to path {@Path}", typeof(T).Name, path);

        var schema = Mapper.Map<TSchema>(obj)!;

        JsonSerializerEx.Serialize(
            path,
            schema,
            JsonSerializerOptions,
            Options.SafeSaves);
    }

    /// <inheritdoc />
    public Task SaveAndMapAsync<T, TSchema>(T obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving {@TypeName} to path {@Path}", typeof(T).Name, path);

        var schema = Mapper.Map<TSchema>(obj)!;

        return JsonSerializerEx.SerializeAsync(
            path,
            schema,
            JsonSerializerOptions,
            Options.SafeSaves);
    }

    /// <inheritdoc />
    public void SaveAndMapMany<T, TSchema>(IEnumerable<T> obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving many {@TypeName} to path {@Path}", typeof(T).Name, path);

        var schema = Mapper.MapMany<T, TSchema>(obj);

        JsonSerializerEx.Serialize(
            path,
            schema,
            JsonSerializerOptions,
            Options.SafeSaves);
    }

    /// <inheritdoc />
    public Task SaveAndMapManyAsync<T, TSchema>(IEnumerable<T> obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving many {@TypeName} to path {@Path}", typeof(T).Name, path);

        var schema = Mapper.MapMany<T, TSchema>(obj);

        return JsonSerializerEx.SerializeAsync(
            path,
            schema,
            JsonSerializerOptions,
            Options.SafeSaves);
    }

    /// <inheritdoc />
    public Task SaveAsync<TSchema>(TSchema obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving {@TypeName} to path {@Path}", typeof(TSchema).Name, path);

        return JsonSerializerEx.SerializeAsync(
            path,
            obj,
            JsonSerializerOptions,
            Options.SafeSaves);
    }

    /// <inheritdoc />
    public void SaveMany<TSchema>(IEnumerable<TSchema> obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving many {@TypeName} to path {@Path}", typeof(TSchema).Name, path);

        JsonSerializerEx.Serialize(
            path,
            obj,
            JsonSerializerOptions,
            Options.SafeSaves);
    }

    /// <inheritdoc />
    public Task SaveManyAsync<TSchema>(IEnumerable<TSchema> obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving many {@TypeName} to path {@Path}", typeof(TSchema).Name, path);

        return JsonSerializerEx.SerializeAsync(
            path,
            obj,
            JsonSerializerOptions,
            Options.SafeSaves);
    }
}