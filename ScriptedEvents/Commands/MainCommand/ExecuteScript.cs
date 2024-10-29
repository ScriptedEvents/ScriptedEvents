using ScriptedEvents.Structures;

namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.IO;
    using System.Linq;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Modules;

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
        public bool SanitizeResponse => true;

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

            if (!Directory.Exists(ScriptModule.BasePath))
            {
                response = "Critical error: Missing script path. Please reload plugin.";
                return false;
            }

            string arg0 = arguments.At(0);
            Script? scr = default;

            if (!MainPlugin.ScriptModule.TryParseScript(arg0, sender, out var script, out var trace))
            {
                response = trace!.Format();
            }

            scr = script!;

            if (!sender.CheckPermission(scr.ExecutePermission))
            {
                response = $"Missing permission: {scr.ExecutePermission}";
                return false;
            }

            if (sender is PlayerCommandSender playerSender && Player.TryGet(playerSender, out Player plr))
            {
                scr.AddPlayerVariable("@SENDER", new[] { plr }, true);
            }

            for (int i = 1; i < 20; i++)
            {
                if (arguments.Count <= i)
                    break;

                scr.DebugLog($"Assigned $ARG{i} variable to executed script.");
                scr.AddLiteralVariable($"$ARG{i}", arguments.At(i), true);
            }

            scr.AddLiteralVariable("$ARGS", string.Join(" ", arguments.Skip(1)), true);

            if (!MainPlugin.ScriptModule.TryRunScript(scr, out var err))
            {
                response = err!.Format();
            }

            response = $"Script '{scr.ScriptName}' executed successfully.";

            return true;
        }
    }
}
