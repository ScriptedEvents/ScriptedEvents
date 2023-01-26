using CommandSystem;
using Exiled.Permissions.Extensions;
using System;
using System.IO;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.API.Features;

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

            try
            {
                Script scr = ScriptHelper.ReadScript(arguments.At(0));

                ScriptHelper.RunScript(scr);
                response = $"Executed {scr.ScriptName} successfully.";
            }
            catch (FileNotFoundException _)
            {
                response = $"Script '{arguments.At(0)}' not found.";
                return false;
            }

            return true;
        }
    }
}
