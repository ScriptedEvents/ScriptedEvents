﻿namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.IO;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using ScriptedEvents.API.Helpers;

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

            if (ScriptHelper.RunningScripts.Count == 0)
            {
                response = "Zero scripts are currently running.";
                return true;
            }

            int amount = ScriptHelper.StopAllScripts();

            response = $"Done! Stopped execution of {amount} scripts.";
            return true;
        }
    }
}
