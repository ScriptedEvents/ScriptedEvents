using ScriptedEvents.API.Features;

namespace ScriptedEvents.Commands.MainCommand
{
    using System;

    using CommandSystem;

    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ExecuteAutorunScript : ICommand
    {
        /// <inheritdoc/>
        public string Command => "executeautorun";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "exa", "runa" };

        /// <inheritdoc/>
        public string Description => "Start all scripts marked as autorun.";

        /// <inheritdoc/>
        public bool SanitizeResponse => true;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.execute-autorun"))
            {
                response = "Missing permission: script.execute-autorun";
                return false;
            }

            string ranScripts = string.Empty;
            foreach (string scrName in ScriptModule.Singleton!.AutoRunScripts)
            {
                ScriptModule.Singleton!.TryReadAndRun(scrName, sender, out var trace);

                if (trace != null)
                {
                    Logger.Error(trace);
                    continue;
                }

                ranScripts += scrName + ", ";
            }

            if (ScriptModule.Singleton!.AutoRunScripts.Count > 0)
            {
                response = $"Running scripts: {ranScripts.Remove(ranScripts.Length - 2)}";
                return true;
            }

            response = $"There are 0 autorun scripts.";
            return true;
        }
    }
}
