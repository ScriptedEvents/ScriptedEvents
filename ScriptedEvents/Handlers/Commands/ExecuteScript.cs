using CommandSystem;
using Exiled.Permissions.Extensions;
using System;
using System.IO;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.API.Features;
using System.Text;
using Exiled.API.Features.Pools;
using MEC;
using Exiled.API.Features;
using ScriptedEvents.API.Features.Exceptions;

namespace ScriptedEvents.Handlers.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ExecuteScript : ICommand
    {
        public string Command => "executescript";

        public string[] Aliases => new[] { "es", "exscript" };

        public string Description => "Start an event";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("es.execute"))
            {
                response = "Missing permission: es.execute";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Missing argument: script name";
                return false;
            }

            if (!Directory.Exists(ScriptHelper.ScriptPath))
            {
                response = "Critical error: Missing script path. Please reload plugin.";
                return true;
            }

            var arg0 = arguments.At(0);
            if (arg0 == "list")
            {
                var files = Directory.GetFiles(ScriptHelper.ScriptPath, "*.txt", SearchOption.TopDirectoryOnly);
                StringBuilder bldr = StringBuilderPool.Pool.Get();

                foreach (string file in files)
                {
                    string scriptName = Path.GetFileNameWithoutExtension(file);
                    string permission = "es.execute";
                    DateTime edited = File.GetLastWriteTime(file);
                    if (MainPlugin.Singleton.Config.RequiredPermissions.TryGetValue(scriptName, out string perm2))
                    {
                        permission += $".{perm2}";
                    }
                    bldr.AppendLine($"{scriptName} (perm: {permission}) (edited: {edited:g})");
                }

                response = $"All found scripts: \n\n{StringBuilderPool.Pool.ToStringReturn(bldr)}";
                return true;
            }

            if (arg0 == "stopall")
            {
                if (!sender.CheckPermission("es.execute.stopall"))
                {
                    response = "Missing permission: es.execute.stopall";
                    return false;
                }

                int amount = ScriptHelper.StopAllScripts();

                response = $"Done! Stopped execution of {amount} scripts.";
                return true;
            }

            if (MainPlugin.Singleton.Config.RequiredPermissions.TryGetValue(arg0, out string perm))
            {
                if (!sender.CheckPermission("es.execute." + perm))
                {
                    response = $"Missing permission: es.execute.{perm}";
                    return false;
                }
            }

            try
            {
                Script scr = ScriptHelper.ReadScript(arg0);

                ScriptHelper.RunScript(scr);
                response = $"Executed {scr.ScriptName} successfully.";
            }
            catch (DisabledScriptException)
            {
                response = $"Script '{arg0}' is disabled.";
                return false;
            }
            catch (FileNotFoundException)
            {
                response = $"Script '{arg0}' not found.";
                return false;
            }

            return true;
        }
    }
}
