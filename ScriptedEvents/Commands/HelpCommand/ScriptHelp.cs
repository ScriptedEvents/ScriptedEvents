namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.Linq;

    using CommandSystem;

    using ScriptedEvents.Actions;
    using ScriptedEvents.API.Enums;
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
            HelpAction help = new HelpAction();
            help.Arguments = arguments.ToArray();

            // Fill out mock script info
            Script mockScript = new Script();
            mockScript.Context = ExecuteContext.ServerConsole;
            mockScript.Sender = sender;
            mockScript.RawText = $"HELP {string.Join(" ", arguments)}";
            mockScript.ScriptName = "HELP COMMAND EXECUTION";
            mockScript.AddFlag("HELPCOMMANDEXECUTION");

            ActionResponse actionResponse = help.Execute(mockScript);

            response = string.IsNullOrWhiteSpace(actionResponse.Message) ? "Done" : actionResponse.Message;

            mockScript.Dispose();
            return actionResponse.Success;
        }
    }
}
