using CommandSystem;
using Exiled.Permissions.Extensions;
using System;
using System.IO;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.API.Features;
using System.Linq;

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

            var arg0 = arguments.At(0);
            if (arg0 == "list")
            {
                var files = Directory.GetFiles(ScriptHelper.ScriptPath, "*.txt", SearchOption.TopDirectoryOnly);
                response = $"All found scripts: \n\n{string.Join(",\n", files.Select(x => Path.GetFileName(x)))}";
                return true;
            }

            try
            {
                Script scr = ScriptHelper.ReadScript(arg0);

                ScriptHelper.RunScript(scr);
                response = $"Executed {scr.ScriptName} successfully.";
            }
            catch (FileNotFoundException _)
            {
                response = $"Script '{arg0}' not found.";
                return false;
            }

            return true;
        }
    }
}
