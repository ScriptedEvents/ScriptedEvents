namespace ScriptedEvents.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;

    public class CustomCommand : ICommand
    {
        public string Command { get; set; }

        public string[] Aliases { get; set; }

        public string Description { get; set; }

        public CommandType Type { get; set; }

        public string[] Scripts { get; set; }

        public string Permission { get; set; }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(Permission))
            {
                response = $"Missing permission: {Permission}";
            }

            Dictionary<string, string> failed = new();

            foreach (string scr in Scripts)
            {
                try
                {
                    ScriptHelper.ReadAndRun(scr, sender);
                }
                catch (DisabledScriptException)
                {
                    failed.Add(scr, "Script is disabled!");
                }
                catch (FileNotFoundException)
                {
                    failed.Add(scr, "Script not found!");
                }
            }

            if (failed.Count > 0)
            {
                response = $"{failed.Count} failed scripts:";
                foreach (var kvp in failed)
                {
                    response += $"\n{kvp.Key} - {kvp.Value}";
                }

                return false;
            }

            response = "Success.";
            return true;
        }
    }
}
