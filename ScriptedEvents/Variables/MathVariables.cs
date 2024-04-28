namespace ScriptedEvents.Variables.Misc
{
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
        };
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
