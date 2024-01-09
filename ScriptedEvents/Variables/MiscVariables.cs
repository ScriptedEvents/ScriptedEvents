namespace ScriptedEvents.Variables.Misc
{
    using System;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class MiscVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Miscellaneous";


        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Round(),
        };

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

                    if (!VariableSystem.TryGetVariable(Arguments[0], out IConditionVariable variable, out _, Source, false))
                    {
                        throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "invalid variable" }));
                    }

                    if (variable is not IStringVariable value)
                    {
                        throw new ArgumentException("yes");
                    }

                    string mode = Arguments.Length < 2 ? "UP" : Arguments[1];
                    float roundedValue = 0;

                    if (mode == "UP")
                    {
                        roundedValue = (float)Math.Ceiling(float.Parse(value.Value));
                    }
                    else if (mode == "DOWN")
                    {
                        roundedValue = (float)Math.Floor(float.Parse(value.Value));
                    }
                    else
                    {
                        throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "mode is neither \"UP\" nor \"DOWN\"." }));
                    }

                    return roundedValue;
                }
            }
        }
    }
}
