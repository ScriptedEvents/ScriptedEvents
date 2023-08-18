namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.IO;
    using System.Text;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Permissions.Extensions;
    using ScriptedEvents.API.Features;

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

            if (!Directory.Exists(ScriptHelper.ScriptPath))
            {
                response = "Critical error: Missing script path. Please reload plugin. [Error Code: SE-127]";
                return true;
            }

            string[] files = Directory.GetFiles(ScriptHelper.ScriptPath, "*.txt", SearchOption.AllDirectories);
            StringBuilder bldr = StringBuilderPool.Pool.Get();

            int i = 0;

            foreach (string file in files)
            {
                if (File.ReadAllText(file).Contains("!-- HELPRESPONSE"))
                    continue;

                try
                {
                    Script scr = ScriptHelper.ReadScript(Path.GetFileNameWithoutExtension(file), sender, true);

                    i++;

                    bldr.AppendLine($"[{i}] {scr.ScriptName} (perm: {scr.ExecutePermission}) (last ran: {scr.LastRead:g}) (edited: {scr.LastEdited:g})");

                    scr.Dispose();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            response = $"All found scripts: \n\n{StringBuilderPool.Pool.ToStringReturn(bldr)}";
            return true;
        }
    }
}
