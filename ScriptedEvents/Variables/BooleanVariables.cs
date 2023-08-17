namespace ScriptedEvents.Variables.Booleans
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class BooleanVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Booleans";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new CassieSpeaking(),

            new IsScriptRunning(),
        };
    }

    public class CassieSpeaking : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{CASSIESPEAKING}";

        /// <inheritdoc/>
        public string ReversedName => "{!CASSIESPEAKING}";

        /// <inheritdoc/>
        public string Description => "Whether or not CASSIE is currently speaking.";

        /// <inheritdoc/>
        public bool Value => Cassie.IsSpeaking;
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
        public string[] Arguments { get; set; }

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
                if (Arguments.Length < 1) return false;

                string scriptName = Arguments[0];
                return ScriptHelper.RunningScripts.Any(scr => scr.Key.ScriptName == scriptName && scr.Value.IsRunning);
            }
        }
    }
}
