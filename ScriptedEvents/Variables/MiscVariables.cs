namespace ScriptedEvents.Variables.Misc
{
    using System;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;
    using UnityEngine;

    public class MiscVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Miscellaneous";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Round(),
        };
    }

    public class Round : IFloatVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUND}";

        /// <inheritdoc/>
        public string Description => "Returns a rounded version of a variable.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
                new Argument("variable", typeof(string), "The name of the variable to round. Requires the variable to be a number.", true),
                new Argument("mode", typeof(string), "Way of rounding the variable, either UP or DOWN.", true),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                if (Arguments.Length < 1)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "variable" }));
                }

                if (!VariableSystem.TryParse(Arguments[0], out float value))
                {
                    throw new ArgumentException($"The provided value '{Arguments[0]}' is not a valid number or variable containing a number. [Error Code: SE-134]");
                }

                string mode = Arguments.Length < 2 ? "UP" : Arguments[1];

                return mode.ToUpper() switch
                {
                    "UP" => Mathf.Ceil(value),
                    "DOWN" => Mathf.Floor(value),
                    _ => throw new ArgumentException($"'{mode}' is not a valid mode. Valid options: UP, DOWN"),
                };
            }
        }
    }
}
