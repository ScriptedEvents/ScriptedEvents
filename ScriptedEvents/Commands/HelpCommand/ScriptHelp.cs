﻿namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.Linq;
    using CommandSystem;
    using ScriptedEvents.Actions;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ScriptHelp : ICommand
    {
        /// <inheritdoc/>
        public string Command => "scripthelp";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "shelp", "scrhelp", "sehelp", "seh", "sch" };

        /// <inheritdoc/>
        public string Description => "Get documentation for everything in Scripted Events.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Missing the API to get the help of. Must be 'LIST', 'LISTVAR', or the name of an action or variable.";
                return false;
            }

            string name = arguments.ElementAt(0);
            if (string.IsNullOrWhiteSpace(name))
            {
                response = "Missing the API to get the help of. Must be 'LIST', 'LISTVAR', or the name of an action or variable.";
                return false;
            }

            HelpAction help = new HelpAction();
            help.Arguments = new[] { name };

            // Fill out mock script info
            Script mockScript = new Script();
            mockScript.Context = ExecuteContext.ServerConsole;
            mockScript.Sender = sender;
            mockScript.RawText = $"HELP {name}";
            mockScript.ScriptName = "HELP COMMAND EXECUTION";
            mockScript.Flags.Add("HELPCOMMANDEXECUTION");

            ActionResponse actionResponse = help.Execute(mockScript);

            response = string.IsNullOrWhiteSpace(actionResponse.Message) ? "Done" : actionResponse.Message;

            mockScript.Dispose();
            return actionResponse.Success;
        }
    }
}
