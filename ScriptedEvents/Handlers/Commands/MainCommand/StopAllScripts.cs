using CommandSystem;
using Exiled.Permissions.Extensions;
using ScriptedEvents.API.Helpers;
using System;
using System.IO;

namespace ScriptedEvents.Handlers.Commands.MainCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StopAllScripts : ICommand
    {
        public string Command => "stopall";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Stops all scripts currently running.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.stopall"))
            {
                response = "Missing permission: script.stopall";
                return false;
            }

            if (!Directory.Exists(ScriptHelper.ScriptPath))
            {
                response = "Critical error: Missing script path. Please reload plugin.";
                return true;
            }

            int amount = ScriptHelper.StopAllScripts();

            response = $"Done! Stopped execution of {amount} scripts.";
            return true;
        }
    }
}
