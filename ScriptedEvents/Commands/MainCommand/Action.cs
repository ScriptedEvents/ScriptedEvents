namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.IO;
    using System.Linq;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Action : ICommand
    {
        public string Command => "action";

        public string[] Aliases => new[] { "act" };

        public string Description => "Runs a specific action with specific arguments.";

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

            if (!ScriptHelper.ActionTypes.TryGetValue(actionName.ToUpper(), out Type argType))
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

            scriptAction.Arguments = arguments.Skip(1).ToArray();

            // Fill out mock script info
            Script mockScript = new Script();
            mockScript.Context = ExecuteContext.ServerConsole;
            mockScript.Sender = sender;
            mockScript.RawText = string.Join(" ", arguments);
            mockScript.ScriptName = "ACTION COMMAND";
            mockScript.Flags.Add("ACTIONCOMMANDEXECUTION");

            ActionResponse actionResponse = scriptAction.Execute(mockScript);

            response = string.IsNullOrWhiteSpace(actionResponse.Message) ? "Done" : actionResponse.Message;

            mockScript.Dispose();
            return actionResponse.Success;
        }
    }
}
