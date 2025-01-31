#region
using System.Collections.Frozen;
using System.Reflection;
using System.Text;
using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Messaging;

/// <summary>
///     A service used to handle the detection and execution of commands execute by an object of a given type
/// </summary>
/// <typeparam name="T">
///     The type of the object executing commands
/// </typeparam>
/// <typeparam name="TOptions">
///     The type of the options object to use for this command interceptor
/// </typeparam>
public sealed class CommandInterceptor<T, TOptions> : ICommandInterceptor<T> where T: ICommandSubject
                                                                             where TOptions: class, ICommandInterceptorOptions
{
    private readonly IDictionary<string, CommandDescriptor> Commands;
    private readonly ILogger<CommandInterceptor<T, TOptions>> Logger;
    private readonly TOptions Options;
    private readonly IServiceProvider ServiceProvider;

    /// <summary>
    ///     Creates a new instance of <see cref="CommandInterceptor{T, TOptions}" />
    /// </summary>
    public CommandInterceptor(IServiceProvider serviceProvider, IOptions<TOptions> options, ILogger<CommandInterceptor<T, TOptions>> logger)
    {
        ServiceProvider = serviceProvider;
        Options = options.Value;
        Logger = logger;
        Commands = new Dictionary<string, CommandDescriptor>(StringComparer.OrdinalIgnoreCase);

        var descriptors = typeof(ICommand<T>).LoadImplementations()
                                             .Select(type => (Type: type, Attributes: type.GetCustomAttributes<CommandAttribute>()))
                                             .SelectMany(
                                                 attributeInfo => attributeInfo.Attributes.Select(
                                                     attribute => new CommandDescriptor
                                                     {
                                                         Type = attributeInfo.Type,
                                                         Details = attribute
                                                     }));

        Commands = descriptors.ToFrozenDictionary(descriptor => descriptor.Details.CommandName, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     async is intentional, so that the try/catch handles any exception that comes from executing the command
    /// </remarks>
    public async ValueTask HandleCommandAsync(T source, string commandStr)
    {
        var command = commandStr[1..];
        var commandParts = new ArgumentCollection(command);

        if (commandParts.Count == 0)
            return;

        if (!commandParts.TryGetNext<string>(out var commandName))
            return;

        if (!Commands.TryGetValue(commandName, out var descriptor))
            return;

        Logger.WithTopics(Topics.Entities.Command, Topics.Actions.Execute)
              .WithProperty(source)
              .LogDebug("Handling command {@CommandStr}", commandStr);

        if (descriptor.Details.RequiresAdmin && !source.IsAdmin)
        {
            Logger.WithTopics(Topics.Entities.Command, Topics.Actions.Execute)
                  .WithProperty(source)
                  .LogWarning(
                      "Non-Admin {@SourceType} {@SourceName} tried to execute admin command {@CommandStr}",
                      source.GetType()
                            .Name,
                      source.Name,
                      commandStr);

            return;
        }

        try
        {
            var commandInstance = (ICommand<T>)ActivatorUtilities.CreateInstance(ServiceProvider, descriptor.Type);

            Logger.WithTopics(Topics.Entities.Command, Topics.Actions.Create)
                  .LogTrace("Successfully created command {@CommandName}", commandName);

            if (commandName.EqualsI("help") || commandName.EqualsI("commands"))
            {
                var helpStr = BuildHelpText(source);

                commandParts = new ArgumentCollection(
                    commandParts.Take(1)
                                .Append(helpStr));
            }

            var commandArgs = new ArgumentCollection(commandParts.Skip(1));

            async ValueTask InnerExecute()
            {
                await commandInstance.ExecuteAsync(source, new ArgumentCollection(commandArgs));

                Logger.WithTopics(Topics.Entities.Command, Topics.Actions.Execute)
                      .WithProperty(source)
                      .LogInformation(
                          "{@SourceType} {@SourceName} executed {@CommandStr}",
                          source.GetType()
                                .Name,
                          source.Name,
                          commandStr);
            }

            await InnerExecute();
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Entities.Command, Topics.Actions.Execute)
                  .WithProperty(source)
                  .LogError(
                      e,
                      "{@SourceType} {@SourceName} failed to execute {@Command}",
                      source.GetType()
                            .Name,
                      source.Name,
                      commandStr);
        }
    }

    /// <inheritdoc />
    public bool IsCommand(string commandStr) => commandStr.StartsWithI(Options.Prefix);

    private string BuildHelpText(T source)
    {
        var commands = Commands.Values
                               .Where(
                                   cmd =>
                                   {
                                       if (cmd.Details.RequiresAdmin)
                                           return source.IsAdmin;

                                       return true;
                                   })
                               .OrderBy(cmd => cmd.Details.CommandName)
                               .ToList();

        var builder = new StringBuilder();

        builder.Append(MessageColor.Orange.ToPrefix());
        builder.AppendLine("Available Commands:");

        var longestCommandName = 3 + commands.Max(cmd => cmd.Details.CommandName.Length);

        foreach (var command in commands)
        {
            builder.Append($"{MessageColor.White.ToPrefix()}{Options.Prefix}{command.Details.CommandName}".PadRight(longestCommandName));
            builder.Append(MessageColor.Yellow.ToPrefix());
            builder.Append(command.Details.HelpText);
            builder.Append('\n');
        }

        return builder.ToString();
    }
}