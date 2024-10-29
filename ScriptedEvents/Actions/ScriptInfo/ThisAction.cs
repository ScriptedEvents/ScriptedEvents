using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class ThisAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "THIS";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.ScriptInfo;

        /// <inheritdoc/>
        public string Description => "Returns information about the script where the action is used.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new OptionsArgument("mode", false,
                new("AUTORUN", "Returns 'TRUE' or 'FALSE' depending on if the script is set to auto-run when the round restarts."),
                new("CALLER", "Returns the name of the script that called this script. 'NONE' if script was called by other means."),
                new("CONTEXT", $"Returns the context in which the script was executed. Valid options are: {string.Join(", ", ((ExecuteContext[])Enum.GetValues(typeof(ExecuteContext))).Where(r => r is not ExecuteContext.None))}"),
                new("DEBUG", $"Returns 'TRUE' or 'FALSE' depending on if the script is in debug mode."),
                new("DURATION", $"Returns the amount of time (in seconds) the script has been running."),
                new("NAME", "Returns the name of the script."),
                new("PATH", "Returns the path to the script on the local directory."),
                new("VARIABLES", "Returns variable names available to this script only.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ret = Arguments[0].ToUpper() switch
            {
                "AUTORUN" => script.HasFlag("AUTORUN").ToUpper(),
                "CALLER" => script.CallerScript is not null ? script.CallerScript.ScriptName : "NONE",
                "CONTEXT" => script.Context.ToString(),
                "DEBUG" => script.IsDebug.ToUpper(),
                "DURATION" => script.RunDuration.TotalSeconds.ToString(),
                "NAME" => script.ScriptName,
                "PATH" => script.FilePath ?? "N/A",
                "VARIABLES" => script.UniqueLiteralVariables.Keys.Concat(script.UniquePlayerVariables.Keys).ToArray().Length != 0 ? string.Join(", ", script.UniqueLiteralVariables.Keys.Concat(script.UniquePlayerVariables.Keys)) : "NONE",
                _ => throw new ArgumentException()
            };
            return new(true, variablesToRet: new[] { ret });
        }
    }
}