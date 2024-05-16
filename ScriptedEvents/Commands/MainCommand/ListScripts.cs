namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features.Pools;
    using Exiled.Permissions.Extensions;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Modules;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListScripts : ICommand
    {
        /// <inheritdoc/>
        public string Command => "list";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Lists all event scripts.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.list"))
            {
                response = "Missing permission: script.list";
                return false;
            }

            if (!Directory.Exists(ScriptModule.BasePath))
            {
                response = ErrorGen.Get(ErrorCode.IOMissing);
                return false;
            }

            List<Script> scripts = MainPlugin.ScriptModule.ListScripts(sender);
            StringBuilder bldr = StringBuilderPool.Pool.Get();

            int i = 0;

            foreach (Script scr in scripts)
            {
                i++;
                bldr.AppendLine($"[{i}] {scr.ScriptName} (perm: {scr.ExecutePermission}) (last ran: {scr.LastRead:g}) (edited: {scr.LastEdited:g})");
                scr.Dispose();
            }

            response = $"All found scripts ({i}): \n\n{StringBuilderPool.Pool.ToStringReturn(bldr)}";
            return true;
        }
    }
}
