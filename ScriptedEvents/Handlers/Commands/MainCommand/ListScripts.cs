using CommandSystem;
using Exiled.API.Features.Pools;
using Exiled.Permissions.Extensions;
using ScriptedEvents.API.Helpers;
using System;
using System.IO;
using System.Text;

namespace ScriptedEvents.Handlers.Commands.MainCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListScripts : ICommand
    {
        public string Command => "list";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Lists all event scripts.";

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

            var files = Directory.GetFiles(ScriptHelper.ScriptPath, "*.txt", SearchOption.TopDirectoryOnly);
            StringBuilder bldr = StringBuilderPool.Pool.Get();

            foreach (string file in files)
            {
                string scriptName = Path.GetFileNameWithoutExtension(file);
                string permission = "script.execute";
                DateTime edited = File.GetLastWriteTimeUtc(file);
                DateTime read = File.GetLastAccessTimeUtc(file);
                if (MainPlugin.Singleton.Config.RequiredPermissions.TryGetValue(scriptName, out string perm2))
                {
                    permission += $".{perm2}";
                }
                bldr.AppendLine($"{scriptName} (perm: {permission}) (last ran: {read:g}) (edited: {edited:g})");
            }

            response = $"All found scripts: \n\n{StringBuilderPool.Pool.ToStringReturn(bldr)}";
            return true;
        }
    }
}
