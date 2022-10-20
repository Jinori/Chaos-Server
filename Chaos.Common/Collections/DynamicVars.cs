using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

namespace Chaos.Common.Collections;

[JsonConverter(typeof(DynamicVarsConverter))]
public class DynamicVars : IEnumerable<KeyValuePair<string, JsonElement>>
{
    private readonly JsonSerializerOptions JsonOptions;
    private readonly ConcurrentDictionary<string, JsonElement> Vars;

    /*
    static DynamicVars() =>
        JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            IgnoreReadOnlyProperties = true,
            IgnoreReadOnlyFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };*/

    public DynamicVars(IDictionary<string, JsonElement> collection, JsonSerializerOptions options)
    {
        JsonOptions = options;
        Vars = new ConcurrentDictionary<string, JsonElement>(collection, StringComparer.OrdinalIgnoreCase);
    }
    
    public DynamicVars(JsonSerializerOptions options)
    {
        JsonOptions = options;
        Vars = new ConcurrentDictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
    }

    public T? Get<T>(string key) => Vars.TryGetValue(key, out var value) ? value.Deserialize<T>(JsonOptions) : default;

    public object? Get(Type type, string key)
    {
        if (!Vars.TryGetValue(key, out var value))
            return default;

        return value.Deserialize(type, JsonOptions);
    }

    public void Set(string key, JsonElement element) => Vars.TryAdd(key, element);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, JsonElement>> GetEnumerator() => Vars.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}