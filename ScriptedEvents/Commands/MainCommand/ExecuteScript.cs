﻿namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.IO;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ExecuteScript : ICommand
    {
        /// <inheritdoc/>
        public string Command => "execute";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "ex", "run" };

        /// <inheritdoc/>
        public string Description => "Start a script.";

        /// <inheritdoc/>
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

                if (sender is PlayerCommandSender playerSender && Player.TryGet(playerSender, out Player plr))
                {
                    scr.AddPlayerVariable("{SENDER}", "The player who executed the script.", new[] { plr });
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
