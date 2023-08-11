using System.Configuration;
using System.Net;
using System.Net.Sockets;
using Chaos.Common.Abstractions;
using Chaos.MetaData;
using Chaos.MetaData.ItemMetadata;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.ItemScripts.Enchantments;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Configuration;

public sealed class OptionsConfigurer : IPostConfigureOptions<IConnectionInfo>,
                                        IPostConfigureOptions<LobbyOptions>,
                                        IPostConfigureOptions<LoginOptions>,
                                        IPostConfigureOptions<WorldOptions>,
                                        IPostConfigureOptions<MetaDataStoreOptions>

{
    private readonly IStagingDirectory StagingDirectory;
    public OptionsConfigurer(IStagingDirectory stagingDirectory) => StagingDirectory = stagingDirectory;

    /// <inheritdoc />
    public void PostConfigure(string? name, IConnectionInfo options)
    {
        if (!string.IsNullOrEmpty(options.HostName))
            options.Address = Dns.GetHostAddresses(options.HostName).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)!;
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
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(MagicPrefixScript.Mutate));
        //add more mutators here
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(SwiftPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(AiryPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(AncientPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(BlazingPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(BreezyPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(BrightPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(BrilliantPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(CripplingPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(CursedPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(DarkPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(EternalPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(HastyPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(HazyPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(HowlingPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(LuckyPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(MeagerPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(MightyPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(MinorPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(ModestPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(MysticalPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(NimblePrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(PersistingPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(PotentPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(PowerfulPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(PrecisionPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(RuthlessPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(SavagePrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(SerenePrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(ShroudedPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(SkillfulPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(SoftPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(SoothingPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(TightPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(TinyPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(ToughPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(ValiantPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(WhirlingPrefixScript.Mutate));
        options.PrefixMutators.Add(MetaNodeMutator<ItemMetaNode>.Create(WisePrefixScript.Mutate));
    }
    

    /// <inheritdoc />
    public void PostConfigure(string? name, WorldOptions options)
    {
        PostConfigure(name, (IConnectionInfo)options);
        PostConfigure(name, options.LoginRedirect);

        WorldOptions.Instance = options;
    }
}