namespace ScriptedEvents.Variables.Misc
{
    using System;
#pragma warning disable SA1402 // File may only contain a single type

    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;
    using UnityEngine;

    public class MathVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Math";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new AbsoluteNumber(),
            new Round(),
        };
    }

    public class Round : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{MATH-ROUND}";

        /// <inheritdoc/>
        public string Description => "Returns a rounded number.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variable", typeof(float), "The variable to round. Requires the variable to be a number.", true),
            new OptionsArgument("mode", false, new("UP"), new("DOWN"), new("NEAREST")),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                float value = (float)Arguments[0];
                string mode = Arguments.Length < 2 ? "NEAREST" : (string)Arguments[1];

                return mode.ToUpper() switch
                {
                    "UP" => Mathf.Ceil(value),
                    "DOWN" => Mathf.Floor(value),
                    "NEAREST" => Mathf.Round(value),
                    _ => throw new ArgumentException(),
                };
            }
        }
    }

    public class AbsoluteNumber : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{MATH-ABS}";

        /// <inheritdoc/>
        public string Description => "Returns a non-negative value of a variable without regard to its sign.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("value", typeof(float), "The value.", true),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                return Mathf.Abs((float)Arguments[0]);
            }
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
