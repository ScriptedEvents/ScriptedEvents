using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.Linq;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using ScriptedEvents.Actions;
    using ScriptedEvents.API.Features;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Action : ICommand
    {
        /// <inheritdoc/>
        public string Command => "action";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "act" };

        /// <inheritdoc/>
        public string Description => "Runs a specific action with specific arguments.";

        public bool SanitizeResponse => true;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.action"))
            {
                response = "Missing permission: script.action";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Missing the name of the action to execute.";
                return false;
            }

            var actionName = arguments.ElementAt(0);
            if (string.IsNullOrWhiteSpace(actionName))
            {
                response = "Missing the name of the action to execute.";
                return false;
            }

            if (!MainPlugin.ScriptModule.TryGetActionType(actionName, out var actionType, out var err))
            {
                response = $"Action {actionName} is not a valid action type.";
            }

            if (actionType is not IScriptAction scriptAction)
            {
                response = "This action cannot be executed using the command.";
                return false;
            }

            scriptAction.RawArguments = arguments.Skip(1).ToArray();
            scriptAction.Arguments = arguments.Skip(1).ToArray<object>();

            // Fill out mock script info
            Script mockScript = new()
            {
                Context = sender is ServerConsoleSender ? ExecuteContext.ServerConsole : ExecuteContext.RemoteAdmin,
                Sender = sender,
                RawText = string.Join(" ", arguments),
                ScriptName = "ACTION COMMAND",
                Actions = new IAction[] { scriptAction },
            };

            mockScript.OriginalActionArgs[scriptAction] = scriptAction.RawArguments;
            mockScript.ResultVariableNames[scriptAction] = Array.Empty<string>();

            if (sender is PlayerCommandSender playerSender && Player.TryGet(playerSender, out Player plr))
            {
                mockScript.AddPlayerVariable("@SENDER", new[] { plr }, true);
            }

            if (MainPlugin.Configs.Debug)
                mockScript.AddFlag("DEBUG");

            mockScript.AddFlag("ACTIONCOMMANDEXECUTION");

            if (!MainPlugin.ScriptModule.TryRunScript(mockScript, out var trace))
            {
                response = $"Failed to run the action.\n{trace!.Format()}";
                mockScript.Dispose();
            }

            response = "Done";
            return true;
        }
    }
}
