namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.IO;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Permissions.Extensions;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ReadScript : ICommand
    {
        /// <inheritdoc/>
        public string Command => "readscript";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "readscr", "rsr" };

        /// <inheritdoc/>
        public string Description => "Start a script.";

        /// <inheritdoc/>
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

            if (!Directory.Exists(ScriptModule.BasePath))
            {
                response = ErrorGen.Get(ErrorCode.IOMissing);
                return false;
            }

            string arg0 = arguments.At(0);

            try
            {
                Script scr = MainPlugin.ScriptModule.ReadScript(arg0, sender);

                if (!sender.CheckPermission(scr.ReadPermission))
                {
                    response = $"Missing permission: {scr.ReadPermission}";
                    return false;
                }

                if (scr.Disabled)
                {
                    Log.Warn($"Note: The {scr.ScriptName} script is disabled, and cannot be executed until the DISABLE flag is removed. Script contents still shown below.");
                }

                StringBuilder sb = StringBuilderPool.Pool.Get();
                sb.AppendLine($"Reading file {arg0}...");
                sb.AppendLine($"Script Name: {scr.ScriptName}");
                sb.AppendLine($"Script Flags: {(scr.Flags.Count > 0 ? string.Join(", ", scr.Flags) : "None")}");
                sb.AppendLine($"Last Ran: {scr.LastRead:f}");
                sb.AppendLine($"Last Edited: {scr.LastEdited:f}");
                sb.AppendLine($"Script Path: {scr.FilePath}");
                sb.AppendLine();
                sb.AppendLine("---- START OF FILE ----");

                int curLine = 0;
                foreach (IAction action in scr.Actions)
                {
                    curLine++;
                    if (action is ICustomReadDisplay display)
                    {
                        if (!display.Read(out string stringDisplay))
                            continue;
                        sb.AppendLine($"[{curLine}]\t{stringDisplay}");
                    }
                    else
                    {
                        sb.AppendLine($"[{curLine}]\t{action.Name} {(action.Arguments is not null ? string.Join(" ", action.Arguments) : string.Empty)}");
                    }
                }

                sb.AppendLine("---- END OF FILE ----");

                response = StringBuilderPool.Pool.ToStringReturn(sb);

                scr.Dispose();
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
