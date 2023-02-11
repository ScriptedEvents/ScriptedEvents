﻿using CommandSystem;
using Exiled.Permissions.Extensions;
using System;
using System.IO;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.API.Features;
using ScriptedEvents.API.Features.Exceptions;

namespace ScriptedEvents.Handlers.Commands.MainCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ExecuteScript : ICommand
    {
        public string Command => "execute";

        public string[] Aliases => new[] { "ex", "run"};

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

            try
            {
                Script scr = ScriptHelper.ReadScript(arg0);

                if (!sender.CheckPermission(scr.ExecutePermission))
                {
                    response = $"Missing permission: {scr.ExecutePermission}";
                    return false;
                }

                response = $"Executed {arg0} successfully.";
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
