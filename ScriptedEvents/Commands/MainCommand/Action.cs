namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandSystem;

    using Exiled.Permissions.Extensions;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Action : ICommand
    {
        /// <inheritdoc/>
        public string Command => "action";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "act" };

        /// <inheritdoc/>
        public string Description => "Runs a specific action with specific arguments.";

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

            string actionName = arguments.ElementAt(0);
            if (string.IsNullOrWhiteSpace(actionName))
            {
                response = "Missing the name of the action to execute.";
                return false;
            }

            if (!ScriptHelper.TryGetActionType(actionName.ToUpper(), out Type argType))
            {
                response = "Invalid argument name provided.";
                return false;
            }

            IAction action = Activator.CreateInstance(argType) as IAction;

            if (action is not IScriptAction scriptAction)
            {
                response = "This action cannot be executed.";
                return false;
            }

            if (action is ILogicAction)
            {
                response = "Logic actions cannot be used in the action command.";
                return false;
            }

            if (action is Actions.CommandAction)
            {
                response = "This action cannot be used in the action command.";
                return false;
            }

            scriptAction.Arguments = arguments.Skip(1).ToArray();

            // Fill out mock script info
            Script mockScript = new()
            {
                Context = sender is ServerConsoleSender ? ExecuteContext.ServerConsole : ExecuteContext.RemoteAdmin,
                Sender = sender,
                RawText = string.Join(" ", arguments),
                ScriptName = "ACTION COMMAND EXECUTION",
            };

            if (MainPlugin.Configs.Debug)
                mockScript.Flags.Add("DEBUG");

            mockScript.Flags.Add("ACTIONCOMMANDEXECUTION");

            ActionResponse actionResponse = scriptAction.Execute(mockScript);

            response = string.IsNullOrWhiteSpace(actionResponse.Message) ? "Done" : actionResponse.Message;

            mockScript.Dispose();
            return actionResponse.Success;
        }
    }
}
