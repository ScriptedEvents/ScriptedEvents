namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features.Pools;
    using Exiled.Permissions.Extensions;

    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;

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
                return false;
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

            if (MainPlugin.Configs.Debug)
            {
                i = 0;
                StringBuilder corobldr = StringBuilderPool.Pool.Get();
                foreach (var data in CoroutineHelper.GetAll())
                {
                    foreach (CoroutineData item in data.Value)
                    {
                        i++;
                        corobldr.AppendLine($"[{i}] TYPE: {data.Key} TAG: {item.Key ?? (item.Handle.HasValue ? (item.Handle.Value.Tag ?? "N/A") : "N/A")}");
                    }
                }

                response += $"\n[DEBUG] All running coroutines:\n\n{StringBuilderPool.Pool.ToStringReturn(corobldr)}";
            }

            return true;
        }
    }
}
