using System;
using System.Linq;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.AllInOne
{
    public class ScriptInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "ScriptInfo";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.ScriptInfo;

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting information related to the script in which the action is ran.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new OptionsArgument("mode", false,
                new OptionValueDepending(
                    "AutoRun", 
                    "If the script is set to auto-run when the round restarts.",
                    typeof(bool)),
                new OptionValueDepending(
                    "CallingScript", 
                    "The name of the script that called this script. 'NONE' if script was called by other means.",
                    typeof(string)),
                new OptionValueDepending(
                    "Context", 
                    "Tthe context in which the script was called.",
                    typeof(ExecuteContext)),
                new OptionValueDepending(
                    "Debug", 
                    "If the script is in debug mode.",
                    typeof(bool)),
                new OptionValueDepending(
                    "Duration", 
                    "Returns the amount of time (in seconds) the script has been running.",
                    typeof(ulong)
                    ),
                new OptionValueDepending(
                    "Name", 
                    "The name of the script.",
                    typeof(string)),
                new OptionValueDepending(
                    "Path", 
                    "Returns the path to the script on the local directory.",
                    typeof(string)),
                new OptionValueDepending(
                    "Variables", 
                    "Variable names available in this script only.",
                    typeof(string))),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ret = Arguments[0]!.ToUpper() switch
            {
                "AUTORUN" => script.HasFlag("AUTORUN").ToUpper(),
                "CALLINGSCRIPT" => script.CallerScript is not null ? script.CallerScript.ScriptName : "NONE",
                "CONTEXT" => script.Context.ToString(),
                "DEBUG" => script.IsDebug.ToUpper(),
                "DURATION" => script.RunDuration.TotalSeconds.ToString(),
                "NAME" => script.ScriptName,
                "PATH" => script.FilePath,
                "VARIABLES" => script.UniqueLiteralVariables.Keys.Concat(script.UniquePlayerVariables.Keys).ToArray().Length != 0 ? string.Join(", ", script.UniqueLiteralVariables.Keys.Concat(script.UniquePlayerVariables.Keys)) : "NONE",
                _ => throw new ArgumentException()
            };
            return new(true, new(ret));
        }
    }
}