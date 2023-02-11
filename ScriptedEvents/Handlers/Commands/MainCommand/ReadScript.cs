using CommandSystem;
using Exiled.Permissions.Extensions;
using System;
using System.IO;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.API.Features;
using ScriptedEvents.API.Features.Exceptions;
using System.Text;
using Exiled.API.Features.Pools;
using ScriptedEvents.API.Features.Actions;

namespace ScriptedEvents.Handlers.Commands.MainCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ReadScript : ICommand
    {
        public string Command => "readscript";

        public string[] Aliases => new[] { "read", "r"};

        public string Description => "Start a script.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.read"))
            {
                response = "Missing permission: script.read";
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
                if (!sender.CheckPermission("script.read." + perm))
                {
                    response = $"Missing permission: script.read.{perm}";
                    return false;
                }
            }

            try
            {
                Script scr = ScriptHelper.ReadScript(arg0);

                StringBuilder sb = StringBuilderPool.Pool.Get();
                sb.AppendLine($"Reading file {arg0}...");
                sb.AppendLine($"Script Name: {scr.ScriptName}");
                sb.AppendLine($"Script Flags: {(scr.Flags.Count > 0 ? string.Join(", ", scr.Flags) : "None")}\n");
                sb.AppendLine("---- START OF FILE ----");

                int curLine = 0;
                foreach (IAction action in scr.Actions)
                {
                    curLine++;
                    if (action is ICustomReadDisplay display)
                    {
                        if (!display.Read(out string stringDisplay))
                            continue;
                        sb.AppendLine($"{curLine}\t{stringDisplay}");
                    }
                    else
                    {
                        sb.AppendLine($"{curLine}\t{action.Name} {(action.Arguments is not null ? string.Join(" ", action.Arguments) : string.Empty)}");
                    }
                }

                sb.AppendLine("---- END OF FILE ----");

                response = StringBuilderPool.Pool.ToStringReturn(sb);
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
