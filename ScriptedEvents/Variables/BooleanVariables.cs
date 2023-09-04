namespace ScriptedEvents.Variables.Booleans
{
    using System;
#pragma warning disable SA1402 // File may only contain a single type
    using System.Linq;

    using Exiled.API.Features;

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
            new VariableExists(),
            new CassieSpeaking(),
            new IsScriptRunning(),
        };
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
        public string[] Arguments { get; set; }

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
                if (Arguments.Length < 1) return false;

                return VariableSystem.GetVariable(Arguments[0], Source, false)?.Item1 != null;
            }
        }
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
                if (Arguments.Length < 1)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "scriptName" }));
                }

                string scriptName = Arguments[0];
                return ScriptHelper.RunningScripts.Any(scr => scr.Key.ScriptName == scriptName && scr.Value.IsRunning);
            }
        }
    }
}
