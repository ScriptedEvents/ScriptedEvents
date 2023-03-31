namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.IO;
    using System.Text;
    using CommandSystem;
    using Exiled.API.Features.Pools;
    using Exiled.Permissions.Extensions;
    using ScriptedEvents.API.Helpers;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListRunning : ICommand
    {
        /// <inheritdoc/>
        public string Command => "listrunning";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "running", "listr", "lr" };

        /// <inheritdoc/>
        public string Description => "Lists all event scripts that are currently running.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.list"))
            {
                response = "Missing permission: script.list";
                return false;
            }

            if (!Directory.Exists(ScriptHelper.ScriptPath))
            {
                response = "Critical error: Missing script path. Please reload plugin.";
                return true;
            }

            StringBuilder bldr = StringBuilderPool.Pool.Get();

            int i = 0;
            foreach (var scriptPair in ScriptHelper.RunningScripts)
            {
                if (!scriptPair.Key.IsRunning)
                    continue;

                i++;

                Script script = scriptPair.Key;
                bldr.AppendLine($"[{i}] {script.ScriptName} | Executed by: {script.Sender?.LogName ?? "Automatic"} | {script.RunDate:g}");
            }

            response = $"All running scripts: \n\n{StringBuilderPool.Pool.ToStringReturn(bldr)}";
            return true;
        }
    }
}
