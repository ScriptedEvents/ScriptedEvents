namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features.Pools;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ScriptedEventsParent : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptedEventsParent"/> class.
        /// </summary>
        public ScriptedEventsParent() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "scriptedevents";

        /// <inheritdoc/>
        public override string[] Aliases => new[] { "scr", "script" };

        /// <inheritdoc/>
        public override string Description => "Parent command for the Scripted Events plugin.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new ExecuteScript());
            RegisterCommand(new ExecuteAutorunScript());
            RegisterCommand(new ListScripts());
            RegisterCommand(new ListRunning());
            RegisterCommand(new ReadScript());
            RegisterCommand(new StopScript());
            RegisterCommand(new StopAllScripts());
            RegisterCommand(new Action());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder sb = StringBuilderPool.Pool.Get();

            sb.AppendLine("Available commands:");
            sb.AppendLine($"- SCRIPT ACTION <ACTIONNAME> <...ACTIONARGS...> - Executes a single {MainPlugin.Singleton.Name} action, without the need for a script.");
            sb.AppendLine("- SCRIPT EXECUTE <SCRIPTNAME> - Executes the script with the given name. (alias: ex, run)");
            sb.AppendLine("- SCRIPT LIST - Lists all scripts and their respective permissions & last read & edited time.");
            sb.AppendLine("- SCRIPT LISTR - Lists all scripts that are currently running.");
            sb.AppendLine("- SCRIPT RSR <SCRIPTNAME> - Reads a script and returns a list of all of its actions.");
            sb.AppendLine("- SCRIPT STS <SCRIPTNAME> - Stops script(s) that are currently running.");
            sb.AppendLine("- SCRIPT EXA - Runs all auto-run scripts again.");
            sb.AppendLine("- SCRIPT STOPALL - Stops all currently executing scripts.");

            response = StringBuilderPool.Pool.ToStringReturn(sb);
            return false;
        }
    }
}
