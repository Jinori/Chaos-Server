using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Chaos.Clients.Abstractions;
using Chaos.CommandInterceptor;
using Chaos.Common.Abstractions;
using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Extensions.DependencyInjection;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.JsonConverters;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Options;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Serialization;
using Chaos.Services.Storage;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;

namespace Chaos;

public class Startup
{
    private static readonly SerializationContext JsonContext;
    private static readonly JsonSerializerOptions JsonSerializerOptions;
    private static bool IsInitialized;

    public IConfiguration Configuration { get; set; }
    public CancellationTokenSource ServerCtx { get; }

    static Startup()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true,
            IgnoreReadOnlyProperties = true,
            IgnoreReadOnlyFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        JsonSerializerOptions.Converters.Add(new PointConverter());
        JsonSerializerOptions.Converters.Add(new LocationConverter());
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        JsonContext = new SerializationContext(JsonSerializerOptions);
    }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        ServerCtx = new CancellationTokenSource();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(ServerCtx);
        var encodingProvider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(encodingProvider);

        services.AddSingleton(Configuration);
        services.AddOptions();

        services.AddOptionsFromConfig<ChaosOptions>(ConfigKeys.Options.Key)
                .Validate(o => !string.IsNullOrEmpty(o.StagingDirectory), "StagingDirectory is required");

        services.AddSingleton<IStagingDirectory, ChaosOptions>(p => p.GetRequiredService<IOptionsSnapshot<ChaosOptions>>().Value);

        services.AddLogging(
            logging =>
            {
                logging.AddConfiguration(Configuration.GetSection(ConfigKeys.Logging.Key));

                logging.AddNLog(
                    Configuration,
                    new NLogProviderOptions
                    {
                        LoggingConfigurationSectionName = ConfigKeys.Logging.NLog.Key
                    });
            });

        RegisterStructuredLoggingTransformations();

        services.AddOptions<JsonSerializerOptions>()
                .Configure<ILogger<WarningJsonTypeInfoResolver>>(
                    (options, logger) =>
                    {
                        if (!IsInitialized)
                        {
                            IsInitialized = true;
                            var defaultResolver = new WarningJsonTypeInfoResolver(logger);
                            var combinedResoler = JsonTypeInfoResolver.Combine(JsonContext, defaultResolver);

                            JsonSerializerOptions.SetTypeResolver(combinedResoler);
                        }

                        ShallowCopy<JsonSerializerOptions>.Merge(JsonSerializerOptions, options);
                    });

        services.AddCommandInterceptorForType<Aisling>("/", a => a.IsAdmin);
        services.AddServerAuthentication();
        services.AddCryptography();
        services.AddPacketSerializer();
        services.AddPathfinding();
        services.AddStorage();
        services.AddScripting();
        services.AddFunctionalScriptRegistry();
        services.AddWorldFactories();
        services.AddTypeMapper();

        services.AddSingleton<IShardGenerator, ExpiringMapInstanceCache>(
            p => (ExpiringMapInstanceCache)p.GetRequiredService<ISimpleCache<MapInstance>>());
    }

    protected void RegisterStructuredLoggingTransformations() =>
        LogManager.Setup()
                  .SetupSerialization(
                      builder =>
                      {
                          builder.RegisterObjectTransformation<ISocketClient>(
                              client => new
                              {
                                  IpAddress = client.RemoteIp
                              });

                          builder.RegisterObjectTransformation<IWorldClient>(
                              client => new
                              {
                                  IpAddress = client.RemoteIp,
                                  Aisling = client.Aisling != null!
                                      ? new
                                      {
                                          Type = nameof(Aisling),
                                          Id = client.Aisling.Id,
                                          Location = ILocation.ToString(client.Aisling),
                                          Name = client.Aisling.Name
                                      }
                                      : null
                              });

                          builder.RegisterObjectTransformation<WorldEntity>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Creation = obj.Creation
                              });

                          builder.RegisterObjectTransformation<MapEntity>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj)
                              });

                          builder.RegisterObjectTransformation<VisibleEntity>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj),
                                  Sprite = obj.Sprite
                              });

                          builder.RegisterObjectTransformation<NamedEntity>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj),
                                  Name = obj.Name
                              });

                          builder.RegisterObjectTransformation<Creature>(
                              obj => new
                              {
                                  Type = obj.GetType().Name,
                                  Id = obj.Id,
                                  Location = ILocation.ToString(obj),
                                  Name = obj.Name
                              });

                          builder.RegisterObjectTransformation<Aisling>(
                              obj => new
                              {
                                  // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                                  IpAddress = obj.Client?.RemoteIp,
                                  Aisling = new
                                  {
                                      Type = nameof(Aisling),
                                      Id = obj.Id,
                                      Location = ILocation.ToString(obj),
                                      Name = obj.Name
                                  }
                              });

                          builder.RegisterObjectTransformation<Monster>(
                              obj => new
                              {
                                  Type = nameof(Monster),
                                  Id = obj.Id,
                                  Location = ILocation.ToString(obj),
                                  Name = obj.Name,
                                  TemplateKey = obj.Template.TemplateKey
                              });

                          builder.RegisterObjectTransformation<Merchant>(
                              obj => new
                              {
                                  Type = nameof(Merchant),
                                  Id = obj.Id,
                                  Location = ILocation.ToString(obj),
                                  Name = obj.Name,
                                  TemplateKey = obj.Template.TemplateKey
                              });

                          builder.RegisterObjectTransformation<GroundItem>(
                              obj => new
                              {
                                  Type = nameof(GroundItem),
                                  Id = obj.Id,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj),
                                  Item = obj.Item
                              });

                          builder.RegisterObjectTransformation<Money>(
                              obj => new
                              {
                                  Type = nameof(Money),
                                  Id = obj.Id,
                                  Creation = obj.Creation,
                                  Location = ILocation.ToString(obj),
                                  Amount = obj.Amount
                              });

                          builder.RegisterObjectTransformation<Item>(
                              item => new
                              {
                                  Uid = item.UniqueId,
                                  Name = item.DisplayName,
                                  TemplateKey = item.Template.TemplateKey,
                                  Count = item.Count
                              });

                          builder.RegisterObjectTransformation<Spell>(
                              spell => new
                              {
                                  Uid = spell.UniqueId,
                                  Name = spell.Template.Name,
                                  TemplateKey = spell.Template.TemplateKey
                              });

                          builder.RegisterObjectTransformation<Skill>(
                              skill => new
                              {
                                  Uid = skill.UniqueId,
                                  Name = skill.Template.Name,
                                  TemplateKey = skill.Template.TemplateKey
                              });

                          builder.RegisterObjectTransformation<Exchange>(
                              exchange => new
                              {
                                  Id = exchange.ExchangeId,
                                  User1 = exchange.User1,
                                  User2 = exchange.User2
                              });

                          builder.RegisterObjectTransformation<MapInstance>(
                              map => new
                              {
                                  InstanceId = map.InstanceId,
                                  TemplateKey = map.Template.TemplateKey,
                                  Name = map.Name
                              });

                          builder.RegisterObjectTransformation<CommandDescriptor>(
                              obj => new
                              {
                                  ExecutedByType = obj.Type.FullName,
                                  RequiresAdmin = obj.Details.RequiresAdmin,
                                  CommandName = obj.Details.CommandName
                              });

                          builder.RegisterObjectTransformation<Dialog>(
                              dialog => new
                              {
                                  TemplateKey = dialog.Template.TemplateKey,
                                  Type = dialog.Template.Type
                              });

                          builder.RegisterObjectTransformation<IEffect>(
                              effect => new
                              {
                                  EffectKey = effect.ScriptKey,
                                  Name = effect.Name
                              });

                          builder.RegisterObjectTransformation<Redirect>(
                              obj => new
                              {
                                  Id = obj.Id,
                                  Name = obj.Name,
                                  Type = obj.Type,
                                  Address = obj.EndPoint
                              });
                      });

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class ConfigKeys
    {
        public static class Options
        {
            public static string Key => "Options";
        }

        public static class Logging
        {
            public static string Key => "Logging";
            public static string UseSeq => $"{Key}:UseSeq";

            public static class NLog
            {
                public static string Key => $"{Logging.Key}:NLog";
            }
        }
    }
}