using System.Configuration;
using System.Net;
using System.Net.Sockets;
using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.MetaData;
using Chaos.Models.Templates;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.ItemScripts.Enchantments;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage.Options;
using Chaos.Utilities;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Configuration;

public sealed class OptionsConfigurer(IStagingDirectory stagingDirectory, IChannelService channelService)
    : IPostConfigureOptions<IConnectionInfo>,
      IPostConfigureOptions<LobbyOptions>,
      IPostConfigureOptions<LoginOptions>,
      IPostConfigureOptions<WorldOptions>,
      IPostConfigureOptions<MetaDataStoreOptions>,
                                        IPostConfigureOptions<ItemTemplate>

{
    private readonly IChannelService ChannelService = channelService;
    private readonly IStagingDirectory StagingDirectory = stagingDirectory;

    /// <inheritdoc />
    public void PostConfigure(string? name, IConnectionInfo options)
    {
        if (!string.IsNullOrEmpty(options.HostName))
            options.Address = Dns.GetHostAddresses(options.HostName)
                                 .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)!;
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, LobbyOptions options)
    {
        PostConfigure(name, (IConnectionInfo)options);

        foreach (var server in options.Servers)
            PostConfigure(name, server);
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, LoginOptions options)
    {
        PostConfigure(name, (IConnectionInfo)options);
        PostConfigure(name, options.WorldRedirect);

        if (Point.TryParse(options.StartingPointStr, out var point))
            options.StartingPoint = point;
        else
            throw new ConfigurationErrorsException($"Unable to parse starting point from config ({options.StartingPointStr})");
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, MetaDataStoreOptions options)
    {
        options.UseBaseDirectory(StagingDirectory.StagingDirectory);

        // ReSharper disable once ArrangeMethodOrOperatorBody
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(MagicPrefixScript.Mutate));

        //dyeable mutator
        options.PrefixMutators.Add(
            ItemMetaNodeMutator.Create(
                (node, template) =>
                {
                    if (!template.IsDyeable)
                        return [];

                    return Enum.GetNames<DisplayColor>()
                               .Select(
                                   colorName => node with
                                   {
                                       Name = $"{colorName} {node.Name}"
                                   });
                }));

        //add more mutators here
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(SwiftPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(AiryPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(AncientPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(BlazingPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(BreezyPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(BrightPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(BrilliantPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(CripplingPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(CursedPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(DarkenedPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(EternalPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(HastyPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(HazyPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(HowlingPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(LuckyPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(MeagerPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(MightyPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(MinorPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(ModestPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(MysticalPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(NimblePrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(PersistingPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(PotentPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(PowerfulPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(PrecisionPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(RuthlessPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(SavagePrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(SerenePrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(ShroudedPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(SkillfulPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(SoftPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(SoothingPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(TightPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(TinyPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(ToughPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(ValiantPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(WhirlingPrefixScript.Mutate));
        options.PrefixMutators.Add(ItemMetaNodeMutator.Create(WisePrefixScript.Mutate));
    }
    

    /// <inheritdoc />
    public void PostConfigure(string? name, WorldOptions options)
    {
        PostConfigure(name, (IConnectionInfo)options);
        PostConfigure(name, options.LoginRedirect);

        foreach (var settings in options.DefaultChannels)
        {
            settings.ChannelName = ChannelService.PrependPrefix(settings.ChannelName);

            ChannelService.RegisterChannel(
                null,
                settings.ChannelName,
                settings.MessageColor ?? CHAOS_CONSTANTS.DEFAULT_CHANNEL_MESSAGE_COLOR,
                Helpers.DefaultChannelMessageHandler,
                true);
        }

        WorldOptions.Instance = options;
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, ItemTemplate options) => throw new NotImplementedException();
}