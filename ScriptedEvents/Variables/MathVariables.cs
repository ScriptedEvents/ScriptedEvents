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
            new MathRound(),
            new MathPower(),
            new MathRoot(),
        };
    }

    public class MathPower : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{MATH-POWER}";

        /// <inheritdoc/>
        public string Description => "Returns an exponentiated number.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("value", typeof(float), "Number to exponentiate.", true),
            new Argument("exponent", typeof(float), "The exponentiating number.", true),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                return (float)Math.Pow((float)Arguments[0], (float)Arguments[1]);
            }
        }
    }

    public class MathRoot : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{MATH-ROOT}";

        /// <inheritdoc/>
        public string Description => "Returns a root of a number.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("value", typeof(float), "The number to root.", true),
            new Argument("rootLevel", typeof(float), "The root level. Default: 2", false),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                float level = Arguments.Length > 1 ? (float)Arguments[1] : 2;
                return (float)Math.Pow((float)Arguments[0], 1 / level);
            }
        }
    }

    public class MathRound : IFloatVariable, IArgumentVariable
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
