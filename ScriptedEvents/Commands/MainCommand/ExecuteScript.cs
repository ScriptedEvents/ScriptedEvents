namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.IO;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Helpers;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ExecuteScript : ICommand
    {
        public string Command => "execute";

        public string[] Aliases => new[] { "ex", "run" };

        public string Description => "Start a script.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.execute"))
            {
                response = "Missing permission: script.execute";
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

            string arg0 = arguments.At(0);
            Script scr = null;

            try
            {
                scr = ScriptHelper.ReadScript(arg0, sender);

                if (!sender.CheckPermission(scr.ExecutePermission))
                {
                    response = $"Missing permission: {scr.ExecutePermission}";
                    return false;
                }

                ScriptHelper.RunScript(scr);

                response = $"Executed {scr.ScriptName} successfully.";
            }
            catch (DisabledScriptException)
            {
                response = $"Script '{scr.ScriptName}' is disabled.";
                scr.Dispose();
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
