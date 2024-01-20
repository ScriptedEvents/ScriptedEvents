namespace ScriptedEvents.Variables.Misc
{
#pragma warning disable SA1402 // File may only contain a single type
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
                new Argument("variable", typeof(IFloatVariable), "The name of the variable to round. Requires the variable to be a number.", true),
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

                if (!VariableSystem.TryParse(Arguments[0], out float value, Source, false))
                {
                    throw new ArgumentException(ErrorGen.Get(137, Arguments[0]));
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
#pragma warning restore SA1402 // File may only contain a single type.
}
