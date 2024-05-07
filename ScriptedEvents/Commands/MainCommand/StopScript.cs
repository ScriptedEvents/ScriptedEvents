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
    public class StopScript : ICommand
    {
        /// <inheritdoc/>
        public string Command => "stopscript";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "sts", "stopscr" };

        /// <inheritdoc/>
        public string Description => "Stops a script.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.stop"))
            {
                response = "Missing permission: script.stop";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Missing argument: script name";
                return false;
            }

            if (!Directory.Exists(ScriptModule.BasePath))
            {
                response = ErrorGen.Get(ErrorCode.IOMissing);
                return false;
            }

            string arg0 = arguments.At(0);

            int amount = MainPlugin.ScriptModule.StopScripts(arg0);
            if (amount == 0)
            {
                response = "Done! No scripts were stopped.";
                return true;
            }

            response = $"Done! Stopped execution of {amount} scripts.";
            return true;
        }
    }
}
