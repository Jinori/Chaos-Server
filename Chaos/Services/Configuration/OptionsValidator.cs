#region
using Chaos.Networking.Options;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Services.Configuration;

public sealed class OptionsValidator(ILogger<OptionsValidator> logger) : IValidateOptions<LobbyOptions>, IValidateOptions<ChaosOptions>
{
    private readonly ILogger<OptionsValidator> Logger = logger;

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, ChaosOptions options)
    {
        if (string.IsNullOrEmpty(options.StagingDirectory))
            return ValidateOptionsResult.Fail("StagingDirectory is required");

        return ValidateOptionsResult.Success;
    }

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, LobbyOptions options)
    {
        foreach (var server in options.Servers)
        {
            if (server.Description.Length > 18)
            {
                Logger.WithTopics(Topics.Servers.LobbyServer, Topics.Entities.Options, Topics.Actions.Validation)
                      .WithProperty(server.Description)
                      .LogError(
                          "Value for {@OptionName} is too long, trimming it to 18 characters",
                          $"{nameof(LobbyOptions)}:{nameof(LobbyOptions.Servers)}:{nameof(LoginServerInfo.Description)}");

                server.Description = server.Description[..18];
            }

            if (server.Name.Length > 9)
            {
                Logger.WithTopics(Topics.Servers.LobbyServer, Topics.Entities.Options, Topics.Actions.Validation)
                      .WithProperty(server.Name)
                      .LogError(
                          "Value for {@OptionName} is too long, trimming it to 9 characters",
                          $"{nameof(LobbyOptions)}:{nameof(LobbyOptions.Servers)}:{nameof(LoginServerInfo.Name)}");

                server.Name = server.Name[..9];
            }
        }

        return ValidateOptionsResult.Success;
    }
}