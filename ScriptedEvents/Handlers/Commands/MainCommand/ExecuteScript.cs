﻿using CommandSystem;
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

namespace ScriptedEvents.Handlers.Commands.MainCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ExecuteScript : ICommand
    {
        public string Command => "execute";

        public string[] Aliases => new[] { "ex", };

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

            var arg0 = arguments.At(0);

            if (MainPlugin.Singleton.Config.RequiredPermissions.TryGetValue(arg0, out string perm))
            {
                if (!sender.CheckPermission("script.execute." + perm))
                {
                    response = $"Missing permission: script.execute.{perm}";
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