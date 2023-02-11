using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using Exiled.Permissions.Extensions;
using ScriptedEvents.API.Helpers;
using System;
using System.IO;
using System.Text;
using UnityEngine.Android;

namespace ScriptedEvents.Commands.MainCommand
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

            string[] files = Directory.GetFiles(ScriptHelper.ScriptPath, "*.txt", SearchOption.AllDirectories);
            StringBuilder bldr = StringBuilderPool.Pool.Get();

            foreach (string file in files)
            {
                try
                {
                    Script scr = ScriptHelper.ReadScript(Path.GetFileNameWithoutExtension(file));
                    bldr.AppendLine($"{scr.ScriptName} (perm: {scr.ExecutePermission}) (last ran: {scr.LastRead:g}) (edited: {scr.LastEdited:g})");
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
