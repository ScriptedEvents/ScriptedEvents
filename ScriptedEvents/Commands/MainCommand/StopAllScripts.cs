namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.IO;

    using CommandSystem;

    using Exiled.Permissions.Extensions;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Modules;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StopAllScripts : ICommand
    {
        /// <inheritdoc/>
        public string Command => "stopall";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Stops all scripts currently running.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.stopall"))
            {
                response = "Missing permission: script.stopall";
                return false;
            }

            if (!Directory.Exists(ScriptModule.BasePath))
            {
                response = ErrorGen.Get(ErrorCode.IOMissing);
                return false;
            }

            if (MainPlugin.ScriptModule.RunningScripts.Count == 0)
            {
                response = "Zero scripts are currently running.";
                return true;
            }

            int amount = MainPlugin.ScriptModule.StopAllScripts();

            response = $"Done! Stopped execution of {amount} scripts.";
            return true;
        }
    }
}
