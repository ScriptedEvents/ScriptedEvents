namespace ScriptedEvents.Variables.Booleans
{
    using System;
#pragma warning disable SA1402 // File may only contain a single type
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class ScriptInfoVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Script Info";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new VariableExists(),
            new IsScriptRunning(),
            new This(),
        };
    }

    public class This : IStringVariable, INeedSourceVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{THIS}";

        /// <inheritdoc/>
        public string Description => "Returns information about the script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", false,
                new("AUTORUN", "'TRUE' or 'FALSE' depending on if the script is set to auto-run when the round restarts."),
                new("CALLER", "The name of the script that called this script."),
                new("CONTEXT", $"The context in which the script was executed. Valid options are: {string.Join(", ", ((ExecuteContext[])Enum.GetValues(typeof(ExecuteContext))).Where(r => r is not ExecuteContext.None))}"),
                new("DEBUG", $"'TRUE' or 'FALSE' depending on if the script is in debug mode."),
                new("DURATION", $"The amount of time (in seconds) the script has been running."),
                new("NAME", "The name of the script."),
                new("PATH", "The path to the script on the local directory.")
                ),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length == 0) return Source.ScriptName;

                string mode = Arguments[0].ToUpper();
                return mode switch
                {
                    "AUTORUN" => Source.HasFlag("AUTORUN").ToUpper(),
                    "CALLER" => Source.CallerScript is not null ? Source.CallerScript.ScriptName : "NONE",
                    "CONTEXT" => Source.Context.ToString(),
                    "DEBUG" => Source.Debug.ToUpper(),
                    "DURATION" => Source.RunDuration.TotalSeconds.ToString(),
                    "NAME" => Source.ScriptName,
                    "PATH" => Source.FilePath ?? "N/A",
                    _ => throw new ArgumentException("Invalid mode.", mode)
                };
            }
        }

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }
    }

    public class VariableExists : IBoolVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{VEXISTS}";

        /// <inheritdoc/>
        public string ReversedName => "{!VEXISTS}";

        /// <inheritdoc/>
        public string Description => "Whether or not the variable with the given name exists in the current context.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("variableName", typeof(string), "The name of the variable.", true),
        };

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public bool Value
        {
            get
            {
                if (Arguments.Length < 1) throw new ArgumentException("No variable provided");

                if (VariableSystemV2.TryGetPlayers(RawArguments[0], Source, out _, requireBrackets: false))
                    return true;

                if (VariableSystemV2.TryGetVariable(RawArguments[0], Source, out _, false, true))
                    return true;

                return false;
            }
        }
    }

    public class IsScriptRunning : IBoolVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{ISRUNNING}";

        /// <inheritdoc/>
        public string ReversedName => "{!ISRUNNING}";

        /// <inheritdoc/>
        public string Description => "Whether or not a specific script is running.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("scriptName", typeof(string), "The name of the script.", true),
        };

        /// <inheritdoc/>
        public bool Value
        {
            get
            {
                string scriptName = (string)Arguments[0];
                return MainPlugin.ScriptModule.RunningScripts.Any(scr => scr.Key.ScriptName == scriptName && scr.Value.IsRunning);
            }
        }
    }
}
